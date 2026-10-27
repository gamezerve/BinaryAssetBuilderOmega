using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify the complete assembly layout against the original Uprising Soviet Engineer record.
internal static class AssembleStructureContract
{
    internal const string Wire="AssembleStructureSpecialAbilityUpdateModuleData";
    internal static readonly uint TypeId=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(Wire);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently qualify the assembly fields, their offsets and the SpecialAbility/Update inheritance chain against the original Engineer record. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<AssembleStructureSpecialAbilityUpdateModuleData>()!=272)throw new InvalidDataException("EP1 assembly header must be 272 bytes.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("SovietEngineer","Soviet/Units/SovietEngineer.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1);
            var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==TypeId).ToArray();
            foreach(var original in GameObjectExtendedRootContract.Original(witness.Item2,"AssembleStructureSpecialAbilityUpdate")){
            uint id=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(original.GetAttribute("id").ToLowerInvariant());
            var module=modules.Single(m=>System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(captured.Bytes.AsSpan(m.Native+4))==id);
            // Reborn: actual retail metadata independently anchors every own field, including the assembly timer, model condition and trailing busy byte.
            int table=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(captured.Bytes.AsSpan(module.Wrapper+8));var fields=GameObjectStockReview.Tokens(captured.Bytes,table);
            foreach(string name in new[]{"NewObjectAssemblyTime","ModelConditionsOnAssembly","BusyDuringAssembly"})if(fields[BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(name)].Offset-module.Native!=Marshal.OffsetOf<AssembleStructureSpecialAbilityUpdateModuleData>(name).ToInt32())throw new InvalidDataException("Stock assembly field offset differs: "+name);
            if(SdkBuildTests.Enabled){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(original,true));
                var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("AssembleStructureSpecialAbilityUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                SchemaNativeContract.VerifyStock(resolved,typeof(AssembleStructureSpecialAbilityUpdateModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
            }
            grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(AssembleStructureSpecialAbilityUpdateModuleData));count++;
            }
        }
        SdkBuildTests.MetadataProgress("AssembleStructureSpecialAbilityUpdate",$"Reborn: AssembleStructureSpecialAbilityUpdate EP1 native/metadata qualification: PASS ({count} original retail records; header={Marshal.SizeOf<AssembleStructureSpecialAbilityUpdateModuleData>()}).",count+" stock records");return grammar;
    }
}
