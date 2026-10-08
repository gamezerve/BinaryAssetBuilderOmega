using System.Security.Cryptography;
using System.Xml;
using System.Xml.Schema;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: expose successful expression-stage evidence separately from still-refused owner inheritance/schema/native admission.
internal static class SdkSoundExpressionReview
{
    // Reborn: repeated singleton fields are authored evidence only, never permission to collapse or overwrite them.
    internal sealed record Singleton(string AssetType,string AssetId,string ChildName,string SchemaType,Dictionary<string,string>[] Occurrences);
    internal sealed record FieldCount(string AssetType,string? ChildName,string Field,int Count);
    internal sealed record Report(string SourcePath,string RawSha256,string ExpressionStageSha256,int Substitutions,int SoundCalculations,int MusicCalculations,FieldCount[] Fields,Singleton[] RepeatedSingletons,SdkIncludeDefineProfile.SourceIdentity[] DefinitionSources)
    {
        // Reborn: computation-stage review does not imply complete owner validation, Core merge equivalence or native/game readiness.
        public bool ReadOnly => true;
        public bool SnapshotOnly => true;
        public bool ExpressionStageReviewed => true;
        public bool OwnerInheritanceValidated => false;
        public bool FullDependencyCoverage => false;
        public bool ProductionBuildReady => false;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: review only rechecked confined expressions and schema singleton multiplicity, without changing authored source or executing owner merges. */
    //-------------------------------------------------------------------------------------------------
    internal static Report Inspect(SdkSourcePathAudit.Report paths,XmlSchemaSet schemas,string sourcePath)
    {
        byte[] raw = SdkEnvironmentPreflight.Read(sourcePath,4*1048576);
        var expression = new SdkIncludeDefineProfile(paths,true,schemas,soundOffsets:true).Apply(sourcePath,raw,beforeInheritance:true);
        if (expression.Bytes == null) throw new InvalidDataException("Sound expression stage refused: "+string.Join("; ",expression.Evidence.Diagnostics));
        XmlDocument xml = new() { XmlResolver = null };
        using (MemoryStream input = new(expression.Bytes,false))
        using (XmlReader reader = XmlReader.Create(input,new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit,XmlResolver = null,MaxCharactersInDocument = 4*1048576 })) xml.Load(reader);
        const string ea = "uri:ea.com:eala:asset";
        List<Singleton> repeated = new();
        foreach (var owner in xml.DocumentElement!.ChildNodes.OfType<XmlElement>().Where(node => node.LocalName is not ("Includes" or "Defines" or "Tags")))
        {
            if (schemas.GlobalTypes[new XmlQualifiedName(owner.LocalName,ea)] is not XmlSchemaComplexType type || type.ContentTypeParticle is not XmlSchemaSequence sequence) continue;
            foreach (var declaration in sequence.Items.OfType<XmlSchemaElement>().Where(item => item.MaxOccurs == 1))
            {
                var occurrences = owner.ChildNodes.OfType<XmlElement>().Where(child => child.NamespaceURI == ea && child.LocalName == declaration.Name).ToArray();
                if (occurrences.Length <= 1) continue;
                if (occurrences.Length > 16 || repeated.Count >= 128) throw new InvalidDataException("Repeated singleton review exceeds 16 occurrences/128 groups.");
                repeated.Add(new(owner.LocalName,owner.GetAttribute("id"),declaration.Name!,declaration.ElementSchemaType!.QualifiedName.Name,
                    occurrences.Select(child => child.Attributes.OfType<XmlAttribute>().Where(field => field.NamespaceURI.Length == 0).ToDictionary(field => field.Name,field => field.Value,StringComparer.Ordinal)).ToArray()));
            }
        }
        // Reborn: post-review raw identities remain snapshots; changed files withhold the entire expression-stage report.
        long recheckedBytes = 0;
        foreach (var source in paths.Sources)
        {
            byte[] observed = SdkEnvironmentPreflight.Read(source.PhysicalPath,4*1048576);
            recheckedBytes += observed.Length;
            if (recheckedBytes > 32*1048576) throw new InvalidDataException("Sound review post-recheck exceeds 32 MiB.");
            if (observed.Length != source.Bytes || Convert.ToHexString(SHA256.HashData(observed)) != source.Sha256)
                throw new InvalidDataException("Source changed after sound expression review.");
        }
        var fields = expression.Evidence.SoundOffsets.GroupBy(item => (item.AssetType,item.ChildName,item.Field)).Select(group => new FieldCount(group.Key.AssetType,group.Key.ChildName,group.Key.Field,group.Count())).OrderBy(item => item.AssetType,StringComparer.Ordinal).ThenBy(item => item.ChildName,StringComparer.Ordinal).ThenBy(item => item.Field,StringComparer.Ordinal).ToArray();
        return new(sourcePath,expression.Evidence.RawSha256,expression.Evidence.ProcessedSha256!,expression.Evidence.Substitutions,expression.Evidence.SoundOffsets.Length,expression.Evidence.MusicVolumeOffsets.Length,fields,repeated.ToArray(),expression.Evidence.DefinitionSources);
    }
}
