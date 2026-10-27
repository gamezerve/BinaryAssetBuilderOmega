using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: cover all official modifier fields and the normal authoring/stream pipeline without touching user mods.
internal static class AttributeModifierCompilerSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare retail roots, authored masks/imports/lists and inherited definition-driven sources before final stream/package readback. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global,string output)
    {
        output=SdkEnvironmentPreflight.Absolute(output);if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh modifier test output required.");Directory.CreateDirectory(output);var saved=Settings.Current;
        try{
            string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;var official=ConscriptSourceInput.Schemas();Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=new[]{manifest,global}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).ToArray();AttributeModifierCompiler.Qualify(manifest,schemas,catalog);int checks=0;
            // Reborn: every official category, including SHRINK, is editable independently of any captured source ID.
            foreach(string category in Enum.GetNames<AttributeModifierCategoryType>()){
                var source=Xml("<AttributeModifier xmlns='uri:ea.com:eala:asset' id='RebornCategory' Category='"+category+"'/>");Check(source);checks++;
            }
            // Reborn: official armor-set indices retain INVALID at zero; stock shrink uses index 14, not 13.
            foreach(string armor in Enum.GetNames<ArmorSetType>()){Check(Xml("<AttributeModifier xmlns='uri:ea.com:eala:asset' id='RebornArmor' ArmorSetType='"+armor+"'/>"));checks++;}
            // Reborn: cover all modifier-list enum values, percent syntax, both model masks, status masks and all own scalar/import fields together.
            string shader=catalog.First(a=>a.TypeName=="ShaderOverride").Name,fx=catalog.First(a=>a.TypeName=="FXList").Name;
            string entries=string.Concat(Enum.GetNames<AttributeType>().Select((name,i)=>"<Modifier Type='"+name+"' Value='"+(i+1)+"%'/>") );
            var full=Xml("<AttributeModifier xmlns='uri:ea.com:eala:asset' id='RebornFullModifier' Category='SHRINK' Duration='1250ms' ReplaceInCategoryIfLongest='true' IgnoreIfAnticategoryActive='true' StartFX='"+fx+"' EndFX='"+fx+"' Shader='"+shader+"' ModelConditionsSet='COVER IRONCURTAIN' ModelConditionsClear='DAMAGED' ObjectStatusToSet='NO_BRIBE IMMUNE_TO_BARK' StackingLimit='7' ArmorSetType='VETERAN'>"+entries+"</AttributeModifier>");
            Check(full);checks++;
            var emptyMasks=Xml("<AttributeModifier xmlns='uri:ea.com:eala:asset' id='RebornEmptyMasks' ModelConditionsSet='' ModelConditionsClear='' ObjectStatusToSet=''/>");Check(emptyMasks);checks++;
            // Reborn: missing typed dependencies and invalid schema values remain genuine build errors.
            foreach(var mutate in new Action<XmlElement>[] {e=>e.SetAttribute("StartFX","RebornMissingModifierFX"),e=>e.SetAttribute("Category","NOT_A_CATEGORY"),e=>e.SetAttribute("StackingLimit","-1")}){
                var bad=(XmlDocument)full.CloneNode(true);mutate(bad.DocumentElement!);try{AttributeModifierCompiler.ProofSource(bad,schemas,catalog);throw new InvalidOperationException("Invalid modifier source accepted.");}catch(InvalidDataException){checks++;}
            }
            // Reborn: instance-only bases stay source-only; definitions and local FX references survive normal preparation and dependency-sorted packaging.
            File.WriteAllText(Path.Combine(output,"base.xml"),"<AssetDeclaration xmlns='uri:ea.com:eala:asset'><AttributeModifier id='RebornModifierBase' Duration='2s' Category='BUFF'/></AssetDeclaration>");
            string entry=Path.Combine(output,"mod.xml");File.WriteAllText(entry,"<AssetDeclaration xmlns='uri:ea.com:eala:asset'><Includes><Include type='instance' source='base.xml'/></Includes><Defines><Define name='REBONUS' value='120%'/></Defines><AttributeModifier id='RebornModifier' inheritFrom='RebornModifierBase' StartFX='RebornModifierFX' EndFX='RebornModifierFX'><Modifier Type='SPEED' Value='=$REBONUS'/><Modifier Type='RATE_OF_FIRE' Value='125%'/><Modifier Type='DAMAGE_MULT' Value='50%'/><Modifier Type='EXPERIENCE' Value='150%'/><Modifier Type='HEALTH_MULT' Value='110%'/></AttributeModifier><AttributeModifier id='RebornLongModifier'>"+entries+"</AttributeModifier><AttributeModifier id='RebornDefaults'/><FXList id='RebornModifierFX'><NuggetList/></FXList></AssetDeclaration>");
            // Reborn: a real GameObject aura consumes the newly emitted modifier in the same mod-first dependency graph.
            var entryXml=Xml(File.ReadAllText(entry));var consumer=Xml(File.ReadAllText(Path.Combine(fixtures,"live-alpha/ConscriptBuildTime1/data/mod.xml")));var aura=consumer.CreateElement("AttributeModifierAuraUpdate",SchemaSet.XmlNamespace);aura.SetAttribute("id","RebornLocalAura");aura.SetAttribute("AttributeModifierName","RebornModifier");aura.SetAttribute("InitiallyActive","true");consumer.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace)[0]!.AppendChild(aura);entryXml.DocumentElement!.AppendChild(entryXml.ImportNode(consumer.DocumentElement!,true));File.WriteAllText(entry,entryXml.OuterXml);
            string prepared=Path.Combine(output,"prepared");ConscriptSourceInput.Prepare(entry,prepared);
            var flattened=Xml(File.ReadAllText(Path.Combine(prepared,"additional.xml")));var derived=flattened.GetElementsByTagName("AttributeModifier",SchemaSet.XmlNamespace).OfType<XmlElement>().Single(e=>e.GetAttribute("id")=="RebornModifier");if(derived.GetAttribute("Duration")!="2s"||derived.HasAttribute("inheritFrom")||flattened.OuterXml.Contains("=$"))throw new InvalidDataException("Modifier inheritance/definition preparation differs.");
            if(!ConscriptMultiAssetBuild.TryRun(Path.Combine(prepared,"game.xml"),manifest,global,Path.Combine(output,"built")))throw new InvalidDataException("Mixed modifier source route was not selected.");
            Console.WriteLine($"Reborn: AttributeModifier compiler: PASS ({checks} category/all-field/default/empty-mask/invalid-source checks, six original stock graphs, all modifier types, source-only inheritance/definitions, local aura/modifier/FX imports and normal linked stream/package readback; no user output or game launch).");

            //-------------------------------------------------------------------------------------------------
            /** Reborn: verify current source independently and refuse a changed emitted duration word for every authored variant. */
            //-------------------------------------------------------------------------------------------------
            void Check(XmlDocument source){var proof=AttributeModifierCompiler.ProofSource(source,schemas,catalog);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var native=AttributeModifierCompiler.Compile(instance.XmlNode);var refs=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();SchemaNativeContract.Verify(proof.DocumentElement!,typeof(AttributeModifier),0,native,refs);var bad=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};bad.InstanceBuffer[8]^=1;try{SchemaNativeContract.Verify(proof.DocumentElement!,typeof(AttributeModifier),0,bad,refs);throw new InvalidOperationException("Corrupt modifier duration accepted.");}catch(InvalidDataException){checks++;}}
        }finally{Settings.Current=saved;}
    }
    // Reborn: detached private source documents retain ordinary EA namespace and schema semantics.
    private static XmlDocument Xml(string text)=>SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(text));
}
