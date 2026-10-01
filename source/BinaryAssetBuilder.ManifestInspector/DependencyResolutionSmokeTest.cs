using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;
using BinaryAssetBuilder.Utility;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: exercise real output-dependency preparation without constructing an output manager or permitting experimental stream writes.
internal static class DependencyResolutionSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: failed and previously successful metadata resolution must be rechecked on retry and external mapping changes. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        string directory = Path.Combine(Path.GetTempPath(), "Reborn-DependencyResolution-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        Settings saved = Settings.Current;
        try
        {
            Settings.Current = new Settings { BuildCache = false, ErrorLevel = 1,
                SchemaPath = Path.Combine(fixtures, "AttributeModifierPipeline.xsd"), DataRoot = directory, DataPaths = new[] { directory },
                TargetPlatform = TargetPlatform.Win32, CustomPostfix = "", StreamPostfix = "",
                StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>(), ProcessedExternalManifests = Array.Empty<string>() };
            XmlDocument fixturesXml = new();
            fixturesXml.Load(Path.Combine(fixtures, "AttributeModifierImportProbe.xml"));
            XmlElement root = fixturesXml.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(element => element.GetAttribute("id") == "AttributeModifier_IronCurtain");
            string source = Path.Combine(directory, "source.xml");
            File.WriteAllText(source, "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">" + root.OuterXml + "</AssetDeclaration>");
            SessionCache cache = new();
            cache.InitializeCache(new List<string>());
            PluginRegistry plugins = new(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32);
            DocumentProcessor processor = new(Settings.Current, plugins, new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32))
                { Cache = cache, SchemaSet = new SchemaSet(false) };
            AssetDeclarationDocument document = processor.ProcessDocumentInternal(source, source, null!,
                new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = false });
            InstanceDeclaration instance = document.SelfInstances.Single();
            Require(instance.Handle.TypeHash == 0 && instance.ReferencedInstances.Count == 3, "Unexpected metadata fixture registration or dependencies.");
            // Reborn: assigning an observed hash enables only this private dependency seam; no plugin/output manager is activated.
            instance.Handle.TypeHash = 0x74425C11u;
            ExpectMissing(document, instance);
            ExpectMissing(document, instance);
            Require(instance.ValidatedReferencedInstances == null && instance.AllDependentInstances == null,
                "Failed dependency preparation retained partially validated metadata.");
            string[] targets = instance.ReferencedInstances.Select((handle, index) => Path.Combine(directory, "target" + index + ".manifest")).ToArray();
            for (int index = 0; index < targets.Length; index++)
                ExternalLinkSmokeTest.WriteFixture(targets[index], instance.ReferencedInstances[index], new ReferencedFileBuffer());
            Settings.Current.ProcessedExternalManifests = targets;
            Prepare(document, instance);
            Require(instance.ValidatedReferencedInstances!.Select(handle => handle.Name).SequenceEqual(instance.ReferencedInstances.Select(handle => handle.Name))
                && instance.AllDependentInstances!.Count == 0, "External resolution changed order or queued stock target compilation.");
            Settings.Current.ProcessedExternalManifests = targets.Take(1).ToArray();
            ExpectMissing(document, instance);
            Require(instance.ValidatedReferencedInstances == null, "External mapping failure left a stale validated table.");
            Settings.Current.ProcessedExternalManifests = targets;
            Prepare(document, instance);
            Require(instance.ValidatedReferencedInstances!.Count == 3 && instance.AllDependentInstances!.Count == 0, "Restored mapping did not recover all ordered dependencies.");
            // Reborn: file checks must also repeat after a previously valid strong-reference pass.
            instance.ReferencedFiles.Add("MissingDependency.dat");
            ExpectMissing(document, instance, ErrorCode.FileNotFound);
            ExpectMissing(document, instance, ErrorCode.FileNotFound);
            instance.ReferencedFiles.Clear();
            Prepare(document, instance);
            Require(instance.ValidatedReferencedInstances!.Count == 3, "Missing-file recovery lost ordered external dependencies.");
            Require(!Directory.EnumerateFiles(directory, "*.bin").Any(), "Dependency metadata test emitted native streams.");
            Console.WriteLine("Dependency resolution self-test: OK (real AddOutputInstance, repeated missing-target failure, ordered external resolution, mapping removal/restoration, no native writes)");
        }
        finally { Settings.Current = saved; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: initialize the same per-attempt output set as PrepareOutputInstances and invoke only its dependency stage. */
    //-------------------------------------------------------------------------------------------------
    private static void Prepare(AssetDeclarationDocument document, InstanceDeclaration instance)
    {
        object state = typeof(AssetDeclarationDocument).GetField("_current", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(document)!;
        state.GetType().GetField("OutputInstanceSet")!.SetValue(state, new SortedDictionary<InstanceHandle, InstanceDeclaration>());
        MethodInfo method = typeof(AssetDeclarationDocument).GetMethod("AddOutputInstance", BindingFlags.NonPublic | BindingFlags.Instance)!;
        try { method.Invoke(document, new object[] { instance }); }
        catch (TargetInvocationException error) when (error.InnerException != null)
        { ExceptionDispatchInfo.Capture(error.InnerException).Throw(); throw; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retries must retain strict UnknownReference errors rather than trusting incomplete prior dependency validation. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectMissing(AssetDeclarationDocument document, InstanceDeclaration instance, ErrorCode expected = ErrorCode.UnknownReference)
    {
        try { Prepare(document, instance); }
        catch (BinaryAssetBuilderException error) when (error.ErrorCode == expected)
        {
            object state = typeof(AssetDeclarationDocument).GetField("_current", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(document)!;
            var visited = (SortedDictionary<InstanceHandle, InstanceDeclaration>)state.GetType().GetField("OutputInstanceSet")!.GetValue(state)!;
            Require(!visited.ContainsKey(instance.Handle) && instance.ValidatedReferencedInstances == null && instance.AllDependentInstances == null,
                "Failed attempt retained a visited marker or partial dependency state.");
            return;
        }
        throw new InvalidDataException("Missing dependency was accepted on repeated preparation or mapping removal.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop metadata dependency proof at the first stale-order or retry invariant violation. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
}
