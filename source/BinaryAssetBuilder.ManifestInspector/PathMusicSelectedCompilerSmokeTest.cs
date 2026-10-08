using System.Text;
using BinaryAssetBuilder.Core;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: characterize actual Core output selection with declared synthetic nonzero local metadata, without stock identity or production claims.
internal static class PathMusicSelectedCompilerSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove original metadata assignment, real dependency preparation, weak self/cycles, checksum separation and failure containment for 1/3/8 owners. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        Require(PathMusicControlledCompiler.LocalTypeHash == 0x48E303B8u && PathMusicControlledCompiler.LocalTypeHash != 0x599CDAF2u
            && PathMusicControlledCompiler.LocalTypeHash != 0x76D0CEEFu,"Declared local type contract changed or collided with observed reference metadata.");
        string root = Path.Combine(Path.GetTempPath(),"Reborn-SelectedMusicV2-"+Guid.NewGuid().ToString("N")); Directory.CreateDirectory(root);
        Settings saved = Settings.Current;
        foreach (int count in new[] { 1,3,8 })
        {
            string input = Path.Combine(root,"input"+count); Directory.CreateDirectory(input);
            string xml = "<AssetDeclaration xmlns='uri:ea.com:eala:asset'>",header = "";
            for (int index = 0; index < count; index++)
            {
                xml += "<PathMusicEvent id='RebornSelected"+index+"' PathfinderEventHeader='events.h' RestartAlternateEvent='RebornSelected"+((index+1)%count)+"'/>";
                header += "#define PATH_EVENT_RebornSelected"+index+" 0x"+(index+1).ToString("X8")+"\n";
            }
            xml += "</AssetDeclaration>"; File.WriteAllText(Path.Combine(input,"events.xml"),xml,new UTF8Encoding(false));
            string headerPath = Path.Combine(input,"events.h"); File.WriteAllText(headerPath,header,new UTF8Encoding(false));
            string[] files = Directory.GetFileSystemEntries(input).Order().ToArray(); bool reachedBefore = false,reachedAfter = false;
            var report = PathMusicControlledCompiler.CompileSelected(input,(_,instances) =>
            {
                reachedBefore = true; Require(instances.All(instance => instance.Handle.TypeHash == 0x48E303B8u && instance.ValidatedReferencedInstances == null
                    && instance.AllDependentInstances == null),"Local v2 metadata was assigned late or dependency tables were pre-forged.");
            },(plugin,instances) =>
            {
                reachedAfter = true; Require(instances.All(instance => instance.ValidatedReferencedInstances != null && instance.ValidatedReferencedInstances.Count == 0
                    && instance.AllDependentInstances != null && instance.AllDependentInstances.Count == 0 && instance.WeakReferencedInstances.Count == 1),"Core weak local closure/dependency preparation differs.");
                Require(plugin.AllTypesHash == 0 && plugin.GetExtendedTypeInformation(0x9A651D89u).TypeHash == 0x48E303B8u
                    && plugin.GetExtendedTypeInformation(0x9A651D89u).ProcessingHash == PathMusicCoreIdentity.Processing,"Local metadata policy differs.");
                // Reborn: prepared tables must remain present and empty; restoring real tables is different from fabricating preparation.
                var owner = instances[0]; var validated = owner.ValidatedReferencedInstances; owner.ValidatedReferencedInstances = null!;
                Refuses(() => plugin.ProcessInstance(owner)); owner.ValidatedReferencedInstances = validated;
                var dependencies = owner.AllDependentInstances; owner.AllDependentInstances = null!; Refuses(() => plugin.ProcessInstance(owner)); owner.AllDependentInstances = dependencies;
                owner.Handle.TypeHash = 0; Refuses(() => plugin.ProcessInstance(owner)); owner.Handle.TypeHash = 0x48E303B8u;
                var weak = owner.WeakReferencedInstances[0]; owner.WeakReferencedInstances[0] = new InstanceHandle("PathMusicEvent","Unknown");
                Refuses(() => plugin.ProcessInstance(owner)); owner.WeakReferencedInstances[0] = weak;
            });
            var repeat = PathMusicControlledCompiler.CompileSelected(input); var old = PathMusicControlledCompiler.Compile(input);
            Require(reachedBefore && reachedAfter && report.TypeHash == 0x48E303B8u && report.LocalProfileVersion == 2 && report.SyntheticTypeDomain
                && !report.CoreOutputSelectionSkipped && report.CoreDependenciesPrepared && !report.Ep1ProcessingHashRecovered && !report.ProductionBuildReady
                && !report.CanUseBuildCache && !report.CanReuseCompiledDocuments && !report.GameLoadProved && report.ProcessorCalls == count
                && report.Rows.SequenceEqual(repeat.Rows) && report.Checksum == repeat.Checksum && report.Rows.SequenceEqual(old.Rows) && report.Checksum != old.Checksum
                && old.TypeHash == 0 && old.CoreOutputSelectionSkipped && !old.CoreDependenciesPrepared && old.LocalProfileVersion == 1
                && files.SequenceEqual(Directory.GetFileSystemEntries(input).Order()) && ReferenceEquals(saved,Settings.Current),"Selected v2 repeat/native/checksum/domain/v1/no-output isolation differs.");
            if (count != 3) continue;
            Refuses(() => PathMusicControlledCompiler.CompileSelected(input,(_,instances) => instances[0].Handle.TypeHash = 0));
            Refuses(() => PathMusicControlledCompiler.CompileSelected(input,(_,instances) => instances[0].ReferencedFiles[0] = "Reborn-missing.h"));
            Refuses(() => PathMusicControlledCompiler.CompileSelected(input,(_,instances) => instances[0].ReferencedInstances.Add(new InstanceHandle("PathMusicEvent","RebornMissingStrong"))));
            Refuses(() => PathMusicControlledCompiler.CompileSelected(input,(_,instances) => instances[0].WeakReferencedInstances[0] = new InstanceHandle("PathMusicEvent","Unknown")));
            Refuses(() => PathMusicControlledCompiler.CompileSelected(input,beforeCompile:(_,_) => File.WriteAllText(headerPath,header+"// Reborn: stale selected build\n",new UTF8Encoding(false))));
            Require(ReferenceEquals(saved,Settings.Current),"Failed selected Core build leaked settings."); File.WriteAllText(headerPath,header,new UTF8Encoding(false));
            Require(PathMusicControlledCompiler.CompileSelected(input).Rows.SequenceEqual(report.Rows),"Restored selected Core build differs.");
        }
        Console.WriteLine("PathMusic selected compiler v2 self-test: OK (declared local type=48E303B8, 1/3/8 original metadata/actual Core selection and dependency tables, weak self/cycles, native/identity invariance and checksum separation, v1 isolation, missing file/strong/weak/zero-hash/stale refusal, settings/no-output/production/cache guards; not EA/EP1 metadata)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: only reviewed local validation and actual Core file/reference failures count as refusal. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuses(Action action)
    {
        try { action(); }
        catch (InvalidDataException) { return; }
        catch (BinaryAssetBuilderException error) when (error.ErrorCode is ErrorCode.FileNotFound or ErrorCode.UnknownReference or ErrorCode.ReferencingError) { return; }
        throw new InvalidDataException("Invalid selected music build accepted.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop immediately on local v2 metadata, selection or containment regressions. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
