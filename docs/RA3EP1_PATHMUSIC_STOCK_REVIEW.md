# PathMusic authored/stock reconciliation — October 8, 2026

## Outcome

New read-only `pathmusic-stock-review <PathMusicEvents.xml> <global.manifest>`
matches the **exact 197 authored event names and instance IDs**, not merely an
equal count. All selected assets have EP1 TypeId `9A651D89`, TypeHash `599CDAF2`,
Tokenized=0 and no strong manifest references/imports. The 19 local
`RestartAlternateEvent` targets resolve within this same named set and correlate
with the stock relocated weak-ID payloads.

Only **3,228 BIN bytes and 152 RELO bytes** are read per observation pass;
unrelated BIN/audio/WorldBuilder payloads are not dumped or decoded. Two complete
command runs return identical reports, and each command internally repeats its
selected slice reads, checks metadata/source hashes and rechecks stream framing.
Source/header/processor recovery and production readiness remain false.

## Observed native shape

| Offset / stream | 178 events without alternate | 19 events with alternate |
| --- | --- | --- |
| BIN size | 16 bytes | 20 bytes |
| BIN +0 | zero | zero |
| BIN +4 | observed opaque event word | observed opaque event word |
| BIN +8 | zero | pointer offset 16 |
| BIN +12 | 1 | 1 |
| BIN +16 | absent | alternate event instance ID |
| RELO | empty | words `8, FFFFFFFF` |
| IMP / strong metadata refs | empty | empty |

The staged `AssetTypePathMusic.xsd` distinguishes authored `PathMusicEvent`
(required PathfinderEventHeader) from `PathMusicEventRuntime` (EventNameHash,
optional weak alternate and default-true IsCacheable). This schema supports the
runtime-field interpretation, but the review deliberately reports offset +4 as
`Word4`: it does not recover how the processor derives/validates that word from
the missing header or prove a general native serializer.

Examples:

| Event | Instance ID | Word4 | Alternate / stored ID |
| --- | --- | --- | --- |
| Initialise | 7D614792 | 01925661 | none |
| MenuTrack | 8CD75D8B | 01B9D554 | none |
| S_A01IgaIntro | A819AE67 | 012922C2 | SR_A01IgaIntro / F590F8AD |
| SR_A01IgaIntro | F590F8AD | 011A2A78 | none |

There are **194 distinct Word4 values** across 197 events. Four records contain
zero: `S_MPThreat1_1BrightonAction`, `S_MPThreat1_1NYCExplore`,
`S_MPThreat1BrightonAction`, `S_MPThreat1NYCExplore`. None of the 197 Word4
values equals its asset instance ID. Zero values must not be silently replaced,
or interpreted as proved valid header constants/hash collisions. Their source
and runtime semantics remain an explicit reverse-engineering question.

## Exact inputs and snapshots

Source:
`D:\OneDrive\CNC Files\CnC_Modding_Support-main (Official XML, Schema, Script, Shader, Maps)\Uprising\Xml (Uprising)\PathMusic\PathMusicEvents.xml`

SHA-256: `BBE869BE8AC6D6C9C127BAA8AEF9C039BE7C7C3B7D740D21DD25EC35F514E4DD`.

Stock:
`D:\Temp\Red Alert 3 Uprising Source Data\Global Data\data\global.manifest`

SHA-256: `08A415789062B1707EDBA3C456884B94791097505431E17A03D10EE34DF050FD`.
Companion `.bin/.relo/.imp` lengths and leading linked checksums match manifest
metadata. Snapshot checks are not an atomic adversarial filesystem transaction
or complete stream cryptographic integrity proof; only selected records are hashed.

## Additional AUDIO source inventory

Targeted filename inventory under `D:\Temp`, the RA3 SDK, RA3 Source Files,
MOD SDK All Games and the CnC support tree returned no `RA3EPMus.h`.
This is scoped inventory evidence, not machine-wide absence proof.

`D:\OneDrive\CNC Files\RA3 MOD SDK\audio\AudioAssets.xml` exists (10,207 bytes),
SHA-256 `8E3972264301AB078AB0A99D1A4179ADECF055AA695689E12BE4B07B9498DD94`.
It defines 94 AudioFile owners, no source Includes, and all 94 referenced payload
paths exist under that RA3 audio root. Payload bodies were not read. This is a
candidate RA3 input collection, not proof of authentic/complete Uprising coverage
of `audioassets.xml`, `audioassets_notfordemo.xml`, `audioassets_retailonly.xml`.
It has not been assigned as the Uprising AUDIO root or used to suppress issues.

## Implementation and tests

`PathMusicStockReview.Inspect` bounds source/manifest metadata, rejects reparse
paths, checks all three linked stream lengths/checksums, reads only selected
chunks, repeats the review and post-rechecks source/manifest identities.
`Review` requires valid non-patch little-endian linked EP1 metadata, unique exact
name sets and stock type/instance identities. Authored owner admission is limited
to literal id/inheritFrom/RestartAlternateEvent attributes with the known base;
this is not general inheritance/schema preparation. It checks 16/20-byte shape,
relocated weak-ID target and sentinel, and keeps every opaque word unchanged.

`PathMusicStockReviewSmokeTest` is registered compiler group **152**. Synthetic
fixtures cover offset/read bounds, both record shapes, exact names versus counts,
duplicate IDs/names, wrong game/link/patch/type/hash/token/instance/size/import/ref
metadata, outside alternate targets, wrong base/attributes, DTD input, changed
base/cache/pointer/target words and relocation offset/sentinel rejection. Focused
tests and all 152 compiler groups pass; missing CLI arguments return exit 1.
An initial dependency rebuild reported the three existing Core/XmlCompiler
warnings; subsequent incremental Release compilation has zero warnings/errors.
No default test invokes native codecs.

Model/marshaller coverage stays 785/1,390 and 762/1,390, EP1-only 48/48.
No Core, official source/schema, registry, processor or resolver changes.

## Next bounded gate

Do not fabricate a header from this table or treat Word4 as the ordinary asset
symbol hash. Recover the reference PathMusic processor/header lookup contract,
including unknown/zero event handling, before admitting authored music emission.
An alternative stock-reference path needs its own exact identity, dependency,
source-role and cache-invalidation contract; this review does not enable it.

Expanded XML graph remains **398 validated, four AUDIO path issues and 198
occurrences of one required header**, exit 2 with no output. Overall effort stays
**52% complete / 48% remaining**: full dependency closure, native production
emission, type-table compatibility and in-game loading are still open.
