using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using Chunk = Relo.Chunk;
using Tracker = Relo.Tracker;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: join bounded module tokens to stock native lists/class IDs; this is not complete module payload/tokenization or publication support.
internal static class GameObjectModuleReview
{
    internal sealed record Module(int Wrapper, uint TypeId, string Type, bool NativeModelPresent, int Native, string[] ClassChain);
    internal sealed record Container(string Name, bool Inline, int CountAt, int Items, Module[] Modules);
    internal sealed record Proof(string Asset, Container[] Containers)
    {
        // Reborn: class names come from the explicitly reviewed in-memory schema candidate, not an unmodified full official-schema compilation claim.
        public string SchemaNameCatalogProfile => SdkShieldSchemaCandidate.Profile + "/" + SdkSchemaHookReview.Policy;
        // Reborn: known class/list topology cannot stand in for full field, dependency, relocation or gameplay validation.
        public bool SelectedModuleTopologyVerified => true;
        public bool CompleteModulePayloadVerified => false;
        public bool GameObjectPublicationEnabled => false;
        public bool GameExecuted => false;
    }

    // Reborn: immutable schema-name lookup can be shared inside one explicit synchronous build, never between Studio worker builds.
    [ThreadStatic] private static Dictionary<uint,string[]>? scopedNames;
    [ThreadStatic] private static int nameCatalogScopes;
    //-------------------------------------------------------------------------------------------------
    /** Reborn: reuse only class-hash names during a build; every stock slice, native field and generated metadata proof is still read and checked. */
    //-------------------------------------------------------------------------------------------------
    internal static IDisposable ReuseNameCatalog()=>new NameCatalogScope();
    private sealed class NameCatalogScope:IDisposable
    {
        private bool disposed;
        // Reborn: nested qualification shares the current thread's immutable name catalog.
        internal NameCatalogScope(){if(nameCatalogScopes++==0)scopedNames=null;}
        // Reborn: the final scope releases lookup authority so a later build must admit its current schemas again.
        public void Dispose(){if(disposed)return;disposed=true;if(--nameCatalogScopes==0)scopedNames=null;}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: capture only the previously fingerprint-admitted slice, then inspect six named module containers. */
    //-------------------------------------------------------------------------------------------------
    internal static Proof Inspect(string path, string name,bool allowNullArmorReferences=false)
    {
        var c = GameObjectStockReview.Capture(path, name,allowNullArmorReferences);
        return Decode(name, c.Bytes, c.Projection.NativeRoot, SchemaNames());
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify count markers, native item pointers, concrete IDs and bounded base-class table chains without interpreting module field values. */
    //-------------------------------------------------------------------------------------------------
    private static Proof Decode(string name, byte[] bytes, int root, Dictionary<uint, string[]> types)
    {
        var tokens = GameObjectStockReview.Tokens(bytes, 0);
        var nativeNames = typeof(SageBinaryData.GameObject).Assembly.GetTypes().Select(t => t.Name).ToHashSet(StringComparer.Ordinal);
        var containers = new List<Container>();
        foreach (string fieldName in new[] { "Draws", "Behaviors", "AI", "Body", "ClientUpdates", "ClientBehaviors" })
        {
            int slot = root + Marshal.OffsetOf<SageBinaryData.GameObject>(fieldName).ToInt32();
            int table = tokens[FastHash.GetHashCode(fieldName)].Offset;
            bool inline = fieldName == "AI";
            // Reborn: the observed absent Allied ClientUpdates root token has target zero, not a token table at the start of the asset.
            if (table == 0 && fieldName != "Body")
            {
                if (Word(slot) != 0 || (inline && Word(slot + 4) != 0)) throw new InvalidDataException("Null module token/native storage differs.");
                containers.Add(new(fieldName, inline, inline ? slot : -1, 0, Array.Empty<Module>()));
                continue;
            }
            Prefix(table, 12);
            var modules = new List<Module>();
            int countAt = -1, items = 0;
            if (fieldName == "Body") modules.Add(ReadModule(table, Pointer(slot, 8)));
            else
            {
                if (Word(table) != 0x6C827073 || Word(table + 4) != 0x5D448FD8) throw new InvalidDataException("Module list token header differs: " + fieldName);
                countAt = inline ? slot : Pointer(slot, 8);
                if (Word(table + 8) != countAt) throw new InvalidDataException("Module count token does not bind its native list.");
                uint count = Word(countAt);
                if (count > (inline ? 2 : 128)) throw new InvalidDataException("Module list count exceeds admitted bounds.");
                Prefix(table + 12, checked((int)count * 12));
                items = count == 0 ? 0 : Pointer(countAt + 4, checked((int)count * 4));
                if (count == 0 && Word(countAt + 4) != 0) throw new InvalidDataException("Empty module list has unreviewed item pointer.");
                var targets = new HashSet<int>();
                for (int i = 0; i < count; i++)
                {
                    int marker = table + 12 + i * 12;
                    if (Word(marker) != 0xB151A0EB || Word(marker + 4) != 0) throw new InvalidDataException("Module list marker differs.");
                    int wrapper = checked((int)Word(marker + 8));
                    int native = Pointer(items + i * 4, 8);
                    if (!targets.Add(native)) throw new InvalidDataException("Aliased module list item.");
                    modules.Add(ReadModule(wrapper, native));
                }
            }
            containers.Add(new(fieldName, inline, countAt, items, modules.ToArray()));
        }
        return new(name, containers.ToArray());

        //-------------------------------------------------------------------------------------------------
        /** Reborn: count-two concrete wrappers and count-one inherited rows identify classes, not ordinary named field records. */
        //-------------------------------------------------------------------------------------------------
        Module ReadModule(int wrapper, int native)
        {
            Prefix(wrapper, 12);
            uint tag = Word(wrapper + 4);
            if (Word(wrapper) != 2 || Word(native) != tag) throw new InvalidDataException("Concrete module token/native class ID differs.");
            string concrete = TypeName(tag);
            var chain = new List<string> { concrete }; var seen = new HashSet<int>();
            int table = checked((int)Word(wrapper + 8));
            while (true)
            {
                Prefix(table, 12);
                if (!seen.Add(table) || seen.Count > 16) throw new InvalidDataException("Module class chain cycle/depth exceeded.");
                var fields = GameObjectStockReview.Tokens(bytes, table);
                if (table + checked((fields.Count + 1) * 12) > root) throw new InvalidDataException("Class table crosses token/native boundary.");
                if (Word(table) != 1) break;
                chain.Add(TypeName(Word(table + 4)));
                table = checked((int)Word(table + 8));
            }
            if (chain[^1] != "ModuleData") throw new InvalidDataException("Module chain does not terminate at ModuleData.");
            var terminal = GameObjectStockReview.Tokens(bytes, table);
            if (!terminal.TryGetValue(FastHash.GetHashCode("id"), out var id) || id.Offset != native + 4)
                throw new InvalidDataException("Module base id does not anchor the native class payload.");
            return new(wrapper, tag, concrete, nativeNames.Contains(concrete), native, chain.ToArray());
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: hash-to-name identification rejects ambiguous or absent native type candidates. */
        //-------------------------------------------------------------------------------------------------
        string TypeName(uint hash) => types.TryGetValue(hash, out var candidates) && candidates.Length == 1 ? candidates[0] : throw new InvalidDataException("Unknown/ambiguous module class: " + hash.ToString("X8"));
        //-------------------------------------------------------------------------------------------------
        /** Reborn: all table and native word reads retain explicit range boundaries. */
        //-------------------------------------------------------------------------------------------------
        uint Word(int at) { if (at < 0 || at > bytes.Length - 4) throw new InvalidDataException("Module word out of bounds."); return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at, 4)); }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: prefix-only tables cannot point to native data as another class table. */
        //-------------------------------------------------------------------------------------------------
        void Prefix(int at, int size) { if (at < 0 || at % 4 != 0 || size < 0 || at > root - size) throw new InvalidDataException("Module token prefix range exceeded."); }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: non-null native pointers must remain inside this asset's captured native region. */
        //-------------------------------------------------------------------------------------------------
        int Pointer(int at, int size)
        {
            uint p = Word(at);

            if (p < root || p > int.MaxValue || size < 0 || p > bytes.Length - size)
                throw new InvalidDataException($"Module native pointer out of bounds: at={at}, pointer=0x{p:X8}, root={root}, bufferLength={bytes.Length}, requiredSize={size}, asset={name}");

            return (int)p;
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: join two independent stock module graphs and exercise real 0/1/2 AI marshaler output plus excess/unknown-class refusals. */
    //-------------------------------------------------------------------------------------------------
    internal static void SelfTest(string manifest)
    {
        var types = SchemaNames();
        foreach (string name in new[] { "GameObject:SovietAntiInfantryInfantry", "GameObject:AlliedConstructionYard" })
        {
            var captured = GameObjectStockReview.Capture(manifest, name);
            var proof = Decode(name, captured.Bytes, captured.Projection.NativeRoot, types);
            Console.WriteLine(JsonSerializer.Serialize(proof, new JsonSerializerOptions { WriteIndented = true }));
            var ai = proof.Containers.Single(c => c.Name == "AI");
            if (!ai.Inline || ai.CountAt != captured.Projection.NativeRoot + 480 || ai.Modules.Length != 1) throw new InvalidDataException("Reviewed stock AI topology drift.");
            int table = GameObjectStockReview.Tokens(captured.Bytes, 0)[FastHash.GetHashCode("AI")].Offset;
            int fields = checked((int)BinaryPrimitives.ReadUInt32LittleEndian(captured.Bytes.AsSpan(ai.Modules[0].Wrapper + 8, 4)));
            // Reborn: detached corruptions cover count/native joins, markers, class IDs, inherited cycles and native item-pointer bounds.
            foreach (var fault in new[] { (table + 8, (uint)(ai.CountAt + 4)), (table + 12, 0u),
                (ai.Modules[0].Native, 0xDEADBEEFu), (fields + 8, (uint)fields), (ai.CountAt + 4, 1u), (ai.CountAt, 3u) })
            {
                var broken = captured.Bytes.ToArray(); BinaryPrimitives.WriteUInt32LittleEndian(broken.AsSpan(fault.Item1, 4), fault.Item2);
                try { Decode(name, broken, captured.Projection.NativeRoot, types); }
                catch (InvalidDataException) { continue; }
                throw new InvalidDataException("Module topology corruption accepted.");
            }
            foreach (var empty in proof.Containers.Where(c => c.Modules.Length == 0))
            {
                var broken = captured.Bytes.ToArray(); int slot = captured.Projection.NativeRoot + Marshal.OffsetOf<SageBinaryData.GameObject>(empty.Name).ToInt32();
                BinaryPrimitives.WriteUInt32LittleEndian(broken.AsSpan(slot, 4), 1);
                try { Decode(name, broken, captured.Projection.NativeRoot, types); }
                catch (InvalidDataException) { continue; }
                throw new InvalidDataException("Null container/native drift accepted.");
            }
        }
        NativeAiControls();
        Console.WriteLine("GameObject modules: PASS (six containers/two stock objects, inline AI native 0/1/2 and refusal controls; no full payload/package/game).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: identify class hashes through the existing fingerprint-pinned diagnostic schema candidate; keep missing native implementations visible. */
    //-------------------------------------------------------------------------------------------------
    private static Dictionary<uint, string[]> SchemaNames()
    {
        // Reborn: names are read-only consumers of the admitted schema, not cached asset bytes or generated output.
        if(nameCatalogScopes>0&&scopedNames!=null)return scopedNames;
        Dictionary<uint, string[]>? names = null;
        var report = SdkEffectiveSchema.Inspect(shieldCandidate: true, reviewHooks: true, onAdmitted: set =>
            names = set.GlobalTypes.Values.Cast<System.Xml.Schema.XmlSchemaType>().Where(t => t.QualifiedName.Namespace == "uri:ea.com:eala:asset")
                .Select(t => t.Name!).GroupBy(FastHash.GetHashCode).ToDictionary(g => g.Key, g => g.Distinct(StringComparer.Ordinal).ToArray()));
        if (!report.SchemaAdmitted || names == null) throw new InvalidDataException("Reviewed module schema name catalog required.");
        if(nameCatalogScopes>0)scopedNames=names;
        return names;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: diagnostic normalized TypeId input tests the actual root marshaler, not schema/source normalization or complete stock AI payload equality. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void NativeAiControls()
    {
        // Reborn: pin official representation/cardinality without claiming full schema-source validation for normalized diagnostic input.
        var schema = new XmlDocument { XmlResolver = null };
        using (var input = new MemoryStream(SdkEnvironmentPreflight.Read(Path.Combine(SdkEnvironmentPreflight.BaselineSchemaRoot(), "AssetTypeGameObject.xsd"), 2 * 1048576), false))
        using (var reader = XmlReader.Create(input, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null })) schema.Load(reader);
        var schemaNs = new XmlNamespaceManager(schema.NameTable); schemaNs.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
        var declaration = schema.SelectSingleNode("/xs:schema/xs:complexType[@name='GameObject']/xs:complexContent/xs:extension/xs:sequence/xs:element[@name='AI']", schemaNs) as XmlElement;
        var choice = schema.SelectSingleNode("/xs:schema/xs:complexType[@name='AIModule']/xs:choice", schemaNs) as XmlElement;
        if (declaration?.GetAttribute("type") != "AIModule" || declaration.GetAttribute("byValue", "uri:ea.com:eala:asset:schema") != "true"
            || choice?.GetAttribute("minOccurs") != "0" || choice.GetAttribute("maxOccurs") != "2") throw new InvalidDataException("Official AI schema representation/cardinality drift.");
        foreach (int count in new[] { 0, 1, 2, 3, -1 })
        {
            var xml = new XmlDocument { XmlResolver = null };
            string body = string.Concat(Enumerable.Range(0, Math.Abs(count)).Select(i => "<AIGateUpdate TypeId='" + (count < 0 ? 0u : 0x347CED1Cu) + "' id='Gate" + i + "' TriggerWidthX='12'/>"));
            xml.LoadXml("<GameObject xmlns='uri:ea.com:eala:asset'><AI>" + body + "</AI></GameObject>");
            var ns = new XmlNamespaceManager(xml.NameTable); ns.AddNamespace("ea", "uri:ea.com:eala:asset");
            SageBinaryData.GameObject* root;
            using var tracker = new Tracker((void**)&root, (uint)sizeof(SageBinaryData.GameObject), false);
            try { Marshaler.Marshal(new Node(xml.DocumentElement!.CreateNavigator()!, ns), root, tracker); }
            catch (BinaryAssetBuilderException) when (count > 2 || count < 0) { continue; }
            if (count > 2 || count < 0) throw new InvalidDataException("Unsupported AI input silently compiled.");
            var chunk = new Chunk(); if (!tracker.MakeRelocatable(chunk)) throw new InvalidDataException("AI diagnostic allocation failed.");
            // Reborn: retain actual native chunk dimensions separately from linked-stream headers/sentinels.
            Console.WriteLine($"AI diagnostic count={count}: BIN/RELO/IMP={chunk.InstanceBuffer.Length}/{chunk.RelocationBuffer.Length}/{chunk.ImportsBuffer.Length}");
            //-------------------------------------------------------------------------------------------------
            /** Reborn: read only the owned native chunk's diagnostic words. */
            //-------------------------------------------------------------------------------------------------
            uint Word(int at) => BinaryPrimitives.ReadUInt32LittleEndian(chunk.InstanceBuffer.AsSpan(at, 4));
            if (Word(480) != count || (count == 0 ? Word(484) != 0 : Word(484) != 600) || Word(488) != 0)
                throw new InvalidDataException("Inline AI list/root offsets differ.");
            // Reborn: every item pointer needs relocation; each diagnostic module keeps its own weak ID/scalar without fabricated imports.
            if (chunk.InstanceBuffer.Length != 600 + count * (4 + sizeof(SageBinaryData.AIGateUpdateModuleData))
                || chunk.RelocationBuffer.Length != (count == 0 ? 0 : (count + 2) * 4) || chunk.ImportsBuffer.Length != 0)
                throw new InvalidDataException("AI native/auxiliary lengths differ.");
            var relocations = Enumerable.Range(0, chunk.RelocationBuffer.Length / 4).Select(i => BinaryPrimitives.ReadUInt32LittleEndian(chunk.RelocationBuffer.AsSpan(i * 4, 4))).ToArray();
            var expected = count == 0 ? Array.Empty<uint>() : new[] { 484u }.Concat(Enumerable.Range(0, count).Select(i => (uint)(600 + i * 4))).Append(uint.MaxValue).ToArray();
            if (!relocations.SequenceEqual(expected))
                throw new InvalidDataException("AI relocation/import slots differ.");
            for (int i = 0; i < count; i++)
            {
                uint native = Word(checked((int)Word(484) + i * 4));
                if (Word((int)native) != 0x347CED1Cu || Word((int)native + 4) != FastHash.GetHashCode(("Gate" + i).ToLowerInvariant())
                    || Word((int)native + 8) != unchecked((uint)BitConverter.SingleToInt32Bits(12)))
                    throw new InvalidDataException("AI item class ID differs.");
            }
        }
    }
}
