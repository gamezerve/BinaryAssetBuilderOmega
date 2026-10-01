using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: exercise actual shader descriptor/document/compiler entries while rejecting stale identities and keeping output/cache gates closed.
internal static class Ep1ShaderOverrideProfileSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: reproduce native source buffers through the isolated plugin, test tampered controls and prove fresh document reload. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        string directory = Path.Combine(Path.GetTempPath(), "Reborn-Ep1ShaderProfile-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        Settings previous = Settings.Current;
        try
        {
            XmlDocument config = new(); config.Load(Path.Combine(fixtures, "Ep1ShaderOverrideProfile.xml"));
            XmlNamespaceManager namespaces = new(config.NameTable); namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
            Settings settings = new(); settings.ReadXml(new Node(config.DocumentElement!.CreateNavigator(), namespaces));
            settings.SchemaPath = Path.Combine(fixtures, "ShaderOverridePipeline.xsd");
            settings.DataRoot = directory; settings.DataPaths = new[] { directory }; settings.TargetPlatform = TargetPlatform.Win32;
            settings.CustomPostfix = ""; settings.StreamPostfix = ""; settings.StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>();
            Settings.Current = settings;
            PluginRegistry registry = new(settings.Plugins, TargetPlatform.Win32);
            Require(registry.DefaultPlugin is Ra3Ep1ShaderOverridePlugin && !settings.BuildCache && !registry.CanReuseCompiledDocuments,
                "Shader descriptor/profile reuse policy differs.");
            ExtendedTypeInformation info = registry.GetExtendedTypeInformation(0xBCC23F6Cu);
            Require(info.TypeHash == 0x3D5B1D16u && info.ProcessingHash == (0x3D5B1D16u ^ 0x45503131u)
                && !info.Tokenized && !info.HasCustomData && !info.UseBuildCache && registry.DefaultPlugin.AllTypesHash == 0x5454A8E9u,
                "Shader native metadata/processing identity differs.");
            ExpectPolicy(registry.ValidateProductionOutput);
            PluginDescriptor mapped = new() { IsEnabled = true, AssetTypes = "ShaderOverride", UseBuildCache = true,
                QualifiedName = settings.Plugins.Single().QualifiedName };
            PluginRegistry mappedRegistry = new(new[] { mapped }, TargetPlatform.Win32);
            Require(!mappedRegistry.GetExtendedTypeInformation(0xBCC23F6Cu).UseBuildCache && !mappedRegistry.CanReuseCompiledDocuments,
                "Explicit shader mapping bypassed cache policy.");
            ExpectPolicy(mappedRegistry.ValidateProductionOutput);
            SessionCache cache = new(); cache.InitializeCache(new List<string>());
            DocumentProcessor processor = new(settings, registry, new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32))
                { Cache = cache, SchemaSet = new SchemaSet(false) };
            string source = Path.Combine(directory, "shaders.xml");
            File.WriteAllText(source, File.ReadAllText(Path.Combine(fixtures, "ShaderOverrideProbe.xml")));
            var options = new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = true };
            AssetDeclarationDocument first = processor.ProcessDocumentInternal(source, source, null!, options);
            Require(first.SelfInstances.Count == 4, "Shader document count differs.");
            // Reborn: exercise schema-inserted priority and optional condition defaults through the same real compiler entry.
            string defaultSource = Path.Combine(directory, "default.xml");
            File.WriteAllText(defaultSource, """
                <AssetDeclaration xmlns="uri:ea.com:eala:asset"><ShaderOverride id="DefaultShader">
                <Rule ReplaceShaderName="Null.fx" ReplaceTechniqueName="Default" />
                </ShaderOverride></AssetDeclaration>
                """);
            InstanceDeclaration defaultInstance = processor.ProcessDocumentInternal(defaultSource, defaultSource, null!, options).SelfInstances.Single();
            AssetBuffer defaultBuffer = registry.DefaultPlugin.ProcessInstance(defaultInstance);
            Require(defaultBuffer.InstanceData.Length == 40 && defaultBuffer.RelocationData.Length == 12
                && defaultBuffer.ImportsData.Length == 0 && BitConverter.ToUInt32(defaultBuffer.InstanceData, 4) == 1
                && BitConverter.ToUInt32(defaultBuffer.InstanceData, 16) == 0,
                "Shader compiler entry lost default priority or absent condition.");
            ((XmlElement)defaultInstance.XmlNode).SetAttribute("Priority", "4294967295");
            Require(BitConverter.ToUInt32(registry.DefaultPlugin.ProcessInstance(defaultInstance).InstanceData, 4) == uint.MaxValue,
                "Shader unsigned priority boundary was truncated.");
            XmlDocument fixture = new(); fixture.Load(Path.Combine(fixtures, "ShaderOverrideProbe.xml"));
            Dictionary<string, byte[]> bytes = new(StringComparer.Ordinal);
            foreach (InstanceDeclaration instance in first.SelfInstances)
            {
                XmlElement original = fixture.DocumentElement!.ChildNodes.OfType<XmlElement>()
                    .Single(element => element.GetAttribute("id") == instance.Handle.InstanceName);
                var expected = ShaderOverrideNativeSmokeTest.Compile(first.XmlDocument.Schemas, original.OuterXml);
                string savedXml = instance.XmlNode.OuterXml;
                AssetBuffer actual = registry.DefaultPlugin.ProcessInstance(instance);
                Require(instance.Handle.TypeHash == 0x3D5B1D16u && actual.InstanceData.SequenceEqual(expected.InstanceBuffer)
                    && actual.RelocationData.SequenceEqual(expected.RelocationBuffer) && actual.ImportsData.Length == 0
                    && instance.XmlNode.OuterXml == savedXml, "Shader compiler changed native source bytes or normalized XML.");
                bytes.Add(instance.Handle.Name, actual.InstanceData);
            }
            InstanceDeclaration psychic = first.SelfInstances.Single(value => value.Handle.InstanceName == "ShaderOverride_PsychicCrush");
            XmlElement root = (XmlElement)psychic.XmlNode;
            XmlElement rule = root.ChildNodes.OfType<XmlElement>().ElementAt(5);
            // Reborn: a schema-valid live value edit must compile literally, never be hidden by an asset-specific stock exception.
            rule.SetAttribute("ReplaceShaderName", "Lightning.fx");
            AssetBuffer variant = registry.DefaultPlugin.ProcessInstance(psychic);
            byte[] literal = bytes[psychic.Handle.Name];
            Require(variant.InstanceData.AsSpan(0, 100).SequenceEqual(literal.AsSpan(0, 100))
                && variant.InstanceData.AsSpan(104).SequenceEqual(literal.AsSpan(104))
                && !variant.InstanceData.AsSpan(100, 4).SequenceEqual(literal.AsSpan(100, 4)),
                "Shader compiler did not preserve the explicit source/stock content delta.");
            rule.SetAttribute("ReplaceShaderName", "Null.fx");
            foreach (var change in new[] { (root, "id", "ChangedIdentity"),
                (root, "Priority", "-1"), (root, "Priority", "4294967296"), (root, "Priority", "=Unknown"),
                (root, "inheritFrom", "BaseShader"), (root, "TypeId", "0"), (rule, "TypeId", "0"),
                (rule, "ReplaceTechniqueName", "NonDefault"), (rule, "IfOriginalShaderIs", ""),
                (rule, "ReplaceShaderName", "Null.fx\\0"), (rule, "ReplaceShaderName", "FXShaderMaterial:Null.fx"),
                (rule, "ReplaceShaderName", "=Unknown"), (rule, "ReplaceShaderName", "../Null.fx"),
                (rule, "ReplaceShaderName", " Null.fx"), (rule, "ReplaceShaderName", "Nüll.fx"),
                (rule, "ReplaceShaderName", new string('A', 126) + ".fx"), (rule, "Unknown", "value") })
                CheckMutation(registry.DefaultPlugin, psychic, change.Item1, change.Item2, change.Item3);
            XmlAttribute foreign = root.OwnerDocument.CreateAttribute("probe", "Priority", "urn:unsupported"); foreign.Value = "1";
            root.Attributes.Append(foreign);
            try { ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(psychic)); }
            finally { root.RemoveAttributeNode(foreign); }
            XmlElement nested = root.OwnerDocument.CreateElement("Control", root.NamespaceURI); rule.AppendChild(nested);
            try { ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(psychic)); }
            finally { rule.RemoveChild(nested); }
            XmlElement extra = (XmlElement)rule.CloneNode(true); root.AppendChild(extra);
            try { ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(psychic)); }
            finally { root.RemoveChild(extra); }
            psychic.ReferencedInstances.Add(new InstanceHandle("FXShaderMaterial", "Null.fx"));
            try { ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(psychic)); }
            finally { psychic.ReferencedInstances.Clear(); }
            psychic.WeakReferencedInstances.Add(new InstanceHandle("FXShaderMaterial", "Null.fx"));
            try { ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(psychic)); }
            finally { psychic.WeakReferencedInstances.Clear(); }
            psychic.ReferencedFiles.Add("UnexpectedFile");
            try { ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(psychic)); }
            finally { psychic.ReferencedFiles.Clear(); }
            psychic.HasCustomData = true;
            try { ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(psychic)); }
            finally { psychic.HasCustomData = false; }
            InstanceDeclaration unvalidated = new(new AssetDeclarationDocument())
                { XmlNode = fixture.DocumentElement!.ChildNodes.OfType<XmlElement>().First() };
            ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(unvalidated));
            rule.SetAttribute("TypeId", "0");
            AssetDeclarationDocument repeated = processor.ProcessDocumentInternal(source, source, null!, options);
            Require(options.UsePrecompiled && repeated.SelfInstances.All(instance =>
                registry.DefaultPlugin.ProcessInstance(instance).InstanceData.SequenceEqual(bytes[instance.Handle.Name])),
                "Shader profile reused poisoned document state or mutated caller options.");
            processor.Cache = null!;
            ExpectPolicy(() => processor.ProcessDocumentInternal("missing", "missing", null!,
                new DocumentProcessor.ProcessOptions { GenerateOutput = true }));
            Ra3Ep1ShaderOverridePlugin isolated = new();
            ExpectRejected(() => isolated.GetExtendedTypeInformation(0xBCC23F6Cu)); isolated.Initialize(TargetPlatform.Win32);
            isolated.GetExtendedTypeInformation(0xBCC23F6Cu).TypeHash = 0;
            Require(isolated.GetExtendedTypeInformation(0xBCC23F6Cu).TypeHash == 0x3D5B1D16u, "Shader metadata was shared with the caller.");
            ExpectRejected(() => isolated.GetExtendedTypeInformation(0x44A5973Du));
            ExpectRejected(() => isolated.ReInitialize(TargetPlatform.Xbox360));
            ExpectRejected(() => isolated.ProcessInstance(repeated.SelfInstances.First()));
            isolated.ReInitialize(TargetPlatform.Win32);
            Require(isolated.ProcessInstance(repeated.SelfInstances.First()).InstanceData.SequenceEqual(bytes[repeated.SelfInstances.First().Handle.Name]),
                "Shader Win32 reinitialization did not recover.");
            Require(!Directory.EnumerateFiles(directory, "*.manifest").Any(), "Shader diagnostic emitted production output.");
            Console.WriteLine("EP1 shader profile self-test: OK (descriptor/document/four compiler entries, literal POIDs, current-value and injected-identity guards, fresh reload, platform/production/cache gates)");
        }
        finally { Settings.Current = previous; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: restore caller-owned normalized XML after testing a stale current-value or injected-identity edit. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckMutation(IAssetBuilderPlugin plugin, InstanceDeclaration instance, XmlElement element, string name, string value)
    {
        bool existed = element.HasAttribute(name); string saved = element.GetAttribute(name);
        element.SetAttribute(name, value);
        try { ExpectRejected(() => plugin.ProcessInstance(instance)); }
        finally { if (existed) element.SetAttribute(name, saved); else element.RemoveAttribute(name); }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require explicit profile/schema rejection rather than allowing incidental null or unsafe marshalling failures. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectRejected(Action action)
    {
        try { action(); }
        catch (Exception error) when (error is NotSupportedException or InvalidOperationException or XmlSchemaException) { return; }
        throw new InvalidDataException("Shader profile accepted unsupported metadata or controls.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: identify output policy rejection before missing source/cache or native output can be accessed. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectPolicy(Action action)
    {
        try { action(); }
        catch (BinaryAssetBuilderException error) when (error.Message.Contains("RA3EP1-ShaderOverride-Experimental-v1", StringComparison.Ordinal)) { return; }
        throw new InvalidDataException("Shader production request bypassed profile policy.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fail the isolated shader compiler proof on its first byte, policy or identity mismatch. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
}
