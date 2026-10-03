using System.Buffers.Binary;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: inventory native audio PE metadata without executing DLL initialization or assuming export names prove signatures.
internal static class NativeAudioApiProbe
{
    // Reborn: this exact required set mirrors Native/Audio.cs; it is not an independently inferred ABI.
    private static readonly string[] Required = { "SIMEX_init","SIMEX_shutdown","SIMEX_id","SIMEX_open","SIMEX_create","SIMEX_close","SIMEX_wclose",
        "SIMEX_info","SIMEX_freesinfo","SIMEX_read","SIMEX_write","SIMEX_setplayloc","SIMEX_setcodec","SIMEX_getsamplerepname","SIMEX_setvbrquality",
        "SIMEX_getsamplerate","SIMEX_resample","SIMEX_getchannelconfig","SIMEX_getnumsamples","SIMEX_getlasterr" };

    // Reborn: exported/imported names are evidence only; function calling conventions still require separate verification.
    internal sealed record Evidence(Machine Machine,PEMagic Magic,IReadOnlyList<string> Exports,IReadOnlyList<string> Dependencies,bool Managed);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: print bounded architecture/import/export evidence and exact current binding gaps, never load the native library. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string path)
    {
        Evidence evidence = Read(path);
        Console.WriteLine($"Native audio API audit: machine={evidence.Machine}, format={evidence.Magic}, managed={evidence.Managed}, exports={evidence.Exports.Count}, sha256={Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)))}");
        foreach (string name in evidence.Dependencies) Console.WriteLine("  import DLL "+name);
        foreach (string name in evidence.Exports.Where(name => name.StartsWith("SIMEX_",StringComparison.Ordinal)).Take(64)) Console.WriteLine("  export "+name);
        foreach (string name in Required) Console.WriteLine($"  current binding {name}: {(evidence.Exports.Contains(name,StringComparer.Ordinal) ? "present" : "MISSING")}");
        Console.WriteLine("Export presence is not signature/codec compatibility; no native code executed.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: read bounded PE directories and named exports with explicit architecture and count bounds. */
    //-------------------------------------------------------------------------------------------------
    internal static Evidence Read(string path)
    {
        if (new FileInfo(path).Length > 16*1024*1024) throw new InvalidDataException("Native audio audit image exceeds 16 MiB.");
        using FileStream stream = File.OpenRead(path); using PEReader pe = new(stream);
        PEHeader header = pe.PEHeaders.PEHeader ?? throw new InvalidDataException("Missing PE optional header.");
        List<string> exports = new(),dependencies = new();
        DirectoryEntry directory = header.ExportTableDirectory;
        if (directory.RelativeVirtualAddress != 0)
        {
            byte[] table = Range(pe,directory.RelativeVirtualAddress,40);
            uint count = Word(table,24),names = Word(table,32);
            if (count > 4096) throw new InvalidDataException("Native audio named export count exceeds bound.");
            for (uint index = 0; index < count; index++)
                exports.Add(Name(pe,checked((int)Word(Range(pe,checked((int)(names+index*4)),4),0))));
            if (exports.Distinct(StringComparer.Ordinal).Count() != exports.Count) throw new InvalidDataException("Duplicate named PE exports.");
        }
        directory = header.ImportTableDirectory;
        if (directory.RelativeVirtualAddress != 0)
        {
            bool ended = false;
            for (int index = 0; index < 128; index++)
            {
                if ((long)index*20+20 > directory.Size) throw new InvalidDataException("Truncated PE import directory.");
                byte[] descriptor = Range(pe,checked(directory.RelativeVirtualAddress+index*20),20);
                if (descriptor.All(value => value == 0)) { ended = true; break; }
                dependencies.Add(Name(pe,checked((int)Word(descriptor,12))));
            }
            if (!ended) throw new InvalidDataException("PE import DLL count exceeds bound.");
        }
        return new Evidence(pe.PEHeaders.CoffHeader.Machine,header.Magic,exports,dependencies,pe.HasMetadata);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: read a bounded mapped RVA range without native loader execution. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Range(PEReader pe,int rva,int count)
    {
        if (rva <= 0 || count is < 1 or > 4096) throw new InvalidDataException("Invalid native audio PE range.");
        return pe.GetSectionData(rva).GetContent(0,count).ToArray();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require short terminated ASCII names rather than trusting arbitrary export/import strings. */
    //-------------------------------------------------------------------------------------------------
    private static string Name(PEReader pe,int rva)
    {
        StringBuilder name = new();
        for (int index = 0; index < 256; index++)
        {
            byte value = Range(pe,checked(rva+index),1)[0];
            if (value == 0) return name.Length > 0 ? name.ToString() : throw new InvalidDataException("Empty PE name.");
            if (value is < 32 or > 126) throw new InvalidDataException("Non-ASCII PE name.");
            name.Append((char)value);
        }
        throw new InvalidDataException("Unterminated PE name exceeds bound.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: decode little-endian PE directory words with managed bounds checks. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(byte[] bytes,int offset) => BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(offset,4));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: pin actual native/reference architectures and API sets without loading either library or executing codecs. */
    //-------------------------------------------------------------------------------------------------
    internal static void SelfTest()
    {
        string root = Directory.GetParent(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!)!.Parent!.FullName;
        Evidence current = Read(Path.Combine(root,"audio.dll"));
        if (current.Machine != Machine.I386 || current.Magic != PEMagic.PE32 || current.Managed || current.Exports.Count != 23
            || Required.Any(name => !current.Exports.Contains(name,StringComparer.Ordinal))
            || !current.Dependencies.Contains("VCRUNTIME140.dll",StringComparer.Ordinal)) throw new InvalidDataException("Current native audio API evidence changed.");
        Evidence reference = Read(Path.Combine(root,"Working RA3 Compiler for Reference","tools","BinaryAssetBuilder.AudioCompiler.dll"));
        if (reference.Machine != Machine.I386 || reference.Magic != PEMagic.PE32 || !reference.Managed || reference.Exports.Count != 0
            || !reference.Dependencies.Contains("MSVCR80.dll",StringComparer.Ordinal)) throw new InvalidDataException("Reference mixed-mode audio API evidence changed.");
        Console.WriteLine("Native audio API self-test: OK (actual x86 PE32, 23 exports/all 20 bindings, current/reference imports, no native code execution)");
    }
}
