using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: exercise private registered music dispatch on real Core instances with production/cache/output guards intact.
internal static class PathMusicControlledCompilerSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: test 1/3/8 prepared native dispatches, repeatability, mutable-object refusal, platform/metadata isolation and source freshness. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        string root = Path.Combine(Path.GetTempPath(),"Reborn-ControlledMusic-"+Guid.NewGuid().ToString("N")); Directory.CreateDirectory(root);
        Settings saved = Settings.Current;
        foreach (int count in new[] { 1,3,8 })
        {
            string input = Path.Combine(root,"input"+count); Directory.CreateDirectory(input);
            string source = "<AssetDeclaration xmlns='uri:ea.com:eala:asset'>",header = "";
            for (int index = 0; index < count; index++)
            {
                source += "<PathMusicEvent id='RebornControlled"+index+"' PathfinderEventHeader='events.h'"
                    +(index+1 < count ? " RestartAlternateEvent='RebornControlled"+(index+1)+"'" : "")
                    +(index == 0 ? " IsCacheable='false'" : "")+"/>";
                header += "#define PATH_EVENT_RebornControlled"+index+" 0x"+(index+1).ToString("X8")+"\n";
            }
            source += "</AssetDeclaration>"; string headerPath = Path.Combine(input,"events.h");
            File.WriteAllText(Path.Combine(input,"events.xml"),source,new UTF8Encoding(false)); File.WriteAllText(headerPath,header,new UTF8Encoding(false));
            string[] files = Directory.GetFileSystemEntries(input).Order().ToArray(); var report = PathMusicControlledCompiler.Compile(input);
            var repeat = PathMusicControlledCompiler.Compile(input);
            Require(report.Rows.Length == count && report.ProcessorCalls == count && report.Checksum == repeat.Checksum && report.Rows.SequenceEqual(repeat.Rows)
                && report.Rows.All(row => row.ImportsBytes == 0) && !report.ProductionBuildReady && !report.CanUseBuildCache && !report.CanReuseCompiledDocuments
                && !report.GameLoadProved && report.CoreOutputSelectionSkipped && report.SyntheticProcessingDomain && ReferenceEquals(saved,Settings.Current)
                && files.SequenceEqual(Directory.GetFileSystemEntries(input).Order()),"Controlled dispatch/repeat/policies/settings/no-output differs.");
            if (count != 3) continue;
            // Reborn: test callbacks receive only owned temporary Core objects; each refused mutation is restored before normal dispatch resumes.
            var checkedReport = PathMusicControlledCompiler.Compile(input,(plugin,instances) =>
            {
                var instance = instances[0]; AssetBuffer baseline = plugin.ProcessInstance(instance); baseline.InstanceData[4] ^= 1;
                Require(plugin.ProcessInstance(instance).InstanceData[4] == 1,"Native buffers alias prior returned bytes.");
                var metadata = plugin.GetExtendedTypeInformation(0x9A651D89u); metadata.TypeHash = 1;
                Require(plugin.GetExtendedTypeInformation(0x9A651D89u).TypeHash == 0,"Metadata return aliases captured admission.");
                Refuses(() => plugin.GetExtendedTypeInformation(0));
                Refuses(() => plugin.ProcessInstance(PathMusicCoreChecksum.Identity(instance.Handle.InstanceName,instance.Handle.InstanceHash)));
                XmlElement node = (XmlElement)instance.XmlNode;
                foreach (var change in new[] { ("id","Other"),("TypeId","0"),("PathfinderEventHeader","elsewhere.h"),("IsCacheable","true"),
                    ("IsCacheable","broken"),("RestartAlternateEvent","Unknown"),("Unknown","x"),("inheritFrom","PathMusicEvent:Other") })
                    Mutate(plugin,instance,node,change.Item1,change.Item2);
                // Reborn: adding/removing a child changes self-closing lexical form; restore that state as well as child membership.
                bool wasEmpty = node.IsEmpty; XmlElement child = node.OwnerDocument.CreateElement("RebornUnknown",node.NamespaceURI); node.AppendChild(child);
                Refuses(() => plugin.ProcessInstance(instance)); node.RemoveChild(child); node.IsEmpty = wasEmpty;
                instance.Handle.TypeHash = 1; Refuses(() => plugin.ProcessInstance(instance)); instance.Handle.TypeHash = 0;
                instance.Handle.InstanceHash ^= 1; Refuses(() => plugin.ProcessInstance(instance)); instance.Handle.InstanceHash ^= 1;
                instance.ProcessingHash ^= 1; Refuses(() => plugin.ProcessInstance(instance)); instance.ProcessingHash ^= 1;
                instance.HasCustomData = true; Refuses(() => plugin.ProcessInstance(instance)); instance.HasCustomData = false;
                instance.ReferencedFiles[0] = "other.h"; Refuses(() => plugin.ProcessInstance(instance)); instance.ReferencedFiles[0] = "events.h";
                instance.ReferencedInstances.Add(instances[1].Handle); Refuses(() => plugin.ProcessInstance(instance)); instance.ReferencedInstances.Clear();
                // Reborn: zero TypeHash must not masquerade as a Core-prepared dependency table.
                var validated = instance.ValidatedReferencedInstances; instance.ValidatedReferencedInstances = new List<InstanceHandle>();
                Refuses(() => plugin.ProcessInstance(instance)); instance.ValidatedReferencedInstances = validated;
                var weak = instance.WeakReferencedInstances[0]; instance.WeakReferencedInstances[0] = new InstanceHandle("PathMusicEvent","Unknown");
                Refuses(() => plugin.ProcessInstance(instance)); instance.WeakReferencedInstances[0] = weak;
                Refuses(() => plugin.ReInitialize(TargetPlatform.Xbox360)); Refuses(() => plugin.ProcessInstance(instance)); plugin.ReInitialize(TargetPlatform.Win32);
                TargetPlatform platform = Settings.Current.TargetPlatform; Settings.Current.TargetPlatform = TargetPlatform.Xbox360;
                Refuses(() => plugin.ProcessInstance(instance)); Settings.Current.TargetPlatform = platform;
                File.WriteAllText(headerPath,header+"// Reborn: owned late edit\n",new UTF8Encoding(false)); Refuses(() => plugin.ProcessInstance(instance));
                File.WriteAllText(headerPath,header,new UTF8Encoding(false));
                Require(plugin.ProcessInstance(instance).InstanceData.SequenceEqual(PathMusicRuntimeProbe.EncodeObserved(1,"RebornControlled1",false).InstanceBuffer),"Restored plugin did not recover.");
            });
            Require(checkedReport.Rows.SequenceEqual(report.Rows) && ReferenceEquals(saved,Settings.Current),"Owned mutation checks changed final native evidence/settings.");
            Refuses(() => PathMusicControlledCompiler.Compile(input,(_,_) => File.WriteAllText(headerPath,header+"// Reborn: fatal stale edit\n",new UTF8Encoding(false))));
            Require(ReferenceEquals(saved,Settings.Current),"Failed controlled build leaked settings."); File.WriteAllText(headerPath,header,new UTF8Encoding(false));
            Require(PathMusicControlledCompiler.Compile(input).Rows.SequenceEqual(report.Rows),"Controlled build did not recover after input restoration.");
        }
        Console.WriteLine("PathMusic controlled compiler self-test: OK (1/3/8 fresh Core XML/private plugin dispatch, zero-TypeHash Core output exclusion, frozen native/checksum equivalence/repeat/detachment, XML/identity/file/weak/strong/forged-table/foreign/platform/stale refusal, settings restoration, production/cache/reuse/no-output guards)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: restore each owned normalized attribute after refusing current malformed or changed XML. */
    //-------------------------------------------------------------------------------------------------
    private static void Mutate(IAssetBuilderPlugin plugin,InstanceDeclaration instance,XmlElement node,string name,string value)
    {
        bool present = node.HasAttribute(name),specified = node.GetAttributeNode(name)?.Specified ?? false;
        string old = node.GetAttribute(name),before = node.OuterXml; node.SetAttribute(name,value);
        // Reborn: setting a schema-default attribute makes it explicit; remove it on restoration to recover its default provenance rather than writing the same value.
        try { Refuses(() => plugin.ProcessInstance(instance)); }
        finally { if (present && specified) node.SetAttribute(name,old); else node.RemoveAttribute(name); }
        Require(node.OuterXml == before,"Owned attribute restoration changed normalized XML: "+name);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: only explicit validation/platform failures count as successful refusal. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuses(Action action)
    { try { action(); } catch (Exception error) when (error is InvalidDataException or NotSupportedException) { return; } throw new InvalidDataException("Invalid controlled music dispatch accepted."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop immediately on controlled native dispatch or containment regressions. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
