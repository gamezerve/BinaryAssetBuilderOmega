using System.Buffers.Binary;
using System.Globalization;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: bridge a bounded actual core instance to authored PCM preparation without enabling a production AudioFile plugin.
internal sealed class AudioFileCorePreparation
{
    private readonly string _sourcePath,_authoredXml,_normalizedXml;
    private readonly uint _hash;
    private readonly Ra3Ep1AudioFileInputProfile.PreparedInput _input;
    private readonly XmlElement _root;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain immutable identity/source evidence and privately owned schema-bound authored input. */
    //-------------------------------------------------------------------------------------------------
    private AudioFileCorePreparation(string source,string xml,string normalized,uint hash,XmlElement root,Ra3Ep1AudioFileInputProfile.PreparedInput input)
    { _sourcePath = source; _authoredXml = xml; _normalizedXml = normalized; _hash = hash; _root = root; _input = input; }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: return detached PCM only; callers cannot alter frozen preparation through an encoder input alias. */
    //-------------------------------------------------------------------------------------------------
    internal byte[] CopyWave() => _input.CopyWave();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reread current source and dependency rather than trusting timestamp/length metadata or stale core hashes. */
    //-------------------------------------------------------------------------------------------------
    internal void VerifyCurrent(InstanceDeclaration instance)
    {
        AudioFileCorePreparation current = Prepare(instance);
        if (_sourcePath != current._sourcePath || _authoredXml != current._authoredXml || _normalizedXml != current._normalizedXml
            || _hash != current._hash || !_input.CopyWave().SequenceEqual(current._input.CopyWave()))
            throw new InvalidDataException("Core AudioFile preparation is stale.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: recheck current files/instance and serialize only the frozen narrow profile; caller headers still pass runtime validation. */
    //-------------------------------------------------------------------------------------------------
    internal AssetBuffer SerializeCurrent(InstanceDeclaration instance,ReadOnlySpan<byte> header)
    {
        VerifyCurrent(instance);
        return _input.SerializeCurrent(_root,instance.Handle,TargetPlatform.Win32,_input.CopyWave(),header);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: resolve only a direct owned WAV leaf, bind current disk/source XML to core identity, and preserve authored default attribution. */
    //-------------------------------------------------------------------------------------------------
    internal static AudioFileCorePreparation Prepare(InstanceDeclaration instance)
    {
        if (instance.XmlNode is not XmlElement normalized || instance.Document == null
            || instance.ProcessingHash != AudioFileIdentitySmokeTest.Processing || instance.InheritFromHandle != null
            || instance.ReferencedInstances.Count != 0 || instance.WeakReferencedInstances.Count != 0 || instance.ReferencedFiles.Count != 1)
            throw new InvalidDataException("Only the isolated direct-file core AudioFile profile is admitted.");
        string source = Path.GetFullPath(instance.Document.SourcePath);
        string authored = new UTF8Encoding(false,true).GetString(ReadBounded(source,8192));
        XmlDocument document = new() { XmlResolver = null };
        string schema = Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"AudioFileIdentityPipeline.xsd");
        document.Schemas.XmlResolver = new XmlUrlResolver(); document.Schemas.Add(SchemaSet.XmlNamespace,schema);
        // Reborn: prohibit DTD/entity expansion before even bounded authored XML is parsed.
        using (XmlReader reader = XmlReader.Create(new StringReader(authored),new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit,XmlResolver = null })) document.Load(reader);
        document.Validate((_,args) => throw new InvalidDataException(args.Message));
        if (document.DocumentElement!.ChildNodes.Count != 1 || document.DocumentElement.FirstChild is not XmlElement root)
            throw new InvalidDataException("Core audio bridge requires exactly one authored AudioFile and no Includes.");
        // Reborn: validate leaf/profile syntax before combining its value with a filesystem directory.
        using MemoryStream knownPcm = new(); AudioEncoderPoc.WriteWave(knownPcm);
        Ra3Ep1AudioFileInputProfile.Prepare(root,instance.Handle,TargetPlatform.Win32,knownPcm.ToArray());
        string leaf = root.GetAttribute("File");
        string file = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(source)!,leaf));
        if (instance.ReferencedFiles[0] != leaf.ToLowerInvariant() || normalized.GetAttribute("File") != file.ToLowerInvariant())
            throw new InvalidDataException("Core AudioFile logical/resolved file metadata differs.");
        byte[] wave = ReadBounded(file,24044);
        var input = Ra3Ep1AudioFileInputProfile.Prepare(root,instance.Handle,TargetPlatform.Win32,wave);
        uint hash = HashProvider.GetTextHash(instance.ProcessingHash,DocumentProcessor.Version.ToString(CultureInfo.InvariantCulture));
        using EncodedWriter text = new(); using (XmlWriter writer = XmlWriter.Create(text)) { root.WriteTo(writer); writer.Flush(); }
        string xml = text.ToString();
        for (int offset = 0; offset < xml.Length; offset += 512) hash = HashProvider.GetTextHash(hash,xml.Substring(offset,Math.Min(512,xml.Length-offset)));
        byte[] dependencies = new byte[256]; BinaryPrimitives.WriteUInt32LittleEndian(dependencies,FastHash.GetHashCode((uint)wave.Length,wave,wave.Length));
        hash ^= FastHash.GetHashCode(dependencies);
        if (instance.Handle.InstanceHash != hash) throw new InvalidDataException("Current WAV/XML does not match core AudioFile InstanceHash; rebuild current sources first.");
        // Reborn: expected normalized shape is generated from the same validated authored defaults, not from stale caller PSVI.
        root.SetAttribute("File",file.ToLowerInvariant()); root.SetAttribute("TypeId",FastHash.GetHashCode("AudioFile").ToString(CultureInfo.InvariantCulture));
        string expectedNormalized = root.OuterXml;
        root.SetAttribute("File",leaf); root.RemoveAttribute("TypeId");
        if (normalized.OuterXml != expectedNormalized) throw new InvalidDataException("Current core AudioFile XML differs from its disk source.");
        return new(source,authored,expectedNormalized,hash,root,input);
    }

    // Reborn: preserve the core text writer's declared serialization encoding.
    private sealed class EncodedWriter : StringWriter { public override Encoding Encoding => Encoding.Default; }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bound each same-handle read and reject reparse traversal; this is not a concurrent adversarial-filesystem guarantee. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] ReadBounded(string path,int limit)
    {
        for (string? current = path; current != null; current = Path.GetDirectoryName(current))
            if ((File.GetAttributes(current) & FileAttributes.ReparsePoint) != 0) throw new InvalidDataException("Core audio bridge does not admit reparse paths.");
        using FileStream stream = new(path,FileMode.Open,FileAccess.Read,FileShare.Read);
        if (stream.Length > limit) throw new InvalidDataException("Core audio bridge source exceeds its bound.");
        byte[] bytes = new byte[(int)stream.Length]; stream.ReadExactly(bytes);
        if (stream.ReadByte() != -1) throw new InvalidDataException("Core audio bridge source changed length during reading.");
        return bytes;
    }
}
