using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify Uprising automatic deposits against independent full retail native and inherited metadata records.
internal static class AutoDepositContract
{
    internal const string Wire="AutoDepositUpdateModuleData";
    internal const uint TypeId=0x4D834921u;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify the retail oil derrick and stock exchange transaction lists without reading or changing user mod sources. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<Ep1AutoDepositUpdateModuleData>()!=48)throw new InvalidDataException("EP1 automatic-deposit header must be 48 bytes.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("OilDerrick","Neutral/Structures/OilDerrick.xml"),("NY_NYStockExchange","Civilian/New_York_NY/Buildings/NY_NYStockExchange.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1);var module=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==TypeId);
            if(SdkBuildTests.Enabled){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var original=GameObjectExtendedRootContract.Original(witness.Item2,"AutoDepositUpdate").Single();behaviors.AppendChild(source.ImportNode(original,true));
                // Reborn: use the official ACCOUNT_ORE definition rather than retaining an unresolved EA expression in this isolated witness.
                string account=GameObjectExtendedRootContract.Original("GlobalData/GlobalDefines.xml","Define").Single(e=>e.GetAttribute("name")=="ACCOUNT_ORE").GetAttribute("value");
                foreach(var row in behaviors.GetElementsByTagName("InitialCaptureBonus",SchemaSet.XmlNamespace).OfType<XmlElement>().Concat(behaviors.GetElementsByTagName("Deposit",SchemaSet.XmlNamespace).OfType<XmlElement>()))if(row.GetAttribute("Account")=="=$ACCOUNT_ORE")row.SetAttribute("Account",account);
                var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("AutoDepositUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                SchemaNativeContract.VerifyStock(resolved,typeof(Ep1AutoDepositUpdateModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
            }
            grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(Ep1AutoDepositUpdateModuleData));count++;
        }
        SdkBuildTests.MetadataProgress("AutoDepositUpdate",$"Reborn: AutoDepositUpdate EP1 native/metadata qualification: PASS ({count} original retail records).",count+" stock records");return grammar;
    }
}
