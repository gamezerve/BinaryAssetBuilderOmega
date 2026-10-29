using System.Text;
using BinaryAssetBuilder.Core.SageXml;
namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: missing ART all/instance includes warn and skip; real source failures retain their normal compiler diagnostics.
internal static class MissingArtIncludeSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise lookup, inheritance, routing, warning coordinates and publication evidence inside a fresh private SDK fixture. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string output)
    {
        output=SdkEnvironmentPreflight.Absolute(output);if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh missing-ART test directory required.");
        string sdk=Path.Combine(output,"sdk"),data=Path.Combine(output,"mod/Data"),entry=Path.Combine(data,"Static_Mod.xml");Directory.CreateDirectory(data);Directory.CreateDirectory(sdk);
        var search=SdkSourceSearch.ForMod(entry,sdk);foreach(string root in search.Roots)Directory.CreateDirectory(root);
        const string ea="uri:ea.com:eala:asset";int checks=0;
        string Asset(string id)=>"<UpgradeTemplate id='"+id+"' Type='OBJECT'/>";
        string Declaration(string includes,string body)=>"<!-- Reborn: private missing-art fixture. --><AssetDeclaration xmlns='"+ea+"'><Includes>"+includes+"</Includes>"+body+"</AssetDeclaration>";
        string Include(string source,string type="all")=>"<Include type='"+type+"' source='"+source+"'/>";
        void Put(string path,string text){Directory.CreateDirectory(Path.GetDirectoryName(path)!);File.WriteAllText(path,text,new UTF8Encoding(false));}
        void Refuse(Action action,string label){bool failed=false;try{action();}catch(Exception e)when(e is InvalidDataException or IOException or System.Xml.XmlException){failed=true;}if(!failed)throw new InvalidDataException("Accepted "+label);checks++;}
        Put(Path.Combine(data,"asset.xml"),Declaration("",Asset("RebornMissingArtOwner")));
        Put(entry,"<AssetDeclaration xmlns='"+ea+"'>\n<Includes>\n"+Include("ART:Missing.w3x")+"\n"+Include("art:Missing.w3x","instance")+"\n"+Include("DATA:asset.xml")+"\n</Includes></AssetDeclaration>");
        if(!ConscriptSourceInput.Detect(entry,sdk))throw new InvalidDataException("Missing ART blocked backend routing.");checks++;
        var input=ConscriptSourcePreparation.Load(entry,search);
        if(input.Warnings.Length!=2||input.Warnings[0].Location?.Line!=3||input.Warnings[1].Location?.Line!=4||input.Game.DocumentElement!.GetAttribute("id")!="RebornMissingArtOwner"||input.AdditionalAssets.Length!=0||input.Searches.Count(s=>s.ExpectedMissing)!=2)throw new InvalidDataException("Missing ART source graph/warning coordinates differ.");checks++;
        SdkSourceSearch.Verify(input.Searches);checks++;
        var roundTrip=System.Text.Json.JsonSerializer.Deserialize<SdkSourceSearch.Selection[]>(System.Text.Json.JsonSerializer.Serialize(input.Searches))!;if(roundTrip.Count(s=>s.ExpectedMissing)!=2)throw new InvalidDataException("Missing ART receipt lost absence evidence.");SdkSourceSearch.Verify(roundTrip);checks++;
        var prepared=Path.Combine(output,"prepared");ConscriptSourceInput.Prepare(entry,prepared,sdk);checks++;
        var environment=new SdkEnvironmentPreflight.Report("ra3ep1",sdk,entry,0,"",data,entry,Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(entry))),Path.Combine(output,"unused"),Array.Empty<SdkEnvironmentPreflight.Mapping>(),true,true,false,false,false,"0x5454A8E9",Array.Empty<string>());
        var paths=SdkSourcePathAudit.Inspect(environment,search:search,w3xSources:true,skipMissingArtIncludes:true);
        var report=SdkMissingDependencyAudit.Analyze(entry,paths,ConscriptSourceInput.Schemas(),Array.Empty<SdkMissingDependencyAudit.Asset>());
        if(!report.SourceGraphComplete||report.Findings.Count(f=>f.Code=="MISSINGART"&&f.Severity=="Warning")!=2||report.Findings.Any(f=>f.Severity=="Error"))throw new InvalidDataException("Dependency preflight treated missing ART as an error.");checks++;
        Put(Path.Combine(data,"base.xml"),Declaration("","<UpgradeTemplate id='BaseUpgrade' Type='OBJECT' BuildCost='17'/>"));
        Put(Path.Combine(data,"asset.xml"),Declaration(Include("ART:InheritedMissing.w3x")+Include("DATA:base.xml","instance"),"<UpgradeTemplate id='RebornMissingArtOwner' inheritFrom='BaseUpgrade'/>"));
        var inherited=ConscriptSourcePreparation.Load(entry,search);if(inherited.Game.DocumentElement!.GetAttribute("BuildCost")!="17"||inherited.Warnings.Length!=3||inherited.AdditionalAssets.Length!=0)throw new InvalidDataException("Skipped ART disrupted instance inheritance.");checks++;
        // Reborn: imported literal definitions must survive skipped ART all/instance includes inside an inherited source closure.
        Put(Path.Combine(data,"prices.xml"),"<AssetDeclaration xmlns='"+ea+"'><Defines><Define name='PRICE' value='23'/></Defines></AssetDeclaration>");
        string defineBase=Path.Combine(data,"define-base.xml");
        Put(defineBase,Declaration(Include("ART:MissingDefineAll.w3x")+Include("ART:MissingDefineInstance.w3x","instance")+Include("DATA:prices.xml"),"<UpgradeTemplate id='DefineBase' Type='OBJECT' BuildCost='=$PRICE'/>"));
        Put(Path.Combine(data,"asset.xml"),Declaration(Include("DATA:define-base.xml","instance"),"<UpgradeTemplate id='RebornMissingArtOwner' inheritFrom='DefineBase' BuildCost='=$PRICE'/>"));
        var defined=ConscriptSourcePreparation.Load(entry,search);
        if(defined.Game.DocumentElement!.GetAttribute("BuildCost")!="23"||defined.Warnings.Length!=4||defined.AdditionalAssets.Length!=0||defined.Searches.Count(s=>s.ExpectedMissing)!=4)throw new InvalidDataException("Skipped nested ART blocked imported definition resolution.");checks++;
        ConscriptSourceInput.Prepare(entry,Path.Combine(output,"defined-prepared"),sdk);checks++;
        SdkSourceSearch.Verify(defined.Searches);checks++;
        Put(defineBase,Declaration(Include("ART:MissingDefineAll.w3x")+Include("DATA:prices.xml"),"<UpgradeTemplate id='DefineBase' Type='OBJECT' BuildCost='=$UNKNOWN'/>"));
        Refuse(()=>ConscriptSourcePreparation.Load(entry,search),"undefined definition behind skipped ART");
        Put(Path.Combine(data,"asset.xml"),Declaration("",Asset("RebornMissingArtOwner")));
        File.WriteAllText(Path.Combine(search.ModArt,"Missing.w3x"),Declaration("",Asset("AppearedLate")));Refuse(()=>SdkSourceSearch.Verify(input.Searches),"late ART source appearance");File.Delete(Path.Combine(search.ModArt,"Missing.w3x"));
        string existing=Path.Combine(search.SdkArt,"Ex/Existing.xml");Put(existing,Declaration("",Asset("ExistingArtOwner")));Put(entry,Declaration(Include("ART:Existing.xml"),Asset("LocalOwner")));
        var found=ConscriptSourcePreparation.Load(entry,search);if(found.Warnings.Length!=0||new[]{found.Game}.Concat(found.AdditionalAssets).Count()!=2)throw new InvalidDataException("Existing ART source was skipped.");checks++;
        // Reborn: an available ART instance still supplies inherited fields without emitting its base asset.
        Put(entry,Declaration(Include("ART:Existing.xml","instance"),"<UpgradeTemplate id='LocalOwner' inheritFrom='ExistingArtOwner'/>"));
        var artBase=ConscriptSourcePreparation.Load(entry,search);if(artBase.Warnings.Length!=0||artBase.Game.DocumentElement!.GetAttribute("Type")!="OBJECT"||artBase.AdditionalAssets.Length!=0)throw new InvalidDataException("Existing ART instance was not used for inheritance.");checks++;
        // Reborn: skipping an absent instance cannot manufacture a required inherited definition.
        Put(entry,Declaration(Include("ART:MissingBase.xml","instance"),"<UpgradeTemplate id='LocalOwner' inheritFrom='NoSuchBase'/>"));
        bool missingBase=false;try{_=ConscriptSourcePreparation.Load(entry,search);}catch(InvalidDataException error)when(error.Message.Contains("NoSuchBase",StringComparison.Ordinal)){missingBase=true;}
        if(!missingBase)throw new InvalidDataException("Required missing instance base was not reported by name.");checks++;
        Put(entry,Declaration(Include("ART:Existing.xml","instance"),Asset("LocalOwner")));
        Put(existing,"<broken");Refuse(()=>ConscriptSourcePreparation.Load(entry,search),"malformed existing ART");
        foreach(var bad in new[]{("DATA:Missing.xml","instance"),("DATA:Missing.xml","all"),("ART:Missing.w3x","reference"),("ART:../../escape.xml","all")}){Put(entry,Declaration(Include(bad.Item1,bad.Item2),Asset("LocalOwner")));Refuse(()=>ConscriptSourcePreparation.Load(entry,search),bad.ToString());}
        Put(entry,Declaration("","<Texture id='MissingTexture' File='ART:Missing.dds'/>"));Refuse(()=>ConscriptSourcePreparation.Load(entry,search),"missing Texture File resource");
        Put(Path.Combine(search.ModArt,"Directory.w3x/placeholder.txt"),"Reborn: directory is not an absent file.");Put(entry,Declaration(Include("ART:Directory.w3x"),Asset("LocalOwner")));Refuse(()=>ConscriptSourcePreparation.Load(entry,search),"ART directory target");
        Put(entry,Declaration(string.Concat(Enumerable.Range(0,130).Select(i=>Include("ART:Absent"+i+".w3x",i%2==0?"all":"instance"))),Asset("ManyMissingArt")));
        var many=ConscriptSourcePreparation.Load(entry,search);if(many.Warnings.Length!=130)throw new InvalidDataException("Missing ART warnings tripped fatal/visibility limits.");checks++;
        // Reborn: restore a valid private entry and prepare an untouched source generation for the native/package smoke test.
        Put(entry,"<AssetDeclaration xmlns='"+ea+"'>\n<Includes>\n"+Include("ART:Missing.w3x")+"\n"+Include("art:Missing.w3x","instance")+"\n"+Include("DATA:asset.xml")+"\n</Includes></AssetDeclaration>");
        ConscriptSourceInput.Prepare(entry,Path.Combine(output,"build-source"),sdk);
        Console.WriteLine($"Reborn: missing ART all/instance includes: PASS ({checks} routing/source/preparation/audit/receipt/absence/existing-file/error controls; no user source or game changes). Output: {output}");
    }
}
