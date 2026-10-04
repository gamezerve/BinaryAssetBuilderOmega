using System.Buffers.Binary;
using System.Text;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;
using BinaryAssetBuilder.Utility;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: compile one bounded selected local AudioEvent against frozen packaged AudioFiles without claiming a general source graph.
internal static class AudioFileLocalEventProbe
{
    // Reborn: source attribution belongs to the actual owned fixture written before core parsing.
    internal const string SourceXml = "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\"><AudioEvent id=\"RebornLocalAudio\" Volume=\"60\" Control=\"INTERRUPT\"><Sound>AudioFile:RebornAudioRAM</Sound><Sound Weight=\"800\">AudioFile:RebornAudioStream</Sound></AudioEvent></AssetDeclaration>";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: derive literal local references from validated caller identities while preserving the fixed event wire shape. */
    //-------------------------------------------------------------------------------------------------
    internal static string Source(AudioFilePackageProbe.Entry[] files)
    {
        ValidateFiles(files);
        return "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\"><AudioEvent id=\"RebornLocalAudio\" Volume=\"60\" Control=\"INTERRUPT\"><Sound>AudioFile:"+files[0].Name+"</Sound><Sound Weight=\"800\">AudioFile:"+files[1].Name+"</Sound></AudioEvent></AssetDeclaration>";
    }

