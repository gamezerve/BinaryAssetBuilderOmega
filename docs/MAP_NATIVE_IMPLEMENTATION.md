# EP1 map native implementation

<!-- Reborn: distinguish the verified source-to-package build from runtime validation. -->
Build Maps now uses these encoders in the shared source/native/stream pipeline.
The complete SampleMod Romania map passed a private source-to-Misc build with real
terrain resources. Its 79 authored assets became 776 assets after WorldBuilder
closure. Eight files (four sidecars, map.version, one raw map and two TGAs) entered
an independently verified Misc BIG. No in-game load has been tested.
User mod XML, Documents output and game archives were not modified.

## GameMap and scripts

`MapNativeCompiler` uses the existing Core reference normalization and source
marshalers, with a separate `Ep1GameMap` layout rather than modifying KW structs.
The EP1 root is 80 bytes. `Ep1EnvironmentObject` is 16 bytes and adds DepthLut.
MapAnalysis is an optional 28-byte record with five counters and a water-area list.
MapMetaData remains a pipeline-only child.

GameScriptList uses the existing native structs. Standalone normalization does not
assign polymorphic class tags: the encoder explicitly selects the two official
GameScriptParameterObjectType/ObjectTypeList classes. Recursive GameScriptGroup
lists use the full group stride, not the CLR recursion placeholder SelfType.
The selected native comparer now verifies these pointer arrays, tags and complete
recursive groups, including relocation ownership and import identities.

Private comparisons to actual unmodified retail map sidecars passed:

| Source | GameMap BIN/RELO/IMP | Script lists |
| --- | --- | --- |
| CAMP_EP01_S01_Romania_Stewart | 311608 / 33888 / 12308 | 6 complete graphs |
| CAMP_EP01_A01_IgaPrefecture_Davis | 445496 / 48512 / 17088 | 8 complete graphs |

GameMap fingerprint: D51AE5E1 / 442681EA, untokenized.
GameScriptList fingerprint: DEBF8788 / A268995A, untokenized.
These results compare fresh source-derived native graphs against retail fields;
they do not merely compare lengths or copy native stock records.

## Terrain atlas prototype

`TerrainAtlasCompiler` models the original 28-byte root: base, tile count/pointer,
base DDS pointer/length, normal DDS pointer/length. Tiles are 12 bytes: TextureID
and four unsigned 16-bit rectangle coordinates. The original type fingerprint is
5F74536D / 18E913EA, untokenized and without asset imports.

The prototype packs complete source tiles in source order, adds 16-pixel wrapping
borders and writes five mip levels. Keeping full tiles retains every declared Cell.
It currently generates lossless BGRA color DDS and the X1R5G5B5 normal format seen
in retail. The DDS reader now recognizes that exact normal format.

The 36 Romania and 40 Iga rectangles match the original native tables with synthetic
rasters using the witnessed tile dimensions. Generated DDS mip lengths/formats are
independently read back. Synthetic rasters remain diagnostic inputs only. Production
uses captured real terrain resources and independently checks DDS headers, lengths,
formats, mip levels, tile IDs/dimensions, non-overlap and relocation ownership.
The lossless color output is larger than original compressed DXT5 output; runtime
loading and visual equivalence still need a game test.

## Current source dependency finding

The SampleMod Romania map requests 72 terrain texture uses (36 base/normal pairs).
The user's `alpha/Art/Terrain` additions resolve all 72. They are all 256x256;
35 are 24-bit TGA and 37 are 32-bit. Raw and RLE 24/32-bit images are decoded into
BGRA; RGB pixels receive opaque alpha. Ten small orientation/RLE/alpha controls pass.
The current inventory is `alpha/.work/map-romania-resources-02/terrain-resources.json`.

Resources follow mod-over-SDK search precedence and are frozen by content hash
beside prepared source XML. Terrain files join source receipt hashes for final
publication checks. Missing files produce MISSINGTERRAIN warnings at the exact
Tile attribute. The optional map is skipped; no partial/placeholder atlas is
published and previous Misc content remains. Invalid existing files still fail.

## References, overrides and linking

The standard stock reference Includes are recorded as catalog boundaries, without
reading their source XML or exposing their definitions/inheritance handles. Other
reference paths are still unsupported. All/instance Includes keep their ordinary
source roles; campaign overrides are compiled rather than silently discarded.

PlayerTechStoreTemplate support was required by CampaignOverrides_EP1. Its native
root is 16 bytes, with five 8-byte row headers and fifteen ordered button imports
(116 bytes for an official store). Three original faction stores pass native
comparison; XSD and current-source native validation remain active in fast builds.

Authored map types have independently witnessed EP1 fingerprints even though
these types are absent from static/global catalogs. The existing linker writes
version-7 streams, then the WorldBuilder resolver adds referenced native closure.
Staging preserves `map_mod.*` and `map.version=_mod` before Misc publication.

## Validation evidence and remaining work

- Full private map build: `alpha/.work/map-official-e2e-01/build.log` and `detail.log`.
- Staged map: `alpha/.work/map-official-e2e-01/Misc/Data/Maps/Official/CAMP_EP01_S01_Romania_Stewart`.
- Final private archive: `alpha/.work/map-official-e2e-01/Maps_Misc.big`.
- Missing-terrain controls: `alpha/.work/map-missing-terrain-01/test.log`.
- Stock reference boundary control: `alpha/.work/map-reference-boundary-01`.

Remaining: in-game map loading/visual validation, broader map/source corpus tests,
and passing the newly built static mod's catalog into map lookup for assets that
exist only in that mod. Current map lookup uses stock catalogs, WorldBuilder and
any supplied built global context. The verified Romania map resolves in that scope.
Do not claim support for binary .map editing or arbitrary missing custom providers.

Developer diagnostics remain separate from Studio's normal build entry point.
