using DeusExMafia.Server.Core.Network.IO;
using DeusExMafia.Server.Core.Players;

namespace DeusExMafia.Server.Core.Network.Packets.ClientBound;

public record PlayerJoinedLobbyClientBoundPacket(Player Player) : IClientBoundPacket {
    public async Task Write(PacketBinaryWriter writer) {
        await writer.WritePlayerAsync(Player);
    }
}
