using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: derive the self-replacement native and SpecialAbility metadata chain from original Uprising construction yards.
internal static class ReplaceSelfContract
{
    internal const string Wire="ReplaceSelfSpecialAbilityModuleData";
    internal const uint TypeId=0x91222181u;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently qualify the complete self-replacement record and inherited ability settings on Soviet and Allied retail construction yards. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<ReplaceSelfSpecialAbilityModuleData>()!=272)throw new InvalidDataException("EP1 self-replacement header must be 272 bytes.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("SovietConstructionYard","Soviet/Structures/SovietConstructionYard.xml"),("AlliedConstructionYard","Allied/Structures/AlliedConstructionYard.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1);var module=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==TypeId);
            if(SdkBuildTests.Enabled){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(GameObjectExtendedRootContract.Original(witness.Item2,"ReplaceSelfSpecialAbility").Single(),true));
                // Reborn: the Allied original uses a local literal Define; normal builds already evaluate it before this isolated module proof.
                var isolated=behaviors.ChildNodes.OfType<XmlElement>().Single();string unpack=isolated.GetAttribute("NewObjectUnpackTime");
                if(unpack.StartsWith("=$",StringComparison.Ordinal))isolated.SetAttribute("NewObjectUnpackTime",GameObjectExtendedRootContract.Original(witness.Item2,"Define").Single(e=>e.GetAttribute("name")==unpack[2..]).GetAttribute("value"));
                var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("ReplaceSelfSpecialAbility",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                SchemaNativeContract.VerifyStock(resolved,typeof(ReplaceSelfSpecialAbilityModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
            }
            grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(ReplaceSelfSpecialAbilityModuleData));count++;
        }
        SdkBuildTests.MetadataProgress("ReplaceSelfSpecialAbility",$"Reborn: ReplaceSelfSpecialAbility EP1 native/metadata qualification: PASS ({count} original retail records; header={Marshal.SizeOf<ReplaceSelfSpecialAbilityModuleData>()}).",count+" stock records");return grammar;
    }
}
