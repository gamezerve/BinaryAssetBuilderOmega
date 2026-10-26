using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: purchase-button authoring resolves real technology and state assets.
internal static class PurchaseTechButtonCompilerSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify original purchase-button imports, reference retargeting and local technology/state chains in a private package. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global,string output)
    {
        output=SdkEnvironmentPreflight.Absolute(output);if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh purchase-button test output required.");Directory.CreateDirectory(output);var saved=Settings.Current;
        try{
            string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;var official=ConscriptSourceInput.Schemas();Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=new[]{manifest,global}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).ToArray();PurchaseTechButtonCompiler.Qualify(manifest,schemas,catalog);int checks=0;
            foreach(string id in new[]{"Purchase_PlayerTech_Allied_AirPower","Purchase_PlayerTech_Allied_ChronoRift_Rank1","Purchase_PlayerTech_Yuriko_Shield_Rank1","Purchase_PlayerTech_Yuriko_Shield_Rank2","Purchase_PlayerTech_Yuriko_Shield_Rank3"}){var source=PurchaseTechButtonCompiler.Original(id,schemas);Check(source);source.DocumentElement!.SetAttribute("PlayerTech","PlayerTech_Generic_Rank_3");source.DocumentElement.SetAttribute("StateData","ButtonStateOpenPlayerPowers");Check(source);}
            // Reborn: absent required attributes and unknown typed targets remain genuine source errors.
            foreach(string field in new[]{"PlayerTech","StateData"})foreach(bool missing in new[]{false,true}){var invalid=PurchaseTechButtonCompiler.Original("Purchase_PlayerTech_Allied_AirPower",schemas);if(missing)invalid.DocumentElement!.RemoveAttribute(field);else invalid.DocumentElement!.SetAttribute(field,"RebornMissingPurchaseTarget");bool rejected=false;try{PurchaseTechButtonCompiler.ProofSource(invalid,schemas,catalog);}catch(InvalidDataException){rejected=true;}if(!rejected)throw new InvalidDataException("Invalid required purchase-button reference was accepted: "+field);}
            var assets=new List<string>();for(int rank=3;rank>=1;rank--){assets.Add("<PurchasePlayerTechButtonTemplate id='RebornPurchase_Rank"+rank+"' PlayerTech='RebornPurchaseTech_Rank"+rank+"' StateData='RebornPurchaseState_Rank"+rank+"'/>");assets.Add("<ButtonSingleStateData id='RebornPurchaseState_Rank"+rank+"'><State Title='NAME:RebornPurchaseRank"+rank+"'/></ButtonSingleStateData>");assets.Add("<PlayerTech id='RebornPurchaseTech_Rank"+rank+"' TechPointsRequired='"+rank+"'><TechDependency>"+(rank==1?"PlayerTech_Generic_Rank_3":"RebornPurchaseTech_Rank"+(rank-1))+"</TechDependency></PlayerTech>");}
            string entry=Path.Combine(output,"mod.xml");File.WriteAllText(entry,"<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+string.Join("",assets)+"</AssetDeclaration>");string prepared=Path.Combine(output,"prepared");ConscriptSourceInput.Prepare(entry,prepared);if(!ConscriptMultiAssetBuild.TryRun(Path.Combine(prepared,"game.xml"),manifest,global,Path.Combine(output,"built")))throw new InvalidDataException("Purchase-button route not selected.");Console.WriteLine($"Reborn: PurchasePlayerTechButtonTemplate: PASS (5 original EP1 graphs, {checks} native original/retarget/corruption controls, required/missing typed-reference refusals, three local purchase/state/technology ranks, mod+stock resolution and final stream/package readback; no user output or game launch).");

            // Reborn: independent validation must reject a corrupted technology import selector.
            void Check(XmlDocument source){var proof=PurchaseTechButtonCompiler.ProofSource(source,schemas,catalog);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var native=PurchaseTechButtonCompiler.Compile(instance.XmlNode);var refs=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();SchemaNativeContract.Verify(proof.DocumentElement!,typeof(Ep1PurchasePlayerTechButtonTemplate),0,native,refs);var bad=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};bad.InstanceBuffer[4]^=1;try{SchemaNativeContract.Verify(proof.DocumentElement!,typeof(Ep1PurchasePlayerTechButtonTemplate),0,bad,refs);throw new InvalidOperationException("Corrupt purchase-button import accepted.");}catch(InvalidDataException){}checks++;}
        }finally{Settings.Current=saved;}
    }
}
