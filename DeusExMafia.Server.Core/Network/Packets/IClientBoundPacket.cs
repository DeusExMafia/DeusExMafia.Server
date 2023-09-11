using DeusExMafia.Server.Core.Network.IO;

namespace DeusExMafia.Server.Core.Network.Packets;

public interface IClientBoundPacket {
    Task Write(PacketBinaryWriter writer);
}
