using System.Text;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: test the shared current-source publication gate using synthetic compressed framing, without loading codecs.
internal static class CoreAudioPackageGateSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: publish only current ordered core bindings and preserve absent/existing outputs when sources or runtime metadata are stale. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        string directory = Path.Combine(Path.GetTempPath(),"Reborn-CoreAudioGate-"+Guid.NewGuid().ToString("N")); Directory.CreateDirectory(directory);
        using MemoryStream pcm = new(); AudioEncoderPoc.WriteWave(pcm); byte[] wave = pcm.ToArray();
        string file = Path.Combine(directory,"input.wav"); File.WriteAllBytes(file,wave);
        string schema = Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"AudioFileIdentityPipeline.xsd");
        var bindings = new[] { Create(false),Create(true) };
        string good = Path.Combine(directory,"good"); CoreAudioPackageGate.Publish(good,bindings);
        AudioFilePackageProbe.Verify(good,bindings.Select(binding => binding.Encoded).ToArray());
        byte[] manifest = File.ReadAllBytes(Path.Combine(good,"diagnostic.manifest"));
        Reject(() => CoreAudioPackageGate.Publish(good,bindings));
        Require(manifest.SequenceEqual(File.ReadAllBytes(Path.Combine(good,"diagnostic.manifest"))),"Existing core package was changed.");
        Reject(() => CoreAudioPackageGate.Verify(bindings.Reverse().ToArray()));
        Reject(() => CoreAudioPackageGate.Verify(new[] { bindings[0],bindings[0] }));
        Reject(() => CoreAudioPackageGate.Verify(bindings.Take(1).ToArray()));
        uint hash = bindings[0].Instance.Handle.InstanceHash; bindings[0].Instance.Handle.InstanceHash ^= 1;
        Reject(() => CoreAudioPackageGate.Publish(Path.Combine(directory,"identity-stale"),bindings)); bindings[0].Instance.Handle.InstanceHash = hash;
        DateTime timestamp = File.GetLastWriteTimeUtc(file); byte[] changed = (byte[])wave.Clone(); changed[100] ^= 1;
        File.WriteAllBytes(file,changed); File.SetLastWriteTimeUtc(file,timestamp);
        Reject(() => CoreAudioPackageGate.Publish(Path.Combine(directory,"wave-stale"),bindings));
        File.WriteAllBytes(file,wave); File.SetLastWriteTimeUtc(file,timestamp);
        string source = Path.Combine(directory,"ram.xml"); string original = File.ReadAllText(source);
        File.WriteAllText(source,original.Replace("XAS","NONE"),new UTF8Encoding(false));
        Reject(() => CoreAudioPackageGate.Publish(Path.Combine(directory,"xml-stale"),bindings)); File.WriteAllText(source,original,new UTF8Encoding(false));
        var wrongRuntime = new AudioFilePackageProbe.Entry("RebornAudioRAM","ram.xml",
            Ra3Ep1AudioFileRuntimeSerializer.Serialize(TargetPlatform.Win32,"DIALOGEVENT:wrongSubTitle",12000,48000,1,Array.Empty<byte>()),bindings[0].Encoded.CopyCustom());
        Reject(() => CoreAudioPackageGate.Publish(Path.Combine(directory,"runtime-stale"),new[] { bindings[0] with { Encoded = wrongRuntime },bindings[1] }));
        foreach (string absent in new[] { "identity-stale","wave-stale","xml-stale","runtime-stale" })
            Require(!Directory.Exists(Path.Combine(directory,absent)),"Rejected core package was published.");
        CoreAudioPackageGate.Publish(Path.Combine(directory,"recovered"),bindings);
        AudioFilePackageProbe.Verify(Path.Combine(directory,"recovered"),bindings.Select(binding => binding.Encoded).ToArray());
        Console.WriteLine("Core audio package gate self-test: OK (current ordered bindings, stale disk XML/PCM/runtime/identity rejection before staging, existing output preservation and recovery; synthetic framing, no codecs)");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: bind each actual parsed core leaf to its own immutable prepared runtime and synthetic test-only custom body. */
        //-------------------------------------------------------------------------------------------------
        CoreAudioPackageGate.Binding Create(bool streamed)
        {
            string name = streamed ? "streamed.xml" : "ram.xml";
            using (FileStream output = new(Path.Combine(directory,name),FileMode.CreateNew,FileAccess.Write)) output.Write(Encoding.UTF8.GetBytes(AudioEncoderPoc.CoreSource(streamed)));
            InstanceDeclaration instance = AudioFileIdentitySmokeTest.Build(directory,schema,AudioFileIdentitySmokeTest.Processing,name);
            var prepared = AudioFileCorePreparation.Prepare(instance);
            byte[] header = streamed ? Convert.FromHexString("0400BB8040002EE0") : Array.Empty<byte>();
            byte[] custom = Convert.FromHexString(streamed ? "8000000C00002EE0DEADBEEF" : "0400BB8000002EE00000000C00002EE0DEADBEEF");
            return new(instance,prepared,new AudioFilePackageProbe.Entry(instance.Handle.InstanceName,name,prepared.SerializeCurrent(instance,header),custom));
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: rejected publication must fail before changing the requested destination. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(Action action)
    {
        try { action(); } catch (Exception exception) when (exception is InvalidDataException or IOException or NotSupportedException) { return; }
        throw new InvalidDataException("Invalid core audio publication unexpectedly succeeded.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop on a source/publication ownership mismatch. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
