using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: campaign map overrides can author tech-store layouts through the same typed source and native reference pipeline as skirmish stores.
internal static class PlayerTechStoreCompiler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: write the retail-qualified root/row/button arrays; XSD enforces five rows and three buttons per row. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlNode source)
    {
        var xml=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.OuterXml));var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);
        Ep1PlayerTechStoreTemplate* root;using var tracker=new Tracker((void**)&root,(uint)sizeof(Ep1PlayerTechStoreTemplate),false);Marshaler.Marshal(new Node(xml.DocumentElement!.CreateNavigator()!,ns),root,tracker);
        var chunk=new Chunk();if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("PlayerTechStoreTemplate native allocation failed.");return chunk;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare every original faction store to the actual static native payload before developer admission. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(string manifest,SchemaSet schemas,ManifestAsset[] catalog)
    {
        var document=ManifestReader.Read(File.ReadAllBytes(manifest));int count=0;
        foreach(var element in GameObjectExtendedRootContract.Original("UI/PlayerTechStoreTemplates.xml","PlayerTechStoreTemplate")){
            var source=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(element.OuterXml));source.Schemas.Add(schemas.Schemas);source.Validate((_,e)=>throw SdkXmlDiagnostics.SchemaError(e));ConscriptBehaviorSurvey.ResolveStrong(source,schemas,catalog);
            var instance=ReferencePipelineSmokeTest.Normalize(schemas,source.OuterXml);var asset=document.Assets.Single(a=>a.Name=="PlayerTechStoreTemplate:"+element.GetAttribute("id"));
            if(asset.TypeId!=0x484D7CF8||asset.TypeHash!=0x71A8F2C8||asset.Tokenized!=0)throw new InvalidDataException("EP1 tech store fingerprint differs.");
            new SelectedNativeGraphComparer(W3DContainerCompiler.ReadStock(manifest,document,asset),asset.References.ToArray(),Compile(instance.XmlNode),instance,captureNativeCoordinates:false).Compare(typeof(Ep1PlayerTechStoreTemplate),0);count++;
        }
        Console.WriteLine($"PlayerTechStoreTemplate native comparison PASS: {count} original stores.");
    }
}
