using System.Buffers.Binary;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using Relo;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: independently assembled native goldens and reviewed schema fixtures separate runtime layout from authored processor readiness.
internal static class PathMusicRuntimeProbeSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: test real tracker plain/alternate/cache padding, strict header rejection and immutable detached output without production source/schema changes. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        byte[] header = Encoding.ASCII.GetBytes("#define PATH_EVENT_RebornHeaderProbe 0x00000001"),copy = (byte[])header.Clone();
        var prepared = PathMusicRuntimeProbe.FromHeader(header,"RebornHeaderProbe");
        Equal(prepared.Chunk,Words(0,1,0,1),Array.Empty<byte>());
        Equal(PathMusicRuntimeProbe.FromHeader(header,"RebornHeaderProbe","RebornAlternate",false).Chunk,
            Words(0,1,16,0,InstanceHandle.GetInstanceId("RebornAlternate")),Words(8,uint.MaxValue));
        Equal(PathMusicRuntimeProbe.EncodeObserved(0),Words(0,0,0,1),Array.Empty<byte>());
        Equal(PathMusicRuntimeProbe.EncodeObserved(int.MaxValue,"RebornAlternate"),Words(0,int.MaxValue,16,1,InstanceHandle.GetInstanceId("RebornAlternate")),Words(8,uint.MaxValue));
        Equal(PathMusicRuntimeProbe.FromHeader(header,"RebornHeaderProbe").Chunk,prepared.Chunk.InstanceBuffer,prepared.Chunk.RelocationBuffer);
        Require(header.SequenceEqual(copy) && prepared.EventValue == 1,"Caller header mutated.");
        // Reborn: output mutation and later input edits must not corrupt an already detached prepared chunk.
        var other = PathMusicRuntimeProbe.FromHeader(header,"RebornHeaderProbe"); other.Chunk.InstanceBuffer[4] = 99;
        header[^1] = (byte)'2'; Require(prepared.Chunk.InstanceBuffer[4] == 1 && PathMusicRuntimeProbe.FromHeader(header,"RebornHeaderProbe").EventValue == 2,"Prepared output/input snapshots alias mutable callers.");
        foreach (string invalid in new[] { "", "#define PATH_EVENT_RebornHeaderProbe 0x0", "// #define PATH_EVENT_RebornHeaderProbe 0x1",
            "#define PATH_EVENT_RebornHeaderProbe 0x1\n#define PATH_EVENT_RebornHeaderProbe 0x2",
            "#define PATH_EVENT_RebornHeaderProbe 0x1suffix", "#define PATH_EVENT_RebornHeaderProbe 0x80000000", "#define PATH_EVENT_RebornHeaderProbe 0xZZ" })
            Refuses(() => PathMusicRuntimeProbe.FromHeader(Encoding.ASCII.GetBytes(invalid),"RebornHeaderProbe"));
        Refuses(() => PathMusicRuntimeProbe.EncodeObserved(uint.MaxValue));
        Refuses(() => PathMusicRuntimeProbe.FromHeader(copy,"RebornHeaderProbe",""));
        Refuses(() => PathMusicRuntimeProbe.FromHeader(copy,"RebornHeaderProbe","PathMusicEvent:Other"));
        XmlSchemaSet? schemas = null; var schema = SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:set => schemas = set);
        Require(schema.SchemaAdmitted && schemas != null,"Reviewed runtime schema unavailable.");
        // Reborn: the runtime type exists but is intentionally not an authored AssetDeclaration root; never widen the official declaration choice.
        Require(!SdkEffectiveSchema.Bind(schemas!,"owned-runtime.xml",Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'><PathMusicEventRuntime id='RebornHeaderProbe' EventNameHash='1'/></AssetDeclaration>")).XmlValidated,"Runtime output type silently became an authored source root.");
        Require(SdkEffectiveSchema.Bind(schemas!,"owned-authored.xml",Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'><PathMusicEvent id='RebornHeaderProbe' PathfinderEventHeader='AUDIO:RebornProbe.h'/></AssetDeclaration>")).XmlValidated,"Authored source root/header contract differs.");
        // Reborn: add a test-only in-memory element referring to the unchanged official runtime type, not a production schema edit.
        XmlSchemaSet wrapper = new() { XmlResolver = null }; wrapper.Add(schemas!);
        using (StringReader text = new("<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns:ea='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset'><xs:element name='RebornRuntimeProbe' type='ea:PathMusicEventRuntime'/></xs:schema>"))
        using (XmlReader reader = XmlReader.Create(text,new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit,XmlResolver = null })) wrapper.Add("uri:ea.com:eala:asset",reader);
        wrapper.Compile();
        Require(ValidateRuntimeType(wrapper,"EventNameHash='1'",true) && ValidateRuntimeType(wrapper,"EventNameHash='1' RestartAlternateEvent='RebornAlternate' IsCacheable='false'",false),"Runtime type fixture differs from reviewed EP1 schema.");
        Require(!ValidateRuntimeType(wrapper,"",true),"Required runtime event word omitted.");
        Console.WriteLine("PathMusic runtime probe self-test: OK (actual Win32 tracker, independent 16/20-byte goldens and relocation/no imports/cache padding, strict unique nonzero header, immutable snapshots, missing/zero/duplicate/lexical/range/alternate refusals, reviewed runtime schema; no processor/stream admission)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate a test-only element against the real runtime type and confirm default cache injection without modifying official schemas. */
    //-------------------------------------------------------------------------------------------------
    private static bool ValidateRuntimeType(XmlSchemaSet schemas,string attributes,bool cacheable)
    {
        XmlDocument xml = new() { XmlResolver = null,Schemas = schemas }; xml.LoadXml("<RebornRuntimeProbe xmlns='uri:ea.com:eala:asset' id='RebornHeaderProbe' "+attributes+"/>");
        int errors = 0; xml.Validate((_,args) => { if (args.Severity == XmlSeverityType.Error) errors++; });
        return errors == 0 && xml.DocumentElement!.GetAttribute("IsCacheable") == (cacheable ? "true" : "false");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: assemble fixture bytes independently of tracker allocation and serialization. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Words(params uint[] values)
    {
        byte[] bytes = new byte[values.Length*4]; for (int index = 0; index < values.Length; index++) BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(index*4,4),values[index]); return bytes;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare every native byte and auxiliary sentinel, not only selected fields. */
    //-------------------------------------------------------------------------------------------------
    private static void Equal(Chunk chunk,byte[] bin,byte[] relo) => Require(chunk.InstanceBuffer.SequenceEqual(bin) && chunk.RelocationBuffer.SequenceEqual(relo) && chunk.ImportsBuffer.Length == 0,"Independent music golden differs.");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: unsupported authored input must stop rather than receive stock-derived event constants. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuses(Action action)
    {
        try { action(); } catch (InvalidDataException) { return; }
        throw new InvalidDataException("Unsupported isolated music input accepted.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop on native layout, snapshot or schema evidence mismatch. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
