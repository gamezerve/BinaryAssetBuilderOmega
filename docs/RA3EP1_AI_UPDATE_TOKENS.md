# Generated EP1 AIUpdate and chooser metadata fragments

October 15, 2026. The [native AI port](RA3EP1_AI_UPDATE_NATIVE.md) now has a separate
generated-token gate. Ep1AIUpdateTokenizer emits metadata from a checked owned
native graph; it never copies stock metadata into the producer output. Complete
independent stock prefixes are used only by the diagnostic verifier.

## Metadata layout

| Region | Start | Size |
| --- | ---: | ---: |
| Concrete AI wrapper | 0 | 12 |
| AI own fields, parent row and terminator | 12 | 216 |
| Empty Update/Contain/Behavior inherited tables | 228 | 72 |
| Terminal ModuleData TypeId/id fields and terminator | 300 | 36 |
| Empty SpecialContactPoints count anchor | 336 | 12 |
| Optional chooser own reference/base fields | 348 | 36 |
| Optional chooser inherited primitive/list fields | 384 | 156 |
| Optional empty ObjectVisionRange count anchor | 540 | 12 |

The native payload therefore starts at **348** without a chooser or **552** with
one. The concrete wrapper uses AIUpdateModuleData's 08DBC00E wire ID, not the C#
model name. Field types and targets include Time, unsigned MaxCombineChildren,
state/locomotor enums, packed booleans, direct ASCII headers and a real strong
TargetingCompareList reference. Empty lists still have count-anchor metadata;
they are not represented by arbitrary zero tables.

All native pointer values and relocation slots rebase once; strong reference
occurrence values do not rebase. Metadata targets have their own coordinate
origin and are not inserted into the native relocation stream.

## Ownership and current admission boundary

The input is one bounded AI native fragment with a 76-byte root, a nonzero local
id hash, optional ASCII storage and an optional aligned 40-byte chooser. Root and
chooser fields, finite reals/times, enum/mask ranges, packed booleans and padding
are checked before output allocation. The optional strong reference must be
nonzero with exactly its expected import slot; actual named target/type admission
is proven separately through Core/catalog tests, not inferred from that number.

Records and string storage must be disjoint and bounded. ASCII text is non-NUL,
at most 4096 characters, with an exact terminator. Only up to three zero alignment
bytes may remain between/following allocations. Unknown tails, aliases and extra,
missing, duplicate or malformed auxiliary slots refuse.

**Both lists are empty-only in this token profile.** The native AI port supports
up to 32 SpecialContactPoints, but their nonempty element-token representation is
not admitted here without independent evidence. ObjectVisionRange remains
empty-only at both native and token gates. No producer strips these inputs.

## Independent evidence and tests

`ep1-ai-update-token-self-test <static> <global>` retains the complete existing
AI native/default/schema/Core/reference/inline-dispatch tests and adds:

- Complete independent metadata prefix comparison for AlliedConstructionYard and
  SovietAntiInfantryInfantry. Only bounded metadata/native/chooser origins are
  normalized, and any target outside those owners refuses.
- **680 explicitly empty-contact source variants**, across all 17 state-machine
  names, 20 locomotor names, optional chooser and absent/empty/nonempty ASCII
  AttackPriority profiles. Original nonempty-contact sources are separately
  asserted to refuse tokenization; the empty variant is a deliberate test source,
  not a silent producer fallback.
- A positive 4096-character text boundary and explicit refusal of the native
  32-contact boundary at the token gate.
- Independent expected field tables using literal stock field hashes, complete
  native inverse rebasing and exact RELO/IMP preservation.
- Native corruption controls and detached mutation of **every metadata word**,
  plus payload, pointer and auxiliary-output faults. Detached verification does
  not invoke the producer again to repair the corrupted output.

The standalone follow-up passes **53 native** and **142 detached metadata,
payload, pointer and auxiliary-output refusals**, including every one of the 138
words in the present-chooser metadata prefix. Its final incremental build has
zero warnings/errors. Full Focused regression is recorded separately below.

The two reviewed stock string/vision lists are empty. Browser lists in these
same stock objects are also empty, and a targeted search of the supplied XML
tree found no SpecialContactPoints definitions. These are not nonempty-list
proof and are not used to invent the element encoding.

## Files and next work

`Ep1AIUpdateTokenizer.cs`, `Ep1AIUpdateTokenSmokeTest.cs`, the opt-in token branch
in `Ep1AIUpdateSmokeTest.cs`, inspector CLI and Focused recipe. The native-only
command remains separately available.

The next step is the inline AI container in the shared combat/geometry/root
composer: explicitly partition AI records from other pre-behavior allocations,
join its list/wrapper metadata, preserve a single Core occurrence table and widen
module-local identity checks across AI/Body/Behaviors. Nonempty list elements,
other AI classes and Draw/client graphs remain separate gates.
Independent root reviews pin that inline AI field's type tag to **5FD8691D**
at native +480 in both selected stock objects; its list count marker targets the
inline count, not a pointer-to-list header. This observation is a contract for the
next composition gate, not an already generated shared-object result.

The selected composed GameObject root remains at **99 fields**. A standalone AI
fragment does not yet make that root publishable, add a new unit to a stream,
expand the Studio alpha or establish gameplay. No game or installed files change.

## Final regression

`artifacts/Ep1AIUpdateTokens-a/batch-report.json`: **53/53 Focused groups pass
in 390.651 seconds**. Native/token AI groups take 27.763/28.998 seconds; eight
runtime hashes remain stable. The batch incremental build succeeds in 2.34
seconds with zero warnings/errors. GameExecuted, InstalledFilesModified and
AlphaCapabilitiesExpanded are false. No Full-suite or gameplay run is claimed.
