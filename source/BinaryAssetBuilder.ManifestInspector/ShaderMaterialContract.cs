using System.Buffers.Binary;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using System.Xml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: recover and qualify draw-material records from bounded original game asset slices, never whole game binary dumps.
internal static class ShaderMaterialContract
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify original shader source/native values and learn all concrete constant grammars from real material tables. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(StockMetadataGrammar grammar,string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        var classes=new HashSet<Type>();int witnesses=0;
        foreach(string name in new[]{"SovietBarracks","JapanWarFactory"}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+name);byte[] bytes=captured.Bytes;
            string path=(name.StartsWith("Soviet")?"Soviet":"Japan")+"/Structures/"+name+".xml";
            var originals=GameObjectExtendedRootContract.Original(path,"ScriptedModelDraw");
            foreach(var module in GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Draws").Modules.Where(m=>m.TypeId==0x4ECF2A4B)){
                var original=originals.Single(e=>FastHash.GetHashCode(e.GetAttribute("id").ToLowerInvariant())==W(bytes,module.Native+4));
                var sourceStates=original.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="ModelConditionState").ToArray();
                int table=GameObjectStockReview.Tokens(bytes,(int)W(bytes,module.Wrapper+8))[FastHash.GetHashCode("ModelConditionState")].Offset;int count=(int)W(bytes,(int)W(bytes,table+8));
                if(count!=sourceStates.Length)throw new InvalidDataException("Material witness state cardinality differs: "+name);
                for(int i=0;i<count;i++){
                    var source=sourceStates[i].ChildNodes.OfType<XmlElement>().SingleOrDefault(e=>e.LocalName=="Material");int state=(int)W(bytes,table+12+i*12+8);int material=GameObjectStockReview.Tokens(bytes,state)[FastHash.GetHashCode("Material")].Offset;
                    if(source==null){if(material!=0)throw new InvalidDataException("Material witness optional presence differs.");continue;}
                    if(material==0)throw new InvalidDataException("Material witness target is missing.");
                    // Reborn: qualify just the original material under a valid baseline owner; unrelated raw EA draw child ordering is normalized by the actual source frontend.
                    var probe=(XmlDocument)baseline.CloneNode(true);var stateOwner=probe.GetElementsByTagName("ModelConditionState",SchemaSet.XmlNamespace).OfType<XmlElement>().First();foreach(XmlNode child in stateOwner.ChildNodes.Cast<XmlNode>().ToArray())stateOwner.RemoveChild(child);stateOwner.AppendChild(probe.ImportNode(source,true));
                    source=SchemaNativeContract.Resolve(probe,schemas,catalog).GetElementsByTagName("Material",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                    int native=GameObjectStockReview.Tokens(bytes,material)[FastHash.GetHashCode("ShaderName")].Offset;
                    SchemaNativeContract.VerifyStock(source,typeof(FXShaderMaterial),0,bytes,native,captured.Asset.References.ToArray());
                    grammar.Learn(bytes,material,captured.Projection.NativeRoot,typeof(FXShaderMaterial));witnesses++;
                    foreach(var constant in source.GetElementsByTagName("Constants",SchemaSet.XmlNamespace).OfType<XmlElement>().SelectMany(e=>e.ChildNodes.OfType<XmlElement>()))classes.Add(SchemaNativeContract.ShaderConstantType(FastHash.GetHashCode(constant.SchemaInfo.SchemaType!.QualifiedName.Name)));
                }
            }
        }
        if(classes.Count!=4||witnesses==0)throw new InvalidDataException("Shader material stock witnesses do not cover all four constant classes.");
        Console.WriteLine("Reborn: shader material stock qualification: "+witnesses+" materials, four constant classes.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: inspect only Material and constant metadata rows needed to connect the existing shader marshaler. */
    //-------------------------------------------------------------------------------------------------
    internal static void Review(string manifest)
    {
        foreach(string name in new[]{"SovietBarracks","JapanWarFactory"}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+name);byte[] bytes=captured.Bytes;
            foreach(var module in GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Draws").Modules.Where(m=>m.TypeId==0x4ECF2A4B)){
                var root=GameObjectStockReview.Tokens(bytes,(int)W(bytes,module.Wrapper+8));int states=root[FastHash.GetHashCode("ModelConditionState")].Offset;int count=(int)W(bytes,(int)W(bytes,states+8));
                for(int i=0;i<count;i++){int state=(int)W(bytes,states+12+i*12+8);var material=GameObjectStockReview.Tokens(bytes,state)[FastHash.GetHashCode("Material")];if(material.Offset==0)continue;
                    Console.WriteLine("Material witness="+name+" draw="+module.Native+" state="+i);Rows(material.Offset);var fields=GameObjectStockReview.Tokens(bytes,material.Offset);int constants=fields[FastHash.GetHashCode("Constants")].Offset;
                    if(constants==0)continue;int constantCount=(int)W(bytes,(int)W(bytes,constants+8));Console.WriteLine("constants="+constantCount+" marker="+W(bytes,constants).ToString("X8")+"/"+W(bytes,constants+4).ToString("X8"));
                    for(int j=0;j<constantCount;j++){int item=constants+12+j*12,target=(int)W(bytes,item+8);Console.WriteLine("item="+W(bytes,item).ToString("X8")+"/"+W(bytes,item+4).ToString("X8")+" target="+target);Rows(target);}
                    break;
                }
            }
            // Reborn: print bounded name/type/target tuples and one base/complex table, not native texture or shader payloads.
            void Rows(int table){for(int i=0;i<8;i++){int at=table+i*12;uint field=W(bytes,at),type=W(bytes,at+4),target=W(bytes,at+8);Console.WriteLine("  "+field.ToString("X8")+" "+type.ToString("X8")+" "+target);if(field==0&&type==0&&target==0)break;if(field==2||field==1){int nested=(int)target;for(int j=0;j<5;j++){int row=nested+j*12;Console.WriteLine("    "+W(bytes,row).ToString("X8")+" "+W(bytes,row+4).ToString("X8")+" "+W(bytes,row+8));if(W(bytes,row)==0&&W(bytes,row+4)==0&&W(bytes,row+8)==0)break;}}}}
        }
    }

    // Reborn: evidence reads stay within the selected material's owning GameObject slice.
    private static uint W(byte[] bytes,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));
}
