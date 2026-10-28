using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify the complete toggle layout against original Uprising infantry, vehicle and naval records, including Hammer leech AI.
internal static class ToggleStatusContract
{
    internal const string Wire="ToggleStatusSpecialAbilityUpdateModuleData";
    internal static readonly uint TypeId=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(Wire);

    // Reborn: retail owners cover weapons, optional AI/module references, module activation and active geometry lists.
    internal static readonly (string,string)[] Witnesses={
        ("SovietAntiInfantryInfantry","Soviet/Units/SovietAntiInfantryInfantry.xml"),
        ("SovietAntiVehicleInfantry","Soviet/Units/SovietAntiVehicleInfantry.xml"),
        ("SovietMiner","Soviet/Units/SovietMiner.xml"),
        ("SovietScoutVehicle","Soviet/Units/SovietScoutVehicle.xml"),
        // Reborn: Hammer supplies the populated LeechWeaponSOLModuleId pointer absent from the other toggle witnesses.
        ("SovietAntiVehicleVehicleTech1","Soviet/Units/SovietAntiVehicleVehicleTech1.xml"),
        ("SovietAntiStructureShip","Soviet/Units/SovietAntiStructureShip.xml"),
        ("JapanFortressShip","EP1/Japan/Units/JapanFortressShip.xml")};

    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify original EA declarations with their actual global definition context and stable child-family order. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlElement[] Originals(string path)
    {
        var definitions=GameObjectExtendedRootContract.Original("GlobalData/GlobalDefines.xml","Define").ToDictionary(d=>d.GetAttribute("name"),d=>d.GetAttribute("value"),StringComparer.Ordinal);
        return GameObjectExtendedRootContract.Original(path,"ToggleStatusSpecialAbilityUpdate").Select(original=>{
            var element=(XmlElement)original.CloneNode(true);
            foreach(var attribute in element.SelectNodes(".//@*")!.Cast<XmlAttribute>())if(attribute.Value.StartsWith('='))attribute.Value=SdkDefinitionSubset.Evaluate(attribute.Value,name=>definitions.GetValueOrDefault(name));
            foreach(var state in element.GetElementsByTagName("ToggleState",SchemaSet.XmlNamespace).OfType<XmlElement>())foreach(var child in state.ChildNodes.OfType<XmlElement>().OrderBy(c=>c.LocalName switch{"SkirmishAiInfo"=>0,"Module"=>1,_=>2}).ToArray())state.AppendChild(child);
            return element;
        }).ToArray();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently qualify toggle fields, populated leech-AI references and the SpecialAbility/Update inheritance chain against original infantry, vehicle and naval records. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<ToggleStatusSpecialAbilityUpdateModuleData>()!=264)throw new InvalidDataException("EP1 toggle header must be 264 bytes.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in Witnesses){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1);
            var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==TypeId).ToArray();
            foreach(var original in Originals(witness.Item2)){
            uint id=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(original.GetAttribute("id").ToLowerInvariant());
            var module=modules.Single(m=>System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(captured.Bytes.AsSpan(m.Native+4))==id);
            // Reborn: actual retail metadata independently anchors every own field, including the toggle state list and trailing flags.
            int table=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(captured.Bytes.AsSpan(module.Wrapper+8));var fields=GameObjectStockReview.Tokens(captured.Bytes,table);
            foreach(string name in new[]{"ReloadAllWeaponsOnToggle","ForceIdleAfterToggle"})if(fields[BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(name)].Offset-module.Native!=Marshal.OffsetOf<ToggleStatusSpecialAbilityUpdateModuleData>(name).ToInt32())throw new InvalidDataException("Stock ToggleStatus field offset differs: "+name);
            if(SdkBuildTests.Enabled){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(original,true));
                var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("ToggleStatusSpecialAbilityUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                SchemaNativeContract.VerifyStock(resolved,typeof(ToggleStatusSpecialAbilityUpdateModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
            }
            grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(ToggleStatusSpecialAbilityUpdateModuleData));count++;
            }
        }
        SdkBuildTests.MetadataProgress("ToggleStatusSpecialAbilityUpdate",$"Reborn: ToggleStatusSpecialAbilityUpdate EP1 native/metadata qualification: PASS ({count} original retail records; header={Marshal.SizeOf<ToggleStatusSpecialAbilityUpdateModuleData>()}).",count+" stock records");return grammar;
    }
}
