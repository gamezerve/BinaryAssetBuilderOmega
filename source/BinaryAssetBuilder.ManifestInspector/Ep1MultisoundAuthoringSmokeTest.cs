using System.Buffers.Binary;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: exercise normal XML preparation, local sound resolution and final stream packaging rather than an isolated plugin whitelist.
internal static class Ep1MultisoundAuthoringSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: cover Uprising defaults, all optional subsound fields, repeated/nested references, inheritance and empty lists. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string output,string manifest,string global)
    {
        if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh Multisound test output required.");
        string data=Path.Combine(output,"mod","Data");Directory.CreateDirectory(data);
        var audio=ManifestReader.Read(File.ReadAllBytes(AuthoredAmbientCompiler.CatalogPath()));string file=audio.Assets.First(a=>a.TypeName=="AudioFile").Name;
        string entry=Path.Combine(data,"Static_Mod.xml");
        File.WriteAllText(entry,"""
            <!-- Reborn: generated private authoring regression; never published to Documents. -->
            <AssetDeclaration xmlns="uri:ea.com:eala:asset">
            <Multisound id="RebornMulti"><Subsound>RebornVoice</Subsound><Subsound>RebornAmbient</Subsound></Multisound>
            <Multisound id="RebornOptional" Control="LOOP PLAY_ONE"><Subsound Weight="0" PitchShiftLow="0" PitchShiftHigh="2" Volume="0" PlayPercent="25" VolumeShift="-10">RebornMulti</Subsound><Subsound Weight="750">RebornVoice</Subsound><Subsound>RebornVoice</Subsound></Multisound>
            <Multisound id="RebornEmpty"/>
            <Multisound id="RebornInherited" inheritFrom="RebornMulti" Control="PLAY_ONE"/>
            <Multisound id="RebornLoop" Control="LOOP"><Subsound>RebornAmbient</Subsound></Multisound>
            <AudioEvent id="RebornVoice"><Sound>REPLACE_FILE</Sound></AudioEvent>
            <AudioEvent id="RebornAmbient"><Sound>REPLACE_FILE</Sound></AudioEvent>
            </AssetDeclaration>
            """.Replace("REPLACE_FILE",file));
        string repo=new DirectoryInfo(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!).Parent!.Parent!.FullName;
        string prepared=Path.Combine(output,"prepared"),compiled=Path.Combine(output,"compiled");
        ConscriptSourceInput.Prepare(entry,prepared,Path.Combine(repo,"alpha"));
        if(!ConscriptMultiAssetBuild.TryRun(Path.Combine(prepared,"game.xml"),manifest,global,compiled))throw new InvalidDataException("Multisound did not select the production backend.");
        string readback=Path.Combine(compiled,"stream-readback");var doc=ManifestReader.Read(File.ReadAllBytes(Path.Combine(readback,"diagnostic.manifest")));
        byte[] bin=File.ReadAllBytes(Path.Combine(readback,"diagnostic.bin"));int offset=8,checks=0;
        Check(doc.Assets.Count==7,"Unexpected asset count or inherited base leakage.");
        foreach(var asset in doc.Assets){var b=bin.AsSpan(offset,checked((int)asset.InstanceDataSize));offset+=b.Length;if(asset.TypeName!="Multisound")continue;
            Check(asset.TypeId==0xA3A7AF37u&&asset.TypeHash==0xF79C5A89u&&asset.Tokenized==0,"Multisound fingerprint differs.");
            uint flags=Get(b,4),count=Get(b,8),pointer=Get(b,12);
            if(asset.Name=="Multisound:RebornOptional"){
                Check(flags==3&&count==3&&pointer==16&&b.Length==120,"Optional graph layout differs.");
                for(int i=0;i<5;i++)Check(Get(b,24+i*4)==100+i*4,"Optional scalar pointer differs.");
                float[] expected={0,2,0,.25f,-10*.01f};for(int i=0;i<5;i++)Check(Get(b,100+i*4)==BitConverter.SingleToUInt32Bits(expected[i]),"Optional value or percentage normalization differs.");
                Check(asset.References.Any(r=>r.TypeId==0xA3A7AF37u),"Nested local multisound import absent.");
            }else if(asset.Name=="Multisound:RebornEmpty")Check(count==0&&pointer==0&&b.Length==16&&asset.References.Count==0,"Empty list invented a dependency.");
            else if(asset.Name=="Multisound:RebornInherited")Check(flags==2&&count==2,"Inheritance lost base subsounds or derived control.");
            else if(asset.Name=="Multisound:RebornLoop")Check(flags==1&&count==1,"Original Uprising LOOP syntax was rejected or changed.");
            else Check(flags==0&&count==2&&asset.References.Count==2,"Local SampleMod-style voice/ambient graph differs.");
        }
        Console.WriteLine($"Reborn: Multisound authoring: PASS ({checks} checks; local/nested/repeated imports, optional fields, inheritance, LOOP, empty and packaged stream readback).");

        // Reborn: assertions inspect the actual serialized graph rather than repeating the writer.
        void Check(bool valid,string message){if(!valid)throw new InvalidDataException(message);checks++;}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: read native Win32 words independently from the production marshaler. */
    //-------------------------------------------------------------------------------------------------
    private static uint Get(ReadOnlySpan<byte> bytes,int offset)=>BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(offset,4));
}
