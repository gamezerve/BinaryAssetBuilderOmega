# Inline root child enums and packed color

October 15, 2026. Three legacy GameObject members were pointer-shaped even though
EP1 stock metadata targets their root words directly. The native model now stores
BuildableStatus, ThingClassType and Color **by value** at the same offsets; the root
remains 600 bytes. Existing typed marshaller overload selection now writes values
there instead of allocating extra enum/color payloads.

| Source child | Native offset | Token type | Representation |
| --- | ---: | --- | --- |
| Buildable | 436 | F57E48C8 | Four-byte enum, official names YES..ONLY_BY_AI |
| ThingClass | 440 | 66BA691B | Four-byte enum, official names UNSPECIFIED..BUFF |
| DisplayColor | 460 | 9E3E975B | Four packed bytes B/G/R/A |

Independent stock metadata in five selected objects points directly to root+offset,
not a pointee. All selected stock values are zero. This is useful positive layout
and absent/default-state evidence, **not positive stock proof of nonzero values**.
The official Color4 declaration explicitly states byValue=true. Enum inventories
are checked against the reviewed official schema; raw enum ranges are 0..3/0..15.

## Source/native/token evidence

`gameobject-inline-child-self-test <static>` runs 4 Buildable literals × 16
ThingClass literals × 4 color profiles = **256 combinations**, plus an omitted
enum/color case. Raw source validates against the reviewed official candidate;
real selected allocation and token composition include destruction, Body and
Geometry. Native output remains exactly 856 bytes with the same owner addresses
in every case. The three root words are never relocation slots or asset imports.

Color profiles independently pin BGRA 44112233, FFFF0080, absent 00000000 and
present-empty FF000000. Present-empty uses the current marshaller's alpha=255
default; absent remains zero. The selected stock set does not establish nonempty
color/default runtime behavior independently, so no gameplay/EA-default parity for
these new edits is claimed. Future nonzero stock/runtime observations remain useful.

Five schema refusals cover unknown enums and invalid color bytes. Seven native
refusals cover out-of-range enum words and attempted pointer relocations in these
inline words. Six detached metadata controls check independent type/native-target
bindings; complete native bytes and relocation coordinates round-trip.

## Root inventory and regression maintenance

The three fields join the flat generated descriptor set, now **89 descriptors**
(62 scalar, 18 extra enum/flag/weak/inline-child, six string headers and three strong
slots). With selected Body, ArmorSet, movement, Behaviors and Geometry the partial
root now has **94 fields**. Previous milestone reports retain their then-current
counts; this growth does not complete the 126-field selected stock root inventory.

Combination tests now derive root counts from the descriptor set rather than old
87/88/89/90/91 constants. Their independently checked offsets, types, native owners,
imports and corruption controls remain in place. This prevents a new reviewed
flat field from accidentally breaking unrelated geometry/body/list boundaries.

Files: `GameObject.cs`, `Ep1GameObjectRootValueTokens.cs`,
`Ep1GameObjectStringTokens.cs`, `GameObjectInlineChildSmokeTest.cs`, descriptor-driven
count updates in the existing combination tests, inspector and Focused recipe.

This remains experimental source/native/token support, not alpha admission,
complete object publication, a new package or in-game acceptance. No game or
installed files are touched.

## Final regression

`artifacts/GameObjectInlineChildren-a/batch-report.json`: **48/48 Focused groups
pass in 127.251 seconds**. The inline-child group takes 3.515 seconds; eight runtime
hashes remain unchanged. The batch build succeeds in 1.21 seconds with zero
warnings/errors. GameExecuted and InstalledFilesModified are false; no Full-suite
or gameplay validation is claimed.
