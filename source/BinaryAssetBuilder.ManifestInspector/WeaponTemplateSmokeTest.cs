using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: exercise the general weapon route on original weapons and authored nugget combinations without editing user sources.
internal static class WeaponTemplateSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify, rebuild original weapons from their SDK source, and build authored Projectile/Suppression/MetaImpact/Damage combinations. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global)
    {
        var saved=Settings.Current;
        try{
            string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;var official=ConscriptSourceInput.Schemas();
            Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=new[]{manifest,global}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).ToArray();
            var grammar=WeaponTemplateCompiler.Qualify(manifest,schemas,catalog);int built=0,identical=0;
            // Reborn: rebuild every qualified original weapon through the production path and compare with its retail bytes.
            var doc=ManifestReader.Read(SdkEnvironmentPreflight.Read(manifest,64*1048576));var offsets=new Dictionary<string,(ManifestAsset,long)>();long offset=8;foreach(var a in doc.Assets){offsets[a.Name]=(a,offset);offset+=a.InstanceDataSize;}
            var firstDiff=new List<string>();
            foreach(var original in GameObjectExtendedRootContract.Original("GlobalData/Weapon.xml","WeaponTemplate")){
                var nuggets=original.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="Nuggets").SelectMany(e=>e.ChildNodes.OfType<XmlElement>()).ToArray();
                if(nuggets.Length==0||original.HasAttribute("inheritFrom")||nuggets.Any(n=>WeaponTemplateCompiler.Classes.All(c=>c.Element!=n.LocalName))||original.SelectNodes(".//@*")!.Cast<XmlAttribute>().Any(a=>a.Value.StartsWith('=')))continue;
                Chunk tokens;try{tokens=Build(original.OuterXml,schemas,official,catalog,grammar);}catch(InvalidDataException error){throw new InvalidDataException("Original weapon rebuild failed for "+original.GetAttribute("id")+": "+error.Message,error);}built++;
                var (asset,at)=offsets["WeaponTemplate:"+original.GetAttribute("id")];var stock=AssetStreamProbe.ReadRange(Path.ChangeExtension(manifest,".bin"),null,at,asset.InstanceDataSize);
                if(System.Environment.GetEnvironmentVariable("REBORN_WEAPON_DUMP") is string dump&&dump.Length!=0)File.WriteAllBytes(Path.Combine(dump,original.GetAttribute("id")+".bin"),tokens.InstanceBuffer);
                if(stock.AsSpan().SequenceEqual(tokens.InstanceBuffer))identical++;else if(firstDiff.Count<5)firstDiff.Add(asset.Name+" stock="+stock.Length+" built="+tokens.InstanceBuffer.Length+" firstDiff@"+Enumerable.Range(0,Math.Min(stock.Length,tokens.InstanceBuffer.Length)).FirstOrDefault(i=>stock[i]!=tokens.InstanceBuffer[i],-1));
            }
            Console.WriteLine($"Reborn: original weapons rebuilt: {built}; byte-identical to retail: {identical}.");foreach(var d in firstDiff)Console.WriteLine("  differs: "+d);
            // Reborn: authored combinations from the Reborn weapon file shape, with stock reference targets.
            string[] authored={
                "<WeaponTemplate xmlns='uri:ea.com:eala:asset' id='RebornWeaponProjectileSuppression' AttackRange='600.0' MinimumAttackRange='300.0' WeaponSpeed='550' ScatterRadius='50' FireFX='FX_SovietRocketLaunch' RadiusDamageAffects='ENEMIES NEUTRALS' CanFireWhileMoving='false' ClipSize='1' ReAcquireDetailType='PER_CLIP' Flags='RELOAD_WHEN_ATTACK_STOPS' ShotsPerBarrel='1' AcceptableAimDelta='180d' RequiredAntiMask='ANTI_GROUND ANTI_STRUCTURE ANTI_WATER' ProjectileCollidesWith='ALLIES ENEMIES NEUTRAL STRUCTURES WALLS'><FiringDuration MinSeconds='1.0s' MaxSeconds='1.0s'/><ClipReloadTime MinSeconds='2.5s' MaxSeconds='2.5s'/><Nuggets><ProjectileNugget ProjectileTemplate='SovietAntiStructureVehicleV4RocketProjectile' WarheadTemplate='SovietAntiStructureVehicleV4RocketWarhead'/><SuppressionNugget Radius='100.0' Suppression='100' DurationSeconds='5s'/></Nuggets></WeaponTemplate>",
                "<WeaponTemplate xmlns='uri:ea.com:eala:asset' id='RebornWeaponDamageMetaImpact' ProjectileCollidesWith='ALLIES ENEMIES NEUTRAL STRUCTURES WALLS' RadiusDamageAffects='ALLIES ENEMIES NEUTRALS' RequiredAntiMask='ANTI_STRUCTURE ANTI_WATER ANTI_GROUND' Flags=''><Nuggets><DamageNugget Damage='750.0' DamageTaperOff='25' Radius='100.0' DamageType='EXPLOSIVE' DeathType='NORMAL'/><MetaImpactNugget ShockWaveAmount='100.0' ShockWaveRadius='100.0' ShockWaveZMult='1.50'/></Nuggets></WeaponTemplate>",
                "<WeaponTemplate xmlns='uri:ea.com:eala:asset' id='RebornWeaponDamageOnly' ClipSize='6'><PreAttackDelay MinSeconds='3.0s' MaxSeconds='3.0s'/><Nuggets><DamageNugget Damage='300.0' DamageType='TESLA' DamageFXType='SOVIET_TESLA' DeathType='ELECTROCUTED' DelayTimeSeconds='0.2s'/></Nuggets></WeaponTemplate>"};
            foreach(string xml in authored){var tokens=Build(xml,schemas,official,catalog,grammar);Console.WriteLine("  authored "+SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(xml)).DocumentElement!.GetAttribute("id")+": "+tokens.InstanceBuffer.Length+" bytes");}
            // Reborn: unreviewed classes fail at their source node.
            try{WeaponTemplateCompiler.Admit(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes("<WeaponTemplate xmlns='uri:ea.com:eala:asset' id='Bad'><Nuggets><InformTargetNugget/></Nuggets></WeaponTemplate>")));throw new InvalidOperationException("Unreviewed nugget accepted.");}catch(InvalidDataException){}
            Console.WriteLine($"Reborn: WeaponTemplate general route: PASS ({built} originals, {identical} identical, {authored.Length} authored).");
        }finally{Settings.Current=saved;}
    }

    private static Chunk Build(string xml,SchemaSet schemas,XmlSchemaSet official,ManifestAsset[] catalog,StockMetadataGrammar grammar)
    {
        var source=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(xml));WeaponTemplateCompiler.Admit(source);
        var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);
        var proof=SchemaNativeContract.Resolve(source,schemas,catalog);proof.DocumentElement!.RemoveAttribute("id");
        return WeaponTemplateCompiler.Build(proof,instance,official,grammar);
    }
}
