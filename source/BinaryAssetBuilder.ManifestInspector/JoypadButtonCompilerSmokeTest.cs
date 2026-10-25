using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
using Chunk=Relo.Chunk;
namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: exercise all four joypad-button variants with independent native proofs and private final package output.
internal static class JoypadButtonCompilerSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify stock graphs, exercise all schema choices and link buttons to locally authored commands and powers. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global,string output)
    {
        output=SdkEnvironmentPreflight.Absolute(output);if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh joypad-button output required.");Directory.CreateDirectory(output);var saved=Settings.Current;
        try{
            string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;var official=ConscriptSourceInput.Schemas();Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=new[]{manifest,global}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).ToArray();JoypadButtonCompiler.Qualify(manifest,schemas,catalog);int checks=0;var assets=new List<string>();
            var templates=new[]{"UI/Joypad/CommandBar/ButtonTemplates.xml","EP1/UI/Joypad/CommandBar/ButtonTemplates_EP1.xml"}.SelectMany(path=>GameObjectExtendedRootContract.Original(path,"JoypadCommandBarButtonTemplate")).GroupBy(e=>e.GetElementsByTagName("Data",SchemaSet.XmlNamespace).OfType<XmlElement>().Single().ChildNodes.OfType<XmlElement>().Single().LocalName).ToDictionary(g=>g.Key,g=>SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(g.First().OuterXml)));
            // Reborn: three schema alternatives have no retail source witness; their inherited layouts and ordinary fields are exercised as authored variants.
            foreach(var pair in new[]{("RedAlert","OpenPlayerPowers"),("PlayerUpgrade","ObjectUpgrade"),("MultiplePowersTargetedSpecialPower","TargetedSpecialPower")}){
                var xml=(XmlDocument)templates[pair.Item2].CloneNode(true);var wrapper=xml.GetElementsByTagName("Data",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var old=wrapper.ChildNodes.OfType<XmlElement>().Single();var replacement=xml.CreateElement(pair.Item1,SchemaSet.XmlNamespace);foreach(XmlAttribute attr in old.Attributes)replacement.SetAttribute(attr.Name,attr.Value);wrapper.ReplaceChild(replacement,old);
                if(pair.Item1=="MultiplePowersTargetedSpecialPower")foreach(string name in new[]{"AdditionalSpecialPowerTemplate","AdditionalRadiusCursor"}){var child=xml.CreateElement(name,SchemaSet.XmlNamespace);child.InnerText=name=="AdditionalRadiusCursor"?"Target_Allied_Chronosphere_1":"SpecialPowerIronCurtain";replacement.AppendChild(child);}
                templates.Add(pair.Item1,xml);
            }
            if(templates.Count!=29)throw new InvalidDataException("All 29 joypad schema alternatives must be exercised.");
            foreach(var pair in templates){
                var source=(XmlDocument)pair.Value.CloneNode(true);Check(source);source.DocumentElement!.SetAttribute("id","RebornJoypad"+pair.Key);var data=source.GetElementsByTagName("Data",SchemaSet.XmlNamespace).OfType<XmlElement>().Single().ChildNodes.OfType<XmlElement>().Single();
                foreach(var element in source.GetElementsByTagName("*",SchemaSet.XmlNamespace).OfType<XmlElement>())if(element.HasAttribute("StateData"))element.SetAttribute("StateData","ButtonStateOpenPlayerPowers");
                if(data.HasAttribute("LogicCommand"))data.SetAttribute("LogicCommand","Command_ActivateSuperSonicSpecialPower");
                if(pair.Key.Contains("Targeted",StringComparison.Ordinal)){data.SetAttribute("ValidTargetOutline","SELECTED");var tint=source.CreateElement("ValidTargetTintColor",SchemaSet.XmlNamespace);tint.SetAttribute("R","0.25");tint.SetAttribute("G","0.5");tint.SetAttribute("B","1");data.PrependChild(tint);}
                if(pair.Key=="PlayerMultipleTargetsTargetedPower"){data.SetAttribute("TargetCount","7");data.SetAttribute("MinTargetDistance","12.5");}
                if(pair.Key=="PlayerControlGroup")data.SetAttribute("GroupShortcut","9");
                foreach(string name in new[]{"StateData","AdditionalSpecialPowerTemplate","AdditionalRadiusCursor"}){var first=data.ChildNodes.OfType<XmlElement>().FirstOrDefault(e=>e.LocalName==name);if(first!=null)data.InsertAfter(first.CloneNode(true),first);}
                Check(source);if(data.HasAttribute("LogicCommand"))data.SetAttribute("LogicCommand","RebornJoypadCommand");if(data.HasAttribute("SpecialPower"))data.SetAttribute("SpecialPower","RebornJoypadPower");if(pair.Key=="PlayerControlGroup")data.SetAttribute("GroupShortcut","=$GROUP");assets.Add(source.DocumentElement.OuterXml);
            }
            assets.Add("<SpecialPowerTemplate id='RebornJoypadPower' TargetType='LOCATION' ReloadTime='15s'/>");assets.Add("<LogicCommand id='RebornJoypadCommand' Type='SPECIAL_POWER'><SpecialPower>RebornJoypadPower</SpecialPower></LogicCommand>");string entry=Path.Combine(output,"mod.xml");File.WriteAllText(entry,"<AssetDeclaration xmlns='uri:ea.com:eala:asset'><Defines><Define name='GROUP' value='9'/></Defines>"+string.Join("",assets)+"</AssetDeclaration>");string prepared=Path.Combine(output,"prepared");ConscriptSourceInput.Prepare(entry,prepared);if(!ConscriptMultiAssetBuild.TryRun(Path.Combine(prepared,"game.xml"),manifest,global,Path.Combine(output,"built")))throw new InvalidDataException("Joypad button route not selected.");
            Console.WriteLine($"Reborn: JoypadCommandBarButtonTemplate: PASS (26 original EP1 variants, all 29 Data choices, {checks} native source/edit controls, filters/enums/tint/markers/state lists, local command/power linking and final stream/package readback; no user output or game launch).");

            // Reborn: independent validation must reject a corrupted concrete Data class identity.
            void Check(XmlDocument source){var proof=JoypadButtonCompiler.ProofSource(source,schemas,catalog);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var native=JoypadButtonCompiler.Compile(instance.XmlNode);var refs=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();SchemaNativeContract.Verify(proof.DocumentElement!,typeof(Ep1JoypadCommandBarButtonTemplate),0,native,refs);var bad=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};bad.InstanceBuffer[8]^=1;try{SchemaNativeContract.Verify(proof.DocumentElement!,typeof(Ep1JoypadCommandBarButtonTemplate),0,bad,refs);throw new InvalidOperationException("Corrupt button class accepted.");}catch(InvalidDataException){}checks++;}
        }finally{Settings.Current=saved;}
    }
}
