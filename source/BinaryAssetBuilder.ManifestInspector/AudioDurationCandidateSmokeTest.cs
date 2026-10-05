using System.Buffers.Binary;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: candidate WAV duration tests never execute native codecs or widen existing core/worker/package profiles.
internal static class AudioDurationCandidateSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: derive runtime sample totals from bounded PCM, retain immutable admission, and reject inconsistent duration headers. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        foreach (bool streamed in new[] { false,true })
        {
            XmlElement root = AudioEncoderPoc.CreateDefinition(streamed);
            InstanceHandle identity = AudioEncoderPoc.Identity(root);
            foreach (int samples in new[] { 12000,12001,24000,47999,48000,95999,96000 })
            {
                byte[] wave = Wave(samples);
                var prepared = Candidate(root,identity,wave);
                Require(prepared.Samples == samples && prepared.Rate == 48000 && prepared.Channels == 1,"Candidate PCM-derived scalars differ.");
                byte[] header = streamed ? Header(samples) : Array.Empty<byte>();
                AssetBuffer runtime = prepared.SerializeCurrent(root,identity,TargetPlatform.Win32,wave,header);
                Require(BinaryPrimitives.ReadInt32LittleEndian(runtime.InstanceData.AsSpan(12,4)) == samples,"Serialized sample total differs.");
                AudioFileRuntimeProbe.Parse(runtime.InstanceData,runtime.InstanceData.Length);
                if (samples != 12000)
                    Reject(() => Ra3Ep1AudioFileInputProfile.Prepare(root,identity,TargetPlatform.Win32,wave));
                else
                {
                    // Reborn: explicit candidate admission at 250ms must preserve every legacy runtime/relocation byte.
                    var legacy = Ra3Ep1AudioFileInputProfile.Prepare(root,identity,TargetPlatform.Win32,wave);
                    AssetBuffer expected = legacy.SerializeCurrent(root,identity,TargetPlatform.Win32,wave,header);
                    Require(runtime.InstanceData.SequenceEqual(expected.InstanceData) && runtime.RelocationData.SequenceEqual(expected.RelocationData)
                        && runtime.ImportsData.SequenceEqual(expected.ImportsData),"Canonical candidate changed legacy wire bytes.");
                }
                byte[] detached = prepared.CopyWave(); detached[44] ^= 1;
                Require(prepared.CopyWave().SequenceEqual(wave),"Duration snapshot exposes writable PCM.");
                byte[] changed = (byte[])wave.Clone(); changed[^1] ^= 1;
                Reject(() => prepared.VerifyCurrent(root,identity,TargetPlatform.Win32,changed));
                int differentSamples = samples == 96000 ? 95999 : samples+1;
                Reject(() => prepared.VerifyCurrent(root,identity,TargetPlatform.Win32,Wave(differentSamples)));
                if (streamed)
                    Reject(() => prepared.SerializeCurrent(root,identity,TargetPlatform.Win32,wave,Header(differentSamples)));
                prepared.VerifyCurrent(root,identity,TargetPlatform.Win32,wave);
                foreach (int offset in new[] { 0,4,8,12,16,20,22,24,28,32,34,36,40 })
                {
                    byte[] invalid = (byte[])wave.Clone(); invalid[offset] ^= 1;
                    Reject(() => Candidate(root,identity,invalid));
                }
            }
            foreach (int samples in new[] { 0,1,11999,96001 })
                Reject(() => Candidate(root,identity,Wave(samples)));
            foreach (int length in new[] { 0,43,24043,24045,192045 })
                Reject(() => Candidate(root,identity,new byte[length]));
            byte[] canonical = Wave(12000);
            foreach (uint declared in new[] { 0u,1u,uint.MaxValue,24001u,192000u })
            {
                foreach (int offset in new[] { 4,40 })
                {
                    byte[] invalid = (byte[])canonical.Clone(); BinaryPrimitives.WriteUInt32LittleEndian(invalid.AsSpan(offset,4),declared);
                    Reject(() => Candidate(root,identity,invalid));
                }
            }
            byte[] extra = new byte[canonical.Length+2]; canonical.CopyTo(extra,0);
            Reject(() => Candidate(root,identity,extra));
            byte[] longer = Wave(48000);
            var frozen = Candidate(root,identity,longer);
            root.SetAttribute("PCCompression","NONE"); Reject(() => Candidate(root,identity,longer)); root.SetAttribute("PCCompression","XAS");
            identity.TypeHash = 0; Reject(() => Candidate(root,identity,longer)); identity.TypeHash = 0x53C81E47u;
            identity.InstanceHash = 1; Reject(() => frozen.VerifyCurrent(root,identity,TargetPlatform.Win32,longer)); identity.InstanceHash = 0;
            Reject(() => Ra3Ep1AudioFileInputProfile.PrepareDurationCandidate(root,identity,TargetPlatform.Xbox360,longer));
            frozen.VerifyCurrent(root,identity,TargetPlatform.Win32,longer);
        }
        Require(Ra3Ep1AudioFileInputProfile.MaximumCandidateWaveBytes == 192044,"Candidate read bound differs.");
        Console.WriteLine("Audio duration candidate self-test: OK (250ms..2s explicit managed preparation; exact RIFF/data/scalars, immutable current source and legacy rejection; no native codec/core/package admission)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: create exact managed PCM fixtures without filesystem writes or native codec initialization. */
    //-------------------------------------------------------------------------------------------------
    internal static byte[] Wave(int samples)
    {
        using MemoryStream stream = new(); AudioEncoderPoc.WriteWave(stream);
        byte[] wave = new byte[44+2*samples]; stream.ToArray().AsSpan(0,44).CopyTo(wave);
        BinaryPrimitives.WriteUInt32LittleEndian(wave.AsSpan(4,4),(uint)(wave.Length-8));
        BinaryPrimitives.WriteUInt32LittleEndian(wave.AsSpan(40,4),(uint)(wave.Length-44));
        return wave;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: construct synthetic streamed headers only to exercise scalar consistency, never claim native encoding evidence. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Header(int samples)
    {
        byte[] header = Convert.FromHexString("0400BB8040000000");
        BinaryPrimitives.WriteUInt32BigEndian(header.AsSpan(4,4),(uint)samples|0x40000000u);
        return header;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require explicit candidate admission at every new preparation call. */
    //-------------------------------------------------------------------------------------------------
    private static Ra3Ep1AudioFileInputProfile.PreparedInput Candidate(XmlElement root,InstanceHandle identity,byte[] wave)
        => Ra3Ep1AudioFileInputProfile.PrepareDurationCandidate(root,identity,TargetPlatform.Win32,wave);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: malformed or stale candidate inputs must fail closed instead of inheriting the wider sample range. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(Action action)
    {
        try { action(); } catch (Exception error) when (error is NotSupportedException or ArgumentException or XmlSchemaValidationException) { return; }
        throw new InvalidDataException("Malformed/stale duration candidate or widened legacy admission accepted.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop immediately when an independent candidate duration assertion differs. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
