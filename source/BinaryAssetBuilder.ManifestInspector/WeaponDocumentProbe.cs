using System.Globalization;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: exercise actual weapon document/dependency/plugin dispatch in owned snapshots without authorizing stream output.
internal static class WeaponDocumentProbe
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: freeze a selected reference weapon and manifests, then compare baseline/repeat/edit through actual Core documents. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string source, string staticManifest, string globalManifest, string directory)
    {
        directory = Path.GetFullPath(directory);
        if (Directory.Exists(directory) || File.Exists(directory) || !Directory.Exists(Path.GetDirectoryName(directory)))
            throw new InvalidDataException("Weapon probe output must be a new child of an existing directory.");
        WeaponStockReview.Inspect(staticManifest, globalManifest);
        using var input = File.OpenRead(source);
        if (input.Length > 16 * 1024 * 1024) throw new InvalidDataException("Weapon reference exceeds 16 MiB.");
        using var reader = XmlReader.Create(input, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null });
        var xml = new XmlDocument { XmlResolver = null }; xml.Load(reader);
        var ns = new XmlNamespaceManager(xml.NameTable); ns.AddNamespace("ea", SchemaSet.XmlNamespace);
        var selected = (XmlElement?)xml.SelectSingleNode("/ea:AssetDeclaration/ea:WeaponTemplate[@id='SovietAntiInfantryInfantryAK47']", ns)
            ?? throw new InvalidDataException("Reference AK47 absent.");
        string authored = "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">" + selected.OuterXml + "</AssetDeclaration>";
        Directory.CreateDirectory(directory);
        var snapshots = new[] { staticManifest, globalManifest }.Select((path, index) => {
            string target = Path.Combine(directory, "external-" + index + ".manifest");
            File.WriteAllBytes(target, File.ReadAllBytes(path)); return target;
        }).ToArray();
        string sourcePath = Path.Combine(directory, "weapon.xml"); File.WriteAllText(sourcePath, authored);
        Settings saved = Settings.Current;
        try
        {
            string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
            FXListNativeSmokeTest.CheckExtractedEnums(fixtures, "WeaponDocumentPipeline.xsd");
            Settings.Current = new Settings { BuildCache = false, ErrorLevel = 1, TargetPlatform = TargetPlatform.Win32,
                SchemaPath = Path.Combine(fixtures, "WeaponDocumentPipeline.xsd"), DataRoot = directory, DataPaths = new[] { directory },
                CustomPostfix = "", StreamPostfix = "", ProcessedExternalManifests = snapshots,
                StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            var profile = new Profile(); profile.Initialize(TargetPlatform.Win32);
            var plugins = new PluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32);
            plugins.AddPlugin(0x94D4D96Eu, profile);
            var cache = new SessionCache(); cache.InitializeCache(new List<string>());
            var processor = new DocumentProcessor(Settings.Current, plugins,
                new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32))
                { Cache = cache, SchemaSet = new SchemaSet(false) };
            var first = Process(); var repeat = Process();
            // Reborn: a modified reference cannot silently become the stock baseline for this diagnostic.
            if (Convert.ToHexString(SHA256.HashData(first.Buffer.InstanceData)) != "2B0C3AABB5521C7C8315A1A5EA22DFCF8D681C6546E9AC58DBF0D0495B2A8C6B")
                throw new InvalidDataException("Weapon document baseline does not match the independently reviewed stock BIN.");
            int refusals = 0;
            var nugget = (XmlElement)((XmlElement)first.Instance.XmlNode).GetElementsByTagName("DamageNugget", SchemaSet.XmlNamespace)[0]!;
            string typeId = nugget.GetAttribute("TypeId");
            try { nugget.SetAttribute("TypeId", "0"); ExpectRefusal(() => profile.ProcessInstance(first.Instance)); }
            finally { nugget.SetAttribute("TypeId", typeId); }
            var weaponRoot = (XmlElement)first.Instance.XmlNode;
            try { weaponRoot.SetAttribute("RebornUnknownAttribute", "1"); ExpectRefusal(() => profile.ProcessInstance(first.Instance)); }
            finally { weaponRoot.RemoveAttribute("RebornUnknownAttribute"); }
            // Reborn: remove external mapping after success to verify that real preparation refuses stale dependency authority.
            try
            {
                Settings.Current.ProcessedExternalManifests = Array.Empty<string>();
                ExpectRefusal(() => { Process(); });
            }
            finally { Settings.Current.ProcessedExternalManifests = snapshots; }
            ExpectRefusal(() => profile.GetExtendedTypeInformation(0));
            if (profile.CanWriteProductionOutput || profile.CanUseBuildCache || profile.CanReuseCompiledDocuments)
                throw new InvalidDataException("Weapon diagnostic accidentally enabled production authority.");
            if (!first.Buffer.InstanceData.SequenceEqual(repeat.Buffer.InstanceData)
                || first.Instance.Handle.InstanceHash != repeat.Instance.Handle.InstanceHash)
                throw new InvalidDataException("Repeated weapon document differs.");
            var editXml = new XmlDocument { XmlResolver = null }; editXml.LoadXml(authored);
            ((XmlElement)editXml.GetElementsByTagName("DamageNugget", SchemaSet.XmlNamespace)[0]!).SetAttribute("Damage", "50.0");
            File.WriteAllText(sourcePath, editXml.OuterXml);
            var edited = Process();
            var expected = first.Buffer.InstanceData.ToArray();
            System.Buffers.Binary.BinaryPrimitives.WriteSingleLittleEndian(expected.AsSpan(2392, 4), 50f);
            if (!expected.SequenceEqual(edited.Buffer.InstanceData)
                || !first.Buffer.RelocationData.SequenceEqual(edited.Buffer.RelocationData)
                || edited.Instance.Handle.InstanceHash == first.Instance.Handle.InstanceHash)
                throw new InvalidDataException("Core weapon edit changed unproven bytes or retained stale hash.");
            File.WriteAllText(sourcePath, authored);
            var restored = Process();
            if (!restored.Buffer.InstanceData.SequenceEqual(first.Buffer.InstanceData)
                || restored.Instance.Handle.InstanceHash != first.Instance.Handle.InstanceHash)
                throw new InvalidDataException("Restored Core weapon differs.");
            if (Directory.EnumerateFiles(directory).Any(path => Path.GetExtension(path) is ".bin" or ".relo" or ".imp"))
                throw new InvalidDataException("Document probe unexpectedly wrote streams.");
            var receipt = new { Asset = first.Instance.Handle.Name, CoreDocumentProcessed = true, CoreDependenciesPrepared = true,
                PluginDispatchUsed = true, BaselineSha256 = Convert.ToHexString(SHA256.HashData(first.Buffer.InstanceData)),
                BaselineInstanceHash = first.Instance.Handle.InstanceHash, EditedInstanceHash = edited.Instance.Handle.InstanceHash,
                Dependencies = first.Instance.ValidatedReferencedInstances.Select(handle => handle.Name).ToArray(),
                RepeatDeterministic = true, DamageEditIsolated = true, SourceRestoreVerified = true, NegativeControlsPassed = refusals,
                ProductionOutputWritten = false, ModPackageBuilt = false, GameExecuted = false };
            File.WriteAllText(Path.Combine(directory, "weapon-document.json"), JsonSerializer.Serialize(receipt, new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine(JsonSerializer.Serialize(receipt, new JsonSerializerOptions { WriteIndented = true }));

            //-------------------------------------------------------------------------------------------------
            /** Reborn: accept only expected validation/unsupported/reference failures, never a successful corrupted compile. */
            //-------------------------------------------------------------------------------------------------
            void ExpectRefusal(Action action)
            {
                try { action(); }
                catch (Exception error) when (error is InvalidDataException or NotSupportedException
                    || error is BinaryAssetBuilderException)
                { refusals++; return; }
                throw new InvalidDataException("Weapon document negative control was accepted.");
            }

            //-------------------------------------------------------------------------------------------------
            /** Reborn: use actual source loading, hashing, derived external resolution and checked plugin dispatch on each fresh attempt. */
            //-------------------------------------------------------------------------------------------------
            (InstanceDeclaration Instance, AssetBuffer Buffer) Process()
            {
                var document = processor.ProcessDocumentInternal(sourcePath, sourcePath, null!,
                    new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = false });
                var instance = document.SelfInstances.Single();
                DependencyResolutionSmokeTest.Prepare(document, instance);
                if (DependencyResolutionSmokeTest.Visited(document).Count != 1 || instance.AllDependentInstances.Count != 0)
                    throw new InvalidDataException("Stock weapon references queued local dependency compilation.");
                return (instance, plugins.GetPlugin(instance.Handle.TypeId).ProcessInstance(instance));
            }
        }
        finally { Settings.Current = saved; }
    }

    // Reborn: private diagnostic registration cannot be selected by the production registry or SDK alpha adapter.
    private sealed class Profile : IAssetBuilderPlugin, IAssetBuilderOutputPolicy
    {
        private bool initialized;
        public string ProfileName => "RA3EP1-AK47-Document-Diagnostic-v1";
        public bool CanWriteProductionOutput => false;
        public bool CanUseBuildCache => false;
        public bool CanReuseCompiledDocuments => false;
        public uint AllTypesHash => 0x5454A8E9u;
        public uint VersionNumber => 1;

        //-------------------------------------------------------------------------------------------------
        /** Reborn: fail closed after unsupported-platform initialization instead of retaining previous Win32 authority. */
        //-------------------------------------------------------------------------------------------------
        public void Initialize(TargetPlatform platform)
        { initialized = false; if (platform != TargetPlatform.Win32) throw new NotSupportedException("Weapon diagnostic supports only Win32."); initialized = true; }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: reinitialization shares the same platform refusal. */
        //-------------------------------------------------------------------------------------------------
        public void ReInitialize(TargetPlatform platform) => Initialize(platform);

        //-------------------------------------------------------------------------------------------------
        /** Reborn: return only independently proven weapon metadata, with a private processor revision rather than an EA hash claim. */
        //-------------------------------------------------------------------------------------------------
        public ExtendedTypeInformation GetExtendedTypeInformation(uint typeId)
        {
            if (!initialized || typeId != 0x94D4D96Eu) throw new NotSupportedException("Unsupported weapon diagnostic type/platform.");
            return new ExtendedTypeInformation { TypeId = typeId, TypeName = "WeaponTemplate", Type = typeof(SageBinaryData.WeaponTemplate),
                TypeHash = 0x27E8D984u, ProcessingHash = 0x27E8D984u ^ 0x45503103u, Tokenized = true, UseBuildCache = false, HasCustomData = false };
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: require concrete prepared references and current schema identity before compiling the fixed native/token shape. */
        //-------------------------------------------------------------------------------------------------
        public AssetBuffer ProcessInstance(InstanceDeclaration instance)
        {
            GetExtendedTypeInformation(instance.Handle.TypeId);
            string[] expected = { "Multisound:SOV_Conscript_GunFireMS", "FXList:FX_MuzzleSoviet", "FXList:FX_MuzzleSoviet" };
            if (instance.Handle.InstanceName != "SovietAntiInfantryInfantryAK47" || instance.InheritFromHandle != null || instance.HasCustomData
                || instance.WeakReferencedInstances.Count != 0 || instance.ReferencedFiles.Count != 0
                || instance.ValidatedReferencedInstances == null || !instance.ValidatedReferencedInstances.Select(handle => handle.Name).SequenceEqual(expected))
                throw new InvalidDataException("Weapon diagnostic requires the exact complete concrete dependency preparation.");
            var root = (XmlElement)instance.XmlNode;
            if (root.SchemaInfo.SchemaType?.Name != "WeaponTemplate") throw new InvalidDataException("Weapon lacks current schema type.");
            // Reborn: revalidate a detached copy after verifying/removing only exact schema-derived Core TypeId annotations.
            var detached = new XmlDocument { XmlResolver = null }; detached.LoadXml(root.OuterXml);
            var originals = new[] { root }.Concat(root.GetElementsByTagName("*").Cast<XmlElement>()).ToArray();
            var copies = new[] { detached.DocumentElement! }.Concat(detached.GetElementsByTagName("*").Cast<XmlElement>().Skip(1)).ToArray();
            for (int index = 0; index < originals.Length; index++)
            {
                if (!originals[index].HasAttribute("TypeId")) continue;
                string name = originals[index].SchemaInfo.SchemaType?.Name ?? throw new InvalidDataException("Injected ID lacks schema type.");
                if (!uint.TryParse(originals[index].GetAttribute("TypeId"), NumberStyles.None, CultureInfo.InvariantCulture, out uint value)
                    || value != FastHash.GetHashCode(name)) throw new InvalidDataException("Forged Core TypeId.");
                copies[index].RemoveAttribute("TypeId");
            }
            var prepared = WeaponSchemaPreparation.Prepare(detached, Settings.Current.ProcessedExternalManifests);
            var chunk = WeaponTokenReproduction.Encode(WeaponNativeProbe.Compile(prepared));
            return new AssetBuffer { InstanceData = chunk.InstanceBuffer, RelocationData = chunk.RelocationBuffer, ImportsData = chunk.ImportsBuffer };
        }
    }
}
