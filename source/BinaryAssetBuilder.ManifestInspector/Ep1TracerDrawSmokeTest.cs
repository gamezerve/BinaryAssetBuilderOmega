using System.Buffers.Binary;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using SageBinaryData;
using Chunk = Relo.Chunk;
using Tracker = Relo.Tracker;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: retain both complete Conscript tracer graphs and real Core reference identities; this is not acceptance of the remaining complete Conscript object.
internal static class Ep1TracerDrawSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare two complete independent stock native/metadata graphs, then exercise optional masks, all weapon slots, FX imports and detached failures together. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest, string globalManifest)
    {
        var saved = Settings.Current;
        try
        {
            if (Marshal.SizeOf<W3DTracerDrawModuleData>() != 112 || Marshal.SizeOf<ObjectStatusValidationDataType>() != 64) throw new InvalidDataException("Tracer native ABI differs.");
            XmlSchemaSet? official = null; SdkEffectiveSchema.Inspect(shieldCandidate: true, reviewHooks: true, onAdmitted: set => official = set);
            if (official == null) throw new InvalidDataException("Reviewed Draw schema required.");
            Settings.Current = new Settings { SchemaPath = Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!, "TracerDrawPipeline.xsd"), BuildCache = false, StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            var schemas = new SchemaSet(false);
            foreach (string name in new[] { "DrawModuleData", "ModuleData", "ObjectStatusValidationDataType" })
            {
                var a = (XmlSchemaComplexType)official.GlobalTypes[new XmlQualifiedName(name, SchemaSet.XmlNamespace)]!;
                var b = (XmlSchemaComplexType)schemas.Schemas.GlobalTypes[a.QualifiedName]!;
                if (a.BaseXmlSchemaType is not XmlSchemaType officialBase || b.BaseXmlSchemaType is not XmlSchemaType focusedBase || officialBase.QualifiedName != focusedBase.QualifiedName || a.AttributeUses.Count != b.AttributeUses.Count) throw new InvalidDataException("Draw focused base drift.");
                foreach (XmlSchemaAttribute attribute in a.AttributeUses.Values) if (b.AttributeUses[attribute.QualifiedName] is not XmlSchemaAttribute other || attribute.AttributeSchemaType is not XmlSchemaType officialType || other.AttributeSchemaType is not XmlSchemaType focusedType || officialType.QualifiedName != focusedType.QualifiedName || attribute.DefaultValue != other.DefaultValue || attribute.Use != other.Use) throw new InvalidDataException("Draw focused attribute drift.");
            }
            var statusType = (XmlSchemaSimpleType)official.GlobalTypes[new XmlQualifiedName("ObjectStatusType", SchemaSet.XmlNamespace)]!;
            if (!((XmlSchemaSimpleTypeRestriction)statusType.Content!).Facets.OfType<XmlSchemaEnumerationFacet>().Select(f => f.Value).SequenceEqual(Enum.GetNames<ObjectStatusType>())) throw new InvalidDataException("Tracer status ordinal drift.");
            var catalog = new[] { Catalog(manifest), Catalog(globalManifest) }.SelectMany(d => d.Assets).ToArray();
            var stock = GameObjectStockReview.Capture(manifest, "GameObject:SovietAntiInfantryInfantry");
            var modules = GameObjectModuleReview.Inspect(manifest, stock.Asset.Name).Containers.Single(c => c.Name == "Draws").Modules.Where(m => m.Type == "W3DTracerDrawModuleData").ToArray();
            if (modules.Length != 2) throw new InvalidDataException("Both Conscript tracer graphs required.");
            Chunk? last = null;
            for (int index = 0; index < 2; index++)
            {
                string source = Source(index == 1);
                Validate(official, source);
                var instance = Normalize(schemas, source, catalog); var native = Compile(instance.XmlNode); var output = Check(native);
                var module = modules[index]; var expected = stock.Bytes.AsSpan(module.Native, 176).ToArray();
                Put(expected, 104, Word(expected, 104) - (uint)module.Native);
                foreach (uint slot in Slots(native.ImportsBuffer))
                {
                    uint original = Word(expected, (int)slot), current = Word(native.InstanceBuffer, (int)slot);
                    if (original == 0 || original > stock.Asset.References.Count || current == 0 || current > instance.ReferencedInstances.Count) throw new InvalidDataException("Tracer stock/import occurrence out of bounds.");
                    var a = stock.Asset.References[(int)original - 1]; var b = instance.ReferencedInstances[(int)current - 1];
                    if (a.TypeId != b.TypeId || a.InstanceId != b.InstanceId) throw new InvalidDataException("Tracer stock import identity differs."); Put(expected, (int)slot, current);
                }
                if (!expected.SequenceEqual(native.InstanceBuffer)) throw new InvalidDataException("Complete stock tracer native graph differs: " + string.Join(",", Enumerable.Range(0, 176).Where(i => expected[i] != native.InstanceBuffer[i]).Take(16).Select(i => $"+{i}:{expected[i]:X2}/{native.InstanceBuffer[i]:X2}")));
                var prefix = stock.Bytes.AsSpan(module.Wrapper, 492).ToArray();
                for (int at = 8; at < prefix.Length; at += 12)
                {
                    uint target = Word(prefix, at); if (target == 0) continue;
                    uint mapped = target >= module.Wrapper && target < module.Wrapper + 492 ? target - (uint)module.Wrapper
                        : target >= module.Native && target < module.Native + 176 ? target - (uint)module.Native + 492 : throw new InvalidDataException("Stock tracer metadata target escapes graph.");
                    Put(prefix, at, mapped);
                }
                if (!prefix.SequenceEqual(output.InstanceBuffer.Take(492))) throw new InvalidDataException("Complete independent stock tracer metadata differs.");
                last = native;
            }
            string fx = catalog.First(a => a.Name.StartsWith("FXList:", StringComparison.Ordinal)).Name;
            int authored = 0;
            foreach (string slot in Enum.GetNames<WeaponSlotType>()) foreach (bool status in new[] { false, true }) foreach (bool blend in new[] { false, true })
            {
                var xml = Xml(Source(false)); var root = xml.DocumentElement!; root.SetAttribute("WeaponSlotType", slot); root.SetAttribute("UseAdditiveBlending", blend ? "true" : "false"); root.SetAttribute("TracerHitFx", fx); root.SetAttribute("TracerEmitFx", fx);
                root.SetAttribute("TextureIndex", "-1"); root.SetAttribute("WeaponSlotID", "3"); root.SetAttribute("SweepSpeed", "0.25"); root.SetAttribute("Width", "12");
                // Reborn: edited real/frame/color assertions must distinguish successful parsing from a repeatable but ignored source edit.
                root.SetAttribute("FrameLifeTime", "123"); ((XmlElement)root.FirstChild!).SetAttribute("r", "0.25"); ((XmlElement)root.ChildNodes[1]!).SetAttribute("a", "0.5");
                if (!status) root.RemoveChild(root.LastChild!);
                Validate(official, root.OuterXml); var instance = Normalize(schemas, root.OuterXml, catalog); var native = Compile(instance.XmlNode); var output = Check(native);
                if (Word(native.InstanceBuffer, 48) != (uint)Enum.Parse<WeaponSlotType>(slot) || Word(native.InstanceBuffer, 52) != 3 || Word(native.InstanceBuffer, 68) != uint.MaxValue || native.InstanceBuffer[108] != (blend ? 1 : 0) || Word(native.InstanceBuffer, 104) != (status ? 112u : 0u) || Slots(native.ImportsBuffer).Length != 3 || instance.ReferencedInstances.Count != 3) throw new InvalidDataException("Authored tracer flags/imports/edits differ.");
                if (Word(native.InstanceBuffer, 16) != 0x41400000 || Word(native.InstanceBuffer, 28) != 0x3E800000 || Word(native.InstanceBuffer, 44) != 0x42F60000 || Word(native.InstanceBuffer, 72) != 0x3E800000 || Word(native.InstanceBuffer, 100) != 0x3F000000) throw new InvalidDataException("Authored tracer real/frame/color edits ignored.");
                var repeat = Check(Compile(Normalize(schemas, root.OuterXml, catalog).XmlNode));
                if (!repeat.InstanceBuffer.SequenceEqual(output.InstanceBuffer) || !repeat.RelocationBuffer.SequenceEqual(output.RelocationBuffer) || !repeat.ImportsBuffer.SequenceEqual(output.ImportsBuffer)) throw new InvalidDataException("Tracer repeat differs."); authored++;
            }
            int nativeFaults = 0, detachedFaults = 0;
            foreach (var mutation in new[] { (0, 0u), (4, 0u), (48, 6u), (104, 0u), (104, 116u), (108, 2u), (108, 0x100u), (140, 64u), (172, 64u) }.Concat(Enumerable.Range(0, 10).Select(i => (8 + i * 4, 0x7FC00000u))).Concat(Enumerable.Range(0, 8).Select(i => (72 + i * 4, 0x7F800000u))))
            { var bad = Clone(last!); Put(bad.InstanceBuffer, mutation.Item1, mutation.Item2); Refuse(() => Ep1TracerDrawTokenizer.Tokenize(bad)); nativeFaults++; }
            foreach (var change in new[] { 0, 1, 2 })
            { var bad = Clone(last!); if (change == 0) bad.RelocationBuffer = Array.Empty<byte>(); else if (change == 1) bad.ImportsBuffer = Array.Empty<byte>(); else bad.RelocationBuffer = Pack(104, 104); Refuse(() => Ep1TracerDrawTokenizer.Tokenize(bad)); nativeFaults++; }
            var good = Check(last!);
            for (int at = 0; at < 492; at += 4) { var bad = Clone(good); Put(bad.InstanceBuffer, at, Word(bad.InstanceBuffer, at) ^ 1u); Refuse(() => Verify(bad, last!)); detachedFaults++; }
            foreach (int at in new[] { 492 + 8, 492 + 56, 492 + 104, 492 + 112 }) { var bad = Clone(good); Put(bad.InstanceBuffer, at, Word(bad.InstanceBuffer, at) ^ 1u); Refuse(() => Verify(bad, last!)); detachedFaults++; }
            var badAux = Clone(good); badAux.ImportsBuffer = Array.Empty<byte>(); Refuse(() => Verify(badAux, last!)); detachedFaults++;
            Refuse(() => Normalize(schemas, Source(false).Replace("FXTracer", "MissingRebornTexture", StringComparison.Ordinal), catalog));
            Refuse(() => Normalize(schemas, Source(false), catalog.Concat(catalog.Where(a => a.Name == "Texture:FXTracer")).ToArray()));
            Refuse(() => Validate(official, Source(false).Replace("PRIMARY_WEAPON", "UNKNOWN_WEAPON", StringComparison.Ordinal)));
            Refuse(() => Compile(Xml("<TracerModelDraw xmlns='uri:ea.com:eala:asset' id='Unknown'/>").DocumentElement!, unknown: true));
            Console.WriteLine($"EP1 Conscript tracers: PASS (two complete 176-byte native/492-byte stock metadata graphs, real Core texture identities; {authored} full authored frame/color/slot/FX/status graphs; {nativeFaults} native/{detachedFaults} detached and 4 dependency/schema/unknown-dispatch refusals; scripted/parachute draws and complete object publication remain open).");
        }
        finally { Settings.Current = saved; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: these two literal leaves reproduce both official Conscript tracers without replacing the full Conscript acceptance target with a reduced unit. */
    //-------------------------------------------------------------------------------------------------
    private static string Source(bool heroic) => "<TracerModelDraw xmlns='uri:ea.com:eala:asset' id='ModuleTag_TracerModelDraw" + (heroic ? "Veterancy" : "") + "' MinLength='" + (heroic ? "50" : "25") + "' MaxLength='50' Width='8' MinSpeed='" + (heroic ? "30" : "25") + "' MaxSpeed='" + (heroic ? "30" : "35") + "' SweepSpeed='" + (heroic ? "1" : "2") + "' SpreadAngle='" + (heroic ? "2" : "5") + "' MinTracersPerFrame='" + (heroic ? "0.5" : "0.1") + "' MaxTracersPerFrame='" + (heroic ? "0.5" : "0.1") + "' FrameLifeTime='" + (heroic ? "35" : "25") + "' WeaponSlotType='PRIMARY_WEAPON' Texture='FXTracer" + (heroic ? "Heroic" : "") + "' UseAdditiveBlending='true'><HeadColor r='1' g='" + (heroic ? "0" : "1") + "' b='" + (heroic ? "0" : "1") + "' a='1'/><TailColor r='" + (heroic ? "0" : "1") + "' g='" + (heroic ? "0" : "1") + "' b='" + (heroic ? "0" : "1") + "' a='" + (heroic ? "0" : "1") + "'/><ObjectStatusValidation ForbiddenStatus='" + (heroic ? "GENERIC_TOGGLE_STATE" : "WEAPON_UPGRADED_01 GENERIC_TOGGLE_STATE") + "'" + (heroic ? " RequiredStatus='WEAPON_UPGRADED_01'" : "") + "/></TracerModelDraw>";
    //-------------------------------------------------------------------------------------------------
    /** Reborn: official reviewed Draw validation happens before internal class-ID injection. */
    //-------------------------------------------------------------------------------------------------
    private static void Validate(XmlSchemaSet schemas, string source) => GameObjectBodyProjectionSmokeTest.Validate(schemas, "<GameObject xmlns='uri:ea.com:eala:asset' id='TracerProbe'><Draws>" + source + "</Draws><Body><ActiveBody id='Body' MaxHealth='100'/></Body></GameObject>");
    //-------------------------------------------------------------------------------------------------
    /** Reborn: plain source names pass actual Core once and must resolve to unique real typed stock identities. */
    //-------------------------------------------------------------------------------------------------
    private static InstanceDeclaration Normalize(SchemaSet schemas, string source, ManifestAsset[] catalog) { if (source.Contains('\\')) throw new InvalidDataException("Authored tracer reference indices refused."); var instance = ReferencePipelineSmokeTest.Normalize(schemas, source); foreach (var r in instance.ReferencedInstances) if (catalog.Count(a => a.TypeId == r.TypeId && a.InstanceId == r.InstanceId) != 1) throw new InvalidDataException("Tracer dependency missing/ambiguous."); return instance; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: dispatch the real Draw container then isolate only the owned tracer pointer and import slots, retaining exact parent-container auxiliary checks. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe Chunk Compile(XmlNode source, bool unknown = false)
    {
        var xml = Xml("<GameObject xmlns='uri:ea.com:eala:asset'><Draws/></GameObject>"); var leaf = (XmlElement)xml.ImportNode(source, true); xml.DocumentElement!.FirstChild!.AppendChild(leaf); leaf.SetAttribute("TypeId", (unknown ? 0xDEADBEEFu : 0x54018E60u).ToString(CultureInfo.InvariantCulture));
        var ns = new XmlNamespaceManager(xml.NameTable); ns.AddNamespace("ea", SchemaSet.XmlNamespace); GameObject* root;
        using var tracker = new Tracker((void**)&root, (uint)sizeof(GameObject), false); Marshaler.Marshal(new Node(xml.DocumentElement.CreateNavigator()!, ns), root, tracker); var chunk = new Chunk(); if (!tracker.MakeRelocatable(chunk)) throw new InvalidDataException("Tracer dispatch allocation failed.");
        var b = chunk.InstanceBuffer; int list = checked((int)Word(b, 472)), items = checked((int)Word(b, list + 4)), native = checked((int)Word(b, items));
        if (Word(b, list) != 1 || Word(b, native) != 0x54018E60) throw new InvalidDataException("Tracer root dispatch differs."); bool status = Word(b, native + 104) != 0;
        var expectedRelo = new List<uint> { 472, (uint)(list + 4), (uint)items }; if (status) expectedRelo.Add((uint)(native + 104));
        uint[] imports = new[] { 56u, 60u, 64u }.Where(s => Word(b, native + (int)s) != 0).ToArray();
        if (!Slots(chunk.RelocationBuffer).SequenceEqual(expectedRelo) || !Slots(chunk.ImportsBuffer).SequenceEqual(imports.Select(s => s + (uint)native))) throw new InvalidDataException("Tracer dispatch auxiliary differs.");
        var result = b.AsSpan(native).ToArray(); if (status) Put(result, 104, Word(result, 104) - (uint)native);
        return new Chunk { InstanceBuffer = result, RelocationBuffer = status ? Pack(104) : Array.Empty<byte>(), ImportsBuffer = Pack(imports) };
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independent stock literal tags, not the producer's hashing calls, pin the complete metadata prefix and reverse native/auxiliary shifts. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(Chunk output, Chunk native)
    {
        bool status = Word(native.InstanceBuffer, 104) != 0; int prefix = status ? 492 : 456; var expected = new byte[prefix];
        Row(0, 2, 0x54018E60, 12); Row(12, 1, 0x17F61728, 276);
        uint[] reals = { 0xE462728C, 0x3DF4521F, 0xA6FA299A, 0x302FD4EA, 0x3D662E5C, 0xF5C32FDD, 0xA4D5693B, 0x8D175CB0, 0x30336FBD, 0xBC7FBB54 };
        for (int i = 0; i < reals.Length; i++) Row(24 + i * 12, reals[i], 0x645864F3, (uint)(prefix + 8 + i * 4));
        foreach (var f in new[] { (144, 0x1848C549u, 0x1848C549u, 48), (156, 0x5E259CB2u, 0x23E650B8u, 52), (168, 0x21E727DAu, 0xBDB63F16u, 56), (180, 0x0AA49D81u, 0x77E3F4F7u, 108), (192, 0x20D267B8u, 0x0D1C8E83u, 60), (204, 0xE3A88351u, 0x0D1C8E83u, 64), (216, 0xF8D8816Bu, 0x23E650B8u, 68) }) Row(f.Item1, f.Item2, f.Item3, (uint)(prefix + f.Item4));
        Row(228, 0xB4CCC49B, 0x16ADF350, 336); Row(240, 0xADA8B561, 0x16ADF350, 396); Row(252, 0xEA73EC5F, 0x3E8F938C, status ? 456u : 0u); Row(276, 1, 0xD75D93A2, 300); Row(300, 0x19B042A0, 0x23E650B8, (uint)prefix); Row(312, 0x8CDC9898, 0xBDF07965, (uint)(prefix + 4));
        foreach (var color in new[] { (336, 72), (396, 88) }) { uint[] hashes = { 0x21DEADB2, 0x620CC514, 0x51AE1A99, 0x4866210B }; for (int i = 0; i < 4; i++) Row(color.Item1 + i * 12, hashes[i], 0x645864F3, (uint)(prefix + color.Item2 + i * 4)); }
        if (status) { Row(456, 0x312281FD, 0xA7BD811E, (uint)(prefix + 112)); Row(468, 0x239EB201, 0xA7BD811E, (uint)(prefix + 144)); }
        if (output.InstanceBuffer.Length != prefix + native.InstanceBuffer.Length || !expected.SequenceEqual(output.InstanceBuffer.Take(prefix)) || !output.RelocationBuffer.SequenceEqual(Pack(Slots(native.RelocationBuffer).Select(s => s + (uint)prefix).ToArray())) || !output.ImportsBuffer.SequenceEqual(Pack(Slots(native.ImportsBuffer).Select(s => s + (uint)prefix).ToArray()))) throw new InvalidDataException("Detached tracer metadata/auxiliary differs.");
        var inverse = output.InstanceBuffer.AsSpan(prefix).ToArray(); if (status) { if (Word(inverse, 104) != prefix + 112) throw new InvalidDataException("Detached tracer status pointer differs."); Put(inverse, 104, 112); } if (!inverse.SequenceEqual(native.InstanceBuffer)) throw new InvalidDataException("Detached tracer native graph differs.");
        //-------------------------------------------------------------------------------------------------
        /** Reborn: expected metadata remains separate from every generated fragment. */
        //-------------------------------------------------------------------------------------------------
        void Row(int at, uint name, uint type, uint target) { Put(expected, at, name); Put(expected, at + 4, type); Put(expected, at + 8, target); }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: generation is followed by an independent detached verifier. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Check(Chunk native) { var output = Ep1TracerDrawTokenizer.Tokenize(native); Verify(output, native); return output; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: linked catalog validation precedes typed lookup. */
    //-------------------------------------------------------------------------------------------------
    private static ManifestDocument Catalog(string path) { var d = ManifestReader.Read(SdkEnvironmentPreflight.Read(path, 64 * 1048576)); TypeRegistryAudit.ValidateTarget(d.Header.Version, d.Header.AllTypesHash); if (!d.Header.IsLinked || d.Validate().Count != 0) throw new InvalidDataException("Invalid tracer catalog."); return d; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: source and detached failures never become silently omitted Draw output. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action) { try { action(); } catch (InvalidDataException) { return; } throw new InvalidDataException("Malformed tracer input/output admitted."); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: XML cannot resolve external entities. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Xml(string source) { var d = new XmlDocument { XmlResolver = null }; d.LoadXml(source); return d; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: each mutation owns all three buffers. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c) => new() { InstanceBuffer = c.InstanceBuffer.ToArray(), RelocationBuffer = c.RelocationBuffer.ToArray(), ImportsBuffer = c.ImportsBuffer.ToArray() };
    //-------------------------------------------------------------------------------------------------
    /** Reborn: empty auxiliary data has no sentinel; nonempty slot lists have exactly one. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(params uint[] slots) { if (slots.Length == 0) return Array.Empty<byte>(); var b = new byte[(slots.Length + 1) * 4]; for (int i = 0; i < slots.Length; i++) Put(b, i * 4, slots[i]); Put(b, slots.Length * 4, uint.MaxValue); return b; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: enumerate validated Tracker slots without their terminal sentinel. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b) => b.Length == 0 ? Array.Empty<uint>() : Enumerable.Range(0, b.Length / 4 - 1).Select(i => Word(b, i * 4)).ToArray();
    //-------------------------------------------------------------------------------------------------
    /** Reborn: word reads stay within owned buffers. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b, int at) { if (at < 0 || at > b.Length - 4) throw new InvalidDataException("Tracer word bounds exceeded."); return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at, 4)); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: writes affect only owned output/test arrays. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b, int at, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at, 4), value);
}
