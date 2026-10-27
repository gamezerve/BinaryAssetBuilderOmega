using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify the complete collision layout against original Uprising Flak Trooper and four Grinder Tank records.
internal static class FireWeaponCollideContract
{
    internal const string Wire="FireWeaponCollideModuleData";
    internal static readonly uint TypeId=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(Wire);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently qualify the collision fields, their offsets and the Collide/Behavior inheritance chain against original infantry and Grinder records. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<Ep1FireWeaponCollideModuleData>()!=112)throw new InvalidDataException("EP1 collision header must be 112 bytes.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("SovietAntiVehicleInfantry","Soviet/Units/SovietAntiVehicleInfantry.xml"),("SovietGrinderVehicle","EP1/Soviet/Units/SovietGrinderVehicle.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1);
            var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==TypeId).ToArray();
            foreach(var original in GameObjectExtendedRootContract.Original(witness.Item2,"FireWeaponCollide")){
            uint id=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(original.GetAttribute("id").ToLowerInvariant());
            var module=modules.Single(m=>System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(captured.Bytes.AsSpan(m.Native+4))==id);
            // Reborn: actual retail metadata independently anchors every own field, including the trailing FireOnce byte and collision timers.
            int table=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(captured.Bytes.AsSpan(module.Wrapper+8));var fields=GameObjectStockReview.Tokens(captured.Bytes,table);
            foreach(string name in new[]{"CollideWeapon","Flags","RequiredStatus","ForbiddenStatus","WhereToFire","DisabledTypesToProcess","DisableType","DisableDuration","FiringInterval","FireOnce"})if(fields[BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(name)].Offset-module.Native!=Marshal.OffsetOf<Ep1FireWeaponCollideModuleData>(name).ToInt32())throw new InvalidDataException("Stock FireWeaponCollide field offset differs: "+name);
            if(SdkBuildTests.Enabled){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(original,true));
                var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("FireWeaponCollide",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                SchemaNativeContract.VerifyStock(resolved,typeof(Ep1FireWeaponCollideModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
            }
            grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(Ep1FireWeaponCollideModuleData));count++;
            }
        }
        SdkBuildTests.MetadataProgress("FireWeaponCollide",$"Reborn: FireWeaponCollide EP1 native/metadata qualification: PASS ({count} original retail records; header={Marshal.SizeOf<Ep1FireWeaponCollideModuleData>()}).",count+" stock records");return grammar;
    }
}
