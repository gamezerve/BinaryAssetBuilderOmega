using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: grouped disposable fixtures prove ordered alias selection and real source preparation without changing user SDK/mod content.
internal static class SdkSourceSearchSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify DATA/ART/AUDIO precedence, stock defining context, late shadows and actual inherited GameObject definition edits together. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string output,string sourceEntry)
    {
        output=SdkEnvironmentPreflight.Absolute(output);SdkEnvironmentPreflight.CheckPath(output);
        if(File.Exists(output) || Directory.Exists(output))throw new InvalidDataException("Fresh search test directory required.");
        var original=ConscriptSourceInput.Load(sourceEntry);
        string sdk=Path.Combine(output,"sdk"),entry=Path.Combine(sdk,"mods","Fixture","Data","mod.xml");
        Directory.CreateDirectory(Path.GetDirectoryName(entry)!);
        foreach(string directory in new[]{"SageXML","Art","Audio"})Directory.CreateDirectory(Path.Combine(sdk,directory));
        var search=SdkSourceSearch.ForMod(entry,sdk);const string ea="uri:ea.com:eala:asset";
        string stockDefine=Path.Combine(search.SdkData,"GlobalData","GlobalDefines.xml"),modDefine=Path.Combine(search.ModData,"GlobalData","GlobalDefines.xml");
        string definitions="<!-- Reborn: fixture definitions. --><AssetDeclaration xmlns='"+ea+"'><Defines><Define name='BUILD_TIME' value='2'/></Defines></AssetDeclaration>";
        Put(stockDefine,definitions);
        var baseXml=new XmlDocument{PreserveWhitespace=true,XmlResolver=null};baseXml.LoadXml(original.Game.DocumentElement!.OuterXml);baseXml.DocumentElement!.SetAttribute("id","BaseConscriptTemplate");
        Put(Path.Combine(search.SdkData,"Base.xml"),"<!-- Reborn: actual complete source base. --><AssetDeclaration xmlns='"+ea+"'>"+baseXml.DocumentElement.OuterXml+"</AssetDeclaration>");
        Put(entry,"<!-- Reborn: stock fallback is source input, not stock binary substitution. --><AssetDeclaration xmlns='"+ea+"'><Includes><Include type='all' source='DATA:GlobalData/GlobalDefines.xml'/><Include type='instance' source='DATA:Base.xml'/></Includes><GameObject id='RebornSearchConscript' inheritFrom='BaseConscriptTemplate' BuildTime='=$BUILD_TIME'/></AssetDeclaration>");
        var fallback=ConscriptSourcePreparation.Load(entry,search);
        if(fallback.Game.DocumentElement!.GetAttribute("BuildTime")!="2" || fallback.Sources.Length!=3 || fallback.Searches.Any(s=>s.Layer!="sdk") || !ConscriptSourceInput.Detect(entry,sdk))throw new InvalidDataException("SDK fallback preparation differs.");
        var captured=search.Select(entry,"DATA:GlobalData/GlobalDefines.xml",search.ModData);
        Put(modDefine,definitions.Replace("value='2'","value='3'"));Refuse(()=>SdkSourceSearch.Verify(new[]{captured}),"late mod shadow");
        var edited=ConscriptSourcePreparation.Load(entry,search);
        if(edited.Game.DocumentElement!.GetAttribute("BuildTime")!="3" || !edited.Searches.Any(s=>s.PhysicalPath==modDefine && s.Layer=="mod"))throw new InvalidDataException("Mod definition did not override stock.");
        ConscriptSourceInput.Prepare(entry,Path.Combine(output,"prepared"),sdk);
        Put(modDefine,"<broken");Refuse(()=>ConscriptSourcePreparation.Load(entry,search),"malformed mod must not fall back");Put(modDefine,definitions);
        string art="SUAntiInfantryInfantry_SKN.w3x",stockArt=Path.Combine(search.SdkArt,"SU",art),modArt=Path.Combine(search.ModArt,art),modFanout=Path.Combine(search.ModArt,"SU",art);
        Put(stockArt,"Reborn: resource fixture, not compiled W3X.");Expect("ART:"+art,stockArt);
        Put(modFanout,"Reborn: mod fanout fixture.");Expect("ART:"+art,modFanout);
        Put(modArt,"Reborn: mod direct fixture.");Expect("ART:"+art,modArt);Expect("ART:SU/"+art,modFanout);
        // Reborn: a found W3X must produce the processor-limit diagnostic, not a fake successful compilation or an AssetDeclaration parse error.
        string resourceEntry=Path.Combine(search.ModData,"resource.xml");Put(resourceEntry,"<!-- Reborn: resource lookup is separate from compilation. --><AssetDeclaration xmlns='"+ea+"'><Includes><Include type='all' source='ART:"+art+"'/></Includes></AssetDeclaration>");
        bool resourceRefused=false;try{ConscriptSourcePreparation.Load(resourceEntry,search);}catch(InvalidDataException error)when(error.Message.Contains("processors are not connected",StringComparison.Ordinal)){resourceRefused=true;}if(!resourceRefused)throw new InvalidDataException("Resolved W3X was not refused by its processor gate.");
        string stockAudio=Path.Combine(search.SdkAudio,"unit.wav"),modAudio=Path.Combine(search.ModAudio,"unit.wav");
        Put(stockAudio,"Reborn: stock audio fixture.");Expect("AUDIO:unit.wav",stockAudio);Put(modAudio,"Reborn: mod audio fixture.");Expect("AUDIO:unit.wav",modAudio);
        Put(Path.Combine(search.SdkData,"GlobalData","relative.xml"),"Reborn: relative context fixture.");
        var relative=search.Select(stockDefine,"relative.xml",search.SdkData);if(relative.Root!=search.SdkData || relative.Layer!="relative")throw new InvalidDataException("Stock relative context escaped.");
        Refuse(()=>search.Select(entry,"DATA:../../escape.xml",search.ModData),"traversal");Refuse(()=>search.Select(entry,"UNKNOWN:file.xml",search.ModData),"unknown alias");
        Refuse(()=>search.Select(entry,"DATA:missing.xml",search.ModData),"missing both roots");
        Directory.CreateDirectory(Path.Combine(search.ModData,"directory.xml"));Put(Path.Combine(search.SdkData,"directory.xml"),definitions);
        Refuse(()=>search.Select(entry,"DATA:directory.xml",search.ModData),"directory override");
        Console.WriteLine("SDK source search: PASS (DATA SDK fallback/mod override; real inherited BuildTime 2->3; no base emission; broken override refusal; ART direct/fanout/stock; AUDIO override/fallback; stock-relative context; late shadow/directory/traversal/missing/unknown refusals; no game or resource compilation).");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: create fixture files only below a fresh caller-owned output tree; never copy or mutate installed stock resources. */
        //-------------------------------------------------------------------------------------------------
        void Put(string path,string text){if(!SdkEnvironmentPreflight.Inside(output,path))throw new InvalidDataException("Fixture path escaped.");Directory.CreateDirectory(Path.GetDirectoryName(path)!);File.WriteAllText(path,text,new UTF8Encoding(false));}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: assert exact physical selection, not merely a success code or file existence. */
        //-------------------------------------------------------------------------------------------------
        void Expect(string logical,string expected){if(search.Select(entry,logical,search.ModData).PhysicalPath!=expected)throw new InvalidDataException("Wrong search winner for "+logical);}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: only known source/path refusals count as negative evidence; unrelated runtime failures remain test failures. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action,string label){try{action();}catch(Exception error)when(error is InvalidDataException or IOException or XmlException or NotSupportedException){return;}throw new InvalidDataException("Expected search refusal: "+label);}
}
