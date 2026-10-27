using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: complete SlowDeath records use source-owned phases, masks, strings and typed FX/OCL/weapon imports rather than a Conscript-shaped template.
internal static class SlowDeathContract
{
    internal const string Wire="SlowDeathBehaviorModuleData";
    internal static readonly uint TypeId=FastHash.GetHashCode(Wire);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve order within each phase family while accepting the original EA order of independent FX/OCL/Weapon/Sound/DieMux fields. */
    //-------------------------------------------------------------------------------------------------
    internal static void Normalize(XmlDocument source)
    {
        foreach(var leaf in source.GetElementsByTagName("SlowDeath",SchemaSet.XmlNamespace).OfType<XmlElement>().ToArray())ConscriptBehaviorSurvey.Prepare(leaf,"");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently qualify original scout/conscript native values and learn populated phase records from actual static/WorldBuilder metadata. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<SlowDeathBehaviorModuleData>()!=116)throw new InvalidDataException("EP1 SlowDeath header must be 116 bytes.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("SovietScoutInfantry","Soviet/Units/SovietScoutInfantry.xml"),("AlliedScoutInfantry","Allied/Units/AlliedScoutInfantry.xml"),("SovietAntiInfantryInfantry","Soviet/Units/SovietAntiInfantryInfantry.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1);var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==TypeId).ToArray();
            foreach(var original in GameObjectExtendedRootContract.Original(witness.Item2,"SlowDeath")){
                uint id=FastHash.GetHashCode(original.GetAttribute("id").ToLowerInvariant());var module=modules.Single(m=>BinaryPrimitives.ReadUInt32LittleEndian(captured.Bytes.AsSpan(m.Native+4))==id);
                int table=BinaryPrimitives.ReadInt32LittleEndian(captured.Bytes.AsSpan(module.Wrapper+8));var fields=GameObjectStockReview.Tokens(captured.Bytes,table);
                // Reborn: retail native targets anchor every inline scalar and flag independently of the writer's field order.
                foreach(var field in typeof(SlowDeathBehaviorModuleData).GetFields().Where(f=>f.Name!="Base"&&!f.FieldType.IsPointer&&!f.FieldType.IsGenericType))if(fields[FastHash.GetHashCode(field.Name)].Offset-module.Native!=Marshal.OffsetOf<SlowDeathBehaviorModuleData>(field.Name).ToInt32())throw new InvalidDataException("Stock SlowDeath field offset differs: "+field.Name);
                if(SdkBuildTests.Enabled){var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(original,true));Normalize(source);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("SlowDeath",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();SchemaNativeContract.VerifyStock(resolved,typeof(SlowDeathBehaviorModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());}
                grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(SlowDeathBehaviorModuleData));count++;
            }
        }
        // Reborn: these real populated phase records qualify metadata grammar only; no unresolved original definition is substituted or used as authored native output.
        foreach(var witness in new[]{(manifest,"IrradiatedTargetDamageDealer"),(WorldBuilderAssetResolver.CatalogPath(),"ExplodingBarrel_Delay")}){
            var captured=GameObjectStockReview.Capture(witness.Item1,"GameObject:"+witness.Item2,allowNullArmorReferences:true);
            foreach(var module in GameObjectModuleReview.Inspect(witness.Item1,captured.Asset.Name,allowNullArmorReferences:true).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==TypeId))grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(SlowDeathBehaviorModuleData));
        }
        SdkBuildTests.MetadataProgress("SlowDeath",$"Reborn: SlowDeath EP1 qualification: PASS ({count} original source/native graphs; populated FX/OCL/Weapon/Sound metadata; header=116).",count+" original records and populated phase metadata");return grammar;
    }
}
