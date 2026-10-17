using System.Buffers.Binary;
using System.Globalization;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using SageBinaryData;
using Chunk = Relo.Chunk;
using Tracker = Relo.Tracker;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: all four complete Conscript Draws share one production normalization pass and Tracker; independent stock tables verify the enclosing list as well as every leaf.
internal static class Ep1DrawsCompositionSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve both full scripted leaves, read the actual two official tracers, then test ordering, subsets, edits, optional status and corrupt ownership in one batch. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(SchemaSet schemas, string sourceRoot, string manifest, ManifestAsset[] catalog, string[] scripted, bool includeRoot = false, bool includeClients = false,bool includeAllModules=false,bool includeRootOwners=false,bool includeEmptyRoots=false,bool includeOriginalRoot=false,string? originalGlobalManifest=null)
    {
        var stock = GameObjectStockReview.Capture(manifest, "GameObject:SovietAntiInfantryInfantry");
        var topology = GameObjectModuleReview.Inspect(manifest, stock.Asset.Name).Containers.Single(c => c.Name == "Draws");
        var source = Xml(Encoding.UTF8.GetString(SdkEnvironmentPreflight.Read(Path.Combine(sourceRoot, "Soviet", "Units", "SovietAntiInfantryInfantry.xml"), 1048576)));
        var tracers = source.SelectNodes("//*[local-name()='Draws' and namespace-uri()='uri:ea.com:eala:asset']/*[local-name()='TracerModelDraw']")!.Cast<XmlElement>().ToArray();
        if (scripted.Length != 2 || tracers.Length != 2 || topology.Modules.Length != 4) throw new InvalidDataException("Complete four-Draw source/stock inventory required.");
        var leaves = scripted.Select(s => Xml(s).DocumentElement!).Concat(tracers).ToArray();
        int stockList = GameObjectStockReview.Tokens(stock.Bytes, 0)[FastHash.GetHashCode("Draws")].Offset;
        int[] stockPrefixes = { 53052, 4512, 492, 492 }; int prefix = 60 + stockPrefixes.Sum();
        if (stockList + 60 != topology.Modules[0].Wrapper || topology.Modules.Where((m, i) => m.Wrapper != stockList + 60 + stockPrefixes.Take(i).Sum()).Any()) throw new InvalidDataException("Independent stock Draws prefix is not contiguous.");
        int[][] orders = { Array.Empty<int>(), new[] { 0 }, new[] { 1 }, new[] { 2 }, new[] { 3 }, new[] { 0, 1 }, new[] { 2, 3 }, new[] { 0, 1, 2, 3 }, new[] { 3, 2, 1, 0 }, new[] { 2, 0, 3, 1 } };
        Chunk? full = null; byte[]? fullExpected = null; InstanceDeclaration? fullInstance = null; int cases = 0, totalImports = 0;
        foreach (var order in orders)
        {
            var pair = Build(schemas, Envelope(leaves, order), catalog); var native = pair.Native;
            var map = new Dictionary<int, int>(); int itemCount = (int)Word(native.InstanceBuffer, 0), items = (int)Word(native.InstanceBuffer, 4);
            if (itemCount != order.Length || items != (order.Length == 0 ? 0 : 8)) throw new InvalidDataException("Shared Draws native count/order differs.");
            for (int i = 0; i < order.Length; i++)
            {
                int at = (int)Word(native.InstanceBuffer, items + i * 4); var leaf = Leaf(native, i);
                var comparer = new SelectedNativeGraphComparer(stock.Bytes, stock.Asset.References.ToArray(), leaf, pair.Instance, allowScriptCrLfEquivalent: true, requireAllReferences: false);
                var result = comparer.Compare(order[i] < 2 ? typeof(W3DScriptedModelDrawModuleData) : typeof(W3DTracerDrawModuleData), topology.Modules[order[i]].Native);
                if (result.ScriptLineEndingEquivalences != (order[i] == 1 ? 1 : 0)) throw new InvalidDataException("Shared Draws Script newline boundary changed.");
                foreach (var entry in comparer.NativeOffsets) map.Add(entry.Key, entry.Value + at);
            }
            var output = Ep1DrawsTokenizer.Tokenize(native); Verify(output, native, pair.Instance);
            var repeat = Build(schemas, Envelope(leaves, order), catalog); var repeated = Ep1DrawsTokenizer.Tokenize(repeat.Native);
            if (!output.InstanceBuffer.SequenceEqual(repeated.InstanceBuffer) || !output.RelocationBuffer.SequenceEqual(repeated.RelocationBuffer) || !output.ImportsBuffer.SequenceEqual(repeated.ImportsBuffer)) throw new InvalidDataException("Shared Draws repeated build differs.");
            if (order.SequenceEqual(new[] { 0, 1, 2, 3 }))
            {
                for (int j = 0; j < 8; j++) map.Add(topology.CountAt + j, j);
                for (int j = 0; j < 16; j++) map.Add(topology.Items + j, items + j);
                var expected = stock.Bytes.AsSpan(stockList, prefix).ToArray();
                for (int at = 8; at < expected.Length; at += 12)
                {
                    uint target = Word(expected, at); if (target == 0) continue;
                    Put(expected, at, target >= stockList && target < stockList + prefix ? target - (uint)stockList : map.TryGetValue((int)target, out int compiled) ? checked((uint)(prefix + compiled)) : throw new InvalidDataException("Stock Draws target escapes shared graph."));
                }
                if (!output.InstanceBuffer.AsSpan(0, prefix).SequenceEqual(expected)) throw new InvalidDataException("Complete independent shared Draws metadata differs.");
                Verify(output, native, pair.Instance, expected);
                full = native; fullExpected = expected; fullInstance = pair.Instance; totalImports = pair.Instance.ReferencedInstances.Count;
                Console.WriteLine($"Complete independent Draws prefix: {prefix} bytes PASS; native BIN/RELO/IMP={native.InstanceBuffer.Length}/{native.RelocationBuffer.Length}/{native.ImportsBuffer.Length}; Core occurrences={totalImports}.");
            }
            cases++;
        }
        // Reborn: edited full source keeps all four modules and all 10/84 model/animation states; this remains a container identity, not a new GameObject asset.
        var edited = Xml(Envelope(leaves, orders[7])); int localId = 0;
        foreach (XmlElement module in edited.DocumentElement!.FirstChild!.ChildNodes.OfType<XmlElement>())
        { module.SetAttribute("id", "RebornSharedDraw" + localId++); if (module.LocalName == "ScriptedModelDraw") module.SetAttribute("MaxRecoilDistance", "17.5"); else module.SetAttribute("Width", "17.5"); }
        var editPair = Build(schemas, edited.OuterXml, catalog); var editOutput = Ep1DrawsTokenizer.Tokenize(editPair.Native); Verify(editOutput, editPair.Native, editPair.Instance);
        if (!editOutput.InstanceBuffer.AsSpan(0, prefix).SequenceEqual(fullExpected)) throw new InvalidDataException("Edited complete Draws topology differs.");
        for (int i = 0; i < 4; i++) { int at = (int)Word(editPair.Native.InstanceBuffer, 8 + i * 4); if (Word(editPair.Native.InstanceBuffer, at + 4) != FastHash.GetHashCode(("RebornSharedDraw" + i).ToLowerInvariant()) || Word(editPair.Native.InstanceBuffer, at + (i < 2 ? 20 : 16)) != 0x418C0000) throw new InvalidDataException("Shared source edit not applied."); } cases++;
        var optional = Xml(Envelope(leaves, orders[7])); var statuses = optional.SelectNodes("//*[local-name()='TracerModelDraw']/*[local-name()='ObjectStatusValidation']")!; if (statuses.Count != 2) throw new InvalidDataException("Both optional tracer status records required."); foreach (XmlNode status in statuses) status.ParentNode!.RemoveChild(status);
        var optionalPair = Build(schemas, optional.OuterXml, catalog); var optionalOutput = Ep1DrawsTokenizer.Tokenize(optionalPair.Native); Verify(optionalOutput, optionalPair.Native, optionalPair.Instance);
        if (Leaf(optionalPair.Native, 2).InstanceBuffer.Length != 112 || Leaf(optionalPair.Native, 3).InstanceBuffer.Length != 112 || optionalPair.Native.InstanceBuffer.Length != full!.InstanceBuffer.Length - 128 || optionalOutput.InstanceBuffer.Length - optionalPair.Native.InstanceBuffer.Length != prefix - 72) throw new InvalidDataException("Optional shared tracer owners not removed exactly."); cases++;
        int faults = Controls(full!, fullExpected!, prefix, fullInstance!);
        foreach (string bad in new[] { Envelope(leaves, new[] { 0, 0 }), Envelope(leaves, new[] { 2, 2 }), Envelope(leaves, new[] { 0, 1 }).Replace("ModuleTag_ParachuteDraw", "moduletag_draw", StringComparison.Ordinal), Envelope(leaves, orders[7]).Replace("Texture=\"FXTracer\"", "Texture=\"RebornMissingDrawTexture\"", StringComparison.Ordinal), Envelope(leaves, orders[7]).Replace("<Draws>", "<Draws><UnknownDraw id='Unknown'/>", StringComparison.Ordinal) })
        { Refuse(() => { var pair = Build(schemas, bad, catalog); Ep1DrawsTokenizer.Tokenize(pair.Native); }); faults++; }
        Console.WriteLine($"EP1 complete shared Draws: PASS ({cases} source cases, four full modules, one Core occurrence table/{totalImports} strong slots, full {prefix}-byte independent stock prefix, {faults} ownership/source and detached boundary refusals; no whole GameObject/package, XInclude execution or new gameplay claim).");
        // Reborn: root composition reuses the independent stock-proven full Draws oracle and original named sources, never separately normalized sibling fragments.
        // Reborn: the all-original-module profile is opt-in and retains existing client-only/selected-root regressions unchanged.
        if(includeAllModules)GameObjectAllModulesSmokeTest.Run(schemas,sourceRoot,manifest,catalog,leaves,includeRootOwners,includeEmptyRoots,includeOriginalRoot,originalGlobalManifest);
        else if(includeClients)GameObjectClientsCompositionSmokeTest.Run(schemas,sourceRoot,manifest,catalog,leaves,fullExpected!);
        else if(includeRoot)GameObjectDrawsCompositionSmokeTest.Run(schemas,manifest,catalog,leaves,full!,fullInstance!,fullExpected!);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: one typed envelope retains complete source members while explicitly naming the separately captured include leaf; no include engine is simulated. */
    //-------------------------------------------------------------------------------------------------
    private static string Envelope(XmlElement[] leaves, int[] order)
    {
        var xml = Xml("<DrawsProbe xmlns='uri:ea.com:eala:asset' id='RebornSharedDraws'><Draws/></DrawsProbe>");
        foreach (int index in order) { var leaf = xml.CreateElement(index < 2 ? "ScriptedModelDraw" : "TracerModelDraw", SchemaSet.XmlNamespace); foreach (XmlAttribute a in leaves[index].Attributes) if (a.Name != "xmlns" && a.Prefix != "xmlns") leaf.SetAttribute(a.LocalName, a.Value); foreach (XmlNode child in leaves[index].ChildNodes) leaf.AppendChild(xml.ImportNode(child, true)); xml.DocumentElement!.FirstChild!.AppendChild(leaf); } return xml.OuterXml;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: normalize all named dependencies together, enforce real catalog identities, then use actual Draw dispatch and one Tracker before extracting only the Draws-owned interval. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe (Chunk Native, InstanceDeclaration Instance) Build(SchemaSet schemas, string source, ManifestAsset[] catalog)
    {
        var instance = ReferencePipelineSmokeTest.Normalize(schemas, source);
        foreach (var reference in instance.ReferencedInstances) if (catalog.Count(a => a.TypeId == reference.TypeId && a.InstanceId == reference.InstanceId) != 1) throw new InvalidDataException("Shared Draws dependency missing/ambiguous.");
        var xml = Xml("<GameObject xmlns='uri:ea.com:eala:asset'/>"); var draws = (XmlElement)xml.ImportNode(instance.XmlNode.FirstChild!, true); xml.DocumentElement!.AppendChild(draws);
        foreach (XmlElement module in draws.ChildNodes.OfType<XmlElement>()) module.SetAttribute("TypeId", (module.LocalName switch { "ScriptedModelDraw" => 0x4ECF2A4Bu, "TracerModelDraw" => 0x54018E60u, _ => throw new InvalidDataException("Shared Draw class not enabled.") }).ToString(CultureInfo.InvariantCulture));
        GameObject* root; using var tracker = new Tracker((void**)&root, (uint)sizeof(GameObject), false); var ns = new XmlNamespaceManager(xml.NameTable); ns.AddNamespace("ea", SchemaSet.XmlNamespace);
        Marshaler.Marshal(new Node(xml.DocumentElement.CreateNavigator()!, ns), root, tracker); var compiled = new Chunk(); if (!tracker.MakeRelocatable(compiled)) throw new InvalidDataException("Shared Draws Tracker failed.");
        int list = (int)Word(compiled.InstanceBuffer, 472); var relos = Slots(compiled.RelocationBuffer); var imports = Slots(compiled.ImportsBuffer);
        if (list < 600 || !relos.Where(s => s < list).SequenceEqual(new[] { 472u }) || imports.Any(s => s < list)) throw new InvalidDataException("Draws parent ownership differs.");
        var native = new Chunk { InstanceBuffer = compiled.InstanceBuffer.AsSpan(list).ToArray(), RelocationBuffer = Pack(relos.Where(s => s >= list).Select(s => s - (uint)list)), ImportsBuffer = Pack(imports.Select(s => s - (uint)list)) };
        foreach (uint slot in Slots(native.RelocationBuffer)) { uint pointer = Word(native.InstanceBuffer, (int)slot); if (pointer < list || pointer >= compiled.InstanceBuffer.Length) throw new InvalidDataException("Draws pointer escapes complete list."); Put(native.InstanceBuffer, (int)slot, pointer - (uint)list); }
        return (native, instance);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: detach a known leaf only for independent typed stock comparison; its import values retain the shared occurrence indices. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Leaf(Chunk native, int index)
    {
        int start = (int)Word(native.InstanceBuffer, 8 + index * 4), end = index + 1 == Word(native.InstanceBuffer, 0) ? native.InstanceBuffer.Length : (int)Word(native.InstanceBuffer, 12 + index * 4);
        var leaf = new Chunk { InstanceBuffer = native.InstanceBuffer.AsSpan(start, end - start).ToArray(), RelocationBuffer = Pack(Slots(native.RelocationBuffer).Where(s => s >= start && s < end).Select(s => s - (uint)start)), ImportsBuffer = Pack(Slots(native.ImportsBuffer).Where(s => s >= start && s < end).Select(s => s - (uint)start)) };
        foreach (uint slot in Slots(leaf.RelocationBuffer)) Put(leaf.InstanceBuffer, (int)slot, Word(leaf.InstanceBuffer, (int)slot) - (uint)start); return leaf;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: inverse rebasing must recover every native byte/slot in order and every shared Core occurrence must be consumed exactly once. */
    //-------------------------------------------------------------------------------------------------
    private static void Verify(Chunk output, Chunk native, InstanceDeclaration instance, byte[]? expected = null)
    {
        int prefix = output.InstanceBuffer.Length - native.InstanceBuffer.Length; if (prefix < 12 || prefix % 12 != 0 || Word(output.InstanceBuffer, 0) != 0x6C827073 || Word(output.InstanceBuffer, 4) != 0x5D448FD8 || Word(output.InstanceBuffer, 8) != prefix) throw new InvalidDataException("Draws metadata header differs.");
        // Reborn: the complete stock-derived prefix is the detached-output oracle, not a second call to the producer.
        if (expected != null && (expected.Length != prefix || !output.InstanceBuffer.AsSpan(0, prefix).SequenceEqual(expected))) throw new InvalidDataException("Detached shared Draws metadata differs.");
        uint[] relos = Slots(native.RelocationBuffer), imports = Slots(native.ImportsBuffer);
        if (!Slots(output.RelocationBuffer).SequenceEqual(relos.Select(s => s + (uint)prefix)) || !Slots(output.ImportsBuffer).SequenceEqual(imports.Select(s => s + (uint)prefix))) throw new InvalidDataException("Draws auxiliary order/origin differs.");
        var recovered = output.InstanceBuffer.AsSpan(prefix).ToArray(); foreach (uint slot in relos) { uint pointer = Word(recovered, (int)slot); if (pointer != Word(native.InstanceBuffer, (int)slot) + (uint)prefix) throw new InvalidDataException("Draws pointer rebase differs."); Put(recovered, (int)slot, pointer - (uint)prefix); }
        if (!recovered.SequenceEqual(native.InstanceBuffer) || !imports.Select(s => Word(native.InstanceBuffer, (int)s)).Order().SequenceEqual(Enumerable.Range(1, instance.ReferencedInstances.Count).Select(i => (uint)i))) throw new InvalidDataException("Draws native payload/shared occurrence consumption differs.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: corrupt container ownership, classes, identities, cross-leaf pointers, auxiliary streams and complete detached metadata independently. */
    //-------------------------------------------------------------------------------------------------
    private static int Controls(Chunk native, byte[] expected, int prefix, InstanceDeclaration instance)
    {
        int faults = 0; int first = (int)Word(native.InstanceBuffer, 8), second = (int)Word(native.InstanceBuffer, 12);
        foreach (var edit in new[] { (0, 65u), (0, uint.MaxValue), (4, 12u), (8, (uint)second), (12, (uint)first), (12, (uint)second + 1), (first, 0xDEADBEEFu), (first + 4, Word(native.InstanceBuffer, second + 4)), (first + 168, (uint)second) }) { var bad = Clone(native); Put(bad.InstanceBuffer, edit.Item1, edit.Item2); Refuse(() => Ep1DrawsTokenizer.Tokenize(bad)); faults++; }
        foreach (bool relo in new[] { false, true }) { var bad = Clone(native); if (relo) bad.RelocationBuffer = Array.Empty<byte>(); else bad.ImportsBuffer = Array.Empty<byte>(); Refuse(() => Ep1DrawsTokenizer.Tokenize(bad)); faults++; }
        var duplicate = Clone(native); duplicate.RelocationBuffer = Pack(Slots(native.RelocationBuffer).Append(4u)); Refuse(() => Ep1DrawsTokenizer.Tokenize(duplicate)); faults++;
        var missingParent = Clone(native); missingParent.RelocationBuffer = Pack(Slots(native.RelocationBuffer).Where(s => s != 4)); Refuse(() => Ep1DrawsTokenizer.Tokenize(missingParent)); faults++;
        var tail = Clone(native); tail.InstanceBuffer = tail.InstanceBuffer.Concat(new byte[4]).ToArray(); Refuse(() => Ep1DrawsTokenizer.Tokenize(tail)); faults++;
        var output = Ep1DrawsTokenizer.Tokenize(native);
        for (int at = 0; at < prefix; at += 4) { var bad = Clone(output); Put(bad.InstanceBuffer, at, Word(bad.InstanceBuffer, at) ^ 1u); Refuse(() => Verify(bad, native, instance, expected)); faults++; }
        foreach (int at in new[] { 0, 4, 8, first + 4, first + 168, second }) { var bad = Clone(output); Put(bad.InstanceBuffer, prefix + at, Word(bad.InstanceBuffer, prefix + at) ^ 1u); Refuse(() => Verify(bad, native, instance, expected)); faults++; }
        foreach (bool relo in new[] { false, true }) { var bad = Clone(output); if (relo) bad.RelocationBuffer = Array.Empty<byte>(); else bad.ImportsBuffer = Array.Empty<byte>(); Refuse(() => Verify(bad, native, instance, expected)); faults++; }
        // Reborn: the writer preserves import indices; the shared Core consumer check, not an invented producer catalog, rejects missing/repeated/out-of-range occurrence words.
        int reference = (int)Slots(native.ImportsBuffer)[0]; foreach (uint value in new[] { 0u, (uint)instance.ReferencedInstances.Count + 1, Word(native.InstanceBuffer, (int)Slots(native.ImportsBuffer)[1]) }) { var bad = Clone(native); Put(bad.InstanceBuffer, reference, value); Refuse(() => Verify(Ep1DrawsTokenizer.Tokenize(bad), bad, instance)); faults++; }
        return faults;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: negative source/binary controls must fail explicitly rather than publishing incomplete Draws. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action) { try { action(); } catch (InvalidDataException) { return; } throw new InvalidDataException("Invalid shared Draws accepted."); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: source parsing retains script whitespace and disables external entities. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Xml(string source) { using var input = new StringReader(source); using var reader = XmlReader.Create(input, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null }); var xml = new XmlDocument { XmlResolver = null, PreserveWhitespace = true }; xml.Load(reader); return xml; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: mutation controls own all native buffers. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk native) => new() { InstanceBuffer = native.InstanceBuffer.ToArray(), RelocationBuffer = native.RelocationBuffer.ToArray(), ImportsBuffer = native.ImportsBuffer.ToArray() };
    //-------------------------------------------------------------------------------------------------
    /** Reborn: bounded auxiliary parsing retains order and requires one sentinel without duplicate slots. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] bytes) { if (bytes.Length == 0) return Array.Empty<uint>(); if (bytes.Length % 4 != 0 || Word(bytes, bytes.Length - 4) != uint.MaxValue) throw new InvalidDataException("Shared Draws sentinel differs."); var result = Enumerable.Range(0, bytes.Length / 4 - 1).Select(i => Word(bytes, i * 4)).ToArray(); if (result.Distinct().Count() != result.Length) throw new InvalidDataException("Shared Draws duplicate slot."); return result; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: nonempty auxiliary output has an explicit final sentinel. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> slots) { var values = slots.ToArray(); if (values.Length == 0) return Array.Empty<byte>(); var bytes = new byte[(values.Length + 1) * 4]; for (int i = 0; i < values.Length; i++) Put(bytes, i * 4, values[i]); Put(bytes, values.Length * 4, uint.MaxValue); return bytes; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: all container/native word reads stay within owned arrays. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] bytes, int at) { if (at < 0 || at > bytes.Length - 4) throw new InvalidDataException("Shared Draws word out of bounds."); return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at, 4)); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: only newly owned output or detached-control buffers are patched. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] bytes, int at, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(at, 4), value);
}
