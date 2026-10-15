using System.Buffers.Binary;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: explicit EP1 enum policies, full KindOf ordering and actual Core weak-name normalization are distinct from payload/existence/publication support.
internal static class GameObjectRootValueSmokeTest
{
    private const string Ns = "uri:ea.com:eala:asset";
    private const string Source = "<GameObject xmlns='uri:ea.com:eala:asset' id='RootValues'><Behaviors><BuildingDestructionBehavior id='ModuleTag_Values'><DieMuxData/><Item/></BuildingDestructionBehavior></Behaviors></GameObject>";
    //-------------------------------------------------------------------------------------------------
    /** Reborn: batch independent stock contracts, every enum literal, cross-word flags and typed weak identities through the composed native/token path. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(XmlSchemaSet schemas, string manifest)
    {
        foreach (string name in new[] { "GameObject:AlliedConstructionYard", "GameObject:SovietAntiInfantryInfantry", "GameObject:SovietAntiStructureShip" })
        {
            var c = GameObjectStockReview.Capture(manifest, name); var fields = GameObjectStockReview.Tokens(c.Bytes, 0);
            foreach (var f in Ep1GameObjectRootValueTokens.ExtraFields)
                if (fields[FastHash.GetHashCode(f.Name)].Type != f.Type || fields[FastHash.GetHashCode(f.Name)].Offset != c.Projection.NativeRoot + f.Offset)
                    throw new InvalidDataException("Extra root token differs from stock: " + name + "/" + f.Name);
            // Reborn: pin engine values independently of the native enum declarations; schema ordinals alone do not encode INVALID=-1 policies.
            var golden = name.EndsWith("AlliedConstructionYard", StringComparison.Ordinal) ? new[] { (84,0), (132,1), (308,0), (312,1), (356,0), (360,-1) } :
                name.EndsWith("SovietAntiStructureShip", StringComparison.Ordinal) ? new[] { (84,1), (132,2), (308,6), (312,0), (356,2), (360,2) } :
                new[] { (84,1), (132,2), (308,2), (312,0), (356,1), (360,0) };
            foreach (var p in golden) if (BinaryPrimitives.ReadInt32LittleEndian(c.Bytes.AsSpan(c.Projection.NativeRoot + p.Item1,4)) != p.Item2) throw new InvalidDataException("Stock root enum engine policy drift.");
            // Reborn: explicit official Allied source names also pin four real stock weak identities; synthetic transformed/health-bar probes remain authored-only.
            if (name.EndsWith("AlliedConstructionYard",StringComparison.Ordinal))
                foreach(var p in new[] {(120,"Allies"),(152,"AlliedConstructionYardCommandSet"),(156,"Portrait_AlliedConstructionYard"),(164,"Button_AlliedConstructionYard_on")})
                    if(BinaryPrimitives.ReadUInt32LittleEndian(c.Bytes.AsSpan(c.Projection.NativeRoot+p.Item1,4))!=FastHash.GetHashCode(p.Item2.ToLowerInvariant()))throw new InvalidDataException("Stock weak source identity differs.");
        }
        int literals = 0;
        foreach (var e in new[] { ("RadarPriority", "RadarPriorityType", typeof(Ep1RadarPriorityType), 84, 0),
            ("EditorSorting", "EditorSortingType", typeof(Ep1EditorSortingType),132,0),
            ("ProductionQueueType", "ProductionQueueType", typeof(Ep1ProductionQueueType),308,-1),
            ("BuildPlacementTypeFlag", "BuildPlacementType", typeof(BuildPlacementType),312,0),
            ("UnitCategory", "UnitCategory", typeof(UnitCategory),356,-1), ("WeaponCategory", "WeaponCategory", typeof(WeaponCategory),360,-1) })
        {
            var facets = Facets(schemas,e.Item2); int value = e.Item5;
            if (Enum.GetNames(e.Item3).Length != facets.Length) throw new InvalidDataException("EP1 root enum name inventory differs: " + e.Item2);
            foreach (var facet in facets)
            {
                string? force = facet.UnhandledAttributes?.SingleOrDefault(a => a.NamespaceURI == "uri:ea.com:eala:asset:schema" && a.LocalName == "forceValue")?.Value;
                if (force != null) value = int.Parse(force,System.Globalization.CultureInfo.InvariantCulture);
                if (!Enum.IsDefined(e.Item3,facet.Value!) || Convert.ToInt32(Enum.Parse(e.Item3,facet.Value!)) != value) throw new InvalidDataException("EP1 enum numeric/name drift: " + e.Item2 + "/" + facet.Value);
                var xml = Xml(Source); xml.DocumentElement!.SetAttribute(e.Item1,facet.Value!); var result = Compile(schemas,xml);
                if (BinaryPrimitives.ReadInt32LittleEndian(result.Chunk.InstanceBuffer.AsSpan(e.Item4,4)) != value) throw new InvalidDataException("Root enum source conversion differs.");
                value++; literals++;
            }
            var bad = Xml(Source); bad.DocumentElement!.SetAttribute(e.Item1,"UNKNOWN_EP1_ENUM"); Refuse(() => Compile(schemas,bad));
        }
        var defaults = GameObjectScalarTokenSmokeTest.Compile(Source);
        if (BinaryPrimitives.ReadInt32LittleEndian(defaults.Chunk.InstanceBuffer.AsSpan(84,4)) != -1) throw new InvalidDataException("EP1 omitted radar default differs.");
        var names = Facets(schemas,"KindOfType").Select(f => f.Value!).ToArray();
        if (!names.SequenceEqual(Enum.GetNames<KindOf>()) || names.Length != KindOfBitFlags.Count) throw new InvalidDataException("EP1 KindOf name/bit order differs.");
        var flags = Xml(Source); int[] indices = { 0,31,32,63,64,names.Length-1 };
        foreach (string field in new[] { "KindOf", "BuildOnRequiredObjectKindOf" }) flags.DocumentElement!.SetAttribute(field,string.Join(" ",indices.Select(i=>names[i])));
        var flagged = Compile(schemas,flags);
        foreach (int offset in new[] { 4,316 })
            for (int word=0;word<10;word++)
            {
                uint expected=0; foreach(int i in indices.Where(i=>i/32==word)) expected|=1u<<(i%32);
                if(BinaryPrimitives.ReadUInt32LittleEndian(flagged.Chunk.InstanceBuffer.AsSpan(offset+word*4,4))!=expected)throw new InvalidDataException("EP1 cross-word KindOf mask differs.");
            }
        flags.DocumentElement!.SetAttribute("KindOf","UNKNOWN_EP1_FLAG"); Refuse(()=>Compile(schemas,flags));
        Weak(schemas);
        Console.WriteLine($"GameObject root values: PASS (15 extra descriptors/three stock roots, {literals} enum literals including forced/sentinel values, {names.Length} KindOf bit order/cross-word masks, seven real-Core weak IDs; no target existence/payload/complete object claim).");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: inherit real official weak-reference types and compare the focused attribute declarations with the admitted GameObject schema before Core normalization. */
    //-------------------------------------------------------------------------------------------------
    private static void Weak(XmlSchemaSet official)
    {
        var saved=Settings.Current;
        try
        {
            string fixture=Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"RootWeakPipeline.xsd");
            Settings.Current=new Settings {SchemaPath=fixture,BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);
            var inputs=new[] {("Side","PlayerTemplate","Allies",120),("CommandSet","LogicCommandSet","AlliedConstructionYardCommandSet",152),
                ("SelectPortrait","PackedTextureImage","Portrait_AlliedConstructionYard",156),("SelectPortraitTransformed","PackedTextureImage","RebornPortraitTransformed",160),
                ("ButtonImage","PackedTextureImage","Button_AlliedConstructionYard_on",164),("ButtonImageTransformed","PackedTextureImage","RebornButtonTransformed",168),("HealthBar","HealthBarTemplate","RebornHealthBarProbe",384)};
            var expected=(XmlSchemaComplexType)official.GlobalTypes[new XmlQualifiedName("GameObject",Ns)]!;
            var focused=(XmlSchemaComplexType)schemas.Schemas.GlobalTypes[new XmlQualifiedName("RootWeakProbeType",Ns)]!;
            var probe=Xml("<RootWeakProbe xmlns='uri:ea.com:eala:asset' id='WeakProbe'/>");
            foreach(var f in inputs)
            {
                var a=(XmlSchemaAttribute)expected.AttributeUses[new XmlQualifiedName(f.Item1)]!; var b=(XmlSchemaAttribute)focused.AttributeUses[new XmlQualifiedName(f.Item1)]!;
                if(a.AttributeSchemaType!.QualifiedName!=b.AttributeSchemaType!.QualifiedName || a.DefaultValue!=b.DefaultValue)throw new InvalidDataException("Focused weak attribute type/default drift: "+f.Item1);
                // Reborn: official WeakReference lexical rules require plain names; Core must infer the target type from the real schema annotation.
                probe.DocumentElement!.SetAttribute(f.Item1,f.Item3);
            }
            var normalized=ReferencePipelineSmokeTest.Normalize(schemas,probe.OuterXml);
            if(normalized.ReferencedInstances.Count!=0 || normalized.WeakReferencedInstances.Count!=7)throw new InvalidDataException("Root weak references became strong imports.");
            var source=Xml(Source);
            foreach(var f in inputs)source.DocumentElement!.SetAttribute(f.Item1,normalized.XmlNode.Attributes![f.Item1]!.Value);
            var compiled=Compile(official,source);
            foreach(var f in inputs)
                if(BinaryPrimitives.ReadUInt32LittleEndian(compiled.Chunk.InstanceBuffer.AsSpan(f.Item4,4))!=FastHash.GetHashCode(f.Item3.ToLowerInvariant()) ||
                    !normalized.WeakReferencedInstances.Any(r=>r.TypeId==FastHash.GetHashCode(f.Item2)&&r.InstanceId==FastHash.GetHashCode(f.Item3.ToLowerInvariant())))throw new InvalidDataException("Core weak identity/native output differs.");
            if(compiled.Chunk.ImportsBuffer.Length!=0)throw new InvalidDataException("Weak root IDs emitted native imports.");
            probe.DocumentElement!.SetAttribute("Side","LogicCommandSet:WrongType"); Refuse(()=>ReferencePipelineSmokeTest.Normalize(schemas,probe.OuterXml));
        }
        finally {Settings.Current=saved;}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: use compiled official simple-type facets while retaining forceValue annotations, not guessed enum ordering. */
    //-------------------------------------------------------------------------------------------------
    private static XmlSchemaEnumerationFacet[] Facets(XmlSchemaSet schemas,string type) => ((XmlSchemaSimpleTypeRestriction)((XmlSchemaSimpleType)schemas.GlobalTypes[new XmlQualifiedName(type,Ns)]!).Content!).Facets.OfType<XmlSchemaEnumerationFacet>().ToArray();
    //-------------------------------------------------------------------------------------------------
    /** Reborn: every positive root literal is schema-validated before native dispatch and projection round trip. */
    //-------------------------------------------------------------------------------------------------
    private static Ep1AlliedModuleSmokeTest.Output Compile(XmlSchemaSet schemas,XmlDocument source) {GameObjectScalarTokenSmokeTest.Validate(schemas,source.OuterXml);var c=GameObjectScalarTokenSmokeTest.Compile(source.OuterXml);GameObjectDestructionProjectionSmokeTest.Authored(c);return c;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: fixture DOMs never resolve external resources. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Xml(string source){var d=new XmlDocument {XmlResolver=null};d.LoadXml(source);return d;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: schema and Core type errors must not silently become zero/native defaults. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action){try{action();}catch(Exception e)when(e is InvalidDataException or BinaryAssetBuilderException){return;}throw new InvalidDataException("Invalid root value admitted.");}
}
