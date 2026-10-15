using System.Buffers.Binary;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: normalize selected root and module names together and independently check real catalog targets before native/projection comparisons.
internal static class GameObjectRootStrongSourceSmokeTest
{
    private const string Ns="uri:ea.com:eala:asset";
    //-------------------------------------------------------------------------------------------------
    /** Reborn: replace diagnostic source indices with real Core reference traversal, preserving repeat occurrences and typed target identities. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string staticManifest,string globalManifest)
    {
        var saved=Settings.Current;
        try
        {
            XmlSchemaSet? official=null;
            SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:s=>official=s);
            if(official==null)throw new InvalidDataException("Reviewed root schema candidate required.");
            string fixture=Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"RootStrongPipeline.xsd");
            Settings.Current=new Settings {SchemaPath=fixture,BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);CheckSchema(official,schemas.Schemas);
            var catalog=new[]{Catalog(staticManifest),Catalog(globalManifest)}.SelectMany(d=>d.Assets).ToArray();
            var types=new[]{"CrowdResponse","UnitTypeIcon","UnitIntro","FXList"};
            // Reborn: report real type availability rather than silently substituting synthetic targets for absent stock families.
            foreach(string type in types)Console.WriteLine($"Root target availability {type}: {catalog.Count(a=>a.TypeId==FastHash.GetHashCode(type))}");
            var targets=types.Select(type=>catalog.FirstOrDefault(a=>a.TypeId==FastHash.GetHashCode(type) && a.Name.StartsWith(type+":",StringComparison.Ordinal) && catalog.Count(other=>other.TypeId==a.TypeId && other.InstanceId==a.InstanceId)==1)).ToArray();
            // Reborn: require the independently observed stock icon/intro/FX families; CrowdResponse absence is a negative gate, never synthetic success.
            if(targets[1]==null || targets[2]==null || targets[3]==null)throw new InvalidDataException("Unique stock UnitTypeIcon, UnitIntro and FXList targets required.");
            // Reborn: an existing but entirely ambiguous CrowdResponse family is a failure, not equivalent to a truly absent family.
            if(targets[0]==null && catalog.Any(a=>a.TypeId==FastHash.GetHashCode("CrowdResponse")))throw new InvalidDataException("CrowdResponse catalog targets are ambiguous or unnamed.");
            int available=Enumerable.Range(0,3).Where(i=>targets[i]!=null).Aggregate(0,(mask,i)=>mask|(1<<i));
            foreach(var target in targets.OfType<ManifestAsset>())Console.WriteLine($"Root/module real catalog target: {target.Name} {target.TypeId:X8}:{target.InstanceId:X8}");
            int cases=0;
            foreach(bool typed in new[]{false,true})for(int mask=0;mask<8;mask++)
            {
                // Reborn: positive cases may use only real present catalog types; absent families remain covered by explicit failure controls below.
                if((mask&~available)!=0)continue;
                var source=Source(targets,mask,typed);
                GameObjectScalarTokenSmokeTest.Validate(official,Adapt(source.DocumentElement!).OuterXml);
                var instance=Normalize(schemas,source.OuterXml,catalog);
                Check(instance,targets,mask);
                var repeat=Normalize(schemas,source.OuterXml,catalog);
                var a=GameObjectScalarTokenSmokeTest.Compile(Adapt(instance.XmlNode).OuterXml).Chunk;
                var b=GameObjectScalarTokenSmokeTest.Compile(Adapt(repeat.XmlNode).OuterXml).Chunk;
                if(!a.InstanceBuffer.SequenceEqual(b.InstanceBuffer) || !a.RelocationBuffer.SequenceEqual(b.RelocationBuffer) || !a.ImportsBuffer.SequenceEqual(b.ImportsBuffer))throw new InvalidDataException("Named root/module repeat drift.");
                cases++;
            }
            int failures=0;var full=Source(targets,available,false);
            foreach(var field in Ep1GameObjectStrongTokens.Fields)
            {
                foreach(string invalid in new[]{"MissingRebornRootTarget","FXList:"+targets[3]!.Name.Split(':',2)[1],"Name\\99"})
                {var bad=(XmlDocument)full.CloneNode(true);bad.DocumentElement!.SetAttribute(field.Name,invalid);Refuse(()=>Normalize(schemas,bad.OuterXml,catalog));failures++;}
            }
            Refuse(()=>Normalize(schemas,full.OuterXml,catalog.Append(targets[1]!).ToArray()));failures++;
            Refuse(()=>Normalize(schemas,full.OuterXml,catalog.Where(a=>a.TypeId!=targets[2]!.TypeId || a.InstanceId!=targets[2]!.InstanceId).ToArray()));failures++;
            var good=Normalize(schemas,full.OuterXml,catalog);var native=GameObjectScalarTokenSmokeTest.Compile(Adapt(good.XmlNode).OuterXml);
            // Reborn: a valid but wrong dependency index must fail semantic checks even if slot ownership and projection bounds remain valid.
            int first=Enumerable.Range(0,3).First(i=>targets[i]!=null);
            Put(native.Chunk.InstanceBuffer,Ep1GameObjectStrongTokens.Fields[first].Offset,2);Refuse(()=>Verify(native,good,targets,available));failures++;
            Console.WriteLine($"GameObject root named source: PASS ({cases} plain/typed presence cases, actual Core shared table, {targets.Count(t=>t!=null)} real catalog target types, repeated FX occurrences, native/projected index identities and {failures} source/catalog/semantic refusals; missing CrowdResponse not synthesized; focused stubs, no target payload/full GameObject/package/game).");
        }
        finally{Settings.Current=saved;}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: full reviewed attribute types/defaults must agree with the focused harness before it can annotate Core references. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckSchema(XmlSchemaSet official,XmlSchemaSet focused)
    {
        var expected=(XmlSchemaComplexType)official.GlobalTypes[new XmlQualifiedName("GameObject",Ns)]!;
        var probe=(XmlSchemaComplexType)focused.GlobalTypes[new XmlQualifiedName("RootStrongProbeType",Ns)]!;
        foreach(var field in Ep1GameObjectStrongTokens.Fields)
        {
            var a=(XmlSchemaAttribute)expected.AttributeUses[new XmlQualifiedName(field.Name)]!;var b=(XmlSchemaAttribute)probe.AttributeUses[new XmlQualifiedName(field.Name)]!;
            if(a.AttributeSchemaType!.QualifiedName!=b.AttributeSchemaType!.QualifiedName || a.DefaultValue!=b.DefaultValue || a.Use!=b.Use || a.FixedValue!=b.FixedValue)throw new InvalidDataException("Root strong schema declaration drift: "+field.Name);
        }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: read bounded metadata only, never whole stock BIN files, and reject foreign/corrupt catalogs. */
    //-------------------------------------------------------------------------------------------------
    private static ManifestDocument Catalog(string path)
    {
        var d=ManifestReader.Read(SdkEnvironmentPreflight.Read(path,64*1048576));
        TypeRegistryAudit.ValidateTarget(d.Header.Version,d.Header.AllTypesHash);
        if(!d.Header.IsLinked || d.Validate().Count!=0)throw new InvalidDataException("Valid linked EP1 catalog required.");return d;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: author names only; repeated FX references must share the same root/module Core occurrence table without hand-entered suffixes. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Source(ManifestAsset?[] targets,int mask,bool typed)
    {
        var d=Xml("<RootStrongProbe xmlns='uri:ea.com:eala:asset' id='NamedStrong'><BuildingDestructionBehavior id='ModuleTag_NamedStrong'><DieMuxData/><Item/><Item/></BuildingDestructionBehavior></RootStrongProbe>");
        for(int i=0;i<3;i++)if((mask&(1<<i))!=0)d.DocumentElement!.SetAttribute(Ep1GameObjectStrongTokens.Fields[i].Name,typed?targets[i]!.Name:targets[i]!.Name.Split(':',2)[1]);
        foreach(XmlElement item in d.GetElementsByTagName("Item",Ns))item.SetAttribute("FX",typed?targets[3]!.Name:targets[3]!.Name.Split(':',2)[1]);return d;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: guard authored diagnostic indices before the real normalizer and demand exactly one catalog identity per resulting occurrence. */
    //-------------------------------------------------------------------------------------------------
    private static InstanceDeclaration Normalize(SchemaSet schemas,string source,ManifestAsset[] catalog)
    {
        if(source.Contains('\\'))throw new InvalidDataException("Authored root/module reference indices are forbidden.");
        var instance=ReferencePipelineSmokeTest.Normalize(schemas,source);
        foreach(var r in instance.ReferencedInstances)
            if(catalog.Count(a=>a.TypeId==r.TypeId && a.InstanceId==r.InstanceId)!=1)throw new InvalidDataException("Missing/ambiguous named root/module target: "+r.Name);
        return instance;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: the diagnostic adapter transfers normalized values without allocating a second reference table or compiling target payloads. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Adapt(XmlNode probe)
    {
        var d=Xml("<GameObject xmlns='uri:ea.com:eala:asset' id='NamedStrong' EditorName='Reborn named root allocations'><Behaviors/></GameObject>");
        foreach(var field in Ep1GameObjectStrongTokens.Fields)
            if(probe.Attributes![field.Name] is XmlAttribute a)d.DocumentElement!.SetAttribute(field.Name,a.Value);
        d.DocumentElement!.FirstChild!.AppendChild(d.ImportNode(probe.ChildNodes.OfType<XmlElement>().Single(),true));return d;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain real Core order and repeated occurrence identities before testing native and composed output. */
    //-------------------------------------------------------------------------------------------------
    private static void Check(InstanceDeclaration instance,ManifestAsset?[] targets,int mask)
    {
        var output=GameObjectScalarTokenSmokeTest.Compile(Adapt(instance.XmlNode).OuterXml);Verify(output,instance,targets,mask);
        GameObjectDestructionProjectionSmokeTest.Authored(output);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: bind every expected source occurrence to its concrete type/instance, one-biased native value and shifted IMP location. */
    //-------------------------------------------------------------------------------------------------
    private static void Verify(Ep1AlliedModuleSmokeTest.Output native,InstanceDeclaration instance,ManifestAsset?[] targets,int mask)
    {
        var expected=Enumerable.Range(0,3).Where(i=>(mask&(1<<i))!=0).Concat(new[]{3,3}).ToArray();
        if(instance.ReferencedInstances.Count!=expected.Length)throw new InvalidDataException("Root/module occurrence count differs.");
        var result=Ep1GameObjectDestructionProjection.Compose(native.Chunk);
        int prefix=GameObjectStockReview.Tokens(result.InstanceBuffer,0)[FastHash.GetHashCode("BuildTime")].Offset-140;
        int items=(int)Word(native.Chunk.InstanceBuffer,native.Native+68);var slots=new List<uint>();
        for(int index=0;index<expected.Length;index++)
        {
            int target=expected[index];var r=instance.ReferencedInstances[index];var wanted=targets[target]!;
            int at=target<3?Ep1GameObjectStrongTokens.Fields[target].Offset:items+8+(index-(expected.Length-2))*20;
            if(r.TypeId!=wanted.TypeId || r.InstanceId!=wanted.InstanceId || Word(native.Chunk.InstanceBuffer,at)!=index+1 || Word(result.InstanceBuffer,prefix+at)!=index+1)
                throw new InvalidDataException("Named root/module import identity differs.");
            slots.Add((uint)(prefix+at));
        }
        var actual=Enumerable.Range(0,result.ImportsBuffer.Length/4-1).Select(i=>Word(result.ImportsBuffer,i*4));
        if(!actual.OrderBy(s=>s).SequenceEqual(slots.OrderBy(s=>s)))throw new InvalidDataException("Named root/module IMP ownership differs.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: fixture XML never resolves external entities. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Xml(string text){var d=new XmlDocument {XmlResolver=null};d.LoadXml(text);return d;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: expected schema/reference/catalog failures cannot silently pass or be swallowed as unrelated exceptions. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action){try{action();}catch(Exception e)when(e is InvalidDataException or BinaryAssetBuilderException or XmlSchemaValidationException){return;}throw new InvalidDataException("Invalid named root source admitted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: native import reads use the supplied owned buffer's Win32 representation. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));
    //-------------------------------------------------------------------------------------------------
    /** Reborn: semantic corruption changes only an owned diagnostic chunk. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
