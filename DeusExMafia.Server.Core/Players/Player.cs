using DeusExMafia.Server.Core.Network;
using DeusExMafia.Server.Core.Network.Packets;

namespace DeusExMafia.Server.Core.Players;

public class Player {
    private readonly Account Account;

    public ClientConnection ClientConnection { get; set; }
    public string Name {
        get {
            return Account.Username;
        }
    }

    public Player(Account account, ClientConnection.Creator clientConnectionCreator) {
        Account = account;
        ClientConnection = clientConnectionCreator(this);
    }

    public async Task SendPacket<U>(U packet) where U : IClientBoundPacket {
        if (ClientConnection == null) {
            return;
        }
        await ClientConnection.SendPacket(packet);
    }
}
