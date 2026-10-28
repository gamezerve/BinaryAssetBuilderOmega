using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify Reaper projectile transformation against the original EP1 native record and inherited replacement grammar.
internal static class ProjectileReplaceSelfContract
{
    internal const string Wire="ProjectileReplaceSelfSpecialAbilityModuleData";
    internal const uint TypeId=0xE2EE1D7Au;
    internal const string Source="EP1/Soviet/Units/SovietHeavyWalkerVehicle.xml";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently anchor both projectile references and the inherited replacement ABI before emitting authored metadata. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(FastHash.GetHashCode(Wire)!=TypeId||Marshal.SizeOf<ProjectileReplaceSelfSpecialAbilityModuleData>()!=280)
            throw new InvalidDataException("EP1 projectile self-replacement wire/header differs.");
        var grammar=ReplaceSelfContract.Qualify(manifest,baseline,schemas,catalog);
        var captured=GameObjectStockReview.Capture(manifest,"GameObject:SovietHeavyWalkerVehicle");
        var module=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==TypeId);
        int table=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(captured.Bytes.AsSpan(module.Wrapper+8));
        var fields=GameObjectStockReview.Tokens(captured.Bytes,table);
        // Reborn: the inline weapon slot and optional OCL pointee have distinct metadata addressing rules.
        int weapon=Marshal.OffsetOf<ProjectileReplaceSelfSpecialAbilityModuleData>("LaunchingWeapon").ToInt32();
        int ocl=Marshal.OffsetOf<ProjectileReplaceSelfSpecialAbilityModuleData>("OtherObjectCreationList").ToInt32();
        if(weapon!=272||ocl!=276||fields[FastHash.GetHashCode("LaunchingWeapon")]!=(0xD046DD22u,module.Native+weapon)
            ||fields[FastHash.GetHashCode("OtherObjectCreationList")]!=(0x42E4E9D1u,System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(captured.Bytes.AsSpan(module.Native+ocl))))
            throw new InvalidDataException("Stock projectile self-replacement reference offset/signature differs.");
        // Reborn: check the captured source independently of the native producer in developer builds.
        if(SdkBuildTests.Enabled){
            var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
            behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(GameObjectExtendedRootContract.Original(Source,"ProjectileReplaceSelfSpecialAbility").Single(),true));
            var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("ProjectileReplaceSelfSpecialAbility",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
            SchemaNativeContract.VerifyStock(resolved,typeof(ProjectileReplaceSelfSpecialAbilityModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
        }
        grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(ProjectileReplaceSelfSpecialAbilityModuleData));
        SdkBuildTests.MetadataProgress("ProjectileReplaceSelfSpecialAbility","Reborn: ProjectileReplaceSelfSpecialAbility: PASS (original Reaper native/metadata; 280-byte record).","1 original record");
        return grammar;
    }
}
