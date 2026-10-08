using System.Security.Cryptography;
using System.Text;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: test two exact library/all aliases without depending on external game/source files.
internal static class SdkKnownMapAliasesSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve original edge roles/spelling, capture targets once, reject changed/forged aliases and retain generic unsafe-path refusal. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        string root = Path.Combine(Path.GetTempPath(),"Reborn-MapAliases-"+Guid.NewGuid().ToString("N")),entry = Path.Combine(root,"global.xml"),output = root+"-NoOutput";
        string library = Path.Combine(root,"SkirmishAI","Personalities","AIPersonalityLibrary.xml"),states = Path.Combine(root,"SkirmishAI","States","AIStateLibrary.xml");
        string target = Path.Combine(root,"maps","official","CAMP_S06_Iceland_Bass","AIP_S06_SovietKrukov.xml"),stateTarget = Path.Combine(Path.GetDirectoryName(target)!,"AIS_S06_SovietKrukov.xml");
        const string logical = "DATA:maps/official/CAMP_S06_Iceland_Bass/AIP_S06_SovietKrukov.xml.";
        const string stateLogical = "DATA:maps/official/CAMP_S06_Iceland_Bass/AIS_S06_SovietKrukov.xml.";
        Write(target,""); Write(stateTarget,"");
        Write(library,"<Includes><Include type='all' source='"+logical+"'/><Include type='all' source='"+logical+"'/></Includes>");
        Write(states,"<Includes><Include type='all' source='"+stateLogical+"'/></Includes>");
        Write(entry,"<Includes><Include type='all' source='DATA:SkirmishAI/Personalities/AIPersonalityLibrary.xml'/><Include type='all' source='DATA:SkirmishAI/States/AIStateLibrary.xml'/></Includes>");
        var environment = SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),root,entry,output,Array.Empty<string>());
        var strict = SdkSourcePathAudit.Inspect(environment);
        Require(strict.Sources.Length == 3 && strict.Issues.Length == 3 && strict.KnownMapAliases.Length == 0,"Default path audit silently enabled aliases.");
        if (!OperatingSystem.IsWindows())
        {
            Refuses(() => SdkKnownMapAliases.Prove(root,library,"all",logical));
            Console.WriteLine("SDK known map aliases self-test: OK (non-Windows admission remains closed)"); return;
        }
        byte[] libraryRaw = File.ReadAllBytes(library);
        var admitted = SdkSourcePathAudit.Inspect(environment,knownMapAliases:true);
        Require(admitted.Sources.Length == 5 && admitted.Includes.Length == 5 && admitted.KnownMapAliases.Length == 2 && admitted.Issues.Length == 0
            && admitted.ScopedPathAuditComplete && admitted.Includes.Count(edge => edge.LogicalPath == logical && edge.Kind == "all") == 2
            && File.ReadAllBytes(library).SequenceEqual(libraryRaw) && !Directory.Exists(output),"Alias target capture/raw roles/output changed.");
        var resolved = SdkKnownMapAliases.ResolveCaptured(admitted,library,"all",logical,root);
        Require(resolved.Path.Equals(target,StringComparison.OrdinalIgnoreCase),"Captured alias differs from exact target.");
        System.Xml.Schema.XmlSchemaSet? schemas = null;
        var schema = SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:set => schemas = set);
        Require(schema.SchemaAdmitted && schemas != null && SdkTypedSourceGraph.BindGraph(schemas,admitted,instanceSoundSingletons:true).ScopedGraphComplete,"Expanded owned source graph failed schema/preparation.");
        Refuses(() => SdkKnownMapAliases.Prove(root,entry,"all",logical));
        Refuses(() => SdkKnownMapAliases.Prove(root,library,"instance",logical));
        Refuses(() => SdkKnownMapAliases.ResolveCaptured(admitted,library,"instance",logical,root));
        foreach (string unsafeLogical in new[] { logical+".",logical+" ","DATA:arbitrary.xml.","DATA:../outside.xml.","DATA:NUL.xml." })
        {
            Require(SdkKnownMapAliases.Prove(root,library,"all",unsafeLogical) == null,"Unknown alias got a proof recipe.");
            Refuses(() => SdkSourcePathAudit.Resolve(unsafeLogical,Path.GetDirectoryName(library)!,root,root,null,null));
        }
        var alias = admitted.KnownMapAliases.Single(item => item.LogicalPath == logical);
        Refuses(() => SdkKnownMapAliases.ResolveCaptured(admitted with { KnownMapAliases = new[] { alias with { Sha256 = "forged" } } },library,"all",logical,root));
        Refuses(() => SdkKnownMapAliases.ResolveCaptured(admitted with { KnownMapAliases = new[] { alias with { PhysicalPath = output } } },library,"all",logical,root));
        Write(target,"<Defines><Define name='Changed' value='1'/></Defines>");
        Refuses(() => SdkKnownMapAliases.ResolveCaptured(admitted,library,"all",logical,root));
        File.WriteAllText(target,"<!DOCTYPE x [<!ENTITY e SYSTEM 'file:///forbidden'>]><AssetDeclaration xmlns='uri:ea.com:eala:asset'>&e;</AssetDeclaration>");
        Refuses(() => SdkSourcePathAudit.Inspect(environment,knownMapAliases:true));
        Require(Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(library))) == Convert.ToHexString(SHA256.HashData(libraryRaw)),"Alias fixture source library changed.");
        Console.WriteLine("SDK known map aliases self-test: OK (exact library/all/native proof, duplicate-edge shared capture, expanded fixture graph, strict old paths, wrong role/context, generic unsafe paths, forged/stale witnesses and uncaptured DTD target refusal; no source/output mutation)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: create owned fixture directories/documents only, never edit official source material. */
    //-------------------------------------------------------------------------------------------------
    private static void Write(string path,string body)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!); File.WriteAllBytes(path,Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+body+"</AssetDeclaration>"));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: changed identity or unsupported native/path semantics must refuse rather than falling back. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuses(Action action)
    {
        try { action(); } catch (Exception error) when (error is InvalidDataException or NotSupportedException) { return; }
        throw new InvalidDataException("Unproved map alias accepted.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop if an alias loses its original role, context, confinement or source snapshot. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
