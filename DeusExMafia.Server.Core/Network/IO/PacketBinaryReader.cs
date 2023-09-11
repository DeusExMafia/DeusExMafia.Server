namespace DeusExMafia.Server.Core.Network.IO;

public class PacketBinaryReader : BinaryReader {
    public PacketBinaryReader(Stream baseStream) : base(baseStream) { }
}
