using System.Buffers.Binary;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: the remaining timeout scalar is an unsigned logic-frame duration, not a raw millisecond/float word despite its MS source name.
internal static class GameObjectDurationSmokeTest
{
    private const string Field="EvaEventSecondDamageFarFromFirstTimeoutMS";
    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify independent stock/default semantics, integer millisecond rounding and full shared Core graph metadata without touching other Duration consumers. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string globalManifest)
    {
        var saved=Settings.Current;
        try
        {
            XmlSchemaSet? official=null;SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:s=>official=s);if(official==null)throw new InvalidDataException("Reviewed duration schema required.");
            foreach(string name in new[]{"AlliedConstructionYard","SovietAntiInfantryInfantry"}){var stock=GameObjectStockReview.Capture(manifest,"GameObject:"+name);var stockRow=GameObjectStockReview.Tokens(stock.Bytes,0)[FastHash.GetHashCode(Field)];if(stockRow.Type!=0x6F6D1AE7 || stockRow.Offset!=stock.Projection.NativeRoot+396 || Word(stock.Bytes,stockRow.Offset)!=155)throw new InvalidDataException("Independent stock timeout duration differs.");}
            Settings.Current=new Settings{SchemaPath=Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"RootDurationPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};var schemas=new SchemaSet(false);
            // Reborn: these source attributes are unqualified even though their named simple types belong to the asset namespace.
            var actual=(XmlSchemaAttribute)((XmlSchemaComplexType)official.GlobalTypes[new XmlQualifiedName("GameObject",SchemaSet.XmlNamespace)]!).AttributeUses[new XmlQualifiedName(Field)]!;var focused=(XmlSchemaAttribute)((XmlSchemaComplexType)schemas.Schemas.GlobalTypes[new XmlQualifiedName("RootDurationProbeType",SchemaSet.XmlNamespace)]!).AttributeUses[new XmlQualifiedName(Field)]!;
            if(actual.DefaultValue!="31000" || actual.AttributeSchemaType!.QualifiedName!=new XmlQualifiedName("DurationUnsignedInt",SchemaSet.XmlNamespace) || actual.DefaultValue!=focused.DefaultValue || actual.AttributeSchemaType.QualifiedName!=focused.AttributeSchemaType!.QualifiedName || actual.Use!=focused.Use)throw new InvalidDataException("Official timeout type/default drift.");
            var catalog=new[]{Catalog(manifest),Catalog(globalManifest)}.SelectMany(d=>d.Assets).ToArray();var targets=new[]{"UnitTypeIcon","UnitIntro","ArmorTemplate","DamageFX","FXList","AttributeModifier","WeaponTemplate","UpgradeTemplate","LocomotorTemplate"}.Select(t=>catalog.First(a=>a.TypeId==FastHash.GetHashCode(t))).Concat(new[]{catalog.Single(a=>a.Name=="TargetingCompareList:DefaultTargetingCompareList")}).ToArray();
            string baseline=GameObjectAiCompositionSmokeTest.Source(targets,15,2,2,false,0).Replace("RootAiProbe","RootDurationProbe",StringComparison.Ordinal);int cases=0;Chunk? populated=null;InstanceDeclaration? populatedInstance=null;
            // Reborn: maximum-input expectation pins current float32 frame conversion, not independent EA edge-rounding parity beyond the observed default.
            foreach(var sample in new (string? Milliseconds,uint Frames)[]{(null,155),("0",0),("1",1),("199",1),("200",1),("201",2),("1000",5),("31000",155),("1000000",5000),("4294967295",21474836)})
            {
                string source=Source(sample.Milliseconds);var pair=Compile(source);var output=Ep1GameObjectDestructionProjection.Compose(pair.Native);Verify(output,pair.Native,pair.Instance,sample.Frames);var repeated=Compile(source);var other=Ep1GameObjectDestructionProjection.Compose(repeated.Native);if(!output.InstanceBuffer.SequenceEqual(other.InstanceBuffer) || !output.RelocationBuffer.SequenceEqual(other.RelocationBuffer) || !output.ImportsBuffer.SequenceEqual(other.ImportsBuffer))throw new InvalidDataException("Duration repeat differs.");if(sample.Milliseconds==null){populated=pair.Native;populatedInstance=pair.Instance;if(pair.Instance.XmlNode.Attributes?[Field]?.Value!="31000")throw new InvalidDataException("Core timeout default was not inserted.");}cases++;
            }
            // Reborn: the direct native fallback must also match stock when Core default insertion is deliberately bypassed at this one field.
            var adapted=GameObjectCombatCompositionSmokeTest.Adapt(populatedInstance!.XmlNode,true);adapted.DocumentElement!.RemoveAttribute(Field);var fallback=GameObjectBodyProjectionSmokeTest.Compile(adapted,selectedBehaviors:true);if(Word(fallback.InstanceBuffer,396)!=155)throw new InvalidDataException("Direct timeout fallback differs from official/stock default.");
            int refusals=0;foreach(string bad in new[]{"-1","4294967296","1.5","31s","NaN","=MissingDuration"}){Refuse(()=>Compile(Source(bad)));refusals++;}
            foreach(bool relocation in new[]{false,true}){var bad=Clone(populated!);if(relocation)bad.RelocationBuffer=Pack(Slots(bad.RelocationBuffer).Append(396u));else bad.ImportsBuffer=Pack(Slots(bad.ImportsBuffer).Append(396u));Refuse(()=>Ep1GameObjectDestructionProjection.Compose(bad));refusals++;}
            var good=Ep1GameObjectDestructionProjection.Compose(populated!);int row=FindRow(good.InstanceBuffer);foreach(int delta in new[]{4,8}){var bad=Clone(good);Put(bad.InstanceBuffer,row+delta,Word(bad.InstanceBuffer,row+delta)^4u);Refuse(()=>Verify(bad,populated!,populatedInstance,155));refusals++;}var rawMilliseconds=Clone(good);int prefix=GameObjectStockReview.Tokens(good.InstanceBuffer,0)[FastHash.GetHashCode("BuildTime")].Offset-140;Put(rawMilliseconds.InstanceBuffer,prefix+396,31000);Refuse(()=>Verify(rawMilliseconds,populated!,populatedInstance,155));refusals++;
            Console.WriteLine($"GameObject duration: PASS (two independent stock words/tags, official/Core/direct default 31000ms=155 frames; {cases} full shared AI/root source cases, zero/ceil/uint edge literals, {refusals} source/slot/detached refusals; {Ep1GameObjectRootValueTokens.AllFields.Count} flat descriptors, up to {Ep1GameObjectRootValueTokens.AllFields.Count+11} partial root fields; current float32 edge policy, no general object publication/game).");
            //-------------------------------------------------------------------------------------------------
            /** Reborn: positive and negative sources differ only at the one reviewed duration attribute. */
            //-------------------------------------------------------------------------------------------------
            string Source(string? milliseconds){if(milliseconds==null)return baseline;var d=Xml(baseline);d.DocumentElement!.SetAttribute(Field,milliseconds);return d.OuterXml;}
            //-------------------------------------------------------------------------------------------------
            /** Reborn: raw official validation precedes actual Core default/reference normalization and independent target identity admission. */
            //-------------------------------------------------------------------------------------------------
            (Chunk Native,InstanceDeclaration Instance) Compile(string source)
            {
                var raw=Xml(source);GameObjectBodyProjectionSmokeTest.Validate(official,GameObjectCombatCompositionSmokeTest.Adapt(raw.DocumentElement!,true).OuterXml);var instance=ReferencePipelineSmokeTest.Normalize(schemas,source);foreach(var r in instance.ReferencedInstances)if(catalog.Count(a=>a.TypeId==r.TypeId && a.InstanceId==r.InstanceId)!=1)throw new InvalidDataException("Duration target missing/ambiguous.");return(GameObjectBodyProjectionSmokeTest.Compile(GameObjectCombatCompositionSmokeTest.Adapt(instance.XmlNode,true),selectedBehaviors:true),instance);
            }
            //-------------------------------------------------------------------------------------------------
            /** Reborn: all prior graph checks remain active, while the duration's primitive tag/native address/value are pinned independently. */
            //-------------------------------------------------------------------------------------------------
            void Verify(Chunk output,Chunk native,InstanceDeclaration instance,uint frames)
            {
                GameObjectAiCompositionSmokeTest.Verify(output,native,instance,targets,15,2,2);var rows=GameObjectStockReview.Tokens(output.InstanceBuffer,0);int prefix=rows[FastHash.GetHashCode("BuildTime")].Offset-140;var field=rows[FastHash.GetHashCode(Field)];if(field.Type!=0x6F6D1AE7 || field.Offset!=prefix+396 || Word(output.InstanceBuffer,field.Offset)!=frames || Word(native.InstanceBuffer,396)!=frames || Slots(native.RelocationBuffer).Contains(396u) || Slots(native.ImportsBuffer).Contains(396u))throw new InvalidDataException("Duration primitive/native/auxiliary contract differs.");
            }
        }
        finally{Settings.Current=saved;}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: linked target catalogs must retain the reviewed EP1 type profile and unique identities. */
    //-------------------------------------------------------------------------------------------------
    private static ManifestDocument Catalog(string path){var d=ManifestReader.Read(SdkEnvironmentPreflight.Read(path,64*1048576));TypeRegistryAudit.ValidateTarget(d.Header.Version,d.Header.AllTypesHash);if(!d.Header.IsLinked || d.Validate().Count!=0)throw new InvalidDataException("Duration catalog invalid.");return d;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: find the actual root metadata row without relying on a fixed scalar descriptor ordinal. */
    //-------------------------------------------------------------------------------------------------
    private static int FindRow(byte[] b){for(int at=0;at<b.Length-12;at+=12){if(Word(b,at)==FastHash.GetHashCode(Field))return at;if(Word(b,at)==0 && Word(b,at+4)==0 && Word(b,at+8)==0)break;}throw new InvalidDataException("Duration root field missing.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: malformed literal/schema/native/metadata inputs cannot become silently defaulted output. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuse(Action action){try{action();}catch(Exception e)when(e is InvalidDataException or OverflowException or FormatException){return;}throw new InvalidDataException("Invalid duration admitted.");}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: controlled XML input does not resolve external entities. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Xml(string text){var d=new XmlDocument{XmlResolver=null};d.LoadXml(text);return d;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: every corruption control owns its mutation buffers. */
    //-------------------------------------------------------------------------------------------------
    private static Chunk Clone(Chunk c)=>new(){InstanceBuffer=c.InstanceBuffer.ToArray(),RelocationBuffer=c.RelocationBuffer.ToArray(),ImportsBuffer=c.ImportsBuffer.ToArray()};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: read empty or sentinel-framed auxiliary word arrays independently. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] b){if(b.Length==0)return Array.Empty<uint>();if(b.Length<8 || b.Length%4!=0 || Word(b,b.Length-4)!=uint.MaxValue)throw new InvalidDataException("Duration auxiliary framing differs.");return Enumerable.Range(0,b.Length/4-1).Select(i=>Word(b,i*4)).ToArray();}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: mutation slots preserve order and one trailing sentinel. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> slots){var a=slots.ToArray();if(a.Length==0)return Array.Empty<byte>();var b=new byte[(a.Length+1)*4];for(int i=0;i<a.Length;i++)Put(b,i*4,a[i]);Put(b,a.Length*4,uint.MaxValue);return b;}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: metadata/native words stay inside bounded little-endian buffers. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at){if(at<0 || at>b.Length-4)throw new InvalidDataException("Duration word bounds exceeded.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: only private expected/mutation arrays receive word writes. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] b,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),value);
}
