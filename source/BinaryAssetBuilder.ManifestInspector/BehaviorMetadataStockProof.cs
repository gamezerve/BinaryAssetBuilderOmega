using System.Buffers.Binary;
using System.Reflection;
using BinaryAssetBuilder.Core.Hashing;
using Chunk = Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: independently validate emitted rows against stock tables and typed comparison paths, distinguishing identical shared stock fields by their logical owner.
internal static class BehaviorMetadataStockProof
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: every name/tag/table coordinate is a literal stock oracle; every native target must bind the matching independently walked native field path. */
    //-------------------------------------------------------------------------------------------------
    internal static int Compare(Chunk tokens,Chunk native,byte[] stock,int wrapper,int end,Type type,IReadOnlyDictionary<string,(int Stock,int Compiled,int Size)> records)
    {
        int prefix=tokens.InstanceBuffer.Length-native.InstanceBuffer.Length;if(prefix!=end-wrapper || prefix<12 || prefix%12!=0)throw new InvalidDataException("Behavior generated/stock metadata extent differs: generated="+prefix+", stock="+(end-wrapper));var output=tokens.InstanceBuffer;var pending=new HashSet<int>();var visited=new HashSet<(int,string)>();
        // Reborn: independently reconstruct the shifted native tail and exact auxiliary order; matching metadata cannot conceal changed values, imports or pointer sources.
        var expected=native.InstanceBuffer.ToArray();foreach(uint slot in Slots(native.RelocationBuffer)){if(slot>expected.Length-4)throw new InvalidDataException("Behavior native relocation bounds.");System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(expected.AsSpan((int)slot,4),checked(Word(native.InstanceBuffer,(int)slot)+(uint)prefix));}
        if(!output.AsSpan(prefix).SequenceEqual(expected) || !Slots(tokens.RelocationBuffer).SequenceEqual(Slots(native.RelocationBuffer).Select(s=>checked(s+(uint)prefix))) || !Slots(tokens.ImportsBuffer).SequenceEqual(Slots(native.ImportsBuffer).Select(s=>checked(s+(uint)prefix))))throw new InvalidDataException("Behavior token native tail/auxiliary transformation differs.");
        for(int at=0;at<prefix;at+=12)
        {
            if(Word(output,at)!=Word(stock,wrapper+at) || Word(output,at+4)!=Word(stock,wrapper+at+4))throw new InvalidDataException("Behavior stock metadata name/tag differs at +"+at);
            uint target=Word(stock,wrapper+at+8),actual=Word(output,at+8);if(target==0){if(actual!=0)throw new InvalidDataException("Behavior stock absent target differs.");}else if(target>=wrapper && target<end){if(actual!=target-wrapper)throw new InvalidDataException("Behavior stock metadata-table target differs at +"+at);}else pending.Add(at+8);
        }
        Record(type,type.Name,wrapper,0);if(pending.Count!=0)throw new InvalidDataException("Behavior stock native targets not context-bound: "+pending.Count);
        return prefix;
        //-------------------------------------------------------------------------------------------------
        /** Reborn: base/class/list structure is read from stock row markers and actual CLR fields, not the producer's schema descriptor implementation. */
        //-------------------------------------------------------------------------------------------------
        void Record(Type current,string path,int table,int depth)
        {
            if(depth>32 || !visited.Add((table,path)))throw new InvalidDataException("Behavior stock metadata context cycle/depth differs.");if(Word(stock,table)==2)table=checked((int)Word(stock,table+8));
            for(int rows=0;;rows++,table+=12)
            {
                if(rows>1024 || table<wrapper || table>end-12)throw new InvalidDataException("Behavior stock record bounds differ.");uint name=Word(stock,table),tag=Word(stock,table+4),target=Word(stock,table+8);if(name==0 && tag==0 && target==0)break;
                if(name==1){var parent=current.GetField("Base")??throw new InvalidDataException("Behavior stock base model missing.");Record(parent.FieldType,path+".Base",(int)target,depth+1);continue;}
                var fields=current.GetFields(BindingFlags.Public|BindingFlags.Instance).Where(f=>FastHash.GetHashCode(f.Name)==name).ToArray();if(fields.Length!=1)throw new InvalidDataException("Behavior stock field name has no unique native member: "+current.Name+"/"+name.ToString("X8"));var field=fields[0];string child=path+"."+field.Name;Type member=field.FieldType;
                if(target==0)continue;if(target>=wrapper && target<end)
                {
                    if(member.IsPointer)member=member.GetElementType()!;
                    if(member.IsGenericType && member.GetGenericTypeDefinition().FullName=="Relo.List`1")List(member.GetGenericArguments()[0],child,(int)target,depth+1);else Record(member,child,(int)target,depth+1);
                }
                else Native(table+8,child);
            }
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: native list counts and primitive/record item contexts come from the independent native walker, including shared stock allocations. */
        //-------------------------------------------------------------------------------------------------
        void List(Type element,string path,int table,int depth)
        {
            if(Word(stock,table)!=0x6C827073 || Word(stock,table+4)!=0x5D448FD8)throw new InvalidDataException("Behavior stock list marker differs.");Native(table+8,path);var header=records[path];uint count=Word(native.InstanceBuffer,header.Compiled);if(count>4096)throw new InvalidDataException("Behavior stock list bound differs.");for(int i=0;i<count;i++){int marker=table+12+i*12;if(marker>end-12 || Word(stock,marker)!=0xB151A0EB || Word(stock,marker+4)!=0)throw new InvalidDataException("Behavior stock item marker differs.");uint target=Word(stock,marker+8);if(target>=wrapper && target<end)Record(element,path+"["+i+"]",(int)target,depth+1);else Native(marker+8,path+"["+i+"]");}
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: a stock address shared by two fields is never used as a global address dictionary; each expected compiled address comes from that exact field context. */
        //-------------------------------------------------------------------------------------------------
        void Native(int stockAt,string path)
        {int at=stockAt-wrapper;if(!pending.Remove(at) || !records.TryGetValue(path,out var record) || Word(stock,stockAt)!=record.Stock || Word(output,at)!=prefix+record.Compiled)throw new InvalidDataException("Behavior contextual native metadata target differs at "+path);}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independent stock and output word reads remain inside their bounded buffers. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at){if(at<0 || at>b.Length-4)throw new InvalidDataException("Behavior stock metadata word bounds differ.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: this oracle independently reads complete unique auxiliary arrays rather than trusting the producer's slot inventory. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b){if(b.Length==0)return Array.Empty<uint>();if(b.Length<8 || b.Length%4!=0 || Word(b,b.Length-4)!=uint.MaxValue)throw new InvalidDataException("Behavior proof auxiliary framing.");var values=Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();if(values.Distinct().Count()!=values.Length)throw new InvalidDataException("Behavior proof duplicate auxiliary source.");return values;}
}
