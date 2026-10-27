using System.Buffers.Binary;
using System.Reflection;
using BinaryAssetBuilder.Core.Hashing;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: source-sized metadata proof. Each record type's row grammar (names, wire tags, class rows and whether a row targets a table or a
// native field) is learned from real stock leaves; explicitly qualified scalar-only optional items use their original schema and wire identities. Generated metadata must reproduce that grammar for every
// record actually present, with list/item cardinality from the independently proved native graph and every native target bound to the
// exact proved field coordinate. The producer writer's own code is never used as the oracle.
internal sealed class StockMetadataGrammar
{
    private enum Kind{Null,Table,Native}
    private sealed class Rows{internal (uint Name,uint Tag)[] Fields=Array.Empty<(uint,uint)>();internal HashSet<Kind>[] Kinds=Array.Empty<HashSet<Kind>>();internal uint? Class;}
    private readonly Dictionary<Type,Rows> grammar=new();
    internal int Witnesses{get;private set;}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: learn from one stock leaf; targets below the stock native root are metadata tables, others are native fields. */
    //-------------------------------------------------------------------------------------------------
    internal void Learn(byte[] stock,int wrapper,int nativeRoot,Type type)
    {
        Record(type,wrapper,0);Witnesses++;

        void Record(Type current,int table,int depth)
        {
            if(depth>48)throw new InvalidDataException("Stock grammar depth exceeded.");
            if(Word(stock,table)==2){uint tag=Word(stock,table+4);var rows=Get(current);if(rows.Class!=null&&rows.Class!=tag)throw new InvalidDataException("Stock grammar class tag differs: "+current.Name);rows.Class=tag;table=checked((int)Word(stock,table+8));}
            var found=new List<(uint,uint,uint)>();
            for(int at=table;;at+=12){if(found.Count>1024||at>=nativeRoot)throw new InvalidDataException("Stock grammar record bounds differ.");uint n=Word(stock,at),t=Word(stock,at+4),x=Word(stock,at+8);if(n==0&&t==0&&x==0)break;found.Add((n,t,x));}
            var r=Get(current);var fields=found.Select(f=>(f.Item1,f.Item2)).ToArray();
            if(r.Fields.Length==0&&r.Kinds.Length==0){r.Fields=fields;r.Kinds=fields.Select(_=>new HashSet<Kind>()).ToArray();}
            else if(!r.Fields.SequenceEqual(fields))throw new InvalidDataException("Stock grammar rows differ between witnesses: "+current.Name);
            for(int i=0;i<found.Count;i++)
            {
                var (name,_,target)=found[i];var kind=target==0?Kind.Null:target<nativeRoot?Kind.Table:Kind.Native;r.Kinds[i].Add(kind);
                if(kind!=Kind.Table)continue;
                if(name==1){Record(Base(current),(int)target,depth+1);continue;}
                var member=Member(current,name).FieldType;if(member.IsPointer)member=member.GetElementType()!;
                if(IsList(member))List(member.GetGenericArguments()[0],(int)target,depth+1);else Record(member,(int)target,depth+1);
            }
        }
        void List(Type element,int table,int depth)
        {
            if(Word(stock,table)!=0x6C827073||Word(stock,table+4)!=0x5D448FD8)throw new InvalidDataException("Stock grammar list marker differs.");
            uint count=Word(stock,(int)Word(stock,table+8));if(count>4096)throw new InvalidDataException("Stock grammar list bound differs.");
            for(int i=0;i<count;i++){int item=table+12+i*12;if(Word(stock,item)!=0xB151A0EB||Word(stock,item+4)!=0)throw new InvalidDataException("Stock grammar item marker differs.");uint target=Word(stock,item+8);if(target!=0&&target<nativeRoot){
                // Reborn: FX constant list wrappers select their real concrete stock class before learning its own/base row grammar.
                // Reborn: weapon nugget wrappers likewise name their concrete class in the wrapper tag.
                Type concrete=element==typeof(SageBinaryData.FXShaderConstant)?SchemaNativeContract.ShaderConstantType(Word(stock,(int)target+4)):element==typeof(SageBinaryData.WeaponEffectNugget)?WeaponTemplateCompiler.Class(Word(stock,(int)target+4)).Native:element;Record(concrete,(int)target,depth);
            }}
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify one standalone generated leaf (metadata prefix + native tail) against the learned grammar and proved coordinates. */
    //-------------------------------------------------------------------------------------------------
    internal int Verify(Chunk tokens,Chunk native,Type type,IReadOnlyDictionary<string,int> records,bool rootList=false)
    {
        int prefix=tokens.InstanceBuffer.Length-native.InstanceBuffer.Length;if(prefix<12||prefix%12!=0)throw new InvalidDataException("Generated metadata extent is invalid.");
        var bytes=tokens.InstanceBuffer;var owned=new bool[prefix/12];
        // Reborn: the native tail must be the proved leaf with only its relocations shifted by the metadata prefix.
        var expected=native.InstanceBuffer.ToArray();foreach(uint slot in Slots(native.RelocationBuffer))BinaryPrimitives.WriteUInt32LittleEndian(expected.AsSpan((int)slot,4),checked(BinaryPrimitives.ReadUInt32LittleEndian(expected.AsSpan((int)slot,4))+(uint)prefix));
        if(!bytes.AsSpan(prefix).SequenceEqual(expected)||!Slots(tokens.RelocationBuffer).SequenceEqual(Slots(native.RelocationBuffer).Select(s=>checked(s+(uint)prefix)))||!Slots(tokens.ImportsBuffer).SequenceEqual(Slots(native.ImportsBuffer).Select(s=>checked(s+(uint)prefix))))throw new InvalidDataException("Generated metadata native tail/auxiliary transformation differs.");
        // Reborn: an inline root list starts at its count marker rather than a synthetic wrapper record.
        if(rootList)List(type,type.Name,0,0);else Record(type,type.Name,0,0);
        if(owned.Contains(false))throw new InvalidDataException("Generated metadata has unowned or unreachable rows.");
        return prefix;

        void Own(int at){if(at<0||at%12!=0||at>prefix-12||owned[at/12])throw new InvalidDataException("Generated metadata overlap, alignment or bounds differ.");owned[at/12]=true;}
        uint W(int at)=>Word(bytes,at);
        uint Coordinate(string path)=>records.TryGetValue(path,out int at)?checked((uint)(prefix+at)):throw new InvalidDataException("Generated metadata target has no proved native field: "+path);
        int Table(int at,string path){uint target=W(at+8);if(target==0||target>=prefix||target%12!=0)throw new InvalidDataException("Generated metadata table target differs: "+path);return (int)target;}

        void Record(Type current,string path,int table,int depth)
        {
            if(depth>48)throw new InvalidDataException("Generated metadata depth exceeded.");
            var rows=grammar.TryGetValue(current,out var r)?r:throw new InvalidDataException("No stock metadata grammar for record "+current.Name+" ("+path+"); this nested shape has no stock witness.");
            if(W(table)==2){Own(table);if(rows.Class==null||W(table+4)!=rows.Class)throw new InvalidDataException("Generated metadata class row differs: "+path);table=Table(table,path);}
            else if(rows.Class!=null&&depth==0)throw new InvalidDataException("Generated metadata class row missing: "+path);
            for(int i=0;i<rows.Fields.Length;i++)
            {
                int at=table+i*12;Own(at);var (name,tag)=rows.Fields[i];if(W(at)!=name||W(at+4)!=tag)throw new InvalidDataException("Generated metadata name/tag differs from stock grammar: "+path+" row "+i);
                var kinds=rows.Kinds[i];uint target=W(at+8);
                if(name==1){if(!kinds.Contains(Kind.Table))throw new InvalidDataException("Stock grammar base row kind differs.");Record(Base(current),path+".Base",Table(at,path),depth+1);continue;}
                var field=Member(current,name);string child=path+"."+field.Name;Type member=field.FieldType;
                if(member.IsPointer)
                {
                    uint pointer=Native(child,0);
                    if(pointer==0){if(target!=0)throw new InvalidDataException("Generated metadata absent optional record has a target: "+child);continue;}
                    var element=member.GetElementType()!;string pointee=child+"*";
                    // Reborn: a named optional record may reuse its independently stock-learned grammar from another owner of the exact same wire type.
                    bool typedRecord=rows.Fields[i].Tag==FastHash.GetHashCode(element.Name);
                    if((kinds.Contains(Kind.Table)||typedRecord)&&grammar.ContainsKey(element)){Record(element,pointee,Table(at,child),depth+1);continue;}
                    if(kinds.Contains(Kind.Table)&&IsList(element)){List(element.GetGenericArguments()[0],pointee,Table(at,child),depth+1);continue;}
                    // Reborn: EA vector-hack wire tags directly target proved coordinate bytes even when all selected stock witnesses omit the optional vector.
                    bool directVector=element==typeof(SageBinaryData.Coord2D)&&rows.Fields[i].Tag==0x4AE31F24u||element==typeof(SageBinaryData.Coord3D)&&rows.Fields[i].Tag==0x29368077u;
                    // Reborn: native mask pointers bind independently proved bits under their exact stock flag wire tag, including previously absent masks.
                    bool directFlags=SchemaNativeContract.IsFlags(element)&&typedRecord;
                    // Reborn: schema-proved optional float pointees use the same independently stock-observed native real wire, not a nested table.
                    bool directReal=element==typeof(float)&&rows.Fields[i].Tag==0x645864F3u;
                    // Reborn: this EP1 optional scalar Poid has a captured Poid tag; its module hash and pointee coordinate are independently source-proved.
                    bool directPassiveModule=current==typeof(SageBinaryData.Ep1PassiveAreaEffectBehaviorModuleData)&&field.Name=="AttachModuleId"&&element==typeof(TypedAssetId<SageBinaryData.ModuleData>)&&rows.Fields[i].Tag==0xBDF07965u;
                    if((!kinds.Contains(Kind.Native)&&!directVector&&!directFlags&&!directReal&&!directPassiveModule)||target!=Coordinate(pointee))throw new InvalidDataException("Generated metadata optional field target differs: "+child);continue;
                }
                if(IsList(member))
                {
                    uint count=Native(child,0);
                    if(target==0){if(count!=0)throw new InvalidDataException("Generated metadata omits a populated list: "+child);continue;}
                    if(!kinds.Contains(Kind.Table))throw new InvalidDataException("Stock grammar list row kind differs: "+child);
                    List(member.GetGenericArguments()[0],child,Table(at,child),depth+1);continue;
                }
                if(kinds.Contains(Kind.Table)&&!kinds.Contains(Kind.Native)){Record(member,child,Table(at,child),depth+1);continue;}
                if(!kinds.Contains(Kind.Native)){if(target!=0)throw new InvalidDataException("Generated metadata target differs from stock absent field: "+child);continue;}
                if(target!=Coordinate(child))throw new InvalidDataException("Generated metadata native field target differs: "+child);
            }
            Own(table+rows.Fields.Length*12);if(W(table+rows.Fields.Length*12)!=0||W(table+rows.Fields.Length*12+4)!=0||W(table+rows.Fields.Length*12+8)!=0)throw new InvalidDataException("Generated metadata record terminator differs: "+path);
        }
        void List(Type element,string path,int table,int depth)
        {
            Own(table);if(W(table)!=0x6C827073||W(table+4)!=0x5D448FD8||W(table+8)!=Coordinate(path))throw new InvalidDataException("Generated metadata list header differs: "+path);
            uint count=Native(path,0);if(count>65536)throw new InvalidDataException("Generated metadata list bound differs.");
            for(int i=0;i<count;i++)
            {
                int item=table+12+i*12;Own(item);string itemPath=path+"["+i+"]";if(W(item)!=0xB151A0EB||W(item+4)!=0)throw new InvalidDataException("Generated metadata item marker differs: "+itemPath);
                // Reborn: independently proved source/native records select the shader class; metadata must reproduce the stock-qualified class wrapper.
                if(element==typeof(SageBinaryData.FXShaderConstant))Record(SchemaNativeContract.ShaderConstantType(Native(itemPath,0)),itemPath,Table(item,itemPath),depth+1);
                else if(element==typeof(SageBinaryData.WeaponEffectNugget))Record(WeaponTemplateCompiler.Class(Native(itemPath,0)).Native,itemPath,Table(item,itemPath),depth+1);
                else if(grammar.ContainsKey(element))Record(element,itemPath,Table(item,itemPath),depth+1);
                else if(W(item+8)!=Coordinate(itemPath))throw new InvalidDataException("Generated metadata list item target differs: "+itemPath);
            }
        }
        uint Native(string path,int offset){int at=records.TryGetValue(path,out int value)?value:throw new InvalidDataException("Generated metadata field has no proved native coordinate: "+path);return Word(native.InstanceBuffer,at+offset);}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stock OCL power lists are empty; their official EP1 item schema is a single four-byte OCL reference, whose wire tag is independently captured on the parent power. */
    //-------------------------------------------------------------------------------------------------
    internal void QualifyOclUpgradePair(uint capturedOclSignature)
    {
        if(capturedOclSignature!=0x42E4E9D1u||System.Runtime.InteropServices.Marshal.SizeOf<SageBinaryData.Ep1OCLUpgradePair>()!=4)throw new InvalidDataException("EP1 OCL upgrade pair must contain exactly one stock-typed strong OCL word.");
        var rows=Get(typeof(SageBinaryData.Ep1OCLUpgradePair));rows.Fields=new[]{(FastHash.GetHashCode("OCL"),capturedOclSignature)};rows.Kinds=new[]{new HashSet<Kind>{Kind.Native}};
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retail SubObjectsUpgrade texture lists are empty; qualify the exact two-reference item from its official schema and an independently captured stock TextureRef wire tag. */
    //-------------------------------------------------------------------------------------------------
    internal void QualifyReplaceTexturePair(uint textureSignature,System.Xml.Schema.XmlSchemaSet schemas)
    {
        var shape=(System.Xml.Schema.XmlSchemaComplexType)schemas.GlobalTypes[new System.Xml.XmlQualifiedName("ReplaceTexture","uri:ea.com:eala:asset")]!;
        if(textureSignature!=0x4C3E3938u||System.Runtime.InteropServices.Marshal.SizeOf<SageBinaryData.ReplaceTexture>()!=8||shape.AttributeUses.Count!=2)throw new InvalidDataException("ReplaceTexture requires exactly two stock-typed TextureRef words.");
        foreach(string name in new[]{"Original","New"})if(shape.AttributeUses[new System.Xml.XmlQualifiedName(name)] is not System.Xml.Schema.XmlSchemaAttribute attribute||attribute.AttributeSchemaType?.QualifiedName.Name!="TextureRef")throw new InvalidDataException("ReplaceTexture schema reference differs: "+name);
        var rows=Get(typeof(SageBinaryData.ReplaceTexture));rows.Fields=new[]{(FastHash.GetHashCode("Original"),textureSignature),(FastHash.GetHashCode("New"),textureSignature)};rows.Kinds=new[]{new HashSet<Kind>{Kind.Native},new HashSet<Kind>{Kind.Native}};
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: selected retail transports have empty extended lists; their exact scalar-only item grammars follow the original XSD and already retail-qualified reference/integer wire identities. */
    //-------------------------------------------------------------------------------------------------
    internal void QualifyTransportItems(System.Xml.Schema.XmlSchemaSet schemas)
    {
        // Reborn: OpenContain member-status items are also scalar-only; the enum wire is the hash of its original named schema type.
        Qualify(typeof(SageBinaryData.MemberTemplateStatusData),8,new[]{("ThingTemplate","GameObjectWeakRef",0x148ED75Du),("ObjectStatus","ObjectStatusType",FastHash.GetHashCode("ObjectStatusType"))});
        Qualify(typeof(SageBinaryData.InitialPayload),8,new[]{("Name","GameObjectWeakRef",0x148ED75Du),("Count","SageInt",0x23E650B8u)});
        Qualify(typeof(SageBinaryData.UpgradeCreation),12,new[]{("Upgrade","UpgradeTemplateRef",0x4ABA1F29u),("Template","GameObjectWeakRef",0x148ED75Du),("Num","SageUnsignedInt",0x763177A4u)});
        // Reborn: no populated retail payload/upgrade witness is claimed; every emitted coordinate is still checked against the independently proved source graph.
        void Qualify(Type type,int size,(string Name,string Wire,uint Tag)[] fields)
        {
            var shape=(System.Xml.Schema.XmlSchemaComplexType)schemas.GlobalTypes[new System.Xml.XmlQualifiedName(type.Name,"uri:ea.com:eala:asset")]!;
            if(System.Runtime.InteropServices.Marshal.SizeOf(type)!=size||shape.AttributeUses.Count!=fields.Length||shape.ContentType!=System.Xml.Schema.XmlSchemaContentType.Empty)throw new InvalidDataException("Transport item schema/native shape differs: "+type.Name);
            foreach(var field in fields)if(shape.AttributeUses[new System.Xml.XmlQualifiedName(field.Name)] is not System.Xml.Schema.XmlSchemaAttribute attribute||attribute.AttributeSchemaType?.QualifiedName.Name!=field.Wire)throw new InvalidDataException("Transport item schema field differs: "+type.Name+"."+field.Name);
            var rows=Get(type);rows.Fields=fields.Select(f=>(FastHash.GetHashCode(f.Name),f.Tag)).ToArray();rows.Kinds=fields.Select(_=>new HashSet<Kind>{Kind.Native}).ToArray();
        }
    }

    private Rows Get(Type type){if(!grammar.TryGetValue(type,out var rows))grammar[type]=rows=new Rows();return rows;}
    private static Type Base(Type type)=>type.GetField("Base")?.FieldType??throw new InvalidDataException("Stock grammar base model missing: "+type.Name);
    private static FieldInfo Member(Type type,uint name){var fields=type.GetFields(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance).Where(f=>FastHash.GetHashCode(f.Name)==name).ToArray();if(fields.Length!=1)throw new InvalidDataException("Stock grammar field has no unique native member: "+type.Name+"/"+name.ToString("X8"));return fields[0];}
    private static bool IsList(Type type)=>type.IsGenericType&&(type.GetGenericTypeDefinition().FullName=="Relo.List`1"||type.GetGenericTypeDefinition().FullName=="Relo.PolymorphicList`1"&&(type.GetGenericArguments()[0]==typeof(SageBinaryData.FXShaderConstant)||type.GetGenericArguments()[0]==typeof(SageBinaryData.WeaponEffectNugget)));
    private static uint Word(byte[] b,int at){if(at<0||at>b.Length-4)throw new InvalidDataException("Metadata grammar word bounds differ.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    private static uint[] Slots(byte[] b){if(b.Length==0)return Array.Empty<uint>();if(b.Length<8||b.Length%4!=0||Word(b,b.Length-4)!=uint.MaxValue)throw new InvalidDataException("Metadata grammar auxiliary framing.");return Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();}
}
