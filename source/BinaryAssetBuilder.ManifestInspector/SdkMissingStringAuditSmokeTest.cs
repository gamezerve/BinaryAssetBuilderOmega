using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: localization fixtures are isolated from both editable mods and installed game files.
internal static class SdkMissingStringAuditSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify CSF/STR parsing and effective XML key lookup together without gameplay or a native compiler matrix. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string output)
    {
        output=SdkEnvironmentPreflight.Absolute(output);if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh localization test directory required.");Directory.CreateDirectory(output);
        var csf=BuildCsf();var stock=SdkStringCatalog.Csf(csf);Require(stock.SetEquals(new[]{"Name:Stock","Desc:Stock"}),"CSF key/value counts or STRW handling differ.");
        const string str="// Fake:Comment\n/* Name:FakeBlock */\nType:ModOnly\n\"quoted END and // comments, \\\"escaped\\\"\nmultiline\"\nEnd\nName:Empty\n\"\"\nEND\n";
        var mod=SdkStringCatalog.Str(Encoding.UTF8.GetBytes(str));Require(mod.SetEquals(new[]{"Type:ModOnly","Name:Empty"}),"STR comments/quoted multiline/empty translation differ.");
        var unicode=Encoding.Unicode.GetPreamble().Concat(Encoding.Unicode.GetBytes(str)).ToArray();Require(SdkStringCatalog.Str(unicode).SetEquals(mod),"UTF-16 STR keys differ.");
        foreach(byte[] invalid in new[]{csf[..^1],new byte[24],csf.Concat(new byte[]{1}).ToArray()}){bool rejected=false;try{SdkStringCatalog.Csf(invalid);}catch(InvalidDataException){rejected=true;}Require(rejected,"Invalid CSF accepted.");}
        foreach(string invalid in new[]{"Name:Incomplete\n\"value\"","Name:Bad\nEND","Name:Broken\n\"unclosed"}){bool rejected=false;try{SdkStringCatalog.Str(Encoding.UTF8.GetBytes(invalid));}catch(InvalidDataException){rejected=true;}Require(rejected,"Invalid STR accepted.");}
        string data=Path.Combine(output,"Data");Directory.CreateDirectory(data);string entry=Path.Combine(data,"mod.xml"),basis=Path.Combine(data,"Base.xml");
        var baseXml=new XmlDocument();baseXml.Load(Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"live-alpha/ConscriptBuildTime1/data/mod.xml"));baseXml.DocumentElement!.SetAttribute("id","StringBase");baseXml.SelectSingleNode("//*[local-name()='DisplayName']")!.InnerText="Name:UnusedBase";baseXml.DocumentElement.SetAttribute("Description","Desc:UnusedBase");baseXml.DocumentElement.SetAttribute("TypeDescription","Type:UnusedBase");
        File.WriteAllText(basis,"<!-- Reborn: overridden inherited keys must not be reported as compiled uses. --><AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+baseXml.DocumentElement.OuterXml+"</AssetDeclaration>");
        File.WriteAllText(entry,"""
        <!-- Reborn: stock and mod keys, missing attribute keys, actual definitions and instance inheritance in one frontend graph. -->
        <AssetDeclaration xmlns="uri:ea.com:eala:asset" xmlns:xai="uri:ea.com:eala:asset:instance">
          <Includes><Include type="instance" source="Base.xml"/></Includes>
          <Defines><Define name="MISSING_TEXT" value="Desc:Missing"/></Defines>
          <GameObject id="StringUnit" inheritFrom="StringBase"
            Description="=$MISSING_TEXT" TypeDescription="Type:ModOnly"
            TypeDescriptionTransformed="Name:Missing">
            <DisplayName xai:joinAction="Replace">Name:Stock</DisplayName>
            <DisplayNameTransformed>Name:Empty</DisplayNameTransformed>
          </GameObject>
        </AssetDeclaration>
        """);
        var sources=new[]{entry,basis}.Select(p=>{byte[] b=File.ReadAllBytes(p);return new SdkSourcePathAudit.Source(p,data,Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(b)),b.Length);}).ToArray();
        var paths=new SdkSourcePathAudit.Report(data,null,null,sources,new[]{new SdkSourcePathAudit.Edge(entry,"instance","Base.xml",basis)},Array.Empty<SdkSourcePathAudit.Resource>(),Array.Empty<SdkSourcePathAudit.Issue>(),true,false,true,false,false,Array.Empty<string>());
        var result=SdkMissingStringAudit.Analyze(entry,paths,ConscriptSourceInput.Schemas(),stock,mod);
        // Reborn: retain synthetic diagnostic evidence even when an assertion fails, without exposing original game translations.
        File.WriteAllText(Path.Combine(output,"report.json"),System.Text.Json.JsonSerializer.Serialize(result,new System.Text.Json.JsonSerializerOptions{WriteIndented=true}));
        Require(result.CatalogComplete&&result.PreparedSourceUsed&&result.Findings.Length==2&&result.Findings.All(f=>f.Code=="MISSINGSTRING"&&f.Severity=="Warning"&&f.Source==entry&&f.Line>0&&f.Column>0)&&result.Findings.Select(f=>f.Target).ToHashSet().SetEquals(new[]{"Desc:Missing","Name:Missing"}),"Effective XML localization lookup or source coordinates differ.");
        // Reborn: missing configuration or a malformed mod table must report incomplete coverage rather than a false clean audit or a cascade of missing labels.
        var unavailable=SdkMissingStringAudit.Run(entry,output,paths,ConscriptSourceInput.Schemas());Require(!unavailable.CatalogComplete&&unavailable.Findings.Single().Code=="STRINGCATALOG","Unconfigured catalog was silently accepted.");
        string stockPath=Path.Combine(output,"stock.csf"),strDirectory=Path.Combine(output,"Misc/Data");Directory.CreateDirectory(strDirectory);File.WriteAllBytes(stockPath,csf);File.WriteAllText(Path.Combine(output,"uprising-alpha.paths"),"# Reborn: synthetic local catalog only.\nStockStrings="+stockPath);
        File.WriteAllText(Path.Combine(strDirectory,"mod.str"),"Name:Broken\n\"unterminated");var malformed=SdkMissingStringAudit.Run(entry,output,paths,ConscriptSourceInput.Schemas());Require(!malformed.CatalogComplete&&malformed.Findings.Single().Code=="STRINGCATALOG","Malformed mod table caused false missing-key claims.");
        File.WriteAllText(Path.Combine(output,"report.json"),System.Text.Json.JsonSerializer.Serialize(result,new System.Text.Json.JsonSerializerOptions{WriteIndented=true}));
        Console.WriteLine("Reborn: missing string audit: PASS (CSF STR/STRW/zero-value records, STR comments/escaped multiline/empty/UTF-16, malformed data, stock+mod union, definitions, effective inherited overrides, warning source positions and incomplete-catalog controls; no game).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: synthetic CSF records exercise both value tags without distributing any original game text. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] BuildCsf()
    {
        using var stream=new MemoryStream();using var writer=new BinaryWriter(stream,Encoding.ASCII,true);Tag(" FSC");writer.Write(3u);writer.Write(3u);writer.Write(2u);writer.Write(0u);writer.Write(0u);
        Label("Name:Stock",true,false);Label("Desc:Stock",true,true);Label("Name:Untranslated",false,false);writer.Flush();return stream.ToArray();
        // Reborn: label/value lengths count bytes and UTF-16 code units respectively.
        void Label(string key,bool value,bool extra){Tag(" LBL");writer.Write(value?1u:0u);writer.Write((uint)key.Length);writer.Write(Encoding.ASCII.GetBytes(key));if(!value)return;Tag(extra?"WRTS":" RTS");writer.Write(1u);writer.Write((ushort)(0xffff^(ushort)'x'));if(extra){writer.Write(3u);writer.Write(Encoding.ASCII.GetBytes("abc"));}}
        // Reborn: preserve literal on-disk tag spelling in independent parser fixtures.
        void Tag(string text)=>writer.Write(Encoding.ASCII.GetBytes(text));
    }

    // Reborn: every parser and source-coverage mismatch fails the test process explicitly.
    private static void Require(bool condition,string message){if(!condition)throw new InvalidDataException(message);}
}
