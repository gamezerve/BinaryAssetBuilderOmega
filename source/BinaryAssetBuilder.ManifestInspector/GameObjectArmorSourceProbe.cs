using System.Buffers.Binary;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using Relo;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prepare real named ArmorSet references through drift-checked EP1 field projection/Core; not full official GameObject validation or publication.
internal static class GameObjectArmorSourceProbe
{
    private const string Ea = "uri:ea.com:eala:asset";
    internal sealed record Binding(string Name, uint TypeId, uint InstanceId);
    internal sealed record Proof(string SourceSha256, string NormalizedSha256, string NativeSha256, string GameObjectId,
        int ArmorCount, int NativeBinBytes, int NativeReloBytes, int NativeImpBytes, Binding[] Dependencies)
    {
        // Reborn: successful name normalization/native subset checks grant no whole-object/package/runtime authority.
        public bool OfficialSchemaValidated => false;
        public bool FocusedOfficialFieldSchemaValidated => true;
        public bool CoreSourceNameNormalizationUsed => true;
        public bool CapturedDependencyIndicesUsed => false;
        public bool NamedTargetsResolved => true;
        public bool NativeImportBindingsVerified => true;
        public bool FullGameObjectPayloadVerified => false;
        public bool GameObjectPublicationEnabled => false;
        public bool GameExecuted => false;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: read bounded source and explicit EP1 manifests, then verify the named armor consumer without publishing files. */
    //-------------------------------------------------------------------------------------------------
    internal static Proof Inspect(string source, params string[] manifests)
    {
        byte[] bytes = SdkEnvironmentPreflight.Read(Path.GetFullPath(source), 65536);
        return Prepare(bytes, Catalog(manifests));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require validated metadata and reject duplicate lookup identities rather than assuming mod/stock first- or last-wins behavior. */
    //-------------------------------------------------------------------------------------------------
    private static ManifestAsset[] Catalog(string[] paths)
    {
        if (paths.Length < 1 || paths.Length > 2) throw new InvalidDataException("One stock and optionally one authored armor manifest are required.");
        var result = new System.Collections.Generic.List<ManifestAsset>();
        var ids = new HashSet<(uint, uint)>();
        foreach (string path in paths)
        {
            var document = ManifestReader.Read(File.ReadAllBytes(Path.GetFullPath(path)));
            TypeRegistryAudit.ValidateTarget(document.Header.Version, document.Header.AllTypesHash);
            if (document.Validate().Count != 0 || !document.Header.IsLinked) throw new InvalidDataException("Invalid GameObject reference catalog metadata.");
            foreach (var asset in document.Assets)
            {
                if (!ids.Add((asset.TypeId, asset.InstanceId))) throw new InvalidDataException("Ambiguous GameObject reference catalog identity.");
                result.Add(asset);
            }
        }
        return result.ToArray();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: admit only explicit BuildTime/ArmorSet source, apply official defaults and actual Core reference normalization, then check native binding slots. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe Proof Prepare(byte[] bytes, ManifestAsset[] catalog)
    {
        var source = Parse(bytes);
        var input = source.DocumentElement!;
        var members = input.ChildNodes.OfType<XmlElement>().ToArray();
        Settings saved = Settings.Current;
        try
        {
            string fixture = Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!, "GameObjectReferencePipeline.xsd");
            ValidateProjection(fixture);
            Settings.Current = new Settings { SchemaPath = fixture, BuildCache = false, StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            var schemas = new SchemaSet(false);
            var prepared = ReferencePipelineSmokeTest.Normalize(schemas, input.OuterXml);
            if (prepared.ReferencedInstances.Count != members.Length * 2 || prepared.WeakReferencedInstances.Count != 0)
                throw new InvalidDataException("GameObject armor projection has an unreviewed default/reference shape.");
            var bindings = prepared.ReferencedInstances.Select(reference => {
                var targets = catalog.Where(asset => asset.TypeId == reference.TypeId && asset.InstanceId == reference.InstanceId).ToArray();
                if (targets.Length != 1) throw new InvalidDataException("Missing or ambiguous named GameObject dependency: " + reference.Name);
                return new Binding(targets[0].Name, reference.TypeId, reference.InstanceId);
            }).ToArray();
            var normalizedMembers = ((XmlElement)prepared.XmlNode).ChildNodes.OfType<XmlElement>().ToArray();
            for (int index = 0; index < members.Length; index++)
            {
                var armor = new InstanceHandle("ArmorTemplate", members[index].GetAttribute("Armor").Trim());
                var damage = new InstanceHandle("DamageFX", members[index].GetAttribute("DamageFX").Trim());
                // Reborn: plain input names must select the declared schema target type, not an unrelated same-named asset or caller-supplied numeric index.
                if (bindings[index * 2].TypeId != armor.TypeId || bindings[index * 2].InstanceId != armor.InstanceId
                    || bindings[index * 2 + 1].TypeId != damage.TypeId || bindings[index * 2 + 1].InstanceId != damage.InstanceId
                    || normalizedMembers[index].GetAttribute("Armor") != armor.InstanceName + "\\" + index * 2
                    || normalizedMembers[index].GetAttribute("DamageFX") != damage.InstanceName + "\\" + (index * 2 + 1))
                    throw new InvalidDataException("Core GameObject reference normalization order/type differs.");
            }
            var namespaces = new XmlNamespaceManager(prepared.XmlNode.OwnerDocument!.NameTable); namespaces.AddNamespace("ea", Ea);
            var node = new Node(prepared.XmlNode.CreateNavigator()!, namespaces);
            SageBinaryData.GameObject* root;
            using var tracker = new Tracker((void**)&root, (uint)sizeof(SageBinaryData.GameObject), false);
            Marshaler.Marshal(node, root, tracker);
            var chunk = new Chunk(); if (!tracker.MakeRelocatable(chunk)) throw new InvalidDataException("GameObject source projection allocation failed.");
            int count = members.Length;
            if (chunk.InstanceBuffer.Length != 600 + count * 12 || Word(chunk.InstanceBuffer, 420) != count || Word(chunk.InstanceBuffer, 424) != 600
                || chunk.RelocationBuffer.Length != 8 || Word(chunk.RelocationBuffer, 0) != 424 || Word(chunk.RelocationBuffer, 4) != uint.MaxValue
                || chunk.ImportsBuffer.Length != (count * 2 + 1) * 4 || Word(chunk.ImportsBuffer, count * 8) != uint.MaxValue)
                throw new InvalidDataException("Named GameObject projection native/auxiliary shape differs.");
            float time = float.Parse(input.GetAttribute("BuildTime"), CultureInfo.InvariantCulture);
            if (Word(chunk.InstanceBuffer, 140) != unchecked((uint)BitConverter.SingleToInt32Bits(time)))
                throw new InvalidDataException("GameObject BuildTime does not reach native output.");
            for (int index = 0; index < count * 2; index++)
            {
                int slot = 600 + index / 2 * 12 + index % 2 * 4;
                uint dependency = Word(chunk.InstanceBuffer, slot);
                if (Word(chunk.ImportsBuffer, index * 4) != slot || dependency != index + 1
                    || bindings[checked((int)dependency - 1)] != bindings[index])
                    throw new InvalidDataException("Named GameObject native import does not select its Core dependency.");
            }
            return new(Hash(bytes), Hash(Encoding.UTF8.GetBytes(prepared.XmlNode.OuterXml)), Hash(chunk.InstanceBuffer),
                input.GetAttribute("id"), count, chunk.InstanceBuffer.Length, chunk.RelocationBuffer.Length, chunk.ImportsBuffer.Length, bindings);
        }
        finally { Settings.Current = saved; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: keep unsupported full-object/directive input out of this narrow diagnostic before a valid schema can mask missing semantics. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Parse(byte[] bytes)
    {
        if (bytes.Length < 1 || bytes.Length > 65536) throw new InvalidDataException("GameObject projection source exceeds 64 KiB.");
        using var stream = new MemoryStream(bytes, false);
        using var reader = XmlReader.Create(stream, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null,
            IgnoreComments = true, IgnoreWhitespace = true, MaxCharactersInDocument = 65536 });
        var xml = new XmlDocument { XmlResolver = null }; xml.Load(reader);
        var root = xml.DocumentElement!;
        if (root.Name != "GameObject" || root.NamespaceURI != Ea || root.GetAttribute("id").Length == 0
            || !float.TryParse(root.GetAttribute("BuildTime"), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out float time)
            || !float.IsFinite(time) || time < 0 || time > 10000
            || root.Attributes.OfType<XmlAttribute>().Any(a => a.Name is not ("xmlns" or "id" or "BuildTime") || a.Value.Contains('\\') || a.Value.StartsWith('='))
            || xml.ChildNodes.Cast<XmlNode>().Any(n => n is not XmlDeclaration && n is not XmlElement))
            throw new InvalidDataException("Only a plain named GameObject BuildTime/ArmorSet projection is admitted.");
        var members = root.ChildNodes.OfType<XmlElement>().ToArray();
        if (members.Length < 1 || members.Length > 32 || root.ChildNodes.Count != members.Length)
            throw new InvalidDataException("GameObject projection requires 1..32 direct ArmorSet members.");
        foreach (var member in members)
            if (member.Name != "ArmorSet" || member.NamespaceURI != Ea || member.HasChildNodes
                || member.GetAttribute("Armor").Trim().Length == 0 || member.GetAttribute("DamageFX").Trim().Length == 0
                || member.Attributes.OfType<XmlAttribute>().Any(a => a.NamespaceURI.Length != 0 || a.Name is not ("Armor" or "DamageFX" or "Conditions")
                    || a.Value.Contains('\\') || a.Value.Contains(':') || a.Value.StartsWith('=')))
                throw new InvalidDataException("ArmorSet projection requires plain Armor/DamageFX names without pre-normalized indices or directives.");
        return xml;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove stock and newly compiled armor names, deterministic indices, changed source values and atomic refusal/settings restoration in one owned batch. */
    //-------------------------------------------------------------------------------------------------
    internal static void SelfTest(string directory, string staticManifest)
    {
        directory = Path.GetFullPath(directory);
        if (Directory.Exists(directory) || File.Exists(directory)) throw new InvalidDataException("GameObject source test output occupied.");
        Directory.CreateDirectory(directory);
        var stock = Catalog(new[] { staticManifest });
        const string original = "<GameObject xmlns='uri:ea.com:eala:asset' id='RebornConsumer' BuildTime='25'><ArmorSet Armor='CryoBeamFrozenArmor' DamageFX='FactionStructureDamageFX' Conditions='SECONDARY_DAMAGE'/><ArmorSet Armor='AlliedConYardArmor' DamageFX='FactionStructureDamageFX'/></GameObject>";
        byte[] bytes = Encoding.UTF8.GetBytes(original); Settings saved = Settings.Current;
        var first = Prepare(bytes, stock); var repeat = Prepare(bytes, stock);
        if (first.NativeSha256 != repeat.NativeSha256 || !first.Dependencies.SequenceEqual(repeat.Dependencies)
            || !ReferenceEquals(saved, Settings.Current) || !Encoding.UTF8.GetBytes(original).SequenceEqual(bytes))
            throw new InvalidDataException("Named GameObject projection repeat/settings/source control differs.");
        var edited = Prepare(Encoding.UTF8.GetBytes(original.Replace("BuildTime='25'", "BuildTime='50'", StringComparison.Ordinal)), stock);
        if (edited.NativeSha256 == first.NativeSha256) throw new InvalidDataException("Changed GameObject BuildTime did not affect native bytes.");
        // Reborn: create an actual scoped Core armor stream under a new ID; no synthetic catalog handle substitutes for the authored target.
        string armor = Path.Combine(directory, "armor.xml"), output = Path.Combine(directory, "armor-build");
        File.WriteAllText(armor, "<!-- Reborn: owned custom armor target. --><ArmorTemplate xmlns='" + Ea + "' id='RebornAuthoredArmor'><Armor Damage='GUN' Percent='250.5'/></ArmorTemplate>");
        GeneralArmorAuthoredBuild.Run(armor, output);
        using var receipt = JsonDocument.Parse(File.ReadAllText(Path.Combine(output, "general-armor-build.json")));
        string authoredManifest = receipt.RootElement.GetProperty("ManifestPath").GetString()!;
        var combined = Catalog(new[] { staticManifest, authoredManifest });
        byte[] customXml = Encoding.UTF8.GetBytes(original.Replace("AlliedConYardArmor", "RebornAuthoredArmor", StringComparison.Ordinal));
        var custom = Prepare(customXml, combined);
        if (custom.Dependencies[2].Name != "ArmorTemplate:RebornAuthoredArmor" || custom.Dependencies[2].InstanceId == first.Dependencies[2].InstanceId)
            throw new InvalidDataException("Authored armor lookup did not reach the named consumer dependency.");
        var faults = new[] {
            original.Replace("AlliedConYardArmor", "MissingArmor", StringComparison.Ordinal),
            original.Replace("FactionStructureDamageFX", "MissingDamageFX", StringComparison.Ordinal),
            original.Replace("AlliedConYardArmor", "AlliedConYardArmor\\2", StringComparison.Ordinal),
            original.Replace("SECONDARY_DAMAGE", "INVALID_CONDITION", StringComparison.Ordinal),
            original.Replace("BuildTime='25'", "BuildTime='NaN'", StringComparison.Ordinal),
            original.Replace("BuildTime='25'", "BuildTime='=$TIME'", StringComparison.Ordinal),
            original.Replace("id='RebornConsumer'", "id='RebornConsumer' inheritFrom='Other'", StringComparison.Ordinal),
            original.Replace("<ArmorSet Armor='AlliedConYardArmor'", "<ArmorSet Unknown='1' Armor='AlliedConYardArmor'", StringComparison.Ordinal),
            original.Replace("Armor='AlliedConYardArmor'", "Armor='WeaponTemplate:SomeWeapon'", StringComparison.Ordinal),
            original.Replace("</GameObject>", "<Behaviors/></GameObject>", StringComparison.Ordinal),
            original.Replace("</GameObject>", "<?hidden data?></GameObject>", StringComparison.Ordinal),
            "<!DOCTYPE x [<!ENTITY x '1'>]>" + original };
        foreach (string fault in faults) Refuse(() => Prepare(Encoding.UTF8.GetBytes(fault), stock));
        Refuse(() => Prepare(customXml, stock));
        Refuse(() => Catalog(new[] { staticManifest, staticManifest }));
        if (!ReferenceEquals(saved, Settings.Current)) throw new InvalidDataException("GameObject refusal did not restore caller settings.");
        File.WriteAllText(Path.Combine(directory, "gameobject-armor-source-proof.json"), JsonSerializer.Serialize(new {
            Stock = first, AuthoredArmor = custom, AuthoredArmorManifest = authoredManifest, RepeatVerified = true,
            ChangedBuildTimeVerified = true, RefusalControls = faults.Length + 2, GameObjectPackageBuilt = false, GameExecuted = false
        }, new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine("GameObject armor source: PASS (drift-checked official field projection/Core name normalization, real authored armor catalog, native imports, repeat/edit and fourteen refusals; no full schema/GameObject package/game).");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: expected source/schema/lookup failures must not silently return a prepared partial success. */
        //-------------------------------------------------------------------------------------------------
        void Refuse(Action action)
        {
            try { action(); }
            catch (Exception error) when (error is InvalidDataException or XmlException) { if (!ReferenceEquals(saved, Settings.Current)) throw new InvalidDataException("Settings leaked on refusal."); return; }
            throw new InvalidDataException("Unsupported GameObject projection was accepted.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: refuse copied-field drift rather than treating a reduced diagnostic GameObject type as the full official schema. */
    //-------------------------------------------------------------------------------------------------
    private static void ValidateProjection(string fixture)
    {
        var original = new XmlDocument { XmlResolver = null };
        var focused = new XmlDocument { XmlResolver = null };
        // Reborn: both local schema inputs are bounded and parsed without external entities.
        foreach (var pair in new[] { (original, Path.Combine(SdkEnvironmentPreflight.BaselineSchemaRoot(), "AssetTypeGameObject.xsd")), (focused, fixture) })
        {
            using var stream = new MemoryStream(SdkEnvironmentPreflight.Read(pair.Item2, 2 * 1048576), false);
            using var reader = XmlReader.Create(stream, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null });
            pair.Item1.Load(reader);
        }
        var ns = new XmlNamespaceManager(original.NameTable); ns.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
        foreach (string xpath in new[] { "/xs:schema/xs:complexType[@name='ArmorTemplateSet']", "/xs:schema/xs:simpleType[@name='DamageFXRef']", "/xs:schema/xs:complexType[@name='GameObject']/xs:complexContent/xs:extension/xs:attribute[@name='BuildTime']" })
        {
            var a = original.SelectSingleNode(xpath, ns) as XmlElement;
            var b = focused.SelectSingleNode(xpath, ns) as XmlElement;
            if (a == null || b == null || Signature(a) != Signature(b)) throw new InvalidDataException("Official GameObject projection field drift: " + xpath);
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: compare expanded element/attribute names and all declaration values, ignoring namespace-prefix declarations and formatting only. */
        //-------------------------------------------------------------------------------------------------
        static string Signature(XmlElement element) => JsonSerializer.Serialize(new {
            element.NamespaceURI, element.LocalName,
            Attributes = element.Attributes.OfType<XmlAttribute>().Where(a => a.NamespaceURI != "http://www.w3.org/2000/xmlns/")
                .OrderBy(a => a.NamespaceURI, StringComparer.Ordinal).ThenBy(a => a.LocalName, StringComparer.Ordinal)
                .Select(a => new { a.NamespaceURI, a.LocalName, a.Value }).ToArray(),
            Children = element.ChildNodes.OfType<XmlElement>().Select(Signature).ToArray()
        });
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: identify immutable source/normalized/native snapshots separately. */
    //-------------------------------------------------------------------------------------------------
    private static string Hash(byte[] bytes) => Convert.ToHexString(SHA256.HashData(bytes));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: inspect explicit little-endian native/auxiliary words. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] bytes, int at) => BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at, 4));
}
