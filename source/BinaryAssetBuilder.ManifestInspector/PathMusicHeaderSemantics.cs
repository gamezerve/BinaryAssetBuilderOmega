using System.Globalization;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: pin reference IL evidence and review a conservative ASCII literal subset without executing the mixed-mode DLL or synthesizing headers.
internal static class PathMusicHeaderSemantics
{
    internal const string ReferenceSha256 = "A14D15ADDEF502C2DF6F6BA272D6098C4C131FBEA9C3ED0149C11C4556347139";
    // Reborn: shape matches describe observed lexical IL; canonical literals are stricter and never grant production admission.
    internal sealed record Line(bool LegacyShapeMatch,uint? Value,bool CanonicalLiteral,string? UnsupportedReason);
    internal sealed record ScanResult(string Event,int Lines,int? MatchedLine,uint? Value,bool? LegacyWarningExpected,bool CanonicalLiteral,string? UnsupportedReason);
    internal sealed record Method(string Name,string Token,int Rva,int IlBytes,string IlSha256);
    internal sealed record Reference(string Sha256,Method[] Methods)
    {
        public bool ReferenceExecuted => false;
        public bool HeaderRecovered => false;
        public bool ProductionBuildReady => false;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: admit only the reviewed exact reference DLL snapshot and read method bodies as metadata, never load native/reference code. */
    //-------------------------------------------------------------------------------------------------
    internal static Reference InspectReference(string path) => VerifyReference(SdkEnvironmentPreflight.Read(Path.GetFullPath(path),4*1048576));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve exact tokens/RVAs/body identities supporting manually reviewed lexical, warning and runtime-copy behavior. */
    //-------------------------------------------------------------------------------------------------
    internal static Reference VerifyReference(byte[] bytes)
    {
        string hash = Hash(bytes); if (hash != ReferenceSha256) throw new InvalidDataException("Unreviewed PathMusic reference DLL snapshot.");
        using MemoryStream input = new(bytes,false); using PEReader pe = new(input); MetadataReader metadata = pe.GetMetadataReader();
        List<Method> methods = new();
        foreach (var expected in new[] { (Token:0x06000117,Name:"ParsePathMusicHeaderLine",Rva:0x2AA0,Bytes:1618),
            (Token:0x06000119,Name:"ProcessPathMusicEventInstance",Rva:0x50C4,Bytes:670) })
        {
            MethodDefinition method = metadata.GetMethodDefinition((MethodDefinitionHandle)MetadataTokens.Handle(expected.Token));
            byte[] body = pe.GetMethodBody(method.RelativeVirtualAddress).GetILBytes() ?? throw new InvalidDataException("Missing reference IL.");
            if (metadata.GetString(method.Name) != expected.Name || method.RelativeVirtualAddress != expected.Rva || body.Length != expected.Bytes)
                throw new InvalidDataException("Reference music method metadata differs.");
            methods.Add(new(expected.Name,$"0x{expected.Token:X8}",method.RelativeVirtualAddress,body.Length,Hash(body)));
        }
        return new(hash,methods.ToArray());
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reproduce observed case-sensitive substring/name/space selection only for bounded ASCII lines and safe signed hexadecimal literals. */
    //-------------------------------------------------------------------------------------------------
    internal static Line ReviewLine(string line,string eventName)
    {
        CheckEvent(eventName);
        if (line.Length > 510 || line.Any(character => character == '\0' || character > 127 || character is '\r' or '\n'))
            throw new InvalidDataException("Bounded single ASCII header line required; getline boundary/ANSI behavior is outside this subset.");
        const string prefix = "PATH_EVENT_";
        if (!line.Contains("#define",StringComparison.Ordinal)) return new(false,null,false,null);
        int start = line.IndexOf(prefix,StringComparison.Ordinal);
        if (start < 0) return new(false,null,false,null);
        int end = line.IndexOf(' ',start);
        if (end < start+prefix.Length || line.Substring(start+prefix.Length,end-start-prefix.Length) != eventName) return new(false,null,false,null);
        int hex = line.IndexOf("0x",StringComparison.Ordinal);
        if (hex < 0) return new(false,null,false,null);
        int finish = hex+2;
        while (finish < line.Length && char.IsAsciiHexDigit(line[finish])) finish++;
        string digits = line.Substring(hex+2,finish-hex-2);
        if (digits.Length is < 1 or > 8 || !uint.TryParse(digits,NumberStyles.AllowHexSpecifier,CultureInfo.InvariantCulture,out uint value) || value > int.MaxValue)
            return new(true,null,false,"Native strtol invalid/overflow behavior is not admitted by this literal subset.");
        string canonical = "#define "+prefix+eventName+" "+line.Substring(hex,finish-hex);
        bool exact = line.Trim() == canonical;
        return new(true,value,exact,exact ? null : "Legacy lexical match is not a canonical standalone definition; comments, earlier hex or trailing text require review.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop at the first lexical match, retain explicit zero versus no match, and never silently resolve unsupported header constructs. */
    //-------------------------------------------------------------------------------------------------
    internal static ScanResult Scan(byte[] header,string eventName)
    {
        CheckEvent(eventName);
        if (header.Length > 1048576 || header.Any(value => value == 0 || value > 127)) throw new InvalidDataException("Bounded ASCII header bytes required.");
        using StringReader input = new(Encoding.ASCII.GetString(header)); string? text; int lines = 0;
        while ((text = input.ReadLine()) != null)
        {
            if (++lines > 8192) throw new InvalidDataException("Header line budget exceeded.");
            Line line = ReviewLine(text,eventName);
            if (line.LegacyShapeMatch) return new(eventName,lines,lines,line.Value,line.Value == null ? null : line.Value == 0,line.CanonicalLiteral,line.UnsupportedReason);
        }
        return new(eventName,lines,null,0,true,false,"No matching reference-style definition; legacy caller retains zero and warns.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate a caller-supplied header snapshot with pinned reference evidence; report diagnostic refusal without rewriting source or emitting assets. */
    //-------------------------------------------------------------------------------------------------
    internal static object InspectHeader(string referencePath,string headerPath,string eventName)
    {
        Reference reference = InspectReference(referencePath); headerPath = Path.GetFullPath(headerPath);
        byte[] bytes = SdkEnvironmentPreflight.Read(headerPath,1048576); ScanResult scan = Scan(bytes,eventName);
        string hash = Hash(bytes); if (Hash(SdkEnvironmentPreflight.Read(headerPath,1048576)) != hash) throw new InvalidDataException("Header changed during review.");
        return new { Reference = reference,HeaderSha256 = hash,Result = scan,ReadOnly = true,SnapshotOnly = true,HeaderRecovered = false,ProductionBuildReady = false };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exclude ambiguous/prefixed names and unbounded inputs from this local diagnostic model. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckEvent(string name)
    {
        if (name.Length is < 1 or > 256 || name.Any(character => !(char.IsAsciiLetterOrDigit(character) || character == '_')))
            throw new InvalidDataException("Bounded literal event identifier required.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: hashes pin metadata/source snapshots, not runtime loading or generic parser equivalence. */
    //-------------------------------------------------------------------------------------------------
    private static string Hash(byte[] bytes) => Convert.ToHexString(SHA256.HashData(bytes));
}
