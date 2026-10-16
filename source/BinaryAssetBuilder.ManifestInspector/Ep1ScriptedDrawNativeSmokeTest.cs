using System.Buffers.Binary;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
using Chunk = Relo.Chunk;
using Tracker = Relo.Tracker;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: compile both complete official Conscript scripted-draw leaves, not a reduced test unit; generic metadata/publication and XInclude execution remain separate gates.
internal static class Ep1ScriptedDrawNativeSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain every source child/script and compare complete typed native graphs, exact auxiliary ownership and semantic stock import identities. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string sourceRoot, string manifest, string globalManifest, bool includeTokens = false, bool includeContainer = false)
    {
        var saved = Settings.Current;
        try
        {
            XmlSchemaSet? official = null; SdkEffectiveSchema.Inspect(shieldCandidate: true, reviewHooks: true, onAdmitted: set => official = set);
            if (official == null) throw new InvalidDataException("Reviewed scripted-draw schema required.");
            // Reborn: this in-memory element alias exposes the unchanged official concrete type to a standalone diagnostic envelope, not a production schema normalization.
            var alias = new XmlSchema { TargetNamespace = SchemaSet.XmlNamespace, ElementFormDefault = XmlSchemaForm.Qualified };
            alias.Items.Add(new XmlSchemaElement { Name = "ScriptedDrawProbe", SchemaTypeName = new XmlQualifiedName("W3DScriptedModelDrawModuleData", SchemaSet.XmlNamespace) });
            // Reborn: the particle alias uses the unchanged official type for authored field/AssetId controls, not an invented reduced declaration.
            alias.Items.Add(new XmlSchemaElement { Name = "ParticleProbe", SchemaTypeName = new XmlQualifiedName("ParticleSysBone", SchemaSet.XmlNamespace) });
            // Reborn: this diagnostic asset envelope uses the unchanged official DrawModules type for one Core pass, not whole GameObject admission.
            var drawEnvelope = new XmlSchemaComplexType { ContentModel = new XmlSchemaComplexContent { Content = new XmlSchemaComplexContentExtension { BaseTypeName = new XmlQualifiedName("BaseAssetType", SchemaSet.XmlNamespace), Particle = new XmlSchemaSequence { Items = { new XmlSchemaElement { Name = "Draws", SchemaTypeName = new XmlQualifiedName("DrawModules", SchemaSet.XmlNamespace) } } } } } };
            alias.Items.Add(new XmlSchemaElement { Name = "DrawsProbe", SchemaType = drawEnvelope });
            official.Add(alias); official.Compile();
            Settings.Current = new Settings { SchemaPath = Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!, "TracerDrawPipeline.xsd"), BuildCache = false, StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            var schemas = new SchemaSet(false);
            // Reborn: remove the seed declarations only from this owned diagnostic SchemaSet; never edit the captured official schema files.
            foreach (XmlSchema seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray()) schemas.Schemas.Remove(seed);
            schemas.Schemas.Add(official); schemas.Schemas.Compile();
            // Reborn: rebind the existing Core reference helpers to this exact reviewed set; stale fixture inheritance/dependency caches are deliberately never used by this focused normalization seam.
            var setup = typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes", BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new MissingMethodException("Core schema helper setup missing."); setup.Invoke(schemas, null);
            // Reborn: authored controls must not derive enum expectations from the same unchecked C# ordinal surface as the marshaler.
            foreach (var surface in new[] { ("FXTriggerType", Enum.GetNames<FXTriggerType>()), ("FXActionType", Enum.GetNames<FXActionType>()), ("AnimationMode", Enum.GetNames<AnimationMode>()), ("AnimationStateFlag", Enum.GetNames<AnimationStateFlag>()) })
            {
                var simple = (XmlSchemaSimpleType)official.GlobalTypes[new XmlQualifiedName(surface.Item1, SchemaSet.XmlNamespace)]!;
                if (!((XmlSchemaSimpleTypeRestriction)simple.Content!).Facets.OfType<XmlSchemaEnumerationFacet>().Select(f => f.Value).SequenceEqual(surface.Item2)) throw new InvalidDataException("Scripted enum order drift: " + surface.Item1);
            }
            var stock = GameObjectStockReview.Capture(manifest, "GameObject:SovietAntiInfantryInfantry");
            var catalog = stock.Document.Assets.Concat(Catalog(globalManifest).Assets).ToArray();
            var modules = GameObjectModuleReview.Inspect(manifest, stock.Asset.Name).Containers.Single(c => c.Name == "Draws").Modules.Where(m => m.Type == "W3DScriptedModelDrawModuleData").ToArray();
            if (modules.Length != 2 || Marshal.SizeOf<W3DScriptedModelDrawModuleData>() != 216) throw new InvalidDataException("Both scripted draws and reviewed ABI required.");
            int particleBindings = CheckParticleBindings(stock.Bytes, modules[0].Wrapper);
            CompilerSmokeTest.TestScriptedModelRecords(); AuthoredParticleControls(schemas);
            var files = new[] { Path.Combine(sourceRoot, "Soviet", "Units", "SovietAntiInfantryInfantry.xml"), Path.Combine(sourceRoot, "Includes", "ParachuteDrawSoviet.xml") };
            int totalRecords = 0, totalStrings = 0, totalImports = 0;
            // Reborn: keep authored names before per-leaf normalization for the common container occurrence table.
            var scriptedSources = new List<string>();
            for (int index = 0; index < files.Length; index++)
            {
                byte[] bytes = SdkEnvironmentPreflight.Read(files[index], 1048576); var digest = SHA256.HashData(bytes); var source = Xml(bytes);
                var draws = source.SelectNodes("//*[local-name()='ScriptedModelDraw' and namespace-uri()='uri:ea.com:eala:asset']")!.Cast<XmlElement>().ToArray();
                if (draws.Length != 1) throw new InvalidDataException("Expected one complete scripted leaf per captured source file.");
                var draw = draws[0]; int models = draw.SelectNodes("ea:ModelConditionState", Namespaces(draw))!.Count, animations = draw.SelectNodes("ea:AnimationState", Namespaces(draw))!.Count;
                if (models != 5 || animations != (index == 0 ? 80 : 4)) throw new InvalidDataException("Complete official draw child inventory changed.");
                var probe = Xml(System.Text.Encoding.UTF8.GetBytes("<ScriptedDrawProbe xmlns='uri:ea.com:eala:asset'/>"));
                foreach (XmlAttribute attribute in draw.Attributes) if (attribute.Prefix != "xmlns" && attribute.Name != "xmlns") probe.DocumentElement!.SetAttribute(attribute.LocalName, attribute.Value);
                foreach (XmlNode child in draw.ChildNodes) probe.DocumentElement!.AppendChild(probe.ImportNode(child, true));
                probe.Schemas.Add(official);
                // Reborn: official source interleaves bone/animation families; retain raw diagnostics and every member while grouping known particles for strict Core validation.
                int originalElements = probe.SelectNodes("//*")!.Count;
                string[] scripts = probe.SelectNodes("//ea:Script", Namespaces(probe.DocumentElement!))!.Cast<XmlElement>().Select(n => n.InnerText).ToArray();
                var rawDiagnostics = new List<string>(); probe.Validate((_, e) => rawDiagnostics.Add(e.Message));
                int reordered = PrepareParticleOrder(probe);
                if (probe.SelectNodes("//*")!.Count != originalElements || !scripts.SequenceEqual(probe.SelectNodes("//ea:Script", Namespaces(probe.DocumentElement!))!.Cast<XmlElement>().Select(n => n.InnerText))) throw new InvalidDataException("Particle preparation lost source members/script contents.");
                probe.Validate((_, e) => throw new InvalidDataException(e.Message));
                if (index == 0)
                {
                    // Reborn: source preparation refuses unknown members and typed render lookup refuses both missing and ambiguous stock identities.
                    var unknown = Xml(System.Text.Encoding.UTF8.GetBytes(probe.OuterXml)); unknown.DocumentElement!.AppendChild(unknown.CreateElement("UnknownDrawChild", SchemaSet.XmlNamespace)); Refuse(() => PrepareParticleOrder(unknown));
                    var missing = Xml(System.Text.Encoding.UTF8.GetBytes(probe.OuterXml)); var model = (XmlElement)missing.SelectSingleNode("//ea:Model[@Name!='']", Namespaces(missing.DocumentElement!))!; string modelName = model.GetAttribute("Name"); model.SetAttribute("Name", "RebornMissingRenderModel"); Refuse(() => ResolveRenderNames(missing, schemas, catalog));
                    var ambiguous = Xml(System.Text.Encoding.UTF8.GetBytes(probe.OuterXml)); uint modelId = new InstanceHandle(modelName).InstanceId; var duplicates = catalog.Where(a => a.InstanceId == modelId).ToArray(); Refuse(() => ResolveRenderNames(ambiguous, schemas, catalog.Concat(duplicates).ToArray()));
                }
                int resolved = ResolveRenderNames(probe, schemas, catalog);
                scriptedSources.Add(probe.OuterXml);
                var instance = ReferencePipelineSmokeTest.Normalize(schemas, probe.OuterXml);
                foreach (var reference in instance.ReferencedInstances) if (catalog.Count(a => a.TypeId == reference.TypeId && a.InstanceId == reference.InstanceId) != 1) throw new InvalidDataException("Scripted draw dependency missing/ambiguous: " + reference.Name);
                var native = Compile(instance.XmlNode);
                var comparer = new SelectedNativeGraphComparer(stock.Bytes, stock.Asset.References.ToArray(), native, instance, allowScriptCrLfEquivalent: true);
                var result = comparer.Compare(typeof(W3DScriptedModelDrawModuleData), modules[index].Native);
                // Reborn: complete independent stock metadata anchors both full graphs and a new weak module ID/scalar edit without reducing their child inventories.
                if (includeTokens)
                {
                    int prefix = index == 0 ? 53052 : 4512;
                    var expected = Ep1ScriptedDrawTokenSmokeTest.Stock(native, stock.Bytes, modules[index].Wrapper, prefix, comparer.NativeOffsets, controls: true);
                    var edited = Xml(System.Text.Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml));
                    string newId = "RebornCompleteScriptedDraw" + index;
                    edited.DocumentElement!.SetAttribute("id", newId);
                    edited.DocumentElement.SetAttribute("MaxRecoilDistance", "17.5");
                    edited.DocumentElement.SetAttribute("NoRotate", "true");
                    edited.DocumentElement.SetAttribute("UseProducerTexture", "true");
                    var editedInstance = ReferencePipelineSmokeTest.Normalize(schemas, edited.OuterXml);
                    var editedNative = Compile(editedInstance.XmlNode);
                    if (Word(editedNative.InstanceBuffer, 4) != FastHash.GetHashCode(newId.ToLowerInvariant()) || Word(editedNative.InstanceBuffer, 20) != 0x418C0000 || editedNative.InstanceBuffer[200] != 1 || editedNative.InstanceBuffer[201] != 1) throw new InvalidDataException("Complete scripted authored edit not applied.");
                    Ep1ScriptedDrawTokenSmokeTest.Verify(BinaryAssetBuilder.XmlCompiler.Ep1ScriptedDrawTokenizer.Tokenize(editedNative), editedNative, prefix, expected);
                    Console.WriteLine("Complete scripted source edit/new module ID: PASS; full child topology retained.");
                }
                if (result.ScriptLineEndingEquivalences != index) throw new InvalidDataException("Selected script newline evidence inventory changed.");
                var repeat = Compile(instance.XmlNode);
                if (!native.InstanceBuffer.SequenceEqual(repeat.InstanceBuffer) || !native.RelocationBuffer.SequenceEqual(repeat.RelocationBuffer) || !native.ImportsBuffer.SequenceEqual(repeat.ImportsBuffer)) throw new InvalidDataException("Complete scripted draw repeat differs.");
                if (!digest.SequenceEqual(SHA256.HashData(SdkEnvironmentPreflight.Read(files[index], 1048576)))) throw new InvalidDataException("Official draw source changed during capture.");
                totalRecords += result.Records; totalStrings += result.Strings; totalImports += result.Imports;
                Console.WriteLine($"Scripted draw {draw.GetAttribute("id")}: models/animationStates={models}/{animations}; native BIN/RELO/IMP={native.InstanceBuffer.Length}/{native.RelocationBuffer.Length}/{native.ImportsBuffer.Length}; raw schema diagnostics={rawDiagnostics.Count}, regrouped particles={reordered}, resolved render names={resolved}; complete stock/native graph PASS (explicit Script CRLF/LF equivalences={result.ScriptLineEndingEquivalences}); source SHA256={Convert.ToHexString(digest)}.");
                // Reborn: detached changes in root scalars, pointed lists, import words, script/text data and auxiliary slots cannot hide behind repeatability.
                foreach (int at in new[] { 0, 4, 16, 164, 168, 172, native.InstanceBuffer.Length - 1 })
                { var bad = Clone(native); bad.InstanceBuffer[at] ^= 1; Refuse(() => new SelectedNativeGraphComparer(stock.Bytes, stock.Asset.References.ToArray(), bad, instance, true).Compare(typeof(W3DScriptedModelDrawModuleData), modules[index].Native)); }
                foreach (bool relo in new[] { false, true })
                { var bad = Clone(native); if (relo) bad.RelocationBuffer = Array.Empty<byte>(); else bad.ImportsBuffer = Array.Empty<byte>(); Refuse(() => new SelectedNativeGraphComparer(stock.Bytes, stock.Asset.References.ToArray(), bad, instance, true).Compare(typeof(W3DScriptedModelDrawModuleData), modules[index].Native)); }
                // Reborn: the one explicit newline equivalence must never accept a changed script character or a changed script length/terminator.
                int stateItems = (int)Word(native.InstanceBuffer, 176), stateCount = (int)Word(native.InstanceBuffer, 172);
                int scriptHeader = Enumerable.Range(0, stateCount).Select(i => (int)Word(native.InstanceBuffer, stateItems + i * 140 + 108)).First(p => p != 0);
                int scriptData = (int)Word(native.InstanceBuffer, scriptHeader + 4);
                foreach (int at in new[] { scriptHeader, scriptData }) { var bad = Clone(native); bad.InstanceBuffer[at] ^= 1; Refuse(() => new SelectedNativeGraphComparer(stock.Bytes, stock.Asset.References.ToArray(), bad, instance, true).Compare(typeof(W3DScriptedModelDrawModuleData), modules[index].Native)); }
            }
            Console.WriteLine($"EP1 complete scripted Draw native graphs: PASS (two stock graphs, 10 model/84 animation states, {totalRecords} visited native records/{totalStrings} string allocations/{totalImports} typed import slots, {particleBindings} complete particle token/native bindings, 288 authored particle/3 flag-default controls and existing combined record fixture, 22 detached/3 source-dependency refusals; one explicitly bounded Script CRLF/LF equivalence; generated metadata checked={includeTokens}; no complete GameObject publication, XInclude execution or LUA execution claim).");
            // Reborn: compile both full scripted leaves and both source tracers together after the independent component gates.
            if (includeContainer) Ep1DrawsCompositionSmokeTest.Run(schemas, sourceRoot, manifest, catalog, scriptedSources.ToArray());
        }
        finally { Settings.Current = saved; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: every selected particle's nine fields are independently anchored by stock metadata, including the eight-byte AssetId string and trailing bools. */
    //-------------------------------------------------------------------------------------------------
    private static int CheckParticleBindings(byte[] bytes, int wrapper)
    {
        var own = GameObjectStockReview.Tokens(bytes, (int)Word(bytes, wrapper + 8)); int found = 0;
        foreach (string family in new[] { "ModelConditionState", "AnimationState" })
        {
            int states = own[FastHash.GetHashCode(family)].Offset, countAt = (int)Word(bytes, states + 8), count = (int)Word(bytes, countAt);
            for (int i = 0; i < count; i++)
            {
                var fields = GameObjectStockReview.Tokens(bytes, (int)Word(bytes, states + 20 + i * 12)); int list = fields[FastHash.GetHashCode("ParticleSysBone")].Offset;
                if (Word(bytes, list) != 0x6C827073 || Word(bytes, list + 4) != 0x5D448FD8) throw new InvalidDataException("Particle stock count marker differs.");
                int header = (int)Word(bytes, list + 8), particles = (int)Word(bytes, header), items = (int)Word(bytes, header + 4);
                for (int j = 0; j < particles; j++)
                {
                    if (Word(bytes, list + 12 + j * 12) != 0xB151A0EB || Word(bytes, list + 16 + j * 12) != 0) throw new InvalidDataException("Particle stock item marker differs.");
                    var particle = GameObjectStockReview.Tokens(bytes, (int)Word(bytes, list + 20 + j * 12));
                    var definitions = new[] { ("BoneName", 0, 0x1A2933AAu), ("FXParticleSystemTemplate", 8, 0x43F1CE36u), ("FXTrigger", 12, 0x808B85BEu), ("FXAction", 16, 0x7C49BC33u), ("PersistID", 20, 0x23E650B8u), ("id", 24, 0x805D25F4u), ("FollowBone", 32, 0x77E3F4F7u), ("OnlyIfOnWater", 33, 0x77E3F4F7u), ("OnlyIfOnLand", 34, 0x77E3F4F7u) };
                    if (particle.Count != definitions.Length) throw new InvalidDataException("Particle stock field count differs.");
                    foreach (var definition in definitions) if (!particle.TryGetValue(FastHash.GetHashCode(definition.Item1), out var token) || token.Type != definition.Item3 || token.Offset != items + j * 36 + definition.Item2) throw new InvalidDataException("Particle stock field ABI differs: " + definition.Item1);
                    found++;
                }
            }
        }
        if (found != 14) throw new InvalidDataException("All fourteen complete Conscript particles required."); return found;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: omitted/empty/nonempty particle AssetId strings and all enum/bool combinations must retain the independent EA/native offsets and exact string relocation ownership. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void AuthoredParticleControls(SchemaSet schemas)
    {
        int cases = 0;
        foreach (string trigger in Enum.GetNames<FXTriggerType>()) foreach (string action in Enum.GetNames<FXActionType>()) foreach (int mask in Enumerable.Range(0, 8)) foreach (string? id in new string?[] { null, "", "RebornParticle" })
        {
            // Reborn: this selected route uses the official source's true/false spellings; it does not widen the legacy general Boolean parser to numeric spellings.
            string source = $"<ParticleProbe xmlns='{SchemaSet.XmlNamespace}' BoneName='FXMUZZLE' FXTrigger='{trigger}' FXAction='{action}' PersistID='-17' FollowBone='{((mask & 1) != 0 ? "true" : "false")}' OnlyIfOnWater='{((mask & 2) != 0 ? "true" : "false")}' OnlyIfOnLand='{((mask & 4) != 0 ? "true" : "false")}'" + (id == null ? "" : " id='" + id + "'") + "/>";
            // Reborn: ParticleSysBone is a nested non-asset record, so absent id must be schema-validated directly rather than invented as an asset InstanceDeclaration.
            var xml = Xml(System.Text.Encoding.UTF8.GetBytes(source)); xml.Schemas.Add(schemas.Schemas); xml.Validate((_, e) => throw new InvalidDataException(e.Message)); ParticleSysBone* particle;
            using var tracker = new Tracker((void**)&particle, (uint)sizeof(ParticleSysBone), false); Marshaler.Marshal(new Node(xml.DocumentElement!.CreateNavigator()!, Namespaces(xml.DocumentElement)), particle, tracker); var chunk = new Chunk(); if (!tracker.MakeRelocatable(chunk)) throw new InvalidDataException("Particle probe allocation failed."); var b = chunk.InstanceBuffer;
            if (Word(b, 12) != (uint)Enum.Parse<FXTriggerType>(trigger) || Word(b, 16) != (uint)Enum.Parse<FXActionType>(action) || Word(b, 20) != unchecked((uint)-17) || Word(b, 24) != (uint)(id?.Length ?? 0) || b[32] != (mask & 1) || b[33] != ((mask >> 1) & 1) || b[34] != ((mask >> 2) & 1) || b[35] != 0 || chunk.ImportsBuffer.Length != 0 || !Slots(chunk.RelocationBuffer).SequenceEqual(id == null ? new[] { 4u } : new[] { 4u, 28u })) throw new InvalidDataException("Authored particle ABI/string/flag control differs.");
            uint pointer = Word(b, 28); if (id == null ? pointer != 0 : pointer < 36 || pointer + id.Length >= b.Length || b[(int)pointer + id.Length] != 0 || System.Text.Encoding.ASCII.GetString(b, (int)pointer, id.Length) != id) throw new InvalidDataException("Particle AssetId header/content differs."); cases++;
        }
        if (cases != 288) throw new InvalidDataException("Particle author control inventory changed.");
        foreach (var flags in new[] { ("", 1u), (" Flags='NONE'", 1u), (" Flags=''", 0u) })
        {
            var xml = Xml(System.Text.Encoding.UTF8.GetBytes("<AnimationState xmlns='uri:ea.com:eala:asset' ParseCondStateType='PARSE_NORMAL'" + flags.Item1 + "/>")); AnimationState* state;
            using var tracker = new Tracker((void**)&state, (uint)sizeof(AnimationState), false); Marshaler.Marshal(new Node(xml.DocumentElement!.CreateNavigator()!, Namespaces(xml.DocumentElement)), state, tracker); var chunk = new Chunk(); if (!tracker.MakeRelocatable(chunk) || Word(chunk.InstanceBuffer, 88) != flags.Item2) throw new InvalidDataException("Animation NONE/empty/default control differs.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stable family grouping preserves every source member and within-family order; unknown children and mixed Script contents are never rewritten or silently dropped. */
    //-------------------------------------------------------------------------------------------------
    private static int PrepareParticleOrder(XmlDocument document)
    {
        int changed = 0;
        foreach (XmlElement parent in document.SelectNodes("//ea:ScriptedDrawProbe | //ea:ModelConditionState | //ea:AnimationState", Namespaces(document.DocumentElement!))!)
        {
            string[] order = parent.LocalName switch { "ScriptedDrawProbe" => new[] { "ModelConditionState", "AnimationState", "AttachModel", "EmbedPortal" }, "ModelConditionState" => new[] { "Model", "Material", "Texture", "WeaponFireFXBone", "WeaponRecoilBone", "WeaponMuzzleFlash", "WeaponLaunchBone", "ParticleSysBone", "FXEvent", "ShadowInfo", "Turret", "SubObject" }, "AnimationState" => new[] { "Animation", "Script", "FXEvent", "LuaEvent", "ParticleSysBone" }, _ => throw new InvalidDataException("Unknown scripted particle preparation.") };
            var children = parent.ChildNodes.OfType<XmlElement>().ToArray();
            if (children.Any(c => c.NamespaceURI != SchemaSet.XmlNamespace || !order.Contains(c.LocalName))) throw new InvalidDataException("Unknown scripted particle cannot be prepared.");
            var sorted = children.OrderBy(c => Array.IndexOf(order, c.LocalName)).ToArray();
            if (!children.SequenceEqual(sorted)) { foreach (var child in sorted) parent.AppendChild(child); changed++; }
        }
        return changed;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: resolve untyped BaseRenderAssetType names by allowed schema-derived catalog identities before actual Core normalization; no captured reference index is authored. */
    //-------------------------------------------------------------------------------------------------
    private static int ResolveRenderNames(XmlDocument document, SchemaSet schemas, ManifestAsset[] catalog)
    {
        int resolved = 0; var render = schemas.GetXmlType("BaseRenderAssetType") ?? throw new InvalidDataException("Render reference schema missing.");
        var allowed = schemas.Schemas.GlobalTypes.Values.Cast<XmlSchemaType>().Where(t => XmlSchemaType.IsDerivedFrom(t, render, XmlSchemaDerivationMethod.None)).Select(t => FastHash.GetHashCode(t.Name!)).ToHashSet();
        foreach (XmlElement model in document.SelectNodes("//ea:Model", Namespaces(document.DocumentElement!))!)
        {
            string name = model.GetAttribute("Name").Trim(); if (name.Length == 0 || name.Contains(':')) continue;
            var handle = new InstanceHandle(name); var targets = catalog.Where(a => a.InstanceId == handle.InstanceId && allowed.Contains(a.TypeId)).ToArray();
            if (targets.Length != 1) throw new InvalidDataException("Render name missing/ambiguous: " + name);
            model.SetAttribute("Name", targets[0].Name); resolved++;
        }
        return resolved;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: actual Draw dispatch owns all allocations; only known module-local relocation pointers are rebased when extracting the complete selected leaf. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe Chunk Compile(XmlNode source)
    {
        var xml = Xml(System.Text.Encoding.UTF8.GetBytes("<GameObject xmlns='uri:ea.com:eala:asset'><Draws/></GameObject>")); var leaf = (XmlElement)xml.ImportNode(source, true); xml.DocumentElement!.FirstChild!.AppendChild(leaf); leaf.SetAttribute("TypeId", 0x4ECF2A4Bu.ToString(CultureInfo.InvariantCulture));
        GameObject* root; using var tracker = new Tracker((void**)&root, (uint)sizeof(GameObject), false);
        Marshaler.Marshal(new Node(xml.DocumentElement.CreateNavigator()!, Namespaces(xml.DocumentElement)), root, tracker); var chunk = new Chunk(); if (!tracker.MakeRelocatable(chunk)) throw new InvalidDataException("Scripted draw allocation failed.");
        int list = (int)Word(chunk.InstanceBuffer, 472), items = (int)Word(chunk.InstanceBuffer, list + 4), native = (int)Word(chunk.InstanceBuffer, items);
        if (Word(chunk.InstanceBuffer, list) != 1 || Word(chunk.InstanceBuffer, native) != 0x4ECF2A4B) throw new InvalidDataException("Scripted Draw dispatch identity differs.");
        uint[] relos = Slots(chunk.RelocationBuffer), imports = Slots(chunk.ImportsBuffer); uint[] parent = { 472, (uint)(list + 4), (uint)items };
        if (!relos.Where(s => s < native).SequenceEqual(parent) || imports.Any(s => s < native)) throw new InvalidDataException("Scripted parent-container auxiliary ownership differs.");
        var bytes = chunk.InstanceBuffer.AsSpan(native).ToArray(); var local = relos.Where(s => s >= native).Select(s => s - (uint)native).ToArray();
        foreach (uint slot in local) { uint pointer = Word(bytes, (int)slot); if (pointer < native || pointer >= chunk.InstanceBuffer.Length) throw new InvalidDataException("Scripted native pointer escapes selected graph."); Put(bytes, (int)slot, pointer - (uint)native); }
        return new Chunk { InstanceBuffer = bytes, RelocationBuffer = Pack(local), ImportsBuffer = Pack(imports.Select(s => s - (uint)native).ToArray()) };
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: input XML is read from bounded captured bytes with DTD and external entity resolution disabled, retaining script whitespace. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Xml(byte[] bytes) { using var stream = new MemoryStream(bytes); using var reader = XmlReader.Create(stream, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null }); var xml = new XmlDocument { XmlResolver = null, PreserveWhitespace = true }; xml.Load(reader); return xml; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: explicit namespaces avoid accepting unrelated elements with matching local names. */
    //-------------------------------------------------------------------------------------------------
    private static XmlNamespaceManager Namespaces(XmlNode node) { var ns = new XmlNamespaceManager(node.OwnerDocument!.NameTable); ns.AddNamespace("ea", SchemaSet.XmlNamespace); return ns; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: only valid linked stock catalogs contribute target identities. */
    //-------------------------------------------------------------------------------------------------
    private static ManifestDocument Catalog(string path) { var d = ManifestReader.Read(SdkEnvironmentPreflight.Read(path, 64 * 1048576)); TypeRegistryAudit.ValidateTarget(d.Header.Version, d.Header.AllTypesHash); if (!d.Header.IsLinked || d.Validate().Count != 0) throw new InvalidDataException("Scripted target catalog invalid."); return d; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: detached faults must throw a diagnostic error, not become an accepted partial graph. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action) { try { action(); } catch (InvalidDataException) { return; } throw new InvalidDataException("Detached scripted graph corruption accepted."); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: every fault owns all three buffers. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c) => new() { InstanceBuffer = c.InstanceBuffer.ToArray(), RelocationBuffer = c.RelocationBuffer.ToArray(), ImportsBuffer = c.ImportsBuffer.ToArray() };
    //-------------------------------------------------------------------------------------------------
    /** Reborn: Tracker slots exclude their terminal sentinel during coordinate conversion. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b) => b.Length == 0 ? Array.Empty<uint>() : Enumerable.Range(0, b.Length / 4 - 1).Select(i => Word(b, i * 4)).ToArray();
    //-------------------------------------------------------------------------------------------------
    /** Reborn: nonempty auxiliary buffers retain one sentinel after rebasing. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(uint[] slots) { if (slots.Length == 0) return Array.Empty<byte>(); var b = new byte[(slots.Length + 1) * 4]; for (int i = 0; i < slots.Length; i++) Put(b, i * 4, slots[i]); Put(b, slots.Length * 4, uint.MaxValue); return b; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: diagnostic reads remain within owned buffers. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b, int at) { if (at < 0 || at > b.Length - 4) throw new InvalidDataException("Scripted native word bounds exceeded."); return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at, 4)); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: rebasing changes only newly owned native bytes. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b, int at, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at, 4), value);
}
