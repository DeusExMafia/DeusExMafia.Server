using DeusExMafia.Server.Core.Network.Handlers;
using DeusExMafia.Server.Core.Network.Packets;
using DeusExMafia.Server.Core.Network.Packets.ClientBound;
using DeusExMafia.Server.Core.Players;

namespace DeusExMafia.Server.Core.Lobbies;

public class Lobby {
    private const int MAX_PLAYER_COUNT = 15;

    private readonly DeusExMafiaServer Server;
    private readonly List<Player> Players = new List<Player>(MAX_PLAYER_COUNT);

    public Lobby(DeusExMafiaServer server) {
        Server = server;
    }

    public bool IsEmpty { get { return Players.Count == 0; } }

    public async Task<bool> TryJoin(Player player) {
        if (Players.Count >= MAX_PLAYER_COUNT) {
            return false;
        }
        Players.Add(player);
        await player.SendPacket(new JoinedLobbyClientBoundPacket(Players));

        player.ClientConnection.Upgrade(new LobbyNetworkHandler(player.ClientConnection.WebSocket, player, this));
        player.ClientConnection.Disconnected += OnPlayerDisconnect;
        await SendGlobalPacket(player, new PlayerJoinedLobbyClientBoundPacket(player));
        return true;
    }

    public async Task SendMessage(Player source, string message) {
        if (string.IsNullOrWhiteSpace(message)) {
            return;
        }
        ReceiveChatMessageClientBoundPacket packet = new ReceiveChatMessageClientBoundPacket($"{source.Name}: {message}");
        await SendGlobalPacket(null, packet);
    }

    private async Task SendGlobalPacket(Player? except, IClientBoundPacket packet) {
        foreach (Player player in Players) {
            if (player == except) {
                continue;
            }
            await player.SendPacket(packet);
        }
    }

    private async Task OnPlayerDisconnect(Player player) {
        if (!Players.Remove(player)) {
            return;
        }
        if (Players.Count == 0) {
            Server.RemoveLobby(this);
            return;
        }
        await SendGlobalPacket(null, new PlayerLeftLobbyClientBoundPacket(player.Name));
    }
}
