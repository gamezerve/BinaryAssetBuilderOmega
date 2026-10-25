using System.Buffers.Binary;
using System.Globalization;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: reuse the implemented EP1 hardpoint/turret ABI with source-sized lists and independently named optional weapon/upgrade imports.
internal static class ConscriptWeaponSetContract
{
    internal static bool Supports(Type type)=>type==typeof(Ep1WeaponSetUpdateModuleData);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: implement the official Weapon sortBy Ordering contract and boolean lexical forms before Core assigns occurrence indices. */
    //-------------------------------------------------------------------------------------------------
    internal static void Normalize(XmlDocument source)
    {
        foreach(XmlElement module in source.GetElementsByTagName("WeaponSetUpdate","uri:ea.com:eala:asset"))
        foreach(var hp in module.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName is "WeaponSlotHardpoint" or "WeaponSlotTurret")){
            // Reborn: report unsigned overflow at the authored hardpoint ID before native allocation loses its source coordinate.
            _=Unsigned(hp,"ID");
            foreach(var node in new[]{hp}.Concat(hp.ChildNodes.OfType<XmlElement>()))foreach(string name in new[]{"AllowInterleavedFiring","AutoChooseWeapon","IsPlayerUpgradePermanent","ConquerMetricsAlwaysCount"})if(node.HasAttribute(name))node.SetAttribute(name,Boolean(node,name,false)?"true":"false");
            foreach(var row in hp.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="Weapon").OrderBy(e=>Enumeration<WeaponSlotType>(e,"Ordering",0)).ToArray())hp.InsertBefore(row,hp.ChildNodes.OfType<XmlElement>().FirstOrDefault(e=>e.LocalName=="TurretSettings"));
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: mask implemented hardpoint/turret records only; hierarchical/contained families remain visible. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlElement actual,XmlElement model)
    {
        foreach(var child in actual.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName is "WeaponSlotHardpoint" or "WeaponSlotTurret").ToArray())actual.RemoveChild(child);
        XmlNode? before=actual.FirstChild;foreach(var child in model.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName is "WeaponSlotHardpoint" or "WeaponSlotTurret"))actual.InsertBefore(actual.OwnerDocument!.ImportNode(child,true),before);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: check every hardpoint/weapon word against source/defaults; no stock count, weapon name or old import ordinal is an expected value. */
    //-------------------------------------------------------------------------------------------------
    internal static void VerifyNative(XmlElement module,Chunk native,InstanceDeclaration instance)
    {
        byte[] bytes=native.InstanceBuffer;var hardpoints=module.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName is "WeaponSlotHardpoint" or "WeaponSlotTurret").ToArray();
        if(Word(bytes,12)!=hardpoints.Length||Word(bytes,16)!=(hardpoints.Length==0?0:44)||hardpoints.Length>(bytes.Length-44)/28)throw new InvalidDataException("WeaponSet source/native hardpoint allocation differs.");
        int next=checked(44+hardpoints.Length*28);var imports=new List<uint>();var relos=new List<uint>();if(hardpoints.Length>0)relos.Add(16);
        for(int i=0;i<hardpoints.Length;i++){
            var hp=hardpoints[i];int at=44+i*28;var rows=hp.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="Weapon").ToArray();
            if(Word(bytes,at)!=0||Word(bytes,at+4)!=Unsigned(hp,"ID")||Word(bytes,at+8)!=Enumeration<WeaponSlotInterleavedStyleType>(hp,"InterleavedStyle",1)||Word(bytes,at+12)!=Enumeration<WeaponChoiceCriteria>(hp,"WeaponChoiceCriteria",0)||Word(bytes,at+16)!=rows.Length||Word(bytes,at+20)!=(rows.Length==0?0:next)||bytes[at+24]!=(Boolean(hp,"AllowInterleavedFiring",false)?1:0)||bytes[at+25]!=(Boolean(hp,"AutoChooseWeapon",true)?1:0)||bytes[at+26]!=0||bytes[at+27]!=0)throw SdkXmlDiagnostics.At(new InvalidDataException("WeaponSlotHardpoint differs from source/defaults."),hp);
            if(rows.Length>0)relos.Add((uint)(at+20));
            foreach(var row in rows){int w=next;if(w<0||w>bytes.Length-80)throw new InvalidDataException("Weapon row allocation is truncated.");if(Word(bytes,w)!=Enumeration<WeaponSlotType>(row,"Ordering",0))throw new InvalidDataException("Weapon Ordering differs from source.");Reference(row,"Template","WeaponTemplate",w+4);Reference(row,"Upgrade","UpgradeTemplate",w+8);MaskWords(row,"ObjectStatus",w+12);MaskWords(row,"ForbiddenObjectStatus",w+44);if(bytes[w+76]!=(Boolean(row,"IsPlayerUpgradePermanent",false)?1:0)||bytes[w+77]!=(Boolean(row,"ConquerMetricsAlwaysCount",false)?1:0)||bytes[w+78]!=0||bytes[w+79]!=0)throw new InvalidDataException("Weapon boolean/default/padding differs.");next=checked(next+80);}
        }
        if(next!=bytes.Length||!Slots(native.ImportsBuffer).Order().SequenceEqual(imports.Order())||!Slots(native.RelocationBuffer).Order().SequenceEqual(relos.Order()))throw new InvalidDataException("WeaponSet native allocation/import/relocation ownership differs.");

        // Reborn: optional strong slots are tied to authored names and the actual common Core occurrence table.
        void Reference(XmlElement row,string name,string type,int at){string value=row.GetAttribute(name).Trim();uint index=Word(bytes,at);if(value.Length==0){if(index!=0)throw new InvalidDataException("Omitted Weapon reference retains an import.");return;}int colon=value.IndexOf(':');if(colon>=0){if(value[..colon]!=type)throw SdkXmlDiagnostics.At(new InvalidDataException("Wrong Weapon reference type."),row.GetAttributeNode(name));value=value[(colon+1)..];}if(index==0||index>instance.ReferencedInstances.Count)throw new InvalidDataException("Weapon import index bounds.");var target=instance.ReferencedInstances[(int)index-1];if(target.TypeId!=FastHash.GetHashCode(type)||target.InstanceId!=InstanceHandle.GetInstanceId(value))throw new InvalidDataException("Weapon import differs from authored target.");imports.Add((uint)at);}
        // Reborn: required and forbidden status masks are independent full EP1 fields, including ALL/subtraction semantics.
        void MaskWords(XmlElement row,string name,int at){var words=new uint[8];foreach(string raw in row.GetAttribute(name).Split((char[]?)null,StringSplitOptions.RemoveEmptyEntries)){bool include=raw[0]!='-';string token=raw[0] is '+' or '-'?raw[1..]:raw;if(token=="ALL"){Array.Fill(words,uint.MaxValue);continue;}int bit=(int)Enum.Parse<ObjectStatusType>(token,false);if(bit<0||bit>=words.Length*32)throw new InvalidDataException("ObjectStatus exceeds EP1 mask width.");words[bit/32]=include?words[bit/32]|(1u<<(bit%32)):words[bit/32]^(1u<<(bit%32));}for(int i=0;i<words.Length;i++)if(Word(bytes,at+i*4)!=words[i])throw SdkXmlDiagnostics.At(new InvalidDataException("Weapon status mask differs: "+name),(XmlNode?)row.GetAttributeNode(name)??row);}
    }

    // Reborn: malformed nested lexicals retain the precise attribute instead of becoming a later marshaler stack trace.
    private static bool Boolean(XmlElement node,string name,bool fallback){try{return node.HasAttribute(name)?XmlConvert.ToBoolean(node.GetAttribute(name).Trim()):fallback;}catch(FormatException e){throw SdkXmlDiagnostics.At(new InvalidDataException("Invalid Weapon boolean: "+name,e),node.GetAttributeNode(name));}}
    // Reborn: enums use their real native ordinals and schema defaults, not the stock hardpoint's selected value.
    private static uint Enumeration<T>(XmlElement node,string name,uint fallback)where T:struct,Enum{try{return node.HasAttribute(name)?Convert.ToUInt32(Enum.Parse<T>(node.GetAttribute(name),false),CultureInfo.InvariantCulture):fallback;}catch(Exception e)when(e is ArgumentException or OverflowException){throw SdkXmlDiagnostics.At(new InvalidDataException("Invalid Weapon enum: "+name,e),node.GetAttributeNode(name));}}
    // Reborn: preserve EA unsigned decimal/hex/zero spellings without imposing a gameplay-specific ID range.
    private static uint Unsigned(XmlElement node,string name){string text=node.HasAttribute(name)?node.GetAttribute(name):"0";try{return text=="0.0"?0:text.StartsWith("0x",StringComparison.OrdinalIgnoreCase)?uint.Parse(text[2..],NumberStyles.HexNumber,CultureInfo.InvariantCulture):uint.Parse(text,CultureInfo.InvariantCulture);}catch(Exception e)when(e is FormatException or OverflowException){throw SdkXmlDiagnostics.At(new InvalidDataException("Invalid Weapon unsigned ID.",e),node.GetAttributeNode(name));}}
    // Reborn: native field and auxiliary framing reads remain bounded to the actual allocated graph.
    private static uint Word(byte[] bytes,int at){if(at<0||at>bytes.Length-4)throw new InvalidDataException("WeaponSet word bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));}
    // Reborn: nonempty auxiliary streams have one sentinel and no duplicate occurrence source slots.
    private static uint[] Slots(byte[] bytes){if(bytes.Length==0)return Array.Empty<uint>();if(bytes.Length<8||bytes.Length%4!=0||Word(bytes,bytes.Length-4)!=uint.MaxValue)throw new InvalidDataException("Weapon auxiliary framing.");var slots=Enumerable.Range(0,bytes.Length/4-1).Select(i=>Word(bytes,i*4)).ToArray();if(slots.Distinct().Count()!=slots.Length)throw new InvalidDataException("Weapon auxiliary duplicates.");return slots;}
}
