using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: original Japan behavior records are qualified before source-sized native and metadata composition.
internal static class JapanModuleContract
{
    internal static readonly (string Source,string Wire,Type Type,string Owner,string Path)[] Families={
        ("ChargeWeaponUpdate","ChargeWeaponUpdateModuleData",typeof(Ep1ChargeWeaponUpdateModuleData),"JapanBaseDefenseAdvanced","Japan/Structures/JapanBaseDefenseAdvanced.xml"),
        ("PrematureDischargeSpecialPower","PrematureDischargeSpecialPowerModuleData",typeof(Ep1PrematureDischargeSpecialPowerModuleData),"JapanBaseDefenseAdvanced","Japan/Structures/JapanBaseDefenseAdvanced.xml"),
        ("SpiderHoleUpdate","SpiderHoleUpdateModuleData",typeof(Ep1SpiderHoleUpdateModuleData),"JapanAntiVehicleInfantry","Japan/Units/JapanAntiVehicleInfantry.xml"),
        ("ActivateSpecialPowerOnCreate","ActivateSpecialPowerOnCreate",typeof(Ep1ActivateSpecialPowerOnCreate),"JapanAntiVehicleInfantry_Ambush","Japan/Units/JapanAntiVehicleInfantry.xml"),
        ("MovementSpecialPower","MovementSpecialPowerModuleData",typeof(Ep1MovementSpecialPowerModuleData),"JapanEngineer","Japan/Units/JapanEngineer.xml"),
        ("TimedOpacityUpdate","TimedOpacityUpdateModuleData",typeof(Ep1TimedOpacityUpdateModuleData),"JapanInfiltrationInfantry","Japan/Units/JapanInfiltrationInfantry.xml"),
        ("MonitorConditionUpdate","MonitorConditionUpdateModuleData",typeof(Ep1MonitorConditionUpdateModuleData),"JapanMiner","Japan/Units/JapanMiner.xml"),
        ("UnitUnpackUpdate","UnitUnpackUpdateModuleData",typeof(UnitUnpackUpdateModuleData),"JapanFortressShip","EP1/Japan/Units/JapanFortressShip.xml"),
        ("LifetimeUpdate","LifetimeUpdateModuleData",typeof(Ep1LifetimeUpdateModuleData),"JapanFortressShipSeigeCannonNode","EP1/Japan/Units/JapanFortressShip.xml"),
        ("Physics","PhysicsBehaviorModuleData",typeof(Ep1PhysicsBehaviorModuleData),"JapanAntiVehicleInfantryTech3","Japan/Units/JapanAntiVehicleInfantryTech3.xml"),
    };
    internal static bool Supports(Type type)=>Families.Any(f=>f.Type==type);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: resolve original literal definition context while retaining the actual declaring GameObject for secondary effect records. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlElement[] Originals(string path,string source,string owner)
    {
        var definitions=GameObjectExtendedRootContract.Original("GlobalData/GlobalDefines.xml","Define").ToDictionary(d=>d.GetAttribute("name"),d=>d.GetAttribute("value"),StringComparer.Ordinal);
        foreach(var define in GameObjectExtendedRootContract.Original(path,"Define"))definitions[define.GetAttribute("name")]=define.GetAttribute("value");
        return GameObjectExtendedRootContract.Original(path,source).Where(e=>e.ParentNode?.ParentNode is XmlElement game&&game.LocalName=="GameObject"&&game.GetAttribute("id")==owner).Select(original=>{var element=(XmlElement)original.CloneNode(true);
            // Reborn: original Sentinel XML reverses the two sequence children; qualify the schema-normalized copy used by production preparation.
            if(element.LocalName=="LinearDamageUpdate")foreach(var status in element.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="ObjectStatusValidation").ToArray())element.PrependChild(status);
            foreach(XmlAttribute attribute in element.SelectNodes(".//@*")!)if(attribute.Value.StartsWith('='))attribute.Value=SdkDefinitionSubset.Evaluate(attribute.Value,name=>definitions.GetValueOrDefault(name));return element;}).ToArray();
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently qualify real EP1 source/native values and learn literal class, reference and optional-pointer metadata grammar. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        // Reborn: header strides measured from original EP1 records, before strings/lists/optional allocations.
        int[] sizes={24,476,136,16,512,16,84,20,44};for(int i=0;i<sizes.Length;i++)if(Marshal.SizeOf(Families[i].Type)!=sizes[i])throw new InvalidDataException("Japan native header differs: "+Families[i].Wire);
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var family in Families)Learn(family.Source,family.Wire,family.Type,family.Owner,family.Path);
        // Reborn: King Oni supplies the allocated movement status/model masks absent from the Engineer power.
        Learn("MovementSpecialPower","MovementSpecialPowerModuleData",typeof(Ep1MovementSpecialPowerModuleData),"JapanAntiVehicleVehicleTech3","Japan/Units/JapanAntiVehicleVehicleTech3.xml");
        SdkBuildTests.MetadataProgress("Japan modules","Reborn: Japan module grammar: PASS (10 families).",count+" original leaves");return grammar;
        // Reborn: inert cannon nodes intentionally have null armor and still retain fully validated native/module topology.
        void Learn(string sourceName,string wireName,Type type,string owner,string path){
            uint wire=FastHash.GetHashCode(wireName);var stock=GameObjectStockReview.Capture(manifest,"GameObject:"+owner,allowNullArmorReferences:true);var modules=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name,allowNullArmorReferences:true).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==wire).ToArray();
            if(modules.Length==0)throw new InvalidDataException("Japan native witness missing: "+wireName+" in "+owner);
            foreach(var module in modules){if(SdkBuildTests.Enabled){var original=Originals(path,sourceName,owner).Single(e=>FastHash.GetHashCode(e.GetAttribute("id").ToLowerInvariant())==System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(stock.Bytes.AsSpan(module.Native+4)));var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(original,true));var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName(sourceName,SchemaSet.XmlNamespace).OfType<XmlElement>().Single();SchemaNativeContract.VerifyStock(resolved,type,wire,stock.Bytes,module.Native,stock.Asset.References.ToArray());}grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,type);count++;}
        }
    }
}
