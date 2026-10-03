using System.Buffers.Binary;
using System.Globalization;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: reconstruct managed AudioFile identity from official XML and owned WAV bytes without compiling audio or publishing streams.
internal static class AudioFileIdentitySmokeTest
{
    // Reborn: keep this test-only processor domain distinct from the observed reference Win32 ProcessingHash.
    internal const uint Processing = 0x52424849u;
    private const string Source = "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\"><AudioFile id=\"RebornIdentity\" File=\"input.wav\" PCSampleRate=\"48000\" PCCompression=\"XAS\" IsStreamedOnPC=\"false\" /></AssetDeclaration>";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare actual core identities with separately validated pre-normalization XML and padded file-hash folding. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        string directory = Path.Combine(Path.GetTempPath(),"Reborn-AudioIdentity-"+Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        using MemoryStream pcm = new(); AudioEncoderPoc.WriteWave(pcm); byte[] wave = pcm.ToArray();
        string schema = Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"AudioFileIdentityPipeline.xsd");
        uint baseline = Check("baseline",Source,wave,Processing);
        Require(Check("repeat",Source,wave,Processing) == baseline,"Identity depends on the resolved absolute fixture directory.");
        byte[] changed = (byte[])wave.Clone(); changed[100] ^= 1;
        Require(Check("sample-change",Source,changed,Processing) != baseline,"Changed PCM content did not affect identity.");
        Require(Check("xml-change",Source.Replace("false","true"),wave,Processing) != baseline,"Changed XML setting did not affect identity.");
        Require(Check("processor-change",Source,wave,Processing+1) != baseline,"Changed processor domain did not affect identity.");
        Require(Check("path-spelling",Source.Replace("input.wav","INPUT.WAV"),wave,Processing) != baseline,"Authored file spelling disappeared before XML hashing.");
        // Reborn: file hashing accepts arbitrary dependency bytes; codec/input-profile validation remains a separate gate.
        Require(Check("length-change",Source,wave.Concat(new byte[] { 0 }).ToArray(),Processing) != baseline,"Dependency length change did not affect identity.");
        Check("empty-dependency",Source,Array.Empty<byte>(),Processing);
        string missing = Path.Combine(directory,"missing"); Directory.CreateDirectory(missing);
        WriteNew(Path.Combine(missing,"audio.xml"),Encoding.UTF8.GetBytes(Source));
        bool rejected = false;
        try { Build(missing,schema,Processing); }
        catch (BinaryAssetBuilderException exception) when (exception.ErrorCode == ErrorCode.FileNotFound) { rejected = true; }
        Require(rejected,"Missing WAV dependency was not rejected as FileNotFound.");
        WriteNew(Path.Combine(missing,"input.wav"),wave);
        Require(Build(missing,schema,Processing).Handle.InstanceHash == baseline,"Fresh missing-file recovery differs from baseline.");
        uint referenceContext = Expected(Source,schema,wave,0x8FE79286u,11,out _,out _);
        Require(referenceContext != baseline,"Reference and experimental contexts unexpectedly collide.");
        Console.WriteLine($"AudioFile identity self-test: OK (core={baseline:X8}, offline RA3 reference-context={referenceContext:X8}; XML/defaults, file content/length, 256-byte dependency padding, missing/recovery; no codec/output/stock-match claim)");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: use fresh exclusive owned files and cache per case, then inspect actual normalized core metadata. */
        //-------------------------------------------------------------------------------------------------
        uint Check(string name,string xml,byte[] bytes,uint processing)
        {
            string path = Path.Combine(directory,name); Directory.CreateDirectory(path);
            WriteNew(Path.Combine(path,"audio.xml"),Encoding.UTF8.GetBytes(xml)); WriteNew(Path.Combine(path,"input.wav"),bytes);
            uint expected = Expected(xml,schema,bytes,processing,DocumentProcessor.Version,out uint xmlHash,out uint fileHash);
            InstanceDeclaration instance = Build(path,schema,processing);
            Require(instance.Handle.InstanceHash == expected,$"Core AudioFile identity differs for {name}: expected {expected:X8}, actual {instance.Handle.InstanceHash:X8}.");
            Require(instance.Handle.TypeId == 0x166B084Du && instance.Handle.TypeHash == 0x53C81E47u && instance.ProcessingHash == processing
                && instance.Handle.InstanceId == InstanceHandle.GetInstanceId("RebornIdentity")
                && instance.ReferencedFiles.Single() == "input.wav" && instance.ReferencedInstances.Count == 0 && instance.WeakReferencedInstances.Count == 0,
                "Core AudioFile identity/dependency metadata differs.");
            Require(((XmlElement)instance.XmlNode).GetAttribute("File") == Path.Combine(path,"input.wav").ToLowerInvariant(),"Core file reference did not normalize to the resolved absolute path.");
            byte[] shortBuffer = new byte[4]; BinaryPrimitives.WriteUInt32LittleEndian(shortBuffer,fileHash);
            Require(expected != (xmlHash ^ FastHash.GetHashCode(shortBuffer)),"Four-byte dependency data unexpectedly equals capacity-padded identity.");
            return expected;
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently capture official defaults before core TypeId/path mutation and fold text plus a fixed padded dependency buffer. */
    //-------------------------------------------------------------------------------------------------
    private static uint Expected(string source,string schema,byte[] wave,uint processing,uint version,out uint xmlHash,out uint fileHash)
    {
        XmlDocument document = new() { XmlResolver = null };
        document.Schemas.XmlResolver = new XmlUrlResolver(); document.Schemas.Add(SchemaSet.XmlNamespace,schema);
        document.LoadXml(source); document.Validate((_,args) => throw new InvalidDataException(args.Message));
        XmlElement root = (XmlElement)document.DocumentElement!.FirstChild!;
        Require(root.HasAttribute("PCQuality") && root.GetAttribute("PCQuality") == "75" && !root.HasAttribute("TypeId"),"Official pre-normalization defaults differ.");
        using EncodedWriter text = new(); using (XmlWriter writer = XmlWriter.Create(text)) { root.WriteTo(writer); writer.Flush(); }
        string serialized = text.ToString(); xmlHash = HashProvider.GetTextHash(processing,version.ToString(CultureInfo.InvariantCulture));
        for (int offset = 0; offset < serialized.Length; offset += 512)
            xmlHash = HashProvider.GetTextHash(xmlHash,serialized.Substring(offset,Math.Min(512,serialized.Length-offset)));
        fileHash = (uint)wave.Length;
        // Reborn: these bounded WAV fixtures fit one actual asynchronous reader chunk; empty input preserves the file-size seed.
        Require(wave.Length < 1024*1024,"Identity fixture exceeds the independently reconstructed single-chunk limit.");
        if (wave.Length > 0) fileHash = FastHash.GetHashCode(fileHash,wave,wave.Length);
        byte[] dependencyCapacity = new byte[256]; BinaryPrimitives.WriteUInt32LittleEndian(dependencyCapacity,fileHash);
        return xmlHash ^ FastHash.GetHashCode(dependencyCapacity);
    }

    // Reborn: match the core writer's declared encoding while capturing text without using its block-folding implementation.
    private sealed class EncodedWriter : StringWriter { public override Encoding Encoding => Encoding.Default; }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: register hash metadata locally, parse through the real DocumentProcessor, restore global settings even after errors. */
    //-------------------------------------------------------------------------------------------------
    internal static InstanceDeclaration Build(string directory,string schema,uint processing,string fileName = "audio.xml")
    {
        Settings saved = Settings.Current;
        try
        {
            Settings.Current = new Settings { BuildCache = false,SchemaPath = schema,DataRoot = directory,DataPaths = new[] { directory },
                TargetPlatform = TargetPlatform.Win32,CustomPostfix = "",StreamPostfix = "",StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            PluginRegistry plugins = new(Array.Empty<PluginDescriptor>(),TargetPlatform.Win32); plugins.AddPlugin(0x166B084Du,new IdentityOnlyPlugin(processing));
            // Reborn: assert that test-only metadata cannot authorize production output or compiled-document reuse.
            bool outputBlocked = false;
            try { plugins.ValidateProductionOutput(); }
            catch (BinaryAssetBuilderException exception) when (exception.ErrorCode == ErrorCode.InternalError
                && exception.Message.Contains("Reborn-HashOnly-AudioFile",StringComparison.Ordinal)) { outputBlocked = true; }
            Require(outputBlocked && !plugins.CanReuseCompiledDocuments,"Hash-only processor unexpectedly authorizes production/reuse.");
            SessionCache cache = new(); cache.InitializeCache(new List<string>());
            DocumentProcessor processor = new(Settings.Current,plugins,new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(),TargetPlatform.Win32)) { Cache = cache,SchemaSet = new SchemaSet(false) };
            string source = Path.Combine(directory,fileName);
            return processor.ProcessDocumentInternal(source,source,null!,new DocumentProcessor.ProcessOptions { GenerateOutput = false }).SelfInstances.Single();
        }
        finally { Settings.Current = saved; }
    }

    // Reborn: private metadata-only processor never authorizes cache reuse, production streams or native compilation.
    private sealed class IdentityOnlyPlugin(uint processing) : IAssetBuilderPlugin,IAssetBuilderOutputPolicy
    {
        public string ProfileName => "Reborn-HashOnly-AudioFile";
        public bool CanWriteProductionOutput => false;
        public bool CanUseBuildCache => false;
        public bool CanReuseCompiledDocuments => false;
        public uint AllTypesHash => 0x5454A8E9u;
        public uint VersionNumber => 1;
        //-------------------------------------------------------------------------------------------------
        /** Reborn: keep this metadata experiment on the only inspected target platform. */
        //-------------------------------------------------------------------------------------------------
        public void Initialize(TargetPlatform platform) { if (platform != TargetPlatform.Win32) throw new NotSupportedException(); }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: preserve the same platform boundary on repeated initialization. */
        //-------------------------------------------------------------------------------------------------
        public void ReInitialize(TargetPlatform platform) => Initialize(platform);
        //-------------------------------------------------------------------------------------------------
        /** Reborn: provide only bounded AudioFile identity metadata, not a production processor or serializer. */
        //-------------------------------------------------------------------------------------------------
        public ExtendedTypeInformation GetExtendedTypeInformation(uint typeId)
        {
            if (typeId != 0x166B084Du) throw new NotSupportedException();
            return new() { TypeId = typeId,TypeName = "AudioFile",TypeHash = 0x53C81E47u,ProcessingHash = processing,HasCustomData = true,UseBuildCache = false };
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: any accidental attempt to compile audio through this hash-only fixture must fail. */
        //-------------------------------------------------------------------------------------------------
        public AssetBuffer ProcessInstance(InstanceDeclaration instance) => throw new NotSupportedException("Hash-only AudioFile fixture cannot compile audio.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: create only new owned fixtures, never overwrite an existing source or game asset. */
    //-------------------------------------------------------------------------------------------------
    private static void WriteNew(string path,byte[] bytes) { using FileStream stream = new(path,FileMode.CreateNew,FileAccess.Write); stream.Write(bytes); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject the first mismatch without treating a diagnostic reconstruction as stock proof. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
