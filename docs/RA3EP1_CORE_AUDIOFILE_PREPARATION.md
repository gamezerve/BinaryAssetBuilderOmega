# Current core AudioFile to authored PCM preparation bridge

Date: October 4, 2026. A managed, inspector-local bridge now connects actual
DocumentProcessor AudioFile instances to the existing narrow authored PCM profile.
It does not register a production AudioFile processor, execute codecs, or publish
audio streams. The native encoder integration is the next gate, not completed here.

## What changed

`AudioFileCorePreparation.Prepare` reads the actual instance's source XML and
direct WAV dependency. The source must be UTF-8, at most 8,192 bytes, contain exactly
one AudioFile, and validate against the checked-in official-type harness. DTDs are
prohibited. Includes, inheritance, strong/weak runtime references, extra file
dependencies and other processing domains are rejected.

The current authored profile only permits a bounded WAV leaf name and canonical
24,044-byte PCM16 mono 48 kHz / 12,000-sample WAV, explicit XAS and RAM/streamed
settings. It validates authored syntax using the existing known PCM fixture before
combining the filename with a directory, then validates the actual bounded WAV.
The resolved path must match core-normalized File and the logical reference table.
Path/file reads reject reparse components and use bounded same-handle reads.
This is not a guarantee against concurrent hostile filesystem replacement.

The bridge reconstructs the current core InstanceHash from authored defaulted XML
and the actual WAV, including 256-byte dependency capacity padding. It compares
the normalized XML shape and core identity before freezing preparation. Original
authored default attribution is preserved: official unused platform defaults must
not become explicitly authored cross-platform settings during the bridge.

`VerifyCurrent` rereads source XML and WAV, checks current core metadata and hash,
then compares frozen XML, normalized shape, path, hash and owned PCM bytes. It does
not rely on timestamps or cached FileHashItem signatures. `CopyWave` returns a
detached buffer. `SerializeCurrent` performs this check before using the existing
runtime serializer and its header checks. Test headers are synthetic, not native
encoder output. Source fixtures are retained in owned GUID temporary directories.

## Validation

`core-audiofile-preparation-self-test` is registered as compiler group 97. Both RAM
and streamed fixtures are parsed by the actual core hash-only pipeline. Tests cover:

- Frozen-buffer ownership and restored byte-identical runtime output.
- Edited normalized File/rate, wrong InstanceHash/TypeHash/ProcessingHash, extra
  file or strong asset dependency metadata.
- Same-size WAV content edits with original timestamp restored: old core identity
  rejects; fresh core parse/preparation accepts current input; old preparation still
  rejects the refreshed identity. Restoring original bytes restores old preparation.
- Missing WAV rejection with exact owned-file move/restore in a finally block.
- Changed source filename, unsupported compression, path traversal, DTD, oversized
  source/WAV and incorrect RAM/streamed header lengths.

Inspector Release/x86 build passes and all **97 compiler groups** were executed
successfully. Model/marshaller counts are unchanged at 785/1,390 and 762/1,390.
No original game files, reference DLLs, official schemas or production audio
registration were modified. The bridge remains internal to the inspector and
requires the explicitly test-only processing fingerprint from the identity audit.

## Remaining work

Overall effort remains approximately **50% complete / 50% remaining**. This is a
bounded preparation gate, not a new usable SDK milestone. Next, refactor the opt-in
encoder PoC so frozen actual core preparations supply settings/PCM and fresh checks
guard native encoding and package publication. Native failure/cleanup, stale source
after encoding, identity/content binding and deterministic package tests must pass
before introducing an isolated AudioFile plugin. General paths, inheritance,
multi-chunk dependencies, original producer identity and in-game loading remain open.
