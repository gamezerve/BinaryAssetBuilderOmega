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
        // Reborn: pretty-printed empty modules must survive repeated validation, while even whitespace-only string values remain authored data.
        var spacing=SdkGameObjectInheritance.Apply(schemas,Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='"+ea+"'><GameObject id='Whitespace'><DisplayName>   </DisplayName><Behaviors><ObjectSMCHelper id='Empty'>\n  <!-- Reborn: empty module formatting. -->\n</ObjectSMCHelper></Behaviors><Body><ActiveBody id='Body'/></Body></GameObject></AssetDeclaration>"));
        Require(spacing.Bytes!=null,"Schema-empty formatting normalization failed: "+string.Join("; ",spacing.Evidence.Diagnostics));
        var spacingXml=SdkXmlDiagnostics.Parse(spacing.Bytes!);Require(spacingXml.GetElementsByTagName("DisplayName",ea)[0]!.InnerText=="   ","Whitespace-only string payload was trimmed.");
        spacingXml.Schemas.Add(schemas);spacingXml.Validate((_,e)=>throw SdkXmlDiagnostics.SchemaError(e));
        foreach(string invalid in new[]{"<GameObject id='A' inheritFrom='B'/><GameObject id='B' inheritFrom='A'/>","<GameObject id='A' inheritFrom='Missing'/>","<GameObject id='A'/><GameObject id='A'/>","<GameObject id='A' Unknown='1'/>","<GameObject id='A' inheritFrom='LogicCommand:B'/>"})
            Require(SdkGameObjectInheritance.Apply(schemas,Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='"+ea+"'>"+invalid+"</AssetDeclaration>")).Bytes==null,"Invalid inheritance accepted: "+invalid);
        string fixture=Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"live-alpha","ConscriptBuildTime1","data","mod.xml");var baseline=new XmlDocument{PreserveWhitespace=true};baseline.Load(fixture);
        var authored=(XmlDocument)baseline.CloneNode(true);var model=(XmlElement)authored.GetElementsByTagName("Model",ea)[0]!;string canonical=model.GetAttribute("Name");int colon=canonical.IndexOf(':');Require(colon>0,"Reference fixture has no concrete type.");
        model.SetAttribute("Name",canonical[(colon+1)..].ToLowerInvariant());ConscriptSourceInput.Admit(authored,baseline,schemas);
        Require(model.GetAttribute("Name")==canonical[(colon+1)..].ToLowerInvariant(),"Comparison rewrote actual authored reference text.");
        // Reborn: syntactically valid retargets reach catalog validation during compilation; source comparison no longer pins names to stock.
        model.SetAttribute("Name","RebornMissingModel");ConscriptSourceInput.Admit(authored,baseline,schemas);
        // Reborn: a typed reference is no longer rejected merely for differing from the captured render template.
        foreach(string bad in new[]{"W3DAnimation:"+canonical[(colon+1)..]})
        {var changed=(XmlDocument)baseline.CloneNode(true);((XmlElement)changed.GetElementsByTagName("Model",ea)[0]!).SetAttribute("Name",bad);ConscriptSourceInput.Admit(changed,baseline,schemas);}
        // Reborn: general partial objects are supported; malformed numeric source must still fail the inheritance/schema stage.
        var partial=new XmlDocument();partial.LoadXml(final.OuterXml);((XmlElement)partial.GetElementsByTagName("ActiveBody",ea)[0]!).SetAttribute("MaxHealth","not-a-number");Require(SdkGameObjectInheritance.Apply(schemas,Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='"+ea+"'>"+partial.DocumentElement!.OuterXml+"</AssetDeclaration>")).Bytes==null,"Malformed inherited source accepted.");
        string data=Path.Combine(output,"data");Directory.CreateDirectory(data);string entry=Path.Combine(data,"mod.xml"),fragment=Path.Combine(data,"fragment.xml");
        string fragmentXml="<!-- Reborn: literal fragment fixture. --><Modules xmlns='"+ea+"'><Physics id='FragmentPhysics' GravityMultiplier='3'/></Modules>";File.WriteAllText(fragment,fragmentXml);
        var paths=new SdkSourcePathAudit.Report(data,null,null,Array.Empty<SdkSourcePathAudit.Source>(),Array.Empty<SdkSourcePathAudit.Edge>(),Array.Empty<SdkSourcePathAudit.Resource>(),Array.Empty<SdkSourcePathAudit.Issue>(),true,false,true,false,false,Array.Empty<string>());
        var fragments=new SdkGameObjectFragments(paths);var owner=FragmentOwner("DATA:fragment.xml");fragments.Expand(owner,entry,data);
        Require(owner.GetElementsByTagName("Physics",ea).Count==1&&owner.GetElementsByTagName("include","http://www.w3.org/2001/XInclude").Count==0&&fragments.Sources.Single().Path==fragment,"Fragment selection or source witness differs.");
        // Reborn: ordinary EA includes preserve the root module and attributes, recurse relative to that file, and retain all captured source witnesses.
        string nested=Path.Combine(data,"nested");Directory.CreateDirectory(nested);
        File.WriteAllText(Path.Combine(nested,"repair.xml"),"<RepairUpdate xmlns='"+ea+"' xmlns:xi='http://www.w3.org/2001/XInclude' id='Repair'><xi:include href='self.xml' parse='xml'/></RepairUpdate>");
        File.WriteAllText(Path.Combine(nested,"self.xml"),"<SelfAudio xmlns='"+ea+"'/>");
        var rootOwner=FragmentOwner("DATA:nested/repair.xml");((XmlElement)rootOwner.GetElementsByTagName("include","http://www.w3.org/2001/XInclude")[0]!).RemoveAttribute("xpointer");
        var roots=new SdkGameObjectFragments(paths);roots.Expand(rootOwner,entry,data);
        Require(rootOwner.SelectSingleNode("//*[local-name()='Behaviors']/*[local-name()='RepairUpdate'][@id='Repair']/*[local-name()='SelfAudio']")!=null&&roots.Sources.Count()==2&&roots.Snapshots.Count()==2,"Whole-root/nested/empty-element include or source witnesses differ.");
        // Reborn: an explicitly empty pointer is malformed, while text mode, wrong namespaces and ordinary-root cycles remain errors.
        var emptyPointer=FragmentOwner("DATA:fragment.xml");((XmlElement)emptyPointer.GetElementsByTagName("include","http://www.w3.org/2001/XInclude")[0]!).SetAttribute("xpointer","");Refuse(()=>new SdkGameObjectFragments(paths).Expand(emptyPointer,entry,data));
        var textOwner=FragmentOwner("DATA:nested/repair.xml");var textInclude=(XmlElement)textOwner.GetElementsByTagName("include","http://www.w3.org/2001/XInclude")[0]!;textInclude.RemoveAttribute("xpointer");textInclude.SetAttribute("parse","text");Refuse(()=>new SdkGameObjectFragments(paths).Expand(textOwner,entry,data));
        File.WriteAllText(Path.Combine(nested,"repair.xml"),"<RepairUpdate xmlns='wrong' id='Repair'/>");textInclude.SetAttribute("parse","xml");Refuse(()=>new SdkGameObjectFragments(paths).Expand(textOwner,entry,data));
        File.WriteAllText(Path.Combine(nested,"repair.xml"),"<RepairUpdate xmlns='"+ea+"' xmlns:xi='http://www.w3.org/2001/XInclude' id='Repair'><xi:include href='repair.xml'/></RepairUpdate>");Refuse(()=>new SdkGameObjectFragments(paths).Expand(textOwner,entry,data));
        foreach(string href in new[]{"DATA:missing.xml","../outside.xml","https://invalid.example/fragment.xml"})Refuse(()=>new SdkGameObjectFragments(paths).Expand(FragmentOwner(href),entry,data));
        var wrong=FragmentOwner("DATA:fragment.xml");((XmlElement)wrong.GetElementsByTagName("include","http://www.w3.org/2001/XInclude")[0]!).SetAttribute("xpointer","xmlns(n=uri:ea.com:eala:asset) xpointer(/n:WrongRoot/child::*)");Refuse(()=>new SdkGameObjectFragments(paths).Expand(wrong,entry,data));
        File.WriteAllText(fragment,"<Modules xmlns='"+ea+"' xmlns:xi='http://www.w3.org/2001/XInclude'><xi:include href='DATA:fragment.xml' xpointer='xmlns(n=uri:ea.com:eala:asset) xpointer(/n:Modules/child::*)'/></Modules>");Refuse(()=>new SdkGameObjectFragments(paths).Expand(FragmentOwner("DATA:fragment.xml"),entry,data));
        // Reborn: the original instance locomotor library includes both locomotors and its schema-declared terrain mapping helper.
        var library=new XmlDocument();library.LoadXml("<AssetDeclaration xmlns='"+ea+"'/>");
        foreach(string type in new[]{"BounceKickTerrainMapping","LocomotorTemplate"})foreach(var asset in GameObjectExtendedRootContract.Original("GlobalData/Locomotor.xml",type))library.DocumentElement!.AppendChild(library.ImportNode(asset,true));
        var locomotors=SdkGameObjectInheritance.Apply(schemas,Encoding.UTF8.GetBytes(library.OuterXml));
        Require(locomotors.Bytes!=null,"Original instance locomotor library failed: "+string.Join("; ",locomotors.Evidence.Diagnostics));
        var resolvedLibrary=new XmlDocument();resolvedLibrary.LoadXml(Encoding.UTF8.GetString(locomotors.Bytes!));
        Require(resolvedLibrary.GetElementsByTagName("BounceKickTerrainMapping",ea).Count==1&&resolvedLibrary.GetElementsByTagName("LocomotorTemplate",ea).Count==122,"Original locomotor/helper source inventory differs.");
        // Reborn: identity failures identify both the failure kind and exact offending asset node for later source path/line mapping.
        foreach(var sample in new[]{("<UnknownAsset id='Unknown'/>","UNSUPPORTEDASSET","UnknownAsset:Unknown"),("<GameObject id='Invalid ID'/>","INVALIDASSETID","GameObject:Invalid ID"),("<GameObject id='Duplicate'/><GameObject id='Duplicate'/>","DUPLICATEASSET","GameObject:Duplicate")}){
            var rejected=SdkGameObjectInheritance.Apply(schemas,Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='"+ea+"'>"+sample.Item1+"</AssetDeclaration>"));
            Require(rejected.Bytes==null&&rejected.DiagnosticNode!=null&&rejected.Evidence.Diagnostics.Any(d=>d.Contains(sample.Item2,StringComparison.Ordinal)&&d.Contains(sample.Item3,StringComparison.Ordinal)),"Source identity diagnostic lost its kind, handle or offending node.");
        }
        Console.WriteLine("Reborn: source identity diagnostics and complete original locomotor instance library: PASS (122 locomotors, one terrain mapping helper; unsupported/invalid/duplicate diagnostics retain asset handles and offending nodes).");
        File.WriteAllBytes(Path.Combine(output,"merged.xml"),result.Bytes!);
        Console.WriteLine("Reborn: GameObject source inheritance: PASS (partial bases, recursive keyed merges, Replace, no default leakage, final admission retained, reference spelling equivalence/typed retarget admission, whole-root/nested/empty-module and child-XPointer includes/source witnesses, cycle/missing/network/traversal/text/namespace controls).");

        // Reborn: this fixture uses the same EA child-node XPointer syntax as the stock parachute source.
        static XmlDocument FragmentOwner(string href){var xml=new XmlDocument{PreserveWhitespace=true};xml.LoadXml("<AssetDeclaration xmlns='uri:ea.com:eala:asset' xmlns:xi='http://www.w3.org/2001/XInclude'><GameObject id='FragmentObject'><Behaviors><xi:include href='"+href+"' xpointer='xmlns(n=uri:ea.com:eala:asset) xpointer(/n:Modules/child::*)'/></Behaviors></GameObject></AssetDeclaration>");return xml;}
    }

    // Reborn: refusals are test successes only for explicit source/schema/path errors, never arbitrary programming failures.
    private static void Refuse(Action action){try{action();}catch(Exception error) when(error is IOException or InvalidDataException or System.Xml.Schema.XmlSchemaException or ArgumentException or NotSupportedException){return;}throw new InvalidDataException("Invalid source/fragment was accepted.");}
    // Reborn: keep every compatibility assertion fatal to the diagnostic command.
    private static void Require(bool condition,string message){if(!condition)throw new InvalidDataException(message);}
}
