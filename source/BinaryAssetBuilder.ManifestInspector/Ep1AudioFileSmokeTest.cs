using System.Buffers.Binary;
using System.Text;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: durable end-to-end authoring tests exercise real codecs, resource snapshots, runtime records and final CDATA lookup names.
internal static class Ep1AudioFileSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: generated private WAVs cover all PC codecs, play locations, stereo, defaults, resampling and non-fixture durations. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string output,string manifest,string global)
    {
        if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh AudioFile regression output required.");
        string data=Path.Combine(output,"mod","Data"),audio=Path.Combine(output,"mod","Audio"),sounds=Path.Combine(audio,"Sounds");Directory.CreateDirectory(data);Directory.CreateDirectory(sounds);
        byte[] mono=Wave(1,12000),stereo=Wave(2,12000);File.WriteAllBytes(Path.Combine(sounds,"mono.wav"),mono);File.WriteAllBytes(Path.Combine(audio,"stereo.wav"),stereo);File.WriteAllBytes(Path.Combine(sounds,"long.wav"),Wave(1,144000));
        var rows=new List<(string Id,string Attributes,int Samples,int Rate,int Channels,int Tag,bool Streamed)>();
        foreach(string codec in new[]{"XAS","NONE","EALAYER3"})foreach(bool streamed in new[]{false,true})rows.Add(("Reborn"+codec+streamed,"File='AUDIO:Sounds/mono.wav' PCCompression='"+codec+"' IsStreamedOnPC='"+streamed.ToString().ToLowerInvariant()+"'",12000,48000,1,codec=="XAS"?4:codec=="NONE"?2:6,streamed));
        rows.Add(("RebornStereo","File='AUDIO:stereo.wav' PCCompression='XAS' IsStreamedOnPC='false'",12000,48000,2,4,false));
        rows.Add(("RebornResample","File='AUDIO:Sounds/mono.wav' PCCompression='XAS' IsStreamedOnPC='false' PCSampleRate='24000'",6000,24000,1,4,false));
        rows.Add(("RebornOutsideSoundsDefaults","File='AUDIO:stereo.wav'",12000,48000,2,2,true));
        rows.Add(("RebornSoundsDefaults","File='AUDIO:Sounds/mono.wav'",12000,48000,1,4,false));
        rows.Add(("RebornEmptySubtitle","File='AUDIO:Sounds/mono.wav' PCCompression='XAS' IsStreamedOnPC='false' SubtitleStringName=''",12000,48000,1,4,false));
        rows.Add(("RebornLong","File='AUDIO:Sounds/long.wav'",144000,48000,1,4,false));
        string entry=Path.Combine(data,"Static_Mod.xml");File.WriteAllText(entry,"<!-- Reborn: generated audio regression, never published to Documents. -->\n<AssetDeclaration xmlns='uri:ea.com:eala:asset'>\n"+string.Join("\n",rows.Select(r=>"<AudioFile id='"+r.Id+"' "+r.Attributes+"/>"))+"\n<AudioEvent id='RebornAudioPlayback' Volume='70' Type='WORLD' SubmixSlider='SOUNDFX'><Sound>"+rows[0].Id+"</Sound></AudioEvent>\n</AssetDeclaration>");
        string repo=new DirectoryInfo(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!).Parent!.Parent!.FullName,prepared=Path.Combine(output,"prepared"),compiled=Path.Combine(output,"compiled");
        ConscriptSourceInput.Prepare(entry,prepared,Path.Combine(repo,"alpha"));
        string game=Path.Combine(prepared,"game.xml");if(!ConscriptMultiAssetBuild.TryRun(game,manifest,global,compiled))throw new InvalidDataException("AudioFile did not select the production mixed/support backend.");
        string readback=Path.Combine(compiled,"stream-readback");var document=ManifestReader.Read(File.ReadAllBytes(Path.Combine(readback,"diagnostic.manifest")));byte[] bin=File.ReadAllBytes(Path.Combine(readback,"diagnostic.bin"));int at=8,checks=0;
        using(var archive=BigArchive.Open(Path.Combine(compiled,"support","RebornConscriptProbe.big"),1000,1048576)){
            Check(document.Assets.Count==rows.Count+1&&archive.Entries.Count==15+rows.Count*3,"Audio/CDATAs are missing or additional assets were invented.");
            foreach(var asset in document.Assets){
                byte[] chunk=bin.AsSpan(at,checked((int)asset.InstanceDataSize)).ToArray();at+=chunk.Length;
                // Reborn: the event imports the emitted local identity, not an unrelated retail AudioFile or an invented sound slot.
                if(asset.TypeName=="AudioEvent"){
                    var target=document.Assets.Single(a=>a.Name=="AudioFile:"+rows[0].Id);Check(asset.Name=="AudioEvent:RebornAudioPlayback"&&asset.References.Count==1&&asset.References[0].TypeId==target.TypeId&&asset.References[0].InstanceId==target.InstanceId,"Local AudioEvent-to-AudioFile linkage differs.");continue;
                }
                var row=rows.Single(r=>asset.Name=="AudioFile:"+r.Id);var header=AudioFileRuntimeProbe.Parse(chunk,chunk.Length);
                Check(header.Samples==row.Samples&&header.Rate==row.Rate&&header.Channels==row.Channels&&(header.HeaderSize==8)==row.Streamed,"WAV-derived audio runtime fields differ.");
                Check(asset.TypeId==0x166B084D&&asset.TypeHash==0x53C81E47&&asset.Tokenized==0&&asset.References.Count==0,"AudioFile native identity or import metadata differs.");
                string name=$"{asset.TypeId:x8}.{asset.TypeHash:x8}.{asset.InstanceId:x8}.{asset.InstanceHash:x8}.cdata";byte[]? first=null;
                foreach(string suffix in new[]{"","_l","_m"}){
                    var item=archive.Entries.Single(e=>e.Name=="data\\mod"+suffix+"\\cdata\\"+name);byte[] custom=archive.ReadEntry(item,checked((int)item.StoredSize));using var stream=new MemoryStream(custom,false);byte[] inline=header.HeaderSize==0?Array.Empty<byte>():chunk.AsSpan((int)header.HeaderPointer,(int)header.HeaderSize).ToArray();
                    Check(AudioCustomDataProbe.Inspect(stream,header,inline).CodecTag==row.Tag,"Requested AudioFile codec was not preserved.");if(first!=null)Check(first.SequenceEqual(custom),"LOD custom payload variants differ.");first=custom;
                }
                if(row.Id=="RebornEmptySubtitle")Check(header.SubtitleLength==0&&header.SubtitlePointer==0,"Explicit empty subtitle was replaced with a generated key.");
            }
        }
        // Reborn: the current early-global packager must reject authored audio rather than discard its custom data.
        string rejectedGlobal=Path.Combine(output,"rejected-global");Refuse(()=>ConscriptMultiAssetBuild.TryRun(game,manifest,global,rejectedGlobal,earlyGlobal:true));checks++;Check(!Directory.Exists(rejectedGlobal),"Early global emitted a package without its AudioFile CDATA.");
        foreach(var broken in new[]{mono[..40],Change(mono,0,0),Change(mono,34,8),Change(mono,32,1),Change(mono,22,3)}){Refuse(()=>Ep1AudioFileCompiler.InspectWave(broken));checks++;}
        foreach(string attributes in new[]{"PCCompression='FLAC'","PCCompression='XAS' PCQuality='101'","PCCompression='XAS' PCSampleRate='96000'"}){var xml=new XmlDocument{XmlResolver=null};xml.LoadXml("<AudioFile id='RebornInvalid' "+attributes+"/>");Refuse(()=>Ep1AudioFileCompiler.Prepare(xml.DocumentElement!,mono,Path.Combine(sounds,"mono.wav")));checks++;}
        var root=new XmlDocument{XmlResolver=null};root.Load(game);string resource=Path.Combine(prepared,root.DocumentElement!.GetAttribute("File").Replace('/',Path.DirectorySeparatorChar));byte[] original=File.ReadAllBytes(resource);
        try{File.WriteAllBytes(resource,Change(original,44,(byte)(original[44]^1)));Refuse(()=>Ep1AudioFileCompiler.Encode(new[]{root},game));checks++;}finally{File.WriteAllBytes(resource,original);}
        Console.WriteLine("Reborn: production AudioFile proof: PASS ("+checks+" checks; "+rows.Count+" codec/play-location/stereo/default/resample/duration cases; real WAV capture, native encoding, runtime/CDATAs and stale-input rejection; no game launch).");

        // Reborn: each assertion checks observable source/native/package behavior rather than mirroring the producer's private implementation.
        void Check(bool ok,string why){if(!ok)throw new InvalidDataException(why);checks++;}
    }
    // Reborn: generate ordinary PCM16 RIFF frames without external encoders or copyrighted audio fixtures.
    private static byte[] Wave(int channels,int samples)
    {
        using var stream=new MemoryStream();using var writer=new BinaryWriter(stream,Encoding.ASCII,true);int size=samples*channels*2;writer.Write(Encoding.ASCII.GetBytes("RIFF"));writer.Write(36+size);writer.Write(Encoding.ASCII.GetBytes("WAVEfmt "));writer.Write(16);writer.Write((ushort)1);writer.Write((ushort)channels);writer.Write(48000);writer.Write(48000*channels*2);writer.Write((ushort)(channels*2));writer.Write((ushort)16);writer.Write(Encoding.ASCII.GetBytes("data"));writer.Write(size);for(int i=0;i<samples;i++)for(int c=0;c<channels;c++)writer.Write((short)(Math.Sin((i+c*17)*0.07)*4096));return stream.ToArray();
    }
    // Reborn: mutation controls use owned copies so later checks always start from the original WAV.
    private static byte[] Change(byte[] original,int at,byte value){var copy=(byte[])original.Clone();copy[at]=value;return copy;}
    // Reborn: malformed and unsupported resources must fail before a native package is accepted.
    private static void Refuse(Action action){try{action();}catch(InvalidDataException){return;}throw new InvalidDataException("Invalid AudioFile control was accepted.");}
}
