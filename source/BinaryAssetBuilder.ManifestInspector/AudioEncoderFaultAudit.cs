namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: opt-in worker-local fault injection records actual native resource calls; it never enables production audio.
internal sealed class AudioEncoderFaultAudit
{
    internal static readonly string[] Scenarios = { "after-open","after-info","after-create","after-write","missing-output-parent","after-encode-wave","before-publish-wave","before-publish-xml" };
    private readonly string _scenario;
    private readonly Dictionary<string,int> _acquired = new(),_released = new();
    private readonly List<string> _statuses = new();
    private int _init,_shutdown,_unload;
    internal string? Directory { get; private set; }
    internal bool Triggered { get; private set; }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject unknown fault names before creating fixtures or loading a native library. */
    //-------------------------------------------------------------------------------------------------
    internal AudioEncoderFaultAudit(string scenario)
    { if (!Scenarios.Contains(scenario,StringComparer.Ordinal)) throw new ArgumentException("Unknown audio fault scenario."); _scenario = scenario; }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: track only this run's fresh owned directory and fail if a caller tries to redirect injection. */
    //-------------------------------------------------------------------------------------------------
    internal void SetDirectory(string directory) { if (Directory != null) throw new InvalidOperationException(); Directory = directory; }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: record an actually returned nonzero native handle before interpreting its status. */
    //-------------------------------------------------------------------------------------------------
    internal void Acquired(string kind) => _acquired[kind] = _acquired.GetValueOrDefault(kind)+1;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: count completed releases, preserving raw status rather than assuming undocumented success semantics. */
    //-------------------------------------------------------------------------------------------------
    internal void Released(string kind,int status) { _released[kind] = _released.GetValueOrDefault(kind)+1; _statuses.Add(kind+"="+status); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: separately record actual initialization/shutdown/unload returns, not merely entry into finally blocks. */
    //-------------------------------------------------------------------------------------------------
    internal void Lifecycle(string phase)
    { if (phase == "init") _init++; else if (phase == "shutdown") _shutdown++; else if (phase == "unload") _unload++; else throw new ArgumentException(); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: choose a nonexistent owned parent to test preflight rejection; never repeat the observed native invalid-parent crash. */
    //-------------------------------------------------------------------------------------------------
    internal string OutputPath(string output)
    {
        if (_scenario != "missing-output-parent" || Triggered) return output;
        Triggered = true; return Path.Combine(output,"missing-parent","encoded");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: inject only at named safe managed boundaries or mutate bounded owned source fixtures after encoding. */
    //-------------------------------------------------------------------------------------------------
    internal void Phase(string phase)
    {
        if (Triggered) return;
        if (_scenario == phase && phase.StartsWith("after-",StringComparison.Ordinal) && phase != "after-encode-wave")
        { Triggered = true; throw new InjectedFaultException(phase); }
        if ((_scenario == "after-encode-wave" && phase == "after-encode") || (_scenario == "before-publish-wave" && phase == "before-publish"))
        {
            Triggered = true; string file = Path.Combine(Directory!,"input.wav"); DateTime timestamp = File.GetLastWriteTimeUtc(file);
            using (FileStream stream = new(file,FileMode.Open,FileAccess.ReadWrite,FileShare.None))
            { if (stream.Length != 24044) throw new InvalidDataException(); stream.Position = 100; int value = stream.ReadByte(); stream.Position = 100; stream.WriteByte((byte)(value^1)); }
            File.SetLastWriteTimeUtc(file,timestamp);
        }
        if (_scenario == "before-publish-xml" && phase == "before-publish")
        {
            Triggered = true; string file = Path.Combine(Directory!,"ram.xml");
            using FileStream stream = new(file,FileMode.Open,FileAccess.ReadWrite,FileShare.None);
            if (stream.Length > 8192) throw new InvalidDataException(); byte[] bytes = new byte[(int)stream.Length]; stream.ReadExactly(bytes);
            string xml = new System.Text.UTF8Encoding(false,true).GetString(bytes);
            byte[] changed = System.Text.Encoding.UTF8.GetBytes(xml.Replace("PCCompression=\"XAS\"","PCCompression=\"NONE\"",StringComparison.Ordinal));
            stream.Position = 0; stream.Write(changed); stream.SetLength(changed.Length);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require expected rejection with complete resource-call balance and no diagnostic destination or staging output. */
    //-------------------------------------------------------------------------------------------------
    internal void AssertRejected(Exception failure)
    {
        // Reborn: balance alone is insufficient; require the intended reached stage and rejection reason rather than an unrelated failure.
        int expectedSource = _scenario.StartsWith("before-publish",StringComparison.Ordinal) ? 2 : 1;
        int expectedInfo = _scenario == "after-open" ? 0 : expectedSource;
        int expectedTarget = _scenario is "after-open" or "after-info" or "missing-output-parent" ? 0 : expectedSource;
        bool expectedFailure = _scenario switch
        {
            "after-open" or "after-info" or "after-create" or "after-write" => failure is InjectedFaultException && failure.Message == "Reborn injected fault: "+_scenario,
            "after-encode-wave" or "before-publish-wave" => failure is InvalidDataException && failure.Message.Contains("does not match core AudioFile InstanceHash",StringComparison.Ordinal),
            "before-publish-xml" => failure is NotSupportedException && failure.Message.Contains("Explicit proven rate/compression",StringComparison.Ordinal),
            "missing-output-parent" => failure is InvalidDataException && failure.Message.Contains("existing absolute parent",StringComparison.Ordinal),
            _ => false
        };
        if (!Triggered || Directory == null || _init != 1 || _shutdown != 1 || _unload != 1
            || !expectedFailure || _acquired.GetValueOrDefault("source") != expectedSource || _acquired.GetValueOrDefault("info") != expectedInfo
            || _acquired.GetValueOrDefault("target") != expectedTarget || _acquired.Any(pair => _released.GetValueOrDefault(pair.Key) != pair.Value)
            || _released.Any(pair => _acquired.GetValueOrDefault(pair.Key) != pair.Value)) throw new InvalidDataException("Fault did not balance completed native resource calls.",failure);
        if (System.IO.Directory.EnumerateDirectories(Directory).Any()) throw new InvalidDataException("Failed audio worker published or staged a package.",failure);
        if (_scenario.StartsWith("before-publish",StringComparison.Ordinal) && _acquired.GetValueOrDefault("source") != 2)
            throw new InvalidDataException("Publication fault did not complete both encodes first.");
        Console.WriteLine($"Audio fault {_scenario}: OK; acquired={string.Join(',',_acquired.Select(pair => pair.Key+":"+pair.Value))}; completed={string.Join(',',_statuses)}; init/shutdown/unload=1/1/1; no package/stage; rejected={failure.Message}");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: run one scenario in its explicitly launched x86 worker, accept only the intended managed rejection family. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string path,string scenario)
    {
        var audit = new AudioEncoderFaultAudit(scenario);
        try { AudioEncoderPoc.Run(path,true,audit); }
        catch (Exception exception) when (exception is InjectedFaultException or InvalidDataException or NotSupportedException)
        { audit.AssertRejected(exception); return; }
        throw new InvalidDataException("Fault scenario unexpectedly published successfully.");
    }

    // Reborn: distinguish deliberate managed fault injection from unrelated failures.
    internal sealed class InjectedFaultException(string phase) : Exception("Reborn injected fault: "+phase);
}
