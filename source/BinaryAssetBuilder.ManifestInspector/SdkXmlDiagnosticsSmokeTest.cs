using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: diagnostic regression uses captured synthetic sources, never the user's editable mod or a game launch.
internal static class SdkXmlDiagnosticsSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify original syntax/schema/inheritance/fragment coordinates and honest ambiguity handling independently of native compilation. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string output)
    {
        output=SdkEnvironmentPreflight.Absolute(output);SdkEnvironmentPreflight.CheckPath(output);
        if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh diagnostic output required.");
        SdkEnvironmentPreflight.DirectoryPath(Path.GetDirectoryName(output)!);
        Directory.CreateDirectory(output);int checks=0;
        string entry=Path.Combine(output,"entry.xml"),basis=Path.Combine(output,"base.xml"),fragment=Path.Combine(output,"fragment.xml");
        const string derived="<?xml version=\"1.0\"?>\n<!-- authored source -->\n\n<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">\n  <GameObject id=\"Derived\" inheritFrom=\"Base\"\n    BuildTime=\"bad\">\n    <Behaviors><Test id=\"Module\" Value=\"changed\" /></Behaviors>\n  </GameObject>\n</AssetDeclaration>";
        const string parent="<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">\n\n  <GameObject id=\"Base\"\n    Scale=\"1.5\" />\n</AssetDeclaration>";
        const string piece="<?xml version=\"1.0\"?>\n<Draws xmlns=\"uri:ea.com:eala:asset\">\n  <Parachute id=\"Draw\"\n    Rate=\"invalid\" />\n</Draws>";
        var snapshots=new[]{new SdkXmlDiagnostics.Snapshot(entry,Bytes(derived)),new SdkXmlDiagnostics.Snapshot(basis,Bytes(parent)),new SdkXmlDiagnostics.Snapshot(fragment,Bytes(piece))};
        var map=new SdkXmlDiagnostics.SourceMap(snapshots);
        // Reborn: preserve code-first duplicate diagnostics through wrappers, with and without available source coordinates.
        var duplicateError=new InvalidDataException("GameObject source preparation: DUPLICATEASSET ' SpecialPowerTemplate:RepeatedPower ' Duplicate declaration.");
        RequireDuplicate(duplicateError,false);SdkXmlDiagnostics.Attach(duplicateError,new(entry,12,5));RequireDuplicate(duplicateError,true);
        // Reborn: the exact copyable label remains unchanged when a source-preparation context is stripped.
        void RequireDuplicate(Exception error,bool hasSource){string? line=SdkXmlDiagnostics.Format(error);if(line==null||!line.StartsWith("DUPLICATEASSET ' SpecialPowerTemplate:RepeatedPower '",StringComparison.Ordinal)||line.Contains("GameObject source preparation:")||hasSource&&!line.Contains("Source: "+entry+"(12,5).",StringComparison.Ordinal))throw new InvalidDataException("Code-first duplicate diagnostic lost label or source evidence.");}
        var final=SdkXmlDiagnostics.Parse(Bytes("<GameObject xmlns=\"uri:ea.com:eala:asset\" id=\"Derived\" BuildTime=\"bad\" Scale=\"1.5\"><Behaviors><Test id=\"Module\" Value=\"changed\" /></Behaviors><Draws><Parachute id=\"Draw\" Rate=\"invalid\" /></Draws></GameObject>"));
        Check(final.DocumentElement!.GetAttributeNode("BuildTime")!,entry,6,false);
        Check(final.DocumentElement.GetAttributeNode("Scale")!,basis,4,false);
        Check(final.SelectSingleNode("//*[local-name()='Test']/@Value")!,entry,7,false);
        Check(final.SelectSingleNode("//*[local-name()='Parachute']/@Rate")!,fragment,4,false);

        // Reborn: included support assets retain their own exact field origin instead of blaming the GameObject entry file.
        foreach(string type in new[]{"ArmorTemplate","WeaponTemplate","LogicCommandSet","Texture","PackedTextureImage","ObjectCreationList","W3DContainer","W3DHierarchy","W3DCollisionBox","W3DMesh"}){
            string supportPath=Path.Combine(output,type+".xml");string text="<AssetDeclaration xmlns='uri:ea.com:eala:asset'>\n<"+type+" id='Support'>\n<Value Bad='broken'/>\n</"+type+"></AssetDeclaration>";
            var support=SdkXmlDiagnostics.Parse(Bytes("<"+type+" xmlns='uri:ea.com:eala:asset' id='Support'><Value Bad='broken'/></"+type+">"));
            var supportMap=new SdkXmlDiagnostics.SourceMap(snapshots.Concat(new[]{new SdkXmlDiagnostics.Snapshot(supportPath,Bytes(text))}));var failure=SdkXmlDiagnostics.At(new InvalidDataException("Support field"),support.SelectSingleNode("//@Bad"));supportMap.Enrich(failure,entry,support);Expect(failure,supportPath,3,false);
        }

        // Reborn: shader Texture constants have no asset id and must retain their enclosing W3DMesh source owner.
        string modelPath=Path.Combine(output,"model.w3x");const string modelText="<AssetDeclaration xmlns='uri:ea.com:eala:asset'>\n<W3DMesh id='Model.Mesh'>\n<FXShader><Constants><Texture Name='DiffuseTexture'>\n<Value Bad='broken'>MissingTexture</Value>\n</Texture></Constants></FXShader>\n</W3DMesh></AssetDeclaration>";
        var model=SdkXmlDiagnostics.Parse(Bytes("<W3DMesh xmlns='uri:ea.com:eala:asset' id='Model.Mesh'><FXShader><Constants><Texture Name='DiffuseTexture'><Value Bad='broken'>MissingTexture</Value></Texture></Constants></FXShader></W3DMesh>"));
        var modelFailure=SdkXmlDiagnostics.At(new InvalidDataException("Model shader field"),model.SelectSingleNode("//@Bad"));new SdkXmlDiagnostics.SourceMap(new[]{new SdkXmlDiagnostics.Snapshot(modelPath,Bytes(modelText))}).Enrich(modelFailure,entry,model);Expect(modelFailure,modelPath,4,false);

        // Reborn: validators see a compact generated document; their offending DOM node must map back to multiline authored input.
        const string xsd="<xs:schema xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" targetNamespace=\"uri:ea.com:eala:asset\" elementFormDefault=\"qualified\"><xs:element name=\"GameObject\"><xs:complexType><xs:sequence><xs:any minOccurs=\"0\" maxOccurs=\"unbounded\" processContents=\"skip\" /></xs:sequence><xs:attribute name=\"id\" type=\"xs:string\"/><xs:attribute name=\"BuildTime\" type=\"xs:float\"/><xs:attribute name=\"Scale\" type=\"xs:float\"/></xs:complexType></xs:element></xs:schema>";
        using(var reader=XmlReader.Create(new StringReader(xsd)))final.Schemas.Add(null,reader);
        try{final.Validate((_,e)=>throw SdkXmlDiagnostics.SchemaError(e));throw new InvalidOperationException("Invalid schema value accepted.");}
        catch(InvalidDataException error){map.Enrich(error,entry,final);Expect(error,entry,6,false);}

        // Reborn: comparison masks can rename both asset and module IDs without corrupting the original offending field locator.
        var masked=(XmlDocument)final.CloneNode(true);masked.DocumentElement!.SetAttribute("id","Masked");((XmlElement)masked.SelectSingleNode("//*[local-name()='Test']")!).SetAttribute("id","MaskedModule");
        Check(SdkXmlDiagnostics.Unmasked(masked.SelectSingleNode("//*[local-name()='Test']/@Value"),final)!,entry,7,false);
        // Reborn: an unrelated changed local ID must not hide the actual extra root owner behind a generic child-count error.
        var comparison=SdkXmlDiagnostics.Parse(Bytes("<GameObject xmlns='uri:ea.com:eala:asset'><VisionInfo id='Baseline'/></GameObject>"));
        var expanded=SdkXmlDiagnostics.Parse(Bytes("<GameObject xmlns='uri:ea.com:eala:asset'><VisionInfo id='Authored'/><ProjectedBuildabilityInfo/></GameObject>"));
        string ownerDifference=ConscriptSourceInput.Difference(expanded.DocumentElement!,comparison.DocumentElement!,"GameObject",out var ownerNode);
        if(ownerNode?.LocalName!="ProjectedBuildabilityInfo"||!ownerDifference.Contains("ProjectedBuildabilityInfo source=1, baseline=0",StringComparison.Ordinal))throw new InvalidDataException("Extra root-owner diagnostic selected an unrelated local ID.");checks++;
        // Reborn: removing an earlier comparison-only owner cannot redirect an AudioArraySound diagnostic to the following VisionInfo.
        var shifted=SdkXmlDiagnostics.Parse(Bytes("<GameObject xmlns='uri:ea.com:eala:asset'><GameDependency/><AudioArraySound/><VisionInfo/></GameObject>"));var shiftedCopy=(XmlDocument)shifted.CloneNode(true);shiftedCopy.DocumentElement!.RemoveChild(shiftedCopy.DocumentElement.FirstChild!);
        if(SdkXmlDiagnostics.Unmasked(shiftedCopy.DocumentElement.FirstChild,shifted)?.LocalName!="AudioArraySound")throw new InvalidDataException("Removed comparison owner shifted the original source locator.");checks++;

        // Reborn: unavailable provenance must be labelled context; a generated line or guessed field must never be advertised as exact.
        var generic=new InvalidDataException("Unavailable field");map.Enrich(generic,entry,final);Expect(generic,entry,5,true);
        var ambiguous=new SdkXmlDiagnostics.SourceMap(snapshots.Concat(new[]{new SdkXmlDiagnostics.Snapshot(Path.Combine(output,"other-fragment.xml"),Bytes(piece))}));
        var duplicate=SdkXmlDiagnostics.At(new InvalidDataException("Ambiguous fragment"),final.SelectSingleNode("//*[local-name()='Parachute']/@Rate"));ambiguous.Enrich(duplicate,entry,final);Expect(duplicate,entry,5,true);
        var unsupported=SdkXmlDiagnostics.Parse(Bytes("<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\"><Unknown /></AssetDeclaration>"));
        var metadataMap=new SdkXmlDiagnostics.SourceMap(new[]{new SdkXmlDiagnostics.Snapshot(entry,Bytes("<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">\n\n <Unknown />\n</AssetDeclaration>"))});
        var unknown=SdkXmlDiagnostics.At(new InvalidDataException("Unsupported asset"),unsupported.DocumentElement!.FirstChild);metadataMap.Enrich(unknown,entry,unsupported);Expect(unknown,entry,3,false);

        // Reborn: syntax and entity refusals keep their physical input identity rather than an outer generated source context.
        foreach(string broken in new[]{"<?xml version=\"1.0\"?>\n\n<Root>\n</Wrong>","<?xml version=\"1.0\"?>\n<!DOCTYPE Root SYSTEM 'file:///never-read'>\n<Root/>"})
        {
            try{SdkXmlDiagnostics.Parse(Bytes(broken),entry);throw new InvalidOperationException("Unsafe/malformed XML accepted.");}
            // Reborn: .NET's DTD prohibition may report line zero; retain the file and explicitly mark unavailable coordinates rather than inventing them.
            catch(XmlException error){var location=SdkXmlDiagnostics.Origin(error);if(location?.File!=entry||location.Line!=error.LineNumber||location.Column!=error.LinePosition||location.Context!=(error.LineNumber<=0))throw new InvalidDataException("Syntax location lost.");checks++;}
        }
        // Reborn: changing a source after capture cannot redirect diagnostics or substitute a new line-number witness.
        File.WriteAllText(entry,"<Other/>");Check(final.DocumentElement.GetAttributeNode("BuildTime")!,entry,6,false);
        string? fallback=SdkXmlDiagnostics.Format(new InvalidDataException("Plain error"),new[]{entry});if(fallback==null||!fallback.Contains("[source context;")||!fallback.Contains("(1,2)"))throw new InvalidDataException("Context fallback is misleading.");checks++;
        // Reborn: exercise the real source frontend as well as isolated provenance, using only a fresh offline fixture directory.
        string fixture=Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"live-alpha","ConscriptBuildTime1","data","mod.xml");
        byte[] complete=File.ReadAllBytes(fixture);
        foreach(var edit in new[]{("UnknownRootField","1"),("Scale","NaN"),("RadarPriority","RebornBadEnum")})
        {
            var authored=SdkXmlDiagnostics.Parse(complete);authored.DocumentElement!.SetAttribute(edit.Item1,edit.Item2);authored.Save(entry);Refuse(entry,entry,edit.Item1);
        }
        var nested=SdkXmlDiagnostics.Parse(complete);((XmlElement)nested.SelectSingleNode("//*[local-name()='Physics']")!).SetAttribute("GravityMultiplier","NaN");nested.Save(entry);Refuse(entry,entry,"GravityMultiplier");
        // Reborn: changing a valid key is supported; a non-ASCII wire key remains a genuine encoding error with its original location.
        nested=SdkXmlDiagnostics.Parse(complete);nested.SelectSingleNode("//*[local-name()='DisplayName']")!.InnerText="Name:Invalidé";nested.Save(entry);Refuse(entry,entry,"DisplayName",attribute:false);
        var baseOwner=SdkXmlDiagnostics.Parse(complete);File.WriteAllText(basis,"<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">"+baseOwner.DocumentElement!.OuterXml+"</AssetDeclaration>");
        File.WriteAllText(entry,"<?xml version=\"1.0\"?>\n<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">\n<Includes><Include type=\"instance\" source=\"base.xml\"/></Includes>\n<GameObject id=\"RebornConscriptTrial\" inheritFrom=\"SovietAntiInfantryInfantry\"\n BuildTime=\"bad\"/>\n</AssetDeclaration>");Refuse(entry,entry,"BuildTime");
        // Reborn: invalid inherited values must point to the instance base rather than the entry that merely Includes it.
        baseOwner.DocumentElement.SetAttribute("Scale","NaN");File.WriteAllText(basis,"<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">\n"+baseOwner.DocumentElement.OuterXml+"</AssetDeclaration>");
        File.WriteAllText(entry,File.ReadAllText(entry).Replace("BuildTime=\"bad\"","BuildTime=\"1\""));Refuse(entry,basis,"Scale");
        // Reborn: actual imported module failures must point inside the captured fragment, not at its enclosing generated GameObject.
        var fragmented=SdkXmlDiagnostics.Parse(complete);var physics=(XmlElement)fragmented.SelectSingleNode("//*[local-name()='Physics']")!;physics.SetAttribute("GravityMultiplier","NaN");
        File.WriteAllText(fragment,"<?xml version=\"1.0\"?>\n<Behaviors xmlns=\"uri:ea.com:eala:asset\">\n"+physics.OuterXml+"\n</Behaviors>");
        var xi=fragmented.CreateElement("xi","include","http://www.w3.org/2001/XInclude");xi.SetAttribute("href","fragment.xml");xi.SetAttribute("xpointer","xmlns(a=uri:ea.com:eala:asset) xpointer(/a:Behaviors/child::*)");physics.ParentNode!.ReplaceChild(xi,physics);
        File.WriteAllText(entry,"<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">\n"+fragmented.DocumentElement!.OuterXml+"</AssetDeclaration>");Refuse(entry,fragment,"GravityMultiplier");
        // Reborn: whole-root includes must retain the actual failing module's source coordinates just like child-XPointer includes.
        File.WriteAllText(fragment,"<?xml version=\"1.0\"?>\n"+physics.OuterXml);xi.RemoveAttribute("xpointer");
        File.WriteAllText(entry,"<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">\n"+fragmented.DocumentElement!.OuterXml+"</AssetDeclaration>");Refuse(entry,fragment,"GravityMultiplier");
        File.WriteAllText(entry,"<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">\n<Includes>\n<Include type=\"all\" source=\"missing.xml\"/>\n</Includes>\n</AssetDeclaration>");Refuse(entry,entry,"source");
        // Reborn: an Includes-only entry and same-ID overlay must report an unsupported inherited module in its real base file.
        var unreviewed=SdkXmlDiagnostics.Parse(complete);var unreviewedBehaviors=(XmlElement)unreviewed.SelectSingleNode("//*[local-name()='Behaviors']")!;
        unreviewedBehaviors.RemoveAll();unreviewedBehaviors.InnerXml="<LineSegmentDamageUpdate xmlns='uri:ea.com:eala:asset' id='ModuleTag_Unreviewed' NodeLifetime='1s' DamageInterval='0.1s' Distance='25'/>";
        File.WriteAllText(basis,"<AssetDeclaration xmlns='uri:ea.com:eala:asset'>\n"+unreviewed.DocumentElement!.OuterXml+"</AssetDeclaration>");
        string overlay=Path.Combine(output,"overlay.xml");string sameId=unreviewed.DocumentElement.GetAttribute("id");
        File.WriteAllText(overlay,"<AssetDeclaration xmlns='uri:ea.com:eala:asset'><Includes><Include type='instance' source='base.xml'/></Includes><GameObject id='"+sameId+"' inheritFrom='"+sameId+"' BuildTime='2'/></AssetDeclaration>");
        File.WriteAllText(entry,"<AssetDeclaration xmlns='uri:ea.com:eala:asset'><Includes><Include type='all' source='overlay.xml'/></Includes></AssetDeclaration>");Refuse(entry,basis,"LineSegmentDamageUpdate",attribute:false);
        // Reborn: generated model graphs can exceed the individual source quota without losing assets or reopening DTD/entity expansion.
        string merged=Path.Combine(output,"additional.xml");string payload=new('1',5*1048576);
        File.WriteAllText(merged,"<AssetDeclaration><W3DMesh id='Large'><Vertices>"+payload+"</Vertices></W3DMesh><W3DCollisionBox id='Last'/></AssetDeclaration>");
        var large=ConscriptMultiAssetBuild.ReadPrepared(merged);
        if(large.DocumentElement!.ChildNodes.Count!=2||large.DocumentElement.FirstChild!.FirstChild!.InnerText!=payload||SdkXmlDiagnostics.Parse(Bytes(large.DocumentElement.FirstChild.OuterXml)).DocumentElement!.GetAttribute("id")!="Large")throw new InvalidDataException("Large prepared model XML was truncated or rejected.");checks++;
        if(SdkXmlDiagnostics.Lines(File.ReadAllBytes(merged),merged).Root!.Elements().Count()!=2)throw new InvalidDataException("Large XML diagnostics lost assets.");checks++;
        try{SdkEnvironmentPreflight.Read(merged,4*1048576);throw new InvalidOperationException("Individual reader quota was ignored.");}
        catch(InvalidDataException error){if(SdkXmlDiagnostics.Origin(error)?.File!=merged||!error.Message.Contains("4194304")||!error.Message.Contains(new FileInfo(merged).Length.ToString()))throw new InvalidDataException("File quota diagnostic omitted actual path/size/limit.");checks++;}
        File.WriteAllText(merged,"<!DOCTYPE AssetDeclaration [<!ENTITY x 'expanded'>]><AssetDeclaration>&x;</AssetDeclaration>");
        try{ConscriptMultiAssetBuild.ReadPrepared(merged);throw new InvalidOperationException("Prepared XML accepted a DTD.");}catch(XmlException){checks++;}
        Console.WriteLine($"XML source diagnostics: PASS ({checks} checks; syntax/schema/derived/base/fragment/mask/ambiguity/snapshot/context/real frontend/large merged XML).");

        // Reborn: all checks compare physical source coordinates and the explicit exact-versus-context contract.
        void Check(XmlNode node,string file,int line,bool context){var error=SdkXmlDiagnostics.At(new InvalidDataException("Probe"),node);map.Enrich(error,entry,final);Expect(error,file,line,context);}
        // Reborn: compare both the authored coordinate and its confidence instead of accepting a filename-only diagnostic.
        void Expect(Exception error,string file,int line,bool context){var actual=SdkXmlDiagnostics.Origin(error);if(actual?.File!=file||actual.Line!=line||actual.Column<1||actual.Context!=context)throw new InvalidDataException("Unexpected XML origin: "+actual+"; expected "+file+":"+line+", context="+context);checks++;}
        // Reborn: assert the actual rejecting frontend retains the physical field's coordinates, not merely a filename in free-form text.
        void Refuse(string input,string origin,string name,bool attribute=true)
        {
            var original=SdkXmlDiagnostics.Lines(File.ReadAllBytes(origin),origin);var field=attribute?(System.Xml.Linq.XObject)original.Descendants().Attributes().First(a=>a.Name.LocalName==name):original.Descendants().First(e=>e.Name.LocalName==name);
            int expected=SdkXmlDiagnostics.Point(origin,field).Line;
            try{ConscriptSourceInput.Prepare(input,Path.Combine(output,"rejected-"+checks));throw new InvalidOperationException("Unsupported XML accepted: "+name);}
            catch(Exception error)when(error is InvalidDataException or XmlException or XmlSchemaException){Expect(error,origin,expected,false);}
        }
    }

    // Reborn: fixed UTF-8 fixtures include deliberate authored whitespace as line-number evidence.
    private static byte[] Bytes(string text)=>Encoding.UTF8.GetBytes(text);
}
