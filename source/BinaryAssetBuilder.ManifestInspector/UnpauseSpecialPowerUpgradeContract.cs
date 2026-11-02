using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: this legacy family remains in the Uprising schema/runtime; compose its two own fields with the retail-proved Upgrade base.
internal static class UnpauseSpecialPowerUpgradeContract
{
    internal const string Wire="UnpauseSpecialPowerUpgradeModuleData";
    internal static readonly uint TypeId=FastHash.GetHashCode(Wire);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reuse independently captured Upgrade/animation metadata without claiming an original populated EP1 Unpause witness. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<UnpauseSpecialPowerUpgradeModuleData>()!=44||Marshal.OffsetOf<UnpauseSpecialPowerUpgradeModuleData>("SpecialPowerTemplate").ToInt32()!=36||Marshal.OffsetOf<UnpauseSpecialPowerUpgradeModuleData>("ObeyRechageOnTrigger").ToInt32()!=40)
            throw new InvalidDataException("UnpauseSpecialPowerUpgrade must extend the EP1 Upgrade base with a strong power reference and one boolean.");
        var grammar=AttributeModifierUpgradeContract.Qualify(manifest,baseline,schemas,catalog);
        grammar.QualifyUnpauseSpecialPowerUpgrade(schemas.Schemas);
        Console.WriteLine("Warning: LEGACYMODULE ' UnpauseSpecialPowerUpgrade ' uses the Uprising schema and retail-qualified Upgrade base; no original Uprising source example was found. In-game unpause behavior is not independently verified.");
        return grammar;
    }
}
