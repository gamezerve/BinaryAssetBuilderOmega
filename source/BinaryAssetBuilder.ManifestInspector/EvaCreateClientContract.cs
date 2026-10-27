using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify EVA-create client native and metadata layouts against original Uprising structures.
internal static class EvaCreateClientContract
{
    internal const string Wire="EvaAnnounceClientCreateModuleData";
    internal static readonly uint TypeId=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(Wire);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently qualify all EVA-create client scalars, references and inherited fields against the original Iron Curtain record. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<Ep1EvaAnnounceClientCreateModuleData>()!=28)throw new InvalidDataException("EP1 EVA-create client header must be 28 bytes.");
        var grammar=new StockMetadataGrammar();int count=0;
        foreach(var witness in new[]{("SovietSuperWeapon","Soviet/Structures/SovietSuperWeapon.xml")}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+witness.Item1,allowNullArmorReferences:true);var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name,allowNullArmorReferences:true).Containers.Single(c=>c.Name=="ClientUpdates").Modules.Where(m=>m.TypeId==TypeId).ToArray();
            var originals=GameObjectExtendedRootContract.Original(witness.Item2,"EvaAnnounceClientCreate");
            if(originals.Length==0||originals.Length!=modules.Length)throw new InvalidDataException("EVA-create client stock witness count differs.");
            // Reborn: qualify all stock storage modules by local module identity, without assuming module positions.
            foreach(var original in originals){
                uint id=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(original.GetAttribute("id").ToLowerInvariant());
                var module=modules.Single(m=>System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(captured.Bytes.AsSpan(m.Native+4))==id);
                // Reborn: direct metadata anchors independently prove every scalar field, including zero/default values and the final byte flags.
                int ownTable=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(captured.Bytes.AsSpan(module.Wrapper+8));var ownFields=GameObjectStockReview.Tokens(captured.Bytes,ownTable);
                // Reborn: the literal tokenizer tags are also proved here so a mismatch names the field instead of failing later in the grammar.
                // Reborn: retail logic-frame counters use the plain uint signature, unlike tagged DurationUnsignedInt values.
                foreach(var (field,tag) in new[]{("AnnouncementEventEnemy",0xC4EC43B1u),("AnnouncementEventAlly",0xC4EC43B1u),("AnnouncementEventOwner",0xC4EC43B1u),("DelayBeforeAnnouncementLogicFrames",0x763177A4u),("OnlyIfVisible",0x77E3F4F7u),("CountAsFirstSightedAnnouncement",0x77E3F4F7u),("UseObjectsPosition",0x77E3F4F7u),("CreateFakeRadarEvent",0x77E3F4F7u)}){
                    if(!ownFields.TryGetValue(BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(field),out var token))throw new InvalidDataException("EVA-create client retail field absent: "+field);
                    int actual=token.Offset-module.Native;int expected=Marshal.OffsetOf<Ep1EvaAnnounceClientCreateModuleData>(field).ToInt32();
                    if(actual!=expected)throw new InvalidDataException("EVA-create client retail offset differs: "+field+" actual="+actual+", expected="+expected);
                    if(token.Type!=tag)throw new InvalidDataException($"EVA-create client retail type tag differs: {field} actual={token.Type:X8}, expected={tag:X8}");
                }
                if(SdkBuildTests.Enabled){
                    var source=(XmlDocument)baseline.CloneNode(true);var updates=source.GetElementsByTagName("ClientUpdates",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();updates.RemoveAll();updates.AppendChild(source.ImportNode(original,true));
                    var resolved=SchemaNativeContract.Resolve(source,schemas,catalog).GetElementsByTagName("EvaAnnounceClientCreate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                    SchemaNativeContract.VerifyStock(resolved,typeof(Ep1EvaAnnounceClientCreateModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
                }
                grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(Ep1EvaAnnounceClientCreateModuleData));count++;
            }
        }
        SdkBuildTests.MetadataProgress("EvaAnnounceClientCreate",$"Reborn: EvaAnnounceClientCreate EP1 native/metadata qualification: PASS ({count} original retail records; header={Marshal.SizeOf<Ep1EvaAnnounceClientCreateModuleData>()}).",count+" stock records");return grammar;
    }
}
