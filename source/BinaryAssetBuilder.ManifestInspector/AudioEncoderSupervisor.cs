using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using BinaryAssetBuilder.Core;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: supervise a disposable codec worker and accept only bounded, current, independently verified diagnostic evidence.
internal static class AudioEncoderSupervisor
{
    // Reborn: versioned job/result metadata never supplies an authoritative output root or production hash.
    internal sealed record Request(int Version,string Nonce,string Library,string Mode,Item[]? Inputs = null);
    internal sealed record Item(string Path,long Length,string Sha256);
    internal sealed record Result(int Version,string Nonce,Item[] Files);
    private sealed record Captured(string Text,bool Overflow);
    private static readonly JsonSerializerOptions JsonOptions = new() { UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow };
    private const int LogLimit = 65536;
    private const string Prefix = "Reborn-SupervisedAudio-";
    // Reborn: protocol v2 binds an optional authored three-file snapshot; older v1 jobs are not silently reused.
    private const int ProtocolVersion = 2;
    internal static readonly string[] TestModes = { "exit","crash-exit","timeout","missing-result","malformed-result","wrong-nonce","log-overflow","fake-success","escape-path","duplicate-key","unknown-field","oversized-result" };
    // Reborn: real native tamper tests remain explicitly opt-in, separate from all default managed worker tests.
    private static readonly string[] NativeTestModes = { "encode-tamper","encode-corrupt-package" };

