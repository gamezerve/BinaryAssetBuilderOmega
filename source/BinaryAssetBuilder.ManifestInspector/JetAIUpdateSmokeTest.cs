using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: cover jet-AI stock imports, authored jet fields, defaults and full object composition without editing user sources.
internal static class JetAIUpdateSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: run complete source/native/relocation/metadata checks and a corrupt-output refusal through the existing production EP1 dispatcher. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global)
    {
        var saved=Settings.Current;
        try{
            string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;var official=ConscriptSourceInput.Schemas();
            Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=new[]{manifest,global}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).ToArray();var baseline=SdkXmlDiagnostics.Parse(File.ReadAllBytes(Path.Combine(fixtures,"live-alpha/ConscriptBuildTime1/data/mod.xml")));
            baseline.DocumentElement!.SetAttribute("BuildTime","4");
            var grammar=JetAIUpdateContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,5)){
                var source=(XmlDocument)baseline.CloneNode(true);var ai=source.GetElementsByTagName("AI",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();ai.RemoveAll();
                var witness=JetAIUpdateContract.Witnesses[Math.Min(variant,1)];var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original(witness.Path,"JetAIUpdate").Single(),true);ai.AppendChild(owner);
                // Reborn: edit all jet fields and both strong imports, while testing shared chooser presence/defaults independently.
                if(variant==2){owner.RemoveAll();owner.SetAttribute("id","RebornJetDefaults");}
                if(variant>=3){owner.SetAttribute("id","RebornJetEdit");foreach(var pair in new[]{("OutOfAmmoDamagePerSecond","3%"),("OutOfAmmoDamageDelay","2s"),("ParkingOffset","12.5"),("NeedsRunway","true"),("ReturnToBaseWhenVictimDead","true"),("KeepsParkingSpaceWhenAirborne","true"),("CirclesForAttack","true"),("StateMachine","JetAIStateMachine"),("AttackPriority","RebornAirPriority"),("StopChaseDistance","333")})owner.SetAttribute(pair.Item1,pair.Item2);owner.SetAttribute("UseForOutOfAmmoCheck",catalog.First(a=>a.Name.StartsWith("WeaponTemplate:")).Name.Split(':')[1]);owner.SetAttribute("OutOfAmmoEvaEvent",catalog.First(a=>a.Name.StartsWith("EvaEvent:")).Name.Split(':')[1]);owner.InnerXml=variant==4?"<UnitAITargetChooserData xmlns='uri:ea.com:eala:asset' IdleScanDelay='0.75s' CanPickDynamicTargets='false' CheckVisionRange='true'/>":"";}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);
                // Reborn: inspect the same isolated core root as production, keeping separately composed root records outside the inline AI owner.
                int cut=Ep1GameObjectRootOwnersProjection.NativeTailStart(full.InstanceBuffer);var bytes=full.InstanceBuffer.AsSpan(0,cut).ToArray();foreach(var field in Ep1GameObjectRootOwnersProjection.Fields)Ep1GameObjectRootOwnersProjection.ClearSlot(bytes,field);
                var layout=Ep1GameObjectStringTokens.Inspect(bytes,Slots(full.RelocationBuffer).Where(s=>s<cut&&!Ep1GameObjectRootOwnersProjection.Fields.Any(f=>f.PointerSlot==s)).ToArray(),Slots(full.ImportsBuffer).Where(s=>s<cut).ToArray(),true);var native=layout.Ai.Modules.Single().Native;var metadata=layout.Ai.Modules.Single().Tokens;var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container=="AI").Element;var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=JetAIUpdateContract.Verify(element,native,references);_=grammar.Verify(metadata,native,typeof(JetAIUpdateContract.Native),proof.Records);checks++;
                _=verifier.Verify(source,full,instance,plan.Origins);checks++;
                // Reborn: changing the emitted parking offset must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<JetAIUpdateContract.Native>(nameof(JetAIUpdateContract.Native.ParkingOffset)).ToInt32()]^=1;
                try{JetAIUpdateContract.Verify(element,corrupt,references);throw new InvalidOperationException("Corrupt jet-AI payload accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: JetAIUpdate: PASS ({checks} two-retail/all-jet-fields/strong-imports/chooser/default/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: enumerate only the sentinel-framed auxiliary words emitted by the real Tracker. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] bytes)=>bytes.Length==0?Array.Empty<uint>():Enumerable.Range(0,bytes.Length/4-1).Select(i=>System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(i*4,4))).ToArray();

}
