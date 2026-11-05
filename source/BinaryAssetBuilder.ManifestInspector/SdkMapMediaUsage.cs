using System.Buffers.Binary;
using System.Text;
using Microsoft.Win32;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: count typed sound and movie parameters in the binary maps the game actually reads.
internal static class SdkMapMediaUsage
{
    private const int MaximumMapBytes = 128 * 1024 * 1024;
    internal sealed record Result(HashSet<string> Audio, HashSet<string> Movies, bool Complete, int ModMaps, int RetailMaps);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: scan only loose official maps and .map members of installed Maps BIGs. */
    //-------------------------------------------------------------------------------------------------
    internal static Result Scan(string modData, HashSet<string> audioCandidates, HashSet<string> movieCandidates)
    {
        var audio = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var movies = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        string? retailData = FindRetailData();
        bool complete = retailData != null;
        int modMaps = 0, retailMaps = 0;
        string official = Path.Combine(modData, "Maps", "Official");
        if (Directory.Exists(official))
        {
            foreach (string map in Directory.EnumerateFiles(official, "*.map", SearchOption.AllDirectories))
                if (TryScan(map, () => SdkEnvironmentPreflight.Read(map, MaximumMapBytes))) modMaps++;
        }
        if (retailData != null)
        {
            foreach (string archivePath in Directory.EnumerateFiles(retailData, "Maps*.big"))
            {
                try
                {
                    using var archive = BigArchive.Open(archivePath);
                    foreach (var member in archive.Entries.Where(e => e.Name.EndsWith(".map", StringComparison.OrdinalIgnoreCase)))
                        if (TryScan(archivePath + ":" + member.Name, () => archive.ReadEntry(member, MaximumMapBytes))) retailMaps++;
                }
                catch (Exception error) when (error is IOException or InvalidDataException or UnauthorizedAccessException)
                {
                    complete = false;
                    Console.WriteLine("Warning: MAPMEDIAAUDIT " + archivePath + ": " + error.Message);
                }
            }
        }
        if (retailMaps == 0)
        {
            complete = false;
            Console.WriteLine("Warning: MAPMEDIAAUDIT No Uprising retail maps were read. Set REBORN_UPRISING_GAME_ROOT to the game folder.");
        }
        return new(audio, movies, complete, modMaps, retailMaps);

        // Reborn: a partially read map catalog cannot prove that an audio event or movie is unused.
        bool TryScan(string name, Func<byte[]> read)
        {
            try
            {
                var map = Decode(read());
                Collect(map, 12, audioCandidates, audio);
                Collect(map, 23, movieCandidates, movies);
                return true;
            }
            catch (Exception error) when (error is IOException or InvalidDataException or UnauthorizedAccessException)
            {
                complete = false;
                Console.WriteLine("Warning: MAPMEDIAAUDIT " + name + ": " + error.Message);
                return false;
            }
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: unwrap the map's EAR/RefPack layers and validate the binary map signature once per map. */
    //-------------------------------------------------------------------------------------------------
    private static ReadOnlySpan<byte> Decode(ReadOnlySpan<byte> data)
    {
        if (RefPack.IsCompressed(data))
            data = RefPack.Decompress(data, MaximumMapBytes);
        if (data.StartsWith("EAR\0"u8))
        {
            if (data.Length < 8) throw new InvalidDataException("Truncated EAR map header.");
            uint size = BinaryPrimitives.ReadUInt32LittleEndian(data[4..]);
            data = RefPack.Decompress(data[8..], MaximumMapBytes);
            if (data.Length != size) throw new InvalidDataException("EAR map length differs.");
        }
        if (!data.StartsWith("CkMp"u8)) throw new InvalidDataException("Unrecognized binary map header.");

        return data;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: match complete length-prefixed Sound (12) and Movie (23) script arguments by type. */
    //-------------------------------------------------------------------------------------------------
    private static void Collect(ReadOnlySpan<byte> data, byte type, HashSet<string> candidates, HashSet<string> used)
    {
        if (candidates.Count == 0) return;
        ReadOnlySpan<byte> marker = [type, 0, 0, 0];
        while (data.Length >= 14)
        {
            int offset = data.IndexOf(marker);
            if (offset < 0 || offset > data.Length - 14) break;
            var parameter = data[offset..];
            int length = BinaryPrimitives.ReadUInt16LittleEndian(parameter[12..]);
            if (length is > 0 and <= 128 && length <= parameter.Length - 14)
            {
                var token = parameter.Slice(14, length);
                if (token.IndexOfAnyExceptInRange((byte)32, (byte)126) < 0)
                {
                    string id = Encoding.ASCII.GetString(token);
                    if (candidates.Contains(id)) used.Add(id);
                }
            }
            data = parameter[marker.Length..];
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: honor an explicit portable SDK install override, then look in registered and conventional Windows installations. */
    //-------------------------------------------------------------------------------------------------
    private static string? FindRetailData()
    {
        string? explicitRoot = Environment.GetEnvironmentVariable("REBORN_UPRISING_GAME_ROOT");
        if (!string.IsNullOrWhiteSpace(explicitRoot)) return Data(explicitRoot);
        if (OperatingSystem.IsWindows())
        {
            foreach (string key in new[]
            {
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Electronic Arts\Electronic Arts\Red Alert 3 Uprising",
                @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Electronic Arts\Electronic Arts\Red Alert 3 Uprising",
                @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 24800"
            })
                foreach (string value in new[] { "Install Dir", "InstallLocation" })
                    if (Registry.GetValue(key, value, null) is string root && Data(root) is string found) return found;
        }
        foreach (var drive in DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Fixed))
        {
            var root = Path.Combine(drive.RootDirectory.FullName, "Program Files (x86)", "Steam", "steamapps", "common", "Command and Conquer Red Alert 3 Uprising");
            if (Data(root) is string found) return found;
        }
        return null;

        // Reborn: require an EP1 executable to distinguish a game install from an arbitrary map folder.
        static string? Data(string root)
        {
            string path = Path.Combine(root, "Data");
            return File.Exists(Path.Combine(path, "ra3ep1_1.1.game")) || File.Exists(Path.Combine(path, "ra3ep1_1.0.game")) ? path : null;
        }
    }
}
