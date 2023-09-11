using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;

namespace DeusExMafia.Server.Core.Network.IO;

public class BinaryWriter : IDisposable, IAsyncDisposable {
    private const int BUFFER_LENGTH = sizeof(long);

    private readonly Stream BaseStream;
    private readonly byte[] Buffer = new byte[BUFFER_LENGTH];

    public BinaryWriter(Stream baseStream) {
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

    public void WriteBoolean(bool value) {
        Write(value);
    }

    public async Task WriteBooleanAsync(bool value) {
        await WriteAsync(value);
    }

    public void WriteByte(sbyte value) {
        Write(value);
    }

    public async Task WriteByteAsync(sbyte value) {
        await WriteAsync(value);
    }

    public void WriteUnsignedByte(byte value) {
        Write(value);
    }

    public async Task WriteUnsignedByteAsync(byte value) {
        await WriteAsync(value);
    }

    public void WriteShort(short value) {
        Write(value);
    }

    public async Task WriteShortAsync(short value) {
        await WriteAsync(value);
    }

    public void WriteUnsignedShort(ushort value) {
        Write(value);
    }

    public async Task WriteUnsignedShortAsync(ushort value) {
        await WriteAsync(value);
    }

    public void WriteInteger(int value) {
        Write(value);
    }

    public async Task WriteIntegerAsync(int value) {
        await WriteAsync(value);
    }

    public void WriteUnsignedInteger(uint value) {
        Write(value);
    }

    public async Task WriteUnsignedIntegerAsync(uint value) {
        await WriteAsync(value);
    }

    public void WriteLong(long value) {
        Write(value);
    }

    public async Task WriteLongAsync(long value) {
        await WriteAsync(value);
    }

    public void WriteUnsignedLong(ulong value) {
        Write(value);
    }

    public async Task WriteUnsignedLongAsync(ulong value) {
        await WriteAsync(value);
    }

    public void WriteFloat(float value) {
        Write(value);
    }

    public async Task WriteFloatAsync(float value) {
        await WriteAsync(value);
    }

    public void WriteDouble(double value) {
        Write(value);
    }

    public async Task WriteDoubleAsync(double value) {
        await WriteAsync(value);
    }

    public void WriteString(string value) {
        ArgumentNullException.ThrowIfNull(value);
        int byteCount = GetByteCount(value);
        WriteUnsignedShort((ushort)byteCount);
        byte[] buffer = GetBuffer(byteCount, value);
        BaseStream.Write(buffer, 0, byteCount);
    }

    public async Task WriteStringAsync(string value) {
        ArgumentNullException.ThrowIfNull(value);
        int byteCount = GetByteCount(value);
        await WriteUnsignedShortAsync((ushort)byteCount);
        byte[] buffer = GetBuffer(byteCount, value);
        await BaseStream.WriteAsync(buffer.AsMemory(0, byteCount));
    }

    private void Write<T>(T value) where T : unmanaged {
        byte[] buffer = GetBuffer<T>();
        FillBuffer(buffer, value);
        BaseStream.Write(buffer, 0, Unsafe.SizeOf<T>());
    }

    private async Task WriteAsync<T>(T value) where T : unmanaged {
        byte[] buffer = GetBuffer<T>();
        FillBuffer(buffer, value);
        await BaseStream.WriteAsync(buffer.AsMemory(0, Unsafe.SizeOf<T>()));
    }

    private byte[] GetBuffer<T>() where T : unmanaged {
        return GetBuffer(Unsafe.SizeOf<T>());
    }

    private byte[] GetBuffer(int size) {
        if (size > BUFFER_LENGTH) {
            return ArrayPool<byte>.Shared.Rent(size);
        }
        return Buffer;
    }

    private byte[] GetBuffer(int size, string value) {
        byte[] buffer = GetBuffer(size);
        Encoding.UTF8.GetBytes(value, buffer);
        return buffer;
    }

    private void FillBuffer<T>(byte[] buffer, T value) where T : unmanaged {
        Unsafe.WriteUnaligned(ref buffer[0], value);

        int size = Unsafe.SizeOf<T>();
        if (BitConverter.IsLittleEndian) {
            buffer.AsSpan(0, size).Reverse();
        }
        if (size > BUFFER_LENGTH) {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    private static int GetByteCount(string value) {
        int byteCount = Encoding.UTF8.GetByteCount(value);
        if (byteCount > ushort.MaxValue) {
            throw new InvalidOperationException($"String size was too large ({byteCount} bytes, should be at most {ushort.MaxValue} bytes)");
        }
        return byteCount;
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
