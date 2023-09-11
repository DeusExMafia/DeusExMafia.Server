using DeusExMafia.Server.Core.Network.IO;

namespace DeusExMafia.Server.Core.Network.Packets.ClientBound;

public record DisconnectClientBoundPacket(string Reason) : IClientBoundPacket {
    public async Task Write(PacketBinaryWriter writer) {
        await writer.WriteStringAsync(Reason);
    }
}
