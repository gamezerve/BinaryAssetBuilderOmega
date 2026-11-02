using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using Relo;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: production multisounds use the recovered EP1 16-byte root and 28-byte children, never the legacy KW child layout.
internal static class Ep1MultisoundCompiler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve ordered typed sound imports, optional scalar pointers and the official control flags. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlNode source)
    {
        var xml=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.OuterXml));
        var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);
        Marshaler.Ep1Multisound* root;
        using var tracker=new Tracker((void**)&root,(uint)sizeof(Marshaler.Ep1Multisound),false);
        Marshaler.Marshal(new Node(xml.DocumentElement!.CreateNavigator()!,ns),root,tracker);
        var chunk=new Chunk();if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("Multisound native allocation failed.");
        return chunk;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: asset IDs live in the manifest; independently prove every source field, pointer and concrete dependency. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlDocument ProofSource(XmlDocument source,SchemaSet schemas,ManifestAsset[] catalog)
    {
        var proof=SchemaNativeContract.Resolve(source,schemas,catalog);proof.DocumentElement!.RemoveAttribute("id");return proof;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify original Uprising default, weighted, PLAY_ONE and LOOP graphs against bounded retail global records. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(string manifest,SchemaSet schemas,ManifestAsset[] catalog)
    {
        var document=ManifestReader.Read(SdkEnvironmentPreflight.Read(manifest,64*1048576));int count=0;
        foreach(string file in new[]{"Sounds/SoundEffects.xml","Sounds/Voice.xml"})
        foreach(var original in GameObjectExtendedRootContract.Original(file,"Multisound")){
            string id=original.GetAttribute("id");var asset=document.Assets.SingleOrDefault(a=>a.Name=="Multisound:"+id);if(asset==null)continue;
            if(asset.TypeId!=0xA3A7AF37u||asset.TypeHash!=0xF79C5A89u||asset.Tokenized!=0)throw new InvalidDataException("Multisound retail fingerprint differs.");
            var source=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(original.OuterXml));var proof=ProofSource(source,schemas,catalog);
            var stock=W3DContainerCompiler.ReadStock(manifest,document,asset);
            SchemaNativeContract.VerifyStock(proof.DocumentElement!,typeof(Marshaler.Ep1Multisound),0,stock,0,asset.References.ToArray());count++;
        }
        if(count==0)throw new InvalidDataException("No original Uprising Multisound witnesses were found.");
        SdkBuildTests.MetadataProgress("Multisound",$"Reborn: Multisound EP1 qualification: PASS ({count} original native graphs).",count+" original records");
    }
}
