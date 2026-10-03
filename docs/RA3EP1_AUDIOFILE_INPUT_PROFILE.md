# Checked authored AudioFile input profile

Date: October 4, 2026. Follow-up to [runtime serialization](RA3EP1_AUDIOFILE_SERIALIZATION.md).
This stage adds the managed input boundary needed before an isolated AudioFile
compiler entry. It is not that complete plugin entry or a linked package.

## Scope

`Ra3Ep1AudioFileInputProfile.Prepare` in
`source/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder/XmlCompiler/Ra3Ep1AudioFileInputProfile.cs`
requires a schema-bound authored AudioFile, Win32, recovered type ID 166B084D /
type hash 53C81E47 and current ID/name agreement. It revalidates current XML in a
detached copy rather than trusting stale schema validation information.

The initial profile deliberately admits only:

- An authored ASCII WAV leaf filename, not an absolute/relative directory path,
  URI, dependency selector, macro or MP3 filename. No filesystem resolution occurs.
- Explicit PCSampleRate 48000, PCCompression XAS and IsStreamedOnPC. All six
  official boolean spellings are accepted. PCQuality must remain 75; the schema
  inserts this default. No quality change or resampling is implemented.
- Canonical 44-byte PCM WAV header plus exactly 24,000 PCM data bytes: PCM16,
  mono, 48 kHz, 12,000 samples. Extra chunks, compressed input, other durations,
  stereo, changed chunk lengths, truncation and trailing data reject. PCM sample
  contents may vary; this is not a requirement to use the generated tone.
- Optional nonempty printable-ASCII SubtitleStringName, or the legacy/reference
  default `DIALOGEVENT:<WAV leaf without extension>SubTitle`.

Official XenonQuality/PS3Quality defaults of 75 are tolerated only as unspecified
schema defaults. Authored cross-platform options, GUIPreset, overrides, formulas,
inheritFrom, nested content and unexpected namespace attributes reject. A valid
compiler-injected TypeId is allowed but changes the exact preparation snapshot.
The focused harness `tests/fixtures/AudioFilePipeline.xsd` includes unchanged EP1
schema files. These are input checks, not proof of every AudioFile schema option.

## Immutable preparation and current-source checks

`PreparedInput` owns the exact XML string and PCM bytes and freezes instance name
and supplied instance hash. It exposes read-only codec 29 / output container 39,
rate, channels, samples, subtitle and play location. `CopyWave` returns a clone.

`VerifyCurrent` re-runs current input admission and compares XML, identity hash
and the complete WAV bytes. Even a valid edit requires fresh preparation. Exact
XML comparison intentionally includes attribute order and empty-element spelling;
this is snapshot identity, not semantic XML equivalence. It is not a content hash
or cache policy, and it does not compute an eventual manifest InstanceHash.

`SerializeCurrent` repeats this gate and requires no header for RAM or exactly
eight bytes with tag 04 for the narrowly selected streamed XAS output. The wire
serializer independently checks embedded rate/channels/samples/play flag.
Custom payload framing must still be checked separately before publication.
No live handles or mutable source arrays escape, no DLL is loaded and no file is
read/written by this profile. It is not an IAssetBuilderPlugin registration.

## Connection to the opt-in native experiment

`AudioEncoderPoc` builds two schema-validated authored definitions, prepares both
against the owned WAV before native initialization, then checks current input
again immediately before native work and before runtime serialization. Native
getters must agree with prepared fields, and encoding settings come from the
preparation rather than unrelated hardcoded values. Owned WAV rereads are bounded
to 24,044 bytes through one open file handle.

The native experiment still requires Windows x86 and the exact previously audited
library SHA256. Schema resolution is available only for the checked-in harness's
official includes; authored XML has no document resolver. This experiment now
requires the repository harness and is not a standalone SDK command.

Encoding occurs inside a fresh temporary output directory and retains the same
independent runtime reader/custom framing checks. This is not transactional mod
publication: an encoding/post-check failure can leave partial owned evidence.
No claim of protection against arbitrary concurrent filesystem mutation is made.
No original game, unpacked asset, registry, WorldBuilder or existing build file
is changed. The production AudioCompiler and its legacy ABI remain unchanged.

## Tests and remaining work

`ep1-audiofile-input-self-test` is managed-only. It checks official defaults,
RAM/streamed runtime sizes, detached snapshots, changed PCM content, identity and
XML edits, metadata/platform/settings rejection, all fixed WAV header boundaries,
subtitle/file restrictions, namespace/child/header corruptions and exact recovery.
It is registered as the 92nd compiler test group; declared groups do not establish
native codec or game compatibility.

All 92 compiler groups and the aggregate six-family diagnostic build suite were
executed successfully. Both Release/x86 projects build successfully. Enum sync
checks pass; the preceding serializer still matches both actual stock native
records after the complete 12,951-envelope audit.

The updated native experiment also ran successfully in:
`C:\Users\drknt\AppData\Local\Temp\Reborn-AudioEncoder-75c1a3dbc83a426ab41ad40a1d4222cb`.
RAM yields 80/8/0 native bytes and 7,160 custom bytes / one block; streamed yields
88/12/0 native bytes and 7,232 SNS bytes / four blocks. Both custom tags are
checked as 04. The inline SNR is `0400BB8040002EE0` and payload SHA256 values
remain the ones recorded in the [serializer report](RA3EP1_AUDIOFILE_SERIALIZATION.md).
This was an actual opted-in native run, separate from the default managed suite.

Next: an isolated processor wrapper with checked dependency/file resolution,
prepared custom-payload ownership and error handling, then identity-based custom
filenames plus verified manifest/BIN/RELO/IMP/custom-data packaging. The current
profile precedes core FileReference normalization and does not yet verify
InstanceDeclaration inheritance/dependency tables. Do not route it directly into
production or general diagnostic builds. Broader codecs/durations, final type
table and in-game loading remain open. Overall effort stays approximately
50% complete / 50% remaining; inventory remains 785/1,390 models and 762/1,390
typed marshallers.
