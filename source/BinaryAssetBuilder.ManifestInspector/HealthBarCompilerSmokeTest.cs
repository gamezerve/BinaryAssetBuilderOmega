using System.Buffers.Binary;
using System.Text;
using System.Text.Json;
using System.Xml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: rebuild all retail health-bar assets from independently decoded EP1 wire offsets in a private stream.
internal static class HealthBarCompilerSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify real BIN/RELO/IMP data, including repeated template imports and nested filter lists. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest, string global, string output)
    {
        output = SdkEnvironmentPreflight.Absolute(output);
        if (Directory.Exists(output) || File.Exists(output)) throw new InvalidDataException("Fresh health-bar evidence directory required.");
        Directory.CreateDirectory(output);
        var stock = ManifestReader.Read(SdkEnvironmentPreflight.Read(manifest, 64 * 1048576));
        // Reborn: retail filters also name WorldBuilder-only objects such as ExplodingBarrel_Delay.
        string worldbuilder = Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!, "../../alpha/catalogs/ra3ep1/worldbuilder.manifest");
        var catalog = stock.Assets.Concat(ManifestReader.Read(SdkEnvironmentPreflight.Read(global, 64 * 1048576)).Assets)
            .Concat(ManifestReader.Read(SdkEnvironmentPreflight.Read(Path.GetFullPath(worldbuilder), 64 * 1048576)).Assets).ToArray();
        var source = new XmlDocument { XmlResolver = null };
        var declaration = source.CreateElement("AssetDeclaration", "uri:ea.com:eala:asset"); source.AppendChild(declaration);
        var originals = stock.Assets.Where(a => a.TypeName is "HealthBarTemplate" or "HealthBarSettings").ToArray();
        if (originals.Length != 7) throw new InvalidDataException("Expected six retail health-bar templates and one settings record.");
        foreach (var asset in originals)
        {
            var bytes = Read(asset, ".bin", a => a.InstanceDataSize);
            var node = Add(declaration, asset.TypeName); node.SetAttribute("id", Name(asset));
            if (asset.TypeName == "HealthBarTemplate")
            {
                if (bytes.Length != 8) throw new InvalidDataException("Retail health-bar template size differs.");
                node.SetAttribute("Length", U(bytes, 4).ToString());
                continue;
            }
            // Reborn: these offsets come from the retail record, not Marshal.OffsetOf or the production marshaler.
            node.SetAttribute("DefaultHealthBar", Strong(U(bytes, 4)));
            int count = checked((int)U(bytes, 8)), items = checked((int)U(bytes, 12));
            if (count != 5 || items != 16 || bytes.Length != 828) throw new InvalidDataException("Retail health-bar settings framing differs.");
            for (int i = 0; i < count; i++)
            {
                int mapping = items + i * 124, filter = mapping + 4;
                var entry = Add(node, "ObjectMapping"); entry.SetAttribute("HealthBar", Strong(U(bytes, mapping)));
                var f = Add(entry, "ObjectFilter");
                if (U(bytes, filter) != 0) throw new InvalidDataException("Nonzero retail filter ID needs a source-name witness.");
                f.SetAttribute("Rule", Enum.GetName(typeof(ObjectFilterRule), U(bytes, filter + 4))!);
                Mask<ObjectFilterRelationship>(f, "Relationship", bytes, filter + 8, 1);
                f.SetAttribute("Alignment", Enum.GetName(typeof(ObjectFilterAlignment), U(bytes, filter + 12))!);
                Mask<KindOf>(f, "Include", bytes, filter + 16, 10);
                Mask<KindOf>(f, "Exclude", bytes, filter + 56, 10);
                for (int p = 0; p < 2; p++)
                {
                    uint at = U(bytes, filter + 96 + p * 4);
                    if (at != 0) Mask<ObjectStatusType>(f, p == 0 ? "StatusBitFlags" : "StatusBitFlagsExclude", bytes, checked((int)at), 5);
                }
                for (int p = 0; p < 2; p++)
                {
                    int n = checked((int)U(bytes, filter + 104 + p * 8)), at = checked((int)U(bytes, filter + 108 + p * 8));
                    for (int j = 0; j < n; j++)
                    {
                        uint id = U(bytes, at + j * 4);
                        var target = catalog.FirstOrDefault(a => a.TypeName == "GameObject" && a.InstanceId == id)
                            ?? throw new InvalidDataException($"Retail health-bar weak target has no source identity: {id:X8}");
                        Add(f, p == 0 ? "IncludeThing" : "ExcludeThing").InnerText = Name(target);
                    }
                }
            }
            // Reborn: retail reference selectors are one-based occurrence indices, including duplicated Medium imports.
            string Strong(uint selector)
            {
                if (selector == 0 || selector > asset.References.Count) throw new InvalidDataException("Retail health-bar reference selector is invalid.");
                var reference = asset.References[(int)selector - 1];
                return Name(catalog.First(a => a.TypeId == reference.TypeId && a.InstanceId == reference.InstanceId));
            }
        }
        string entryFile = Path.Combine(output, "mod.xml"); source.Save(entryFile);
        string prepared = Path.Combine(output, "prepared"), built = Path.Combine(output, "built");
        ConscriptSourceInput.Prepare(entryFile, prepared);
        if (!ConscriptMultiAssetBuild.TryRun(Path.Combine(prepared, "game.xml"), manifest, global, built))
            throw new InvalidDataException("Health-bar mixed build route was not selected.");
        string resultPath = Path.Combine(built, "stream-readback", "diagnostic.manifest");
        var result = ManifestReader.Read(SdkEnvironmentPreflight.Read(resultPath, 64 * 1048576));
        int checks = 0;
        foreach (var original in originals)
        {
            var emitted = result.Assets.Single(a => a.Name == original.Name);
            foreach (var stream in new[] { ".bin", ".relo", ".imp" })
            {
                Func<ManifestAsset, int> size = stream == ".bin" ? a => a.InstanceDataSize : stream == ".relo" ? a => a.RelocationDataSize : a => a.ImportsDataSize;
                long at = 8;
                foreach (var a in result.Assets) { if (ReferenceEquals(a, emitted)) break; at += size(a); }
                var actual = AssetStreamProbe.ReadRange(Path.ChangeExtension(resultPath, stream), null, at, size(emitted));
                if (!actual.AsSpan().SequenceEqual(Read(original, stream, size)))
                    throw new InvalidDataException("Health-bar retail rebuild differs: " + original.Name + stream);
                checks++;
            }
            if (!emitted.References.SequenceEqual(original.References)) throw new InvalidDataException("Health-bar reference occurrence table differs.");
        }
        File.WriteAllText(Path.Combine(output, "healthbar-report.json"), JsonSerializer.Serialize(new { RetailAssets = 7, ExactStreamComparisons = checks, GameExecuted = false, PublishedModRebuilt = false }));
        Console.WriteLine($"Reborn: HealthBar EP1 retail rebuild: PASS (7 assets, {checks} exact BIN/RELO/IMP comparisons).");

        // Reborn: read only the selected tiny asset segments, never the entire retail static BIN.
        byte[] Read(ManifestAsset asset, string extension, Func<ManifestAsset, int> size)
        {
            long offset = 8;
            foreach (var item in stock.Assets) { if (ReferenceEquals(item, asset)) break; offset += size(item); }
            return AssetStreamProbe.ReadRange(Path.ChangeExtension(manifest, extension), null, offset, size(asset));
        }
    }

    // Reborn: decode an explicit retail little-endian field using bounds-checked spans.
    private static uint U(byte[] data, int offset) => BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(offset, 4));
    // Reborn: recover an XML instance identity from its typed manifest display name.
    private static string Name(ManifestAsset asset) => asset.Name[(asset.Name.IndexOf(':') + 1)..];
    // Reborn: preserve the EA namespace on every independently reconstructed source element.
    private static XmlElement Add(XmlElement parent, string name)
    {
        var node = parent.OwnerDocument!.CreateElement(name, "uri:ea.com:eala:asset"); parent.AppendChild(node); return node;
    }
    // Reborn: use official enum ordinals to reconstruct masks instead of borrowing marshaler output.
    private static void Mask<T>(XmlElement node, string field, byte[] bytes, int offset, int words) where T : struct, Enum
    {
        var names = new List<string>();
        for (int bit = 0; bit < words * 32; bit++)
            if ((U(bytes, offset + bit / 32 * 4) & (1u << (bit % 32))) != 0)
                names.Add(Enum.GetName(typeof(T), bit) ?? throw new InvalidDataException("Unknown retail health-bar mask bit: " + field + "/" + bit));
        if (names.Count != 0) node.SetAttribute(field, string.Join(" ", names));
    }
}
