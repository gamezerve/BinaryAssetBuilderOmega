using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: route the validated no-import modifier subset through actual descriptors, registry and document stages with all production/cache gates closed.
internal static class Ep1AttributeModifierProfileSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove compiler entry against eight native fixtures, policy/platform isolation and fresh full-document processing. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        XmlDocument config = new();
        config.Load(Path.Combine(fixtures, "Ep1AttributeModifierProfile.xml"));
        XmlNamespaceManager namespaces = new(config.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Settings settings = new();
        settings.ReadXml(new Node(config.DocumentElement!.CreateNavigator(), namespaces));
        PluginRegistry registry = new(settings.Plugins, TargetPlatform.Win32);
        Require(registry.DefaultPlugin is Ra3Ep1AttributeModifierPlugin && !settings.BuildCache && !registry.CanReuseCompiledDocuments,
            "Explicit settings did not select the isolated modifier profile or disabled-cache policy.");
        ExtendedTypeInformation info = registry.GetExtendedTypeInformation(0xC5E07887u);
        Require(info.TypeHash == 0x74425C11u && !info.Tokenized && !info.UseBuildCache && !info.HasCustomData
            && info.ProcessingHash == (0x74425C11u ^ 0x45503111u) && registry.DefaultPlugin.AllTypesHash == 0x5454A8E9u,
            "Modifier identity or native/processor policy differs.");
        XmlSchemaSet schemas = new() { XmlResolver = new XmlUrlResolver() };
        schemas.Add(null, Path.Combine(fixtures, "AttributeModifierPipeline.xsd"));
        schemas.Compile();
        XmlDocument fixture = new();
        fixture.Load(Path.Combine(fixtures, "AttributeModifierProbe.xml"));
        foreach (XmlElement root in fixture.DocumentElement!.ChildNodes.OfType<XmlElement>())
        {
            InstanceDeclaration declaration = Declare(root.OuterXml, schemas);
            AssetBuffer buffer = registry.DefaultPlugin.ProcessInstance(declaration);
            Relo.Chunk expected = AttributeModifierNativeSmokeTest.Compile(schemas, root.OuterXml);
            Require(buffer.InstanceData.SequenceEqual(expected.InstanceBuffer) && buffer.RelocationData.SequenceEqual(expected.RelocationBuffer)
                && buffer.ImportsData.SequenceEqual(expected.ImportsBuffer), "Profile compiler entry differs from a golden-tested native modifier.");
        }
        ExpectPolicy(() => registry.ValidateProductionOutput());
        DocumentProcessor guarded = new(settings, registry, null!);
        ExpectPolicy(() => { using OutputManager manager = new(guarded, null!, "unused", "unused", null!, null!, Array.Empty<string>()); });
        foreach (string unsupported in new[]
        {
            """<AttributeModifier xmlns="uri:ea.com:eala:asset" id="Import" StartFX="FX_VoiceSuppressed" />""",
            """<AttributeModifier xmlns="uri:ea.com:eala:asset" id="Import" EndFX="FX_VoiceSuppressed" />""",
            """<AttributeModifier xmlns="uri:ea.com:eala:asset" id="Import" Shader="SomeShader" />""",
            """<AttributeModifier xmlns="uri:ea.com:eala:asset" id="Inherited" inheritFrom="BaseModifier" />"""
        }) ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(Declare(unsupported, schemas)));
        // Reborn: inject a formula after valid schema annotation so the profile guard, not schema validation, must reject it.
        InstanceDeclaration formula = Declare("""<AttributeModifier xmlns="uri:ea.com:eala:asset" id="Formula" />""", schemas);
        formula.XmlNode.Attributes!["Duration"]!.Value = "=Unresolved";
        ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(formula));
        // Reborn: no-import mode must also reject a valid XML id edited after declaration identity was computed.
        InstanceDeclaration staleIdentity = Declare("""<AttributeModifier xmlns="uri:ea.com:eala:asset" id="OriginalIdentity" />""", schemas);
        ((XmlElement)staleIdentity.XmlNode).SetAttribute("id", "ChangedIdentity");
        ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(staleIdentity));
        // Reborn: stale schema annotations cannot authorize an invalid value inserted after validation.
        InstanceDeclaration invalidated = Declare("""<AttributeModifier xmlns="uri:ea.com:eala:asset" id="Invalidated" />""", schemas);
        invalidated.XmlNode.Attributes!["StackingLimit"]!.Value = "not-an-integer";
        bool schemaRejected = false;
        try { registry.DefaultPlugin.ProcessInstance(invalidated); }
        catch (XmlSchemaValidationException) { schemaRejected = true; }
        Require(schemaRejected, "Post-validation mutation bypassed detached schema revalidation.");
        ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(Declare("""<AttributeModifier xmlns="uri:ea.com:eala:asset" id="NoValidation" />""")));
        ExpectRejected(() => registry.GetExtendedTypeInformation(0x3A6C5E8Eu));
        InstanceDeclaration prefixed = Declare("""<p:AttributeModifier xmlns:p="uri:ea.com:eala:asset" id="Prefix" />""", schemas);
        Require(prefixed.Handle.TypeId == 0xC5E07887u && registry.DefaultPlugin.ProcessInstance(prefixed).InstanceData.Length == 56,
            "Namespace prefix changed modifier identity or default native output.");
        // Reborn: even type-specific settings requesting cache reuse cannot override the processor policy.
        PluginDescriptor mapped = new() { IsEnabled = true, AssetTypes = "AttributeModifier", UseBuildCache = true,
            QualifiedName = settings.Plugins.Single().QualifiedName };
        PluginRegistry mappedRegistry = new(new[] { mapped }, TargetPlatform.Win32);
        Require(!mappedRegistry.GetExtendedTypeInformation(0xC5E07887u).UseBuildCache && !mappedRegistry.CanReuseCompiledDocuments,
            "Explicit mapping bypassed modifier cache policy.");
        ExpectPolicy(() => mappedRegistry.ValidateProductionOutput());
        Ra3Ep1AttributeModifierPlugin isolated = new();
        ExpectRejected(() => isolated.GetExtendedTypeInformation(0xC5E07887u));
        isolated.Initialize(TargetPlatform.Win32);
        isolated.GetExtendedTypeInformation(0xC5E07887u).TypeHash = 0;
        Require(isolated.GetExtendedTypeInformation(0xC5E07887u).TypeHash == 0x74425C11u, "Caller mutated isolated profile metadata.");
        ExpectRejected(() => isolated.ReInitialize(TargetPlatform.Xbox360));
        ExpectRejected(() => isolated.GetExtendedTypeInformation(0xC5E07887u));
        isolated.ReInitialize(TargetPlatform.Win32);
        Require(isolated.ProcessInstance(prefixed).InstanceData.Length == 56, "Win32 reinitialization failed.");
        ExpectRejected(() => isolated.Initialize(TargetPlatform.PlayStation3));
        TestDocument(fixtures, registry);
        Console.WriteLine("EP1 modifier profile self-test: OK (eight native compiler entries, explicit registry, standalone/schema/platform gates, production/cache guards, fresh document stages)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate actual document defaults, identity metadata and nested TypeId injection without session or precompiled reuse. */
    //-------------------------------------------------------------------------------------------------
    private static void TestDocument(string fixtures, PluginRegistry registry)
    {
        string directory = Path.Combine(Path.GetTempPath(), "Reborn-Ep1ModifierProfile-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        Settings previous = Settings.Current;
        try
        {
            Settings.Current = new Settings { BuildCache = false, SchemaPath = Path.Combine(fixtures, "AttributeModifierPipeline.xsd"),
                DataRoot = directory, DataPaths = new[] { directory }, TargetPlatform = TargetPlatform.Win32,
                CustomPostfix = "", StreamPostfix = "", StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            string source = Path.Combine(directory, "modifiers.xml");
            File.WriteAllText(source, File.ReadAllText(Path.Combine(fixtures, "AttributeModifierProbe.xml")));
            SessionCache cache = new();
            cache.InitializeCache(new List<string>());
            DocumentProcessor processor = new(Settings.Current, registry, new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32))
                { Cache = cache, SchemaSet = new SchemaSet(false) };
            var options = new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = true };
            AssetDeclarationDocument first = processor.ProcessDocumentInternal(source, source, null!, options);
            Require(first.SelfInstances.Count == 8, "Full document modifier count differs.");
            var bytes = first.SelfInstances.ToDictionary(instance => instance.Handle.InstanceId,
                instance => registry.DefaultPlugin.ProcessInstance(instance).InstanceData);
            InstanceDeclaration orange = first.SelfInstances.Single(instance => instance.Handle.InstanceName == "AttributeModifier_RedAlert_Orange");
            Require(orange.XmlNode.Attributes!["StackingLimit"]!.Value == "1" && orange.Handle.TypeHash == 0x74425C11u,
                "Full document defaults or EP1 type metadata differ.");
            // Reborn: unchanged source must replace a poisoned declaration rather than silently enable experimental session reuse.
            orange.XmlNode.Attributes["StackingLimit"]!.Value = "999";
            AssetDeclarationDocument repeated = processor.ProcessDocumentInternal(source, source, null!, options);
            Require(repeated.SelfInstances.All(instance => registry.DefaultPlugin.ProcessInstance(instance).InstanceData.SequenceEqual(bytes[instance.Handle.InstanceId]))
                && options.UsePrecompiled, "Experimental modifier document reused stale state or mutated caller options.");
            InstanceDeclaration tampered = repeated.SelfInstances.First();
            tampered.XmlNode.Attributes!["TypeId"]!.Value = "0";
            ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(tampered));
            processor.Cache = null!;
            ExpectPolicy(() => processor.ProcessDocumentInternal("missing", "missing", null!, new DocumentProcessor.ProcessOptions { GenerateOutput = true }));
            MethodInfo precompiled = typeof(DocumentProcessor).GetMethod("LoadPrecompiledReference", BindingFlags.NonPublic | BindingFlags.Instance)!;
            Require(precompiled.Invoke(processor, new object?[] { first, "missing", Array.Empty<string>() }) is false,
                "Experimental modifier precompiled lookup was enabled.");
            Require(!Directory.EnumerateFiles(directory, "*.manifest").Any(), "Profile document proof emitted production output.");
        }
        finally { Settings.Current = previous; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: keep schema-validated and deliberately unvalidated declarations distinct for fail-closed profile tests. */
    //-------------------------------------------------------------------------------------------------
    private static InstanceDeclaration Declare(string xml, XmlSchemaSet? schemas = null)
    {
        XmlDocument document = new() { XmlResolver = null };
        document.LoadXml(xml);
        if (schemas != null) { document.Schemas = schemas; document.Validate((_, args) => throw new XmlSchemaValidationException(args.Message)); }
        return new InstanceDeclaration(new AssetDeclarationDocument()) { XmlNode = document.DocumentElement! };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require explicit unsupported-operation rejection, not incidental null/compiler failures. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectRejected(Action action)
    {
        try { action(); } catch (Exception error) when (error is NotSupportedException or InvalidOperationException) { return; }
        throw new InvalidDataException("Experimental EP1 modifier accepted an unsupported operation.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: production failures must identify the modifier profile before touching output/cache paths. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectPolicy(Action action)
    {
        try { action(); }
        catch (BinaryAssetBuilderException error) when (error.Message.Contains("RA3EP1-AttributeModifier-Experimental-v1", StringComparison.Ordinal)) { return; }
        throw new InvalidDataException("Modifier production request bypassed profile policy.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop isolated modifier-profile proof at its first policy or byte mismatch. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
}
