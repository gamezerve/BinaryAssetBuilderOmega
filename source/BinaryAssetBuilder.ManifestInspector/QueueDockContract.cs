using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: QueueDock strings and enum are source-bound, while independent stock tokens establish ABI offsets and inheritance metadata.
internal static class QueueDockContract
{
    internal static readonly string[] Names={"DockStartBoneName","DockActionBoneName","DockEndBoneName","QueueBonePrefix","TurnTableBoneName"};

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject unrepresentable bone text at the authored field instead of replacing Unicode or embedded terminators. */
    //-------------------------------------------------------------------------------------------------
    internal static void Validate(XmlElement owner)
    {
        foreach(string name in Names)if(owner.GetAttribute(name).Any(c=>c>127||c=='\0'))throw SdkXmlDiagnostics.At(new InvalidDataException("QueueDock bone text must be ASCII without NUL: "+name),owner.GetAttributeNode(name));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: remove only comparison-copy bone values; the actual source retains defaults, empty text and authored spelling. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlElement actual,XmlElement model){foreach(string name in Names){actual.RemoveAttribute(name);if(model.HasAttribute(name))actual.SetAttribute(name,model.GetAttribute(name));}}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: use the stock refinery's field/type/native anchors and actual text to qualify the new struct rather than inferring layout from XML order alone. */
    //-------------------------------------------------------------------------------------------------
    internal static bool QualifyStock(string manifest)
    {
        var captured=GameObjectStockReview.Capture(manifest,"GameObject:SovietRefinery");var module=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.Type=="QueueDockModuleData");
        var b=captured.Bytes;int table=(int)Word(b,module.Wrapper+8);var fields=GameObjectStockReview.Tokens(b,table);
        if(Marshal.SizeOf<QueueDockModuleData>()!=52||module.TypeId!=0xAF4C1F21u||fields.Count!=7||Word(b,table)!=1||!module.ClassChain.SequenceEqual(new[]{"QueueDockModuleData","CreateModuleData","BehaviorModuleData","ModuleData"}))throw new InvalidDataException("QueueDock stock header/inheritance shape differs.");
        string[] expected={"DockStart","DockAction","DockEnd","Que","BONE_TABLE"};
        for(int i=0;i<Names.Length;i++){
            int offset=8+i*8;if(Marshal.OffsetOf<QueueDockModuleData>(Names[i]).ToInt32()!=offset||!fields.TryGetValue(FastHash.GetHashCode(Names[i]),out var field)||field.Type!=0x1A2933AAu||field.Offset!=module.Native+offset)throw new InvalidDataException("QueueDock stock string field differs: "+Names[i]);
            uint length=Word(b,module.Native+offset),pointer=Word(b,module.Native+offset+4);byte[] value=Encoding.ASCII.GetBytes(expected[i]);if(length!=value.Length||pointer>b.Length-value.Length-1||!b.AsSpan((int)pointer,value.Length).SequenceEqual(value)||b[(int)pointer+value.Length]!=0)throw new InvalidDataException("QueueDock stock bone text differs.");
        }
        var animation=fields[FastHash.GetHashCode("DockingAnimation")];if(animation.Type!=0x15723E0Bu||animation.Offset!=module.Native+48||Word(b,module.Native+48)!=(uint)ModelConditionFlagType.DOCKING_ACTIVE)throw new InvalidDataException("QueueDock stock animation differs.");
        // Reborn: all 192 literal metadata bytes are rebased from their independent stock coordinates before generic metadata can use them.
        var expectedTokens=Metadata(192,0);var stockTokens=b.AsSpan(module.Wrapper,192).ToArray();for(int at=8;at<192;at+=12){uint target=Word(stockTokens,at);if(target!=0)BinaryPrimitives.WriteUInt32LittleEndian(stockTokens.AsSpan(at,4),target>=module.Native?target-(uint)module.Native+192:target-(uint)module.Wrapper);}
        if(!stockTokens.SequenceEqual(expectedTokens))throw new InvalidDataException("QueueDock literal metadata differs from stock.");return true;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove all native bytes, five canonical string allocations, enum value and exact RELO ownership from authored source. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlElement owner,Chunk native,Chunk tokens)
    {
        Validate(owner);var b=native.InstanceBuffer;if(b.Length<52||Word(b,0)!=0xAF4C1F21u||Word(b,4)!=FastHash.GetHashCode(owner.GetAttribute("id").ToLowerInvariant())||native.ImportsBuffer.Length!=0)throw new InvalidDataException("QueueDock native identity/import framing differs.");
        foreach(var binding in ConscriptModuleScalarContract.Bindings(typeof(QueueDockModuleData)))if(!b.AsSpan(binding.Offset,binding.Width).SequenceEqual(ConscriptModuleScalarContract.Bytes(owner,binding)))throw new InvalidDataException("QueueDock docking enum differs from source.");
        var covered=new bool[b.Length];Array.Fill(covered,true,0,52);var slots=new List<uint>();
        for(int i=0;i<Names.Length;i++){
            int at=8+i*8;byte[] value=Encoding.ASCII.GetBytes(owner.GetAttribute(Names[i]));uint pointer=Word(b,at+4);
            if(Word(b,at)!=value.Length||pointer<52||pointer>b.Length-value.Length-1||covered.AsSpan((int)pointer,value.Length+1).Contains(true)||!b.AsSpan((int)pointer,value.Length).SequenceEqual(value)||b[(int)pointer+value.Length]!=0)throw new InvalidDataException("QueueDock bone header/text/ownership differs: "+Names[i]);
            Array.Fill(covered,true,(int)pointer,value.Length+1);slots.Add((uint)(at+4));
        }
        int gap=0;for(int at=0;at<b.Length;at++){if(covered[at]){gap=0;continue;}if(b[at]!=0||++gap>3)throw new InvalidDataException("Unowned QueueDock native bytes.");}
        var expected=slots.Append(uint.MaxValue).ToArray();if(native.RelocationBuffer.Length!=expected.Length*4||Enumerable.Range(0,expected.Length).Any(i=>Word(native.RelocationBuffer,i*4)!=expected[i]))throw new InvalidDataException("QueueDock relocation ownership differs.");
        if(tokens.InstanceBuffer.Length-b.Length!=192||!tokens.InstanceBuffer.AsSpan(0,192).SequenceEqual(Metadata(192,0)))throw new InvalidDataException("QueueDock generated/literal metadata differ.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: the independently stock-qualified literal writer contains only six own fields and the three base tables. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Metadata(int prefix,int native)
    {
        var b=new byte[192];Row(0,2,0xAF4C1F21u,12);Row(12,1,FastHash.GetHashCode("CreateModuleData"),108);
        for(int i=0;i<Names.Length;i++)Row(24+i*12,FastHash.GetHashCode(Names[i]),0x1A2933AAu,(uint)(prefix+native+8+i*8));
        Row(84,FastHash.GetHashCode("DockingAnimation"),0x15723E0Bu,(uint)(prefix+native+48));Row(108,1,FastHash.GetHashCode("BehaviorModuleData"),132);Row(132,1,FastHash.GetHashCode("ModuleData"),156);Row(156,FastHash.GetHashCode("TypeId"),0x23E650B8u,(uint)(prefix+native));Row(168,FastHash.GetHashCode("id"),0xBDF07965u,(uint)(prefix+native+4));return b;
        // Reborn: token targets are metadata offsets or absolute native words, never relocation slots.
        void Row(int at,uint name,uint type,uint target){BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),name);BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at+4,4),type);BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at+8,4),target);}
    }
    // Reborn: all callers establish native/table bounds before reading fixed-size words.
    private static uint Word(byte[] b,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));
}
