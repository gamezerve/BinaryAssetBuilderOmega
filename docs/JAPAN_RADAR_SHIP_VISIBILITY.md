<!-- Reborn: distinguish verified source-to-binary model data from unverified engine visibility. -->
# JapanRadarShipReborn visibility investigation

Investigated on 2026-11-02. The original audit below was read-only. The follow-up
identified and fixed a native mesh serialization defect; its effect on in-game
visibility still requires confirmation after a mod rebuild. No user mod source
or installed package was changed.

## Follow-up: shader equivalence and missing declaration terminator

The user confirmed Titan becomes visible when its KW shader names are replaced
with appropriate stock Uprising shaders. This establishes a shader dependency
for Titan's visibility; it does not establish the same cause for this ship.

The actual custom model from the user's RA3 SDK,
`Art/Debug/JURadarShip_Deneme/JURADARSHIP_RBRN.w3x`, and the alpha Reborn model
contain the same 21,340 ordered XML elements, attributes and text values.
Whitespace differs. Both select `ObjectsJapan.fx`, technique 2.

`ObjectsJapan.fx` entry sources in RA3 and Uprising are byte-identical. More
decisively, the shipped `objectsjapan.fxo` files extracted from each game's
`Data/Shaders.big` produce **byte-identical complete D3DX effect disassemblies**.
Their raw FXO hashes differ (97 bytes), so raw binary identity is not claimed.
Both native effects expose the same four techniques: Default, Default_M,
Default_L, _CreateShadowMap. Index 2 is valid Default_L. Replacing ObjectsJapan
with ObjectsAllied is especially unhelpful: those two Uprising FXOs are already
byte-identical in the inspected installation.

Selected chunks were read from the user's real EA-generated
`Red Alert 3/Mods/Reborn/Reborn_1.07_Streams.big` and the currently published
Uprising `Reborn_1.00_Streams.big`. These reads were bounded to the selected
assets, not full stream dumps. The hierarchy and container native BIN/RELO/IMP
are identical. Mesh geometry, material values, declaration text and typed texture
references also match; the concrete difference is at the declaration's end:

- EA allocates a NUL after the 76 counted ASCII declaration bytes. Its mesh
  chunk has 173,824 bytes, including alignment padding.
- Our previous writer allocated only 76 bytes, placing the descriptor at the
  very end of the 173,820-byte mesh chunk without a terminator.
- `BuildVertexDeclaration_Ep1` now allocates count + 1, explicitly writes the
  NUL, and retains the original count. W3X processor version increases to 9.
- The native verifier now requires that out-of-count terminator.

The regression compiles the user's source read-only through the actual schema,
reference normalization and W3X processor, into a private directory. Before the
fix it fails for a missing terminator. After the fix, the **entire resulting mesh
BIN, RELO and IMP match the EA-generated witness byte for byte**. An independent
original EP1 `JURADARSHIP_COL.NEWSKIN` check also finds a terminator immediately
after its 108 counted declaration bytes. The missing-texture regression passes
all 12 controls after the fix.

This is a confirmed compiler defect and a strong visibility suspect, not an
in-game proof of the final root cause. Rebuild Reborn with the synchronized alpha
tools and test in a new game. If the ship remains invisible, investigate the
GameObject draw/model-condition/stream binding; do not change this already
equivalent shader or speculate about geometry transforms first.

Private follow-up evidence: `alpha/.work/RadarShaderAudit-20261102/`, including
retail effect disassemblies, `ra3-model-comparison.json`, selected chunk metadata,
and the `before-fix/` and `after-fix/` regression outputs. The reusable test is
`w3d-vertex-declaration-self-test` with source, stock manifest, EA mesh witness
and a fresh private output directory.

The subsequent [KW shader render-state investigation](SHADER_RENDER_STATE_INVESTIGATION.md)
reproduces shared tint corruption in SampleMod's experimental KW shaders.
This ship instead uses the stock ObjectsJapan shader and a rigid mesh; a common
cause for its invisibility and the Titan's skinned-body failure is not established.

## Verified in the currently deployed Streams BIG

- `W3DContainer:JURADARSHIP_RBRN` exists and imports its custom hierarchy,
  collision box and mesh. The mesh has three texture imports; their identities
  were previously located in the stock static catalog.
- `W3DMesh:JURADARSHIP_RBRN.JURADARSHIP_RBRN` has 1,764 vertices,
  1,484 triangles and `Hidden=false`.
- An independent source/native audit passed 40,381 scalar/index checks for
  positions, normals, tangents, binormals, both UV channels, default white vertex
  colors, triangle indices/planes and shader values. Source-selected technique
  index 2 is preserved. This checks serialization, not rendering in the engine.
- The previous captured GameObject has one draw module and Scale=1. Its source
  selects the custom container in every model condition state.
- The current hierarchy differs from the previous captured package only in the
  reverted M33 values (1 to 0). Retail EA matrices also contain M33=0; setting
  these values to 1 again is not an evidence-based fix.

Private evidence: `alpha/.work/radar-model-audit-current-path.txt` points to the
audit's `report.json`. These files are local developer evidence, not SDK inputs.

## Actual custom-export differences

Compared with `JURADARSHIP_SKN.JURADARSHIP.w3x`, the custom mesh preserves the
vertex/triangle counts but translates vertices approximately
`(+4.8954, +0.0254, -16.9103)`. Local Z bounds change from approximately
`[-20.0573, 20.0573]` to `[-36.9676, 3.14694]`.

Its attachment pivot also changes rotation, translation and fixup scale. The
original pivot 1 has identity rotation and fixup scale 1.25; the custom pivot
has approximately a -90 degree Z rotation and fixup scale 1. Shader selection
changes from index 0 to index 2, adds SpecularColor, drops the explicit white
VertexColors array, and adds a second UV channel. The compiler supplies white
vertex colors correctly. In the official Uprising Objects.fxh source, technique
index 2 corresponds to Default_L; this is not by itself proof of an invalid shader.

These are relevant export/rendering differences, not proof that the ship is
submerged or that any one difference causes invisibility. The original model's
successful stock roundtrip does not prove this custom export renders correctly.

## Remaining discriminating test

Use the same GameObject with the original EA container/hierarchy/mesh graph,
then compare with the custom export. Do not change gameplay, textures or several
compiler paths simultaneously. If the original graph renders, isolate custom
geometry/attachment transforms and shader selection separately. If it does not,
inspect the GameObject's draw imports and runtime model loading before editing
the W3X exporter or inventing another matrix fix. No in-game visibility result
for that comparison is claimed here.
