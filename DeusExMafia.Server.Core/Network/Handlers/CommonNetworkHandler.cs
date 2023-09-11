using DeusExMafia.Server.Core.Network.Packets.ServerBound;
using DeusExMafia.Server.Core.Players;
using System.Net.WebSockets;

namespace DeusExMafia.Server.Core.Network.Handlers;

public abstract class CommonNetworkHandler<TSelf> : NetworkHandler<TSelf>, ICommonNetworkHandler where TSelf : CommonNetworkHandler<TSelf>, ICommonNetworkHandler {
    protected CommonNetworkHandler(WebSocket webSocket, NetworkState<TSelf> state, Player player) : base(webSocket, state, player) { }

    public abstract Task OnSendChatMessage(SendChatMessageServerBoundPacket packet);
}
