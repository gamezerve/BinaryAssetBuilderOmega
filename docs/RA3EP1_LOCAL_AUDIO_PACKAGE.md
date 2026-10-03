# Fixed local AudioEvent to AudioFile package

Date: October 4, 2026. The encoded RAM/streamed AudioFiles now have a local
AudioEvent parent in the same verified v7 diagnostic package. This is fixed
local tuple/selector closure, not general core source-graph resolution,
production AudioFile admission, an EA hash algorithm or a playable mod.

## Pipeline and implementation

`AudioFileLocalEventProbe.Build` writes an exclusive owned `event.xml`, then
uses the existing core DocumentProcessor, official AudioEvent schema harness
and isolated Ra3Ep1AudioEventPlugin with GenerateOutput=false. The core performs
schema validation/default insertion and reference normalization. The fixed
source is AudioEvent RebornLocalAudio, Volume 60, Control INTERRUPT, with two
Sound references: RebornAudioRAM (default Weight 1000) and RebornAudioStream
(Weight 800). Both child Volume values default to 100 percent.

`Compile` revokes any previous validated table before checking the current event
identity and its two ordered normalized references against the captured local
AudioFile records. It explicitly prepares concrete handles with recovered type
hash 53C81E47 and each diagnostic content hash, then invokes the existing checked
AudioEvent processor. Failure clears preparation; valid recovery recompiles.
These are explicitly prepared local handles, not a claim that the core's general
DependencyResolution/Include/FileReference pipeline already handles local audio.

`Entry` owns copied event BIN/RELO/IMP and both AudioFile ID/hash tuples.
Its diagnostic InstanceHash includes native event BIN and dependency fingerprints.
Changing a valid custom payload invalidates the captured parent even though the
event's selectors/native bytes need not change. Fresh compilation refreshes the
parent hash. These hashes are not asserted to equal EA production InstanceHash.

`AudioFilePackageProbe.Serialize/Verify/Publish` now accept an optional frozen
parent. The previous two-entry mode is retained and its existing tests still pass.
The three-entry mode orders both AudioFiles before the event, writes its ordered
16-byte reference table, and independently checks both reader views plus native
selector-to-local-identity resolution. No external manifest is required.
Only the two AudioFiles get custom-data files; the AudioEvent has none.

## Native mapping

| Event offset | Meaning / observed value |
| --- | --- |
| 136 / 140 | Sound count 2 / relative array pointer 152 |
| 152 / 164 | One-biased selectors 1 / 2 |
| 156 / 168 | Weight 1000 / 800 |
| 160 / 172 | Child volume float 1.0 / 1.0 |

Event native sizes are 176/8/12 bytes. RELO words are 140, FFFFFFFF;
IMP words are 152, 164, FFFFFFFF. Independent checks reject altered array
count/pointer, selectors, weights, child volumes and relocation/import tables.
The supported base fields/defaults are checked by the existing schema-bound
AudioEvent processor; this package helper does not independently recover every
AudioEvent base-field byte or add wider audio options.

## Actual encoded package

The opt-in `audio-encoder-poc <absolute-audited-audio.dll>` command now creates
both its original two-file package and a separate `local-event-package` after
native shutdown. Actual successful output:

`C:\Users\drknt\AppData\Local\Temp\Reborn-AudioEncoder-628c5897144d4f11a82909f98532b8e5\local-event-package`.

The final native run also passed at
`C:\Users\drknt\AppData\Local\Temp\Reborn-AudioEncoder-efe46c215796404c8701bf3ed7c8673a\local-event-package`,
with the same custom hashes and package dimensions.

| File | Bytes |
| --- | --- |
| diagnostic.manifest | 324 |
| diagnostic.bin / diagnostic.relo / diagnostic.imp | 352 / 36 / 20 |
| DIAGNOSTIC_ONLY.txt | 210 |
| diagnostic/cdata/166b084d.53c81e47.44521d84.bbe648a4.cdata | 7,160 |
| diagnostic/cdata/166b084d.53c81e47.05bcd850.e7d8d8b8.cdata | 7,232 |

The manifest has three non-tokenized records: two AudioFiles (166B084D /
53C81E47) and one AudioEvent (844D7B9F / 560C2E45). Total native BIN is 344,
maximum native chunk 176, maximum RELO/IMP chunks 12/12, reference buffer 16
bytes and external-manifest buffer zero. Both internal readers pass; an actual
CLI inspect reports zero validation errors. The custom payloads and their SHA256
values remain identical to the [two-entry package](RA3EP1_AUDIOFILE_PACKAGE.md).
No codec decoding, audible playback or Uprising loading was performed.

## Tests and validation

`local-audio-package-self-test` is the 94th declared compiler test group.
Default fixtures use synthetic compressed bodies and never initialize codecs.
It exercises the actual core-normalized event, explicitly prepared local records,
two-reader native/reference offsets, 352/36/20 linked sizes and no external
manifests. Current wrong/missing/reversed/duplicate identities, wrong event type
hash, edited selector ordinals/names and nonfinite scalars reject and clear
preparation. Restoring valid input recovers the same frozen native/hash output.

Native copy mutations, stream/manifest corruptions, wrong selector/weight/pointer
and RELO/IMP data reject. A leaf custom body edit requires fresh parent preparation;
stale publication fails before staging. Refreshed data publishes and reads back;
existing output is preserved and original inputs recover identical payloads.
The original two-entry suite separately retains its missing/orphan/custom
corruption and frozen-copy coverage.

All 94 compiler groups, aggregate diagnostic build tests, layout tests and enum
sync checks passed. Both Release/x86 projects build. Model/marshaller inventory
remains 785/1,390 and 762/1,390. Estimated overall effort remains approximately
50% complete / 50% remaining; this narrow closure does not close SDK/game gates.

## Next gates

The full isolated AudioFile processor still needs core-normalized source/file
dependency checks, source snapshot publication and verified production
InstanceHash/processing policy. The general diagnostic-build command still does
not accept AudioFile roots. Arbitrary Include graphs, other durations/channels/
codecs, native error recovery, final type-table alignment, SDK/WorldBuilder
packaging and actual game loading remain open. Explicit fixed preparation must
not be mistaken for general dependency resolution or production/cache approval.

Staging retains the existing absent-destination/exclusive-create/readback/rename
policy. Failed owned evidence is retained; no original game or user output is
modified. No Qibbi writes or PRs are involved.
