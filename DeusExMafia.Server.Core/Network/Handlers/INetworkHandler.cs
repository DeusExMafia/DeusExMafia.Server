using DeusExMafia.Server.Core.Network.Packets;
using System.Net.WebSockets;

namespace DeusExMafia.Server.Core.Network.Handlers;

public interface INetworkHandler {
    Task HandlePacket(int receivedSize, Stream stream);
    Task SendPacket<U>(U packet) where U : IClientBoundPacket;
    Task Disconnect(WebSocketCloseStatus status, string reason);
}
