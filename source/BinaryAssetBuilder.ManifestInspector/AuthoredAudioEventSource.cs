using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: admit one literal caller event against the frozen ordered audio pair, not a general source graph.
internal static class AuthoredAudioEventSource
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate raw authored structure before schema defaults or core reference normalization can hide unsupported input. */
    //-------------------------------------------------------------------------------------------------
    internal static string Validate(byte[] bytes,string ram,string streamed)
    {
        if (bytes.Length > 8192 || bytes.AsSpan().StartsWith(new byte[] { 0xEF,0xBB,0xBF })) throw new InvalidDataException("Authored event exceeds UTF-8 source bounds.");
        XmlDocument document = new() { XmlResolver = null };
        using (XmlReader reader = XmlReader.Create(new StringReader(new UTF8Encoding(false,true).GetString(bytes)),new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit,XmlResolver = null })) document.Load(reader);
        if (document.FirstChild is XmlDeclaration declaration && declaration.Encoding.Length > 0 && !declaration.Encoding.Equals("utf-8",StringComparison.OrdinalIgnoreCase)) throw new InvalidDataException("Authored event must declare UTF-8.");
        XmlElement? wrapper = document.DocumentElement;
        if (wrapper == null || wrapper.Name != "AssetDeclaration" || wrapper.NamespaceURI != "uri:ea.com:eala:asset"
            || wrapper.Attributes.Count != 1 || wrapper.GetAttribute("xmlns") != wrapper.NamespaceURI
            || wrapper.ChildNodes.OfType<XmlElement>().Count() != 1 || wrapper.ChildNodes.OfType<XmlElement>().Single() is not XmlElement root
            || root.Name != "AudioEvent" || root.Attributes.Count != 3 || !root.HasAttribute("id")
            || root.GetAttribute("Volume") != "60" || root.GetAttribute("Control") != "INTERRUPT")
            throw new InvalidDataException("Authored event requires one literal id/Volume=60/Control=INTERRUPT AudioEvent.");
        AudioFileDiagnosticIdentity.Validate(root.GetAttribute("id"));
        XmlElement[] sounds = root.ChildNodes.OfType<XmlElement>().ToArray();
        if (sounds.Length != 2 || sounds.Any(sound => sound.Name != "Sound" || sound.NamespaceURI != wrapper.NamespaceURI || sound.ChildNodes.OfType<XmlElement>().Any())
            || sounds[0].Attributes.Count != 0 || sounds[1].Attributes.Count != 1 || sounds[1].GetAttribute("Weight") != "800"
            || sounds[0].InnerText != "AudioFile:"+ram || sounds[1].InnerText != "AudioFile:"+streamed)
            throw new InvalidDataException("Authored event requires exact ordered RAM/streamed literal Sound references and weights.");
        // Reborn: schema validation rejects extra text/structure; only trusted checked-in schema includes can resolve.
        document.Schemas.XmlResolver = new XmlUrlResolver();
        document.Schemas.Add(wrapper.NamespaceURI,Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"AudioEventPipeline.xsd"));
        document.Validate((_,args) => throw new InvalidDataException(args.Message));
        return root.GetAttribute("id");
    }
}
