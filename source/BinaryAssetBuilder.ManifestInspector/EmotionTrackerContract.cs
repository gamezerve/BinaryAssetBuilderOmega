using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify the complete emotion tracker layout against original Uprising Soviet, Allied and Imperial Engineer records.
internal static class EmotionTrackerContract
{
    internal const string Wire="EmotionTrackerUpdateModuleData";
    internal static readonly uint TypeId=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(Wire);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently qualify the emotion tracker fields, their offsets and the Update/Contain inheritance chain against original faction Engineer records. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<Ep1EmotionTrackerUpdateModuleData>()!=56)throw new InvalidDataException("EP1 emotion tracker header must be 56 bytes.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("SovietEngineer","Soviet/Units/SovietEngineer.xml"),("AlliedEngineer","Allied/Units/AlliedEngineer.xml"),("JapanEngineer","Japan/Units/JapanEngineer.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1);
            var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==TypeId).ToArray();
            foreach(var original in GameObjectExtendedRootContract.Original(witness.Item2,"EmotionTrackerUpdate")){
            uint id=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(original.GetAttribute("id").ToLowerInvariant());
            var module=modules.Single(m=>System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(captured.Bytes.AsSpan(m.Native+4))==id);
            // Reborn: actual retail metadata independently anchors every own field, including the scan distances, update delay, fear level and trailing veterancy flag.
            int table=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(captured.Bytes.AsSpan(module.Wrapper+8));var fields=GameObjectStockReview.Tokens(captured.Bytes,table);
            foreach(string name in new[]{"TauntAndPointDistance","HeroScanDistance","TauntAndPointUpdateDelay","FearScanDistance","ImmuneToFearLevel","IgnoreVeterancy"})if(fields[BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(name)].Offset-module.Native!=Marshal.OffsetOf<Ep1EmotionTrackerUpdateModuleData>(name).ToInt32())throw new InvalidDataException("Stock emotion tracker field offset differs: "+name);
            if(SdkBuildTests.Enabled){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(original,true));
                var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("EmotionTrackerUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                SchemaNativeContract.VerifyStock(resolved,typeof(Ep1EmotionTrackerUpdateModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
            }
            grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(Ep1EmotionTrackerUpdateModuleData));count++;
            }
        }
        SdkBuildTests.MetadataProgress("EmotionTrackerUpdate",$"Reborn: EmotionTrackerUpdate EP1 native/metadata qualification: PASS ({count} original retail records; header={Marshal.SizeOf<Ep1EmotionTrackerUpdateModuleData>()}).",count+" stock records");return grammar;
    }
}
