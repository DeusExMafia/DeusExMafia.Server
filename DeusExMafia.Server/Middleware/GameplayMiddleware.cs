using DeusExMafia.Server.Core;
using DeusExMafia.Server.Core.Network;
using DeusExMafia.Server.Core.Network.Handlers;
using DeusExMafia.Server.Core.Players;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;
using System.Net.WebSockets;

namespace DeusExMafia.Server.Middleware;

class GameplayMiddleware : IMiddleware {
    private const string GAMEPLAY_PATH = "/gameplay";

    private readonly AccountServerSet AccountServerSet;
    private readonly DeusExMafiaServer Server = new DeusExMafiaServer();

    public GameplayMiddleware(IOptions<AccountServerSet> accountServerSetOptions) {
        AccountServerSet = accountServerSetOptions.Value;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next) {
        if (context.Request.Path == GAMEPLAY_PATH) {
            await Verify(context);
        } else {
            await next(context);
        }
    }

    private async Task Verify(HttpContext context) {
        using HttpClient client = new HttpClient() {
            BaseAddress = new Uri(AccountServerSet.Address)
        };
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        try {
            Account? account = await client.GetFromJsonAsync<Account>(context.Request.Query["token"]);
            if (account == null) {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }
            await Initiate(context, account);
        } catch (HttpRequestException e) {
            context.Response.StatusCode = (int)(e.StatusCode ?? HttpStatusCode.BadRequest);
        }
    }

    private async Task Initiate(HttpContext context, Account account) {
        if (!context.WebSockets.IsWebSocketRequest) {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return;
        }
        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
        Player player = new Player(account, player => new ClientConnection(webSocket, player, new LoginNetworkHandler(webSocket, player)));

        await Server.JoinLobby(player);
        await player.ClientConnection.Listen();
    }
}
