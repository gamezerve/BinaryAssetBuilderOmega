using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify the complete passive effect layout against original Uprising Chief Engineer, Allied Engineer and Hospital records.
internal static class PassiveAreaEffectContract
{
    internal const string Wire="PassiveAreaEffectBehaviorModuleData";
    internal static readonly uint TypeId=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(Wire);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: original Engineer radius definitions retain their actual global and local definition values for stock qualification. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlElement[] Originals(string path)
    {
        var definitions=GameObjectExtendedRootContract.Original("GlobalData/GlobalDefines.xml","Define").ToDictionary(d=>d.GetAttribute("name"),d=>d.GetAttribute("value"),StringComparer.Ordinal);
        // Reborn: the Allied health-tent radius is defined locally in AlliedEngineer.xml, overriding the imported global scope.
        foreach(var definition in GameObjectExtendedRootContract.Original(path,"Define"))definitions[definition.GetAttribute("name")]=definition.GetAttribute("value");
        return GameObjectExtendedRootContract.Original(path,"PassiveAreaEffectBehavior").Select(original=>{var element=(XmlElement)original.CloneNode(true);foreach(var attribute in element.SelectNodes(".//@*")!.Cast<XmlAttribute>())if(attribute.Value.StartsWith('='))attribute.Value=SdkDefinitionSubset.Evaluate(attribute.Value,name=>definitions.GetValueOrDefault(name));return element;}).ToArray();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently qualify the passive effect fields, their offsets and the Update/Contain inheritance chain against original Engineer and Hospital records. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<Ep1PassiveAreaEffectBehaviorModuleData>()!=80)throw new InvalidDataException("EP1 passive effect header must be 80 bytes.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("SovietChiefEngineer","EP1/Soviet/Units_Campaign/SovietChiefEngineer.xml"),("AlliedEngineer","Allied/Units/AlliedEngineer.xml"),("HospitalTechStructure","Neutral/Structures/HospitalTechStructure.xml")}){
            // Reborn: campaign Chief Engineer and tech Hospital are original WorldBuilder assets, not static entries.
            string stream=witness.Item1=="AlliedEngineer"?manifest:WorldBuilderAssetResolver.CatalogPath();
            var captured=GameObjectStockReview.Capture(stream,"GameObject:"+witness.Item1,allowNullArmorReferences:true);
            var modules=GameObjectModuleReview.Inspect(stream,captured.Asset.Name,allowNullArmorReferences:true).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==TypeId).ToArray();
            foreach(var original in Originals(witness.Item2)){
            uint id=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(original.GetAttribute("id").ToLowerInvariant());
            var module=modules.Single(m=>System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(captured.Bytes.AsSpan(m.Native+4))==id);
            // Reborn: actual retail metadata independently anchors every own field, including all inline values and the six trailing flags.
            int table=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(captured.Bytes.AsSpan(module.Wrapper+8));var fields=GameObjectStockReview.Tokens(captured.Bytes,table);
            foreach(string name in new[]{"EffectRadius","MaxBeneficiaries","HealingPercentPerSecond","HealingPointsPerSecond","SurveyDelayFrames","UpgradeRequired","AntiCategoryMask","AntiFX","HealFX","DisabledTypesToProcess","NonStackable","AffectAttached","AffectWoundedOnly","AffectUnderAttack","AffectContained","ShowDebugCircle"})if(fields[BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(name)].Offset-module.Native!=Marshal.OffsetOf<Ep1PassiveAreaEffectBehaviorModuleData>(name).ToInt32())throw new InvalidDataException("Stock passive effect field offset differs: "+name);
            if(SdkBuildTests.Enabled){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(original,true));
                var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("PassiveAreaEffectBehavior",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                SchemaNativeContract.VerifyStock(resolved,typeof(Ep1PassiveAreaEffectBehaviorModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
            }
            grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(Ep1PassiveAreaEffectBehaviorModuleData));count++;
            }
        }
        SdkBuildTests.MetadataProgress("PassiveAreaEffectBehavior",$"Reborn: PassiveAreaEffectBehavior EP1 native/metadata qualification: PASS ({count} original retail records; header={Marshal.SizeOf<Ep1PassiveAreaEffectBehaviorModuleData>()}).",count+" stock records");return grammar;
    }
}
