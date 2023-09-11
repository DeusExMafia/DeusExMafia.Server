using DeusExMafia.Server.Core.Players;

namespace DeusExMafia.Server.Core.Lobbies;

public class LobbyCollection {
    private readonly DeusExMafiaServer Server;
    private readonly List<Lobby> Lobbies = new List<Lobby>();

    public LobbyCollection(DeusExMafiaServer server) {
        Server = server;
    }

    public async Task<Lobby> Join(Player player) {
        foreach (Lobby lobby in Lobbies) {
            if (await lobby.TryJoin(player)) {
                return lobby;
            }
        }
        return await CreateLobby(player);
    }

    public void Remove(Lobby lobby) {
        if (!lobby.IsEmpty) {
            return;
        }
        Lobbies.Remove(lobby);
    }

    private async Task<Lobby> CreateLobby(Player player) {
        Lobby lobby = new Lobby(Server);
        await lobby.TryJoin(player);
        Lobbies.Add(lobby);
        return lobby;
    }
}
