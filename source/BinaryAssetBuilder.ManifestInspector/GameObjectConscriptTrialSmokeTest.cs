using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
// Reborn: share the exact official Core schema helpers with the bounded recruitment command compiler.
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify six bounded source scalars and a diagnostic new identity on the complete Conscript graph, then prepare unlaunched trial candidates.
internal static class GameObjectConscriptTrialSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: recompile original-source edits through actual normalization/allocation and prove the entire graph differs only at six reviewed words. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(XmlDocument baseline,Chunk native,InstanceDeclaration original,Chunk tokens,SchemaSet sourceSchemas,ManifestAsset[] catalog,string manifest,string globalManifest,Func<XmlDocument,(Chunk Native,InstanceDeclaration Instance)> build,Action<Chunk,InstanceDeclaration> verifyOriginal,string? alphaSource=null,string? alphaOutput=null)
    {
        // Reborn: qualify the stock-matching construction-command marshaler before exposing any new-ID recruitment package.
        var schemas=sourceSchemas.Schemas;ConscriptRecruitmentSmokeTest.Qualify(sourceSchemas,manifest);
        if(Word(native.InstanceBuffer,140)!=0x40800000u)throw new InvalidDataException("Conscript original BuildTime anchor is not 4.");
        uint newId=InstanceHandle.GetInstanceId("RebornConscriptTrial");if(catalog.Any(a=>a.TypeId==0x942FFF2Du && a.InstanceId==newId))throw new InvalidDataException("Trial identity collides with a real catalog asset.");
        // Reborn: selected XML is safely parsed and compared to the complete captured source; only the original identity and six scalars enter Studio.
        XmlDocument? selected=null;XmlDocument? authoredCommand=null;
        if(alphaSource!=null)
        {
            if(alphaOutput==null || Directory.Exists(alphaOutput) || File.Exists(alphaOutput))throw new InvalidDataException("Fresh Conscript alpha output required.");
            var settings=new XmlReaderSettings{DtdProcessing=DtdProcessing.Prohibit,XmlResolver=null,MaxCharactersInDocument=1048576};
            using var reader=XmlReader.Create(alphaSource,settings);var input=new XmlDocument{PreserveWhitespace=true,XmlResolver=null};input.Load(reader);
            selected=new XmlDocument{PreserveWhitespace=true,XmlResolver=null};selected.LoadXml(input.DocumentElement?.OuterXml??throw new InvalidDataException("Conscript root absent."));
            string id=selected.DocumentElement!.GetAttribute("id");if(!ConscriptSourceInput.Identity(id) || id!="SovietAntiInfantryInfantry" && catalog.Any(a=>a.TypeId==0x942FFF2D && a.InstanceId==InstanceHandle.GetInstanceId(id)))throw new InvalidDataException("Custom GameObject ID is invalid or collides with another stock GameObject.");
            // Reborn: only a command explicitly captured by source preparation is compiled; new IDs alone never synthesize one.
            string commandPath=Path.Combine(Path.GetDirectoryName(alphaSource)!,"command.xml");if(File.Exists(commandPath)){using var commandReader=XmlReader.Create(commandPath,settings);authoredCommand=new XmlDocument{PreserveWhitespace=true,XmlResolver=null};authoredCommand.Load(commandReader);}
        }
        // Reborn: all unedited bytes retain their complete stock oracle; only six independently located words may differ.
        int[] scalarOffsets=ConscriptEditableScalars.Offsets(native.InstanceBuffer);
        string sourceSnapshot=baseline.OuterXml;var originalDependencies=original.ReferencedInstances.Select(h=>new AssetId(h.TypeId,h.InstanceId)).ToArray();string root=alphaOutput??Path.Combine(Path.GetTempPath(),"Reborn-ConscriptTrials-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(root);int refusals=0;
        foreach(string id in selected==null?new[]{"SovietAntiInfantryInfantry","RebornConscriptTrial"}:new[]{selected.DocumentElement!.GetAttribute("id")})
        {
            var source=selected??(XmlDocument)baseline.CloneNode(true);if(selected==null){source.DocumentElement!.SetAttribute("id",id);source.DocumentElement.SetAttribute("BuildTime","1");}Admit(source);var pair=build(source);Verify(pair.Native,pair.Instance,id,source);var repeated=build(source);Verify(repeated.Native,repeated.Instance,id,source);
            var projected=Ep1GameObjectEmptyRootProjection.ComposeOriginal(pair.Native,schemas);var expected=Expected(source);if(!Equal(projected,expected) || !Equal(projected,Ep1GameObjectEmptyRootProjection.ComposeOriginal(repeated.Native,schemas)))throw new InvalidDataException("Authored Conscript metadata/native edit differs.");
            var bad=Clone(pair.Native);bad.InstanceBuffer[192]^=1;Refuse(()=>Verify(bad,pair.Instance,id,source));refusals++;
            foreach(int offset in scalarOffsets){bad=Clone(pair.Native);bad.InstanceBuffer[offset]^=1;Refuse(()=>Verify(bad,pair.Instance,id,source));refusals++;}
            var streams=GameObjectOriginalStreamSmokeTest.Run(projected,pair.Instance,manifest,globalManifest,true);
            if(authoredCommand!=null)streams=ConscriptRecruitmentSmokeTest.Compose(streams,pair.Instance,sourceSchemas,manifest,globalManifest,authoredCommand);
            Package(Path.Combine(root,id!="SovietAntiInfantryInfantry"?"new-id":"override"),source,streams,id);
        }
        foreach(var change in new[]{("id","Invalid Conscript"),("BuildTime","NaN"),("BuildTime","not-a-real"),("UnknownRootField","2")}){var bad=(XmlDocument)baseline.CloneNode(true);bad.DocumentElement!.SetAttribute("BuildTime","1");bad.DocumentElement.SetAttribute(change.Item1,change.Item2);Refuse(()=>Admit(bad));refusals++;}
        // Reborn: independently compile each authored word and the combined profile; compare the entire projected graph, not only six floats.
        foreach(int selectedField in Enumerable.Range(0,6).Concat(new[]{-1,-2,-3}))
        {
            var probe=(XmlDocument)baseline.CloneNode(true);float[] values={2,25,500,350,650,90};
            for(int i=0;i<6;i++)if(selectedField<0 || selectedField==i){var field=ConscriptEditableScalars.Fields[i];float value=selectedField==-2?field.Min:selectedField==-3?field.Max:values[i];ConscriptEditableScalars.Element(probe,field).SetAttribute(field.Attribute,value.ToString(System.Globalization.CultureInfo.InvariantCulture));}
            Admit(probe);var pair=build(probe);Verify(pair.Native,pair.Instance,"SovietAntiInfantryInfantry",probe);
            if(!Equal(Expected(probe),Ep1GameObjectEmptyRootProjection.ComposeOriginal(pair.Native,schemas)))throw new InvalidDataException("Conscript scalar matrix metadata differs.");
        }
        foreach(var field in ConscriptEditableScalars.Fields)foreach(string invalid in new[]{"NaN","Infinity","-Infinity","not-a-number"}){var bad=(XmlDocument)baseline.CloneNode(true);ConscriptEditableScalars.Element(bad,field).SetAttribute(field.Attribute,invalid);Refuse(()=>Admit(bad));refusals++;}
        // Reborn: all existing primitive processor descriptors enter one source/native/metadata test; schema rules replace the former six-field business whitelist.
        var rootProbe=(XmlDocument)baseline.CloneNode(true);GameObjectScalarContract.AuthorProbe(rootProbe);Admit(rootProbe);var scalarPair=build(rootProbe);Verify(scalarPair.Native,scalarPair.Instance,"SovietAntiInfantryInfantry",rootProbe);
        if(!Equal(Expected(rootProbe),Ep1GameObjectEmptyRootProjection.ComposeOriginal(scalarPair.Native,schemas)))throw new InvalidDataException("Grouped root primitive metadata differs.");
        foreach(var binding in GameObjectScalarContract.Bindings){var corrupted=Clone(scalarPair.Native);corrupted.InstanceBuffer[binding.Native.Offset]^=1;Refuse(()=>Verify(corrupted,scalarPair.Instance,"SovietAntiInfantryInfantry",rootProbe));refusals++;}
        // Reborn: zero/default BuildTime and values beyond historical alpha caps are schema-representable inputs, not automatic authoring errors.
        var uncapped=(XmlDocument)baseline.CloneNode(true);uncapped.DocumentElement!.SetAttribute("BuildTime","0");uncapped.DocumentElement.SetAttribute("Scale","2");ConscriptEditableScalars.Element(uncapped,ConscriptEditableScalars.Fields[3]).SetAttribute("VisionRange","6000");Admit(uncapped);var uncappedPair=build(uncapped);Verify(uncappedPair.Native,uncappedPair.Instance,"SovietAntiInfantryInfantry",uncapped);
        uncapped.DocumentElement.RemoveAttribute("BuildTime");uncapped.DocumentElement.SetAttribute("IsTrainable","1");Admit(uncapped);var defaultPair=build(uncapped);Verify(defaultPair.Native,defaultPair.Instance,"SovietAntiInfantryInfantry",uncapped);
        Console.WriteLine("GameObject primitive capability: PASS ("+GameObjectScalarContract.Bindings.Count+" schema/ABI root descriptors; grouped native/metadata codecs; uncapped/default real values; topology/references retained).");
        var added=(XmlDocument)baseline.CloneNode(true);added.DocumentElement!.SetAttribute("BuildTime","1");added.DocumentElement.AppendChild(added.CreateElement("UnreviewedChild",baseline.DocumentElement!.NamespaceURI));Refuse(()=>Admit(added));refusals++;
        if(baseline.OuterXml!=sourceSnapshot)throw new InvalidDataException("Authored trial mutated original source.");Console.WriteLine($"Conscript authored trials: PASS (source-compiled identities={(selected==null?2:1)}, schema/ABI root primitives and existing nested numeric owners; grouped source/native/metadata controls, 49 modules/six owners/geometry retained, 275 references, {refusals} source/native refusals; candidates={root}; offline recruitment candidate only; no install, launch or gameplay proof).");
        //-------------------------------------------------------------------------------------------------
        /** Reborn: only two diagnostic identities and six bounded numeric fields may differ from the captured source; unknown changes never compile silently. */
        //-------------------------------------------------------------------------------------------------
        void Admit(XmlDocument input)=>ConscriptSourceInput.Admit(input,(XmlDocument)baseline.CloneNode(true),schemas);
        //-------------------------------------------------------------------------------------------------
        /** Reborn: restore six independently anchored scalars before reusing original native ownership; every other byte and auxiliary/dependency order stays unchanged. */
        //-------------------------------------------------------------------------------------------------
        void Verify(Chunk value,InstanceDeclaration instance,string id,XmlDocument source){if(instance.Handle.Name!="GameObject:"+id || instance.Handle.InstanceId!=InstanceHandle.GetInstanceId(id) || !instance.ReferencedInstances.Select(h=>new AssetId(h.TypeId,h.InstanceId)).SequenceEqual(originalDependencies))throw new InvalidDataException("Authored identity/reference differs.");var restored=Clone(value);GameObjectScalarContract.VerifyAndRestore(source,value.InstanceBuffer,restored.InstanceBuffer,native.InstanceBuffer);for(int i=1;i<scalarOffsets.Length;i++){int offset=scalarOffsets[i];if(Word(value.InstanceBuffer,offset)!=ConscriptEditableScalars.Bits(source,ConscriptEditableScalars.Fields[i]))throw new InvalidDataException("Conscript scalar native value differs: "+ConscriptEditableScalars.Fields[i].Attribute);Put(restored.InstanceBuffer,offset,Word(native.InstanceBuffer,offset));}if(!Equal(restored,native))throw new InvalidDataException("Unreviewed authored native delta.");verifyOriginal(restored,instance);}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: expected metadata keeps the entire independently captured stock graph except the six reviewed native value locations. */
        //-------------------------------------------------------------------------------------------------
        Chunk Expected(XmlDocument source){var expected=Clone(tokens);GameObjectScalarContract.Patch(source,expected.InstanceBuffer,88824);for(int i=1;i<scalarOffsets.Length;i++)Put(expected.InstanceBuffer,88824+scalarOffsets[i],ConscriptEditableScalars.Bits(source,ConscriptEditableScalars.Fields[i]));return expected;}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: reuse only low-level BIG packing, not armor admission; independently recover every GameObject stream variant and CRLF marker before exposing a fresh candidate. */
    //-------------------------------------------------------------------------------------------------
    private static void Package(string directory,XmlDocument source,Dictionary<string,byte[]> streams,string id)
    {
        string[] extensions={".manifest",".bin",".relo",".imp"};byte[][] payloads=extensions.Select(e=>streams["diagnostic"+e]).ToArray();byte[] archive=ArmorDiagnosticPackage.Serialize(payloads,true,true);const string config="add-big RebornConscriptProbe.big\n";var files=new Dictionary<string,byte[]>{["RebornConscriptProbe.big"]=archive,["reborn_1.0.cfg"]=Encoding.ASCII.GetBytes(config),["reborn_1.0.skudef"]=Encoding.ASCII.GetBytes(config),["trial-source.xml"]=Encoding.UTF8.GetBytes(source.OuterXml),["DO_NOT_LAUNCH_YET.txt"]=Encoding.UTF8.GetBytes("Reborn: unlaunched Conscript candidate only. Mod SKU fragment, not stock SKU replacement. New-ID trial redirects the existing Conscript UNIT_BUILD command; all other Barracks choices/CommandSet remain stock. Separate runtime preflight and game-test authorization required.\n")};
        ModifierFXStreamSmokeTest.WriteNew(directory,files);Verify();Refuse(()=>ModifierFXStreamSmokeTest.WriteNew(directory,files));int corruptions=0;
        foreach(string name in files.Keys){byte[] bad=files[name].ToArray();bad[0]^=1;string path=Path.Combine(directory,name);try{File.WriteAllBytes(path,bad);Refuse(Verify);corruptions++;}finally{File.WriteAllBytes(path,files[name]);}}Verify();
        var receipt=new{Asset="GameObject:"+id,BuildTime=ConscriptEditableScalars.Read(source,ConscriptEditableScalars.Fields[0]),EditableValues=ConscriptEditableScalars.Fields.ToDictionary(f=>f.Attribute,f=>ConscriptEditableScalars.Read(source,f)),StreamProfile="conscript-explicit-source",AssetCount=ManifestReader.Read(streams["diagnostic.manifest"]).Assets.Count,AutomaticCommandGenerated=false,CommandOverride=ManifestReader.Read(streams["diagnostic.manifest"]).Assets.Skip(1).Select(a=>a.Name).FirstOrDefault(),MetadataPrefix=88824,ReferenceOccurrences=275,ArchiveEntryCount=15,ArchiveSha256=Convert.ToHexString(SHA256.HashData(archive)),SourceSha256=Convert.ToHexString(SHA256.HashData(files["trial-source.xml"])),OfflineValidated=true,PackageCorruptionsRejected=corruptions,NewIdentity=id!="SovietAntiInfantryInfantry",RecruitableNewIdentity=id!="SovietAntiInfantryInfantry" && ManifestReader.Read(streams["diagnostic.manifest"]).Assets.Any(a=>a.Name=="LogicCommand:"+ConscriptRecruitmentSmokeTest.CommandId),GameExecuted=false,InstalledFilesModified=false,RuntimeReady=false,InstanceHashPolicy="experimental-normalized-source",ConfigPath=Path.Combine(directory,"reborn_1.0.cfg"),NextGate="Controlled runtime preflight and override gameplay test; new-ID trial reuses the existing Conscript production slot; verify actual newly created identity in game."};File.WriteAllText(Path.Combine(directory,"trial-plan.json"),JsonSerializer.Serialize(receipt,new JsonSerializerOptions{WriteIndented=true}));
        //-------------------------------------------------------------------------------------------------
        /** Reborn: verify exact prepared config/source/notice bytes, independent BIG header/index geometry and all fifteen recovered variant payloads. */
        //-------------------------------------------------------------------------------------------------
        void Verify(){foreach(var file in files)if(!File.ReadAllBytes(Path.Combine(directory,file.Key)).SequenceEqual(file.Value))throw new InvalidDataException("Conscript trial prepared file differs.");string path=Path.Combine(directory,"RebornConscriptProbe.big");byte[] stored=File.ReadAllBytes(path);if(Encoding.ASCII.GetString(stored,0,4)!="BIG4" || Word(stored,4)!=stored.Length || BinaryPrimitives.ReadUInt32BigEndian(stored.AsSpan(8,4))!=15)throw new InvalidDataException("Conscript trial BIG header differs.");using var big=BigArchive.Open(path,15,4096);long at=BinaryPrimitives.ReadUInt32BigEndian(stored.AsSpan(12,4));int index=0;foreach(string variant in new[]{"","_l","_m"})foreach(string extension in extensions.Append(".version")){var entry=big.Entries[index++];byte[] expected=extension==".version"?Encoding.ASCII.GetBytes("\r\n"):payloads[Array.IndexOf(extensions,extension)];if(entry.Name!="data\\mod"+variant+extension || entry.Offset!=at || entry.StoredSize!=expected.Length || !big.ReadEntry(entry,1048576).SequenceEqual(expected))throw new InvalidDataException("Conscript trial variant differs.");at+=entry.StoredSize;}if(at!=stored.Length)throw new InvalidDataException("Conscript trial unowned archive suffix.");}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: explicit refusal controls never count a successful corrupt candidate as rejection. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action){try{action();}catch(InvalidDataException){return;}throw new InvalidDataException("Conscript trial corruption accepted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: complete comparisons include native and auxiliary bytes/order. */
    //-------------------------------------------------------------------------------------------------
    private static bool Equal(Chunk a,Chunk b)=>a.InstanceBuffer.SequenceEqual(b.InstanceBuffer) && a.RelocationBuffer.SequenceEqual(b.RelocationBuffer) && a.ImportsBuffer.SequenceEqual(b.ImportsBuffer);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: restoration and corruption controls own their buffers. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c)=>new(){InstanceBuffer=c.InstanceBuffer.ToArray(),RelocationBuffer=c.RelocationBuffer.ToArray(),ImportsBuffer=c.ImportsBuffer.ToArray()};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: bounded scalar inspection cannot escape the qualified graph. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at){if(at<0 || at>b.Length-4)throw new InvalidDataException("Conscript trial word bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: restore or author only a specifically reviewed word in an owned copy. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
