# Bounded authored AudioEvent control flags

Date: October 4, 2026.

The supervised four-file authored audio path now accepts the four control flags
already admitted by the isolated EP1 AudioEvent plugin. This widens authored
source admission, not the production plugin registry or runtime/game behavior.

## Current contract

Use `supervised-authored-audio-event-poc <absolute-audited-audio.dll> <source-directory>`
with the existing [source](RA3EP1_AUTHORED_AUDIO_EVENT.md),
[scalar](RA3EP1_AUDIO_EVENT_SCALARS.md) and [list](RA3EP1_AUDIO_EVENT_LISTS.md) limits.
Control remains required as an attribute, but may be explicitly empty (zero bits).
Otherwise it is a whitespace-separated list of unique literal tokens:

| Token | EP1 bit word |
| --- | --- |
| LOOP | 00000001 |
| INTERRUPT | 00000008 |
| FADE_ON_KILL | 00000020 |
| IMMEDIATE_DECAY_ON_KILL | 00000100 |

All subsets are admitted; the combined word is 00000129. Token order and ordinary
XML whitespace may vary. The text is bounded to 128 characters. Duplicate tokens,
numeric enum values, lowercase variants, punctuation, formulas and non-XML
whitespace reject. An omitted Control attribute is not silently defaulted.

SEQUENTIAL, RANDOMSTART, SMART_LIMITING, FADE_ON_START and ALLOW_KILL_MID_FILE remain
closed. Presence in official XSD or the low-level marshaller is not sufficient:
this path retains the narrower existing plugin admission gate. In particular,
EP1 inserts SMART_LIMITING before FADE_ON_KILL; legacy KW bit ordinals must not
be substituted for the EP1 mapping.

Example singleton with two admitted flags:

```xml
<!-- Reborn: diagnostic control-bit source, not an in-game playback guarantee. -->
<AssetDeclaration xmlns="uri:ea.com:eala:asset">
  <AudioEvent id="CallerLocalEvent" Volume="37.5" Control="LOOP INTERRUPT">
    <Sound Weight="875">AudioFile:Caller_Stream-02</Sound>
  </AudioEvent>
</AssetDeclaration>
```

## Implementation and rejection gates

`AuthoredAudioEventSource.ReadControls` maps exact tokens to pinned EP1 words
without accepting numeric Enum.TryParse inputs. ReadSettings captures ControlBits
alongside volume, weights and selected list. Settings.Validate rejects bits outside
00000129 even for internally reconstructed profiles.

`AudioFileLocalEventProbe.CheckNative` requires the source-derived word at offset
44. It does not merely permit any bit combination. Package readback preserves
this check; parent acceptance still independently recompiles frozen event source
and compares full native data, diagnostic hash and current originals. Bit changes
change the event content hash. Raw source reordering is stale even when the
resulting bit mask is identical. No codec enum, schema or production plugin changes
were needed; the existing generated INTERRUPT event remains byte-identical.

## Executed validation

Release/x86 build passed. All 101 compiler groups executed successfully and all
33 enum checks passed. Models/marshallers remain 785/1,390 and 762/1,390.

Managed tests independently patch the expected word at offset 44 for all sixteen
subsets and compare every native byte plus RELO/IMP. Every package round-trips
through both readers. Hash changes and altered control words reject against the
captured profile. Admission tests cover ordinary whitespace/order, unknown and
unsupported tokens, numeric values, duplicates, lowercase, malformed text, missing
Control and same-size/timestamp-preserving equivalent-mask source edits/recovery.

`authored-audio-control-native-proof <audited-dll>` passed these actual supervised
audio-encoding integrations. Event flags are compiled by managed EP1 code, not
interpreted by audio.dll; these are not playback/loop/fade behavior tests.

| Selection | Control word | Accepted job suffix |
| --- | --- | --- |
| Streamed only | 00000000 | 100db02f34e84cb2a68330311601c761 |
| RAM only, LOOP | 00000001 | b7b83d8c3ae8457c9f7a768952a7f0b3 |
| Reversed pair, all four flags | 00000129 | 7bb77264c22a4150bafa69397d4ce133 |

Each suffix identifies an owned directory under
`C:\Users\drknt\AppData\Local\Temp\Reborn-SupervisedAudio-<suffix>`.
The proof directly checks control words, selected references/weights and native
sizes. All original source hashes remain unchanged after encoding. Each case also
rejects stale original event XML and tampered input copies without acceptance.
Only owned test fixtures are mutated. Legacy fixed supervised encoding and both
artifact/package tamper tests also passed. Default tests do not invoke native codecs.

## Remaining work

Overall effort remains approximately 50% complete / 50% remaining. The subsequent
[variable pool preflight](RA3EP1_AUTHORED_AUDIO_POOL.md) now validates an explicit
1–8-source inventory through actual core preparations, without native encoding.
Next: supervised variable-pool encoding and independently checked publication.
Additional fields/controls, broader WAV/codecs, production
hash/cache parity, remaining native layouts, WorldBuilder and actual Uprising loading
remain open. No atomic filesystem transaction or native-code security sandbox is claimed.
