using System.Buffers.Binary;
using System.Text;
using System.Xml.Schema;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove explicit synthetic-domain Core manifest identity publication without modifying diagnostic or production admission.
internal static class PathMusicExperimentalCoreSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: cover bounded 1/3/8 packages, actual identity/checksum fields, repeat/native equivalence, cross-profile/tamper/stale/no-overwrite refusal. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        XmlSchemaSet? schemas = null; var reviewed = SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:set => schemas = set);
        Require(reviewed.SchemaAdmitted && schemas != null,"Reviewed experimental music schema unavailable.");
        string root = Path.Combine(Path.GetTempPath(),"Reborn-ExperimentalMusicV1-"+Guid.NewGuid().ToString("N")); Directory.CreateDirectory(root);
        foreach (int count in new[] { 1,3,8 })
        {
            string input = Path.Combine(root,"input"+count); Directory.CreateDirectory(input);
            string xml = "<AssetDeclaration xmlns='uri:ea.com:eala:asset'>",header = "";
            for (int index = 0; index < count; index++)
            {
                xml += "<PathMusicEvent id='RebornExperimental"+index+"' PathfinderEventHeader='events.h'"
                    +(index+1 < count ? " RestartAlternateEvent='RebornExperimental"+(index+1)+"'" : "")+"/>";
                header += "#define PATH_EVENT_RebornExperimental"+index+" 0x"+(index+1).ToString("X8")+"\n";
            }
            xml += "</AssetDeclaration>"; string sourcePath = Path.Combine(input,"events.xml"),headerPath = Path.Combine(input,"events.h");
            File.WriteAllText(sourcePath,xml,new UTF8Encoding(false)); File.WriteAllText(headerPath,header,new UTF8Encoding(false));
            var preparation = PathMusicCorePreparation.Read(input,schemas); var report = preparation.Preflight();
            string output = Path.Combine(root,"first"+count),repeat = Path.Combine(root,"repeat"+count),legacy = Path.Combine(root,"legacy"+count);
            preparation.PublishExperimentalPackage(output); preparation.VerifyExperimentalPackage(output); preparation.PublishExperimentalPackage(repeat);
            preparation.PublishDiagnosticPackage(legacy); preparation.VerifyDiagnosticPackage(legacy);
            var snapshot = PathMusicAuthoredSnapshot.Read(input,schemas); var oldBytes = PathMusicPackageProbe.Serialize(snapshot);
            foreach (var file in oldBytes) Require(File.ReadAllBytes(Path.Combine(legacy,file.Key)).SequenceEqual(file.Value),"Legacy diagnostic bytes changed.");
            var parsed = ManifestReader.Read(File.ReadAllBytes(Path.Combine(output,"diagnostic.manifest")));
            uint checksum = PathMusicCoreChecksum.Inspect(preparation).CoreChecksum;
            Require(parsed.Header.StreamChecksum == checksum && parsed.Header.AllTypesHash == 0 && parsed.Header.Version == 7
                && parsed.Assets.Select(asset => asset.InstanceHash).SequenceEqual(report.Rows.Select(row => row.CoreInstanceHash))
                && parsed.Assets.All(asset => asset.TypeHash == 0),"Experimental Core identity/checksum metadata differs.");
            foreach (string path in Directory.GetFiles(output)) Require(File.ReadAllBytes(path).SequenceEqual(File.ReadAllBytes(Path.Combine(repeat,Path.GetFileName(path)))),"Repeated experimental package differs.");
            foreach (string name in new[] { "diagnostic.bin","diagnostic.relo","diagnostic.imp" })
            {
                byte[] bytes = File.ReadAllBytes(Path.Combine(output,name));
                Require(BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(4,4)) == checksum
                    && bytes.AsSpan(8).SequenceEqual(oldBytes[name].AsSpan(8)),"Experimental profile changed native chunks or linked checksum differs.");
            }
            Refuses(() => preparation.VerifyDiagnosticPackage(output)); Refuses(() => PathMusicPackageProbe.Verify(output,snapshot));
            Refuses(() => preparation.VerifyExperimentalPackage(legacy)); Refuses(() => preparation.PublishExperimentalPackage(output));
            // Reborn: renaming the marker alone cannot disguise diagnostic hashes as experimental Core identities.
            string legacyNotice = Path.Combine(legacy,"DIAGNOSTIC_ONLY.txt"),experimentalNotice = Path.Combine(legacy,"EXPERIMENTAL_CORE_V1.txt");
            File.Move(legacyNotice,experimentalNotice); Refuses(() => preparation.VerifyExperimentalPackage(legacy)); File.Move(experimentalNotice,legacyNotice);
            // Reborn: every owned profile file is restored after one-byte corruption, and missing/extra members reject exact file-set admission.
            foreach (string path in Directory.GetFiles(output))
            {
                byte[] good = File.ReadAllBytes(path),bad = (byte[])good.Clone(); bad[^1] ^= 1; File.WriteAllBytes(path,bad);
                Refuses(() => preparation.VerifyExperimentalPackage(output)); File.WriteAllBytes(path,good);
            }
            string marker = Path.Combine(output,"EXPERIMENTAL_CORE_V1.txt"),held = Path.Combine(root,"held"+count);
            File.Move(marker,held); Refuses(() => preparation.VerifyExperimentalPackage(output)); File.Move(held,marker);
            string extra = Path.Combine(output,"Reborn-extra.txt"); File.WriteAllText(extra,"Reborn: owned extra member");
            Refuses(() => preparation.VerifyExperimentalPackage(output)); File.Move(extra,Path.Combine(root,"extra"+count)); preparation.VerifyExperimentalPackage(output);
            string[] before = Directory.GetFileSystemEntries(root).Order().ToArray();
            File.WriteAllText(headerPath,header+"// Reborn: unused header edit\n",new UTF8Encoding(false));
            string stale = Path.Combine(root,"stale"+count); Refuses(() => preparation.PublishExperimentalPackage(stale)); Refuses(() => preparation.VerifyExperimentalPackage(output));
            Require(before.SequenceEqual(Directory.GetFileSystemEntries(root).Order()),"Stale experimental preparation created staging/output.");
            var refreshed = PathMusicCorePreparation.Read(input,schemas); Refuses(() => refreshed.VerifyExperimentalPackage(output));
            refreshed.PublishExperimentalPackage(Path.Combine(root,"refreshed"+count)); File.WriteAllText(headerPath,header,new UTF8Encoding(false));
            // Reborn: a root-level XML comment can retain Core owner hashes; exact profile source fingerprints still forbid old-byte acceptance.
            File.WriteAllText(sourcePath,xml.Replace("</AssetDeclaration>","<!-- Reborn: raw provenance --> </AssetDeclaration>"),new UTF8Encoding(false));
            Refuses(() => preparation.VerifyExperimentalPackage(output)); var comment = PathMusicCorePreparation.Read(input,schemas);
            Require(comment.Preflight().Rows.Select(row => row.CoreInstanceHash).SequenceEqual(report.Rows.Select(row => row.CoreInstanceHash)),"Outer comment changed owner XML identity unexpectedly.");
            Refuses(() => comment.VerifyExperimentalPackage(output)); File.WriteAllText(sourcePath,xml,new UTF8Encoding(false)); preparation.VerifyExperimentalPackage(output);
            int retained = Directory.GetDirectories(root,"Reborn-MusicPackage-Staging-*").Length;
            string late = Path.Combine(root,"late"+count);
            Refuses(() => preparation.PublishExperimentalPackage(late,() => File.WriteAllText(headerPath,header+"// Reborn: late input edit\n",new UTF8Encoding(false))));
            Require(!Directory.Exists(late),"Late input edit committed experimental output."); File.WriteAllText(headerPath,header,new UTF8Encoding(false));
            string raced = Path.Combine(root,"raced"+count);
            Refuses(() => preparation.PublishExperimentalPackage(raced,() => { Directory.CreateDirectory(raced); File.WriteAllText(Path.Combine(raced,"owner.txt"),"Reborn: competing output"); }));
            Require(File.ReadAllText(Path.Combine(raced,"owner.txt")) == "Reborn: competing output"
                && Directory.GetDirectories(root,"Reborn-MusicPackage-Staging-*").Length == retained+2,"Competing output or retained evidence was lost.");
            string occupied = Path.Combine(root,"occupied"+count); File.WriteAllText(occupied,"Reborn: existing file"); Refuses(() => preparation.PublishExperimentalPackage(occupied));
            Require(File.ReadAllText(occupied) == "Reborn: existing file","Existing destination file was overwritten."); preparation.VerifyExperimentalPackage(output);
        }
        Console.WriteLine("PathMusic experimental Core v1 self-test: OK (1/3/8 actual hashes/padded checksum, two readers/native invariance/repeat/legacy bytes, cross-profile/marker/corrupt/missing/extra/raw-comment/stale/refreshed/late/raced/occupied refusal, retained staging; synthetic domain only)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: invalid profile, stale evidence and existing destination checks must fail closed. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuses(Action action)
    { try { action(); } catch (Exception error) when (error is InvalidDataException or IOException) { return; } throw new InvalidDataException("Invalid experimental music package accepted."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop at the first experimental profile identity/native/publication regression. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
