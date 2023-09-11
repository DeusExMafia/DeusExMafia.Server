using DeusExMafia.Server.Core.Network.Handlers;

namespace DeusExMafia.Server.Core.Network.Packets.Exceptions;

public class UnregisteredPacketException<T, U> : PacketException<T> where T : INetworkHandler where U : IClientBoundPacket {
    public UnregisteredPacketException(U packet, NetworkState<T> state) : base(state, $"Unregistered packet {packet.GetType().Name} in state {state.Name}") { }
}
