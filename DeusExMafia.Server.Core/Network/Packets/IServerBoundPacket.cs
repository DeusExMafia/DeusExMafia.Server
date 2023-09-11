using DeusExMafia.Server.Core.Network.Handlers;

namespace DeusExMafia.Server.Core.Network.Packets;

public interface IServerBoundPacket<in T> where T : INetworkHandler {
    void Apply(T listener);
}
