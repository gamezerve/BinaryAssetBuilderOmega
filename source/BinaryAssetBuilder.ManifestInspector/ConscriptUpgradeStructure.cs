using System.Buffers.Binary;
using System.Globalization;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: source-driven upgrade lists and optional animation records share one explicit ABI contract, independent of emitted native indices.
internal static class ConscriptUpgradeStructure
{
    internal sealed record Plan(Dictionary<string,uint[]> Lists,Dictionary<string,byte[]?> Optional);
    internal static readonly string[] Fields={"TriggeredBy","ConflictsWith","CustomAnimAndDuration"};

    // Reborn: only real UpgradeModuleData inheritance enables these fields; similarly named children of other modules gain no bypass.
    internal static bool Supports(Type type){for(Type? current=type;current!=null;current=current.GetField("Base")?.FieldType)if(current==typeof(UpgradeModuleData))return true;return false;}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: replace flexible children in comparison copies only; the caller retains authored order, values and presence for real compilation. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlElement actual,XmlElement model)
    {
        foreach(var child in actual.ChildNodes.OfType<XmlElement>().Where(e=>Fields.Contains(e.LocalName)).ToArray())actual.RemoveChild(child);
        XmlNode? anchor=actual.FirstChild;
        foreach(var child in model.ChildNodes.OfType<XmlElement>().Where(e=>Fields.Contains(e.LocalName)))actual.InsertBefore(actual.OwnerDocument!.ImportNode(child,true),anchor);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: derive exact typed hashes and nested field words from source and catalog, not stock list length or compiler output. */
    //-------------------------------------------------------------------------------------------------
    internal static Plan? Read(ConscriptModuleScalarContract.Owner owner,ManifestAsset[] catalog)
    {
        if(!Supports(owner.NativeType))return null;
        Type type=owner.NativeType;string path=type.Name;while(type!=typeof(UpgradeModuleData)){type=type.GetField("Base")!.FieldType;path+=".Base";}
        // Reborn: list validation is linear in authored entries, not a full stock-catalog scan for every repeated trigger.
        var available=catalog.Where(a=>a.TypeId==FastHash.GetHashCode("UpgradeTemplate")).GroupBy(a=>a.InstanceId).ToDictionary(g=>g.Key,g=>g.Count());
        var lists=new Dictionary<string,uint[]>(StringComparer.Ordinal);
        foreach(string name in new[]{"TriggeredBy","ConflictsWith"}){
            var values=new List<uint>();foreach(var child in owner.Element.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName==name)){
                string value=child.InnerText.Trim();uint id=InstanceHandle.GetInstanceId(value);
                if(available.GetValueOrDefault(id)!=1)throw SdkXmlDiagnostics.At(new InvalidDataException("Missing or ambiguous upgrade asset: UpgradeTemplate:"+value),child);
                values.Add(id);
            }
            lists.Add(path+"."+name,values.ToArray());
        }
        var optional=new Dictionary<string,byte[]?>(StringComparer.Ordinal){{path+".CustomAnimAndDuration",Animation(owner.Element)}};return new(lists,optional);
    }

    // Reborn: reject invalid nested numerics at the authored source node before Core allocation can replace them with a generic overflow stack.
    internal static void ValidateSource(ConscriptModuleScalarContract.Owner owner){if(Supports(owner.NativeType))_=Animation(owner.Element);}

    // Reborn: optional presence and schema-defaulted scalar words are shared by early source validation and independent native checking.
    private static byte[]? Animation(XmlElement module)
    {
        var animation=module.ChildNodes.OfType<XmlElement>().SingleOrDefault(e=>e.LocalName=="CustomAnimAndDuration");byte[]? bytes=null;
        if(animation!=null){
            bytes=new byte[12];uint state=animation.HasAttribute("AnimState")?(uint)Enum.Parse<ModelConditionFlagType>(animation.GetAttribute("AnimState")):0;
            BinaryPrimitives.WriteUInt32LittleEndian(bytes,state);
            BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(4),Unsigned(animation,"Frames"));
            BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(8),Unsigned(animation,"Trigger"));
        }
        return bytes;
    }

    // Reborn: nested SageUnsignedInt fields use official decimal/hex/zero lexical forms rather than XML's narrower built-in unsigned datatype.
    private static uint Unsigned(XmlElement element,string name)
    {
        string text=element.HasAttribute(name)?element.GetAttribute(name):"0";
        try{return text=="0.0"?0:text.StartsWith("0x",StringComparison.OrdinalIgnoreCase)?uint.Parse(text[2..],NumberStyles.AllowHexSpecifier,CultureInfo.InvariantCulture):uint.Parse(text,NumberStyles.Integer,CultureInfo.InvariantCulture);}
        catch(Exception error)when(error is FormatException or OverflowException){throw SdkXmlDiagnostics.At(new InvalidDataException("Invalid SageUnsignedInt in CustomAnimAndDuration/@"+name+": "+text,error),(XmlNode?)element.GetAttributeNode(name)??element);}
    }
}
