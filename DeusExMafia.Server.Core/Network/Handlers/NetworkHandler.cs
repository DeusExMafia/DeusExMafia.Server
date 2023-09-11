using DeusExMafia.Server.Core.Network.Packets;
using DeusExMafia.Server.Core.Network.Packets.ClientBound;
using DeusExMafia.Server.Core.Players;
using System.Net.WebSockets;

namespace DeusExMafia.Server.Core.Network.Handlers;

public abstract class NetworkHandler<TSelf> : INetworkHandler where TSelf : NetworkHandler<TSelf> {
    private readonly WebSocket WebSocket;
    private readonly PacketHandler<TSelf> PacketHandler;

    protected Player Player { get; }

    protected NetworkHandler(WebSocket webSocket, NetworkState<TSelf> state, Player player) {
        WebSocket = webSocket;
        PacketHandler = new PacketHandler<TSelf>((TSelf)this, state);
        Player = player;
    }

    public async Task HandlePacket(int receivedSize, Stream stream) {
        try {
            await PacketHandler.Handle(receivedSize, stream);
        } catch (Exception e) {
            await Disconnect(WebSocketCloseStatus.InvalidPayloadData, e.ToString());
        }
    }

    public async Task SendPacket<U>(U packet) where U : IClientBoundPacket {
        if (WebSocket.State != WebSocketState.Open) {
            return;
        }

        try {
            byte[] buffer = await PacketHandler.GetPacketBuffer(packet);
            await WebSocket.SendAsync(buffer, WebSocketMessageType.Binary, true, CancellationToken.None);
        } catch (Exception e) {
            await Disconnect(WebSocketCloseStatus.InternalServerError, e.ToString());
            throw;
        }
    }

    public async Task Disconnect(WebSocketCloseStatus status, string reason) {
        await SendPacket(new DisconnectClientBoundPacket(reason));
        await WebSocket.CloseAsync(status, null, CancellationToken.None);
    }
}
