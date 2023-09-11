using DeusExMafia.Server.Core.Network.Handlers;
using DeusExMafia.Server.Core.Network.Packets;
using DeusExMafia.Server.Core.Players;
using System.Net.WebSockets;

namespace DeusExMafia.Server.Core.Network;

public class ClientConnection {
    private const int BUFFER_SIZE = 1024 * 4;

    public event AsyncPlayerAction? Disconnected;

    private readonly byte[] Buffer = new byte[BUFFER_SIZE];
    private readonly Player Player;
    private INetworkHandler NetworkHandler;

    public WebSocket WebSocket { get; }

    public ClientConnection(WebSocket webSocket, Player player, LoginNetworkHandler networkHandler) {
        WebSocket = webSocket;
        Player = player;
        NetworkHandler = networkHandler;
    }

    public async Task Listen() {
        while (WebSocket.State == WebSocketState.Open) {
            WebSocketReceiveResult result = await WebSocket.ReceiveAsync(Buffer, CancellationToken.None);
            await Handle(result);
        }
        Disconnected?.Invoke(Player);
    }

    public async Task SendPacket<U>(U packet) where U : IClientBoundPacket {
        await NetworkHandler.SendPacket(packet);
    }

    public async Task Disconnect(WebSocketCloseStatus status, string reason) {
        await NetworkHandler.Disconnect(status, reason);
    }

    public void Upgrade(INetworkHandler networkHandler) {
        NetworkHandler = networkHandler ?? throw new ArgumentNullException(nameof(networkHandler));
    }

    private async Task Handle(WebSocketReceiveResult result) {
        if (!result.EndOfMessage) {
            await Disconnect(WebSocketCloseStatus.MessageTooBig, $"Packet must be no larger than {BUFFER_SIZE} bytes");
            return;
        }
        if (result.MessageType == WebSocketMessageType.Binary) {
            await HandlePacket(result.Count);
            return;
        }
        if (result.MessageType == WebSocketMessageType.Close) {
            await WebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, WebSocket.CloseStatusDescription, CancellationToken.None);
            return;
        }
        await Disconnect(WebSocketCloseStatus.InvalidMessageType, $"Got an invalid message type: {result.MessageType}");
    }

    private async Task HandlePacket(int receivedSize) {
        using MemoryStream memoryStream = new MemoryStream(Buffer);
        await NetworkHandler.HandlePacket(receivedSize, memoryStream);
    }

    public delegate ClientConnection Creator(Player player);
}
