using System.Buffers.Binary;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: source-authored equivalence lists are weak GameObject IDs, not copied stock arrays or generated commands.
internal static class GameObjectEquivalentToContract
{
    internal static XmlElement[] Entries(XmlDocument source)=>source.DocumentElement!.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="EquivalentTo").ToArray();
    //-------------------------------------------------------------------------------------------------
    /** Reborn: comparison copies omit the separately proved list without changing real source order or contents. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlDocument source){foreach(var entry in Entries(source))entry.ParentNode!.RemoveChild(entry);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare each native weak word with its authored GameObject target, preserving duplicates and ordering as the XSD permits. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlDocument source,byte[] bytes,Ep1GameObjectEquivalentTo.Layout layout)
    {
        var entries=Entries(source);if(entries.Length!=layout.Count)throw new InvalidDataException("EquivalentTo source/native count differs.");
        for(int i=0;i<entries.Length;i++){string id=entries[i].InnerText.Trim();if(id.StartsWith("GameObject:",StringComparison.Ordinal))id=id[11..];if(!ConscriptSourceInput.Identity(id))throw SdkXmlDiagnostics.At(new InvalidDataException("EquivalentTo requires a valid GameObject identity."),entries[i]);if(Word(bytes,layout.Items+i*4)!=InstanceHandle.GetInstanceId(id))throw SdkXmlDiagnostics.At(new InvalidDataException("EquivalentTo native ID differs from authored target: "+id),entries[i]);}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify the original naval Miner list and its direct weak-word metadata targets before authored use. */
    //-------------------------------------------------------------------------------------------------
    internal static bool Qualify(string manifest)
    {
        var stock=GameObjectStockReview.Capture(manifest,"GameObject:SovietMiner_Naval");int root=stock.Projection.NativeRoot;var layout=Ep1GameObjectEquivalentTo.Inspect(stock.Bytes,Array.Empty<uint>(),false,root);var source=new XmlDocument();source.LoadXml("<GameObject xmlns='uri:ea.com:eala:asset'/>");
        foreach(var entry in GameObjectExtendedRootContract.Original("Soviet/Units/SovietMiner.xml","EquivalentTo"))source.DocumentElement!.AppendChild(source.ImportNode(entry,true));Verify(source,stock.Bytes,layout);
        var row=GameObjectStockReview.Tokens(stock.Bytes,0)[FastHash.GetHashCode("EquivalentTo")];int table=row.Offset;if(row.Type!=0x148ED75Du||layout.Count==0||Word(stock.Bytes,table)!=0x6C827073||Word(stock.Bytes,table+4)!=0x5D448FD8||Word(stock.Bytes,table+8)!=root+452)throw new InvalidDataException("Original naval Miner equivalence metadata differs.");
        for(int i=0;i<layout.Count;i++)if(Word(stock.Bytes,table+12+i*12)!=0xB151A0EB||Word(stock.Bytes,table+16+i*12)!=0||Word(stock.Bytes,table+20+i*12)!=layout.Items+i*4)throw new InvalidDataException("Original equivalence item metadata differs.");
        return true;
    }
    // Reborn: all original/generated metadata word reads are confined to their asset buffer.
    private static uint Word(byte[] b,int at){if(at<0||at>b.Length-4)throw new InvalidDataException("EquivalentTo proof bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
}
