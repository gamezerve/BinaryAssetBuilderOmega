using System.Buffers.Binary;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: independently qualify the six original populated owners and all 49 original modules on one selected root/Core/Tracker, without claiming complete original root values.
internal static class GameObjectRootOwnersSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: actual original owner sources join the already qualified module source before a single normalization/allocation pass. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(SchemaSet schemas,XmlDocument moduleSource,XmlElement[] objects,XmlDocument definitions,string manifest,ManifestAsset[] catalog,Func<Chunk,Chunk,InstanceDeclaration,int> verifyModules)
    {
        var stock=GameObjectStockReview.Capture(manifest,"GameObject:SovietAntiInfantryInfantry");var fields=Ep1GameObjectRootOwnersProjection.Fields;int[] lengths={84,336,840,660,228,264};var source=(XmlDocument)moduleSource.CloneNode(true);
        string account=definitions.GetElementsByTagName("Define",SchemaSet.XmlNamespace).OfType<XmlElement>().Single(e=>e.GetAttribute("name")=="ACCOUNT_ORE").GetAttribute("value");if(account!="0")throw new InvalidDataException("Combined original account changed.");
        foreach(var field in fields){var owner=objects.SelectMany(o=>o.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName==field.Name)).Last();var copy=(XmlElement)source.ImportNode(owner,true);if(field.Name=="ObjectResourceInfo")foreach(XmlElement cost in copy.GetElementsByTagName("BuildCost",SchemaSet.XmlNamespace))if(cost.GetAttribute("Account")=="=$ACCOUNT_ORE")cost.SetAttribute("Account",account);source.DocumentElement!.AppendChild(copy);}
        // Reborn: order only the direct root children by the admitted official compiled sequence; nested source order stays intact.
        var rootType=(XmlSchemaComplexType)((XmlSchemaElement)schemas.Schemas.GlobalElements[new XmlQualifiedName("GameObject",SchemaSet.XmlNamespace)]!).ElementSchemaType!;var order=new List<string>();Order(rootType.ContentTypeParticle);foreach(var child in source.DocumentElement!.ChildNodes.OfType<XmlElement>().OrderBy(e=>order.IndexOf(e.LocalName)).ToArray())source.DocumentElement.AppendChild(child);
        var pair=Build(source);var before=Clone(pair.Native);var output=Ep1GameObjectRootOwnersProjection.Compose(pair.Native,schemas.Schemas);int prefix=Verify(output,pair.Native,pair.Instance);var repeat=Build(source);if(!Equal(pair.Native,before) || !Equal(pair.Native,repeat.Native) || !Equal(output,Ep1GameObjectRootOwnersProjection.Compose(repeat.Native,schemas.Schemas)))throw new InvalidDataException("Combined root determinism/input isolation differs.");int refusals=0;
        Refuse(()=>Ep1GameObjectDestructionProjection.Compose(pair.Native,schemas.Schemas));refusals++;
        foreach(var field in fields)
        {
            foreach(uint target in new[]{0u,Word(pair.Native.InstanceBuffer,476),Word(pair.Native.InstanceBuffer,field.Slot)+4}){var bad=Clone(pair.Native);Put(bad.InstanceBuffer,field.Slot,target);Refuse(()=>Ep1GameObjectRootOwnersProjection.Compose(bad,schemas.Schemas));refusals++;}
            int row=Row(output.InstanceBuffer,FastHash.GetHashCode(field.Name));foreach(int at in new[]{row+4,row+8,(int)Word(output.InstanceBuffer,row+8)+4,prefix+field.Slot}){var bad=Clone(output);bad.InstanceBuffer[at]^=1;Refuse(()=>Verify(bad,pair.Native,pair.Instance));refusals++;}
        }
        foreach(bool relo in new[]{true,false}){var bad=Clone(pair.Native);if(relo)bad.RelocationBuffer=Array.Empty<byte>();else bad.ImportsBuffer=Array.Empty<byte>();Refuse(()=>Ep1GameObjectRootOwnersProjection.Compose(bad,schemas.Schemas));refusals++;}
        var orphan=Clone(pair.Native);orphan.InstanceBuffer=orphan.InstanceBuffer.Concat(new byte[]{1,0,0,0}).ToArray();Refuse(()=>Ep1GameObjectRootOwnersProjection.Compose(orphan,schemas.Schemas));refusals++;
        Console.WriteLine($"Combined Conscript root owners: PASS (49 modules + six full native/metadata owners; one Core/{pair.Instance.ReferencedInstances.Count} occurrences/one Tracker; 110 selected partial-root fields; BIN/RELO/IMP={output.InstanceBuffer.Length}/{output.RelocationBuffer.Length}/{output.ImportsBuffer.Length}, prefix={prefix}; {refusals} scope/ownership/output refusals; no complete original root, playable publication, game or alpha expansion).");
        //-------------------------------------------------------------------------------------------------
        /** Reborn: schema/identity checks and concrete stock dependency resolution precede one actual Core normalization. */
        //-------------------------------------------------------------------------------------------------
        (Chunk Native,InstanceDeclaration Instance) Build(XmlDocument input){GameObjectBodyProjectionSmokeTest.Validate(schemas.Schemas,input.OuterXml);var raw=(XmlDocument)input.CloneNode(true);raw.Schemas.Add(schemas.Schemas);raw.Validate((_,e)=>throw new InvalidDataException(e.Message));Ep1SelectedModuleIdentity.ValidateSource(raw.DocumentElement!,true);ConscriptBehaviorSurvey.ResolveStrong(raw,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,raw.OuterXml);foreach(var reference in instance.ReferencedInstances)if(catalog.Count(a=>a.TypeId==reference.TypeId && a.InstanceId==reference.InstanceId)!=1)throw new InvalidDataException("Combined root unresolved dependency.");var normalized=new XmlDocument{XmlResolver=null};normalized.LoadXml(instance.XmlNode.OuterXml);return(GameObjectBodyProjectionSmokeTest.Compile(normalized,true,true,true),instance);}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: compare real composed owner metadata to stock, then independently detach the preexisting metadata for its full 49-module oracle rather than regenerate it. */
        //-------------------------------------------------------------------------------------------------
        int Verify(Chunk result,Chunk native,InstanceDeclaration instance)
        {
            var b=result.InstanceBuffer;var n=native.InstanceBuffer;var root=GameObjectStockReview.Tokens(b,0);var original=GameObjectStockReview.Tokens(stock.Bytes,0);int shift=root[FastHash.GetHashCode("BuildTime")].Offset-140;if(root.Count!=110)throw new InvalidDataException("Combined root field inventory differs.");BehaviorContainerStockProof.VerifyTail(result,native,shift);
            if(!Slots(native.ImportsBuffer).Select(s=>Word(n,(int)s)).Order().SequenceEqual(Enumerable.Range(1,instance.ReferencedInstances.Count).Select(i=>(uint)i)))throw new InvalidDataException("Combined root Core occurrences not consumed once.");
            int cut=(int)Word(n,416),tableEnd=0;
            for(int i=0;i<fields.Count;i++)
            {
                var field=fields[i];int start=(int)Word(n,field.Slot),end=i+1<fields.Count?(int)Word(n,fields[i+1].Slot):n.Length;var token=root[FastHash.GetHashCode(field.Name)];var stockToken=original[FastHash.GetHashCode(field.Name)];if(token.Type!=stockToken.Type)throw new InvalidDataException("Combined root owner wire tag differs: "+field.Name);var leaf=Extract(native,start,end);var comparer=new SelectedNativeGraphComparer(stock.Bytes,stock.Asset.References.ToArray(),leaf,instance,requireAllReferences:false,captureNativeCoordinates:false,captureNativePaths:true);comparer.Compare(field.NativeType,(int)Word(stock.Bytes,stock.Projection.NativeRoot+field.Slot));
                var detached=Extract(result,shift+start,shift+end,lengths[i]);b.AsSpan(token.Offset,lengths[i]).CopyTo(detached.InstanceBuffer);for(int at=8;at<lengths[i];at+=12){uint target=Word(detached.InstanceBuffer,at);if(target!=0)Put(detached.InstanceBuffer,at,target>=token.Offset && target<token.Offset+lengths[i]?target-(uint)token.Offset:target>=shift+start && target<shift+end?target-(uint)(shift+start)+(uint)lengths[i]:throw new InvalidDataException("Combined owner metadata crosses boundaries."));}
                int stockEnd=original.Values.Select(t=>t.Offset).Where(at=>at>stockToken.Offset && at<stock.Projection.NativeRoot).DefaultIfEmpty(stock.Projection.NativeRoot).Min();BehaviorMetadataStockProof.Compare(detached,leaf,stock.Bytes,stockToken.Offset,stockEnd,field.NativeType,comparer.NativeRecords);
                if(i>0 && token.Offset!=tableEnd)throw new InvalidDataException("Combined owner tables not contiguous.");tableEnd=token.Offset+lengths[i];
            }
            if(tableEnd!=shift)throw new InvalidDataException("Combined final owner metadata extent differs.");
            int insertion=Row(b,FastHash.GetHashCode(fields[0].Name)),extra=72,basePrefix=root[FastHash.GetHashCode(fields[0].Name)].Offset-extra;var basis=Extract(native,0,cut);foreach(var field in fields)Put(basis.InstanceBuffer,field.Slot,0);basis.RelocationBuffer=Pack(Slots(basis.RelocationBuffer).Where(s=>!fields.Any(f=>f.Slot==s)));
            var old=new Chunk{InstanceBuffer=new byte[basePrefix+cut]};b.AsSpan(0,insertion).CopyTo(old.InstanceBuffer);b.AsSpan(insertion+extra,basePrefix-insertion).CopyTo(old.InstanceBuffer.AsSpan(insertion));for(int at=8;at<basePrefix;at+=12){uint target=Word(old.InstanceBuffer,at);if(target!=0)Put(old.InstanceBuffer,at,target<basePrefix+extra?target-(uint)(target>=insertion+extra?extra:0):target>=shift && target<shift+cut?target-(uint)shift+(uint)basePrefix:throw new InvalidDataException("Base metadata crosses appended owner boundary."));}
            basis.InstanceBuffer.CopyTo(old.InstanceBuffer,basePrefix);foreach(uint slot in Slots(basis.RelocationBuffer))Put(old.InstanceBuffer,basePrefix+(int)slot,Word(basis.InstanceBuffer,(int)slot)+(uint)basePrefix);old.RelocationBuffer=Pack(Slots(basis.RelocationBuffer).Select(s=>s+(uint)basePrefix));old.ImportsBuffer=Pack(Slots(basis.ImportsBuffer).Select(s=>s+(uint)basePrefix));if(verifyModules(old,basis,instance)!=49)throw new InvalidDataException("Combined original module inventory differs.");return shift;
        }
        //-------------------------------------------------------------------------------------------------
        /** Reborn: only the official root sequence/choice element declarations determine direct source ordering. */
        //-------------------------------------------------------------------------------------------------
        void Order(XmlSchemaParticle particle){if(particle is XmlSchemaElement e)order.Add(e.Name!);else if(particle is XmlSchemaGroupBase group)foreach(XmlSchemaParticle child in group.Items)Order(child);else throw new InvalidDataException("Unreviewed compiled root source particle.");}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: extract one native interval and optionally reserve a metadata prefix; imports retain their shared Core indices. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Extract(Chunk source,int start,int end,int prefix=0)
    {
        if(start<0 || end<=start || end>source.InstanceBuffer.Length)throw new InvalidDataException("Combined extraction bounds.");
        var result=new Chunk{InstanceBuffer=new byte[prefix+end-start],RelocationBuffer=Pack(Slots(source.RelocationBuffer).Where(s=>s>=start && s<end).Select(s=>s-(uint)start+(uint)prefix)),ImportsBuffer=Pack(Slots(source.ImportsBuffer).Where(s=>s>=start && s<end).Select(s=>s-(uint)start+(uint)prefix))};source.InstanceBuffer.AsSpan(start,end-start).CopyTo(result.InstanceBuffer.AsSpan(prefix));
        foreach(uint slot in Slots(result.RelocationBuffer))
        {
            uint target=Word(result.InstanceBuffer,(int)slot);
            // Reborn: only the six root pointers deliberately removed from the detached base may cross its boundary; no other base allocation gets a blanket exemption.
            if(target<start || target>=end){if(start==0 && prefix==0 && slot is 416 or 468 or 532 or 540 or 552 or 564)continue;throw new InvalidDataException("Combined native pointer crosses owner.");}
            Put(result.InstanceBuffer,(int)slot,target-(uint)start+(uint)prefix);
        }
        return result;
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: root row lookup is independent of production descriptor serialization. */
    //-------------------------------------------------------------------------------------------------
    private static int Row(byte[] b,uint name){for(int at=0;at<4096;at+=12){if(Word(b,at)==name)return at;if(Word(b,at)==0 && Word(b,at+4)==0 && Word(b,at+8)==0)break;}throw new InvalidDataException("Combined root row absent.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: deliberate corruptions require explicit refusal, never an exception swallowed as a passing row. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action){try{action();}catch(InvalidDataException){return;}throw new InvalidDataException("Invalid combined root accepted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: repeat and mutation scenarios own their three byte arrays. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c)=>new(){InstanceBuffer=c.InstanceBuffer.ToArray(),RelocationBuffer=c.RelocationBuffer.ToArray(),ImportsBuffer=c.ImportsBuffer.ToArray()};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: deterministic equality includes auxiliary order, not just native bytes. */
    //-------------------------------------------------------------------------------------------------
    private static bool Equal(Chunk a,Chunk b)=>a.InstanceBuffer.SequenceEqual(b.InstanceBuffer) && a.RelocationBuffer.SequenceEqual(b.RelocationBuffer) && a.ImportsBuffer.SequenceEqual(b.ImportsBuffer);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independent framing checks reject malformed, duplicate and truncated auxiliary slot arrays. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b){if(b.Length==0)return Array.Empty<uint>();if(b.Length<8 || b.Length%4!=0 || Word(b,b.Length-4)!=uint.MaxValue)throw new InvalidDataException("Combined auxiliary framing.");var slots=Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();if(slots.Distinct().Count()!=slots.Length)throw new InvalidDataException("Combined duplicate auxiliary.");return slots;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently retain empty streams and one final nonempty sentinel. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> values){var slots=values.ToArray();if(slots.Length==0)return Array.Empty<byte>();var result=new byte[(slots.Length+1)*4];for(int i=0;i<slots.Length;i++)Put(result,i*4,slots[i]);Put(result,slots.Length*4,uint.MaxValue);return result;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: all stock, native and metadata reads remain bounded. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at){if(at<0 || at>b.Length-4)throw new InvalidDataException("Combined word bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: patch only owned little-endian test buffers. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
