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
    // Reborn: each class is admitted only after its EP1 layout was qualified against the retail weapon corpus (Damage 231, Projectile 148, Suppression 70, MetaImpact 49 records).
    internal static readonly NuggetClass[] Classes={
        new("DamageNugget","DamageNuggetType",typeof(DamageNuggetType),0xC0F53E99u),
        new("ProjectileNugget","ProjectileNuggetType",typeof(Ep1ProjectileNuggetType),0x87C7BDCCu),
        new("SuppressionNugget","SuppressionNuggetType",typeof(Ep1SuppressionNuggetType),0xF4A6ADA7u),
        new("MetaImpactNugget","MetaImpactNuggetType",typeof(Ep1MetaImpactNuggetType),0xAF4289FBu)};
    internal static readonly uint TypeId=FastHash.GetHashCode("WeaponTemplate");

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
        var grammar=new StockMetadataGrammar();int count=0,proved=0;
        foreach(var original in GameObjectExtendedRootContract.Original("GlobalData/Weapon.xml","WeaponTemplate"))
        {
            // Reborn: witnesses are complete standalone originals (no inheritance or define expressions) whose nugget classes are all reviewed.
            var nuggets=original.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="Nuggets").SelectMany(e=>e.ChildNodes.OfType<XmlElement>()).ToArray();
            if(nuggets.Length==0||original.HasAttribute("inheritFrom")||nuggets.Any(n=>Classes.All(c=>c.Element!=n.LocalName))||original.SelectNodes(".//@*")!.Cast<XmlAttribute>().Any(a=>a.Value.StartsWith('=')))continue;
            if(!offsets.TryGetValue("WeaponTemplate:"+original.GetAttribute("id"),out var hit))continue;
            var (asset,at)=hit;if(asset.TypeId!=TypeId||asset.Tokenized!=1||asset.InstanceDataSize<400||asset.InstanceDataSize>1048576)throw new InvalidDataException("Weapon stock fingerprint differs: "+asset.Name);
            var bytes=AssetStreamProbe.ReadRange(bin,null,at,asset.InstanceDataSize);int native=NativeStart(bytes);
            grammar.Learn(bytes,0,native,typeof(WeaponTemplate));count++;
            if(SdkBuildTests.Enabled){
                var source=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(original.OuterXml));var proof=SchemaNativeContract.Resolve(source,schemas,catalog);proof.DocumentElement!.RemoveAttribute("id");
                try{SchemaNativeContract.VerifyStock(proof.DocumentElement,typeof(WeaponTemplate),0,bytes,native,asset.References.ToArray());}catch(InvalidDataException error){throw new InvalidDataException("Weapon stock native qualification failed for "+asset.Name+": "+error.Message,error);}proved++;
            }
        }
        if(count<100)throw new InvalidDataException("Too few original weapon witnesses qualified: "+count);
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
