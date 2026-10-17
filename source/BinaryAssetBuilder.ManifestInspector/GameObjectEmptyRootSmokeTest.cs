using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: capture only sixteen named empty/null root contracts, never infer populated child ABI support from zero storage.
internal static class GameObjectEmptyRootSmokeTest
{
    // Reborn: native root slots come from the independently inventoried 600-byte stock root.
    internal static readonly (string Name,int Slot,bool List)[] Fields={
        ("Browser",44,true),("GameDependency",412,false),("BuildFadeInOnCreateList",444,false),("BuildVariations",448,false),("EquivalentTo",452,true),
        ("ReplaceModule",504,false),("InheritableModule",508,false),("AutoResolveArmor",512,false),("AutoResolveWeapon",516,false),
        ("FormationPreviewDecal",520,false),("FormationPreviewItemDecal",524,false),("AudioArraySound",536,false),("UpgradeCameo",544,true),
        ("AutoResolveInfo",556,false),("ProjectedBuildabilityInfo",568,true),("DisplayUpgrade",576,true)};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: report bounded stock own rows and empty-list marker/target facts before admitting any production literals. */
    //-------------------------------------------------------------------------------------------------
    internal static void Survey(string manifest)
    {
        var stock=GameObjectStockReview.Capture(manifest,"GameObject:SovietAntiInfantryInfantry");var root=GameObjectStockReview.Tokens(stock.Bytes,0);
        foreach(var field in Fields)
        {
            var token=root[FastHash.GetHashCode(field.Name)];int native=stock.Projection.NativeRoot+field.Slot;int offset=(int)Marshal.OffsetOf<GameObject>(field.Name);if(offset!=field.Slot || Word(stock.Bytes,native)!=0 || field.List && Word(stock.Bytes,native+4)!=0)throw new InvalidDataException("Stock empty root ABI differs: "+field.Name);
            Console.WriteLine(JsonSerializer.Serialize(new{field.Name,field.Slot,field.List,Type=$"{token.Type:X8}",token.Offset,Marker=field.List?new[]{Word(stock.Bytes,token.Offset),Word(stock.Bytes,token.Offset+4),Word(stock.Bytes,token.Offset+8)}:Array.Empty<uint>(),NativeRoot=stock.Projection.NativeRoot}));
        }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: all sixteen source/ABI/stock contracts join the already independently qualified 49-module/six-owner output; named-field completeness never substitutes for original values. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(XmlSchemaSet schemas,XmlDocument source,XmlElement[] originals,Chunk native,InstanceDeclaration instance,string manifest,Func<Chunk,Chunk,InstanceDeclaration,int> verifyOwners)
    {
        var stock=GameObjectStockReview.Capture(manifest,"GameObject:SovietAntiInfantryInfantry");var stockRows=GameObjectStockReview.Tokens(stock.Bytes,0);
        if(Marshal.SizeOf<GameObject>()!=600)throw new InvalidDataException("Empty root native size differs.");
        // Reborn: declarations must retain reviewed optionality, source spelling, cardinality and type identity, not merely coincidentally generate zero bytes.
        var rootSchema=(XmlSchemaComplexType)((XmlSchemaElement)schemas.GlobalElements[new XmlQualifiedName("GameObject",SchemaSet.XmlNamespace)]!).ElementSchemaType!;var declarations=new Dictionary<string,XmlSchemaElement>(StringComparer.Ordinal);Collect(rootSchema.ContentTypeParticle);
        foreach(var field in Fields)
        {
            if((int)Marshal.OffsetOf<GameObject>(field.Name)!=field.Slot)throw new InvalidDataException("Empty root field offset differs.");Type type=typeof(GameObject).GetField(field.Name)!.FieldType;
            if(field.List? !type.IsGenericType || type.GetGenericTypeDefinition().FullName!="Relo.List`1":!type.IsPointer)throw new InvalidDataException("Empty root field storage category differs.");
            for(int at=field.Slot;at<field.Slot+(field.List?8:4);at+=4)if(Word(stock.Bytes,stock.Projection.NativeRoot+at)!=0 || Word(native.InstanceBuffer,at)!=0)throw new InvalidDataException("Empty root native/source profile differs.");
            if(field.Name=="Browser")
            {if(rootSchema.AttributeUses[new XmlQualifiedName("Browser")] is not XmlSchemaAttribute a || a.Use==XmlSchemaUse.Required || a.AttributeSchemaType?.Name!="StringList" || !string.IsNullOrWhiteSpace(a.DefaultValue) || !string.IsNullOrWhiteSpace(a.FixedValue))throw new InvalidDataException("Browser must remain the optional empty StringList attribute.");}
            else
            // Reborn: built-in schema types can have a null Name property; qualified identity also prevents accepting a same-spelling foreign namespace type.
            {string typeName=field.Name switch{"GameDependency"=>"GameDependencyType","BuildFadeInOnCreateList" or "BuildVariations"=>"StringList","EquivalentTo"=>"GameObjectWeakRef","FormationPreviewItemDecal"=>"FormationPreviewDecal","UpgradeCameo"=>"string","DisplayUpgrade"=>"UpgradeTemplateWeakRef",_=>field.Name};var expectedType=new XmlQualifiedName(typeName,field.Name=="UpgradeCameo"?XmlSchema.Namespace:SchemaSet.XmlNamespace);if(!declarations.TryGetValue(field.Name,out var e) || e.ElementSchemaType?.QualifiedName!=expectedType || e.MinOccurs!=0 || (e.MaxOccurs>1)!=field.List || e.DefaultValue!=null || e.FixedValue!=null)throw new InvalidDataException("Empty root source declaration drift: "+field.Name+"; type="+e?.ElementSchemaType?.QualifiedName+"; declared="+e?.SchemaTypeName+"; min/max="+e?.MinOccurs+"/"+e?.MaxOccurs+"; default="+e?.DefaultValue+"; fixed="+e?.FixedValue);}
        }
        foreach(var original in originals)Ep1GameObjectEmptyRootProjection.ValidateSource(original);Ep1GameObjectEmptyRootProjection.ValidateSource(source.DocumentElement!);
        var before=Clone(native);var output=Ep1GameObjectEmptyRootProjection.Compose(native,schemas);int prefix=Verify(output);if(!Equal(before,native) || !Equal(output,Ep1GameObjectEmptyRootProjection.Compose(native,schemas)))throw new InvalidDataException("Empty root repeat/input isolation differs.");int refusals=0;
        foreach(var field in Fields)
        {
            for(int at=field.Slot;at<field.Slot+(field.List?8:4);at+=4){var bad=Clone(native);Put(bad.InstanceBuffer,at,1);Refuse(()=>Ep1GameObjectEmptyRootProjection.Compose(bad,schemas));refusals++;}
            var rows=GameObjectStockReview.Tokens(output.InstanceBuffer,0);int row=Find(output.InstanceBuffer,FastHash.GetHashCode(field.Name));foreach(int at in new[]{row,row+4,row+8}){var bad=Clone(output);bad.InstanceBuffer[at]^=1;Refuse(()=>Verify(bad));refusals++;}
            if(field.List)foreach(int at in new[]{rows[FastHash.GetHashCode(field.Name)].Offset,rows[FastHash.GetHashCode(field.Name)].Offset+4,rows[FastHash.GetHashCode(field.Name)].Offset+8}){var bad=Clone(output);bad.InstanceBuffer[at]^=1;Refuse(()=>Verify(bad));refusals++;}
            var edited=(XmlDocument)source.CloneNode(true);if(field.Name=="Browser")edited.DocumentElement!.SetAttribute(field.Name,"RebornUnreviewed");else edited.DocumentElement!.AppendChild(edited.CreateElement(field.Name,SchemaSet.XmlNamespace));Refuse(()=>Ep1GameObjectEmptyRootProjection.ValidateSource(edited.DocumentElement!));refusals++;
        }
        foreach(bool relo in new[]{true,false}){var bad=Clone(output);if(relo)bad.RelocationBuffer=Array.Empty<byte>();else bad.ImportsBuffer=Array.Empty<byte>();Refuse(()=>Verify(bad));refusals++;}
        var emptyBrowser=(XmlDocument)source.CloneNode(true);emptyBrowser.DocumentElement!.SetAttribute("Browser","");GameObjectBodyProjectionSmokeTest.Validate(schemas,emptyBrowser.OuterXml);Ep1GameObjectEmptyRootProjection.ValidateSource(emptyBrowser.DocumentElement!);
        Console.WriteLine($"Conscript complete named root: PASS (126/126 stock names and wire tags; five empty list tables/eleven null targets; 49 modules + six populated owners; one Core/{instance.ReferencedInstances.Count} occurrences/one Tracker; BIN/RELO/IMP={output.InstanceBuffer.Length}/{output.RelocationBuffer.Length}/{output.ImportsBuffer.Length}, prefix={prefix}; {refusals} native/source/output refusals; original root values, full inheritance/publication and gameplay remain unproven).");
        //-------------------------------------------------------------------------------------------------
        /** Reborn: collect only direct root particles, never similarly named nested child declarations. */
        //-------------------------------------------------------------------------------------------------
        void Collect(XmlSchemaParticle particle){if(particle is XmlSchemaElement e){if(!declarations.TryAdd(e.Name!,e))throw new InvalidDataException("Ambiguous root declaration.");}else if(particle is XmlSchemaGroupBase group)foreach(XmlSchemaParticle child in group.Items)Collect(child);else throw new InvalidDataException("Unreviewed root declaration particle.");}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: independent stock names/tags/empty marker targets and exact native inversion precede detaching actual composed 110-field metadata for its old oracle. */
        //-------------------------------------------------------------------------------------------------
        int Verify(Chunk result)
        {
            var b=result.InstanceBuffer;var rows=GameObjectStockReview.Tokens(b,0);int shift=rows[FastHash.GetHashCode("BuildTime")].Offset-140;BehaviorContainerStockProof.VerifyTail(result,native,shift);
            if(rows.Count!=126 || !rows.Keys.Order().SequenceEqual(stockRows.Keys.Order()) || rows.Any(p=>p.Value.Type!=stockRows[p.Key].Type))throw new InvalidDataException("Complete named root stock name/type set differs.");
            int first=rows[FastHash.GetHashCode("Browser")].Offset,table=first;
            foreach(var field in Fields)
            {
                var token=rows[FastHash.GetHashCode(field.Name)];var original=stockRows[FastHash.GetHashCode(field.Name)];
                if(!field.List){if(token.Offset!=0 || original.Offset!=0)throw new InvalidDataException("Null root metadata target differs.");continue;}
                if(token.Offset!=table || Word(b,table)!=Word(stock.Bytes,original.Offset) || Word(b,table+4)!=Word(stock.Bytes,original.Offset+4) || Word(stock.Bytes,original.Offset+8)!=stock.Projection.NativeRoot+field.Slot || Word(b,table+8)!=shift+field.Slot)throw new InvalidDataException("Empty root count marker/native target differs.");table+=12;
            }
            if(table!=shift)throw new InvalidDataException("Empty root metadata extent differs.");int insert=Find(b,FastHash.GetHashCode("Browser")),extra=192,basePrefix=first-extra;var old=new Chunk{InstanceBuffer=new byte[basePrefix+native.InstanceBuffer.Length]};
            b.AsSpan(0,insert).CopyTo(old.InstanceBuffer);b.AsSpan(insert+extra,basePrefix-insert).CopyTo(old.InstanceBuffer.AsSpan(insert));
            for(int at=8;at<basePrefix;at+=12){uint target=Word(old.InstanceBuffer,at);if(target!=0)Put(old.InstanceBuffer,at,target<first?target-(uint)(target>=insert+extra?extra:0):target>=shift && target<result.InstanceBuffer.Length?target-(uint)shift+(uint)basePrefix:throw new InvalidDataException("Detached base target crosses empty tables."));}
            // Reborn: reconstruct the independently checked native inverse for the old oracle, without regenerating its observed metadata.
            native.InstanceBuffer.CopyTo(old.InstanceBuffer,basePrefix);foreach(uint slot in Slots(native.RelocationBuffer))Put(old.InstanceBuffer,basePrefix+(int)slot,Word(native.InstanceBuffer,(int)slot)+(uint)basePrefix);old.RelocationBuffer=Pack(Slots(native.RelocationBuffer).Select(s=>s+(uint)basePrefix));old.ImportsBuffer=Pack(Slots(native.ImportsBuffer).Select(s=>s+(uint)basePrefix));if(verifyOwners(old,native,instance)!=basePrefix)throw new InvalidDataException("Detached original owner prefix differs.");return shift;
        }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: root row lookup refuses malformed name mutations rather than relying on a producer field index. */
    //-------------------------------------------------------------------------------------------------
    private static int Find(byte[] b,uint name){for(int at=0;at<4096;at+=12){if(Word(b,at)==name)return at;if(Word(b,at)==0 && Word(b,at+4)==0 && Word(b,at+8)==0)break;}throw new InvalidDataException("Empty root field absent.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: data/schema refusals must be explicit; a successful corrupt output is never counted as rejection. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action){try{action();}catch(InvalidDataException){return;}throw new InvalidDataException("Invalid empty root profile accepted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independent complete buffers protect input isolation and mutation controls. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c)=>new(){InstanceBuffer=c.InstanceBuffer.ToArray(),RelocationBuffer=c.RelocationBuffer.ToArray(),ImportsBuffer=c.ImportsBuffer.ToArray()};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: determinism includes every auxiliary byte and its order. */
    //-------------------------------------------------------------------------------------------------
    private static bool Equal(Chunk a,Chunk b)=>a.InstanceBuffer.SequenceEqual(b.InstanceBuffer) && a.RelocationBuffer.SequenceEqual(b.RelocationBuffer) && a.ImportsBuffer.SequenceEqual(b.ImportsBuffer);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independent auxiliary framing rejects duplicate slots and missing sentinels. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b){if(b.Length==0)return Array.Empty<uint>();if(b.Length<8 || b.Length%4!=0 || Word(b,b.Length-4)!=uint.MaxValue)throw new InvalidDataException("Empty root auxiliary framing.");var slots=Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();if(slots.Distinct().Count()!=slots.Length)throw new InvalidDataException("Duplicate empty root auxiliary slot.");return slots;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: coordinate inversion preserves the independent native auxiliary order and empty stream form. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> values){var slots=values.ToArray();if(slots.Length==0)return Array.Empty<byte>();var b=new byte[(slots.Length+1)*4];for(int i=0;i<slots.Length;i++)Put(b,i*4,slots[i]);Put(b,slots.Length*4,uint.MaxValue);return b;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: bounded little-endian reads do not dump unrelated game binary content. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at){if(at<0 || at>b.Length-4)throw new InvalidDataException("Empty root word bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: patch only owned output/mutation arrays. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
