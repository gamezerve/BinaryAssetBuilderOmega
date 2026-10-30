using System.Buffers.Binary;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: general EP1 WeaponTemplate route: source-sized reviewed nugget classes, schema-derived metadata and independent native/metadata proofs against original weapons.
internal static class WeaponTemplateCompiler
{
    internal sealed record NuggetClass(string Element,string Schema,Type Native,uint TypeId);
    // Reborn: all 35 official EP1 choices have explicit concrete layouts; available retail records and schema-backed controls qualify them separately.
    internal static readonly NuggetClass[] Classes={
        new("DamageNugget","DamageNuggetType",typeof(DamageNuggetType),FastHash.GetHashCode("DamageNuggetType")),
        new("DOTNugget","DOTNuggetType",typeof(Ep1DOTNuggetType),FastHash.GetHashCode("DOTNuggetType")),
        new("DamageAndSpawnNugget","DamageAndSpawnNuggetType",typeof(Ep1DamageAndSpawnNuggetType),FastHash.GetHashCode("DamageAndSpawnNuggetType")),
        new("ProjectileNugget","ProjectileNuggetType",typeof(Ep1ProjectileNuggetType),FastHash.GetHashCode("ProjectileNuggetType")),
        new("SuppressionNugget","SuppressionNuggetType",typeof(Ep1SuppressionNuggetType),FastHash.GetHashCode("SuppressionNuggetType")),
        new("TerrainCraterNugget","TerrainCraterNuggetType",typeof(Ep1TerrainCraterNuggetType),FastHash.GetHashCode("TerrainCraterNuggetType")),
        new("WeaponOCLNugget","WeaponOCLNuggetType",typeof(Ep1WeaponOCLNuggetType),FastHash.GetHashCode("WeaponOCLNuggetType")),
        new("ActivateLaserNugget","ActivateLaserNuggetType",typeof(Ep1ActivateLaserNuggetType),FastHash.GetHashCode("ActivateLaserNuggetType")),
        new("ActivateStreamNugget","ActivateStreamNuggetType",typeof(Ep1ActivateStreamNuggetType),FastHash.GetHashCode("ActivateStreamNuggetType")),
        new("ActivateLiftObjectNugget","ActivateLiftObjectNuggetType",typeof(Ep1ActivateLiftObjectNuggetType),FastHash.GetHashCode("ActivateLiftObjectNuggetType")),
        new("ActivateLinearDamageNugget","ActivateLinearDamageNuggetType",typeof(Ep1ActivateLinearDamageNuggetType),FastHash.GetHashCode("ActivateLinearDamageNuggetType")),
        new("ActivateCircularDamageNugget","ActivateCircularDamageNuggetType",typeof(Ep1ActivateCircularDamageNuggetType),FastHash.GetHashCode("ActivateCircularDamageNuggetType")),
        new("SlavesAttackNugget","SlavesAttackNuggetType",typeof(Ep1SlavesAttackNuggetType),FastHash.GetHashCode("SlavesAttackNuggetType")),
        new("MetaImpactNugget","MetaImpactNuggetType",typeof(Ep1MetaImpactNuggetType),FastHash.GetHashCode("MetaImpactNuggetType")),
        new("LeechHealthDamageNugget","LeechHealthDamageNuggetType",typeof(Ep1LeechHealthDamageNuggetType),FastHash.GetHashCode("LeechHealthDamageNuggetType")),
        new("SecondaryDamageNugget","SecondaryDamageNuggetType",typeof(Ep1SecondaryDamageNuggetType),FastHash.GetHashCode("SecondaryDamageNuggetType")),
        new("SpecialPowerNugget","SpecialPowerNuggetType",typeof(Ep1SpecialPowerNuggetType),FastHash.GetHashCode("SpecialPowerNuggetType")),
        new("AttributeModifierNugget","AttributeModifierNuggetType",typeof(Ep1AttributeModifierNuggetType),FastHash.GetHashCode("AttributeModifierNuggetType")),
        new("DamageContainedNugget","DamageContainedNuggetType",typeof(Ep1DamageContainedNuggetType),FastHash.GetHashCode("DamageContainedNuggetType")),
        new("LuaEventNugget","LuaEventNuggetType",typeof(Ep1LuaEventNuggetType),FastHash.GetHashCode("LuaEventNuggetType")),
        new("LineDamageNugget","LineDamageNuggetType",typeof(Ep1LineDamageNuggetType),FastHash.GetHashCode("LineDamageNuggetType")),
        new("SeedTiberiumNugget","SeedTiberiumNuggetType",typeof(Ep1SeedTiberiumNuggetType),FastHash.GetHashCode("SeedTiberiumNuggetType")),
        new("ParalyzeNugget","ParalyzeNuggetType",typeof(Ep1ParalyzeNuggetType),FastHash.GetHashCode("ParalyzeNuggetType")),
        new("InformationWarfareNugget","InformationWarfareNuggetType",typeof(Ep1InformationWarfareNuggetType),FastHash.GetHashCode("InformationWarfareNuggetType")),
        new("SpendStolenTiberiumNugget","SpendStolenTiberiumNuggetType",typeof(Ep1SpendStolenTiberiumNuggetType),FastHash.GetHashCode("SpendStolenTiberiumNuggetType")),
        new("ReportWeaponFiredStatNugget","ReportWeaponFiredStatNuggetType",typeof(Ep1ReportWeaponFiredStatNuggetType),FastHash.GetHashCode("ReportWeaponFiredStatNuggetType")),
        new("ScatterProjectileNugget","ScatterProjectileNuggetType",typeof(Ep1ScatterProjectileNuggetType),FastHash.GetHashCode("ScatterProjectileNuggetType")),
        new("FireOnObjectsNugget","FireOnObjectsNuggetType",typeof(Ep1FireOnObjectsNuggetType),FastHash.GetHashCode("FireOnObjectsNuggetType")),
        new("TintObjectsNugget","TintObjectsNuggetType",typeof(Ep1TintObjectsNuggetType),FastHash.GetHashCode("TintObjectsNuggetType")),
        new("ContainedObjectAttackNugget","ContainedObjectAttackNuggetType",typeof(Ep1ContainedObjectAttackNuggetType),FastHash.GetHashCode("ContainedObjectAttackNuggetType")),
        new("InformTargetNugget","InformTargetNuggetType",typeof(Ep1InformTargetNuggetType),FastHash.GetHashCode("InformTargetNuggetType")),
        new("AttachNugget","AttachNugget",typeof(Ep1AttachNugget),FastHash.GetHashCode("AttachNugget")),
        new("StripMaxHealthPercentNugget","StripMaxHealthPercentNuggetType",typeof(Ep1StripMaxHealthPercentNuggetType),FastHash.GetHashCode("StripMaxHealthPercentNuggetType")),
        new("GrapplingHookNugget","GrapplingHookNuggetType",typeof(Ep1GrapplingHookNuggetType),FastHash.GetHashCode("GrapplingHookNuggetType")),
        new("LeechPercentMaxHealthDamageNugget","LeechPercentMaxHealthDamageNuggetType",typeof(Ep1LeechPercentMaxHealthDamageNuggetType),FastHash.GetHashCode("LeechPercentMaxHealthDamageNuggetType"))};
    internal static readonly uint TypeId=FastHash.GetHashCode("WeaponTemplate");
    // Reborn: EP1 overrides are the authoritative source for an identically named weapon in the original static stream.
    internal static XmlElement[] Originals()=>GameObjectExtendedRootContract.Original("GlobalData/Weapon.xml","WeaponTemplate").Concat(GameObjectExtendedRootContract.Original("EP1/GlobalData/Weapon_EP1.xml","WeaponTemplate")).GroupBy(e=>e.GetAttribute("id"),StringComparer.Ordinal).Select(g=>g.Last()).ToArray();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: an authored nugget element selects its reviewed class; unreviewed classes fail at their own source node. */
    //-------------------------------------------------------------------------------------------------
    internal static NuggetClass Nugget(XmlElement element)=>Classes.SingleOrDefault(c=>c.Element==element.LocalName)??throw SdkXmlDiagnostics.At(new InvalidDataException("Weapon nugget is not supported yet: "+element.LocalName+". Supported: "+string.Join(", ",Classes.Select(c=>c.Element))+"."),element);

