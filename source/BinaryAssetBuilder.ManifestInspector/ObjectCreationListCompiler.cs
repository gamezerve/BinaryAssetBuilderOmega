using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using Relo;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: EP1 ObjectCreationList authoring. The native layout is the official marshaler's (see Ep1ObjectCreationList), the stream is
// untokenized like stock, and every build first re-qualifies the derivation against original EP1 OCLs: a fixed static witness set for
// the root/CreateObject family, plus one global stock witness for every nugget family or nugget field the authored lists use that the
// fixed set does not exhibit. A family or field with no stock witness is refused instead of being emitted on trust.
internal static class ObjectCreationListCompiler
{
    internal const uint TypeId=0xE86E4D61u;
    private const uint TypeHash=0x1477F2E1u;
    internal sealed record Capture(ManifestAsset Asset,byte[] Bytes);

    // Reborn: original EP1 static-stream OCLs (DATA:EP1 sources) covering flags in both option words, dispositions, forces, angles,
    // lifetimes, velocity, an enum pointee (StatusToCopyFromParent) and weak object lists.
    private static readonly (string File,string Id)[] StaticWitnesses={
        ("EP1/Japan/Props/JapanFortressShipAirborne_Dying.xml","JUFortressShipAirborne_Die_OCL"),
        ("EP1/Japan/Units/JapanFortressShip.xml","OCL_SeaDeathOCL"),
        ("EP1/Japan/Units/JapanFortressShip.xml","OCL_AirDeathOCL"),
        ("EP1/Allied/Units/AlliedArtilleryVehicle.xml","OCL_AlliedArtilleryPreAttack_Marker"),
        ("EP1/Allied/Units/AlliedFutureTank.xml","OCL_AlliedFutureTankNeutronScramblerNode"),
        ("EP1/Allied/Units/AlliedLegionnaireInfantry.xml","OCL_CryoSpill"),
        ("EP1/Soviet/Units/SovietDesolatorInfantry.xml","OCL_DesolatorBlatGoo"),
        ("EP1/Soviet/Units/SovietHeavyWalkerVehicle.xml","SUHeavyWalkerVehicle_Legs_OCL")};

