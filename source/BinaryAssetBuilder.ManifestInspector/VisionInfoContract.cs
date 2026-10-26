using System.Buffers.Binary;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: optional VisionInfo owns its actual presence, forty-byte native record and plain field metadata independently of a Conscript template.
internal static class VisionInfoContract
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: restore VisionInfo only in the disposable comparison copy; no source or emitted object receives a hidden vision record. */
    //-------------------------------------------------------------------------------------------------
    internal static void MaskComparison(XmlDocument source,XmlDocument baseline)
    {
        var actual=ConscriptVariableContainers.Find(source,"VisionInfo");var model=ConscriptVariableContainers.Find(baseline,"VisionInfo");actual?.ParentNode!.RemoveChild(actual);if(model==null)return;
        XmlElement? next=null;for(XmlNode? following=model.NextSibling;following!=null;following=following.NextSibling)if(following is XmlElement element&&(next=source.DocumentElement!.ChildNodes.OfType<XmlElement>().FirstOrDefault(e=>e.LocalName==element.LocalName&&e.NamespaceURI==element.NamespaceURI))!=null)break;
        source.DocumentElement!.InsertBefore(source.ImportNode(model,true),next);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify the populated retail infantry record and the genuinely absent PlayerSpellBook record without changing user sources. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+baseline.DocumentElement!.GetAttribute("id"));var field=GameObjectStockReview.Tokens(captured.Bytes,0)[FastHash.GetHashCode("VisionInfo")];int native=BinaryPrimitives.ReadInt32LittleEndian(captured.Bytes.AsSpan(captured.Projection.NativeRoot+560));if(field.Type!=0x649F97D1u||native<captured.Projection.NativeRoot+600)throw new InvalidDataException("Stock populated VisionInfo pointer/type differs.");
        if(SdkBuildTests.Enabled){var resolved=SchemaNativeContract.Resolve(baseline,schemas,catalog);SchemaNativeContract.VerifyStock(ConscriptVariableContainers.Find(resolved,"VisionInfo")!,typeof(Ep1VisionInfo),0,captured.Bytes,native,captured.Asset.References.ToArray());}
        var grammar=new StockMetadataGrammar();grammar.Learn(captured.Bytes,field.Offset,captured.Projection.NativeRoot,typeof(Ep1VisionInfo));
        var absent=GameObjectStockReview.Capture(manifest,"GameObject:PlayerSpellBook",allowNullArmorReferences:true);var absentField=GameObjectStockReview.Tokens(absent.Bytes,0)[FastHash.GetHashCode("VisionInfo")];if(absentField.Type!=field.Type||absentField.Offset!=0||BinaryPrimitives.ReadUInt32LittleEndian(absent.Bytes.AsSpan(absent.Projection.NativeRoot+560))!=0)throw new InvalidDataException("Stock absent VisionInfo declaration differs.");
        SdkBuildTests.MetadataProgress("VisionInfo","Reborn: VisionInfo present/absent retail qualification: PASS (native pointer, plain field metadata and null spellbook declaration).","present and absent retail records");return grammar;
    }
}
