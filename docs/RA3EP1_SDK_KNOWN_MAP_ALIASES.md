# Known map Include aliases — October 8, 2026

## Result and scope

Explicit `--known-map-aliases`, together with `--instance-sound-singletons`,
expands the real global source graph from **396 to 398 validated documents**
and from 664 to **666 Include edges**, with zero XML blockers or earlier-valid
regressions. Path issues fall from six to four. There are still 198 dependency
occurrences of one required AUDIO PathMusic header; graph closure is false.
This is diagnostic source preparation, not native emission or game readiness.

Official XML/XSD and Core implementation remain unchanged. Both expanded runs
agree on document statuses and processed hashes; all 398 captured raw files
were rehashed after validation with zero mismatches. No output directory exists.

## Exact admission contract

Only two `all` Includes have recipes:

| Source library under DATA | Original Include target under DATA |
| --- | --- |
| `SkirmishAI/Personalities/AIPersonalityLibrary.xml` | `maps/official/CAMP_S06_Iceland_Bass/AIP_S06_SovietKrukov.xml.` |
| `SkirmishAI/States/AIStateLibrary.xml` | `maps/official/CAMP_S06_Iceland_Bass/AIS_S06_SovietKrukov.xml.` |

Slash variants and case-insensitive Windows identities are recognized, but the
original edge spelling and `all` role are retained. There is no generic trailing
dot removal, source rewrite, registry lookup, alternate root or manifest fallback.
An `all` Include does not gain `instance` inheritance-handle authority.

`SdkKnownMapAliases.Prove` requires the exact source library/role, Windows native
canonical equality of dotted and ordinary paths, bounded equal byte snapshots,
and confinement/reparse checks from the existing strict reader/resolver. The
ordinary target must be captured as valid XML with matching SHA-256/length.
`ResolveCaptured` replays the native proof and requires an unchanged witness
present in captured inventory. At most two witnesses are retained. This is a
bounded snapshot proof, not an atomic concurrent-filesystem transaction.

Integration points: `SdkSourcePathAudit.Inspect`,
`SdkIncludeDefineProfile`, `SdkInstanceInheritanceProfile`, and `Program`.
The path-only command, defaults and earlier preprocessing profiles remain strict.
Unknown aliases still use the original resolver and are rejected when unsafe.

## Map source preparation evidence

Reference root:
`D:\OneDrive\CNC Files\CnC_Modding_Support-main (Official XML, Schema, Script, Shader, Maps)\Uprising\Xml (Uprising)`.

Both ordinary map paths were prepared independently before alias admission:

- AIP closure: four documents, three Includes, all validated. One overlay follows
  `BasePersonality` → `SoloBasePersonality` → `SovietSoloBasePersonality` →
  `AIP_S06_SovietKrukov`; the global graph retains four prepared-source witnesses.
- AIS closure: two documents, one Include, both validated. The map state source
  has no inherited owner and no inheritance overlay; its context includes
  `SkirmishAI/States/SovietBaseStates.xml`.

AIP raw SHA-256 (3,657 bytes):
`E24D6E4E54F232E1D07EAEEB521B810A4C889D4038900C691736CAC5235467B9`.
Processed inherited document:
`AA99FC6925919C2D3187049C495D7D19E9E43F619B7C8A5F7ACA8BE60171D911`.
AIS raw SHA-256 (2,477 bytes):
`9E9A92CA296CE32DD5C18D9D89FCDF07D4E97DCEEF02F3419BC3519DACA6C5ED`.
These are source snapshot identities, not native asset/type hashes.

## Validation and reproduction

Run `sdk-known-map-aliases-self-test` for owned fixtures. The registered compiler
group 151 covers exact library/role, native equivalence, duplicate-edge shared
capture, schema-valid expanded fixture graph, unchanged default paths, wrong
context/role, generic unsafe paths, forged/stale witnesses and DTD target refusal.
Non-Windows admission remains closed. Four CLI misuse checks reject missing
singleton profile, duplicate option, path-only command and the older offset profile.

Run the existing `sdk-typed-source-graph ra3ep1` command with the reviewed schema
root, reference root, `global.xml` and a new unused output-directory argument;
append `--instance-sound-singletons --known-map-aliases`.
The expanded real graph exits 2 because dependencies remain unresolved, not
because the newly captured map XML fails schema preparation. Omit the alias
option to reproduce the strict 396-document/six-path-issue baseline.

All **151 compiler groups pass**. Model/marshaller coverage remains 785/1,390
and 762/1,390 (EP1-only 48/48); an incremental Release build has zero errors
and warnings. No native codecs are invoked by these default tests.

## Remaining work and risk

The four path issues are three `AUDIO:audioassets*.xml` Includes from
`Sounds/MasterAudioAssets.xml`, plus the header reference from
`PathMusic/BasePathMusicEvent.xml`. No explicit AUDIO root is supplied, so this
does not prove physical absence. All 198 typed occurrences share
`AUDIO:Pathfinder\RA3EPMus\PC\RA3EPMus.h`.
Recover authentic AUDIO sources/header or establish a separately validated
precompiled-reference contract and PathMusic processor semantics. Compiled stock
manifests cannot simply substitute for these source inputs.

Overall effort stays **52% complete / 48% remaining**: this closes two source
paths, not a major native layout, type-table, dependency or game-loading gate.
See [dependency classification](RA3EP1_SDK_DEPENDENCY_REVIEW.md) for stock evidence.
