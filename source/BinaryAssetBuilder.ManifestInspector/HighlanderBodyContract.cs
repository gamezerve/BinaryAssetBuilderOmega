using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Schema;
using SageBinaryData;
namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: the retained legacy Highlander marker has no own native fields; its ActiveBody base keeps full EP1 source/native proof.
internal static class HighlanderBodyContract
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: admit only the exact fieldless official extension and disclose that its game behavior lacks a populated retail witness. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(XmlSchemaSet schemas)
    {
        var schema=schemas.GlobalTypes[new XmlQualifiedName("HighlanderBodyModuleData","uri:ea.com:eala:asset")] as XmlSchemaComplexType;
        var extension=(schema?.ContentModel as XmlSchemaComplexContent)?.Content as XmlSchemaComplexContentExtension;
        if(extension?.BaseTypeName.Name!="ActiveBodyModuleData"||extension.Attributes.Count!=0||extension.Particle is not XmlSchemaSequence sequence||sequence.Items.Count!=0||Marshal.SizeOf<Ep1ActiveBodyModuleData>()!=76)
            throw new InvalidDataException("HighlanderBody must remain the fieldless official EP1 ActiveBody extension.");
        Console.WriteLine("Warning: LEGACYMODULE ' HighlanderBody ' uses the official fieldless extension and the retail-qualified EP1 ActiveBody base; no populated Uprising retail witness was found. In-game Highlander behavior is not independently verified.");
    }
}
