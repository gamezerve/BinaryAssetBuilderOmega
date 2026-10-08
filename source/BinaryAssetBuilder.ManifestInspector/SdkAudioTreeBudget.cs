using System.Xml;
using System.Xml.Schema;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: admit document breadth only for bounded shallow audio owners, never globally widen arbitrary asset trees.
internal sealed class SdkAudioTreeBudget
{
    private const string Ea = "uri:ea.com:eala:asset";
    internal const int ElementLimit = 16384;
    private long work;
    private readonly int originalElements,owners,maxOwner;
    // Reborn: element-pair units are a diagnostic budget, not a full CPU cost, elapsed-time or native compilation proof.
    internal sealed record Evidence(int OriginalElements,int Owners,int LargestOriginalOwner,int FinalElements,long MergePairUnits,int ElementLimit,int OwnerElementLimit,int OwnerLimit,int DepthLimit,long PairUnitLimit,int OwnerNodeLimit);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve old 8192 behavior unless explicit broad audio input passes every owner/schema/metadata/depth limit. */
    //-------------------------------------------------------------------------------------------------
    internal static SdkAudioTreeBudget? Prove(XmlElement root,XmlElement[] assets,XmlSchemaSet schemas)
    {
        int count = root.SelectNodes(".//*")!.Count;
        if (count <= 8192) return null;
        if (count > ElementLimit || assets.Length is < 1 or > 2048 || count-assets.Sum(Size) > 1024)
            throw new InvalidDataException("Audio breadth requires at most 16384 elements, 2048 owners and 1024 metadata elements.");
        int maximum = 0;
        foreach (var asset in assets)
        {
            if (asset.NamespaceURI != Ea || asset.Prefix.Length != 0 || asset.LocalName is not ("AudioEvent" or "AudioEventOverridable" or "Multisound" or "MusicTrack")
                || schemas.GlobalTypes[new XmlQualifiedName(asset.LocalName,Ea)] is not XmlSchemaComplexType type || type.AttributeWildcard != null)
                throw new InvalidDataException("Broad audio documents require only reviewed named audio owners without wildcard attributes.");
            maximum = Math.Max(maximum,Check(asset,0));
        }
        return new(count,assets.Length,maximum);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain document budget identity while counting actual resolved-base joins separately from raw source breadth. */
    //-------------------------------------------------------------------------------------------------
    private SdkAudioTreeBudget(int elements,int ownerCount,int maximum) { originalElements = elements; owners = ownerCount; maxOwner = maximum; }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: charge bounded resolved-base/source node pairs before actual Core joining; reject aggregate amplification work. */
    //-------------------------------------------------------------------------------------------------
    internal void Charge(XmlElement basis,XmlElement owner)
    {
        int left = Check(basis,0),right = Check(owner,0);
        work += (long)left*right;
        if (work > 1048576) throw new InvalidDataException("Audio merge pair-work exceeds 1048576 units.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: publish breadth evidence only after complete Core output still satisfies total and individual owner bounds. */
    //-------------------------------------------------------------------------------------------------
    internal Evidence Verify(XmlElement output)
    {
        int count = output.SelectNodes(".//*")!.Count;
        if (count > ElementLimit) throw new InvalidDataException("Final audio tree exceeds 16384 elements.");
        foreach (var asset in output.ChildNodes.OfType<XmlElement>().Where(node => node.LocalName is not ("Tags" or "Includes" or "Defines"))) Check(asset,0);
        return new(originalElements,owners,maxOwner,count,work,ElementLimit,128,2048,8,1048576,512);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bound every audio owner to 128 elements, eight child levels and 64 attributes per element before copying/joining. */
    //-------------------------------------------------------------------------------------------------
    private static int Check(XmlElement node,int depth)
    {
        // Reborn: element counts alone cannot bound comment/text density; retain a separate total-node owner cap.
        if (depth == 0 && 1+node.SelectNodes(".//node()")!.Count > 512) throw new InvalidDataException("Audio owner exceeds 512 total XML nodes.");
        if (depth > 8 || node.Attributes.Count > 64) throw new InvalidDataException("Audio owner exceeds eight child levels or 64 attributes per element.");
        int count = 1;
        foreach (var child in node.ChildNodes.OfType<XmlElement>())
        {
            count += Check(child,depth+1);
            if (count > 128) throw new InvalidDataException("Audio owner exceeds 128 elements.");
        }
        return count;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: measure asset element count including the root while keeping metadata outside the owner slice. */
    //-------------------------------------------------------------------------------------------------
    private static int Size(XmlElement asset) => 1+asset.SelectNodes(".//*")!.Count;
}
