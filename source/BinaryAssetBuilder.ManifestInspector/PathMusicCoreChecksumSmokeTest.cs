using System.Text;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: pin padded Core checksum field selection and order before admitting a separately versioned experimental manifest profile.
internal static class PathMusicCoreChecksumSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: test fresh 1/3/8 music identities, repeatability, order/hash/type/count effects, weak/reference distinctions and stale refusal. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        XmlSchemaSet? schemas = null; var reviewed = SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:set => schemas = set);
        Require(reviewed.SchemaAdmitted && schemas != null,"Reviewed checksum schema unavailable.");
        string root = Path.Combine(Path.GetTempPath(),"Reborn-MusicChecksum-"+Guid.NewGuid().ToString("N")); Directory.CreateDirectory(root);
        foreach (int count in new[] { 1,3,8 })
        {
            string input = Path.Combine(root,"input"+count); Directory.CreateDirectory(input);
            string xml = "<AssetDeclaration xmlns='uri:ea.com:eala:asset'>",header = "";
            for (int index = 0; index < count; index++)
            {
                xml += "<PathMusicEvent id='RebornChecksum"+index+"' PathfinderEventHeader='events.h'"
                    +(index+1 < count ? " RestartAlternateEvent='RebornChecksum"+(index+1)+"'" : "")+"/>";
                header += "#define PATH_EVENT_RebornChecksum"+index+" 0x"+(index+1).ToString("X8")+"\n";
            }
            xml += "</AssetDeclaration>";
            File.WriteAllText(Path.Combine(input,"events.xml"),xml,new UTF8Encoding(false));
            string path = Path.Combine(input,"events.h"); File.WriteAllText(path,header,new UTF8Encoding(false));
            var preparation = PathMusicCorePreparation.Read(input,schemas); var report = PathMusicCoreChecksum.Inspect(preparation);
            Require(report.CoreChecksum == report.ExpectedChecksum && report.UsedBytes == count*20 && report.CapacityBytes == 256
                && report.SyntheticProcessingDomain && !report.ProductionBuildReady && !report.PackagePolicyChanged
                && report == PathMusicCoreChecksum.Inspect(preparation),"Core checksum report/repeat/domain differs.");
            var identities = preparation.Preflight().Rows.Select(row => PathMusicCoreChecksum.Identity(row.Name,row.CoreInstanceHash)).ToArray();
            uint baseline = report.CoreChecksum;
            Require(PathMusicCoreChecksum.Independent(identities,true) != baseline,"Used-length hashing accidentally matches padded checksum.");
            if (count > 1) Require(PathMusicCoreChecksum.Core(identities.Reverse().ToArray()) != baseline,"Checksum lost authored owner order.");
            // Reborn: output checksum excludes weak targets and file names; these affect the preceding Core InstanceHash, not the fifth word.
            identities[0].WeakReferencedInstances.Add(new InstanceHandle("PathMusicEvent","RebornWeakTarget")); identities[0].ReferencedFiles.Add("Reborn-not-a-real-file.h");
            Require(PathMusicCoreChecksum.Core(identities) == baseline,"Weak/file metadata entered output checksum directly.");
            identities[0].Handle.InstanceHash ^= 1; CheckChanged(identities,baseline); identities[0].Handle.InstanceHash ^= 1;
            identities[0].Handle.TypeHash = 1; CheckChanged(identities,baseline); identities[0].Handle.TypeHash = 0;
            identities[0].ReferencedInstances.Add(new InstanceHandle("PathMusicEvent","RebornStrongA")); CheckChanged(identities,baseline);
            uint oneStrong = PathMusicCoreChecksum.Core(identities); identities[0].ReferencedInstances[0] = new InstanceHandle("PathMusicEvent","RebornStrongB");
            Require(PathMusicCoreChecksum.Core(identities) == oneStrong,"Strong target value entered checksum instead of its count.");
            identities[0].ReferencedInstances.Clear(); Require(PathMusicCoreChecksum.Core(identities) == baseline,"Restored identity projection differs.");
            File.WriteAllText(path,header+"// Reborn: unused header edit\n",new UTF8Encoding(false));
            Refuses(() => PathMusicCoreChecksum.Inspect(preparation));
            Require(PathMusicCoreChecksum.Inspect(PathMusicCorePreparation.Read(input,schemas)).CoreChecksum != baseline,"Fresh header identity edit lost checksum propagation.");
            File.WriteAllText(path,header,new UTF8Encoding(false)); Require(PathMusicCoreChecksum.Inspect(preparation) == report,"Checksum restoration differs.");
        }
        Require(PathMusicCoreChecksum.Core(Array.Empty<InstanceDeclaration>()) == 0 && PathMusicCoreChecksum.Independent(Array.Empty<InstanceDeclaration>()) == 0,"Empty checksum differs.");
        Refuses(() => PathMusicCoreChecksum.Independent(Enumerable.Range(0,9).Select(index => PathMusicCoreChecksum.Identity("RebornOverflow"+index,1)).ToArray()));
        Console.WriteLine("PathMusic Core checksum self-test: OK (1/3/8 fresh identities, 20-byte rows/256-byte padding, repeat/order/hash/type/strong-count/weak-file exclusions, stale/refreshed/restored, empty/overflow; package policy unchanged)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: mutations must change actual Core and independently reconstructed checksums identically. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckChanged(InstanceDeclaration[] identities,uint baseline)
    { uint actual = PathMusicCoreChecksum.Core(identities); Require(actual != baseline && actual == PathMusicCoreChecksum.Independent(identities),"Checksum field selection differs."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stale snapshots and out-of-domain checksum requests must fail closed. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuses(Action action)
    { try { action(); } catch (InvalidDataException) { return; } throw new InvalidDataException("Invalid music checksum request accepted."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop immediately at a padded music checksum contract regression. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
