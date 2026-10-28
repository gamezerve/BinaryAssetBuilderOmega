using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify complete Terror Drone attachment, EP1 curve movement and sweeping laser records before authored use.
internal static class ScoutVehicleBehaviorContract
{
    internal static readonly (string Source,string Wire,Type Type)[] Families={
        ("AttachUpdate","AttachUpdateModuleData",typeof(Ep1AttachUpdateModuleData)),
        ("BezierProjectile","BezierProjectileBehaviorModuleData",typeof(Ep1BezierProjectileBehaviorModuleData)),
        ("SweepingLaserState","SweepingLaserStateModuleData",typeof(SweepingLaserStateModuleData))};
    internal static bool Supports(Type type)=>Families.Any(f=>f.Type==type);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: read original source records, resolving their single captured global disabled-mask definition in its defining context. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlElement Original(string name)
    {
        var owner=(XmlElement)GameObjectExtendedRootContract.Original("Soviet/Units/SovietScoutVehicle.xml",name).Single().CloneNode(true);
        foreach(var attribute in owner.SelectNodes(".//@*")!.Cast<XmlAttribute>().Where(a=>a.Value.StartsWith('='))){if(attribute.Value!="=$DISABLEDTYPE_ALL")throw new InvalidDataException("Unresolved original scout definition: "+attribute.Value);attribute.Value=GameObjectExtendedRootContract.Original("GlobalData/GlobalDefines.xml","Define").Single(e=>e.GetAttribute("name")=="DISABLEDTYPE_ALL").GetAttribute("value");}
        return owner;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently check every native field and reference in the three original retail graphs, then retain their metadata grammar. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<Ep1AttachUpdateModuleData>()!=368||Marshal.OffsetOf<Ep1AttachUpdateModuleData>(nameof(Ep1AttachUpdateModuleData.AttachBoneName)).ToInt32()!=348||Marshal.SizeOf<Ep1BezierProjectileBehaviorModuleData>()!=256||Marshal.SizeOf<SweepingLaserStateModuleData>()!=76)throw new InvalidDataException("Scout behavior EP1 ABI extents/offsets differ.");
        var grammar=new StockMetadataGrammar();var stock=GameObjectStockReview.Capture(manifest,"GameObject:SovietScoutVehicle");var modules=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules;
        foreach(var family in Families){uint typeId=FastHash.GetHashCode(family.Wire);var module=modules.Single(m=>m.TypeId==typeId);
            if(SdkBuildTests.Enabled){var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=(XmlElement)source.ImportNode(Original(family.Source),true);ConscriptVariableBehaviorSource.DropLeafFormatting(owner);behaviors.AppendChild(owner);var element=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName(family.Source,SchemaSet.XmlNamespace).OfType<XmlElement>().Single();SchemaNativeContract.VerifyStock(element,family.Type,typeId,stock.Bytes,module.Native,stock.Asset.References.ToArray());}
            grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,family.Type);
        }
        // Reborn: sweeping states inherit the same optional particle/offset/status graphs as plain lasers; learn populated retail variants too.
        foreach(var witness in LaserStateContract.Witnesses){var laser=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Name);foreach(var module in GameObjectModuleReview.Inspect(manifest,laser.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==LaserStateContract.TypeId))grammar.Learn(laser.Bytes,module.Wrapper,laser.Projection.NativeRoot,typeof(LaserStateModuleData));}
        SdkBuildTests.MetadataProgress("Scout behaviors","Reborn: scout behavior EP1 native/metadata qualification: PASS (three complete retail Attach/Bezier/Sweep records, real imports and inherited fields).","3 original records");return grammar;
    }
}
