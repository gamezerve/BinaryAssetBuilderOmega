using System.Buffers.Binary;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.XmlCompiler;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: root strong references own source-ordered dependency occurrences and native-ordered import slots; changes reindex every later occurrence independently.
internal static class GameObjectStrongReferenceContract
{
    internal static IReadOnlyList<Ep1GameObjectScalarTokens.Field> Fields=>Ep1GameObjectStrongTokens.Fields;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: root reference presence is authorable; real target existence/type is still required by Core catalog resolution. */
    //-------------------------------------------------------------------------------------------------
    internal static void Validate(XmlDocument source,XmlDocument baseline)
    {
        foreach(var field in Fields)
        {
            string authored=source.DocumentElement!.GetAttribute(field.Name);
            if(!string.IsNullOrWhiteSpace(authored) && !ConscriptSourceInput.Identity(authored))throw new InvalidDataException("Root strong-reference name is invalid: "+field.Name);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reference names and optional presence are semantic inputs whose occurrence table is verified independently of structural comparison. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlDocument candidate,XmlDocument model)
    {
        foreach(var field in Fields){candidate.DocumentElement!.RemoveAttribute(field.Name);if(model.DocumentElement!.HasAttribute(field.Name))candidate.DocumentElement.SetAttribute(field.Name,model.DocumentElement.GetAttribute(field.Name));}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retarget every already-present root import together, retaining the exact source occurrence positions. */
    //-------------------------------------------------------------------------------------------------
    internal static void AuthorProbe(XmlDocument source,XmlDocument baseline,byte[] native,IReadOnlyList<AssetId> original,IReadOnlyList<ManifestAsset> catalog)
    {
        foreach(var field in Fields)if(!string.IsNullOrWhiteSpace(baseline.DocumentElement!.GetAttribute(field.Name)))
        {
            uint occurrence=Word(native,field.Offset);if(occurrence==0 || occurrence>original.Count)throw new InvalidDataException("Root strong-reference probe occurrence differs: "+field.Name);var current=original[(int)occurrence-1];
            // Reborn: use a real different stock target of the exact captured type so source resolution is tested without inventing a missing dependency.
            var target=catalog.FirstOrDefault(asset=>asset.TypeId==current.TypeId && asset.InstanceId!=current.InstanceId)??throw new InvalidDataException("Root strong-reference probe target unavailable: "+field.Name);int separator=target.Name.IndexOf(':');if(separator<0 || separator==target.Name.Length-1)throw new InvalidDataException("Root strong-reference catalog name differs: "+field.Name);source.DocumentElement!.SetAttribute(field.Name,target.Name[(separator+1)..]);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: derive root occurrences from actual aligned XML traversal and retain every non-root stock dependency in its original relative order. */
    //-------------------------------------------------------------------------------------------------
    internal static AssetId[] Expected(XmlDocument source,XmlDocument baseline,byte[] native,IReadOnlyList<AssetId> original)
    {
        Validate(source,baseline);var oldRoots=Present(baseline);var newRoots=Present(source);if(original.Count<oldRoots.Length)throw new InvalidDataException("Root dependency baseline too short.");
        for(int i=0;i<oldRoots.Length;i++)if(Word(native,oldRoots[i].Offset)!=i+1 || original[i]!=new AssetId(TargetType(oldRoots[i]),InstanceHandle.GetInstanceId(baseline.DocumentElement!.GetAttribute(oldRoots[i].Name))))throw new InvalidDataException("Root dependencies are not the independently anchored source prefix.");
        return newRoots.Select(f=>new AssetId(TargetType(f),InstanceHandle.GetInstanceId(source.DocumentElement!.GetAttribute(f.Name)))).Concat(original.Skip(oldRoots.Length)).ToArray();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: source attribute traversal determines occurrence numbers, while an empty reference has no Core occurrence. */
    //-------------------------------------------------------------------------------------------------
    private static Ep1GameObjectScalarTokens.Field[] Present(XmlDocument source)=>source.DocumentElement!.Attributes.OfType<XmlAttribute>().Where(a=>!string.IsNullOrWhiteSpace(a.Value)).Select(a=>Fields.FirstOrDefault(f=>f.Name==a.LocalName && a.NamespaceURI.Length==0)).Where(f=>f.Name!=null).ToArray();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: root target families come from the reviewed official declarations, independently of token wire wrapper hashes. */
    //-------------------------------------------------------------------------------------------------
    private static uint TargetType(Ep1GameObjectScalarTokens.Field field)=>FastHash.GetHashCode(field.Name);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serializer counts depend on authored presence rather than a fixed 275-occurrence trial assumption. */
    //-------------------------------------------------------------------------------------------------
    internal static int ExpectedCount(XmlDocument source)=>273+Present(source).Length;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: restore selectors and IMP presence after string coordinates have returned to baseline, checking every surviving module occurrence before restoration. */
    //-------------------------------------------------------------------------------------------------
    internal static void Restore(XmlDocument source,XmlDocument baseline,Chunk actual,Chunk original)
    {
        var roots=Present(source);var oldRoots=Present(baseline);var baselineSlots=Slots(original.ImportsBuffer);var nonRoots=baselineSlots.Where(s=>!Fields.Any(f=>f.Offset==s)).ToArray();var expectedSlots=Fields.Where(f=>roots.Contains(f)).Select(f=>(uint)f.Offset).Concat(nonRoots).ToArray();
        if(!Slots(actual.ImportsBuffer).SequenceEqual(expectedSlots))throw new InvalidDataException("Authored root/module import slot order differs.");
        foreach(var field in Fields){int index=Array.IndexOf(roots,field);if(Word(actual.InstanceBuffer,field.Offset)!=(index<0?0u:(uint)(index+1)))throw new InvalidDataException("Authored root import occurrence differs: "+field.Name);Put(actual.InstanceBuffer,field.Offset,Word(original.InstanceBuffer,field.Offset));}
        foreach(uint slot in nonRoots){uint old=Word(original.InstanceBuffer,(int)slot);if(old<=oldRoots.Length || Word(actual.InstanceBuffer,(int)slot)!=old-oldRoots.Length+roots.Length)throw new InvalidDataException("Authored module import reindex differs at "+slot);Put(actual.InstanceBuffer,(int)slot,old);}
        actual.ImportsBuffer=original.ImportsBuffer.ToArray();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: expected token/native graphs replace root import slots and shift every non-root selector without changing payload allocations or RELO. */
    //-------------------------------------------------------------------------------------------------
    internal static void Patch(XmlDocument source,XmlDocument baseline,Chunk expected,int prefix)
    {
        var roots=Present(source);var oldRoots=Present(baseline);var nonRoots=Slots(expected.ImportsBuffer).Where(s=>!Fields.Any(f=>prefix+f.Offset==s)).ToArray();
        foreach(uint slot in nonRoots){uint old=Word(expected.InstanceBuffer,(int)slot);if(old<=oldRoots.Length)throw new InvalidDataException("Expected non-root import aliases root occurrence.");Put(expected.InstanceBuffer,(int)slot,checked((uint)(old-oldRoots.Length+roots.Length)));}
        foreach(var field in Fields){int index=Array.IndexOf(roots,field);Put(expected.InstanceBuffer,prefix+field.Offset,index<0?0u:(uint)(index+1));}
        expected.ImportsBuffer=Pack(Fields.Where(f=>roots.Contains(f)).Select(f=>(uint)(prefix+f.Offset)).Concat(nonRoots));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: sentinel-framed import tables retain native marshaler insertion order and reject aliased slot ownership. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] bytes){if(bytes.Length<8 || bytes.Length%4!=0 || Word(bytes,bytes.Length-4)!=uint.MaxValue)throw new InvalidDataException("Root import framing differs.");var slots=Enumerable.Range(0,bytes.Length/4-1).Select(i=>Word(bytes,i*4)).ToArray();if(slots.Distinct().Count()!=slots.Length)throw new InvalidDataException("Root import slot alias.");return slots;}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: deterministic IMP emission preserves a single terminal sentinel. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Pack(IEnumerable<uint> slots){var values=slots.Append(uint.MaxValue).ToArray();var bytes=new byte[values.Length*4];for(int i=0;i<values.Length;i++)Put(bytes,i*4,values[i]);return bytes;}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reindex only an owned bounded native/import word. */
    //-------------------------------------------------------------------------------------------------
    private static void Put(byte[] bytes,int offset,uint value){if(offset<0 || offset>bytes.Length-4)throw new InvalidDataException("Root import write bounds.");BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(offset,4),value);}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bounded native import-slot reads never infer occurrence positions from attribute order. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] bytes,int offset){if(offset<0 || offset>bytes.Length-4)throw new InvalidDataException("Root strong-reference slot is out of bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(offset,4));}
}
