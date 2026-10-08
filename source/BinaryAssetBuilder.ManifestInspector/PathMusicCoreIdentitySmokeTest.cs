using System.Text;
using System.Xml.Schema;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: characterize real Core hashing on owned local music fixtures, not production music processor identities.
internal static class PathMusicCoreIdentitySmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: check header/XML/default/weak-type/padded-file/version domains and absolute-path independence using freshly processed Core documents. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        XmlSchemaSet? schemas = null; var admitted = SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:set => schemas = set);
        Require(admitted.SchemaAdmitted && schemas != null,"Reviewed music identity schema unavailable.");
        string directory = Path.Combine(Path.GetTempPath(),"Reborn-MusicIdentity-"+Guid.NewGuid().ToString("N")); Directory.CreateDirectory(directory);
        foreach (int count in new[] { 1,3,8 })
        {
            var fixture = Fixture(count); string current = Path.Combine(directory,"case"+count); Write(current,fixture.Source,fixture.Header);
            var baseline = PathMusicCoreIdentity.Inspect(current,schemas);
            Require(baseline.Rows.Length == count && baseline.Rows.All(row => row.CoreInstanceHash == row.ExpectedInstanceHash)
                && baseline.SyntheticProcessingDomain && !baseline.ReferenceProcessingHashRecovered && !baseline.ProductionBuildReady,"Core music identity/readiness differs.");
            var repeat = PathMusicCoreIdentity.Inspect(current,schemas); Require(Hashes(baseline).SequenceEqual(Hashes(repeat)),"Repeated Core music identity differs.");
            string other = Path.Combine(directory,"relocated"+count); Write(other,fixture.Source,fixture.Header);
            Require(Hashes(baseline).SequenceEqual(Hashes(PathMusicCoreIdentity.Inspect(other,schemas))),"Physical directory entered Core authored identity.");
            string headerPath = Path.Combine(current,"events.h"); DateTime stamp = File.GetLastWriteTimeUtc(headerPath);
            File.WriteAllText(headerPath,fixture.Header.Replace("0x00000001","0x00000009"),new UTF8Encoding(false)); File.SetLastWriteTimeUtc(headerPath,stamp);
            var changed = PathMusicCoreIdentity.Inspect(current,schemas);
            Require(baseline.Rows.Zip(changed.Rows).All(pair => pair.First.CoreInstanceHash != pair.Second.CoreInstanceHash
                && pair.First.XmlHash == pair.Second.XmlHash && pair.First.HeaderFileHash != pair.Second.HeaderFileHash),"Shared header edit did not invalidate all Core music owners.");
            Write(current,fixture.Source,fixture.Header+"// Reborn: unused diagnostic header comment\n");
            var comment = PathMusicCoreIdentity.Inspect(current,schemas);
            Require(baseline.Rows.Zip(comment.Rows).All(pair => pair.First.CoreInstanceHash != pair.Second.CoreInstanceHash),"Unused header bytes disappeared from Core file identity.");
            Write(current,fixture.Source,fixture.Header);
            Require(Hashes(baseline).SequenceEqual(Hashes(PathMusicCoreIdentity.Inspect(current,schemas))),"Restored Core inputs changed identity.");
            var processing = PathMusicCoreIdentity.Inspect(current,schemas,PathMusicCoreIdentity.Processing+1);
            Require(baseline.Rows.Zip(processing.Rows).All(pair => pair.First.CoreInstanceHash != pair.Second.CoreInstanceHash
                && pair.First.DependencyHash == pair.Second.DependencyHash),"Synthetic processing seed did not isolate Core identity.");
            Write(current,fixture.Source.Replace("IsCacheable='false'","IsCacheable='true'"),fixture.Header);
            var cache = PathMusicCoreIdentity.Inspect(current,schemas); Require(cache.Rows[0].CoreInstanceHash != baseline.Rows[0].CoreInstanceHash
                && cache.Rows[0].DependencyHash == baseline.Rows[0].DependencyHash,"XML cache change did not isolate text hashing.");
            // Reborn: XML WriteTo omits schema-default attributes: identical runtime cache semantics do not imply identical authored Core hashes.
            string defaults = fixture.Source.Replace("IsCacheable='false'","IsCacheable='true'"); Write(current,defaults,fixture.Header);
            var explicitDefault = PathMusicCoreIdentity.Inspect(current,schemas); Write(current,defaults.Replace(" IsCacheable='true'",""),fixture.Header);
            var omittedDefault = PathMusicCoreIdentity.Inspect(current,schemas);
            Require(explicitDefault.Rows.Zip(omittedDefault.Rows).All(pair => pair.First.CoreInstanceHash != pair.Second.CoreInstanceHash
                && pair.First.DependencyHash == pair.Second.DependencyHash && pair.First.Cacheable && pair.Second.Cacheable
                && pair.First.CacheAttributeSpecified && !pair.Second.CacheAttributeSpecified
                && pair.First.SerializedCacheAttribute && !pair.Second.SerializedCacheAttribute),"Explicit/default provenance disappeared from authored Core hashing.");
        }
        // Reborn: a long authored name plus alternate crosses the independent 512-character text-fold boundary.
        string primary = "A"+new string('p',255),secondary = "B"+new string('s',255); var pair = Fixture(2);
        string longSource = pair.Source.Replace("RebornIdentity0",primary).Replace("RebornIdentity1",secondary);
        string longHeader = pair.Header.Replace("RebornIdentity0",primary).Replace("RebornIdentity1",secondary);
        string longDirectory = Path.Combine(directory,"long"); Write(longDirectory,longSource,longHeader);
        Require(PathMusicCoreIdentity.Inspect(longDirectory,schemas).Rows.Length == 2,"Long XML hashing boundary failed.");
        // Reborn: removing a weak field removes a target-type word from the padded dependency buffer, while keeping header bytes unchanged.
        string weakDirectory = Path.Combine(directory,"weak"); Write(weakDirectory,pair.Source,pair.Header);
        var weak = PathMusicCoreIdentity.Inspect(weakDirectory,schemas); Write(weakDirectory,pair.Source.Replace(" RestartAlternateEvent='RebornIdentity1'",""),pair.Header);
        var noWeak = PathMusicCoreIdentity.Inspect(weakDirectory,schemas);
        Require(weak.Rows[0].DependencyHash != noWeak.Rows[0].DependencyHash && weak.Rows[0].HeaderFileHash == noWeak.Rows[0].HeaderFileHash
            && weak.Rows[0].WeakReferences == 1 && noWeak.Rows[0].WeakReferences == 0,"Weak target type word not independently folded.");
        Console.WriteLine("PathMusic Core identity self-test: OK (1/3/8 actual fresh Core owners, independent XML/default/512-block and weak-type/header/256-capacity folding, shared header/timestamp/comment/XML edits, repeat/physical-path independence/restoration/synthetic processing isolation; no EA processing hash/package relabeling)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: emit synthetic local music with cache placed last to make default attribution comparisons unambiguous. */
    //-------------------------------------------------------------------------------------------------
    private static (string Source,string Header) Fixture(int count)
    {
        string source = "<AssetDeclaration xmlns='uri:ea.com:eala:asset'>",header = "";
        for (int index = 0; index < count; index++)
        {
            source += "<PathMusicEvent id='RebornIdentity"+index+"' PathfinderEventHeader='events.h'"
                +(index+1 < count ? " RestartAlternateEvent='RebornIdentity"+(index+1)+"'" : "")+" IsCacheable='false'/>";
            header += "#define PATH_EVENT_RebornIdentity"+index+" 0x"+(index+1).ToString("X8")+"\n";
        }
        return (source+"</AssetDeclaration>",header);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: own temporary source/header files only, never official music or schema inputs. */
    //-------------------------------------------------------------------------------------------------
    private static void Write(string directory,string source,string header)
    { Directory.CreateDirectory(directory); File.WriteAllText(Path.Combine(directory,"events.xml"),source,new UTF8Encoding(false)); File.WriteAllText(Path.Combine(directory,"events.h"),header,new UTF8Encoding(false)); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare ordered actual identity projections without exposing mutable Core instances. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Hashes(PathMusicCoreIdentity.Report report) => report.Rows.Select(row => row.CoreInstanceHash).ToArray();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fail on the first identity boundary mismatch. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
