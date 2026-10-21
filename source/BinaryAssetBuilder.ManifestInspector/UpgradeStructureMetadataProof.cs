using System.Buffers.Binary;
using System.Reflection;
using BinaryAssetBuilder.Core.Hashing;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: captured field/tag grammar remains the oracle while source-sized weak lists and optional animation records change metadata extents.
internal static class UpgradeStructureMetadataProof
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: walk stock and output field contexts independently, own every metadata row, and bind native targets to the separately checked source/native walker. */
    //-------------------------------------------------------------------------------------------------
    internal static int Compare(Chunk tokens,Chunk native,byte[] stock,int wrapper,int end,Type type,IReadOnlyDictionary<string,(int Stock,int Compiled,int Size)> records,ConscriptUpgradeStructure.Plan plan)
    {
        int prefix=tokens.InstanceBuffer.Length-native.InstanceBuffer.Length;if(prefix<12||prefix%12!=0)throw new InvalidDataException("Upgrade metadata extent is invalid.");
        BehaviorContainerStockProof.VerifyTail(tokens,native,prefix);var bytes=tokens.InstanceBuffer;var owned=new bool[prefix/12];
        Record(type,type.Name,wrapper,0,0);
        if(owned.Contains(false))throw new InvalidDataException("Upgrade metadata has unowned or unreachable rows.");return prefix;

        // Reborn: a row can be reached only once and must remain entirely inside this module's metadata prefix.
        void Own(int at){if(at<0||at%12!=0||at>prefix-12||owned[at/12])throw new InvalidDataException("Upgrade metadata overlap, alignment or bounds differ.");owned[at/12]=true;}
        // Reborn: expected names/tags come from the captured stock grammar, never from producer schema enumeration.
        void Header(int old,int at){if(old<wrapper||old>end-12||Word(stock,old)!=Word(bytes,at)||Word(stock,old+4)!=Word(bytes,at+4))throw new InvalidDataException("Upgrade metadata field name/tag differs.");}
        // Reborn: unchanged fields retain exact stock context; source-sized items have independent compiled coordinates but no invented stock index.
        void Native(int targetAt,string path,int? oldTarget){if(!records.TryGetValue(path,out var record)||oldTarget.HasValue&&record.Stock!=oldTarget.Value||Word(bytes,targetAt)!=prefix+record.Compiled)throw new InvalidDataException("Upgrade metadata native field target differs: "+path);}

        //-------------------------------------------------------------------------------------------------
        /** Reborn: records preserve the captured base/field order, including terminators and optional pointer presence from the source contract. */
        //-------------------------------------------------------------------------------------------------
        void Record(Type current,string path,int old,int at,int depth)
        {
            if(depth>32)throw new InvalidDataException("Upgrade metadata record depth exceeded.");
            if(Word(stock,old)==2){Own(at);Header(old,at);Record(current,path,(int)Word(stock,old+8),(int)Word(bytes,at+8),depth+1);return;}
            for(int row=0;;row++,old+=12,at+=12){
                if(row>1024)throw new InvalidDataException("Upgrade record row bound exceeded.");Own(at);Header(old,at);
                uint name=Word(stock,old),tag=Word(stock,old+4),target=Word(stock,old+8);int actual=(int)Word(bytes,at+8);
                if(name==0&&tag==0&&target==0){if(actual!=0)throw new InvalidDataException("Upgrade metadata terminator target differs.");break;}
                if(name==1){var parent=current.GetField("Base")??throw new InvalidDataException("Upgrade metadata base is missing.");Record(parent.FieldType,path+".Base",(int)target,actual,depth+1);continue;}
                var matches=current.GetFields(BindingFlags.Public|BindingFlags.Instance).Where(f=>FastHash.GetHashCode(f.Name)==name).ToArray();if(matches.Length!=1)throw new InvalidDataException("Upgrade metadata native field is ambiguous.");
                var field=matches[0];string child=path+"."+field.Name;Type member=field.FieldType;
                if(plan.Optional.TryGetValue(child,out var animation)){
                    if(animation==null){if(actual!=0)throw new InvalidDataException("Omitted animation still has metadata storage.");}
                    else Animation(child,actual);
                    continue;
                }
                if(target==0){if(actual!=0)throw new InvalidDataException("Unimplemented upgrade field presence change: "+child);continue;}
                if(target>=wrapper&&target<end){
                    if(member.IsPointer)member=member.GetElementType()!;
                    if(member.IsGenericType&&member.GetGenericTypeDefinition().FullName=="Relo.List`1")List(member.GetGenericArguments()[0],child,(int)target,actual,depth+1);else Record(member,child,(int)target,actual,depth+1);
                }else Native(at+8,child,(int)target);
            }
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: weak-list markers and cardinality follow authored values; all other nested lists retain their independent captured shape. */
        //-------------------------------------------------------------------------------------------------
        void List(Type element,string path,int old,int at,int depth)
        {
            Own(at);Header(old,at);if(Word(stock,old)!=0x6C827073||Word(stock,old+4)!=0x5D448FD8)throw new InvalidDataException("Upgrade list marker differs.");Native(at+8,path,(int)Word(stock,old+8));
            var record=records[path];int count=checked((int)Word(native.InstanceBuffer,record.Compiled));
            if(plan.Lists.TryGetValue(path,out var expected)){
                if(count!=expected.Length)throw new InvalidDataException("Upgrade source/native/metadata list count differs.");
                for(int i=0;i<count;i++){int item=checked(at+12+i*12);Own(item);if(Word(bytes,item)!=0xB151A0EB||Word(bytes,item+4)!=0)throw new InvalidDataException("Upgrade weak-list item marker differs.");Native(item+8,path+"["+i+"]",null);}
                return;
            }
            if(count!=Word(stock,record.Stock))throw new InvalidDataException("Unimplemented upgrade nested list layout change.");
            for(int i=0;i<count;i++){int item=checked(at+12+i*12),original=checked(old+12+i*12);Own(item);Header(original,item);uint target=Word(stock,original+8);if(target>=wrapper&&target<end)Record(element,path+"["+i+"]",(int)target,(int)Word(bytes,item+8),depth+1);else Native(item+8,path+"["+i+"]",(int)target);}
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: the already reviewed AnimAndDuration ABI has exactly three scalar rows and a terminator, including omitted attribute defaults. */
        //-------------------------------------------------------------------------------------------------
        void Animation(string path,int at)
        {
            var names=new[]{"AnimState","Frames","Trigger"};if(!records.TryGetValue(path,out var record)||record.Size!=12)throw new InvalidDataException("Optional animation native context missing.");
            for(int i=0;i<4;i++){int row=checked(at+i*12);Own(row);uint name=i<3?FastHash.GetHashCode(names[i]):0,tag=i==0?0x15723E0Bu:i<3?0x763177A4u:0,target=i<3?(uint)(prefix+record.Compiled+i*4):0;if(Word(bytes,row)!=name||Word(bytes,row+4)!=tag||Word(bytes,row+8)!=target)throw new InvalidDataException("Optional animation metadata row differs.");}
        }
    }

    // Reborn: stock, metadata and native word reads are always bounds checked before interpreting coordinates.
    private static uint Word(byte[] bytes,int at){if(at<0||at>bytes.Length-4)throw new InvalidDataException("Upgrade metadata word bounds differ.");return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));}
}
