<!-- Reborn: describe the normal authored WAV-to-runtime/CDATAs path separately from older diagnostic-only audio pools. -->
# AudioFile authoring

Include AudioFile declarations in the current alpha's `Static_Mod.xml` build.
`AUDIO:` uses mod-over-SDK lookup, like the other source/resource aliases.
The WAV is captured into the build's private resource snapshot; the original
audio file and authored XML are not rewritten.

```xml
<!-- Reborn: encode a mod-owned WAV and expose it through a normal AudioEvent. -->
<AssetDeclaration xmlns="uri:ea.com:eala:asset">
  <AudioFile id="RebornSample"
             File="AUDIO:Sounds/RebornSample.wav"
             PCCompression="XAS"
             IsStreamedOnPC="false" />
  <AudioEvent id="RebornSampleEvent" Volume="80"
              Type="WORLD" SubmixSlider="SOUNDFX">
    <Sound>RebornSample</Sound>
  </AudioEvent>
</AssetDeclaration>
```

GameObject sound fields reference the AudioEvent ID. Its Sound/Attack/Decay
entries reference AudioFile IDs, not WAV filenames.

## Supported source and output

- Input: RIFF/WAVE PCM16. The parser accepts ancillary RIFF chunks and checks
  declared chunk lengths, block alignment, channel/rate metadata and complete
  sample frames. Mono and stereo are exercised; the native envelope also admits
  four/six-channel input. Other sample encodings fail explicitly.
- PC compression: `XAS`, `NONE`, `EALAYER3`. PCQuality applies to EALAYER3;
  PCSampleRate overrides are passed to the native resampler. A zero/omitted
  PCSampleRate preserves the source rate. The PC SND header uses a 16-bit sample
  rate; requested rates/quality are checked against that format.
- `IsStreamedOnPC` controls RAM versus streaming. For a source WAV directly in
  a Sounds directory, omitted compression/play-location settings default to
  XAS/RAM. Other directories default to NONE/streaming, matching the EA compiler
  convention. These defaults are frozen before File becomes a snapshot path.
- An omitted subtitle becomes `DIALOGEVENT:<original WAV stem>SubTitle`.
  Explicit empty subtitles remain empty. Subtitle keys currently use printable
  ASCII. GUIPreset remains editor metadata; platform-specific Xenon/PS3 settings
  do not change Win32 output.

The existing repository PC encoder DLL is deployed with the synchronized alpha
toolchain. Encoding runs as a batch in an isolated x86 worker. Source hashes are
checked before encoding, and the parent independently validates the resulting
runtime prefix and SNR/SNS framing against source-derived metadata.

The EP1 runtime prefix is 32 bytes before optional subtitle/streamed header
storage. RAM sounds carry their SNR data in CDATA. Streamed sounds carry the
eight-byte SNR header inline and SNS blocks in CDATA. CDATA names use the **final
manifest** type ID/type hash/instance ID/instance hash, and are included under
each actual mod stream variant. Package bytes and receipt hashes are verified
after insertion. No retail audio archive is replaced or copied wholesale.

## Scope and verification

The old diagnostic pool's identity, duration and eight-source restrictions are
not the production AudioFile authoring path. The real SampleMod pool of 135 WAVs
(including clips longer than those old fixtures) encoded and packaged correctly.

`audio-file-authoring-self-test` exercises generated WAV capture, twelve PC
codec/play-location/stereo/resample/default/subtitle/duration cases, an actual
local AudioEvent-to-AudioFile import and all three CDATA lookup paths. Its 98
checks include malformed/unsupported WAVs, stale captured-input rejection and
explicit early-global custom-data rejection.
These are offline serialization/package tests; they do not claim in-game hearing.

Keep authored AudioFile and AudioEvent definitions in Static_Mod for this alpha.
The existing AdditionalMaps global packager copies stream sidecars without
audio CDATA directories, so authored AudioFile use there fails explicitly
instead of publishing a package missing its sound payloads. Global audio
loading remains a separate integration task.

The full SampleMod Sounds.xml also has two Multisound declarations and references
some absent KW AudioFile identities, including `GUJugge_moveStara`. Those are
separate issues from WAV encoding. This change does not invent sound files,
silently skip Multisound, or turn required missing audio references into warnings.
