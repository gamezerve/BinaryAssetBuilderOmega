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

// Reborn: cover EVA-create client stock imports, authored logic-frame counts, defaults and full object composition without editing user sources.
internal static class EvaCreateClientSmokeTest
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
            var grammar=EvaCreateClientContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,5)){
                var source=(XmlDocument)baseline.CloneNode(true);var updates=source.GetElementsByTagName("ClientUpdates",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();updates.RemoveAll();var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original("Soviet/Structures/SovietSuperWeapon.xml","EvaAnnounceClientCreate").First(),true);updates.AppendChild(owner);
                // Reborn: raw frame counts include values that must never be converted to seconds, multiplied by 30 or truncated to signed int.
                owner.SetAttribute("DelayBeforeAnnouncementLogicFrames",variant switch{0=>"0",1=>"17",2=>"150",3=>"4294967295",_=>"0"});
                if(variant>=1&&variant<=3){foreach(string name in new[]{"OnlyIfVisible","CountAsFirstSightedAnnouncement","UseObjectsPosition","CreateFakeRadarEvent"})owner.SetAttribute(name,"true");string eva=catalog.First(a=>a.Name.StartsWith("EvaEvent:")).Name.Split(':')[1];foreach(string name in new[]{"AnnouncementEventEnemy","AnnouncementEventAlly","AnnouncementEventOwner"})owner.SetAttribute(name,eva);}
                if(variant==4){owner.RemoveAll();owner.SetAttribute("id","RebornEvaCreateDefaults");}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("EvaAnnounceClientCreate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=Compile(normalized);var element=resolved.GetElementsByTagName("EvaAnnounceClientCreate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(Ep1EvaAnnounceClientCreateModuleData),EvaCreateClientContract.TypeId,native,references);var metadata=Ep1EvaCreateClientTokenizer.Tokenize(native);_=grammar.Verify(metadata,native,typeof(Ep1EvaAnnounceClientCreateModuleData),proof.Records);checks++;
                // Reborn: authored EVA-create client variants also traverse the complete production GameObject allocator and metadata composer.
                if(variant>=1){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted logic-frame count must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<Ep1EvaAnnounceClientCreateModuleData>(nameof(Ep1EvaAnnounceClientCreateModuleData.DelayBeforeAnnouncementLogicFrames)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(Ep1EvaAnnounceClientCreateModuleData),EvaCreateClientContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt EVA-create client payload accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: EvaAnnounceClientCreate: PASS ({checks} retail/raw-frame/uint-boundary/EVA-import/default/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: dispatch one normalized EVA-create client through the actual client-update allocator, preserving the Core reference occurrence table. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe Chunk Compile(XmlElement source)
    {
        var document=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.OuterXml));document.DocumentElement!.SetAttribute("TypeId",EvaCreateClientContract.TypeId.ToString(System.Globalization.CultureInfo.InvariantCulture));var ns=new XmlNamespaceManager(document.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);GameObject* root;using var tracker=new Relo.Tracker((void**)&root,(uint)sizeof(GameObject),false);Marshaler.Marshal(new Node(document.DocumentElement.CreateNavigator()!,ns),(ClientUpdateModuleData**)&root->Body,tracker);var chunk=new Chunk();if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("EVA-create client Tracker failed.");int start=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(chunk.InstanceBuffer.AsSpan(488,4));var imports=chunk.ImportsBuffer.Length==0?Array.Empty<uint>():Enumerable.Range(0,chunk.ImportsBuffer.Length/4-1).Select(i=>System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(chunk.ImportsBuffer.AsSpan(i*4,4))-(uint)start).ToArray();var packed=imports.Length==0?Array.Empty<byte>():new byte[(imports.Length+1)*4];for(int i=0;i<imports.Length;i++)System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(packed.AsSpan(i*4,4),imports[i]);if(imports.Length!=0)System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(packed.AsSpan(imports.Length*4,4),uint.MaxValue);return new Chunk{InstanceBuffer=chunk.InstanceBuffer.AsSpan(start).ToArray(),RelocationBuffer=Array.Empty<byte>(),ImportsBuffer=packed};
    }

}
