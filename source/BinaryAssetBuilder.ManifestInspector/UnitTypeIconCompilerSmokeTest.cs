using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: icon assets must work through ordinary inheritance, local image dependencies and final package serialization.
internal static class UnitTypeIconCompilerSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify retail records, exercise optional/edited fields and publish an isolated linked icon/image graph without touching user mods. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global,string output)
    {
        var saved=Settings.Current;
        try{
            output=Path.GetFullPath(output);if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh icon evidence directory required.");Directory.CreateDirectory(output);
            string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;var official=ConscriptSourceInput.Schemas();
            Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=new[]{manifest,global}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).ToArray();UnitTypeIconCompiler.Qualify(manifest,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,4)){
                var source=UnitTypeIconCompiler.Original("RiflemenIcon",schemas);
                // Reborn: new identities and retargeted images use real Core imports; the vector stores signed fractional values.
                source.DocumentElement!.SetAttribute("id","RebornIcon"+variant);
                if(variant==1){source.DocumentElement.SetAttribute("Image","UnitTypeIcon_EngineerNew");source.DocumentElement.InnerXml="<Offset xmlns='uri:ea.com:eala:asset' x='12.5' y='-3.5'/>";}
                if(variant>=2)source.DocumentElement.RemoveAll();
                if(variant>=2)source.DocumentElement.SetAttribute("id","RebornIcon"+variant);
                if(variant==3)source.DocumentElement.SetAttribute("Image","UnitTypeIcon_RiflemenNew");
                var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var native=UnitTypeIconCompiler.Compile(instance.XmlNode);var refs=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                SchemaNativeContract.Verify(UnitTypeIconCompiler.ProofSource(source,schemas,catalog).DocumentElement!,typeof(UnitTypeIcon),0,native,refs);
                // Reborn: the image word must match its declared dependency, even when the rest of the native layout is valid.
                var bad=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};bad.InstanceBuffer[4]^=1;
                try{SchemaNativeContract.Verify(UnitTypeIconCompiler.ProofSource(source,schemas,catalog).DocumentElement!,typeof(UnitTypeIcon),0,bad,refs);throw new InvalidOperationException("Corrupt icon import accepted.");}catch(InvalidDataException){}
            }
            // Reborn: source-only bases and definitions feed an icon whose local PackedTextureImage is declared later.
            File.WriteAllText(Path.Combine(output,"base.xml"),"<AssetDeclaration xmlns='uri:ea.com:eala:asset'><UnitTypeIcon id='RebornIconBase' Image='RebornIconImage'><Offset x='0' y='0'/></UnitTypeIcon></AssetDeclaration>");
            string entry=Path.Combine(output,"mod.xml");
            File.WriteAllText(entry,"<AssetDeclaration xmlns='uri:ea.com:eala:asset'><Includes><Include type='instance' source='base.xml'/></Includes><Defines><Define name='DX' value='12.5'/></Defines><UnitTypeIcon id='RebornDerivedIcon' inheritFrom='RebornIconBase'><Offset x='=$DX' y='-3.5'/></UnitTypeIcon><UnitTypeIcon id='RiflemenIcon' Image='UnitTypeIcon_EngineerNew'/><PackedTextureImage id='RebornIconImage' Texture='PackedImages_004' Rotated='false'><Dimensions x='82' y='82'/><Coords x='85' y='1'/><TextureDimensions x='256' y='256'/></PackedTextureImage></AssetDeclaration>");
            string prepared=Path.Combine(output,"prepared");ConscriptSourceInput.Prepare(entry,prepared);string built=Path.Combine(output,"built");if(!ConscriptMultiAssetBuild.TryRun(Path.Combine(prepared,"game.xml"),manifest,global,built))throw new InvalidDataException("Icon source route was not selected.");
            var linked=ManifestReader.Read(File.ReadAllBytes(Directory.GetFiles(Path.Combine(built,"stream-readback"),"*.manifest").Single()));var icon=linked.Assets.Single(a=>a.Name=="UnitTypeIcon:RebornDerivedIcon");var image=linked.Assets.Single(a=>a.Name=="PackedTextureImage:RebornIconImage");
            if(linked.Assets.Count!=3||icon.Tokenized!=0||icon.TypeHash!=0xF9E68B56u||icon.References.Count!=1||icon.References[0].TypeId!=image.TypeId||icon.References[0].InstanceId!=image.InstanceId)throw new InvalidDataException("Icon linked local-image identity or fingerprint differs.");
            Console.WriteLine("Reborn: UnitTypeIcon: PASS (three retail records; edited/omitted images and offsets; source-only inheritance/definitions; local image import; existing-ID override; final stream/package readback; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
