using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: falling and straight-line projectile processors retain separate concrete source/native layouts.
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
        // Reborn: straight-line and falling state machines have different concrete movement records and must never be interchanged.
        string machine=source.HasAttribute("StateMachine")?source.GetAttribute("StateMachine"):"BombProjectile";
        string? expected=machine switch{"BombProjectile"=>"MoveToGround","StraightLineProjectile"=>"MoveInStraightLine",_=>null};
        if(children.Length!=1||expected==null||children[0].LocalName!=expected)
            throw SdkXmlDiagnostics.At(new InvalidDataException("ProjectileUpdate requires a supported matching movement: BombProjectile/MoveToGround or StraightLineProjectile/MoveInStraightLine. Requested state machine: "+machine+"."),movement??source);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain complete slow-death grammar and independently compare original aircraft-death and straight-line projectile records. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<ProjectileUpdateModuleData>()!=28||Marshal.SizeOf<ProjectileMoveToGroundData>()!=28||Marshal.SizeOf<ProjectileMoveInStraightLineData>()!=16||Marshal.SizeOf<ShipSlowDeathBehaviorModuleData>()!=116)throw new InvalidDataException("EP1 projectile layout extent differs.");
        var grammar=SlowDeathContract.Qualify(manifest,baseline,schemas,catalog);
        var stock=GameObjectStockReview.Capture(manifest,"GameObject:SovietBomberAircraft_Dying",allowNullArmorReferences:true);
        var modules=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name,allowNullArmorReferences:true).Containers.Single(c=>c.Name=="Behaviors").Modules;
        foreach(var family in Families){
            uint wire=FastHash.GetHashCode(family.Wire);var module=modules.Single(m=>m.TypeId==wire);
            if(SdkBuildTests.Enabled){var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(GameObjectExtendedRootContract.Original("Soviet/Props/SovietBomberAircraft_Dying.xml",family.Source).Single(),true));SlowDeathContract.Normalize(source);var element=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName(family.Source,SchemaSet.XmlNamespace).OfType<XmlElement>().Single();SchemaNativeContract.VerifyStock(element,family.Type,wire,stock.Bytes,module.Native,stock.Asset.References.ToArray());}
            grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,family.Type);
        }
        // Reborn: the original Guardian projectile independently qualifies the 16-byte straight-line movement and its metadata class wrapper.
        var shell=GameObjectStockReview.Capture(manifest,"GameObject:AlliedAntiVehicleVehicleTech1Projectile",allowNullArmorReferences:true);
        uint projectile=FastHash.GetHashCode("ProjectileUpdateModuleData");var straightModule=GameObjectModuleReview.Inspect(manifest,shell.Asset.Name,allowNullArmorReferences:true).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==projectile);
        if(SdkBuildTests.Enabled){var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(GameObjectExtendedRootContract.Original("Allied/Props/AlliedAntiVehicleVehicleTech1Projectile.xml","ProjectileUpdate").Single(),true));var element=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("ProjectileUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();SchemaNativeContract.VerifyStock(element,typeof(ProjectileUpdateModuleData),projectile,shell.Bytes,straightModule.Native,shell.Asset.References.ToArray());}
        grammar.Learn(shell.Bytes,straightModule.Wrapper,shell.Projection.NativeRoot,typeof(ProjectileUpdateModuleData));
        SdkBuildTests.MetadataProgress("Dying projectiles","Reborn: projectile qualification: PASS (28-byte header, 28-byte falling and 16-byte straight-line movement, 116-byte ship death).","3 original concrete modules");return grammar;
    }
}
