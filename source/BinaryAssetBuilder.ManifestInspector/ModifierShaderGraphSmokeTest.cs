using System.Buffers.Binary;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;
using BinaryAssetBuilder.Utility;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove real cross-family document normalization, local dependency resolution and compiler entries without production/linker writes.
internal static class ModifierShaderGraphSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: connect two modifiers to local shader roots, validate their native selectors and test target removal, reload and identity tampering. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        string directory = Path.Combine(Path.GetTempPath(), "Reborn-ModifierShaderGraph-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        Settings saved = Settings.Current;
        try
        {
            Settings.Current = new Settings { BuildCache = false, ErrorLevel = 1, SchemaPath = Path.Combine(fixtures, "ModifierShaderPipeline.xsd"),
                DataRoot = directory, DataPaths = new[] { directory }, TargetPlatform = TargetPlatform.Win32, CustomPostfix = "", StreamPostfix = "",
                StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>(), ProcessedExternalManifests = Array.Empty<string>() };
            Ra3Ep1AttributeModifierPlugin modifierPlugin = new(true); modifierPlugin.Initialize(TargetPlatform.Win32);
            Ra3Ep1ShaderOverridePlugin shaderPlugin = new(); shaderPlugin.Initialize(TargetPlatform.Win32);
            PluginRegistry registry = new(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32);
            registry.AddPlugin(0xC5E07887u, modifierPlugin); registry.AddPlugin(0xBCC23F6Cu, shaderPlugin);
            Require(!registry.CanReuseCompiledDocuments && !registry.GetExtendedTypeInformation(0xC5E07887u).UseBuildCache
                && !registry.GetExtendedTypeInformation(0xBCC23F6Cu).UseBuildCache,
                "Cross-family mapping bypassed cache/reuse policy.");
            ExpectPolicy(registry.ValidateProductionOutput);
            SessionCache cache = new(); cache.InitializeCache(new List<string>());
            DocumentProcessor processor = new(Settings.Current, registry, new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32))
                { Cache = cache, SchemaSet = new SchemaSet(false) };
            XmlDocument shaders = new(); shaders.Load(Path.Combine(fixtures, "ShaderOverrideProbe.xml"));
            XmlElement iron = shaders.DocumentElement!.ChildNodes.OfType<XmlElement>()
                .Single(element => element.GetAttribute("id") == "ShaderOverride_ObjectsIronCurtain");
            XmlElement plain = shaders.DocumentElement.ChildNodes.OfType<XmlElement>()
                .Single(element => element.GetAttribute("id") == "ShaderOverride_ObjectsWithoutXrayEffect");
            string source = Path.Combine(directory, "graph.xml");
            string input = """
                <AssetDeclaration xmlns="uri:ea.com:eala:asset">
                <AttributeModifier id="RebornLinkIron" Shader="ShaderOverride_ObjectsIronCurtain" />
                <AttributeModifier id="RebornLinkPlain" Shader="ShaderOverride:ShaderOverride_ObjectsWithoutXrayEffect" />
                """ + iron.OuterXml + plain.OuterXml + "</AssetDeclaration>";
            File.WriteAllText(source, input);
            var options = new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = true };
            AssetDeclarationDocument document = processor.ProcessDocumentInternal(source, source, null!, options);
            Require(document.SelfInstances.Count == 4, "Cross-family document lost roots.");
            InstanceDeclaration a = document.SelfInstances.Single(instance => instance.Handle.InstanceName == "RebornLinkIron");
            InstanceDeclaration b = document.SelfInstances.Single(instance => instance.Handle.InstanceName == "RebornLinkPlain");
            InstanceDeclaration ironTarget = document.SelfInstances.Single(instance => instance.Handle.InstanceName == iron.GetAttribute("id"));
            InstanceDeclaration plainTarget = document.SelfInstances.Single(instance => instance.Handle.InstanceName == plain.GetAttribute("id"));
            CheckEdge(document, registry, a, ironTarget); CheckEdge(document, registry, b, plainTarget);
            foreach (var target in new[] { (Instance: ironTarget, Source: iron), (Instance: plainTarget, Source: plain) })
            {
                var native = ShaderOverrideNativeSmokeTest.Compile(document.XmlDocument.Schemas, target.Source.OuterXml);
                AssetBuffer actual = registry.GetPlugin(target.Instance.Handle.TypeId).ProcessInstance(target.Instance);
                Require(target.Instance.Handle.TypeHash == 0x3D5B1D16u && actual.InstanceData.SequenceEqual(native.InstanceBuffer)
                    && actual.RelocationData.SequenceEqual(native.RelocationBuffer) && actual.ImportsData.Length == 0,
                    "Resolved local shader did not compile through its observed EP1 profile.");
            }
            // Reborn: missing a formerly valid local target must fail on every attempt and recover only after restoration.
            Require(document.SelfInstances.Remove(ironTarget.Handle), "Could not remove local shader target.");
            ExpectMissing(document, a); ExpectMissing(document, a);
            document.SelfInstances.Add(ironTarget); CheckEdge(document, registry, a, ironTarget);
            InstanceHandle original = a.ReferencedInstances[0];
            a.ReferencedInstances[0] = plainTarget.Handle;
            try { ExpectRejected(() => registry.GetPlugin(a.Handle.TypeId).ProcessInstance(a)); }
            finally { a.ReferencedInstances[0] = original; }
            // Reborn: matching dependency slots must not authorize a modifier XML id edited after declaration identity was computed.
            XmlElement modifierRoot = (XmlElement)a.XmlNode;
            string savedId = modifierRoot.GetAttribute("id");
            modifierRoot.SetAttribute("id", "RebornStaleModifierIdentity");
            try { ExpectRejected(() => registry.GetPlugin(a.Handle.TypeId).ProcessInstance(a)); }
            finally { modifierRoot.SetAttribute("id", savedId); }
            // Reborn: source retargeting changes dependency identity although a one-slot import remains encoded as word one.
            File.WriteAllText(source, input.Replace("id=\"RebornLinkIron\" Shader=\"ShaderOverride_ObjectsIronCurtain\"",
                "id=\"RebornLinkIron\" Shader=\"ShaderOverride_ObjectsWithoutXrayEffect\"", StringComparison.Ordinal));
            AssetDeclarationDocument retargeted = processor.ProcessDocumentInternal(source, source, null!, options);
            InstanceDeclaration moved = retargeted.SelfInstances.Single(instance => instance.Handle.InstanceName == "RebornLinkIron");
            InstanceDeclaration newTarget = retargeted.SelfInstances.Single(instance => instance.Handle.InstanceName == plain.GetAttribute("id"));
            CheckEdge(retargeted, registry, moved, newTarget);
            Require(moved.ReferencedInstances[0].InstanceId != ironTarget.Handle.InstanceId && options.UsePrecompiled,
                "Source retargeting retained stale dependency metadata or changed caller options.");
            // Reborn: poisoning a mapped shader declaration must be cured by fresh source reload, not cached native metadata.
            ((XmlElement)newTarget.XmlNode).SetAttribute("TypeId", "0");
            ExpectRejected(() => registry.GetPlugin(newTarget.Handle.TypeId).ProcessInstance(newTarget));
            AssetDeclarationDocument refreshed = processor.ProcessDocumentInternal(source, source, null!, options);
            InstanceDeclaration recovered = refreshed.SelfInstances.Single(instance => instance.Handle.InstanceName == plain.GetAttribute("id"));
            Require(registry.GetPlugin(recovered.Handle.TypeId).ProcessInstance(recovered).InstanceData.Length == 44,
                "Mixed profile reused a poisoned local shader declaration.");
            processor.Cache = null!;
            ExpectPolicy(() => processor.ProcessDocumentInternal("missing", "missing", null!,
                new DocumentProcessor.ProcessOptions { GenerateOutput = true }));
            Require(!Directory.EnumerateFiles(directory, "*.manifest").Any() && !Directory.EnumerateFiles(directory, "*.bin").Any(),
                "Cross-family diagnostic emitted production streams.");
            Console.WriteLine("Modifier/shader graph self-test: OK (registered cross-family documents, forward local references, exact native selectors, target loss/recovery, identity tampering, source retarget/reload; no production/linker output)");
        }
        finally { Settings.Current = saved; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate the actual local dependency closure and decode the final one-biased import against its ordered target identity. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckEdge(AssetDeclarationDocument document, PluginRegistry registry, InstanceDeclaration modifier, InstanceDeclaration shader)
    {
        Require(modifier.Handle.TypeHash == 0x74425C11u && modifier.ReferencedInstances.Count == 1
            && modifier.ReferencedInstances[0].TypeId == shader.Handle.TypeId && modifier.ReferencedInstances[0].InstanceId == shader.Handle.InstanceId
            && ((XmlElement)modifier.XmlNode).GetAttribute("Shader").EndsWith("\\0", StringComparison.Ordinal),
            "Core did not normalize the shader reference to ordered dependency zero.");
        DependencyResolutionSmokeTest.Prepare(document, modifier);
        Require(modifier.ValidatedReferencedInstances!.Single() == shader.Handle && modifier.AllDependentInstances!.Count == 1
            && modifier.AllDependentInstances.Single() == shader.Handle
            && DependencyResolutionSmokeTest.Visited(document).Count == 2 && shader.ValidatedReferencedInstances!.Count == 0,
            "Real local dependency resolution failed to queue the intended shader.");
        AssetBuffer actual = registry.GetPlugin(modifier.Handle.TypeId).ProcessInstance(modifier);
        Require(actual.InstanceData.Length == 60 && actual.RelocationData.Length == 8 && actual.ImportsData.Length == 8
            && Read(actual.InstanceData, 40) == 56 && Read(actual.InstanceData, 56) == 1
            && Read(actual.RelocationData, 0) == 40 && Read(actual.RelocationData, 4) == uint.MaxValue
            && Read(actual.ImportsData, 0) == 56 && Read(actual.ImportsData, 4) == uint.MaxValue,
            "Modifier shader pointer, import bias or relocation/import sentinel differs.");
        uint selected = Read(actual.InstanceData, (int)Read(actual.ImportsData, 0)) - 1;
        Require(modifier.ValidatedReferencedInstances[(int)selected] == shader.Handle,
            "Final native import selects the wrong resolved shader identity.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: missing targets must clear partial success and visit markers rather than pass a subsequent retry. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectMissing(AssetDeclarationDocument document, InstanceDeclaration modifier)
    {
        try { DependencyResolutionSmokeTest.Prepare(document, modifier); }
        catch (BinaryAssetBuilderException error) when (error.ErrorCode == ErrorCode.UnknownReference)
        {
            Require(modifier.ValidatedReferencedInstances == null && modifier.AllDependentInstances == null
                && DependencyResolutionSmokeTest.Visited(document).Count == 0, "Missing shader retained partial graph success.");
            return;
        }
        throw new InvalidDataException("Removed shader target was still resolved.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject stale cross-family metadata and injected identities explicitly before native allocation. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectRejected(Action action)
    {
        try { action(); }
        catch (Exception error) when (error is NotSupportedException or InvalidOperationException) { return; }
        throw new InvalidDataException("Mixed diagnostic accepted stale metadata.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: both mapped profiles must keep production policy rejection ahead of missing source or output access. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectPolicy(Action action)
    {
        try { action(); }
        catch (BinaryAssetBuilderException error) when (error.Message.Contains("Experimental", StringComparison.Ordinal)) { return; }
        throw new InvalidDataException("Mixed diagnostic bypassed production policy.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: decode final native selector/offset words independently of native host pointer layout. */
    //-------------------------------------------------------------------------------------------------
    private static uint Read(byte[] data, int offset) => BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(offset, 4));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop the cross-family proof on its first reference, identity, native or policy mismatch. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
}
