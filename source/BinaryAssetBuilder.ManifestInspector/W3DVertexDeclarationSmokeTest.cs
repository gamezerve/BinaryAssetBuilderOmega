using System.Buffers.Binary;
using System.Reflection;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: exercise the actual custom mesh against EA's compiled witness, including the declaration's out-of-count terminator.
internal static class W3DVertexDeclarationSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile a read-only source mesh into a private output and require exact EA native data and import/relocation layout. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string source,string manifest,string witness,string output)
    {
        if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh vertex-declaration evidence directory required.");
        Directory.CreateDirectory(output);
        Settings.Current=new Settings{SchemaPath=ReferencePipelineSmokeTest.FindFixture(),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
        var schemas=new SchemaSet(false);
        foreach(var seed in schemas.Schemas.Schemas().Cast<System.Xml.Schema.XmlSchema>().ToArray())schemas.Schemas.Remove(seed);
        schemas.Schemas.Add(ConscriptSourceInput.Schemas());schemas.Schemas.Compile();
        typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.NonPublic|BindingFlags.Instance)!.Invoke(schemas,null);
        var original=SdkXmlDiagnostics.Parse(SdkEnvironmentPreflight.Read(source,16*1048576));
        var mesh=original.GetElementsByTagName("W3DMesh",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
        var doc=new XmlDocument();doc.AppendChild(doc.ImportNode(mesh,true));
        var catalog=ManifestReader.Read(SdkEnvironmentPreflight.Read(manifest,64*1048576)).Assets.ToArray();
        ConscriptBehaviorSurvey.ResolveStrong(doc,schemas,catalog);
        var instance=ReferencePipelineSmokeTest.Normalize(schemas,doc.OuterXml);
        var chunk=W3DGeometryCompiler.Compile(instance,schemas);
        File.WriteAllBytes(Path.Combine(output,"mesh.bin"),chunk.InstanceBuffer);
        File.WriteAllBytes(Path.Combine(output,"mesh.relo"),chunk.RelocationBuffer);
        File.WriteAllBytes(Path.Combine(output,"mesh.imp"),chunk.ImportsBuffer);
        var bytes=chunk.InstanceBuffer;
        int data=checked((int)BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(4,4)));
        int length=checked((int)BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(data+12,4)));
        int at=checked((int)BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(data+16,4)));
        if(at<0||length<0||at+length>=bytes.Length||bytes[at+length]!=0)
            throw new InvalidDataException("Compiled vertex declaration has no allocated NUL terminator after its counted text.");
        var expected=SdkEnvironmentPreflight.Read(witness,4*1048576);
        if(!bytes.SequenceEqual(expected))throw new InvalidDataException("Compiled mesh differs from the EA native witness.");
        foreach(var pair in new[]{(".relo",chunk.RelocationBuffer),(".imp",chunk.ImportsBuffer)})
            if(!pair.Item2.SequenceEqual(File.ReadAllBytes(Path.ChangeExtension(witness,pair.Item1))))
                throw new InvalidDataException("Mesh "+pair.Item1+" differs from the EA witness.");
        Console.WriteLine("Reborn: vertex declaration PASS (allocated NUL after "+length+" counted bytes; complete BIN/RELO/IMP equal to EA witness; no user build or game).");
    }
}
