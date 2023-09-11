using DeusExMafia.Server.Core.Network.Handlers;
using DeusExMafia.Server.Core.Network.Packets;
using DeusExMafia.Server.Core.Network.Packets.ClientBound;
using DeusExMafia.Server.Core.Network.Packets.Exceptions;
using DeusExMafia.Server.Core.Network.Packets.ServerBound;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace DeusExMafia.Server.Core.Network;

public static class NetworkState {
    public static readonly NetworkState<LoginNetworkHandler> LOGIN = new NetworkState<LoginNetworkHandler>(new NetworkState<LoginNetworkHandler>.PacketHolder()
        .RegisterClientBound<JoinedLobbyClientBoundPacket>()
        .RegisterClientBound<DisconnectClientBoundPacket>()
    );
    public static readonly NetworkState<LobbyNetworkHandler> LOBBY = new NetworkState<LobbyNetworkHandler>(new NetworkState<LobbyNetworkHandler>.PacketHolder()
        .RegisterServerBound<SendChatMessageServerBoundPacket>(SendChatMessageServerBoundPacket.Create)
        .RegisterClientBound<DisconnectClientBoundPacket>()
        .RegisterClientBound<PlayerJoinedLobbyClientBoundPacket>()
        .RegisterClientBound<PlayerLeftLobbyClientBoundPacket>()
        .RegisterClientBound<ReceiveChatMessageClientBoundPacket>()
    );
}

public class NetworkState<T> where T : INetworkHandler {
    private readonly PacketHolder Holder;

    public string Name { get; }

    internal NetworkState(PacketHolder holder, [CallerMemberName] string name = "") {
        Holder = holder;
        Name = name;
    }

    public PacketCreator<T> GetCreator(int id) {
        if (Holder.TryGetCreator(id, out PacketCreator<T>? packet)) {
            return packet;
        }
        throw new InvalidPacketIdException<T>(id, this);
    }

    public int GetId<U>(U packet) where U : IClientBoundPacket {
        if (Holder.TryGetId(packet, out int id)) {
            return id;
        }
        throw new UnregisteredPacketException<T, U>(packet, this);
    }

    internal class PacketHolder {
        private readonly Dictionary<int, PacketCreator<T>> IdToCreator = new Dictionary<int, PacketCreator<T>>();
        private readonly Dictionary<Type, int> TypeToId = new Dictionary<Type, int>();

        public PacketHolder RegisterServerBound<U>(PacketCreator<T> creator) where U : IServerBoundPacket<T> {
            if (IdToCreator.ContainsValue(creator)) {
                throw new ArgumentException($"Registering server bound packet {typeof(U).Name} twice");
            }
            IdToCreator[IdToCreator.Count] = creator;
            return this;
        }

        public PacketHolder RegisterClientBound<U>() where U : IClientBoundPacket {
            if (TypeToId.ContainsKey(typeof(U))) {
                throw new ArgumentException($"Registering client bound packet {typeof(U).Name} twice");
            }
            TypeToId[typeof(U)] = TypeToId.Count;
            return this;
        }

        public bool TryGetCreator(int id, [NotNullWhen(true)] out PacketCreator<T>? creator) {
            return IdToCreator.TryGetValue(id, out creator);
        }

        public bool TryGetId<U>(U packet, out int id) where U : IClientBoundPacket {
            return TypeToId.TryGetValue(packet.GetType(), out id);
        }
    }
}
