using System.Reflection;
using System.Buffers.Binary;
using System.Text;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: dispatch only privately paired local music owners through a registered Core plugin in memory, keeping production output and cache closed.
internal static class PathMusicControlledCompiler
{
    // Reborn: local v2 metadata fingerprints the explicitly reviewed native contract; this recipe is neither an EA schema hash nor a temporary selection sentinel.
    internal const string LocalTypeContract = "Reborn-PathMusic-LocalType-v2\nWin32;root=16;base@0=0;event@4=canonical-nonzero-int32;weak-pointer@8=16-or-null;cache-byte@12;zero-padding@13..15;alternate=uint32-local-SAGE-ID;relo=8,FFFFFFFF;imports=none\n";
    internal static uint LocalTypeHash => BinaryPrimitives.ReadUInt32LittleEndian(SHA256.HashData(Encoding.UTF8.GetBytes(LocalTypeContract)));
    // Reborn: detached native evidence records successful private plugin dispatch, not stock EP1 processor or game compatibility.
    internal sealed record Row(string Name,uint InstanceId,uint InstanceHash,int BinBytes,int RelocationBytes,int ImportsBytes,string BinSha256,string RelocationSha256);
    internal sealed record Report(uint ProcessingHash,uint DocumentVersion,uint Checksum,int ProcessorCalls,Row[] Rows,uint TypeHash = 0)
    {
        public bool SyntheticProcessingDomain => true;
        public bool ProductionBuildReady => false;
        public bool CanUseBuildCache => false;
        public bool CanReuseCompiledDocuments => false;
        public bool GameLoadProved => false;
        // Reborn: v1 retains zero-hash exclusion; explicit local v2 invokes real selection without pretending that its type identity is stock.
        public bool CoreOutputSelectionSkipped => TypeHash == 0;
        public bool CoreDependenciesPrepared => TypeHash != 0;
        public bool SyntheticTypeDomain => TypeHash != 0;
        public bool Ep1ProcessingHashRecovered => false;
        public int LocalProfileVersion => TypeHash == 0 ? 1 : 2;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prepare fresh Core XML, prove zero-TypeHash output exclusion, admit private local closure and dispatch memory-only native buffers. */
    //-------------------------------------------------------------------------------------------------
    internal static Report Compile(string directory,Action<IAssetBuilderPlugin,InstanceDeclaration[]>? beforeCompile = null)
    { return Compile(directory,false,null,beforeCompile); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: explicitly opt into declared local v2 metadata and actual Core selection; test faults cannot supply an arbitrary type/processing hash. */
    //-------------------------------------------------------------------------------------------------
    internal static Report CompileSelected(string directory,Action<IAssetBuilderPlugin,InstanceDeclaration[]>? beforeSelection = null,Action<IAssetBuilderPlugin,InstanceDeclaration[]>? beforeCompile = null)
    { return Compile(directory,true,beforeSelection,beforeCompile); }

    // Reborn: actual compiled buffers are detached payload evidence, never a public authority to select arbitrary manifest hashes.
    internal sealed record Payload(Report Report,Relo.Chunk[] Chunks);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: capture only freshly selected and independently checked actual plugin buffers, cloning each array before the private compiler lifetime ends. */
    //-------------------------------------------------------------------------------------------------
    internal static Payload CompileSelectedPayload(string directory)
    {
        Payload? payload = null;
        Compile(directory,true,null,null,(report,buffers) => payload = new(report,buffers.Select(buffer => new Relo.Chunk {
            InstanceBuffer = (byte[])buffer.InstanceData.Clone(),RelocationBuffer = (byte[])buffer.RelocationData.Clone(),ImportsBuffer = (byte[])buffer.ImportsData.Clone() }).ToArray()));
        return payload ?? throw new InvalidDataException("Selected music payload was not captured.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: share private native containment while keeping zero-hash v1 and declared nonzero local v2 metadata paths distinct from creation onward. */
    //-------------------------------------------------------------------------------------------------
    private static Report Compile(string directory,bool selected,Action<IAssetBuilderPlugin,InstanceDeclaration[]>? beforeSelection,Action<IAssetBuilderPlugin,InstanceDeclaration[]>? beforeCompile,Action<Report,AssetBuffer[]>? onCompiled = null)
    {
        directory = Path.GetFullPath(directory); var preparation = PathMusicCorePreparation.Read(directory);
        var evidence = preparation.Preflight(); var expected = preparation.Compile(); Settings saved = Settings.Current;
        try
        {
            string schema = Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"PathMusicIdentityPipeline.xsd");
            Settings.Current = new Settings { BuildCache = false,SchemaPath = schema,DataRoot = directory,DataPaths = new[] { directory },
                TargetPlatform = TargetPlatform.Win32,ErrorLevel = selected ? 1 : 0,CustomPostfix = "",StreamPostfix = "",StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            LocalPlugin plugin = new(preparation,directory,selected); plugin.Initialize(TargetPlatform.Win32);
            PluginRegistry plugins = new(Array.Empty<PluginDescriptor>(),TargetPlatform.Win32); plugins.AddPlugin(0x9A651D89u,plugin);
            RefuseProduction(plugins.ValidateProductionOutput);
            if (plugins.CanReuseCompiledDocuments) throw new InvalidDataException("Controlled music enabled document reuse.");
            SessionCache cache = new(); cache.InitializeCache(new List<string>());
            DocumentProcessor processor = new(Settings.Current,plugins,new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(),TargetPlatform.Win32))
                { Cache = cache,SchemaSet = new SchemaSet(false) };
            // Reborn: GenerateOutput=true must refuse before file lookup; this path never reaches OutputManager or production commit.
            RefuseProduction(() => processor.ProcessDocumentInternal("Reborn-missing","Reborn-missing",null!,new DocumentProcessor.ProcessOptions { GenerateOutput = true }));
            string path = Path.Combine(directory,"events.xml");
            AssetDeclarationDocument document = processor.ProcessDocumentInternal(path,path,null!,new DocumentProcessor.ProcessOptions { GenerateOutput = false });
            InstanceDeclaration[] instances = document.SelfInstances.ToArray();
            if (instances.Length != evidence.Rows.Length) throw new InvalidDataException("Controlled music owner count differs.");
            if (selected)
            {
                beforeSelection?.Invoke(plugin,instances); PrepareSelected(document,instances);
            }
            else foreach (var instance in instances) ProveOutputSelectionSkipped(document,instance);
            plugin.Admit(instances); preparation.VerifyCurrent();
            // Reborn: owned tests may mutate prepared objects but cannot remove any plugin freshness/identity/dependency check.
            beforeCompile?.Invoke(plugin,instances);
            Row[] rows = new Row[instances.Length]; AssetBuffer[] buffers = new AssetBuffer[instances.Length];
            for (int index = 0; index < instances.Length; index++)
            {
                var instance = instances[index]; AssetBuffer actual = plugins.GetPlugin(instance.Handle.TypeId).ProcessInstance(instance);
                buffers[index] = actual;
                if (!actual.InstanceData.AsSpan().SequenceEqual(expected[index].InstanceBuffer)
                    || !actual.RelocationData.AsSpan().SequenceEqual(expected[index].RelocationBuffer)
                    || !actual.ImportsData.AsSpan().SequenceEqual(expected[index].ImportsBuffer)) throw new InvalidDataException("Controlled music differs from frozen native chunks.");
                rows[index] = new(instance.Handle.InstanceName,instance.Handle.InstanceId,instance.Handle.InstanceHash,actual.InstanceData.Length,
                    actual.RelocationData.Length,actual.ImportsData.Length,Hash(actual.InstanceData),Hash(actual.RelocationData));
            }
            uint checksum = PathMusicCoreChecksum.Core(instances);
            // Reborn: independent expected identities use declared metadata before checksum packing, not hashes edited on the actual prepared Core owners.
            InstanceDeclaration[] expectedIdentities = evidence.Rows.Select(row => PathMusicCoreChecksum.Identity(row.Name,row.CoreInstanceHash)).ToArray();
            foreach (var identity in expectedIdentities) identity.Handle.TypeHash = plugin.TypeHash;
            if (checksum != PathMusicCoreChecksum.Independent(instances) || checksum != PathMusicCoreChecksum.Independent(expectedIdentities))
                throw new InvalidDataException("Controlled music identity checksum differs.");
            preparation.VerifyCurrent(); Report result = new(evidence.ProcessingHash,evidence.DocumentVersion,checksum,plugin.Calls,rows,plugin.TypeHash);
            onCompiled?.Invoke(result,buffers); preparation.VerifyCurrent(); return result;
        }
        finally { Settings.Current = saved; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: select all real Core owners through unchanged AddOutputInstance, requiring exact membership and actual completed zero-strong dependency tables. */
    //-------------------------------------------------------------------------------------------------
    private static void PrepareSelected(AssetDeclarationDocument document,InstanceDeclaration[] instances)
    {
        object state = typeof(AssetDeclarationDocument).GetField("_current",BindingFlags.NonPublic|BindingFlags.Instance)!.GetValue(document)!;
        state.GetType().GetField("OutputInstanceSet")!.SetValue(state,new SortedDictionary<InstanceHandle,InstanceDeclaration>());
        var method = typeof(AssetDeclarationDocument).GetMethod("AddOutputInstance",BindingFlags.NonPublic|BindingFlags.Instance)!;
        foreach (var instance in instances) Invoke(method,document,instance);
        var output = (SortedDictionary<InstanceHandle,InstanceDeclaration>)state.GetType().GetField("OutputInstanceSet")!.GetValue(state)!;
        if (output.Count != instances.Length || instances.Any(instance => instance.Handle.TypeHash != LocalTypeHash
            || !output.TryGetValue(instance.Handle,out var admitted) || !ReferenceEquals(admitted,instance)
            || instance.ValidatedReferencedInstances == null || instance.ValidatedReferencedInstances.Count != 0
            || instance.AllDependentInstances == null || instance.AllDependentInstances.Count != 0))
            throw new InvalidDataException("Local v2 music Core selection/dependency preparation differs.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: pin the existing zero-TypeHash early return without substituting a nonzero hash or manufacturing validated Core dependency tables. */
    //-------------------------------------------------------------------------------------------------
    private static void ProveOutputSelectionSkipped(AssetDeclarationDocument document,InstanceDeclaration instance)
    {
        object state = typeof(AssetDeclarationDocument).GetField("_current",BindingFlags.NonPublic|BindingFlags.Instance)!.GetValue(document)!;
        state.GetType().GetField("OutputInstanceSet")!.SetValue(state,new SortedDictionary<InstanceHandle,InstanceDeclaration>());
        Invoke(typeof(AssetDeclarationDocument).GetMethod("AddOutputInstance",BindingFlags.NonPublic|BindingFlags.Instance)!,document,instance);
        var selected = (SortedDictionary<InstanceHandle,InstanceDeclaration>)state.GetType().GetField("OutputInstanceSet")!.GetValue(state)!;
        if (instance.Handle.TypeHash != 0 || selected.Count != 0 || instance.ValidatedReferencedInstances != null || instance.AllDependentInstances != null)
            throw new InvalidDataException("Zero-TypeHash music no longer follows the reviewed Core output-selection exclusion.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: unwrap Core stage errors without swallowing a failed admission or native dispatch. */
    //-------------------------------------------------------------------------------------------------
    private static void Invoke(MethodInfo method,object target,params object[] arguments)
    { try { method.Invoke(target,arguments); } catch (TargetInvocationException error) when (error.InnerException != null) { ExceptionDispatchInfo.Capture(error.InnerException).Throw(); throw; } }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require explicit output-policy refusal rather than accepting unrelated file or schema failures as proof. */
    //-------------------------------------------------------------------------------------------------
    private static void RefuseProduction(Action action)
    {
        try { action(); }
        catch (BinaryAssetBuilderException error) when (error.ErrorCode == ErrorCode.InternalError && error.Message.Contains("Reborn-Local-PathMusic-Core-v",StringComparison.Ordinal)) { return; }
        throw new InvalidDataException("Controlled music enabled production output.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fingerprint detached native bytes without presenting a payload digest as a Core identity. */
    //-------------------------------------------------------------------------------------------------
    private static string Hash(byte[] bytes) => Convert.ToHexString(SHA256.HashData(bytes));

    // Reborn: no public factory or production descriptor exposes this closure-bound plugin.
    private sealed class LocalPlugin(PathMusicCorePreparation preparation,string directory,bool selected) : IAssetBuilderPlugin,IAssetBuilderOutputPolicy
    {
        // Reborn: owner object identity and exact normalized XML are private admission witnesses, not a caller-provided hash/name allow-list.
        private sealed record Bound(PathMusicCorePreparation.Row Row,string Xml);
        private readonly Dictionary<InstanceDeclaration,Bound> _owners = new();
        private bool _initialized;
        private byte[] _header = Array.Empty<byte>();
        internal int Calls { get; private set; }
        public string ProfileName => selected ? "Reborn-Local-PathMusic-Core-v2" : "Reborn-Local-PathMusic-Core-v1";
        internal uint TypeHash => selected ? LocalTypeHash : 0;
        public bool CanWriteProductionOutput => false;
        public bool CanUseBuildCache => false;
        public bool CanReuseCompiledDocuments => false;
        public uint AllTypesHash => 0;
        public uint VersionNumber => selected ? 2u : 1u;

        //-------------------------------------------------------------------------------------------------
        /** Reborn: unsupported platforms invalidate initialization before throwing; native ABI admission remains Win32-only. */
        //-------------------------------------------------------------------------------------------------
        public void Initialize(TargetPlatform platform)
        { _initialized = false; if (platform != TargetPlatform.Win32 || IntPtr.Size != 4 || (selected && LocalTypeHash == 0)) throw new NotSupportedException("Controlled music requires Win32/nonzero declared local v2 metadata."); _initialized = true; }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: retain invalidation on repeated platform initialization. */
        //-------------------------------------------------------------------------------------------------
        public void ReInitialize(TargetPlatform platform) => Initialize(platform);

        //-------------------------------------------------------------------------------------------------
        /** Reborn: return fresh synthetic metadata without leaking mutable registry type information or recovered EP1 constants. */
        //-------------------------------------------------------------------------------------------------
        public ExtendedTypeInformation GetExtendedTypeInformation(uint typeId)
        {
            if (!_initialized || typeId != 0x9A651D89u) throw new NotSupportedException("Controlled music type/platform not admitted.");
            return new() { TypeId = typeId,TypeName = "PathMusicEvent",TypeHash = TypeHash,ProcessingHash = PathMusicCoreIdentity.Processing,HasCustomData = false,UseBuildCache = false };
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: bind the complete private closure after v1 exclusion or actual v2 Core dependency preparation, before native dispatch. */
        //-------------------------------------------------------------------------------------------------
        internal void Admit(InstanceDeclaration[] instances)
        {
            if (!_initialized || _owners.Count != 0) throw new InvalidDataException("Controlled music admission state differs.");
            preparation.VerifyCurrent(); var report = preparation.Preflight();
            _header = SdkEnvironmentPreflight.Read(Path.Combine(directory,"events.h"),1048576);
            if (Hash(_header) != report.HeaderSha256 || instances.Length != report.Rows.Length) throw new InvalidDataException("Controlled header/owners differ.");
            for (int index = 0; index < instances.Length; index++)
            {
                var instance = instances[index]; var row = report.Rows[index];
                Validate(instance,row); _owners.Add(instance,new(row,instance.XmlNode.OuterXml));
            }
            preparation.VerifyCurrent();
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: refuse stale, foreign, edited or unprepared instances before serializing a fresh Win32 native buffer from captured canonical header bytes. */
        //-------------------------------------------------------------------------------------------------
        public AssetBuffer ProcessInstance(InstanceDeclaration instance)
        {
            if (!_initialized || !_owners.TryGetValue(instance,out var bound)) throw new InvalidDataException("Music instance is not privately admitted.");
            preparation.VerifyCurrent(); Validate(instance,bound.Row);
            if (instance.XmlNode.OuterXml != bound.Xml) throw new InvalidDataException("Prepared music XML was edited.");
            var chunk = PathMusicRuntimeProbe.FromHeader(_header,bound.Row.Name,bound.Row.Alternate,bound.Row.Cacheable).Chunk;
            preparation.VerifyCurrent(); Calls++;
            return new() { InstanceData = chunk.InstanceBuffer,RelocationData = chunk.RelocationBuffer,ImportsData = chunk.ImportsBuffer };
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: require captured identity/file/weak closure and the selected profile's exact absent-v1 or prepared-v2 Core dependency state. */
        //-------------------------------------------------------------------------------------------------
        private void Validate(InstanceDeclaration instance,PathMusicCorePreparation.Row row)
        {
            if (Settings.Current.TargetPlatform != TargetPlatform.Win32 || instance.Handle.TypeId != 0x9A651D89u || instance.Handle.TypeHash != TypeHash
                || instance.Handle.InstanceName != row.Name || instance.Handle.InstanceId != row.InstanceId || instance.Handle.InstanceHash != row.CoreInstanceHash
                || instance.ProcessingHash != PathMusicCoreIdentity.Processing || instance.HasCustomData || instance.InheritFromHandle != null
                || instance.ReferencedInstances.Count != 0
                || (selected ? instance.ValidatedReferencedInstances == null || instance.ValidatedReferencedInstances.Count != 0
                    || instance.AllDependentInstances == null || instance.AllDependentInstances.Count != 0
                    : instance.ValidatedReferencedInstances != null || instance.AllDependentInstances != null)
                || instance.ReferencedFiles.Count != 1 || instance.ReferencedFiles[0] != "events.h"
                || instance.WeakReferencedInstances.Count != (row.Alternate == null ? 0 : 1)
                || (row.Alternate != null && (instance.WeakReferencedInstances[0].TypeId != 0x9A651D89u
                    || instance.WeakReferencedInstances[0].InstanceId != InstanceHandle.GetInstanceId(row.Alternate)
                    || instance.WeakReferencedInstances[0].InstanceName != row.Alternate))) throw new InvalidDataException("Controlled music identity/dependency projection differs.");
        }
    }
}
