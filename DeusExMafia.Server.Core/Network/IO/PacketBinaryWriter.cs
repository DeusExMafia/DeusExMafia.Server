using DeusExMafia.Server.Core.Players;

namespace DeusExMafia.Server.Core.Network.IO;

public class PacketBinaryWriter : BinaryWriter {
    public PacketBinaryWriter(Stream baseStream) : base(baseStream) { }

    public async Task WritePlayerAsync(Player player) {
        await WriteStringAsync(player.Name);
    }

    public async Task WriteCollectionAsync<T>(ICollection<T> values, Func<PacketBinaryWriter, T, Task> valueWriter) {
        await WriteIntegerAsync(values.Count);
        foreach (T value in values) {
            await valueWriter(this, value);
        }
    }
}
