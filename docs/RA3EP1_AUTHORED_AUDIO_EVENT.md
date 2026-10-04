# Frozen caller AudioEvent source through the supervised audio worker

Date: October 4, 2026.

`supervised-authored-audio-event-poc <absolute-audited-audio.dll> <source-directory>`
explicitly admits a fourth caller-owned file, event.xml. Its original bytes and
literal event identity survive preparation, child encoding and package publication.
The parent independently recompiles the frozen event before diagnostic acceptance.
The existing three-file command remains unchanged and does not read event.xml.

## Narrow input contract

ram.xml, streamed.xml and input.wav retain the existing
[audio source contract](RA3EP1_AUTHORED_AUDIO_SNAPSHOT.md) and
[literal identity boundary](RA3EP1_AUTHORED_AUDIO_IDENTITIES.md).
event.xml is UTF-8 without BOM, at most 8,192 bytes, with an optional UTF-8 XML
declaration. No authored DTD/entity resolution, Includes, inheritance, formulas,
additional assets or external paths are admitted. Trusted checked-in schema
includes remain available to schema validation only.

The event must have exactly id, Volume and Control="INTERRUPT" as authored
attributes. Its id uses the same 1–128 ASCII letter/digit/underscore/hyphen rule.
Volume now accepts bounded decimal literals in 0..100. The
[selected-list follow-up](RA3EP1_AUDIO_EVENT_LISTS.md) admits one or two distinct
Sound entries in XML order, each with an optional Weight in 0..1,000,000
(default 1000); both-zero mixtures reject. See the
[scalar follow-up](RA3EP1_AUDIO_EVENT_SCALARS.md) for parsing and native evidence.
References must exactly match
AudioFile:<caller-name>; aliases, unknown names, other types, selectors and
duplicate targets reject; reversed pairs are now legal. Comments/formatting are retained in the raw
snapshot rather than regenerated from schema-defaulted XML.

Example event.xml for Caller_RAM-01 and Caller_Stream-02:

```xml
<!-- Reborn: bounded diagnostic caller event; not a production music definition. -->
<AssetDeclaration xmlns="uri:ea.com:eala:asset">
  <AudioEvent id="CallerLocalEvent" Volume="60" Control="INTERRUPT">
    <Sound>AudioFile:Caller_RAM-01</Sound>
    <Sound Weight="800">AudioFile:Caller_Stream-02</Sound>
  </AudioEvent>
</AssetDeclaration>
```

This intentionally proves source provenance and local closure without widening
the already verified 176/8/12 event BIN/RELO/IMP shape. It does not yet expose
arbitrary event controls, additional scalar fields, sound counts or dependency graphs.

## Implementation and acceptance

- `AuthoredAudioEventSource.Validate`: checks raw authored structure, exact
  literal references and trusted schema validity before worker/native launch.
- `AuthoredAudioSnapshot.Read(directory, includeEvent: true)`: freezes four
  privately owned byte arrays and EventName; Install/VerifyCopies/VerifyCurrent
  include event.xml. Existing three-file snapshots ignore unrelated event files.
- `AudioEncoderSupervisor`: explicit encode-authored-event mode fixes four-file
  inventory cardinality; mixing three/four-file modes rejects. Protocol stays v2
  because the exact source inventory already binds content and mode is explicit.
- `AudioFileLocalEventProbe.Build(..., authoredName: ...)`: validates the installed
  bounded source before actual core schema/reference normalization, does not
  rewrite it, and checks current concrete references before compilation.
- Event Entry retains the caller name/ID. Package manifest serialization and both
  independent readers require that identity plus the ordered dependency tuples.
- Parent result validation first verifies frozen source copies, then independently
  recompiles event.xml and compares its ID, diagnostic dependency/content hash and
  exact native bytes. Current originals are reread before ACCEPTED.json.

Native child isolation, time/log/file bounds and no-overwrite publication remain
unchanged. No production plugin/cache activation, native-code security sandbox or
atomic multi-file filesystem transaction is claimed. Failed jobs retain evidence
without acceptance; original user sources are never rewritten by the pipeline.

## Executed evidence

The evidence below records the initial fixed-scalar milestone; the scalar follow-up
adds current variable-volume/weight validation and native evidence.

Release/x86 build passed and all 101 compiler groups executed successfully.
The added cases extend existing snapshot/local-event groups. All 33 enum checks
passed; model/marshaller counts remain 785/1,390 and 762/1,390.

Managed tests include event identity/provenance preservation, detached copies,
source edits preserving timestamp/size, explicit mode mismatch, unknown/case-alias/
duplicate/wrong-type/selector references, unsupported event settings, inherited
sources, Includes, DTD, invalid encoding/BOM and oversized input. The existing
fixed generated event source remains byte-identical.

Opt-in command `authored-audio-event-native-proof <audited-dll>` passed using owned
silence PCM, custom subtitles and Caller_RAM-01/Caller_Stream-02/CallerLocalEvent.
This is generated test input, not an assertion that a user's stock game assets
have been compiled.

Input fixture:
`C:\Users\drknt\AppData\Local\Temp\Reborn-AuthoredNative-d87748ce149f4e678bc1646af29871a3`

Accepted job:
`C:\Users\drknt\AppData\Local\Temp\Reborn-SupervisedAudio-c156740b58794409aadd4a9ac61c78c1`

Mixed diagnostic manifest size is 322 bytes; BIN/RELO/IMP remains 336/36/20 for
these subtitles. Audio core hashes remain E0EA63F6/E64B0ED6 and diagnostic content
hashes F56BD503/21AFD592; those are different identity domains, not stock compiler
parity. Event name and ordered manifest dependencies are independently checked.
Original hashes of all four files are unchanged after normal encoding.

A same-size/timestamp-preserving original event edit after snapshot capture rejects
parent acceptance after child encoding. A tampered event input copy rejects at the
child's inventory gate before codec startup. Both tests modify only owned fixtures.
The legacy three-file authored native proof and both native artifact/package tamper
tests also passed. Default compiler tests never invoke native codecs.

## Next gates and effort

Approximately 50% complete / 50% remaining overall; this bounded source-closure
milestone does not establish an SDK release or justify a percentage jump.
Volume/weight expansion and selected-list cardinality are covered by their
follow-ups. Larger audio pools remain open. Broader WAV/resampling/codecs, production
hash/cache parity, remaining native type layouts, WorldBuilder and actual game
loading remain required work.
