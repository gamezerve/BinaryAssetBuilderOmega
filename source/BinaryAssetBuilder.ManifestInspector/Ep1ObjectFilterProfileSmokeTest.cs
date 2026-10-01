using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove a bounded filter compiler profile through actual descriptor/document stages without activating production output or cache reuse.
internal static class Ep1ObjectFilterProfileSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: match eleven compiler entries to stock-tested native buffers and reject stale weak metadata or expanded eligibility. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        string directory = Path.Combine(Path.GetTempPath(), "Reborn-Ep1ObjectFilterProfile-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        Settings previous = Settings.Current;
        try
        {
            XmlDocument config = new();
            config.Load(Path.Combine(fixtures, "Ep1ObjectFilterProfile.xml"));
            XmlNamespaceManager namespaces = new(config.NameTable);
            namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
            Settings settings = new();
            settings.ReadXml(new Node(config.DocumentElement!.CreateNavigator(), namespaces));
            settings.SchemaPath = Path.Combine(fixtures, "ObjectFilterPipeline.xsd");
            settings.DataRoot = directory; settings.DataPaths = new[] { directory }; settings.TargetPlatform = TargetPlatform.Win32;
            settings.CustomPostfix = ""; settings.StreamPostfix = ""; settings.StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>();
            Settings.Current = settings;
            PluginRegistry registry = new(settings.Plugins, TargetPlatform.Win32);
            Require(registry.DefaultPlugin is Ra3Ep1ObjectFilterPlugin && !settings.BuildCache && !registry.CanReuseCompiledDocuments,
                "Filter descriptor did not isolate its processor or cache policy.");
            ExtendedTypeInformation info = registry.GetExtendedTypeInformation(0x44A5973Du);
            Require(info.TypeHash == 0xDF72B4BAu && info.ProcessingHash == (0xDF72B4BAu ^ 0x45503121u)
                && !info.Tokenized && !info.UseBuildCache && !info.HasCustomData && registry.DefaultPlugin.AllTypesHash == 0x5454A8E9u,
                "Filter profile identity/native policy differs.");
            ExpectPolicy(registry.ValidateProductionOutput);
            PluginDescriptor mapped = new() { IsEnabled = true, AssetTypes = "ObjectFilterAsset", UseBuildCache = true,
                QualifiedName = settings.Plugins.Single().QualifiedName };
            PluginRegistry mappedRegistry = new(new[] { mapped }, TargetPlatform.Win32);
            Require(!mappedRegistry.GetExtendedTypeInformation(0x44A5973Du).UseBuildCache && !mappedRegistry.CanReuseCompiledDocuments,
                "Explicit filter mapping bypassed cache policy.");
            ExpectPolicy(mappedRegistry.ValidateProductionOutput);
            SessionCache cache = new();
            cache.InitializeCache(new List<string>());
            DocumentProcessor processor = new(settings, registry, new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32))
                { Cache = cache, SchemaSet = new SchemaSet(false) };
            string source = Path.Combine(directory, "filters.xml");
            File.WriteAllText(source, File.ReadAllText(Path.Combine(fixtures, "ObjectFilterProbe.xml")));
            var options = new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = true };
            AssetDeclarationDocument first = processor.ProcessDocumentInternal(source, source, null!, options);
            Require(first.SelfInstances.Count == 11, "Filter profile document count differs.");
            XmlDocument fixture = new(); fixture.Load(Path.Combine(fixtures, "ObjectFilterProbe.xml"));
            var schemas = first.XmlDocument.Schemas;
            Dictionary<string, byte[]> bytes = new(StringComparer.Ordinal);
            foreach (InstanceDeclaration instance in first.SelfInstances)
            {
                XmlElement original = fixture.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(element => element.GetAttribute("id") == instance.Handle.InstanceName);
                var expected = ObjectFilterNativeSmokeTest.Compile(schemas, original.OuterXml);
                AssetBuffer actual = registry.DefaultPlugin.ProcessInstance(instance);
                Require(instance.Handle.TypeHash == 0xDF72B4BAu && actual.InstanceData.SequenceEqual(expected.InstanceBuffer)
                    && actual.RelocationData.SequenceEqual(expected.RelocationBuffer) && actual.ImportsData.Length == 0,
                    "Filter compiler entry differs from stock-tested native output.");
                bytes.Add(instance.Handle.Name, actual.InstanceData);
            }
            InstanceDeclaration enter = first.SelfInstances.Single(value => value.Handle.InstanceName == "InfiltrationCanEnterObjectFilter");
            CheckWeakTampering(registry.DefaultPlugin, enter);
            string originalId = ((XmlElement)enter.XmlNode).GetAttribute("id");
            ((XmlElement)enter.XmlNode).SetAttribute("id", "ChangedIdentity");
            ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(enter));
            ((XmlElement)enter.XmlNode).SetAttribute("id", originalId);
            InstanceDeclaration unvalidated = new(new AssetDeclarationDocument()) { XmlNode = fixture.DocumentElement!.ChildNodes.OfType<XmlElement>().First() };
            ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(unvalidated));
            XmlElement filter = enter.XmlNode.ChildNodes.OfType<XmlElement>().Single();
            // Reborn: stale schema annotations cannot authorize broader rules, status masks, inheritance or formulas.
            foreach (var mutation in new[] { (Name: "Rule", Value: "ANY"), (Name: "Relationship", Value: "ALLIES"),
                (Name: "Alignment", Value: "GOOD"), (Name: "Include", Value: "ALL"), (Name: "Exclude", Value: "INFANTRY"),
                (Name: "StatusBitFlags", Value: "NO_BRIBE"), (Name: "Rule", Value: "=Unresolved") })
            {
                bool existed = filter.HasAttribute(mutation.Name); string saved = filter.GetAttribute(mutation.Name);
                filter.SetAttribute(mutation.Name, mutation.Value);
                try { ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(enter)); }
                finally { if (existed) filter.SetAttribute(mutation.Name, saved); else filter.RemoveAttribute(mutation.Name); }
            }
            ((XmlElement)enter.XmlNode).SetAttribute("inheritFrom", "BaseFilter");
            ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(enter));
            ((XmlElement)enter.XmlNode).RemoveAttribute("inheritFrom");
            filter.GetElementsByTagName("IncludeThing").OfType<XmlElement>().First().SetAttribute("TypeId", "0");
            ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(enter));
            // Reborn: a cached poisoned declaration must be replaced by unchanged source, including checked injected leaf identities.
            AssetDeclarationDocument repeated = processor.ProcessDocumentInternal(source, source, null!, options);
            Require(options.UsePrecompiled && repeated.SelfInstances.All(instance => registry.DefaultPlugin.ProcessInstance(instance).InstanceData.SequenceEqual(bytes[instance.Handle.Name])),
                "Filter profile reused poisoned document state or mutated caller options.");
            processor.Cache = null!;
            ExpectPolicy(() => processor.ProcessDocumentInternal("missing", "missing", null!, new DocumentProcessor.ProcessOptions { GenerateOutput = true }));
            Ra3Ep1ObjectFilterPlugin isolated = new();
            ExpectRejected(() => isolated.GetExtendedTypeInformation(0x44A5973Du));
            isolated.Initialize(TargetPlatform.Win32);
            isolated.GetExtendedTypeInformation(0x44A5973Du).TypeHash = 0;
            Require(isolated.GetExtendedTypeInformation(0x44A5973Du).TypeHash == 0xDF72B4BAu, "Caller mutated profile metadata.");
            ExpectRejected(() => isolated.GetExtendedTypeInformation(0xC5E07887u));
            ExpectRejected(() => isolated.ReInitialize(TargetPlatform.Xbox360));
            ExpectRejected(() => isolated.GetExtendedTypeInformation(0x44A5973Du));
            isolated.ReInitialize(TargetPlatform.Win32);
            Require(isolated.ProcessInstance(repeated.SelfInstances.First()).InstanceData.Length >= 124, "Filter Win32 reinitialization failed.");
            Require(!Directory.EnumerateFiles(directory, "*.manifest").Any(), "Filter profile proof emitted production output.");
            Console.WriteLine("EP1 ObjectFilter profile self-test: OK (descriptor/document/eleven compiler entries, weak identity/order checks, narrow eligibility, fresh source reload, platform/production/cache gates)");
        }
        finally { Settings.Current = previous; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: checked weak leaves must not silently compile with a reordered, wrong-type, missing or extra metadata table. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckWeakTampering(IAssetBuilderPlugin plugin, InstanceDeclaration instance)
    {
        InstanceHandle[] saved = instance.WeakReferencedInstances.ToArray();
        foreach (InstanceHandle[] invalid in new[] { saved.Reverse().ToArray(), saved.Take(1).ToArray(),
            new[] { new InstanceHandle("FXList", saved[0].InstanceName), saved[1] },
            new[] { new InstanceHandle("GameObject", "WrongWeakName"), saved[1] }, saved.Concat(new[] { saved[0] }).ToArray() })
        {
            instance.WeakReferencedInstances.Clear(); instance.WeakReferencedInstances.AddRange(invalid);
            try { ExpectRejected(() => plugin.ProcessInstance(instance)); }
            finally { instance.WeakReferencedInstances.Clear(); instance.WeakReferencedInstances.AddRange(saved); }
        }
        instance.ReferencedInstances.Add(new InstanceHandle("FXList", "UnexpectedStrong"));
        try { ExpectRejected(() => plugin.ProcessInstance(instance)); }
        finally { instance.ReferencedInstances.Clear(); }
        instance.ReferencedFiles.Add("UnexpectedFile");
        try { ExpectRejected(() => plugin.ProcessInstance(instance)); }
        finally { instance.ReferencedFiles.Clear(); }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: unsupported profile operations must fail explicitly instead of incidental parser/null failures. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectRejected(Action action)
    {
        try { action(); }
        catch (Exception error) when (error is NotSupportedException or InvalidOperationException) { return; }
        throw new InvalidDataException("Filter profile accepted unsupported metadata or controls.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: production rejection must identify the profile before output/cache/source access. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectPolicy(Action action)
    {
        try { action(); }
        catch (BinaryAssetBuilderException error) when (error.Message.Contains("RA3EP1-ObjectFilter-Experimental-v1", StringComparison.Ordinal)) { return; }
        throw new InvalidDataException("Filter production request bypassed profile policy.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop compiler-profile proof at its first identity, policy or byte mismatch. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
}
