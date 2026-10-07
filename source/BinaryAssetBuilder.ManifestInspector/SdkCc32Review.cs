using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: separately review the pinned CC32 cross-QName command without admitting it to any preprocessing profile.
internal static class SdkCc32Review
{
    private const string Ea = "uri:ea.com:eala:asset";
    private const string OwnerName = "AIPersonalityDefinition:AIP_CC_32_AlliedEnemy";
    private const string TargetName = "AIStrategicStateDefinition:AlliedCaptureTech_MEDIUM";
    private const string RawHash = "F9A2ED51CF5A2D3395013A174F0D6D39A5B20625CE5C72F762C71F9FD70D4B77";
    // Reborn: manifest reference presence is metadata only, not decoding of the owner's native state lists.
    internal sealed record NativeEvidence(string OwnerName,string TypeId,string InstanceId,string TypeHash,int InstanceBytes,int ReferenceCount,string TargetName,string TargetTypeId,string TargetInstanceId,bool TargetReferencePresent);
    // Reborn: isolate reviewed source/core behavior from unchanged graph admission and unproved native payload compatibility.
    internal sealed record Result(string SourcePath,string RawSha256,string ProcessedSha256,string BaseProcessedSha256,string CommandName,string MatchedName,string TargetId,int BeforeTargets,int AfterTargets,bool OwnerValidated,bool ExistingProfileRefused,SdkInstanceInheritanceProfile.PreparedSource[] PreparedSources,string ManifestPath,string ManifestSha256,NativeEvidence Native,bool ReadOnly = true,bool PartialOwnerReview = true,bool ProductionBuildReady = false,bool NativeStateLayoutVerified = false);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prepare the captured base under existing guards, run the pinned original command through the unchanged core and inspect bounded EP1 metadata only. */
    //-------------------------------------------------------------------------------------------------
    internal static Result Review(string sourceRoot,string manifestPath)
    {
        sourceRoot = SdkEnvironmentPreflight.DirectoryPath(sourceRoot);
        string entry = Path.Combine(sourceRoot,"EP1","SkirmishAI","Personalities","CommandersChallenge","AIP_CC_32.xml");
        byte[] raw = SdkEnvironmentPreflight.Read(entry,4*1048576);
        if (Hash(raw) != RawHash) throw new InvalidDataException("CC32 source differs from the reviewed snapshot; no generic cross-QName authority.");
        string output = Path.Combine(Path.GetTempPath(),"Reborn-Cc32-NoOutput-"+Guid.NewGuid().ToString("N"));
        var environment = SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),sourceRoot,entry,output,Array.Empty<string>());
        var paths = SdkSourcePathAudit.Inspect(environment);
        XmlSchemaSet? schemas = null; var schema = SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:set => schemas = set);
        if (!schema.SchemaAdmitted || schemas == null || paths.StoppedAtLimit) throw new InvalidDataException("Pinned admitted schema and complete bounded source inventory required.");
        var guarded = new SdkInstanceInheritanceProfile(schemas,paths,stateReadds:true).Apply(entry,raw);
        if (guarded.Bytes != null || !guarded.Evidence.Diagnostics.Any(message => message.Contains("Cross-QName child ID collisions remain closed.",StringComparison.Ordinal)))
            throw new InvalidDataException("Existing CC32 cross-QName refusal changed; review is not an admission fallback.");
        string basePath = Path.Combine(Path.GetDirectoryName(entry)!,"AIP_CC_BaseAlliedBalanced.xml");
        var prepared = new SdkInstanceInheritanceProfile(schemas,paths,stateReadds:true).Apply(basePath,SdkEnvironmentPreflight.Read(basePath,4*1048576));
        if (prepared.Bytes == null) throw new InvalidDataException("Reviewed CC32 base preparation failed.");
        var baseXml = Parse(prepared.Bytes); var ownerXml = Parse(raw);
        var basis = baseXml.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(node => node.GetAttribute("id") == "AIP_CC_BaseAlliedBalanced");
        var owner = ownerXml.DocumentElement!.ChildNodes.OfType<XmlElement>().Single(node => node.GetAttribute("id") == "AIP_CC_32_AlliedEnemy");
        const string target = "AlliedCaptureTech_MEDIUM";
        var command = owner.ChildNodes.OfType<XmlElement>().Single(node => node.GetAttribute("id") == target);
        var matched = basis.ChildNodes.OfType<XmlElement>().Single(node => node.GetAttribute("id") == target);
        if (command.LocalName != "BuildState" || command.GetAttribute("joinAction","uri:ea.com:eala:asset:instance") != "Remove" || matched.LocalName != "StrategicState")
            throw new InvalidDataException("Pinned command/resolved target shape changed.");
        owner.RemoveAttribute("inheritFrom");
        var merged = (XmlElement)NodeJoiner.Override(schemas,ownerXml,basis,owner);
        var final = Parse(Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='"+Ea+"'/>")); final.DocumentElement!.AppendChild(final.ImportNode(merged,true));
        byte[] processed = Encoding.UTF8.GetBytes(final.OuterXml);
        bool valid = SdkEffectiveSchema.Bind(schemas,entry,processed).XmlValidated;
        int after = merged.ChildNodes.OfType<XmlElement>().Count(node => node.GetAttribute("id") == target);
        if (!valid || after != 0) throw new InvalidDataException("Actual core CC32 review did not remove the exact target into a schema-valid owner.");
        manifestPath = SdkEnvironmentPreflight.Absolute(manifestPath);
        byte[] metadata = SdkEnvironmentPreflight.Read(manifestPath,16*1048576); string manifestHash = Hash(metadata);
        var manifest = ManifestReader.Read(metadata);
        if (manifest.Header.Version != TargetProfile.Uprising.ManifestVersion || manifest.Header.AllTypesHash != TargetProfile.Uprising.AllTypesHash || manifest.Validate().Count != 0)
            throw new InvalidDataException("Validated EP1 manifest metadata required.");
        var native = Observe(manifest.Assets);
        // Reborn: recheck exactly captured source/metadata identities before publishing snapshot witnesses, without claiming atomic filesystem locking.
        foreach (var source in paths.Sources)
            if (Hash(SdkEnvironmentPreflight.Read(source.PhysicalPath,4*1048576)) != source.Sha256) throw new InvalidDataException("CC32 source closure changed during review.");
        if (Hash(SdkEnvironmentPreflight.Read(manifestPath,16*1048576)) != manifestHash || Directory.Exists(output)) throw new InvalidDataException("Metadata changed or unexpected output was created.");
        return new(entry,RawHash,Hash(processed),prepared.Evidence.ProcessedSha256!,command.LocalName,matched.LocalName,target,1,after,valid,true,prepared.Evidence.PreparedSources,manifestPath,manifestHash,native);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: match native target references by the complete type/instance identity pair, not only display names or instance IDs. */
    //-------------------------------------------------------------------------------------------------
    internal static NativeEvidence Observe(IReadOnlyList<ManifestAsset> assets)
    {
        var owner = assets.Single(asset => asset.Name == OwnerName);
        var target = assets.Single(asset => asset.Name == TargetName);
        return new(owner.Name,$"0x{owner.TypeId:X8}",$"0x{owner.InstanceId:X8}",$"0x{owner.TypeHash:X8}",owner.InstanceDataSize,owner.References.Count,target.Name,$"0x{target.TypeId:X8}",$"0x{target.InstanceId:X8}",owner.References.Any(reference => reference.TypeId == target.TypeId && reference.InstanceId == target.InstanceId));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: parse bounded reviewed XML with DTD/external entity lookup disabled. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Parse(byte[] bytes)
    {
        var xml = new XmlDocument { XmlResolver = null };
        using var reader = XmlReader.Create(new MemoryStream(bytes),new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit,XmlResolver = null,MaxCharactersInDocument = 4*1048576 }); xml.Load(reader); return xml;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain byte-level snapshot fingerprints separately from native asset hashes. */
    //-------------------------------------------------------------------------------------------------
    private static string Hash(byte[] bytes) => Convert.ToHexString(SHA256.HashData(bytes));
}