    //-------------------------------------------------------------------------------------------------
    /** Reborn: create one owned job, launch only this apphost without a visible window, enforce exit/time/log limits and verify results before acceptance. */
    //-------------------------------------------------------------------------------------------------
    internal static string Run(string library,string mode = "encode",int timeoutMs = 30000,Action<string>? jobCreated = null,AuthoredAudioSnapshot? authored = null)
    {
        if (mode != "encode" && mode is not ("encode-authored" or "encode-authored-event") && !TestModes.Contains(mode,StringComparer.Ordinal) && !NativeTestModes.Contains(mode,StringComparer.Ordinal)) throw new ArgumentException("Unknown supervised worker mode.");
        if ((mode is "encode-authored" or "encode-authored-event") != (authored != null)
            || (mode == "encode-authored-event") != (authored?.EventName != null)) throw new InvalidDataException("Authored mode requires a matching validated input snapshot.");
        if (timeoutMs is < 100 or > 30000) throw new ArgumentOutOfRangeException(nameof(timeoutMs));
        string exe = Environment.ProcessPath ?? throw new NotSupportedException("An inspector apphost is required.");
        if (!exe.EndsWith("BinaryAssetBuilder.ManifestInspector.exe",StringComparison.OrdinalIgnoreCase)) throw new NotSupportedException("Supervisor requires the Windows inspector apphost.");
        string nonce = Guid.NewGuid().ToString("N"); string job = Path.Combine(Path.GetTempPath(),Prefix+nonce);
        Directory.CreateDirectory(job);
        Item[]? inputs = null;
        if (authored != null) { string inputRoot = Path.Combine(job,"inputs"); authored.Install(inputRoot); inputs = Inventory(inputRoot); }
        WriteNew(Path.Combine(job,"request.json"),JsonSerializer.SerializeToUtf8Bytes(new Request(ProtocolVersion,nonce,Path.GetFullPath(library),mode,inputs),JsonOptions));
        // Reborn: managed regressions can inspect the exact owned job after rejection without parsing worker output or guessing paths.
        jobCreated?.Invoke(job);
        Console.WriteLine("Owned supervised audio job: "+job);
        ProcessStartInfo start = new(exe) { UseShellExecute = false,CreateNoWindow = true,RedirectStandardOutput = true,RedirectStandardError = true,WorkingDirectory = job };
        start.ArgumentList.Add("audio-encoder-worker"); start.ArgumentList.Add(job); start.ArgumentList.Add(nonce);
        using Process process = Process.Start(start) ?? throw new InvalidDataException("Audio worker did not start.");
        Task<Captured> stdout = Capture(process.StandardOutput),stderr = Capture(process.StandardError);
        bool timedOut = false;
        using (CancellationTokenSource timeout = new(timeoutMs))
        {
            try { process.WaitForExitAsync(timeout.Token).GetAwaiter().GetResult(); }
            catch (OperationCanceledException)
            {
                timedOut = true; if (!process.HasExited) process.Kill(entireProcessTree:true);
                if (!process.WaitForExit(5000)) throw new InvalidDataException("Timed-out audio worker did not terminate.");
            }
        }
        // Reborn: bounded concurrent drains avoid pipe deadlock; even rejected worker logs are not printed as unbounded output.
        Task drain = Task.WhenAll(stdout,stderr);
        if (!drain.Wait(5000)) throw new InvalidDataException("Audio worker pipes did not close.");
        // Reborn: retain capped untrusted logs as evidence in the owned job, not as unlimited console output or success signals.
        WriteNew(Path.Combine(job,"worker.stdout.txt"),Encoding.UTF8.GetBytes(stdout.Result.Text));
        WriteNew(Path.Combine(job,"worker.stderr.txt"),Encoding.UTF8.GetBytes(stderr.Result.Text));
        if (timedOut) throw new InvalidDataException("Audio worker timed out; partial evidence retained, no acceptance.");
        if (process.ExitCode != 0) throw new InvalidDataException($"Audio worker exit={process.ExitCode}; partial evidence retained, no acceptance.");
        if (stdout.Result.Overflow || stderr.Result.Overflow) throw new InvalidDataException("Audio worker log limit exceeded; no acceptance.");
        ValidateResult(job,nonce,authored);
        // Reborn: worker success cannot authorize a stale caller source; reread original inputs before acceptance without writing them.
        authored?.VerifyCurrent();
        WriteNew(Path.Combine(job,"ACCEPTED.json"),JsonSerializer.SerializeToUtf8Bytes(new { Version = ProtocolVersion,Nonce = nonce,DiagnosticOnly = true },JsonOptions));
        Console.WriteLine("Supervised audio: ACCEPTED (exit=0, bounded protocol/hash inventory and independent package/core readback; diagnostic only).");
        return job;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: run only a nonce-matched owned request; synthetic transport failures never load the native DLL. */
    //-------------------------------------------------------------------------------------------------
    internal static void Worker(string job,string nonce)
    {
        job = Path.GetFullPath(job);
        if (!Guid.TryParseExact(nonce,"N",out _) || job != Path.Combine(Path.GetTempPath(),Prefix+nonce)) throw new InvalidDataException("Worker job root/nonce differs.");
        Request request = ReadJson<Request>(Path.Combine(job,"request.json"));
        if (request.Version != ProtocolVersion || request.Nonce != nonce || (request.Mode != "encode" && request.Mode is not ("encode-authored" or "encode-authored-event") && !TestModes.Contains(request.Mode,StringComparer.Ordinal) && !NativeTestModes.Contains(request.Mode,StringComparer.Ordinal))) throw new InvalidDataException("Worker request differs.");
        AuthoredAudioSnapshot? authored = null;
        if (request.Mode is "encode-authored" or "encode-authored-event")
        {
            string inputs = Path.Combine(job,"inputs"); Item[] actualInputs = Inventory(inputs);
            // Reborn: mode fixes the snapshot cardinality; an event cannot be silently dropped or injected into the three-file path.
            int count = request.Mode == "encode-authored-event" ? 4 : 3;
            if (request.Inputs == null || request.Inputs.Length != count || actualInputs.Length != count || actualInputs.Any(item => !request.Inputs.Contains(item))) throw new InvalidDataException("Worker authored snapshot inventory differs.");
        }
        else if (request.Inputs != null) throw new InvalidDataException("Unexpected authored worker inputs.");
        string result = Path.Combine(job,"result.json"),work = Path.Combine(job,"worker");
        if (request.Mode == "exit") { Environment.Exit(37); return; }
        // Reborn: simulate an access-violation exit code without crashing or loading native code; do not repeat the known unsafe DLL condition.
        if (request.Mode == "crash-exit") { Environment.Exit(unchecked((int)0xC0000005u)); return; }
        if (request.Mode == "timeout") { Thread.Sleep(30000); return; }
        if (request.Mode == "missing-result") return;
        if (request.Mode == "malformed-result") { WriteNew(result,Encoding.UTF8.GetBytes("{")); return; }
        if (request.Mode == "log-overflow") { Console.Write(new string('x',LogLimit+1)); return; }
        if (request.Mode == "duplicate-key") { WriteNew(result,Encoding.UTF8.GetBytes("{\"Version\":1,\"Version\":1,\"Nonce\":\""+nonce+"\",\"Files\":[]}")); return; }
        if (request.Mode == "unknown-field") { WriteNew(result,Encoding.UTF8.GetBytes("{\"Version\":1,\"Nonce\":\""+nonce+"\",\"Files\":[],\"Unknown\":true}")); return; }
        if (request.Mode == "oversized-result") { WriteNew(result,new byte[32769]); return; }
        if (request.Mode is "wrong-nonce" or "fake-success" or "escape-path")
        {
            Item[] files = request.Mode == "escape-path" ? new[] { new Item("../request.json",1,new string('0',64)) } : Array.Empty<Item>();
            WriteNew(result,JsonSerializer.SerializeToUtf8Bytes(new Result(ProtocolVersion,request.Mode == "wrong-nonce" ? new string('0',32) : nonce,files),JsonOptions)); return;
        }
        CompilerSmokeTest.InitializeHashProvider();
        // Reborn: input validation needs the managed symbol tables, never the native codec; synthetic transport failures above bypass this branch.
        authored = request.Mode is "encode-authored" or "encode-authored-event" ? AuthoredAudioSnapshot.Read(Path.Combine(job,"inputs"),request.Mode == "encode-authored-event") : null;
        AudioEncoderPoc.Run(request.Library,true,null,work,authored);
        // Reborn: mutate only new owned evidence; a matching inventory alone must not authorize an invalid manifest.
        if (request.Mode == "encode-corrupt-package") Flip(Path.Combine(work,"package","diagnostic.manifest"));
        Item[] inventory = Inventory(work);
        if (request.Mode == "encode-tamper") Flip(Path.Combine(work,"ram.runtime.bin"));
        // Reborn: result completion is exclusive and emitted only after normal codec shutdown/unload and full worker verification.
        WriteNew(result,JsonSerializer.SerializeToUtf8Bytes(new Result(ProtocolVersion,nonce,inventory),JsonOptions));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove parent-side byte and semantic rejection with actual encoded child results; never accept these diagnostic tamper jobs. */
    //-------------------------------------------------------------------------------------------------
    internal static void NativeTamperTests(string library)
    {
        foreach (string mode in NativeTestModes)
        {
            string? job = null; bool rejected = false;
            try { Run(library,mode,30000,path => job = path); }
            catch (InvalidDataException exception)
            {
                string expected = mode == "encode-tamper" ? "inventory/hash differs" : "Frozen audio package bytes differ";
                if (!exception.Message.Contains(expected,StringComparison.Ordinal)) throw new InvalidDataException("Native tamper job failed for unintended reason.",exception);
                rejected = true;
            }
            if (!rejected || job == null || File.Exists(Path.Combine(job,"ACCEPTED.json"))) throw new InvalidDataException("Invalid native result was accepted.");
        }
        Console.WriteLine("Supervised native tamper tests: OK (changed artifact hash; corrupt manifest with matching inventory; no acceptance).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: flip one byte of a bounded owned artifact to exercise parent rejection, never mutate a source/game file. */
    //-------------------------------------------------------------------------------------------------
    private static void Flip(string path)
    { CheckPath(path); using FileStream stream = new(path,FileMode.Open,FileAccess.ReadWrite,FileShare.None); if (stream.Length is < 1 or > 1048576) throw new InvalidDataException(); int value = stream.ReadByte(); stream.Position = 0; stream.WriteByte((byte)(value^1)); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject forged paths/inventories, then rehash every bounded artifact and reconstruct both diagnostic packages without loading codecs. */
    //-------------------------------------------------------------------------------------------------
    internal static void ValidateResult(string job,string nonce,AuthoredAudioSnapshot? authored = null)
    {
        string result = Path.Combine(job,"result.json"),work = Path.Combine(job,"worker");
        if (!File.Exists(result)) throw new InvalidDataException("Audio worker missing result; no acceptance.");
        Result value = ReadJson<Result>(result);
        if (value.Version != ProtocolVersion || value.Nonce != nonce || value.Files == null || value.Files.Length is < 1 or > 64) throw new InvalidDataException("Worker result nonce/version/inventory differs.");
        HashSet<string> paths = new(StringComparer.OrdinalIgnoreCase);
        foreach (Item item in value.Files)
        {
            if (item == null || string.IsNullOrEmpty(item.Path) || item.Path.Length > 256 || item.Path.Contains('\\') || Path.IsPathRooted(item.Path)
                || item.Path.Split('/').Any(part => part.Length == 0 || part is "." or "..") || !paths.Add(item.Path)
                || item.Length is < 0 or > 1048576 || item.Sha256 == null || item.Sha256.Length != 64 || item.Sha256.Any(character => !Uri.IsHexDigit(character)))
                throw new InvalidDataException("Worker result path/size/hash is not bounded or unique.");
            string resolved = Path.GetFullPath(Path.Combine(work,item.Path));
            if (!resolved.StartsWith(work+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase)) throw new InvalidDataException("Worker result escapes owned root.");
        }
        Item[] actual = Inventory(work);
        if (actual.Length != value.Files.Length || actual.Any(item => !value.Files.Contains(item))) throw new InvalidDataException("Worker artifact inventory/hash differs.");
        authored?.VerifyCopies(work);
        if (authored != null) authored.VerifyCopies(Path.Combine(job,"inputs"));
        var entries = new[] { Entry(false),Entry(true) };
        string schema = Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"AudioFileIdentityPipeline.xsd");
        var bindings = entries.Select(entry =>
        {
            // Reborn: this fixed PoC admits only its canonical authored source before invoking the core loader; no forged Includes/DTD/formulas are evaluated.
            if (authored == null && !Read(Path.Combine(work,entry.Source),8192).SequenceEqual(Encoding.UTF8.GetBytes(AudioEncoderPoc.CoreSource(entry.Source == "streamed.xml"))))
                throw new InvalidDataException("Worker authored source differs from fixed profile.");
            InstanceDeclaration instance = AudioFileIdentitySmokeTest.Build(work,schema,AudioFileIdentitySmokeTest.Processing,entry.Source);
            return new CoreAudioPackageGate.Binding(instance,AudioFileCorePreparation.Prepare(instance),entry);
        }).ToArray();
        CoreAudioPackageGate.Verify(bindings); AudioFilePackageProbe.Verify(Path.Combine(work,"package"),entries);
        // Reborn: frozen encoder PCM and the informational sidecar must agree with independently reconstructed current core identities.
        byte[] wave = Read(Path.Combine(work,"input.wav"),24044);
        foreach (string prefix in new[] { "ram","streamed" })
            if (!Read(Path.Combine(work,prefix+".input.wav"),24044).SequenceEqual(wave)) throw new InvalidDataException("Worker frozen PCM snapshot differs.");
        string expectedSidecar = "Diagnostic evidence only; core hash and package content hash are different domains.\n"+
            string.Join("\n",bindings.Select(binding => $"{binding.Encoded.Name}: core={binding.Instance.Handle.InstanceHash:X8}, diagnostic-content={binding.Encoded.Hash:X8}"))+"\n";
        if (!Read(Path.Combine(work,"core-identities.txt"),8192).SequenceEqual(Encoding.UTF8.GetBytes(expectedSidecar))) throw new InvalidDataException("Worker identity sidecar differs.");
        if (authored?.EventName == null && !Read(Path.Combine(work,"event.xml"),8192).SequenceEqual(Encoding.UTF8.GetBytes(AudioFileLocalEventProbe.Source(entries)))) throw new InvalidDataException("Worker derived event source differs.");
        byte[] mixedBin = Read(Path.Combine(work,"local-event-package","diagnostic.bin")),mixedRelo = Read(Path.Combine(work,"local-event-package","diagnostic.relo")),mixedImp = Read(Path.Combine(work,"local-event-package","diagnostic.imp"));
        // Reborn: authored subtitles change AudioFile native lengths; locate the fixed event after the independently reconstructed leaves, not old fixture offsets.
        int eventStart = 8+entries.Sum(entry => entry.CopyNative().InstanceData.Length),eventReloStart = 8+entries.Sum(entry => entry.CopyNative().RelocationData.Length);
        if (mixedBin.Length != eventStart+176 || mixedRelo.Length != eventReloStart+8 || mixedImp.Length != 20) throw new InvalidDataException("Worker mixed package shape differs.");
        var localEvent = new AudioFileLocalEventProbe.Entry(new AssetBuffer { InstanceData = mixedBin.AsSpan(eventStart).ToArray(),RelocationData = mixedRelo.AsSpan(eventReloStart).ToArray(),ImportsData = mixedImp.AsSpan(8).ToArray() },entries,authored?.EventName ?? "RebornLocalAudio",authored?.EventSettings);
        if (authored?.EventName != null)
        {
            // Reborn: independently recompile frozen authored event XML in the parent; a matching inventory and valid selector shape alone are insufficient.
            var rebuilt = AudioFileLocalEventProbe.Build(work,entries,authoredName:authored.EventName);
            AssetBuffer expectedEvent = rebuilt.CopyNative(),actualEvent = localEvent.CopyNative();
            if (rebuilt.Id != localEvent.Id || rebuilt.Hash != localEvent.Hash || !expectedEvent.InstanceData.SequenceEqual(actualEvent.InstanceData)
                || !expectedEvent.RelocationData.SequenceEqual(actualEvent.RelocationData) || !expectedEvent.ImportsData.SequenceEqual(actualEvent.ImportsData))
                throw new InvalidDataException("Worker authored event differs from independently compiled source.");
        }
        AudioFilePackageProbe.Verify(Path.Combine(work,"local-event-package"),entries,localEvent);

        //-------------------------------------------------------------------------------------------------
        /** Reborn: reconstruct immutable encoded leaves from bounded raw runtime/custom evidence, not worker-supplied native metadata. */
        //-------------------------------------------------------------------------------------------------
        AudioFilePackageProbe.Entry Entry(bool streamed)
        {
            string prefix = Path.Combine(work,streamed ? "streamed" : "ram");
            return new(authored == null ? (streamed ? "RebornAudioStream" : "RebornAudioRAM") : (streamed ? authored.StreamName : authored.RamName),streamed ? "streamed.xml" : "ram.xml",
                new AssetBuffer { InstanceData = Read(prefix+".runtime.bin"),RelocationData = Read(prefix+".runtime.relo"),ImportsData = Array.Empty<byte>() },Read(prefix+(streamed ? ".sns" : ".snr")));
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: inventory only shallow non-reparse owned evidence; bound files, directories and aggregate bytes before hashing. */
    //-------------------------------------------------------------------------------------------------
    internal static Item[] Inventory(string work)
    {
        List<Item> result = new(); int directories = 0; long total = 0; Visit(work,0); return result.OrderBy(item => item.Path,StringComparer.Ordinal).ToArray();
        //-------------------------------------------------------------------------------------------------
        /** Reborn: inspect each directory before descending so symlinks cannot expand traversal beyond the owned tree. */
        //-------------------------------------------------------------------------------------------------
        void Visit(string directory,int depth)
        {
            if (depth > 4 || ++directories > 16) throw new InvalidDataException("Worker artifact directory bound exceeded.");
            CheckPath(directory);
            foreach (string path in Directory.EnumerateFileSystemEntries(directory))
            {
                CheckPath(path);
                if (Directory.Exists(path)) { Visit(path,depth+1); continue; }
                if (result.Count == 64) throw new InvalidDataException("Worker artifact file bound exceeded.");
                byte[] bytes = Read(path); total += bytes.Length; if (total > 4*1048576) throw new InvalidDataException("Worker artifact aggregate bound exceeded.");
                result.Add(new(Path.GetRelativePath(work,path).Replace('\\','/'),bytes.Length,Convert.ToHexString(SHA256.HashData(bytes))));
            }
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject reparse ancestry before bounded file reads; this does not promise adversarial concurrent filesystem isolation. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckPath(string path)
    { for (string? current = Path.GetFullPath(path); current != null; current = Path.GetDirectoryName(current)) if ((File.GetAttributes(current) & FileAttributes.ReparsePoint) != 0) throw new InvalidDataException("Worker evidence has reparse ancestry."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: use a single bounded read handle and reject length changes rather than reading arbitrary binaries. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Read(string path,int limit = 1048576)
    { CheckPath(path); using FileStream stream = File.OpenRead(path); if (stream.Length > limit) throw new InvalidDataException("Worker file exceeds bound."); byte[] bytes = new byte[(int)stream.Length]; stream.ReadExactly(bytes); if (stream.ReadByte() != -1) throw new InvalidDataException("Worker file grew during read."); return bytes; }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject oversized, duplicate-key or unknown-field JSON contracts before interpreting worker-controlled metadata. */
    //-------------------------------------------------------------------------------------------------
    private static T ReadJson<T>(string path)
    {
        try
        {
            byte[] bytes = Read(path,32768); using JsonDocument document = JsonDocument.Parse(bytes); Unique(document.RootElement);
            return JsonSerializer.Deserialize<T>(bytes,JsonOptions) ?? throw new InvalidDataException("Worker JSON is null.");
        }
        catch (JsonException exception) { throw new InvalidDataException("Worker JSON is invalid.",exception); }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: inspect nested objects too so duplicate file metadata keys cannot override earlier declarations. */
        //-------------------------------------------------------------------------------------------------
        void Unique(JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Object)
            { HashSet<string> names = new(StringComparer.Ordinal); foreach (JsonProperty property in element.EnumerateObject()) { if (!names.Add(property.Name)) throw new InvalidDataException("Worker JSON duplicate key."); Unique(property.Value); } }
            else if (element.ValueKind == JsonValueKind.Array) foreach (JsonElement child in element.EnumerateArray()) Unique(child);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: drain each pipe concurrently with capped retained text; never block a worker on an unread output pipe. */
    //-------------------------------------------------------------------------------------------------
    private static async Task<Captured> Capture(StreamReader reader)
    {
        StringBuilder text = new(); char[] buffer = new char[4096]; bool overflow = false; int count;
        while ((count = await reader.ReadAsync(buffer)) != 0) { int keep = Math.Min(count,LogLimit-text.Length); if (keep > 0) text.Append(buffer,0,keep); if (keep != count) overflow = true; }
        return new(text.ToString(),overflow);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: write only newly owned protocol files; partial or existing results are never overwritten. */
    //-------------------------------------------------------------------------------------------------
    private static void WriteNew(string path,byte[] bytes) { using FileStream stream = new(path,FileMode.CreateNew,FileAccess.Write); stream.Write(bytes); }
}
