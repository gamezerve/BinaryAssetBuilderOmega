using System.Buffers.Binary;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
using Chunk=Relo.Chunk;
using Tracker=Relo.Tracker;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: independently pin fourteen official root defaults and distinguish Time seconds, float scalars and raw logic-frame integers.
internal static class GameObjectRootDefaultSmokeTest
{
    // Reborn: literal offsets/native words are independent of the marshaler and schema-default materialization.
    private static readonly (string Name,string Default,int Slot,uint Word)[] Fields={
        ("VoiceAttackChargeTimeout","150",240,150),("VoiceMoveLandToWaterTimeout","5s",244,Float(5)),("VoiceMoveWaterToLandTimeout","5s",248,Float(5)),
        ("VoiceSelectUnderFireTimeout","20s",252,Float(20)),("VoiceSelectUnderFireDamageTime","3s",256,Float(3)),("MaxDistanceForEngaged","150",260,Float(150)),
        ("EngagedStateTimeout","30",264,30),("CamouflageDetectorLevel","1",296,1),("PathPriority","25",304,25),("ReinvisibilityDelay","0.5s",372,Float(.5f)),
        ("InvisibilityOpacityMin","0.4",376,Float(.4f)),("InvisibilityOpacityMax","0.9",380,Float(.9f)),("SubGroupPriority","500",388,500),("EvaEventSecondDamageFarFromFirstScanRange","200",392,Float(200))};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: admit the same effective official schema without requiring installed game sources. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(){XmlSchemaSet? schemas=null;SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:s=>schemas=s);if(schemas==null)throw new InvalidDataException("Reviewed root default schema required.");Run(schemas);}
    //-------------------------------------------------------------------------------------------------
    /** Reborn: schema defaults, direct omitted-source compilation, explicit equivalent values and zero overrides must agree with independent native words. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(XmlSchemaSet schemas)
    {
        var root=(XmlSchemaComplexType)schemas.GlobalTypes[new XmlQualifiedName("GameObject",SchemaSet.XmlNamespace)]!;
        foreach(var field in Fields)if(root.AttributeUses[new XmlQualifiedName(field.Name)] is not XmlSchemaAttribute a || a.DefaultValue!=field.Default)throw new InvalidDataException("Official root default anchor changed: "+field.Name);
        var source=new XmlDocument{XmlResolver=null};source.LoadXml("<GameObject xmlns='uri:ea.com:eala:asset' id='RebornRootDefaults'><Body><ActiveBody id='RebornDefaultBody'/></Body></GameObject>");
        var omitted=Compile(source);foreach(var field in Fields)if(Word(omitted.InstanceBuffer,field.Slot)!=field.Word)throw new InvalidDataException("Omitted root default differs: "+field.Name);
        var explicitSource=(XmlDocument)source.CloneNode(true);foreach(var field in Fields)explicitSource.DocumentElement!.SetAttribute(field.Name,field.Default);var explicitNative=Compile(explicitSource);if(!Equal(omitted,explicitNative))throw new InvalidDataException("Explicit/omitted root defaults differ.");
        var zeroSource=(XmlDocument)source.CloneNode(true);foreach(var field in Fields)zeroSource.DocumentElement!.SetAttribute(field.Name,field.Default.EndsWith('s')?"0s":"0");var zero=Compile(zeroSource);foreach(var field in Fields)if(Word(zero.InstanceBuffer,field.Slot)!=0)throw new InvalidDataException("Explicit zero root override differs: "+field.Name);
        Console.WriteLine("GameObject root defaults: PASS (14 independent official/default native-word anchors; omitted/explicit-default/explicit-zero vectors; raw logic-frame integers remain distinct from Time seconds; root-only diagnostic allocation, no object publication).");
        //-------------------------------------------------------------------------------------------------
        /** Reborn: validate required Body before detaching that diagnostic owner; root marshaling receives no schema-injected default attributes. */
        //-------------------------------------------------------------------------------------------------
        unsafe Chunk Compile(XmlDocument raw)
        {
            GameObjectBodyProjectionSmokeTest.Validate(schemas,raw.OuterXml);var xml=(XmlDocument)raw.CloneNode(true);xml.DocumentElement!.RemoveChild(xml.GetElementsByTagName("Body",SchemaSet.XmlNamespace)[0]!);var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);GameObject* native;using var tracker=new Tracker((void**)&native,(uint)sizeof(GameObject),false);Marshaler.Marshal(new Node(xml.DocumentElement.CreateNavigator()!,ns),native,tracker);var output=new Chunk();if(!tracker.MakeRelocatable(output) || output.InstanceBuffer.Length!=600 || output.RelocationBuffer.Length!=0 || output.ImportsBuffer.Length!=0)throw new InvalidDataException("Root-only default diagnostic ownership differs.");return output;
        }
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: exact comparisons include every root and auxiliary byte. */
    //-------------------------------------------------------------------------------------------------
    private static bool Equal(Chunk a,Chunk b)=>a.InstanceBuffer.SequenceEqual(b.InstanceBuffer) && a.RelocationBuffer.SequenceEqual(b.RelocationBuffer) && a.ImportsBuffer.SequenceEqual(b.ImportsBuffer);
    //-------------------------------------------------------------------------------------------------
    /** Reborn: literal IEEE native expectations do not reuse marshal conversion helpers. */
    //-------------------------------------------------------------------------------------------------
    private static uint Float(float value)=>unchecked((uint)BitConverter.SingleToInt32Bits(value));
    //-------------------------------------------------------------------------------------------------
    /** Reborn: bounded reads keep default diagnostics inside their native root buffer. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] b,int at){if(at<0 || at>b.Length-4)throw new InvalidDataException("Root default word bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
}
