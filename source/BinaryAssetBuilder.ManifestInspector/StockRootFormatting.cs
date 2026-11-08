using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: formatting removal is limited to original XSD empty-content records; real string and mixed content remain untouched.
internal static class StockRootFormatting
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: follow the compiled original schema down the actual document and discard only forbidden indentation characters. */
    //-------------------------------------------------------------------------------------------------
    internal static void Normalize(XmlElement root,XmlSchemaSet schemas)
    {
        if(schemas.GlobalTypes[new XmlQualifiedName(root.LocalName,root.NamespaceURI)] is XmlSchemaType type)Visit(root,type);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: an empty complex record can contain comments, but cannot contain formatting text at runtime schema validation. */
    //-------------------------------------------------------------------------------------------------
    private static void Visit(XmlElement element,XmlSchemaType type)
    {
        if(type is not XmlSchemaComplexType complex)return;
        if(complex.ContentType==XmlSchemaContentType.Empty)
            foreach(var blank in element.ChildNodes.Cast<XmlNode>().Where(n=>n.NodeType is XmlNodeType.Whitespace or XmlNodeType.SignificantWhitespace||n.NodeType==XmlNodeType.Text&&string.IsNullOrWhiteSpace(n.Value)).ToArray())element.RemoveChild(blank);
        var declarations=Elements(complex.ContentTypeParticle).ToArray();
        foreach(var child in element.ChildNodes.OfType<XmlElement>())
        {
            var declaration=declarations.FirstOrDefault(d=>d.QualifiedName.Name==child.LocalName&&d.QualifiedName.Namespace==child.NamespaceURI);
            if(declaration?.ElementSchemaType is XmlSchemaType childType)Visit(child,childType);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve original sequence/choice boundaries while finding a validated child declaration. */
    //-------------------------------------------------------------------------------------------------
    private static IEnumerable<XmlSchemaElement> Elements(XmlSchemaParticle particle)
    {
        if(particle is XmlSchemaElement element)yield return element;
        else if(particle is XmlSchemaGroupBase group)
            foreach(var item in group.Items.OfType<XmlSchemaParticle>())foreach(var child in Elements(item))yield return child;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove formatting-only cleanup while retaining actual string text, comments and invalid non-whitespace/unknown content. */
    //-------------------------------------------------------------------------------------------------
    internal static void SelfTest()
    {
        var schemas=ConscriptSourceInput.Schemas();
        string prefix="<GameObject xmlns='uri:ea.com:eala:asset' id='RebornFormattingProof'><DisplayName>  Keep this text  </DisplayName><Behaviors><ToppleUpdate id='RebornTopple'>";
        string suffix="</ToppleUpdate></Behaviors><Body><PropBody id='RebornBody'/></Body></GameObject>";
        foreach(string text in new[]{"  \n\t ","  <!-- Reborn: retain source comments. --> \n"})
        {
            var document=SdkXmlDiagnostics.Parse(System.Text.Encoding.UTF8.GetBytes(prefix+text+suffix));
            Normalize(document.DocumentElement!,schemas);document.Schemas.Add(schemas);document.Validate((_,error)=>throw SdkXmlDiagnostics.SchemaError(error));
            if(document.GetElementsByTagName("DisplayName",SchemaSet.XmlNamespace)[0]!.InnerText!="  Keep this text  ")throw new InvalidDataException("Formatting changed real string content.");
            if(text.Contains("<!--")&&!document.OuterXml.Contains("retain source comments"))throw new InvalidDataException("Formatting removed a comment.");
        }
        foreach(string text in new[]{"not whitespace","<UnknownField/>"})
        {
            var document=SdkXmlDiagnostics.Parse(System.Text.Encoding.UTF8.GetBytes(prefix+text+suffix));Normalize(document.DocumentElement!,schemas);document.Schemas.Add(schemas);
            bool rejected=false;document.Validate((_,_)=>rejected=true);
            if(!rejected)throw new InvalidDataException("Formatting admitted invalid empty-record content.");
        }
        Console.WriteLine("Reborn: source formatting proof passed: 2 positive and 2 negative cases; string/comment contents retained.");
    }}
