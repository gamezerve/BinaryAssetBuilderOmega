using System.Text;
using System.Xml.Schema;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: test mandatory fresh Core publication gates without changing the established diagnostic package identity policy.
internal static class PathMusicCorePackageSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove 1/3/8 gated packages, legacy-byte equivalence, pre-staging and late stale/forged/raced output refusal with retained evidence. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        XmlSchemaSet? schemas = null; var evidence = SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:set => schemas = set);
        Require(evidence.SchemaAdmitted && schemas != null,"Reviewed Core music package schema unavailable.");
        string directory = Path.Combine(Path.GetTempPath(),"Reborn-CoreMusicPackage-"+Guid.NewGuid().ToString("N")); Directory.CreateDirectory(directory);
        foreach (int count in new[] { 1,3,8 })
        {
            string input = Path.Combine(directory,"input"+count); Directory.CreateDirectory(input);
            string source = "<AssetDeclaration xmlns='uri:ea.com:eala:asset'>",header = "";
            for (int index = 0; index < count; index++)
            {
                source += "<PathMusicEvent id='RebornCorePackage"+index+"' PathfinderEventHeader='events.h'"
                    +(index+1 < count ? " RestartAlternateEvent='RebornCorePackage"+(index+1)+"'" : "")+"/>";
                header += "#define PATH_EVENT_RebornCorePackage"+index+" 0x"+(index+1).ToString("X8")+"\n";
            }
            source += "</AssetDeclaration>";
            string sourcePath = Path.Combine(input,"events.xml"),headerPath = Path.Combine(input,"events.h");
            File.WriteAllText(sourcePath,source,new UTF8Encoding(false)); File.WriteAllText(headerPath,header,new UTF8Encoding(false));
            var prepared = PathMusicCorePreparation.Read(input,schemas); string output = Path.Combine(directory,"first"+count),repeat = Path.Combine(directory,"repeat"+count);
            prepared.PublishDiagnosticPackage(output); prepared.VerifyDiagnosticPackage(output); prepared.PublishDiagnosticPackage(repeat);
            var legacy = PathMusicPackageProbe.Serialize(PathMusicAuthoredSnapshot.Read(input,schemas));
            foreach (var file in legacy) Require(File.ReadAllBytes(Path.Combine(output,file.Key)).SequenceEqual(file.Value)
                && File.ReadAllBytes(Path.Combine(repeat,file.Key)).SequenceEqual(file.Value),"Core gate changed legacy diagnostic identity/bytes.");
            Refuses(() => prepared.PublishDiagnosticPackage(output)); prepared.VerifyDiagnosticPackage(output);
            string[] before = Directory.GetFileSystemEntries(directory).Order().ToArray(); DateTime stamp = File.GetLastWriteTimeUtc(headerPath);
            File.WriteAllText(headerPath,header.Replace("0x00000001","0x00000009"),new UTF8Encoding(false)); File.SetLastWriteTimeUtc(headerPath,stamp);
            string stale = Path.Combine(directory,"stale"+count); Refuses(() => prepared.PublishDiagnosticPackage(stale)); Refuses(() => prepared.VerifyDiagnosticPackage(output));
            Require(!Directory.Exists(stale) && before.SequenceEqual(Directory.GetFileSystemEntries(directory).Order()),"Stale Core music gate created staging.");
            var refreshed = PathMusicCorePreparation.Read(input,schemas); Refuses(() => refreshed.VerifyDiagnosticPackage(output));
            refreshed.PublishDiagnosticPackage(Path.Combine(directory,"refreshed"+count)); File.WriteAllText(headerPath,header,new UTF8Encoding(false));
            prepared.VerifyDiagnosticPackage(output);
            // Reborn: change only owned source bytes after staged readback; rejection must retain staging but never rename it to approved output.
            int retainedBefore = Directory.GetDirectories(directory,"Reborn-MusicPackage-Staging-*").Length;
            string late = Path.Combine(directory,"late"+count);
            Refuses(() => prepared.PublishDiagnosticPackage(late,() => File.WriteAllText(headerPath,header+"// Reborn: late edit\n",new UTF8Encoding(false))));
            Require(!Directory.Exists(late),"Late stale input committed a Core music package."); File.WriteAllText(headerPath,header,new UTF8Encoding(false));
            // Reborn: a competing destination remains untouched even after valid staged bytes exist.
            string raced = Path.Combine(directory,"raced"+count);
            Refuses(() => prepared.PublishDiagnosticPackage(raced,() => { Directory.CreateDirectory(raced); File.WriteAllText(Path.Combine(raced,"owner.txt"),"Reborn: preserve competing output"); }));
            Require(File.ReadAllText(Path.Combine(raced,"owner.txt")) == "Reborn: preserve competing output","Competing music destination was overwritten.");
            string binPath = Path.Combine(output,"diagnostic.bin"); byte[] good = File.ReadAllBytes(binPath),bad = (byte[])good.Clone(); bad[12] ^= 1;
            File.WriteAllBytes(binPath,bad); Refuses(() => prepared.VerifyDiagnosticPackage(output)); File.WriteAllBytes(binPath,good); prepared.VerifyDiagnosticPackage(output);
            // Reborn: retained staging is owned diagnostic evidence; no broad cleanup or silent acceptance follows failure.
            Require(Directory.GetDirectories(directory,"Reborn-MusicPackage-Staging-*").Length == retainedBefore+2,"Failed Core publication evidence was not retained.");
        }
        Console.WriteLine("PathMusic Core package self-test: OK (1/3/8 current-Core publication/readback gates, exact legacy diagnostic bytes/repeat, stale-before-staging/refreshed/late-edit/competing-output/corruption refusal, retained staging; no Core hash relabeling/production admission)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: failed freshness/readback/no-overwrite checks cannot become successful publication. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuses(Action action)
    { try { action(); } catch (Exception error) when (error is InvalidDataException or IOException) { return; } throw new InvalidDataException("Invalid Core music publication accepted."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop at the first Core publication/ownership policy regression. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
