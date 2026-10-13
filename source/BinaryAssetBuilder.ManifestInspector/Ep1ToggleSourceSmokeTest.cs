using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: actual Core source/default/reference normalization must reproduce selected native graphs without any captured-index substitution.
internal static class Ep1ToggleSourceSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: normalize three complete selected modules, resolve named concrete dependencies, then exercise authored lists and source refusal boundaries together. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest, string globalManifest)
    {
        Settings saved = Settings.Current;
        try
        {
            string fixtureRoot = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
            Ep1ToggleAiSmokeTest.CheckSchema(Path.Combine(fixtureRoot, "ToggleAiPipeline.xsd"));
            string fixture = Path.Combine(fixtureRoot, "ToggleModulePipeline.xsd"); CheckSchema(fixture);
            Settings.Current = new Settings { SchemaPath = fixture, BuildCache = false, StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            var schemas = new SchemaSet(false);
            var docs = new[] { Catalog(manifest), Catalog(globalManifest) };
            var catalog = docs.SelectMany(d => d.Assets).ToArray();
            foreach (var c in Ep1ToggleModuleSmokeTest.Cases())
            {
                var capture = GameObjectStockReview.Capture(manifest, "GameObject:" + c.Name);
                var proof = ToggleStatusLayoutReview.Inspect(manifest, "GameObject:" + c.Name);
                var instance = Normalize(schemas, c.Xml, catalog, new HashSet<uint>());
                Resolve(instance, catalog);
                var chunk = Ep1ToggleModuleSmokeTest.Compile(instance.XmlNode.OuterXml);
                Ep1ToggleModuleSmokeTest.Compare(capture.Bytes, proof, chunk, capture.Asset.References.ToArray(), instance);
                // Reborn: rebasing must reject the wrong valid Core dependency, not merely bounds-check and clear imported words.
                var wrong = new Relo.Chunk { InstanceBuffer = chunk.InstanceBuffer.ToArray(), RelocationBuffer = chunk.RelocationBuffer.ToArray(), ImportsBuffer = chunk.ImportsBuffer.ToArray() };
                BinaryPrimitives.WriteUInt32LittleEndian(wrong.InstanceBuffer.AsSpan(8, 4), 2);
                Refuse(() => Ep1ToggleModuleSmokeTest.Compare(capture.Bytes, proof, wrong, capture.Asset.References.ToArray(), instance));
                var repeat = Normalize(schemas, c.Xml, catalog, new HashSet<uint>());
                if (instance.ReferencedInstances.Count != repeat.ReferencedInstances.Count
                    || !chunk.InstanceBuffer.SequenceEqual(Ep1ToggleModuleSmokeTest.Compile(repeat.XmlNode.OuterXml).InstanceBuffer)) throw new InvalidDataException("Toggle Core source repeat drift.");
                Console.WriteLine($"Toggle actual Core {c.Name}: dependencies={instance.ReferencedInstances.Count}; selected full native graph/import identities match (no captured source indices)");
            }
            const string own = "<ToggleStatusSpecialAbilityUpdate xmlns='uri:ea.com:eala:asset' id='Custom' ForceIdleAfterToggle='true'><ToggleState AIUpdateModuleId='CustomAi' LocomotorSet='SPECIALABILITY_ACTIVE' GenericLuaEventParameter='event payload'><SkirmishAiInfo ToggleHint='TOGGLE_ALWAYS'/><Module ModuleId='Drain' TriggerType='TARGET_OBJECT'/><ActiveGeometry>ship geom</ActiveGeometry><ActiveGeometry>air_geom</ActiveGeometry></ToggleState></ToggleStatusSpecialAbilityUpdate>";
            var locals = new HashSet<uint> { FastHash.GetHashCode("customai"), FastHash.GetHashCode("drain") };
            var authored = Normalize(schemas, own, catalog, locals); var output = Ep1ToggleModuleSmokeTest.Compile(authored.XmlNode.OuterXml);
            int state = (int)Word(output.InstanceBuffer, 256), list = (int)Word(output.InstanceBuffer, state + 292);
            if (Word(output.InstanceBuffer, state + 104) != 18 || Word(output.InstanceBuffer, state + 288) != 1 || Word(output.InstanceBuffer, state + 296) != 2
                || Word(output.InstanceBuffer, list) != FastHash.GetHashCode("drain") || Word(output.InstanceBuffer, list + 4) != 2 || output.InstanceBuffer[list + 8] != 1)
                throw new InvalidDataException("Authored Core source list/default/local-reference output differs.");
            foreach (string changed in new[] { own.Replace("TARGET_OBJECT", "UNKNOWN", StringComparison.Ordinal),
                own.Replace("SPECIALABILITY_ACTIVE", "COMBO2", StringComparison.Ordinal), own.Replace("ModuleId='Drain'", "ModuleId='Missing'", StringComparison.Ordinal),
                own.Replace("<SkirmishAiInfo ToggleHint='TOGGLE_ALWAYS'/>", "", StringComparison.Ordinal), own.Replace("ForceIdleAfterToggle='true'", "UnknownField='true'", StringComparison.Ordinal) })
                Refuse(() => Normalize(schemas, changed, catalog, locals));
            string missing = Ep1ToggleModuleSmokeTest.Cases()[0].Xml.Replace("SOV_Conscript_ArmMolotovCocktailMS", "MissingSound", StringComparison.Ordinal);
            Refuse(() => Normalize(schemas, missing, catalog, locals));
            Refuse(() => Normalize(schemas, Ep1ToggleModuleSmokeTest.Cases()[0].Xml.Replace("SOV_Conscript_ArmMolotovCocktailMS", "SOV_Conscript_ArmMolotovCocktailMS\\99", StringComparison.Ordinal), catalog, locals));
            Refuse(() => Normalize(schemas, own, catalog, new HashSet<uint>()));
            var absent = Normalize(schemas, Ep1ToggleModuleSmokeTest.Cases()[0].Xml.Replace("SovietAntiInfantryInfantryAK47", "MissingWeapon", StringComparison.Ordinal), catalog, locals);
            Refuse(() => Resolve(absent, catalog));
            var sound = catalog.Single(a => a.TypeId == 0xA3A7AF37u && a.InstanceId == FastHash.GetHashCode("sov_conscript_armmolotovcocktailms"));
            Refuse(() => Normalize(schemas, Ep1ToggleModuleSmokeTest.Cases()[0].Xml, catalog.Append(sound).ToArray(), locals));
            Console.WriteLine("EP1 toggle source: PASS (three actual-Core/default/native graphs, independent audio family lookup, strong dependency/import identities, authored lists/local IDs and schema/missing/ambiguous-reference refusals; no full GameObject source graph, polymorphic dispatch, stock nonempty-list proof or package/game).");
        }
        finally { Settings.Current = saved; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: admit only linked EP1 catalogs with validated metadata; named lookup is independent of captured per-object dependency indices. */
    //-------------------------------------------------------------------------------------------------
    private static ManifestDocument Catalog(string path)
    {
        var doc = ManifestReader.Read(SdkEnvironmentPreflight.Read(path, 64 * 1048576));
        TypeRegistryAudit.ValidateTarget(doc.Header.Version, doc.Header.AllTypesHash);
        if (!doc.Header.IsLinked || doc.Validate().Count != 0) throw new InvalidDataException("Expected a valid linked EP1 dependency catalog."); return doc;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: explicitly prepare polymorphic audio names and local module-ID checks before calling the real Core reference handler; never inject stock indices. */
    //-------------------------------------------------------------------------------------------------
    private static InstanceDeclaration Normalize(SchemaSet schemas, string source, ManifestAsset[] catalog, HashSet<uint> localIds)
    {
        var xml = new XmlDocument { XmlResolver = null }; xml.LoadXml(source);
        foreach (XmlElement e in xml.GetElementsByTagName("*"))
        {
            foreach (string field in new[] { "EnterStateSound", "InitiateIntentToExitStateSound", "PackSound", "UnpackSound", "PrepSoundLoop", "TriggerSound", "ActiveLoopSound" })
                if (e.HasAttribute(field) && e.GetAttribute(field).Length != 0)
                {
                    string raw = e.GetAttribute(field);
                    if (raw.Contains('\\')) throw new InvalidDataException("Authored audio references cannot contain diagnostic indices.");
                    if (raw.Contains(':')) continue;
                    uint id = FastHash.GetHashCode(raw.ToLowerInvariant());
                    var matches = catalog.Where(a => a.InstanceId == id && a.TypeId is 0x844D7B9Fu or 0xA3A7AF37u).ToArray();
                    if (matches.Length != 1) throw new InvalidDataException("Missing/ambiguous audio dependency: " + raw);
                    e.SetAttribute(field, (matches[0].TypeId == 0x844D7B9Fu ? "AudioEvent:" : "Multisound:") + raw);
                }
            foreach (string field in new[] { "AIUpdateModuleId", "LeechWeaponSOLModuleId", "StealthOthersInvisibilityModuleId", "ModuleId" })
                if (e.HasAttribute(field) && e.GetAttribute(field).Length != 0 && !localIds.Contains(FastHash.GetHashCode(e.GetAttribute(field).ToLowerInvariant())))
                    throw new InvalidDataException("Missing local module ID: " + e.GetAttribute(field));
        }
        return ReferencePipelineSmokeTest.Normalize(schemas, xml.OuterXml);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: every Core-normalized strong target must uniquely exist in the supplied linked catalogs before native comparison. */
    //-------------------------------------------------------------------------------------------------
    private static void Resolve(InstanceDeclaration instance, ManifestAsset[] catalog)
    {
        foreach (var r in instance.ReferencedInstances)
            if (catalog.Count(a => a.TypeId == r.TypeId && a.InstanceId == r.InstanceId) != 1) throw new InvalidDataException("Missing/ambiguous toggle dependency: " + r.Name);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exact copied schema declarations, default/sequence/reference annotations and the EP1 locomotor name order must agree before use. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckSchema(string fixture)
    {
        var focused = Read(fixture); string root = SdkEnvironmentPreflight.BaselineSchemaRoot();
        foreach (var pair in new[] { ("Modules/ToggleStatusSpecialAbilityUpdate.xsd", new[] { "ToggleStatusNugget", "ToggleStatusSpecialAbilityUpdateModuleData" }),
            ("Modules/ActivateModuleSpecialAbilityUpdate.xsd", new[] { "TriggerType", "SpecialPowerModuleInfo" }), ("Modules/BaseModules.xsd", new[] { "AnimAndDuration" }),
            ("AssetTypeArmorTemplate.xsd", new[] { "ArmorSetType" }) })
        {
            var stock = Read(Path.Combine(root, pair.Item1));
            foreach (string name in pair.Item2) if (Shape(Select(focused, name)) != Shape(Select(stock, name))) throw new InvalidDataException("Toggle module declaration drift: " + name);
        }
        var global = Read(Path.Combine(root, "Includes", "GlobalGameData.xsd"));
        var names = Select(global, "LocomotorSetType").GetElementsByTagName("enumeration", "http://www.w3.org/2001/XMLSchema").Cast<XmlElement>().Select(e => e.GetAttribute("value"));
        if (!Enum.GetNames<Ep1ToggleLocomotorSetType>().SequenceEqual(names)) throw new InvalidDataException("EP1 toggle locomotor order drift.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: schema drift readers are bounded and cannot resolve external XML entities. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Read(string path)
    {
        var d = new XmlDocument { XmlResolver = null }; using var s = new MemoryStream(SdkEnvironmentPreflight.Read(path, 2 * 1048576), false);
        using var r = XmlReader.Create(s, new XmlReaderSettings { XmlResolver = null, DtdProcessing = DtdProcessing.Prohibit }); d.Load(r); return d;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare only explicit selected type declarations, not an implicitly admitted full schema graph. */
    //-------------------------------------------------------------------------------------------------
    private static XmlElement Select(XmlDocument d, string name)
    {
        var ns = new XmlNamespaceManager(d.NameTable); ns.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
        return (XmlElement)(d.SelectSingleNode("/xs:schema/*[@name='" + name + "']", ns) ?? throw new InvalidDataException("Missing toggle declaration: " + name));
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: declaration comparison ignores only comments/indentation/redundant namespace syntax, retaining semantic attribute and child order. */
    //-------------------------------------------------------------------------------------------------
    private static string Shape(XmlElement e) => e.NamespaceURI + ":" + e.LocalName + "[" + string.Join("|", e.Attributes.Cast<XmlAttribute>().Where(a => a.NamespaceURI != "http://www.w3.org/2000/xmlns/")
        .OrderBy(a => a.NamespaceURI + ":" + a.LocalName, StringComparer.Ordinal).Select(a => a.NamespaceURI + ":" + a.LocalName + "=" + a.Value)) + "]{" + string.Join("", e.ChildNodes.OfType<XmlElement>().Select(Shape)) + "}";
    //-------------------------------------------------------------------------------------------------
    /** Reborn: native checks remain bounded to the owned chunk. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b, int at) { if (at < 0 || at > b.Length - 4) throw new InvalidDataException("Toggle Core native bounds exceeded."); return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at, 4)); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: schema, local-ID and strong-reference failures must not become partial successes. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action) { try { action(); } catch (Exception e) when (e is InvalidDataException or BinaryAssetBuilderException or System.Xml.Schema.XmlSchemaValidationException) { return; } throw new InvalidDataException("Invalid toggle Core source accepted."); }
}
