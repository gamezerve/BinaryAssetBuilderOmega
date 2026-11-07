using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify object-storage native and metadata layouts against original Uprising spellbooks.
internal static class StoreObjectsContract
{
    internal const string Wire="StoreObjectsSpecialPowerModuleData";
    internal const string LureWire="AddObjectsToLureUpdateSpecialPowerModuleData";
    internal const uint LureTypeId=0xA7DEFFCCu;
    // Reborn: use the retail Yuriko lift activation module identity.
    internal const string LiftWire="AddObjectsToLiftUpdateSpecialPowerModuleData";
    internal const uint LiftTypeId=0xD9B247DFu;
    // Reborn: identify the retail Psychic Slam throw module independently of its activation module.
    internal const string FlingWire="FlingStoredObjectsSpecialPowerModuleData";
    internal const uint FlingTypeId=0x12D1FAD9u;
    internal static readonly uint TypeId=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(Wire);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently qualify all object-storage scalars and references and inherited fields against the original Chrono Swap and Chrono Rift powers. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<StoreObjectsSpecialPowerModuleData>()!=Marshal.SizeOf<SpecialPowerModuleData>()+16)throw new InvalidDataException("EP1 object-storage header must add radius/link words, an OCL import and a typed marker ID.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("PlayerSpellBook","System/PlayerSpellBook.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1,allowNullArmorReferences:true);var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name,allowNullArmorReferences:true).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==TypeId).ToArray();
            var originals=GameObjectExtendedRootContract.Original(witness.Item2,"StoreObjectsSpecialPower");
            // Reborn: qualify all stock storage modules by local module identity, without assuming module positions.
            foreach(var original in originals){
                uint id=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(original.GetAttribute("id").ToLowerInvariant());
                var module=modules.Single(m=>System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(captured.Bytes.AsSpan(m.Native+4))==id);
                if(SdkBuildTests.Enabled){
                    var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(original,true));
                    var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("StoreObjectsSpecialPower",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                    SchemaNativeContract.VerifyStock(resolved,typeof(StoreObjectsSpecialPowerModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
                }
                grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(StoreObjectsSpecialPowerModuleData));count++;
            }
        }
        SdkBuildTests.MetadataProgress("StoreObjectsSpecialPower",$"Reborn: StoreObjectsSpecialPower EP1 native/metadata qualification: PASS ({count} original retail records; header={Marshal.SizeOf<StoreObjectsSpecialPowerModuleData>()}).",count+" stock records");return grammar;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: learn the extra lure link word and inherited storage metadata from the three retail Yuriko ranks. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar QualifyLure(string manifest) =>
        QualifyYurikoActivation<AddObjectsToLureUpdateSpecialPowerModuleData>(manifest, LureTypeId,
            nameof(AddObjectsToLureUpdateSpecialPowerModuleData.LureObjectLinkID), new[] { 101, 102, 103 });

    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify lift activation against the three Psychic Slam ranks, which share lift link 101. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar QualifyLift(string manifest) =>
        QualifyYurikoActivation<AddObjectsToLiftUpdateSpecialPowerModuleData>(manifest, LiftTypeId,
            nameof(AddObjectsToLiftUpdateSpecialPowerModuleData.LiftObjectLinkID), new[] { 101, 101, 101 });

    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify all three throw ranks and learn the repeated weak-reference mappings from the stock object. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar QualifyFling(string manifest)
    {
        const string identity = "GameObject:JapanYurikoTech3";
        const int storeLinkOffset = 476;
        const int liftLinkOffset = 480;
        const int objectMapOffset = 492;
        if (Marshal.SizeOf<FlingStoredObjectsSpecialPowerModuleData>() != 500 ||
            Marshal.OffsetOf<FlingStoredObjectsSpecialPowerModuleData>(nameof(FlingStoredObjectsSpecialPowerModuleData.StoreObjectsLinkID)).ToInt32() != storeLinkOffset ||
            Marshal.OffsetOf<FlingStoredObjectsSpecialPowerModuleData>(nameof(FlingStoredObjectsSpecialPowerModuleData.LiftObjectLinkID)).ToInt32() != liftLinkOffset ||
            Marshal.OffsetOf<FlingStoredObjectsSpecialPowerModuleData>(nameof(FlingStoredObjectsSpecialPowerModuleData.ObjectMap)).ToInt32() != objectMapOffset ||
            Marshal.SizeOf<FlingStoredObjectsObjectMap>() != 8)
            throw new InvalidDataException("EP1 stored-object throw layout differs.");

        var stock = GameObjectStockReview.Capture(manifest, identity, allowNullArmorReferences: true);
        var modules = GameObjectModuleReview.Inspect(manifest, identity, allowNullArmorReferences: true)
            .Containers.Single(container => container.Name == "Behaviors").Modules.Where(module => module.TypeId == FlingTypeId).ToArray();
        var ranks = modules.Select(module => (
            StoreLink: System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(stock.Bytes.AsSpan(module.Native + storeLinkOffset, 4)),
            LiftLink: System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(stock.Bytes.AsSpan(module.Native + liftLinkOffset, 4)),
            MappingCount: System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(stock.Bytes.AsSpan(module.Native + objectMapOffset, 4))));
        if (!ranks.OrderBy(rank => rank.StoreLink).SequenceEqual(new[] { (101, 101, 45), (102, 101, 45), (103, 101, 45) }))
            throw new InvalidDataException("Stock Yuriko throw ranks differ.");

        var grammar = new StockMetadataGrammar();
        foreach (var module in modules)
            grammar.Learn(stock.Bytes, module.Wrapper, stock.Projection.NativeRoot, typeof(FlingStoredObjectsSpecialPowerModuleData));
        return grammar;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify the extra link field and learn each activation module's original storage metadata. */
    //-------------------------------------------------------------------------------------------------
    private static StockMetadataGrammar QualifyYurikoActivation<T>(string manifest, uint typeId, string linkField, int[] expectedLinks)
        where T : struct
    {
        const string identity = "GameObject:JapanYurikoTech3";
        const int linkOffset = 492;
        if (Marshal.SizeOf<T>() != 496 || Marshal.OffsetOf<T>(linkField).ToInt32() != linkOffset)
            throw new InvalidDataException($"EP1 {typeof(T).Name} layout differs.");

        var stock = GameObjectStockReview.Capture(manifest, identity, allowNullArmorReferences: true);
        var modules = GameObjectModuleReview.Inspect(manifest, identity, allowNullArmorReferences: true)
            .Containers.Single(container => container.Name == "Behaviors").Modules.Where(module => module.TypeId == typeId).ToArray();
        var links = modules.Select(module => System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(stock.Bytes.AsSpan(module.Native + linkOffset, 4)));
        if (!links.Order().SequenceEqual(expectedLinks))
            throw new InvalidDataException($"Stock Yuriko {linkField} values differ.");

        var grammar = new StockMetadataGrammar();
        foreach (var module in modules)
            grammar.Learn(stock.Bytes, module.Wrapper, stock.Projection.NativeRoot, typeof(T));
        return grammar;
    }
}
