using System.Buffers.Binary;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: replace captured destruction FX input indices with actual Core name/default normalization and independent linked-catalog admission.
internal static class Ep1DestructionSourceSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove one complete stock graph, ordered multi-item dependencies and fail-closed missing/type/ambiguity controls without publishing GameObjects. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest)
    {
        var saved = Settings.Current;
        try
        {
            string fixture = Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!, "DestructionPipeline.xsd");
            CheckSchema(fixture);
            Settings.Current = new Settings { SchemaPath = fixture, BuildCache = false, StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            var schemas = new SchemaSet(false);
            var document = ManifestReader.Read(SdkEnvironmentPreflight.Read(manifest, 64 * 1048576));
            TypeRegistryAudit.ValidateTarget(document.Header.Version, document.Header.AllTypesHash);
            if (!document.Header.IsLinked || document.Validate().Count != 0) throw new InvalidDataException("Valid linked EP1 catalog required.");
            var catalog = document.Assets.ToArray();
            const string source = "<BuildingDestructionBehavior xmlns='uri:ea.com:eala:asset' id='ModuleTag_BuildingDestruction'><DieMuxData DeathTypes='ALL'/><Item BonePrefix='BONE_CONTACT_POINT_' FX='SovietRefineryHit' CreationObject='AlliedBarracks_Debris' FlingVelocity='100.0'/></BuildingDestructionBehavior>";
            var stock = GameObjectStockReview.Capture(manifest, "GameObject:AlliedConstructionYard");
            var module = GameObjectModuleReview.Inspect(manifest, "GameObject:AlliedConstructionYard").Containers.SelectMany(g => g.Modules).Single(m => m.Type == "BuildingDestructionModuleData");
            var normalized = Normalize(schemas, source, catalog);
            var output = Ep1AlliedModuleSmokeTest.Compile(normalized.XmlNode.OuterXml, module.Type);
            // Reborn: raw root/module validation is distinct from normalized FX import input; moved strings must preserve actual Core dependency indices.
            System.Xml.Schema.XmlSchemaSet? rootSchemas=null;
            SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:s=>rootSchemas=s);
            if(rootSchemas==null)throw new InvalidDataException("Reviewed root schema candidate required for string/import integration.");
            GameObjectRootStringSmokeTest.WithNamedFx(rootSchemas,source,normalized.XmlNode.OuterXml);
            Ep1AlliedModuleSmokeTest.Compare(stock.Bytes, module, output, 72, stock.Asset.References.ToArray(), normalized);
            // Reborn: generate, independently compare and round-trip the module fragment from actual Core native output, never a captured prefix.
            DestructionTokenWriterSmokeTest.Stock(output, stock.Bytes, module);
            var target = normalized.ReferencedInstances.Single();
            if (target.TypeId != FastHash.GetHashCode("FXList") || target.InstanceId != FastHash.GetHashCode("sovietrefineryhit")) throw new InvalidDataException("Destruction source target differs.");
            // Reborn: both schema-inserted weak SubObject identity and native default values must survive Core, not be patched after compilation.
            if (normalized.XmlNode.Attributes!["FillerMesh"]?.Value != "NEWSKIN_FILL" || normalized.XmlNode.Attributes["MaxDebrisCloudsPerSecond"]?.Value != "2.0")
                throw new InvalidDataException("Destruction Core schema defaults absent.");
            var repeat = Normalize(schemas, source, catalog);
            var again = Ep1AlliedModuleSmokeTest.Compile(repeat.XmlNode.OuterXml, module.Type);
            if (!output.Chunk.InstanceBuffer.SequenceEqual(again.Chunk.InstanceBuffer) || !output.Chunk.RelocationBuffer.SequenceEqual(again.Chunk.RelocationBuffer) || !output.Chunk.ImportsBuffer.SequenceEqual(again.Chunk.ImportsBuffer))
                throw new InvalidDataException("Destruction Core repeat drift.");
            // Reborn: select a distinct real catalog FX identity, not an arbitrary numeric import, to prove semantic mismatch rejection and ordered dependency emission.
            var other = catalog.First(a => a.TypeId == target.TypeId && a.InstanceId != target.InstanceId && a.Name.StartsWith("FXList:", StringComparison.Ordinal));
            string otherName = other.Name["FXList:".Length..];
            var changed = Normalize(schemas, source.Replace("SovietRefineryHit", otherName, StringComparison.Ordinal), catalog);
            var changedOutput = Ep1AlliedModuleSmokeTest.Compile(changed.XmlNode.OuterXml, module.Type);
            Refuse(() => Ep1AlliedModuleSmokeTest.Compare(stock.Bytes, module, changedOutput, 72, stock.Asset.References.ToArray(), changed));
            string multi = source.Replace("</BuildingDestructionBehavior>", "<Item BonePrefix='second' FX='" + otherName + "'/><Item BonePrefix='third' FX='SovietRefineryHit'/></BuildingDestructionBehavior>", StringComparison.Ordinal);
            var ordered = Normalize(schemas, multi, catalog);
            var many = Ep1AlliedModuleSmokeTest.Compile(ordered.XmlNode.OuterXml, module.Type);
            // Reborn: ordered repeated named FX occurrences also exercise variable-sized token-list emission.
            DestructionTokenWriterSmokeTest.Authored(many);
            int items = (int)Word(many.Chunk.InstanceBuffer, many.Native + 68);
            // Reborn: Core appends each source reference occurrence here; repeated identity is retained rather than deduplicated at this seam.
            if (ordered.ReferencedInstances.Count != 3 || ordered.ReferencedInstances[0].InstanceId != target.InstanceId || ordered.ReferencedInstances[1].InstanceId != other.InstanceId || ordered.ReferencedInstances[2].InstanceId != target.InstanceId ||
                Word(many.Chunk.InstanceBuffer, many.Native + 64) != 3 || Word(many.Chunk.InstanceBuffer, items + 8) != 1 || Word(many.Chunk.InstanceBuffer, items + 28) != 2 || Word(many.Chunk.InstanceBuffer, items + 48) != 3)
                throw new InvalidDataException("Destruction ordered/repeated Core imports differ.");
            var imports = Enumerable.Range(0, many.Chunk.ImportsBuffer.Length / 4).Select(i => Word(many.Chunk.ImportsBuffer, i * 4));
            if (!imports.SequenceEqual(new[] { (uint)(items + 8), (uint)(items + 28), (uint)(items + 48), uint.MaxValue })) throw new InvalidDataException("Destruction authored import slots differ.");
            foreach (string invalid in new[] { source.Replace("SovietRefineryHit", "MissingFxForRebornSourceProbe", StringComparison.Ordinal),
                source.Replace("SovietRefineryHit", "GameObject:AlliedBarracks_Debris", StringComparison.Ordinal),
                source.Replace("SovietRefineryHit", "SovietRefineryHit\\999", StringComparison.Ordinal),
                source.Replace("<DieMuxData DeathTypes='ALL'/>", "", StringComparison.Ordinal),
                source.Replace("FlingVelocity='100.0'", "UnknownField='100.0'", StringComparison.Ordinal) })
                Refuse(() => Normalize(schemas, invalid, catalog));
            var duplicate = catalog.Single(a => a.TypeId == target.TypeId && a.InstanceId == target.InstanceId);
            Refuse(() => Normalize(schemas, source, catalog.Append(duplicate).ToArray()));
            Console.WriteLine("EP1 destruction source: PASS (actual Core named FX/defaults, complete stock native graph/dispatch/import identity, ordered 1/2/3 occurrence imports with repeated target, repeat and missing/type/ambiguity/index/schema refusals; focused inheritance stubs, no generic source closure/tokenized GameObject/package/game).");
        }
        finally { Settings.Current = saved; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject authored diagnostic indices before Core; every normalized strong target must uniquely exist in independently read stock metadata. */
    //-------------------------------------------------------------------------------------------------
    private static InstanceDeclaration Normalize(SchemaSet schemas, string source, ManifestAsset[] catalog)
    {
        var xml = new XmlDocument { XmlResolver = null }; xml.LoadXml(source);
        foreach (XmlElement e in xml.GetElementsByTagName("Item", SchemaSet.XmlNamespace))
            if (e.GetAttribute("FX").Contains('\\')) throw new InvalidDataException("Authored FX must not contain a diagnostic index.");
        var instance = ReferencePipelineSmokeTest.Normalize(schemas, xml.OuterXml);
        foreach (var r in instance.ReferencedInstances)
            if (r.TypeId != FastHash.GetHashCode("FXList") || catalog.Count(a => a.TypeId == r.TypeId && a.InstanceId == r.InstanceId) != 1)
                throw new InvalidDataException("Missing/ambiguous destruction FX: " + r.Name);
        return instance;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: copied base declarations must retain official semantic attributes/child order; the remaining declarations are direct official includes. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckSchema(string fixture)
    {
        var focused = Read(fixture); var stock = Read(Path.Combine(SdkEnvironmentPreflight.BaselineSchemaRoot(), "Modules", "BaseModules.xsd"));
        foreach (string name in new[] { "ModuleData", "BehaviorModuleData" })
            if (Shape(Select(focused, name)) != Shape(Select(stock, name))) throw new InvalidDataException("Destruction schema base drift: " + name);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: schema readers prohibit entities and bound memory before declaration comparison. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Read(string path)
    {
        var d = new XmlDocument { XmlResolver = null }; using var s = new MemoryStream(SdkEnvironmentPreflight.Read(path, 2 * 1048576), false);
        using var r = XmlReader.Create(s, new XmlReaderSettings { XmlResolver = null, DtdProcessing = DtdProcessing.Prohibit }); d.Load(r); return d;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare only explicitly copied top-level declarations, never implicitly admit a whole schema graph. */
    //-------------------------------------------------------------------------------------------------
    private static XmlElement Select(XmlDocument d, string name)
    {
        var ns = new XmlNamespaceManager(d.NameTable); ns.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
        return (XmlElement)(d.SelectSingleNode("/xs:schema/*[@name='" + name + "']", ns) ?? throw new InvalidDataException("Missing destruction declaration: " + name));
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: ignore comments/indentation/namespace syntax while preserving semantic declaration shape. */
    //-------------------------------------------------------------------------------------------------
    private static string Shape(XmlElement e) => e.NamespaceURI + ":" + e.LocalName + "[" + string.Join("|", e.Attributes.Cast<XmlAttribute>().Where(a => a.NamespaceURI != "http://www.w3.org/2000/xmlns/")
        .OrderBy(a => a.NamespaceURI + ":" + a.LocalName, StringComparer.Ordinal).Select(a => a.NamespaceURI + ":" + a.LocalName + "=" + a.Value)) + "]{" + string.Join("", e.ChildNodes.OfType<XmlElement>().Select(Shape)) + "}";
    //-------------------------------------------------------------------------------------------------
    /** Reborn: malformed schema/reference/native controls must fail explicitly. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action) { try { action(); } catch (Exception e) when (e is InvalidDataException or BinaryAssetBuilderException) { return; } throw new InvalidDataException("Invalid destruction source admitted."); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: read only within owned native buffers. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b, int at) { if (at < 0 || at > b.Length - 4) throw new InvalidDataException("Destruction source bounds exceeded."); return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at, 4)); }
}
