using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify subobject-upgrade native and metadata layouts against original Uprising aircraft and construction yards.
internal static class SubObjectsUpgradeContract
{
    internal const string Wire="SubObjectsUpgradeModuleData";
    internal static readonly uint TypeId=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(Wire);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: normalize only disjoint field-family order on expanded in-memory XML; preserve nugget order, repeated list order, values and invalid/duplicate nodes for schema diagnostics. */
    //-------------------------------------------------------------------------------------------------
    internal static void Normalize(XmlDocument source,System.Xml.Schema.XmlSchemaSet schemas)
    {
        foreach(var module in source.GetElementsByTagName("SubObjectsUpgrade",SchemaSet.XmlNamespace).OfType<XmlElement>().ToArray()){
            Order(module,"SubObjectsUpgradeModuleData");foreach(var nugget in module.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="Nugget"&&e.NamespaceURI==SchemaSet.XmlNamespace))Order(nugget,"SubObjectsUpgradeNugget");
        }

        // Reborn: the compiled original XSD supplies field-family precedence, including inherited Upgrade fields.
        void Order(XmlElement owner,string name)
        {
            var type=(System.Xml.Schema.XmlSchemaComplexType)schemas.GlobalTypes[new XmlQualifiedName(name,SchemaSet.XmlNamespace)]!;var names=new List<string>();Collect(type.ContentTypeParticle);var ranks=names.Distinct().Select((n,i)=>(n,i)).ToDictionary(pair=>pair.n,pair=>pair.i,StringComparer.Ordinal);var children=owner.ChildNodes.OfType<XmlElement>().ToArray();foreach(var child in children.OrderBy(e=>e.NamespaceURI==SchemaSet.XmlNamespace?ranks.GetValueOrDefault(e.LocalName,int.MaxValue):int.MaxValue))owner.AppendChild(child);
            // Reborn: flatten only actual sequence groups; this module has no interchangeable choice branches.
            void Collect(System.Xml.Schema.XmlSchemaParticle particle){if(particle is System.Xml.Schema.XmlSchemaElement field)names.Add(field.QualifiedName.Name);else if(particle is System.Xml.Schema.XmlSchemaSequence sequence)foreach(var item in sequence.Items.OfType<System.Xml.Schema.XmlSchemaParticle>())Collect(item);}
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently qualify all subobject-upgrade scalars and references and inherited fields against the original base and EP1 subobject-upgrade powers. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<Ep1SubObjectsUpgradeModuleData>()!=44||Marshal.SizeOf<Ep1SubObjectsUpgradeNugget>()!=44||Marshal.OffsetOf<Ep1SubObjectsUpgradeNugget>("Upgrade").ToInt32()!=24||Marshal.OffsetOf<Ep1SubObjectsUpgradeNugget>("CustomAnimAndDuration").ToInt32()!=36)throw new InvalidDataException("EP1 SubObjectsUpgrade root/nugget ABI differs.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("AlliedBomberAircraft","Allied/Units/AlliedBomberAircraft.xml"),("AlliedConstructionYard","Allied/Structures/AlliedConstructionYard.xml"),("AlliedGunshipAircraft","EP1/Allied/Units/AlliedGunshipAircraft.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1);var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==TypeId).ToArray();
            var originals=GameObjectExtendedRootContract.Original(witness.Item2,"SubObjectsUpgrade");
            // Reborn: qualify all stock subobject-upgrade modules by local module identity, without assuming module positions.
            foreach(var original in originals){
                uint id=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(original.GetAttribute("id").ToLowerInvariant());
                var module=modules.Single(m=>System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(captured.Bytes.AsSpan(m.Native+4))==id);
                // Reborn: root nugget count and actual repeated-record metadata are anchored by retail data, not the KW model.
                int ownTable=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(captured.Bytes.AsSpan(module.Wrapper+8));var nugget=GameObjectStockReview.Tokens(captured.Bytes,ownTable)[BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("Nugget")];
                if(nugget.Type!=0x7F67C232u||System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(captured.Bytes.AsSpan(nugget.Offset+8))!=module.Native+36)throw new InvalidDataException("Stock SubObjectsUpgrade nugget list anchor differs.");
                if(SdkBuildTests.Enabled){
                    var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(original,true));
                    var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("SubObjectsUpgrade",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                    SchemaNativeContract.VerifyStock(resolved,typeof(Ep1SubObjectsUpgradeModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
                }
                grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(Ep1SubObjectsUpgradeModuleData));count++;
            }
        }
        // Reborn: an independent retail root texture field provides the strong-reference wire signature for source-authored replacement pairs.
        var shadow=GameObjectStockReview.Capture(manifest,"GameObject:SovietAntiInfantryInfantry");int shadowTable=GameObjectStockReview.Tokens(shadow.Bytes,0)[BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("ShadowInfo")].Offset;uint texture=GameObjectStockReview.Tokens(shadow.Bytes,shadowTable)[BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("Texture")].Type;grammar.QualifyReplaceTexturePair(texture,schemas.Schemas);
        SdkBuildTests.MetadataProgress("SubObjectsUpgrade",$"Reborn: SubObjectsUpgrade EP1 native/metadata qualification: PASS ({count} original retail records; header={Marshal.SizeOf<Ep1SubObjectsUpgradeModuleData>()}).",count+" stock records");return grammar;
    }
}
