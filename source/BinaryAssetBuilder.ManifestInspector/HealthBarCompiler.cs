using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
using Relo;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify untokenized EP1 health-bar headers and all authored reference/filter data before publication.
internal static class HealthBarCompiler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retail offsets are independent witnesses; a type hash alone cannot detect a malformed runtime pointer layout. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlDocument source, SchemaSet schemas, ManifestAsset[] catalog,
        InstanceDeclaration instance, Chunk native)
    {
        if (Marshal.SizeOf<HealthBarTemplate>() != 8 || Marshal.SizeOf<HealthBarSettings>() != 16 ||
            Marshal.SizeOf<HealthBarObjectMapping>() != 124 ||
            Marshal.OffsetOf<HealthBarSettings>(nameof(HealthBarSettings.DefaultHealthBar)).ToInt32() != 4 ||
            Marshal.OffsetOf<HealthBarSettings>(nameof(HealthBarSettings.ObjectMapping)).ToInt32() != 8 ||
            Marshal.OffsetOf<HealthBarObjectMapping>(nameof(HealthBarObjectMapping.ObjectFilter)).ToInt32() != 4)
            throw new InvalidDataException("Health-bar record layout differs from the retail EP1 header/mapping ABI.");
        var proof = SchemaNativeContract.Resolve(source, schemas, catalog);
        proof.DocumentElement!.RemoveAttribute("id");
        Type type = source.DocumentElement!.LocalName == "HealthBarSettings"
            ? typeof(HealthBarSettings) : typeof(HealthBarTemplate);
        SchemaNativeContract.Verify(proof.DocumentElement!, type, 0, native,
            instance.ReferencedInstances.Select(r => new AssetId(r.TypeId, r.InstanceId)).ToArray());
    }
}
