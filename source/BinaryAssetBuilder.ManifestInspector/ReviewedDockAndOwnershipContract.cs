using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.XmlCompiler;
using SageBinaryData;
using Chunk = Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: recorded retail record sizes and field locations supplement the source and relocation proof.
internal static class ReviewedDockAndOwnershipContract
{
    private sealed record Layout(uint Id, int Size, params (string Name, int Offset)[] Fields);

    private static readonly Dictionary<Type, Layout> Layouts = new()
    {
        [typeof(Ep1SupplyCenterDockUpdateModuleData)] = new(0xC2F1EC20u, 0x24,
            ("ValueMultiplier", 0x1c), ("DistributedDeposit", 0x20)),
        [typeof(Ep1TemporaryOwnerSpecialPowerModuleData)] = new(0xC662DEB4u, 0x1fc, ("SlaveStatus", 0x1dc)),
        [typeof(GateProxyBehaviorModuleData)] = new(0x0D6BB67Bu, 0x44, ("Base", 0)),
        [typeof(SupplyCenterProductionExitUpdateModuleData)] = new(0x05F72D93u, 0x10,
            ("UnitCreatePoint", 8), ("NaturalRallyPoint", 12)),
        [typeof(RepairDockUpdateModuleData)] = new(0x14B9EF65u, 0x20, ("FramesForFullHeal", 0x1c)),
        [typeof(RailedTransportDockUpdateModuleData)] = new(0x729319CBu, 0x24,
            ("PullInsideDurationInFrames", 0x1c), ("PushOutsideDurationInFrames", 0x20))
    };

    //-------------------------------------------------------------------------------------------------
    /** Reborn: expose the three retail-backed module types already defined by the EP1 XSD as named GameObject behaviors. */
    //-------------------------------------------------------------------------------------------------
    internal static void AddBehaviorBindings(XmlSchemaSet schemas)
    {
        const string ns = "uri:ea.com:eala:asset";
        var name = new XmlQualifiedName("BehaviorModules", ns);
        var schema = schemas.Schemas(ns).Cast<XmlSchema>().Single(item => item.SchemaTypes.Contains(name));
        var modules = (XmlSchemaComplexType)schema.SchemaTypes[name]!;
        var choice = (XmlSchemaChoice)modules.Particle!;
        foreach (string module in new[] { "RepairDockUpdate", "RailedTransportDockUpdate", "SupplyCenterProductionExitUpdate" })
        {
            if (!choice.Items.OfType<XmlSchemaElement>().Any(element => element.Name == module))
                choice.Items.Add(new XmlSchemaElement { Name = module, SchemaTypeName = new XmlQualifiedName(module + "ModuleData", ns) });
        }
        schemas.Reprocess(schema);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: select only the reviewed dock and ownership layouts. */
    //-------------------------------------------------------------------------------------------------
    internal static bool Supports(Type type) => Layouts.ContainsKey(type);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove the source values, retail field offsets, complete allocation ownership and metadata targets. */
    //-------------------------------------------------------------------------------------------------
    internal static Chunk Verify(XmlElement source, Type nativeType, Chunk native, XmlSchemaSet schemas, AssetId[] references)
    {
        var layout = Layouts[nativeType];
        string wire = Ep1BehaviorMetadataTokenizer.ReviewedWireName(source.LocalName);
        if (Marshal.SizeOf(nativeType) != layout.Size || FastHash.GetHashCode(wire) != layout.Id ||
            layout.Fields.Any(field => Marshal.OffsetOf(nativeType, field.Name).ToInt32() != field.Offset))
            throw new InvalidDataException("EP1 retail ABI differs for " + wire + ".");

        SchemaNativeContract.Verify(source, nativeType, layout.Id, native, references);
        var metadata = Ep1BehaviorMetadataTokenizer.Tokenize(native, schemas, wire, nativeType);
        if (nativeType == typeof(Ep1SupplyCenterDockUpdateModuleData))
            CheckScalarRow(metadata, native, "DistributedDeposit", 0x77e3f4f7u, 0x20);
        if (nativeType == typeof(Ep1TemporaryOwnerSpecialPowerModuleData))
            CheckScalarRow(metadata, native, "SlaveStatus", 0xa7bd811eu, 0x1dc);
        return metadata;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: the EA witness places the boolean metadata row at the original deposit byte. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckScalarRow(Chunk metadata, Chunk native, string field, uint wire, int offset)
    {
        int start = checked((int)BinaryPrimitives.ReadUInt32LittleEndian(metadata.InstanceBuffer.AsSpan(8, 4)));
        int prefix = metadata.InstanceBuffer.Length - native.InstanceBuffer.Length;
        var row = GameObjectStockReview.Tokens(metadata.InstanceBuffer, start)[FastHash.GetHashCode(field)];
        if (row.Type != wire || row.Offset != prefix + offset)
            throw new InvalidDataException("EP1 metadata target differs for " + field + ".");
    }
}
