using System.Buffers.Binary;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: compile campaign FireSpecialPowerTemplate imports and a mod-defined power without modifying user sources or launching the game.
internal static class OclFireSpecialPowerSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare actual WorldBuilder paradrops, prove each authored +208 import, refuse corruption and read back a privately published mod stream. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global,string output)
    {
        // Reborn: keep every test input/output absolute before entering the standard source preparation path.
        output=Path.GetFullPath(output);
        if(Directory.Exists(output))throw new InvalidDataException("Fresh OCL evidence directory required.");Directory.CreateDirectory(output);
        var saved=Settings.Current;
        try{
            string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;var official=ConscriptSourceInput.Schemas();
            Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=new[]{manifest,global}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).ToArray();
            var sources=GameObjectExtendedRootContract.Original("Allied/Units_Campaign/CAMP_AlliedBomberAircraft_Infantry_1.xml","ObjectCreationList").Select(e=>SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(e.OuterXml))).ToArray();
            ObjectCreationListCompiler.Qualify(manifest,global,schemas,catalog,sources);int checks=0;
            var example=catalog.First(a=>a.TypeId==FastHash.GetHashCode("SpecialPowerTemplate"));var local=example with{InstanceId=InstanceHandle.GetInstanceId("RebornOclPower"),Name="SpecialPowerTemplate:RebornOclPower",References=Array.Empty<AssetId>()};var available=catalog.Append(local).ToArray();
            foreach(int variant in Enumerable.Range(0,4)){
                var source=(XmlDocument)sources[variant%2].CloneNode(true);if(variant>=2){source.DocumentElement!.SetAttribute("id","RebornOcl"+variant);foreach(var child in source.DocumentElement.ChildNodes.OfType<XmlElement>())child.SetAttribute("FireSpecialPowerTemplate","RebornOclPower");}
                var proof=ObjectCreationListCompiler.ProofSource(source,schemas,available);var resolved=SchemaNativeContract.Resolve(source,schemas,available);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var native=ObjectCreationListCompiler.Compile(instance.XmlNode);var refs=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();SchemaNativeContract.Verify(proof.DocumentElement!,typeof(Ep1ObjectCreationList),0,native,refs);
                int count=checked((int)Word(native.InstanceBuffer,8)),items=checked((int)Word(native.InstanceBuffer,12));var imports=Enumerable.Range(0,native.ImportsBuffer.Length/4).Select(i=>Word(native.ImportsBuffer,i*4)).ToHashSet();
                for(int i=0;i<count;i++){int slot=items+i*248+208;uint index=Word(native.InstanceBuffer,slot);var expected=variant>=2?new AssetId(local.TypeId,local.InstanceId):new AssetId(FastHash.GetHashCode("SpecialPowerTemplate"),InstanceHandle.GetInstanceId("SpecialPower_EjectPassengers"));if(!imports.Contains((uint)slot)||index==0||index>refs.Length||refs[index-1]!=expected)throw new InvalidDataException("OCL FireSpecialPowerTemplate import identity/slot differs.");}checks++;
                // Reborn: the reference word and IMP inventory are independently bound to the current XML in both build modes.
                var bad=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};BinaryPrimitives.WriteUInt32LittleEndian(bad.InstanceBuffer.AsSpan(items+208),0);try{SchemaNativeContract.Verify(proof.DocumentElement!,typeof(Ep1ObjectCreationList),0,bad,refs);throw new InvalidOperationException("Corrupt OCL power import accepted.");}catch(InvalidDataException){checks++;}
            }
            // Reborn: one ordinary authored declaration compiles a new power and its OCL together into private output with the standard linker/package readback.
            var document=(XmlDocument)sources[0].CloneNode(true);document.DocumentElement!.SetAttribute("id","RebornFirePowerOcl");foreach(var child in document.DocumentElement.ChildNodes.OfType<XmlElement>())child.SetAttribute("FireSpecialPowerTemplate","RebornOclPower");string entry=Path.Combine(output,"Static_Mod.xml");File.WriteAllText(entry,"<!-- Reborn: isolated OCL/power integration source. --><AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+document.DocumentElement.OuterXml+"<SpecialPowerTemplate id='RebornOclPower' TargetType='LOCATION'/></AssetDeclaration>");string prepared=Path.Combine(output,"prepared");ConscriptSourceInput.Prepare(entry,prepared);if(!ConscriptMultiAssetBuild.TryRun(Path.Combine(prepared,"game.xml"),manifest,global,Path.Combine(output,"built")))throw new InvalidDataException("OCL mixed stream route not selected.");checks++;
            // Reborn: a detached prepared OCL field still reports its original authoring file and exact field location.
            var diagnostic=SdkXmlDiagnostics.At(new InvalidDataException("OCL field provenance probe."),document.GetElementsByTagName("CreateObject",SchemaSet.XmlNamespace).OfType<XmlElement>().First().GetAttributeNode("FireSpecialPowerTemplate"));ConscriptCompileLog.ForPrepared(Path.Combine(prepared,"game.xml")).Enrich(diagnostic);var location=SdkXmlDiagnostics.Origin(diagnostic);if(location==null||location.Context||location.Line<=0||!location.File.Equals(entry,StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("OCL field lost original source coordinates.");checks++;
            Console.WriteLine($"Reborn: OCL FireSpecialPowerTemplate: PASS ({checks} retail/mod-defined targets, exact +208 IMP slots, corrupt import rejection and final stream/package readback; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }

    // Reborn: native reads remain inside the compiled OCL buffers.
    private static uint Word(byte[] bytes,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));
}
