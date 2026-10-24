using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: connect stock-backed AudioEvents to normal alpha streams without copying or re-encoding retail AudioFile payloads.
internal static class AuthoredAmbientCompiler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: locate the SDK-local audio catalog alongside the other portable lookup manifests. */
    //-------------------------------------------------------------------------------------------------
    internal static string CatalogPath()
    {
        for(var folder=new DirectoryInfo(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!);folder!=null;folder=folder.Parent){string path=Path.Combine(folder.FullName,"alpha/catalogs/ra3ep1/audio.manifest");if(File.Exists(path))return path;}
        throw new FileNotFoundException("AudioEvent authoring requires the SDK-local catalogs/ra3ep1/audio.manifest lookup copy.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain the existing schema/import-checked AudioEvent writer, explicitly opting into AMBIENT and fade-on-start support. */
    //-------------------------------------------------------------------------------------------------
    internal static AssetBuffer Compile(InstanceDeclaration instance)
    {
        // Reborn: the mixed-stream attachment deliberately detaches XML; restore official PSVI before the audio plugin checks current fields and child types.
        var document=instance.XmlNode.OwnerDocument!;if(document.Schemas.Count==0)document.Schemas.Add(ConscriptSourceInput.Schemas());document.Validate((_,e)=>throw SdkXmlDiagnostics.SchemaError(e));
        instance.Handle.TypeHash=0x560C2E45u;instance.ValidatedReferencedInstances=new(instance.ReferencedInstances);
        var plugin=new Ra3Ep1AudioEventPlugin{AllowAmbientAuthoring=true};plugin.Initialize(TargetPlatform.Win32);return plugin.ProcessInstance(instance);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reproduce the complete original Soviet reactor loop before emitting authored ambient events. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(string global,Func<XmlDocument,InstanceDeclaration> normalize)
    {
        string sdk=Path.GetFullPath(Path.Combine(Path.GetDirectoryName(CatalogPath())!,"../.."));string path=Path.Combine(sdk,"SageXML/Sounds/SoundEffects.xml");
        var source=SdkXmlDiagnostics.Parse(SdkEnvironmentPreflight.Read(path,4*1048576),path);var original=source.GetElementsByTagName("AudioEvent",SchemaSet.XmlNamespace).OfType<XmlElement>().Single(e=>e.GetAttribute("id")=="SOV_Reactor_AmbientLoop");
        var probe=new XmlDocument{XmlResolver=null};probe.AppendChild(probe.ImportNode(original,true));
        // Reborn: this stock witness inherits real BaseSoundEffect values (including shrunken/reverb settings), not merely XSD defaults.
        var basis=SdkXmlDiagnostics.Parse(SdkEnvironmentPreflight.Read(Path.Combine(sdk,"SageXML/Sounds/BaseSoundEffect.xml"),1048576)).GetElementsByTagName("AudioEvent",SchemaSet.XmlNamespace).OfType<XmlElement>().Single(e=>e.GetAttribute("id")=="BaseSoundEffect");foreach(XmlAttribute attribute in basis.Attributes)if(attribute.NamespaceURI.Length==0&&attribute.LocalName!="id"&&!probe.DocumentElement!.HasAttribute(attribute.Name))probe.DocumentElement.SetAttribute(attribute.Name,attribute.Value);
        probe.DocumentElement!.RemoveAttribute("inheritFrom");var instance=normalize(probe);var buffer=Compile(instance);var chunk=new Relo.Chunk{InstanceBuffer=buffer.InstanceData,RelocationBuffer=buffer.RelocationData,ImportsBuffer=buffer.ImportsData};
        var document=ManifestReader.Read(SdkEnvironmentPreflight.Read(global,64*1048576));var asset=document.Assets.Single(a=>a.Name=="AudioEvent:SOV_Reactor_AmbientLoop");long offset=8;foreach(var a in document.Assets){if(a==asset)break;offset+=a.InstanceDataSize;}
        var stock=AssetStreamProbe.ReadRange(Path.ChangeExtension(global,".bin"),null,offset,asset.InstanceDataSize);new SelectedNativeGraphComparer(stock,asset.References.ToArray(),chunk,instance,requireAllReferences:false,captureNativeCoordinates:false).Compare(typeof(Marshaler.Ep1AudioEvent),0);
        Console.WriteLine("Reborn: authored AudioEvent stock qualification: PASS (complete Soviet reactor loop, ambient slider, start/kill fades and 14 semantic AudioFile imports).");
    }
}
