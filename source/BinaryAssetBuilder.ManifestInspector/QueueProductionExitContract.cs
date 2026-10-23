using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: source-bound exit scalars and two inline vectors are independently checked against stock ABI and literal metadata.
internal static class QueueProductionExitContract
{
    private static readonly string[] Vectors={"UnitCreatePoint","NaturalRallyPoint"};
    private static readonly string[] Axes={"x","y","z"};

    //-------------------------------------------------------------------------------------------------
    /** Reborn: required vectors and each finite/defaulted component retain their actual source location on invalid input. */
    //-------------------------------------------------------------------------------------------------
    internal static void Validate(XmlElement owner){foreach(string name in Vectors){var element=owner.ChildNodes.OfType<XmlElement>().SingleOrDefault(e=>e.LocalName==name)??throw SdkXmlDiagnostics.At(new InvalidDataException("QueueProductionExitUpdate requires "+name),owner);for(int axis=0;axis<3;axis++)_=ConscriptModuleScalarContract.Bytes(element,VectorBinding(axis));}}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: comparison copies mask only vector values; actual native output remains checked against all authored components. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlElement actual,XmlElement model){foreach(string name in Vectors){var element=actual.ChildNodes.OfType<XmlElement>().Single(e=>e.LocalName==name);var basis=model.ChildNodes.OfType<XmlElement>().Single(e=>e.LocalName==name);foreach(string axis in Axes){element.RemoveAttribute(axis);if(basis.HasAttribute(axis))element.SetAttribute(axis,basis.GetAttribute(axis));}}}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require exact native field offsets, Time wire type, inheritance tables and original nonzero vector payload from the stock refinery. */
    //-------------------------------------------------------------------------------------------------
    internal static bool QualifyStock(string manifest)
    {
        var captured=GameObjectStockReview.Capture(manifest,"GameObject:SovietRefinery");var module=GameObjectModuleReview.Inspect(manifest,captured.Asset.Name).Containers.Single(c=>c.Name=="Behaviors").Modules.Single(m=>m.Type=="QueueProductionExitUpdateModuleData");
        if(Marshal.SizeOf<Ep1QueueProductionExitUpdateModuleData>()!=48||module.TypeId!=0x9063FB6Cu||!module.ClassChain.SequenceEqual(new[]{"QueueProductionExitUpdateModuleData","UpdateModuleData","ContainModuleData","BehaviorModuleData","ModuleData"}))throw new InvalidDataException("EP1 exit stock ABI/inheritance differs.");
        var b=captured.Bytes;var expected=Metadata(252);var actual=b.AsSpan(module.Wrapper,252).ToArray();for(int at=8;at<252;at+=12){uint target=Word(actual,at);if(target!=0)BinaryPrimitives.WriteUInt32LittleEndian(actual.AsSpan(at,4),target>=module.Native?target-(uint)module.Native+252:target-(uint)module.Wrapper);}
        if(!actual.SequenceEqual(expected))throw new InvalidDataException("EP1 exit literal metadata/native field anchors differ from stock.");
        var sample=new XmlDocument();sample.LoadXml("<QueueProductionExitUpdate id='ModuleTag_Exit'><UnitCreatePoint x='37.5'/><NaturalRallyPoint x='75'/></QueueProductionExitUpdate>");
        CheckValues(sample.DocumentElement!,b.AsSpan(module.Native,48).ToArray());return true;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: all 48 native bytes and 252 metadata bytes are owned explicitly; this inline module cannot invent imports or allocations. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlElement owner,Chunk native,Chunk metadata)
    {
        Validate(owner);var b=native.InstanceBuffer;if(b.Length!=48||Word(b,0)!=0x9063FB6Cu||Word(b,4)!=FastHash.GetHashCode(owner.GetAttribute("id").ToLowerInvariant())||native.RelocationBuffer.Length!=0||native.ImportsBuffer.Length!=0)throw new InvalidDataException("EP1 exit identity/native allocation/auxiliary framing differs.");
        CheckValues(owner,b);if(metadata.InstanceBuffer.Length-b.Length!=252||!metadata.InstanceBuffer.AsSpan(0,252).SequenceEqual(Metadata(252)))throw new InvalidDataException("EP1 exit generated/literal metadata differ.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: share independently parsed engine-unit/packed-boolean codecs while checking both vector records component by component. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckValues(XmlElement owner,byte[] b)
    {
        foreach(var binding in ConscriptModuleScalarContract.Bindings(typeof(Ep1QueueProductionExitUpdateModuleData)))if(!b.AsSpan(binding.Offset,binding.Width).SequenceEqual(ConscriptModuleScalarContract.Bytes(owner,binding)))throw new InvalidDataException("EP1 exit scalar differs: "+binding.Name);
        for(int vector=0;vector<2;vector++){var element=owner.ChildNodes.OfType<XmlElement>().Single(e=>e.LocalName==Vectors[vector]);for(int axis=0;axis<3;axis++)if(!b.AsSpan(20+vector*12+axis*4,4).SequenceEqual(ConscriptModuleScalarContract.Bytes(element,VectorBinding(axis))))throw new InvalidDataException("EP1 exit vector differs: "+Vectors[vector]+"."+Axes[axis]);}
    }

    // Reborn: Vector3 schema components are SageReal/default 0.0; their inline signature is not a nested token table.
    private static ConscriptModuleScalarContract.Binding VectorBinding(int axis)=>new(Axes[axis],axis*4,4,"Single","0.0",0x645864F3u);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: literal stock-qualified own/base metadata retains byte-wide booleans, Time and direct Vector3 targets. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Metadata(int prefix)
    {
        var b=new byte[252];Row(0,2,0x9063FB6Cu,12);Row(12,1,FastHash.GetHashCode("UpdateModuleData"),144);
        string[] names={"PlacementViewAngle","ExitDelaySeconds","AllowAirborneCreation","InitialBurst","NoExitPath","CanRallyToSlaughter","ClearAlliesFromDestination","UnitCreatePoint","NaturalRallyPoint"};
        uint[] types={0x4E16EA4D,0x998669E8,0x77E3F4F7,0x763177A4,0x77E3F4F7,0x77E3F4F7,0x77E3F4F7,0x29368077,0x29368077};int[] offsets={8,12,44,16,45,46,47,20,32};
        for(int i=0;i<names.Length;i++)Row(24+i*12,FastHash.GetHashCode(names[i]),types[i],(uint)(prefix+offsets[i]));
        Row(144,1,FastHash.GetHashCode("ContainModuleData"),168);Row(168,1,FastHash.GetHashCode("BehaviorModuleData"),192);Row(192,1,FastHash.GetHashCode("ModuleData"),216);Row(216,FastHash.GetHashCode("TypeId"),0x23E650B8u,(uint)prefix);Row(228,FastHash.GetHashCode("id"),0xBDF07965u,(uint)(prefix+4));return b;
        // Reborn: metadata offsets and native targets have distinct coordinate spaces and are never native relocation slots.
        void Row(int at,uint name,uint type,uint target){BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at,4),name);BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at+4,4),type);BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(at+8,4),target);}
    }
    // Reborn: the caller proves fixed module/table bounds before reading words.
    private static uint Word(byte[] b,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));
}
