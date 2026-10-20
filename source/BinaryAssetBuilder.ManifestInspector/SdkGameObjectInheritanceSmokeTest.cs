using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: test partial source merges, literal/script handling and safe fragment expansion separately from completed native GameObject publication.
internal static class SdkGameObjectInheritanceSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: write only a fresh bounded diagnostic directory; never edit either live mod source tree or launch a game. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string output)
    {
        output=SdkEnvironmentPreflight.Absolute(output);SdkEnvironmentPreflight.CheckPath(output);
        if(File.Exists(output)||Directory.Exists(output))throw new InvalidDataException("Fresh inheritance test directory required.");
        SdkEnvironmentPreflight.DirectoryPath(Path.GetDirectoryName(output)!);Directory.CreateDirectory(output);
        var schemas=ConscriptSourceInput.Schemas();
        const string ea="uri:ea.com:eala:asset";
        var source=Encoding.UTF8.GetBytes("""
        <AssetDeclaration xmlns="uri:ea.com:eala:asset" xmlns:xai="uri:ea.com:eala:asset:instance">
          <!-- Reborn: abstract bases intentionally contain no BuildCost/Amount or complete Conscript graph. -->
          <GameObject id="BaseObject" BuildTime="4"><DisplayName>Name:Base</DisplayName><Behaviors><Physics id="Physics" GravityMultiplier="1"/></Behaviors><Body><ActiveBody id="Body"/></Body></GameObject>
          <GameObject id="BaseInfantry" inheritFrom="BaseObject"><Body><ActiveBody id="Body" MaxHealth="100"/></Body></GameObject>
          <GameObject id="Final" inheritFrom="BaseInfantry" BuildTime="1"><DisplayName xai:joinAction="Replace">Name:Final</DisplayName><Behaviors><Physics id="Physics" GravityMultiplier="2"/><ObjectSMCHelper id="SMC"/></Behaviors><Body><ActiveBody id="Body" MaxHealth="200"/></Body></GameObject>
        </AssetDeclaration>
        """);
        var result=SdkGameObjectInheritance.Apply(schemas,source);Require(result.Bytes!=null,"Partial/nested merge failed: "+string.Join("; ",result.Evidence.Diagnostics));
        var merged=new XmlDocument();merged.LoadXml(Encoding.UTF8.GetString(result.Bytes!));var final=merged.GetElementsByTagName("GameObject",ea).OfType<XmlElement>().Single(e=>e.GetAttribute("id")=="Final");
        Require(final.GetAttribute("BuildTime")=="1"&&!final.HasAttribute("inheritFrom")&&final.GetElementsByTagName("DisplayName",ea)[0]!.InnerText=="Name:Final","Root override or Replace directive differs.");
        Require(((XmlElement)final.GetElementsByTagName("Physics",ea)[0]!).GetAttribute("GravityMultiplier")=="2"&&final.GetElementsByTagName("ObjectSMCHelper",ea).Count==1&&((XmlElement)final.GetElementsByTagName("ActiveBody",ea)[0]!).GetAttribute("MaxHealth")=="200","Nested keyed module/body inheritance differs.");
        Require(!final.HasAttribute("RadarPriority")&&!((XmlElement)final.GetElementsByTagName("ActiveBody",ea)[0]!).HasAttribute("MaxHealthDamagedPercentage"),"Validation injected explicit defaults into source.");
        foreach(string invalid in new[]{"<GameObject id='A' inheritFrom='B'/><GameObject id='B' inheritFrom='A'/>","<GameObject id='A' inheritFrom='Missing'/>","<GameObject id='A'/><GameObject id='A'/>","<GameObject id='A' Unknown='1'/>","<GameObject id='A' inheritFrom='LogicCommand:B'/>"})
            Require(SdkGameObjectInheritance.Apply(schemas,Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='"+ea+"'>"+invalid+"</AssetDeclaration>")).Bytes==null,"Invalid inheritance accepted: "+invalid);
        string fixture=Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"live-alpha","ConscriptBuildTime1","data","mod.xml");var baseline=new XmlDocument{PreserveWhitespace=true};baseline.Load(fixture);
        var authored=(XmlDocument)baseline.CloneNode(true);var model=(XmlElement)authored.GetElementsByTagName("Model",ea)[0]!;string canonical=model.GetAttribute("Name");int colon=canonical.IndexOf(':');Require(colon>0,"Reference fixture has no concrete type.");
        model.SetAttribute("Name",canonical[(colon+1)..].ToLowerInvariant());ConscriptSourceInput.Admit(authored,baseline,schemas);
        Require(model.GetAttribute("Name")==canonical[(colon+1)..].ToLowerInvariant(),"Comparison rewrote actual authored reference text.");
        foreach(string bad in new[]{"RebornMissingModel","W3DAnimation:"+canonical[(colon+1)..]})
        {var changed=(XmlDocument)baseline.CloneNode(true);((XmlElement)changed.GetElementsByTagName("Model",ea)[0]!).SetAttribute("Name",bad);Refuse(()=>ConscriptSourceInput.Admit(changed,baseline,schemas));}
        // Reborn: partial source validity must never bypass the completed-object/native admission gate.
        var partial=new XmlDocument();partial.LoadXml(final.OuterXml);Refuse(()=>ConscriptSourceInput.Admit(partial,baseline,schemas));
        string data=Path.Combine(output,"data");Directory.CreateDirectory(data);string entry=Path.Combine(data,"mod.xml"),fragment=Path.Combine(data,"fragment.xml");
        string fragmentXml="<!-- Reborn: literal fragment fixture. --><Modules xmlns='"+ea+"'><Physics id='FragmentPhysics' GravityMultiplier='3'/></Modules>";File.WriteAllText(fragment,fragmentXml);
        var paths=new SdkSourcePathAudit.Report(data,null,null,Array.Empty<SdkSourcePathAudit.Source>(),Array.Empty<SdkSourcePathAudit.Edge>(),Array.Empty<SdkSourcePathAudit.Resource>(),Array.Empty<SdkSourcePathAudit.Issue>(),true,false,true,false,false,Array.Empty<string>());
        var fragments=new SdkGameObjectFragments(paths);var owner=FragmentOwner("DATA:fragment.xml");fragments.Expand(owner,entry,data);
        Require(owner.GetElementsByTagName("Physics",ea).Count==1&&owner.GetElementsByTagName("include","http://www.w3.org/2001/XInclude").Count==0&&fragments.Sources.Single().Path==fragment,"Fragment selection or source witness differs.");
        foreach(string href in new[]{"DATA:missing.xml","../outside.xml","https://invalid.example/fragment.xml"})Refuse(()=>new SdkGameObjectFragments(paths).Expand(FragmentOwner(href),entry,data));
        var wrong=FragmentOwner("DATA:fragment.xml");((XmlElement)wrong.GetElementsByTagName("include","http://www.w3.org/2001/XInclude")[0]!).SetAttribute("xpointer","xmlns(n=uri:ea.com:eala:asset) xpointer(/n:WrongRoot/child::*)");Refuse(()=>new SdkGameObjectFragments(paths).Expand(wrong,entry,data));
        File.WriteAllText(fragment,"<Modules xmlns='"+ea+"' xmlns:xi='http://www.w3.org/2001/XInclude'><xi:include href='DATA:fragment.xml' xpointer='xmlns(n=uri:ea.com:eala:asset) xpointer(/n:Modules/child::*)'/></Modules>");Refuse(()=>new SdkGameObjectFragments(paths).Expand(FragmentOwner("DATA:fragment.xml"),entry,data));
        File.WriteAllBytes(Path.Combine(output,"merged.xml"),result.Bytes!);
        Console.WriteLine("Reborn: GameObject source inheritance: PASS (partial bases, recursive keyed merges, Replace, no default leakage, final admission retained, reference spelling equivalence/retarget refusal, confined fragments/source witnesses and cycle/missing/network/traversal controls).");

        // Reborn: this fixture uses the same EA child-node XPointer syntax as the stock parachute source.
        static XmlDocument FragmentOwner(string href){var xml=new XmlDocument{PreserveWhitespace=true};xml.LoadXml("<AssetDeclaration xmlns='uri:ea.com:eala:asset' xmlns:xi='http://www.w3.org/2001/XInclude'><GameObject id='FragmentObject'><Behaviors><xi:include href='"+href+"' xpointer='xmlns(n=uri:ea.com:eala:asset) xpointer(/n:Modules/child::*)'/></Behaviors></GameObject></AssetDeclaration>");return xml;}
    }

    // Reborn: refusals are test successes only for explicit source/schema/path errors, never arbitrary programming failures.
    private static void Refuse(Action action){try{action();}catch(Exception error) when(error is IOException or InvalidDataException or System.Xml.Schema.XmlSchemaException or ArgumentException or NotSupportedException){return;}throw new InvalidDataException("Invalid source/fragment was accepted.");}
    // Reborn: keep every compatibility assertion fatal to the diagnostic command.
    private static void Require(bool condition,string message){if(!condition)throw new InvalidDataException(message);}
}
