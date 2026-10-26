using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
namespace BinaryAssetBuilder.ManifestInspector;
// Reborn: typed proof views preserve the retail jet ABI while retaining the independent AI flag/chooser validation.
internal static class JetAIUpdateContract
{
    internal const uint TypeId=0x4486CF48;
    [StructLayout(LayoutKind.Sequential)]
    internal struct Native
    {
        public AIUpdateContract.Native Base;
        public Percentage OutOfAmmoDamagePerSecond;
        public Time OutOfAmmoDamageDelay;
        public float ParkingOffset;
        public Relo.AssetReference<WeaponTemplate> UseForOutOfAmmoCheck;
        public Relo.AssetReference<BaseAssetType> OutOfAmmoEvaEvent;
        public SageBool NeedsRunway,ReturnToBaseWhenVictimDead,KeepsParkingSpaceWhenAirborne,CirclesForAttack;
    }
    internal static readonly (string Name,string Path)[] Witnesses={
        ("AlliedBomberAircraft","Allied/Units/AlliedBomberAircraft.xml"),
        ("SovietBomberAircraft","Soviet/Units/SovietBomberAircraft.xml")};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain explicit existing AI contact/vision-list diagnostics and prove every current jet byte, pointer and strong reference. */
    //-------------------------------------------------------------------------------------------------
    internal static SchemaNativeContract.Result Verify(XmlElement source,Relo.Chunk native,IReadOnlyList<AssetId> references)
    {
        if(!string.IsNullOrWhiteSpace(source.GetAttribute("SpecialContactPoints")))throw SdkXmlDiagnostics.At(new InvalidDataException("JetAIUpdate inherited SpecialContactPoints item metadata is not implemented."),source.GetAttributeNode("SpecialContactPoints"));
        if(source.GetElementsByTagName("ObjectVisionRange",SchemaSet.XmlNamespace).Count!=0)throw SdkXmlDiagnostics.At(new InvalidDataException("JetAIUpdate inherited ObjectVisionRange item native layout is not implemented."),source);
        return SchemaNativeContract.Verify(source,typeof(Native),TypeId,native,references);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently anchor the 100-byte header and every own offset/signature against retail Century and Kirov AI modules. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<Native>()!=100||Marshal.SizeOf<Ep1JetAIUpdateModuleData>()!=100)throw new InvalidDataException("EP1 jet AI header must be 100 bytes.");
        var grammar=new StockMetadataGrammar();
        foreach(var witness in Witnesses){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Name);var module=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="AI").Modules.Single(m=>m.TypeId==TypeId);
            int table=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(captured.Bytes.AsSpan(module.Wrapper+8));var fields=GameObjectStockReview.Tokens(captured.Bytes,table);
            foreach(var field in typeof(Native).GetFields().Where(f=>f.Name!="Base"))if(fields[FastHash.GetHashCode(field.Name)].Offset-module.Native!=Marshal.OffsetOf<Native>(field.Name).ToInt32())throw new InvalidDataException("Stock jet AI field offset differs: "+field.Name);
            if(fields[FastHash.GetHashCode("OutOfAmmoEvaEvent")].Type!=0xC4EC43B1)throw new InvalidDataException("Jet out-of-ammo EVA is a strong EvaEvent reference.");
            var source=(XmlDocument)baseline.CloneNode(true);var old=source.GetElementsByTagName("AI",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();old.RemoveAll();old.AppendChild(source.ImportNode(GameObjectExtendedRootContract.Original(witness.Path,"JetAIUpdate").Single(),true));
            if(SdkBuildTests.Enabled){var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);SchemaNativeContract.VerifyStock(ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container=="AI").Element,typeof(Native),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());}
            grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(Native));
        }
        SdkBuildTests.MetadataProgress("JetAIUpdate","Reborn: JetAIUpdate EP1 native/metadata qualification: PASS (2 original retail records; header=100).","2 stock records");return grammar;
    }
}
