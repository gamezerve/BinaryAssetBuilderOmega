using System.Buffers.Binary;
using System.Text;
using System.Xml.Schema;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove actual selected Core/plugin payload publication under a distinct local v2 identity policy, not a production SDK.
internal static class PathMusicSelectedPackageSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: test 1/3/8 v2 packages with independent identity/native readback, repeat/v1 isolation and bounded failure/no-overwrite evidence. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        XmlSchemaSet? schemas = null; var reviewed = SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:set => schemas = set);
        Require(reviewed.SchemaAdmitted && schemas != null,"Reviewed selected package schema unavailable.");
        string root = Path.Combine(Path.GetTempPath(),"Reborn-SelectedMusicPackageV2-"+Guid.NewGuid().ToString("N")); Directory.CreateDirectory(root);
        foreach (int count in new[] { 1,3,8 })
        {
            string input = Path.Combine(root,"input"+count); Directory.CreateDirectory(input);
            string source = "<AssetDeclaration xmlns='uri:ea.com:eala:asset'>",header = "";
            for (int index = 0; index < count; index++)
            {
                source += "<PathMusicEvent id='RebornSelectedPackage"+index+"' PathfinderEventHeader='events.h' RestartAlternateEvent='RebornSelectedPackage"+((index+1)%count)+"'/>";
                header += "#define PATH_EVENT_RebornSelectedPackage"+index+" 0x"+(index+1).ToString("X8")+"\n";
            }
            source += "</AssetDeclaration>"; string sourcePath = Path.Combine(input,"events.xml"),headerPath = Path.Combine(input,"events.h");
            File.WriteAllText(sourcePath,source,new UTF8Encoding(false)); File.WriteAllText(headerPath,header,new UTF8Encoding(false));
            var preparation = PathMusicCorePreparation.Read(input,schemas); var report = preparation.Preflight();
            string output = Path.Combine(root,"v2-"+count),repeat = Path.Combine(root,"repeat-"+count),v1 = Path.Combine(root,"v1-"+count);
            preparation.PublishSelectedPackage(output); preparation.VerifySelectedPackage(output); preparation.PublishSelectedPackage(repeat);
            preparation.PublishExperimentalPackage(v1); preparation.VerifyExperimentalPackage(v1);
            var parsed = ManifestReader.Read(File.ReadAllBytes(Path.Combine(output,"diagnostic.manifest")));
            var previous = ManifestReader.Read(File.ReadAllBytes(Path.Combine(v1,"diagnostic.manifest")));
            var identities = report.Rows.Select(row => PathMusicCoreChecksum.Identity(row.Name,row.CoreInstanceHash)).ToArray();
            foreach (var identity in identities) identity.Handle.TypeHash = PathMusicControlledCompiler.LocalTypeHash;
            uint checksum = PathMusicCoreChecksum.Independent(identities);
            Require(parsed.Header.StreamChecksum == checksum && checksum != previous.Header.StreamChecksum && parsed.Header.AllTypesHash == 0
                && parsed.Assets.Count == count && parsed.Assets.All(asset => asset.TypeHash == 0x48E303B8u && asset.ImportsDataSize == 0 && asset.References.Count == 0)
                && parsed.Assets.Select(asset => asset.InstanceHash).SequenceEqual(report.Rows.Select(row => row.CoreInstanceHash))
                && previous.Assets.All(asset => asset.TypeHash == 0),"Selected package manifest identities/profile separation differ.");
            foreach (string path in Directory.GetFiles(output)) Require(File.ReadAllBytes(path).SequenceEqual(File.ReadAllBytes(Path.Combine(repeat,Path.GetFileName(path)))),"Repeated selected package differs.");
            foreach (string name in new[] { "diagnostic.bin","diagnostic.relo","diagnostic.imp" })
            {
                byte[] bytes = File.ReadAllBytes(Path.Combine(output,name));
                Require(BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(4,4)) == checksum
                    && bytes.AsSpan(8).SequenceEqual(File.ReadAllBytes(Path.Combine(v1,name)).AsSpan(8)),"Selected native payload/framing differs from v1.");
            }
            Refuses(() => preparation.VerifyExperimentalPackage(output)); Refuses(() => preparation.VerifyDiagnosticPackage(output));
            Refuses(() => preparation.VerifySelectedPackage(v1)); Refuses(() => preparation.PublishSelectedPackage(output));
            if (count != 3) continue;
            // Reborn: changing a detached actual payload cannot poison a later privately generated package policy.
            var detached = preparation.SelectedPayload(); detached.Chunks[0].InstanceBuffer[4] ^= 1; detached.Report.Rows[0] = detached.Report.Rows[0] with { InstanceHash = 0 };
            preparation.VerifySelectedPackage(output);
            foreach (string path in Directory.GetFiles(output))
            {
                byte[] good = File.ReadAllBytes(path),bad = (byte[])good.Clone(); bad[^1] ^= 1; File.WriteAllBytes(path,bad);
                Refuses(() => preparation.VerifySelectedPackage(output)); File.WriteAllBytes(path,good);
            }
            // Reborn: exact v2 membership remains mandatory; owned missing/extra files are retained outside the approved package, not deleted.
            string marker = Path.Combine(output,"EXPERIMENTAL_CORE_V2.txt"),held = Path.Combine(root,"held-v2");
            File.Move(marker,held); Refuses(() => preparation.VerifySelectedPackage(output)); File.Move(held,marker);
            string extra = Path.Combine(output,"Reborn-extra.txt"); File.WriteAllText(extra,"Reborn: owned extra"); Refuses(() => preparation.VerifySelectedPackage(output));
            File.Move(extra,Path.Combine(root,"retained-extra"));
            string[] before = Directory.GetFileSystemEntries(root).Order().ToArray();
            File.WriteAllText(headerPath,header+"// Reborn: changed captured header\n",new UTF8Encoding(false));
            Refuses(() => preparation.PublishSelectedPackage(Path.Combine(root,"stale"))); Refuses(() => preparation.VerifySelectedPackage(output));
            Require(before.SequenceEqual(Directory.GetFileSystemEntries(root).Order()),"Stale selected package created staging/output.");
            Refuses(() => PathMusicCorePreparation.Read(input,schemas).VerifySelectedPackage(output)); File.WriteAllText(headerPath,header,new UTF8Encoding(false));
            File.WriteAllText(sourcePath,source.Replace("</AssetDeclaration>","<!-- Reborn: raw source provenance --> </AssetDeclaration>"),new UTF8Encoding(false));
            var comment = PathMusicCorePreparation.Read(input,schemas);
            Require(comment.Preflight().Rows.Select(row => row.CoreInstanceHash).SequenceEqual(report.Rows.Select(row => row.CoreInstanceHash)),"Outer comment altered owner identity unexpectedly.");
            Refuses(() => comment.VerifySelectedPackage(output)); File.WriteAllText(sourcePath,source,new UTF8Encoding(false));
            int retained = Directory.GetDirectories(root,"Reborn-MusicPackage-Staging-*").Length;
            string late = Path.Combine(root,"late"); Refuses(() => preparation.PublishSelectedPackage(late,() => File.WriteAllText(headerPath,header+"// Reborn: late source fault\n",new UTF8Encoding(false))));
            Require(!Directory.Exists(late),"Late selected input fault committed output."); File.WriteAllText(headerPath,header,new UTF8Encoding(false));
            string raced = Path.Combine(root,"raced"); Refuses(() => preparation.PublishSelectedPackage(raced,() => { Directory.CreateDirectory(raced); File.WriteAllText(Path.Combine(raced,"owner.txt"),"Reborn: competing owner"); }));
            Require(File.ReadAllText(Path.Combine(raced,"owner.txt")) == "Reborn: competing owner"
                && Directory.GetDirectories(root,"Reborn-MusicPackage-Staging-*").Length == retained+2,"Competing output or retained staging was lost.");
            string occupied = Path.Combine(root,"occupied"); File.WriteAllText(occupied,"Reborn: preserve file"); Refuses(() => preparation.PublishSelectedPackage(occupied));
            Require(File.ReadAllText(occupied) == "Reborn: preserve file","Existing selected output file was overwritten."); preparation.VerifySelectedPackage(output);
        }
        Console.WriteLine("PathMusic selected package v2 self-test: OK (1/3/8 actual selected plugin payloads, local type/Core hashes/padded checksum, two readers/native/weak self-cycles/repeat/v1 isolation, detached/corrupt/missing/extra/stale/raw-comment/late/raced/occupied refusal, retained staging; not production/stock identity)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: wrong profiles, stale input, corrupt bytes and existing destinations must refuse publication/readback. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuses(Action action)
    { try { action(); } catch (Exception error) when (error is InvalidDataException or IOException) { return; } throw new InvalidDataException("Invalid selected music package accepted."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop at the first actual selected payload/profile/publication regression. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
