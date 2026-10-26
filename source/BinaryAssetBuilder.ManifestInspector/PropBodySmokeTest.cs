using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: cover prop body stock identities and authored local module IDs and full object composition without editing user sources.
internal static class PropBodySmokeTest
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
            var grammar=PropBodyContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,2)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Body",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original("System/PlayerSpellBook.xml","PropBody").Single(),true);behaviors.AppendChild(owner);
                // Reborn: edit the local prop body identity through the official inherited module contract.
                if(variant==1){owner.SetAttribute("id","RebornPropBody");}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);if(ConscriptEditableScalars.ReceiptValues(source)["MaxHealth"]!=null)throw new InvalidDataException("PropBody acquired an invented health receipt.");checks++;var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("PropBody",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);int bodyAt=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(full.InstanceBuffer.AsSpan(488));int geometryAt=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(full.InstanceBuffer.AsSpan(500));if(geometryAt-bodyAt!=8)throw new InvalidDataException("PropBody must not allocate ActiveBody fields.");
                var native=new Chunk{InstanceBuffer=full.InstanceBuffer.AsSpan(bodyAt,8).ToArray(),RelocationBuffer=Array.Empty<byte>(),ImportsBuffer=Array.Empty<byte>()};var element=resolved.GetElementsByTagName("PropBody",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(PropBodyModuleData),PropBodyContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.TokenizePropBody(native,official);_=grammar.Verify(metadata,native,typeof(PropBodyModuleData),proof.Records);checks++;
                // Reborn: one authored prop body variant also traverses the complete production GameObject allocator and metadata composer.
                if(variant==1){_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted module identity must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[4]^=1;
                try{SchemaNativeContract.Verify(element,typeof(PropBodyModuleData),PropBodyContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt prop body payload accepted.");}catch(InvalidDataException){checks++;}
            }
            // Reborn: unsupported body diagnostics identify their real owner, and health fields remain invalid on PropBody under the official schema.
            var invalid=(XmlDocument)baseline.CloneNode(true);var invalidBody=invalid.GetElementsByTagName("Body",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();invalidBody.InnerXml="<UnknownBody xmlns='uri:ea.com:eala:asset' id='Unknown'/>";
            try{ConscriptModuleScalarContract.Owners(invalid);throw new InvalidOperationException("Unknown body accepted.");}catch(InvalidDataException error){if(!error.Message.Contains("Body/UnknownBody",StringComparison.Ordinal)||SdkXmlDiagnostics.Node(error)?.LocalName!="UnknownBody")throw;checks++;}
            invalidBody.InnerXml="<PropBody xmlns='uri:ea.com:eala:asset' id='InvalidHealth' MaxHealth='100'/>";
            try{ConscriptVariableBehaviorSource.Admit(invalid,baseline,official);throw new InvalidOperationException("PropBody accepted ActiveBody-only health.");}catch(InvalidDataException error){if(!error.Message.Contains("MaxHealth",StringComparison.Ordinal))throw;checks++;}
            Console.WriteLine($"Reborn: PropBody: PASS ({checks} original/identity/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
