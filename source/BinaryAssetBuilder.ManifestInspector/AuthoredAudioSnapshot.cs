using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: own a bounded authored RAM/streamed pair and PCM snapshot; never rewrite caller sources or silently rename their identities.
internal sealed class AuthoredAudioSnapshot
{
    internal static readonly string[] Names = { "ram.xml","streamed.xml","input.wav" };
    private readonly Dictionary<string,byte[]> _files;
    private readonly string _directory;
    // Reborn: preserve caller names independently of worker-provided manifests or metadata.
    internal string RamName { get; private init; } = "";
    internal string StreamName { get; private init; } = "";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain only privately read owned buffers and the source directory needed for a final current-source check. */
    //-------------------------------------------------------------------------------------------------
    private AuthoredAudioSnapshot(string directory,Dictionary<string,byte[]> files) { _directory = directory; _files = files; }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: return detached bytes so a worker/encoder cannot alter the parent's frozen source evidence. */
    //-------------------------------------------------------------------------------------------------
    internal byte[] Copy(string name) => (byte[])_files[name].Clone();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject current source changes even if timestamps/sizes are preserved; this is not an atomic filesystem transaction. */
    //-------------------------------------------------------------------------------------------------
    internal void VerifyCurrent()
    {
        AuthoredAudioSnapshot current = Read(_directory);
        if (Names.Any(name => !_files[name].SequenceEqual(current._files[name]))) throw new InvalidDataException("Authored audio snapshot is stale; prepare current sources again.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare owned worker/input copies against frozen caller bytes before any core loader evaluates source XML. */
    //-------------------------------------------------------------------------------------------------
    internal void VerifyCopies(string directory)
    { foreach (string name in Names) if (!_files[name].SequenceEqual(ReadFile(Path.Combine(directory,name),name == "input.wav" ? 24044 : 8192))) throw new InvalidDataException("Authored audio copy differs from frozen snapshot."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: install only these three files into a fresh empty owned directory, preserving original authored bytes/default attribution. */
    //-------------------------------------------------------------------------------------------------
    internal void Install(string directory)
    {
        if (Directory.Exists(directory) && Directory.EnumerateFileSystemEntries(directory).Any()) throw new InvalidDataException("Authored snapshot destination must be empty.");
        Directory.CreateDirectory(directory);
        foreach (string name in Names) { using FileStream stream = new(Path.Combine(directory,name),FileMode.CreateNew,FileAccess.Write); stream.Write(_files[name]); }
        VerifyCopies(directory);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate all external XML/PCM in the parent before launching a native worker; only existing narrow direct-file identities are admitted. */
    //-------------------------------------------------------------------------------------------------
    internal static AuthoredAudioSnapshot Read(string directory)
    {
        directory = Path.GetFullPath(directory);
        Dictionary<string,byte[]> files = Names.ToDictionary(name => name,name => ReadFile(Path.Combine(directory,name),name == "input.wav" ? 24044 : 8192));
        string schema = Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"AudioFileIdentityPipeline.xsd");
        // Reborn: collect validated names in source-slot order and reject aliases in the actual SAGE identity domain.
        List<string> identities = new();
        foreach (bool streamed in new[] { false,true })
        {
            string name = streamed ? "streamed.xml" : "ram.xml";
            // Reborn: current core/snapshot readers share a strict UTF-8 text boundary; reject BOM/other declared encodings explicitly.
            if (files[name].AsSpan().StartsWith(new byte[] { 0xEF,0xBB,0xBF })) throw new InvalidDataException("Authored XML must be UTF-8 without BOM.");
            string xml = new UTF8Encoding(false,true).GetString(files[name]); XmlDocument document = new() { XmlResolver = null };
            document.Schemas.XmlResolver = new XmlUrlResolver(); document.Schemas.Add("uri:ea.com:eala:asset",schema);
            using (XmlReader reader = XmlReader.Create(new StringReader(xml),new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit,XmlResolver = null })) document.Load(reader);
            if (document.FirstChild is XmlDeclaration declaration && declaration.Encoding.Length > 0 && !declaration.Encoding.Equals("utf-8",StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Authored XML declaration must use UTF-8.");
            document.Validate((_,args) => throw new InvalidDataException(args.Message));
            if (document.DocumentElement!.LocalName != "AssetDeclaration" || document.DocumentElement.ChildNodes.Count != 1 || document.DocumentElement.FirstChild is not XmlElement root
                || root.LocalName != "AudioFile" || root.GetAttribute("File") != "input.wav")
                throw new InvalidDataException("Authored audio requires one AudioFile and input.wav leaf per declaration.");
            AudioFileDiagnosticIdentity.Validate(root.GetAttribute("id")); identities.Add(root.GetAttribute("id"));
            var input = Ra3Ep1AudioFileInputProfile.Prepare(root,AudioEncoderPoc.Identity(root),TargetPlatform.Win32,files["input.wav"]);
            if (input.Streamed != streamed) throw new InvalidDataException("Authored audio play location disagrees with its RAM/streamed source slot.");
        }
        if (InstanceHandle.GetInstanceId(identities[0]) == InstanceHandle.GetInstanceId(identities[1]))
            throw new InvalidDataException("Authored audio identities collide in the SAGE instance-ID domain.");
        return new(directory,files) { RamName = identities[0],StreamName = identities[1] };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: read only bounded named files with same-handle length checks and no reparse ancestry; no recursive user-directory scan. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] ReadFile(string path,int limit)
    {
        for (string? current = path; current != null; current = Path.GetDirectoryName(current))
            if ((File.GetAttributes(current) & FileAttributes.ReparsePoint) != 0) throw new InvalidDataException("Authored audio does not admit reparse paths.");
        using FileStream stream = File.OpenRead(path); if (stream.Length > limit) throw new InvalidDataException("Authored audio input exceeds its bound.");
        byte[] bytes = new byte[(int)stream.Length]; stream.ReadExactly(bytes); if (stream.ReadByte() != -1) throw new InvalidDataException("Authored audio source changed length during reading."); return bytes;
    }
}
