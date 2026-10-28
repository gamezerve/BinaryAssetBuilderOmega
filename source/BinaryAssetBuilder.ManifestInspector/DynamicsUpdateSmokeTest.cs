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

// Reborn: cover original dynamics records, schema defaults, optional volumes and transforms and full object composition without editing user sources.
internal static class DynamicsUpdateSmokeTest
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
            var grammar=DynamicsUpdateContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            for(int variant=0;variant<5;variant++){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();string originalPath=variant==1?"Allied/Props/AlliedDynamicsImpact.xml":variant==2?"Civilian/Shiro_Sanitarium_SS/Destructible_Props/SS_FlingBarrel01.xml":"BaseObjects/BaseVehicle.xml";
                var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original(originalPath,"DynamicsUpdate").Single(),true);ConscriptVariableBehaviorSource.DropLeafFormatting(owner);behaviors.AppendChild(owner);
                // Reborn: all volume/shape families and optional transforms vary independently of the selected retail witnesses.
                if(variant==3){owner.SetAttribute("id","RebornDynamicsEdited");owner.SetAttribute("Motion","STATIC");owner.SetAttribute("Collision","NONCOLLIDING");owner.SetAttribute("IntuitiveCollision","COLLIDES_WITH_EVERYTHING");owner.SetAttribute("ContactTag","TREE");owner.SetAttribute("DrawPriority","MUST_BE_DRAWN");owner.SetAttribute("FreezeDistanceFromCamera","0");
                    owner.InnerXml="<Volume xmlns='uri:ea.com:eala:asset' Mass='5' Bounciness='75%' Spinniness='4' StaticFriction='0.3' KineticFriction='0.4' AngularDrag='0.2' LinearDrag='0.25' ContactTag='DEBRIS' ReportContacts='true'><Sphere Radius='3'><Translation x='1' y='2' z='3'/><Rotation x='0.1' y='0.2' z='0.3' w='0.4'/></Sphere><Sphere Radius='4'/><Capsule Radius='2' HalfLength='5'/><Box HalfSizeX='1' HalfSizeY='2' HalfSizeZ='3' EdgeRadius='0.5'/><Cylinder Radius='2' HalfLength='4' EdgeRadius='0.25'/><Triangle X1='1' Y1='2' Z1='3' X2='4' Y2='5' Z2='6' X3='7' Y3='8' Z3='9' EdgeRadius='0.1'/></Volume>";}
                // Reborn: omitted attributes and an allocated-empty volume keep official defaults without acquiring captured shape counts.
                if(variant==4){owner.RemoveAll();owner.SetAttribute("id","RebornDynamicsDefaults");owner.InnerXml="<Volume xmlns='uri:ea.com:eala:asset'/>";var second=(XmlElement)owner.CloneNode(true);second.SetAttribute("id","RebornSecondDynamics");second.InnerXml="";behaviors.AppendChild(second);}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("DynamicsUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().First();
                var native=ConscriptBehaviorSurvey.Compile(normalized,DynamicsUpdateContract.TypeId);var element=resolved.GetElementsByTagName("DynamicsUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().First();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(DynamicsUpdateModuleData),DynamicsUpdateContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,DynamicsUpdateContract.Wire,typeof(DynamicsUpdateModuleData));_=grammar.Verify(metadata,native,typeof(DynamicsUpdateModuleData),proof.Records);checks++;
                var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;
                // Reborn: corruption of an authored fixed field must fail independent native/source validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<DynamicsUpdateModuleData>(nameof(DynamicsUpdateModuleData.FreezeDistanceFromCamera)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(DynamicsUpdateModuleData),DynamicsUpdateContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt DynamicsUpdate payload accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: DynamicsUpdate: PASS ({checks} original/default/edit/multiple-module/composition/corruption checks; all five shape families, optional transforms, retail volume layout; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
