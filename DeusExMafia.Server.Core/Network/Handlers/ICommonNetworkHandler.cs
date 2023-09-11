using DeusExMafia.Server.Core.Network.Packets.ServerBound;

namespace DeusExMafia.Server.Core.Network.Handlers;

public interface ICommonNetworkHandler : INetworkHandler {
    Task OnSendChatMessage(SendChatMessageServerBoundPacket packet);
}
