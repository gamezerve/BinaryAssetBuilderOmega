using System.Text;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: characterize duplicate singleton upgrade requirements without admitting generic source normalization.
internal static class SdkUpgradeSemanticsSmokeTest
{
    // Reborn: focused review witnesses are XML/schema observations, never full-source admission or native game compatibility.
    internal sealed record Witness(string Id,int RawBranches,int ProcessedBranches,string Requirement,string Condition,bool RawValidated,bool ProcessedValidated,string ProcessedSha256);
    // Reborn: publish explicit partial scope and untouched source identity without any output files or production admission.
    internal sealed record ReviewResult(string SourcePath,string RawSha256,Witness[] Witnesses,bool ReadOnly = true,bool PartialOwnerReview = true,bool ProductionBuildReady = false);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: review only the two known authored upgrade shapes against the pinned effective schema and unchanged core, with bounded rechecked input. */
    //-------------------------------------------------------------------------------------------------
    internal static ReviewResult Review(string path)
    {
        if (!Path.IsPathFullyQualified(path) || !File.Exists(path) || new FileInfo(path).Length is < 1 or > 4*1048576) throw new InvalidDataException("Absolute existing upgrade XML up to 4 MiB required.");
        path = Path.GetFullPath(path); byte[] raw = ReadBounded(path); string hash = Convert.ToHexString(SHA256.HashData(raw));
        XmlSchemaSet? schemas = null; var schema = SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:set => schemas = set);
        if (!schema.SchemaAdmitted || schemas == null) throw new InvalidDataException("Pinned reviewed schema required.");
        var xml = new XmlDocument { XmlResolver = null };
        using (var reader = XmlReader.Create(new MemoryStream(raw),new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit,XmlResolver = null,MaxCharactersInDocument = 4*1048576 })) xml.Load(reader);
        if (xml.DocumentElement?.LocalName != "AssetDeclaration" || xml.DocumentElement.NamespaceURI != "uri:ea.com:eala:asset") throw new InvalidDataException("EA AssetDeclaration required.");
        var assets = xml.DocumentElement.ChildNodes.OfType<XmlElement>().Where(asset => asset.LocalName == "UpgradeTemplate" && asset.NamespaceURI == "uri:ea.com:eala:asset").ToArray();
        XmlElement basis = assets.Single(asset => asset.GetAttribute("id") == "BasePurchasableUpgrade");
        if (basis.ChildNodes.OfType<XmlElement>().Any() || basis.HasAttribute("inheritFrom")) throw new InvalidDataException("Reviewed empty local upgrade base required.");
        List<Witness> witnesses = new();
        foreach (var target in new[] { (Id:"Upgrade_AlliedTech2",Leaf:"RequiredObject",Value:"AlliedRefinery"),(Id:"Upgrade_AlliedTech3",Leaf:"NeededUpgrade",Value:"Upgrade_AlliedTech2") })
        {
            var owner = assets.Single(asset => asset.GetAttribute("id") == target.Id);
            var branches = owner.ChildNodes.OfType<XmlElement>().ToArray();
            if (owner.GetAttribute("inheritFrom") != "BasePurchasableUpgrade" || branches.Length != 2
                || branches.Any(branch => branch.LocalName != "GameDependency" || branch.NamespaceURI != "uri:ea.com:eala:asset")
                || branches[0].Attributes.OfType<XmlAttribute>().Any(attribute => attribute.NamespaceURI != "http://www.w3.org/2000/xmlns/")
                || branches[0].ChildNodes.OfType<XmlElement>().Count() != 1 || branches[0].FirstChild is not XmlElement leaf
                || leaf.LocalName != target.Leaf || leaf.InnerText != target.Value || leaf.Attributes.Count != 0
                || branches[1].ChildNodes.OfType<XmlElement>().Any() || branches[1].GetAttribute("ForbiddenModelConditions") != "STRUCTURE_UNPACKING"
                || branches[1].Attributes.OfType<XmlAttribute>().Any(attribute => attribute.NamespaceURI != "http://www.w3.org/2000/xmlns/" && attribute.Name != "ForbiddenModelConditions"))
                throw new InvalidDataException("Upgrade source differs from the reviewed complementary pair.");
            var derived = (XmlElement)owner.CloneNode(true);
            // Reborn: reproduce declaration loading's consumed marker only for this isolated local-owner experiment, not generic graph admission.
            derived.RemoveAttribute("inheritFrom");
            var merged = (XmlElement)NodeJoiner.Override(schemas,xml,basis,derived);
            var resultBranches = merged.ChildNodes.OfType<XmlElement>().ToArray();
            Require(resultBranches.Length == 1 && resultBranches[0].GetAttribute("ForbiddenModelConditions") == "STRUCTURE_UNPACKING"
                && resultBranches[0].ChildNodes.OfType<XmlElement>().Count() == 1 && resultBranches[0].FirstChild is XmlElement resultLeaf
                && resultLeaf.LocalName == target.Leaf && resultLeaf.InnerText == target.Value,"Real upgrade requirement/condition was lost during core folding.");
            byte[] processed = Source(merged.OuterXml);
            var rawBinding = SdkEffectiveSchema.Bind(schemas,path,Source(owner.OuterXml)); var processedBinding = SdkEffectiveSchema.Bind(schemas,path,processed);
            Require(!rawBinding.XmlValidated && processedBinding.XmlValidated,"Real upgrade raw/processed schema contract changed.");
            witnesses.Add(new(target.Id,branches.Length,resultBranches.Length,target.Value,"STRUCTURE_UNPACKING",rawBinding.XmlValidated,processedBinding.XmlValidated,Convert.ToHexString(SHA256.HashData(processed))));
        }
        if (new FileInfo(path).Length != raw.Length || Convert.ToHexString(SHA256.HashData(ReadBounded(path))) != hash) throw new InvalidDataException("Upgrade source changed during focused review.");
        return new(path,hash,witnesses.ToArray());
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: execute actual core singleton folding and expose validation-hidden attribute conflicts while keeping diagnostic admission closed. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        const string xsd = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'><xs:simpleType name='FileReference'><xs:restriction base='xs:anyURI'/></xs:simpleType><xs:complexType name='BaseInheritableAsset'><xs:attribute name='id' type='xs:string' use='required'/><xs:attribute name='inheritFrom' type='xs:string'/></xs:complexType><xs:complexType name='GameDependencyType'><xs:sequence><xs:element name='RequiredObject' type='xs:string' minOccurs='0' maxOccurs='unbounded'/><xs:element name='ForbiddenUpgrade' type='xs:string' minOccurs='0' maxOccurs='unbounded'/><xs:element name='NeededUpgrade' type='xs:string' minOccurs='0' maxOccurs='unbounded'/></xs:sequence><xs:attribute name='ForbiddenModelConditions' type='xs:string'/></xs:complexType><xs:complexType name='UpgradeTemplate'><xs:complexContent><xs:extension base='BaseInheritableAsset'><xs:sequence><xs:element name='GameDependency' type='GameDependencyType' minOccurs='0' maxOccurs='1'/></xs:sequence><xs:attribute name='LocalPlayerBuildOnHoldEvaEvent' type='xs:string'/></xs:extension></xs:complexContent></xs:complexType><xs:element name='AssetDeclaration'><xs:complexType><xs:sequence><xs:element name='UpgradeTemplate' type='UpgradeTemplate' maxOccurs='unbounded'/></xs:sequence></xs:complexType></xs:element></xs:schema>";
        var schemas = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(xsd) },"entry.xsd").Schemas;
        const string required = "<GameDependency><RequiredObject>AlliedRefinery</RequiredObject></GameDependency>";
        const string condition = "<GameDependency ForbiddenModelConditions='STRUCTURE_UNPACKING'/>";
        foreach (string payload in new[] { required,"<GameDependency><NeededUpgrade>Upgrade_AlliedTech2</NeededUpgrade></GameDependency>" })
        {
            byte[] raw = Source("<UpgradeTemplate id='Base'/><UpgradeTemplate id='Owner' inheritFrom='Base'>"+payload+condition+"</UpgradeTemplate>");
            Require(!Valid(raw),"Duplicate singleton source unexpectedly validated.");
            XmlElement merged = Merge("",payload+condition);
            var branches = merged.ChildNodes.OfType<XmlElement>().ToArray();
            Require(branches.Length == 1 && branches[0].GetAttribute("ForbiddenModelConditions") == "STRUCTURE_UNPACKING" && branches[0].ChildNodes.OfType<XmlElement>().Count() == 1 && branches[0].InnerText == (payload == required ? "AlliedRefinery" : "Upgrade_AlliedTech2") && Valid(Source(merged.OuterXml)),"Core did not fold complementary singleton requirements without losing content.");
            Require(Merge("",condition+payload).OuterXml == merged.OuterXml,"Complementary singleton folding became order-sensitive.");
            var rejected = SdkSelfAttributeInheritance.Apply(schemas,raw,filters:true);
            Require(rejected.Bytes == null && rejected.Evidence.ProcessedSha256 == null && rejected.Evidence.Overlays.Length == 0 && rejected.Evidence.Filters.Length == 0,"Filter profile admitted duplicate singleton normalization or leaked evidence.");
        }
        // Reborn: successful final validation cannot detect a lost earlier condition when duplicate singleton attributes conflict.
        XmlElement conflict = Merge("","<GameDependency ForbiddenModelConditions='FIRST'/>"+condition);
        Require(conflict.FirstChild is XmlElement branch && branch.GetAttribute("ForbiddenModelConditions") == "STRUCTURE_UNPACKING" && Valid(Source(conflict.OuterXml)),"Core last-write-wins singleton attribute behavior changed.");
        // Reborn: repeated anonymous dependency leaves concatenate rather than deduplicate even though their parent is a singleton.
        XmlElement duplicates = Merge("",required+required);
        Require(duplicates.FirstChild!.ChildNodes.OfType<XmlElement>().Count() == 2 && Valid(Source(duplicates.OuterXml)),"Core repeated requirement multiplicity changed.");
        // Reborn: folded cardinality does not repair source/schema attribute naming drift.
        XmlElement drift = Merge("",required+condition,"UnknownEvaEvent='BuildOnHold'");
        Require(!Valid(Source(drift.OuterXml)),"Unknown EVA field was silently renamed or accepted by final schema validation.");
        Console.WriteLine("SDK upgrade semantics self-test: OK (raw duplicate-singleton rejection, actual core complementary folding/order, retained object/upgrade/condition requirements, validation-hidden last-write-wins conflicts, repeated leaf multiplicity, EVA-name drift and unchanged atomic filter-profile refusal)");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: exercise unvalidated owned singleton branches through the unchanged inheritance joiner. */
        //-------------------------------------------------------------------------------------------------
        XmlElement Merge(string before,string after,string attributes = "")
        {
            var xml = new XmlDocument { XmlResolver = null };
            xml.LoadXml(Encoding.UTF8.GetString(Source("<UpgradeTemplate id='Base'>"+before+"</UpgradeTemplate><UpgradeTemplate id='Owner' inheritFrom='Base' "+attributes+">"+after+"</UpgradeTemplate>")));
            var assets = xml.DocumentElement!.ChildNodes.OfType<XmlElement>().ToArray();
            return (XmlElement)NodeJoiner.Override(schemas,xml,assets[0],assets[1]);
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: independently validate raw and merged fixtures rather than equating core folding with production compatibility. */
        //-------------------------------------------------------------------------------------------------
        bool Valid(byte[] bytes)
        {
            var xml = new XmlDocument { XmlResolver = null,Schemas = schemas }; xml.LoadXml(Encoding.UTF8.GetString(bytes));
            bool valid = true; xml.Validate((_,_) => valid = false); return valid;
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: encode owned upgrade witnesses without editing reference XML or schemas. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Source(string body) => Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+body+"</AssetDeclaration>");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bound allocation through an opened stream and refuse growth beyond the captured length before publishing source evidence. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] ReadBounded(string path)
    {
        using var stream = File.OpenRead(path);
        if (stream.Length is < 1 or > 4*1048576) throw new InvalidDataException("Upgrade XML exceeds bounded review input.");
        byte[] bytes = new byte[(int)stream.Length]; stream.ReadExactly(bytes);
        if (stream.ReadByte() != -1) throw new InvalidDataException("Upgrade XML grew during bounded read.");
        return bytes;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop on changed singleton semantics before any future normalization admission. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
