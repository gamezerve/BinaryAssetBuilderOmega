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

// Reborn: cover EVA-ready client stock identities and authored upgrade lists, inherited references and local module IDs and full object composition without editing user sources.
internal static class EvaReadyClientSmokeTest
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
            var grammar=EvaReadyClientContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,5)){
                var source=(XmlDocument)baseline.CloneNode(true);var clients=source.GetElementsByTagName("ClientUpdates",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var group=(XmlElement)clients.ChildNodes.OfType<XmlElement>().First().CloneNode(true);clients.RemoveAll();var originals=GameObjectExtendedRootContract.Original("System/PlayerSpellBook.xml","EvaAnnounceSpecialPowerReadyClientUpdate");var owner=(XmlElement)source.ImportNode(variant==1?originals.Last():originals.First(),true);clients.AppendChild(owner);
                // Reborn: retarget all four strong references and turn both stock-enabled flags off.
                if(variant==2){owner.SetAttribute("id","RebornEvaRetarget");owner.SetAttribute("AnnouncementEventEnemy","MassProductionUpgradeAuthorized");owner.SetAttribute("AnnouncementEventAlly","AssemblyUpgradeAuthorized");owner.SetAttribute("AnnouncementEventOwner","FreeTradeUpradeAuthorized");owner.SetAttribute("SpecialPowerTemplate","SpecialPowerJapanBalloonAttack_L2");owner.SetAttribute("JustMonitorPercentReady","false");owner.SetAttribute("CheckPlayerPowerVisibility","false");}
                if(variant==3){owner.SetAttribute("id","RebornEvaDefaults");foreach(string field in new[]{"AnnouncementEventEnemy","AnnouncementEventAlly","AnnouncementEventOwner","JustMonitorPercentReady","CheckPlayerPowerVisibility"})owner.RemoveAttribute(field);}
                // Reborn: existing group-audio and multiple EVA leaves share the real list allocator and reference table.
                if(variant==4){clients.PrependChild(source.ImportNode(group,true));owner.SetAttribute("id","RebornEvaOne");var second=(XmlElement)owner.CloneNode(true);second.SetAttribute("id","RebornEvaTwo");second.SetAttribute("SpecialPowerTemplate","SpecialPowerJapanBalloonAttack_L3");clients.AppendChild(second);}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("EvaAnnounceSpecialPowerReadyClientUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().First();
                var native=CompileLeaf(normalized);var element=resolved.GetElementsByTagName("EvaAnnounceSpecialPowerReadyClientUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().First();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(Ep1EvaAnnounceSpecialPowerReadyClientUpdateModuleData),EvaReadyClientContract.TypeId,native,references);var metadata=Ep1EvaReadyClientTokenizer.Tokenize(native);_=grammar.Verify(metadata,native,typeof(Ep1EvaAnnounceSpecialPowerReadyClientUpdateModuleData),proof.Records);checks++;
                // Reborn: one authored EVA-ready client variant also traverses the complete production GameObject allocator and metadata composer.
                if(variant==4){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted visibility flag must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[25]^=1;
                try{SchemaNativeContract.Verify(element,typeof(Ep1EvaAnnounceSpecialPowerReadyClientUpdateModuleData),EvaReadyClientContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt EVA-ready client payload accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: EvaAnnounceSpecialPowerReadyClientUpdate: PASS ({checks} two originals/all references/flags/default/mixed-list/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: one client leaf uses the actual production client-update dispatcher and normal Tracker import inventory. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe Chunk CompileLeaf(XmlElement source)
    {
        var document=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.OuterXml));var element=document.DocumentElement!;element.SetAttribute("TypeId",EvaReadyClientContract.TypeId.ToString(System.Globalization.CultureInfo.InvariantCulture));var ns=new XmlNamespaceManager(document.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);
        GameObject* root;using var tracker=new Relo.Tracker((void**)&root,(uint)sizeof(GameObject),false);Marshaler.Marshal(new Node(element.CreateNavigator()!,ns),(ClientUpdateModuleData**)&root->Body,tracker);var all=new Chunk();if(!tracker.MakeRelocatable(all))throw new InvalidDataException("EVA client allocation failed.");
        int start=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(all.InstanceBuffer.AsSpan(488));if(start<600||all.InstanceBuffer.Length-start!=28||all.RelocationBuffer.Length!=8||System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(all.RelocationBuffer)!=488u||System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(all.RelocationBuffer.AsSpan(4))!=uint.MaxValue)throw new InvalidDataException("EVA diagnostic root/leaf ownership differs.");
        var imports=all.ImportsBuffer.ToArray();for(int at=0;at+4<imports.Length;at+=4){uint slot=System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(imports.AsSpan(at,4));if(slot<start||slot>=start+28)throw new InvalidDataException("EVA import escapes its leaf.");System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(imports.AsSpan(at,4),slot-(uint)start);}
        return new Chunk{InstanceBuffer=all.InstanceBuffer.AsSpan(start).ToArray(),RelocationBuffer=Array.Empty<byte>(),ImportsBuffer=imports};
    }

}
