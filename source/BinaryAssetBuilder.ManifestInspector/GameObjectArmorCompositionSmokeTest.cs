using System.Buffers.Binary;
using System.Xml;
using System.Xml.Schema;
using System.Text.Json;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: combine actual Core ArmorSet names with root/body/destruction output; no complete asset or runtime publication authority.
internal static class GameObjectArmorCompositionSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove ordered armor lists and optional DamageFX imports in one named native/token graph, with stock metadata corroboration. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string globalManifest,string? authoredDirectory=null)
    {
        var saved=Settings.Current;
        try
        {
            XmlSchemaSet? official=null;SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:s=>official=s);
            if(official==null)throw new InvalidDataException("Reviewed armor/root schema required.");
            Settings.Current=new Settings {SchemaPath=Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"RootArmorBodyPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);CheckSchema(official,schemas.Schemas);
            var catalog=new[]{Catalog(manifest),Catalog(globalManifest)}.SelectMany(d=>d.Assets).ToArray();
            // Reborn: optional authored mode must consume a real scoped Core armor stream, never a synthetic catalog entry.
            var stockCatalog=catalog;ManifestAsset? authoredArmor=null;
            if(authoredDirectory!=null)
            {
                var built=BuildArmor(authoredDirectory);authoredArmor=built.Assets.Single(a=>a.Name=="ArmorTemplate:RebornComposedArmor");
                if(stockCatalog.Any(a=>a.TypeId==authoredArmor.TypeId && a.InstanceId==authoredArmor.InstanceId))throw new InvalidDataException("Authored armor identity collides with stock.");
                catalog=stockCatalog.Concat(built.Assets).ToArray();
            }
            string[] types={"UnitTypeIcon","UnitIntro","ArmorTemplate","DamageFX","FXList","AttributeModifier"};
            var targets=types.Select(type=>catalog.First(a=>a.TypeId==FastHash.GetHashCode(type))).ToArray();
            if(authoredArmor!=null)targets[2]=authoredArmor;
            Chunk? populated=null;InstanceDeclaration? populatedInstance=null;int cases=0;
            foreach(int count in new[]{0,1,2,32})foreach(bool damage in new[]{false,true})foreach(bool strings in new[]{false,true})
            {
                string source=Source(targets,count,damage);var raw=Adapt(Xml(source).DocumentElement!,strings);GameObjectBodyProjectionSmokeTest.Validate(official,raw.OuterXml);
                var instance=Normalize(schemas,source,catalog);var native=GameObjectBodyProjectionSmokeTest.Compile(Adapt(instance.XmlNode,strings));var output=Check(native,instance,count,damage,targets);
                var repeatInstance=Normalize(schemas,source,catalog);var repeat=Check(GameObjectBodyProjectionSmokeTest.Compile(Adapt(repeatInstance.XmlNode,strings)),repeatInstance,count,damage,targets);
                if(!output.InstanceBuffer.SequenceEqual(repeat.InstanceBuffer) || !output.RelocationBuffer.SequenceEqual(repeat.RelocationBuffer) || !output.ImportsBuffer.SequenceEqual(repeat.ImportsBuffer))throw new InvalidDataException("Combined ArmorSet repeat drift.");
                if(count==2 && damage && strings){populated=native;populatedInstance=instance;}cases++;
            }
            // Reborn: stock establishes the emitted root/list/member field contracts independently of the production writer.
            foreach(string name in new[]{"GameObject:AlliedConstructionYard","GameObject:SovietAntiInfantryInfantry"})
            {
                var c=GameObjectStockReview.Capture(manifest,name);var root=GameObjectStockReview.Tokens(c.Bytes,0);int table=root[FastHash.GetHashCode("ArmorSet")].Offset;
                if(root[FastHash.GetHashCode("ArmorSet")].Type!=0x5E5AF575 || Word(c.Bytes,table)!=0x6C827073 || Word(c.Bytes,table+4)!=0x5D448FD8 || Word(c.Bytes,table+8)!=c.Projection.NativeRoot+420)throw new InvalidDataException("Stock ArmorSet root/list contract differs.");
                foreach(var armor in c.Projection.Armors)
                {
                    var fields=GameObjectStockReview.Tokens(c.Bytes,armor.TokenTable);
                    foreach(var f in Fields())if(fields[FastHash.GetHashCode(f.Name)].Type!=f.Type || fields[FastHash.GetHashCode(f.Name)].Offset!=armor.NativeOffset+f.Offset)throw new InvalidDataException("Stock ArmorSet member contract differs.");
                }
            }
            int items=(int)Word(populated!.InstanceBuffer,424),list=(int)Word(populated.InstanceBuffer,476),failures=0;
            foreach(var fault in new[]{(420,33u),(420,0u),(424,4u),(424,(uint)list),(items+8,1u<<23),(items,0u)})
            {var bad=Clone(populated);Put(bad.InstanceBuffer,fault.Item1,fault.Item2);if(fault.Item1==items)Refuse(()=>Check(bad,populatedInstance!,2,true,targets));else Refuse(()=>Ep1GameObjectDestructionProjection.Compose(bad));failures++;}
            var missing=Clone(populated);missing.RelocationBuffer=Pack(Slots(missing.RelocationBuffer).Where(s=>s!=424));Refuse(()=>Ep1GameObjectDestructionProjection.Compose(missing));failures++;
            var import=Clone(populated);import.ImportsBuffer=Pack(Slots(import.ImportsBuffer).Where(s=>s!=items+4));Refuse(()=>Ep1GameObjectDestructionProjection.Compose(import));failures++;
            var alias=Clone(populated);Put(alias.InstanceBuffer,424,Word(alias.InstanceBuffer,128));Refuse(()=>Ep1GameObjectDestructionProjection.Compose(alias));failures++;
            string valid=Source(targets,2,true);
            // Reborn: removing the newly built manifest must make the same consumer fail, proving it did not silently bind to stock.
            if(authoredArmor!=null){Refuse(()=>Normalize(schemas,valid,stockCatalog));failures++;}
            foreach(string invalid in new[]{valid.Replace(targets[2].Name,"MissingRebornArmor",StringComparison.Ordinal),valid.Replace(targets[3].Name,"MissingRebornDamageFX",StringComparison.Ordinal),valid.Replace(targets[2].Name,targets[4].Name,StringComparison.Ordinal),valid.Replace("SECONDARY_DAMAGE","BAD_ARMOR_CONDITION",StringComparison.Ordinal)}){Refuse(()=>Normalize(schemas,invalid,catalog));failures++;}
            var good=Check(populated,populatedInstance!,2,true,targets);var rootGood=GameObjectStockReview.Tokens(good.InstanceBuffer,0);int armorTable=rootGood[FastHash.GetHashCode("ArmorSet")].Offset,metadataFailures=0;
            foreach(int at in new[]{(Ep1GameObjectRootValueTokens.AllFields.Count+2)*12+4,(Ep1GameObjectRootValueTokens.AllFields.Count+2)*12+8,armorTable,armorTable+4,armorTable+8,armorTable+12,armorTable+20,armorTable+40,armorTable+44,armorTable+52,armorTable+56,armorTable+64,armorTable+68})
            {var bad=Clone(good);Put(bad.InstanceBuffer,at,Word(bad.InstanceBuffer,at)^4u);Refuse(()=>Verify(bad,populated,populatedInstance!,2,true,targets));metadataFailures++;}
            Console.WriteLine($"GameObject ArmorSet composition: PASS ({cases} actual-Core cases: 0/1/2/32 armors, optional DamageFX, dynamic strings, root/body/destruction and shared references; two stock contracts, 89 root fields when both present, {failures} input and {metadataFailures} metadata refusals; no complete GameObject/package/game).");
            if(authoredArmor!=null)Console.WriteLine($"Authored armor consumer: PASS (real Core stream target {authoredArmor.Name}; target removal refuses; partial combined consumer only, no GameObject manifest/package/game).");
        }
        finally{Settings.Current=saved;}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: create only a fresh owned evidence child, build a genuine custom armor stream and independently read its published metadata. */
    //-------------------------------------------------------------------------------------------------
    internal static ManifestDocument BuildArmor(string directory)
    {
        directory=SdkEnvironmentPreflight.Absolute(directory);
        if(Directory.Exists(directory) || File.Exists(directory))throw new InvalidDataException("Authored armor composition output occupied.");
        SdkEnvironmentPreflight.DirectoryPath(Path.GetDirectoryName(directory)!);Directory.CreateDirectory(directory);
        string source=Path.Combine(directory,"source.xml");
        File.WriteAllText(source,"<!-- Reborn: owned authored armor composition fixture. --><ArmorTemplate xmlns='uri:ea.com:eala:asset' id='RebornComposedArmor'><Armor Damage='GUN' Percent='250.5'/></ArmorTemplate>");
        GeneralArmorAuthoredBuild.Run(source,Path.Combine(directory,"armor-build"));
        using var receipt=JsonDocument.Parse(File.ReadAllText(Path.Combine(directory,"armor-build","general-armor-build.json")));
        return Catalog(receipt.RootElement.GetProperty("ManifestPath").GetString()!);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: copied member declarations must retain the official type/default/fixed/use contracts before Core annotates references. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckSchema(XmlSchemaSet official,XmlSchemaSet focused)
    {
        var a=(XmlSchemaComplexType)official.GlobalTypes[new XmlQualifiedName("ArmorTemplateSet",SchemaSet.XmlNamespace)]!;var b=(XmlSchemaComplexType)focused.GlobalTypes[a.QualifiedName]!;
        if(a.AttributeUses.Count!=b.AttributeUses.Count)throw new InvalidDataException("ArmorSet declaration count drift.");
        foreach(XmlSchemaAttribute attr in a.AttributeUses.Values){var other=(XmlSchemaAttribute)b.AttributeUses[attr.QualifiedName]!;if(other==null || attr.AttributeSchemaType!.QualifiedName!=other.AttributeSchemaType!.QualifiedName || attr.Use!=other.Use || attr.DefaultValue!=other.DefaultValue || attr.FixedValue!=other.FixedValue)throw new InvalidDataException("ArmorSet attribute declaration drift.");}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: the independent stock reader checks linked EP1 metadata without reading a whole BIN file. */
    //-------------------------------------------------------------------------------------------------
    private static ManifestDocument Catalog(string path){var d=ManifestReader.Read(SdkEnvironmentPreflight.Read(path,64*1048576));TypeRegistryAudit.ValidateTarget(d.Header.Version,d.Header.AllTypesHash);if(!d.Header.IsLinked || d.Validate().Count!=0)throw new InvalidDataException("Valid ArmorSet lookup catalog required.");return d;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: repeated named targets deliberately remain ordered Core occurrences rather than a deduplicated hand-built index table. */
    //-------------------------------------------------------------------------------------------------
    private static string Source(ManifestAsset[] t,int count,bool damage)=>$"<RootArmorBodyProbe xmlns='uri:ea.com:eala:asset' id='ArmoredBody' UnitTypeIcon='{t[0].Name}' UnitIntro='{t[1].Name}'>"+string.Concat(Enumerable.Range(0,count).Select(i=>$"<ArmorSet Armor='{t[2].Name}' "+(damage?$"DamageFX='{t[3].Name}' ":"")+$"Conditions='{(i%2==0?"SECONDARY_DAMAGE":"")}'/>"))+$"<BuildingDestructionBehavior id='Destroy'><DieMuxData/><Item FX='{t[4].Name}'/></BuildingDestructionBehavior><ActiveBody id='Body' MaxHealth='1234' HealingBuffFX='{t[4].Name}'><DamageRangeModifier Min='25%' Max='75%' AttributeModifier='{t[5].Name}'/></ActiveBody></RootArmorBodyProbe>";
    //-------------------------------------------------------------------------------------------------
    /** Reborn: root ArmorSet children precede Behaviors, and existing EP1 Body allocation stays last on the same Tracker. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Adapt(XmlNode probe,bool strings)
    {
        var d=Xml("<GameObject xmlns='uri:ea.com:eala:asset' id='ArmoredBody'><Behaviors/><Body/></GameObject>");foreach(string name in new[]{"UnitTypeIcon","UnitIntro"})d.DocumentElement!.SetAttribute(name,probe.Attributes![name]!.Value);
        if(strings)d.DocumentElement!.SetAttribute("EditorName",new string('R',300));
        foreach(XmlElement child in probe.ChildNodes.OfType<XmlElement>())
            if(child.LocalName=="ArmorSet")d.DocumentElement!.InsertBefore(d.ImportNode(child,true),d.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace)[0]);
            else if(child.LocalName=="BuildingDestructionBehavior")d.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace)[0]!.AppendChild(d.ImportNode(child,true));
            else d.GetElementsByTagName("Body",SchemaSet.XmlNamespace)[0]!.AppendChild(d.ImportNode(child,true));return d;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject authored numeric suffixes and require one concrete catalog target for every actual Core strong occurrence. */
    //-------------------------------------------------------------------------------------------------
    private static InstanceDeclaration Normalize(SchemaSet schemas,string source,ManifestAsset[] catalog){if(source.Contains('\\'))throw new InvalidDataException("Authored ArmorSet indices refused.");var r=ReferencePipelineSmokeTest.Normalize(schemas,source);foreach(var h in r.ReferencedInstances)if(catalog.Count(a=>a.TypeId==h.TypeId && a.InstanceId==h.InstanceId)!=1)throw new InvalidDataException("Missing/ambiguous ArmorSet graph dependency.");return r;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compose once and separately validate current native/source identities and every newly generated armor token. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Check(Chunk native,InstanceDeclaration instance,int count,bool damage,ManifestAsset[] targets){var output=Ep1GameObjectDestructionProjection.Compose(native);Verify(output,native,instance,count,damage,targets);return output;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate detached output, armor/source occurrence order and full native/auxiliary inverse shifting without invoking the writer again. */
    //-------------------------------------------------------------------------------------------------
    private static void Verify(Chunk output,Chunk native,InstanceDeclaration instance,int count,bool damage,ManifestAsset[] targets)
    {
        var b=output.InstanceBuffer;var root=GameObjectStockReview.Tokens(b,0);int prefix=root[FastHash.GetHashCode("BuildTime")].Offset-140,items=(int)Word(native.InstanceBuffer,424),body=(int)Word(native.InstanceBuffer,488);
        if(root.Count!=(count==0?88:89) || Word(b,prefix+420)!=count || (count!=0 && Word(b,prefix+424)!=prefix+items) || Word(b,prefix+488)!=prefix+body)throw new InvalidDataException("Combined ArmorSet root ownership differs.");
        int expectedPrefix=(Ep1GameObjectRootValueTokens.AllFields.Count+3+(count==0?0:1))*12+24+384+384+(count==0?0:12+count*60);
        if(prefix!=expectedPrefix)throw new InvalidDataException("Combined ArmorSet token extent differs.");
        if(count!=0)
        {
            int table=root[FastHash.GetHashCode("ArmorSet")].Offset;
            if(root[FastHash.GetHashCode("ArmorSet")].Type!=0x5E5AF575 || table!=prefix-(12+count*60) || Word(b,table)!=0x6C827073 || Word(b,table+4)!=0x5D448FD8 || Word(b,table+8)!=prefix+420)throw new InvalidDataException("ArmorSet root/list token differs.");
            for(int i=0;i<count;i++)
            {
                int memberTable=table+12+count*12+i*48;
                if(Word(b,table+12+i*12)!=0xB151A0EB || Word(b,table+16+i*12)!=0 || Word(b,table+20+i*12)!=memberTable)throw new InvalidDataException("ArmorSet marker/table ownership differs.");
                var fields=GameObjectStockReview.Tokens(b,memberTable);
                foreach(var f in Fields())if(!fields.TryGetValue(FastHash.GetHashCode(f.Name),out var token) || token.Type!=f.Type || token.Offset!=prefix+items+i*12+f.Offset)throw new InvalidDataException("ArmorSet member token differs.");
                if(Word(b,prefix+items+i*12+8)!=(i%2==0?8192u:0u))throw new InvalidDataException("ArmorSet authored condition mask differs.");
            }
        }
        int behavior=(int)Word(native.InstanceBuffer,476),module=(int)Word(native.InstanceBuffer,behavior+8),destructionItems=(int)Word(native.InstanceBuffer,module+68);
        var expected=new List<(int Slot,int Target)>{(368,0),(400,1)};
        for(int i=0;i<count;i++){expected.Add((items+i*12,2));if(damage)expected.Add((items+i*12+4,3));else if(Word(b,prefix+items+i*12+4)!=0)throw new InvalidDataException("Omitted DamageFX emitted an index.");}
        expected.Add((destructionItems+8,4));expected.Add((body+44,4));expected.Add((body+76+8,5));
        if(instance.ReferencedInstances.Count!=expected.Count)throw new InvalidDataException("Armor graph occurrence count differs.");
        for(int i=0;i<expected.Count;i++){var r=instance.ReferencedInstances[i];var t=targets[expected[i].Target];if(r.TypeId!=t.TypeId || r.InstanceId!=t.InstanceId || Word(b,prefix+expected[i].Slot)!=i+1)throw new InvalidDataException("Armor graph dependency identity/index differs.");}
        var recovered=b.AsSpan(prefix).ToArray();foreach(uint s in Slots(output.RelocationBuffer)){if(s<prefix || Word(b,(int)s)<prefix)throw new InvalidDataException("Armor graph pointer escapes native storage.");Put(recovered,(int)s-prefix,Word(b,(int)s)-(uint)prefix);}
        if(!recovered.SequenceEqual(native.InstanceBuffer) || !Slots(output.RelocationBuffer).Select(s=>s-(uint)prefix).SequenceEqual(Slots(native.RelocationBuffer)) || !Slots(output.ImportsBuffer).Select(s=>s-(uint)prefix).SequenceEqual(Slots(native.ImportsBuffer)) || !Slots(output.ImportsBuffer).Order().SequenceEqual(expected.Select(e=>(uint)(prefix+e.Slot)).Order()))throw new InvalidDataException("Armor graph native/RELO/IMP round trip differs.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independent stock-derived member contracts are separate from the inline production emitter. */
    //-------------------------------------------------------------------------------------------------
    private static (string Name,int Offset,uint Type)[] Fields()=>new[]{("Armor",0,0x34E9127Bu),("DamageFX",4,0x9A95EDFFu),("Conditions",8,0xED8BC8E9u)};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: fixture XML does not resolve external entities. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Xml(string s){var d=new XmlDocument {XmlResolver=null};d.LoadXml(s);return d;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: all negative controls use detached buffers. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c)=>new(){InstanceBuffer=c.InstanceBuffer.ToArray(),RelocationBuffer=c.RelocationBuffer.ToArray(),ImportsBuffer=c.ImportsBuffer.ToArray()};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: native streams retain absent versus sentinel-terminated forms. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b)=>b.Length==0?Array.Empty<uint>():Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();
    //-------------------------------------------------------------------------------------------------
    /** Reborn: detached owner faults preserve one final sentinel. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> s){var a=s.Append(uint.MaxValue).ToArray();var b=new byte[a.Length*4];for(int i=0;i<a.Length;i++)Put(b,i*4,a[i]);return b;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: schema/type/catalog and native/token failures must remain controlled refusals. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action a){try{a();}catch(Exception e)when(e is InvalidDataException or BinaryAssetBuilderException or XmlSchemaValidationException){return;}throw new InvalidDataException("Malformed ArmorSet graph admitted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: little-endian native/token reads stay bounded. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));
    //-------------------------------------------------------------------------------------------------
    /** Reborn: only owned test buffers are mutated. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint v)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),v);
}
