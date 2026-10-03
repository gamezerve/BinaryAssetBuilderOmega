using System.Buffers.Binary;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: lock reference-proven text block boundaries before attempting any production audio InstanceHash match.
internal static class HashingWriterBoundarySmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: read pinned reference IL without loading assemblies, then compare current writer/XML hashes with independent block folding. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        ReferenceEvidence();
        Type type = typeof(HashProvider).Assembly.GetType("BinaryAssetBuilder.Core.Hashing.HashingWriter",true)!;
        foreach (int length in new[] { 0,1,511,512,513,1023,1024,1025,1536,2048 })
        {
            string text = new('x',length); uint expected = Fold(0x12345678u,text);
            foreach (int chunk in new[] { 1,7,511,512,513,4096 })
            {
                using TextWriter writer = (TextWriter)Activator.CreateInstance(type,new object[] { 0x12345678u })!;
                for (int offset = 0; offset < length; offset += chunk) writer.Write(text.Substring(offset,Math.Min(chunk,length-offset)));
                writer.Flush(); uint actual = (uint)type.GetMethod("GetFinalHash")!.Invoke(writer,null)!;
                Require(actual == expected,$"HashingWriter boundary differs: length={length}, writeChunk={chunk}, expected={expected:X8}, actual={actual:X8}.");
                Require((uint)type.GetMethod("GetFinalHash")!.Invoke(writer,null)! == actual,"Repeated finalization changed the text hash.");
            }
        }
        // Reborn: exact-size final-block edits must contribute, instead of colliding with the old dropped-block result.
        Require(Fold(0x12345678u,new string('x',512)) != 0x12345678u,"Independent block fold unexpectedly equals the seed.");
        foreach (int padding in new[] { 450,451,452,500,962,963,964,1500 })
        {
            XmlDocument document = new() { XmlResolver = null }; document.LoadXml("<Probe value=\""+new string('x',padding)+"\" />");
            XmlNode node = document.DocumentElement!;
            using EncodedStringWriter text = new(); using (XmlWriter writer = XmlWriter.Create(text)) { node.WriteTo(writer); writer.Flush(); }
            Require(HashProvider.GetXmlHash(0x12345678u,ref node) == Fold(0x12345678u,text.ToString()),"XML serialization/hash block folding differs.");
        }
        Console.WriteLine($"HashingWriter boundary self-test: OK (pinned reference IL inclusive 512-character blocks; 60 length/write partitions; XML folding; current document version {DocumentProcessor.Version}; no reference/native execution or production hash claim)");
    }

    // Reborn: use the same declared text encoding while independently capturing XmlWriter's actual serialized text.
    private sealed class EncodedStringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.Default;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: fold every complete or final partial character block exactly once, independent of writer call boundaries. */
    //-------------------------------------------------------------------------------------------------
    private static uint Fold(uint seed,string text)
    {
        for (int offset = 0; offset < text.Length; offset += 512) seed = HashProvider.GetTextHash(seed,text.Substring(offset,Math.Min(512,text.Length-offset)));
        return seed;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bound and pin official reference metadata before interpreting exact IL offsets; do not execute old mixed-mode code. */
    //-------------------------------------------------------------------------------------------------
    private static void ReferenceEvidence()
    {
        string root = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"../.."));
        string tools = Path.Combine(root,"Working RA3 Compiler for Reference","tools");
        using FileStream coreStream = Pinned(Path.Combine(tools,"BinaryAssetBuilder.Core.dll"),"B36D5015F97532457D64072034F9F5C629A4C060A2D34C49C4F89FD6E6F79E47");
        using PEReader core = new(coreStream); MetadataReader metadata = core.GetMetadataReader();
        byte[] block = Body(core,metadata,"BinaryAssetBuilder.Core.HashingWriter","Write",158);
        byte[] init = Body(core,metadata,"BinaryAssetBuilder.Core.HashingWriter",".cctor",11);
        byte[] validate = Body(core,metadata,"BinaryAssetBuilder.Core.AssetDeclarationDocument","ValidateInstances",757);
        Require(block[0x6A] == 0x31 && init[0] == 0x20 && BinaryPrimitives.ReadInt32LittleEndian(init.AsSpan(1)) == 512,"Reference HashingWriter no longer has the observed inclusive block boundary.");
        Require(validate[0x61] == 0x1F && validate[0x62] == 11,"Reference identity seed document version differs.");
        var platform = metadata.TypeDefinitions.Select(handle => metadata.GetTypeDefinition(handle)).Single(type => metadata.GetString(type.Namespace) == "BinaryAssetBuilder.Core" && metadata.GetString(type.Name) == "TargetPlatform");
        var field = platform.GetFields().Select(handle => metadata.GetFieldDefinition(handle)).Single(value => metadata.GetString(value.Name) == "Win32");
        var constant = metadata.GetConstant(field.GetDefaultValue());
        Require(metadata.GetBlobReader(constant.Value).ReadInt32() == 0,"Reference platform branch is not Win32=0.");
        using FileStream audioStream = Pinned(Path.Combine(tools,"BinaryAssetBuilder.AudioCompiler.dll"),"A14D15ADDEF502C2DF6F6BA272D6098C4C131FBEA9C3ED0149C11C4556347139");
        using PEReader audio = new(audioStream); byte[] info = Body(audio,audio.GetMetadataReader(),"BinaryAssetBuilder.AudioCompiler.Plugin","GetAudioFileExtendedTypeInformation",207);
        Require(info[0x40] == 0x20 && BinaryPrimitives.ReadUInt32LittleEndian(info.AsSpan(0x41)) == 0x8FE79286u
            && info[0x65] == 0x20 && BinaryPrimitives.ReadUInt32LittleEndian(info.AsSpan(0x66)) == 0x53C81E47u,"Reference Win32 AudioFile processor/type fingerprints differ.");
        Console.WriteLine("  Reference identity evidence: document seed version=11, Win32=0 AudioFile ProcessingHash=8FE79286 / TypeHash=53C81E47; current experimental profiles must not be relabeled as stock.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: pin small reference DLL bytes and reset the same file handle for metadata-only inspection. */
    //-------------------------------------------------------------------------------------------------
    private static FileStream Pinned(string path,string expected)
    {
        FileStream stream = File.OpenRead(path);
        if (stream.Length > 8*1024*1024 || Convert.ToHexString(SHA256.HashData(stream)) != expected) { stream.Dispose(); throw new InvalidDataException("Reference identity evidence fingerprint differs."); }
        stream.Position = 0; return stream;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: locate the exact pinned method shape by full type/name/code length, not by loading its assembly. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Body(PEReader pe,MetadataReader reader,string typeName,string methodName,int size)
    {
        foreach (var handle in reader.TypeDefinitions)
        {
            var type = reader.GetTypeDefinition(handle);
            if (reader.GetString(type.Namespace)+"."+reader.GetString(type.Name) != typeName) continue;
            foreach (var methodHandle in type.GetMethods())
            {
                var method = reader.GetMethodDefinition(methodHandle);
                if (reader.GetString(method.Name) == methodName && method.RelativeVirtualAddress != 0)
                { byte[] bytes = pe.GetMethodBody(method.RelativeVirtualAddress).GetILBytes()!; if (bytes.Length == size) return bytes; }
            }
        }
        throw new InvalidDataException("Pinned identity evidence method shape missing.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop on the first reference/hash contract mismatch. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
