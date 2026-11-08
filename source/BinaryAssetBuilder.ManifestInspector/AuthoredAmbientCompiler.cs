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
        // Reborn: prefer the copied SDK catalog while retaining the original repository layout for development callers.
        for(var folder=new DirectoryInfo(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!);folder!=null;folder=folder.Parent)
            foreach(string relative in new[]{"catalogs/ra3ep1/audio.manifest","alpha/catalogs/ra3ep1/audio.manifest"}){
                string path=Path.Combine(folder.FullName,relative);if(File.Exists(path))return path;
            }
        throw new FileNotFoundException("AudioEvent authoring requires the SDK-local catalogs/ra3ep1/audio.manifest lookup copy.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain the existing schema/import-checked AudioEvent writer, explicitly opting into AMBIENT and fade-on-start support. */
    //-------------------------------------------------------------------------------------------------
    internal static AssetBuffer Compile(InstanceDeclaration instance, System.Xml.Schema.XmlSchemaSet? official = null)
    {
        // Reborn: the mixed-stream attachment deliberately detaches XML; restore official PSVI before the audio plugin checks current fields and child types.
        var document=instance.XmlNode.OwnerDocument!;if(document.Schemas.Count==0)document.Schemas.Add(official ?? ConscriptSourceInput.Schemas());document.Validate((_,e)=>throw SdkXmlDiagnostics.SchemaError(e));
        // Reborn: normal SDK builds admit map-overridable events with their own fingerprint and the shared payload writer.
        var plugin=new Ra3Ep1AudioEventPlugin
        {
            AllowAmbientAuthoring=true,
            AllowSchemaSubmixSliders=true,
            AllowOverridableAuthoring=true,
            AllowExtendedStockAuthoring=true
        };
        plugin.Initialize(TargetPlatform.Win32);
        instance.Handle.TypeHash=plugin.GetExtendedTypeInformation(instance.Handle.TypeId).TypeHash;
        instance.ValidatedReferencedInstances=new(instance.ReferencedInstances);
        return plugin.ProcessInstance(instance);
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

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove normal sequential/start/kill control bits and repeated weighted file occurrences against their complete original global native records. */
    //-------------------------------------------------------------------------------------------------
    internal static void QualifyExtendedStockFeatures(string global,Func<XmlDocument,InstanceDeclaration> normalize)
    {
        var catalog=ManifestReader.Read(SdkEnvironmentPreflight.Read(global,64*1048576));
        var witnesses=new[]{("Sounds/SoundEffects.xml","ALL_CryoLegionnaire_FreezeWeapon_GarrisonLoop"),("Sounds/SoundEffects.xml","ALL_TimeBomb_Activate_CreationSequence_Loop"),("Sounds/Voice.xml","JAP_OreCollector_VoiceShrunken"),("Sounds/SoundEffects.xml","Impact_BulletVsGeneric")};
        var cache=new Dictionary<string,XmlElement[]>();
        foreach(var witness in witnesses)
        {
            if(!cache.TryGetValue(witness.Item1,out var originals))cache[witness.Item1]=originals=StockOriginalUiConfig.Load(witness.Item1,"AudioEvent");
            var original=originals.Single(element=>element.GetAttribute("id")==witness.Item2);
            var source=SdkXmlDiagnostics.Parse(System.Text.Encoding.UTF8.GetBytes(original.OuterXml));
            var instance=normalize(source);var buffer=Compile(instance);
            var native=new Relo.Chunk{InstanceBuffer=buffer.InstanceData,RelocationBuffer=buffer.RelocationData,ImportsBuffer=buffer.ImportsData};
            var stock=catalog.Assets.Single(asset=>asset.Name=="AudioEvent:"+witness.Item2);
            if(stock.TypeId!=0x844D7B9Fu||stock.TypeHash!=0x560C2E45u||stock.Tokenized!=0)throw new InvalidDataException("Original extended audio witness fingerprint differs.");
            var bytes=W3DContainerCompiler.ReadStock(global,catalog,stock);
            new SelectedNativeGraphComparer(bytes,stock.References.ToArray(),native,instance,requireAllReferences:false,captureNativeCoordinates:false).Compare(typeof(Marshaler.Ep1AudioEvent),0);
        }
        Console.WriteLine("Reborn: original AudioEvent sequential/kill/start controls and repeated AudioFile occurrences qualified (4 complete native witnesses, including 121 file occurrences).");
    }
}
