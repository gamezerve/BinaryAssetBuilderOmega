using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.XmlCompiler;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify one explicit source edit and a new identity on the complete Conscript graph, then prepare unlaunched trial candidates.
internal static class GameObjectConscriptTrialSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: recompile original-source edits through actual normalization/allocation and prove the entire graph differs only at the authored BuildTime word. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(XmlDocument baseline,Chunk native,InstanceDeclaration original,Chunk tokens,XmlSchemaSet schemas,ManifestAsset[] catalog,string manifest,string globalManifest,Func<XmlDocument,(Chunk Native,InstanceDeclaration Instance)> build,Action<Chunk,InstanceDeclaration> verifyOriginal)
    {
        if(Word(native.InstanceBuffer,140)!=0x40800000u)throw new InvalidDataException("Conscript original BuildTime anchor is not 4.");
        uint newId=InstanceHandle.GetInstanceId("RebornConscriptTrial");if(catalog.Any(a=>a.TypeId==0x942FFF2Du && a.InstanceId==newId))throw new InvalidDataException("Trial identity collides with a real catalog asset.");
        string sourceSnapshot=baseline.OuterXml;var originalDependencies=original.ReferencedInstances.Select(h=>new AssetId(h.TypeId,h.InstanceId)).ToArray();string root=Path.Combine(Path.GetTempPath(),"Reborn-ConscriptTrials-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(root);int refusals=0;
        foreach(string id in new[]{"SovietAntiInfantryInfantry","RebornConscriptTrial"})
        {
            var source=(XmlDocument)baseline.CloneNode(true);source.DocumentElement!.SetAttribute("id",id);source.DocumentElement.SetAttribute("BuildTime","1");Admit(source);var pair=build(source);Verify(pair.Native,pair.Instance,id);var repeated=build(source);Verify(repeated.Native,repeated.Instance,id);
            var projected=Ep1GameObjectEmptyRootProjection.ComposeOriginal(pair.Native,schemas);var expected=Clone(tokens);Put(expected.InstanceBuffer,88824+140,0x3F800000u);if(!Equal(projected,expected) || !Equal(projected,Ep1GameObjectEmptyRootProjection.ComposeOriginal(repeated.Native,schemas)))throw new InvalidDataException("Authored Conscript metadata/native edit differs.");
            var bad=Clone(pair.Native);bad.InstanceBuffer[192]^=1;Refuse(()=>Verify(bad,pair.Instance,id));refusals++;bad=Clone(pair.Native);Put(bad.InstanceBuffer,140,0x40800000u);Refuse(()=>Verify(bad,pair.Instance,id));refusals++;
            var streams=GameObjectOriginalStreamSmokeTest.Run(projected,pair.Instance,manifest,globalManifest,true);Package(Path.Combine(root,id=="RebornConscriptTrial"?"new-id":"override"),source,streams,id);
        }
        foreach(var change in new[]{("id","UnreviewedConscript"),("BuildTime","2"),("Scale","2")}){var bad=(XmlDocument)baseline.CloneNode(true);bad.DocumentElement!.SetAttribute("BuildTime","1");bad.DocumentElement.SetAttribute(change.Item1,change.Item2);Refuse(()=>Admit(bad));refusals++;}
        var added=(XmlDocument)baseline.CloneNode(true);added.DocumentElement!.SetAttribute("BuildTime","1");added.DocumentElement.AppendChild(added.CreateElement("UnreviewedChild",baseline.DocumentElement!.NamespaceURI));Refuse(()=>Admit(added));refusals++;
        if(baseline.OuterXml!=sourceSnapshot)throw new InvalidDataException("Authored trial mutated original source.");Console.WriteLine($"Conscript authored trials: PASS (two source-compiled identities, BuildTime 4 -> 1, 49 modules/six owners/geometry unchanged, 275 references, exact edited metadata/native graphs, {refusals} source/native refusals; candidates={root}; no install, launch, recruitable new-ID or gameplay proof).");
        //-------------------------------------------------------------------------------------------------
        /** Reborn: only the two literal identities and BuildTime=1 may differ from this already qualified captured source; unknown changes are never silently compiled. */
        //-------------------------------------------------------------------------------------------------
        void Admit(XmlDocument input){var copy=(XmlDocument)input.CloneNode(true);string id=copy.DocumentElement!.GetAttribute("id");if(id is not ("SovietAntiInfantryInfantry" or "RebornConscriptTrial") || copy.DocumentElement.GetAttribute("BuildTime")!="1")throw new InvalidDataException("Unreviewed Conscript source edit.");copy.DocumentElement.SetAttribute("id",baseline.DocumentElement!.GetAttribute("id"));copy.DocumentElement.SetAttribute("BuildTime",baseline.DocumentElement.GetAttribute("BuildTime"));if(copy.OuterXml!=sourceSnapshot)throw new InvalidDataException("Additional Conscript source changes are outside this trial.");}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: restore exactly one independently anchored scalar before reusing complete original native ownership; all other bytes, auxiliary order and dependency identities remain unchanged. */
        //-------------------------------------------------------------------------------------------------
        void Verify(Chunk value,InstanceDeclaration instance,string id){if(instance.Handle.Name!="GameObject:"+id || instance.Handle.InstanceId!=InstanceHandle.GetInstanceId(id) || !instance.ReferencedInstances.Select(h=>new AssetId(h.TypeId,h.InstanceId)).SequenceEqual(originalDependencies) || Word(value.InstanceBuffer,140)!=0x3F800000u)throw new InvalidDataException("Authored identity/reference/BuildTime differs.");var restored=Clone(value);Put(restored.InstanceBuffer,140,0x40800000u);if(!Equal(restored,native))throw new InvalidDataException("Unreviewed authored native delta.");verifyOriginal(restored,instance);}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: reuse only low-level BIG packing, not armor admission; independently recover every GameObject stream variant and CRLF marker before exposing a fresh candidate. */
    //-------------------------------------------------------------------------------------------------
    private static void Package(string directory,XmlDocument source,Dictionary<string,byte[]> streams,string id)
    {
        string[] extensions={".manifest",".bin",".relo",".imp"};byte[][] payloads=extensions.Select(e=>streams["diagnostic"+e]).ToArray();byte[] archive=ArmorDiagnosticPackage.Serialize(payloads,true,true);const string config="add-big RebornConscriptProbe.big\n";var files=new Dictionary<string,byte[]>{["RebornConscriptProbe.big"]=archive,["reborn_1.0.cfg"]=Encoding.ASCII.GetBytes(config),["reborn_1.0.skudef"]=Encoding.ASCII.GetBytes(config),["trial-source.xml"]=Encoding.UTF8.GetBytes(source.OuterXml),["DO_NOT_LAUNCH_YET.txt"]=Encoding.UTF8.GetBytes("Reborn: unlaunched Conscript candidate only. Mod SKU fragment, not stock SKU replacement. New ID has no CommandButton/CommandSet/spawn integration. Separate runtime preflight and game-test authorization required.\n")};
        ModifierFXStreamSmokeTest.WriteNew(directory,files);Verify();Refuse(()=>ModifierFXStreamSmokeTest.WriteNew(directory,files));int corruptions=0;
        foreach(string name in files.Keys){byte[] bad=files[name].ToArray();bad[0]^=1;string path=Path.Combine(directory,name);try{File.WriteAllBytes(path,bad);Refuse(Verify);corruptions++;}finally{File.WriteAllBytes(path,files[name]);}}Verify();
        var receipt=new{Asset="GameObject:"+id,BuildTime=1,StreamProfile="bounded-conscript-trial",MetadataPrefix=88824,ReferenceOccurrences=275,ArchiveEntryCount=15,ArchiveSha256=Convert.ToHexString(SHA256.HashData(archive)),SourceSha256=Convert.ToHexString(SHA256.HashData(files["trial-source.xml"])),OfflineValidated=true,PackageCorruptionsRejected=corruptions,NewIdentity=id=="RebornConscriptTrial",RecruitableNewIdentity=false,GameExecuted=false,InstalledFilesModified=false,RuntimeReady=false,InstanceHashPolicy="experimental-normalized-source",ConfigPath=Path.Combine(directory,"reborn_1.0.cfg"),NextGate="Controlled runtime preflight and override gameplay test; new identity also requires explicit recruitment/spawn integration."};File.WriteAllText(Path.Combine(directory,"trial-plan.json"),JsonSerializer.Serialize(receipt,new JsonSerializerOptions{WriteIndented=true}));
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
