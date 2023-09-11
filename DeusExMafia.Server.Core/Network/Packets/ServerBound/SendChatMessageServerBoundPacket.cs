using DeusExMafia.Server.Core.Network.Handlers;
using DeusExMafia.Server.Core.Network.IO;

namespace DeusExMafia.Server.Core.Network.Packets.ServerBound;

public record SendChatMessageServerBoundPacket(string Message) : IServerBoundPacket<ICommonNetworkHandler> {
    public void Apply(ICommonNetworkHandler listener) {
        listener.OnSendChatMessage(this);
    }

    public static IServerBoundPacket<ICommonNetworkHandler> Create(PacketBinaryReader reader) {
        return new SendChatMessageServerBoundPacket(reader.ReadString());
    }
}
