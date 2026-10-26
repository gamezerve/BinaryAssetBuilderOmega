using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify EP1 transport storage and inherited containment against four independent retail vehicles.
internal static class TransportContainContract
{
    internal const string Wire="TransportContainModuleData";
    internal static readonly uint TypeId=FastHash.GetHashCode(Wire);
    internal static readonly (string Name,string Path)[] Witnesses={
        ("AlliedBomberAircraft","Allied/Units/AlliedBomberAircraft.xml"),
        ("AlliedAntiAirVehicleTech1","Allied/Units/AlliedAntiAirVehicleTech1.xml"),
        ("SovietAntiGroundAircraft","Soviet/Units/SovietAntiGroundAircraft.xml"),
        ("JapanLightTransportVehicle","Japan/Units/JapanLightTransportVehicle.xml")};

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove retail native defaults and filters, learn the complete metadata chain, and qualify simple optional list items from the original schema. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in Witnesses){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Name);
            var module=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.TypeId==TypeId);
            int table=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(captured.Bytes.AsSpan(module.Wrapper+8));var fields=GameObjectStockReview.Tokens(captured.Bytes,table);
            // Reborn: independently bind every own scalar slot and both array wire identities to actual retail metadata.
            foreach(var field in typeof(TransportContainModuleData).GetFields().Where(f=>f.Name!="Base"&&!f.FieldType.IsPointer&&(!f.FieldType.IsGenericType||f.FieldType.GetGenericTypeDefinition()!=typeof(Relo.List<>))))
                if(fields[FastHash.GetHashCode(field.Name)].Offset-module.Native!=Marshal.OffsetOf<TransportContainModuleData>(field.Name).ToInt32())throw new InvalidDataException("Stock TransportContain field offset differs: "+field.Name);
            if(fields[FastHash.GetHashCode("InitialPayload")].Type!=0xB967AE82u||fields[FastHash.GetHashCode("UpgradeCreationTrigger")].Type!=0x1CE024D5u)throw new InvalidDataException("Stock TransportContain list signatures differ.");
            if(SdkBuildTests.Enabled){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(GameObjectExtendedRootContract.Original(witness.Path,"TransportContain").Single(),true));
                var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("TransportContain",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                SchemaNativeContract.VerifyStock(resolved,typeof(TransportContainModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
            }
            grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(TransportContainModuleData));count++;
        }
        grammar.QualifyTransportItems(schemas.Schemas);
        SdkBuildTests.MetadataProgress("TransportContain",$"Reborn: TransportContain EP1 native/metadata qualification: PASS ({count} original retail records; header={Marshal.SizeOf<TransportContainModuleData>()}; optional payload/upgrade item grammar is schema-derived).",count+" stock records");return grammar;
    }
}
