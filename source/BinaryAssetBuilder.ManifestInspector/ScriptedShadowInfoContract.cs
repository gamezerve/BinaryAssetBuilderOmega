using System.Buffers.Binary;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: ScriptedModelDraw/ModelConditionState/ShadowInfo (draw-state decals) is qualified on the original EP1 SovietCrane repair decal,
// the same shape the official SDK writes: a pointer to a 60-byte ShadowInfo record and its own 18-row metadata table.
internal static class ScriptedShadowInfoContract
{
    private const string Witness="SovietCrane";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove the stock native record from its original source and learn the ShadowInfo row grammar from the real draw-state table. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(StockMetadataGrammar grammar,string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+Witness);byte[] bytes=captured.Bytes;int witnesses=0;
        var originals=GameObjectExtendedRootContract.Original("Soviet/Structures/"+Witness+".xml","ScriptedModelDraw");
        foreach(var module in GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Draws").Modules.Where(m=>m.TypeId==0x4ECF2A4B)){
            // Reborn: inherited draws are not declared in the witness file; only its own declared draws are compared state-by-state.
            var original=originals.SingleOrDefault(e=>FastHash.GetHashCode(e.GetAttribute("id").ToLowerInvariant())==W(bytes,module.Native+4));if(original==null)continue;
            var sourceStates=original.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="ModelConditionState").ToArray();
            int table=GameObjectStockReview.Tokens(bytes,(int)W(bytes,module.Wrapper+8))[FastHash.GetHashCode("ModelConditionState")].Offset;int count=(int)W(bytes,(int)W(bytes,table+8));
            if(count!=sourceStates.Length)throw new InvalidDataException("ShadowInfo witness state cardinality differs: "+Witness);
            for(int i=0;i<count;i++){
                var source=sourceStates[i].ChildNodes.OfType<XmlElement>().SingleOrDefault(e=>e.LocalName=="ShadowInfo");int state=(int)W(bytes,table+12+i*12+8);int shadow=GameObjectStockReview.Tokens(bytes,state)[FastHash.GetHashCode("ShadowInfo")].Offset;
                if(source==null){if(shadow!=0)throw new InvalidDataException("ShadowInfo witness optional presence differs.");continue;}
                if(shadow==0)throw new InvalidDataException("ShadowInfo witness target is missing.");
                // Reborn: resolve the original decal (GlobalDefines sizes) under a valid baseline draw state, exactly like the material witness.
                var probe=(XmlDocument)baseline.CloneNode(true);var owner=probe.GetElementsByTagName("ModelConditionState",SchemaSet.XmlNamespace).OfType<XmlElement>().First();foreach(XmlNode child in owner.ChildNodes.Cast<XmlNode>().ToArray())owner.RemoveChild(child);var decal=(XmlElement)owner.AppendChild(probe.ImportNode(source,true))!;Evaluate(decal);
                var resolved=SchemaNativeContract.Resolve(probe,schemas,catalog).GetElementsByTagName("ShadowInfo",SchemaSet.XmlNamespace).OfType<XmlElement>().Single(e=>e.ParentNode is XmlElement parent&&parent.LocalName=="ModelConditionState");
                int native=GameObjectStockReview.Tokens(bytes,shadow)[FastHash.GetHashCode("Type")].Offset;
                SchemaNativeContract.VerifyStock(resolved,typeof(ShadowInfo),0,bytes,native,captured.Asset.References.ToArray());
                grammar.Learn(bytes,shadow,captured.Projection.NativeRoot,typeof(ShadowInfo));witnesses++;
            }
        }
        if(witnesses==0)throw new InvalidDataException("No original draw-state ShadowInfo witness found on "+Witness+".");
        Console.WriteLine("Reborn: EP1 scripted draw ShadowInfo stock qualification: PASS ("+Witness+", "+witnesses+" witness(es)).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: the original decal sizes are GlobalDefines expressions (e.g. "= ( $SOVIET_REPAIR_DRONE_LEASH_DISTANCE * 2 ) + $..."). Evaluate only
        + - * / ( ) over numeric literals and numeric defines; anything else is refused. The stock native comparison then proves the result. */
    //-------------------------------------------------------------------------------------------------
    private static void Evaluate(XmlElement element)
    {
        Dictionary<string,string>? defines=null;
        foreach(XmlAttribute attribute in element.Attributes.Cast<XmlAttribute>().ToArray()){
            if(!attribute.Value.StartsWith('='))continue;
            defines??=GameObjectExtendedRootContract.Original("GlobalData/GlobalDefines.xml","Define").ToDictionary(d=>d.GetAttribute("name"),d=>d.GetAttribute("value"),StringComparer.Ordinal);
            attribute.Value=Expression(attribute.Value,defines,0).ToString("R",System.Globalization.CultureInfo.InvariantCulture);
        }
    }

    private static float Expression(string text,IReadOnlyDictionary<string,string> defines,int depth)
    {
        if(depth>16)throw new InvalidDataException("ShadowInfo witness define recursion exceeded.");
        string body=text.StartsWith('=')?text[1..]:text;int at=0;double value=Sum();Skip();if(at!=body.Length)throw new InvalidDataException("ShadowInfo witness define expression is not supported: "+text);return (float)value;
        double Sum(){double v=Product();for(;;){Skip();if(Take('+'))v+=Product();else if(Take('-'))v-=Product();else return v;}}
        double Product(){double v=Atom();for(;;){Skip();if(Take('*'))v*=Atom();else if(Take('/'))v/=Atom();else return v;}}
        double Atom()
        {
            Skip();if(Take('(')){double v=Sum();Skip();if(!Take(')'))throw new InvalidDataException("ShadowInfo witness define parenthesis differs: "+text);return v;}
            if(Take('-'))return -Atom();
            if(Take('$')){int start=at;while(at<body.Length&&(char.IsLetterOrDigit(body[at])||body[at]=='_'))at++;string name=body[start..at];
                if(!defines.TryGetValue(name,out var value))throw new InvalidDataException("ShadowInfo witness define is missing: "+name);return Expression(value,defines,depth+1);}
            int begin=at;while(at<body.Length&&(char.IsDigit(body[at])||body[at]=='.'))at++;
            if(begin==at||!double.TryParse(body[begin..at],System.Globalization.NumberStyles.Float,System.Globalization.CultureInfo.InvariantCulture,out double number))throw new InvalidDataException("ShadowInfo witness define expression is not supported: "+text);return number;
        }
        void Skip(){while(at<body.Length&&char.IsWhiteSpace(body[at]))at++;}
        bool Take(char c){if(at<body.Length&&body[at]==c){at++;return true;}return false;}
    }

    private static uint W(byte[] bytes,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));
}
