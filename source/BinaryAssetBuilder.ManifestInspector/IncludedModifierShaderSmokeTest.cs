using System.Buffers.Binary;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;
using BinaryAssetBuilder.Utility;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: integrate real nested includes with local shader and metadata-only external FX dependencies, never unported FX compilation.
internal static class IncludedModifierShaderSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: test both instance/all inclusion locations, nested reload, missing inputs and mixed local/external selector identities. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        string directory = Path.Combine(Path.GetTempPath(), "Reborn-IncludedModifierShader-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        Settings saved = Settings.Current;
        try
        {
            foreach (string mode in new[] { "instance", "all" }) CheckMode(fixtures, Path.Combine(directory, mode), mode);
            Console.WriteLine("Included modifier/shader self-test: OK (nested instance/all includes, mixed local shader/external FX selectors, leaf edits and loss/recovery, external removal/refresh/recovery, reference-output policy; no production streams)");
        }
        finally { Settings.Current = saved; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: run each include mode in an independent real cache/document graph and retain separate external metadata versus native evidence. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckMode(string fixtures, string directory, string mode)
    {
        Directory.CreateDirectory(directory);
        Settings.Current = new Settings { BuildCache = false, ErrorLevel = 1, SchemaPath = Path.Combine(fixtures, "ModifierShaderPipeline.xsd"),
            DataRoot = directory, DataPaths = new[] { directory }, TargetPlatform = TargetPlatform.Win32,
            CustomPostfix = "", StreamPostfix = "", StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>(),
            ProcessedExternalManifests = Array.Empty<string>() };
        Ra3Ep1AttributeModifierPlugin modifierPlugin = new(true); modifierPlugin.Initialize(TargetPlatform.Win32);
        Ra3Ep1ShaderOverridePlugin shaderPlugin = new(); shaderPlugin.Initialize(TargetPlatform.Win32);
        PluginRegistry plugins = new(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32);
        plugins.AddPlugin(0xC5E07887u, modifierPlugin); plugins.AddPlugin(0xBCC23F6Cu, shaderPlugin);
        SessionCache cache = new(); cache.InitializeCache(new List<string>());
        DocumentProcessor processor = new(Settings.Current, plugins, new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32))
            { Cache = cache, SchemaSet = new SchemaSet(false) };
        string parent = Path.Combine(directory, "parent.xml"), child = Path.Combine(directory, "child.xml"), leaf = Path.Combine(directory, "leaf.xml");
        string leafXml = """
            <AssetDeclaration xmlns="uri:ea.com:eala:asset"><ShaderOverride id="RebornIncludedShader" Priority="50">
            <Rule ReplaceShaderName="Null.fx" ReplaceTechniqueName="Default" />
            </ShaderOverride></AssetDeclaration>
            """;
        File.WriteAllText(leaf, leafXml);
        File.WriteAllText(child, """
            <AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes><Include type="all" source="leaf.xml" /></Includes></AssetDeclaration>
            """);
        string parentXml = "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\"><Includes><Include type=\"" + mode
            + "\" source=\"child.xml\" /></Includes><AttributeModifier id=\"RebornIncludedModifier\" StartFX=\"RebornExternalFX\" Shader=\"RebornIncludedShader\" /></AssetDeclaration>";
        File.WriteAllText(parent, parentXml);
        InstanceHandle external = new("FXList", "RebornExternalFX");
        string manifest = Path.Combine(directory, "external.manifest");
        ExternalLinkSmokeTest.WriteFixture(manifest, external, new ReferencedFileBuffer());
        Settings.Current.ProcessedExternalManifests = new[] { manifest };
        var options = new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = true };
        AssetDeclarationDocument document = processor.ProcessDocumentInternal(parent, parent, null!, options);
        AssetBuffer initial = CheckGraph(document, plugins, external, mode, leaf);
        // Reborn: change only the grandchild XML and require a new literal native word through the unchanged parent include graph.
        File.WriteAllText(leaf, leafXml.Replace("Priority=\"50\"", "Priority=\"51\"", StringComparison.Ordinal));
        AssetDeclarationDocument changed = processor.ProcessDocumentInternal(parent, parent, null!, options);
        AssetBuffer updated = CheckGraph(changed, plugins, external, mode, leaf);
        Require(Read(initial.InstanceData, 4) == 50 && Read(updated.InstanceData, 4) == 51
            && initial.InstanceData.AsSpan(0, 4).SequenceEqual(updated.InstanceData.AsSpan(0, 4))
            && initial.InstanceData.AsSpan(8).SequenceEqual(updated.InstanceData.AsSpan(8))
            && initial.RelocationData.SequenceEqual(updated.RelocationData), "Grandchild edit did not reload through unchanged include ancestors.");
        InstanceDeclaration modifier = changed.SelfInstances.Single();
        Settings.Current.ProcessedExternalManifests = Array.Empty<string>();
        ExpectMissingReference(changed, modifier); ExpectMissingReference(changed, modifier);
        Settings.Current.ProcessedExternalManifests = new[] { manifest };
        CheckGraph(changed, plugins, external, mode, leaf);
        // Reborn: a same-path manifest identity replacement must invalidate the external target rather than reuse its old index.
        ExternalLinkSmokeTest.WriteFixture(manifest, new InstanceHandle("FXList", "RebornDifferentFX"), new ReferencedFileBuffer());
        ExpectMissingReference(changed, modifier); ExpectMissingReference(changed, modifier);
        ExternalLinkSmokeTest.WriteFixture(manifest, external, new ReferencedFileBuffer());
        CheckGraph(changed, plugins, external, mode, leaf);
        // Reborn: remove only this harness-owned leaf; nested source absence must fail repeatedly and recover after recreation.
        File.Delete(leaf);
        ExpectMissingFile(() => processor.ProcessDocumentInternal(parent, parent, null!, options));
        ExpectMissingFile(() => processor.ProcessDocumentInternal(parent, parent, null!, options));
        File.WriteAllText(leaf, leafXml);
        AssetDeclarationDocument restored = processor.ProcessDocumentInternal(parent, parent, null!, options);
        Require(Read(CheckGraph(restored, plugins, external, mode, leaf).InstanceData, 4) == 50 && options.UsePrecompiled,
            "Nested missing-file recovery retained stale native values or mutated caller options.");
        // Reborn: reference Includes request a child production build under current core semantics; experimental profiles must reject it.
        File.WriteAllText(parent, parentXml.Replace("type=\"" + mode + "\"", "type=\"reference\"", StringComparison.Ordinal));
        ExpectPolicy(() => processor.ProcessDocumentInternal(parent, parent, null!, options));
        // Reborn: real inclusion cycles and schema failures must still reject repeatedly, then recover without stale processing flags.
        File.WriteAllText(parent, parentXml);
        string cycle = "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\"><Includes><Include type=\"all\" source=\"parent.xml\" /></Includes></AssetDeclaration>";
        File.WriteAllText(leaf, cycle);
        ExpectDocumentError(() => processor.ProcessDocumentInternal(parent, parent, null!, options), ErrorCode.CircularDependency);
        ExpectDocumentError(() => processor.ProcessDocumentInternal(parent, parent, null!, options), ErrorCode.CircularDependency);
        File.WriteAllText(leaf, leafXml.Replace("Priority=\"50\"", "Priority=\"invalid\"", StringComparison.Ordinal));
        ExpectDocumentError(() => processor.ProcessDocumentInternal(parent, parent, null!, options), ErrorCode.SchemaValidation);
        ExpectDocumentError(() => processor.ProcessDocumentInternal(parent, parent, null!, options), ErrorCode.SchemaValidation);
        File.WriteAllText(leaf, leafXml);
        CheckGraph(processor.ProcessDocumentInternal(parent, parent, null!, options), plugins, external, mode, leaf);
        // Reborn: the shared stack fix must also preserve retries with ordinary resident document reuse, not only fresh experimental documents.
        SessionCache reusableCache = new(); reusableCache.InitializeCache(new List<string>());
        DocumentProcessor reusable = new(Settings.Current, new PluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32),
            new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32)) { Cache = reusableCache, SchemaSet = new SchemaSet(false) };
        File.WriteAllText(leaf, cycle);
        ExpectDocumentError(() => reusable.ProcessDocumentInternal(parent, parent, null!, options), ErrorCode.CircularDependency);
        ExpectDocumentError(() => reusable.ProcessDocumentInternal(parent, parent, null!, options), ErrorCode.CircularDependency);
        File.WriteAllText(leaf, leafXml);
        Require(reusable.ProcessDocumentInternal(parent, parent, null!, options).SelfInstances.Count == 1,
            "Ordinary cached document processing could not recover after a real cycle.");
        Require(!Directory.EnumerateFiles(directory, "*.bin").Any() && Directory.EnumerateFiles(directory, "*.manifest").Count() == 1,
            "Include diagnostic wrote streams or an unexpected manifest.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify real inclusion location, mixed ordered resolution and each native import selecting its exact local or external target. */
    //-------------------------------------------------------------------------------------------------
    private static AssetBuffer CheckGraph(AssetDeclarationDocument document, PluginRegistry plugins, InstanceHandle external, string mode, string leaf)
    {
        Require(document.SelfInstances.Count == 1, "Include graph unexpectedly inlined shader XML into parent self roots.");
        InstanceDeclaration modifier = document.SelfInstances.Single();
        InstanceDeclaration shader = document.FindInstance(new InstanceHandle("ShaderOverride", "RebornIncludedShader"),
            FindLocation.None, out FindLocation location);
        Require(shader != null && location == (mode == "instance" ? FindLocation.Tentative : FindLocation.All)
            && string.Equals(shader.Document.SourcePath, leaf, StringComparison.OrdinalIgnoreCase),
            "Nested Include target has the wrong source document or inclusion location.");
        DependencyResolutionSmokeTest.Prepare(document, modifier);
        Require(modifier.ReferencedInstances.Count == 2 && modifier.ValidatedReferencedInstances!.Count == 2
            && modifier.AllDependentInstances!.Count == 1 && modifier.AllDependentInstances.Single() == shader!.Handle
            && DependencyResolutionSmokeTest.Visited(document).Count == 2,
            "Mixed include graph queued external FX compilation or lost local closure.");
        AssetBuffer compiled = plugins.GetPlugin(modifier.Handle.TypeId).ProcessInstance(modifier);
        Require(compiled.InstanceData.Length == 60 && compiled.RelocationData.Length == 8 && compiled.ImportsData.Length == 12,
            "Mixed local/external modifier native buffer sizes differ.");
        uint fxIndex = Read(compiled.InstanceData, 12) - 1, shaderPointer = Read(compiled.InstanceData, 40);
        Require(shaderPointer == 56 && fxIndex < 2, "Mixed modifier reference offsets/indices differ.");
        uint shaderIndex = Read(compiled.InstanceData, (int)shaderPointer) - 1;
        Require(shaderIndex < 2 && shaderIndex != fxIndex
            && modifier.ValidatedReferencedInstances[(int)fxIndex].TypeId == external.TypeId
            && modifier.ValidatedReferencedInstances[(int)fxIndex].InstanceId == external.InstanceId
            && modifier.ValidatedReferencedInstances[(int)shaderIndex] == shader!.Handle
            && new[] { Read(compiled.ImportsData, 0), Read(compiled.ImportsData, 4) }.Order().SequenceEqual(new uint[] { 12, 56 })
            && Read(compiled.ImportsData, 8) == uint.MaxValue
            && Read(compiled.RelocationData, 0) == 40 && Read(compiled.RelocationData, 4) == uint.MaxValue,
            "Mixed native selectors do not match the ordered validated local/external identities.");
        AssetBuffer target = plugins.GetPlugin(shader!.Handle.TypeId).ProcessInstance(shader);
        Require(target.InstanceData.Length == 40 && target.RelocationData.Length == 12 && target.ImportsData.Length == 0,
            "Included shader did not compile through the bounded profile.");
        return target;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: external mapping changes must reject every preparation attempt without retaining partial validated references. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectMissingReference(AssetDeclarationDocument document, InstanceDeclaration modifier)
    {
        try { DependencyResolutionSmokeTest.Prepare(document, modifier); }
        catch (BinaryAssetBuilderException error) when (error.ErrorCode == ErrorCode.UnknownReference)
        {
            Require(modifier.ValidatedReferencedInstances == null && modifier.AllDependentInstances == null
                && !DependencyResolutionSmokeTest.Visited(document).ContainsKey(modifier.Handle), "Missing external target retained partial success.");
            return;
        }
        throw new InvalidDataException("Missing external FX was accepted.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: nested missing source errors must remain FileNotFound rather than silently becoming an empty shader document. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectMissingFile(Action action)
        => ExpectDocumentError(action, ErrorCode.FileNotFound);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve actual nested error categories across failed retries instead of incidental stale-stack/null failures. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectDocumentError(Action action, ErrorCode expected)
    {
        try { action(); }
        catch (BinaryAssetBuilderException error) when (error.ErrorCode == expected) { return; }
        throw new InvalidDataException($"Expected included document error {expected} was not raised.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: experimental mapped processors must reject reference Include child output before any production stream write. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectPolicy(Action action)
    {
        try { action(); }
        catch (BinaryAssetBuilderException error) when (error.Message.Contains("Experimental", StringComparison.Ordinal)) { return; }
        throw new InvalidDataException("Reference Include bypassed experimental output policy.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: decode bounded little-endian native values without host pointer assumptions. */
    //-------------------------------------------------------------------------------------------------
    private static uint Read(byte[] data, int offset) => BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(offset, 4));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop include integration at its first location, identity, reload or native mismatch. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
}
