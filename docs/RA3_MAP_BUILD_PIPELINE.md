# RA3 map compilation pipeline

<!-- Reborn: preserve read-only findings from the actual SDK scripts, authored map XML and a previously built override package. -->
Inspected on 2026-11-03. No map was compiled, no batch file was executed,
and no SDK source, mod source or installed game file was changed.

## Which SDK workflow was inspected

The supplied SDK includes Bibber's extended build scripts in addition to EA's
compiler tools. It must not be treated as an untouched official EA workflow.

- `Working RA3 Compiler for Reference/BuildMap.bat` is byte-identical to
  `D:/OneDrive/CNC Files/RA3 MOD SDK/BuildMap.bat`. It compiles a single map
  from `%APPDATA%/<UserDataLeafName>/Maps/<map-name>`.
- The repository reference `defaultscript.cs` has an eleventh mod build step:
  **Build Official Maps**, immediately before BIG creation. It calls
  `BuildMapV2.bat <mod-name>`.
- That V2 script enumerates direct subdirectories of
  `Mods/<mod>/Data/Maps/Official` and stages their compiled results in
  `Mods/<mod>/Misc/Data/Maps/Official/<map-name>`.
- The external SDK's current `defaultscript.cs` has ten steps and no
  **Build Official Maps** option/call. Its ninth step creates BIG files.
  Merely placing maps in that SDK's Data tree does not activate the repository
  reference's custom map-build stage.

`Documentation/Bibber/CompilingMaps.htm` explicitly identifies `map.xml` as the
compiled input and `overrides.xml` as the intended place for custom assets and
modifications.

## Inputs: a map is more than its .map file

The representative actual map was
`Mods/Reborn/Data/Maps/Official/CAMP_J07_Yokohama_Stewart` in the external SDK.

| Input | Role observed in this workflow |
| --- | --- |
| `<map-name>.map` | Existing binary map file; the batch copies it to the final map folder. It is not the BAB command-line input. |
| `map.xml` | WorldBuilder/exported asset graph; the actual BAB compilation entry point. |
| `overrides.xml` | All-included local modifications and additional assets. |
| AI XML files | Map-local player/state-machine data included by the exported graph. |
| `.tga` files | Existing map preview/art images explicitly copied to the map folder. |
| Map-local Art/Audio/Data | Source lookup roots supplied to BAB. |
| Optional `Assets` directory | Extracted `.asset`/CDATA data, merged only when `/ma` is enabled. |

The inspected `map.xml` contains three reference Includes (`static.xml`,
`global.xml`, `audio.xml`), 107 instance Includes, and one all-Include for
`overrides.xml`. Its directly declared asset families are:

- One `GameMap`: 164 waypoints, 171 structures, 42 units, 2,555 props,
  87 audio objects, six unknown objects and 15 roads, plus metadata,
  environment data, world dictionary, post-effects and map analysis.
- One `TerrainTextureAtlas`, with an atlas size of 2048 and terrain
  diffuse/normal source textures under `ART:Terrain`.
- Ten `GameScriptList` assets containing map/player script sets and actions.

Thus normal GameObject/Texture/W3X support is useful but does not itself implement
map compilation. GameScriptList data is distinct from the mod's global Lua
`Misc/Data/Scripts/Scripts.lua`.

## Batch stages

1. **Find the SDK and map.** The old scripts query RA3/SDK registry locations
   and otherwise use their own directory. BuildMap obtains UserDataLeafName
   for the AppData map path; V2 uses the chosen mod's Official map folders.
2. **Prepare shared scratch/cache locations.** Both copy input to
   `SDK/Mods/Maps/<map-name>` and use `SDK/BuiltMods` for generated streams.
   Cache-clearing switches can delete shared cache/session files.
3. **Compile map.xml.** Both invoke `Tools/BinaryAssetBuilder.exe` on
   `Mods/Maps/<map-name>/map.xml`, with output/intermediate roots in BuiltMods.
   ART/AUDIO/DATA lookup lists prefer the map-specific folders, then SDK Mods,
   then Art/Audio/SageXml fallbacks.
4. **Optionally merge extracted assets.** `/ma` invokes AssetMerger against the
   compiled map manifest and its Assets directory. This flag defaults false.
5. **Fix the generated map stream.** Unless `/nf` is selected, the scripts call
   HashFix, temporarily place the original WorldBuilder manifest beside the map,
   call MapAssetResolver, remove that temporary manifest, then call ReferenceFix.
   The final command adds `worldbuilder.11.manifest` with marker 2 and removes
   the plain `worldbuilder.manifest` reference. The included documentation
   explains that the correction prevents invisible assets depending on
   WorldBuilder data. Full resolver internals were not reverse engineered here.
6. **Publish map outputs.** The original BuildMap copies generated data back
   into the AppData map directory, excluding `.asset` intermediates. V2 instead
   prepares the mod's Misc map folder, applies its `_mod` naming convention and
   explicitly copies the source `.map` and `.tga` files.
7. **Package the mod.** The reference Studio script runs MakeBig on the mod's
   BuiltMods output for Streams BIG and on Misc for Misc BIG, excluding `.asset`.
   Its SKUDef references both BIGs when they exist. V2's map data therefore
   enters **Misc BIG**, although those files internally contain asset streams.

Relevant BAB switches are confirmed by the available builder Settings source:

| Switch | Meaning |
| --- | --- |
| `/ls:true` | Linked streams |
| `/pc:true` | Use an available referenced manifest instead of recompiling that stream |
| `/ss:true` | Stable asset sort; not “single stream” |
| `/osh:false` | Disable string-hash output |
| `/slowclean:true` | Force slow asset/CDATA cleanup |
| `/od`, `/iod` | Generated output and intermediate output roots |

