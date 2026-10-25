using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: fragments are textually included in the defining owner's expression scope, with raw file witnesses and physical diagnostics retained.
internal static class FragmentDefinitionSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prepare original turret expressions, nested child selections, repeated contexts, inheritance and missing-definition diagnostics privately. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string output)
    {
        output=SdkEnvironmentPreflight.Absolute(output);if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh fragment-definition output required.");Directory.CreateDirectory(output);
        const string ea="uri:ea.com:eala:asset";string entry=Path.Combine(output,"mod.xml"),fragment=Path.Combine(output,"weapons.xml"),defs=Path.Combine(output,"defs.xml");
        string original=GameObjectExtendedRootContract.Original("EP1/Includes/SovietHeavyWalkerWeaponSet.xml","WeaponSetUpdate").Single().OuterXml;File.WriteAllText(fragment,original);Put(defs,"<Defines><Define name='FAST_IDLE_SCAN_DELAY' value='0.0s'/><Define name='MEDIUM_IDLE_SCAN_DELAY' value='0.4s'/></Defines>");
        string own="<GameObject id='Walker'><Behaviors><xi:include href='weapons.xml'/></Behaviors><Body><ActiveBody id='Body' MaxHealth='100'/></Body></GameObject>";Put(entry,"<Includes><Include type='all' source='defs.xml'/></Includes>"+own);var prepared=ConscriptSourcePreparation.Load(entry);CheckDelays(prepared.Game,"0.0s","0.4s");
        Require(prepared.Sources.Any(s=>s.Path==fragment&&s.Sha256==Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(fragment))))&&File.ReadAllText(fragment)==original,"Raw fragment witness or authored bytes changed.");
        // Reborn: the same fragment resolves independently in each declaring document; a base never borrows the derived object's local definitions.
        string baseFile=Path.Combine(output,"base.xml");Put(baseFile,"<Defines><Define name='FAST_IDLE_SCAN_DELAY' value='0.2s'/><Define name='MEDIUM_IDLE_SCAN_DELAY' value='0.6s'/></Defines>"+own.Replace("id='Walker'","id='BaseWalker'"));Put(entry,"<Includes><Include type='instance' source='base.xml'/></Includes><Defines><Define name='FAST_IDLE_SCAN_DELAY' value='9s'/><Define name='MEDIUM_IDLE_SCAN_DELAY' value='8s'/></Defines><GameObject id='DerivedWalker' inheritFrom='BaseWalker'/>");CheckDelays(ConscriptSourcePreparation.Load(entry).Game,"0.2s","0.6s");
        Put(entry,"<Defines><Define name='FAST_IDLE_SCAN_DELAY' value='1s'/><Define name='MEDIUM_IDLE_SCAN_DELAY' value='2s'/></Defines>"+own);CheckDelays(ConscriptSourcePreparation.Load(entry).Game,"1s","2s");
        // Reborn: recursive relative XIncludes and child::* selections feed the same owner expression pass.
        string nested=Path.Combine(output,"nested");Directory.CreateDirectory(nested);File.WriteAllText(Path.Combine(nested,"rows.xml"),"<Rows xmlns='"+ea+"'><WeaponSlotHardpoint ID='1'><Weapon Template='=$WEAPON'/></WeaponSlotHardpoint></Rows>");File.WriteAllText(Path.Combine(nested,"outer.xml"),"<WeaponSetUpdate xmlns='"+ea+"' xmlns:xi='http://www.w3.org/2001/XInclude' id='Nested'><xi:include href='rows.xml' xpointer='xmlns(a=uri:ea.com:eala:asset) xpointer(/a:Rows/child::*)'/></WeaponSetUpdate>");Put(entry,"<Defines><Define name='WEAPON' value='SovietAntiInfantryInfantryAK47'/></Defines><GameObject id='NestedWalker'><Behaviors><xi:include href='nested/outer.xml'/></Behaviors><Body><ActiveBody id='Body' MaxHealth='100'/></Body></GameObject>");var nestedInput=ConscriptSourcePreparation.Load(entry);Require(nestedInput.Game.GetElementsByTagName("Weapon",ea).OfType<XmlElement>().Single().GetAttribute("Template")=="SovietAntiInfantryInfantryAK47"&&nestedInput.Sources.Any(s=>s.Path==Path.Combine(nested,"rows.xml")),"Nested fragment context differs.");
        // Reborn: missing definitions identify the original fragment field, not a generated XML line or a partially substituted result.
        Put(entry,"<Includes><Include type='all' source='defs.xml'/></Includes>"+own);string broken=original.Replace("=$FAST_IDLE_SCAN_DELAY","=$REBORN_MISSING");File.WriteAllText(fragment,broken);bool rejected=false;
        try{ConscriptSourcePreparation.Load(entry);}catch(InvalidDataException error){var location=SdkXmlDiagnostics.Origin(error);Require(error.Message.Contains("REBORN_MISSING",StringComparison.Ordinal)&&location?.File==fragment&&location.Line>0&&!location.Context,"Missing fragment definition lost its physical field origin: "+SdkXmlDiagnostics.Format(error));rejected=true;}
        Require(rejected&&File.ReadAllText(fragment)==broken,"Missing definition was accepted or source bytes changed.");
        Console.WriteLine("Reborn: XInclude definitions: PASS (original four-turret Heavy Walker/0.0s/0.4s, independent owner and inherited-base contexts, nested relative child-XPointer selection, raw witnesses, missing-definition file/line, no user mod output).");

        // Reborn: all test sources live under this new private output directory.
        static void Put(string path,string body)=>File.WriteAllText(path,"<AssetDeclaration xmlns='uri:ea.com:eala:asset' xmlns:xi='http://www.w3.org/2001/XInclude'>"+body+"</AssetDeclaration>");
    }
    // Reborn: assert every original turret resolves its proper fast/medium literal, including both repeated medium entries.
    private static void CheckDelays(XmlDocument xml,string fast,string medium){var values=xml.GetElementsByTagName("TurretAITargetChooserData","uri:ea.com:eala:asset").OfType<XmlElement>().Select(e=>e.GetAttribute("IdleScanDelay")).ToArray();Require(values.SequenceEqual(new[]{fast,fast,medium,medium}),"Original Heavy Walker fragment delays differ: "+string.Join(",",values));}
    // Reborn: make context or diagnostic regressions fail the private test command.
    private static void Require(bool condition,string message){if(!condition)throw new InvalidDataException(message);}
}
