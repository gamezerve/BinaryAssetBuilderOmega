using System.Text;
using System.Xml.Schema;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: test package publication/readback only with owned synthetic local music files and no production identity admission.
internal static class PathMusicPackageSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove 1/3/8 ordered music packages, both readers, weak offsets, deterministic bytes, stale/corrupt/orphan/no-overwrite refusal. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        XmlSchemaSet? schemas = null; var evidence = SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:set => schemas = set);
        Require(evidence.SchemaAdmitted && schemas != null,"Music package reviewed schema unavailable.");
        string directory = Path.Combine(Path.GetTempPath(),"Reborn-MusicPackageTest-"+Guid.NewGuid().ToString("N")); Directory.CreateDirectory(directory);
        for (int count = 1; count <= 8; count = count == 1 ? 3 : count == 3 ? 8 : 9)
        {
            string source = "<AssetDeclaration xmlns='uri:ea.com:eala:asset'>",header = "";
            for (int index = 0; index < count; index++)
            {
                source += "<PathMusicEvent id='RebornPackage"+index+"' PathfinderEventHeader='events.h' IsCacheable='"+(index % 2 == 0 ? "false" : "true")+"'"
                    +(index+1 < count ? " RestartAlternateEvent='RebornPackage"+(index+1)+"'" : "")+"/>";
                header += "#define PATH_EVENT_RebornPackage"+index+" 0x"+(index+1).ToString("X8")+"\n";
            }
            source += "</AssetDeclaration>";
            File.WriteAllText(Path.Combine(directory,"events.xml"),source,new UTF8Encoding(false));
            string headerPath = Path.Combine(directory,"events.h"); File.WriteAllText(headerPath,header,new UTF8Encoding(false));
            var snapshot = PathMusicAuthoredSnapshot.Read(directory,schemas); var payload = PathMusicPackageProbe.Serialize(snapshot);
            string output = Path.Combine(directory,"first"+count),repeat = Path.Combine(directory,"repeat"+count);
            PathMusicPackageProbe.Publish(output,snapshot); PathMusicPackageProbe.Verify(output,snapshot); PathMusicPackageProbe.Publish(repeat,snapshot);
            Require(payload["diagnostic.bin"].Length == 8+16*count+4*(count-1) && payload["diagnostic.relo"].Length == 8+8*(count-1)
                && payload["diagnostic.imp"].Length == 8,"Music linked sizes differ.");
            foreach (var file in payload)
                Require(File.ReadAllBytes(Path.Combine(output,file.Key)).SequenceEqual(File.ReadAllBytes(Path.Combine(repeat,file.Key))),"Repeated music package differs.");
            payload["diagnostic.bin"][8] = 99; PathMusicPackageProbe.Verify(output,snapshot);
            Refuses(() => PathMusicPackageProbe.Publish(output,snapshot));
            string existing = Path.Combine(directory,"existing"+count); File.WriteAllBytes(existing,new byte[] { 42 });
            Refuses(() => PathMusicPackageProbe.Publish(existing,snapshot)); Require(File.ReadAllBytes(existing).SequenceEqual(new byte[] { 42 }),"Existing music output replaced.");
            foreach (string name in payload.Keys)
            {
                string path = Path.Combine(output,name); byte[] good = File.ReadAllBytes(path);
                byte[] flip = (byte[])good.Clone(); flip[^1] ^= 1;
                foreach (byte[] bad in new[] { good[..^1],good.Concat(new byte[] { 0 }).ToArray(),flip })
                { File.WriteAllBytes(path,bad); Refuses(() => PathMusicPackageProbe.Verify(output,snapshot)); File.WriteAllBytes(path,good); }
                File.Move(path,path+".missing"); Refuses(() => PathMusicPackageProbe.Verify(output,snapshot)); File.Move(path+".missing",path);
            }
            // Reborn: target identities, stream checksums, native event/cache/pointer/padding and weak-target/sentinel words rather than only tail damage.
            foreach (var target in new[] { (Name:"diagnostic.manifest",Offsets:new[] { 4,8,12,52,56,60,64,84,88,92 }),
                (Name:"diagnostic.bin",Offsets:count == 1 ? new[] { 0,4,8,12,16,20,21 } : new[] { 0,4,8,12,16,20,21,24 }),
                (Name:"diagnostic.relo",Offsets:count == 1 ? new[] { 0,4 } : new[] { 0,4,8,12 }),
                (Name:"diagnostic.imp",Offsets:new[] { 0,4 }) })
            {
                string path = Path.Combine(output,target.Name); byte[] good = File.ReadAllBytes(path);
                foreach (int offset in target.Offsets)
                { byte[] bad = (byte[])good.Clone(); bad[offset] ^= 1; File.WriteAllBytes(path,bad); Refuses(() => PathMusicPackageProbe.Verify(output,snapshot)); }
                File.WriteAllBytes(path,good);
            }
            string orphan = Path.Combine(output,"orphan"); File.WriteAllBytes(orphan,new byte[] { 1 }); Refuses(() => PathMusicPackageProbe.Verify(output,snapshot));
            File.Move(orphan,Path.Combine(directory,"retained-orphan"+count)); PathMusicPackageProbe.Verify(output,snapshot);
            // Reborn: stale same-size/timestamp header edits reject before creation of any staging/output folder.
            DateTime stamp = File.GetLastWriteTimeUtc(headerPath); string staleOutput = Path.Combine(directory,"stale"+count);
            string[] before = Directory.GetFileSystemEntries(directory).Order().ToArray();
            File.WriteAllText(headerPath,header.Replace("0x00000001","0x00000009"),new UTF8Encoding(false)); File.SetLastWriteTimeUtc(headerPath,stamp);
            Refuses(() => PathMusicPackageProbe.Publish(staleOutput,snapshot)); Refuses(() => PathMusicPackageProbe.Verify(output,snapshot));
            Require(!Directory.Exists(staleOutput) && before.SequenceEqual(Directory.GetFileSystemEntries(directory).Order()),"Stale music preparation staged output.");
            var refreshed = PathMusicAuthoredSnapshot.Read(directory,schemas); Refuses(() => PathMusicPackageProbe.Verify(output,refreshed));
            PathMusicPackageProbe.Publish(Path.Combine(directory,"refreshed"+count),refreshed);
            File.WriteAllText(headerPath,header,new UTF8Encoding(false)); PathMusicPackageProbe.Verify(output,snapshot);
        }
        Console.WriteLine("PathMusic package self-test: OK (1/3/8 native closures, v7/two readers/weak IDs/linked offsets, diagnostic zero type/catalog hashes, repeat/detachment, corrupt/missing/orphan/stale/no-overwrite/refreshed rejection; no production/game admission)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: invalid diagnostic bytes and stale evidence must not become accepted output. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuses(Action action)
    {
        try { action(); } catch (Exception error) when (error is InvalidDataException or IOException or ArgumentException) { return; }
        throw new InvalidDataException("Invalid music package accepted.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop at the first package framing/ownership/publication regression. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
