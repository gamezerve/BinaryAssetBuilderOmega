using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.XmlCompiler;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: independently bind EP1 spawn scalars, weak lists and optional records to source, with disjoint native allocation ownership.
internal static class SpawnBehaviorContract
{
    internal static readonly string[] OwnChildren={"Die","SpawnTemplate","ObjectStatusValidation"};

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate nested records before native allocation so invalid flags/numerics retain authored file and line diagnostics. */
    //-------------------------------------------------------------------------------------------------
    internal static void Validate(XmlElement owner)
    {
        foreach(var child in owner.ChildNodes.OfType<XmlElement>()){
            if(child.LocalName=="ObjectStatusValidation")foreach(var field in ConscriptModuleScalarContract.RecordBindings(typeof(ObjectStatusValidationDataType)))_=ConscriptModuleScalarContract.Bytes(child,field);
            if(child.LocalName=="Die"){
                foreach(var field in ConscriptModuleScalarContract.RecordBindings(typeof(DieMuxDataType)))_=ConscriptModuleScalarContract.Bytes(child,field);
                foreach(string name in new[]{"ExemptStatus","RequiredStatus"})if(child.HasAttribute(name))_=Status(child,name);
                foreach(string value in Words(child.GetAttribute("VeterancyLevels")))if(value.Any(c=>c>127||c=='\0'))throw SdkXmlDiagnostics.At(new InvalidDataException("Spawn Die veterancy labels must be ASCII without NUL."),child.GetAttributeNode("VeterancyLevels"));
            }
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: remove variable own children in comparison copies only; real source values/counts are consumed by Verify. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlElement actual,XmlElement model)
    {
        foreach(var child in actual.ChildNodes.OfType<XmlElement>().Where(e=>OwnChildren.Contains(e.LocalName)).ToArray())actual.RemoveChild(child);
        foreach(var child in model.ChildNodes.OfType<XmlElement>().Where(e=>OwnChildren.Contains(e.LocalName)))actual.AppendChild(actual.OwnerDocument!.ImportNode(child,true));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare the complete stock refinery Spawn metadata and native source values, not just a type-name admission entry. */
    //-------------------------------------------------------------------------------------------------
    internal static bool QualifyStock(string manifest,System.Xml.Schema.XmlSchemaSet schemas,ManifestAsset[] catalog)
    {
        var captured=GameObjectStockReview.Capture(manifest,"GameObject:SovietRefinery");var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules;
        var module=modules.Single(m=>m.Type=="SpawnBehaviorModuleData");var next=modules[Array.IndexOf(modules,module)+1];
        if(Marshal.SizeOf<Ep1SpawnBehaviorModuleData>()!=96||next.Native-module.Native!=100||next.Wrapper-module.Wrapper!=564||module.TypeId!=0x0D916ADFu||!module.ClassChain.SequenceEqual(new[]{"SpawnBehaviorModuleData","UpgradeModuleData","BehaviorModuleData","ModuleData"}))throw new InvalidDataException("EP1 Spawn stock ABI differs.");
        var native=new Chunk{InstanceBuffer=captured.Bytes.AsSpan(module.Native,100).ToArray(),RelocationBuffer=new byte[8],ImportsBuffer=Array.Empty<byte>()};
        Put(native.InstanceBuffer,72,Word(native.InstanceBuffer,72)-(uint)module.Native);Put(native.RelocationBuffer,0,72);Put(native.RelocationBuffer,4,uint.MaxValue);
        var sample=new XmlDocument();sample.LoadXml("<SpawnBehavior id='ModuleTag_Spawn' SpawnNumberData='1' SpawnReplaceDelayData='9999s' IsOneShotData='true' FadeInTime='1s' SpawnOnUnpack='true' SetPurchasePrice='true'><SpawnTemplate>SovietMiner</SpawnTemplate></SpawnBehavior>");
        Verify(sample.DocumentElement!,native,catalog);
        var generated=Ep1BehaviorMetadataTokenizer.Tokenize(native,schemas,"SpawnBehaviorModuleData",typeof(Ep1SpawnBehaviorModuleData));int prefix=generated.InstanceBuffer.Length-native.InstanceBuffer.Length;
        if(prefix!=564)throw new InvalidDataException("Spawn metadata prefix differs from stock.");
        for(int at=0;at<prefix;at+=12){uint target=Word(captured.Bytes,module.Wrapper+at+8);if(target!=0)target=target>=module.Native?target-(uint)module.Native+(uint)prefix:target-(uint)module.Wrapper;
            if(Word(generated.InstanceBuffer,at)!=Word(captured.Bytes,module.Wrapper+at)||Word(generated.InstanceBuffer,at+4)!=Word(captured.Bytes,module.Wrapper+at+4)||Word(generated.InstanceBuffer,at+8)!=target)throw new InvalidDataException("Spawn stock field/table target differs at metadata +"+at);
        }
        return true;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove every scalar, source-sized list, optional record, pointer and padding byte without freezing authored counts/values to stock. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlElement owner,Chunk native,ManifestAsset[] catalog)
    {
        Validate(owner);var b=native.InstanceBuffer;if(b.Length<96||Word(b,0)!=0x0D916ADFu||Word(b,4)!=Weak(owner.GetAttribute("id")))throw new InvalidDataException("Spawn native identity/header differs.");
        var covered=new bool[b.Length];var relos=new HashSet<uint>();Claim(0,96);foreach(int offset in new[]{33,34,35})if(b[offset]!=0)throw new InvalidDataException("Spawn upgrade padding differs.");
        foreach(var field in ConscriptModuleScalarContract.Bindings(typeof(Ep1SpawnBehaviorModuleData)))Check(owner,field,0);
        var upgrade=ConscriptUpgradeStructure.Read(new("Behaviors",0,owner,typeof(Ep1SpawnBehaviorModuleData)),catalog)!;
        foreach(var field in new[]{("TriggeredBy",12),("ConflictsWith",20)})List(field.Item2,upgrade.Lists.Single(p=>p.Key.EndsWith("."+field.Item1,StringComparison.Ordinal)).Value);
        var animation=upgrade.Optional.Values.Single();int anim=Pointer(8,animation!=null,12);if(animation!=null&&!b.AsSpan(anim,12).SequenceEqual(animation))throw new InvalidDataException("Spawn upgrade animation differs.");
        List(68,owner.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="SpawnTemplate").Select(e=>Weak(e.InnerText.Trim())).ToArray());
        var status=owner.ChildNodes.OfType<XmlElement>().SingleOrDefault(e=>e.LocalName=="ObjectStatusValidation");int validation=Pointer(76,status!=null,64);if(status!=null)foreach(var field in ConscriptModuleScalarContract.RecordBindings(typeof(ObjectStatusValidationDataType)))Check(status,field,validation);
        var die=owner.ChildNodes.OfType<XmlElement>().SingleOrDefault(e=>e.LocalName=="Die");int death=Pointer(64,die!=null,Marshal.SizeOf<DieMuxDataType>());
        if(die!=null){
            foreach(var field in ConscriptModuleScalarContract.RecordBindings(typeof(DieMuxDataType)))Check(die,field,death);
            foreach(var field in new[]{("ExemptStatus",8),("RequiredStatus",12)}){bool present=die.HasAttribute(field.Item1);int target=Pointer(death+field.Item2,present,32);if(present&&!b.AsSpan(target,32).SequenceEqual(Status(die,field.Item1)))throw new InvalidDataException("Spawn Die status differs.");}
            var labels=Words(die.GetAttribute("VeterancyLevels"));int array=Array(death,labels.Length,8);
            for(int i=0;i<labels.Length;i++){byte[] text=Encoding.ASCII.GetBytes(labels[i]);int at=array+i*8;if(Word(b,at)!=text.Length)throw new InvalidDataException("Spawn veterancy string length differs.");int target=Pointer(at+4,true,text.Length+1);if(!b.AsSpan(target,text.Length).SequenceEqual(text)||b[target+text.Length]!=0)throw new InvalidDataException("Spawn veterancy text differs.");}
        }
        int gap=0;for(int i=0;i<b.Length;i++){if(covered[i]){gap=0;continue;}if(b[i]!=0||++gap>3)throw new InvalidDataException("Spawn unowned native allocation.");}
        if(!Slots(native.RelocationBuffer).Order().SequenceEqual(relos.Order())||native.ImportsBuffer.Length!=0)throw new InvalidDataException("Spawn relocation/import inventory differs.");

        // Reborn: exact source codecs are independent of the marshaler's allocated values and preserve packed boolean widths.
        void Check(XmlElement element,ConscriptModuleScalarContract.Binding field,int start){if(!b.AsSpan(start+field.Offset,field.Width).SequenceEqual(ConscriptModuleScalarContract.Bytes(element,field)))throw new InvalidDataException("Spawn authored field differs: "+element.LocalName+"."+field.Name);}
        // Reborn: all owned heap records are bounded, disjoint and present exactly when authored; tokenization separately checks record alignment.
        int Pointer(int at,bool present,int size){uint target=Word(b,at);if(!present){if(target!=0)throw new InvalidDataException("Absent Spawn field has storage.");return 0;}if(target<96||target>int.MaxValue)throw new InvalidDataException("Spawn pointer differs.");Claim((int)target,size);relos.Add((uint)at);return (int)target;}
        // Reborn: zero lists have no pointer relocation; nonempty lists own their complete checked item extent.
        int Array(int at,int count,int stride){if(Word(b,at)!=count)throw new InvalidDataException("Spawn list count differs.");return Pointer(at+4,count!=0,checked(count*stride));}
        // Reborn: weak reference rows contain authored engine hashes rather than strong import selectors.
        void List(int at,uint[] expected){int items=Array(at,expected.Length,4);for(int i=0;i<expected.Length;i++)if(Word(b,items+i*4)!=expected[i])throw new InvalidDataException("Spawn weak list value differs.");}
        // Reborn: fixed header coverage never excuses overlaps in optional records or array/string payloads.
        void Claim(int at,int size){if(at<0||size<0||at>b.Length-size||covered.AsSpan(at,size).Contains(true))throw new InvalidDataException("Spawn native allocation bounds/overlap differs.");System.Array.Fill(covered,true,at,size);}
    }

    // Reborn: optional Die status pointers use the same native bit ordinal codec as inline object-status masks.
    private static byte[] Status(XmlElement element,string name)=>ConscriptModuleScalarContract.Bytes(element,new(name,0,32,"Flags:SageBinaryData.ObjectStatusType",null,0xA7BD811E));
    // Reborn: XML list whitespace separates veterancy labels without preserving indentation as string data.
    private static string[] Words(string text)=>text.Split((char[]?)null,StringSplitOptions.RemoveEmptyEntries);
    // Reborn: weak asset/local module IDs follow invariant lowercase POID hashing, not one-based Core references.
    private static uint Weak(string text)=>FastHash.GetHashCode(text.ToLowerInvariant());
    // Reborn: require the canonical relocation sentinel and unique slots before comparing the exact owned set.
    private static uint[] Slots(byte[] bytes){if(bytes.Length==0)return System.Array.Empty<uint>();if(bytes.Length<8||bytes.Length%4!=0||Word(bytes,bytes.Length-4)!=uint.MaxValue)throw new InvalidDataException("Spawn auxiliary terminator differs.");var slots=Enumerable.Range(0,bytes.Length/4-1).Select(i=>Word(bytes,i*4)).ToArray();if(slots.Distinct().Count()!=slots.Length)throw new InvalidDataException("Spawn duplicate relocation.");return slots;}
    // Reborn: callers bound each fixed header and owned record before word reads.
    private static uint Word(byte[] bytes,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));
    // Reborn: diagnostic stock rebasing only changes the copied slice, never the installed stream.
    private static void Put(byte[] bytes,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(at,4),value);
}