    // Reborn: preserve copied native output and complete local dependency fingerprints, not mutable preparation handles.
    internal sealed class Entry
    {
        private readonly byte[] _bin,_relo,_imp;
        private readonly (uint Id,uint Hash)[] _dependencies;
        internal string Name { get; }
        // Reborn: retain source-derived scalar expectations rather than accepting arbitrary worker weight/volume words.
        internal AuthoredAudioEventSource.Settings Settings { get; }
        internal uint Id => InstanceHandle.GetInstanceId(Name);
        internal uint Hash { get; }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: freeze compiled event data and bind its diagnostic hash to both local AudioFile content identities. */
        //-------------------------------------------------------------------------------------------------
        internal Entry(AssetBuffer native,AudioFilePackageProbe.Entry[] files,string name = "RebornLocalAudio",AuthoredAudioEventSource.Settings? settings = null)
        {
            Settings = settings ?? AuthoredAudioEventSource.Settings.Default;
            ValidateFiles(files); CheckNative(native,Settings);
            // Reborn: freeze the admitted event identity separately from its content/dependency diagnostic hash.
            AudioFileDiagnosticIdentity.Validate(name); Name = name;
            _bin = (byte[])native.InstanceData.Clone(); _relo = (byte[])native.RelocationData.Clone(); _imp = (byte[])native.ImportsData.Clone();
            _dependencies = Settings.Slots().Select(slot => (files[slot].Id,files[slot].Hash)).ToArray();
            using MemoryStream hash = new(); hash.Write(_bin); using BinaryWriter writer = new(hash,Encoding.UTF8,true);
            foreach (var file in _dependencies) { writer.Write(0x166B084Du); writer.Write(0x53C81E47u); writer.Write(file.Id); writer.Write(file.Hash); }
            writer.Flush(); Hash = FastHash.GetHashCode(hash.ToArray());
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: prevent stale parent evidence from being reused after local custom payload identities change. */
        //-------------------------------------------------------------------------------------------------
        internal void ValidateDependencies(AudioFilePackageProbe.Entry[] files)
        {
            ValidateFiles(files);
            if (!_dependencies.SequenceEqual(Settings.Slots().Select(slot => (files[slot].Id,files[slot].Hash)))) throw new InvalidDataException("Local AudioEvent dependency fingerprints are stale.");
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: expose a detached ordered manifest reference list bound only to actually selected concrete leaves. */
        //-------------------------------------------------------------------------------------------------
        internal AssetId[] References(AudioFilePackageProbe.Entry[] files)
        { ValidateDependencies(files); return Settings.Slots().Select(slot => new AssetId(0x166B084Du,files[slot].Id)).ToArray(); }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: return only detached event buffers to the package writer and tests. */
        //-------------------------------------------------------------------------------------------------
        internal AssetBuffer CopyNative() => new() { InstanceData = (byte[])_bin.Clone(),RelocationData = (byte[])_relo.Clone(),ImportsData = (byte[])_imp.Clone() };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require the same unique ordered RAM/streamed local records that the fixed AudioFile package admits. */
    //-------------------------------------------------------------------------------------------------
    private static void ValidateFiles(AudioFilePackageProbe.Entry[] files)
    {
        if (files.Length != 2 || files[0].Source != "ram.xml" || files[1].Source != "streamed.xml" || files[0].Id == files[1].Id)
            throw new InvalidDataException("Local AudioEvent requires unique RAM then streamed dependency records.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently require the admitted one/two-Sound wire shape, one-biased imports and exact relocation/import tables. */
    //-------------------------------------------------------------------------------------------------
    internal static void CheckNative(AssetBuffer native,AuthoredAudioEventSource.Settings? settings = null)
    {
        // Reborn: changing allowed scalars must still match their independently prepared source values exactly.
        settings ??= AuthoredAudioEventSource.Settings.Default;
        settings.Validate();
        byte[] imports = new byte[settings.ImportsLength];
        for (int index = 0; index < settings.Count; index++) BinaryPrimitives.WriteUInt32LittleEndian(imports.AsSpan(index*4),(uint)(152+12*index));
        BinaryPrimitives.WriteUInt32LittleEndian(imports.AsSpan(settings.Count*4),uint.MaxValue);
        if (native.InstanceData.Length != settings.NativeLength || !native.RelocationData.SequenceEqual(Convert.FromHexString("8C000000FFFFFFFF"))
            || !native.ImportsData.SequenceEqual(imports)) throw new InvalidDataException("Local AudioEvent native shape differs.");
        byte[] bin = native.InstanceData;
        if (BinaryPrimitives.ReadUInt32LittleEndian(bin.AsSpan(136)) != settings.Count || BinaryPrimitives.ReadUInt32LittleEndian(bin.AsSpan(140)) != 152
            || BinaryPrimitives.ReadUInt32LittleEndian(bin.AsSpan(4)) != settings.VolumeBits) throw new InvalidDataException("Local AudioEvent scalar/count/pointer differs.");
        for (int index = 0; index < settings.Count; index++)
            if (BinaryPrimitives.ReadUInt32LittleEndian(bin.AsSpan(152+12*index)) != index+1
                || BinaryPrimitives.ReadUInt32LittleEndian(bin.AsSpan(156+12*index)) != settings.WeightAt(index)
                || BinaryPrimitives.ReadUInt32LittleEndian(bin.AsSpan(160+12*index)) != 0x3F800000u)
                throw new InvalidDataException("Local AudioEvent selectors/weights differ.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: check current normalized identities against local metadata before authorizing concrete AudioFile slots and compiling. */
    //-------------------------------------------------------------------------------------------------
    internal static Entry Compile(InstanceDeclaration instance,AudioFilePackageProbe.Entry[] files,Ra3Ep1AudioEventPlugin plugin,string name = "RebornLocalAudio")
    {
        instance.ValidatedReferencedInstances = null!; ValidateFiles(files);
        if (instance.Handle.TypeId != 0x844D7B9Fu || instance.Handle.TypeHash != 0x560C2E45u || instance.Handle.InstanceName != name
            || instance.ReferencedInstances.Count is < 1 or > 2) throw new InvalidDataException("Local AudioEvent identity/reference shape differs.");
        List<InstanceHandle> concrete = new();
        List<int> slots = new();
        for (int index = 0; index < instance.ReferencedInstances.Count; index++)
        {
            InstanceHandle original = instance.ReferencedInstances[index];
            int slot = Array.FindIndex(files,file => file.Name == original.InstanceName && file.Id == original.InstanceId);
            if (slot < 0 || slots.Contains(slot)) throw new InvalidDataException("Local AudioEvent target is unknown or repeated.");
            slots.Add(slot); var file = files[slot];
            if (original.TypeId != 0x166B084Du || original.InstanceId != file.Id || original.InstanceName != file.Name)
                throw new InvalidDataException("Local AudioEvent normalized dependency order/identity differs.");
            concrete.Add(new InstanceHandle("AudioFile",file.Name) { TypeHash = 0x53C81E47u,InstanceHash = file.Hash });
        }
        instance.ValidatedReferencedInstances = concrete;
        try { return new Entry(plugin.ProcessInstance(instance),files,name,AuthoredAudioEventSource.ReadSettings((System.Xml.XmlElement)instance.Node,slots.ToArray())); }
        catch { instance.ValidatedReferencedInstances = null!; throw; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: parse the owned source through the actual core schema/reference normalizer, then explicitly prepare fixed local metadata. */
    //-------------------------------------------------------------------------------------------------
    internal static Entry Build(string directory,AudioFilePackageProbe.Entry[] files,Action<InstanceDeclaration,Ra3Ep1AudioEventPlugin>? audit = null,string? authoredName = null)
    {
        string path = Path.Combine(directory,"event.xml");
        if (authoredName == null) { using FileStream writer = new(path,FileMode.CreateNew,FileAccess.Write); writer.Write(Encoding.UTF8.GetBytes(Source(files))); }
        else
        {
            // Reborn: validate a bounded installed event before core loading; do not rewrite its raw caller provenance.
            using FileStream reader = File.OpenRead(path); if (reader.Length > 8192) throw new InvalidDataException("Authored event exceeds source bound.");
            byte[] bytes = new byte[(int)reader.Length]; reader.ReadExactly(bytes); if (reader.ReadByte() != -1) throw new InvalidDataException("Authored event length changed.");
            if (AuthoredAudioEventSource.Validate(bytes,files[0].Name,files[1].Name) != authoredName) throw new InvalidDataException("Authored event identity differs.");
        }
        Settings saved = Settings.Current;
        try
        {
            Settings.Current = new Settings { BuildCache = false,SchemaPath = Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"AudioEventPipeline.xsd"),
                DataRoot = directory,DataPaths = new[] { directory },TargetPlatform = TargetPlatform.Win32,CustomPostfix = "",StreamPostfix = "",StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            Ra3Ep1AudioEventPlugin plugin = new(); plugin.Initialize(TargetPlatform.Win32);
            PluginRegistry registry = new(Array.Empty<PluginDescriptor>(),TargetPlatform.Win32); registry.AddPlugin(0x844D7B9Fu,plugin);
            SessionCache cache = new(); cache.InitializeCache(new List<string>());
            DocumentProcessor processor = new(Settings.Current,registry,new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(),TargetPlatform.Win32)) { Cache = cache,SchemaSet = new SchemaSet(false) };
            InstanceDeclaration instance = processor.ProcessDocumentInternal(path,path,null!,new DocumentProcessor.ProcessOptions { GenerateOutput = false }).SelfInstances.Single();
            // Reborn: optional managed regressions inspect/revoke current core preparation before the final fixed compile.
            audit?.Invoke(instance,plugin);
            return Compile(instance,files,plugin,authoredName ?? "RebornLocalAudio");
        }
        finally { Settings.Current = saved; }
    }
}
