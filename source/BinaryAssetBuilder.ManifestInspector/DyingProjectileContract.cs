using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: falling-projectile support is deliberately explicit; other real movement processors remain separate implementations.
internal static class DyingProjectileContract
{
    internal static readonly (string Source,string Wire,Type Type)[] Families={
        ("ProjectileUpdate","ProjectileUpdateModuleData",typeof(ProjectileUpdateModuleData)),
        ("ShipSlowDeath","ShipSlowDeathBehaviorModuleData",typeof(ShipSlowDeathBehaviorModuleData))};
    internal static bool Supports(Type type)=>Families.Any(f=>f.Type==type);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: report unsupported movement processors at the authored XML rather than accepting incompatible runtime state data. */
    //-------------------------------------------------------------------------------------------------
    internal static void Validate(XmlElement source)
    {
        if(source.LocalName!="ProjectileUpdate")return;
        var movement=source.ChildNodes.OfType<XmlElement>().SingleOrDefault(e=>e.LocalName=="Movement");
        var children=movement?.ChildNodes.OfType<XmlElement>().ToArray()??Array.Empty<XmlElement>();
        if(children.Length!=1||children[0].LocalName!="MoveToGround"||source.HasAttribute("StateMachine")&&source.GetAttribute("StateMachine")!="BombProjectile")
            throw SdkXmlDiagnostics.At(new InvalidDataException("ProjectileUpdate currently implements BombProjectile/Movement/MoveToGround; the requested movement processor is not implemented."),movement??source);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain complete inherited slow-death grammar and independently compare both original concrete aircraft-death records. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<ProjectileUpdateModuleData>()!=28||Marshal.SizeOf<ProjectileMoveToGroundData>()!=28||Marshal.SizeOf<ShipSlowDeathBehaviorModuleData>()!=116)throw new InvalidDataException("EP1 dying-projectile layout extent differs.");
        var grammar=SlowDeathContract.Qualify(manifest,baseline,schemas,catalog);
        var stock=GameObjectStockReview.Capture(manifest,"GameObject:SovietBomberAircraft_Dying",allowNullArmorReferences:true);
        var modules=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name,allowNullArmorReferences:true).Containers.Single(c=>c.Name=="Behaviors").Modules;
        foreach(var family in Families){
            uint wire=FastHash.GetHashCode(family.Wire);var module=modules.Single(m=>m.TypeId==wire);
            if(SdkBuildTests.Enabled){var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(GameObjectExtendedRootContract.Original("Soviet/Props/SovietBomberAircraft_Dying.xml",family.Source).Single(),true));SlowDeathContract.Normalize(source);var element=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName(family.Source,SchemaSet.XmlNamespace).OfType<XmlElement>().Single();SchemaNativeContract.VerifyStock(element,family.Type,wire,stock.Bytes,module.Native,stock.Asset.References.ToArray());}
            grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,family.Type);
        }
        SdkBuildTests.MetadataProgress("Dying projectiles","Reborn: dying-projectile qualification: PASS (original 28-byte projectile, 28-byte falling movement and 116-byte ship death).","2 original concrete modules and falling movement");return grammar;
    }
}
