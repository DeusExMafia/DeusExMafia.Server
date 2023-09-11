using DeusExMafia.Server.Core.Network.IO;

namespace DeusExMafia.Server.Core.Network.Packets.ClientBound;

public record ReceiveChatMessageClientBoundPacket(string Message) : IClientBoundPacket {
    public async Task Write(PacketBinaryWriter writer) {
        await writer.WriteStringAsync(Message);
    }
}
