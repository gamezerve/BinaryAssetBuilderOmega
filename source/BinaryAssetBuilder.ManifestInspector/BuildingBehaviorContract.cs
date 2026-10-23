using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: BuildingBehavior serializes optional model subobject names, not new asset references or executable behavior code.
internal static class BuildingBehaviorContract
{
    internal static readonly string[] Attributes={"NightWindowObject","FireWindowObject","GlowWindowObject","NightFireWindowObject"};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject only unrepresentable text early, preserving optional and explicitly empty source values. */
    //-------------------------------------------------------------------------------------------------
    internal static void Validate(XmlElement owner)
    {
        foreach(string name in Attributes){var attribute=owner.GetAttributeNode(name);if(attribute!=null&&attribute.Value.Any(c=>c>127||c=='\0'))throw SdkXmlDiagnostics.At(new InvalidDataException("BuildingBehavior model subobject name must be ASCII without NUL."),attribute);}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: comparison templates select the class, not fixed subobject names or attribute presence. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlElement source,XmlElement baseline)
    {
        foreach(string name in Attributes){source.RemoveAttribute(name);if(baseline.HasAttribute(name))source.SetAttribute(name,baseline.GetAttribute(name));}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify both original Power Plant modules against stock native bytes, local IDs, inherited metadata and owned strings. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Qualify(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<BuildingBehaviorModuleData>()!=40)throw new InvalidDataException("BuildingBehavior native header is not forty bytes.");
        var captured=GameObjectStockReview.Capture(manifest,"GameObject:SovietPowerPlant");var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Where(m=>m.Type=="BuildingBehaviorModuleData").ToArray();
        var originals=GameObjectExtendedRootContract.Original("Soviet/Structures/SovietPowerPlant.xml","BuildingBehavior");if(modules.Length==0||modules.Length!=originals.Length)throw new InvalidDataException("BuildingBehavior stock/source witness count differs.");var grammar=new StockMetadataGrammar();
        foreach(var original in originals){uint local=FastHash.GetHashCode(original.GetAttribute("id").ToLowerInvariant());var module=modules.Single(m=>BinaryPrimitives.ReadUInt32LittleEndian(captured.Bytes.AsSpan(m.Native+4,4))==local);if(module.TypeId!=FastHash.GetHashCode("BuildingBehaviorModuleData"))throw new InvalidDataException("BuildingBehavior stock class identity differs.");
            var document=(XmlDocument)baseline.CloneNode(true);var behaviors=document.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(e=>e.LocalName=="Behaviors");behaviors.RemoveAll();behaviors.AppendChild(document.ImportNode(original,true));var resolved=SchemaNativeContract.Resolve(document,schemas,catalog);var element=resolved.GetElementsByTagName("BuildingBehavior",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
            SchemaNativeContract.VerifyStock(element,typeof(BuildingBehaviorModuleData),module.TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(BuildingBehaviorModuleData));
        }
        return grammar;
    }
}