    // Reborn: native class words and stock wrapper tags both name the official schema class.
    internal static NuggetClass Class(uint id)=>Classes.SingleOrDefault(c=>c.TypeId==id)??throw new InvalidDataException($"Weapon nugget class {id:X8} is not reviewed.");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject unreviewed nugget classes before normalization so the error points to the authored element. */
    //-------------------------------------------------------------------------------------------------
    internal static void Admit(XmlDocument source)
    {
        foreach(XmlAttribute attribute in source.SelectNodes("//@TypeId")!)throw SdkXmlDiagnostics.At(new InvalidDataException("TypeId is compiler-generated; omit the authored discriminator."),attribute);
        foreach(var list in source.DocumentElement!.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="Nuggets"))foreach(var nugget in list.ChildNodes.OfType<XmlElement>())_=Nugget(nugget);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Core-normalized source receives explicit class words, then one native Tracker allocation. */
    //-------------------------------------------------------------------------------------------------
    internal static Chunk Compile(XmlNode normalized,System.Xml.Schema.XmlSchemaSet official)
    {
        // Reborn: materialize official EP1 schema defaults first; the legacy native writer's own fallbacks are KW values (e.g. RadiusDamageAffects).
        var typed=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(normalized.OuterXml));typed.Schemas.Add(official);typed.Validate((_,e)=>throw SdkXmlDiagnostics.SchemaError(e));
        Defaults(typed.DocumentElement!);var xml=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(typed.OuterXml));
        foreach(var list in xml.DocumentElement!.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="Nuggets"))foreach(var nugget in list.ChildNodes.OfType<XmlElement>())nugget.SetAttribute("TypeId",Nugget(nugget).TypeId.ToString(System.Globalization.CultureInfo.InvariantCulture));
        return WeaponNativeProbe.Compile(xml);
    }

    // Reborn: write each validated element's missing official attribute defaults explicitly (PSVI defaults are not serialized).
    private static void Defaults(XmlElement element)
    {
        if(element.SchemaInfo.SchemaType is System.Xml.Schema.XmlSchemaComplexType complex)
            foreach(System.Xml.Schema.XmlSchemaAttribute attribute in complex.AttributeUses.Values)
                if(attribute.DefaultValue!=null&&attribute.QualifiedName.Namespace.Length==0&&element.GetAttributeNode(attribute.QualifiedName.Name) is var existing&&(existing==null||!existing.Specified)){element.RemoveAttribute(attribute.QualifiedName.Name);element.SetAttribute(attribute.QualifiedName.Name,attribute.DefaultValue);}
        foreach(var child in element.ChildNodes.OfType<XmlElement>())Defaults(child);
    }

    // Reborn: schema-derived weapon metadata over the actual native graph.
    internal static Chunk Tokenize(Chunk native,System.Xml.Schema.XmlSchemaSet official)=>Ep1BehaviorMetadataTokenizer.TokenizeWeapon(native,official,id=>{var c=Class(id);return(c.Schema,c.Native);});

    //-------------------------------------------------------------------------------------------------
    /** Reborn: learn the weapon metadata grammar from original weapons whose nuggets are all reviewed; developer builds also prove each native record from its original source. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,SchemaSet schemas,ManifestAsset[] catalog)
    {
        var doc=ManifestReader.Read(SdkEnvironmentPreflight.Read(manifest,64*1048576));if(!doc.Header.IsLinked||doc.Validate().Count!=0)throw new InvalidDataException("Valid linked weapon stock stream required.");
        string bin=Path.ChangeExtension(manifest,".bin");var header=AssetStreamProbe.ReadRange(bin,null,0,8);if(BinaryPrimitives.ReadUInt32LittleEndian(header)!=0xBABB0000||BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(4))!=doc.Header.StreamChecksum)throw new InvalidDataException("Weapon stock stream checksum differs.");
        var offsets=new Dictionary<string,(ManifestAsset Asset,long Offset)>(StringComparer.Ordinal);long offset=8;foreach(var a in doc.Assets){offsets[a.Name]=(a,offset);offset+=a.InstanceDataSize;}
        var grammar=new StockMetadataGrammar();int count=0,proved=0;var failures=new List<string>();
        // Reborn: some original campaign weapons target WorldBuilder identities; qualification must use the complete configured original catalog.
        catalog=catalog.Concat(ManifestReader.Read(SdkEnvironmentPreflight.Read(WorldBuilderAssetResolver.CatalogPath(),64*1048576)).Assets).DistinctBy(a=>new AssetId(a.TypeId,a.InstanceId)).ToArray();
        foreach(var original in Originals())
        {
            // Reborn: witnesses are complete standalone originals (no inheritance or define expressions) whose nugget classes are all reviewed.
            var nuggets=original.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="Nuggets").SelectMany(e=>e.ChildNodes.OfType<XmlElement>()).ToArray();
            if(nuggets.Length==0||original.HasAttribute("inheritFrom")||nuggets.Any(n=>Classes.All(c=>c.Element!=n.LocalName))||original.SelectNodes(".//@*")!.Cast<XmlAttribute>().Any(a=>a.Value.StartsWith('=')))continue;
            if(!offsets.TryGetValue("WeaponTemplate:"+original.GetAttribute("id"),out var hit))continue;
            var (asset,at)=hit;if(asset.TypeId!=TypeId||asset.Tokenized!=1||asset.InstanceDataSize<400||asset.InstanceDataSize>1048576)throw new InvalidDataException("Weapon stock fingerprint differs: "+asset.Name);
            try{
            var bytes=AssetStreamProbe.ReadRange(bin,null,at,asset.InstanceDataSize);int native=NativeStart(bytes);
            grammar.Learn(bytes,0,native,typeof(WeaponTemplate));count++;
            if(SdkBuildTests.Enabled){
                var source=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(original.OuterXml));var proof=SchemaNativeContract.Resolve(source,schemas,catalog);proof.DocumentElement!.RemoveAttribute("id");
                try{SchemaNativeContract.VerifyStock(proof.DocumentElement,typeof(WeaponTemplate),0,bytes,native,asset.References.ToArray());}catch(InvalidDataException error){throw new InvalidDataException("Weapon stock native qualification failed for "+asset.Name+": "+error.Message,error);}proved++;
            }
            }catch(InvalidDataException error){failures.Add(asset.Name+": "+error.Message);}
        }
        // Reborn: report the complete corpus's layout issues together, while still failing qualification before any authored stream publication.
        if(failures.Count!=0){foreach(string failure in failures)Console.WriteLine("Weapon qualification error: "+failure);throw new InvalidDataException("Weapon corpus qualification failed for "+failures.Count+" records; see individual diagnostics above.");}
        if(count<100)throw new InvalidDataException("Too few original weapon witnesses qualified: "+count);
        // Reborn: the eight schema-declared families absent from retail still have explicit field/tag/class contracts, not a stock-usage gate.
        grammar.QualifyWeaponSchemaTypes(schemas.Schemas,Classes);
        SdkBuildTests.MetadataProgress("WeaponTemplate",$"Reborn: WeaponTemplate EP1 native/metadata qualification: PASS ({count} original weapons learned, {proved} native records proved; classes {string.Join(", ",Classes.Select(c=>c.Element))}).",count+" stock weapons");
        return grammar;
    }

    // Reborn: the stock root own table targets AttackRange directly after the 4-byte asset base word.
    internal static int NativeStart(byte[] tokens)
    {
        var rows=GameObjectStockReview.Tokens(tokens,0);if(!rows.TryGetValue(FastHash.GetHashCode("AttackRange"),out var attack)||attack.Offset<16||attack.Offset>tokens.Length-340)throw new InvalidDataException("Weapon stock root table differs.");
        return attack.Offset-4;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile, prove and tokenize one authored weapon; the stream receives metadata + native bytes. */
    //-------------------------------------------------------------------------------------------------
    internal static Chunk Build(XmlDocument resolvedProof,InstanceDeclaration instance,System.Xml.Schema.XmlSchemaSet official,StockMetadataGrammar grammar)
    {
        var native=Compile(instance.XmlNode,official);var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
        var proof=SchemaNativeContract.Verify(resolvedProof.DocumentElement!,typeof(WeaponTemplate),0,native,references);
        var tokens=Tokenize(native,official);_=grammar.Verify(tokens,native,typeof(WeaponTemplate),proof.Records);return tokens;
    }
}
