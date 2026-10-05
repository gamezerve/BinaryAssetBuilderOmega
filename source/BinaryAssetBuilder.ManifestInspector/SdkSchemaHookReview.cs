using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: admit only two exact fingerprint-reviewed dummy-hook warnings after positive/negative authored-source probes; never waive arbitrary diagnostics.
internal static class SdkSchemaHookReview
{
    internal const string Policy = "diagnostic-dummy-hooks-v1";
    internal const string CandidateDigest = "3A54DC3DDC54C1AACF15C03AF6AD67ED007AD99608CEDF78CC43EAD45AA969D3";
    // Reborn: exact warning/provenance matching intentionally fails closed if runtime wording or schema snapshots change.
    private static readonly string[] ExpectedWarnings = {
        "The 'VertexData' attribute is ignored, because the value of 'prohibited' for attribute use only prevents inheritance of an identically named attribute from the base type definition. [file:///C:/Reborn-InMemorySchemas/assettypew3d.xsd:337]",
        "The 'Handle' attribute is ignored, because the value of 'prohibited' for attribute use only prevents inheritance of an identically named attribute from the base type definition. [file:///C:/Reborn-InMemorySchemas/assettypepathmusic.xsd:43]" };
    // Reborn: these fixtures prove XML schema input semantics only, not valid geometry, codec data or production processor output.
    internal const string Track = "<PathMusicTrack id='ReviewedTrack' File='AUDIO:track.mus' Type='MAIN' PathfinderTrackHeader='AUDIO:track.h'/>";
    internal const string Mesh = "<W3DMesh id='ReviewedMesh'><BoundingBox><Min X='0' Y='0' Z='0'/><Max X='0' Y='0' Z='0'/></BoundingBox><BoundingSphere Radius='0'><Center X='0' Y='0' Z='0'/></BoundingSphere><Vertices><V X='0' Y='0' Z='0'/></Vertices><Triangles><T><V>0</V><V>0</V><V>0</V><Nrm X='0' Y='0' Z='0'/><Dist>0</Dist></T></Triangles><FXShader ShaderName='reviewed'/></W3DMesh>";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require the exact candidate/diagnostics and absent effective hooks, then prove both valid inputs and forbidden authored hook rejection before diagnostic admission. */
    //-------------------------------------------------------------------------------------------------
    internal static string[] Review(SdkEffectiveSchema.Evidence evidence,string candidateDigest)
    {
        if (candidateDigest != CandidateDigest || !evidence.EngineCompiled || !evidence.Schemas.IsCompiled || evidence.Errors.Length != 0
            || !evidence.Warnings.OrderBy(value => value,StringComparer.Ordinal).SequenceEqual(ExpectedWarnings.OrderBy(value => value,StringComparer.Ordinal)))
            throw new InvalidDataException("Dummy-hook review requires exact pinned candidate and warning inventory.");
        foreach (var hook in new[] { ("W3DMesh","VertexData"),("PathMusicTrack","Handle") })
        {
            var type = evidence.Schemas.GlobalTypes[new XmlQualifiedName(hook.Item1,"uri:ea.com:eala:asset")] as XmlSchemaComplexType;
            if (type == null || type.AttributeWildcard != null || type.AttributeUses[new XmlQualifiedName(hook.Item2)] != null)
                throw new InvalidDataException("Reviewed hook is admitted by an effective attribute or wildcard.");
        }
        SdkEffectiveSchema.Binding track = Probe(Track),mesh = Probe(Mesh);
        if (!track.XmlValidated || track.Fields.Length != 2 || !track.Fields.Any(field => field.Name == "File" && field.LogicalPath == "AUDIO:track.mus")
            || !track.Fields.Any(field => field.Name == "PathfinderTrackHeader" && field.LogicalPath == "AUDIO:track.h") || !mesh.XmlValidated || mesh.Fields.Length != 0)
            throw new InvalidDataException("Reviewed positive source probes failed.");
        foreach (var rejected in new[] { (Track.Replace("id='ReviewedTrack'","id='ReviewedTrack' Handle='1'"),"Handle"),(Mesh.Replace("id='ReviewedMesh'","id='ReviewedMesh' VertexData='true'"),"VertexData") })
        {
            var probe = Probe(rejected.Item1);
            if (probe.XmlValidated || probe.Fields.Length != 0 || !probe.Errors.Any(error => error.Contains(rejected.Item2,StringComparison.Ordinal)))
                throw new InvalidDataException("Forbidden authored dummy-hook value was not rejected.");
        }
        return new[] { "Exact candidate digest and two warning/provenance messages matched.","W3DMesh.VertexData and PathMusicTrack.Handle absent from effective attributes; no attribute wildcard.",
            "Minimal schema-valid mesh/track source probes passed; authored VertexData/Handle probes rejected with no trusted partial fields.","Track File and PathfinderTrackHeader still bind as two file dependencies; processors/payloads/game loading remain unproved." };

        //-------------------------------------------------------------------------------------------------
        /** Reborn: validate owned in-memory source probes only, without external Includes, payload resolution or processor execution. */
        //-------------------------------------------------------------------------------------------------
        SdkEffectiveSchema.Binding Probe(string body) => SdkEffectiveSchema.Bind(evidence.Schemas,"in-memory-reviewed-hook-probe.xml",Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+body+"</AssetDeclaration>"));
    }
}
