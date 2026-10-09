using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

// Reborn: a self-expiring helper validates debugger lifetime without reading assets or launching children.
internal static class DebuggerLifetimeControl
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: publish owned identity, remain alive briefly, then prove natural completion after detachment. */
    //-------------------------------------------------------------------------------------------------
    private static int Main(string[] args)
    {
        if (args.Length != 1 || !Path.IsPathRooted(args[0]) || !Directory.Exists(args[0])) return 2;
        for (DirectoryInfo directory = new DirectoryInfo(args[0]); directory != null; directory = directory.Parent)
            if ((directory.Attributes & FileAttributes.ReparsePoint) != 0) return 3;
        using (Process self = Process.GetCurrentProcess())
            File.WriteAllText(Path.Combine(args[0], "started.txt"), self.Id + "|" + self.StartTime.ToUniversalTime().Ticks);
        Thread.Sleep(5000);
        File.WriteAllText(Path.Combine(args[0], "completed.txt"), "REBORN_NATURAL_COMPLETION");
        return 0;
    }
}
