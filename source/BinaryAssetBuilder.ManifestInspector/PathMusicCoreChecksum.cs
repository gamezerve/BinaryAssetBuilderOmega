using System.Buffers.Binary;
using System.Reflection;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: independently audit the existing Core output checksum for bounded synthetic-domain music identities without changing package policy.
internal static class PathMusicCoreChecksum
{
    // Reborn: immutable checksum evidence explicitly excludes recovered EP1 metadata and production admission.
    internal sealed record Report(uint ProcessingHash,uint DocumentVersion,int Owners,int UsedBytes,int CapacityBytes,uint CoreChecksum,uint ExpectedChecksum)
    {
        public bool SyntheticProcessingDomain => true;
        public bool Ep1ProcessingHashRecovered => false;
        public bool ProductionBuildReady => false;
        public bool PackagePolicyChanged => false;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: derive ordered identity-only Core declarations from private fresh preparation, then compare an independent padded byte layout. */
    //-------------------------------------------------------------------------------------------------
    internal static Report Inspect(PathMusicCorePreparation preparation)
    {
        preparation.VerifyCurrent(); var evidence = preparation.Preflight();
        if (evidence.Rows.Length is < 1 or > 8 || evidence.ProcessingHash != PathMusicCoreIdentity.Processing)
            throw new InvalidDataException("Bounded synthetic music checksum domain required.");
        InstanceDeclaration[] instances = evidence.Rows.Select(row => Identity(row.Name,row.CoreInstanceHash)).ToArray();
        for (int index = 0; index < instances.Length; index++)
            if (instances[index].Handle.InstanceId != evidence.Rows[index].InstanceId || instances[index].Handle.TypeId != 0x9A651D89u)
                throw new InvalidDataException("Core checksum identity projection differs.");
        uint actual = Core(instances),expected = Independent(instances);
        if (actual != expected) throw new InvalidDataException("Core music checksum differs from independent padded identity table.");
        preparation.VerifyCurrent();
        return new(evidence.ProcessingHash,evidence.DocumentVersion,instances.Length,instances.Length*20,256,actual,expected);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: initialize identity-only declarations through public XML APIs; no processor, codec, source mutation or compiler admission is performed. */
    //-------------------------------------------------------------------------------------------------
    internal static InstanceDeclaration Identity(string name,uint hash)
    {
        XmlDocument document = new(); XmlElement node = document.CreateElement("PathMusicEvent","uri:ea.com:eala:asset"); node.SetAttribute("id",name);
        InstanceDeclaration instance = new(); instance.Initialize(null!); instance.XmlNode = node;
        instance.Handle.TypeHash = 0; instance.Handle.InstanceHash = hash; return instance;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: invoke the existing unchanged Core checksum implementation rather than copying its algorithm into the production compiler. */
    //-------------------------------------------------------------------------------------------------
    internal static uint Core(InstanceDeclaration[] instances) => (uint)typeof(AssetDeclarationDocument)
        .GetMethod("ComputeOutputChecksum",BindingFlags.NonPublic|BindingFlags.Static)!.Invoke(null,new object[] { instances })!;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reconstruct five little-endian words per owner and fixed 256-byte capacity for only the admitted 1–8 owner domain. */
    //-------------------------------------------------------------------------------------------------
    internal static uint Independent(InstanceDeclaration[] instances,bool usedLengthOnly = false)
    {
        if (instances.Length == 0) return 0;
        if (instances.Length > 8) throw new InvalidDataException("Music checksum audit is bounded to eight owners.");
        byte[] bytes = new byte[usedLengthOnly ? instances.Length*20 : 256];
        for (int index = 0; index < instances.Length; index++)
        {
            var instance = instances[index]; uint[] words = { instance.Handle.TypeId,instance.Handle.TypeHash,instance.Handle.InstanceId,
                instance.Handle.InstanceHash,(uint)instance.ReferencedInstances.Count };
            for (int word = 0; word < words.Length; word++) BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(index*20+word*4,4),words[word]);
        }
        return FastHash.GetHashCode(bytes);
    }
}
