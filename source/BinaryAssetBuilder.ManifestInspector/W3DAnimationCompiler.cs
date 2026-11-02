extern alias W3x;
using System.Buffers.Binary;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: authored W3X channels become the original EP1 runtime timecoded/delta records, never source-channel structs.
internal static class W3DAnimationCompiler
{
    internal sealed record Channel(int Pivot,int Type,int First,float[][] Values,bool[] Binary);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject actual invalid channel ranges and values before the unsafe legacy animation processor sees them. */
    //-------------------------------------------------------------------------------------------------
    internal static void Admit(XmlElement source)
    {
        int count=Number(source,"NumFrames");
        if(count>0&&Number(source,"FrameRate")==0)throw SdkXmlDiagnostics.At(new InvalidDataException("A nonempty animation requires a positive FrameRate."),source);
        var seen=new HashSet<(int,int)>();
        foreach(var node in Elements(source,"Channels").SelectMany(e=>e.ChildNodes.OfType<XmlElement>())){
            try{
                var c=Read(node);if(!seen.Add((c.Pivot,c.Type)))throw new InvalidDataException("Duplicate animation Pivot/Type channel.");
                if(c.First>count-c.Values.Length)throw new InvalidDataException("Animation FirstFrame plus channel sample count exceeds NumFrames.");
                if(c.Values.SelectMany(v=>v).Any(v=>!float.IsFinite(v)))throw new InvalidDataException("Animation samples must be finite.");
                if(c.Type==3&&c.Values.Any(v=>v.Sum(x=>x*x)<1e-12f))throw new InvalidDataException("Animation quaternion cannot have zero length.");
            }catch(Exception error)when(error is InvalidDataException or FormatException or OverflowException){throw SdkXmlDiagnostics.At(new InvalidDataException(error.Message,error),node);}
        }
        foreach(var settings in Elements(source,"CompressionSettings")){
            foreach(string name in new[]{"MaxTranslationError","MaxRotationError","MaxVisibilityError","MaxAdaptiveDeltaError","ForceReductionRate"})
                if(settings.HasAttribute(name)&&(!float.IsFinite(XmlConvert.ToSingle(settings.GetAttribute(name)))||XmlConvert.ToSingle(settings.GetAttribute(name))<0))
                    throw SdkXmlDiagnostics.At(new InvalidDataException("Animation compression settings must be finite and nonnegative: "+name),settings.GetAttributeNode(name));
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: use Core-owned hierarchy imports, then independently decode every emitted sample and BinaryMove boundary. */
    //-------------------------------------------------------------------------------------------------
    internal static AssetBuffer Compile(InstanceDeclaration instance)
    {
        var source=(XmlElement)instance.XmlNode;Admit(source);
        foreach(var node in Elements(source,"Channels").SelectMany(e=>e.ChildNodes.OfType<XmlElement>()))
            node.SetAttribute("TypeId",(node.LocalName=="ChannelScalar"?0x50D28EF5u:0xF642DD20u).ToString(System.Globalization.CultureInfo.InvariantCulture));
        var plugin=new W3x::BinaryAssetBuilder.W3XCompiler.Plugin{PreserveAnimationSamples=true};plugin.Initialize(TargetPlatform.Win32);
        var result=plugin.ProcessInstance(instance);Verify(source,result);
        return result;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bounded playback readback is independent of the compressor and checks both EP1 runtime channel families. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlElement source,AssetBuffer buffer)
    {
        var b=buffer.InstanceData;uint W(int at){Range(at,4);return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
        float F(int at)=>BitConverter.UInt32BitsToSingle(W(at));
        void Range(int at,int size){if(at<0||size<0||at>b.Length-size)throw new InvalidDataException("Animation runtime range exceeds BIN.");}
        int frames=Number(source,"NumFrames");var channels=Elements(source,"Channels").SelectMany(e=>e.ChildNodes.OfType<XmlElement>()).Select(Read).ToArray();
        if(W(0)!=0||W(8)!=frames||W(12)!=Number(source,"FrameRate")||W(16)!=channels.Length)throw new InvalidDataException("EP1 animation runtime header differs.");
        // Reborn: hierarchy occurrence ownership uses the same index-plus-one contract as all other emitted W3D assets.
        string hierarchy=source.GetAttribute("Hierarchy");uint reference=hierarchy.Contains('\\')?checked(uint.Parse(hierarchy[(hierarchy.LastIndexOf('\\')+1)..],System.Globalization.CultureInfo.InvariantCulture)+1):0;
        if(W(4)!=reference)throw new InvalidDataException("Animation hierarchy import differs.");
        int list=checked((int)W(20));Range(list,channels.Length*4);
        for(int i=0;i<channels.Length;i++){
            var c=channels[i];int p=checked((int)W(list+i*4)),width=c.Type==3?4:1;Range(p,36);
            if(W(p+4)!=c.Type||W(p+12)!=width||(W(p+16)&65535)!=c.Pivot)throw new InvalidDataException("Animation channel identity differs.");
            var expected=Expand(c,frames);var decoded=new float[frames][];var binary=new bool[frames];float tolerance=0;
            if(W(p)==0x60D8CF69){
                int keys=checked((int)W(p+8)),flags=checked((int)W(p+24)),values=checked((int)W(p+32));
                if(keys!=frames||W(p+20)!=keys||W(p+28)!=keys*width)throw new InvalidDataException("Animation timecoded vector/count differs.");
                Range(flags,keys*2);Range(values,keys*width*4);
                for(int k=0;k<keys;k++){ushort key=BinaryPrimitives.ReadUInt16LittleEndian(b.AsSpan(flags+k*2,2));if((key&32767)!=k)throw new InvalidDataException("Animation timecode differs.");binary[k]=(key&32768)!=0;decoded[k]=Enumerable.Range(0,width).Select(j=>F(values+(k*width+j)*4)).ToArray();}
            }else if(W(p)==0x77F97305){
                Range(p,44);float scale=F(p+20);int bits=checked((int)W(p+32)),init=checked((int)W(p+28)),data=checked((int)W(p+40));
                if(bits is not (4 or 8)||W(p+24)!=width||W(p+8)<frames)throw new InvalidDataException("Animation adaptive delta header differs.");
                int blocks=(frames+15)/16,size=bits==4?9:17;if(W(p+36)!=blocks*width*size)throw new InvalidDataException("Animation adaptive delta block count differs.");Range(init,width*4);Range(data,blocks*width*size);
                var state=Enumerable.Range(0,width).Select(j=>F(init+j*4)).ToArray();if(frames>0)decoded[0]=(float[])state.Clone();
                for(int block=0;block<blocks;block++)for(int j=0;j<width;j++){
                    int at=data+(block*width+j)*size;byte filter=b[at];float step=filter<16?MathF.Pow(10,filter-8):(float)(1-Math.Sin(Math.PI/2*(filter-16)/240.0));step*=scale;if(bits==8)step/=16;
                    for(int k=0;k<16;k++){int delta=bits==8?(sbyte)b[at+1+k]:((b[at+1+k/2]>>(k%2*4))&15);if(bits==4&&delta>=8)delta-=16;state[j]+=delta*step;int frame=1+block*16+k;if(frame<frames){decoded[frame]??=new float[width];decoded[frame][j]=state[j];}}
                }
                var settings=Elements(source,"CompressionSettings").FirstOrDefault();tolerance=settings?.HasAttribute("MaxAdaptiveDeltaError")==true?XmlConvert.ToSingle(settings.GetAttribute("MaxAdaptiveDeltaError")):0.001f;
            }else throw new InvalidDataException("Unknown EP1 animation runtime channel class.");
            for(int k=0;k<frames;k++){
                if(binary[k]!=(k>=c.First&&k<c.First+c.Binary.Length&&c.Binary[k-c.First]))throw new InvalidDataException("Animation BinaryMove boundary differs.");
                for(int j=0;j<width;j++)if(!float.IsFinite(decoded[k][j])||MathF.Abs(decoded[k][j]-expected[k][j])>tolerance+2e-5f+MathF.Abs(expected[k][j])*2e-6f)throw new InvalidDataException("Animation playback differs at channel "+i+", frame "+k+", component "+j);
            }
        }
        foreach(var pair in new[]{(buffer.RelocationData,false),(buffer.ImportsData,true)}){
            var aux=pair.Item1;if(aux.Length==0)continue;if(aux.Length%4!=0||BinaryPrimitives.ReadUInt32LittleEndian(aux.AsSpan(aux.Length-4))!=uint.MaxValue)throw new InvalidDataException("Animation auxiliary framing differs.");
            for(int i=0;i<aux.Length-4;i+=4){int slot=checked((int)BinaryPrimitives.ReadUInt32LittleEndian(aux.AsSpan(i)));if(pair.Item2){if(slot!=4||W(slot)==0)throw new InvalidDataException("Animation hierarchy import slot differs.");}else{int target=checked((int)W(slot));Range(target,0);}}
        }
    }

    // Reborn: reconstruct the same source defaults without borrowing the native producer's buffers.
    private static float[][] Expand(Channel c,int count)=>Enumerable.Range(0,count).Select(i=>i>=c.First&&i<c.First+c.Values.Length?c.Values[i-c.First]:c.Type==3?new[]{0f,0f,0f,1f}:new[]{c.Type==4?1f:0f}).ToArray();
    // Reborn: schema validation defines the family; semantic validation also prevents scalar/orientation mismatches.
    private static Channel Read(XmlElement node)
    {
        int type=Array.IndexOf(new[]{"XTranslation","YTranslation","ZTranslation","Orientation","Visibility"},node.GetAttribute("Type"));
        if(type<0||(node.LocalName=="ChannelQuaternion")!=(type==3))throw new InvalidDataException("Animation channel family and Type are incompatible.");
        var frames=node.ChildNodes.OfType<XmlElement>().ToArray();var values=frames.Select(f=>type==3?new[]{"X","Y","Z","W"}.Select(a=>f.HasAttribute(a)?XmlConvert.ToSingle(f.GetAttribute(a)):0f).ToArray():new[]{XmlConvert.ToSingle(f.InnerText)}).ToArray();
        return new(Number(node,"Pivot"),type,Number(node,"FirstFrame"),values,frames.Select(f=>f.HasAttribute("BinaryMove")&&XmlConvert.ToBoolean(f.GetAttribute("BinaryMove"))).ToArray());
    }
    // Reborn: absent unsigned source attributes have the marshaler's zero default.
    private static int Number(XmlElement node,string name)=>node.HasAttribute(name)?checked((int)XmlConvert.ToUInt32(node.GetAttribute(name))):0;
    // Reborn: namespace-scoped child enumeration excludes foreign metadata from native input.
    private static IEnumerable<XmlElement> Elements(XmlElement node,string name)=>node.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName==name&&e.NamespaceURI==SchemaSet.XmlNamespace);
}
