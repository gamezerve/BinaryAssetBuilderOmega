using System.Buffers.Binary;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using SageBinaryData;
using Chunk=Relo.Chunk;
using Tracker=Relo.Tracker;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: actual native root/destruction/EP1 body allocations share references and one token/native coordinate system, not a complete GameObject asset.
internal static class GameObjectBodyProjectionSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile and compose dynamic strings, two root imports, destruction FX and body/range imports as one reviewed native graph. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string staticManifest,string globalManifest)
    {
        var saved=Settings.Current;
        try
        {
            XmlSchemaSet? official=null;SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:s=>official=s);
            if(official==null)throw new InvalidDataException("Reviewed root/body schema required.");
            Settings.Current=new Settings {SchemaPath=Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"RootBodyPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);
            foreach(string name in new[]{"UnitTypeIcon","UnitIntro"})
            {
                var full=(XmlSchemaComplexType)official.GlobalTypes[new XmlQualifiedName("GameObject",SchemaSet.XmlNamespace)]!;
                var focus=(XmlSchemaComplexType)schemas.Schemas.GlobalTypes[new XmlQualifiedName("RootBodyProbeType",SchemaSet.XmlNamespace)]!;
                var a=(XmlSchemaAttribute)full.AttributeUses[new XmlQualifiedName(name)]!;var b=(XmlSchemaAttribute)focus.AttributeUses[new XmlQualifiedName(name)]!;
                if(a.AttributeSchemaType!.QualifiedName!=b.AttributeSchemaType!.QualifiedName || a.DefaultValue!=b.DefaultValue || a.Use!=b.Use)throw new InvalidDataException("Root/body reference declaration drift.");
            }
            var catalog=new[]{Catalog(staticManifest),Catalog(globalManifest)}.SelectMany(d=>d.Assets).ToArray();
            var targets=new[]{"UnitTypeIcon","UnitIntro","FXList","AttributeModifier"}.Select(type=>catalog.First(a=>a.TypeId==FastHash.GetHashCode(type))).ToArray();
            Chunk? populated=null;InstanceDeclaration? populatedInstance=null;int cases=0;
            foreach(int count in new[]{0,1,3,64})foreach(bool strings in new[]{false,true})
            {
                string ranges=string.Concat(Enumerable.Repeat($"<DamageRangeModifier Min='25%' Max='75%' AttributeModifier='{targets[3].Name}'/>",count));
                string source=$"<RootBodyProbe xmlns='uri:ea.com:eala:asset' id='CombinedBody' UnitTypeIcon='{targets[0].Name}' UnitIntro='{targets[1].Name}'><BuildingDestructionBehavior id='ModuleTag_Destroy'><DieMuxData/><Item FX='{targets[2].Name}' BonePrefix='BONE_'/></BuildingDestructionBehavior><ActiveBody id='ModuleTag_Body' MaxHealth='4321' HealingBuffFX='{targets[2].Name}' DamagedAttributeModifier='{targets[3].Name}' ReallyDamagedAttributeModifier='{targets[3].Name}' BurningDeathFX='{targets[2].Name}'>{ranges}</ActiveBody></RootBodyProbe>";
                Validate(official,Adapt(Xml(source).DocumentElement!,strings).OuterXml);
                var instance=Normalize(schemas,source,catalog);var native=Compile(Adapt(instance.XmlNode,strings));var output=Check(native,instance,count,targets);
                var repeat=Check(Compile(Adapt(Normalize(schemas,source,catalog).XmlNode,strings)),instance,count,targets);
                if(!output.InstanceBuffer.SequenceEqual(repeat.InstanceBuffer) || !output.RelocationBuffer.SequenceEqual(repeat.RelocationBuffer) || !output.ImportsBuffer.SequenceEqual(repeat.ImportsBuffer))throw new InvalidDataException("Root/body composed repeat differs.");
                if(count==3 && strings){populated=native;populatedInstance=instance;}cases++;
            }
            // Reborn: root Body token type and native slot are corroborated against independent stock metadata, not inferred from the new C# class name.
            var stock=GameObjectStockReview.Capture(staticManifest,"GameObject:AlliedConstructionYard");
            var bodyRoot=GameObjectStockReview.Tokens(stock.Bytes,0)[FastHash.GetHashCode("Body")];
            if(bodyRoot.Type!=0xAFAD56F4 || Word(stock.Bytes,stock.Projection.NativeRoot+488)<stock.Projection.NativeRoot+600)throw new InvalidDataException("Stock Body root contract differs.");
            int body=(int)Word(populated!.InstanceBuffer,488),list=(int)Word(populated.InstanceBuffer,476),module=(int)Word(populated.InstanceBuffer,list+8);int failures=0;
            foreach(var fault in new[]{(488,4u),(488,(uint)module),(488,(uint)(body+4)),(body,0u),(body+68,(uint)(module+72)),(module+68,(uint)(body+76)),(body+64,65u)})
            {var bad=Clone(populated);Put(bad.InstanceBuffer,fault.Item1,fault.Item2);Refuse(()=>Ep1GameObjectDestructionProjection.Compose(bad));failures++;}
            var missing=Clone(populated);missing.RelocationBuffer=Pack(Slots(missing.RelocationBuffer).Where(s=>s!=488));Refuse(()=>Ep1GameObjectDestructionProjection.Compose(missing));failures++;
            var missingImport=Clone(populated);missingImport.ImportsBuffer=Pack(Slots(missingImport.ImportsBuffer).Where(s=>s!=body+44));Refuse(()=>Ep1GameObjectDestructionProjection.Compose(missingImport));failures++;
            // Reborn: an out-of-buffer import must not disappear when native owners are partitioned into fragments.
            var outside=Clone(populated);outside.ImportsBuffer=Pack(Slots(outside.ImportsBuffer).Append((uint)outside.InstanceBuffer.Length));Refuse(()=>Ep1GameObjectDestructionProjection.Compose(outside));failures++;
            var alias=Clone(populated);Put(alias.InstanceBuffer,56,(uint)body);Put(alias.InstanceBuffer,52,0);alias.RelocationBuffer=Pack(Slots(alias.RelocationBuffer).Append(56u));Refuse(()=>Ep1GameObjectDestructionProjection.Compose(alias));failures++;
            // Reborn: corrupt generated metadata independently of the producer and require fresh composed graph verification to reject it.
            var valid=Check(populated,populatedInstance!,3,targets);var rootTokens=GameObjectStockReview.Tokens(valid.InstanceBuffer,0);
            int bodyTable=rootTokens[FastHash.GetHashCode("Body")].Offset,metadataFailures=0;
            foreach(int at in Enumerable.Range(0,Ep1GameObjectRootValueTokens.AllFields.Count).SelectMany(i=>new[]{i*12+4,i*12+8}).Concat(new[]{(Ep1GameObjectRootValueTokens.AllFields.Count+1)*12+4,(Ep1GameObjectRootValueTokens.AllFields.Count+1)*12+8,bodyTable+4,bodyTable+8,bodyTable+16,bodyTable+20,bodyTable+236,bodyTable+260,bodyTable+320,bodyTable+332,bodyTable+364,bodyTable+368,bodyTable+376,bodyTable+380,bodyTable+388,bodyTable+392}))
            {var bad=Clone(valid);Put(bad.InstanceBuffer,at,Word(bad.InstanceBuffer,at)^4u);Refuse(()=>Verify(bad,populated,populatedInstance!,3,targets));metadataFailures++;}
            Console.WriteLine($"GameObject body composition: PASS ({cases} actual-Core/native cases, dynamic root strings, one root/behavior/body reference table, 0/1/3/64 ranges, 88 root tokens and complete native/RELO/IMP round trips; {failures} owner and {metadataFailures} metadata faults refused; explicit EP1 allocator, no general source/asset publication/package/game).");
        }
        finally{Settings.Current=saved;}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: admit real EP1 metadata only and require unique concrete identities for every source occurrence. */
    //-------------------------------------------------------------------------------------------------
    private static ManifestDocument Catalog(string path){var d=ManifestReader.Read(SdkEnvironmentPreflight.Read(path,64*1048576));TypeRegistryAudit.ValidateTarget(d.Header.Version,d.Header.AllTypesHash);if(!d.Header.IsLinked || d.Validate().Count!=0)throw new InvalidDataException("Valid root/body catalog required.");return d;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: actual Core normalization runs once over the whole selected graph; indices are never authored or patched after compilation. */
    //-------------------------------------------------------------------------------------------------
    private static InstanceDeclaration Normalize(SchemaSet schemas,string source,ManifestAsset[] catalog)
    {
        if(source.Contains('\\'))throw new InvalidDataException("Authored root/body indices refused.");var instance=ReferencePipelineSmokeTest.Normalize(schemas,source);
        foreach(var r in instance.ReferencedInstances)if(catalog.Count(a=>a.TypeId==r.TypeId && a.InstanceId==r.InstanceId)!=1)throw new InvalidDataException("Missing/ambiguous root/body target.");return instance;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: rearrange the focused diagnostic envelope into actual root/Behaviors/Body syntax without creating another dependency table. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Adapt(XmlNode probe,bool strings)
    {
        var d=Xml("<GameObject xmlns='uri:ea.com:eala:asset' id='CombinedBody'><Behaviors/><Body/></GameObject>");
        foreach(string name in new[]{"UnitTypeIcon","UnitIntro"})d.DocumentElement!.SetAttribute(name,probe.Attributes![name]!.Value);
        if(strings)d.DocumentElement!.SetAttribute("EditorName",new string('R',300));
        var children=probe.ChildNodes.OfType<XmlElement>().ToArray();d.DocumentElement!.FirstChild!.AppendChild(d.ImportNode(children[0],true));d.DocumentElement.LastChild!.AppendChild(d.ImportNode(children[1],true));return d;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: raw authored Body is already present, so validation must not append the older required-body placeholder. */
    //-------------------------------------------------------------------------------------------------
    internal static void Validate(XmlSchemaSet schemas,string source){var b=SdkEffectiveSchema.Bind(schemas,"in-memory-root-body.xml",Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+source+"</AssetDeclaration>"));if(!b.XmlValidated)throw new InvalidDataException("Raw root/body source refused: "+string.Join("; ",b.Errors));}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: opt into the separate EP1 allocator after existing root/behavior dispatch, on the same Tracker and native Body pointer slot. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlDocument source)
    {
        var d=(XmlDocument)source.CloneNode(true);var body=(XmlElement)d.DocumentElement!.LastChild!.FirstChild!;var bodyContainer=body.ParentNode!;d.DocumentElement.RemoveChild(bodyContainer);
        // Reborn: direct ArmorSet children may precede Behaviors; never treat the first root child as the behavior container.
        ((XmlElement)d.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace)[0]!.FirstChild!).SetAttribute("TypeId","227975924");body.SetAttribute("TypeId","1510882715");
        var ns=new XmlNamespaceManager(d.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);
        GameObject* root;using var tracker=new Tracker((void**)&root,(uint)sizeof(GameObject),false);
        Marshaler.Marshal(new Node(d.DocumentElement.CreateNavigator()!,ns),root,tracker);
        Marshaler.MarshalEp1ActiveBody(new Node(body.CreateNavigator()!,ns),&root->Body,tracker);
        var chunk=new Chunk();if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("Root/body allocation failed.");return chunk;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently verify root/container/body chains, shared occurrence identities, token owners and inverse native/auxiliary rebasing. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Check(Chunk native,InstanceDeclaration instance,int count,ManifestAsset[] targets)
    {
        var result=Ep1GameObjectDestructionProjection.Compose(native);Verify(result,native,instance,count,targets);return result;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate detached output without re-running the producer, including cross-fragment metadata targets and preserved native owners. */
    //-------------------------------------------------------------------------------------------------
    private static void Verify(Chunk result,Chunk native,InstanceDeclaration instance,int count,ManifestAsset[] targets)
    {
        var b=result.InstanceBuffer;var root=GameObjectStockReview.Tokens(b,0);
        int prefix=root[FastHash.GetHashCode("BuildTime")].Offset-140,body=(int)Word(native.InstanceBuffer,488),list=(int)Word(native.InstanceBuffer,476),module=(int)Word(native.InstanceBuffer,list+8);
        int container=(Ep1GameObjectRootValueTokens.AllFields.Count+3)*12,destruction=container+24,bodyTable=destruction+384;
        if(root.Count!=88 || root[FastHash.GetHashCode("Body")].Type!=0xAFAD56F4 || root[FastHash.GetHashCode("Body")].Offset!=bodyTable || prefix!=bodyTable+324+count*60 || Word(b,prefix+488)!=prefix+body || Word(b,bodyTable)!=2 || Word(b,bodyTable+4)!=0x5A0E3D9B || Word(b,bodyTable+8)!=bodyTable+12 || Word(b,prefix+body)!=0x5A0E3D9B || root[FastHash.GetHashCode("Behaviors")].Offset!=container || Word(b,container+8)!=prefix+list || Word(b,container+20)!=destruction)throw new InvalidDataException("Composed root/body/behavior chain differs.");
        foreach(var f in Ep1GameObjectRootValueTokens.AllFields)if(root[FastHash.GetHashCode(f.Name)].Type!=f.Type || root[FastHash.GetHashCode(f.Name)].Offset!=prefix+f.Offset)throw new InvalidDataException("Composed root field ownership differs.");
        if(Word(b,bodyTable+12)!=1 || Word(b,bodyTable+16)!=FastHash.GetHashCode("BodyModuleData") || Word(b,bodyTable+20)!=bodyTable+228 || Word(b,bodyTable+236)!=bodyTable+252 || Word(b,bodyTable+260)!=bodyTable+276)throw new InvalidDataException("Composed body base chain differs.");
        var bodyFields=GameObjectStockReview.Tokens(b,bodyTable+12);
        foreach(var f in Ep1ActiveBodyTokenizer.Fields)if(bodyFields[FastHash.GetHashCode(f.Name)].Type!=f.Type || bodyFields[FastHash.GetHashCode(f.Name)].Offset!=prefix+body+f.Offset)throw new InvalidDataException("Composed body field target differs.");
        if(bodyFields[FastHash.GetHashCode("DamageRangeModifier")].Offset!=bodyTable+312 || Word(b,bodyTable+320)!=prefix+body+64)throw new InvalidDataException("Composed body range count binding differs.");
        for(int i=0;i<count;i++)
        {
            int table=bodyTable+324+count*12+i*48;
            if(Word(b,bodyTable+324+i*12)!=0xB151A0EB || Word(b,bodyTable+332+i*12)!=table)throw new InvalidDataException("Composed body child table ownership differs.");
            var fields=GameObjectStockReview.Tokens(b,table);
            foreach(var f in new[]{("Min",0,0x645864F3u),("Max",4,0x645864F3u),("AttributeModifier",8,0x09DDCFF3u)})
                if(!fields.TryGetValue(FastHash.GetHashCode(f.Item1),out var token) || token.Type!=f.Item3 || token.Offset!=prefix+body+76+i*12+f.Item2)throw new InvalidDataException("Composed body child target differs.");
        }
        var destructionFields=GameObjectStockReview.Tokens(b,destruction+12);int itemTable=destructionFields[FastHash.GetHashCode("Item")].Offset;
        if(Word(b,itemTable+8)!=prefix+module+64)throw new InvalidDataException("Composed destruction count binding differs.");
        var itemFields=GameObjectStockReview.Tokens(b,(int)Word(b,itemTable+20));
        foreach(var f in new[]{("BonePrefix",0),("FX",8),("CreationObject",12),("FlingVelocity",16)})if(itemFields[FastHash.GetHashCode(f.Item1)].Offset!=prefix+module+72+f.Item2)throw new InvalidDataException("Composed destruction child target differs.");
        int items=(int)Word(native.InstanceBuffer,module+68);var slots=new[]{368,400,items+8,body+44,body+48,body+52,body+60}.Concat(Enumerable.Range(0,count).Select(i=>body+76+i*12+8)).ToArray();
        if(instance.ReferencedInstances.Count!=slots.Length)throw new InvalidDataException("Combined occurrence count differs.");
        for(int i=0;i<slots.Length;i++)
        {
            var target=targets[i==0?0:i==1?1:i is 2 or 3 or 6?2:3];var r=instance.ReferencedInstances[i];
            if(r.TypeId!=target.TypeId || r.InstanceId!=target.InstanceId || Word(b,prefix+slots[i])!=i+1)throw new InvalidDataException("Combined root/behavior/body import index/identity differs.");
        }
        var recovered=b.AsSpan(prefix).ToArray();
        foreach(uint slot in Slots(result.RelocationBuffer)){if(slot<prefix || Word(b,(int)slot)<prefix)throw new InvalidDataException("Composed pointer escapes native graph.");Put(recovered,(int)slot-prefix,Word(b,(int)slot)-(uint)prefix);}
        if(!recovered.SequenceEqual(native.InstanceBuffer) || !Slots(result.RelocationBuffer).Select(s=>s-(uint)prefix).SequenceEqual(Slots(native.RelocationBuffer)) || !Slots(result.ImportsBuffer).Select(s=>s-(uint)prefix).SequenceEqual(Slots(native.ImportsBuffer)) || !Slots(result.ImportsBuffer).OrderBy(s=>s).SequenceEqual(slots.Select(s=>(uint)(prefix+s)).OrderBy(s=>s)))throw new InvalidDataException("Combined native/RELO/IMP round trip differs.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: confined fixture DOMs disable external entity resolution. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Xml(string text){var d=new XmlDocument {XmlResolver=null};d.LoadXml(text);return d;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: mutation controls never edit live compiler buffers. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c)=>new(){InstanceBuffer=c.InstanceBuffer.ToArray(),RelocationBuffer=c.RelocationBuffer.ToArray(),ImportsBuffer=c.ImportsBuffer.ToArray()};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: absent auxiliary streams are not synthetic sentinels. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b)=>b.Length==0?Array.Empty<uint>():Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();
    //-------------------------------------------------------------------------------------------------
    /** Reborn: detached owner faults preserve final sentinel encoding. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> slots){var a=slots.Append(uint.MaxValue).ToArray();var b=new byte[a.Length*4];for(int i=0;i<a.Length;i++)Put(b,i*4,a[i]);return b;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: cross-owner pointers must fail before producing a tokenized projection. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action a){try{a();}catch(InvalidDataException){return;}throw new InvalidDataException("Malformed root/body owner admitted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: native/token reads use bounded little-endian words. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));
    //-------------------------------------------------------------------------------------------------
    /** Reborn: only owned diagnostic chunks are mutated. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
