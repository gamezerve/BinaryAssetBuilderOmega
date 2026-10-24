using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: original airfield native records and metadata independently anchor recall limits and the complete parking/filter layout.
internal static class AirfieldBehaviorContract
{
    // Reborn: only the two concrete EP1 airfield models participate in this family contract.
    internal static bool Supports(Type type)=>type==typeof(Ep1RecallUnitsSpecialPowerModuleData)||type==typeof(Ep1ParkingPlaceBehaviorModuleData);
    internal static string Wire(Type type)=>type.Name.Substring(3);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove original Soviet/Allied airfield records against their complete sources and learn their actual inherited metadata grammar. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(Type type,string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(!Supports(type)||Marshal.SizeOf(type)!=(type==typeof(Ep1RecallUnitsSpecialPowerModuleData)?484:60))throw new InvalidDataException("EP1 airfield native header differs.");
        string wire=Wire(type),elementName=wire.Replace("ModuleData","");uint typeId=FastHash.GetHashCode(wire);var grammar=new StockMetadataGrammar();int count=0;
        foreach(string name in new[]{"SovietAirfield","AlliedAirfield"}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+name);var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.TypeId==typeId).ToArray();
            var originals=GameObjectExtendedRootContract.Original((name.StartsWith("Soviet",StringComparison.Ordinal)?"Soviet":"Allied")+"/Structures/"+name+".xml",elementName);
            if(modules.Length!=originals.Length)throw new InvalidDataException("Airfield stock/source module count differs: "+name+":"+elementName);
            foreach(var original in originals){
                uint local=FastHash.GetHashCode(original.GetAttribute("id").ToLowerInvariant());var module=modules.Single(m=>BinaryPrimitives.ReadUInt32LittleEndian(captured.Bytes.AsSpan(m.Native+4,4))==local);
                var document=(XmlDocument)baseline.CloneNode(true);var behaviors=document.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(document.ImportNode(original,true));
                var resolved=SchemaNativeContract.Resolve(document,schemas,catalog).GetElementsByTagName(elementName,SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                SchemaNativeContract.VerifyStock(resolved,type,typeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,type);count++;
            }
        }
        if(count==0)throw new InvalidDataException("Airfield stock module witness absent: "+elementName);
        Console.WriteLine($"Reborn: {elementName} EP1 native/metadata qualification: PASS ({count} original airfield module(s)).");return grammar;
    }
}
