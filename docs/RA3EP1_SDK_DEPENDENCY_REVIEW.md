# Remaining dependency and path classification

## Outcome (2026-10-08)

Read-only `sdk-dependency-review ra3ep1 <schema-root> <source-root>
<source-entry.xml> <new-output-directory>` runs the admitted known-sound
singleton XML profile, then groups recorded failures without changing resolver
rules. Current snapshot: **396/396 captured XML documents validate**, six path
issues, **198 dependency occurrences but only one distinct logical dependency
path**. Exit code remains 2; no output directory is created.

The 198 occurrences are all `PathMusicEvent/@PathfinderEventHeader` pointing to
`AUDIO:Pathfinder\RA3EPMus\PC\RA3EPMus.h`: one on BasePathMusicEvent.xml and
197 on prepared PathMusicEvents.xml owners. This is not 198 missing physical
files. No explicit AUDIO root was supplied; ResourcePath does not prove absence.

## Six path issues

| Count | Document / logical path | Evidence classification |
| --- | --- | --- |
| 3 | Sounds/MasterAudioAssets.xml: AUDIO:audioassets.xml, audioassets_notfordemo.xml, audioassets_retailonly.xml | AudioRootNotSupplied; all-role source Includes remain unresolved |
| 1 | PathMusic/BasePathMusicEvent.xml: AUDIO:Pathfinder/RA3EPMus/PC/RA3EPMus.h | AudioRootNotSupplied; required typed build header, not an AudioFile payload |
| 1 | SkirmishAI/Personalities/AIPersonalityLibrary.xml: DATA:maps/official/CAMP_S06_Iceland_Bass/AIP_S06_SovietKrukov.xml. | KnownTrailingDotOutsideResolver |
| 1 | SkirmishAI/States/AIStateLibrary.xml: DATA:maps/official/CAMP_S06_Iceland_Bass/AIS_S06_SovietKrukov.xml. | KnownTrailingDotOutsideResolver |

The staged `AssetTypePathMusic.xsd` declares PathfinderEventHeader as required
FileReference on PathMusicEvent. Current source searching found no dedicated
PathMusicEvent implementation in the managed source tree; valid XML alone
therefore does not establish a working event processor or native emission.

## Exact local trailing-dot observations

Only the two known candidates were observed, under the explicit source root.
On this Windows/.NET host, File.Exists/OpenRead accept the dotted spelling,
Path.GetFullPath canonicalizes it to the ordinary `.xml` path, and both reads
have equal length/hash. Candidate hashes were rechecked during observation.
No arbitrary trailing-dot path is normalized or admitted by the resolver.

| Candidate | Bytes | Ordinary and dotted SHA-256 |
| --- | ---: | --- |
| Maps/official/CAMP_S06_Iceland_Bass/AIP_S06_SovietKrukov.xml | 3657 | E24D6E4E54F232E1D07EAEEB521B810A4C889D4038900C691736CAC5235467B9 |
| Maps/official/CAMP_S06_Iceland_Bass/AIS_S06_SovietKrukov.xml | 2477 | 9E9A92CA296CE32DD5C18D9D89FCDF07D4E97DCEEF02F3419BC3519DACA6C5ED |

Both candidates report CandidateCaptured=false: they are outside the current
396 reachable source documents, because their only observed Include references
are rejected before traversal. Thus 396/396 is not proof that these two additional
sources are valid. A future narrowly scoped alias option must capture them,
preserve Include roles, prepare their own closures and re-run the expanded graph.
Byte snapshots/canonical spellings are not a native file-ID or atomic filesystem
identity proof, and do not authorize rewriting the reference XML.

## Audio inputs and compiled assets are different evidence

Targeted filename inventory found no audioassets*.xml or RA3EPMus.h in these
explicitly searched locations:

- D:/TEMP/RA3 Uprising Sources
- D:/TEMP/Red Alert 3 Uprising Source Data
- D:/TEMP/Audio Manifest
- D:/OneDrive/Documents/GitHub/BinaryAssetBuilderOmega2
- The user's Uprising support directory (Xml/Schemas/Libraries/Shaders)

This is scoped search evidence, not a claim that no copy exists anywhere on the
machine. No directory was guessed as AUDIO root and no registry fallback ran.

Stock metadata provides distinct compiled-asset evidence:

- `D:/TEMP/Red Alert 3 Uprising Source Data/Global Data/data/global.manifest`
  reports 197 PathMusicEvent entries, TypeId 9A651D89 / TypeHash 599CDAF2,
  manifest v7 and AllTypesHash 5454A8E9. Counts alone do not match names/IDs or
  recover the authored header/event-number mapping.
- `D:/TEMP/Red Alert 3 Uprising Source Data/EnglishAudio/data/audio.manifest`
  reports 12951 AudioFile entries, TypeId 166B084D / TypeHash 53C81E47,
  manifest v7 and the same AllTypesHash. This English audio package does not
  establish complete coverage of the three all-role source XML Includes.

Metadata snapshot SHA-256:

- global.manifest: `08A415789062B1707EDBA3C456884B94791097505431E17A03D10EE34DF050FD`
- English audio.manifest: `0CF62FBECC89F15746E523042A6F0422869D98FE41EB742C3104324A81D9A2E4`

Only manifest metadata was inspected for these findings, not audio.bin or
WorldBuilder payloads. A compiled manifest is not interchangeable with an all
source Include or a C header. Do not fabricate an empty header/source collection,
silently suppress dependencies or infer event constants from counts.

## Implementation and validation

`source/BinaryAssetBuilder.ManifestInspector/SdkDependencyReview.cs`:
`Inspect` groups by issue, owner type, field kind/name and exact logical path,
retaining occurrence counts and affected documents. `Category` distinguishes
AudioRootNotSupplied from ScopedResourceAbsent and unclassified failures.
`Observe` reads only two exact confined non-reparse source candidates (4 MiB
per file), with bounded native dotted-path observations and a snapshot recheck.
Stopped inventories or more than 4096 dependency occurrences refuse classification.

Program rechecks all captured source hashes before printing the report.
ResolverChanged, FullDependencyCoverage and ProductionBuildReady remain false.
Earlier graph/source report behavior is unchanged. Group 150 in
`SdkDependencyReviewSmokeTest.cs` covers occurrence-vs-path deduplication, distinct
field/document identity, supplied-vs-unsupplied root, scoped absence, stopped
inventory rejection and unknown alias isolation. Default fixtures need no external
source/game files or codecs.

Validation: focused fixtures and all 150 compiler groups pass. Missing arguments
and wrong target return exit 1. Real review returns exit 2 with no output directory,
after captured-source post-rechecks. Coverage remains 785/1390 complex models,
762/1390 typed marshallers and 48/48 EP1-only types.

## Next gate and estimate

First characterize both uncaptured map sources under their explicit ordinary
paths, including their own inheritance/Include contexts. Only then consider a
separate two-alias opt-in and expanded-graph validation; retain generic trailing-
dot/space/device/traversal refusals. Separately locate authentic AUDIO inputs or
prove a target-aware precompiled-reference contract, preserving all/reference/
instance semantics and complete native identity checks. The PathMusic header
and processor require their own recovery/validation, not arbitrary resolver edits.

Effort remains **52% complete / 48% remaining**. Classification improves the
work breakdown but closes no additional source/dependency/native/game gate.
