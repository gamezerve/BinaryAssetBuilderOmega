using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: EP1 production scalars and optional upgrade translations are checked against real refinery/Barracks native and metadata witnesses.
internal static class ProductionUpdateContract
{
    internal const int NativeSize=84;
    internal const int MetadataSize=408;
    private const uint TypeId=0x6EC75439u;


    //-------------------------------------------------------------------------------------------------
    /** Reborn: the source/native proof owns the weak bonus ID, modifier lists and upgrade translations. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlElement actual,XmlElement model)
    {
        actual.RemoveAttribute("BonusForType");
        if (model.HasAttribute("BonusForType"))
            actual.SetAttribute("BonusForType", model.GetAttribute("BonusForType"));
        foreach(var child in actual.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName is "GiveUpgradeToProduced" or "QuantityModifier" or "ProductionModifier").ToArray())actual.RemoveChild(child);
        foreach(var child in model.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName is "GiveUpgradeToProduced" or "QuantityModifier" or "ProductionModifier"))actual.AppendChild(actual.OwnerDocument!.ImportNode(child,true));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify both omitted refinery storage and the Barracks' actual three-row upgrade graph, including native imports and nested stock metadata. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar QualifyGraph(string manifest,XmlDocument baseline,SchemaSet schemas,ManifestAsset[] catalog)
    {
        _=QualifyStock(manifest);var grammar=new StockMetadataGrammar();
        foreach(string name in new[]{"SovietRefinery","SovietBarracks"}){
            var captured=GameObjectStockReview.Capture(manifest,"GameObject:"+name);var module=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.Type=="ProductionUpdateModuleData");
            var original=GameObjectExtendedRootContract.Original("Soviet/Structures/"+name+".xml","ProductionUpdate").Single();var probe=(XmlDocument)baseline.CloneNode(true);var list=probe.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();list.RemoveAll();list.AppendChild(probe.ImportNode(original,true));
            var resolved=SchemaNativeContract.Resolve(probe,schemas,catalog);var owner=resolved.GetElementsByTagName("ProductionUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();SchemaNativeContract.VerifyStock(owner,typeof(Ep1ProductionUpdateModuleData),TypeId,captured.Bytes,module.Native,captured.Asset.References.ToArray());
            grammar.Learn(captured.Bytes,module.Wrapper,captured.Projection.NativeRoot,typeof(Ep1ProductionUpdateModuleData));
            // Reborn: print only the newly recovered list wire signature, never unrelated stock payloads.
            if(name=="SovietBarracks"){int table=(int)Word(captured.Bytes,module.Wrapper+8),give=GameObjectStockReview.Tokens(captured.Bytes,table)[FastHash.GetHashCode("GiveUpgradeToProduced")].Offset;var translation=GameObjectStockReview.Tokens(captured.Bytes,give)[FastHash.GetHashCode("UpgradeTranslate")];Console.WriteLine("Reborn: production UpgradeTranslate stock signature="+translation.Type.ToString("X8")+".");}
        }
        // Reborn: the EA populated witness independently identifies both quantity metadata rows.
        grammar.QualifyQuantityModifier(schemas.Schemas);
        // Reborn: the retail modifier loader calls the same inline filter reader used by stock walls.
        var wall = GameObjectStockReview.Capture(manifest, "GameObject:SovietWallPiece");
        var wallModule = GameObjectModuleReview.Inspect(manifest, wall.Asset.Name).Containers
            .Single(container => container.Name == "Behaviors").Modules.Single(module => module.TypeId == WallBuilderContract.TypeId);
        int wallFields = checked((int)Word(wall.Bytes, wallModule.Wrapper + 8));
        int filterFields = GameObjectStockReview.Tokens(wall.Bytes, wallFields)[FastHash.GetHashCode("ConnectingObjectFilter")].Offset;
        grammar.Learn(wall.Bytes, filterFields, wall.Projection.NativeRoot, typeof(ObjectFilter));
        grammar.QualifyProductionModifier(schemas.Schemas);
        return grammar;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require the exact stock refinery ABI, inheritance chain, literal metadata rows and its nondefault authored values. */
    //-------------------------------------------------------------------------------------------------
    internal static bool QualifyStock(string manifest)
    {
        var captured=GameObjectStockReview.Capture(manifest,"GameObject:SovietRefinery");var modules=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules;
        var module=modules.Single(m=>m.Type=="ProductionUpdateModuleData");int index=Array.IndexOf(modules,module);
        if(Marshal.SizeOf<Ep1ProductionUpdateModuleData>()!=NativeSize||module.TypeId!=TypeId||!module.ClassChain.SequenceEqual(new[]{"ProductionUpdateModuleData","UpdateModuleData","ContainModuleData","BehaviorModuleData","ModuleData"}))throw new InvalidDataException("EP1 production stock ABI/inheritance differs.");
        if(index+1<modules.Length&&(modules[index+1].Native-module.Native!=NativeSize||modules[index+1].Wrapper-module.Wrapper!=MetadataSize))throw new InvalidDataException("EP1 production stock native/metadata extent differs.");
        var b=captured.Bytes;var expected=Metadata();var actual=b.AsSpan(module.Wrapper,MetadataSize).ToArray();
        for(int at=8;at<MetadataSize;at+=12){uint target=Word(actual,at);if(target!=0)BinaryPrimitives.WriteUInt32LittleEndian(actual.AsSpan(at,4),target>=module.Native?target-(uint)module.Native+MetadataSize:target-(uint)module.Wrapper);}
        if(!actual.SequenceEqual(expected))throw new InvalidDataException("EP1 production literal metadata/native field anchors differ from stock.");
        var sample=new XmlDocument();sample.LoadXml("<ProductionUpdate id='ModuleTag_ProductionUpdate' GiveNoXP='true' Type='VEHICLE' NumDoorAnimations='1' DoorOpeningTime='0.66s' DoorWaitOpenTime='3.00s' DoorCloseTime='0.66s' NewObjectFadeInTime='0.5s'/>");
        CheckValues(sample.DocumentElement!,b.AsSpan(module.Native,NativeSize).ToArray());return true;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: all 84 native bytes and 408 metadata bytes are owned explicitly; this module has no imports or allocations in its admitted form. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlElement owner,Chunk native,Chunk metadata)
    {
        var b=native.InstanceBuffer;
        if(b.Length!=NativeSize||Word(b,0)!=TypeId||Word(b,4)!=FastHash.GetHashCode(owner.GetAttribute("id").ToLowerInvariant())||native.RelocationBuffer.Length!=0||native.ImportsBuffer.Length!=0)throw new InvalidDataException("EP1 production identity/native allocation/auxiliary framing differs.");
        CheckValues(owner,b);if(metadata.InstanceBuffer.Length-b.Length!=MetadataSize||!metadata.InstanceBuffer.AsSpan(0,MetadataSize).SequenceEqual(Metadata()))throw new InvalidDataException("EP1 production generated/literal metadata differ.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: check the weak bonus ID and scalars, with empty lists and translation storage for the fixed stock fixture. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckValues(XmlElement owner,byte[] b)
    {
        foreach(var binding in ConscriptModuleScalarContract.Bindings(typeof(Ep1ProductionUpdateModuleData)))if(!b.AsSpan(binding.Offset,binding.Width).SequenceEqual(ConscriptModuleScalarContract.Bytes(owner,binding)))throw new InvalidDataException("EP1 production scalar differs: "+binding.Name);
        // Reborn: without a reviewed DisabledType flag codec the disabled-type word is not authorable and must remain empty.
        bool flags=ConscriptModuleScalarContract.Bindings(typeof(Ep1ProductionUpdateModuleData)).Any(f=>f.Name=="DisabledTypesToProcess");
        if(!flags&&(Word(b,32)!=0||owner.GetAttribute("DisabledTypesToProcess").Trim().Length!=0))throw SdkXmlDiagnostics.At(new InvalidDataException("ProductionUpdate DisabledTypesToProcess has no reviewed EP1 flag codec."),(XmlNode?)owner.GetAttributeNode("DisabledTypesToProcess")??owner);
        var bonus = owner.GetAttribute("BonusForType").Trim();
        int colon = bonus.IndexOf(':');
        uint expectedBonus = bonus.Length == 0 ? 0 : InstanceHandle.GetInstanceId(bonus[(colon + 1)..]);
        if (Word(b, 44) != expectedBonus)
            throw new InvalidDataException("EP1 production weak bonus ID differs at +0x2c.");
        foreach(int at in new[]{60,64,68,72,76})if(Word(b,at)!=0)throw new InvalidDataException("EP1 production list/pointer storage is populated at +"+at);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: literal own/base metadata with schema-ordered rows, packed boolean targets and two empty list count tables after the base chain. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Metadata()
    {
        const int prefix=MetadataSize;var b=new byte[MetadataSize];Row(0,2,TypeId,12);Row(12,1,FastHash.GetHashCode("UpdateModuleData"),276);
        string[] names={"MaxQueueEntries","NumDoorAnimations","DoorOpeningTime","DoorWaitOpenTime","DoorCloseTime","ConstructionCompleteDuration","DisabledTypesToProcess","GiveNoXP","UnitInvulnerableTime","SpecialPrepModelconditionTime","VeteranUnitsFromVeteranFactory","BonusForType","SecondaryQueue","Type","IgnorePreReqs","ProductionTimeDelayScalar","NewObjectFadeInTime"};
        uint[] types={0x763177A4,0x23E650B8,0x998669E8,0x998669E8,0x998669E8,0x763177A4,0x196570C9,0x77E3F4F7,0x763177A4,0x763177A4,0x77E3F4F7,0x148ED75D,0x77E3F4F7,0xBC88486A,0x77E3F4F7,0x998669E8,0x998669E8};
        int[] offsets={8,12,16,20,24,28,32,80,36,40,81,44,82,48,83,52,56};
        for(int i=0;i<names.Length;i++)Row(24+i*12,FastHash.GetHashCode(names[i]),types[i],(uint)(prefix+offsets[i]));
        Row(228,FastHash.GetHashCode("QuantityModifier"),0x249DFB83u,384);Row(240,FastHash.GetHashCode("ProductionModifier"),0xEF83869Au,396);Row(252,FastHash.GetHashCode("GiveUpgradeToProduced"),0xB290DB31u,0);
        Row(276,1,FastHash.GetHashCode("ContainModuleData"),300);Row(300,1,FastHash.GetHashCode("BehaviorModuleData"),324);Row(324,1,FastHash.GetHashCode("ModuleData"),348);
        Row(348,FastHash.GetHashCode("TypeId"),0x23E650B8u,(uint)prefix);Row(360,FastHash.GetHashCode("id"),0xBDF07965u,(uint)(prefix+4));
        Row(384,0x6C827073u,0x5D448FD8u,(uint)(prefix+60));Row(396,0x6C827073u,0x5D448FD8u,(uint)(prefix+68));return b;
        // Reborn: metadata-internal targets and native targets have distinct coordinate spaces; neither becomes a relocation.
        void Row(int at,uint name,uint type,uint target){BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),name);BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at+4,4),type);BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at+8,4),target);}
    }
    // Reborn: callers prove fixed module/table bounds before reading words.
    private static uint Word(byte[] b,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));
}
