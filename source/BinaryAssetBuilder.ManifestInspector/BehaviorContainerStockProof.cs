using System.Buffers.Binary;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: independently qualify complete shared behavior metadata, parent ownership and every original leaf against stock and one semantic Core occurrence table.
internal static class BehaviorContainerStockProof
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: literal stock container rows precede independently field-context-checked leaves; the producer receives none of these oracle coordinates. */
    //-------------------------------------------------------------------------------------------------
    internal static (int Prefix,int Refusals) Run(Chunk native,System.Xml.Schema.XmlSchemaSet schemas,GameObjectModuleReview.Module[] modules,byte[] stock,AssetId[] references,InstanceDeclaration instance,Func<uint,string,Type> model,Chunk? observed=null,bool controls=true)
    {
        // Reborn: enclosing root tests may supply detached observed metadata; the same independent stock oracle must accept that actual output before any producer repeat comparison.
        var before=Clone(native);var tokens=observed??Ep1BehaviorMetadataTokenizer.TokenizeContainer(native,schemas);int prefix=Verify(tokens);if(!Equal(before,native) || !Equal(tokens,Ep1BehaviorMetadataTokenizer.TokenizeContainer(native,schemas)))throw new InvalidDataException("Shared behavior metadata repeat/input isolation differs.");if(!controls)return(prefix,0);int refusals=0;
        foreach(int at in new[]{0,4,8,12,16,20,prefix,prefix+4}){var bad=Clone(tokens);bad.InstanceBuffer[at]^=1;Refuse(()=>Verify(bad));refusals++;}
        for(int i=0;i<modules.Length;i++){int wrapper=checked((int)Word(tokens.InstanceBuffer,20+i*12));foreach(int at in new[]{20+i*12,wrapper+4,wrapper+8,prefix+(int)Word(native.InstanceBuffer,8+i*4)+4}){var bad=Clone(tokens);bad.InstanceBuffer[at]^=1;Refuse(()=>Verify(bad));refusals++;}}
        foreach(bool relo in new[]{true,false}){var bad=Clone(tokens);if(relo)bad.RelocationBuffer=Array.Empty<byte>();else bad.ImportsBuffer=Array.Empty<byte>();Refuse(()=>Verify(bad));refusals++;}
        // Reborn: production controls reject broken parent framing, duplicate identities, unknown classes, cross-owner pointers and unowned storage before publishing metadata.
        int first=(int)Word(native.InstanceBuffer,8);foreach(var mutation in new (int At,uint Value)[]{(0,0),(0,65),(4,12),(8,0),(12,(uint)first),(first,0),(first+4,0),((int)Word(native.InstanceBuffer,12)+4,Word(native.InstanceBuffer,first+4))}){var bad=Clone(native);Put(bad.InstanceBuffer,mutation.At,mutation.Value);Refuse(()=>Ep1BehaviorMetadataTokenizer.TokenizeContainer(bad,schemas));refusals++;}
        uint inner=Slots(native.RelocationBuffer).First(s=>s>=first);var crossed=Clone(native);Put(crossed.InstanceBuffer,(int)inner,8);Refuse(()=>Ep1BehaviorMetadataTokenizer.TokenizeContainer(crossed,schemas));refusals++;
        var orphan=Clone(native);orphan.InstanceBuffer=orphan.InstanceBuffer.Concat(new byte[]{0xA5,0,0,0}).ToArray();Refuse(()=>Ep1BehaviorMetadataTokenizer.TokenizeContainer(orphan,schemas));refusals++;
        foreach(bool relo in new[]{true,false}){var bad=Clone(native);if(relo)bad.RelocationBuffer=Array.Empty<byte>();else bad.ImportsBuffer=Array.Empty<byte>();Refuse(()=>Ep1BehaviorMetadataTokenizer.TokenizeContainer(bad,schemas));refusals++;}
        return(prefix,refusals);
        //-------------------------------------------------------------------------------------------------
        /** Reborn: validate the complete shared tail first, then detach metadata/native owners solely for reuse of the independent per-field stock oracle. */
        //-------------------------------------------------------------------------------------------------
        int Verify(Chunk output)
        {
            int shift=output.InstanceBuffer.Length-native.InstanceBuffer.Length;int table=GameObjectStockReview.Tokens(stock,0)[FastHash.GetHashCode("Behaviors")].Offset,end=GameObjectStockReview.Tokens(stock,0)[FastHash.GetHashCode("AI")].Offset;int count=modules.Length;
            if(shift!=end-table || shift<12+count*12 || Word(stock,table)!=0x6C827073 || Word(stock,table+4)!=0x5D448FD8 || Word(stock,(int)Word(stock,table+8))!=count || Word(output.InstanceBuffer,0)!=0x6C827073 || Word(output.InstanceBuffer,4)!=0x5D448FD8 || Word(output.InstanceBuffer,8)!=shift)throw new InvalidDataException("Shared behavior stock count metadata differs.");
            VerifyTail(output,native,shift);int next=12+count*12;
            for(int i=0;i<count;i++)
            {
                int marker=12+i*12,wrapper=modules[i].Wrapper,stockEnd=i+1<count?modules[i+1].Wrapper:end,length=stockEnd-wrapper;if(Word(stock,table+marker)!=0xB151A0EB || Word(stock,table+marker+4)!=0 || Word(stock,table+marker+8)!=wrapper || wrapper-table!=next || Word(output.InstanceBuffer,marker)!=0xB151A0EB || Word(output.InstanceBuffer,marker+4)!=0 || Word(output.InstanceBuffer,marker+8)!=next)throw new InvalidDataException("Shared behavior ordered stock marker differs.");
                int start=(int)Word(native.InstanceBuffer,8+i*4),limit=i+1<count?(int)Word(native.InstanceBuffer,12+i*4):native.InstanceBuffer.Length;var leaf=Extract(native,start,limit);var comparer=new SelectedNativeGraphComparer(stock,references,leaf,instance,requireAllReferences:false,captureNativeCoordinates:false,captureNativePaths:true);Type type=model(modules[i].TypeId,modules[i].Type);comparer.Compare(type,modules[i].Native);
                var detached=new Chunk{InstanceBuffer=new byte[length+limit-start],RelocationBuffer=Pack(Slots(leaf.RelocationBuffer).Select(s=>s+(uint)length)),ImportsBuffer=Pack(Slots(leaf.ImportsBuffer).Select(s=>s+(uint)length))};output.InstanceBuffer.AsSpan(next,length).CopyTo(detached.InstanceBuffer);output.InstanceBuffer.AsSpan(shift+start,limit-start).CopyTo(detached.InstanceBuffer.AsSpan(length));
                for(int at=8;at<length;at+=12){uint target=Word(detached.InstanceBuffer,at);if(target==0)continue;if(target<shift){if(target<next || target>=next+length)throw new InvalidDataException("Shared behavior metadata crosses sibling tables.");Put(detached.InstanceBuffer,at,target-(uint)next);}else{if(target<shift+start || target>=shift+limit)throw new InvalidDataException("Shared behavior metadata crosses native leaf owners.");Put(detached.InstanceBuffer,at,target-(uint)(shift+start)+(uint)length);}}
                foreach(uint slot in Slots(leaf.RelocationBuffer)){uint target=Word(detached.InstanceBuffer,length+(int)slot);if(target<shift+start || target>=shift+limit)throw new InvalidDataException("Shared behavior detached pointer ownership differs.");Put(detached.InstanceBuffer,length+(int)slot,target-(uint)(shift+start)+(uint)length);}BehaviorMetadataStockProof.Compare(detached,leaf,stock,wrapper,stockEnd,type,comparer.NativeRecords);next+=length;
            }
            if(next!=shift)throw new InvalidDataException("Shared behavior metadata has an unowned suffix.");return shift;
        }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently inverse-check every native byte and exact auxiliary order in a generated shared graph, including unchanged Core occurrence values. */
    //-------------------------------------------------------------------------------------------------
    internal static void VerifyTail(Chunk output,Chunk native,int prefix)
    {if(prefix<0 || output.InstanceBuffer.Length!=prefix+native.InstanceBuffer.Length)throw new InvalidDataException("Shared behavior output bounds differ.");var expected=native.InstanceBuffer.ToArray();foreach(uint slot in Slots(native.RelocationBuffer))Put(expected,(int)slot,checked(Word(expected,(int)slot)+(uint)prefix));if(!output.InstanceBuffer.AsSpan(prefix).SequenceEqual(expected) || !Slots(output.RelocationBuffer).SequenceEqual(Slots(native.RelocationBuffer).Select(s=>s+(uint)prefix)) || !Slots(output.ImportsBuffer).SequenceEqual(Slots(native.ImportsBuffer).Select(s=>s+(uint)prefix)))throw new InvalidDataException("Shared behavior native tail/auxiliary transformation differs.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: leaf extraction retains common Core indices but refuses any pointer escaping its native owner. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Extract(Chunk shared,int start,int end){var leaf=new Chunk{InstanceBuffer=shared.InstanceBuffer.AsSpan(start,end-start).ToArray(),RelocationBuffer=Pack(Slots(shared.RelocationBuffer).Where(s=>s>=start && s<end).Select(s=>s-(uint)start)),ImportsBuffer=Pack(Slots(shared.ImportsBuffer).Where(s=>s>=start && s<end).Select(s=>s-(uint)start))};foreach(uint slot in Slots(leaf.RelocationBuffer)){uint target=Word(leaf.InstanceBuffer,(int)slot);if(target<start || target>=end)throw new InvalidDataException("Shared proof native pointer crosses owners.");Put(leaf.InstanceBuffer,(int)slot,target-(uint)start);}return leaf;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: deliberate corruption must throw a bounded data refusal, never merely return a nonmatching diagnostic. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action){try{action();}catch(InvalidDataException){return;}throw new InvalidDataException("Corrupt shared behavior metadata accepted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: controls and deterministic comparisons use owned buffers only. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk value)=>new(){InstanceBuffer=value.InstanceBuffer.ToArray(),RelocationBuffer=value.RelocationBuffer.ToArray(),ImportsBuffer=value.ImportsBuffer.ToArray()};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: shared determinism includes all three generated buffers. */
    //-------------------------------------------------------------------------------------------------
    private static bool Equal(Chunk a,Chunk b)=>a.InstanceBuffer.SequenceEqual(b.InstanceBuffer) && a.RelocationBuffer.SequenceEqual(b.RelocationBuffer) && a.ImportsBuffer.SequenceEqual(b.ImportsBuffer);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: auxiliary framing, uniqueness and alignment are verified independently of the writer. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b){if(b.Length==0)return Array.Empty<uint>();if(b.Length<8 || b.Length%4!=0 || Word(b,b.Length-4)!=uint.MaxValue)throw new InvalidDataException("Shared proof auxiliary framing differs.");var values=Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();if(values.Distinct().Count()!=values.Length || values.Any(s=>s%4!=0))throw new InvalidDataException("Shared proof auxiliary slots differ.");return values;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: detached auxiliary arrays retain original order and nonempty sentinels. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> source){var values=source.ToArray();if(values.Length==0)return Array.Empty<byte>();var result=new byte[(values.Length+1)*4];for(int i=0;i<values.Length;i++)Put(result,i*4,values[i]);Put(result,values.Length*4,uint.MaxValue);return result;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: metadata and native oracle reads stay in their bounded admitted buffers. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at){if(at<0 || at>b.Length-4)throw new InvalidDataException("Shared proof word bounds differ.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: patches affect generated or detached owned buffers only. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint value){if(at<0 || at>b.Length-4)throw new InvalidDataException("Shared proof write bounds differ.");BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);}
}
