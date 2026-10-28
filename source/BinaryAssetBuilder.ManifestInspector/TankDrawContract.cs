using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify Tesla Tank's EP1 draw extension while retaining inherited scripted rendering capabilities.
internal static class TankDrawContract
{
    internal const uint TypeId=0x8A811D3Eu;
    internal const string Path="Soviet/Units/SovietAntiVehicleVehicleTech2.xml";
    //-------------------------------------------------------------------------------------------------
    /** Reborn: learn the retail terrain/list identities and independently qualify the optional schema-only articulation pair. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<Ep1W3DTankDrawModuleData>()!=228)throw new InvalidDataException("EP1 TankDraw header differs.");
        var grammar=new StockMetadataGrammar();var stock=GameObjectStockReview.Capture(manifest,"GameObject:SovietAntiVehicleVehicleTech2");
        var module=GameObjectModuleReview.Inspect(manifest,stock.Asset.Name).Containers.Single(c=>c.Name=="Draws").Modules.Single(m=>m.TypeId==TypeId);
        if(SdkBuildTests.Enabled){var source=(XmlDocument)baseline.CloneNode(true);var draws=source.GetElementsByTagName("Draws",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();draws.RemoveAll();draws.AppendChild(source.ImportNode(GameObjectExtendedRootContract.Original(Path,"TankDraw").Single(),true));var element=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("TankDraw",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();SchemaNativeContract.VerifyStock(element,typeof(Ep1W3DTankDrawModuleData),TypeId,stock.Bytes,module.Native,stock.Asset.References.ToArray());}
        grammar.Learn(stock.Bytes,module.Wrapper,stock.Projection.NativeRoot,typeof(Ep1W3DTankDrawModuleData));
        grammar.QualifyTankArticulation(schemas.Schemas);
        ShaderMaterialContract.Qualify(grammar,manifest,baseline,schemas,catalog);
        ScriptedShadowInfoContract.Qualify(grammar,manifest,baseline,schemas,catalog);
        ScriptedFXEventContract.Qualify(grammar,manifest,baseline,schemas,catalog);
        ScriptedTurretContract.Qualify(grammar,manifest,baseline,schemas,catalog);
        SdkBuildTests.MetadataProgress("TankDraw","Reborn: TankDraw: PASS (original Tesla native/metadata, 228-byte root; articulation pair is schema-derived).","1 original record");return grammar;
    }
}
