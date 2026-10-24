using System.Buffers.Binary;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: GameObject/@Browser is the WorldBuilder object-browser path (StringList). Its populated native list and metadata grammar are
// qualified against the original EP1 WorldBuilder stream object VL_RussianPortBuilding (REGION Vladivostok_VL STRUCTURE).
internal static class GameObjectBrowserContract
{
    private const string Witness="VL_RussianPortBuilding";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: tokens are printable ASCII and bounded; the marshaler splits on whitespace exactly like the official list parser. */
    //-------------------------------------------------------------------------------------------------
    internal static string[] Tokens(XmlDocument source)
    {
        var attribute=source.DocumentElement!.GetAttributeNode("Browser");var tokens=Ep1GameObjectBrowser.Tokens(attribute?.Value??string.Empty);
        if(tokens.Length>Ep1GameObjectBrowser.MaxItems||tokens.Any(t=>t.Length>Ep1GameObjectBrowser.MaxText||t.Any(c=>c<=32||c>126)))throw SdkXmlDiagnostics.At(new InvalidDataException("GameObject Browser entries must be printable ASCII words (at most "+Ep1GameObjectBrowser.MaxItems+")."),attribute);
        return tokens;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: Browser text/count is a source value proved separately, not a structural difference from the comparison template. */
    //-------------------------------------------------------------------------------------------------
    internal static void Mask(XmlDocument candidate,XmlDocument model)
    {
        _=Tokens(candidate);candidate.DocumentElement!.RemoveAttribute("Browser");
        if(model.DocumentElement!.HasAttribute("Browser"))candidate.DocumentElement.SetAttribute("Browser",model.DocumentElement.GetAttribute("Browser"));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: native count, order and every text equal the authored tokens. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(XmlDocument source,byte[] native,Ep1GameObjectBrowser.Layout layout)
    {
        var tokens=Tokens(source);if(tokens.Length!=layout.Count)throw new InvalidDataException("GameObject Browser source/native count differs.");
        for(int i=0;i<tokens.Length;i++){int record=layout.Items+i*8;uint length=Word(native,record),pointer=Word(native,record+4);
            if(length!=tokens[i].Length||!native.AsSpan((int)pointer,(int)length).SequenceEqual(Encoding.ASCII.GetBytes(tokens[i])))throw new InvalidDataException("GameObject Browser item text differs: "+tokens[i]);}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove the populated layout and the list/item metadata rule on the original WorldBuilder object before authored use. */
    //-------------------------------------------------------------------------------------------------
    internal static bool Qualify(string staticManifest)
    {
        string manifest=Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(staticManifest))!,"..","..","WorldBuilder","data","worldbuilder.manifest"));
        if(!File.Exists(manifest))throw new InvalidDataException("Populated GameObject Browser requires the original EP1 WorldBuilder stream witness: "+manifest);
        var document=ManifestReader.Read(SdkEnvironmentPreflight.Read(manifest,64*1048576));TypeRegistryAudit.ValidateTarget(document.Header.Version,document.Header.AllTypesHash);
        if(document.Validate().Count!=0||!document.Header.IsLinked||document.Header.ContainerPrefixSize!=4)throw new InvalidDataException("Valid linked EP1 WorldBuilder stream required.");
        var asset=document.Assets.SingleOrDefault(a=>a.Name=="GameObject:"+Witness)??throw new InvalidDataException("WorldBuilder Browser witness is missing: "+Witness);
        if(asset.TypeId!=0x942FFF2Du||asset.TypeHash!=0xCEB9FE36u||asset.Tokenized!=1||asset.InstanceDataSize>1048576)throw new InvalidDataException("WorldBuilder Browser witness fingerprint differs.");
        string bin=Path.ChangeExtension(manifest,".bin");var header=AssetStreamProbe.ReadRange(bin,null,0,8);if(Word(header,0)!=0xBABB0000||Word(header,4)!=document.Header.StreamChecksum)throw new InvalidDataException("WorldBuilder stream checksum differs.");
        long offset=8;foreach(var entry in document.Assets){if(ReferenceEquals(entry,asset))break;offset+=entry.InstanceDataSize;}
        var bytes=AssetStreamProbe.ReadRange(bin,null,offset,asset.InstanceDataSize);var rows=GameObjectStockReview.Tokens(bytes,0);
        // Reborn: locate the native root independently through BuildTime (+140), then require the Browser table to bind the +44 count word.
        int root=rows[FastHash.GetHashCode("BuildTime")].Offset-140;var browserRow=rows[FastHash.GetHashCode("Browser")];
        if(root<=0||browserRow.Type!=0x6D66AE18u)throw new InvalidDataException("WorldBuilder Browser witness root/row differs.");
        var layout=Ep1GameObjectBrowser.Inspect(bytes,Array.Empty<uint>(),requireRelocations:false,root:root);
        var original=GameObjectExtendedRootContract.Original("Civilian/Vladivostok_VL/Buildings/"+Witness+".xml","GameObject").Single(e=>e.GetAttribute("id")==Witness);
        var tokens=Ep1GameObjectBrowser.Tokens(original.GetAttribute("Browser"));if(tokens.Length==0||tokens.Length!=layout.Count)throw new InvalidDataException("WorldBuilder Browser witness source/native count differs.");
        for(int i=0;i<tokens.Length;i++){int record=layout.Items+i*8;uint length=Word(bytes,record),pointer=Word(bytes,record+4);if(length!=tokens[i].Length||!bytes.AsSpan((int)pointer,(int)length).SequenceEqual(Encoding.ASCII.GetBytes(tokens[i])))throw new InvalidDataException("WorldBuilder Browser witness text differs.");}
        int table=browserRow.Offset;
        if(Word(bytes,table)!=0x6C827073||Word(bytes,table+4)!=0x5D448FD8||Word(bytes,table+8)!=root+Ep1GameObjectBrowser.CountSlot)throw new InvalidDataException("WorldBuilder Browser list header metadata differs.");
        for(int i=0;i<layout.Count;i++){int row=table+12+i*12;if(Word(bytes,row)!=0xB151A0EB||Word(bytes,row+4)!=0||Word(bytes,row+8)!=layout.Items+i*8)throw new InvalidDataException("WorldBuilder Browser item metadata differs at "+i+".");}
        Console.WriteLine("Reborn: EP1 GameObject Browser stock qualification: PASS (WorldBuilder "+Witness+", "+layout.Count+" item(s)).");
        return true;
    }

    private static uint Word(byte[] b,int at){if(at<0||at>b.Length-4)throw new InvalidDataException("Browser word bounds.");return BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(at,4));}
}
