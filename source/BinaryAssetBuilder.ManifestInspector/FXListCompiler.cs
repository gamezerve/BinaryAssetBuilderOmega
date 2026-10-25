using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
using Chunk=Relo.Chunk;
using Tracker=Relo.Tracker;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: compile general authored FXList assets with the existing native nugget writers and independent source/catalog proof.
internal static class FXListCompiler
{
    private static readonly Type[] Types={typeof(ForceFeedbackFXNugget),typeof(EvaEventFXNugget),typeof(SoundFXNugget),typeof(RayEffectFXNugget),typeof(LightPulseFXNugget),typeof(DynamicDecalFXNugget),typeof(LaserFXNugget),typeof(CameraShakerVolumeFXNugget),typeof(ViewShakeFXNugget),typeof(AttachedModelFXNugget),typeof(TerrainScorchFXNugget),typeof(TintDrawableFXNugget),typeof(FXListAtBonePosFXNugget),typeof(FXParticleSysBoneNugget),typeof(ParticleSystemFXNugget)};

    //-------------------------------------------------------------------------------------------------
    /** Reborn: only concrete official FX nugget types with a real writer may allocate records; unimplemented types fail at their source node. */
    //-------------------------------------------------------------------------------------------------
    internal static Type NuggetType(XmlElement element)
    {
        string name=element.SchemaInfo.SchemaType?.QualifiedName.Name??"";return Types.SingleOrDefault(t=>t.Name==name)??throw SdkXmlDiagnostics.At(new InvalidDataException("FXList nugget writer is not implemented: "+element.LocalName+" ("+name+")."),element);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: account for the retail particle InitialDelay sentinel before validating the independent proof copy; the source and produced asset remain unchanged. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlDocument ProofSource(XmlDocument source,SchemaSet schemas,ManifestAsset[] catalog)
    {
        var copy=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.DocumentElement!.OuterXml));
        foreach(var particle in copy.GetElementsByTagName("ParticleSystem",SchemaSet.XmlNamespace).OfType<XmlElement>())if(!particle.ChildNodes.OfType<XmlElement>().Any(e=>e.LocalName=="InitialDelay")){
            var delay=copy.CreateElement("InitialDelay",SchemaSet.XmlNamespace);delay.SetAttribute("Low","-1");delay.SetAttribute("High","-1");delay.SetAttribute("Type","CONSTANT");particle.AppendChild(delay);
        }
        var proof=SchemaNativeContract.Resolve(copy,schemas,catalog);foreach(var nugget in proof.DocumentElement!.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="NuggetList").SelectMany(e=>e.ChildNodes.OfType<XmlElement>()))_=NuggetType(nugget);proof.DocumentElement.RemoveAttribute("id");return proof;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Core supplies all TypeId and import selectors before one native FXList Tracker allocation. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlNode source)
    {
        var xml=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.OuterXml));
        // Reborn: select concrete nugget identities from validated element names; never trust authored TypeId attributes or silently allocate a null class.
        foreach(var wrapper in xml.GetElementsByTagName("NuggetList",SchemaSet.XmlNamespace).OfType<XmlElement>())foreach(var nugget in wrapper.ChildNodes.OfType<XmlElement>()){
            string name=nugget.LocalName=="FXParticleSysBone"?"FXParticleSysBoneNugget":nugget.LocalName+"FXNugget";Type native=Types.SingleOrDefault(t=>t.Name==name)??throw SdkXmlDiagnostics.At(new InvalidDataException("FX nugget writer is absent: "+nugget.LocalName),nugget);nugget.SetAttribute("TypeId",FastHash.GetHashCode(native.Name).ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
        var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);FXList* root;using var tracker=new Tracker((void**)&root,(uint)sizeof(FXList),false);Marshaler.Marshal(new Node(xml.DocumentElement!.CreateNavigator()!,ns),root,tracker);var chunk=new Chunk();if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("FXList native allocation failed.");return chunk;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare native/source interpretation against original empty, sound and particle/filter FXList assets using bounded linked-stream reads. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(string manifest,SchemaSet schemas,ManifestAsset[] catalog)
    {
        var doc=ManifestReader.Read(SdkEnvironmentPreflight.Read(manifest,64*1048576));TypeRegistryAudit.ValidateTarget(doc.Header.Version,doc.Header.AllTypesHash);if(!doc.Header.IsLinked||doc.Validate().Count!=0)throw new InvalidDataException("Valid linked FXList stock stream required.");var header=AssetStreamProbe.ReadRange(Path.ChangeExtension(manifest,".bin"),null,0,8);if(BinaryPrimitives.ReadUInt32LittleEndian(header)!=0xBABB0000||BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(4))!=doc.Header.StreamChecksum)throw new InvalidDataException("FXList stock stream checksum differs.");int count=0;
        foreach(var witness in new[]{("GlobalData/FXList.xml","FX_NONE"),("GlobalData/FXList.xml","FX_DebrisHitGround"),("GlobalData/FXListSoviet.xml","FX_SOV_LargeBearBloodImpact"),("GlobalData/FXList.xml","FX_VehicleCrush"),("GlobalData/FXList.xml","FX_Neutral_FlingableExplodingBarrelExplosion"),("GlobalData/FXListCivilian.xml","FX_SpecialDebrisExplosionShake_NoAudio"),("GlobalData/FXListJapan.xml","FX_JAP_ShurikenGroundHit"),("GlobalData/FXListAllied.xml","FX_ALL_CenturyBomber_BombExplosion")}){
            var original=GameObjectExtendedRootContract.Original(witness.Item1,"FXList").Single(e=>e.GetAttribute("id")==witness.Item2);var source=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(original.OuterXml));var proof=ProofSource(source,schemas,catalog);var asset=doc.Assets.Single(a=>a.Name=="FXList:"+witness.Item2);if(asset.Tokenized!=0||asset.TypeHash!=0x17B3B82D)throw new InvalidDataException("FXList stock fingerprint differs.");long offset=8;foreach(var a in doc.Assets){if(ReferenceEquals(a,asset))break;offset+=a.InstanceDataSize;}
            var bytes=AssetStreamProbe.ReadRange(Path.ChangeExtension(manifest,".bin"),null,offset,asset.InstanceDataSize);SchemaNativeContract.VerifyStock(proof.DocumentElement!,typeof(FXList),0,bytes,0,asset.References.ToArray());count++;
        }
        SdkBuildTests.MetadataProgress("FXList",$"Reborn: FXList stock native qualification: PASS ({count} original records).",count+" stock records");
    }
}
