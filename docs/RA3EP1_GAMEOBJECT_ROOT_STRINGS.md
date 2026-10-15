# Root strings and dynamic behavior allocation ownership

## Implementation

Six inline string-header fields now join the generated root metadata:
Description, DescriptionTransformed, TypeDescription, TypeDescriptionTransformed,
EditorName and ExperienceScalarTable. Total represented root fields is **84**
including Behaviors. This is not SDK completion or full GameObject support.

`Ep1GameObjectStringTokens.Inspect` replaces the old fixed behavior allocation
assumption (list=600, item=608, module=612) with pointer decoding and explicit
ownership validation. The 600-byte native root remains fixed; only its subsequent
allocations move. The decoded single-behavior list still has the admitted count /
item-array / module shape, but its starting address can follow root string data.

Root string pointers must reference bounded, null-terminated native ASCII data
between the root and behavior list. Absent strings require length=0/pointer=0;
explicit empty strings retain a nonnull allocated terminator and relocation.
Partial allocation overlaps are refused; identical shared spans are permitted by
the ownership check but not separately stock-corroborated here. Root relocations
must be exactly the present string-pointer slots plus the three decoded behavior
envelope slots. Root strong imports and other pointer owners remain refused.

The composer splits the destruction tail at the decoded module address, not a
hardcoded 612. Generated token targets, native pointer values and RELO/IMP slots
are rebased using that actual allocation boundary. Complete original native bytes
and reference indices survive inverse shifting. No stock prefix is copied into
the production writer.

## Batched evidence

- Six stock string-header field/type/native offsets match two independent roots.
- Four authored modes: omitted, explicit empty, varying ASCII with XML escaping,
  and 4096 characters per field; each uses three destruction Items.
- Literal source/header/length/text/terminator checks before and after global
  rebasing; independently decoded root/container/module links and full native /
  auxiliary round trips. Present strings must actually move the module.
- 37 detached native failures: malformed lengths, pointers into root/module,
  missing terminators/relocation owners, non-ASCII bytes and partial overlap.
- A separate integration validates raw authored root/module XML, then compiles
  the separately Core-normalized named FX leaf with root strings. Moved module
  allocation and nonempty native imports must coexist and preserve all indices.
- Combined metadata descriptors now have 166 independent type/target corruption
  checks; prior module/base/input/enum/flag/weak controls remain in the same batch.

The first authored test fragment was accidentally imported without the EA
namespace; schema validation refused it. The test wrapper was corrected to give
unqualified fragments the proper default namespace, without weakening the schema.

## Explicit limits

Only controlled ASCII fixtures/native storage are admitted by this projection
contract. This is not a general Unicode/ANSI codec validation or localization
pipeline. The shared legacy string marshaler is unchanged; a future source build
profile must explicitly validate encoding before that stage rather than assuming
that ASCII-looking native output proves lossless arbitrary Unicode input.

Schema tests use the explicit reviewed shield-first/hook candidate. Required Body
is validation context only, as in prior partial-root tests. Strong root references,
other allocation owners, additional containers/modules and complete source graph
admission are not made supported. No asset registry processor, manifest/BIG,
alpha package change, game launch or gameplay acceptance is claimed.

## Files and next gates

- `source/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder/XmlCompiler/Ep1GameObjectStringTokens.cs`
- `Ep1GameObjectDestructionProjection.Compose`: dynamic module boundary and target rebasing.
- `source/BinaryAssetBuilder.ManifestInspector/GameObjectRootStringSmokeTest.cs`
- `GameObjectScalarTokenSmokeTest.Compile`: recover actual module start for tests.
- `Ep1DestructionSourceSmokeTest.Run`: actual Core FX + moved strings integration.

Next gates: remaining root strong references and indirect fields, source encoding
admission, additional required modules/containers and a complete validated mixed
GameObject stream before expanding the SDK alpha.

## Final regression

`artifacts/GameObjectRootStrings-a/batch-report.json`: **30/30 Focused groups pass**
in **83.366 seconds**. Root/native family checks take **6.683 seconds**; actual
Core/destruction/string-integration checks **6.494 seconds**. The no-root-string
stock-source baseline is now BIN/RELO/IMP **2152/24/8**, including a 1428-byte
partial token prefix. Present-string native sizes/addresses vary as tested.
37 string ownership/content corruptions, 166 metadata type/target corruptions
and all prior refusal controls pass. The batch incremental build takes 1.18
seconds with zero warnings/errors. Eight runtime hashes remain unchanged during
workers; no game runs or installation files change. No new Full-suite result,
general encoding, complete GameObject publication or alpha release is claimed.
