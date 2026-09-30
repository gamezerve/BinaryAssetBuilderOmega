using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: test the real descriptor/registry/plugin route without inventing a production-ready aggregate registry.
internal static class Ep1ArmorProfileSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify explicit profile loading, exact compiler-entry bytes and fail-closed output/cache/platform policies. */
    //-------------------------------------------------------------------------------------------------
    public static void Run()
    {
        string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        XmlDocument settingsXml = new();
        settingsXml.Load(Path.Combine(fixtures, "Ep1ArmorProfile.xml"));
        XmlNamespaceManager namespaces = new(settingsXml.NameTable);
        namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
        Settings settings = new();
        settings.ReadXml(new Node(settingsXml.DocumentElement!.CreateNavigator()!, namespaces));
        PluginRegistry registry = new(settings.Plugins, TargetPlatform.Win32);
        Require(registry.DefaultPlugin is Ra3Ep1ArmorPlugin && !settings.BuildCache, "Explicit settings did not select the EP1 armor profile.");
        ExtendedTypeInformation info = registry.GetExtendedTypeInformation(0x3A6C5E8Eu);
        Require(info.TypeHash == 0xA0E237D8u && info.Tokenized && !info.UseBuildCache && !info.HasCustomData
            && info.ProcessingHash != (0x6D59C409u ^ 1u) && registry.DefaultPlugin.AllTypesHash == 0x5454A8E9u,
            "EP1 metadata, processor revision or cache policy differs.");
        XmlSchemaSet schemas = ArmorTokenSmokeTest.LoadSchemas(fixtures);
        string xml = File.ReadAllText(Path.Combine(fixtures, "ArmorTokenProbe.xml"));
        InstanceDeclaration declaration = Declare(xml, schemas);
        AssetBuffer output = registry.GetPlugin(declaration.Handle.TypeId).ProcessInstance(declaration);
        Relo.Chunk expected = ArmorTokenSmokeTest.Compile(schemas, xml);
        Require(output.InstanceData.SequenceEqual(expected.InstanceBuffer) && output.RelocationData.SequenceEqual(expected.RelocationBuffer)
            && output.ImportsData.SequenceEqual(expected.ImportsBuffer), "Real plugin output differs from golden-tested tokenization.");
        ExpectProductionBlocked(() => registry.ValidateProductionOutput());
        // Reborn: the real output-manager constructor must stop before touching an existing output path.
        DocumentProcessor processor = new(settings, registry, null!);
        ExpectProductionBlocked(() =>
        {
            using OutputManager manager = new(processor, null!, "invalid-output-path", "invalid-intermediate-path", null!, null!, Array.Empty<string>());
        });
        ExpectRejected(() => registry.GetExtendedTypeInformation(0x942FFF2Du));
        ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(Declare("""<GameObject xmlns="uri:ea.com:eala:asset" id="Unsupported" />""")));
        ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(Declare("""<ArmorTemplate xmlns="uri:ea.com:eala:asset" id="Inherited" inheritFrom="BaseArmor" />""")));
        ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(Declare("""<ArmorTemplate xmlns="uri:ea.com:eala:asset" id="Formula" Default="=Unresolved" />""")));
        ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(Declare("""<ArmorTemplate xmlns="uri:ea.com:eala:asset" id="Unknown" Unexpected="true" />""")));
        ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(Declare("""<ArmorTemplate xmlns="uri:ea.com:eala:asset" id="All"><Armor Damage="ALL" Percent="5" /></ArmorTemplate>""")));

        InstanceDeclaration prefixed = Declare("""<p:ArmorTemplate xmlns:p="uri:ea.com:eala:asset" id="Prefixed" />""", schemas);
        Require(prefixed.Handle.TypeId == 0x3A6C5E8Eu && registry.DefaultPlugin.ProcessInstance(prefixed).InstanceData.Length == 128,
            "An XML namespace prefix changed the asset type identity.");
        InstanceDeclaration prefixedInheritance = Declare("""<p:ArmorTemplate xmlns:p="uri:ea.com:eala:asset" id="InheritedPrefix" inheritFrom="BaseArmor" />""");
        Require(prefixedInheritance.InheritFromHandle.TypeId == 0x3A6C5E8Eu,
            "A namespace prefix changed the unqualified inheritance type identity.");

        // Reborn: even an explicit type mapping with UseBuildCache=true must honor the processor policy.
        PluginDescriptor mapped = new() { IsEnabled = true, AssetTypes = "ArmorTemplate", UseBuildCache = true,
            QualifiedName = settings.Plugins.Single().QualifiedName };
        PluginRegistry mappedRegistry = new(new[] { mapped }, TargetPlatform.Win32);
        Require(!mappedRegistry.GetExtendedTypeInformation(0x3A6C5E8Eu).UseBuildCache, "Settings bypassed experimental cache policy.");
        ExpectProductionBlocked(() => mappedRegistry.ValidateProductionOutput());

        Ra3Ep1ArmorPlugin isolated = new();
        ExpectRejected(() => isolated.GetExtendedTypeInformation(0x3A6C5E8Eu));
        isolated.Initialize(TargetPlatform.Win32);
        ExtendedTypeInformation mutableCopy = isolated.GetExtendedTypeInformation(0x3A6C5E8Eu);
        mutableCopy.TypeHash = 0;
        Require(isolated.GetExtendedTypeInformation(0x3A6C5E8Eu).TypeHash == 0xA0E237D8u, "Caller mutated the profile's registration.");
        ExpectRejected(() => isolated.ReInitialize(TargetPlatform.Xbox360));
        ExpectRejected(() => isolated.GetExtendedTypeInformation(0x3A6C5E8Eu));
        isolated.ReInitialize(TargetPlatform.Win32);
        Require(isolated.ProcessInstance(declaration).InstanceData.SequenceEqual(output.InstanceData), "Reinitialized profile output changed.");
        ExpectRejected(() => isolated.Initialize(TargetPlatform.PlayStation3));
        Console.WriteLine("EP1 armor profile self-test: OK (descriptor/registry/ProcessInstance, exact tokens, prefix identity, output/cache guards, platforms, isolation)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: use the production declaration identity while keeping optional schema validation explicit in negative fixtures. */
    //-------------------------------------------------------------------------------------------------
    private static InstanceDeclaration Declare(string xml, XmlSchemaSet? schemas = null)
    {
        XmlDocument document = new() { XmlResolver = null };
        document.LoadXml(xml);
        if (schemas != null)
        {
            document.Schemas = schemas;
            document.Validate((_, args) => throw new XmlSchemaValidationException(args.Message));
        }
        return new InstanceDeclaration(new AssetDeclarationDocument()) { XmlNode = document.DocumentElement! };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: distinguish intentional profile rejections from unexpected null/reflection/compiler failures. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectRejected(Action action)
    {
        try { action(); }
        catch (Exception exception) when (exception is NotSupportedException or InvalidOperationException or BinaryAssetBuilderException) { return; }
        throw new InvalidDataException("Experimental EP1 armor profile unexpectedly accepted an unsupported operation.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: output tests must fail specifically on profile policy, not on an unrelated constructor/settings error. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectProductionBlocked(Action action)
    {
        try { action(); }
        catch (BinaryAssetBuilderException exception) when (exception.Message.Contains("RA3EP1-Armor-Experimental-v1", StringComparison.Ordinal)
            && exception.Message.Contains("experimental", StringComparison.Ordinal)) { return; }
        throw new InvalidDataException("Production output was not blocked by the explicit EP1 profile policy.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop the profile proof at the first metadata or stream mismatch. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
}
