using System.Buffers.Binary;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: recover pinned RA3 reference music metadata statically, explicitly separating it from EP1 stock type identity and synthetic Core domains.
internal static class PathMusicReferenceIdentity
{
    // Reborn: EP1 stock type identity is comparison evidence only, never permission to reuse the RA3 reference processing domain.
    internal const uint Ep1StockTypeHash = 0x599CDAF2u;
    internal sealed record Report(string ReferenceSha256,uint TypeId,uint ReferenceProcessingHash,uint ReferenceTypeHash,bool HasCustomData,
        uint ReferenceAllTypesHash,uint ReferenceWin32Version,uint Ep1ObservedTypeHash,PathMusicHeaderSemantics.Method[] Methods)
    {
        public bool ReadOnly => true;
        public bool ReferenceExecuted => false;
        public bool ReferenceProcessingHashRecovered => true;
        public bool ReferenceTypeHashMatchesEp1 => ReferenceTypeHash == Ep1ObservedTypeHash;
        public bool Ep1ProcessingHashRecovered => false;
        public bool ProductionBuildReady => false;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: review one bounded reparse-checked reference snapshot and recheck its raw bytes without loading the mixed-mode DLL. */
    //-------------------------------------------------------------------------------------------------
    internal static Report Inspect(string path)
    {
        path = Path.GetFullPath(path); byte[] bytes = SdkEnvironmentPreflight.Read(path,4*1048576); Report result = Verify(bytes);
        if (!bytes.AsSpan().SequenceEqual(SdkEnvironmentPreflight.Read(path,4*1048576))) throw new InvalidDataException("Reference music identity changed during review.");
        return result;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: extract reviewed event branches/field assignments from an exact DLL pin, retaining method provenance and refusing speculative EP1 equivalence. */
    //-------------------------------------------------------------------------------------------------
    internal static Report Verify(byte[] bytes)
    {
        var reference = PathMusicHeaderSemantics.VerifyReference(bytes);
        using MemoryStream input = new(bytes,false); using PEReader pe = new(input); MetadataReader metadata = pe.GetMetadataReader();
        List<PathMusicHeaderSemantics.Method> methods = new();
        //-------------------------------------------------------------------------------------------------
        /** Reborn: capture only named reviewed methods from the exact immutable reference snapshot. */
        //-------------------------------------------------------------------------------------------------
        byte[] Body(int token,string name,int rva,int length)
        {
            // Reborn: pin named method provenance before interpreting reviewed instruction offsets.
            MethodDefinition method = metadata.GetMethodDefinition((MethodDefinitionHandle)MetadataTokens.Handle(token));
            byte[] il = pe.GetMethodBody(method.RelativeVirtualAddress).GetILBytes() ?? throw new InvalidDataException("Missing reference identity IL.");
            if (metadata.GetString(method.Name) != name || method.RelativeVirtualAddress != rva || il.Length != length) throw new InvalidDataException("Reference identity method differs.");
            methods.Add(new(name,$"0x{token:X8}",rva,length,Convert.ToHexString(SHA256.HashData(il)))); return il;
        }
        byte[] info = Body(0x0600011F,"GetPathMusicExtendedTypeInformation",0x14A8,236);
        // Reborn: require the event type branch to enter the event metadata block, not a neighboring map/track branch.
        Require(Literal(info,0x4D) == 0x9A651D89u && info[0x52] == 0x2E && 0x54+(sbyte)info[0x53] == 0x8E,"Event identity branch differs.");
        uint processing = Literal(info,0x8F),typeHash = Literal(info,0xA1);
        Field(info,0x94,"ProcessingHash",metadata); Field(info,0x9B,"HasCustomData",metadata); Field(info,0xA6,"TypeHash",metadata); Field(info,0xB6,"TypeName",metadata);
        Require(info[0x9A] == 0x16 && info[0xAC] == 0x7E && Word(info,0xAD) == 0x040000FEu
            && metadata.GetString(metadata.GetFieldDefinition((FieldDefinitionHandle)MetadataTokens.Handle(0x040000FE)).Name).Contains("TypeName@PathMusicEvent@",StringComparison.Ordinal),"Event custom/type-name metadata differs.");
        byte[] allTypes = Body(0x0600011D,"GetAllTypesHash",0x143C,6); uint catalog = Literal(allTypes,0); Require(allTypes[5] == 0x2A,"Catalog return differs.");
        byte[] version = Body(0x0600011E,"GetVersionNumber",0x1450,75); uint win32 = Literal(version,0x18);
        // Reborn: Win32 is the zero platform branch; the distinct platform-one return must not be selected accidentally.
        Require(version[0x16] == 0x2D && 0x18+(sbyte)version[0x17] == 0x1E && version[0x1D] == 0x2A,"Win32 version branch differs.");
        byte[] dispatch = Body(0x06000121,"GetExtendedTypeInformation",0x167C,62);
        Require(Literal(dispatch,0x11) == 0x9A651D89u && dispatch[0x16] == 0x2E && 0x18+(sbyte)dispatch[0x17] == 0x36
            && dispatch[0x38] == 0x28 && Word(dispatch,0x39) == 0x0600011Fu,"Event metadata dispatcher differs.");
        byte[] processor = Body(0x0600011C,"ProcessInstance",0x687C,130);
        Require(Literal(processor,0x45) == 0x9A651D89u && processor[0x4A] == 0x2E && 0x4C+(sbyte)processor[0x4B] == 0x7A
            && processor[0x7C] == 0x28 && Word(processor,0x7D) == 0x06000119u,"Event processor dispatcher differs.");
        Require(processing == 0x76D0CEE6u && typeHash == 0x76D0CEEFu && catalog == 0x54EEE764u && win32 == 0x20B00004u,"Reviewed reference literal identities differ.");
        return new(reference.Sha256,0x9A651D89u,processing,typeHash,false,catalog,win32,Ep1StockTypeHash,methods.ToArray());
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: admit only the reviewed full-width ldc.i4 form at a known instruction boundary. */
    //-------------------------------------------------------------------------------------------------
    private static uint Literal(byte[] il,int offset)
    { Require(il[offset] == 0x20,"Reference literal opcode differs."); return Word(il,offset+1); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate field assignment tokens and metadata names, never infer identity semantics from literal proximity alone. */
    //-------------------------------------------------------------------------------------------------
    private static void Field(byte[] il,int offset,string name,MetadataReader metadata)
    {
        Require(il[offset] == 0x7D,"Reference field-store opcode differs."); Handle handle = MetadataTokens.Handle((int)Word(il,offset+1));
        Require(handle.Kind == HandleKind.MemberReference && metadata.GetString(metadata.GetMemberReference((MemberReferenceHandle)handle).Name) == name,"Reference identity field differs.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: decode unsigned words only from the exact pinned IL snapshot. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] bytes,int offset) => BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(offset,4));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fail closed when a reviewed identity/dispatch witness differs. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
