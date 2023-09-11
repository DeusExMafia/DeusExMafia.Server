using DeusExMafia.Server.Core.Network;
using DeusExMafia.Server.Core.Network.Packets;

namespace DeusExMafia.Server.Core.Players;

public class Player {
    public ClientConnection ClientConnection { get; set; }
    public string Name { get; } = "Player" + Random.Shared.Next(1000);

    public Player(ClientConnection.Creator clientConnectionCreator) {
        ClientConnection = clientConnectionCreator(this);
    }

    public async Task SendPacket<U>(U packet) where U : IClientBoundPacket {
        if (ClientConnection == null) {
            return;
        }
        await ClientConnection.SendPacket(packet);
    }
}
