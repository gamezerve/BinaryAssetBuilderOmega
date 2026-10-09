# EP1 runtime TypeHash consumer — October 9, 2026

## Result

A ResourceManager body at preferred VA `004AB5C0` performs an inline lookup
against the same TypeId-keyed list as the previously reviewed registry. At
`004AB615` it compares an entry-like record's `+8` hash with the registered
metadata object's `+8` hash. Its mismatch path includes the explicit diagnostic
`Type hash mismatch for type`, followed by expected and actual hash reads.
This is actual runtime hash-consumption evidence, not merely a matching literal.

The three reviewed direct calls to the small lookup `00417340` instead consume
the **opaque metadata word at +12**. They are not this hash comparison. The
meaning of that word and their stack word at `esp+B8` remains unresolved.

The entry layout is consistent with the inspector's 48-byte v6/v7 manifest
record: TypeId at 0, InstanceId at 4, TypeHash at 8, and Tokenized at 44.
See `source/BinaryAssetBuilder.ManifestInspector/ManifestReader.cs` and
`ManifestModel.cs`. Complete pointer provenance from stream parsing to this
function has **not** been recovered, so this report keeps runtime word 44
unnamed. In particular, it does not equate metadata object+12 with Tokenized.

## Pinned evidence

Image: `D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game`.
SHA-256: `ABA6A8825A744A78487837D78AD5992962257CE68404F639D600F11DEEA2580B`.
Addresses below are preferred-image VAs, not observed live process addresses.

| Reviewed region | Raw offset | Length | SHA-256 |
| --- | --- | ---: | --- |
| Lookup/hash gate | `000AB5C0` | 96 | `7EFDD9891F63790733E8823E93132BEE7ABE9BE8013CD19FF497CC32429C4349` |
| Mismatch diagnostic | `000AB61E` | 230 | `1C76143EF396CF4D9E9ED49BE34964FBE3C79E3403A0157D2D5F92861D866DF8` |
| First direct caller context | `000CEA54` | 69 | `61B95C5C3AFDCB3933161AE2F29DFAA8B2FAE1C496EF5FB24EF349E9178DC476` |
| Second direct caller context | `000CF194` | 69 | `C60813E03B61DB81BC9718806CD5C12B4E871E6BFFAE1DD242AAA666BDAE0731` |
| Third direct caller context | `000CF8CE` | 50 | `BF9812B269DAACDFDE22B8D2709887E979DA49295D524847C2006300C8B0D533` |

The contexts are bounded instruction regions, not claimed function boundaries.
Microsoft Visual Studio `dumpbin /DISASM:BYTES /RANGE` was used for static
instruction review. Ranges must start/end on complete instructions: a decoder
started inside an instruction or cut through a final instruction can produce
spurious trailing/leading disassembly. Those artifacts are not evidence.

## Observed control flow

The function loads list head `00CEA428`, gets the entry-like pointer from its
stack argument, reads TypeId/InstanceId from offsets 0/4, follows node+0,
compares node+4 to TypeId and retrieves metadata from node+12. This agrees with
[the separately reviewed registry](RA3EP1_REGISTRY_LOOKUP.md).

| Condition, in observed order | Action |
| --- | --- |
| No matching node or null metadata | Continue at `004AB704` without this comparison |
| Entry word at +44 is nonzero | Continue without this comparison |
| Entry hash at +8 is zero | Continue without this comparison |
| Entry hash equals metadata+8 | Continue without mismatch diagnostic |
| Otherwise | Enter diagnostic path at `004AB61E` |

These are engine branch observations, **not permission to emit zero hashes,
set a bypass flag, admit unresolved types or claim successful loading**.

Two further controls can skip the diagnostic to `004AB704`: the Boolean-like
return after call `0040EDB0` at `004AB625`, and a subsequent virtual call through
the object referenced by `00CE959C`. Their configuration and semantics remain
unknown. The final diagnostic virtual call at `004AB702` has not been identified
as fatal, recoverable or exception-throwing. Consequently, a mismatch is proven
to reach a diagnostic path, **not an unconditional resource-load rejection**.

`004AB67C` pushes the full message start `00BF2F50`, which contains the build-data
reminder followed by the mismatch text. Expected hash is read from metadata+8
at `004AB6C0`; actual hash is read from entry+8 at `004AB6E5`. The diagnostic's
source-location string at `00BF2F98` identifies
`E:\Projects\Ra3_ep1\Production\code\Libraries\Source\assetmanager\resourcemanager.cpp:224`.
That embedded path is provenance, not an available local source file.

The reviewed E8 scan yields direct small-lookup call candidates at raw offsets
`000CEA6B`, `000CF1AB`, `000CF8D8`. Each reviewed context tests `esp+B8 == 1`,
entry word+44, metadata presence and metadata word+12 to form a local Boolean.
The scan is a bounded raw-byte candidate scan, not proof of an exhaustive call
graph: indirect calls, other inline expansions and instruction/data ambiguity
outside the reviewed contexts remain out of scope.

## Reproduce and tests

`scripts/Get-Ra3Ep1TypeHashConsumer.ps1` imports the pinned registry evidence,
asserts five exact code slices in `Assert-HashConsumerSlices`, checks the reviewed
direct call-candidate set, and models only the first branch decision in
`Get-ObservedHashDecision`. It inherits bounded read-only image checks and
invokes only the existing repo-owned managed inspector's name-hash diagnostic.
It does **not** execute the game, native compiler or codecs. The image SHA is
checked again after the review; this is not an atomic filesystem snapshot.

```powershell
# Reborn: reproduce the pinned static TypeHash consumer without executing the target or changing compiler policy.
./scripts/Get-Ra3Ep1TypeHashConsumer.ps1 -ImagePath 'D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game' -AsJson
# Reborn: test reviewed code pins, pure branch cases and detached-memory corruption rejection.
./scripts/Test-Ra3Ep1TypeHashConsumer.ps1 -ImagePath 'D:\SteamLibrary\steamapps\common\Command and Conquer Red Alert 3 Uprising\Data\ra3ep1_1.0.game'
```

Validation: repeat JSON, seven pure branch cases (including noncanonical word 7
and zero runtime hash), and five detached code mutations rejected by slice pins.
The registry regression also passes. No C# compiler changes or full compiler
suite rerun are claimed; the previously executed suite remains 165 groups.

## Migration implications and next gate

Runtime name/table hash initialization now connects to a concrete comparison
consumer. This strengthens the need for authentic EP1 type identity rather than
merely replacing source XML/XSD. It does not derive authoring ProcessingHash,
AllTypesHash aggregation, cache identity or a compatible production processor.

Next recover bounded caller/record provenance for this function, then characterize
word44 and diagnostic control behavior without changing production admission.
PathMusicEvent initializer coverage, the missing EP1 AUDIO header, authentic
processing metadata, remaining native layouts and game-load validation are
separate unfinished gates. Engineering effort remains approximately **52%
complete / 48% remaining**; this closes one RE question, not a usable SDK release.
