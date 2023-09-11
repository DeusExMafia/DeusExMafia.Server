using DeusExMafia.Server.Core.Network.Handlers;

namespace DeusExMafia.Server.Core.Network.Packets.Exceptions;

public class InvalidPacketIdException<T> : PacketException<T> where T : INetworkHandler {
    public int Id { get; }

    public InvalidPacketIdException(int id, NetworkState<T> state) : base(state, $"Invalid packet id {id} in state {state.Name}") {
        Id = id;
    }
}
