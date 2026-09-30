using System.Reflection;
using System.Xml;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: exercise the real document pipeline without authorizing experimental production stream writes.
internal static class Ep1ArmorDocumentSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate official schema defaults through document loading/validation/hash stages and force stale-session reload. */
    //-------------------------------------------------------------------------------------------------
    public static void Run(string directory)
    {
        directory = Path.GetFullPath(directory);
        Directory.CreateDirectory(directory);
        Settings saved = Settings.Current;
        try
        {
            string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
            Settings.Current = new Settings { BuildCache = false, SchemaPath = Path.Combine(fixtures, "ArmorDocumentPipeline.xsd"),
                DataRoot = directory, DataPaths = new[] { directory }, TargetPlatform = TargetPlatform.Win32, CustomPostfix = "", StreamPostfix = "",
                StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            PluginDescriptor descriptor = new() { IsEnabled = true, AssetTypes = "#all", UseBuildCache = false,
                QualifiedName = "BinaryAssetBuilder.XmlCompiler.Ra3Ep1ArmorPlugin, BinaryAssetBuilder.XmlCompiler" };
            PluginRegistry plugins = new(new[] { descriptor }, TargetPlatform.Win32);
            SessionCache cache = new();
            cache.InitializeCache(new System.Collections.Generic.List<string>());
            DocumentProcessor processor = new(Settings.Current, plugins, new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32))
                { Cache = cache, SchemaSet = new SchemaSet(false) };
            Require(!plugins.CanReuseCompiledDocuments, "Experimental document reuse was enabled.");
            string source = Path.Combine(directory, "armor-document.xml");
            File.WriteAllText(source, """<AssetDeclaration xmlns="uri:ea.com:eala:asset"><ArmorTemplate id="DocumentArmor" /></AssetDeclaration>""");
            var options = new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = true };
            AssetDeclarationDocument document = processor.ProcessDocumentInternal(source, source, null!, options);
            InstanceDeclaration instance = document.SelfInstances.Single();
            Require(instance.XmlNode.Attributes!["Default"]!.Value == "100" && instance.Handle.TypeId == 0x3A6C5E8Eu
                && instance.ProcessingHash == plugins.GetExtendedTypeInformation(instance.Handle.TypeId).ProcessingHash,
                "Full document pipeline did not insert defaults or bind EP1 identity/processor metadata.");
            AssetBuffer first = plugins.GetPlugin(instance.Handle.TypeId).ProcessInstance(instance);
            Require(first.InstanceData.Length == 128, "Document-to-profile compilation did not produce empty-list armor tokens.");

            // Reborn: poison only the in-memory cached declaration; an unchanged source file must override this stale state.
            instance.XmlNode.Attributes["Default"]!.Value = "5";
            AssetDeclarationDocument repeated = processor.ProcessDocumentInternal(source, source, null!, options);
            InstanceDeclaration fresh = repeated.SelfInstances.Single();
            AssetBuffer second = plugins.GetPlugin(fresh.Handle.TypeId).ProcessInstance(fresh);
            Require(fresh.XmlNode.Attributes!["Default"]!.Value == "100" && first.InstanceData.SequenceEqual(second.InstanceData)
                && options.UsePrecompiled, "Experimental document reused stale state or mutated caller options.");

            // Reborn: populated documents also receive core-generated TypeId attributes on nested Armor records.
            XmlDocument populatedXml = new();
            populatedXml.Load(Path.Combine(fixtures, "ArmorTokenProbe.xml"));
            string populatedSource = Path.Combine(directory, "armor-populated-document.xml");
            File.WriteAllText(populatedSource, "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">" + populatedXml.DocumentElement!.OuterXml + "</AssetDeclaration>");
            AssetDeclarationDocument populated = processor.ProcessDocumentInternal(populatedSource, populatedSource, null!, options);
            InstanceDeclaration populatedInstance = populated.SelfInstances.Single();
            AssetBuffer populatedBuffer = plugins.GetPlugin(populatedInstance.Handle.TypeId).ProcessInstance(populatedInstance);
            var golden = ArmorTokenSmokeTest.Compile(ArmorTokenSmokeTest.LoadSchemas(fixtures), populatedXml.DocumentElement.OuterXml);
            Require(populatedBuffer.InstanceData.SequenceEqual(golden.InstanceBuffer)
                && populatedBuffer.RelocationData.SequenceEqual(golden.RelocationBuffer), "Full populated document differs from golden-tested compiler output.");
            populatedInstance.XmlNode.Attributes!["TypeId"]!.Value = "0";
            bool wrongIdentityRejected = false;
            try { plugins.GetPlugin(populatedInstance.Handle.TypeId).ProcessInstance(populatedInstance); }
            catch (NotSupportedException) { wrongIdentityRejected = true; }
            Require(wrongIdentityRejected, "Profile accepted a tampered core-injected TypeId.");

            // Reborn: replacing cached documents must retain Windows case-insensitive circular-inclusion detection.
            var stack = (System.Collections.Generic.List<string>)typeof(DocumentProcessor)
                .GetField("_documentStack", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(processor)!;
            Require(stack.Count == 0, "Successful document processing left an inclusion stack entry.");
            stack.Add(source.ToUpperInvariant());
            bool cycleRejected = false;
            try { processor.ProcessDocumentInternal(source, source, null!, options); }
            catch (BinaryAssetBuilderException exception) when (exception.ErrorCode == ErrorCode.CircularDependency) { cycleRejected = true; }
            finally { stack.RemoveAt(0); }
            Require(cycleRejected, "Fresh experimental documents bypassed circular-inclusion detection.");

            // Reborn: production requests must reject before cache/file access, including the stream-hint fast path.
            processor.Cache = null!;
            ExpectPolicy(() => processor.ProcessDocumentInternal("missing", "missing", null!,
                new DocumentProcessor.ProcessOptions { GenerateOutput = true, UsePrecompiled = true }));
            MethodInfo precompiled = typeof(DocumentProcessor).GetMethod("LoadPrecompiledReference", BindingFlags.Instance | BindingFlags.NonPublic)!;
            Require(precompiled.Invoke(processor, new object?[] { document, "missing", Array.Empty<string>() }) is false,
                "Experimental precompiled reference lookup was not disabled before file access.");
            Require(!File.Exists(Path.ChangeExtension(source, ".manifest")), "Document test unexpectedly emitted a production manifest.");
            Console.WriteLine("EP1 armor document self-test: OK (official base schema, document defaults/hash, compiler entry, forced session reload, production/precompiled guards)");
        }
        finally { Settings.Current = saved; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: a production denial must identify profile policy rather than a missing cache, schema or file. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectPolicy(Action action)
    {
        try { action(); }
        catch (BinaryAssetBuilderException exception) when (exception.Message.Contains("RA3EP1-Armor-Experimental-v1", StringComparison.Ordinal)) { return; }
        throw new InvalidDataException("Document production request bypassed experimental profile policy.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject any false document/cache proof at its first failed invariant. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
}
