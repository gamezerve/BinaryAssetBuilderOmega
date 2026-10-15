using System.Buffers.Binary;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: independently prove movement records against stock before combining them with actual Core combat/body/armor graphs.
internal static class GameObjectMovementCompositionSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: test the root-specific enum, four independent stock lists, real references, dynamic composition and refusal controls without publishing a GameObject. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string globalManifest)
    {
        Stock(manifest);
        var saved=Settings.Current;
        try
        {
            XmlSchemaSet? official=null;SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:s=>official=s);
            if(official==null)throw new InvalidDataException("Reviewed movement source schema required.");
            Settings.Current=new Settings{SchemaPath=Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"RootMovementPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);Drift(official,schemas.Schemas);
            var catalog=new[]{manifest,globalManifest}.SelectMany(p=>Catalog(p).Assets).ToArray();
            string[] types={"UnitTypeIcon","UnitIntro","ArmorTemplate","DamageFX","FXList","AttributeModifier","WeaponTemplate","UpgradeTemplate","LocomotorTemplate"};
            var targets=types.Select(t=>catalog.First(a=>a.TypeId==FastHash.GetHashCode(t))).ToArray();
            int cases=0;Chunk? populated=null;InstanceDeclaration? populatedInstance=null;
            foreach(int count in new[]{0,1,3,32})foreach(bool reverse in new[]{false,true})foreach(int position in new[]{0,1,2})foreach(bool strings in new[]{false,true})
            {
                string source=Source(targets,count,"SPECIALABILITY_ACTIVE",reverse,position);
                var (native,instance)=Compile(source,strings);var output=Ep1GameObjectDestructionProjection.Compose(native);Verify(output,native,instance,targets[8],count,18);
                var repeat=Compile(source,strings);var other=Ep1GameObjectDestructionProjection.Compose(repeat.Native);
                if(!output.InstanceBuffer.SequenceEqual(other.InstanceBuffer) || !output.RelocationBuffer.SequenceEqual(other.RelocationBuffer) || !output.ImportsBuffer.SequenceEqual(other.ImportsBuffer))throw new InvalidDataException("Movement repeat differs.");
                if(count==3 && !reverse && position==0 && strings){populated=native;populatedInstance=instance;}cases++;
            }
            foreach(string condition in Enum.GetNames<Ep1RootLocomotorSetType>())
            {
                var pair=Compile(Source(targets,1,condition,false,0),false);Verify(Ep1GameObjectDestructionProjection.Compose(pair.Native),pair.Native,pair.Instance,targets[8],1,(int)Enum.Parse<Ep1RootLocomotorSetType>(condition));cases++;
            }
            // Reborn: omitted optional attributes are native zeros and create no phantom import; an explicit empty record is still a present list.
            var absent=Compile(Source(targets,1,"NORMAL",false,0).Replace($" Speed='75' Condition='NORMAL' Locomotor='{targets[8].Name}'","",StringComparison.Ordinal),false);
            int emptyItems=(int)Word(absent.Native.InstanceBuffer,432);
            if(absent.Native.InstanceBuffer.AsSpan(emptyItems,12).IndexOfAnyExcept((byte)0)>=0 || Slots(absent.Native.ImportsBuffer).Contains((uint)(emptyItems+8)))throw new InvalidDataException("Movement omitted defaults/import differ.");
            Ep1GameObjectDestructionProjection.Compose(absent.Native);cases++;
            int movement=(int)Word(populated!.InstanceBuffer,432),list=(int)Word(populated.InstanceBuffer,476),faults=0;
            foreach(var fault in new[]{(428,33u),(432,0u),(432,Word(populated.InstanceBuffer,424)),(432,(uint)list),(movement+4,19u),(movement+4,0xFFFFFFFEu),(movement,0x7F800000u),(movement,0x7FC00000u)})
            {var bad=Clone(populated);Put(bad.InstanceBuffer,fault.Item1,fault.Item2);Refuse(()=>Ep1GameObjectDestructionProjection.Compose(bad));faults++;}
            foreach(uint slot in new[]{432u,(uint)(movement+8)})
            {var bad=Clone(populated);if(slot==432)bad.RelocationBuffer=Pack(Slots(bad.RelocationBuffer).Where(s=>s!=slot));else bad.ImportsBuffer=Pack(Slots(bad.ImportsBuffer).Where(s=>s!=slot));Refuse(()=>Ep1GameObjectDestructionProjection.Compose(bad));faults++;}
            var alias=Clone(populated);Put(alias.InstanceBuffer,124,0);Put(alias.InstanceBuffer,128,(uint)movement);Refuse(()=>Ep1GameObjectDestructionProjection.Compose(alias));faults++;
            string valid=Source(targets,3,"NORMAL",false,0);
            foreach(string bad in new[]{valid.Replace("Condition='NORMAL'","Condition='COMBO2'",StringComparison.Ordinal),valid.Replace("Condition='NORMAL'","Condition='UNKNOWN'",StringComparison.Ordinal),valid.Replace(targets[8].Name,"MissingRebornLocomotor",StringComparison.Ordinal),valid.Replace("Speed='75'","Speed='not-a-real'",StringComparison.Ordinal)}){Refuse(()=>Compile(bad,false));faults++;}
            var good=Ep1GameObjectDestructionProjection.Compose(populated);var root=GameObjectStockReview.Tokens(good.InstanceBuffer,0);int table=root[FastHash.GetHashCode("LocomotorSet")].Offset,memberTable=(int)Word(good.InstanceBuffer,table+20),metadataFaults=0;
            foreach(int offset in new[]{table,table+4,table+8,table+12,table+16,table+20,memberTable,memberTable+4,memberTable+8,memberTable+12,memberTable+16,memberTable+20,memberTable+24,memberTable+28,memberTable+32,memberTable+36})
            {var bad=Clone(good);Put(bad.InstanceBuffer,offset,Word(bad.InstanceBuffer,offset)^4u);Refuse(()=>Verify(bad,populated,populatedInstance!,targets[8],3,18));metadataFaults++;}
            Console.WriteLine($"GameObject movement composition: PASS (four stock roots, {cases} Core/native cases including 20 enum literals, 0/1/3/32 records, three behavior orders, armor/body/weapons/strings; {faults} input and {metadataFaults} metadata refusals; 90-field partial root when present; no generic locomotor processor/object publication/game).");

            //-------------------------------------------------------------------------------------------------
            /** Reborn: raw official validation precedes focused Core normalization and unique catalog closure, with no caller-authored reference indices. */
            //-------------------------------------------------------------------------------------------------
            (Chunk Native,InstanceDeclaration Instance) Compile(string source,bool strings)
            {
                if(source.Contains('\\'))throw new InvalidDataException("Authored movement indices refused.");
                var raw=new XmlDocument{XmlResolver=null};raw.LoadXml(source);GameObjectBodyProjectionSmokeTest.Validate(official,GameObjectCombatCompositionSmokeTest.Adapt(raw.DocumentElement!,strings).OuterXml);
                var instance=ReferencePipelineSmokeTest.Normalize(schemas,source);
                foreach(var reference in instance.ReferencedInstances)if(catalog.Count(a=>a.TypeId==reference.TypeId && a.InstanceId==reference.InstanceId)!=1)throw new InvalidDataException("Movement target missing/ambiguous.");
                return(GameObjectBodyProjectionSmokeTest.Compile(GameObjectCombatCompositionSmokeTest.Adapt(instance.XmlNode,strings),selectedBehaviors:true),instance);
            }
        }
        finally{Settings.Current=saved;}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: source templates share the already reviewed three-module combat graph without embedding native indices or a second allocator. */
    //-------------------------------------------------------------------------------------------------
    private static string Source(ManifestAsset[] targets,int count,string condition,bool reverse,int position)
    {
        string source=GameObjectCombatCompositionSmokeTest.Source(targets,reverse,2,3,position).Replace("RootCombatProbe","RootMovementProbe",StringComparison.Ordinal);
        int after=source.IndexOf("/>",StringComparison.Ordinal)+2;
        return source.Insert(after,string.Concat(Enumerable.Repeat($"<LocomotorSet Speed='75' Condition='{condition}' Locomotor='{targets[8].Name}'/>",count)));
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: schema names/defaults/reference annotations are checked independently rather than accepting a convenient diagnostic-only definition. */
    //-------------------------------------------------------------------------------------------------
    private static void Drift(XmlSchemaSet official,XmlSchemaSet focused)
    {
        var key=new XmlQualifiedName("LocomotorSet",SchemaSet.XmlNamespace);var a=(XmlSchemaComplexType)official.GlobalTypes[key]!;var b=(XmlSchemaComplexType)focused.GlobalTypes[key]!;
        if(a.AttributeUses.Count!=b.AttributeUses.Count)throw new InvalidDataException("Movement schema inventory drift.");
        foreach(XmlSchemaAttribute field in a.AttributeUses.Values)
        {
            var other=(XmlSchemaAttribute)b.AttributeUses[field.QualifiedName]!;
            if(other==null || field.AttributeSchemaType!.QualifiedName!=other.AttributeSchemaType!.QualifiedName || field.DefaultValue!=other.DefaultValue || field.FixedValue!=other.FixedValue || field.Use!=other.Use || !Annotations(field).SequenceEqual(Annotations(other)))throw new InvalidDataException("Movement schema field drift.");
        }
        var e=(XmlSchemaSimpleType)official.GlobalTypes[new XmlQualifiedName("LocomotorSetType",SchemaSet.XmlNamespace)]!;
        var names=((XmlSchemaSimpleTypeRestriction)e.Content!).Facets.OfType<XmlSchemaEnumerationFacet>().Select(f=>f.Value!).ToArray();
        if(names.Length!=20 || names.Where((n,i)=>!Enum.IsDefined(typeof(Ep1RootLocomotorSetType),n) || (int)Enum.Parse<Ep1RootLocomotorSetType>(n)!=i-1).Any())throw new InvalidDataException("Root movement enum drift.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: pipeline-only and target-type attributes survive the focused schema while unrelated balance/UI annotations need no native representation. */
    //-------------------------------------------------------------------------------------------------
    private static IEnumerable<string> Annotations(XmlSchemaAttribute a)=>(a.UnhandledAttributes??Array.Empty<XmlAttribute>()).Where(x=>x.NamespaceURI=="uri:ea.com:eala:asset:schema").Select(x=>x.LocalName+"="+x.Value).Order();
    //-------------------------------------------------------------------------------------------------
    /** Reborn: both independent target catalogs must be valid linked EP1 metadata rather than merely readable files. */
    //-------------------------------------------------------------------------------------------------
    private static ManifestDocument Catalog(string path){var d=ManifestReader.Read(SdkEnvironmentPreflight.Read(path,64*1048576));TypeRegistryAudit.ValidateTarget(d.Header.Version,d.Header.AllTypesHash);if(!d.Header.IsLinked || d.Validate().Count!=0)throw new InvalidDataException("Movement catalog invalid.");return d;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: pin independent stock values including the late EP1 conditions and exact member token types/relative locations. */
    //-------------------------------------------------------------------------------------------------
    private static void Stock(string manifest)
    {
        foreach(var item in new[]{("SovietAntiInfantryInfantry",new[]{50f},new[]{0}),("SovietAntiNavyShipTech2",new[]{75f,75f,75f},new[]{0,12,17}),("AlliedArtilleryVehicle",new[]{75f,75f,0f},new[]{0,0,18}),("JapanFortressShip",new[]{40f,30f},new[]{0,6})})
        {
            var stock=GameObjectStockReview.Capture(manifest,"GameObject:"+item.Item1);var b=stock.Bytes;int native=stock.Projection.NativeRoot,items=(int)Word(b,native+432);var root=GameObjectStockReview.Tokens(b,0);var field=root[FastHash.GetHashCode("LocomotorSet")];
            if(Word(b,native+428)!=item.Item2.Length || field.Type!=0x6E6BF482)throw new InvalidDataException("Stock movement root differs.");
            VerifyTable(b,field.Offset,native,items,item.Item2.Length);
            var source=new XmlDocument{XmlResolver=null};source.LoadXml("<GameObject xmlns='uri:ea.com:eala:asset' id='StockMovement'><Behaviors><BuildingDestructionBehavior id='Destroy'><DieMuxData/><Item/></BuildingDestructionBehavior></Behaviors><Body><ActiveBody id='Body' MaxHealth='100'/></Body></GameObject>");
            for(int i=0;i<item.Item2.Length;i++)
            {
                uint reference=Word(b,items+i*12+8);
                if(BitConverter.Int32BitsToSingle((int)Word(b,items+i*12))!=item.Item2[i] || (int)Word(b,items+i*12+4)!=item.Item3[i] || reference==0 || reference>stock.Asset.References.Count)throw new InvalidDataException("Stock movement values differ.");
                var identity=stock.Asset.References[(int)reference-1];if(identity.TypeId!=FastHash.GetHashCode("LocomotorTemplate") || stock.Document.Assets.Count(a=>a.TypeId==identity.TypeId && a.InstanceId==identity.InstanceId)!=1)throw new InvalidDataException("Stock movement dependency type/identity differs.");
                // Reborn: captured indices are used only for this isolated native-equality diagnostic; authored source above always uses actual Core names.
                var record=source.CreateElement("LocomotorSet",SchemaSet.XmlNamespace);record.SetAttribute("Speed",item.Item2[i].ToString("R",System.Globalization.CultureInfo.InvariantCulture));record.SetAttribute("Condition",Enum.GetName((Ep1RootLocomotorSetType)item.Item3[i])!);record.SetAttribute("Locomotor","CapturedMovement\\"+(reference-1));source.DocumentElement!.InsertBefore(record,source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace)[0]);
            }
            var compiled=GameObjectBodyProjectionSmokeTest.Compile(source,selectedBehaviors:true);int actualItems=(int)Word(compiled.InstanceBuffer,432);
            if(!compiled.InstanceBuffer.AsSpan(actualItems,item.Item2.Length*12).SequenceEqual(b.AsSpan(items,item.Item2.Length*12)))throw new InvalidDataException("Stock movement native marshalling differs.");
        }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independent list metadata verification admits no extra members, wrong types, dangling targets or nonzero terminators. */
    //-------------------------------------------------------------------------------------------------
    private static void VerifyTable(byte[] b,int table,int native,int items,int count)
    {
        if(Word(b,table)!=0x6C827073 || Word(b,table+4)!=0x5D448FD8 || Word(b,table+8)!=native+428)throw new InvalidDataException("Movement count token differs.");
        for(int i=0;i<count;i++)
        {
            int marker=table+12+i*12,fields=(int)Word(b,marker+8);
            if(Word(b,marker)!=0xB151A0EB || Word(b,marker+4)!=0)throw new InvalidDataException("Movement member marker differs.");
            var rows=GameObjectStockReview.Tokens(b,fields);if(rows.Count!=3)throw new InvalidDataException("Movement member inventory differs.");
            foreach(var f in new[]{("Speed",0x645864F3u,0),("Condition",0x845D12B2u,4),("Locomotor",0xFED20C6Cu,8)})if(!rows.TryGetValue(FastHash.GetHashCode(f.Item1),out var row) || row.Type!=f.Item2 || row.Offset!=items+i*12+f.Item3)throw new InvalidDataException("Movement member binding differs.");
        }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: generated metadata and all native/auxiliary coordinates are independently reversible; named targets retain Core's one-biased occurrence indices. */
    //-------------------------------------------------------------------------------------------------
    private static void Verify(Chunk output,Chunk native,InstanceDeclaration instance,ManifestAsset target,int count,int condition)
    {
        var b=output.InstanceBuffer;var roots=GameObjectStockReview.Tokens(b,0);int prefix=roots[FastHash.GetHashCode("BuildTime")].Offset-140,items=(int)Word(native.InstanceBuffer,432);
        if(roots.Count!=(count==0?89:90) || Word(b,prefix+428)!=count)throw new InvalidDataException("Movement composed root differs.");
        if(count!=0)
        {
            var field=roots[FastHash.GetHashCode("LocomotorSet")];int expectedTable=prefix-12-count*60;
            if(field.Type!=0x6E6BF482 || field.Offset!=expectedTable || Word(b,prefix+432)!=prefix+items)throw new InvalidDataException("Movement metadata prefix extent differs.");
            VerifyTable(b,field.Offset,prefix,prefix+items,count);
            for(int i=0;i<count;i++)
            {
                int at=prefix+items+i*12,index=(int)Word(b,at+8)-1;
                if(Word(b,at)!=BitConverter.SingleToUInt32Bits(75f) || unchecked((int)Word(b,at+4))!=condition || index<0 || index>=instance.ReferencedInstances.Count)throw new InvalidDataException("Movement native data/index differs.");
                var reference=instance.ReferencedInstances[index];if(reference.TypeId!=target.TypeId || reference.InstanceId!=target.InstanceId || !Slots(output.ImportsBuffer).Contains((uint)(at+8)))throw new InvalidDataException("Movement Core reference identity differs.");
            }
        }
        else if(roots.ContainsKey(FastHash.GetHashCode("LocomotorSet")))throw new InvalidDataException("Absent movement emitted metadata.");
        var recovered=b.AsSpan(prefix).ToArray();foreach(uint slot in Slots(output.RelocationBuffer)){if(slot<prefix || Word(b,(int)slot)<prefix)throw new InvalidDataException("Movement rebased pointer escapes native graph.");Put(recovered,(int)slot-prefix,Word(b,(int)slot)-(uint)prefix);}
        if(!recovered.SequenceEqual(native.InstanceBuffer) || !Slots(output.RelocationBuffer).Select(s=>s-(uint)prefix).SequenceEqual(Slots(native.RelocationBuffer)) || !Slots(output.ImportsBuffer).Select(s=>s-(uint)prefix).SequenceEqual(Slots(native.ImportsBuffer)))throw new InvalidDataException("Movement native/RELO/IMP round trip differs.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: malformed sources and owned diagnostic mutations must fail closed. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action){try{action();}catch(Exception e)when(e is InvalidDataException or XmlSchemaValidationException or BinaryAssetBuilderException){return;}throw new InvalidDataException("Malformed movement graph admitted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: negative controls own each mutable buffer. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c)=>new(){InstanceBuffer=c.InstanceBuffer.ToArray(),RelocationBuffer=c.RelocationBuffer.ToArray(),ImportsBuffer=c.ImportsBuffer.ToArray()};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: auxiliary slot readers exclude the already validated final sentinel. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b)=>b.Length==0?Array.Empty<uint>():Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();
    //-------------------------------------------------------------------------------------------------
    /** Reborn: owned slot-list mutations preserve an empty stream or its sentinel. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> slots){var a=slots.ToArray();if(a.Length==0)return Array.Empty<byte>();var b=new byte[(a.Length+1)*4];for(int i=0;i<a.Length;i++)Put(b,i*4,a[i]);Put(b,a.Length*4,uint.MaxValue);return b;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: native/token word reads use little-endian bounded spans. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));
    //-------------------------------------------------------------------------------------------------
    /** Reborn: negative controls only write owned buffers. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
