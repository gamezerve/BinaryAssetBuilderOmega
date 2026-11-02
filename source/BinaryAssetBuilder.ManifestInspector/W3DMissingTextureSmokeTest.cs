using System.Buffers.Binary;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove warnings and actual material normalization agree, with no fabricated textures or dangling native imports.
internal static class W3DMissingTextureSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise known/missing/ambiguous/required texture references through schema diagnostics and the real mesh compiler. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string output,string manifest)
    {
        if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh missing-texture test directory required.");Directory.CreateDirectory(output);
        Settings.Current=new Settings{SchemaPath=ReferencePipelineSmokeTest.FindFixture(),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
        var official=ConscriptSourceInput.Schemas();var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<System.Xml.Schema.XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.NonPublic|BindingFlags.Instance)!.Invoke(schemas,null);
        var texture=ManifestReader.Read(SdkEnvironmentPreflight.Read(manifest,64*1048576)).Assets.First(a=>a.TypeName=="Texture");string known=texture.Name[(texture.Name.IndexOf(':')+1)..];var catalog=new[]{texture};int checks=0;
        string mesh=SdkSchemaHookReview.Mesh.Replace("<W3DMesh ","<W3DMesh xmlns='uri:ea.com:eala:asset' ").Replace("<FXShader ShaderName='reviewed'/>","<FXShader ShaderName='reviewed'><Constants><Texture Name='DiffuseTexture'><Value>"+known+"</Value></Texture><Texture Name='SpecMap'><Value>RebornMissingTextureProbe</Value></Texture><Float Name='EnvMult'><Value>0.5</Value></Float></Constants></FXShader>");
        var doc=Parse(mesh);Validate(doc);var removed=W3DMissingTexturePolicy.OmitMissingBindings(doc,schemas,catalog);Check(removed.SequenceEqual(new[]{"Texture:RebornMissingTextureProbe"}),"Wrong missing binding removed: "+string.Join(",",removed)+"; stock="+texture.Name+"; stored="+texture.InstanceId.ToString("x8")+"; computed="+new InstanceHandle(known).InstanceId.ToString("x8"));
        Check(doc.GetElementsByTagName("Float",SchemaSet.XmlNamespace).Count==1&&doc.GetElementsByTagName("Texture",SchemaSet.XmlNamespace).Count==1,"Unrelated material constants were removed.");
        ConscriptBehaviorSurvey.ResolveStrong(doc,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,doc.OuterXml);var chunk=W3DGeometryCompiler.Compile(instance,schemas);
        Check(instance.ReferencedInstances.Count==1&&chunk.ImportsBuffer.Length==8&&Word(chunk.InstanceBuffer,76)==2,"Known texture/import occurrence or material count differs.");
        var allMissing=Parse(mesh.Replace(known,"Texture:RebornAnotherMissingTexture"));Validate(allMissing);Check(W3DMissingTexturePolicy.OmitMissingBindings(allMissing,schemas,catalog).Length==2,"Typed missing binding was not omitted.");
        ConscriptBehaviorSurvey.ResolveStrong(allMissing,schemas,catalog);var empty=ReferencePipelineSmokeTest.Normalize(schemas,allMissing.OuterXml);var emptyChunk=W3DGeometryCompiler.Compile(empty,schemas);
        Check(empty.ReferencedInstances.Count==0&&emptyChunk.ImportsBuffer.Length==0&&Word(emptyChunk.InstanceBuffer,76)==1,"Missing texture left an import or fabricated an asset.");
        var ambiguous=Parse(mesh.Replace("RebornMissingTextureProbe",known));Validate(ambiguous);Check(W3DMissingTexturePolicy.OmitMissingBindings(ambiguous,schemas,new[]{texture,texture}).Length==0,"Ambiguous existing texture was silently dropped.");Refuse(()=>ConscriptBehaviorSurvey.ResolveStrong(ambiguous,schemas,new[]{texture,texture}));checks++;
        var wrong=Parse(mesh.Replace("RebornMissingTextureProbe","GameObject:RebornMissingTextureProbe"));Validate(wrong);Check(W3DMissingTexturePolicy.OmitMissingBindings(wrong,schemas,catalog).Length==0,"Wrongly typed reference was downgraded to a shader default.");
        string path=Path.Combine(output,"source.xml"),body="<AssetDeclaration xmlns='uri:ea.com:eala:asset'>\n"+mesh+"\n<GameObject id='RequiredTextureUser'><Draws><ScriptedModelDraw id='Draw' TrackMarks='RebornMissingTextureProbe'/></Draws></GameObject>\n</AssetDeclaration>";File.WriteAllText(path,body);byte[] bytes=File.ReadAllBytes(path);
        var source=new SdkSourcePathAudit.Source(path,output,Convert.ToHexString(SHA256.HashData(bytes)),bytes.Length);var paths=new SdkSourcePathAudit.Report(output,null,null,new[]{source},Array.Empty<SdkSourcePathAudit.Edge>(),Array.Empty<SdkSourcePathAudit.Resource>(),Array.Empty<SdkSourcePathAudit.Issue>(),true,false,true,false,false,Array.Empty<string>());
        var report=SdkMissingDependencyAudit.Analyze(path,paths,official,new[]{new SdkMissingDependencyAudit.Asset(texture.TypeName,known,texture.TypeId,texture.InstanceId)});
        Check(report.Findings.Count(f=>f.Code=="MISSINGASSET"&&f.Severity=="Warning"&&f.Owner=="W3DMesh:ReviewedMesh")==1,"Optional shader warning absent.");
        Check(report.Findings.Count(f=>f.Code=="MISSINGASSET"&&f.Severity=="Error"&&f.Owner=="GameObject:RequiredTextureUser")==1,"Required non-shader texture reference stopped being an error.");
        Check(report.Findings.Where(f=>f.Code=="MISSINGASSET").All(f=>f.Line>0&&f.Column>0&&f.Source==path),"Source coordinates were lost.");
        var raw=XDocument.Parse(body);var value=raw.Descendants(XName.Get("Texture",SchemaSet.XmlNamespace)).First().Elements().Single();Check(W3DMissingTexturePolicy.IsOptionalBinding(value)&&!W3DMissingTexturePolicy.IsOptionalBinding(raw.Descendants(XName.Get("ScriptedModelDraw",SchemaSet.XmlNamespace)).Single().Attribute("TrackMarks")!),"Optional binding path scope differs.");
        Console.WriteLine("Reborn: missing shader texture proof: PASS ("+checks+" warning/error, typed, ambiguous, retained-material and native import controls; no placeholder assets or game launch).");

        // Reborn: source typing is established before the same material normalization used by the production build.
        void Validate(XmlDocument xml){xml.Schemas.Add(official);xml.Validate((_,e)=>throw SdkXmlDiagnostics.SchemaError(e));}
        // Reborn: count semantic assertions independently of the producer's own verifier.
        void Check(bool ok,string why){if(!ok)throw new InvalidDataException(why);checks++;}
    }
    // Reborn: test XML has the real EA namespace and no external resolver.
    private static XmlDocument Parse(string text){var doc=new XmlDocument{XmlResolver=null};doc.LoadXml(text);return doc;}
    // Reborn: read the emitted fixed shader header without consulting marshaler field offsets.
    private static uint Word(byte[] bytes,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));
    // Reborn: ambiguity remains a real linking error rather than a missing-texture warning.
    private static void Refuse(Action action){try{action();}catch(InvalidDataException){return;}throw new InvalidDataException("Ambiguous texture was accepted.");}
}
