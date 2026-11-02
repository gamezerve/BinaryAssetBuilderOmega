using System.Buffers.Binary;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove normal authoring preserves the complete official slider enum and distinguishes NONE from an omitted slider.
internal static class AudioEventSliderSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile all six sliders and an omitted default through source preparation, native output and packaged stream readback. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string output,string manifest,string global)
    {
        if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh audio-slider regression directory required.");Directory.CreateDirectory(output);
        string[] names={"SOUNDFX","VOICE","MUSIC","AMBIENT","MOVIE","NONE"};
        var schemas=ConscriptSourceInput.Schemas();var slider=(System.Xml.Schema.XmlSchemaSimpleType)schemas.GlobalTypes[new XmlQualifiedName("AudioVolumeSlider","uri:ea.com:eala:asset")]!;
        var restriction=(System.Xml.Schema.XmlSchemaSimpleTypeRestriction)slider.Content!;
        if(!restriction.Facets.OfType<System.Xml.Schema.XmlSchemaEnumerationFacet>().Select(f=>f.Value).SequenceEqual(names)||!Enum.GetNames<SageBinaryData.AudioVolumeSlider>().SequenceEqual(names))throw new InvalidDataException("Original Uprising slider enum/model differs.");
        var audio=ManifestReader.Read(File.ReadAllBytes(AuthoredAmbientCompiler.CatalogPath()));string target=audio.Assets.First(a=>a.TypeName=="AudioFile").Name;
        string entry=Path.Combine(output,"Static_Mod.xml"),prepared=Path.Combine(output,"prepared"),compiled=Path.Combine(output,"compiled");
        File.WriteAllText(entry,"<!-- Reborn: private audio slider regression; no publication to Documents. -->\n<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+string.Join("",names.Select((n,i)=>"<AudioEvent id='RebornSlider"+i+"' SubmixSlider='"+n+"'><Sound>"+target+"</Sound></AudioEvent>"))+"<AudioEvent id='RebornSliderOmitted'><Sound>"+target+"</Sound></AudioEvent></AssetDeclaration>");
        string sdk=Path.GetFullPath(Path.Combine(Path.GetDirectoryName(AuthoredAmbientCompiler.CatalogPath())!,"../.."));
        ConscriptSourceInput.Prepare(entry,prepared,sdk);
        if(!ConscriptMultiAssetBuild.TryRun(Path.Combine(prepared,"game.xml"),manifest,global,compiled))throw new InvalidDataException("Slider source did not select normal compilation.");
        string readback=Path.Combine(compiled,"stream-readback");var document=ManifestReader.Read(File.ReadAllBytes(Path.Combine(readback,"diagnostic.manifest")));byte[] bin=File.ReadAllBytes(Path.Combine(readback,"diagnostic.bin"));int offset=8;
        if(document.Assets.Count!=7)throw new InvalidDataException("Audio slider asset count differs.");
        foreach(var asset in document.Assets){var bytes=bin.AsSpan(offset,checked((int)asset.InstanceDataSize));offset+=bytes.Length;uint pointer=BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(80,4));
            if(asset.Name=="AudioEvent:RebornSliderOmitted"){if(pointer!=0||bytes.Length!=164)throw new InvalidDataException("Absent slider must remain null.");}
            else {int expected=int.Parse(asset.Name["AudioEvent:RebornSlider".Length..]);if(pointer!=164||bytes.Length!=168||BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice((int)pointer,4))!=expected)throw new InvalidDataException("Serialized slider ordinal/allocation differs: "+asset.Name);}
            if(asset.TypeHash!=0x560C2E45u||asset.References.Count!=1)throw new InvalidDataException("AudioEvent metadata or AudioFile dependency changed.");
        }
        // Reborn: an original VOICE declaration must point to native slider 1 at the recovered optional base offset.
        var retail=ManifestReader.Read(File.ReadAllBytes(global));var voice=GameObjectExtendedRootContract.Original("Sounds/Voice.xml","AudioEvent").First(e=>e.GetAttribute("SubmixSlider")=="VOICE"&&retail.Assets.Any(a=>a.Name=="AudioEvent:"+e.GetAttribute("id")));
        var retailAsset=retail.Assets.Single(a=>a.Name=="AudioEvent:"+voice.GetAttribute("id"));var stock=W3DContainerCompiler.ReadStock(global,retail,retailAsset);uint voicePointer=BinaryPrimitives.ReadUInt32LittleEndian(stock.AsSpan(80,4));if(voicePointer==0||voicePointer>stock.Length-4||BinaryPrimitives.ReadUInt32LittleEndian(stock.AsSpan((int)voicePointer,4))!=1)throw new InvalidDataException("Original Uprising VOICE ordinal differs.");
        // Reborn: permitting the official enum must not admit arbitrary names or integer selectors.
        foreach(string invalid in new[]{"UNKNOWN","6"}){string bad=Path.Combine(output,"bad-"+invalid+".xml");File.WriteAllText(bad,"<AssetDeclaration xmlns='uri:ea.com:eala:asset'><AudioEvent id='Bad' SubmixSlider='"+invalid+"'/></AssetDeclaration>");bool refused=false;try{ConscriptSourceInput.Prepare(bad,Path.Combine(output,"bad-prepared-"+invalid),sdk);}catch(Exception error)when(error is InvalidDataException or System.Xml.Schema.XmlSchemaException or global::BinaryAssetBuilderException){refused=true;}if(!refused)throw new InvalidDataException("Invalid slider was admitted.");}
        Console.WriteLine("Reborn: AudioEvent sliders: PASS (all six official values, original retail VOICE=1, omitted null, explicit NONE, AudioFile imports, invalid values and final packaged stream readback).");
    }
}
