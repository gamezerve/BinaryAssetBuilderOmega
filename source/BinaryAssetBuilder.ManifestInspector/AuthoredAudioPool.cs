using System.Text;
using System.Text.Json;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: freeze a bounded explicit variable AudioFile inventory without scanning directories or enabling native/production compilation.
internal sealed class AuthoredAudioPool
{
    // Reborn: metadata is immutable and distinct from mutable core instances; source order belongs to the inventory.
    internal sealed record Row(string Source,string Wave,string Name,uint Id,bool Streamed);
    internal sealed record CoreRow(string Source,string Name,uint Id,uint CoreHash,bool Streamed);
    private readonly string _directory;
    private readonly Dictionary<string,byte[]> _files;
    private readonly Row[] _rows;
    internal Row[] Rows => (Row[])_rows.Clone();
    internal string[] FileNames => _files.Keys.ToArray();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: own only freshly read private byte buffers and immutable inventory metadata. */
    //-------------------------------------------------------------------------------------------------
    private AuthoredAudioPool(string directory,Dictionary<string,byte[]> files,Row[] rows)
    { _directory = directory; _files = files; _rows = rows; }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: return detached evidence rather than exposing frozen source/WAV buffers to callers. */
    //-------------------------------------------------------------------------------------------------
    internal byte[] Copy(string name) => (byte[])_files[name].Clone();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare exact current inventory/source/dependency bytes, including timestamp-preserving edits or inventory reorder. */
    //-------------------------------------------------------------------------------------------------
    internal void VerifyCurrent()
    {
        var current = Read(_directory);
        if (_files.Count != current._files.Count || _files.Any(file => !current._files.TryGetValue(file.Key,out byte[]? bytes) || !file.Value.SequenceEqual(bytes)))
            throw new InvalidDataException("Authored audio pool snapshot is stale.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify every installed named file against the parent's frozen bytes before core source loading. */
    //-------------------------------------------------------------------------------------------------
    internal void VerifyCopies(string directory)
    { foreach (var file in _files) if (!file.Value.SequenceEqual(ReadFile(Path.Combine(directory,file.Key),Limit(file.Key)))) throw new InvalidDataException("Authored audio pool copy differs."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: install exclusively into an empty non-reparse owned directory; never overwrite or enumerate caller sources. */
    //-------------------------------------------------------------------------------------------------
    internal void Install(string directory)
    {
        CheckPath(directory);
        if (Directory.Exists(directory) && Directory.EnumerateFileSystemEntries(directory).Any()) throw new InvalidDataException("Audio pool destination must be empty.");
        Directory.CreateDirectory(directory);
        foreach (var file in _files) { using FileStream writer = new(Path.Combine(directory,file.Key),FileMode.CreateNew,FileAccess.Write); writer.Write(file.Value); }
        VerifyCopies(directory);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: accept 1..8 explicitly named XML sources and only their bounded direct WAV dependencies, with strict inventory syntax and unique SAGE IDs. */
    //-------------------------------------------------------------------------------------------------
    internal static AuthoredAudioPool Read(string directory)
    {
        directory = Path.GetFullPath(directory);
        Dictionary<string,byte[]> files = new(StringComparer.OrdinalIgnoreCase) { ["audio-pool.json"] = ReadFile(Path.Combine(directory,"audio-pool.json"),4096) };
        using JsonDocument json = JsonDocument.Parse(new UTF8Encoding(false,true).GetString(files["audio-pool.json"]),new JsonDocumentOptions { MaxDepth = 4 });
        JsonElement root = json.RootElement;
        if (root.ValueKind != JsonValueKind.Object || root.EnumerateObject().Count() != 2 || root.EnumerateObject().Select(field => field.Name).Distinct(StringComparer.Ordinal).Count() != 2
            || !root.TryGetProperty("version",out var version) || version.ValueKind != JsonValueKind.Number || !version.TryGetInt32(out int number) || number != 1
            || !root.TryGetProperty("sources",out var sources) || sources.ValueKind != JsonValueKind.Array || sources.GetArrayLength() is < 1 or > 8)
            throw new InvalidDataException("Audio pool inventory requires exactly version=1 and 1..8 sources.");
        HashSet<string> names = new(StringComparer.OrdinalIgnoreCase); HashSet<uint> ids = new(); List<Row> rows = new();
        using MemoryStream pcm = new(); AudioEncoderPoc.WriteWave(pcm); byte[] knownWave = pcm.ToArray();
        foreach (JsonElement item in sources.EnumerateArray())
        {
            if (item.ValueKind != JsonValueKind.String) throw new InvalidDataException("Audio pool sources must be literal XML leaf names.");
            string source = item.GetString()!;
            if (!source.EndsWith(".xml",StringComparison.Ordinal) || source.Length > 100) throw new InvalidDataException("Audio pool requires bounded .xml leaf names.");
            AudioFileDiagnosticIdentity.Validate(source[..^4]);
            ValidateDeviceLeaf(source);
            if (!names.Add(source)) throw new InvalidDataException("Audio pool repeats a source name or case alias.");
            byte[] bytes = ReadFile(Path.Combine(directory,source),8192); files.Add(source,bytes);
            XmlElement audio = Definition(bytes);
            AudioFileDiagnosticIdentity.Validate(audio.GetAttribute("id")); var identity = AudioEncoderPoc.Identity(audio);
            // Reborn: validate the direct File leaf and settings before combining authored text with the filesystem root.
            Ra3Ep1AudioFileInputProfile.Prepare(audio,identity,TargetPlatform.Win32,knownWave);
            string wave = audio.GetAttribute("File");
            ValidateDeviceLeaf(wave);
            if (!files.TryGetValue(wave,out byte[]? waveBytes)) { waveBytes = ReadFile(Path.Combine(directory,wave),24044); files.Add(wave,waveBytes); }
            var input = Ra3Ep1AudioFileInputProfile.Prepare(audio,identity,TargetPlatform.Win32,waveBytes);
            if (!ids.Add(identity.InstanceId)) throw new InvalidDataException("Audio pool names collide in the SAGE instance-ID domain.");
            rows.Add(new(source,wave,identity.InstanceName,identity.InstanceId,input.Streamed));
        }
        if (files.Count > 17 || files.Values.Sum(bytes => bytes.Length) > 262144) throw new InvalidDataException("Audio pool aggregate snapshot exceeds its bound.");
        return new(directory,files,rows.ToArray());
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prepare each frozen source with the actual isolated core and recheck originals; no native codec, event graph or stream publication is authorized. */
    //-------------------------------------------------------------------------------------------------
    internal (string Directory,CoreRow[] Rows) Preflight()
    {
        string owned = Path.Combine(Path.GetTempPath(),"Reborn-AudioPool-"+Guid.NewGuid().ToString("N")); Install(owned);
        string schema = Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"AudioFileIdentityPipeline.xsd");
        List<CoreRow> result = new();
        foreach (Row row in _rows)
        {
            VerifyCopies(owned);
            InstanceDeclaration instance = AudioFileIdentitySmokeTest.Build(owned,schema,AudioFileIdentitySmokeTest.Processing,row.Source);
            var prepared = AudioFileCorePreparation.Prepare(instance); prepared.VerifyCurrent(instance);
            if (instance.Handle.InstanceName != row.Name || instance.Handle.InstanceId != row.Id || prepared.Settings.FileName != row.Wave || prepared.Settings.Streamed != row.Streamed)
                throw new InvalidDataException("Audio pool core identity/dependency differs from its frozen inventory.");
            result.Add(new(row.Source,row.Name,row.Id,instance.Handle.InstanceHash,row.Streamed));
        }
        VerifyCopies(owned); VerifyCurrent(); return (owned,result.ToArray());
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: parse strict UTF-8, one direct AudioFile and only trusted schema includes before invoking any core loader. */
    //-------------------------------------------------------------------------------------------------
    private static XmlElement Definition(byte[] bytes)
    {
        if (bytes.AsSpan().StartsWith(new byte[] { 0xEF,0xBB,0xBF })) throw new InvalidDataException("Audio pool XML must be UTF-8 without BOM.");
        XmlDocument document = new() { XmlResolver = null };
        using (XmlReader reader = XmlReader.Create(new StringReader(new UTF8Encoding(false,true).GetString(bytes)),new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit,XmlResolver = null })) document.Load(reader);
        if (document.FirstChild is XmlDeclaration declaration && declaration.Encoding.Length > 0 && !declaration.Encoding.Equals("utf-8",StringComparison.OrdinalIgnoreCase)) throw new InvalidDataException("Audio pool XML must declare UTF-8.");
        if (document.DocumentElement?.LocalName != "AssetDeclaration" || document.DocumentElement.NamespaceURI != "uri:ea.com:eala:asset"
            || document.DocumentElement.ChildNodes.Count != 1 || document.DocumentElement.FirstChild is not XmlElement audio || audio.LocalName != "AudioFile")
            throw new InvalidDataException("Audio pool source requires exactly one AudioFile without Includes.");
        document.Schemas.XmlResolver = new XmlUrlResolver(); document.Schemas.Add("uri:ea.com:eala:asset",Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"AudioFileIdentityPipeline.xsd"));
        document.Validate((_,args) => throw new InvalidDataException(args.Message)); return audio;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: derive a bounded file limit only for the inventory or already validated source/dependency leaf. */
    //-------------------------------------------------------------------------------------------------
    private static int Limit(string name) => name == "audio-pool.json" ? 4096 : name.EndsWith(".xml",StringComparison.Ordinal) ? 8192 : 24044;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: ordinary ASCII leaf syntax must not open a Windows reserved device, including names with additional extensions. */
    //-------------------------------------------------------------------------------------------------
    private static void ValidateDeviceLeaf(string name)
    {
        string stem = name.Split('.')[0].ToUpperInvariant();
        if (stem is "CON" or "PRN" or "AUX" or "NUL" || (stem.Length == 4 && (stem.StartsWith("COM",StringComparison.Ordinal) || stem.StartsWith("LPT",StringComparison.Ordinal)) && stem[3] is >= '1' and <= '9'))
            throw new InvalidDataException("Audio pool cannot open Windows device leaf names.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: read bounded bytes through one handle with no reparse ancestry, never a whole binary or recursive source tree. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] ReadFile(string path,int limit)
    {
        CheckPath(path); using FileStream reader = File.OpenRead(path);
        if (reader.Length > limit) throw new InvalidDataException("Audio pool file exceeds its bound.");
        byte[] bytes = new byte[(int)reader.Length]; reader.ReadExactly(bytes); if (reader.ReadByte() != -1) throw new InvalidDataException("Audio pool source length changed during reading."); return bytes;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject existing reparse ancestors for source reads and fresh owned-copy installation alike. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckPath(string path)
    {
        for (string? current = Path.GetFullPath(path); current != null; current = Path.GetDirectoryName(current))
            if ((File.Exists(current) || Directory.Exists(current)) && (File.GetAttributes(current) & FileAttributes.ReparsePoint) != 0) throw new InvalidDataException("Audio pool cannot use reparse paths.");
    }
}
