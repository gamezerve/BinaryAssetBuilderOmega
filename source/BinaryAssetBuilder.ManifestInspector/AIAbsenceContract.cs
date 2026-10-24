using System.Buffers.Binary;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: GameObjects without an AI container (many structures). Stock witness: GameObject:SovietCrane, whose original source has no AI:
// its native inline AI list is empty (count +480 = 0, items +484 = 0, no relocation) and its root keeps a zero-count "AI" declaration.
internal static class AIAbsenceContract
{
    internal const uint Wire=0x5FD8691D;
    private const string Witness="SovietCrane";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: comparison copies only: drop the baseline AI container when the authored object declares none. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlDocument copy,XmlDocument comparison)
    {
        if(Containers(copy).Length!=0)return;
        foreach(var ai in Containers(comparison))ai.ParentNode!.RemoveChild(ai);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: comparison reference tables use stock occurrence coordinates; reinsert the baseline AI container at its baseline position. */
    //-------------------------------------------------------------------------------------------------
    internal static void RestoreForReferences(XmlDocument document,XmlDocument baseline)
    {
        if(Containers(document).Length!=0)return;var model=Containers(baseline);if(model.Length!=1)return;
        var next=model[0].NextSibling;while(next!=null&&next is not XmlElement)next=next.NextSibling;
        var anchor=next is XmlElement following?document.DocumentElement!.ChildNodes.OfType<XmlElement>().FirstOrDefault(e=>e.LocalName==following.LocalName):null;
        var imported=document.ImportNode(model[0],true);if(anchor!=null)document.DocumentElement!.InsertBefore(imported,anchor);else document.DocumentElement!.AppendChild(imported);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove the absent-AI native and metadata shape on the original object before an authored AI-less object is accepted. */
    //-------------------------------------------------------------------------------------------------
    internal static bool Qualify(string manifest)
    {
        if(GameObjectExtendedRootContract.Original("Soviet/Structures/"+Witness+".xml","GameObject").Single(e=>e.GetAttribute("id")==Witness).ChildNodes.OfType<XmlElement>().Any(e=>e.LocalName=="AI"))throw new InvalidDataException("AI-absence witness source unexpectedly declares AI: "+Witness);
        var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+Witness);var rows=GameObjectStockReview.Tokens(captured.Bytes,0);int root=captured.Projection.NativeRoot;
        if(!rows.TryGetValue(FastHash.GetHashCode("AI"),out var row))throw new InvalidDataException("Stock AI-less object has no AI root declaration: "+Witness);
        if(row.Type!=Wire||row.Offset<=0||Word(captured.Bytes,row.Offset)!=0x6C827073||Word(captured.Bytes,row.Offset+4)!=0x5D448FD8||Word(captured.Bytes,row.Offset+8)!=root+480)throw new InvalidDataException("Stock AI-less declaration differs: "+Witness);
        if(Word(captured.Bytes,root+480)!=0||Word(captured.Bytes,root+484)!=0)throw new InvalidDataException("Stock AI-less native list is not empty: "+Witness);
        Console.WriteLine("Reborn: EP1 GameObject without AI stock qualification: PASS ("+Witness+": empty inline AI list, zero-count declaration).");
        return true;
    }

    private static XmlElement[] Containers(XmlDocument document)=>document.DocumentElement!.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="AI").ToArray();
    private static uint Word(byte[] b,int at){if(at<0||at>b.Length-4)throw new InvalidDataException("AI-absence witness bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
}
