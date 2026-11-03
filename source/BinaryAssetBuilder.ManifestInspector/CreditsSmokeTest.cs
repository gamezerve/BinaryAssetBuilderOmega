using System.Buffers.Binary;
using System.Reflection;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify the real credits graph and compile a read-only copy of authored credits into a private global package.
internal static class CreditsSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: cover retail fonts/colors/strings, authored scrolling and all text entries, empty lists and final stream readback. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global,string source,string output)
    {
        if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh credits evidence directory required.");Directory.CreateDirectory(output);
        var saved=Settings.Current;
        try{
            Settings.Current=new Settings{SchemaPath=ReferencePipelineSmokeTest.FindFixture(),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<System.Xml.Schema.XmlSchema>().ToArray())schemas.Schemas.Remove(seed);
            schemas.Schemas.Add(ConscriptSourceInput.Schemas());schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=ManifestReader.Read(File.ReadAllBytes(manifest)).Assets.Concat(ManifestReader.Read(File.ReadAllBytes(global)).Assets).ToArray();
            CreditsCompiler.Qualify(global,schemas,catalog);
            var authored=SdkXmlDiagnostics.Parse(SdkEnvironmentPreflight.Read(source,4*1048576));var root=authored.GetElementsByTagName("Credits",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
            var xml=new XmlDocument();xml.AppendChild(xml.ImportNode(root,true));
            CreditsCompiler.NormalizeFormatting(xml.DocumentElement!);
            var native=CreditsCompiler.Compile(xml.DocumentElement!);var proof=SchemaNativeContract.Resolve(xml,schemas,catalog);proof.DocumentElement!.RemoveAttribute("id");
            SchemaNativeContract.Verify(proof.DocumentElement,typeof(Credits),0,native,Array.Empty<AssetId>());
            int items=root.ChildNodes.OfType<XmlElement>().Count(e=>e.LocalName=="CreditItem"),texts=root.GetElementsByTagName("Text",SchemaSet.XmlNamespace).Count;
            if(BinaryPrimitives.ReadUInt32LittleEndian(native.InstanceBuffer.AsSpan(24,4))!=items||native.ImportsBuffer.Length!=0)throw new InvalidDataException("Authored credits item count/imports differ.");
            var empty=(XmlElement)root.CloneNode(true);foreach(var item in empty.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName is "Style" or "CreditItem").ToArray())empty.RemoveChild(item);
            var emptyNative=CreditsCompiler.Compile(empty);if(BinaryPrimitives.ReadUInt32LittleEndian(emptyNative.InstanceBuffer.AsSpan(16,4))!=0||BinaryPrimitives.ReadUInt32LittleEndian(emptyNative.InstanceBuffer.AsSpan(24,4))!=0)throw new InvalidDataException("Credits empty arrays differ.");
            string entry=Path.Combine(output,"MapMetaData_Global.xml");File.WriteAllBytes(entry,File.ReadAllBytes(source));string prepared=Path.Combine(output,"prepared");ConscriptSourceInput.Prepare(entry,prepared);
            if(!ConscriptMultiAssetBuild.TryRun(Path.Combine(prepared,"game.xml"),manifest,global,Path.Combine(output,"built"),earlyGlobal:true))throw new InvalidDataException("Credits global route not selected.");
            string path=Directory.GetFiles(Path.Combine(output,"built","stream-readback"),"*.manifest").Single();var linked=ManifestReader.Read(File.ReadAllBytes(path));var asset=linked.Assets.Single(a=>a.Name=="Credits:"+root.GetAttribute("id"));
            if(asset.TypeHash!=0x10D657EEu||asset.Tokenized!=0||asset.References.Count!=0||!W3DContainerCompiler.ReadStock(path,linked,asset).SequenceEqual(native.InstanceBuffer))throw new InvalidDataException("Credits stream fingerprint/payload differs.");
            Console.WriteLine($"Reborn: Credits PASS (complete retail graph; {items} authored credit items/{texts} text lines, empty lists and early-global package/readback; no user build or game).");
        }finally{Settings.Current=saved;}
    }
}
