using System.Security.Cryptography;
using System.Text;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: immutable build-local evidence binds a completely verified variable graph to its exact source, ordered targets and serialized buffers.
internal sealed class ConscriptGraphQualification
{
    private readonly string source;
    private readonly AssetId[] references;
    private readonly byte[][] hashes;
    internal int MetadataPrefix {get;}
    internal int ReferenceCount=>references.Length;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: the independent complete native/metadata verifier creates this snapshot only after successful ownership checks. */
    //-------------------------------------------------------------------------------------------------
    internal ConscriptGraphQualification(InstanceDeclaration instance,Chunk verified,int prefix)
    {
        if(prefix<12||prefix%12!=0||prefix>verified.InstanceBuffer.Length-600)throw new InvalidDataException("Invalid qualified graph extent.");
        source=instance.XmlNode.OuterXml;references=instance.ReferencedInstances.Select(h=>new AssetId(h.TypeId,h.InstanceId)).ToArray();MetadataPrefix=prefix;
        hashes=new[]{verified.InstanceBuffer,verified.RelocationBuffer,verified.ImportsBuffer}.Select(bytes=>SHA256.HashData(bytes)).ToArray();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: later source edits, target reorders, identity changes or buffer mutations cannot reuse a previous graph qualification. */
    //-------------------------------------------------------------------------------------------------
    internal void Verify(InstanceDeclaration instance,AssetBuffer buffer)
    {
        if(instance.XmlNode.OuterXml!=source||instance.Handle.TypeId!=0x942FFF2D||instance.Handle.TypeHash!=0xCEB9FE36||instance.Handle.InstanceHash!=FastHash.GetHashCode(Encoding.UTF8.GetBytes(source))
            ||!instance.ReferencedInstances.Select(h=>new AssetId(h.TypeId,h.InstanceId)).SequenceEqual(references)
            ||instance.ValidatedReferencedInstances==null||!instance.ValidatedReferencedInstances.SequenceEqual(instance.ReferencedInstances))throw new InvalidDataException("Variable graph source/identity/reference qualification is stale.");
        byte[][] buffers={buffer.InstanceData,buffer.RelocationData,buffer.ImportsData};
        for(int i=0;i<buffers.Length;i++)if(!SHA256.HashData(buffers[i]).SequenceEqual(hashes[i]))throw new InvalidDataException("Variable graph serialized bytes differ from their qualification.");
    }
}
