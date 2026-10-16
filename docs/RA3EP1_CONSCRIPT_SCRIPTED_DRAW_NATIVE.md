# Complete selected Conscript scripted-draw native graphs

## Outcome and exact boundary

The complete official main and parachute scripted-draw leaves now compile through
real Draw dispatch and actual Core name normalization, without removing models,
animation states, particles, scripts or other source members. This is component
work toward the **complete new-ID Conscript**, not a reduced substitute unit.

| Draw | Model states | Animation states | Native BIN / RELO / IMP |
| --- | ---: | ---: | --- |
| `ModuleTag_Draw` | 5 | 80 | 25304 / 712 / 636 |
| `ModuleTag_ParachuteDraw` | 5 | 4 | 2200 / 72 / 56 |

Across both graphs the verifier visits 4,767 typed fields/records, 70 nonnull
string allocations and **171 strong import slots**. Each import matches the
independent stock type/instance identity. The exact relocation/import slot
inventory and complete Core reference consumption must agree; repeated builds
match all three buffers.

The main graph matches stock native bytes through typed pointer/reference
canonicalization, including script text. The parachute graph has exactly **one
explicit Script CRLF/LF equivalence**: its supplied source contains a 177-byte LF
script, while stock contains the otherwise identical 182-byte CRLF script with
five CRLF pairs. Source and producer bytes are not rewritten to hide this. Only
paired CRLF versus LF is compared equivalently for the explicitly selected
`.Script` field; arbitrary text/whitespace changes and corrupt lengths/terminators
still fail. This is not byte-identical script storage or LUA execution evidence.

The subsequent [recursive metadata gate](RA3EP1_CONSCRIPT_SCRIPTED_DRAW_TOKENS.md)
now covers both complete selected scripted graphs. Full Draws/container
composition, complete GameObject streams, XInclude execution, gameplay and
general art-asset compilation remain open. Stock W3D references are **not**
newly compiled models.

## Two actual compiler defects corrected

### ParticleSysBone: correct size, wrong interior layout

The previous record was already 36 bytes, but size-only checks missed misplaced
fields and an incorrect hash representation for `id`. All fourteen selected
Conscript particles independently bind nine stock token names/types to these
native offsets:

| Offset | Field | Stock token type |
| ---: | --- | --- |
| 0 | BoneName, ASCII header | `1A2933AA` |
| 8 | FXParticleSystemTemplate | `43F1CE36` |
| 12 | FXTrigger | `808B85BE` |
| 16 | FXAction | `7C49BC33` |
| 20 | PersistID | `23E650B8` |
| 24 | id, **8-byte ASCII AssetId header** | `805D25F4` |
| 32 / 33 / 34 | FollowBone / OnlyIfOnWater / OnlyIfOnLand | `77E3F4F7` |
| 35 | padding | zero |

EA's local RA3 `BinaryAssetBuilder.Tokenizer.dll` independently confirms this in
method **0x06000057**, including `Marshaler.Marshal<char>` at offset 24. This is a
metadata/IL read, not execution of the mixed-mode DLL. The old POID/hash description
in historical implementation notes is superseded. The current marshaler naturally
uses its existing ASCII overload after the field type is corrected.

Authored controls combine 3 trigger enums, 4 action enums, 8 boolean masks and
absent/empty/nonempty ids: **288 cases**. Literal signed PersistID, complete
header/text/terminator bytes, exact relocations at BoneName+4 and optional id+28,
enum ordinals, tail flags and padding are checked. These tests use the official
source's `true`/`false` spellings; no global numeric-Boolean parser expansion is
claimed.

### AnimationState default flags

Omitted Flags must default to the `NONE` bit (**1**), not an empty bit set (**0**).
The source/schema/stock distinction is pinned by omitted, explicit `NONE` and
explicit empty-list controls. The existing combined animation/particle compiler
fixture is also executed directly in this group; its FXAction expectation is
corrected from particle+20 to particle+16. Native particle size remains 36.

## Source preparation and reference resolution

The main official XML has two raw schema diagnostics from interleaved bone
families. Two parent sequences require stable family grouping before strict
validation. Every member, within-family order, total descendant count and every
script's text are retained. Unknown children refuse. The parachute leaf needs no
reordering. Original external files are read-only; their SHA-256 values are
rechecked after compilation.

Untyped `BaseRenderAssetType` names are resolved against allowed schema-derived
types in the real static/global catalogs, then passed as concrete **names** to
the existing Core reference normalizer. Four main and three parachute render
names resolve this way. Missing and ambiguous model identities refuse; no
captured input reference indices are authored.

The explicitly reviewed in-memory schema candidate gains two diagnostic global
element aliases for the unchanged official concrete types. The focused test
rebinds Core's reference helpers to that exact set. Its old seed inheritance/
dependency caches are deliberately not used. This is a reference-normalization
test seam, **not** a general replacement SchemaSet/DocumentProcessor constructor,
production schema admission or unrestricted source/inheritance support.

The two source leaves are read separately from the official source tree. The
parachute include's presence and full contents are retained as the target, but
the legacy `Mvp.Xml` XInclude/XPointer runtime path is not executed by this test.
That integration still needs its own gate rather than an implied pass.

## Files and regression

- `SelectedNativeGraphComparer.cs`: recursive byte/pointer/string/list/fixed-mask/
  import comparison, bounded reads, exact auxiliary ownership and orphan-storage
  rejection. Stock string interning need not match compiled allocation addresses.
- `Ep1ScriptedDrawNativeSmokeTest.cs`: official source capture/preparation, actual
  Core names/Draw dispatch, both complete selected graphs and fault controls.
- `Modules/BaseModules.cs`, `Marshaler.ModulesScriptedModelDraw.cs`: the particle
  ABI and animation-default fixes.
- `UprisingLayoutSmokeTest.cs`, `CompilerSmokeTest.cs`: corrected independent
  particle offsets and existing combined-record expectations.

The test includes **22 detached native/auxiliary/script refusals** and **3 source/
dependency refusals**, in addition to the 288 particle and three default controls.
The Focused batch includes it when explicitly passed `-UprisingXmlRoot`; without
that external source input, the existing 58 groups remain the default.

Final Focused regression with the explicit XML root passed **59/59** groups in
**217.593 seconds**. The scripted-draw group passed in **8.906 seconds**; all
eight frozen runtime file hashes remained unchanged. The final batch build
completed in 1.55 seconds, zero warnings/errors. Earlier rebuilding attempts
reported the existing 111 solution warnings or two existing XmlCompiler warnings;
they are not represented as a clean full rebuild. Initial test/compile failures
were corrected before this final source/run.

Local evidence: `artifacts/ConscriptScriptedDrawNative-a/batch-report.json`,
`ep1-scripted-draw-native.log` and `compiler-build.log`. No Full matrix, game
launch, install mutation, alpha expansion or whole-Conscript publication is
claimed. The broad runner itself was not rerun; its relevant existing combined
animation/particle fixture was executed directly inside this new group.

The subsequent token checkpoint implements recursive metadata for the shared
model-state, animation-state, animation, bone, particle, string and list shapes.
Next: combine the two scripted draws and two already-tokenized tracers on one
Draws container/reference graph. Neither checkpoint itself publishes a unit.
