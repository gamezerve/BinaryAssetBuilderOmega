using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Xml;
using BinaryAssetBuilder.Core;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: reconcile authored event names with bounded stock words without synthesizing a missing Pathfinder header or enabling a processor.
internal static class PathMusicStockReview
{
    // Reborn: raw word observations and weak-reference correlations are not recovered header constants or native emission readiness.
    internal sealed record Row(string Name,uint InstanceId,uint Word0,uint Word4,uint Word8,uint Word12,uint? Word16,string? Alternate,string SliceSha256,string RelocationSha256);
    internal sealed record Report(string SourceSha256,string ManifestSha256,int Events,int AlternateReferences,int SelectedBinBytes,int SelectedRelocationBytes,Row[] Rows)
    {
        public bool ReadOnly => true;
        public bool SnapshotOnly => true;
        public bool HeaderRecovered => false;
        public bool ProcessorRecovered => false;
        public bool ProductionBuildReady => false;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: read only source/manifest metadata and exact 16/20-byte music slices, replay observations and refuse incompatible stream boundaries/checksums. */
    //-------------------------------------------------------------------------------------------------
    internal static Report Inspect(string sourcePath,string manifestPath)
    {
        sourcePath = Path.GetFullPath(sourcePath); manifestPath = Path.GetFullPath(manifestPath);
        byte[] source = SdkEnvironmentPreflight.Read(sourcePath,4*1048576),metadata = SdkEnvironmentPreflight.Read(manifestPath,16*1048576);
        ManifestDocument manifest = ManifestReader.Read(metadata);
        CheckStreams(manifest,manifestPath);
        // Reborn: the reconciliation core selects only exact bounded music records, not unrelated stream payloads.
        byte[] Read(string extension,long offset,int count) => AssetStreamProbe.ReadRange(Path.ChangeExtension(manifestPath,extension),null,offset,count);
        var first = Review(source,manifest,Read) with { ManifestSha256 = Hash(metadata) };
        var second = Review(source,manifest,Read);
        CheckStreams(manifest,manifestPath);
        if (!first.Rows.SequenceEqual(second.Rows) || Hash(SdkEnvironmentPreflight.Read(sourcePath,4*1048576)) != first.SourceSha256
            || Hash(SdkEnvironmentPreflight.Read(manifestPath,16*1048576)) != first.ManifestSha256)
            throw new InvalidDataException("Music source/manifest/selected slices changed during observation.");
        return first;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require exact name sets and EP1 identities, then correlate only zero base word, relocated weak alternate identity and stock true cache word. */
    //-------------------------------------------------------------------------------------------------
    internal static Report Review(byte[] source,ManifestDocument manifest,Func<string,long,int,byte[]> read)
    {
        TypeRegistryAudit.ValidateTarget(manifest.Header.Version,manifest.Header.AllTypesHash);
        if (!manifest.Header.IsLinked || manifest.Header.IsBigEndian || manifest.Validate().Count != 0 || manifest.ReferencedManifests.Any(item => item.IsPatch))
            throw new InvalidDataException("Non-patch valid linked little-endian EP1 manifest required.");
        XmlDocument xml = new() { XmlResolver = null };
        using (MemoryStream input = new(source,false))
        using (XmlReader reader = XmlReader.Create(input,new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit,XmlResolver = null,MaxCharactersInDocument = 4*1048576 })) xml.Load(reader);
        const string ea = "uri:ea.com:eala:asset";
        if (xml.DocumentElement?.LocalName != "AssetDeclaration" || xml.DocumentElement.NamespaceURI != ea) throw new InvalidDataException("EA source declaration required.");
        var owners = xml.DocumentElement.ChildNodes.OfType<XmlElement>().Where(node => node.LocalName == "PathMusicEvent").ToArray();
        if (owners.Length == 0 || owners.Length > 512 || owners.Any(node => node.NamespaceURI != ea || node.ChildNodes.OfType<XmlElement>().Any()
            || node.InnerText.Trim().Length != 0 || node.GetAttribute("inheritFrom") != "PathMusicEvent:BasePathMusicEvent"
            || node.Attributes.OfType<XmlAttribute>().Any(attribute => attribute.Name is not ("id" or "inheritFrom" or "RestartAlternateEvent"))))
            throw new InvalidDataException("Bounded literal stock-style music owner shape required; no general inheritance preparation.");
        var names = owners.Select(node => node.GetAttribute("id")).ToArray();
        if (names.Any(name => name.Length == 0 || name.Length > 256 || name.Any(character => !(char.IsAsciiLetterOrDigit(character) || character == '_')))
            || names.Distinct(StringComparer.Ordinal).Count() != names.Length) throw new InvalidDataException("Unique safe authored music names required.");
        var expected = owners.ToDictionary(node => "PathMusicEvent:"+node.GetAttribute("id"),StringComparer.Ordinal);
        var selected = manifest.Assets.Where(asset => asset.TypeName == "PathMusicEvent" || asset.TypeId == 0x9A651D89u).ToArray();
        if (selected.Length != owners.Length || selected.Select(asset => asset.Name).Distinct(StringComparer.Ordinal).Count() != selected.Length
            || selected.Any(asset => !expected.ContainsKey(asset.Name))) throw new InvalidDataException("Exact authored/stock music name sets required, not equal counts alone.");
        List<Row> rows = new(); long offset = manifest.Header.ContainerPrefixSize+4,reloOffset = offset;
        foreach (ManifestAsset asset in manifest.Assets)
        {
            if (expected.TryGetValue(asset.Name,out var owner))
            {
                string name = owner.GetAttribute("id"),alternate = owner.GetAttribute("RestartAlternateEvent");
                if (asset.TypeId != 0x9A651D89u || asset.TypeHash != 0x599CDAF2u || asset.Tokenized != 0 || asset.InstanceId != InstanceHandle.GetInstanceId(name)
                    || asset.InstanceDataSize != (alternate.Length == 0 ? 16 : 20) || asset.RelocationDataSize != (alternate.Length == 0 ? 0 : 8) || asset.ImportsDataSize != 0 || asset.References.Count != 0
                    || (alternate.Length != 0 && !names.Contains(alternate,StringComparer.Ordinal))) throw new InvalidDataException($"Stock music identity/shape/alternate closure differs: {asset.Name}, type={asset.TypeId:X8}/{asset.TypeHash:X8}, id={asset.InstanceId:X8}/{InstanceHandle.GetInstanceId(name):X8}, token={asset.Tokenized}, sizes={asset.InstanceDataSize}/{asset.RelocationDataSize}/{asset.ImportsDataSize}, refs={asset.References.Count}.");
                byte[] bytes = read(".bin",offset,asset.InstanceDataSize),relocations = asset.RelocationDataSize == 0 ? Array.Empty<byte>() : read(".relo",reloOffset,8);
                if (bytes.Length != asset.InstanceDataSize || relocations.Length != asset.RelocationDataSize) throw new InvalidDataException("Complete bounded music slices required.");
                uint word0 = Word(bytes,0),word4 = Word(bytes,4),word8 = Word(bytes,8),word12 = Word(bytes,12);
                uint? word16 = alternate.Length == 0 ? null : Word(bytes,16);
                if (word0 != 0 || word12 != 1 || word8 != (alternate.Length == 0 ? 0u : 16u)
                    || (alternate.Length != 0 && (word16 != InstanceHandle.GetInstanceId(alternate) || Word(relocations,0) != 8 || Word(relocations,4) != uint.MaxValue)))
                    throw new InvalidDataException("Observed music base/cache/weak-reference correlation differs.");
                rows.Add(new(name,asset.InstanceId,word0,word4,word8,word12,word16,alternate.Length == 0 ? null : alternate,Hash(bytes),Hash(relocations)));
            }
            offset = checked(offset+asset.InstanceDataSize);
            reloOffset = checked(reloOffset+asset.RelocationDataSize);
        }
        int alternates = rows.Count(row => row.Alternate != null);
        return new(Hash(source),"",rows.Count,alternates,rows.Count*16+alternates*4,alternates*8,rows.ToArray());
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate all three stream lengths and framing checksums without reading unrelated binary payloads or decoding audio. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckStreams(ManifestDocument manifest,string path)
    {
        TypeRegistryAudit.ValidateTarget(manifest.Header.Version,manifest.Header.AllTypesHash);
        if (!manifest.Header.IsLinked || manifest.Validate().Count != 0 || manifest.ReferencedManifests.Any(item => item.IsPatch)) throw new InvalidDataException("Valid non-patch linked EP1 streams required.");
        long prefix = manifest.Header.ContainerPrefixSize+4;
        foreach (var stream in new[] { (Extension:".bin",Bytes:manifest.Assets.Sum(asset => (long)asset.InstanceDataSize)),
            (Extension:".relo",Bytes:manifest.Assets.Sum(asset => (long)asset.RelocationDataSize)),(Extension:".imp",Bytes:manifest.Assets.Sum(asset => (long)asset.ImportsDataSize)) })
        {
            string file = Path.ChangeExtension(path,stream.Extension); SdkEnvironmentPreflight.CheckPath(file);
            if (new FileInfo(file).Length != checked(prefix+stream.Bytes)
                || Word(AssetStreamProbe.ReadRange(file,null,manifest.Header.ContainerPrefixSize,4),0) != manifest.Header.StreamChecksum)
                throw new InvalidDataException("Music stream length/checksum framing differs.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve raw little-endian observations without naming an unproved Pathfinder hash algorithm. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] bytes,int offset) => BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(offset,4));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: snapshot hashes identify reviewed bytes, not complete audio/game compatibility. */
    //-------------------------------------------------------------------------------------------------
    private static string Hash(byte[] bytes) => Convert.ToHexString(SHA256.HashData(bytes));
}
