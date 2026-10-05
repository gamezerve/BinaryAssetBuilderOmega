using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: verify pinned normalization is deterministic, in-memory and explicitly distinct from untouched reference schema evidence.
internal static class SdkShieldSchemaCandidateSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: keep first Shield inheritance aligned with the existing native-layout regression while rejecting altered/reapplied source bytes. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe void Run()
    {
        string physical = Path.Combine(SdkEnvironmentPreflight.BaselineSchemaRoot(),SdkShieldSchemaCandidate.FileName.Replace('/',Path.DirectorySeparatorChar));
        byte[] original = SdkEnvironmentPreflight.Read(physical,2*1048576); byte[] copy = original.ToArray(),other = Encoding.UTF8.GetBytes("untouched");
        Dictionary<string,byte[]> snapshots = new(StringComparer.OrdinalIgnoreCase) { [SdkShieldSchemaCandidate.FileName] = original,["other.xsd"] = other };
        var change = SdkShieldSchemaCandidate.Apply(snapshots);
        Require(original.SequenceEqual(copy) && ReferenceEquals(snapshots["other.xsd"],other) && change.BeforeSha256 == SdkShieldSchemaCandidate.ExpectedHash && change.AfterSha256 != change.BeforeSha256,"Candidate mutated original/other snapshots or lost fingerprint attribution.");
        XmlDocument xml = new() { XmlResolver = null }; using MemoryStream bytes = new(snapshots[SdkShieldSchemaCandidate.FileName],false); xml.Load(bytes);
        XmlNamespaceManager namespaces = new(xml.NameTable); namespaces.AddNamespace("xs","http://www.w3.org/2001/XMLSchema");
        var definitions = xml.SelectNodes("/xs:schema/xs:complexType[@name='ShieldSphereUpdateModuleData']",namespaces)!;
        Require(definitions.Count == 1 && definitions[0]!.SelectSingleNode("xs:complexContent/xs:extension",namespaces)?.Attributes?["base"]?.Value == "SphereModuleUpdateModuleData","Candidate kept the wrong Shield declaration.");
        Require(typeof(SageBinaryData.ShieldSphereUpdateModuleData).GetField("Base")!.FieldType == typeof(SageBinaryData.SphereModuleUpdateModuleData)
            && sizeof(SageBinaryData.ShieldSphereUpdateModuleData) == 312 && sizeof(SageBinaryData.YurikoShieldSphereUpdateModuleData) == 328,"Shield candidate no longer matches existing x86 layout evidence.");
        Dictionary<string,byte[]> repeated = new(StringComparer.OrdinalIgnoreCase) { [SdkShieldSchemaCandidate.FileName] = copy };
        Require(SdkShieldSchemaCandidate.Apply(repeated).AfterSha256 == change.AfterSha256,"Candidate serialization is not deterministic.");
        Reject(() => SdkShieldSchemaCandidate.Apply(repeated));
        repeated[SdkShieldSchemaCandidate.FileName] = copy.Concat(Encoding.UTF8.GetBytes("\n<!-- unreviewed -->")).ToArray(); Reject(() => SdkShieldSchemaCandidate.Apply(repeated));
        var report = SdkEffectiveSchema.Inspect(shieldCandidate:true);
        Require(report.CandidateProfile == SdkShieldSchemaCandidate.Profile && report.Changes.Length == 1 && report.SchemaCatalogSha256 != report.CandidateCatalogSha256
            && report.ReadOnly && report.SnapshotOnly && !report.FullDependencyCoverage && !report.ProductionBuildReady
            && !report.Errors.Any(error => error.Contains("ShieldSphereUpdateModuleData' has already",StringComparison.Ordinal)),"Candidate hid transformation evidence or retained the reviewed duplicate.");
        // Reborn: structurally compiled schemas with unreviewed prohibition warnings must not publish trusted source bindings.
        Require(report.SchemaEngineCompiled && !report.SchemaCompiled && report.Errors.Length == 0 && report.Warnings.Length == 2
            && report.Warnings.Any(warning => warning.Contains("VertexData",StringComparison.Ordinal)) && report.Warnings.Any(warning => warning.Contains("Handle",StringComparison.Ordinal))
            && report.SourceBinding == null && report.EffectiveFileAttributes.Length == 0,"Candidate warnings were lost or silently admitted.");
        Require(Convert.ToHexString(SHA256.HashData(SdkEnvironmentPreflight.Read(physical,2*1048576))) == SdkShieldSchemaCandidate.ExpectedHash,"Candidate rewrote staged reference bytes.");
        Console.WriteLine("SDK Shield schema candidate self-test: OK (pinned/deterministic memory-only transform, unchanged reference, 312/328-byte model inheritance, original/candidate digest separation, changed/reapplied rejection; not production schema approval)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: candidate transformations require the original reviewed fingerprint every time; no generic duplicate deletion fallback. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(Action action)
    { try { action(); } catch (InvalidDataException) { return; } throw new InvalidDataException("Unreviewed Shield candidate input accepted."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: halt at the first mismatch between pinned transformation evidence and unchanged reference/native model expectations. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
