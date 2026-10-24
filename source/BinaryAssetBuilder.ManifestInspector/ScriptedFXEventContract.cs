using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: scripted FX events are source-sized frame-triggered FXList records; original reactor sparks supply their independent stock grammar.
internal static class ScriptedFXEventContract
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare original reactor event fields/imports to retail bytes and learn each nested event table without copying stock payloads. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(StockMetadataGrammar grammar,string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<FXEvent>()!=28)throw new InvalidDataException("EP1 scripted FXEvent header differs.");
        var captured=GameObjectStockReview.Capture(manifest,"GameObject:SovietPowerPlantAdvanced");var bytes=captured.Bytes;int witnesses=0;
        var originals=GameObjectExtendedRootContract.Original("Soviet/Structures/SovietPowerPlantAdvanced.xml","ScriptedModelDraw");
        foreach(var module in GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Draws").Modules.Where(m=>m.TypeId==0x4ECF2A4B)){
            var original=originals.SingleOrDefault(e=>FastHash.GetHashCode(e.GetAttribute("id").ToLowerInvariant())==W(bytes,module.Native+4));if(original==null)continue;
            var root=GameObjectStockReview.Tokens(bytes,(int)W(bytes,module.Wrapper+8));
            foreach(string stateName in new[]{"ModelConditionState","AnimationState"}){
                var states=original.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName==stateName).ToArray();int table=root[FastHash.GetHashCode(stateName)].Offset;int count=(int)W(bytes,(int)W(bytes,table+8));
                if(count!=states.Length)throw new InvalidDataException("FXEvent stock/source state cardinality differs: "+stateName);
                for(int i=0;i<count;i++){
                    var events=states[i].ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="FXEvent").ToArray();int state=(int)W(bytes,table+12+i*12+8);int eventTable=GameObjectStockReview.Tokens(bytes,state)[FastHash.GetHashCode("FXEvent")].Offset;int eventCount=(int)W(bytes,(int)W(bytes,eventTable+8));
                    if(eventCount!=events.Length)throw new InvalidDataException("FXEvent stock/source event cardinality differs.");if(eventCount==0)continue;
                    var probe=(XmlDocument)baseline.CloneNode(true);var owner=probe.GetElementsByTagName(stateName,SchemaSet.XmlNamespace).OfType<XmlElement>().First();foreach(XmlNode child in owner.ChildNodes.Cast<XmlNode>().ToArray())owner.RemoveChild(child);foreach(var fx in events)owner.AppendChild(probe.ImportNode(fx,true));
                    var resolved=SchemaNativeContract.Resolve(probe,schemas,catalog).GetElementsByTagName(stateName,SchemaSet.XmlNamespace).OfType<XmlElement>().First().ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="FXEvent").ToArray();
                    for(int j=0;j<eventCount;j++){int eventTokens=(int)W(bytes,eventTable+12+j*12+8);int native=GameObjectStockReview.Tokens(bytes,eventTokens)[FastHash.GetHashCode("Frame")].Offset;SchemaNativeContract.VerifyStock(resolved[j],typeof(FXEvent),0,bytes,native,captured.Asset.References.ToArray());grammar.Learn(bytes,eventTokens,captured.Projection.NativeRoot,typeof(FXEvent));witnesses++;}
                }
            }
        }
        if(witnesses==0)throw new InvalidDataException("No original scripted FXEvent stock witness found.");
        Console.WriteLine($"Reborn: scripted FXEvent stock qualification: PASS ({witnesses} original reactor spark event(s)).");
    }

    // Reborn: targeted evidence reads remain inside the already captured GameObject slice.
    private static uint W(byte[] bytes,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));
}
