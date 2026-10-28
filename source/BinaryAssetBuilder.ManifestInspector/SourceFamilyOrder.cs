using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: original EA sources often interleave element families that the XSD declares as an ordered sequence; EA's compiler accepts
// them and fills each list in source order. Stable-sort them into XSD order (each family keeps its own order) before validation.
internal static class SourceFamilyOrder
{
    private static readonly (string Element,string[] Order)[] Families={
        ("LaserState",new[]{"LaserEndParticleSystem","LaserStartParticleSystem","EndOffset","ObjectStatusValidation"}),
        ("ScriptedModelDraw",new[]{"ModelConditionState","AnimationState","AttachModel","EmbedPortal"}),
        ("ModelConditionState",new[]{"Model","Material","Texture","WeaponFireFXBone","WeaponRecoilBone","WeaponMuzzleFlash","WeaponLaunchBone","ParticleSysBone","FXEvent","ShadowInfo","Turret","SubObject"}),
        ("AnimationState",new[]{"Animation","Script","FXEvent","LuaEvent","ParticleSysBone"})};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: only elements whose children are all known families are reordered; anything else is left for schema validation to report. */
    //-------------------------------------------------------------------------------------------------
    internal static void Normalize(XmlDocument source)
    {
        foreach(var family in Families)
            foreach(var leaf in source.GetElementsByTagName(family.Element,SchemaSet.XmlNamespace).OfType<XmlElement>().ToArray()){
                var children=leaf.ChildNodes.OfType<XmlElement>().ToArray();if(children.Any(e=>e.NamespaceURI!=SchemaSet.XmlNamespace||Array.IndexOf(family.Order,e.LocalName)<0))continue;
                var sorted=children.OrderBy(e=>Array.IndexOf(family.Order,e.LocalName)).ToArray();if(sorted.SequenceEqual(children))continue;
                foreach(var child in children)leaf.RemoveChild(child);foreach(var child in sorted)leaf.AppendChild(child);
            }
    }
}
