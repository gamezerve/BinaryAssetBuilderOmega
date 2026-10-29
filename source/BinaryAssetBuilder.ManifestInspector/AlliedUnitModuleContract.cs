using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: stock-qualified Allied infantry, vehicle, naval and aircraft modules share one source-driven composition contract.
internal static class AlliedUnitModuleContract
{
    internal static readonly (string Source,string Wire,Type Type,string Owner,string Path,string Container)[] Families={
        ("HitReactionBehavior","HitReactionBehaviorModuleData",typeof(Ep1HitReactionBehaviorModuleData),"AlliedAntiInfantryInfantry","Allied/Units/AlliedAntiInfantryInfantry.xml","Behaviors"),
        ("ActivateModuleSpecialPower","ActivateModuleSpecialPower",typeof(Ep1ActivateModuleSpecialPower),"AlliedAntiStructureVehicle","Allied/Units/AlliedAntiStructureVehicle.xml","Behaviors"),
        ("BeamSpecialAbilityUpdate","BeamSpecialAbilityUpdateModuleData",typeof(Ep1BeamSpecialAbilityUpdateModuleData),"AlliedFutureTank","EP1/Allied/Units/AlliedFutureTank.xml","Behaviors"),
        ("DominateEnemySpecialPower","DominateEnemySpecialPowerModuleData",typeof(Ep1DominateEnemySpecialPowerModuleData),"AlliedInfiltrationInfantry","Allied/Units/AlliedInfiltrationInfantry.xml","Behaviors"),
        ("InvisibilityBroadcastUpdate","InvisibilityBroadcastUpdateModuleData",typeof(Ep1InvisibilityBroadcastUpdateModuleData),"AlliedAntiVehicleVehicleTech3","Allied/Units/AlliedAntiVehicleVehicleTech3.xml","Behaviors"),
        ("LinearDamageUpdate","LinearDamageUpdateModuleData",typeof(Ep1LinearDamageUpdateModuleData),"AlliedAntiNavalScout","Allied/Units/AlliedAntiNavalScout.xml","Behaviors"),
        ("MagneticEffectUpdate","MagneticEffectUpdateModuleData",typeof(Ep1MagneticEffectUpdateModuleData),"AlliedAntiNavyShipTech1","Allied/Units/AlliedAntiNavyShipTech1.xml","Behaviors"),
        ("ReconstituteStateSpecialAbility","ReconstituteStateSpecialAbilityModuleData",typeof(Ep1ReconstituteStateSpecialAbilityModuleData),"AlliedCommandoTech1","Allied/Units/AlliedCommandoTech1.xml","Behaviors"),
        ("ShieldSphereUpdate","ShieldSphereUpdateModuleData",typeof(ShieldSphereUpdateModuleData),"AlliedAntiStructureVehicle","Allied/Units/AlliedAntiStructureVehicle.xml","Behaviors"),
        ("SpecialDisguiseUpdate","SpecialDisguiseUpdateModuleData",typeof(Ep1SpecialDisguiseUpdateModuleData),"AlliedInfiltrationInfantry","Allied/Units/AlliedInfiltrationInfantry.xml","Behaviors"),
        ("StoreCurrentStateUpdate","StoreCurrentStateUpdateModuleData",typeof(Ep1StoreCurrentStateUpdateModuleData),"AlliedCommandoTech1","Allied/Units/AlliedCommandoTech1.xml","Behaviors"),
        ("ModelConditionEvaEventSelectorClientBehavior","ModelConditionEvaEventSelectorClientBehaviorModuleData",typeof(Ep1ModelConditionEvaEventSelectorClientBehaviorModuleData),"AlliedInfiltrationInfantry","Allied/Units/AlliedInfiltrationInfantry.xml","ClientBehaviors"),
        ("UpgradeSoundSelectorClientBehavior","UpgradeSoundSelectorClientBehaviorModuleData",typeof(Ep1UpgradeSoundSelectorClientBehaviorModuleData),"AlliedAntiAirVehicleTech1","Allied/Units/AlliedAntiAirVehicleTech1.xml","ClientBehaviors"),
    };
    internal static bool Supports(Type type)=>Families.Any(f=>f.Type==type);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: learn actual stock class/field/reference grammar for every selected family; developer mode additionally compares original source values. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        // Reborn: measured EP1 header sizes exclude separately allocated strings, lists and optional records.
        int[] sizes={48,484,300,272,44,48,160,272,312,280,16,16,16};for(int i=0;i<Families.Length;i++)if(Marshal.SizeOf(Families[i].Type)!=sizes[i])throw new InvalidDataException("Allied EP1 native header size differs: "+Families[i].Wire);
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var family in Families){
            uint wire=FastHash.GetHashCode(family.Wire);var stock=GameObjectStockReview.Capture(manifest,"GameObject:"+family.Owner);
            var modules=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name==family.Container).Modules.Where(m=>m.TypeId==wire).ToArray();
            if(modules.Length==0)throw new InvalidDataException("Original Allied module witness missing: "+family.Wire);
            // Reborn: original module values independently qualify the model without using its producer as an oracle.
            XmlDocument? resolved=null;
            if(SdkBuildTests.Enabled){var source=(XmlDocument)baseline.CloneNode(true);var container=source.GetElementsByTagName(family.Container,SchemaSet.XmlNamespace).OfType<XmlElement>().Single();container.RemoveAll();foreach(var original in GameObjectExtendedRootContract.Original(family.Path,family.Source))container.AppendChild(source.ImportNode(original,true));resolved=SchemaNativeContract.Resolve(source,schemas,catalog);}
            foreach(var module in modules){
                if(resolved!=null){var original=resolved.GetElementsByTagName(family.Source,SchemaSet.XmlNamespace).OfType<XmlElement>().Single(e=>Hash(e)==Word(stock.Bytes,module.Native+4));SchemaNativeContract.VerifyStock(original,family.Type,wire,stock.Bytes,module.Native,stock.Asset.References.ToArray());}
                grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,family.Type);count++;
            }
        }
        // Reborn: Mirage terrain mappings populate the disguise list; the original Spy legitimately has an empty list.
        var mirage=GameObjectStockReview.Capture(manifest,"GameObject:AlliedAntiVehicleVehicleTech3");var disguise=GameObjectModuleReview.Inspect(manifest,mirage.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==FastHash.GetHashCode("SpecialDisguiseUpdateModuleData"));if(SdkBuildTests.Enabled){var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(GameObjectExtendedRootContract.Original("Allied/Units/AlliedAntiVehicleVehicleTech3.xml","SpecialDisguiseUpdate").Single(),true));var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("SpecialDisguiseUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();SchemaNativeContract.VerifyStock(resolved,typeof(Ep1SpecialDisguiseUpdateModuleData),disguise.TypeId,mirage.Bytes,disguise.Native,mirage.Asset.References.ToArray());}
        grammar.Learn(mirage.Bytes,disguise.Wrapper,mirage.Projection.NativeRoot,typeof(Ep1SpecialDisguiseUpdateModuleData));count++;
        SdkBuildTests.MetadataProgress("Allied unit modules","Reborn: Allied unit module grammar: PASS (13 families).",count+" original leaves");return grammar;
    }
    // Reborn: optional retail EVA identifiers retain zero, while ordinary named modules use local Poid hashing.
    private static uint Hash(XmlElement element)=>element.HasAttribute("id")?FastHash.GetHashCode(element.GetAttribute("id").ToLowerInvariant()):0u;
    private static uint Word(byte[] bytes,int at)=>System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));
}
