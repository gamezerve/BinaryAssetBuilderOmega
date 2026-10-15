using System.Buffers.Binary;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: selected root/armor/body and destruction/weapon behaviors share one native graph and Core occurrence table; still not a complete publishable object.
internal static class GameObjectCombatCompositionSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: batch both module orders, scalar/range/list edits, actual named targets and an optional newly compiled armor consumer. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string globalManifest,string? authoredDirectory=null,bool includeSmc=false)
    {
        var saved=Settings.Current;
        try
        {
            XmlSchemaSet? official=null;SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:s=>official=s);
            if(official==null)throw new InvalidDataException("Reviewed combat source schema required.");
            Settings.Current=new Settings {SchemaPath=Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"RootCombatPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};var schemas=new SchemaSet(false);
            var catalog=new[]{Catalog(manifest),Catalog(globalManifest)}.SelectMany(d=>d.Assets).ToArray();var stockCatalog=catalog;
            ManifestAsset? authored=null;
            if(authoredDirectory!=null){var build=GameObjectArmorCompositionSmokeTest.BuildArmor(authoredDirectory);authored=build.Assets.Single(a=>a.Name=="ArmorTemplate:RebornComposedArmor");if(catalog.Any(a=>a.TypeId==authored.TypeId && a.InstanceId==authored.InstanceId))throw new InvalidDataException("Combat authored armor collision.");catalog=catalog.Concat(build.Assets).ToArray();}
            string[] names={"UnitTypeIcon","UnitIntro","ArmorTemplate","DamageFX","FXList","AttributeModifier","WeaponTemplate","UpgradeTemplate"};var targets=names.Select(name=>catalog.First(a=>a.TypeId==FastHash.GetHashCode(name))).ToArray();if(authored!=null)targets[2]=authored;
            Chunk? populated=null;InstanceDeclaration? populatedInstance=null;int cases=0;
            foreach(int helperPosition in includeSmc?new[]{0,1,2}:new[]{-1})foreach(bool reverse in new[]{false,true})foreach(int weapons in new[]{1,2})foreach(int ranges in new[]{0,3})foreach(bool strings in new[]{false,true})
            {
                string source=Source(targets,reverse,weapons,ranges,helperPosition);GameObjectBodyProjectionSmokeTest.Validate(official,Adapt(Xml(source).DocumentElement!,strings).OuterXml);
                var instance=Normalize(schemas,source,catalog);var native=GameObjectBodyProjectionSmokeTest.Compile(Adapt(instance.XmlNode,strings),selectedBehaviors:true);var a=Check(native,instance,reverse,weapons,ranges,targets,helperPosition);
                var repeat=Normalize(schemas,source,catalog);var b=Check(GameObjectBodyProjectionSmokeTest.Compile(Adapt(repeat.XmlNode,strings),selectedBehaviors:true),repeat,reverse,weapons,ranges,targets,helperPosition);
                if(!a.InstanceBuffer.SequenceEqual(b.InstanceBuffer) || !a.RelocationBuffer.SequenceEqual(b.RelocationBuffer) || !a.ImportsBuffer.SequenceEqual(b.ImportsBuffer))throw new InvalidDataException("Combat projection repeat differs.");
                if(!reverse && weapons==2 && ranges==3 && strings && helperPosition==(includeSmc?0:-1)){populated=native;populatedInstance=instance;}cases++;
            }
            // Reborn: single selected weapon behavior remains valid too; no artificial destruction module is required by the native graph writer.
            var single=Xml(Source(targets,false,1,0));single.DocumentElement!.RemoveChild(single.GetElementsByTagName("BuildingDestructionBehavior",SchemaSet.XmlNamespace)[0]!);
            var one=Normalize(schemas,single.OuterXml,catalog);var oneNative=GameObjectBodyProjectionSmokeTest.Compile(Adapt(one.XmlNode,true),selectedBehaviors:true);var oneOutput=Ep1GameObjectDestructionProjection.Compose(oneNative);
            if(Word(oneNative.InstanceBuffer,(int)Word(oneNative.InstanceBuffer,476))!=1 || GameObjectStockReview.Tokens(oneOutput.InstanceBuffer,0).Count!=Ep1GameObjectRootValueTokens.AllFields.Count+3)throw new InvalidDataException("Single selected weapon behavior projection differs.");
            int list=(int)Word(populated!.InstanceBuffer,476),first=(int)Word(populated.InstanceBuffer,list+8),second=(int)Word(populated.InstanceBuffer,list+12),body=(int)Word(populated.InstanceBuffer,488),failures=0;
            foreach(var fault in new[]{(list,includeSmc?4u:3u),(list+12,(uint)first),(list+8,(uint)second),(first,0u),(second,0u),(second+16,(uint)(first+72)),(488,(uint)second)}){var bad=Clone(populated);Put(bad.InstanceBuffer,fault.Item1,fault.Item2);Refuse(()=>Ep1GameObjectDestructionProjection.Compose(bad));failures++;}
            var missing=Clone(populated);missing.RelocationBuffer=Pack(Slots(missing.RelocationBuffer).Where(s=>s!=list+12));Refuse(()=>Ep1GameObjectDestructionProjection.Compose(missing));failures++;
            int selectedHelperPosition=includeSmc?0:-1;string valid=Source(targets,false,2,3,selectedHelperPosition);
            foreach(string bad in new[]{valid.Replace("id='Shooter'","id='Destroy'",StringComparison.Ordinal),valid.Replace("id='Shooter'","",StringComparison.Ordinal),valid.Replace(targets[6].Name,"MissingRebornCombatWeapon",StringComparison.Ordinal)}){Refuse(()=>GameObjectBodyProjectionSmokeTest.Compile(Adapt(Normalize(schemas,bad,catalog).XmlNode,false),selectedBehaviors:true));failures++;}
            if(authored!=null){Refuse(()=>Normalize(schemas,valid,stockCatalog));failures++;}
            var good=Check(populated,populatedInstance!,false,2,3,targets,selectedHelperPosition);var root=GameObjectStockReview.Tokens(good.InstanceBuffer,0);int container=root[FastHash.GetHashCode("Behaviors")].Offset,firstTable=(int)Word(good.InstanceBuffer,container+20),secondTable=(int)Word(good.InstanceBuffer,container+32),metadataFailures=0;
            foreach(int at in new[]{container,container+4,container+8,container+12,container+20,container+24,container+32,firstTable+4,firstTable+8,secondTable+4,secondTable+8,secondTable+32,root[FastHash.GetHashCode("Body")].Offset+8}){var bad=Clone(good);Put(bad.InstanceBuffer,at,Word(bad.InstanceBuffer,at)^4u);Refuse(()=>Verify(bad,populated,populatedInstance!,false,2,3,targets,selectedHelperPosition));metadataFailures++;}
            Console.WriteLine($"GameObject combat composition: PASS ({cases} Core/native cases plus single weapon module, SMC={includeSmc}, all selected behavior orders, armor/health/weapons/upgrade/ranges and dynamic strings; {failures} input and {metadataFailures} metadata refusals; {Ep1GameObjectRootValueTokens.AllFields.Count+3}-field partial root, no general object processor/publication/package/game).");
            if(authored!=null)Console.WriteLine("Authored armor + weapon consumer graph: PASS (new Core armor target and independent stock weapon/upgrade targets share one partial object; removal refuses; no complete consumer stream/game).");
        }
        finally{Settings.Current=saved;}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: validated linked metadata supplies real target identities without native target payload dumping. */
    //-------------------------------------------------------------------------------------------------
    private static ManifestDocument Catalog(string path){var d=ManifestReader.Read(SdkEnvironmentPreflight.Read(path,64*1048576));TypeRegistryAudit.ValidateTarget(d.Header.Version,d.Header.AllTypesHash);if(!d.Header.IsLinked || d.Validate().Count!=0)throw new InvalidDataException("Combat catalog invalid.");return d;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: one actual Core traversal assigns all selected root/armor/module/body reference occurrences, with no authored numeric indices. */
    //-------------------------------------------------------------------------------------------------
    private static InstanceDeclaration Normalize(SchemaSet schemas,string source,ManifestAsset[] catalog){if(source.Contains('\\'))throw new InvalidDataException("Authored combat indices refused.");var r=ReferencePipelineSmokeTest.Normalize(schemas,source);foreach(var h in r.ReferencedInstances)if(catalog.Count(a=>a.TypeId==h.TypeId && a.InstanceId==h.InstanceId)!=1)throw new InvalidDataException("Combat target missing/ambiguous.");return r;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: controlled sources exercise source order, real identities and literal fields without pretending to support a general mod input graph. */
    //-------------------------------------------------------------------------------------------------
    internal static string Source(ManifestAsset[] t,bool reverse,int weapons,int ranges,int helperPosition=-1)
    {
        string destruction=$"<BuildingDestructionBehavior id='Destroy'><DieMuxData/><Item FX='{t[4].Name}' BonePrefix='BONE_'/></BuildingDestructionBehavior>";
        string weapon=$"<WeaponSetUpdate id='Shooter'><WeaponSlotHardpoint ID='1'>"+string.Concat(Enumerable.Range(0,weapons).Select(i=>$"<Weapon Ordering='{(i==0?"PRIMARY_WEAPON":"SECONDARY_WEAPON")}' Template='{t[6].Name}' Upgrade='{t[7].Name}' ForbiddenObjectStatus='GENERIC_TOGGLE_STATE'/>"))+"</WeaponSlotHardpoint></WeaponSetUpdate>";
        var modules=reverse?new List<string>{weapon,destruction}:new List<string>{destruction,weapon};if(helperPosition>=0)modules.Insert(helperPosition,"<ObjectSMCHelper id='Smc'/>");
        return $"<RootCombatProbe xmlns='uri:ea.com:eala:asset' id='CombatProbe' UnitTypeIcon='{t[0].Name}' UnitIntro='{t[1].Name}'><ArmorSet Armor='{t[2].Name}' DamageFX='{t[3].Name}'/>"+string.Concat(modules)+$"<ActiveBody id='Body' MaxHealth='2000' HealingBuffFX='{t[4].Name}'>"+string.Concat(Enumerable.Repeat($"<DamageRangeModifier Min='25%' Max='75%' AttributeModifier='{t[5].Name}'/>",ranges))+"</ActiveBody></RootCombatProbe>";
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: move focused children into the real root/ArmorSet/Behaviors/Body containers while retaining normalized source order. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlDocument Adapt(XmlNode probe,bool strings)
    {
        var d=Xml("<GameObject xmlns='uri:ea.com:eala:asset' id='CombatProbe'><Behaviors/><Body/></GameObject>");foreach(string name in new[]{"UnitTypeIcon","UnitIntro"})d.DocumentElement!.SetAttribute(name,probe.Attributes![name]!.Value);if(strings)d.DocumentElement!.SetAttribute("EditorName",new string('R',300));
        // Reborn: focused extension schemas may append diagnostic root children; the raw GameObject adapter restores their actual official container order.
        foreach(XmlElement child in probe.ChildNodes.OfType<XmlElement>())if(child.LocalName is "ArmorSet" or "LocomotorSet" or "Geometry" or "DisplayName" or "DisplayNameTransformed" or "Flammability" or "LiveCameraOffset" or "VisionInfo")d.DocumentElement!.AppendChild(d.ImportNode(child,true));else if(child.LocalName=="ActiveBody")d.GetElementsByTagName("Body",SchemaSet.XmlNamespace)[0]!.AppendChild(d.ImportNode(child,true));else d.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace)[0]!.AppendChild(d.ImportNode(child,true));
        foreach(var child in d.DocumentElement!.ChildNodes.OfType<XmlElement>().OrderBy(e=>e.LocalName switch{"DisplayName"=>0,"DisplayNameTransformed"=>1,"ArmorSet"=>2,"LocomotorSet"=>3,"Flammability"=>4,"Behaviors"=>5,"Body"=>6,"Geometry"=>7,"LiveCameraOffset"=>8,"VisionInfo"=>9,_=>throw new InvalidDataException("Unreviewed root child adapter.")}).ToArray())d.DocumentElement.AppendChild(child);return d;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: separate output verification from production so detached metadata cannot be repaired by a repeated write. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Check(Chunk native,InstanceDeclaration instance,bool reverse,int weapons,int ranges,ManifestAsset[] targets,int helperPosition=-1){var output=Ep1GameObjectDestructionProjection.Compose(native);Verify(output,native,instance,reverse,weapons,ranges,targets,helperPosition);return output;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently bind the two concrete wrappers, shared occurrence identities and full native/auxiliary coordinates. */
    //-------------------------------------------------------------------------------------------------
    private static void Verify(Chunk output,Chunk native,InstanceDeclaration instance,bool reverse,int weapons,int ranges,ManifestAsset[] targets,int helperPosition=-1)
    {
        var b=output.InstanceBuffer;var root=GameObjectStockReview.Tokens(b,0);int prefix=root[FastHash.GetHashCode("BuildTime")].Offset-140,list=(int)Word(native.InstanceBuffer,476),body=(int)Word(native.InstanceBuffer,488),armor=(int)Word(native.InstanceBuffer,424),container=(Ep1GameObjectRootValueTokens.AllFields.Count+4)*12;
        var classes=reverse?new List<uint>{0x431722A9u,0x0D96A2F4u}:new List<uint>{0x0D96A2F4u,0x431722A9u};if(helperPosition>=0)classes.Insert(helperPosition,0x7F72F242u);
        if(root.Count!=Ep1GameObjectRootValueTokens.AllFields.Count+3 || root[FastHash.GetHashCode("Behaviors")].Offset!=container || Word(b,container)!=0x6C827073 || Word(b,container+4)!=0x5D448FD8 || Word(b,container+8)!=prefix+list || Word(b,prefix+list)!=classes.Count)throw new InvalidDataException("Combat root/count metadata differs.");
        int next=container+12+classes.Count*12,weaponPrefix=372+weapons*108;var refs=new List<(int Slot,int Target)>{(368,0),(400,1),(armor,2),(armor+4,3)};
        for(int i=0;i<classes.Count;i++)
        {
            uint type=classes[i];bool isWeapon=type==0x431722A9u;int module=(int)Word(native.InstanceBuffer,list+8+i*4),table=(int)Word(b,container+20+i*12);
            if(Word(b,container+12+i*12)!=0xB151A0EB || Word(b,container+16+i*12)!=0 || table!=next || Word(b,table)!=2 || Word(b,table+4)!=type || Word(b,table+8)!=table+12 || Word(b,prefix+list+8+i*4)!=prefix+module || Word(b,prefix+module)!=type)throw new InvalidDataException("Combat module marker/type/native binding differs.");
            var fields=GameObjectStockReview.Tokens(b,table+12);
            if(type==0x7F72F242u)
            {
                string[] bases={"ObjectHelperModuleData","UpdateModuleData","ContainModuleData","BehaviorModuleData","ModuleData"};
                for(int j=0;j<bases.Length;j++)if(Word(b,table+12+j*24)!=1 || Word(b,table+16+j*24)!=FastHash.GetHashCode(bases[j]) || Word(b,table+20+j*24)!=table+36+j*24)throw new InvalidDataException("Combat SMC inheritance binding differs.");
                var terminal=GameObjectStockReview.Tokens(b,table+132);if(terminal[FastHash.GetHashCode("TypeId")].Offset!=prefix+module || terminal[FastHash.GetHashCode("id")].Offset!=prefix+module+4)throw new InvalidDataException("Combat SMC native identity binding differs.");next+=168;
            }
            else if(isWeapon)
            {
                if(fields[FastHash.GetHashCode("ActiveWhenDisabledBy")].Offset!=prefix+module+8)throw new InvalidDataException("Combat weapon scalar target differs.");int hp=(int)Word(native.InstanceBuffer,module+16),wp=(int)Word(native.InstanceBuffer,hp+20),hpList=fields[FastHash.GetHashCode("WeaponSlotHardpoint")].Offset;
                if(Word(b,hpList+8)!=prefix+module+12)throw new InvalidDataException("Combat hardpoint count target differs.");var hpf=GameObjectStockReview.Tokens(b,(int)Word(b,hpList+20));int wl=hpf[FastHash.GetHashCode("Weapon")].Offset;if(Word(b,wl+8)!=prefix+hp+16)throw new InvalidDataException("Combat weapon count target differs.");
                for(int j=0;j<weapons;j++){var row=GameObjectStockReview.Tokens(b,(int)Word(b,wl+20+j*12));foreach(var f in new[]{("Template",4),("Upgrade",8),("ForbiddenObjectStatus",44)})if(row[FastHash.GetHashCode(f.Item1)].Offset!=prefix+wp+j*80+f.Item2)throw new InvalidDataException("Combat weapon field target differs.");refs.Add((wp+j*80+4,6));refs.Add((wp+j*80+8,7));}next+=weaponPrefix;
            }
            else{int items=(int)Word(native.InstanceBuffer,module+68);refs.Add((items+8,4));foreach(var f in new[]{("AssociatedModule",52),("MaxDebrisCloudsPerSecond",56),("FillerMesh",60)})if(fields[FastHash.GetHashCode(f.Item1)].Offset!=prefix+module+f.Item2)throw new InvalidDataException("Combat destruction scalar target differs.");int it=fields[FastHash.GetHashCode("Item")].Offset;if(Word(b,it+8)!=prefix+module+64)throw new InvalidDataException("Combat destruction count target differs.");next+=384;}
        }
        if(root[FastHash.GetHashCode("Body")].Offset!=next || Word(b,next)!=2 || Word(b,next+4)!=0x5A0E3D9B || Word(b,next+8)!=next+12 || Word(b,prefix+488)!=prefix+body)throw new InvalidDataException("Combat body wrapper differs.");next+=324+ranges*60;
        if(root[FastHash.GetHashCode("ArmorSet")].Offset!=next || prefix!=next+72 || Word(b,next+8)!=prefix+420)throw new InvalidDataException("Combat armor/prefix extent differs.");refs.Add((body+44,4));for(int i=0;i<ranges;i++)refs.Add((body+76+i*12+8,5));
        if(instance.ReferencedInstances.Count!=refs.Count)throw new InvalidDataException("Combat reference occurrence count differs.");for(int i=0;i<refs.Count;i++){var r=instance.ReferencedInstances[i];var target=targets[refs[i].Target];if(r.TypeId!=target.TypeId || r.InstanceId!=target.InstanceId || Word(b,prefix+refs[i].Slot)!=i+1)throw new InvalidDataException("Combat reference identity/index differs.");}
        var recovered=b.AsSpan(prefix).ToArray();foreach(uint s in Slots(output.RelocationBuffer)){if(s<prefix || Word(b,(int)s)<prefix)throw new InvalidDataException("Combat pointer escapes native owner.");Put(recovered,(int)s-prefix,Word(b,(int)s)-(uint)prefix);}
        if(!recovered.SequenceEqual(native.InstanceBuffer) || !Slots(output.RelocationBuffer).Select(s=>s-(uint)prefix).SequenceEqual(Slots(native.RelocationBuffer)) || !Slots(output.ImportsBuffer).Select(s=>s-(uint)prefix).SequenceEqual(Slots(native.ImportsBuffer)) || !Slots(output.ImportsBuffer).Order().SequenceEqual(refs.Select(r=>(uint)(prefix+r.Slot)).Order()))throw new InvalidDataException("Combat native/RELO/IMP round trip differs.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: closed fixture XML disables external entity resolution. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Xml(string s){var d=new XmlDocument {XmlResolver=null};d.LoadXml(s);return d;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: mutation controls own all buffers. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c)=>new(){InstanceBuffer=c.InstanceBuffer.ToArray(),RelocationBuffer=c.RelocationBuffer.ToArray(),ImportsBuffer=c.ImportsBuffer.ToArray()};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: absent auxiliary streams stay absent. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b)=>b.Length==0?Array.Empty<uint>():Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();
    //-------------------------------------------------------------------------------------------------
    /** Reborn: detached slot removal preserves sentinel format. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> s){var a=s.Append(uint.MaxValue).ToArray();var b=new byte[a.Length*4];for(int i=0;i<a.Length;i++)Put(b,i*4,a[i]);return b;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: expected source/selection/native failures cannot turn into partial success. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action a){try{a();}catch(Exception e)when(e is InvalidDataException or BinaryAssetBuilderException or XmlSchemaValidationException){return;}throw new InvalidDataException("Malformed combat graph admitted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: all reads remain within explicit little-endian native/token slices. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));
    //-------------------------------------------------------------------------------------------------
    /** Reborn: only owned diagnostic buffers are changed. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint v)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),v);
}
