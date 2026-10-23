using System.Buffers.Binary;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: source-bound destruction items own exact native strings, weak identities, FX imports and list storage independently of producer metadata.
internal static class BuildingDestructionContract
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain actual supported DieMux fields and reject only the variable status/veterancy storage not implemented by the independent token proof. */
    //-------------------------------------------------------------------------------------------------
    internal static void Validate(XmlElement owner)
    {
        var die=owner.ChildNodes.OfType<XmlElement>().SingleOrDefault(e=>e.LocalName=="DieMuxData")??throw SdkXmlDiagnostics.At(new InvalidDataException("Building destruction requires DieMuxData."),owner);
        foreach(string field in new[]{"VeterancyLevels","ExemptStatus","RequiredStatus"})if(field=="VeterancyLevels"?!string.IsNullOrWhiteSpace(die.GetAttribute(field)):die.HasAttribute(field))throw SdkXmlDiagnostics.At(new InvalidDataException("Building destruction variable DieMux storage is not implemented: "+field),die.GetAttributeNode(field));
        if(die.ChildNodes.OfType<XmlElement>().Any())throw SdkXmlDiagnostics.At(new InvalidDataException("Building destruction DieMux child storage is not implemented."),die);
        foreach(var binding in ConscriptModuleScalarContract.RecordBindings(typeof(DieMuxDataType)))_=ConscriptModuleScalarContract.Bytes(die,binding);
        var items=Items(owner);if(items.Length==0)throw SdkXmlDiagnostics.At(new InvalidDataException("Building destruction requires at least one Item."),owner);
        foreach(var item in items){foreach(var binding in ConscriptModuleScalarContract.RecordBindings(typeof(BuildingDestructionItem)))_=ConscriptModuleScalarContract.Bytes(item,binding);if(item.GetAttribute("BonePrefix").Any(c=>c>127||c=='\0'))throw SdkXmlDiagnostics.At(new InvalidDataException("Building destruction BonePrefix must be ASCII without NUL."),item.GetAttributeNode("BonePrefix"));}
    }

    // Reborn: repeated items are source ordered; no frozen stock cardinality or separate gameplay quota is introduced.
    internal static XmlElement[] Items(XmlElement owner)=>owner.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="Item").ToArray();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: only comparison copies use the stock-shaped template; actual values/list rows remain intact for native compilation and independent proof. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlElement actual,XmlElement model)
    {
        foreach(string field in new[]{"AssociatedModule","FillerMesh"}){actual.RemoveAttribute(field);if(model.HasAttribute(field))actual.SetAttribute(field,model.GetAttribute(field));}
        foreach(var child in actual.ChildNodes.OfType<XmlElement>().ToArray())actual.RemoveChild(child);
        foreach(var child in model.ChildNodes.OfType<XmlElement>())actual.AppendChild(actual.OwnerDocument!.ImportNode(child,true));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: check every native word/allocation against source and catalog, including optional FX presence and exact auxiliary ownership. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlElement owner,Chunk native,InstanceDeclaration instance,ManifestAsset[] catalog)
    {
        Validate(owner);var b=native.InstanceBuffer;var items=Items(owner);int storage=checked(72+items.Length*20);
        if(b.Length<storage||Word(0)!=0x0D96A2F4u||Word(4)!=Weak(owner.GetAttribute("id"))||Word(64)!=items.Length||Word(68)!=72)throw new InvalidDataException("Building destruction identity/list native header differs.");
        foreach(int at in new[]{8,12,16,20})if(Word(at)!=0)throw new InvalidDataException("Unsupported destruction DieMux allocation.");
        var die=owner.ChildNodes.OfType<XmlElement>().Single(e=>e.LocalName=="DieMuxData");
        foreach(var binding in ConscriptModuleScalarContract.RecordBindings(typeof(DieMuxDataType)))Check(die,binding,8);
        foreach(var binding in ConscriptModuleScalarContract.Bindings(typeof(BuildingDestructionModuleData)))Check(owner,binding,0);
        if(Word(52)!=OptionalWeak(owner,"AssociatedModule",null)||Word(60)!=OptionalWeak(owner,"FillerMesh","NEWSKIN_FILL"))throw new InvalidDataException("Building destruction weak module/mesh identity differs.");
        var covered=new bool[b.Length];Array.Fill(covered,true,0,storage);var relos=new HashSet<uint>{68};var imports=new HashSet<uint>();
        for(int index=0;index<items.Length;index++){
            var item=items[index];int at=72+index*20;uint length=Word(at),pointer=Word(at+4);
            if(!item.HasAttribute("BonePrefix")){if(length!=0||pointer!=0)throw new InvalidDataException("Absent destruction bone has storage.");}
            else{byte[] expected=Encoding.ASCII.GetBytes(item.GetAttribute("BonePrefix"));if(length!=expected.Length||pointer<storage||pointer>b.Length-expected.Length-1||covered.AsSpan((int)pointer,expected.Length+1).Contains(true)||!b.AsSpan((int)pointer,expected.Length).SequenceEqual(expected)||b[(int)pointer+expected.Length]!=0)throw new InvalidDataException("Destruction bone text/storage differs.");Array.Fill(covered,true,(int)pointer,expected.Length+1);relos.Add((uint)(at+4));}
            string fx=item.GetAttribute("FX");uint selector=Word(at+8);
            if(fx.Length==0){if(selector!=0)throw new InvalidDataException("Absent destruction FX has an import.");}
            else{string name=fx;int colon=name.IndexOf(':');if(colon>=0){if(name[..colon]!="FXList")throw new InvalidDataException("Destruction FX type differs.");name=name[(colon+1)..];}var target=new AssetId(FastHash.GetHashCode("FXList"),InstanceHandle.GetInstanceId(name));if(catalog.Count(a=>a.TypeId==target.TypeId&&a.InstanceId==target.InstanceId)!=1||selector==0||selector>instance.ReferencedInstances.Count)throw new InvalidDataException("Destruction FX target/index missing or ambiguous.");var reference=instance.ReferencedInstances[(int)selector-1];if(reference.TypeId!=target.TypeId||reference.InstanceId!=target.InstanceId)throw new InvalidDataException("Destruction FX import identity differs.");imports.Add((uint)(at+8));}
            if(Word(at+12)!=OptionalWeak(item,"CreationObject",null))throw new InvalidDataException("Destruction weak debris identity differs.");
            foreach(var binding in ConscriptModuleScalarContract.RecordBindings(typeof(BuildingDestructionItem)))Check(item,binding,at);
        }
        int gap=0;for(int at=0;at<b.Length;at++){if(covered[at]){gap=0;continue;}if(b[at]!=0||++gap>3)throw new InvalidDataException("Unowned destruction native allocation.");}
        if(!Slots(native.RelocationBuffer).Order().SequenceEqual(relos.Order())||!Slots(native.ImportsBuffer).Order().SequenceEqual(imports.Order()))throw new InvalidDataException("Destruction relocation/import ownership differs.");

        // Reborn: shared scalar codecs preserve schema defaults and converted engine units without invoking the native writer as its own oracle.
        void Check(XmlElement element,ConscriptModuleScalarContract.Binding binding,int start){if(!b.AsSpan(start+binding.Offset,binding.Width).SequenceEqual(ConscriptModuleScalarContract.Bytes(element,binding)))throw new InvalidDataException("Destruction authored value differs: "+element.LocalName+"."+binding.Name);}
        // Reborn: all module-relative reads remain in the previously bounded allocation.
        uint Word(int at)=>BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));
    }

    // Reborn: POID hashing follows invariant lower-case native weak identity semantics; absence differs from an authored empty value.
    private static uint OptionalWeak(XmlElement element,string name,string? fallback)=>element.HasAttribute(name)?Weak(element.GetAttribute(name)):fallback==null?0:Weak(fallback);
    // Reborn: weak fields contain the exact authored identity spelling, not a strong-reference occurrence index.
    private static uint Weak(string value)=>FastHash.GetHashCode(value.ToLowerInvariant());
    // Reborn: accept precisely the canonical auxiliary terminator, rejecting duplicate slots and trailing records.
    private static uint[] Slots(byte[] bytes){if(bytes.Length==0)return Array.Empty<uint>();if(bytes.Length<8||bytes.Length%4!=0||BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(bytes.Length-4))!=uint.MaxValue)throw new InvalidDataException("Invalid destruction auxiliary terminator.");var values=Enumerable.Range(0,bytes.Length/4-1).Select(i=>BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(i*4,4))).ToArray();if(values.Distinct().Count()!=values.Length)throw new InvalidDataException("Duplicate destruction auxiliary slot.");return values;}
}
