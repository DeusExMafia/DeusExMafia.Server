using DeusExMafia.Server.Core;
using DeusExMafia.Server.Core.Network;
using DeusExMafia.Server.Core.Network.Handlers;
using DeusExMafia.Server.Core.Players;
using System.Net;
using System.Net.WebSockets;

namespace DeusExMafia.Server.Middleware;

class GameplayMiddleware : IMiddleware {
    private const string GAMEPLAY_PATH = "/gameplay";

    private readonly DeusExMafiaServer Server = new DeusExMafiaServer();

    public async Task InvokeAsync(HttpContext context, RequestDelegate next) {
        if (context.Request.Path == GAMEPLAY_PATH) {
            await Initiate(context);
        } else {
            await next(context);
        }
    }

    private async Task Initiate(HttpContext context) {
        if (!context.WebSockets.IsWebSocketRequest) {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return;
        }
        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
        Player player = new Player(player => new ClientConnection(webSocket, player, new LoginNetworkHandler(webSocket, player)));

        await Server.JoinLobby(player);
        await player.ClientConnection.Listen();
    }
}
