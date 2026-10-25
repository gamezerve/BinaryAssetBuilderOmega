using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: compile the admitted selected Damage through actual Core; keep the fixed diagnostic and exported alpha separate.
internal static class WeaponAuthoredBuild
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: build boundary/stock/user-tested/new values and repeat from owned authored sources, preserving selected inputs. */
    //-------------------------------------------------------------------------------------------------
    internal static void SelfTest(string directory, string staticManifest, string globalManifest)
    {
        directory = Path.GetFullPath(directory);
        if (Directory.Exists(directory) || File.Exists(directory) || !Directory.Exists(Path.GetDirectoryName(directory)))
            throw new InvalidDataException("Weapon regression requires a fresh child directory.");
        Directory.CreateDirectory(directory);
        string fixture = File.ReadAllText(Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!, "WeaponAuthoringProbe.xml"));
        var hashes = new HashSet<uint>(); byte[]? repeated = null;
        foreach (int value in new[] { 0, 5, 50, 73, 10000, 73 })
        {
            string name = "damage-" + value + (value == 73 && repeated != null ? "-repeat" : "");
            string source = Path.Combine(directory, name + ".xml");
            File.WriteAllText(source, fixture.Replace("Damage=\"5.0\"", "Damage=\"" + value + ".0\""));
            byte[] before = File.ReadAllBytes(source); string output = Path.Combine(directory, name);
            Run(source, output, staticManifest, globalManifest);
            using var receipt = JsonDocument.Parse(File.ReadAllText(Path.Combine(output, "authored-weapon-build.json")));
            byte[] package = File.ReadAllBytes(receipt.RootElement.GetProperty("PackagePath").GetString()!);
            uint hash = receipt.RootElement.GetProperty("SelectedInstanceHash").GetUInt32();
            if (!before.SequenceEqual(File.ReadAllBytes(source)) || receipt.RootElement.GetProperty("Damage").GetInt32() != value)
                throw new InvalidDataException("Selected weapon source/value changed.");
            if (name.EndsWith("-repeat", StringComparison.Ordinal))
            { if (!package.SequenceEqual(repeated!)) throw new InvalidDataException("Repeated authored weapon package differs."); }
            else if (!hashes.Add(hash)) throw new InvalidDataException("Different authored weapon values retained one instance hash.");
            if (value == 73) repeated = package;
            bool refused = false;
            try { Run(source, output, staticManifest, globalManifest); } catch (InvalidDataException) { refused = true; }
            if (!refused || !package.SequenceEqual(File.ReadAllBytes(receipt.RootElement.GetProperty("PackagePath").GetString()!)))
                throw new InvalidDataException("Occupied authored weapon output was overwritten.");
        }
        Console.WriteLine("Authored weapon regression: PASS (five values, repeated package, source preservation, distinct hashes, occupied output and scoped/package controls; no game).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: freeze selected XML, prove stock baseline and isolated edit, then verify scoped streams/default-reader packaging. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string source, string directory, string staticManifest, string globalManifest)
    {
        using var input = File.Open(source, FileMode.Open, FileAccess.Read, FileShare.Read);
        if (input.Length == 0 || input.Length > 16384) throw new InvalidDataException("Selected weapon XML exceeds admission bounds.");
        byte[] bytes = new byte[(int)input.Length]; input.ReadExactly(bytes);
        var admitted = WeaponAuthoringAdmission.Admit(bytes);
        directory = Path.GetFullPath(directory);
        // Reborn: reuse checked admission staging before creating compiler outputs; selected input remains locked read-only.
        WeaponAuthoringAdmission.Stage(source, directory);
        WeaponStockReview.Inspect(staticManifest, globalManifest);
        var snapshots = new[] { staticManifest, globalManifest }.Select((path, index) => {
            string target = Path.Combine(directory, "external-" + index + ".manifest");
            File.WriteAllBytes(target, File.ReadAllBytes(path)); return target;
        }).ToArray();
        var xml = new XmlDocument { XmlResolver = null }; xml.LoadXml(admitted.Xml);
        ((XmlElement)xml.GetElementsByTagName("DamageNugget", SchemaSet.XmlNamespace)[0]!).SetAttribute("Damage", "5.0");
        string sourcePath = Path.Combine(directory, "weapon.xml");
        File.WriteAllText(sourcePath, Declaration(xml.DocumentElement!.OuterXml));
        Settings saved = Settings.Current;
        try
        {
            string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
            FXListNativeSmokeTest.CheckExtractedEnums(fixtures, "WeaponDocumentPipeline.xsd");
            Settings.Current = new Settings { BuildCache = false, ErrorLevel = 1, TargetPlatform = TargetPlatform.Win32,
                SchemaPath = Path.Combine(fixtures, "WeaponDocumentPipeline.xsd"), DataRoot = directory, DataPaths = new[] { directory },
                CustomPostfix = "", StreamPostfix = "", ProcessedExternalManifests = snapshots,
                LinkedStreams = true, ExternalManifestReferences = "static.manifest;global.manifest",
                StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            var profile = new WeaponDocumentProbe.Profile(); profile.Initialize(TargetPlatform.Win32);
            var plugins = new PluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32) { DefaultPlugin = profile };
            plugins.AddPlugin(0x94D4D96Eu, profile);
            var cache = new SessionCache(); cache.InitializeCache(new List<string>());
            var processor = new DocumentProcessor(Settings.Current, plugins,
                new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32))
                { Cache = cache, SchemaSet = new SchemaSet(false) };
            var baseline = Process();
            if (Convert.ToHexString(SHA256.HashData(baseline.Buffer.InstanceData)) != "2B0C3AABB5521C7C8315A1A5EA22DFCF8D681C6546E9AC58DBF0D0495B2A8C6B")
                throw new InvalidDataException("Authored weapon baseline differs from reviewed stock.");
            File.WriteAllText(sourcePath, Declaration(admitted.Xml));
            // Reborn: keep the independent baseline/source comparison, but compile the selected source twice only for developer determinism tests.
            var selected = Process(); var repeat = SdkBuildTests.Enabled?Process():selected;
            byte[] expected = baseline.Buffer.InstanceData.ToArray();
            BinaryPrimitives.WriteSingleLittleEndian(expected.AsSpan(2392, 4), admitted.Damage);
            if (!expected.SequenceEqual(selected.Buffer.InstanceData) || !selected.Buffer.InstanceData.SequenceEqual(repeat.Buffer.InstanceData)
                || !baseline.Buffer.RelocationData.SequenceEqual(selected.Buffer.RelocationData)
                || !baseline.Buffer.ImportsData.SequenceEqual(selected.Buffer.ImportsData)
                || selected.Instance.Handle.InstanceHash != repeat.Instance.Handle.InstanceHash
                || (admitted.Damage != 5 && baseline.Instance.Handle.InstanceHash == selected.Instance.Handle.InstanceHash))
                throw new InvalidDataException("Selected weapon changed unproven bytes or retained stale identity.");
            string manifest = WeaponStreamProbe.Write(directory, processor, selected.Document, selected.Instance, selected.Buffer, true);
            string package = WeaponDiagnosticPackage.PathFor(manifest);
            var receipt = new { WeaponId = selected.Instance.Handle.InstanceName, admitted.Damage, EditableFieldCount = 1,
                SelectedInputSha256 = Convert.ToHexString(SHA256.HashData(bytes)), ActualCoreOutputUsed = true,
                StockBaselineCompared = true, AuthoredPayloadVerified = true, RepeatDeterministic = SdkBuildTests.Enabled,
                SelectedInstanceHash = selected.Instance.Handle.InstanceHash, BaselineInstanceHash = baseline.Instance.Handle.InstanceHash,
                PackagePath = package, PackageSha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(package))),
                ConfigPath = Path.Combine(Path.GetDirectoryName(package)!, "weaponprobe_1.0.cfg"),
                ManifestPath = manifest, ExportedAlphaChanged = false, OverrideAppliedInGame = false,
                InstalledFilesModified = false, GameExecuted = false };
            string json = JsonSerializer.Serialize(receipt, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Path.Combine(directory, "authored-weapon-build.json"), json); Console.WriteLine(json);

            //-------------------------------------------------------------------------------------------------
            /** Reborn: process fresh selected source and concrete external dependencies without cached compiled output. */
            //-------------------------------------------------------------------------------------------------
            (AssetDeclarationDocument Document, InstanceDeclaration Instance, AssetBuffer Buffer) Process()
            {
                var document = processor.ProcessDocumentInternal(sourcePath, sourcePath, null!,
                    new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = false });
                var instance = document.SelfInstances.Single(); DependencyResolutionSmokeTest.Prepare(document, instance);
                if (DependencyResolutionSmokeTest.Visited(document).Count != 1 || instance.AllDependentInstances.Count != 0)
                    throw new InvalidDataException("Authored weapon unexpectedly queued local dependencies.");
                return (document, instance, plugins.GetPlugin(instance.Handle.TypeId).ProcessInstance(instance));
            }
        }
        finally { Settings.Current = saved; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: construct one declaration only from already admitted namespace/shape-checked canonical XML. */
    //-------------------------------------------------------------------------------------------------
    private static string Declaration(string xml) => "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">" + xml + "</AssetDeclaration>";
}
