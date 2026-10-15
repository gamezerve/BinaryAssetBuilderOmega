using System.Buffers.Binary;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: pointed localization-key headers, unsigned flammability words and camera vectors join actual Core combat/geometry graphs without enabling full objects.
internal static class GameObjectPointerRootSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: batch every presence mask, text/default/scalar profile and behavior order; optional authored armor remains a genuine independently built target. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string globalManifest,string? authoredDirectory=null)
    {
        var saved=Settings.Current;
        try
        {
            XmlSchemaSet? official=null;SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:s=>official=s);if(official==null)throw new InvalidDataException("Reviewed pointed-root schema required.");Stock(official,manifest);
            Settings.Current=new Settings{SchemaPath=Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"RootPointerPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};var schemas=new SchemaSet(false);Drift(official,schemas.Schemas);
            var stock=new[]{Catalog(manifest),Catalog(globalManifest)}.SelectMany(d=>d.Assets).ToArray();var catalog=stock;ManifestAsset? authored=null;
            if(authoredDirectory!=null){var built=GameObjectArmorCompositionSmokeTest.BuildArmor(authoredDirectory);authored=built.Assets.Single(a=>a.Name=="ArmorTemplate:RebornComposedArmor");if(catalog.Any(a=>a.TypeId==authored.TypeId && a.InstanceId==authored.InstanceId))throw new InvalidDataException("Pointed-root authored target collides.");catalog=catalog.Concat(built.Assets).ToArray();}
            string[] names={"UnitTypeIcon","UnitIntro","ArmorTemplate","DamageFX","FXList","AttributeModifier","WeaponTemplate","UpgradeTemplate","LocomotorTemplate"};var targets=names.Select(t=>catalog.First(a=>a.TypeId==FastHash.GetHashCode(t))).ToArray();if(authored!=null)targets[2]=authored;
            int cases=0;Chunk? populated=null;InstanceDeclaration? populatedInstance=null;
            foreach(int mask in Enumerable.Range(0,16))foreach(int profile in new[]{0,1,2})foreach(bool reverse in new[]{false,true})foreach(int position in new[]{0,1,2})
            {
                string source=Source(targets,mask,profile,reverse,position);var pair=Compile(source,profile==2,catalog);var output=Check(pair.Native,pair.Instance,targets,mask,profile);var repeat=Compile(source,profile==2,catalog);var other=Check(repeat.Native,repeat.Instance,targets,mask,profile);
                if(!output.InstanceBuffer.SequenceEqual(other.InstanceBuffer) || !output.RelocationBuffer.SequenceEqual(other.RelocationBuffer) || !output.ImportsBuffer.SequenceEqual(other.ImportsBuffer))throw new InvalidDataException("Pointed root repeat differs.");
                if(mask==15 && profile==2 && !reverse && position==0){populated=pair.Native;populatedInstance=pair.Instance;}cases++;
            }
            int first=(int)Word(populated!.InstanceBuffer,404),second=(int)Word(populated.InstanceBuffer,408),flammability=(int)Word(populated.InstanceBuffer,464),camera=(int)Word(populated.InstanceBuffer,528),list=(int)Word(populated.InstanceBuffer,476),failures=0,metadataFailures=0;
            foreach(var fault in new[]{(404,(uint)(first+1)),(404,(uint)list),(408,(uint)first),(464,(uint)first),(528,Word(populated.InstanceBuffer,424)),(first,uint.MaxValue),(first+4,(uint)flammability),(camera,0x7F800000u),(camera+4,0x7FC00000u)}){var bad=Clone(populated);Put(bad.InstanceBuffer,fault.Item1,fault.Item2);Refuse(()=>Ep1GameObjectDestructionProjection.Compose(bad));failures++;}
            foreach(uint slot in new[]{404u,408u,464u,528u,(uint)(first+4),(uint)(second+4)}){var bad=Clone(populated);bad.RelocationBuffer=Pack(Slots(bad.RelocationBuffer).Where(s=>s!=slot));Refuse(()=>Ep1GameObjectDestructionProjection.Compose(bad));failures++;}
            var imported=Clone(populated);imported.ImportsBuffer=Pack(Slots(imported.ImportsBuffer).Append(404u));Refuse(()=>Ep1GameObjectDestructionProjection.Compose(imported));failures++;
            string valid=Source(targets,15,2,false,0);
            foreach(string bad in new[]{valid.Replace("Fuel='200'","Fuel='4294967296'",StringComparison.Ordinal),valid.Replace("Fuel='200'","Fuel='-1'",StringComparison.Ordinal),valid.Replace("x='0.25'","x='NaN'",StringComparison.Ordinal),valid.Replace("NAME:Reborn &amp; &lt;Unit&gt;","NAME:é",StringComparison.Ordinal)}){Refuse(()=>{var p=Compile(bad,true,catalog);Ep1GameObjectDestructionProjection.Compose(p.Native);});failures++;}
            if(authored!=null){Refuse(()=>Compile(valid,true,stock));failures++;}
            var good=Check(populated,populatedInstance!,targets,15,2);int table=GameObjectStockReview.Tokens(good.InstanceBuffer,0)[FastHash.GetHashCode("Flammability")].Offset;
            foreach(var field in Fields()){int row=FindRow(good.InstanceBuffer,field.Name);foreach(int delta in new[]{4,8}){var bad=Clone(good);Put(bad.InstanceBuffer,row+delta,Word(bad.InstanceBuffer,row+delta)^4u);Refuse(()=>Verify(bad,populated,populatedInstance!,targets,15,2));metadataFailures++;}}
            foreach(int at in new[]{table+4,table+8,table+16,table+20,table+28,table+32,table+40,table+44,table+48}){var bad=Clone(good);Put(bad.InstanceBuffer,at,Word(bad.InstanceBuffer,at)^4u);Refuse(()=>Verify(bad,populated,populatedInstance!,targets,15,2));metadataFailures++;}
            // Reborn: the former signed primitive tag must explicitly refuse even though its four-byte native width matches unsigned fields.
            var signed=Clone(good);Put(signed.InstanceBuffer,table+4,0x23E650B8);Refuse(()=>Verify(signed,populated,populatedInstance!,targets,15,2));metadataFailures++;
            Console.WriteLine($"GameObject pointed roots: PASS ({cases} actual-Core/native cases: all 16 presence masks, three text/default/scalar profiles, six behavior orders with full selected movement/armor/body/geometry graph; {failures} input/{metadataFailures} metadata refusals, up to {Ep1GameObjectRootValueTokens.AllFields.Count+9} partial root fields; positive stock names, absent-only selected stock flammability/camera; no localization-table/general object publication/game).");
            //-------------------------------------------------------------------------------------------------
            /** Reborn: full raw GameObject validation precedes focused Core normalization and unique independent target closure. */
            //-------------------------------------------------------------------------------------------------
            (Chunk Native,InstanceDeclaration Instance) Compile(string source,bool strings,ManifestAsset[] admitted)
            {
                if(source.Contains('\\'))throw new InvalidDataException("Authored pointed-root dependency indices refused.");var raw=Xml(source);GameObjectBodyProjectionSmokeTest.Validate(official,GameObjectCombatCompositionSmokeTest.Adapt(raw.DocumentElement!,strings).OuterXml);var instance=ReferencePipelineSmokeTest.Normalize(schemas,source);foreach(var r in instance.ReferencedInstances)if(admitted.Count(a=>a.TypeId==r.TypeId && a.InstanceId==r.InstanceId)!=1)throw new InvalidDataException("Pointed-root target missing/ambiguous.");return(GameObjectBodyProjectionSmokeTest.Compile(GameObjectCombatCompositionSmokeTest.Adapt(instance.XmlNode,strings),selectedBehaviors:true),instance);
            }
        }
        finally{Settings.Current=saved;}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: real catalogs retain validated linked EP1 headers and identity metadata. */
    //-------------------------------------------------------------------------------------------------
    private static ManifestDocument Catalog(string path){var d=ManifestReader.Read(SdkEnvironmentPreflight.Read(path,64*1048576));TypeRegistryAudit.ValidateTarget(d.Header.Version,d.Header.AllTypesHash);if(!d.Header.IsLinked || d.Validate().Count!=0)throw new InvalidDataException("Pointed-root catalog invalid.");return d;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: copied four-attribute Flammability closure must match official types/defaults/use; vectors and strings reuse actual included/builtin types. */
    //-------------------------------------------------------------------------------------------------
    private static void Drift(XmlSchemaSet official,XmlSchemaSet focused){var key=new XmlQualifiedName("Flammability",SchemaSet.XmlNamespace);var a=(XmlSchemaComplexType)official.GlobalTypes[key]!;var b=(XmlSchemaComplexType)focused.GlobalTypes[key]!;if(a.AttributeUses.Count!=4 || b.AttributeUses.Count!=4)throw new InvalidDataException("Flammability inventory drift.");foreach(XmlSchemaAttribute field in a.AttributeUses.Values){var other=(XmlSchemaAttribute)b.AttributeUses[field.QualifiedName]!;if(other==null || other.AttributeSchemaType!.QualifiedName!=field.AttributeSchemaType!.QualifiedName || other.DefaultValue!=field.DefaultValue || other.Use!=field.Use)throw new InvalidDataException("Flammability field type/default drift.");}}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: five independent stock roots pin all four tags, native pointer relationships and real name-header marshalling; selected flammability/camera are null. */
    //-------------------------------------------------------------------------------------------------
    private static void Stock(XmlSchemaSet schemas,string manifest)
    {
        foreach(string name in new[]{"AlliedConstructionYard","SovietAntiInfantryInfantry","JapanFortressShip","AlliedPowerPlantTurbine","NanoSwarmHiveEffect"})
        {
            var stock=GameObjectStockReview.Capture(manifest,"GameObject:"+name);var fields=GameObjectStockReview.Tokens(stock.Bytes,0);var source=Xml("<GameObject xmlns='uri:ea.com:eala:asset' id='StockPointerRoots'><Behaviors><BuildingDestructionBehavior id='Destroy'><DieMuxData/><Item/></BuildingDestructionBehavior></Behaviors><Body><ActiveBody id='Body' MaxHealth='100'/></Body></GameObject>");
            // Reborn: an independent positive unsigned root word pins the primitive tag even when selected Flammability pointers are absent.
            if(fields[FastHash.GetHashCode("CampnessValue")].Type!=0x763177A4)throw new InvalidDataException("Stock unsigned primitive tag differs.");
            foreach(var field in Fields())
            {
                var row=fields[FastHash.GetHashCode(field.Name)];int native=(int)Word(stock.Bytes,stock.Projection.NativeRoot+field.Slot);if(row.Type!=field.Type)throw new InvalidDataException("Stock pointed-root tag differs.");
                if(field.Name is "DisplayName" or "DisplayNameTransformed"){if(row.Offset!=native)throw new InvalidDataException("Stock pointed-name header relationship differs.");if(native!=0){var child=source.CreateElement(field.Name,SchemaSet.XmlNamespace);child.InnerText=ReadText(stock.Bytes,native);source.DocumentElement!.InsertBefore(child,source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace)[0]);}}
                else if(row.Offset!=0 || native!=0)throw new InvalidDataException("Selected absent-only flammability/camera stock boundary changed; review the positive graph before claiming it.");
            }
            GameObjectBodyProjectionSmokeTest.Validate(schemas,source.OuterXml);var compiled=GameObjectBodyProjectionSmokeTest.Compile(source,selectedBehaviors:true);var output=Ep1GameObjectDestructionProjection.Compose(compiled);var roots=GameObjectStockReview.Tokens(output.InstanceBuffer,0);int prefix=roots[FastHash.GetHashCode("BuildTime")].Offset-140;
            foreach(var field in Fields().Take(2)){int original=(int)Word(stock.Bytes,stock.Projection.NativeRoot+field.Slot),actual=(int)Word(compiled.InstanceBuffer,field.Slot);if((original==0)!=(actual==0) || original!=0 && (ReadText(stock.Bytes,original)!=ReadText(compiled.InstanceBuffer,actual) || roots[FastHash.GetHashCode(field.Name)].Offset!=prefix+actual))throw new InvalidDataException("Stock pointed-name native/text/token differs.");}
        }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: source adds no new asset references; text is XML-escaped and includes explicit-empty and long special-character cases. */
    //-------------------------------------------------------------------------------------------------
    private static string Source(ManifestAsset[] targets,int mask,int profile,bool reverse,int position)
    {
        string source=GameObjectGeometryCompositionSmokeTest.Source(targets,2,true,reverse,position).Replace("RootGeometryProbe","RootPointerProbe",StringComparison.Ordinal),children="";
        foreach(var field in Fields().Select((f,i)=>(f,i)))
        {
            if((mask&(1<<field.i))==0)continue;
            if(field.i<2)children+=$"<{field.f.Name}>"+System.Security.SecurityElement.Escape(Text(profile))+$"</{field.f.Name}>";
            else if(field.i==2)children+=profile switch{0=>"<Flammability Fuel='5' MaxBurnRate='10' Decay='4294967295' Resistance='7'/>",1=>"<Flammability/>",_=>"<Flammability Fuel='200' MaxBurnRate='300' Decay='400' Resistance='500'/>"};
            else children+=profile switch{0=>"<LiveCameraOffset x='1' y='-2' z='3'/>",1=>"<LiveCameraOffset/>",_=>"<LiveCameraOffset x='0.25' y='-3.5' z='250'/>"};
        }
        return source.Insert(source.LastIndexOf("</RootPointerProbe>",StringComparison.Ordinal),children);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: expected string values are source literals, not decoded producer output. */
    //-------------------------------------------------------------------------------------------------
    private static string Text(int profile)=>profile==0?"NAME:RootPointer":profile==1?"":"NAME:Reborn & <Unit> "+new string('R',300);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the full selected graph verifier and independently check every new root record/value/metadata table. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Check(Chunk native,InstanceDeclaration instance,ManifestAsset[] targets,int mask,int profile){var output=Ep1GameObjectDestructionProjection.Compose(native);Verify(output,native,instance,targets,mask,profile);return output;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: added root fields shift old fragments but never change strong occurrence identities or permit their pointer records to masquerade as inline values. */
    //-------------------------------------------------------------------------------------------------
    private static void Verify(Chunk output,Chunk native,InstanceDeclaration instance,ManifestAsset[] targets,int mask,int profile)
    {
        int count=Enumerable.Range(0,4).Count(i=>(mask&(1<<i))!=0),tableBytes=(mask&4)==0?0:60;GameObjectGeometryCompositionSmokeTest.Verify(output,native,instance,targets,2,true,count,tableBytes);var b=output.InstanceBuffer;var roots=GameObjectStockReview.Tokens(b,0);int prefix=roots[FastHash.GetHashCode("BuildTime")].Offset-140;
        foreach(var item in Fields().Select((f,i)=>(f,i)))
        {
            var f=item.f;int pointer=(int)Word(native.InstanceBuffer,f.Slot);if((mask&(1<<item.i))==0){if(pointer!=0 || roots.ContainsKey(FastHash.GetHashCode(f.Name)))throw new InvalidDataException("Absent pointed root retained data/metadata.");continue;}
            var field=roots[FastHash.GetHashCode(f.Name)];if(pointer<600 || pointer>=Word(native.InstanceBuffer,476) || field.Type!=f.Type || field.Offset!=(item.i==2?prefix-60:prefix+pointer) || Word(b,prefix+f.Slot)!=prefix+pointer || !Slots(native.RelocationBuffer).Contains((uint)f.Slot))throw new InvalidDataException("Pointed-root metadata/native ownership differs.");
            if(item.i<2){if(ReadText(b,prefix+pointer)!=Text(profile))throw new InvalidDataException("Pointed root authored text differs.");uint text=Word(native.InstanceBuffer,pointer+4);if((text!=0)!=Slots(native.RelocationBuffer).Contains((uint)(pointer+4)))throw new InvalidDataException("Pointed text nested relocation differs.");}
            else if(item.i==2){var values=profile==0?new[]{5u,10u,uint.MaxValue,7u}:profile==1?new uint[4]:new[]{200u,300u,400u,500u};var rows=GameObjectStockReview.Tokens(b,prefix-60);if(rows.Count!=4)throw new InvalidDataException("Flammability metadata inventory differs.");int i=0;foreach(string name in new[]{"Fuel","MaxBurnRate","Decay","Resistance"}){var row=rows[FastHash.GetHashCode(name)];if(row.Type!=0x763177A4 || row.Offset!=prefix+pointer+i*4 || Word(b,row.Offset)!=values[i])throw new InvalidDataException("Flammability literal/native/token differs.");i++;}}
            else{var vector=profile==0?new[]{1f,-2f,3f}:profile==1?new float[3]:new[]{0.25f,-3.5f,250f};for(int i=0;i<3;i++)if(Word(b,prefix+pointer+i*4)!=BitConverter.SingleToUInt32Bits(vector[i]))throw new InvalidDataException("Camera vector literal differs.");}
        }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independent diagnostic fields do not reuse the production owner inventory. */
    //-------------------------------------------------------------------------------------------------
    private static (string Name,int Slot,uint Type)[] Fields()=>new[]{("DisplayName",404,0x1A2933AAu),("DisplayNameTransformed",408,0x1A2933AAu),("Flammability",464,0x10444D6Cu),("LiveCameraOffset",528,0x29368077u)};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: metadata corruption locates root rows by name without assuming their new ordering. */
    //-------------------------------------------------------------------------------------------------
    private static int FindRow(byte[] b,string name){uint hash=FastHash.GetHashCode(name);for(int i=0;i<512;i++){int at=i*12;if(Word(b,at)==hash)return at;if(Word(b,at)==0 && Word(b,at+4)==0 && Word(b,at+8)==0)break;}throw new InvalidDataException("Pointed root row missing.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: ASCII text headers have bounded length/pointer/terminator semantics; arbitrary localization tables/Unicode are not established. */
    //-------------------------------------------------------------------------------------------------
    private static string ReadText(byte[] b,int header){uint length=Word(b,header),pointer=Word(b,header+4);if(pointer==0){if(length!=0)throw new InvalidDataException("Absent pointed text has length.");return "";}if(pointer>=b.Length || length>b.Length-pointer-1 || b[(int)(pointer+length)]!=0)throw new InvalidDataException("Pointed text bounds differ.");var span=b.AsSpan((int)pointer,(int)length);if(span.IndexOfAnyInRange((byte)128,byte.MaxValue)>=0 || span.IndexOf((byte)0)>=0)throw new InvalidDataException("Unsupported pointed text encoding.");return Encoding.ASCII.GetString(span);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: source/native/metadata failures cannot become partially successful composition. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action a){try{a();}catch(Exception e)when(e is InvalidDataException or OverflowException or FormatException){return;}throw new InvalidDataException("Malformed pointed root admitted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: controlled XML inputs do not resolve external entities. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Xml(string text){var d=new XmlDocument{XmlResolver=null};d.LoadXml(text);return d;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: all mutation buffers are privately owned. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c)=>new(){InstanceBuffer=c.InstanceBuffer.ToArray(),RelocationBuffer=c.RelocationBuffer.ToArray(),ImportsBuffer=c.ImportsBuffer.ToArray()};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: auxiliary slots retain checked sentinel framing. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b){if(b.Length==0)return Array.Empty<uint>();if(b.Length<8 || b.Length%4!=0 || Word(b,b.Length-4)!=uint.MaxValue)throw new InvalidDataException("Pointed-root slot stream malformed.");return Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: mutation/extraction keeps empty streams or one trailing sentinel. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> slots){var a=slots.ToArray();if(a.Length==0)return Array.Empty<byte>();var b=new byte[(a.Length+1)*4];for(int i=0;i<a.Length;i++)Put(b,i*4,a[i]);Put(b,a.Length*4,uint.MaxValue);return b;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: native/metadata word access is bounded little-endian data, never pointer dereferencing. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at){if(at<0 || at>b.Length-4)throw new InvalidDataException("Pointed-root read bounds exceeded.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: only owned output/control arrays are modified. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
