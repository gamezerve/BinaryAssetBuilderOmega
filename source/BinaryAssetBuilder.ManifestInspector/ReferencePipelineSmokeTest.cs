using System.Buffers.Binary;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: exercise official schema defaults through the actual core reference normalizer and native marshaller.
internal static class ReferencePipelineSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: locate the checked-in focused schema harness without depending on a machine-specific repo path. */
    //-------------------------------------------------------------------------------------------------
    internal static string FindFixture()
    {
        for (DirectoryInfo? directory = new(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            string candidate = Path.Combine(directory.FullName, "tests", "fixtures", "ReferencePipeline.xsd");
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }
        throw new FileNotFoundException("ReferencePipeline.xsd not found; use reference-self-test <schema-path> for standalone deployments.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify inserted defaults, inherited refType, typed weak names and fail-closed type diagnostics. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string schemaPath, params string[] manifestPaths)
    {
        string previousSchemaPath = Settings.Current.SchemaPath;
        try
        {
            Settings.Current.SchemaPath = Path.GetFullPath(schemaPath);
            SchemaSet schemas = new(false);
            const string defaultXml = """<InfiltratorContain xmlns="uri:ea.com:eala:asset" id="ReferenceSmoke" ObjectRef="  GameObject:TestInfiltrator  " />""";
            InstanceDeclaration defaults = Normalize(schemas, defaultXml);
            Require(defaults.ReferencedInstances.Count == 4, "Expected four schema-inserted strong references.");
            Require(defaults.WeakReferencedInstances.Single().Name == "GameObject:TestInfiltrator", "Typed weak dependency lost its declared type or instance name.");
            Require(defaults.XmlNode.Attributes!["ObjectRef"]!.Value == "TestInfiltrator", "Weak reference still contains a type prefix or whitespace.");
            Relo.Chunk first = CheckDefaultChunk(defaults);
            InstanceDeclaration repeated = Normalize(schemas, defaultXml);
            Relo.Chunk second = CheckDefaultChunk(repeated);
            Require(first.InstanceBuffer.SequenceEqual(second.InstanceBuffer)
                && first.RelocationBuffer.SequenceEqual(second.RelocationBuffer)
                && first.ImportsBuffer.SequenceEqual(second.ImportsBuffer)
                && defaults.ReferencedInstances.Select(reference => (reference.TypeId, reference.InstanceId))
                    .SequenceEqual(repeated.ReferencedInstances.Select(reference => (reference.TypeId, reference.InstanceId))),
                "Repeated schema-to-import build is not deterministic.");
            CheckOldSessionCache();

            InstanceDeclaration aliases = Normalize(schemas,
                """<ReferenceProbe xmlns="uri:ea.com:eala:asset" id="Aliases" Alias="DerivedFXList:One\999" Override="MyFilter"><Child>Two</Child></ReferenceProbe>""");
            Require(aliases.ReferencedInstances.Select(reference => reference.Name).SequenceEqual(
                new[] { "DerivedFXList:One", "ObjectFilterAsset:MyFilter", "FXList:Two" }), "Inherited/overridden reference target or traversal order differs.");
            Require(aliases.XmlNode.Attributes!["Alias"]!.Value == @"DerivedFXList:One\0", "Stale numeric suffix was not replaced by the dependency index.");
            Require(aliases.XmlNode.Attributes!["Override"]!.Value == @"MyFilter\1", "Attribute-level refType did not override the alias type.");
            Require(aliases.XmlNode.FirstChild!.InnerText == @"Two\2", "Element reference did not use the shared dependency table.");

            ExpectReferencingError(schemas, """<ReferenceProbe xmlns="uri:ea.com:eala:asset" id="BadType" Alias="GameObject:Wrong" />""");
            ExpectReferencingError(schemas, """<ReferenceProbe xmlns="uri:ea.com:eala:asset" id="MissingType" Missing="Unknown" />""");
            InstanceDeclaration empty = Normalize(schemas, """<ReferenceProbe xmlns="uri:ea.com:eala:asset" id="Empty" Alias="  " />""");
            Require(empty.ReferencedInstances.Count == 0, "Empty optional reference generated a dependency.");

            if (manifestPaths.Length != 0)
            {
                CheckManifestTargets(defaults, manifestPaths);
            }
            Console.WriteLine("Reference pipeline self-test: OK (defaults, inherited/overridden refType, typed weak IDs, type errors, determinism, old-cache rejection)");
        }
        finally
        {
            Settings.Current.SchemaPath = previousSchemaPath;
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: invoke the production normalization stage on schema-validated DOM nodes, not a test reimplementation. */
    //-------------------------------------------------------------------------------------------------
    private static InstanceDeclaration Normalize(SchemaSet schemas, string xml)
    {
        XmlDocument document = new();
        document.LoadXml(xml);
        document.Schemas.Add(schemas.Schemas);
        document.Validate((_, args) => throw new InvalidDataException(args.Message));
        InstanceDeclaration instance = new(new AssetDeclarationDocument()) { XmlNode = document.DocumentElement! };
        MethodInfo method = typeof(AssetDeclarationDocument).GetMethod("HandleAssetReferenceType", BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new MissingMethodException("Production asset-reference normalization method was not found.");
        using MemoryStream typeIds = new();
        using BinaryWriter writer = new(typeIds);
        foreach (XPathNavigator navigator in instance.XmlNode.CreateNavigator()!.SelectDescendants(string.Empty, SchemaSet.XmlNamespace, true))
        {
            NormalizeValue(navigator);
            if (navigator.MoveToFirstAttribute())
            {
                do { NormalizeValue(navigator); } while (navigator.MoveToNextAttribute());
                navigator.MoveToParent();
            }
        }
        Require(typeIds.Length == (instance.ReferencedInstances.Count + instance.WeakReferencedInstances.Count) * sizeof(uint), "Reference type-ID stream count differs.");
        return instance;

        //-------------------------------------------------------------------------------------------------
        /** Reborn: classify strong/weak schema types before invoking the shared private pipeline seam. */
        //-------------------------------------------------------------------------------------------------
        void NormalizeValue(XPathNavigator navigator)
        {
            XmlSchemaType? type = navigator.SchemaInfo?.SchemaType;
            if (type is null)
            {
                return;
            }
            bool weak = XmlSchemaType.IsDerivedFrom(type, schemas.XmlWeakReferenceType, XmlSchemaDerivationMethod.None);
            bool strong = !weak && XmlSchemaType.IsDerivedFrom(type, schemas.XmlAssetReferenceType, XmlSchemaDerivationMethod.None);
            if (!strong && !weak)
            {
                return;
            }
            try
            {
                method.Invoke(null, new object[] { navigator, instance, writer, strong, schemas });
            }
            catch (TargetInvocationException exception) when (exception.InnerException is not null)
            {
                ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
            }
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: ensure index zero is a real import and all inserted default dependencies reach native output. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe Relo.Chunk CheckDefaultChunk(InstanceDeclaration instance)
    {
        XmlNamespaceManager namespaces = new(instance.XmlNode.OwnerDocument!.NameTable);
        namespaces.AddNamespace("ea", SchemaSet.XmlNamespace);
        Node node = new(instance.XmlNode.CreateNavigator()!, namespaces);
        InfiltratorContainModuleData* root;
        using Tracker tracker = new((void**)&root, (uint)sizeof(InfiltratorContainModuleData), false);
        global::Marshaler.Marshal(node, root, tracker);
        Relo.Chunk chunk = new();
        tracker.MakeRelocatable(chunk);
        Require(chunk.InstanceBuffer.Length == 136 && chunk.RelocationBuffer.Length == 0 && chunk.ImportsBuffer.Length == 20, "Default infiltrator output is not 136/0/20.");
        int[] slots = [44, 60, 64, 68];
        string[] names = ["ObjectFilterAsset:InfiltrationCanEnterObjectFilter", "EvaEvent:EnemyBuildingInfiltrated", "EvaEvent:OurBuildingInfiltrated", "FXList:FX_Building_Infiltrated_Generic"];
        for (int index = 0; index < slots.Length; index++)
        {
            uint dependencyIndex = BinaryPrimitives.ReadUInt32LittleEndian(chunk.InstanceBuffer.AsSpan(slots[index]));
            Require(dependencyIndex < instance.ReferencedInstances.Count && instance.ReferencedInstances[(int)dependencyIndex].Name == names[index], "Default reference does not select its correct dependency.");
            Require(BinaryPrimitives.ReadUInt32LittleEndian(chunk.ImportsBuffer.AsSpan(index * 4)) == slots[index], "Default import slot differs.");
        }
        Require(BinaryPrimitives.ReadUInt32LittleEndian(chunk.ImportsBuffer.AsSpan(16)) == uint.MaxValue, "Default import sentinel is missing.");
        Require(BinaryPrimitives.ReadUInt32LittleEndian(chunk.InstanceBuffer.AsSpan(88)) == FastHash.GetHashCode("testinfiltrator"), "Typed weak reference hashes the wrong name.");
        Console.WriteLine("  Schema-default InfiltratorContain=136/0/20 (four resolved imports, including dependency index zero)");
        return chunk;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove production session-cache validation rejects the previous document-processor revision. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckOldSessionCache()
    {
        Type cacheType = typeof(BinaryAssetBuilder.Core.Session.SessionCache);
        Type lastType = cacheType.GetNestedType("LastState", BindingFlags.NonPublic)!;
        object oldState = Activator.CreateInstance(lastType, nonPublic: true)!;
        lastType.GetProperty("Version")!.SetValue(oldState, BinaryAssetBuilder.Core.Session.SessionCache.CacheVersion);
        lastType.GetProperty("DocumentProcessorVersion")!.SetValue(oldState, DocumentProcessor.Version - 2u);
        BinaryAssetBuilder.Core.Session.SessionCache cache = new();
        FieldInfo last = cacheType.GetField("_last", BindingFlags.Instance | BindingFlags.NonPublic)!;
        last.SetValue(cache, oldState);
        cache.InitializeCache(new System.Collections.Generic.List<string>());
        Require(last.GetValue(cache) is null, "Previous reference-normalization cache was accepted.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify default dependency type/instance IDs against real manifests without opening any BIN stream. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckManifestTargets(InstanceDeclaration instance, string[] manifestPaths)
    {
        HashSet<AssetId> ids = new();
        foreach (string path in manifestPaths)
        {
            ManifestDocument manifest = ManifestReader.Read(File.ReadAllBytes(path));
            Require(manifest.Header.Version == 7 && manifest.Header.AllTypesHash == 0x5454A8E9 && manifest.Validate().Count == 0, $"Not a validated EP1 manifest: {path}");
            ids.UnionWith(manifest.Assets.Select(asset => new AssetId(asset.TypeId, asset.InstanceId)));
        }
        foreach (InstanceHandle reference in instance.ReferencedInstances)
        {
            Require(ids.Contains(new AssetId(reference.TypeId, reference.InstanceId)), $"Default dependency absent from supplied EP1 manifests: {reference.Name}");
            Console.WriteLine($"  Real EP1 dependency: {reference.Name} {reference.TypeId:X8}:{reference.InstanceId:X8}");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject invalid explicit types and missing schema refType instead of silently creating an import. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectReferencingError(SchemaSet schemas, string xml)
    {
        try
        {
            Normalize(schemas, xml);
        }
        catch (BinaryAssetBuilderException exception) when (exception.ErrorCode == ErrorCode.ReferencingError)
        {
            return;
        }
        throw new InvalidDataException("Expected a schema reference-type error.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: report pipeline assertions as actionable validation failures. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidDataException(message);
        }
    }
}
