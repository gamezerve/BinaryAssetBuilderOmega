using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: credits use the original EP1 string/array graph, with fresh authored native storage and no asset imports.
internal static class CreditsCompiler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: EA treats indentation-only Text nodes as blank lines; retain Text elements and all non-formatting content. */
    //-------------------------------------------------------------------------------------------------
    internal static void NormalizeFormatting(XmlElement root)
    {
        foreach(XmlElement text in root.GetElementsByTagName("Text",SchemaSet.XmlNamespace))
            foreach(XmlNode whitespace in text.ChildNodes.Cast<XmlNode>().Where(n=>n.NodeType is XmlNodeType.Whitespace or XmlNodeType.SignificantWhitespace).ToArray())
                text.RemoveChild(whitespace);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: use the existing tracker and marshaler so all nested fonts, arrays and blank strings own their relocations. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlNode source)
    {
        if(sizeof(Credits)!=32||sizeof(CreditStyle)!=24||sizeof(CreditElement)!=12||sizeof(ScrollInfo)!=12)throw new InvalidDataException("EP1 credits native layout differs.");
        var xml=SdkXmlDiagnostics.Parse(System.Text.Encoding.UTF8.GetBytes(source.OuterXml));var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);
        NormalizeFormatting(xml.DocumentElement!);
        Credits* root;using var tracker=new Tracker((void**)&root,(uint)sizeof(Credits),false);
        Marshaler.Marshal(new Node(xml.DocumentElement!.CreateNavigator()!,ns),root,tracker);
        var chunk=new Chunk();if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("Credits native allocation failed.");return chunk;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify every stock style, item and text using the original EA source and actual EP1 global BIN/RELO/IMP record. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(string global,SchemaSet schemas,ManifestAsset[] catalog)
    {
        var doc=ManifestReader.Read(SdkEnvironmentPreflight.Read(global,64*1048576));var asset=doc.Assets.Single(a=>a.Name=="Credits:RA3Credits");
        if(asset.TypeId!=0xED226AE9u||asset.TypeHash!=0x10D657EEu||asset.Tokenized!=0||asset.References.Count!=0||asset.ImportsDataSize!=0)throw new InvalidDataException("EP1 Credits fingerprint differs.");
        var original=GameObjectExtendedRootContract.Original("GlobalData/Credits.xml","Credits").Single();
        var source=SdkXmlDiagnostics.Parse(System.Text.Encoding.UTF8.GetBytes(original.OuterXml));
        NormalizeFormatting(source.DocumentElement!);
        var proof=SchemaNativeContract.Resolve(source,schemas,catalog);proof.DocumentElement!.RemoveAttribute("id");
        SchemaNativeContract.VerifyStock(proof.DocumentElement,typeof(Credits),0,W3DContainerCompiler.ReadStock(global,doc,asset),0,Array.Empty<AssetId>());
    }
}
