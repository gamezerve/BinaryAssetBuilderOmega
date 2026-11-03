using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData
{
    // Reborn: retail EP1 tech stores use a 16-byte root with five source rows, each owning three button imports.
    [StructLayout(LayoutKind.Sequential)]
    public struct Ep1PlayerTechStoreTemplate
    {
        public BaseAssetType Base;
        public FactionType Faction;
        public List<Ep1PlayerTechStoreTemplateRow> Row;
    }
    // Reborn: row/button cardinalities are enforced by the official XSD; native arrays retain authored order.
    [StructLayout(LayoutKind.Sequential)]
    public struct Ep1PlayerTechStoreTemplateRow
    {
        public List<AssetReference<Ep1PurchasePlayerTechButtonTemplate>> Button;
    }
}
