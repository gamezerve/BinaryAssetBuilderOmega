# Authored local AudioEvent volume and weights

Date: October 4, 2026.

The four-file supervised authored audio path now accepts variable event Volume
and independent Sound weights without loosening source/native agreement.
The original generated event still produces the same fixed fixture bytes.
No production audio registration, new codec or playable mod is introduced.

## Current contract

Use `supervised-authored-audio-event-poc <absolute-audited-audio.dll> <source-directory>`
with the [four-file input contract](RA3EP1_AUTHORED_AUDIO_EVENT.md).

Volume remains required, but accepts a plain decimal literal in 0..100 inclusive,
at most 16 characters, using digits and a decimal point. Invariant decimal parsing
checks the range before binary float conversion: 100.0000001 rejects rather than
rounding into an allowed float. Signs, exponents, percent suffixes, commas,
whitespace, formulas and nonfinite values are outside this diagnostic profile.
Native encoding follows the existing marshaller's float conversion and 0.01f
multiplication, not decimal division. Tiny values may round under normal float
semantics; no higher-precision game behavior is claimed.

Each Sound may omit Weight or have only Weight as an attribute. Omission uses the
official 1000 default. Explicit weights are unsigned integer literals, at most
seven characters, in 0..1,000,000 inclusive. One zero weight is allowed; both zero
reject. This cap and all-zero rejection are diagnostic safety limits, not claims
of restrictions discovered in the game engine.

Control now follows the [four-flag contract](RA3EP1_AUDIO_EVENT_CONTROLS.md).
The [selected-list follow-up](RA3EP1_AUDIO_EVENT_LISTS.md)
now admits one/two distinct local references in XML order. Child Volume, additional
event attributes/ranges, general controls, larger sound counts, aliases and external
references remain closed.
All previous XML/PCM limits and stale-source checks remain active.

## Implementation

`AuthoredAudioEventSource.Read` returns a validated name and immutable Settings
record. Raw admission occurs before schema defaults are inserted. ReadSettings
also checks current core scalar values during compilation; absent authored
weights and schema-inserted defaults converge on the same evidence.
`AuthoredAudioSnapshot.EventSettings` retains the parent's independently admitted
source profile, not worker-supplied expectations.

`AudioFileLocalEventProbe.Entry` freezes Settings alongside native buffers and
dependency fingerprints. CheckNative requires exact source-derived words:
Volume at offset 4, RAM Weight at 156 and streamed Weight at 168. The 176/8/12
event envelope, one-biased selectors, child default volumes, relocation and
import tables remain unchanged. Invalid reconstructed Settings also reject.

Package readback checks those same settings. The supervisor constructs expected
settings from its frozen original snapshot, then recompiles the copied event
independently and compares full BIN/RELO/IMP and diagnostic identity. A child
cannot bless changed scalar bytes by claiming matching settings or updating its
inventory alone. Protocol stays v2; original sources remain read-only.

## Executed evidence

Release/x86 build passed. All 101 compiler groups execute successfully, including
expanded managed snapshot/local-event tests. All 33 enum checks passed; models
and marshallers remain 785/1,390 and 762/1,390. Default tests use synthetic custom
audio bodies and do not invoke native codecs.

Independent full-native comparisons patch only the three expected scalar words
in the prior verified event fixture:

| Authored Volume | Native word | RAM / streamed weights |
| --- | --- | --- |
| 37.5 | 3EC00000 | 125 / 875 |
| 0 | 00000000 | 0 / 1,000,000 |
| 100 | 3F800000 | 1,000,000 / 0 |
| 60 | 3F199999 | 1,000 / 1,000 (both omitted in XML) |

All other native bytes and both auxiliary tables match the baseline. Packages
round-trip through both readers, changed settings change the diagnostic event
hash, and mutations of each scalar word reject against the frozen profile.
Admission tests cover decimal boundaries, unsigned limits/defaults, NaN/INF,
negative/overflow/nonliteral values, all-zero mixtures and snapshot recovery.

Opt-in `authored-audio-event-native-proof <audited-dll>` passed with Volume=37.5,
weights 125/875, custom identities, silence PCM and custom subtitles. It directly
checks the three binary words after encoding, not just normalized XML or Settings.

Input fixture:
`C:\Users\drknt\AppData\Local\Temp\Reborn-AuthoredNative-5b0182e395504adaa239a3c980039a89`

Accepted job:
`C:\Users\drknt\AppData\Local\Temp\Reborn-SupervisedAudio-31ce64fc0a424327b055d1f246d49f24`

All four original source hashes remain unchanged after encoding. A timestamp/
size-preserving original event edit rejects parent acceptance; a tampered event
input copy rejects at the child inventory gate before codec startup. The legacy
fixed supervised native proof and both artifact/package tamper tests also pass.
These are owned generated test fixtures, not user game assets or game-load proof.

## Remaining work

Approximately 50% complete / 50% remaining overall. This extends source admission
within a fixed local graph and does not change model coverage or establish an SDK
release. The selected-list follow-up now covers singleton/reversed references.
The control follow-up admits four existing proven flags. Larger pools and additional
fields/controls still require separate evidence.
Broader WAV settings/codecs, production hashes/cache parity, remaining native
layouts, WorldBuilder and actual Uprising loading remain open.
