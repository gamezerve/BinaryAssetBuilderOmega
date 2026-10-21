using System.Buffers.Binary;
using System.Runtime.InteropServices;
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

// Reborn: standalone EP1 hardpoint native/token writer proof; no legacy dispatch replacement, turret support or general object publication.
internal static class Ep1WeaponSetSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare complete selected Conscript/Peacekeeper graphs, actual Core target identities, authored boundaries and detached faults. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string globalManifest)
    {
        var saved=Settings.Current;
        try
        {
            if(Marshal.SizeOf<Ep1WeaponSetUpdateModuleData>()!=44 || Marshal.SizeOf<Ep1WeaponSlotHardpoint>()!=28 || Marshal.SizeOf<Ep1WeaponSlotWeaponData>()!=80)throw new InvalidDataException("EP1 weapon-set ABI differs.");
            XmlSchemaSet? official=null;SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:s=>official=s);
            if(official==null)throw new InvalidDataException("Reviewed weapon-set schema required.");
            Settings.Current=new Settings {SchemaPath=Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"WeaponSetPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);
            // Reborn: all four inherited bases are empty apart from ModuleData identity; the focused schema must preserve their actual chain.
            foreach(string name in new[]{"ModuleData","BehaviorModuleData","ContainModuleData","UpdateModuleData"})
            {
                var a=(XmlSchemaComplexType)official.GlobalTypes[new XmlQualifiedName(name,SchemaSet.XmlNamespace)]!;var b=(XmlSchemaComplexType)schemas.Schemas.GlobalTypes[a.QualifiedName]!;
                if(a.BaseXmlSchemaType is not XmlSchemaType ab || b.BaseXmlSchemaType is not XmlSchemaType bb || ab.QualifiedName!=bb.QualifiedName || a.AttributeUses.Count!=b.AttributeUses.Count)throw new InvalidDataException("Weapon-set focused base drift.");
            }
            var catalog=new[]{Catalog(manifest),Catalog(globalManifest)}.SelectMany(d=>d.Assets).ToArray();
            foreach(bool soviet in new[]{false,true})
            {
                string name=soviet?"SovietAntiInfantryInfantry":"AlliedAntiInfantryInfantry";
                string source="<WeaponSetUpdate xmlns='uri:ea.com:eala:asset' id='ModuleTag_WeaponSetUpdate'><WeaponSlotHardpoint ID='1'><Weapon Ordering='PRIMARY_WEAPON' Template='"+(soviet?"SovietAntiInfantryInfantryAK47":"AlliedAntiInfantryInfantryShotgun")+"' ForbiddenObjectStatus='GENERIC_TOGGLE_STATE'/>"+(soviet?"<Weapon Ordering='SECONDARY_WEAPON' Template='SovietAntiInfantryInfantryMolotovCocktail' ObjectStatus='GENERIC_TOGGLE_STATE'/>":"")+"</WeaponSlotHardpoint></WeaponSetUpdate>";
                Ep1BasicModuleSmokeTest.Validate(official,source,false);var instance=Normalize(schemas,source,catalog);var native=Compile(instance.XmlNode);var result=Check(native,instance,catalog);
                var stock=GameObjectStockReview.Capture(manifest,"GameObject:"+name);var module=GameObjectModuleReview.Inspect(manifest,"GameObject:"+name).Containers.SelectMany(c=>c.Modules).Single(m=>m.Type=="WeaponSetUpdateModuleData");
                var expected=stock.Bytes.AsSpan(module.Native,native.InstanceBuffer.Length).ToArray();
                foreach(uint slot in Slots(native.RelocationBuffer))Put(expected,(int)slot,Word(expected,(int)slot)-(uint)module.Native);
                foreach(uint slot in Slots(native.ImportsBuffer))
                {
                    uint captured=Word(expected,(int)slot);if(captured==0 || captured>stock.Asset.References.Count)throw new InvalidDataException("Stock weapon reference bounds differ.");
                    var identity=stock.Asset.References[(int)captured-1];uint index=Word(native.InstanceBuffer,(int)slot);var r=instance.ReferencedInstances[(int)index-1];
                    if(identity.TypeId!=r.TypeId || identity.InstanceId!=r.InstanceId)throw new InvalidDataException("Stock weapon reference identity differs.");Put(expected,(int)slot,index);
                }
                // Reborn: bounded detached byte differences identify actual ABI/default mismatches without dumping native stock payloads.
                if(!expected.SequenceEqual(native.InstanceBuffer))throw new InvalidDataException("Complete selected weapon-set native graph differs: "+string.Join(", ",Enumerable.Range(0,expected.Length).Where(i=>expected[i]!=native.InstanceBuffer[i]).Take(16).Select(i=>$"+{i}: stock={expected[i]:X2}, built={native.InstanceBuffer[i]:X2}")));
                int prefix=result.InstanceBuffer.Length-native.InstanceBuffer.Length;var metadata=stock.Bytes.AsSpan(module.Wrapper,prefix).ToArray();
                for(int at=8;at<prefix;at+=12){uint target=Word(metadata,at);if(target!=0)Put(metadata,at,target>=(uint)module.Native?target-(uint)module.Native+(uint)prefix:target-(uint)module.Wrapper);}
                if(!metadata.SequenceEqual(result.InstanceBuffer.AsSpan(0,prefix).ToArray()))throw new InvalidDataException("Generated weapon-set metadata differs from independent stock.");
                Console.WriteLine($"EP1 weapon set stock {name}: native/token bytes={native.InstanceBuffer.Length}/{prefix}, complete selected native/metadata/import identities PASS.");
            }
            var weapon=catalog.First(a=>a.Name=="WeaponTemplate:SovietAntiInfantryInfantryAK47");var upgrade=catalog.First(a=>a.TypeId==0xE1AFE75B);Chunk? populated=null;InstanceDeclaration? populatedInstance=null;
            foreach(var shape in new[]{(0,0),(1,0),(1,1),(2,3),(8,16)})
            {
                string rows=string.Concat(Enumerable.Repeat($"<Weapon Ordering='PRIMARY_WEAPON' Template='{weapon.Name}' Upgrade='{upgrade.Name}' ObjectStatus='GENERIC_TOGGLE_STATE' ForbiddenObjectStatus='UNSELECTABLE' IsPlayerUpgradePermanent='true' ConquerMetricsAlwaysCount='true'/>",shape.Item2));
                string source="<WeaponSetUpdate xmlns='uri:ea.com:eala:asset' id='AuthoredWeapons'>"+string.Concat(Enumerable.Range(0,shape.Item1).Select(i=>$"<WeaponSlotHardpoint ID='{i+1}' AutoChooseWeapon='false' AllowInterleavedFiring='true'>{rows}</WeaponSlotHardpoint>"))+"</WeaponSetUpdate>";
                Ep1BasicModuleSmokeTest.Validate(official,source,false);var instance=Normalize(schemas,source,catalog);var native=Compile(instance.XmlNode);var a=Check(native,instance,catalog);var repeat=Normalize(schemas,source,catalog);var b=Check(Compile(repeat.XmlNode),repeat,catalog);
                if(!a.InstanceBuffer.SequenceEqual(b.InstanceBuffer) || !a.RelocationBuffer.SequenceEqual(b.RelocationBuffer) || !a.ImportsBuffer.SequenceEqual(b.ImportsBuffer) || instance.ReferencedInstances.Count!=shape.Item1*shape.Item2*2)throw new InvalidDataException("Authored weapon-set repeat/occurrence count differs.");
                for(int i=0;i<shape.Item1;i++)if(native.InstanceBuffer[44+i*28+24]!=1 || native.InstanceBuffer[44+i*28+25]!=0)throw new InvalidDataException("EP1 hardpoint explicit flags lost.");
                if(shape==(2,3)){populated=native;populatedInstance=instance;}
            }
            int failures=0;
            foreach(var fault in new[]{(0,0u),(12,9u),(16,0u),(20,1u),(44,1u),(52,2u),(56,6u),(60,17u),(64,0u),(68,2u),(100,1u)})
            {var bad=Clone(populated!);Put(bad.InstanceBuffer,fault.Item1,fault.Item2);Refuse(()=>Ep1WeaponSetTokenizer.Tokenize(bad));failures++;}
            var truncated=Clone(populated!);truncated.InstanceBuffer=truncated.InstanceBuffer.AsSpan(0,68).ToArray();Refuse(()=>Ep1WeaponSetTokenizer.Tokenize(truncated));failures++;
            var missing=Clone(populated!);missing.ImportsBuffer=Array.Empty<byte>();Refuse(()=>Ep1WeaponSetTokenizer.Tokenize(missing));failures++;
            var noRelo=Clone(populated!);noRelo.RelocationBuffer=Array.Empty<byte>();Refuse(()=>Ep1WeaponSetTokenizer.Tokenize(noRelo));failures++;
            string unsupported="<WeaponSetUpdate xmlns='uri:ea.com:eala:asset' id='Unsupported'><WeaponSlotTurret ID='1'><TurretSettings/></WeaponSlotTurret></WeaponSetUpdate>";
            Refuse(()=>Compile(ReferencePipelineSmokeTest.Normalize(schemas,unsupported).XmlNode));failures++;
            Refuse(()=>Normalize(schemas,"<WeaponSetUpdate xmlns='uri:ea.com:eala:asset' id='Bad'><WeaponSlotHardpoint><Weapon Template='MissingRebornWeapon'/></WeaponSlotHardpoint></WeaponSetUpdate>",catalog));failures++;
            // Reborn: detached metadata mutations exercise the independent verifier, not only producer-side native admission.
            var good=Check(populated!,populatedInstance!,catalog);int metadataFailures=0;
            foreach(int at in new[]{0,4,8,16,20,28,32,40,44,104,128,152,172,176,184,188,204,208,212,216,224,244,248,256,260,268,272,280,284,292,296,304,308,316,320,336,340,344,348,356,388,392,400,404,412,416,424,428,436,440,448,452,460,464})
            {var bad=Clone(good);Put(bad.InstanceBuffer,at,Word(bad.InstanceBuffer,at)^4u);Refuse(()=>Verify(bad,populated!,populatedInstance!,catalog));metadataFailures++;}
            Console.WriteLine($"EP1 WeaponSetUpdate: PASS (44/28/80 separate ABI, two complete selected stock native/token graphs, real Core weapon/upgrade imports, five authored list shapes, forbidden-status/auto-choice fields, {failures} input and {metadataFailures} metadata refusals; hardpoints only, no legacy dispatch/object composition/package/game).");
        }
        finally{Settings.Current=saved;}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: read bounded independently validated linked EP1 catalogs, not native target payloads. */
    //-------------------------------------------------------------------------------------------------
    private static ManifestDocument Catalog(string path){var d=ManifestReader.Read(SdkEnvironmentPreflight.Read(path,64*1048576));TypeRegistryAudit.ValidateTarget(d.Header.Version,d.Header.AllTypesHash);if(!d.Header.IsLinked || d.Validate().Count!=0)throw new InvalidDataException("Weapon-set catalog invalid.");return d;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: source names pass actual Core once and require unique real type/instance identities; authored suffixes are forbidden. */
    //-------------------------------------------------------------------------------------------------
    private static InstanceDeclaration Normalize(SchemaSet schemas,string source,ManifestAsset[] catalog){if(source.Contains('\\'))throw new InvalidDataException("Authored weapon indices refused.");var r=ReferencePipelineSmokeTest.Normalize(schemas,source);foreach(var h in r.ReferencedInstances)if(catalog.Count(a=>a.TypeId==h.TypeId && a.InstanceId==h.InstanceId)!=1)throw new InvalidDataException("Weapon/upgrade target missing or ambiguous.");return r;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: explicitly opt into the separate EP1 root model without changing legacy GameObject or weapon-set dispatch. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe Chunk Compile(XmlNode source)
    {
        var d=new XmlDocument {XmlResolver=null};d.LoadXml(source.OuterXml);var ns=new XmlNamespaceManager(d.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);
        Ep1WeaponSetUpdateModuleData* root;using var tracker=new Tracker((void**)&root,(uint)sizeof(Ep1WeaponSetUpdateModuleData),false);
        // Reborn: standalone concrete ID initialization normally belongs to polymorphic allocation, not the empty Update base marshaller.
        *(uint*)root=0x431722A9;Marshaler.Marshal(new Node(d.DocumentElement!.CreateNavigator()!,ns),root,tracker);var chunk=new Chunk();if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("EP1 weapon-set allocation failed.");return chunk;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently decode emitted hardpoint/weapon tokens, verify named import slot identities and reverse all pointer/auxiliary shifts. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Check(Chunk native,InstanceDeclaration instance,ManifestAsset[] catalog)
    {
        var output=Ep1WeaponSetTokenizer.Tokenize(native);Verify(output,native,instance,catalog);return output;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: fresh output validation must also reject metadata mutations without calling the writer to repair them. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(Chunk output,Chunk native,InstanceDeclaration instance,ManifestAsset[] catalog,bool requireAllReferences=true)
    {
        var b=output.InstanceBuffer;int prefix=b.Length-native.InstanceBuffer.Length,h=(int)Word(native.InstanceBuffer,12),weapons=44+h*28;var imports=new List<uint>();
        var own=GameObjectStockReview.Tokens(b,12);int list=own[FastHash.GetHashCode("WeaponSlotHardpoint")].Offset;
        if(Word(b,0)!=2 || Word(b,4)!=0x431722A9 || Word(b,8)!=12 || own.Count!=6 || Word(b,12)!=1 || Word(b,16)!=FastHash.GetHashCode("UpdateModuleData") || Word(b,20)!=96 || list!=204 || Word(b,list)!=0x6C827073 || Word(b,list+4)!=0x5D448FD8 || Word(b,list+8)!=prefix+12 || own[FastHash.GetHashCode("ActiveWhenDisabledBy")].Type!=0x196570C9 || own[FastHash.GetHashCode("ActiveWhenDisabledBy")].Offset!=prefix+8 || own[FastHash.GetHashCode("WeaponSlotHardpoint")].Type!=0xCE5853D7)throw new InvalidDataException("Weapon-set wrapper/count binding differs.");
        foreach(var link in new[]{(96,"ContainModuleData",120),(120,"BehaviorModuleData",144),(144,"ModuleData",168)})if(Word(b,link.Item1)!=1 || Word(b,link.Item1+4)!=FastHash.GetHashCode(link.Item2) || Word(b,link.Item1+8)!=link.Item3)throw new InvalidDataException("Weapon-set inherited metadata differs.");
        var terminal=GameObjectStockReview.Tokens(b,168);
        if(terminal[FastHash.GetHashCode("TypeId")].Type!=0x23E650B8 || terminal[FastHash.GetHashCode("TypeId")].Offset!=prefix || terminal[FastHash.GetHashCode("id")].Type!=0xBDF07965 || terminal[FastHash.GetHashCode("id")].Offset!=prefix+4)throw new InvalidDataException("Weapon-set terminal identity token differs.");
        for(int i=0;i<h;i++)
        {
            int hp=44+i*28,count=(int)Word(native.InstanceBuffer,hp+16),table=(int)Word(b,list+20+i*12);var fields=GameObjectStockReview.Tokens(b,table);
            if(Word(b,list+12+i*12)!=0xB151A0EB || Word(b,list+16+i*12)!=0 || fields.Count!=7)throw new InvalidDataException("Hardpoint marker/field count differs.");
            foreach(var f in new[]{("TypeId",0,0x23E650B8u),("ID",4,0x763177A4u),("AllowInterleavedFiring",24,0x77E3F4F7u),("InterleavedStyle",8,0xBE669829u),("WeaponChoiceCriteria",12,0x1D110792u),("AutoChooseWeapon",25,0x77E3F4F7u)})if(!fields.TryGetValue(FastHash.GetHashCode(f.Item1),out var token) || token.Type!=f.Item3 || token.Offset!=prefix+hp+f.Item2)throw new InvalidDataException("Weapon hardpoint field type/target differs.");
            int wl=fields[FastHash.GetHashCode("Weapon")].Offset;if(fields[FastHash.GetHashCode("Weapon")].Type!=0x77994E4B || Word(b,wl)!=0x6C827073 || Word(b,wl+4)!=0x5D448FD8 || Word(b,wl+8)!=prefix+hp+16)throw new InvalidDataException("Weapon row count target differs.");
            for(int j=0;j<count;j++)
            {
                int w=weapons+j*80,wt=(int)Word(b,wl+20+j*12);var row=GameObjectStockReview.Tokens(b,wt);
                if(Word(b,wl+12+j*12)!=0xB151A0EB || Word(b,wl+16+j*12)!=0 || row.Count!=7)throw new InvalidDataException("Weapon marker/field count differs.");
                foreach(var f in new[]{("Ordering",0,0x1848C549u),("Template",4,0xD046DD22u),("Upgrade",8,0x4ABA1F29u),("ObjectStatus",12,0xA7BD811Eu),("ForbiddenObjectStatus",44,0xA7BD811Eu),("IsPlayerUpgradePermanent",76,0x77E3F4F7u),("ConquerMetricsAlwaysCount",77,0x77E3F4F7u)})if(!row.TryGetValue(FastHash.GetHashCode(f.Item1),out var token) || token.Type!=f.Item3 || token.Offset!=prefix+w+f.Item2)throw new InvalidDataException("Weapon row field type/target differs.");
                foreach(int offset in new[]{4,8})
                {
                    uint index=Word(native.InstanceBuffer,w+offset);if(index==0)continue;if(index>instance.ReferencedInstances.Count)throw new InvalidDataException("Weapon row index bounds differ.");var r=instance.ReferencedInstances[(int)index-1];uint type=offset==4?0x94D4D96Eu:0xE1AFE75Bu;
                    if(r.TypeId!=type || catalog.Count(a=>a.TypeId==r.TypeId && a.InstanceId==r.InstanceId)!=1 || Word(b,prefix+w+offset)!=index)throw new InvalidDataException("Weapon row import identity differs.");imports.Add((uint)(w+offset));
                }
            }
            weapons+=count*80;
        }
        var recovered=b.AsSpan(prefix).ToArray();foreach(uint s in Slots(output.RelocationBuffer))Put(recovered,(int)s-prefix,Word(b,(int)s)-(uint)prefix);
        // Reborn: complete objects share one occurrence table across siblings; standalone controls retain full-table consumption by default.
        if(!recovered.SequenceEqual(native.InstanceBuffer) || !Slots(output.RelocationBuffer).Select(s=>s-(uint)prefix).SequenceEqual(Slots(native.RelocationBuffer)) || !Slots(output.ImportsBuffer).Select(s=>s-(uint)prefix).SequenceEqual(imports) || requireAllReferences&&instance.ReferencedInstances.Count!=imports.Count)throw new InvalidDataException("Weapon fragment native/auxiliary occurrence round trip differs.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: negative fixtures own their buffers. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c)=>new(){InstanceBuffer=c.InstanceBuffer.ToArray(),RelocationBuffer=c.RelocationBuffer.ToArray(),ImportsBuffer=c.ImportsBuffer.ToArray()};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: absent auxiliary streams do not create a phantom sentinel entry. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b)=>b.Length==0?Array.Empty<uint>():Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();
    //-------------------------------------------------------------------------------------------------
    /** Reborn: malformed source/native inputs fail explicitly, without swallowing unrelated exceptions. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action a){try{a();}catch(Exception e)when(e is InvalidDataException or BinaryAssetBuilderException or XmlSchemaValidationException){return;}throw new InvalidDataException("Malformed EP1 weapon set admitted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: diagnostic reads use bounded little-endian words. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));
    //-------------------------------------------------------------------------------------------------
    /** Reborn: detached fault and comparison writes never modify stock data. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint v)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),v);
}
