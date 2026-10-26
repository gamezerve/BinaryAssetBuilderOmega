using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core.SageXml;
namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: same-ID inheritance resolves source instances, not native stock payloads or self-recursive local nodes.
internal static class SdkSameIdInstanceSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate the original PlayerSpellBook source overlay and a complete same-ID Conscript package using only private files. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global,string output)
    {
        output=SdkEnvironmentPreflight.Absolute(output);if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh same-ID test output required.");Directory.CreateDirectory(output);
        string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,repo=Path.GetFullPath(Path.Combine(fixtures,"../..")),sdk=Path.Combine(repo,"alpha");
        string playerData=Path.Combine(output,"player","Data");Directory.CreateDirectory(playerData);string playerEntry=Path.Combine(playerData,"Static_Mod.xml"),stockPlayer=Path.Combine(sdk,"SageXML","System","PlayerSpellBook.xml");byte[] stockBytes=File.ReadAllBytes(stockPlayer);
        File.WriteAllText(playerEntry,"<AssetDeclaration xmlns='uri:ea.com:eala:asset'>\n<Includes><Include type='instance' source='DATA:System/PlayerSpellBook.xml'/></Includes>\n<GameObject id='PlayerSpellBook' inheritFrom='PlayerSpellBook' EditorName='RebornPlayerSpellBook'/>\n</AssetDeclaration>");var player=ConscriptSourcePreparation.Load(playerEntry,SdkSourceSearch.ForMod(playerEntry,sdk));var game=player.Game.DocumentElement!;
        Require(game.GetAttribute("id")=="PlayerSpellBook"&&game.GetAttribute("EditorName")=="RebornPlayerSpellBook"&&game.GetAttribute("Side")=="Neutral"&&!game.HasAttribute("inheritFrom")&&player.AdditionalAssets.Length==0&&game.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).Count==1,"Original PlayerSpellBook same-ID preparation lost inherited fields or emitted its base.");
        // Reborn: local override fields map to the mod source; inherited fields map to the captured original XML.
        foreach(var field in new[]{("EditorName",playerEntry),("Side",stockPlayer)}){var error=SdkXmlDiagnostics.At(new InvalidDataException("Same-ID source field probe."),game.GetAttributeNode(field.Item1));player.Locations!.Enrich(error,playerEntry,player.Game);var origin=SdkXmlDiagnostics.Origin(error);Require(origin!=null&&!origin.Context&&origin.Line>0&&origin.File.Equals(field.Item2,StringComparison.OrdinalIgnoreCase),"Same-ID source location lost its defining owner: "+field.Item1);}
        Require(File.ReadAllBytes(stockPlayer).SequenceEqual(stockBytes),"Original PlayerSpellBook source changed.");
        string data=Path.Combine(output,"conscript","Data");Directory.CreateDirectory(data);string basis=Path.Combine(data,"base.xml"),entry=Path.Combine(data,"Static_Mod.xml");var baseXml=SdkXmlDiagnostics.Parse(File.ReadAllBytes(Path.Combine(fixtures,"live-alpha/ConscriptBuildTime1/data/mod.xml")));baseXml.DocumentElement!.SetAttribute("BuildTime","4");string id=baseXml.DocumentElement.GetAttribute("id");File.WriteAllText(basis,"<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+baseXml.DocumentElement.OuterXml+"</AssetDeclaration>");
        string valid="<AssetDeclaration xmlns='uri:ea.com:eala:asset'><Includes><Include type='instance' source='base.xml'/></Includes><Defines><Define name='BUILD_TIME' value='2'/></Defines><GameObject id='"+id+"' inheritFrom='GameObject:"+id+"' BuildTime='=$BUILD_TIME'/></AssetDeclaration>";File.WriteAllText(entry,valid);
        var prepared=ConscriptSourcePreparation.Load(entry);Require(prepared.Game.DocumentElement!.GetAttribute("id")==id&&prepared.Game.DocumentElement.GetAttribute("BuildTime")=="2"&&prepared.AdditionalAssets.Length==0&&!prepared.Game.OuterXml.Contains("__RebornInstanceBase_",StringComparison.Ordinal),"Same-ID Conscript source/definition expansion or base removal failed.");
        // Reborn: include explicit command/set consumers so the same mixed package path used by Reborn also proves local references to the override.
        string packageEntry=Path.Combine(data,"package.xml");File.WriteAllText(packageEntry,valid.Replace("</AssetDeclaration>","<LogicCommand id='RebornInstanceBuild' Type='UNIT_BUILD'><Object>"+id+"</Object></LogicCommand><LogicCommandSet id='RebornInstanceCommands'><Cmd>RebornInstanceBuild</Cmd></LogicCommandSet></AssetDeclaration>",StringComparison.Ordinal));
        string preparedPath=Path.Combine(output,"prepared");ConscriptSourceInput.Prepare(packageEntry,preparedPath);if(!ConscriptMultiAssetBuild.TryRun(Path.Combine(preparedPath,"game.xml"),manifest,global,Path.Combine(output,"built")))throw new InvalidDataException("Same-ID complete GameObject route not selected.");
        // Reborn: another local consumer inherits the resolved override, and a legitimate authored alias-like ID cannot collide with a private base.
        File.WriteAllText(entry,valid.Replace("</AssetDeclaration>","<GameObject id='Follower' inheritFrom='"+id+"'/><GameObject id='__RebornInstanceBase_0' inheritFrom='"+id+"'/></AssetDeclaration>",StringComparison.Ordinal));var collision=ConscriptSourcePreparation.Load(entry);Require(collision.AdditionalAssets.Length==2&&collision.AdditionalAssets.Single(d=>d.DocumentElement!.GetAttribute("id")=="Follower").DocumentElement!.GetAttribute("BuildTime")=="2"&&collision.AdditionalAssets.Any(d=>d.DocumentElement!.GetAttribute("id")=="__RebornInstanceBase_0"),"Private base collided with an authored ID or the local child bypassed the override.");
        // Reborn: source visibility still rejects missing/foreign-role/ambiguous bases and genuine local/document cycles.
        File.WriteAllText(entry,valid.Replace("<Include type='instance' source='base.xml'/>",string.Empty,StringComparison.Ordinal));Refuse(()=>ConscriptSourcePreparation.Load(entry));
        File.WriteAllText(entry,valid.Replace("type='instance'","type='reference'",StringComparison.Ordinal));Refuse(()=>ConscriptSourcePreparation.Load(entry));
        File.WriteAllText(entry,valid.Replace("type='instance'","type='all'",StringComparison.Ordinal));Refuse(()=>ConscriptSourcePreparation.Load(entry));
        string other=Path.Combine(data,"other.xml");File.Copy(basis,other);File.WriteAllText(entry,valid.Replace("</Includes>","<Include type='instance' source='other.xml'/></Includes>",StringComparison.Ordinal));Refuse(()=>ConscriptSourcePreparation.Load(entry));
        File.WriteAllText(entry,"<AssetDeclaration xmlns='uri:ea.com:eala:asset'><GameObject id='A' inheritFrom='B'/><GameObject id='B' inheritFrom='A'/></AssetDeclaration>");Refuse(()=>ConscriptSourcePreparation.Load(entry));
        byte[] originalBaseBytes=File.ReadAllBytes(basis);
        File.WriteAllText(basis,"<AssetDeclaration xmlns='uri:ea.com:eala:asset'><Includes><Include type='instance' source='Static_Mod.xml'/></Includes><GameObject id='"+id+"' inheritFrom='"+id+"'/></AssetDeclaration>");File.WriteAllText(entry,valid);Refuse(()=>ConscriptSourcePreparation.Load(entry));
        // Reborn: leave the private package source witnesses unchanged after deliberate invalid-graph controls.
        File.WriteAllBytes(basis,originalBaseBytes);File.WriteAllText(entry,valid);
        Console.WriteLine("Reborn: same-ID instance inheritance: PASS (original PlayerSpellBook and exact local/base source coordinates, typed same-ID Conscript with definitions, complete stream/package, local child, alias collision, missing/reference/all/ambiguous/local-cycle/document-cycle refusals; no user output or game launch).");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: count only expected source/schema/path refusals, never arbitrary implementation failures. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action){try{action();}catch(Exception error)when(error is InvalidDataException or XmlException or System.Xml.Schema.XmlSchemaException){return;}throw new InvalidDataException("Invalid same-ID source graph was accepted.");}
    // Reborn: keep source/native and isolation assertions fatal to the diagnostic command.
    private static void Require(bool condition,string message){if(!condition)throw new InvalidDataException(message);}
}
