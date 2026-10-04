# Caller-owned AudioFile identities and local package closure

Date: October 4, 2026.

The supervised authored audio command no longer requires RebornAudioRAM and
RebornAudioStream as asset names. It preserves validated names from caller XML,
derives local AudioEvent references from them, and independently reconstructs
the same identities in the parent before acceptance. This remains an isolated
diagnostic path, not a production AudioFile processor or playable Uprising mod.

## Contract and implementation

The filenames remain ram.xml, streamed.xml and input.wav. RAM versus streamed
play location comes from the source slot and checked settings, never the asset
name. Names may contain 1–128 ASCII letters, digits, underscores or hyphens.
Whitespace, non-ASCII, path/reference separators and XML punctuation reject.
The pair must have distinct actual SAGE instance IDs: case aliases and any hash
collision reject rather than being silently renamed or sharing a custom file.

- `AudioFileDiagnosticIdentity.Validate`: common literal-name boundary.
- `AuthoredAudioSnapshot.Read`: validates both authored names and rejects ID
  collisions; privately retained RamName/StreamName come from frozen source bytes.
- `AudioFilePackageProbe.Entry`, `Serialize`, `Verify`: native play location is
  checked against the source slot; manifest names, IDs and custom filenames derive
  from the caller identity. RAM must precede streamed and IDs must be unique.
- `AudioFileLocalEventProbe.Source`, `Build`, `Compile`: generated local event
  XML refers to those names; actual core-normalized references must match the
  ordered concrete dependency records before compilation. RebornLocalAudio,
  its two Sound entries, weights and selectors remain fixed.
- `CoreAudioPackageGate.Verify`: source-slot ordering, unique IDs, current core
  identity and exact prepared runtime must agree before diagnostic publication.
- `AudioEncoderSupervisor.ValidateResult`: expected names originate from the
  parent's frozen snapshot, not child-reported metadata. Copied source bytes,
  generated event XML, core identities and both packages are revalidated.

Protocol remains version 2 because the same exact input byte inventory already
binds authored IDs. No native API, codec enum, cache or production registry changed.
The canonical 250 ms mono 48 kHz PCM16/XAS profile and all existing input limits
remain unchanged. See the [source contract](RA3EP1_AUTHORED_AUDIO_SNAPSHOT.md).

## Executed native evidence

`authored-audio-native-proof <audited-dll>` used silence PCM, custom subtitle
DIALOGEVENT:authored_snapshotSubTitle, and names Caller_RAM-01/Caller_Stream-02.
This is generated owned test input, not a claim of compiling user game assets.

Input fixture:
`C:\Users\drknt\AppData\Local\Temp\Reborn-AuthoredNative-6fe1bc57889343958392eaaabc5e20fa`

Accepted job:
`C:\Users\drknt\AppData\Local\Temp\Reborn-SupervisedAudio-4706c8b84c2f448f9a14aef40c31711a`

Core hashes are E0EA63F6/E64B0ED6; diagnostic payload hashes remain
F56BD503/21AFD592. Different authored names affect core identity, but do not
change the encoded PCM payload. The package stores names/IDs separately and
the local event diagnostic hash binds its ordered dependency fingerprints.
The native proof independently checks manifest names, SAGE IDs and event tuples.
The silent RAM payload SHA256 remains
823F8A283EFCA8FCE1D65C6E9EE569E5CDD5EE0B89AA34ACC5AC020F2DFC8C64.
Original source hashes are unchanged after encoding. Stale original WAV and
tampered worker-input tests still reject without an ACCEPTED marker.

Managed regressions cover custom names, former fixture names exchanged between
RAM/streamed slots, case aliases, unsafe/oversized names, reversed source order,
wrong normalized dependencies and stale event fingerprints after renaming.
The baseline fixed-name source remains byte-identical. These extend existing
groups rather than adding a new test group.

Validation: Release/x86 build passed; all 101 compiler groups executed and passed,
including the managed supervisor rejection suite. All 33 enum checks passed.
The legacy fixed-name supervised native proof and both native artifact/package
tamper tests also passed. Model/marshaller counts remain 785/1,390 and 762/1,390.

## Remaining gates

This admits arbitrary literal names for exactly two checked leaves, not arbitrary
asset counts or an authored event graph. General source discovery, inheritance,
broader WAV settings/resampling/codecs, production cache/InstanceHash parity,
WorldBuilder integration and actual game loading remain open. Acceptance is
time-of-validation diagnostic evidence, not an atomic filesystem transaction or
a security sandbox for native code.

Overall weighted effort remains approximately 50% complete / 50% remaining.
The [caller-event follow-up](RA3EP1_AUTHORED_AUDIO_EVENT.md) now admits an explicit
four-file source snapshot with a narrow independently recompiled local AudioEvent.
Next: broaden event settings only with source/native validation before attempting
variable graph size or production registration.
