using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: dispatch only privately paired local music owners through a registered Core plugin in memory, keeping production output and cache closed.
internal static class PathMusicControlledCompiler
{
    // Reborn: detached native evidence records successful private plugin dispatch, not stock EP1 processor or game compatibility.
    internal sealed record Row(string Name,uint InstanceId,uint InstanceHash,int BinBytes,int RelocationBytes,int ImportsBytes,string BinSha256,string RelocationSha256);
    internal sealed record Report(uint ProcessingHash,uint DocumentVersion,uint Checksum,int ProcessorCalls,Row[] Rows)
    {
        public bool SyntheticProcessingDomain => true;
        public bool ProductionBuildReady => false;
        public bool CanUseBuildCache => false;
        public bool CanReuseCompiledDocuments => false;
        public bool GameLoadProved => false;
        // Reborn: zero TypeHash triggers Core's existing output-selection early return; private local closure does not claim standard dependency preparation.
        public bool CoreOutputSelectionSkipped => true;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prepare fresh Core XML, prove zero-TypeHash output exclusion, admit private local closure and dispatch memory-only native buffers. */
    //-------------------------------------------------------------------------------------------------
    internal static Report Compile(string directory,Action<IAssetBuilderPlugin,InstanceDeclaration[]>? beforeCompile = null)
    {
        directory = Path.GetFullPath(directory); var preparation = PathMusicCorePreparation.Read(directory);
        var evidence = preparation.Preflight(); var expected = preparation.Compile(); Settings saved = Settings.Current;
        try
        {
            string schema = Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"PathMusicIdentityPipeline.xsd");
            Settings.Current = new Settings { BuildCache = false,SchemaPath = schema,DataRoot = directory,DataPaths = new[] { directory },
                TargetPlatform = TargetPlatform.Win32,CustomPostfix = "",StreamPostfix = "",StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            LocalPlugin plugin = new(preparation,directory); plugin.Initialize(TargetPlatform.Win32);
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
            foreach (var instance in instances) ProveOutputSelectionSkipped(document,instance);
            plugin.Admit(instances); preparation.VerifyCurrent();
            // Reborn: owned tests may mutate prepared objects but cannot remove any plugin freshness/identity/dependency check.
            beforeCompile?.Invoke(plugin,instances);
            Row[] rows = new Row[instances.Length];
            for (int index = 0; index < instances.Length; index++)
            {
                var instance = instances[index]; AssetBuffer actual = plugins.GetPlugin(instance.Handle.TypeId).ProcessInstance(instance);
                if (!actual.InstanceData.AsSpan().SequenceEqual(expected[index].InstanceBuffer)
                    || !actual.RelocationData.AsSpan().SequenceEqual(expected[index].RelocationBuffer)
                    || !actual.ImportsData.AsSpan().SequenceEqual(expected[index].ImportsBuffer)) throw new InvalidDataException("Controlled music differs from frozen native chunks.");
                rows[index] = new(instance.Handle.InstanceName,instance.Handle.InstanceId,instance.Handle.InstanceHash,actual.InstanceData.Length,
                    actual.RelocationData.Length,actual.ImportsData.Length,Hash(actual.InstanceData),Hash(actual.RelocationData));
            }
            uint checksum = PathMusicCoreChecksum.Core(instances);
            if (checksum != PathMusicCoreChecksum.Independent(instances) || checksum != PathMusicCoreChecksum.Inspect(preparation).CoreChecksum)
                throw new InvalidDataException("Controlled music identity checksum differs.");
            preparation.VerifyCurrent(); return new(evidence.ProcessingHash,evidence.DocumentVersion,checksum,plugin.Calls,rows);
        }
        finally { Settings.Current = saved; }
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
        catch (BinaryAssetBuilderException error) when (error.ErrorCode == ErrorCode.InternalError && error.Message.Contains("Reborn-Local-PathMusic-Core-v1",StringComparison.Ordinal)) { return; }
        throw new InvalidDataException("Controlled music enabled production output.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fingerprint detached native bytes without presenting a payload digest as a Core identity. */
    //-------------------------------------------------------------------------------------------------
    private static string Hash(byte[] bytes) => Convert.ToHexString(SHA256.HashData(bytes));

    // Reborn: no public factory or production descriptor exposes this closure-bound plugin.
    private sealed class LocalPlugin(PathMusicCorePreparation preparation,string directory) : IAssetBuilderPlugin,IAssetBuilderOutputPolicy
    {
        // Reborn: owner object identity and exact normalized XML are private admission witnesses, not a caller-provided hash/name allow-list.
        private sealed record Bound(PathMusicCorePreparation.Row Row,string Xml);
        private readonly Dictionary<InstanceDeclaration,Bound> _owners = new();
        private bool _initialized;
        private byte[] _header = Array.Empty<byte>();
        internal int Calls { get; private set; }
        public string ProfileName => "Reborn-Local-PathMusic-Core-v1";
        public bool CanWriteProductionOutput => false;
        public bool CanUseBuildCache => false;
        public bool CanReuseCompiledDocuments => false;
        public uint AllTypesHash => 0;
        public uint VersionNumber => 1;

        //-------------------------------------------------------------------------------------------------
        /** Reborn: unsupported platforms invalidate initialization before throwing; native ABI admission remains Win32-only. */
        //-------------------------------------------------------------------------------------------------
        public void Initialize(TargetPlatform platform)
        { _initialized = false; if (platform != TargetPlatform.Win32 || IntPtr.Size != 4) throw new NotSupportedException("Controlled music requires Win32."); _initialized = true; }

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
            return new() { TypeId = typeId,TypeName = "PathMusicEvent",TypeHash = 0,ProcessingHash = PathMusicCoreIdentity.Processing,HasCustomData = false,UseBuildCache = false };
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: bind the complete privately validated local closure once after proving standard Core output exclusion, before native dispatch. */
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
        /** Reborn: require captured identity and private file/weak closure while standard Core dependency tables remain absent under zero-TypeHash exclusion. */
        //-------------------------------------------------------------------------------------------------
        private void Validate(InstanceDeclaration instance,PathMusicCorePreparation.Row row)
        {
            if (Settings.Current.TargetPlatform != TargetPlatform.Win32 || instance.Handle.TypeId != 0x9A651D89u || instance.Handle.TypeHash != 0
                || instance.Handle.InstanceName != row.Name || instance.Handle.InstanceId != row.InstanceId || instance.Handle.InstanceHash != row.CoreInstanceHash
                || instance.ProcessingHash != PathMusicCoreIdentity.Processing || instance.HasCustomData || instance.InheritFromHandle != null
                || instance.ReferencedInstances.Count != 0 || instance.ValidatedReferencedInstances != null || instance.AllDependentInstances != null
                || instance.ReferencedFiles.Count != 1 || instance.ReferencedFiles[0] != "events.h"
                || instance.WeakReferencedInstances.Count != (row.Alternate == null ? 0 : 1)
                || (row.Alternate != null && (instance.WeakReferencedInstances[0].TypeId != 0x9A651D89u
                    || instance.WeakReferencedInstances[0].InstanceId != InstanceHandle.GetInstanceId(row.Alternate)
                    || instance.WeakReferencedInstances[0].InstanceName != row.Alternate))) throw new InvalidDataException("Controlled music identity/dependency projection differs.");
        }
    }
}
