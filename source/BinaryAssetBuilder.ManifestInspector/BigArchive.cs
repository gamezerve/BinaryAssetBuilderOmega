using System.Buffers.Binary;
using System.Text;

namespace BinaryAssetBuilder.ManifestInspector;

internal sealed record BigEntry(string Name, uint Offset, uint StoredSize);

internal sealed class BigArchive : IDisposable
{
    private readonly FileStream _stream;

    private BigArchive(FileStream stream, IReadOnlyList<BigEntry> entries)
    {
        _stream = stream;
        Entries = entries;
    }

    public IReadOnlyList<BigEntry> Entries { get; }

    public static BigArchive Open(string path)
    {
        var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        try
        {
            using var reader = new BinaryReader(stream, Encoding.ASCII, leaveOpen: true);
            var magic = Encoding.ASCII.GetString(ReadExactly(reader, 4));
            if (magic is not ("BIG4" or "BIGF"))
            {
                throw new InvalidDataException($"Unsupported BIG magic '{magic}'.");
            }

            _ = ReadUInt32BigEndian(reader); // Archive size.
            var entryCount = ReadUInt32BigEndian(reader);
            var headerSize = ReadUInt32BigEndian(reader);
            if (entryCount > 1_000_000 || headerSize > stream.Length)
            {
                throw new InvalidDataException("Invalid BIG directory header.");
            }

            var entries = new List<BigEntry>(checked((int)entryCount));
            for (var index = 0u; index < entryCount; index++)
            {
                var offset = ReadUInt32BigEndian(reader);
                var storedSize = ReadUInt32BigEndian(reader);
                var name = ReadNullTerminatedAscii(reader, headerSize);
                if ((ulong)offset + storedSize > (ulong)stream.Length)
                {
                    throw new InvalidDataException($"BIG entry '{name}' extends past the archive boundary.");
                }

                entries.Add(new BigEntry(name, offset, storedSize));
            }

            return new BigArchive(stream, entries);
        }
        catch
        {
            stream.Dispose();
            throw;
        }
    }

    public byte[] ReadEntry(BigEntry entry, int maximumStoredBytes = 256 * 1024 * 1024)
    {
        if (entry.StoredSize > maximumStoredBytes)
        {
            throw new InvalidDataException(
                $"Entry '{entry.Name}' is {entry.StoredSize:N0} bytes; limit is {maximumStoredBytes:N0} bytes.");
        }

        _stream.Position = entry.Offset;
        var bytes = new byte[entry.StoredSize];
        _stream.ReadExactly(bytes);
        return bytes;
    }

    public byte[] ReadEntryRange(BigEntry entry, long relativeOffset, int count)
    {
        if (relativeOffset < 0 || count < 0 || relativeOffset > entry.StoredSize - count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(relativeOffset),
                $"Range offset={relativeOffset:N0}, count={count:N0} is outside '{entry.Name}' ({entry.StoredSize:N0} bytes)." );
        }

        _stream.Position = checked(entry.Offset + relativeOffset);
        byte[] bytes = new byte[count];
        _stream.ReadExactly(bytes);
        return bytes;
    }

    public void Dispose() => _stream.Dispose();

    private static uint ReadUInt32BigEndian(BinaryReader reader) =>
        BinaryPrimitives.ReadUInt32BigEndian(ReadExactly(reader, sizeof(uint)));

    private static byte[] ReadExactly(BinaryReader reader, int length)
    {
        var bytes = reader.ReadBytes(length);
        if (bytes.Length != length)
        {
            throw new EndOfStreamException();
        }

        return bytes;
    }

    private static string ReadNullTerminatedAscii(BinaryReader reader, uint headerSize)
    {
        var bytes = new List<byte>();
        while (reader.BaseStream.Position < headerSize)
        {
            var value = reader.ReadByte();
            if (value == 0)
            {
                return Encoding.ASCII.GetString(bytes.ToArray());
            }

            bytes.Add(value);
        }

        throw new InvalidDataException("Unterminated BIG entry name.");
    }
}
