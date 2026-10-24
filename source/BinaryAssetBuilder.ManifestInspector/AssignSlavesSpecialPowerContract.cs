using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: the assign-slaves leaf adds no native fields; qualify its actual SpecialPower inheritance and stock metadata rather than cloning an infantry module.
internal static class AssignSlavesSpecialPowerContract
{
    internal const string Wire="AssignSlavesTargetObjectSpecialPowerModuleData";
    internal static uint TypeId=>FastHash.GetHashCode(Wire);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently prove two original EP1 repair-power modules against their authored sources and learn their exact metadata inheritance. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<AssignSlavesTargetObjectSpecialPowerModuleData>()!=Marshal.SizeOf<SpecialPowerModuleData>())throw new InvalidDataException("AssignSlaves special power must add no native storage to SpecialPowerModuleData.");
        var grammar=new StockMetadataGrammar();
        foreach(var witness in new[]{("SovietCrane","Soviet/Structures/SovietCrane.xml"),("AlliedWarFactory","Allied/Structures/AlliedWarFactory.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1);var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==TypeId).ToArray();
            var originals=GameObjectExtendedRootContract.Original(witness.Item2,"AssignSlavesTargetObjectSpecialPower");if(originals.Length==0||modules.Length!=originals.Length)throw new InvalidDataException("AssignSlaves stock/source witness cardinality differs.");
            foreach(var original in originals){var module=modules.Single(m=>BinaryPrimitives.ReadUInt32LittleEndian(captured.Bytes.AsSpan(m.Native+4,4))==FastHash.GetHashCode(original.GetAttribute("id").ToLowerInvariant()));
                var document=(XmlDocument)baseline.CloneNode(true);var behaviors=document.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(document.ImportNode(original,true));
                var source=SchemaNativeContract.Resolve(document,schemas,catalog).GetElementsByTagName("AssignSlavesTargetObjectSpecialPower",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                SchemaNativeContract.VerifyStock(source,typeof(AssignSlavesTargetObjectSpecialPowerModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(AssignSlavesTargetObjectSpecialPowerModuleData));
            }
        }
        Console.WriteLine("Reborn: AssignSlaves special power: original SovietCrane/AlliedWarFactory native and metadata witnesses qualified.");return grammar;
    }
}
