namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: protect only discovered XML inputs, from their first read until the native stream is qualified.
internal sealed class SdkXmlInputScope : IDisposable
{
    [ThreadStatic] private static SdkXmlInputScope? capturing;

    private readonly Dictionary<string, FileStream> inputs = new(StringComparer.OrdinalIgnoreCase);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: collect source XML handles during preparation and keep them open during compilation. */
    //-------------------------------------------------------------------------------------------------
    internal void Prepare(string entry, string output, string sdk, string stockData)
    {
        capturing = this;
        try
        {
            ConscriptSourceInput.Prepare(entry, output, sdk, stockData);
        }
        finally
        {
            capturing = null;
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: deny writes, renames and deletes without changing attributes or preventing other readers. */
    //-------------------------------------------------------------------------------------------------
    internal static void HoldIfCapturing(string path)
    {
        if (capturing == null || !Path.GetExtension(path).Equals(".xml", StringComparison.OrdinalIgnoreCase))
            return;

        string fullPath = Path.GetFullPath(path);
        if (capturing.inputs.ContainsKey(fullPath))
            return;

        try
        {
            capturing.inputs.Add(fullPath, File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read));
        }
        catch (IOException error)
        {
            throw new IOException($"Cannot lock XML source for compilation: {fullPath}. Close any writer and retry.", error);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: allow the build to keep captured media bytes while the original can change. */
    //-------------------------------------------------------------------------------------------------
    internal static bool IsMutableMedia(string path) =>
        Path.GetExtension(path).ToLowerInvariant() is ".dds" or ".tga" or ".wav" or ".map";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: release this stream's XML handles on success or error. */
    //-------------------------------------------------------------------------------------------------
    public void Dispose()
    {
        foreach (var stream in inputs.Values)
            stream.Dispose();
        inputs.Clear();
    }
}
