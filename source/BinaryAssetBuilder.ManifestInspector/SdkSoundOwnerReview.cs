using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: review complete pinned sound owners without normalizing source or expanding preprocessing admission.
internal static class SdkSoundOwnerReview
{
    private const string Ea = "uri:ea.com:eala:asset";
    private const string RawHash = "F054D586CAAB71C22AEEA42AA2A71CF91568913DA7FC50D5CE844AD191A2073C";
    private const string BaseHash = "69AF7CD3D9C7DDACA8B394AC7976CF4690BD2501E73716110E650950247D86EC";
    // Reborn: complete isolated-owner XML agreement remains distinct from source graph admission and native stream/game proof.
    internal sealed record Owner(string Id,string ProcessedSha256,string[] Fields,string[] Children,bool CompleteExplicitProjectionVerified,bool XmlValidated);
    internal sealed record Report(string SourcePath,string RawSha256,string BaseSourcePath,string BaseRawSha256,string BaseProcessedSha256,int ExpressionSubstitutions,Owner[] Owners,SdkInstanceInheritanceProfile.PreparedSource[] PreparedSources)
    {
        // Reborn: an isolated complete XML owner is not a complete source/dependency build or production emission claim.
        public bool ReadOnly => true;
        public bool SnapshotOnly => true;
        public bool ExistingProfileRefused => true;
        public bool WholeSourceValidated => false;
        public bool FullDependencyCoverage => false;
        public bool ProductionBuildReady => false;
        public bool NativeOwnerBytesVerified => false;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prepare the pinned direct base under existing guards and compare actual Core output with every independently predicted owner field/child. */
    //-------------------------------------------------------------------------------------------------
    internal static Report Review(string sourceRoot)
    {
        sourceRoot = SdkEnvironmentPreflight.DirectoryPath(sourceRoot);
        string entry = Path.Combine(sourceRoot,"Sounds","SoundEffects.xml"),basePath = Path.Combine(sourceRoot,"Sounds","BaseSoundEffect.xml");
        byte[] raw = SdkEnvironmentPreflight.Read(entry,4*1048576),baseRaw = SdkEnvironmentPreflight.Read(basePath,4*1048576);
        if (Hash(raw) != RawHash || Hash(baseRaw) != BaseHash) throw new InvalidDataException("Sound owner/base differs from pinned review snapshot.");
        string output = Path.Combine(Path.GetTempPath(),"Reborn-SoundOwner-NoOutput-"+Guid.NewGuid().ToString("N"));
        var environment = SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),sourceRoot,entry,output,Array.Empty<string>());
        var paths = SdkSourcePathAudit.Inspect(environment);
        XmlSchemaSet? schemas = null;
        var schema = SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:set => schemas = set);
        if (!schema.SchemaAdmitted || schemas == null || paths.StoppedAtLimit) throw new InvalidDataException("Admitted schema and complete bounded source inventory required.");
        var refused = new SdkInstanceInheritanceProfile(schemas,paths,soundOffsets:true).Apply(entry,raw);
        if (refused.Bytes != null || !refused.Evidence.Diagnostics.Any(message => message.Contains("Child occurrence bound exceeded before copying.",StringComparison.Ordinal)))
            throw new InvalidDataException("Existing sound singleton refusal changed; review is not an admission fallback.");
        var prepared = new SdkInstanceInheritanceProfile(schemas,paths,soundOffsets:true).Apply(basePath,baseRaw);
        if (prepared.Bytes == null) throw new InvalidDataException("Guarded sound base preparation failed.");
        var basis = Parse(prepared.Bytes).DocumentElement!.ChildNodes.OfType<XmlElement>().Single(node => node.LocalName == "AudioEvent" && node.GetAttribute("id") == "BaseSoundEffect");
        var originalBase = Parse(baseRaw).DocumentElement!.ChildNodes.OfType<XmlElement>().Single(node => node.LocalName == "AudioEvent" && node.GetAttribute("id") == "BaseSoundEffect");
        Verify(originalBase,basis);
        var expression = new SdkIncludeDefineProfile(paths,true,schemas,soundOffsets:true).Apply(entry,raw,beforeInheritance:true);
        if (expression.Bytes == null) throw new InvalidDataException("Pinned owner expression stage failed.");
        var xml = Parse(expression.Bytes); List<Owner> owners = new();
        foreach (string id in new[] { "BuildingInfiltrated1","StreetLampCrush" })
        {
            var owner = xml.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(node => node.LocalName == "AudioEvent" && node.GetAttribute("id") == id);
            var expected = Predict(basis,owner);
            var joining = (XmlElement)owner.CloneNode(true); joining.RemoveAttribute("inheritFrom");
            var merged = (XmlElement)NodeJoiner.Override(schemas,xml,basis,joining);
            Verify(expected,merged);
            byte[] processed = Source(merged.OuterXml);
            bool valid = SdkEffectiveSchema.Bind(schemas,entry,processed).XmlValidated;
            if (!valid) throw new InvalidDataException("Complete isolated sound owner fails final schema.");
            owners.Add(new(id,Hash(processed),Fields(merged),merged.ChildNodes.OfType<XmlElement>().Select(node => node.OuterXml).ToArray(),true,valid));
        }
        long rechecked = 0;
        foreach (var source in paths.Sources)
        {
            byte[] observed = SdkEnvironmentPreflight.Read(source.PhysicalPath,4*1048576); rechecked += observed.Length;
            if (rechecked > 32*1048576 || observed.Length != source.Bytes || Hash(observed) != source.Sha256) throw new InvalidDataException("Sound review source closure changed or exceeds post-recheck bound.");
        }
        if (Directory.Exists(output)) throw new InvalidDataException("Unexpected review output directory.");
        return new(entry,RawHash,basePath,BaseHash,prepared.Evidence.ProcessedSha256!,expression.Evidence.Substitutions,owners.ToArray(),prepared.Evidence.PreparedSources);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: predict only the two reviewed full literal child bodies and explicit base/owner scalar overlays, not generic duplicate normalization. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlElement Predict(XmlElement basis,XmlElement owner)
    {
        if (basis.Name != "AudioEvent" || owner.Name != "AudioEvent" || basis.NamespaceURI != Ea || owner.NamespaceURI != Ea
            || basis.GetAttribute("id") != "BaseSoundEffect" || basis.HasAttribute("inheritFrom") || basis.ChildNodes.OfType<XmlElement>().Any()
            || owner.GetAttribute("inheritFrom") != "AudioEvent:BaseSoundEffect") throw new InvalidDataException("Exact empty direct AudioEvent base and owner handle required.");
        string id = owner.GetAttribute("id");
        string original,final;
        if (id == "BuildingInfiltrated1")
        {
            original = "<PitchShift Low='-10' High='-5'/><InitialDelay Low='0' High='50'/><PitchShift Low='-1' High='1'/><Sound>WBSpy_infiltrateBldgP</Sound>";
            final = "<PitchShift Low='-1' High='1'/><InitialDelay Low='0' High='50'/><Sound>WBSpy_infiltrateBldgP</Sound>";
        }
        else if (id == "StreetLampCrush")
        {
            original = "<NonInterruptibleTime Low='0.0s' High='0.5s'/><PitchShift Low='-10' High='10'/><NonInterruptibleTime Low='0.0s' High='0.8s'/><Delay Low='0' High='100'/><Sound>WBStreetLamp_crushA</Sound><Sound>WBStreetLamp_crushB</Sound><Sound>WBStreetLamp_crushC</Sound>";
            final = "<PitchShift Low='-10' High='10'/><Delay Low='0' High='100'/><NonInterruptibleTime Low='0.0s' High='0.8s'/><Sound>WBStreetLamp_crushA</Sound><Sound>WBStreetLamp_crushB</Sound><Sound>WBStreetLamp_crushC</Sound>";
        }
        else throw new InvalidDataException("Only the two pinned sound owners are reviewed.");
        var originalChildren = Parse(Source("<AudioEvent>"+original+"</AudioEvent>")).DocumentElement!.FirstChild!;
        if (!Children(owner).SequenceEqual(Children(originalChildren),StringComparer.Ordinal)) throw new InvalidDataException("Reviewed complete authored sound child body changed.");
        var predicted = (XmlElement)Parse(Source("<AudioEvent>"+final+"</AudioEvent>")).DocumentElement!.FirstChild!;
        foreach (var source in new[] { basis,owner })
            foreach (XmlAttribute field in source.Attributes)
            {
                if (field.NamespaceURI == "http://www.w3.org/2000/xmlns/") continue;
                if (field.NamespaceURI.Length != 0 || field.Value.StartsWith('=') || field.Value.Length > 4096)
                    throw new InvalidDataException("Literal explicit owner/base fields required; directives and expressions remain closed in full projection.");
                if (field.Name != "inheritFrom") predicted.SetAttribute(field.Name,field.Value);
            }
        return predicted;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject any difference in explicit root fields, child QName/order/multiplicity/attributes or reference text before publishing owner evidence. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlElement expected,XmlElement actual)
    {
        if (expected.Name != actual.Name || expected.NamespaceURI != actual.NamespaceURI || !Fields(expected).SequenceEqual(Fields(actual),StringComparer.Ordinal)
            || !Children(expected).SequenceEqual(Children(actual),StringComparer.Ordinal)) throw new InvalidDataException("Complete sound owner projection differs from predicted fields/children.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain explicit attribute QName/presence with length-prefix encoding, ignoring only namespace declaration placement. */
    //-------------------------------------------------------------------------------------------------
    private static string[] Fields(XmlNode node) => node.Attributes!.OfType<XmlAttribute>().Where(field => field.NamespaceURI != "http://www.w3.org/2000/xmlns/")
        .OrderBy(field => field.NamespaceURI,StringComparer.Ordinal).ThenBy(field => field.Name,StringComparer.Ordinal).Select(field => Encode(field.NamespaceURI)+Encode(field.Name)+Encode(field.Value)).ToArray();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare bounded direct empty/text-only sound leaves; unexpected nested/text payload cannot disappear in a projection. */
    //-------------------------------------------------------------------------------------------------
    private static string[] Children(XmlNode owner)
    {
        var leaves = owner.ChildNodes.OfType<XmlElement>().ToArray();
        if (leaves.Length > 16 || owner.ChildNodes.OfType<XmlText>().Any(text => !string.IsNullOrWhiteSpace(text.Value))) throw new InvalidDataException("Bounded leaf-only sound owner required.");
        return leaves.Select(leaf => {
            if (leaf.ChildNodes.OfType<XmlElement>().Any() || leaf.InnerText.Length > 4096) throw new InvalidDataException("Nested sound leaf payload remains closed.");
            return Encode(leaf.NamespaceURI)+Encode(leaf.Name)+string.Concat(Fields(leaf).Select(Encode))+Encode(leaf.InnerText);
        }).ToArray();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: avoid ambiguous projection equality when XML values contain separators. */
    //-------------------------------------------------------------------------------------------------
    private static string Encode(string text) => text.Length+":"+text;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: parse only bounded source bytes with external entities and DTDs disabled. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Parse(byte[] bytes)
    {
        var xml = new XmlDocument { XmlResolver = null };
        using var reader = XmlReader.Create(new MemoryStream(bytes),new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit,XmlResolver = null,MaxCharactersInDocument = 4*1048576 }); xml.Load(reader); return xml;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: wrap isolated owners in memory without changing reference files or producing streams. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Source(string body) => Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='"+Ea+"'>"+body+"</AssetDeclaration>");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: XML snapshot hashes are not native asset identities or game compatibility proof. */
    //-------------------------------------------------------------------------------------------------
    private static string Hash(byte[] bytes) => Convert.ToHexString(SHA256.HashData(bytes));
}
