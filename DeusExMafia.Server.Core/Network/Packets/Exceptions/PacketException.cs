using DeusExMafia.Server.Core.Network.Handlers;

namespace DeusExMafia.Server.Core.Network.Packets.Exceptions;

public abstract class PacketException<T> : Exception where T : INetworkHandler {
    public NetworkState<T> State { get; }

    protected PacketException(NetworkState<T> state, string message) : base(message) {
        State = state;
    }
}
