using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: apply one fingerprint-pinned diagnostic Shield normalization to owned memory only; never rewrite reference or staged schemas.
internal static class SdkShieldSchemaCandidate
{
    // Reborn: every candidate transformation retains before/after evidence and cannot be confused with an official schema hash.
    internal sealed record Change(string SchemaFile,string BeforeSha256,string AfterSha256,string KeptBase,string RemovedBase,string Rule);
    internal const string Profile = "diagnostic-shield-first-v1";
    internal const string FileName = "modules/shieldsphereupdate.xsd";
    internal const string ExpectedHash = "DA4B7000A8B24F201ADDF5F1C597FA4036206E089372B21F1A2AD6C211442A80";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the Sphere-based first declaration supported by the existing 312/328-byte shield model proofs; reject any unreviewed input bytes. */
    //-------------------------------------------------------------------------------------------------
    internal static Change Apply(Dictionary<string,byte[]> snapshots)
    {
        if (!snapshots.TryGetValue(FileName,out byte[]? bytes) || Convert.ToHexString(SHA256.HashData(bytes)) != ExpectedHash) throw new InvalidDataException("Shield candidate requires exact reviewed source fingerprint.");
        XmlDocument xml = new() { XmlResolver = null,PreserveWhitespace = true }; using MemoryStream input = new(bytes,false);
        using XmlReader reader = XmlReader.Create(input,new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit,XmlResolver = null,MaxCharactersInDocument = 2*1048576 }); xml.Load(reader);
        XmlNamespaceManager namespaces = new(xml.NameTable); namespaces.AddNamespace("xs","http://www.w3.org/2001/XMLSchema");
        XmlNodeList declarations = xml.SelectNodes("/xs:schema/xs:complexType[@name='ShieldSphereUpdateModuleData']",namespaces)!;
        if (declarations.Count != 2 || declarations[0]!.SelectSingleNode("xs:complexContent/xs:extension",namespaces)?.Attributes?["base"]?.Value != "SphereModuleUpdateModuleData"
            || declarations[1]!.SelectSingleNode("xs:complexContent/xs:extension",namespaces)?.Attributes?["base"]?.Value != "UpdateModuleData") throw new InvalidDataException("Reviewed Shield declaration structure differs.");
        xml.DocumentElement!.RemoveChild(declarations[1]!);
        using MemoryStream output = new();
        using (XmlWriter writer = XmlWriter.Create(output,new XmlWriterSettings { Encoding = new UTF8Encoding(false),Indent = false,NewLineHandling = NewLineHandling.None })) xml.Save(writer);
        byte[] candidate = output.ToArray(); snapshots[FileName] = candidate;
        return new(FileName,ExpectedHash,Convert.ToHexString(SHA256.HashData(candidate)),"SphereModuleUpdateModuleData","UpdateModuleData",
            "Remove only the second duplicate ShieldSphereUpdateModuleData declaration in owned memory; preserve original reference bytes and catalog digest.");
    }
}
