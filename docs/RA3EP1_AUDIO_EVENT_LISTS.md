# Selected local AudioEvent Sound lists

Date: October 4, 2026.

The four-file supervised authored path now admits one or two unique Sound
references chosen from the frozen RAM/streamed pair, in caller XML order.
Singleton RAM, singleton streamed and reversed pairs have managed and opt-in
native proofs. The generated baseline event remains byte-identical.

## Source contract and scope

Use `supervised-authored-audio-event-poc <absolute-audited-audio.dll> <source-directory>`.
The same ram.xml, streamed.xml, input.wav and event.xml are required. Both audio
leaves are still encoded and packaged, even when an event selects only one.
This changes event selection/order, not source discovery or audio pool size.

References must exactly match AudioFile:<caller-name>. One or two distinct targets
are admitted. Empty lists, more than two Sounds, duplicates, case aliases, unknown
names, selectors and other types reject. XML order is authoritative: streamed
followed by RAM is legal and must retain that order in the manifest.

Volume/Weight rules remain the [scalar contract](RA3EP1_AUDIO_EVENT_SCALARS.md).
A singleton must have positive weight; a pair may contain one zero weight but
not two. Control now follows the [four-flag contract](RA3EP1_AUDIO_EVENT_CONTROLS.md).
Includes, inheritance, additional child types,
arbitrary audio files, broader WAV settings and production registration stay closed.

Example: only the streamed leaf is selected (the RAM file still exists):

```xml
<!-- Reborn: bounded singleton diagnostic event, not a playable mod definition. -->
<AssetDeclaration xmlns="uri:ea.com:eala:asset">
  <AudioEvent id="CallerLocalEvent" Volume="37.5" Control="INTERRUPT">
    <Sound Weight="875">AudioFile:Caller_Stream-02</Sound>
  </AudioEvent>
</AssetDeclaration>
```

## Implementation and invariants

`AuthoredAudioEventSource.Read` maps literal names to frozen source slots and stores
cardinality, selected order and ordinal weights in immutable Settings. Slots returns
detached arrays. Current core scalar/reference cardinality is checked again.

`AudioFileLocalEventProbe.Compile` resolves normalized references by exact name/ID
against the bounded pool, not by assuming package slot equals Sound ordinal.
Unknown/repeated targets reject. Event hashes capture only selected ordered
dependency fingerprints: selected-leaf changes invalidate the event; unused custom
data changes do not. Both preparations and all original source bytes are still
checked globally before acceptance.

Entry.References returns selected manifest references in XML order. Selectors remain
one-biased ordinals into that reference table: selector 1 can resolve to streamed,
although streamed occupies package asset 1. It is not a package-array index.

Event size is 152+12*N; RELO remains 8; IMP is 4*(N+1), including sentinel. Count,
pointer, each selector/weight/default child volume and full auxiliary tables are
checked. Manifest reference buffers use 8*N bytes. Package readers and parent
acceptance derive expected sizes from frozen evidence, not child-reported metadata.
Independent parent recompilation still compares full event BIN/RELO/IMP and hash.

## Executed validation

Release/x86 build and all 101 compiler groups passed. All 33 enum checks passed;
models/marshallers remain 785/1,390 and 762/1,390. Added cases extend existing
managed groups; default tests do not execute native codecs.

Independent complete native fixtures cover singletons and reversed pairs, exact
import sentinels and manifest reference order. Corrupt count/pointer/selector/weight/
child-volume/auxiliary tables reject. Tests also cover malformed profiles, empty/
duplicate/oversized lists, detached ownership, selected dependency staleness and
unused-leaf hash scope. Baseline source/output remains unchanged.

`authored-audio-list-native-proof <audited-dll>` passed all three selections with
Volume=37.5, RAM weight 125, streamed weight 875, silence PCM and custom names.
Native sizes, selectors/weights and manifest tuples are checked directly. Each
selection also passes stale-original rejection and pre-codec input-copy tamper
rejection without acceptance. Original hashes remain unchanged after normal encoding;
only owned test fixtures are mutated.

| Selection | Event BIN/RELO/IMP | Mixed BIN/RELO/IMP | Manifest |
| --- | --- | --- | --- |
| RAM only | 164/8/8 | 324/36/16 | 314 |
| Streamed only | 164/8/8 | 324/36/16 | 314 |
| Streamed, RAM | 176/8/12 | 336/36/20 | 322 |

Accepted evidence directories:

- RAM: `C:\Users\drknt\AppData\Local\Temp\Reborn-SupervisedAudio-9754e1e3e8e74d6e89ca27dbf3f5c342`
- Streamed: `C:\Users\drknt\AppData\Local\Temp\Reborn-SupervisedAudio-60bfbbc48a0845ebba99ae5dd652212b`
- Reversed: `C:\Users\drknt\AppData\Local\Temp\Reborn-SupervisedAudio-dcd39ab0f1bb42558348cb8332bbee33`

Legacy fixed supervised encoding and both native artifact/package tamper tests also
passed. These are diagnostic generated sources, not stock hash parity or game-load proof.

## Next gates

Approximately 50% complete / 50% remaining overall. The control follow-up covers
four existing plugin-admitted flags. Next: a separately bounded variable audio pool/source inventory.
Broader WAV/codecs, production hash/cache parity, remaining native layouts,
WorldBuilder and in-game loading remain required. No atomic filesystem transaction
or native-code security sandbox is claimed.
