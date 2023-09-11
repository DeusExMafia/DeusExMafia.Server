using DeusExMafia.Server.Core.Players;
using System.Net.WebSockets;

namespace DeusExMafia.Server.Core.Network.Handlers;

public class LoginNetworkHandler : NetworkHandler<LoginNetworkHandler> {
    public LoginNetworkHandler(WebSocket webSocket, Player player) : base(webSocket, NetworkState.LOGIN, player) { }
}