The `.map` file is copied unchanged by these batch stages. Its map asset graph,
terrain atlas, script data and referenced assets are what BAB compiles from XML.
The batch alone does not prove how every raw .map chunk is used by the engine.

## Verified _mod output convention

V2 renames emitted `map.bin/imp/manifest/relo` to
`map_mod.bin/imp/manifest/relo`, then writes `_mod` to **map.version**.
An existing emitted map.version is first renamed map_mod.version, if present;
the selector remains the unsuffixed map.version.

This is independently present in the user's real
`Documents/Red Alert 3/Mods/deneme/Deneme_1.00_Misc.big`:

- Map folder: `data/maps/official/camp_a01_brightonbeach_smith`.
- The original .map, preview and art .tga files are present.
- `map.version` contains exactly `_mod` followed by CRLF.
- `map_mod.bin`, `.imp`, `.manifest` and `.relo` are present.
- The map_mod manifest is version 6 with 1,719 assets.
- Its referenced-manifest table contains marker 2 for
  `worldbuilder.11.manifest`, and marker 1 for `static.manifest`,
  `global.manifest` and `audio.manifest`.

The scripts and built package agree on the selector convention. Engine-side
loader behavior was not newly traced in this inspection. RA3's exact WorldBuilder
filename/patch marker must not be copied blindly into an Uprising implementation.

## Map metadata and early global data are separate

The reference Studio's **Build Global Data** step separately compiles
`Data/AdditionalMaps/MapMetaData_*.xml`; this is a different file pattern and
stage from `Data/Maps/Official/*/map.xml`.

The exported GameMap's nested MapMetaData includes the map file path, dimensions,
player count, official/multiplayer flags and starting positions. The GameMap XSD
marks this nested element `pipelineOnly=true`; it is not an ordinary runtime
GameMap field to serialize blindly.

Neither batch explicitly invokes a map-catalog registration step. Overriding an
existing official map can retain its existing catalog path. Adding an entirely
new official/campaign map may need separate metadata/campaign registration;
compiling its map.xml alone must not be advertised as completing that work.

## Implications for Uprising SDK

The current global-source build is not a replacement for this map pipeline.
The next implementation needs:

1. Normal map XML Include/instance/reference processing with mod-supplied
   overrides and the appropriate stock plus current mod stream providers.
2. Reviewed EP1 `GameMap`, `TerrainTextureAtlas` and `GameScriptList` native
   layouts/processors, including their generated resources and dependencies.
3. A map-specific WorldBuilder dependency/reference resolution step, reusing
   current selected-asset closure where appropriate but verifying the EP1
   map stream/provider contract separately.
4. A private per-build/per-map workspace under the SDK's `.work`, rather than
   the legacy shared Mods/Maps folder or deleting files in authored map sources.
5. Staged map files, version selector and compiled sidecars packaged together
   under the intended Misc BIG map path. Preserve the relative `.map` path when
   overriding an existing official map.
6. A **Build Maps** toggle whose skipped/missing-input behavior follows the
   current global-build convention, with final publication through the existing
   rollback/receipt path.
7. A first Uprising map comparison using an unchanged original exported map and
   its original compiled stream, followed by one controlled override/game test.

The legacy scripts also delete existing map outputs before success and V2 ends
with a successful overall exit even when an individual map only printed
“Map not compiled.” Those behaviors should not be ported into the current SDK.
The V2 source explicitly copies .map/.tga files; other loose map files need an
explicit packaging policy rather than an assumed wildcard copy of the source.

## Evidence locations

- Repository reference: `Working RA3 Compiler for Reference/defaultscript.cs`,
  `BuildMap.bat`, `BuildMapV2.bat` and
  `Documentation/Bibber/CompilingMaps.htm`.
- External SDK: `D:/OneDrive/CNC Files/RA3 MOD SDK/defaultscript.cs`,
  `BuildMap.bat` and the representative Reborn map directory above.
- Type declarations: `schemas/ra3ep1/xsd/AssetTypeGameMap.xsd`.
- BAB option meanings: `source/BinaryAssetBuilder.Core/BinaryAssetBuilder/Settings.cs`.
- Private bounded package audit: `alpha/.work/ra3-map-package-audit.json`.

The investigation itself did not modify map sources.

## Implementation follow-up

<!-- Reborn: distinguish implemented routing and packaging from outstanding native source compilation. -->
Studio now has a persisted **Build Maps** checkbox, unchecked by default. Step 9
enumerates direct Official map folders and requires `map.xml` plus a `.map`.
Missing folders/entries are optional skips. All maps must compile in private work
directories before any generated files are copied into the mod's Misc tree.
Existing unrelated Misc files are preserved. Validation-only builds do not copy
map outputs into Misc.

The staging command requires a valid linked EP1 graph containing exactly one
GameMap. It verifies sidecar sizes, kind markers and stream checksums, writes
`map_mod.bin/imp/manifest/relo` and the unsuffixed `map.version` containing
`_mod\r\n`, and retains stream CDATA. `.map` and `.tga` are copied unchanged.
Misc packaging then includes those files under `Data/Maps/Official/<map>`.

<!-- Reborn: the scaffold now has real authored native compilation and a private full map package test. -->
GameMap, TerrainTextureAtlas and GameScriptList are implemented in the shared
source/native/link route. Source capture treats the standard static/global/audio
reference Includes as catalog boundaries. Real terrain TGAs and campaign overrides
compile, then the WorldBuilder resolver supplies missing native dependencies.

The complete SampleMod Romania map passed privately: 79 authored assets, 776 linked
assets after resolution, eight output files and verified Misc BIG packaging.
Missing terrain warnings skip only the affected map and preserve existing Misc.
No in-game map test has been performed. See
[native implementation and remaining work](MAP_NATIVE_IMPLEMENTATION.md).
