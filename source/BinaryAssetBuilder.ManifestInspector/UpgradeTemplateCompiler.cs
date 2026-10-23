using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Xml;
using System.Reflection;
using System.Text;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Utility;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify UpgradeTemplate source/native layout and untokenized stream flags against bounded original EP1 asset slices before authored output.
internal static class UpgradeTemplateCompiler
{
    internal sealed record Capture(ManifestAsset Asset,byte[] Bytes,int Native);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: read only the selected upgrade payload, bind it to the linked stream checksum and locate its root by its first native field. */
    //-------------------------------------------------------------------------------------------------
    internal static Capture Stock(string manifest,string name)
    {
        var document=ManifestReader.Read(SdkEnvironmentPreflight.Read(manifest,64*1048576));
        TypeRegistryAudit.ValidateTarget(document.Header.Version,document.Header.AllTypesHash);
        if(document.Validate().Count!=0||!document.Header.IsLinked||document.Header.ContainerPrefixSize!=4)throw new InvalidDataException("Valid linked EP1 upgrade stream required.");
        var asset=document.Assets.Single(a=>a.Name==name);if(asset.TypeId!=FastHash.GetHashCode("UpgradeTemplate")||asset.TypeHash!=0x5D55CE2C||asset.Tokenized!=0||asset.InstanceDataSize>1048576)throw new InvalidDataException("Unsupported upgrade asset fingerprint.");
        string bin=Path.ChangeExtension(manifest,".bin");var header=AssetStreamProbe.ReadRange(bin,null,0,8);if(Word(header,0)!=0xBABB0000||Word(header,4)!=document.Header.StreamChecksum)throw new InvalidDataException("Upgrade stream checksum differs.");
        long offset=8;foreach(var entry in document.Assets){if(ReferenceEquals(entry,asset))break;offset+=entry.InstanceDataSize;}
        var bytes=AssetStreamProbe.ReadRange(bin,null,offset,asset.InstanceDataSize);if(bytes.Length<Marshal.SizeOf<Ep1UpgradeTemplate>())throw new InvalidDataException("Upgrade native header is truncated.");return new(asset,bytes,0);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: show field-sized stock ABI evidence only, never a full game binary dump or an authored package. */
    //-------------------------------------------------------------------------------------------------
    internal static void Review(string manifest)
    {
        var saved=Settings.Current;
        try{
            string fixture=ReferencePipelineSmokeTest.FindFixture();Settings.Current=new Settings{SchemaPath=fixture,BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<System.Xml.Schema.XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(ConscriptSourceInput.Schemas());schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=ManifestReader.Read(SdkEnvironmentPreflight.Read(manifest,64*1048576)).Assets.ToArray();Qualify(manifest,schemas,catalog);
        }finally{Settings.Current=saved;}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify plain, labelled, prerequisite and EVA-bearing upgrades against actual EP1 native data, not emitted templates. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(string manifest,SchemaSet schemas,ManifestAsset[] catalog)
    {
        string? original=null;for(var folder=new DirectoryInfo(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!);folder!=null;folder=folder.Parent){string path=Path.Combine(folder.FullName,"alpha/SageXML/GlobalData/upgrade.xml");if(File.Exists(path)){original=path;break;}}
        if(original==null)throw new InvalidDataException("Original EP1 upgrade source not found.");var document=SdkXmlDiagnostics.Parse(SdkEnvironmentPreflight.Read(original,1048576),original);var owners=document.DocumentElement!.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="UpgradeTemplate").ToDictionary(e=>e.GetAttribute("id"));
        foreach(string id in new[]{"Upgrade_AlliesFaction","InfantryCaptureBuilding","Upgrade_Pack","Upgrade_JapanWarFactoryTech3"}){
            var source=Xml(Resolve(id).OuterXml);var proof=ProofSource(source,schemas,catalog);var stock=Stock(manifest,"UpgradeTemplate:"+id);
            SchemaNativeContract.VerifyStock(proof.DocumentElement!,typeof(Ep1UpgradeTemplate),0,stock.Bytes,0,stock.Asset.References.ToArray());
            Console.WriteLine("Reborn: EP1 upgrade stock qualification: "+id+": PASS (native="+stock.Bytes.Length+", tokenized=0).");
        }
        // Reborn: recursive same-type Core inheritance preserves official source defaults and the two complementary GameDependency branches.
        XmlElement Resolve(string id){var node=(XmlElement)owners[id].CloneNode(true);
            // Reborn: Core joins structured XML nodes, not formatting-only whitespace; retain all leaf text unchanged.
            foreach(XmlNode text in node.SelectNodes(".//text()")!.Cast<XmlNode>().ToArray())if(string.IsNullOrWhiteSpace(text.Value)&&text.ParentNode!.ChildNodes.OfType<XmlElement>().Any())text.ParentNode.RemoveChild(text);
            SdkUpgradeNormalization.Normalize(schemas.Schemas,node);if(node.HasAttribute("inheritFrom")){string basis=node.GetAttribute("inheritFrom");node.RemoveAttribute("inheritFrom");node=(XmlElement)NodeJoiner.Override(schemas.Schemas,node.OwnerDocument!,Resolve(basis),node);}return node;}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: the stream header owns the asset ID; the native record has no XML id or inherited runtime field for it. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlDocument ProofSource(XmlDocument source,SchemaSet schemas,ManifestAsset[] catalog)
    {
        var proof=SchemaNativeContract.Resolve(source,schemas,catalog);proof.DocumentElement!.RemoveAttribute("id");return proof;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: run the EP1 writer on Core-normalized references, returning native BIN/RELO/IMP only because stock upgrades are not tokenized. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlNode source)
    {
        var xml=Xml(source.OuterXml);var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);Ep1UpgradeTemplate* root;using var tracker=new Tracker((void**)&root,(uint)sizeof(Ep1UpgradeTemplate),false);
        Marshaler.Marshal(new Node(xml.DocumentElement!.CreateNavigator()!,ns),root,tracker);var chunk=new Chunk();if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("UpgradeTemplate native allocation failed.");return chunk;
    }

    // Reborn: detached XML copies preserve source/default presence without mutating authored files.
    private static XmlDocument Xml(string text)=>SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(text));

    // Reborn: bounded little-endian reads keep diagnostics restricted to the selected asset payload.
    private static uint Word(byte[] bytes,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));
}
