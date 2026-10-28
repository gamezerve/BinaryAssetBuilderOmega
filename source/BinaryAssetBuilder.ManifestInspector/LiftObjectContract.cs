using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify original defense, IFV and Yuriko lift payloads against their complete source and metadata graphs.
internal static class LiftObjectContract
{
    internal const string Wire="LiftObjectUpdateModuleData";
    internal static readonly (string Owner,string Path)[] Witnesses={
        ("AlliedBaseDefense","Allied/Structures/AlliedBaseDefense.xml"),
        ("AlliedAntiAirVehicleTech1","Allied/Units/AlliedAntiAirVehicleTech1.xml"),
        ("JapanCommandoTech1","Japan/Units/JapanCommandoTech1.xml")};

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove native extents, original field values/imports and nested metadata grammar before composing authored lift updates. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<Ep1LiftObjectUpdateModuleData>()!=76)throw new InvalidDataException("Lift update EP1 ABI extent differs.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var family in Witnesses){
            uint typeId=FastHash.GetHashCode(Wire);var stock=GameObjectStockReview.Capture(manifest,"GameObject:"+family.Owner);
            var module=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==typeId);
            if(SdkBuildTests.Enabled){var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(GameObjectExtendedRootContract.Original(family.Path,"LiftObjectUpdate").Single(),true));var element=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("LiftObjectUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();SchemaNativeContract.VerifyStock(element,typeof(Ep1LiftObjectUpdateModuleData),typeId,stock.Bytes,module.Native,stock.Asset.References.ToArray());}
            grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,typeof(Ep1LiftObjectUpdateModuleData));count++;
        }
        // Reborn: the original Barracks receiver supplies the same named ObjectFilter record for authored optional lift-state filters.
        var receiver=GameObjectStockReview.Capture(manifest,"GameObject:AlliedBarracks");
        foreach(var module in GameObjectModuleReview.Inspect(manifest,receiver.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==TechUpgradeReceiverContract.TypeId))grammar.Learn(receiver.Bytes,module.Wrapper,receiver.Projection.NativeRoot,typeof(TechUpgradeReceiverModuleData));
        SdkBuildTests.MetadataProgress("Lift updates",$"Reborn: lift updates: PASS ({count} original lift native and metadata graphs).",count+" original records");return grammar;
    }
}
