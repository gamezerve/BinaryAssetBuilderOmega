using System.Buffers.Binary;
using System.Xml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: implemented nonmodule record attributes are schema/native capabilities, not constants from one stock Conscript.
internal static class GameObjectNestedValueContract
{
    internal sealed record Owner(XmlElement Element,Type Type,int Slot,int Index=-1,int ChildSlot=-1);

    // Reborn: XML selects existing record owners; list cardinality and optional-pointer allocation remain explicit structural capabilities.
    internal static Owner[] Owners(XmlDocument source)
    {
        var owners=new List<Owner>();
        foreach(var root in source.DocumentElement!.ChildNodes.OfType<XmlElement>())
        {
            switch(root.LocalName)
            {
                case "VisionInfo":owners.Add(new(root,typeof(Ep1VisionInfo),560));break;
                case "Geometry":
                    owners.Add(new(root,typeof(Ep1Geometry),500));int shapeIndex=0;
                    foreach(var shape in root.ChildNodes.OfType<XmlElement>())if(shape.LocalName=="Shape")owners.Add(new(shape,typeof(Ep1Shape),500,shapeIndex++,12));else if(shape.LocalName=="PlacementShape")owners.Add(new(shape,typeof(Ep1Shape),500,-1,4));
                    break;
                case "LocomotorSet":owners.Add(new(root,typeof(Ep1LocomotorSet),432,owners.Count(o=>o.Element.LocalName=="LocomotorSet")));break;
                case "ObjectResourceInfo":
                    int i=0;foreach(var child in root.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="BuildCost"))owners.Add(new(child,typeof(MoneyTransaction),416,i++,4));break;
            }
        }
        return owners.ToArray();
    }

    // Reborn: reject unrepresentable native values at their real source attribute before comparison or compilation.
    internal static void Validate(XmlDocument source)
    {
        foreach(var owner in Owners(source))foreach(var binding in ConscriptModuleScalarContract.RecordBindings(owner.Type))
        {
            var bytes=ConscriptModuleScalarContract.Bytes(owner.Element,binding);
            if(binding.Codec=="SageBool"&&owner.Element.HasAttribute(binding.Name))owner.Element.SetAttribute(binding.Name,bytes[0]==0?"false":"true");
        }
    }

    // Reborn: normalize only fixed-width schema/native attributes, retaining unsupported child structure, references and record counts for admission diagnostics.
    internal static void Mask(XmlDocument source,XmlDocument baseline)
    {
        var owners=Owners(source);var models=Owners(baseline);
        if(owners.Length!=models.Length)throw new InvalidDataException("Nested record allocation changes are not implemented yet.");
        for(int i=0;i<owners.Length;i++)
        {
            var owner=owners[i];var model=models[i];
            if(owner.Type!=model.Type||owner.Index!=model.Index)throw SdkXmlDiagnostics.At(new InvalidDataException("Nested record owner topology differs."),owner.Element);
            foreach(var binding in ConscriptModuleScalarContract.RecordBindings(owner.Type)){owner.Element.RemoveAttribute(binding.Name);if(model.Element.HasAttribute(binding.Name))owner.Element.SetAttribute(binding.Name,model.Element.GetAttribute(binding.Name));}
        }
    }

    // Reborn: independently encode authored values at live owner pointers; never accept a changed output merely because it was produced by the compiler.
    internal static Dictionary<int,byte[]> Expected(XmlDocument source,byte[] native)
    {
        var result=new Dictionary<int,byte[]>();
        foreach(var owner in Owners(source))
        {
            int start=checked((int)Word(native,owner.Slot));
            if(owner.ChildSlot>=0)start=checked((int)Word(native,start+owner.ChildSlot));
            if(owner.Index>=0)start=checked(start+owner.Index*(owner.Type==typeof(MoneyTransaction)?8:owner.Type==typeof(Ep1Shape)?48:12));
            foreach(var binding in ConscriptModuleScalarContract.RecordBindings(owner.Type))
            {
                int at=checked(start+binding.Offset);var expected=ConscriptModuleScalarContract.Bytes(owner.Element,binding);
                if(start==0||at<0||at>native.Length-expected.Length||!native.AsSpan(at,expected.Length).SequenceEqual(expected))throw SdkXmlDiagnostics.At(new InvalidDataException("Nested native value differs: "+owner.Element.LocalName+"/@"+binding.Name+" (offset="+at+", codec="+binding.Codec+", expected="+Convert.ToHexString(expected)+", actual="+(at>=0&&at<=native.Length-expected.Length?Convert.ToHexString(native.AsSpan(at,expected.Length)):"out of bounds")+")"),(XmlNode?)owner.Element.GetAttributeNode(binding.Name)??owner.Element);
                result.Add(at,expected);
            }
        }
        return result;
    }

    // Reborn: malformed native pointers cannot escape the actual emitted buffer.
    private static uint Word(byte[] bytes,int at){if(at<0||at>bytes.Length-4)throw new InvalidDataException("Nested owner pointer bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));}
}
