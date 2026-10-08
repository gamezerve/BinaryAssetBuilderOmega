using System.Buffers.Binary;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: reconstruct actual Core music identities under an explicit synthetic processing domain, without compiling or relabeling diagnostic packages.
internal static class PathMusicCoreIdentity
{
    // Reborn: this metadata-only processing seed is intentionally not asserted to be the EA audio compiler's ProcessingHash.
    internal const uint Processing = 0x52424D49u;
    internal sealed record Row(string Name,uint InstanceId,uint CoreInstanceHash,uint ExpectedInstanceHash,uint XmlHash,uint HeaderFileHash,uint DependencyHash,int WeakReferences,string NormalizedHeader,bool Cacheable,bool CacheAttributeSpecified,bool SerializedCacheAttribute);
    internal sealed record Report(uint ProcessingHash,uint DocumentVersion,string SourceSha256,string HeaderSha256,string DiagnosticFingerprint,Row[] Rows)
    {
        public bool ReadOnly => true;
        public bool SyntheticProcessingDomain => true;
        public bool ReferenceProcessingHashRecovered => false;
        public bool ProductionBuildReady => false;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare actual fresh Core instances against independently defaulted XML and padded dependency folding, checking current raw/native snapshots. */
    //-------------------------------------------------------------------------------------------------
    internal static Report Inspect(string directory,XmlSchemaSet? reviewedSchemas = null,uint processing = Processing)
    {
        directory = Path.GetFullPath(directory); var snapshot = PathMusicAuthoredSnapshot.Read(directory,reviewedSchemas); var frozen = snapshot.Preflight();
        byte[] source = SdkEnvironmentPreflight.Read(Path.Combine(directory,"events.xml"),32768),header = SdkEnvironmentPreflight.Read(Path.Combine(directory,"events.h"),1048576);
        // Reborn: independently folded headers must fit one Core reader chunk; this narrower diagnostic gate does not widen general source admission.
        if (header.Length >= 1048576) throw new InvalidDataException("Music identity proof requires a header below one MiB.");
        string schema = Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"PathMusicIdentityPipeline.xsd");
        XmlDocument authored = new() { XmlResolver = null }; authored.Schemas.XmlResolver = new XmlUrlResolver(); authored.Schemas.Add(SchemaSet.XmlNamespace,schema);
        using (XmlReader reader = XmlReader.Create(new StringReader(new UTF8Encoding(false,true).GetString(source)),new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit,XmlResolver = null })) authored.Load(reader);
        authored.Validate((_,args) => { if (args.Severity == XmlSeverityType.Error) throw new InvalidDataException(args.Message); });
        XmlElement[] roots = authored.DocumentElement!.ChildNodes.OfType<XmlElement>().ToArray();
        InstanceDeclaration[] instances = Build(directory,schema,processing);
        if (instances.Length != roots.Length || roots.Length != frozen.Rows.Length) throw new InvalidDataException("Music Core owner count differs.");
        uint fileHash = FastHash.GetHashCode((uint)header.Length,header,header.Length);
        Row[] rows = new Row[roots.Length];
        for (int index = 0; index < roots.Length; index++)
        {
            XmlElement root = roots[index]; InstanceDeclaration instance = instances[index]; var prepared = frozen.Rows[index];
            uint xmlHash = HashProvider.GetTextHash(processing,DocumentProcessor.Version.ToString(CultureInfo.InvariantCulture));
            using EncodedWriter text = new(); using (XmlWriter writer = XmlWriter.Create(text)) { root.WriteTo(writer); writer.Flush(); }
            string serialized = text.ToString();
            for (int offset = 0; offset < serialized.Length; offset += 512) xmlHash = HashProvider.GetTextHash(xmlHash,serialized.Substring(offset,Math.Min(512,serialized.Length-offset)));
            // Reborn: weak refs contribute their schema target TYPE word before file hashes, not strong import indices or target instance hashes.
            byte[] dependencies = new byte[256]; int fileOffset = 0;
            if (prepared.Alternate != null) { BinaryPrimitives.WriteUInt32LittleEndian(dependencies,0x9A651D89u); fileOffset = 4; }
            BinaryPrimitives.WriteUInt32LittleEndian(dependencies.AsSpan(fileOffset),fileHash); uint dependencyHash = FastHash.GetHashCode(dependencies),expected = xmlHash ^ dependencyHash;
            string physical = Path.Combine(directory,"events.h").ToLowerInvariant();
            if (instance.Handle.TypeId != 0x9A651D89u || instance.Handle.TypeHash != 0 || instance.Handle.InstanceId != prepared.InstanceId
                || instance.Handle.InstanceName != prepared.Name || instance.ProcessingHash != processing || instance.Handle.InstanceHash != expected
                || instance.InheritFromHandle != null || instance.ReferencedInstances.Count != 0 || instance.ReferencedFiles.Count != 1 || instance.ReferencedFiles[0] != "events.h"
                || instance.WeakReferencedInstances.Count != (prepared.Alternate == null ? 0 : 1)) throw new InvalidDataException("Actual Core music identity/dependencies differ from independent folding.");
            if (prepared.Alternate != null && (instance.WeakReferencedInstances[0].TypeId != 0x9A651D89u
                || instance.WeakReferencedInstances[0].InstanceId != InstanceHandle.GetInstanceId(prepared.Alternate))) throw new InvalidDataException("Core weak music identity differs.");
            root.SetAttribute("PathfinderEventHeader",physical); root.SetAttribute("TypeId",0x9A651D89u.ToString(CultureInfo.InvariantCulture));
            if (instance.XmlNode.OuterXml != root.OuterXml) throw new InvalidDataException("Core normalized music XML/default/path/TypeId differs.");
            rows[index] = new(prepared.Name,prepared.InstanceId,instance.Handle.InstanceHash,expected,xmlHash,fileHash,dependencyHash,instance.WeakReferencedInstances.Count,physical,
                prepared.Cacheable,root.GetAttributeNode("IsCacheable")!.Specified,serialized.Contains("IsCacheable=",StringComparison.Ordinal));
        }
        snapshot.Compile(); snapshot.VerifyCurrent();
        if (!source.AsSpan().SequenceEqual(SdkEnvironmentPreflight.Read(Path.Combine(directory,"events.xml"),32768))
            || !header.AsSpan().SequenceEqual(SdkEnvironmentPreflight.Read(Path.Combine(directory,"events.h"),1048576))) throw new InvalidDataException("Music identity inputs changed during proof.");
        return new(processing,DocumentProcessor.Version,frozen.SourceSha256,frozen.HeaderSha256,frozen.DiagnosticFingerprint,rows);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: use a fresh cache and private metadata-only plugin, restore all global settings and prohibit production output/reuse. */
    //-------------------------------------------------------------------------------------------------
    private static InstanceDeclaration[] Build(string directory,string schema,uint processing)
    {
        Settings saved = Settings.Current;
        try
        {
            Settings.Current = new Settings { BuildCache = false,SchemaPath = schema,DataRoot = directory,DataPaths = new[] { directory },TargetPlatform = TargetPlatform.Win32,
                CustomPostfix = "",StreamPostfix = "",StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            PluginRegistry plugins = new(Array.Empty<PluginDescriptor>(),TargetPlatform.Win32); plugins.AddPlugin(0x9A651D89u,new IdentityOnlyPlugin(processing));
            bool blocked = false; try { plugins.ValidateProductionOutput(); }
            catch (BinaryAssetBuilderException error) when (error.ErrorCode == ErrorCode.InternalError && error.Message.Contains("Reborn-HashOnly-PathMusic",StringComparison.Ordinal)) { blocked = true; }
            if (!blocked || plugins.CanReuseCompiledDocuments) throw new InvalidDataException("Music hash-only plugin enabled production/reuse.");
            SessionCache cache = new(); cache.InitializeCache(new List<string>());
            DocumentProcessor processor = new(Settings.Current,plugins,new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(),TargetPlatform.Win32)) { Cache = cache,SchemaSet = new SchemaSet(false) };
            string path = Path.Combine(directory,"events.xml");
            return processor.ProcessDocumentInternal(path,path,null!,new DocumentProcessor.ProcessOptions { GenerateOutput = false }).SelfInstances.ToArray();
        }
        finally { Settings.Current = saved; }
    }

