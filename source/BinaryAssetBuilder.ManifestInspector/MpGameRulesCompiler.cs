using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: author multiplayer rules through the real EP1 singleton layout rather than the legacy KW record.
internal static class MpGameRulesCompiler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile every official cash/timer field with schema defaults and no synthetic references. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlNode source)
    {
        if(sizeof(Ep1MpGameRules)!=24)throw new InvalidDataException("EP1 MpGameRules native size differs.");
        var xml=SdkXmlDiagnostics.Parse(System.Text.Encoding.UTF8.GetBytes(source.OuterXml));var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);
        Ep1MpGameRules* root;using var tracker=new Tracker((void**)&root,(uint)sizeof(Ep1MpGameRules),false);
        Marshaler.Marshal(new Node(xml.DocumentElement!.CreateNavigator()!,ns),root,tracker);
        var chunk=new Chunk();if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("MpGameRules native allocation failed.");return chunk;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently qualify the original EA global record and its transfer-timer/default cash offsets. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(string global,SchemaSet schemas,ManifestAsset[] catalog)
    {
        var doc=ManifestReader.Read(SdkEnvironmentPreflight.Read(global,64*1048576));var asset=doc.Assets.Single(a=>a.Name=="MpGameRules:TheMpGameRules");
        if(asset.TypeId!=0x2C358B80u||asset.TypeHash!=0x34D91648u||asset.Tokenized!=0||asset.InstanceDataSize!=24||asset.RelocationDataSize!=0||asset.ImportsDataSize!=0||asset.References.Count!=0)throw new InvalidDataException("EP1 MpGameRules fingerprint differs.");
        var original=GameObjectExtendedRootContract.Original("Shell/MpGameRules.xml","MpGameRules").Single();
        var source=SdkXmlDiagnostics.Parse(System.Text.Encoding.UTF8.GetBytes(original.OuterXml));var proof=SchemaNativeContract.Resolve(source,schemas,catalog);proof.DocumentElement!.RemoveAttribute("id");
        var native=W3DContainerCompiler.ReadStock(global,doc,asset);
        SchemaNativeContract.VerifyStock(proof.DocumentElement,typeof(Ep1MpGameRules),0,native,0,Array.Empty<AssetId>());
        if(!Compile(source.DocumentElement!).InstanceBuffer.SequenceEqual(native))throw new InvalidDataException("MpGameRules original source/native roundtrip differs.");
    }
}
