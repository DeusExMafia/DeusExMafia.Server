using DeusExMafia.Server.Core.Lobbies;
using DeusExMafia.Server.Core.Network.Packets.ServerBound;
using DeusExMafia.Server.Core.Players;
using System.Net.WebSockets;

namespace DeusExMafia.Server.Core.Network.Handlers;

public class LobbyNetworkHandler : CommonNetworkHandler<LobbyNetworkHandler> {
    private readonly Lobby Lobby;

    public LobbyNetworkHandler(WebSocket webSocket, Player player, Lobby lobby) : base(webSocket, NetworkState.LOBBY, player) {
        Lobby = lobby;
    }

    public override async Task OnSendChatMessage(SendChatMessageServerBoundPacket packet) {
        await Lobby.SendMessage(Player, packet.Message);
    }
}
