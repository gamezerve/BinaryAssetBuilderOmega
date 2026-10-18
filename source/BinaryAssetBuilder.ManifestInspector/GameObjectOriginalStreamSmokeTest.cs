using System.Buffers.Binary;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Utility;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: serialize only the independently qualified original Conscript to fresh diagnostic streams, never install or launch them.
internal static class GameObjectOriginalStreamSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: real catalog identities retain every normalized occurrence, a diagnostic source hash, actual Core checksum packing and independent stream/selector readback. */
    //-------------------------------------------------------------------------------------------------
    internal static Dictionary<string,byte[]> Run(Chunk qualified,InstanceDeclaration original,string staticManifest,string globalManifest,bool authoredTrial=false)
    {
        string[] paths={Path.GetFullPath(staticManifest),Path.GetFullPath(globalManifest)},runtimeNames={"data\\static.manifest","data\\global.manifest"};var snapshots=paths.Select(p=>SdkEnvironmentPreflight.Read(p,64*1048576)).ToArray();var catalogs=snapshots.Select(b=>ManifestReader.Read(b)).ToArray();
        foreach(var catalog in catalogs){TypeRegistryAudit.ValidateTarget(catalog.Header.Version,catalog.Header.AllTypesHash);if(!catalog.Header.IsLinked || catalog.Validate().Count!=0 || catalog.ReferencedManifests.Any(r=>r.IsPatch))throw new InvalidDataException("Original stream external catalog is not a plain valid EP1 stream.");}
        var dependencies=original.ReferencedInstances.Select(h=>new AssetId(h.TypeId,h.InstanceId)).ToArray();ValidateDependencies(dependencies,catalogs);if(dependencies.Length!=275)throw new InvalidDataException("Original stream occurrence count differs.");
        // Reborn: the normalization-only source has no processed document context; attach an explicitly diagnostic source identity, not an invented full EA processed-document receipt.
        var document=new AssetDeclarationDocument();Type stateType=typeof(AssetDeclarationDocument).GetNestedType("CurrentState",BindingFlags.NonPublic)!;object state=Activator.CreateInstance(stateType,true)!;stateType.GetField("SourcePath")!.SetValue(state,"OriginalConscript.normalized.xml");typeof(AssetDeclarationDocument).GetField("_current",BindingFlags.NonPublic|BindingFlags.Instance)!.SetValue(document,state);
        var instance=new InstanceDeclaration(document){XmlNode=original.XmlNode.CloneNode(true)};instance.ReferencedInstances.AddRange(original.ReferencedInstances);instance.ValidatedReferencedInstances=new(original.ReferencedInstances);instance.Handle.TypeHash=0xCEB9FE36u;instance.Handle.InstanceHash=FastHash.GetHashCode(Encoding.UTF8.GetBytes(original.XmlNode.OuterXml));if(instance.Handle.InstanceHash==0)throw new InvalidDataException("Diagnostic original source hash is zero.");
        var chunk=new AssetBuffer{InstanceData=qualified.InstanceBuffer.ToArray(),RelocationData=qualified.RelocationBuffer.ToArray(),ImportsData=qualified.ImportsBuffer.ToArray()};var ordered=new[]{instance};var chunks=new[]{chunk};var flags=new[]{true};
        uint checksum=(uint)typeof(AssetDeclarationDocument).GetMethod("ComputeOutputChecksum",BindingFlags.NonPublic|BindingFlags.Static)!.Invoke(null,new object[]{ordered})!;var packed=new byte[256];uint[] identity={instance.Handle.TypeId,instance.Handle.TypeHash,instance.Handle.InstanceId,instance.Handle.InstanceHash,(uint)dependencies.Length};for(int i=0;i<identity.Length;i++)Put(packed,i*4,identity[i]);if(checksum!=FastHash.GetHashCode(packed))throw new InvalidDataException("Original stream independent checksum packing differs.");
        var runtime=new ReferencedFileBuffer();typeof(OutputManager).GetMethod("AddExternalManifestReferences",BindingFlags.NonPublic|BindingFlags.Static)!.Invoke(null,new object[]{runtime,paths,string.Join(';',runtimeNames)});
        int refusals=0;Refuse(()=>BoundedDiagnosticBuild.Serialize(ordered,chunks,checksum,runtime,flags));refusals++;
        var payloads=Serialize(instance,chunk);payloads["DIAGNOSTIC_ONLY.txt"]=Encoding.UTF8.GetBytes("Reborn: bounded Conscript diagnostic stream only; normalized-source hash is experimental, not full EA processing identity. External native payloads are not validated/copied. No gameplay or general GameObject admission.\n");
        string root=Path.Combine(Path.GetTempPath(),"Reborn-OriginalConscriptStream-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(root);string first=Path.Combine(root,"first");ModifierFXStreamSmokeTest.WriteNew(first,payloads);Verify(first,payloads);var repeat=Serialize(instance,chunk);repeat["DIAGNOSTIC_ONLY.txt"]=payloads["DIAGNOSTIC_ONLY.txt"].ToArray();if(payloads.Any(p=>!p.Value.SequenceEqual(repeat[p.Key])))throw new InvalidDataException("Original stream serialization is nondeterministic.");string second=Path.Combine(root,"repeat");ModifierFXStreamSmokeTest.WriteNew(second,repeat);Verify(second,repeat);Refuse(()=>ModifierFXStreamSmokeTest.WriteNew(first,payloads));refusals++;
        // Reborn: replace the byte-snapshot expectation too, forcing semantic/header/readback checks rather than trivial saved-file equality to reject corrupt streams.
        foreach(string name in new[]{"diagnostic.bin","diagnostic.relo","diagnostic.imp","diagnostic.manifest"})
        {
            byte[] bytes=payloads[name];foreach(var bad in new[]{Flip(bytes,0),Flip(bytes,4),bytes[..^1],bytes.Concat(new byte[]{0xA5}).ToArray(),Flip(bytes,Math.Min(60,bytes.Length-1))}){Corrupt(name,bad);refusals++;}
        }
        // Reborn: every serialized reference occurrence and import selector is checked against the original normalized source, not merely manifest self-consistency.
        int refs=4+48+48;for(int i=0;i<dependencies.Length;i++){var bad=payloads["diagnostic.manifest"].ToArray();bad[refs+i*8]^=1;Corrupt("diagnostic.manifest",bad);refusals++;}
        foreach(uint slot in Slots(chunk.ImportsData)){var bad=payloads["diagnostic.bin"].ToArray();Put(bad,8+(int)slot,0);Corrupt("diagnostic.bin",bad);refusals++;}
        var missing=catalogs.Select(c=>c with{Assets=c.Assets.Where(a=>a.TypeId!=dependencies[0].TypeId || a.InstanceId!=dependencies[0].InstanceId).ToArray()}).ToArray();Refuse(()=>ValidateDependencies(dependencies,missing));refusals++;
        // Reborn: reject ambiguous catalogs, relabeled tokenization, changed fixed identities and truncated qualified chunks without widening default dispatch.
        Refuse(()=>ValidateDependencies(dependencies,catalogs.Concat(catalogs).ToArray()));refusals++;
        Refuse(()=>BoundedDiagnosticBuild.Verify(first,ordered,chunks,payloads,checksum,runtimeNames,new[]{false}));refusals++;
        var wrongXml=original.XmlNode.CloneNode(true);wrongXml.Attributes!["id"]!.Value="RebornUnadmittedConscript";var wrongIdentity=new InstanceDeclaration(document){XmlNode=wrongXml,ValidatedReferencedInstances=new(original.ReferencedInstances)};wrongIdentity.ReferencedInstances.AddRange(original.ReferencedInstances);wrongIdentity.Handle.TypeHash=instance.Handle.TypeHash;wrongIdentity.Handle.InstanceHash=instance.Handle.InstanceHash;Refuse(()=>Serialize(wrongIdentity,chunk));refusals++;
        byte[] savedData=chunk.InstanceData;try{chunk.InstanceData=savedData[..^1];Refuse(()=>Serialize(instance,chunk));refusals++;}finally{chunk.InstanceData=savedData;}
        var savedReferences=instance.ValidatedReferencedInstances;try{instance.ValidatedReferencedInstances=savedReferences.AsEnumerable().Reverse().ToList();Refuse(()=>Serialize(instance,chunk));refusals++;}finally{instance.ValidatedReferencedInstances=savedReferences;}
        foreach(uint hash in new[]{0u,0xCEB9FE37u}){uint saved=instance.Handle.TypeHash;try{instance.Handle.TypeHash=hash;Refuse(()=>Serialize(instance,chunk));refusals++;}finally{instance.Handle.TypeHash=saved;}}
        for(int i=0;i<paths.Length;i++)if(!SHA256.HashData(snapshots[i]).SequenceEqual(SHA256.HashData(SdkEnvironmentPreflight.Read(paths[i],64*1048576))))throw new InvalidDataException("Original external manifest changed during stream proof.");Verify(first,payloads);
        Console.WriteLine($"Conscript {(authoredTrial?"authored":"original")} stream: PASS ({instance.Handle.Name}; one tokenized GameObject, 275 ordered external reference occurrences, two real metadata/runtime mappings, independent Core checksum and Manifest/Utility readback; {refusals} stream/dependency/profile refusals; deterministic fresh outputs; diagnostic={Path.Combine(first,"diagnostic.manifest")}; no external native closure, full processed-document identity or game execution).");
        // Reborn: expose only stream bytes that have completed every readback and refusal control, so the separate trial packager cannot bypass this gate.
        return payloads;
        //-------------------------------------------------------------------------------------------------
        /** Reborn: preserve original-stock serializer admission while explicitly selecting the separate two-identity authored profile. */
        //-------------------------------------------------------------------------------------------------
        Dictionary<string,byte[]> Serialize(InstanceDeclaration declaration,AssetBuffer buffer)=>authoredTrial?BoundedDiagnosticBuild.SerializeConscriptTrial(declaration,buffer,checksum,runtime):BoundedDiagnosticBuild.SerializeOriginalConscript(declaration,buffer,checksum,runtime);
        //-------------------------------------------------------------------------------------------------
        /** Reborn: independent source selectors, stock root directory shape and native payload readback precede trusting a serialized diagnostic. */
        //-------------------------------------------------------------------------------------------------
        void Verify(string directory,Dictionary<string,byte[]> expected)
        {
            // Reborn: the reader admits bounded trailing bytes for forensic use; a freshly serialized diagnostic must end exactly at its declared source-name buffer.
            byte[] stored=File.ReadAllBytes(Path.Combine(directory,"diagnostic.manifest"));var parsed=ManifestReader.Read(stored);var header=parsed.Header;
            if(stored.Length!=4L+48+48L*header.AssetCount+header.AssetReferenceBufferSize+header.ReferenceManifestNameBufferSize+header.AssetNameBufferSize+header.SourceFileNameBufferSize)throw new InvalidDataException("Original stream manifest extent differs.");
            BoundedDiagnosticBuild.Verify(directory,ordered,chunks,expected,checksum,runtimeNames,flags);var metadata=ManifestReader.Read(File.ReadAllBytes(Path.Combine(directory,"diagnostic.manifest")));if(!metadata.Assets.Single().References.SequenceEqual(dependencies))throw new InvalidDataException("Original stream source reference order differs.");byte[] bin=File.ReadAllBytes(Path.Combine(directory,"diagnostic.bin"));if(GameObjectStockReview.Tokens(bin.AsSpan(8).ToArray(),0).Count!=126)throw new InvalidDataException("Original stream root directory differs.");foreach(uint slot in Slots(chunk.ImportsData)){uint index=Word(bin,8+(int)slot);if(index==0 || index>dependencies.Length || dependencies[index-1]!=metadata.Assets.Single().References[(int)index-1])throw new InvalidDataException("Original stream import selector differs.");}
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: restore only the one freshly owned test file after each deliberate corruption, even when rejection fails. */
        //-------------------------------------------------------------------------------------------------
        void Corrupt(string name,byte[] bad){string path=Path.Combine(first,name);byte[] saved=payloads[name];try{File.WriteAllBytes(path,bad);var changed=payloads.ToDictionary(p=>p.Key,p=>p.Value);changed[name]=bad;Refuse(()=>Verify(first,changed));}finally{File.WriteAllBytes(path,saved);}}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: each strong occurrence must select exactly one actual asset across the supplied physical catalogs; duplicates and missing targets refuse. */
    //-------------------------------------------------------------------------------------------------
    private static void ValidateDependencies(AssetId[] ids,ManifestDocument[] catalogs){foreach(var id in ids)if(catalogs.Sum(c=>c.Assets.Count(a=>a.TypeId==id.TypeId && a.InstanceId==id.InstanceId))!=1)throw new InvalidDataException("Original stream dependency absent or ambiguous.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: malformed diagnostic data or an attempted overwrite must fail explicitly. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action){try{action();}catch(Exception e)when(e is InvalidDataException or IOException or NotSupportedException){return;}throw new InvalidDataException("Original stream corruption accepted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: deliberate mutations use isolated byte copies. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Flip(byte[] source,int at){var b=source.ToArray();b[at]^=1;return b;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: selectors require exact sentinel-framed unique source locations. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b){if(b.Length<8 || b.Length%4!=0 || Word(b,b.Length-4)!=uint.MaxValue)throw new InvalidDataException("Original stream import framing.");var slots=Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();if(slots.Distinct().Count()!=slots.Length)throw new InvalidDataException("Original stream duplicate import source.");return slots;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: stream word reads remain bounded. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at){if(at<0 || at>b.Length-4)throw new InvalidDataException("Original stream word bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: coordinate mutations affect only owned arrays. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
