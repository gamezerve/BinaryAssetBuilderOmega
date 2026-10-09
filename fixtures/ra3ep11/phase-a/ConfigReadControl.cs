using System;
using System.Diagnostics;
using System.IO;

// Reborn: this detached helper performs one bounded fixture read, not a game launch or asset load.
internal static class ConfigReadControl
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: require the one-byte external probe and expose the successful managed read and own PID. */
    //-------------------------------------------------------------------------------------------------
    private static int Main(string[] args)
    {
        if (args.Length != 1 || !Path.IsPathRooted(args[0])) return 2;
        FileInfo file = new FileInfo(args[0]);
        if (!file.Exists || file.Length != 1) return 3;
        for (DirectoryInfo directory = file.Directory; directory != null; directory = directory.Parent)
            if ((directory.Attributes & FileAttributes.ReparsePoint) != 0) return 4;
        if ((file.Attributes & FileAttributes.ReparsePoint) != 0) return 4;
        using (FileStream input = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read, 1))
        {
            byte[] value = new byte[1];
            if (input.Read(value, 0, 1) != 1 || value[0] != 10) return 5;
        }
        Console.WriteLine("{0}|READ|1|10", Process.GetCurrentProcess().Id);
        return 0;
    }
}
