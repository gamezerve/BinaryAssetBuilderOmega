using System.Buffers.Binary;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: existing nonempty root strong-reference slots may retarget assets while preserving the qualified occurrence topology and import ABI.
internal static class GameObjectStrongReferenceContract
{
    internal static IReadOnlyList<Ep1GameObjectScalarTokens.Field> Fields=>Ep1GameObjectStrongTokens.Fields;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: permit one-for-one retargeting only; adding or removing an import remains a dynamic dependency-topology capability. */
    //-------------------------------------------------------------------------------------------------
    internal static void Validate(XmlDocument source,XmlDocument baseline)
    {
        foreach(var field in Fields)
        {
            string authored=source.DocumentElement!.GetAttribute(field.Name),original=baseline.DocumentElement!.GetAttribute(field.Name);bool authoredPresent=!string.IsNullOrWhiteSpace(authored),originalPresent=!string.IsNullOrWhiteSpace(original);
            if(authoredPresent!=originalPresent)throw new InvalidDataException("Root strong-reference presence cannot change yet: "+field.Name);
            if(authoredPresent && !ConscriptSourceInput.Identity(authored))throw new InvalidDataException("Root strong-reference name is invalid: "+field.Name);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: strong-reference target names are semantic inputs once their fixed import slots have passed one-for-one admission. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlDocument candidate,XmlDocument model)
    {
        foreach(var field in Fields){candidate.DocumentElement!.RemoveAttribute(field.Name);if(model.DocumentElement!.HasAttribute(field.Name))candidate.DocumentElement.SetAttribute(field.Name,model.DocumentElement.GetAttribute(field.Name));}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retarget every already-present root import together, retaining the exact source occurrence positions. */
    //-------------------------------------------------------------------------------------------------
    internal static void AuthorProbe(XmlDocument source,XmlDocument baseline,byte[] native,IReadOnlyList<AssetId> original,IReadOnlyList<ManifestAsset> catalog)
    {
        foreach(var field in Fields)if(!string.IsNullOrWhiteSpace(baseline.DocumentElement!.GetAttribute(field.Name)))
        {
            uint occurrence=Word(native,field.Offset);if(occurrence==0 || occurrence>original.Count)throw new InvalidDataException("Root strong-reference probe occurrence differs: "+field.Name);var current=original[(int)occurrence-1];
            // Reborn: use a real different stock target of the exact captured type so source resolution is tested without inventing a missing dependency.
            var target=catalog.FirstOrDefault(asset=>asset.TypeId==current.TypeId && asset.InstanceId!=current.InstanceId)??throw new InvalidDataException("Root strong-reference probe target unavailable: "+field.Name);int separator=target.Name.IndexOf(':');if(separator<0 || separator==target.Name.Length-1)throw new InvalidDataException("Root strong-reference catalog name differs: "+field.Name);source.DocumentElement!.SetAttribute(field.Name,target.Name[(separator+1)..]);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: derive expected target IDs from authored names while pinning each target type and occurrence index to the reviewed baseline graph. */
    //-------------------------------------------------------------------------------------------------
    internal static AssetId[] Expected(XmlDocument source,XmlDocument baseline,byte[] native,IReadOnlyList<AssetId> original)
    {
        Validate(source,baseline);var expected=original.ToArray();
        foreach(var field in Fields)
        {
            string value=source.DocumentElement!.GetAttribute(field.Name);uint occurrence=Word(native,field.Offset);
            if(string.IsNullOrWhiteSpace(value)){if(occurrence!=0)throw new InvalidDataException("Absent root strong reference retained an import: "+field.Name);continue;}
            if(occurrence==0 || occurrence>expected.Length)throw new InvalidDataException("Root strong-reference occurrence is out of range: "+field.Name);
            var old=expected[occurrence-1];expected[occurrence-1]=new AssetId(old.TypeId,InstanceHandle.GetInstanceId(value));
        }
        return expected;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bounded native import-slot reads never infer occurrence positions from attribute order. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] bytes,int offset){if(offset<0 || offset>bytes.Length-4)throw new InvalidDataException("Root strong-reference slot is out of bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(offset,4));}
}
