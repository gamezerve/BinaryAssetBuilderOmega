using System.Buffers.Binary;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: exercise actual Core normalization, runtime compression and independent playback decoding without a game launch.
internal static class W3DAnimationSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: cover translation/quaternion/visibility channels, partial spans, steps, compression settings and corrupt output. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest)
    {
        Settings.Current=new Settings{SchemaPath=ReferencePipelineSmokeTest.FindFixture(),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
        var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<System.Xml.Schema.XmlSchema>().ToArray())schemas.Schemas.Remove(seed);
        schemas.Schemas.Add(ConscriptSourceInput.Schemas());schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.NonPublic|BindingFlags.Instance)!.Invoke(schemas,null);
        int checks=0;var families=new HashSet<uint>();
        foreach(int frames in new[]{1,2,17,33,257})foreach(bool binary in new[]{false,true})foreach(string settings in new[]{"","<CompressionSettings AllowAdaptiveDelta='false'/>","<CompressionSettings AllowTimeCoded='false'/>","<CompressionSettings AllowTimeCoded='false' AllowAdaptiveDelta='false'/>"}){
            string xml=Source(frames,binary,settings);var instance=ReferencePipelineSmokeTest.Normalize(schemas,xml);var buffer=W3DAnimationCompiler.Compile(instance);checks++;
            var b=buffer.InstanceData;uint W(int at)=>BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));int list=(int)W(20);for(int i=0;i<W(16);i++)families.Add(W((int)W(list+i*4)));
            var repeat=W3DAnimationCompiler.Compile(ReferencePipelineSmokeTest.Normalize(schemas,xml));
            if(!buffer.InstanceData.SequenceEqual(repeat.InstanceData)||!buffer.RelocationData.SequenceEqual(repeat.RelocationData)||!buffer.ImportsData.SequenceEqual(repeat.ImportsData))throw new InvalidDataException("Animation output is nondeterministic.");checks++;
            byte original=b[8];b[8]^=1;Refuse(()=>W3DAnimationCompiler.Verify((XmlElement)instance.XmlNode,buffer));b[8]=original;checks++;
        }
        foreach(string source in new[]{Source(3,false,"").Replace("FirstFrame='0'","FirstFrame='3'"),Source(3,false,"").Replace("Type='XTranslation'","Type='Orientation'"),Source(3,false,"").Replace("0.01","NaN")}){
            var xml=new XmlDocument{XmlResolver=null};xml.LoadXml(source);Refuse(()=>W3DAnimationCompiler.Admit(xml.DocumentElement!));checks++;
        }
        // Reborn: XML boolean numerals and an empty animation with no hierarchy are valid optional/default controls.
        foreach(string xml in new[]{Source(17,true,"<CompressionSettings AllowAdaptiveDelta='0'/>").Replace("BinaryMove='true'","BinaryMove='1'"),"<W3DAnimation xmlns='uri:ea.com:eala:asset' id='RebornEmptyAnimation'><Channels/></W3DAnimation>"}){
            W3DAnimationCompiler.Compile(ReferencePipelineSmokeTest.Normalize(schemas,xml));checks++;
        }
        // Reborn: prove the last legal timestamp plus BinaryMove bit, and reject real timecode overflow without a blanket source-frame cap.
        string boundary="<W3DAnimation xmlns='uri:ea.com:eala:asset' id='RebornFrameBoundary' NumFrames='32768' FrameRate='30'><CompressionSettings AllowAdaptiveDelta='false'/><Channels><ChannelScalar Pivot='0' Type='XTranslation' FirstFrame='0'>"+string.Concat(Enumerable.Repeat("<Frame>0</Frame>",32767))+"<Frame BinaryMove='true'>1</Frame></ChannelScalar></Channels></W3DAnimation>";
        W3DAnimationCompiler.Compile(ReferencePipelineSmokeTest.Normalize(schemas,boundary));checks++;
        Refuse(()=>W3DAnimationCompiler.Compile(ReferencePipelineSmokeTest.Normalize(schemas,boundary.Replace("NumFrames='32768'","NumFrames='32769'"))));checks++;
        // Reborn: both writer families must actually run; merely accepting the source whitelist is insufficient.
        if(!families.SetEquals(new[]{0x60D8CF69u,0x77F97305u}))throw new InvalidDataException("Animation tests did not exercise both runtime encodings.");
        Qualify(manifest);Console.WriteLine("Reborn: EP1 animation proof: PASS ("+checks+" real-dispatch, deterministic playback, partial-span, BinaryMove and corruption checks; both encodings; original stock ABI).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independent original records establish the 24-byte root and scalar/quaternion timecoded/delta list contracts. */
    //-------------------------------------------------------------------------------------------------
    private static void Qualify(string manifest)
    {
        if(System.Runtime.InteropServices.Marshal.SizeOf<W3DAnimationRuntime>()!=24||System.Runtime.InteropServices.Marshal.SizeOf<AnimationChannelTimecoded>()!=36||System.Runtime.InteropServices.Marshal.SizeOf<AnimationChannelDelta>()!=44)throw new InvalidDataException("Animation native struct sizes differ from EP1.");
        var doc=ManifestReader.Read(SdkEnvironmentPreflight.Read(manifest,64*1048576));var found=new HashSet<(uint,uint)>();
        foreach(var asset in doc.Assets.Where(a=>a.TypeName=="W3DAnimation"&&a.InstanceDataSize<100000)){
            var b=W3DContainerCompiler.ReadStock(manifest,doc,asset);uint W(int at)=>BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));
            if(asset.Tokenized!=0||W(0)!=0||W(4)!=1||asset.References.Count!=1)throw new InvalidDataException("Stock animation root/import ABI differs.");
            int list=(int)W(20);for(int i=0;i<W(16);i++){
                int at=(int)W(list+i*4);uint type=W(at),width=W(at+12);if(type is not (0x60D8CF69 or 0x77F97305)||width is not (1 or 4))throw new InvalidDataException("Stock animation channel ABI differs.");
                if(type==0x60D8CF69&&(W(at+20)!=W(at+8)||W(at+28)!=W(at+8)*width))throw new InvalidDataException("Stock animation timecode/vector counts differ.");
                if(type==0x77F97305&&(W(at+24)!=width||W(at+32) is not (4 or 8)))throw new InvalidDataException("Stock animation delta/vector layout differs.");found.Add((type,width));
            }
            if(found.Count==4)return;
        }
        throw new InvalidDataException("Original EP1 animation witnesses missing.");
    }

    // Reborn: build source samples with nonconstant scalar/rotation values and a shorter visibility span with defaults on both sides.
    private static string Source(int frames,bool binary,string settings)
    {
        var s=new StringBuilder("<W3DAnimation xmlns='uri:ea.com:eala:asset' id='RebornAnimationProof' Hierarchy='W3DHierarchy:RebornHierarchy' NumFrames='"+frames+"' FrameRate='30'>"+settings+"<Channels>");
        foreach(string type in new[]{"XTranslation","YTranslation","ZTranslation","Visibility","Orientation"}){
            bool quat=type=="Orientation";int first=type=="Visibility"&&frames>2?1:0,count=frames-first-(first==0?0:1);s.Append('<').Append(quat?"ChannelQuaternion":"ChannelScalar").Append(" Pivot='1' Type='").Append(type).Append("' FirstFrame='").Append(first).Append("'>");
            for(int i=0;i<count;i++){
                string flag=binary&&i==count/2?" BinaryMove='true'":"";
                if(quat)s.Append("<Frame X='0' Y='0' Z='").Append(Math.Sin(i*0.01).ToString("R",CultureInfo.InvariantCulture)).Append("' W='").Append(Math.Cos(i*0.01).ToString("R",CultureInfo.InvariantCulture)).Append('\'').Append(flag).Append("/>");
                else s.Append("<Frame").Append(flag).Append('>').Append((type=="Visibility"?i%2:i*0.01).ToString("R",CultureInfo.InvariantCulture)).Append("</Frame>");
            }
            s.Append("</").Append(quat?"ChannelQuaternion":"ChannelScalar").Append('>');
        }
        return s.Append("</Channels></W3DAnimation>").ToString();
    }
    // Reborn: negative controls must be rejected for their semantic error, never counted as successful builds.
    private static void Refuse(Action action){try{action();}catch(InvalidDataException){return;}throw new InvalidDataException("Invalid animation control was accepted.");}
}
