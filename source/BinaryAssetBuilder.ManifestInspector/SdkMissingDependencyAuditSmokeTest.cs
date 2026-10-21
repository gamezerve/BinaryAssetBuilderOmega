using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove aggregate missing dependencies, mod/stock union and instance-only exclusion independently of gameplay/native compilation.
internal static class SdkMissingDependencyAuditSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: use a fresh diagnostic directory only, leaving both live SDK mod directories and their packages untouched. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string output)
    {
        output=SdkEnvironmentPreflight.Absolute(output);SdkEnvironmentPreflight.CheckPath(output);
        if(File.Exists(output)||Directory.Exists(output))throw new InvalidDataException("Fresh dependency test directory required.");
        SdkEnvironmentPreflight.DirectoryPath(Path.GetDirectoryName(output)!);Directory.CreateDirectory(output);
        var sources=new List<SdkSourcePathAudit.Source>();var edges=new List<SdkSourcePathAudit.Edge>();
        var schemas=new XmlSchemaSet{XmlResolver=null};
        const string xsd="""
        <xs:schema xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns="uri:ea.com:eala:asset"
            xmlns:xas="uri:ea.com:eala:asset:schema" targetNamespace="uri:ea.com:eala:asset" elementFormDefault="qualified">
          <!-- Reborn: minimal independent schema distinguishes actual strong reference fields from ordinary strings. -->
          <xs:simpleType name="AssetReference"><xs:restriction base="xs:string"/></xs:simpleType>
          <xs:simpleType name="WeakReference"><xs:restriction base="xs:string"/></xs:simpleType>
          <xs:simpleType name="ThingRef" xas:refType="Thing"><xs:restriction base="AssetReference"/></xs:simpleType>
          <!-- Reborn: upgrade trigger IDs are typed weak asset references, unlike unconstrained dynamic-name strings. -->
          <xs:simpleType name="UpgradeTemplateWeakRef" xas:refType="UpgradeTemplate"><xs:restriction base="WeakReference"/></xs:simpleType>
          <xs:complexType name="Base"><xs:attribute name="id" type="xs:string"/></xs:complexType>
          <xs:complexType name="Thing"><xs:complexContent><xs:extension base="Base"/></xs:complexContent></xs:complexType>
          <xs:complexType name="SpecificThing"><xs:complexContent><xs:extension base="Thing"/></xs:complexContent></xs:complexType>
          <xs:complexType name="OtherSpecificThing"><xs:complexContent><xs:extension base="Thing"/></xs:complexContent></xs:complexType>
          <xs:complexType name="OtherThing"><xs:complexContent><xs:extension base="Base"/></xs:complexContent></xs:complexType>
          <xs:complexType name="UpgradeTemplate"><xs:complexContent><xs:extension base="Base"/></xs:complexContent></xs:complexType>
          <xs:complexType name="User"><xs:complexContent><xs:extension base="Base">
            <xs:attribute name="Target" type="ThingRef"/><xs:attribute name="Number" type="xs:string"/>
            <xs:attribute name="Note" type="xs:string"/>
            <xs:attribute name="Weak" type="WeakReference"/>
            <xs:attribute name="Upgrade" type="UpgradeTemplateWeakRef"/>
          </xs:extension></xs:complexContent></xs:complexType>
          <xs:element name="Thing" type="Thing"/><xs:element name="SpecificThing" type="SpecificThing"/><xs:element name="User" type="User"/>
          <xs:element name="AssetDeclaration"><xs:complexType><xs:sequence>
            <xs:any minOccurs="0" maxOccurs="unbounded" processContents="lax"/>
          </xs:sequence></xs:complexType></xs:element>
        </xs:schema>
        """;
        using(var reader=XmlReader.Create(new StringReader(xsd),new XmlReaderSettings{DtdProcessing=DtdProcessing.Prohibit,XmlResolver=null}))schemas.Add(null,reader);
        schemas.Compile();
        string targets=Store("Targets.xml","<Thing id='NewLocal'/><SpecificThing id='LocalDerived'/>");
        string witness=Store("Instance.xml","<Defines><Define name='INSTANCE_DEF' value='2'/></Defines><Thing id='InstanceOnly'/>");
        string definitions=Store("Defs.xml","""
        <Defines>
          <Define name="CHAIN" value="=$MISSING_DEF"/>
          <Define name="UNUSED" value="=$ANOTHER_MISSING"/>
          <Define name="TARGET_ID" value="NewLocal"/>
          <Define name="QUOTED" value="='$LiteralDollarNotAReference'"/>
          <Define name="FIRST" value="=$LATER"/>
          <Define name="LATER" value="2"/>
        </Defines>
        """);
        string child=Store("Child.xml","<Includes><Include type='all' source='Defs.xml'/></Includes><User id='Child' Number='=$PARENT_ONLY'/>");
        string entry=Store("mod.xml","""
        <Includes><Include type="all" source="Targets.xml"/><Include type="instance" source="Instance.xml"/>
          <Include type="all" source="Defs.xml"/><Include type="all" source="Child.xml"/></Includes>
        <Defines><Define name="PARENT_ONLY" value="1"/></Defines>
        <User id="Local" Target="NewLocal" Note="ThisIsNotAnAssetReference" Number="=$INSTANCE_DEF" Weak="DynamicNameNotProved"/>
        <User id="Stock" Target="Thing:Stock"/>
        <User id="Expression" Target="=$TARGET_ID"/>
        <User id="Derived" Target="LocalDerived"/>
        <User id="Missing1" Target="MissingA"/><User id="Missing2" Target="MissingB"/>
        <User id="Instance" Target="InstanceOnly"/><User id="WrongType" Target="WrongType"/>
        <User id="Consumer" Number="=$CHAIN"/>
        """);
        edges.Add(new(entry,"all","Targets.xml",targets));edges.Add(new(entry,"instance","Instance.xml",witness));
        edges.Add(new(entry,"all","Defs.xml",definitions));edges.Add(new(entry,"all","Child.xml",child));edges.Add(new(child,"all","Defs.xml",definitions));
        var paths=new SdkSourcePathAudit.Report(output,null,null,sources.ToArray(),edges.ToArray(),Array.Empty<SdkSourcePathAudit.Resource>(),Array.Empty<SdkSourcePathAudit.Issue>(),true,false,true,false,false,Array.Empty<string>());
        var stock=new[]{SdkMissingDependencyAudit.Identity("Thing","Stock"),SdkMissingDependencyAudit.Identity("Thing","Stock"),SdkMissingDependencyAudit.Identity("Thing","NewLocal"),SdkMissingDependencyAudit.Identity("OtherThing","WrongType")};
        var report=SdkMissingDependencyAudit.Analyze(entry,paths,schemas,stock);
        Require(report.Findings.Count(f=>f.Code=="MISSINGASSET")==4,"Missing targets, instance-only exclusion or type checking differs.");
        Require(report.Findings.Count(f=>f.Code=="MISSINGDEF")==3,"Missing definition aggregation or defining-document scope differs.");
        Require(report.Findings.Any(f=>f.Code=="MISSINGDEF"&&f.Owner=="Define:CHAIN"&&f.Target=="$MISSING_DEF"),"Definition-to-definition use site absent.");
        Require(report.Findings.Any(f=>f.Code=="MISSINGDEF"&&f.Owner=="Define:UNUSED"),"Unused broken definition was silently skipped.");
        Require(report.Findings.Count(f=>f.Code=="FORWARDDEF")==1&&report.Findings.Any(f=>f.Code=="BLOCKEDDEF"&&f.Owner=="User:Consumer"),"Forward and transitive blocked definitions were not distinguished.");
        Require(!report.Findings.Any(f=>f.Code=="AMBIGUOUSDEF"||f.Target.Contains("LiteralDollar")||f.Target.Contains("ThisIsNot")),"Diamond imports, quoted literals or ordinary strings produced false positives.");
        Require(report.Findings.All(f=>f.Line>0&&f.Column>0)&&report.StockAssets==3&&report.CheckedAssetReferences==8&&report.UncheckedAssetReferences==1&&!report.FullDependencyCoverage,"Coordinates, stock deduplication, weak deferral or typed-reference scope differs.");
        // Reborn: the very same missing runtime identity becomes valid when it is explicitly present in a stock stream.
        var withStock=SdkMissingDependencyAudit.Analyze(entry,paths,schemas,stock.Append(SdkMissingDependencyAudit.Identity("Thing","InstanceOnly")));
        Require(withStock.Findings.Count(f=>f.Code=="MISSINGASSET")==3,"Stock targets did not satisfy a legitimate runtime reference.");
        // Reborn: exact stock types beat stock-derived alternatives, while a mod-local derived match beats external stock alternatives.
        var priority=SdkMissingDependencyAudit.Analyze(entry,paths,schemas,stock.Append(SdkMissingDependencyAudit.Identity("SpecificThing","Stock")).Append(SdkMissingDependencyAudit.Identity("OtherSpecificThing","LocalDerived")));
        Require(!priority.Findings.Any(f=>f.Code=="AMBIGUOUSASSET")&&priority.Findings.Count(f=>f.Code=="MISSINGASSET")==4,"Core source/exact reference precedence differs.");
        // Reborn: changing an audited file invalidates the snapshot instead of reusing stale availability evidence.
        File.AppendAllText(targets,"\n");bool staleRejected=false;try{SdkMissingDependencyAudit.Analyze(entry,paths,schemas,stock);}catch(InvalidDataException){staleRejected=true;}
        Require(staleRejected,"Stale source audit was accepted.");
        // Reborn: typed upgrade weak names check the mod/stock union, preserve coordinates, and report typos rather than unsupported-layout errors.
        string upgradeEntry=Store("upgrade-weak.xml","<User id='ValidUpgrade' Upgrade='Upgrade_AlliesFaction'/><User id='BadUpgrade' Upgrade='Upgrade_AlliesFactaon'/><UpgradeTemplate id='LocalUpgrade'/><User id='LocalUpgradeUser' Upgrade='LocalUpgrade'/>");
        var upgradePaths=paths with{Sources=new[]{sources.Last()},Includes=Array.Empty<SdkSourcePathAudit.Edge>()};
        var upgradeReport=SdkMissingDependencyAudit.Analyze(upgradeEntry,upgradePaths,schemas,new[]{SdkMissingDependencyAudit.Identity("UpgradeTemplate","Upgrade_AlliesFaction")});
        Require(upgradeReport.Findings.Length==1&&upgradeReport.Findings[0].Code=="MISSINGASSET"&&upgradeReport.Findings[0].Target=="UpgradeTemplate:Upgrade_AlliesFactaon"&&upgradeReport.Findings[0].Line>0,"Typed upgrade weak dependency validation differs.");
        File.WriteAllText(Path.Combine(output,"report.json"),System.Text.Json.JsonSerializer.Serialize(report,new System.Text.Json.JsonSerializerOptions{WriteIndented=true}));
        Console.WriteLine("Reborn: missing dependency audit: PASS (aggregate missing assets/definitions, chain owners, mod+stock union, wrong types, instance-only exclusion, forward/private scope, diamond/quoted controls, positions and stale-source refusal).");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: materialize only fresh small test fixtures and capture hashes as the production path inventory does. */
        //-------------------------------------------------------------------------------------------------
        string Store(string name,string body){string path=Path.Combine(output,name);byte[] bytes=Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'>\n"+body+"\n</AssetDeclaration>");File.WriteAllBytes(path,bytes);sources.Add(new(path,output,Convert.ToHexString(SHA256.HashData(bytes)),bytes.Length));return path;}
    }

    // Reborn: make every diagnostic compatibility assertion fail the test process explicitly.
    private static void Require(bool condition,string message){if(!condition)throw new InvalidDataException(message);}
}