    // Reborn: match Core's declared XML writer encoding without invoking its XML hash/block implementation.
    private sealed class EncodedWriter : StringWriter { public override Encoding Encoding => Encoding.Default; }

    // Reborn: metadata exists only in this local proof; no caller can obtain this plugin for production registration.
    private sealed class IdentityOnlyPlugin(uint processing) : IAssetBuilderPlugin,IAssetBuilderOutputPolicy
    {
        public string ProfileName => "Reborn-HashOnly-PathMusic";
        public bool CanWriteProductionOutput => false;
        public bool CanUseBuildCache => false;
        public bool CanReuseCompiledDocuments => false;
        public uint AllTypesHash => 0;
        public uint VersionNumber => 1;
        //-------------------------------------------------------------------------------------------------
        /** Reborn: only the inspected Win32 target participates in this metadata proof. */
        //-------------------------------------------------------------------------------------------------
        public void Initialize(TargetPlatform platform) { if (platform != TargetPlatform.Win32) throw new NotSupportedException(); }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: retain platform refusal on repeated initialization. */
        //-------------------------------------------------------------------------------------------------
        public void ReInitialize(TargetPlatform platform) => Initialize(platform);
        //-------------------------------------------------------------------------------------------------
        /** Reborn: expose synthetic processing metadata with zero type hash, never recovered EA processing metadata. */
        //-------------------------------------------------------------------------------------------------
        public ExtendedTypeInformation GetExtendedTypeInformation(uint typeId)
        {
            if (typeId != 0x9A651D89u) throw new NotSupportedException();
            return new() { TypeId = typeId,TypeName = "PathMusicEvent",TypeHash = 0,ProcessingHash = processing,HasCustomData = false,UseBuildCache = false };
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: accidental native processing through this hash-only registration must always fail. */
        //-------------------------------------------------------------------------------------------------
        public AssetBuffer ProcessInstance(InstanceDeclaration instance) => throw new NotSupportedException("Hash-only music proof cannot compile.");
    }
}
