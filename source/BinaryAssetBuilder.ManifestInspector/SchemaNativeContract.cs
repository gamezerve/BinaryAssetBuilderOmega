using System.Buffers.Binary;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: generic schema/native proof. The expected native graph is derived from the schema-qualified, reference-resolved source
// (official XSD enumeration order, XSD defaults, resolved strong identities) walked in parallel with the reviewed C# record layout.
// It never calls the producer marshaler: every scalar, enum, flag set, string, list, optional pointer and strong import of an authored
// leaf is recomputed and compared, all bytes/relocations/imports must be owned, and authored source fields that the native model does
// not consume are refused instead of being silently dropped.
internal sealed class SchemaNativeContract
{
    // Reborn: origin marker for variable-container leaves admitted through this proof instead of a fixed stock-shaped template.
    internal const string Origin="RebornSchemaVerified";
    // Reborn: explicitly opted-in leaf families (container, wire class, production leaf metadata writer); each is qualified against stock before use.
    internal sealed record Family(Type Type,string Container,uint TypeId,Func<Chunk,Chunk> Leaf);
    internal static readonly Family[] Families={
        // Reborn: the existing tracer writer supports both optional-status shapes without freezing the Conscript template.
        new(typeof(W3DTracerDrawModuleData),"Draws",0x54018E60u,BinaryAssetBuilder.XmlCompiler.Ep1TracerDrawTokenizer.Tokenize),
        new(typeof(W3DScriptedModelDrawModuleData),"Draws",0x4ECF2A4Bu,BinaryAssetBuilder.XmlCompiler.Ep1ScriptedDrawTokenizer.Tokenize),
        new(typeof(ModelConditionAudioLoopClientBehaviorModuleData),"ClientBehaviors",0x5A3EDBF4u,leaf=>FromList(leaf,BinaryAssetBuilder.XmlCompiler.Ep1ClientBehaviorsTokenizer.Tokenize)),
        new(typeof(Ep1ModelConditionSoundSelectorClientBehaviorModuleData),"ClientBehaviors",0x327CFB76u,leaf=>FromList(leaf,BinaryAssetBuilder.XmlCompiler.Ep1ClientBehaviorsTokenizer.Tokenize)),
        new(typeof(AnimationSoundClientBehaviorModuleData),"ClientBehaviors",0x27EF791Du,leaf=>FromList(leaf,BinaryAssetBuilder.XmlCompiler.Ep1ClientBehaviorsTokenizer.Tokenize))};
    internal static bool Supports(Type type)=>Families.Any(f=>f.Type==type);
    internal static Family For(Type type)=>Families.Single(f=>f.Type==type);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: run a list-level production writer on a one-leaf canonical list and return that leaf's standalone metadata + native tail. */
    //-------------------------------------------------------------------------------------------------
    internal static Chunk FromList(Chunk leaf,Func<Chunk,Chunk> tokenize)
    {
        int n=leaf.InstanceBuffer.Length;var list=new byte[12+n];Put(list,0,1);Put(list,4,8);Put(list,8,12);leaf.InstanceBuffer.CopyTo(list,12);
        var leafRelos=Slots(leaf.RelocationBuffer);var leafImports=Slots(leaf.ImportsBuffer);foreach(uint slot in leafRelos)Put(list,12+(int)slot,checked(Get(list,12+(int)slot)+12));
        var tokens=tokenize(new Chunk{InstanceBuffer=list,RelocationBuffer=Pack(new uint[]{4,8}.Concat(leafRelos.Select(s=>s+12))),ImportsBuffer=Pack(leafImports.Select(s=>s+12))});
        int prefix=tokens.InstanceBuffer.Length-list.Length;var t=tokens.InstanceBuffer;
        if(prefix<36||Get(t,0)!=0x6C827073||Get(t,12)!=0xB151A0EB||Get(t,20)!=24)throw new InvalidDataException("One-leaf list metadata framing differs.");
        int local=prefix-24;var output=new byte[local+n];t.AsSpan(24,local).CopyTo(output);
        for(int at=8;at<local;at+=12){uint target=Get(output,at);if(target==0)continue;
            if(target>=24&&target<prefix)Put(output,at,target-24);else if(target>=prefix+12)Put(output,at,checked((uint)local+target-(uint)prefix-12));else throw new InvalidDataException("One-leaf metadata target escapes the leaf.");}
        leaf.InstanceBuffer.CopyTo(output,local);foreach(uint slot in leafRelos)Put(output,local+(int)slot,checked(Get(output,local+(int)slot)+(uint)local));
        return new Chunk{InstanceBuffer=output,RelocationBuffer=Pack(leafRelos.Select(s=>s+(uint)local)),ImportsBuffer=Pack(leafImports.Select(s=>s+(uint)local))};
    }
    private static uint Get(byte[] b,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
    private static byte[] Pack(IEnumerable<uint> slots){var values=slots.ToArray();if(values.Length==0)return Array.Empty<byte>();var bytes=new byte[(values.Length+1)*4];for(int i=0;i<values.Length;i++)Put(bytes,i*4,values[i]);Put(bytes,values.Length*4,uint.MaxValue);return bytes;}

    internal sealed record Result(IReadOnlyDictionary<string,int> Records,IReadOnlyCollection<uint> Relocations,IReadOnlyCollection<uint> Imports);

    private readonly byte[] b;private readonly bool stockMode;private readonly uint typeId;private readonly IReadOnlyList<AssetId> references;
    private readonly bool[] covered;private readonly HashSet<uint> relos=new();private readonly HashSet<uint> imports=new();
    private readonly Dictionary<string,int> records=new(StringComparer.Ordinal);private readonly HashSet<XmlNode> consumed=new();

    private SchemaNativeContract(byte[] bytes,bool stockMode,uint typeId,IReadOnlyList<AssetId> references){b=bytes;this.stockMode=stockMode;this.typeId=typeId;this.references=references;covered=new bool[bytes.Length];}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove one compiled leaf (pointers rebased to the leaf) with complete byte, relocation and import ownership. */
    //-------------------------------------------------------------------------------------------------
    internal static Result Verify(XmlElement resolved,Type type,uint typeId,Chunk leaf,IReadOnlyList<AssetId> referenced)
    {
        var proof=new SchemaNativeContract(leaf.InstanceBuffer,false,typeId,referenced);
        proof.Claim(0,Size(type));proof.Struct(type,resolved,0,type.Name,0);proof.Consumed(resolved);
        int gap=0;for(int i=0;i<proof.b.Length;i++){if(proof.covered[i]){gap=0;continue;}if(proof.b[i]!=0||++gap>3)throw new InvalidDataException("Schema-proved native allocation is unowned at +"+i+" ("+type.Name+").");}
        if(!Slots(leaf.RelocationBuffer).ToHashSet().SetEquals(proof.relos))throw new InvalidDataException("Schema-proved relocation inventory differs ("+type.Name+").");
        if(!Slots(leaf.ImportsBuffer).ToHashSet().SetEquals(proof.imports))throw new InvalidDataException("Schema-proved import inventory differs ("+type.Name+").");
        return new(proof.records,proof.relos,proof.imports);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify repeated root records from their schema-qualified elements without treating unrelated GameObject attributes as list fields. */
    //-------------------------------------------------------------------------------------------------
    internal static Result VerifyInlineList(XmlElement root,string name,Type itemType,Chunk leaf,IReadOnlyList<AssetId> referenced)
    {
        var proof=new SchemaNativeContract(leaf.InstanceBuffer,false,0,referenced);proof.Claim(0,8);proof.records[itemType.Name]=0;proof.List(itemType,root,name,0,itemType.Name,0);
        foreach(var child in Children(root,name))proof.Consumed(child);
        int gap=0;for(int i=0;i<proof.b.Length;i++){if(proof.covered[i])gap=0;else if(proof.b[i]!=0||++gap>3)throw new InvalidDataException("Unowned root inline-list native allocation.");}
        if(!Slots(leaf.RelocationBuffer).ToHashSet().SetEquals(proof.relos)||!Slots(leaf.ImportsBuffer).ToHashSet().SetEquals(proof.imports))throw new InvalidDataException("Root inline-list auxiliary inventory differs.");return new(proof.records,proof.relos,proof.imports);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify the same derivation against a stock leaf (absolute stock pointers, stock occurrence table; stock interning allowed). */
    //-------------------------------------------------------------------------------------------------
    internal static void VerifyStock(XmlElement resolved,Type type,uint typeId,byte[] stock,int native,IReadOnlyList<AssetId> stockReferences)
    {
        // Reborn: stock witness comparisons are developer regressions; Verify above still checks every current source/native output in fast builds.
        if(!SdkBuildTests.Enabled)return;
        var proof=new SchemaNativeContract(stock,true,typeId,stockReferences);proof.Struct(type,resolved,native,type.Name,0);proof.Consumed(resolved);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate a whole GameObject copy against the reference schema set and resolve strong references to concrete Type:Name identities. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlDocument Resolve(XmlDocument source,SchemaSet schemas,ManifestAsset[] catalog)
    {
        var document=new XmlDocument{XmlResolver=null,PreserveWhitespace=true};document.LoadXml(source.DocumentElement!.OuterXml);document.Schemas.Add(schemas.Schemas);
        document.Validate((_,e)=>throw new InvalidDataException(e.Message));ConscriptBehaviorSurvey.ResolveStrong(document,schemas,catalog);return document;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: native records are walked in declared field order; inheritance stays on the same source element. */
    //-------------------------------------------------------------------------------------------------
    private void Struct(Type type,XmlElement? e,int at,string path,int depth)
    {
        if(depth>48)throw new InvalidDataException("Schema-proved record depth exceeded.");
        int size=Size(type);Range(at,size);records[path]=at;var used=new bool[size];
        foreach(var field in type.GetFields(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic))
        {
            int offset=Marshal.OffsetOf(type,field.Name).ToInt32(),width=FieldSize(field.FieldType);
            if(offset<0||offset+width>size)throw new InvalidDataException("Schema-proved field escapes its record: "+type.Name+"."+field.Name);Array.Fill(used,true,offset,width);
            Field(type,field,e,at+offset,path+"."+field.Name,depth);
        }
        for(int i=0;i<size;i++)if(!used[i]&&b[at+i]!=0)throw new InvalidDataException("Schema-proved record padding is not zero: "+path+" +"+i);
    }

    private void Field(Type owner,FieldInfo field,XmlElement? e,int at,string path,int depth)
    {
        Type type=field.FieldType;string name=field.Name;records[path]=at;
        // Reborn: shader constant class IDs come from the validated concrete schema type, not authored TypeId text or the enclosing draw class.
        // Reborn: button class identities derive from the validated schema; the unextended retail Power record has a zero class word.
        if(owner==typeof(Ep1PlayerPowerButtonTemplateData)&&name=="TypeId"){Type concrete=PlayerPowerButtonCompiler.DataType(e!);Expect(at,BitConverter.GetBytes(concrete==typeof(Ep1PlayerPowerButtonTemplateData)?0u:FastHash.GetHashCode(concrete.Name[3..])),path);return;}
        if(owner==typeof(FXShaderConstant)&&name=="TypeId"){string wire=e?.SchemaInfo.SchemaType?.QualifiedName.Name??throw new InvalidDataException("Shader constant schema type missing.");uint id=FastHash.GetHashCode(wire);_=ShaderConstantType(id);Expect(at,BitConverter.GetBytes(id),path);return;}
        // Reborn: scalar shader Values are child elements, unlike the attribute-only scalar owners admitted earlier.
        if((owner==typeof(FXShaderConstantInt)||owner==typeof(FXShaderConstantBool))&&name=="Value"){var value=Child(e,name)??throw new InvalidDataException("Shader constant Value is required.");var carrier=value.OwnerDocument.CreateElement("Scalar");carrier.SetAttribute("Value",value.InnerText);Expect(at,ConscriptModuleScalarContract.Bytes(carrier,new("Value",0,Size(type),Scalar(type)!,null,0)),path);return;}
        // Reborn: FX nugget class IDs are defined by each concrete validated source schema, not the enclosing FXList asset identity.
        // Reborn: Color3/Color4 runtime storage is packed BGRA; an existing color node defaults its unauthored alpha to 255.
        if(owner==typeof(Color)&&name=="A"&&e!=null&&!e.HasAttribute("A")){Expect(at,new byte[]{255},path);return;}
        // Reborn: each concrete particle module carries the hash of its validated schema class, independent of its enclosing asset.
        if(owner==typeof(FXParticleBaseModule)&&name=="TypeId"){Type concrete=FXParticleSystemCompiler.ModuleType(e!);Expect(at,BitConverter.GetBytes(FastHash.GetHashCode(concrete.Name)),path);return;}
        if(owner==typeof(FXNugget)&&name=="TypeId"){uint id=FastHash.GetHashCode(e?.SchemaInfo.SchemaType?.QualifiedName.Name??throw new InvalidDataException("FX nugget schema type missing."));_=FXListCompiler.NuggetType(e!);Expect(at,BitConverter.GetBytes(id),path);return;}
        // Reborn: statically typed EP1 weapon slot records carry a zero class word in retail native arrays.
        if(owner==typeof(Ep1WeaponSlotHardpoint)&&name=="TypeId"){Expect(at,new byte[4],path);return;}
        if(owner==typeof(ModuleData))
        {
            if(name=="TypeId"){Expect(at,BitConverter.GetBytes(typeId),path);return;}
            if(name=="id"){var id=e?.GetAttributeNode("id")??throw new InvalidDataException("Schema-proved module requires an id.");consumed.Add(id);Expect(at,BitConverter.GetBytes(FastHash.GetHashCode(id.Value.ToLowerInvariant())),path);return;}
        }
        if(name=="Base"&&!type.IsPrimitive&&!type.IsEnum&&!type.IsGenericType){Struct(type,e,at,path,depth+1);return;}
        if(IsFlags(type)){Flags(e,name,at,Size(type),path);return;}
        if(type.IsEnum){Enumeration(e,name,at,path);return;}
        if(Scalar(type) is string codec){ScalarValue(e,name,at,codec,Size(type),path);return;}
        if(type.IsPointer){
            // Reborn: particle role wrappers contain zero or one concrete polymorphic module, not a record of the abstract pointer type.
            if(FXParticleSystemCompiler.IsModule(type.GetElementType()!)){ParticleModule(e,name,at,path,depth);return;}
            // Reborn: one required Data wrapper owns exactly one concrete EP1 button record.
            if(type.GetElementType()==typeof(Ep1PlayerPowerButtonTemplateData)){var wrapper=Child(e,name)??throw new InvalidDataException("Button Data is required.");var child=wrapper.ChildNodes.OfType<XmlElement>().Single();consumed.Add(child);Type concrete=PlayerPowerButtonCompiler.DataType(child);uint target=Word(at);Link(at,target,Size(concrete),path);Struct(concrete,child,(int)target,path+"*",depth+1);return;}
            Pointer(type.GetElementType()!,e,name,at,path,depth);return;
        }
        if(type.IsGenericType)
        {
            string generic=type.GetGenericTypeDefinition().FullName!;
            switch(generic)
            {
                // Reborn: FX materials serialize the official empty TechniqueName default as an allocated empty string, selecting TechniqueIndex.
                case "Relo.String`1":var text=Source(e,name).Text;Text(owner==typeof(FXShaderMaterial)&&name=="TechniqueName"?text??string.Empty:text,at,path,false);return;
                case "Relo.AssetReference`1":case "Relo.AssetReference`2":Reference(Source(e,name).Text,at,path);return;
                case "TypedAssetId`1":Weak(Source(e,name).Text,at,path);return;
                case "Relo.List`1":List(type.GetGenericArguments()[0],e,name,at,path,depth);return;
                case "Relo.PolymorphicList`1":
                    // Reborn: FXList owns a normal pointer array of schema-selected concrete FX nuggets.
                    // Reborn: particle events are an ordinary source-sized polymorphic pointer array.
                    if(type.GetGenericArguments()[0]==typeof(FXParticleEventBase)){ParticleEvents(e,name,at,path,depth);return;}
                    if(type.GetGenericArguments()[0]==typeof(FXNugget)){FXNuggets(e,name,at,path,depth);return;}
                    if(type.GetGenericArguments()[0]==typeof(FXShaderConstant)){ShaderConstants(e,name,at,path,depth);return;}
                    if(Children(e,name).Length!=0||Word(at)!=0||Word(at+4)!=0)throw new InvalidDataException("Schema-proved polymorphic list is not supported: "+path);return;
            }
            throw new InvalidDataException("Schema-proved generic field type is not reviewed: "+type);
        }
        if(type.Name=="StringHash"){Hash(Source(e,name).Text,at,path);return;}
        if(type.IsValueType&&!type.IsPrimitive){Struct(type,Child(e,name),at,path,depth+1);return;}
        throw new InvalidDataException("Schema-proved field type is not reviewed: "+path+" ("+type+")");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: own the shader wrapper, pointer array and concrete records in authored order; every class is derived from its source schema. */
    //-------------------------------------------------------------------------------------------------
    private void ShaderConstants(XmlElement? owner,string name,int at,string path,int depth)
    {
        var wrapper=Child(owner,name);var children=wrapper?.ChildNodes.OfType<XmlElement>().ToArray()??Array.Empty<XmlElement>();
        if(Word(at)!=children.Length)throw new InvalidDataException("Shader constant source/native count differs: "+path);uint items=Word(at+4);
        if(children.Length==0){if(items!=0)throw new InvalidDataException("Empty shader constant list has storage.");return;}
        Link(at+4,items,checked(children.Length*4),path);
        for(int i=0;i<children.Length;i++){
            var child=children[i];consumed.Add(child);uint id=FastHash.GetHashCode(child.SchemaInfo.SchemaType?.QualifiedName.Name??"");Type concrete=ShaderConstantType(id);int slot=checked((int)items+i*4);uint target=Word(slot);Link(slot,target,Size(concrete),path+"["+i+"]");Struct(concrete,child,(int)target,path+"["+i+"]",depth+1);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify every authored FX nugget pointer, concrete payload and source-owned byte without limiting nugget count or order to a sample. */
    //-------------------------------------------------------------------------------------------------
    private void FXNuggets(XmlElement? owner,string name,int at,string path,int depth)
    {
        var wrapper=Child(owner,name);var children=wrapper?.ChildNodes.OfType<XmlElement>().ToArray()??Array.Empty<XmlElement>();
        if(Word(at)!=children.Length)throw new InvalidDataException("FX nugget source/native count differs: "+path);uint items=Word(at+4);
        if(children.Length==0){if(items!=0)throw new InvalidDataException("Empty FX nugget list has storage.");return;}
        Link(at+4,items,checked(children.Length*4),path);
        for(int i=0;i<children.Length;i++){var child=children[i];consumed.Add(child);Type concrete=FXListCompiler.NuggetType(child);int slot=checked((int)items+i*4);uint target=Word(slot);Link(slot,target,Size(concrete),path+"["+i+"]");Struct(concrete,child,(int)target,path+"["+i+"]",depth+1);}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bind an optional particle role to its concrete source class and prove the complete pointed-to native allocation. */
    //-------------------------------------------------------------------------------------------------
    private void ParticleModule(XmlElement? owner,string name,int at,string path,int depth)
    {
        var wrapper=Child(owner,name);var children=wrapper?.ChildNodes.OfType<XmlElement>().ToArray()??Array.Empty<XmlElement>();if(children.Length>1)throw new InvalidDataException("Particle role has multiple concrete modules: "+path);uint target=Word(at);
        if(children.Length==0){if(target!=0)throw new InvalidDataException("Empty particle role has native storage: "+path);return;}
        var child=children[0];consumed.Add(child);Type concrete=FXParticleSystemCompiler.ModuleType(child);Link(at,target,Size(concrete),path);Struct(concrete,child,(int)target,path+"*",depth+1);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: particle events retain authored order, concrete class IDs and all independent import/relocation ownership. */
    //-------------------------------------------------------------------------------------------------
    private void ParticleEvents(XmlElement? owner,string name,int at,string path,int depth)
    {
        var wrapper=Child(owner,name);var children=wrapper?.ChildNodes.OfType<XmlElement>().ToArray()??Array.Empty<XmlElement>();if(Word(at)!=children.Length)throw new InvalidDataException("Particle event count differs: "+path);uint items=Word(at+4);
        if(children.Length==0){if(items!=0)throw new InvalidDataException("Empty particle event list has storage.");return;}Link(at+4,items,checked(children.Length*4),path);
        for(int i=0;i<children.Length;i++){var child=children[i];consumed.Add(child);Type concrete=FXParticleSystemCompiler.ModuleType(child);int slot=(int)items+i*4;uint target=Word(slot);Link(slot,target,Size(concrete),path);Struct(concrete,child,(int)target,path+"["+i+"]",depth+1);}
    }

    // Reborn: only the four official FXShaderConstant concrete families share this polymorphic ABI; unrelated polymorphic lists remain explicit unsupported layouts.
    internal static Type ShaderConstantType(uint id)=>id switch{0xA59096A6=>typeof(FXShaderConstantTexture),0xDF08DF25=>typeof(FXShaderConstantFloat),0x89181982=>typeof(FXShaderConstantInt),0xA3F84C3D=>typeof(FXShaderConstantBool),_=>throw new InvalidDataException("Unknown FX shader constant class: "+id.ToString("X8"))};

    //-------------------------------------------------------------------------------------------------
    /** Reborn: lists come from repeated child elements, or from one whitespace list attribute for leaf item types. */
    //-------------------------------------------------------------------------------------------------
    private void List(Type element,XmlElement? e,string name,int at,string path,int depth)
    {
        bool leaf=Leaf(element)||Scalar(element)!=null||element.IsEnum;string[]? tokens=null;XmlElement[] children=Array.Empty<XmlElement>();
        var attribute=e?.GetAttributeNode(name);
        if(leaf&&attribute!=null){consumed.Add(attribute);tokens=attribute.Value.Split((char[]?)null,StringSplitOptions.RemoveEmptyEntries);}
        else{children=Children(e,name);foreach(var child in children)consumed.Add(child);}
        int count=tokens?.Length??children.Length;uint stride=(uint)Size(element);
        if(Word(at)!=count)throw new InvalidDataException("Schema-proved list count differs: "+path+" source="+count+" native="+Word(at));
        uint pointer=Word(at+4);
        if(count==0){if(pointer!=0)throw new InvalidDataException("Empty schema-proved list has storage: "+path);return;}
        Link(at+4,pointer,checked((int)(count*stride)),path);
        for(int i=0;i<count;i++)
        {
            int item=checked((int)pointer+i*(int)stride);string itemPath=path+"["+i+"]";records[itemPath]=item;
            if(tokens!=null&&(Scalar(element)!=null||element.IsEnum))TokenValue(element,attribute!,tokens[i],item,itemPath);
            else if(tokens!=null)LeafValue(element,null,tokens[i],item,itemPath);
            else if(leaf)LeafValue(element,children[i],children[i].InnerText,item,itemPath);
            else Struct(element,children[i],item,itemPath,depth+1);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: an optional pointee exists exactly when its source attribute/element exists. */
    //-------------------------------------------------------------------------------------------------
    private void Pointer(Type element,XmlElement? e,string name,int at,string path,int depth)
    {
        var source=Source(e,name);uint pointer=Word(at);
        if(source.Node==null){if(pointer!=0)throw new InvalidDataException("Absent schema-proved optional record has storage: "+path);return;}
        Link(at,pointer,Size(element),path);int target=(int)pointer;string pointee=path+"*";records[pointee]=target;
        if(element.IsGenericType&&element.GetGenericTypeDefinition().FullName=="Relo.String`1"){Text(source.Text,target,pointee,source.Node is XmlElement);return;}
        if(Leaf(element)){LeafValue(element,source.Node as XmlElement,source.Text,target,pointee);return;}
        // Reborn: optional scalar/enum/flag pointees take their value from the same source attribute.
        if(source.Node is XmlAttribute){
            if(IsFlags(element)){Flags(e,name,target,Size(element),pointee);return;}
            if(element.IsEnum){Enumeration(e,name,target,pointee);return;}
            if(Scalar(element) is string pointedCodec){ScalarValue(e,name,target,pointedCodec,Size(element),pointee);return;}
        }
        Struct(element,source.Node as XmlElement??throw new InvalidDataException("Schema-proved optional record requires an element: "+path),target,pointee,depth+1);
    }

    // Reborn: scalar/enum list items reuse the single-value codecs on a detached carrier attribute holding one token.
    private void TokenValue(Type type,XmlAttribute list,string token,int at,string path)
    {
        var carrier=list.OwnerDocument.CreateElement("Item");carrier.SetAttribute("Value",token);
        if(type.IsEnum){var item=Item(list.SchemaInfo?.SchemaType as XmlSchemaSimpleType)??list.SchemaInfo?.SchemaType as XmlSchemaSimpleType;Expect(at,BitConverter.GetBytes(Ordinal(item,token.Trim(),path)),path);return;}
        Expect(at,ConscriptModuleScalarContract.Bytes(carrier,new ConscriptModuleScalarContract.Binding("Value",0,Size(type),Scalar(type)!,null,0)),path);
    }

    private void LeafValue(Type type,XmlElement? element,string? text,int at,string path)
    {
        // Reborn: shader Float/Value lists contain primitive floats with source-sized allocation, not nested synthetic value records.
        if(type==typeof(float)){var carrier=(element?.OwnerDocument??new XmlDocument()).CreateElement("Scalar");carrier.SetAttribute("Value",text??"");Expect(at,ConscriptModuleScalarContract.Bytes(carrier,new("Value",0,4,"Single",null,0)),path);return;}
        if(type.IsGenericType){string generic=type.GetGenericTypeDefinition().FullName!;
            if(generic=="Relo.String`1"){Text(text,at,path,false);return;}
            if(generic is "Relo.AssetReference`1" or "Relo.AssetReference`2"){Reference(text,at,path);return;}
            if(generic=="TypedAssetId`1"){Weak(text,at,path);return;}}
        if(type.Name=="StringHash"){Hash(text,at,path);return;}
        throw new InvalidDataException("Schema-proved list/pointer leaf item type is not reviewed: "+path+" ("+type+")");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Relo strings are a length word plus an owned NUL-terminated ASCII pointee; an absent value has no storage. */
    //-------------------------------------------------------------------------------------------------
    private void Text(string? text,int at,string path,bool lineEndings)
    {
        Range(at,8);uint length=Word(at),pointer=Word(at+4);
        if(text==null){if(length!=0||pointer!=0)throw new InvalidDataException("Absent schema-proved string has storage: "+path);return;}
        if(text.Any(c=>c>127||c=='\0'))throw new InvalidDataException("Schema-proved string must be ASCII without NUL: "+path);
        // Reborn: element text (scripts) may be serialized with either LF or CRLF line endings; nothing else may differ.
        var candidates=lineEndings?new[]{text,text.Replace("\r\n","\n",StringComparison.Ordinal),text.Replace("\r\n","\n",StringComparison.Ordinal).Replace("\n","\r\n",StringComparison.Ordinal)}:new[]{text};
        string? match=candidates.FirstOrDefault(c=>c.Length==length);if(match==null)throw new InvalidDataException("Schema-proved string length differs: "+path);
        Link(at+4,pointer,match.Length+1,path);var expected=Encoding.ASCII.GetBytes(match);
        if(!b.AsSpan((int)pointer,match.Length).SequenceEqual(expected)||b[pointer+match.Length]!=0)throw new InvalidDataException("Schema-proved string text differs: "+path);
    }

    private void Reference(string? text,int at,string path)
    {
        Range(at,4);uint index=Word(at);text=text?.Trim();
        if(string.IsNullOrEmpty(text)){if(index!=0)throw new InvalidDataException("Absent schema-proved reference has an index: "+path);return;}
        int colon=text.IndexOf(':');if(colon<=0||colon==text.Length-1)throw new InvalidDataException("Schema-proved reference is not resolved to a concrete type: "+path+" = "+text);
        var expected=new AssetId(FastHash.GetHashCode(text[..colon]),InstanceHandle.GetInstanceId(text[(colon+1)..]));
        if(index==0||index>references.Count)throw new InvalidDataException("Schema-proved reference index bounds differ: "+path);
        var actual=references[(int)index-1];if(actual.TypeId!=expected.TypeId||actual.InstanceId!=expected.InstanceId)throw new InvalidDataException("Schema-proved reference identity differs: "+path+" = "+text);
        if(!stockMode&&!imports.Add((uint)at))throw new InvalidDataException("Duplicate schema-proved import slot.");
    }

    private void Weak(string? text,int at,string path)
    {
        text=text?.Trim();uint expected=0;if(!string.IsNullOrEmpty(text)){int colon=text.IndexOf(':');expected=InstanceHandle.GetInstanceId(colon>0?text[(colon+1)..]:text);}
        Expect(at,BitConverter.GetBytes(expected),path);
    }

    private void Hash(string? text,int at,string path)=>Expect(at,BitConverter.GetBytes(string.IsNullOrEmpty(text)?0u:FastHash.GetHashCode(text)),path);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: enum ordinals are the official XSD enumeration positions, never the producer's C# enum ordinals. */
    //-------------------------------------------------------------------------------------------------
    private void Enumeration(XmlElement? e,string name,int at,string path)
    {
        var attribute=e?.GetAttributeNode(name);int value=0;
        if(attribute!=null){consumed.Add(attribute);value=Ordinal(attribute.SchemaInfo?.SchemaType as XmlSchemaSimpleType,attribute.Value.Trim(),path);}
        else if(Children(e,name).Length!=0)throw new InvalidDataException("Schema-proved enum element form is not reviewed: "+path);
        Expect(at,BitConverter.GetBytes(value),path);
    }

    private void Flags(XmlElement? e,string name,int at,int width,string path)
    {
        var bytes=new byte[width];var attribute=e?.GetAttributeNode(name);
        if(attribute!=null)
        {
            consumed.Add(attribute);var list=attribute.SchemaInfo?.SchemaType as XmlSchemaSimpleType;var item=Item(list)??throw new InvalidDataException("Schema-proved flag list type missing: "+path);
            foreach(string token in attribute.Value.Split((char[]?)null,StringSplitOptions.RemoveEmptyEntries))
            {
                if(token[0] is '+' or '-')throw new InvalidDataException("Schema-proved prefixed flag operations are not reviewed: "+path);
                // Reborn: SAGE ALL fills the entire mask even when an XSD enumeration includes ALL as a sentinel literal rather than an ordinary bit.
                if(token=="ALL"){Array.Fill(bytes,(byte)255);continue;}
                int bit=TryOrdinal(item,token);
                if(bit==int.MinValue)throw new InvalidDataException("Schema-proved flag is not an official enumeration value: "+path+" = "+token);
                if(bit<0)continue;
                if(bit>=width*8)throw new InvalidDataException("Schema-proved flag exceeds native width: "+path+" = "+token);bytes[bit/8]|=(byte)(1<<(bit%8));
            }
        }
        else if(Children(e,name).Length!=0)throw new InvalidDataException("Schema-proved flag element form is not reviewed: "+path);
        Expect(at,bytes,path);
    }

    private void ScalarValue(XmlElement? e,string name,int at,string codec,int width,string path)
    {
        if(e==null){Expect(at,new byte[width],path);return;}
        var attribute=e.GetAttributeNode(name);if(attribute!=null)consumed.Add(attribute);
        else if(Children(e,name).Length!=0)throw new InvalidDataException("Schema-proved scalar element form is not reviewed: "+path);
        Expect(at,ConscriptModuleScalarContract.Bytes(e,new ConscriptModuleScalarContract.Binding(name,0,width,codec,null,0)),path);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: every authored attribute and child of the leaf must have been consumed by a native field. */
    //-------------------------------------------------------------------------------------------------
    private void Consumed(XmlElement root)
    {
        foreach(var element in root.SelectNodes("descendant-or-self::*")!.OfType<XmlElement>())
        {
            if(element!=root&&!consumed.Contains(element))throw SdkXmlDiagnostics.At(new InvalidDataException("Source element has no reviewed native field: "+element.LocalName),element);
            foreach(XmlAttribute attribute in element.Attributes)
                // Reborn: official pipelineOnly attributes (such as CrusherInfo/id) participate in XML inheritance but have no runtime native field.
                if(attribute.Specified&&attribute.NamespaceURI.Length==0&&attribute.LocalName!="TypeId"&&!consumed.Contains(attribute)&&attribute.SchemaInfo?.SchemaAttribute?.UnhandledAttributes?.Any(a=>a.LocalName=="pipelineOnly"&&a.NamespaceURI=="uri:ea.com:eala:asset:schema"&&a.Value=="true")!=true)throw SdkXmlDiagnostics.At(new InvalidDataException("Source attribute has no reviewed native field: "+element.LocalName+"/@"+attribute.LocalName),attribute);
        }
    }

    private (XmlNode? Node,string? Text) Source(XmlElement? e,string name)
    {
        if(e==null)return (null,null);var attribute=e.GetAttributeNode(name);
        // Reborn: an unauthored schema default of "" (e.g. SoundOrEvaEvent/@EvaEvent) is absent in stock output: no string or pointee storage.
        if(attribute!=null&&!attribute.Specified&&attribute.Value.Length==0){consumed.Add(attribute);return (null,null);}
        if(attribute!=null){consumed.Add(attribute);return (attribute,attribute.Value);}
        var children=Children(e,name);if(children.Length>1)throw new InvalidDataException("Schema-proved single field has repeated source elements: "+name);
        if(children.Length==1){consumed.Add(children[0]);return (children[0],children[0].InnerText);}return (null,null);
    }

    private XmlElement? Child(XmlElement? e,string name){var children=Children(e,name);if(children.Length>1)throw new InvalidDataException("Schema-proved record has repeated source elements: "+name);if(children.Length==1)consumed.Add(children[0]);return children.SingleOrDefault();}
    private static XmlElement[] Children(XmlElement? e,string name)=>e==null?Array.Empty<XmlElement>():e.ChildNodes.OfType<XmlElement>().Where(c=>c.LocalName==name).ToArray();

    // Reborn: a pointer word must be in bounds, owned exactly once and (for compiled leaves) appear in the relocation inventory.
    private void Link(int slot,uint pointer,int size,string path)
    {
        if(pointer==0||pointer>b.Length)throw new InvalidDataException("Schema-proved pointer presence/bounds differ: "+path);
        if(!stockMode&&!relos.Add((uint)slot))throw new InvalidDataException("Duplicate schema-proved relocation slot.");Claim((int)pointer,size);
    }

    private void Claim(int at,int size)
    {
        Range(at,size);if(stockMode)return;
        if(covered.AsSpan(at,size).Contains(true))throw new InvalidDataException("Schema-proved allocation overlaps another owner at +"+at);Array.Fill(covered,true,at,size);
    }

    // Reborn: field-sized hex evidence identifies actual ABI/default mismatches without dumping unrelated asset buffers.
    private void Expect(int at,byte[] expected,string path){Range(at,expected.Length);if(!b.AsSpan(at,expected.Length).SequenceEqual(expected))throw new InvalidDataException("Schema-proved native value differs: "+path+" expected="+Convert.ToHexString(expected)+" actual="+Convert.ToHexString(b.AsSpan(at,expected.Length)));}
    private void Range(int at,int size){if(at<0||size<0||at>b.Length-size)throw new InvalidDataException("Schema-proved native bounds differ.");}
    private uint Word(int at){Range(at,4);return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}

    private static int Ordinal(XmlSchemaSimpleType? type,string token,string path){int value=TryOrdinal(type,token);if(value==int.MinValue)throw new InvalidDataException("Schema-proved enum value is not an official enumeration: "+path+" = "+token);return value;}
    // Reborn: retail TerrainScorch starts RANDOM at -1, so SCORCH_1..16 and TERRAIN_MAPPED follow the sentinel rather than zero-based XSD positions.
    private static int TryOrdinal(XmlSchemaSimpleType? type,string token)
    {
        for(int depth=0;type!=null&&depth<16;depth++)
        {
            if(type.Content is XmlSchemaSimpleTypeRestriction restriction)
            {
                var facets=restriction.Facets.OfType<XmlSchemaEnumerationFacet>().ToArray();var values=facets.Select(f=>f.Value).ToList();
                // Reborn: a leading official INVALID sentinel is ordinal -1 (not stored), shifting every other value down by one.
                // Reborn: stock BuildPlacementType and AIUpgradeHeuristicType keep INVALID at ordinal zero; stock upgrade IGNORE is 5, not 4.
                // Reborn: schema forceValue aliases reuse an earlier ordinal and do not consume a new native flag bit.
                if(values.Count>0){int index=values.IndexOf(token);if(index<0)return int.MinValue;int next=(values[0]=="INVALID"&&type.QualifiedName.Name is not ("BuildPlacementType" or "AIUpgradeHeuristicType" or "FXParticleSystem_GeometryType")||type.QualifiedName.Name=="TerrainScorchType"&&values[0]=="RANDOM")?-1:0;var ordinals=new Dictionary<string,int>(StringComparer.Ordinal);for(int i=0;i<=index;i++){string? alias=facets[i].UnhandledAttributes?.FirstOrDefault(a=>a.LocalName=="forceValue")?.Value;if(alias!=null){if(!int.TryParse(alias,System.Globalization.NumberStyles.Integer,System.Globalization.CultureInfo.InvariantCulture,out next)&&!ordinals.TryGetValue(alias,out next))throw new InvalidDataException("Schema enum alias target is not earlier: "+alias);}ordinals[values[i]!]=next;if(i==index)return next;next++;}}
            }
            type=type.BaseXmlSchemaType as XmlSchemaSimpleType;
        }
        return int.MinValue;
    }
    private static XmlSchemaSimpleType? Item(XmlSchemaSimpleType? type)
    {
        for(int depth=0;type!=null&&depth<16;depth++){if(type.Content is XmlSchemaSimpleTypeList list)return list.BaseItemType;type=type.BaseXmlSchemaType as XmlSchemaSimpleType;}
        return null;
    }

    internal static bool IsFlags(Type type){var fields=type.IsValueType&&!type.IsPrimitive&&!type.IsEnum?type.GetFields(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic):Array.Empty<FieldInfo>();return fields.Length==1&&fields[0].Name=="Value"&&fields[0].GetCustomAttribute<FixedBufferAttribute>()?.ElementType==typeof(uint);}
    private static string? Scalar(Type type)=>type.Name switch{"Single" or "Int32" or "UInt32" or "UInt16" or "Byte" or "SByte" or "SageBool" or "Time" or "Angle" or "Percentage" or "Velocity" or "Duration"=>type.Name,_=>null};
    internal static bool Leaf(Type type)=>type.IsGenericType?type.GetGenericTypeDefinition().FullName is "Relo.String`1" or "Relo.AssetReference`1" or "Relo.AssetReference`2" or "TypedAssetId`1":type.Name=="StringHash";

    internal static int Size(Type type)=>type.IsPointer?4:type.IsEnum?Marshal.SizeOf(Enum.GetUnderlyingType(type)):type.IsGenericType?type.GetGenericTypeDefinition().FullName switch{"Relo.List`1" or "Relo.String`1" or "Relo.PolymorphicList`1"=>8,"Relo.AssetReference`1" or "Relo.AssetReference`2" or "TypedAssetId`1"=>4,_=>throw new InvalidDataException("Unknown schema-proved generic size: "+type)}:Marshal.SizeOf(type);
    private static int FieldSize(Type type)=>Size(type);

    private static uint[] Slots(byte[] bytes)
    {
        if(bytes.Length==0)return Array.Empty<uint>();if(bytes.Length<8||bytes.Length%4!=0||BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(bytes.Length-4))!=uint.MaxValue)throw new InvalidDataException("Schema-proved auxiliary framing differs.");
        return Enumerable.Range(0,bytes.Length/4-1).Select(i=>BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(i*4,4))).ToArray();
    }
}
