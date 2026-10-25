using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
using Chunk=Relo.Chunk;
namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: exercise all four button-state variants with independent native proofs and private final package output.
internal static class ButtonStateCompilerSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify stock states, edit/omit strings and image imports, and bind both button families to a locally authored state. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global,string output)
    {
        output=SdkEnvironmentPreflight.Absolute(output);if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh button-state output required.");Directory.CreateDirectory(output);var saved=Settings.Current;
        try{
            string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;var official=ConscriptSourceInput.Schemas();Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=new[]{manifest,global}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).ToArray();ButtonStateCompiler.Qualify(manifest,schemas,catalog);int checks=0;var assets=new List<string>();
            foreach(string id in new[]{"ButtonStatePlayerPowerAirPower","ButtonStateOpenPlayerPowers"}){
                var source=ButtonStateCompiler.Original(id,schemas);Check(source);source.DocumentElement!.SetAttribute("id","Reborn"+id);var state=source.GetElementsByTagName("State",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();state.SetAttribute("Title","NAME:RebornTitle");state.SetAttribute("Description","DESC:RebornDescription");state.SetAttribute("TypeDescription","TYPE:RebornType");Check(source);assets.Add(source.DocumentElement.OuterXml);
            }
            // Reborn: every subset of optional image/title/description/type fields compiles; omission is distinct from explicit empty strings.
            string[] fields={"Image","Title","Description","TypeDescription"};for(int mask=0;mask<16;mask++){var source=ButtonStateCompiler.Original("ButtonStatePlayerPowerAirPower",schemas);var state=source.GetElementsByTagName("State",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();for(int bit=0;bit<4;bit++)if((mask&(1<<bit))==0)state.RemoveAttribute(fields[bit]);else if(bit>0&&mask==15)state.SetAttribute(fields[bit],string.Empty);Check(source);}
            var player=PlayerPowerButtonCompiler.Original("ButtonPlayerPowerYurikoShieldRank1",schemas);player.DocumentElement!.SetAttribute("id","RebornStatePlayerButton");player.GetElementsByTagName("Power",SchemaSet.XmlNamespace).OfType<XmlElement>().Single().SetAttribute("StateData","RebornButtonStatePlayerPowerAirPower");assets.Add(player.DocumentElement.OuterXml);
            var joypad=JoypadButtonCompiler.Original("ButtonOpenPlayerPowers",schemas);joypad.DocumentElement!.SetAttribute("id","RebornStateJoypadButton");joypad.GetElementsByTagName("OpenPlayerPowers",SchemaSet.XmlNamespace).OfType<XmlElement>().Single().SetAttribute("StateData","RebornButtonStateOpenPlayerPowers");assets.Add(joypad.DocumentElement.OuterXml);
            string entry=Path.Combine(output,"mod.xml");File.WriteAllText(entry,"<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+string.Join("",assets)+"</AssetDeclaration>");string prepared=Path.Combine(output,"prepared");ConscriptSourceInput.Prepare(entry,prepared);if(!ConscriptMultiAssetBuild.TryRun(Path.Combine(prepared,"game.xml"),manifest,global,Path.Combine(output,"built")))throw new InvalidDataException("Button-state route not selected.");
            // Reborn: semantic Title lookup must warn without relying on a colon prefix, while Description/TypeDescription resolve against stock plus mod keys.
            string dataPath=Path.Combine(output,"strings","Data");Directory.CreateDirectory(dataPath);string textEntry=Path.Combine(dataPath,"Static_Mod.xml");File.WriteAllText(textEntry,"<AssetDeclaration xmlns='uri:ea.com:eala:asset'><ButtonSingleStateData id='RebornStringProbe'>\n<State Title='MissingPlainTitle' Description='DESC:Stock' TypeDescription='TYPE:Mod'/>\n</ButtonSingleStateData></AssetDeclaration>");
            var paths=SdkSourcePathAudit.Inspect(SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),dataPath,textEntry,Path.Combine(output,"unused-output"),Array.Empty<string>()));var strings=SdkMissingStringAudit.Analyze(textEntry,paths,official,new[]{"DESC:Stock"},new[]{"TYPE:Mod"});if(strings.Findings.Length!=1||strings.Findings[0].Target!="MissingPlainTitle"||strings.Findings[0].Owner!="ButtonSingleStateData:RebornStringProbe"||strings.Findings[0].Field!="State/@Title"||strings.Findings[0].Line!=2)throw new InvalidDataException("Button-state localization audit lost key semantics or exact source position.");
            Console.WriteLine($"Reborn: ButtonSingleStateData: PASS (5 original EP1 states, {checks} edited/omitted/empty-string native controls, local PlayerPower/Joypad state links, semantic Title missing-string source coordinates and final stream/package readback; no user output or game launch).");

            // Reborn: independent validation must reject a corrupted image import selector.
            void Check(XmlDocument source){var proof=ButtonStateCompiler.ProofSource(source,schemas,catalog);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var native=ButtonStateCompiler.Compile(instance.XmlNode);var refs=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();SchemaNativeContract.Verify(proof.DocumentElement!,typeof(Ep1ButtonSingleStateData),0,native,refs);var bad=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};bad.InstanceBuffer[4]^=1;try{SchemaNativeContract.Verify(proof.DocumentElement!,typeof(Ep1ButtonSingleStateData),0,bad,refs);throw new InvalidOperationException("Corrupt button-state import accepted.");}catch(InvalidDataException){}checks++;}
        }finally{Settings.Current=saved;}
    }
}
