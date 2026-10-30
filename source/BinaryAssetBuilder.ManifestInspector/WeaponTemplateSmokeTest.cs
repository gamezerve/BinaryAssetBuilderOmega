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
            foreach(var original in WeaponTemplateCompiler.Originals()){
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
            // Reborn: all 35 schema choices compile even when a family has no original retail use; missing required fields remain real schema errors.
            var choice=(XmlSchemaComplexType)official.GlobalTypes[new XmlQualifiedName("WeaponTemplateNuggets",SchemaSet.XmlNamespace)]!;var elements=(choice.Particle as XmlSchemaChoice??throw new InvalidDataException("Weapon nugget schema choice missing.")).Items.OfType<XmlSchemaElement>().ToArray();
            if(elements.Length!=35||!elements.Select(e=>e.Name).Order().SequenceEqual(WeaponTemplateCompiler.Classes.Select(c=>c.Element).Order()))throw new InvalidDataException("Official weapon choice coverage differs.");
            var failures=new List<string>();int allChoices=0;
            foreach(var element in elements){string required=element.Name switch{"SecondaryDamageNugget"=>" Amount='5'","AttachNugget"=>" AttachModuleId='ModuleTag_Attach'","DOTNugget"=>" DamageInterval='1s' DamageDuration='3s'",_=>""};string child=element.Name switch{"DamageContainedNugget"=>"<DamageObjectFilter Rule='ALL'/>","DamageAndSpawnNugget"=>"<SpawnTemplate>SovietAntiInfantryInfantry</SpawnTemplate>",_=>""};
                string xml="<WeaponTemplate xmlns='uri:ea.com:eala:asset' id='RebornAll"+element.Name+"'><Nuggets><"+element.Name+required+">"+child+"</"+element.Name+"></Nuggets></WeaponTemplate>";
                try{_=Build(xml,schemas,official,catalog,grammar);allChoices++;}catch(InvalidDataException error){failures.Add(element.Name+": "+error.Message);}
            }
            if(failures.Count!=0){foreach(string error in failures)Console.WriteLine("Weapon authored choice error: "+error);throw new InvalidDataException("Weapon authored choice controls failed: "+failures.Count);}
            // Reborn: explicit model membership covers the entire root schema rather than a list of approved authored fields.
            var root=(XmlSchemaComplexContentExtension)((XmlSchemaComplexContent)((XmlSchemaComplexType)official.GlobalTypes[new XmlQualifiedName("WeaponTemplate",SchemaSet.XmlNamespace)]!).ContentModel!).Content!;
            foreach(var name in root.Attributes.OfType<XmlSchemaAttribute>().Select(a=>a.Name).Concat(((XmlSchemaGroupBase)root.Particle!).Items.OfType<XmlSchemaElement>().Select(e=>e.Name)))if(typeof(WeaponTemplate).GetField(name!)==null)throw new InvalidDataException("Weapon root field missing: "+name);
            // Reborn: no-retail-witness families also exercise nondefault fields and real weak/strong targets, not merely zero-filled records.
            string ocl=catalog.First(a=>a.TypeName=="ObjectCreationList").Name.Split(':',2)[1];
            string[] noRetail={"<DamageAndSpawnNugget SpawnProbability='0.35' SpawnedModelConditionFlags='DAMAGED'><SpawnTemplate>SovietAntiInfantryInfantry</SpawnTemplate></DamageAndSpawnNugget>","<TerrainCraterNugget Depth='12' Lift='25%'/>","<LuaEventNugget EventName='RebornWeaponEvent' SendToEnemies='true' SendToAllies='true' SendToNeutral='true' Radius='37'/>","<SeedTiberiumNugget FieldAmount='1000' SpawnedInFieldBonus='2' TargetAsPrimaryObject='true' WeaponOCL='"+ocl+"'/>","<InformationWarfareNugget RadarJamRadius='45' RadarJamDuration='3s'/>","<SpendStolenTiberiumNugget AmountToSpend='3'/>","<ScatterProjectileNugget ScatterMin='5' ScatterMax='30'/>","<ContainedObjectAttackNugget Radius='40'><SpecialObjectFilter Rule='ALL' Include='INFANTRY'/></ContainedObjectAttackNugget>"};
            foreach(string nugget in noRetail)_=Build("<WeaponTemplate xmlns='uri:ea.com:eala:asset' id='RebornNoRetail'><Nuggets>"+nugget+"</Nuggets></WeaponTemplate>",schemas,official,catalog,grammar);
            // Reborn: genuine unknown elements still fail at their authored node instead of being silently skipped.
            try{WeaponTemplateCompiler.Admit(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes("<WeaponTemplate xmlns='uri:ea.com:eala:asset' id='Bad'><Nuggets><UnknownNugget/></Nuggets></WeaponTemplate>")));throw new InvalidOperationException("Unknown nugget accepted.");}catch(InvalidDataException){}
            Console.WriteLine($"Reborn: WeaponTemplate complete schema route: PASS ({built} originals, {identical} identical, {authored.Length} authored combinations, {allChoices} official nugget choices, {noRetail.Length} nondefault no-retail shapes, complete root field inventory).");
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
