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

// Reborn: cover falling projectiles and ship slow-death with editable/default values and native metadata composition without editing user sources.
internal static class DyingProjectileSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: run complete original/default/edited source/native/relocation/metadata checks through the production EP1 dispatcher. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global)
    {
        var saved=Settings.Current;string? savedDeveloper=System.Environment.GetEnvironmentVariable(SdkBuildTests.EnvironmentKey);
        try{
            string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;var official=ConscriptSourceInput.Schemas();
            Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=new[]{manifest,global}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).ToArray();var baseline=SdkXmlDiagnostics.Parse(File.ReadAllBytes(Path.Combine(fixtures,"live-alpha/ConscriptBuildTime1/data/mod.xml")));
            baseline.DocumentElement!.SetAttribute("BuildTime","4");
            var grammar=DyingProjectileContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;
            for(int variant=0;variant<3;variant++){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();
                foreach(var family in DyingProjectileContract.Families){var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original("Soviet/Props/SovietBomberAircraft_Dying.xml",family.Source).Single(),true);ConscriptVariableBehaviorSource.DropLeafFormatting(owner);behaviors.AppendChild(owner);
                    // Reborn: authored/default records are independent of the captured original IDs and values.
                    if(variant>0){owner.RemoveAll();owner.SetAttribute("id","Reborn"+family.Source+variant);}
                    // Reborn: the projectile always owns its schema-required movement; omitted movement scalars retain official defaults.
                    if(variant>0&&family.Source=="ProjectileUpdate")owner.InnerXml="<Movement xmlns='uri:ea.com:eala:asset'><MoveToGround/></Movement>";
                    if(variant==2&&family.Source=="ProjectileUpdate"){
                        owner.SetAttribute("StateMachine","BombProjectile");owner.SetAttribute("MinDetonationHeight","25");owner.SetAttribute("MinTargetDistance","8");owner.SetAttribute("Flags","LAUNCH_AT_TARGET_POS TRACK_TARGET");
                        owner.InnerXml="<Movement xmlns='uri:ea.com:eala:asset'><MoveToGround InitialSpeed='3' Acceleration='0.2' TerminalVelocity='7' RotationSpeedPerSecond='30d' TerminalRotation='75d' ForwardVelocityDamping='20%'/></Movement>";
                    }
                    if(variant==2&&family.Source=="ShipSlowDeath"){
                        owner.SetAttribute("DestructionDelay","15s");owner.SetAttribute("Fade","true");owner.SetAttribute("FadeTime","2s");owner.InnerXml="<FX xmlns='uri:ea.com:eala:asset' Type='FINAL'><FX>FX_NONE</FX></FX><DieMuxData xmlns='uri:ea.com:eala:asset' DeathTypes='ALL'/>";
                    }

                }

                _=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml));var refs=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                foreach(var family in DyingProjectileContract.Families){uint typeId=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(family.Wire);var native=ConscriptBehaviorSurvey.Compile(normalized.GetElementsByTagName(family.Source,SchemaSet.XmlNamespace).OfType<XmlElement>().Single(),typeId);var element=resolved.GetElementsByTagName(family.Source,SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var proof=SchemaNativeContract.Verify(element,family.Type,typeId,native,refs);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,family.Wire,family.Type);_=grammar.Verify(metadata,native,family.Type,proof.Records);checks++;
                    // Reborn: changing a proved flag/threshold word must fail without relying on the producer writer.
                    var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[8]^=1;
                    try{SchemaNativeContract.Verify(element,family.Type,typeId,corrupt,refs);throw new InvalidOperationException("Corrupt dying-projectile update accepted.");}catch(InvalidDataException){checks++;}
                }

            }
            // Reborn: test optional geometry on a complete object; source/native ownership checks remain mandatory in the fast path.
            System.Environment.SetEnvironmentVariable(SdkBuildTests.EnvironmentKey,"0");
            var absent=(XmlDocument)baseline.CloneNode(true);var geometry=GameObjectGeometryContract.Owner(absent)!;geometry.ParentNode!.RemoveChild(geometry);
            var plan=ConscriptVariableBehaviorSource.Admit(absent,baseline,official);var resolvedAbsent=SchemaNativeContract.Resolve(absent,schemas,catalog);var instanceAbsent=ReferencePipelineSmokeTest.Normalize(schemas,resolvedAbsent.OuterXml);var normalizedAbsent=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instanceAbsent.XmlNode.OuterXml));var full=GameObjectBodyProjectionSmokeTest.Compile(normalizedAbsent,true,true,true);
            var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);_=verifier.Verify(absent,full,instanceAbsent,plan.Origins);checks++;
            var dangling=new Chunk{InstanceBuffer=full.InstanceBuffer.ToArray(),RelocationBuffer=full.RelocationBuffer,ImportsBuffer=full.ImportsBuffer};System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(dangling.InstanceBuffer.AsSpan(500),600u);
            try{_=verifier.Verify(absent,dangling,instanceAbsent,plan.Origins);throw new InvalidOperationException("Dangling omitted Geometry accepted.");}catch(InvalidDataException){checks++;}
            Console.WriteLine($"Reborn: dying projectiles: PASS ({checks} original/default/edit native/metadata and leaf checks; falling movement, timing, angles, damping, option bits and inherited ship-death phases omitted Geometry and corrupt-native rejection; no game launch).");
        }finally{Settings.Current=saved;System.Environment.SetEnvironmentVariable(SdkBuildTests.EnvironmentKey,savedDeveloper);}
    }
}
