using DeusExMafia.Server.Core.Network.Handlers;
using DeusExMafia.Server.Core.Network.IO;

namespace DeusExMafia.Server.Core.Network.Packets;

public delegate IServerBoundPacket<T> PacketCreator<in T>(PacketBinaryReader reader) where T : INetworkHandler;
