using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: independent stock witnesses and generated source/auxiliary controls cover the Allied module batch without modifying mod sources.
internal static class AlliedUnitModuleSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare all original families, then prove emitted leaf ownership, metadata grammar and corrupt auxiliary rejection. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global)
    {
        // Reborn: exercise the same per-build name catalog lifetime as the production mixed compiler.
        using var nameCatalog=GameObjectModuleReview.ReuseNameCatalog();
        var saved=Settings.Current;
        try{
            string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;var official=ConscriptSourceInput.Schemas();
            Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=new[]{manifest,global}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).ToArray();var baseline=SdkXmlDiagnostics.Parse(File.ReadAllBytes(Path.Combine(fixtures,"live-alpha/ConscriptBuildTime1/data/mod.xml")));
            var grammar=AlliedUnitModuleContract.Qualify(manifest,baseline,schemas,catalog);int checkedLeaves=0,refusals=0;
            foreach(var family in AlliedUnitModuleContract.Families){
                var source=(XmlDocument)baseline.CloneNode(true);var container=source.GetElementsByTagName(family.Container,SchemaSet.XmlNamespace).OfType<XmlElement>().Single();container.RemoveAll();var original=GameObjectExtendedRootContract.Original(family.Path,family.Source).First();container.AppendChild(source.ImportNode(original,true));
                var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml));uint wire=FastHash.GetHashCode(family.Wire);
                Chunk native;
                if(family.Container=="Behaviors")native=ConscriptBehaviorSurvey.Compile(normalized.GetElementsByTagName(family.Source,SchemaSet.XmlNamespace)[0]!,wire);
                else{var carrier=new XmlDocument();carrier.LoadXml("<ClientProbe xmlns='uri:ea.com:eala:asset'/>");carrier.DocumentElement!.AppendChild(carrier.ImportNode(normalized.GetElementsByTagName("ClientBehaviors",SchemaSet.XmlNamespace)[0]!,true));native=Ep1ClientBehaviorsNativeSmokeTest.Leaf(Ep1ClientBehaviorsNativeSmokeTest.Compile(carrier.DocumentElement),0);}
                var element=resolved.GetElementsByTagName(family.Source,SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();var proof=SchemaNativeContract.Verify(element,family.Type,wire,native,references);
                var tokens=family.Container=="Behaviors"?Ep1BehaviorMetadataTokenizer.Tokenize(native,official,family.Wire,family.Type):Ep1BehaviorMetadataTokenizer.TokenizeAlliedClient(native,official,family.Wire,family.Type);_=grammar.Verify(tokens,native,family.Type,proof.Records);checkedLeaves++;
                var broken=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer.ToArray(),ImportsBuffer=native.ImportsBuffer.ToArray()};
                if(broken.RelocationBuffer.Length!=0)broken.RelocationBuffer=broken.RelocationBuffer[4..];else if(broken.ImportsBuffer.Length!=0)broken.ImportsBuffer=broken.ImportsBuffer[4..];else broken.InstanceBuffer[8]^=0x40;
                bool rejected=false;try{SchemaNativeContract.Verify(element,family.Type,wire,broken,references);}catch(InvalidDataException){rejected=true;}if(!rejected)throw new InvalidDataException("Corrupt Allied leaf was accepted: "+family.Wire);refusals++;
            }
            int expressions=CheckExpressions();Console.WriteLine($"Allied unit modules: PASS ({checkedLeaves} original generated families, {refusals} corrupt native/auxiliary refusals, {expressions} arithmetic controls; no game launch).");
        }finally{Settings.Current=saved;}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: check scalar precedence, signs, scientific literals and explicit rejection without changing legacy expression defaults. */
    //-------------------------------------------------------------------------------------------------
    private static int CheckExpressions()
    {
        var definitions=new Dictionary<string,string>(StringComparer.Ordinal){{"RADIUS","20"},{"SCALE","2.5"}};int count=0;
        foreach(var probe in new[]{("=$RADIUS*2","40"),("=($RADIUS+4)/2","12"),("=-$RADIUS+$SCALE*4","-10"),("=1e2+2","102"),("=+(2-3)*4","-4")}){if(SdkAssetNumericExpressions.Evaluate(probe.Item1,definitions)!=probe.Item2)throw new InvalidDataException("Numeric arithmetic result differs.");count++;}
        foreach(string expression in new[]{"=$radius*2","=1/0","=1+","=(1+2","=Run(2)","=1e309"}){bool rejected=false;try{SdkAssetNumericExpressions.Evaluate(expression,definitions);}catch(InvalidDataException){rejected=true;}if(!rejected)throw new InvalidDataException("Invalid numeric expression accepted.");count++;}
        byte[] xml=Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'><Defines><Define name='RADIUS' value='20'/></Defines><GameObject id='Probe' VisionRange='=$RADIUS*2'/></AssetDeclaration>");
        if(SdkLocalDefineProfile.ApplyBeforeInheritance(xml).Bytes!=null||SdkLocalDefineProfile.ApplyBeforeInheritance(xml,numericExpressions:true).Bytes==null)throw new InvalidDataException("Legacy and production arithmetic profiles were conflated.");return count+1;
    }
}
