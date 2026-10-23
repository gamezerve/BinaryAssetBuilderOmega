using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: source-sized reviewed draw/client lists share container ownership, not a captured Conscript module count.
internal static class ConscriptVariableContainers
{
    internal static readonly string[] Names={"Draws","ClientUpdates","ClientBehaviors"};
    // Reborn: only independently stock-qualified pointer-list families use this variable-container contract.
    internal static bool Supports(string name)=>Names.Contains(name,StringComparer.Ordinal);
    // Reborn: direct-owner selection never confuses nested module children with root containers.
    internal static XmlElement? Find(XmlDocument source,string name)=>source.DocumentElement!.ChildNodes.OfType<XmlElement>().SingleOrDefault(e=>e.LocalName==name);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: restore baseline topology only in an owned comparison copy, preserving source order relative to neighboring root fields. */
    //-------------------------------------------------------------------------------------------------
    internal static void Restore(XmlDocument source,XmlDocument baseline)
    {
        foreach(string name in Names){
            var actual=Find(source,name);var model=Find(baseline,name);if(actual!=null)actual.ParentNode!.RemoveChild(actual);if(model==null)continue;
            var following=model.NextSibling;XmlElement? next=null;
            while(following!=null){if(following is XmlElement element&&(next=Find(source,element.LocalName))!=null)break;following=following.NextSibling;}
            source.DocumentElement!.InsertBefore(source.ImportNode(model,true),next);
        }
    }
}
