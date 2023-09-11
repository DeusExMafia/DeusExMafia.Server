using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;

namespace DeusExMafia.Server.Core.Network.IO;

public class BinaryReader : IDisposable, IAsyncDisposable {
    private const int BUFFER_LENGTH = sizeof(long);

    private readonly Stream BaseStream;
    private readonly byte[] Buffer = new byte[BUFFER_LENGTH];

    public BinaryReader(Stream baseStream) {
        BaseStream = baseStream;
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync() {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }

    public bool ReadBoolean() {
        return Read<bool>();
    }

    public async Task<bool> ReadBooleanAsync(CancellationToken cancellationToken = default) {
        return await ReadAsync<bool>(cancellationToken);
    }

    public sbyte ReadByte() {
        return Read<sbyte>();
    }

    public async Task<sbyte> ReadByteAsync(CancellationToken cancellationToken = default) {
        return await ReadAsync<sbyte>(cancellationToken);
    }

    public byte ReadUnsignedByte() {
        return Read<byte>();
    }

    public async Task<byte> ReadUnsignedByteAsync(CancellationToken cancellationToken = default) {
        return await ReadAsync<byte>(cancellationToken);
    }

    public virtual short ReadShort() {
        return Read<short>();
    }

    public virtual async Task<short> ReadShortAsync(CancellationToken cancellationToken = default) {
        return await ReadAsync<short>(cancellationToken);
    }

    public virtual ushort ReadUnsignedShort() {
        return Read<ushort>();
    }

    public virtual async Task<ushort> ReadUnsignedShortAsync(CancellationToken cancellationToken = default) {
        return await ReadAsync<ushort>(cancellationToken);
    }

    public virtual int ReadInteger() {
        return Read<int>();
    }

    public virtual async Task<int> ReadIntegerAsync(CancellationToken cancellationToken = default) {
        return await ReadAsync<int>(cancellationToken);
    }

    public virtual uint ReadUnsignedInteger() {
        return Read<uint>();
    }

    public virtual async Task<uint> ReadUnsignedIntegerAsync(CancellationToken cancellationToken = default) {
        return await ReadAsync<uint>(cancellationToken);
    }

    public virtual long ReadLong() {
        return Read<long>();
    }

    public virtual async Task<long> ReadLongAsync(CancellationToken cancellationToken = default) {
        return await ReadAsync<long>(cancellationToken);
    }

    public virtual ulong ReadUnsignedLong() {
        return Read<ulong>();
    }

    public virtual async Task<ulong> ReadUnsignedLongAsync(CancellationToken cancellationToken = default) {
        return await ReadAsync<ulong>(cancellationToken);
    }

    public float ReadFloat() {
        return Read<float>();
    }

    public async Task<float> ReadFloatAsync(CancellationToken cancellationToken = default) {
        return await ReadAsync<float>(cancellationToken);
    }

    public double ReadDouble() {
        return Read<double>();
    }

    public async Task<double> ReadDoubleAsync(CancellationToken cancellationToken = default) {
        return await ReadAsync<double>(cancellationToken);
    }

    public virtual string ReadString() {
        int length = ReadUnsignedShort();
        byte[] buffer = ReadBuffer(length);
        return Encoding.UTF8.GetString(buffer, 0, length);
    }

    public virtual async Task<string> ReadStringAsync(CancellationToken cancellationToken = default) {
        int length = await ReadUnsignedShortAsync(cancellationToken);
        byte[] buffer = await ReadBufferAsync(length, cancellationToken);
        return Encoding.UTF8.GetString(buffer);
    }

    private unsafe T Read<T>() where T : unmanaged {
        byte[] buffer = ReadBuffer<T>();
        return FinishReading<T>(buffer);
    }

    private async Task<T> ReadAsync<T>(CancellationToken cancellationToken = default) where T : unmanaged {
        byte[] buffer = await ReadBufferAsync<T>(cancellationToken);
        return FinishReading<T>(buffer);
    }

    private byte[] ReadBuffer<T>() where T : unmanaged {
        return ReadBuffer(Unsafe.SizeOf<T>());
    }

    private async Task<byte[]> ReadBufferAsync<T>(CancellationToken cancellationToken = default) where T : unmanaged {
        return await ReadBufferAsync(Unsafe.SizeOf<T>(), cancellationToken);
    }

    protected byte[] ReadBuffer(int size) {
        byte[] buffer = GetBuffer(size);
        BaseStream.ReadExactly(buffer, 0, size);
        return buffer;
    }

    protected async Task<byte[]> ReadBufferAsync(int size, CancellationToken cancellationToken = default) {
        byte[] buffer = GetBuffer(size);
        await BaseStream.ReadExactlyAsync(buffer, 0, size, cancellationToken);
        return buffer;
    }

    private byte[] GetBuffer(int size) {
        if (size > BUFFER_LENGTH) {
            return ArrayPool<byte>.Shared.Rent(size);
        }
        return Buffer;
    }

    private T FinishReading<T>(byte[] buffer) where T : unmanaged {
        int size = Unsafe.SizeOf<T>();
        if (BitConverter.IsLittleEndian) {
            buffer.AsSpan(0, size).Reverse();
        }

        T result = Unsafe.ReadUnaligned<T>(ref buffer[0]);
        if (size > BUFFER_LENGTH) {
            ArrayPool<byte>.Shared.Return(buffer);
        }
        return result;
    }

    private void Dispose(bool disposing) {
        if (disposing) {
            BaseStream.Dispose();
        }
    }

    private async Task DisposeAsync(bool disposing) {
        if (disposing) {
            await BaseStream.DisposeAsync();
        }
    }
}
