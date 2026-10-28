using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify motion/collision settings and real optional volume/shape records using bounded static and WorldBuilder asset slices.
internal static class DynamicsUpdateContract
{
    internal const string Wire="DynamicsUpdateModuleData";
    internal const uint TypeId=0x155A87BBu;
    internal static readonly (string Name,string Path,bool WorldBuilder)[] Witnesses={
        ("SovietMortarcycle","BaseObjects/BaseVehicle.xml",false),
        ("AlliedDynamicsImpact","Allied/Props/AlliedDynamicsImpact.xml",false),
        ("SS_FlingBarrel01","Civilian/Shiro_Sanitarium_SS/Destructible_Props/SS_FlingBarrel01.xml",true),
        ("SS_FlingAlliedCargoContainer01","Civilian/Shiro_Sanitarium_SS/Destructible_Props/SS_FlingAlliedCargoContainer01.xml",true)};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify absent/present volumes, three populated shape families and actual optional transforms before authored native publication. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<DynamicsUpdateModuleData>()!=36||Marshal.SizeOf<Ep1DynamicsVolumeType>()!=76||Marshal.OffsetOf<Ep1DynamicsVolumeType>(nameof(Ep1DynamicsVolumeType.ReportContacts)).ToInt32()!=72)throw new InvalidDataException("EP1 DynamicsUpdate/Volume native layout differs.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in Witnesses){string path=witness.WorldBuilder?WorldBuilderAssetResolver.CatalogPath():manifest;var stock=GameObjectStockReview.Capture(path,"GameObject:"+witness.Name,true);var module=GameObjectModuleReview.Inspect(path,stock.Asset.Name,true).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==TypeId);
            if(SdkBuildTests.Enabled){var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original(witness.Path,"DynamicsUpdate").Single(),true);ConscriptVariableBehaviorSource.DropLeafFormatting(owner);behaviors.AppendChild(owner);
                var element=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("DynamicsUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();SchemaNativeContract.VerifyStock(element,typeof(DynamicsUpdateModuleData),TypeId,stock.Bytes,module.Native,stock.Asset.References.ToArray());}
            grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,typeof(DynamicsUpdateModuleData));count++;
        }
        grammar.QualifyDynamicsScalarShapes(schemas.Schemas);
        SdkBuildTests.MetadataProgress("DynamicsUpdate",$"Reborn: DynamicsUpdate EP1 native/metadata qualification: PASS ({count} retail records, absent/present volumes, sphere/capsule/box and transform witnesses).",count+" original records");return grammar;
    }
}