    //-------------------------------------------------------------------------------------------------
    /** Reborn: official marshaler ABI (RA3 BinaryAssetBuilder.XmlCompiler Marshal<...Nugget> push sizes and field offsets). */
    //-------------------------------------------------------------------------------------------------
    private static void ReviewAbi()
    {
        if(FastHash.GetHashCode("ObjectCreationList")!=TypeId)throw new InvalidDataException("ObjectCreationList type id differs.");
        var sizes=new (Type Type,int Size)[]{(typeof(Ep1ObjectCreationList),56),(typeof(Ep1CreateObjectNugget),248),(typeof(Ep1AttackNugget),16),(typeof(Ep1FireWeaponNugget),8),(typeof(Ep1FlingObjectNugget),52),(typeof(Ep1StrafeAreaNugget),32),(typeof(Ep1ParabolicCurveNugget),48)};
        foreach(var (type,size) in sizes)if(Marshal.SizeOf(type)!=size)throw new InvalidDataException("EP1 "+type.Name+" ABI size differs: "+Marshal.SizeOf(type)+" != "+size);
        var offsets=new (Type Type,string Field,int Offset)[]{
            (typeof(Ep1ObjectCreationList),"Flags",4),(typeof(Ep1ObjectCreationList),"CreateObject",8),(typeof(Ep1ObjectCreationList),"ParabolicCurve",48),
            (typeof(Ep1CreateObjectNugget),"Options",16),(typeof(Ep1CreateObjectNugget),"Disposition",24),(typeof(Ep1CreateObjectNugget),"StartingModelConditions",64),
            (typeof(Ep1CreateObjectNugget),"TempModelCondition",124),(typeof(Ep1CreateObjectNugget),"WaypointSpawn",140),(typeof(Ep1CreateObjectNugget),"FadeSound",172),
            (typeof(Ep1CreateObjectNugget),"FireSpecialPowerTemplate",208),(typeof(Ep1CreateObjectNugget),"Offset",216),(typeof(Ep1CreateObjectNugget),"CreateObject",236),
            (typeof(Ep1CreateObjectNugget),"DisabledWhileBusy",245),(typeof(Ep1FlingObjectNugget),"DistanceAtTarget",44),(typeof(Ep1FlingObjectNugget),"UsePrimaryObjectForOffsets",48),
            (typeof(Ep1StrafeAreaNugget),"MaintainOffestToTarget",28),(typeof(Ep1ParabolicCurveNugget),"Offset",44)};
        foreach(var (type,field,offset) in offsets)if(Marshal.OffsetOf(type,field).ToInt32()!=offset)throw new InvalidDataException("EP1 "+type.Name+"."+field+" ABI offset differs.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: read one untokenized stock OCL payload bound to its linked stream checksum. */
    //-------------------------------------------------------------------------------------------------
    internal static Capture Stock(string manifest,string name)
    {
        var document=ManifestReader.Read(SdkEnvironmentPreflight.Read(manifest,64*1048576));
        TypeRegistryAudit.ValidateTarget(document.Header.Version,document.Header.AllTypesHash);
        if(document.Validate().Count!=0||!document.Header.IsLinked||document.Header.ContainerPrefixSize!=4)throw new InvalidDataException("Valid linked EP1 stream required for ObjectCreationList qualification.");
        var asset=document.Assets.SingleOrDefault(a=>a.Name==name)??throw new InvalidDataException("Stock ObjectCreationList witness is missing: "+name);
        if(asset.TypeId!=TypeId||asset.TypeHash!=TypeHash||asset.Tokenized!=0||asset.InstanceDataSize>1048576)throw new InvalidDataException("Unsupported ObjectCreationList asset fingerprint: "+name);
        string bin=Path.ChangeExtension(manifest,".bin");var header=AssetStreamProbe.ReadRange(bin,null,0,8);if(Word(header,0)!=0xBABB0000||Word(header,4)!=document.Header.StreamChecksum)throw new InvalidDataException("ObjectCreationList stream checksum differs.");
        long offset=8;foreach(var entry in document.Assets){if(ReferenceEquals(entry,asset))break;offset+=entry.InstanceDataSize;}
        var bytes=AssetStreamProbe.ReadRange(bin,null,offset,asset.InstanceDataSize);if(bytes.Length<Marshal.SizeOf<Ep1ObjectCreationList>())throw new InvalidDataException("ObjectCreationList native root is truncated.");
        return new(asset,bytes);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify the static witness set, then one global witness for each authored nugget family/field not yet exhibited. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(string manifest,string global,SchemaSet schemas,ManifestAsset[] catalog,IEnumerable<XmlDocument> authored)
    {
        ReviewAbi();string sage=SageXml();var covered=new HashSet<string>(StringComparer.Ordinal);int globals=0;
        var cache=new Dictionary<string,XmlDocument>(StringComparer.OrdinalIgnoreCase);
        // Reborn: keep source feature admission in both modes; stock binary comparisons are optional developer tests.
        foreach(var (file,id) in StaticWitnesses){var source=Owner(Load(Path.Combine(sage,file)),id,file);if(SdkBuildTests.Enabled)Verify(source,manifest,schemas,catalog);covered.UnionWith(Features(source.DocumentElement!));}
        var pool=Load(Path.Combine(sage,"GlobalData/ObjectCreationLists.xml")).DocumentElement!.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="ObjectCreationList"&&Eligible(e)).ToArray();
        foreach(var document in authored)
            foreach(string feature in Features(document.DocumentElement!).OrderBy(f=>f,StringComparer.Ordinal))
            {
                if(covered.Contains(feature))continue;
                var witness=pool.FirstOrDefault(e=>Features(e).Contains(feature))
                    ??throw SdkXmlDiagnostics.At(new InvalidDataException("ObjectCreationList "+feature+" has no original EP1 stock witness; this nugget family/field is not enabled."),Node(document.DocumentElement!,feature)??document.DocumentElement!);
                var source=Xml(witness.OuterXml);if(SdkBuildTests.Enabled)Verify(source,global,schemas,catalog);covered.UnionWith(Features(source.DocumentElement!));globals++;
            }
        Console.WriteLine(SdkBuildTests.Enabled?"Reborn: EP1 ObjectCreationList stock qualification: PASS ("+StaticWitnesses.Length+" static + "+globals+" global witness(es); untokenized native).":"Reborn: ObjectCreationList source admission completed; developer stock comparisons skipped.");

        XmlDocument Load(string path){if(!cache.TryGetValue(path,out var xml))cache[path]=xml=SdkXmlDiagnostics.Parse(SdkEnvironmentPreflight.Read(path,4*1048576),path);return xml;}
    }

    private static void Verify(XmlDocument source,string manifest,SchemaSet schemas,ManifestAsset[] catalog)
    {
        string id=source.DocumentElement!.GetAttribute("id");var stock=Stock(manifest,"ObjectCreationList:"+id);
        try{SchemaNativeContract.VerifyStock(ProofSource(source,schemas,catalog).DocumentElement!,typeof(Ep1ObjectCreationList),0,stock.Bytes,0,stock.Asset.References.ToArray());}
        catch(InvalidDataException error){throw new InvalidDataException("EP1 ObjectCreationList stock qualification failed for "+id+": "+error.Message,error);}
    }

    // Reborn: authored-shape features: nugget family, nugget attributes and nugget child elements, plus root attributes (id excluded).
    private static HashSet<string> Features(XmlElement ocl)
    {
        var set=new HashSet<string>(StringComparer.Ordinal);
        foreach(XmlAttribute a in ocl.Attributes)if(a.Specified&&a.NamespaceURI.Length==0&&a.LocalName is not ("id" or "inheritFrom"))set.Add("@"+a.LocalName);
        foreach(var nugget in ocl.ChildNodes.OfType<XmlElement>())
        {
            set.Add(nugget.LocalName);
            foreach(XmlAttribute a in nugget.Attributes)if(a.Specified&&a.NamespaceURI.Length==0)set.Add(nugget.LocalName+"/@"+a.LocalName);
            foreach(var child in nugget.ChildNodes.OfType<XmlElement>())set.Add(nugget.LocalName+"/"+child.LocalName);
        }
        return set;
    }

    // Reborn: a feature name back to its first authored node, so a refusal points at the user's file/line.
    private static XmlNode? Node(XmlElement ocl,string feature)
    {
        if(feature.StartsWith('@'))return ocl.GetAttributeNode(feature[1..]);
        string[] parts=feature.Split('/');var nugget=ocl.ChildNodes.OfType<XmlElement>().FirstOrDefault(e=>e.LocalName==parts[0]&&(parts.Length==1||(parts[1].StartsWith('@')?e.HasAttribute(parts[1][1..]):e.ChildNodes.OfType<XmlElement>().Any(c=>c.LocalName==parts[1]))));
        if(nugget==null||parts.Length==1)return nugget;return parts[1].StartsWith('@')?nugget.GetAttributeNode(parts[1][1..]):nugget.ChildNodes.OfType<XmlElement>().First(c=>c.LocalName==parts[1]);
    }

    // Reborn: global witnesses must be literal (no Define expressions) and self-contained (no inheritance).
    private static bool Eligible(XmlElement ocl)=>!ocl.HasAttribute("inheritFrom")&&!ocl.SelectNodes(".//@*")!.Cast<XmlAttribute>().Any(a=>a.Value.StartsWith('=')||a.Value.Contains('$'))&&!ocl.SelectNodes(".//text()")!.Cast<XmlNode>().Any(t=>(t.Value??"").TrimStart().StartsWith('='));

    private static XmlDocument Owner(XmlDocument file,string id,string path)
    {
        var owner=file.DocumentElement!.ChildNodes.OfType<XmlElement>().SingleOrDefault(e=>e.LocalName=="ObjectCreationList"&&e.GetAttribute("id")==id)??throw new InvalidDataException("Original EP1 ObjectCreationList witness source not found: "+path+" / "+id);
        if(!Eligible(owner))throw new InvalidDataException("Original EP1 ObjectCreationList witness is not literal: "+id);return Xml(owner.OuterXml);
    }

    private static string SageXml()
    {
        for(var folder=new DirectoryInfo(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!);folder!=null;folder=folder.Parent){string path=Path.Combine(folder.FullName,"alpha","SageXML");if(File.Exists(Path.Combine(path,"GlobalData","ObjectCreationLists.xml")))return path;}
        throw new InvalidDataException("Original EP1 SageXML source root not found for ObjectCreationList qualification.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: the stream header owns the asset ID; the native root (BaseAssetType) has no id field. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlDocument ProofSource(XmlDocument source,SchemaSet schemas,ManifestAsset[] catalog)
    {
        var proof=SchemaNativeContract.Resolve(source,schemas,catalog);proof.DocumentElement!.RemoveAttribute("id");return proof;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: run the EP1 writer on Core-normalized references; native BIN/RELO/IMP only, because stock OCLs are not tokenized. */
    //-------------------------------------------------------------------------------------------------
    internal static unsafe Chunk Compile(XmlNode source)
    {
        var xml=Xml(source.OuterXml);var ns=new XmlNamespaceManager(xml.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);Ep1ObjectCreationList* root;using var tracker=new Tracker((void**)&root,(uint)sizeof(Ep1ObjectCreationList),false);
        Marshaler.Marshal(new Node(xml.DocumentElement!.CreateNavigator()!,ns),root,tracker);var chunk=new Chunk();if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("ObjectCreationList native allocation failed.");return chunk;
    }

    private static XmlDocument Xml(string text)=>SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(text));
    private static uint Word(byte[] bytes,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));
}
