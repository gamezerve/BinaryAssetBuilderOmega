using System.Buffers.Binary;
using System.Xml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: exercise actual authored camera input and native global publication without rebuilding the user's Reborn mod.
internal static class CameraSettingsSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify retail bytes, edited/omitted values, attribute ordering and an independently read-back early-global stream. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global,string source,string output)
    {
        if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh camera evidence directory required.");
        Directory.CreateDirectory(output);CameraSettingsCompiler.Qualify(global);
        var xml=SdkXmlDiagnostics.Parse(SdkEnvironmentPreflight.Read(source,1048576));
        var camera=xml.GetElementsByTagName("CameraSettings","uri:ea.com:eala:asset").OfType<XmlElement>().Single();
        var native=CameraSettingsCompiler.Compile(camera);
        if(native.InstanceBuffer.Length!=84||native.RelocationBuffer.Length!=0||native.ImportsBuffer.Length!=0)
            throw new InvalidDataException("Camera native record differs.");
        foreach(string field in new[]{"DefaultMinHeight","DefaultMaxHeight"}){
            int offset=System.Runtime.InteropServices.Marshal.OffsetOf<CameraSettings>(field).ToInt32();
            if(BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32LittleEndian(native.InstanceBuffer.AsSpan(offset,4)))!=XmlConvert.ToSingle(camera.GetAttribute(field)))throw new InvalidDataException("Authored camera value differs: "+field);
        }
        var omitted=(XmlElement)camera.CloneNode(true);omitted.RemoveAttribute("DefaultMaxHeight");
        if(BinaryPrimitives.ReadUInt32LittleEndian(CameraSettingsCompiler.Compile(omitted).InstanceBuffer.AsSpan(8,4))!=0)throw new InvalidDataException("Omitted camera attribute did not remain zero-initialized.");
        var reversed=(XmlElement)camera.CloneNode(false);reversed.RemoveAll();foreach(XmlAttribute attribute in camera.Attributes.Cast<XmlAttribute>().Reverse())reversed.SetAttribute(attribute.LocalName,attribute.Value);
        if(!CameraSettingsCompiler.Compile(reversed).InstanceBuffer.SequenceEqual(native.InstanceBuffer))throw new InvalidDataException("Camera attribute ordering changed native storage.");
        string entry=Path.Combine(output,"MapMetaData_Global.xml");File.WriteAllBytes(entry,File.ReadAllBytes(source));
        string prepared=Path.Combine(output,"prepared");ConscriptSourceInput.Prepare(entry,prepared);
        if(!ConscriptMultiAssetBuild.TryRun(Path.Combine(prepared,"game.xml"),manifest,global,Path.Combine(output,"built"),earlyGlobal:true))throw new InvalidDataException("Camera global route not selected.");
        string linked=Directory.GetFiles(Path.Combine(output,"built","stream-readback"),"*.manifest").Single();
        var doc=ManifestReader.Read(File.ReadAllBytes(linked));var asset=doc.Assets.Single(a=>a.Name=="CameraSettings:"+camera.GetAttribute("id"));
        if(asset.TypeId!=0xE862FF86u||asset.TypeHash!=0xFDCC69DEu||asset.Tokenized!=0||asset.InstanceDataSize!=84||asset.References.Count!=0)
            throw new InvalidDataException("Camera linked fingerprint differs.");
        if(!W3DContainerCompiler.ReadStock(linked,doc,asset).SequenceEqual(native.InstanceBuffer))throw new InvalidDataException("Camera authored fields changed in stream publication.");
        Console.WriteLine("Reborn: CameraSettings PASS (retail 84-byte global witness; authored heights, omission, attribute order and actual early-global package/readback; no user build or game).");
    }
}
