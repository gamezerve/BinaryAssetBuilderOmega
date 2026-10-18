using System.Buffers.Binary;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: existing root ASCII allocations may change content at a fixed byte length while the dynamic pointer/relocation topology remains stock-qualified.
internal static class GameObjectRootStringContract
{
    internal static IReadOnlyList<Ep1GameObjectScalarTokens.Field> Fields=>Ep1GameObjectStringTokens.Fields;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain allocation presence and byte extent until general dynamic root reallocation is independently qualified. */
    //-------------------------------------------------------------------------------------------------
    internal static void Validate(XmlDocument source,XmlDocument baseline)
    {
        foreach(var field in Fields)
        {
            byte[] authored=Ascii(source.DocumentElement!.GetAttribute(field.Name),field.Name),original=Ascii(baseline.DocumentElement!.GetAttribute(field.Name),field.Name);
            if(authored.Length!=original.Length)throw new InvalidDataException("Root string byte length cannot change yet: "+field.Name+" ("+original.Length+" -> "+authored.Length+")");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: admitted fixed-allocation string content is removed from the structural comparison, never silently omitted from native verification. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlDocument candidate,XmlDocument model)
    {
        foreach(var field in Fields){candidate.DocumentElement!.RemoveAttribute(field.Name);if(model.DocumentElement!.HasAttribute(field.Name))candidate.DocumentElement.SetAttribute(field.Name,model.DocumentElement.GetAttribute(field.Name));}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: mutate every populated string without changing its byte extent to exercise all current allocation owners together. */
    //-------------------------------------------------------------------------------------------------
    internal static void AuthorProbe(XmlDocument source,XmlDocument baseline)
    {
        foreach(var field in Fields)
        {
            string value=baseline.DocumentElement!.GetAttribute(field.Name);if(value.Length==0)continue;char[] changed=value.ToCharArray();int at=Array.FindIndex(changed,c=>c is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9');if(at<0)throw new InvalidDataException("Root string probe lacks a mutable ASCII character: "+field.Name);changed[at]=changed[at]=='R'?'S':'R';source.DocumentElement!.SetAttribute(field.Name,new string(changed));
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify exact string headers/content/terminators and restore only owned bytes before whole-stock-graph comparison. */
    //-------------------------------------------------------------------------------------------------
    internal static void VerifyAndRestore(XmlDocument source,byte[] actual,byte[] restored,byte[] baseline)
    {
        Verify(source,actual,0);
        foreach(var field in Fields)
        {
            uint pointer=Word(baseline,field.Offset+4),length=Word(baseline,field.Offset);baseline.AsSpan(field.Offset,field.Width).CopyTo(restored.AsSpan(field.Offset,field.Width));if(pointer!=0)baseline.AsSpan((int)pointer,(int)length+1).CopyTo(restored.AsSpan((int)pointer,(int)length+1));
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: package publication rechecks rebased root string pointers and exact authored ASCII payloads. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlDocument source,byte[] native,int prefix)
    {
        foreach(var field in Fields)
        {
            byte[] expected=Ascii(source.DocumentElement!.GetAttribute(field.Name),field.Name);uint length=Word(native,prefix+field.Offset),pointer=Word(native,prefix+field.Offset+4);
            if(length!=expected.Length || (expected.Length==0?pointer!=0:pointer<prefix || pointer>native.Length-expected.Length-1) || (expected.Length!=0 && (!native.AsSpan((int)pointer,expected.Length).SequenceEqual(expected) || native[(int)pointer+expected.Length]!=0)))throw new InvalidDataException("GameObject root string differs: "+field.Name);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: expected metadata changes only the already-rebased native-tail payload of fixed-size string allocations. */
    //-------------------------------------------------------------------------------------------------
    internal static void Patch(XmlDocument source,byte[] target,int prefix)
    {
        foreach(var field in Fields)
        {
            byte[] value=Ascii(source.DocumentElement!.GetAttribute(field.Name),field.Name);uint length=Word(target,prefix+field.Offset),pointer=Word(target,prefix+field.Offset+4);if(length!=value.Length || (value.Length==0?pointer!=0:pointer>target.Length-value.Length-1))throw new InvalidDataException("Expected root string allocation differs: "+field.Name);if(pointer!=0){value.CopyTo(target.AsSpan((int)pointer));target[(int)pointer+value.Length]=0;}
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: the reviewed root string route remains bounded non-NUL ASCII, matching its existing native projection contract. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Ascii(string value,string name){if(value.Any(c=>c==0 || c>127))throw new InvalidDataException("Root string currently requires non-NUL ASCII: "+name);return Encoding.ASCII.GetBytes(value);}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: string header reads are bounded independently of compiler-owned pointer traversal. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] bytes,int offset){if(offset<0 || offset>bytes.Length-4)throw new InvalidDataException("Root string header is out of bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(offset,4));}
}
