using DeusExMafia.Server.Core.Lobbies;
using DeusExMafia.Server.Core.Players;

namespace DeusExMafia.Server.Core;

public class DeusExMafiaServer {
    private readonly LobbyCollection Lobbies;

    public DeusExMafiaServer() {
        Lobbies = new LobbyCollection(this);
    }

    public async Task<Lobby> JoinLobby(Player player) {
        return await Lobbies.Join(player);
    }

    public void RemoveLobby(Lobby lobby) {
        Lobbies.Remove(lobby);
    }
}
