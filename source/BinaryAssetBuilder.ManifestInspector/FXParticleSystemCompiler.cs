using System.Buffers.Binary;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
using Chunk=Relo.Chunk;
using Tracker=Relo.Tracker;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: compile authored particle templates through the real native writers and independently verify their source-owned graphs.
internal static class FXParticleSystemCompiler
{
    private static readonly string[] Roles={"Physics","Draw","Volume","Velocity","Event","Update"};

    //-------------------------------------------------------------------------------------------------
    /** Reborn: recognize only the native particle-module ancestry, never an arbitrary reflected runtime structure. */
    //-------------------------------------------------------------------------------------------------
    internal static bool IsModule(Type type)
    {
        for(int i=0;i<16;i++){if(type==typeof(FXParticleBaseModule))return true;var parent=type.GetField("Base");if(parent==null)return false;type=parent.FieldType;}return false;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: select concrete modules from their validated schema declarations and require a real native writer. */
    //-------------------------------------------------------------------------------------------------
    internal static Type ModuleType(XmlElement source)
    {
        string name=source.SchemaInfo.SchemaType?.QualifiedName.Name??"";var type=typeof(FXParticleSystemTemplate).Assembly.GetType("SageBinaryData."+name);
        if(type==null||!IsModule(type)||typeof(Marshaler).GetMethod("Marshal",new[]{typeof(Node),type.MakePointerType(),typeof(Tracker)})==null)throw SdkXmlDiagnostics.At(new InvalidDataException("Particle module writer is not implemented: "+source.LocalName+" ("+name+")."),source);return type;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: materialize the two schema-initialized damping records in a proof copy without changing authored files or the produced payload. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlDocument ProofSource(XmlDocument source,SchemaSet schemas,ManifestAsset[] catalog)
    {
        var copy=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.DocumentElement!.OuterXml));var update=copy.DocumentElement!.ChildNodes.OfType<XmlElement>().SingleOrDefault(e=>e.LocalName=="Update")?.ChildNodes.OfType<XmlElement>().SingleOrDefault(e=>e.LocalName=="Default");
        if(update!=null)foreach(string name in new[]{"SizeRateDamping","AngularDampingXY"})if(!update.ChildNodes.OfType<XmlElement>().Any(e=>e.LocalName==name)){var value=copy.CreateElement(name,SchemaSet.XmlNamespace);value.SetAttribute("Low","1");value.SetAttribute("High","1");var order=new[]{"SizeRate","SizeRateDamping","AngleZ","AngularRateZ","AngularDamping","AngleXY","AngularRateXY","AngularDampingXY"};var next=update.ChildNodes.OfType<XmlElement>().FirstOrDefault(e=>Array.IndexOf(order,e.LocalName)>Array.IndexOf(order,name));update.InsertBefore(value,next);}
        var proof=SchemaNativeContract.Resolve(copy,schemas,catalog);foreach(var role in proof.DocumentElement!.ChildNodes.OfType<XmlElement>().Where(e=>Roles.Contains(e.LocalName)))foreach(var module in role.ChildNodes.OfType<XmlElement>())_=ModuleType(module);proof.DocumentElement.RemoveAttribute("id");return proof;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve Core import indices while deriving all polymorphic particle class IDs from the official schema. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlNode source,XmlSchemaSet schemas)
    {
        var xml=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.OuterXml));foreach(var attribute in xml.SelectNodes("//@TypeId")!.Cast<XmlAttribute>().ToArray())attribute.OwnerElement!.RemoveAttributeNode(attribute);xml.Schemas.Add(schemas);xml.Validate((_,e)=>throw SdkXmlDiagnostics.SchemaError(e));
        foreach(var role in xml.DocumentElement!.ChildNodes.OfType<XmlElement>().Where(e=>Roles.Contains(e.LocalName)))foreach(var module in role.ChildNodes.OfType<XmlElement>())module.SetAttribute("TypeId",FastHash.GetHashCode(ModuleType(module).Name).ToString(System.Globalization.CultureInfo.InvariantCulture));
        var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);FXParticleSystemTemplate* root;using var tracker=new Tracker((void**)&root,(uint)sizeof(FXParticleSystemTemplate),false);Marshaler.Marshal(new Node(xml.DocumentElement.CreateNavigator()!,ns),root,tracker);var chunk=new Chunk();if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("Particle native allocation failed.");return chunk;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bind the GPU path to the original complete Tiny particle graph through selected checksum-bound native bytes. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(string manifest,SchemaSet schemas,ManifestAsset[] catalog)
    {
        if(Marshal.SizeOf<FXParticleSystemTemplate>()!=176||Marshal.SizeOf<FXParticleDrawGpu>()!=60)throw new InvalidDataException("EP1 particle root/GPU header layout differs.");
        var doc=ManifestReader.Read(SdkEnvironmentPreflight.Read(manifest,64*1048576));TypeRegistryAudit.ValidateTarget(doc.Header.Version,doc.Header.AllTypesHash);if(!doc.Header.IsLinked||doc.Validate().Count!=0)throw new InvalidDataException("Valid linked particle stock stream required.");var header=AssetStreamProbe.ReadRange(Path.ChangeExtension(manifest,".bin"),null,0,8);if(BinaryPrimitives.ReadUInt32LittleEndian(header)!=0xBABB0000||BinaryPrimitives.ReadUInt32LittleEndian(header.AsSpan(4))!=doc.Header.StreamChecksum)throw new InvalidDataException("Particle stock checksum differs.");
        const string id="Civ_Destruction_Tiny";var original=GameObjectExtendedRootContract.Original("FXParticleSystems/"+id+".xml","FXParticleSystemTemplate").Single();var source=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(original.OuterXml));var proof=ProofSource(source,schemas,catalog);var asset=doc.Assets.Single(a=>a.Name=="FXParticleSystemTemplate:"+id);if(asset.Tokenized!=0)throw new InvalidDataException("Tokenized particle layout is not supported.");long offset=8;foreach(var entry in doc.Assets){if(ReferenceEquals(entry,asset))break;offset+=entry.InstanceDataSize;}var bytes=AssetStreamProbe.ReadRange(Path.ChangeExtension(manifest,".bin"),null,offset,asset.InstanceDataSize);SchemaNativeContract.VerifyStock(proof.DocumentElement!,typeof(FXParticleSystemTemplate),0,bytes,0,asset.References.ToArray());
        SdkBuildTests.MetadataProgress("FXParticleSystemTemplate","Reborn: particle GPU stock native qualification: PASS (complete Civ_Destruction_Tiny graph).","original GPU/physics/box/ortho/update graph");
    }
}
