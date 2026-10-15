using System.Buffers.Binary;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: geometry completes another selected owner in the shared movement/armor/body/three-behavior graph, not a generally publishable GameObject.
internal static class GameObjectGeometryCompositionSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: batch actual-Core target closure, dynamic owner order, generated geometry metadata and optional genuinely new compiled armor in one partial object. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string globalManifest,string? authoredDirectory=null)
    {
        var saved=Settings.Current;
        try
        {
            // Reborn: this native-only optional-Body boundary is not claimed to satisfy the official required-Body source schema.
            Minimal();
            XmlSchemaSet? official=null;SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:s=>official=s);if(official==null)throw new InvalidDataException("Reviewed geometry composition schema required.");
            Settings.Current=new Settings{SchemaPath=Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"RootGeometryPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};var schemas=new SchemaSet(false);
            var stock=new[]{Catalog(manifest),Catalog(globalManifest)}.SelectMany(d=>d.Assets).ToArray();var catalog=stock;ManifestAsset? authored=null;
            if(authoredDirectory!=null){var built=GameObjectArmorCompositionSmokeTest.BuildArmor(authoredDirectory);authored=built.Assets.Single(a=>a.Name=="ArmorTemplate:RebornComposedArmor");if(catalog.Any(a=>a.TypeId==authored.TypeId && a.InstanceId==authored.InstanceId))throw new InvalidDataException("Geometry authored armor target collides.");catalog=catalog.Concat(built.Assets).ToArray();}
            string[] types={"UnitTypeIcon","UnitIntro","ArmorTemplate","DamageFX","FXList","AttributeModifier","WeaponTemplate","UpgradeTemplate","LocomotorTemplate"};var targets=types.Select(t=>catalog.First(a=>a.TypeId==FastHash.GetHashCode(t))).ToArray();if(authored!=null)targets[2]=authored;
            int cases=0;Chunk? populated=null;InstanceDeclaration? populatedInstance=null;
            foreach(int profile in new[]{0,1,2})foreach(bool moving in new[]{false,true})foreach(bool reverse in new[]{false,true})foreach(int position in new[]{0,1,2})foreach(bool strings in new[]{false,true})
            {
                var pair=Compile(Source(targets,profile,moving,reverse,position),strings,catalog);var output=Ep1GameObjectDestructionProjection.Compose(pair.Native);Verify(output,pair.Native,pair.Instance,targets,profile,moving);
                var repeat=Compile(Source(targets,profile,moving,reverse,position),strings,catalog);var other=Ep1GameObjectDestructionProjection.Compose(repeat.Native);
                if(!output.InstanceBuffer.SequenceEqual(other.InstanceBuffer) || !output.RelocationBuffer.SequenceEqual(other.RelocationBuffer) || !output.ImportsBuffer.SequenceEqual(other.ImportsBuffer))throw new InvalidDataException("Geometry composition repeat differs.");
                if(profile==2 && moving && !reverse && position==0 && strings){populated=pair.Native;populatedInstance=pair.Instance;}cases++;
            }
            int geometry=(int)Word(populated!.InstanceBuffer,500),body=(int)Word(populated.InstanceBuffer,488),list=(int)Word(populated.InstanceBuffer,476),first=(int)Word(populated.InstanceBuffer,list+8),failures=0,metadataFailures=0;
            foreach(var fault in new[]{(500,0u),(500,(uint)body),(500,(uint)first),(500,(uint)(geometry+2)),(500,(uint)(populated.InstanceBuffer.Length-75)),(geometry+12,Word(populated.InstanceBuffer,424)),(geometry,(uint)body),(body+68,(uint)geometry),(geometry+24,2u)}){var bad=Clone(populated);Put(bad.InstanceBuffer,fault.Item1,fault.Item2);Refuse(()=>Ep1GameObjectDestructionProjection.Compose(bad));failures++;}
            var missing=Clone(populated);missing.RelocationBuffer=Pack(Slots(missing.RelocationBuffer).Where(s=>s!=500));Refuse(()=>Ep1GameObjectDestructionProjection.Compose(missing));failures++;
            var imported=Clone(populated);imported.ImportsBuffer=Pack(Slots(imported.ImportsBuffer).Append(500u));Refuse(()=>Ep1GameObjectDestructionProjection.Compose(imported));failures++;
            var tail=Clone(populated);tail.InstanceBuffer=tail.InstanceBuffer.Concat(new byte[4]).ToArray();Refuse(()=>Ep1GameObjectDestructionProjection.Compose(tail));failures++;
            string valid=Source(targets,2,true,false,0);
            foreach(string bad in new[]{valid.Replace("Type='RING'","Type='UNKNOWN'",StringComparison.Ordinal),valid.Replace(targets[8].Name,"MissingRebornGeometryLocomotor",StringComparison.Ordinal)}){Refuse(()=>{var p=Compile(bad,true,catalog);Ep1GameObjectDestructionProjection.Compose(p.Native);});failures++;}
            // Reborn: equal source strings in different fragments are valid when the allocator gives each fragment its own data, not a semantic name collision.
            var sameText=Compile(valid.Replace("Name='geometry shape'","Name='BONE_'",StringComparison.Ordinal),true,catalog);Verify(Ep1GameObjectDestructionProjection.Compose(sameText.Native),sameText.Native,sameText.Instance,targets,2,true);cases++;
            var crossOwner=Clone(populated);int shapeItems=(int)Word(crossOwner.InstanceBuffer,geometry+12),destruction=(int)Word(crossOwner.InstanceBuffer,list+12),destructionItems=(int)Word(crossOwner.InstanceBuffer,destruction+68);Put(crossOwner.InstanceBuffer,shapeItems+8,5);Put(crossOwner.InstanceBuffer,shapeItems+12,Word(crossOwner.InstanceBuffer,destructionItems+4));Refuse(()=>Ep1GameObjectDestructionProjection.Compose(crossOwner));failures++;
            if(authored!=null){Refuse(()=>Compile(valid,false,stock));failures++;}
            var good=Ep1GameObjectDestructionProjection.Compose(populated);var roots=GameObjectStockReview.Tokens(good.InstanceBuffer,0);int table=roots[FastHash.GetHashCode("Geometry")].Offset,shapeList=(int)Word(good.InstanceBuffer,table+44),shapeTable=(int)Word(good.InstanceBuffer,shapeList+20),contactList=(int)Word(good.InstanceBuffer,table+56),contactTable=(int)Word(good.InstanceBuffer,contactList+20),container=roots[FastHash.GetHashCode("Behaviors")].Offset;
            foreach(int at in new[]{table+8,table+20,table+32,table+44,table+56,table+60,shapeList+8,shapeList+20,shapeTable+8,shapeTable+128,contactList+8,contactList+20,contactTable+8,contactTable+20,container+8,container+20,roots[FastHash.GetHashCode("Body")].Offset+8}){var bad=Clone(good);Put(bad.InstanceBuffer,at,Word(bad.InstanceBuffer,at)^4u);Refuse(()=>Verify(bad,populated,populatedInstance!,targets,2,true));metadataFailures++;}
            Console.WriteLine($"GameObject geometry composition: PASS ({cases} actual-Core/native cases, absent/simple/complex geometry, movement, six behavior orders, armor/body/weapon/ranges/strings; {failures} input and {metadataFailures} metadata refusals; up to {Ep1GameObjectRootValueTokens.AllFields.Count+5} reviewed root fields, no general processor/full object publication/game).");
            if(authored!=null)Console.WriteLine("Authored armor + geometry combat consumer: PASS (genuine new Core target joins all selected owners; removal refuses; no complete consumer stream/game).");

            //-------------------------------------------------------------------------------------------------
            /** Reborn: official raw validation precedes actual Core normalization and unique independent catalog admission. */
            //-------------------------------------------------------------------------------------------------
            (Chunk Native,InstanceDeclaration Instance) Compile(string source,bool strings,ManifestAsset[] admitted)
            {
                if(source.Contains('\\'))throw new InvalidDataException("Authored geometry reference indices refused.");var raw=Xml(source);GameObjectBodyProjectionSmokeTest.Validate(official,GameObjectCombatCompositionSmokeTest.Adapt(raw.DocumentElement!,strings).OuterXml);var instance=ReferencePipelineSmokeTest.Normalize(schemas,source);
                foreach(var reference in instance.ReferencedInstances)if(admitted.Count(a=>a.TypeId==reference.TypeId && a.InstanceId==reference.InstanceId)!=1)throw new InvalidDataException("Geometry composition target missing/ambiguous.");
                return(GameObjectBodyProjectionSmokeTest.Compile(GameObjectCombatCompositionSmokeTest.Adapt(instance.XmlNode,strings),selectedBehaviors:true),instance);
            }
        }
        finally{Settings.Current=saved;}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: reference catalogs must retain independently validated linked EP1 header/type metadata. */
    //-------------------------------------------------------------------------------------------------
    private static ManifestDocument Catalog(string path){var d=ManifestReader.Read(SdkEnvironmentPreflight.Read(path,64*1048576));TypeRegistryAudit.ValidateTarget(d.Header.Version,d.Header.AllTypesHash);if(!d.Header.IsLinked || d.Validate().Count!=0)throw new InvalidDataException("Geometry composition catalog invalid.");return d;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise real native allocation of helper plus geometry without Body/armor/movement, then independently invert all native pointers. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void Minimal()
    {
        var source=Xml("<GameObject xmlns='uri:ea.com:eala:asset' id='MinimalGeometry'><Behaviors><ObjectSMCHelper id='Helper' TypeId='2138239554'/></Behaviors><Geometry><Shape/></Geometry></GameObject>");var ns=new XmlNamespaceManager(source.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);SageBinaryData.GameObject* root;using var tracker=new Relo.Tracker((void**)&root,(uint)sizeof(SageBinaryData.GameObject),false);Marshaler.Marshal(new Node(source.DocumentElement!.CreateNavigator()!,ns),root,tracker);var native=new Chunk();if(!tracker.MakeRelocatable(native))throw new InvalidDataException("Minimal geometry allocation failed.");var output=Ep1GameObjectDestructionProjection.Compose(native);var fields=GameObjectStockReview.Tokens(output.InstanceBuffer,0);int prefix=fields[FastHash.GetHashCode("BuildTime")].Offset-140;
        if(native.InstanceBuffer.Length!=696 || Word(native.InstanceBuffer,500)!=620 || fields.Count!=Ep1GameObjectRootValueTokens.AllFields.Count+2 || fields.ContainsKey(FastHash.GetHashCode("Body")) || fields.ContainsKey(FastHash.GetHashCode("ArmorSet")) || fields.ContainsKey(FastHash.GetHashCode("LocomotorSet")) || fields[FastHash.GetHashCode("Geometry")].Offset!=prefix-252 || Word(output.InstanceBuffer,prefix+500)!=prefix+620 || output.ImportsBuffer.Length!=0)throw new InvalidDataException("Minimal helper/geometry native-token envelope differs.");
        var recovered=output.InstanceBuffer.AsSpan(prefix).ToArray();foreach(uint slot in Slots(output.RelocationBuffer))Put(recovered,(int)slot-prefix,Word(output.InstanceBuffer,(int)slot)-(uint)prefix);
        if(!recovered.SequenceEqual(native.InstanceBuffer) || !Slots(output.RelocationBuffer).Select(s=>s-(uint)prefix).SequenceEqual(Slots(native.RelocationBuffer)))throw new InvalidDataException("Minimal helper/geometry native inverse differs.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: controlled sources share actual asset names and local Body links; geometry adds no strong asset imports. */
    //-------------------------------------------------------------------------------------------------
    internal static string Source(ManifestAsset[] targets,int profile,bool moving,bool reverse,int position)
    {
        string source=GameObjectCombatCompositionSmokeTest.Source(targets,reverse,2,3,position).Replace("RootCombatProbe","RootGeometryProbe",StringComparison.Ordinal).Replace("id='Destroy'","id='Destroy' AssociatedModule='Body'",StringComparison.Ordinal);
        if(moving)source=source.Insert(source.IndexOf("/>",StringComparison.Ordinal)+2,$"<LocomotorSet Speed='75' Condition='SPECIALABILITY_ACTIVE' Locomotor='{targets[8].Name}'/>");
        if(profile!=0)
        {
            string shape=profile==1?"<Shape Type='BOX' MajorRadius='20' MinorRadius='10' Height='30'/>":"<Shape Type='RING' Name='geometry shape' Other='other shape' MajorRadius='75' MinorRadius='12' Height='30' FrontAngle='90d'><Offset x='-2' y='3' z='4'/></Shape>";
            string geometry="<Geometry IsSmall='true'>"+(profile==2?"<RotationAnchorOffset x='2' y='-3'/><PlacementShape Type='CYLINDER' MajorRadius='80' Height='5'/>":"")+string.Concat(Enumerable.Repeat(shape,profile==1?1:3))+(profile==2?string.Concat(Enumerable.Repeat("<ContactPoint Label='contact label'><Pos x='11' y='-12' z='13'/></ContactPoint>",2)):"")+"</Geometry>";
            source=source.Insert(source.LastIndexOf("</RootGeometryProbe>",StringComparison.Ordinal),geometry);
        }
        return source;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently verify partial root/module/body/list relationships, every geometry metadata row and the complete shared occurrence/native/auxiliary graph. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(Chunk output,Chunk native,InstanceDeclaration instance,ManifestAsset[] targets,int profile,bool moving,int pointerRoots=0,int pointerMetadata=0,IReadOnlyList<int>? extraImportSlots=null)
    {
        var b=output.InstanceBuffer;var n=native.InstanceBuffer;var root=GameObjectStockReview.Tokens(b,0);int prefix=root[FastHash.GetHashCode("BuildTime")].Offset-140,rootCount=Ep1GameObjectRootValueTokens.AllFields.Count+3+(moving?1:0)+(profile==0?0:1)+pointerRoots,list=(int)Word(n,476),body=(int)Word(n,488),armor=(int)Word(n,424),locomotor=(int)Word(n,432),geometry=(int)Word(n,500),container=(rootCount+1)*12;
        if(root.Count!=rootCount || root[FastHash.GetHashCode("Behaviors")].Offset!=container || Word(b,container)!=0x6C827073 || Word(b,container+4)!=0x5D448FD8 || Word(b,container+8)!=prefix+list || Word(b,prefix+list)!=3 || Word(b,prefix+488)!=prefix+body || (profile==0?Word(b,prefix+500)!=0:Word(b,prefix+500)!=prefix+geometry))throw new InvalidDataException("Geometry composed root/container differs.");
        foreach(var field in Ep1GameObjectRootValueTokens.AllFields)if(root[FastHash.GetHashCode(field.Name)].Type!=field.Type || root[FastHash.GetHashCode(field.Name)].Offset!=prefix+field.Offset)throw new InvalidDataException("Geometry composed flat root target differs.");
        var references=new List<(int Slot,int Target)>{(368,0),(400,1),(armor,2),(armor+4,3)};if(moving)references.Add((locomotor+8,8));int next=container+48;
        for(int i=0;i<3;i++)
        {
            int module=(int)Word(n,list+8+i*4),table=(int)Word(b,container+20+i*12);uint type=Word(n,module);
            if(Word(b,container+12+i*12)!=0xB151A0EB || Word(b,container+16+i*12)!=0 || table!=next || Word(b,table)!=2 || Word(b,table+4)!=type || Word(b,table+8)!=table+12 || Word(b,prefix+list+8+i*4)!=prefix+module)throw new InvalidDataException("Geometry composed behavior marker differs.");
            if(type==0x7F72F242){string[] bases={"ObjectHelperModuleData","UpdateModuleData","ContainModuleData","BehaviorModuleData","ModuleData"};for(int j=0;j<bases.Length;j++)if(Word(b,table+12+j*24)!=1 || Word(b,table+16+j*24)!=FastHash.GetHashCode(bases[j]) || Word(b,table+20+j*24)!=table+36+j*24)throw new InvalidDataException("Geometry SMC base chain differs.");var terminal=GameObjectStockReview.Tokens(b,table+132);if(terminal[FastHash.GetHashCode("TypeId")].Offset!=prefix+module || terminal[FastHash.GetHashCode("id")].Offset!=prefix+module+4)throw new InvalidDataException("Geometry SMC identity binding differs.");next+=168;}
            else if(type==0x0D96A2F4){var fields=GameObjectStockReview.Tokens(b,table+12);if(fields[FastHash.GetHashCode("AssociatedModule")].Offset!=prefix+module+52 || Word(n,module+52)!=FastHash.GetHashCode("body"))throw new InvalidDataException("Geometry destruction local Body binding differs.");int item=(int)Word(n,module+68),itemList=fields[FastHash.GetHashCode("Item")].Offset;if(Word(b,itemList+8)!=prefix+module+64)throw new InvalidDataException("Geometry destruction count target differs.");var itemFields=GameObjectStockReview.Tokens(b,(int)Word(b,itemList+20));if(itemFields[FastHash.GetHashCode("FX")].Offset!=prefix+item+8 || itemFields[FastHash.GetHashCode("BonePrefix")].Offset!=prefix+item)throw new InvalidDataException("Geometry destruction item target differs.");references.Add((item+8,4));next+=384;}
            else if(type==0x431722A9){var fields=GameObjectStockReview.Tokens(b,table+12);if(fields[FastHash.GetHashCode("ActiveWhenDisabledBy")].Offset!=prefix+module+8)throw new InvalidDataException("Geometry weapon field binding differs.");int hardpoint=(int)Word(n,module+16),weapons=(int)Word(n,hardpoint+20),hardpointList=fields[FastHash.GetHashCode("WeaponSlotHardpoint")].Offset;if(Word(b,hardpointList+8)!=prefix+module+12)throw new InvalidDataException("Geometry hardpoint count target differs.");var hardpointFields=GameObjectStockReview.Tokens(b,(int)Word(b,hardpointList+20));int weaponList=hardpointFields[FastHash.GetHashCode("Weapon")].Offset;if(Word(b,weaponList+8)!=prefix+hardpoint+16)throw new InvalidDataException("Geometry weapon count target differs.");for(int j=0;j<2;j++){var weaponFields=GameObjectStockReview.Tokens(b,(int)Word(b,weaponList+20+j*12));foreach(var f in new[]{("Template",4),("Upgrade",8),("ForbiddenObjectStatus",44)})if(weaponFields[FastHash.GetHashCode(f.Item1)].Offset!=prefix+weapons+j*80+f.Item2)throw new InvalidDataException("Geometry weapon child target differs.");references.Add((weapons+j*80+4,6));references.Add((weapons+j*80+8,7));}next+=588;}
            else throw new InvalidDataException("Unreviewed geometry composition behavior.");
        }
        if(root[FastHash.GetHashCode("Body")].Offset!=next || Word(b,next)!=2 || Word(b,next+4)!=0x5A0E3D9B || Word(b,next+8)!=next+12)throw new InvalidDataException("Geometry composed Body wrapper differs.");var bodyFields=GameObjectStockReview.Tokens(b,next+12);if(bodyFields[FastHash.GetHashCode("MaxHealth")].Offset!=prefix+body+8)throw new InvalidDataException("Geometry composed Body target differs.");next+=504;
        int rangeList=bodyFields[FastHash.GetHashCode("DamageRangeModifier")].Offset;if(Word(b,rangeList+8)!=prefix+body+64 || bodyFields[FastHash.GetHashCode("HealingBuffFX")].Offset!=prefix+body+44)throw new InvalidDataException("Geometry Body range/FX target differs.");for(int i=0;i<3;i++){var fields=GameObjectStockReview.Tokens(b,(int)Word(b,rangeList+20+i*12));foreach(var f in new[]{("Min",0),("Max",4),("AttributeModifier",8)})if(fields[FastHash.GetHashCode(f.Item1)].Offset!=prefix+body+76+i*12+f.Item2)throw new InvalidDataException("Geometry Body range child target differs.");}
        if(root[FastHash.GetHashCode("ArmorSet")].Offset!=next || Word(b,next+8)!=prefix+420)throw new InvalidDataException("Geometry composed armor metadata differs.");var armorFields=GameObjectStockReview.Tokens(b,(int)Word(b,next+20));foreach(var f in new[]{("Armor",0),("DamageFX",4),("Conditions",8)})if(armorFields[FastHash.GetHashCode(f.Item1)].Offset!=prefix+armor+f.Item2)throw new InvalidDataException("Geometry armor member target differs.");next+=72;
        if(moving){if(root[FastHash.GetHashCode("LocomotorSet")].Offset!=next || Word(b,next+8)!=prefix+428 || Word(b,prefix+locomotor+4)!=18)throw new InvalidDataException("Geometry composed movement metadata differs.");var fields=GameObjectStockReview.Tokens(b,(int)Word(b,next+20));foreach(var f in new[]{("Speed",0),("Condition",4),("Locomotor",8)})if(fields[FastHash.GetHashCode(f.Item1)].Offset!=prefix+locomotor+f.Item2)throw new InvalidDataException("Geometry movement member target differs.");next+=72;}
        references.Add((body+44,4));for(int i=0;i<3;i++)references.Add((body+76+i*12+8,5));
        if(instance.ReferencedInstances.Count!=references.Count+(extraImportSlots?.Count??0))throw new InvalidDataException("Geometry shared occurrence count differs.");for(int i=0;i<references.Count;i++){var reference=instance.ReferencedInstances[i];var expected=targets[references[i].Target];if(reference.TypeId!=expected.TypeId || reference.InstanceId!=expected.InstanceId || Word(b,prefix+references[i].Slot)!=i+1)throw new InvalidDataException("Geometry shared reference identity/index differs.");}
        if(profile==0){if(prefix!=next+pointerMetadata || root.ContainsKey(FastHash.GetHashCode("Geometry")))throw new InvalidDataException("Absent geometry emitted metadata.");}
        else
        {
            var field=root[FastHash.GetHashCode("Geometry")];int size=profile==1?252:804;if(field.Type!=FastHash.GetHashCode("Geometry") || field.Offset!=next || prefix!=next+size+pointerMetadata)throw new InvalidDataException("Geometry root field/metadata extent differs.");
            // Reborn: extract the composed fragment into local coordinates, then run the standalone independent verifier against native owner bytes.
            var original=Extract(native,geometry,n.Length);var local=new Chunk{InstanceBuffer=new byte[size+n.Length-geometry],RelocationBuffer=Pack(Slots(output.RelocationBuffer).Where(s=>s>=prefix+geometry).Select(s=>s-(uint)(prefix+geometry)+(uint)size)),ImportsBuffer=Array.Empty<byte>()};b.AsSpan(next,size).CopyTo(local.InstanceBuffer);b.AsSpan(prefix+geometry).CopyTo(local.InstanceBuffer.AsSpan(size));
            for(int at=8;at<size;at+=12){uint target=Word(local.InstanceBuffer,at);if(target==0)continue;if(target>=next && target<next+size)Put(local.InstanceBuffer,at,target-(uint)next);else if(target>=prefix+geometry && target<b.Length)Put(local.InstanceBuffer,at,target-(uint)(prefix+geometry)+(uint)size);else throw new InvalidDataException("Composed geometry metadata crosses fragment owners.");}
            foreach(uint slot in Slots(local.RelocationBuffer))Put(local.InstanceBuffer,(int)slot,Word(local.InstanceBuffer,(int)slot)-(uint)(prefix+geometry)+(uint)size);Ep1GeometryTokenSmokeTest.Verify(local,original);
        }
        var recovered=b.AsSpan(prefix).ToArray();foreach(uint slot in Slots(output.RelocationBuffer)){if(slot<prefix || Word(b,(int)slot)<prefix)throw new InvalidDataException("Geometry combined pointer escapes native owners.");Put(recovered,(int)slot-prefix,Word(b,(int)slot)-(uint)prefix);}
        if(!recovered.SequenceEqual(n) || !Slots(output.RelocationBuffer).Select(s=>s-(uint)prefix).SequenceEqual(Slots(native.RelocationBuffer)) || !Slots(output.ImportsBuffer).Select(s=>s-(uint)prefix).SequenceEqual(Slots(native.ImportsBuffer)) || !Slots(output.ImportsBuffer).Order().SequenceEqual(references.Select(r=>(uint)(prefix+r.Slot)).Concat((extraImportSlots??Array.Empty<int>()).Select(s=>(uint)(prefix+s))).Order()))throw new InvalidDataException("Geometry full native/RELO/IMP inverse differs.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: selected geometry native extraction rebases only owned pointers and never imports or repairs cross-owner aliases. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Extract(Chunk source,int start,int end){var c=new Chunk{InstanceBuffer=source.InstanceBuffer.AsSpan(start,end-start).ToArray(),RelocationBuffer=Pack(Slots(source.RelocationBuffer).Where(s=>s>=start && s<end).Select(s=>s-(uint)start)),ImportsBuffer=Array.Empty<byte>()};foreach(uint slot in Slots(c.RelocationBuffer)){uint target=Word(c.InstanceBuffer,(int)slot);if(target<start || target>=end)throw new InvalidDataException("Geometry extraction cross-owner pointer.");Put(c.InstanceBuffer,(int)slot,target-(uint)start);}return c;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: source/corruption failures cannot become partial graph success. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action){try{action();}catch(InvalidDataException){return;}throw new InvalidDataException("Malformed geometry composition admitted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: fixture documents disable external entity resolution. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Xml(string source){var d=new XmlDocument{XmlResolver=null};d.LoadXml(source);return d;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: all corruption controls own their buffers. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c)=>new(){InstanceBuffer=c.InstanceBuffer.ToArray(),RelocationBuffer=c.RelocationBuffer.ToArray(),ImportsBuffer=c.ImportsBuffer.ToArray()};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: validated native/output auxiliary streams retain explicit sentinels. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b){if(b.Length==0)return Array.Empty<uint>();if(b.Length<8 || b.Length%4!=0 || Word(b,b.Length-4)!=uint.MaxValue)throw new InvalidDataException("Geometry composition slot stream differs.");return Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: owned slot-list edits preserve empty streams or the trailing sentinel. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> slots){var a=slots.ToArray();if(a.Length==0)return Array.Empty<byte>();var b=new byte[(a.Length+1)*4];for(int i=0;i<a.Length;i++)Put(b,i*4,a[i]);Put(b,a.Length*4,uint.MaxValue);return b;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: diagnostic reads remain bounded little-endian words. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at){if(at<0 || at>b.Length-4)throw new InvalidDataException("Geometry composition read bounds exceeded.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: only owned output/corruption arrays are changed. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
