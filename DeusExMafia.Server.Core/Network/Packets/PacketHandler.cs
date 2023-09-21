using DeusExMafia.Server.Core.Network.Handlers;
using DeusExMafia.Server.Core.Network.IO;

namespace DeusExMafia.Server.Core.Network.Packets;

internal class PacketHandler<T> where T : NetworkHandler<T> {
    private readonly T NetworkHandler;
    private readonly NetworkState<T> State;

    public PacketHandler(T networkHandler, NetworkState<T> state) {
        NetworkHandler = networkHandler;
        State = state;
    }

    public async Task<byte[]> GetPacketBuffer<U>(U packet) where U : IClientBoundPacket {
        int id = State.GetId(packet);
        using MemoryStream memoryStream = new MemoryStream();
        using PacketBinaryWriter writer = new PacketBinaryWriter(memoryStream);
        await writer.WriteIntegerAsync(id);
        await packet.Write(writer);
        return memoryStream.ToArray();
    }

    public async Task Handle(int receivedSize, Stream stream) {
        using PacketBinaryReader reader = new PacketBinaryReader(stream);
        IServerBoundPacket<T> packet = await ReadPacket(reader);
        //if (stream.Position != receivedSize) {
        //    await NetworkHandler.Disconnect(WebSocketCloseStatus.InvalidPayloadData, $"Invalid packet payload: Expected {stream.Position} bytes, got {receivedSize} bytes instead");
        //    return;
        //}
        packet.Apply(NetworkHandler);
    }

    private async Task<IServerBoundPacket<T>> ReadPacket(PacketBinaryReader reader) {
        int id = await reader.ReadIntegerAsync();
        return State.GetCreator(id)(reader);
    }
}
