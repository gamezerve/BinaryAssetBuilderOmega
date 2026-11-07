using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

internal static class UnitAISettingsCompiler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: emit the EP1 singleton with inline victory filters and tracked native imports. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlNode source)
    {
        if(sizeof(UnitAISettings)!=468 || sizeof(ObjectFilter)!=120)
            throw new InvalidDataException("EP1 UnitAISettings native layout differs.");
        var xml=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.OuterXml));
        var namespaces=new XmlNamespaceManager(xml.NameTable);
        namespaces.AddNamespace("ea",SchemaSet.XmlNamespace);
        UnitAISettings* root;
        using var tracker=new Tracker((void**)&root,(uint)sizeof(UnitAISettings),false);
        Marshaler.Marshal(new Node(xml.DocumentElement!.CreateNavigator()!,namespaces),root,tracker);
        var chunk=new Chunk();
        if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("UnitAISettings native allocation failed.");
        return chunk;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify the authored stock singleton against the original global native record and its auxiliary slots. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(string manifest,SchemaSet schemas,ManifestAsset[] stock)
    {
        var catalog=ManifestReader.Read(SdkEnvironmentPreflight.Read(manifest,64*1048576));
        var asset=catalog.Assets.Single(a=>a.Name=="UnitAISettings:Settings_UnitAI");
        if(asset.TypeId!=0xB0B8A920u || asset.TypeHash!=0x2D4A40A3u || asset.Tokenized!=0 ||
            asset.InstanceDataSize!=480 || asset.RelocationDataSize!=8 || asset.ImportsDataSize!=8 || asset.References.Count!=1)
            throw new InvalidDataException("EP1 UnitAISettings stock fingerprint differs.");
        var source=GameObjectExtendedRootContract.Original("System/UnitAISettings.xml","UnitAISettings").Single();
        var xml=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.OuterXml));
        var proof=SchemaNativeContract.Resolve(xml,schemas,stock);
        var instance=ReferencePipelineSmokeTest.Normalize(schemas,proof.OuterXml);
        var chunk=Compile(instance.XmlNode);
        proof.DocumentElement!.RemoveAttribute("id");
        SchemaNativeContract.Verify(proof.DocumentElement,typeof(UnitAISettings),0,chunk,asset.References.ToArray());
        if(!chunk.InstanceBuffer.SequenceEqual(W3DContainerCompiler.ReadStock(manifest,catalog,asset)) ||
            !chunk.RelocationBuffer.SequenceEqual(new byte[]{0x4C,1,0,0,255,255,255,255}) ||
            !chunk.ImportsBuffer.SequenceEqual(new byte[]{0x84,0,0,0,255,255,255,255}))
            throw new InvalidDataException("EP1 UnitAISettings stock source/native layout differs.");
    }
}
