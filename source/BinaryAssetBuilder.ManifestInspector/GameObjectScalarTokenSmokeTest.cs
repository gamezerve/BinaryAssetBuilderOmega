using System.Buffers.Binary;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.XmlCompiler;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: batch scalar token admission against independent stock metadata plus explicitly authored schema/native values; not complete object source compilation.
internal static class GameObjectScalarTokenSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: audit every descriptor in two stock roots, compile two all-field value sets and reject schema-invalid scalar inputs. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(XmlSchemaSet schemas, string manifest)
    {
        foreach (string name in new[] { "GameObject:AlliedConstructionYard", "GameObject:SovietAntiInfantryInfantry" })
        {
            var captured = GameObjectStockReview.Capture(manifest, name); var tokens = GameObjectStockReview.Tokens(captured.Bytes, 0);
            foreach (var field in Ep1GameObjectScalarTokens.Fields)
                if (!tokens.TryGetValue(FastHash.GetHashCode(field.Name), out var token) || token.Type != field.Type || token.Offset != captured.Projection.NativeRoot + field.Offset)
                    throw new InvalidDataException("Scalar descriptor differs from independent stock: " + name + "/" + field.Name);
        }
        foreach (bool alternate in new[] { false, true })
        {
            var xml = new XmlDocument { XmlResolver = null };
            xml.LoadXml("<GameObject xmlns='uri:ea.com:eala:asset' id='ScalarProbe'><Behaviors><BuildingDestructionBehavior id='ModuleTag_ScalarProbe'><DieMuxData/><Item/></BuildingDestructionBehavior></Behaviors></GameObject>");
            foreach (var field in Ep1GameObjectScalarTokens.Fields)
                xml.DocumentElement!.SetAttribute(field.Name, Text(field.Type, alternate));
            Validate(schemas, xml.OuterXml);
            var native = Compile(xml.OuterXml);
            foreach (var field in Ep1GameObjectScalarTokens.Fields)
            {
                uint actual = field.Width == 1 ? native.Chunk.InstanceBuffer[field.Offset] : field.Width == 2 ? BinaryPrimitives.ReadUInt16LittleEndian(native.Chunk.InstanceBuffer.AsSpan(field.Offset, 2)) : BinaryPrimitives.ReadUInt32LittleEndian(native.Chunk.InstanceBuffer.AsSpan(field.Offset, 4));
                uint expected = field.Type is 0x645864F3u or 0x4E16EA4Du ? Float(alternate ? 0.25f : 0.5f) : field.Type == 0x998669E8u ? Float(alternate ? 1 : 2) : field.Type == 0x77E3F4F7u ? (alternate ? 0u : 1u) : (alternate ? 11u : 7u);
                if (actual != expected) throw new InvalidDataException($"Authored scalar {field.Name} differs: {actual:X8}/{expected:X8}.");
            }
            GameObjectDestructionProjectionSmokeTest.Authored(native);
            foreach (var invalid in new[] { ("TransportSlotCount", "256"), ("RefundValue", "65536"), ("IsAutoBuilt", "sometimes"), ("BuildTime", "not-a-number"), ("UnknownRootField", "1") })
            {
                var bad = (XmlDocument)xml.CloneNode(true); bad.DocumentElement!.SetAttribute(invalid.Item1, invalid.Item2);
                // Reborn: official SageUnsignedShort is a string pattern and admits 65536 lexically; native ushort conversion must still refuse overflow.
                Refuse(() => { Validate(schemas, bad.OuterXml); Compile(bad.OuterXml); });
            }
        }
        Console.WriteLine($"GameObject scalar tokens: PASS ({Ep1GameObjectScalarTokens.Fields.Count} descriptors/two independent stock roots, two all-field authored sets, packed widths/time units and schema refusals; partial root, no defaults/source closure/publication claim).");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: literal source values are independent of marshaller output and cover each reviewed native scalar representation. */
    //-------------------------------------------------------------------------------------------------
    private static string Text(uint type, bool alternate) => type switch {
        0x645864F3u => alternate ? "0.25" : "0.5", 0x4E16EA4Du => alternate ? "0.25r" : "0.5r",
        0x998669E8u => alternate ? "1000ms" : "2s", 0x77E3F4F7u => alternate ? "false" : "true", _ => alternate ? "11" : "7" };
    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate authored root attributes in a schema-complete container; the required Body is validation context, not part of this native projection. */
    //-------------------------------------------------------------------------------------------------
    internal static void Validate(XmlSchemaSet schemas, string source)
    {
        string complete = source.Replace("</GameObject>", "<Body><ActiveBody id='ValidationBody' MaxHealth='100'/></Body></GameObject>", StringComparison.Ordinal);
        var binding = SdkEffectiveSchema.Bind(schemas, "in-memory-scalar-root.xml", Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'>" + complete + "</AssetDeclaration>"));
        if (!binding.XmlValidated) throw new InvalidDataException("Scalar root source refused: " + string.Join("; ", binding.Errors));
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal explicit scalar values through real GameObject dispatch after validation, adding the concrete module class ID internally. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Ep1AlliedModuleSmokeTest.Output Compile(string source)
    {
        var xml = new XmlDocument { XmlResolver = null }; xml.LoadXml(source);
        ((XmlElement)xml.DocumentElement!.FirstChild!.FirstChild!).SetAttribute("TypeId", "227975924");
        var ns = new XmlNamespaceManager(xml.NameTable); ns.AddNamespace("ea", "uri:ea.com:eala:asset");
        GameObject* root; using var tracker = new Tracker((void**)&root, (uint)sizeof(GameObject), false);
        Marshaler.Marshal(new Node(xml.DocumentElement.CreateNavigator()!, ns), root, tracker);
        var chunk = new Chunk(); if (!tracker.MakeRelocatable(chunk)) throw new InvalidDataException("Scalar root allocation failed.");
        // Reborn: root strings can move allocations before Behaviors; recover the actual module pointer instead of returning a fixed 612.
        int list = (int)BinaryPrimitives.ReadUInt32LittleEndian(chunk.InstanceBuffer.AsSpan(476,4));
        int items = (int)BinaryPrimitives.ReadUInt32LittleEndian(chunk.InstanceBuffer.AsSpan(list+4,4));
        return new(chunk,(int)BinaryPrimitives.ReadUInt32LittleEndian(chunk.InstanceBuffer.AsSpan(items,4)));
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: malformed scalar/schema cases must fail rather than compile as omitted/default values. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action) { try { action(); } catch (Exception e) when (e is InvalidDataException or OverflowException or FormatException) { return; } throw new InvalidDataException("Invalid scalar root input admitted."); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: expected floats come from independent source literals. */
    //-------------------------------------------------------------------------------------------------
    private static uint Float(float value) => unchecked((uint)BitConverter.SingleToInt32Bits(value));
}
