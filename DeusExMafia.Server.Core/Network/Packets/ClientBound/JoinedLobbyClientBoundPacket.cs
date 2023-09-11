using DeusExMafia.Server.Core.Network.IO;
using DeusExMafia.Server.Core.Players;

namespace DeusExMafia.Server.Core.Network.Packets.ClientBound;

public record JoinedLobbyClientBoundPacket(List<Player> Players) : IClientBoundPacket {
    public async Task Write(PacketBinaryWriter writer) {
        await writer.WriteCollectionAsync(Players, async (writer, player) => await writer.WritePlayerAsync(player));
    }
}
