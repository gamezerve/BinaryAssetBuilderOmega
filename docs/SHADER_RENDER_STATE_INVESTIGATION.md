# KW shader rendering failures in Uprising

<!-- Reborn: preserve the independently reproduced render-state defect and distinguish it from unproved model-visibility causes. -->
Investigated on 2026-11-02 after the user reported red surrounding models and
an invisible Titan body. No user mod, deployed package or game installation was
changed. Tests use a private hidden Direct3D9 device, not the game.

## Confirmed shared tint register corruption

All four FXOs currently in `alpha/mods/SampleMod/Misc/Shaders/Compiled` match
the generated loader candidates byte for byte. The loader candidates are
**known to corrupt Uprising's shared render state**, despite passing effect
creation. They are not ready for gameplay use.

Uprising's original `RegisterMap.fxh` reserves `c2` for `TintColor`.
Its `Common.fxh` explicitly requires reserving global constants even when a
shader does not use them, to prevent other constants from overwriting them.
The candidate preparation script only bound matching existing KW declarations;
it did not reserve the complete Uprising global register map.

Legacy FXC assembly for ObjectsGDI confirms the collision:

- Vertex shader: `Shroud` uses `c2`.
- Pixel shader: `EnvMult` uses `c2`.
- Uprising expects engine-managed `TintColor` in that same register.

The native probe seeds both vertex and pixel `c2` with
`(0.21, 0.43, 0.65, 0.87)`, creates the effect using Uprising's exact recovered
skip-constants list, and executes technique 0/pass 0. It does not draw geometry.
Every native effect/pass operation succeeded. The flags disable state saving;
the measurement is made inside the pass, before EndPass/End.

| FXO | Vertex c2 after BeginPass | Pixel c2 after BeginPass |
| --- | --- | --- |
| Candidate ObjectsGDI | `(1, 1, 0, 0)` | `(1, 0, 0, 0)` |
| Candidate ObjectsNOD | `(1, 1, 0, 0)` | `(1, 0, 0, 0)` |
| Candidate ObjectsAlienIonHull | `(1, 1, 0, 0)` | `(0, 0, 0, 0)` |
| Candidate ObjectsGenericEnvMap | `(1, 1, 1, 1)` | `(1, 1, 1, 1)` |
| Original Uprising DefaultW3D | seed preserved | seed preserved |
| Private ObjectsGDI tint-reservation control | seed preserved | seed preserved |

The private control adds the original-style shared, unmanaged `TintColor`
declaration at c2 to a separate source copy. This removes the demonstrated
collision in both stages while keeping effect loading successful. This is a
focused regression control, **not a fully migrated shader**. It does not prove
that every other reserved register or technique is correct.

The red `(1, 0, 0)` pixel-register overwrite directly explains how producing
one custom unit can affect otherwise unrelated stock models. This is a shader
port defect; adding textures or changing W3X asset IDs cannot correct it.

## Titan's invisible body: a separate unresolved rendering path

Update: the user subsequently replaced the KW shader names with suitable stock
Uprising shaders and confirmed the full object renders normally. The shader
dependency is therefore established; further Titan visibility debugging is not
needed for that stock-shader configuration. The historical evaluator analysis
below describes the unfinished KW shader port only.

`alpha/Art/GU/GUVEHICLEF_SKN.GUVEHICLEF.w3x` declares a `GeometryType="Skin"`
body with bone influences and `objectsgdi.fx`. The separate
`GUVEHICLEF_SKN.UGRAIL_01.w3x` weapon is `GeometryType="Normal"` and uses
`ObjectsNOD.fx`. The body and weapon therefore exercise different transform
paths; visibility of the weapon does not validate skinned-body rendering.

The candidate still uses KW's `Objects_P0`/`Objects_P1` and other legacy
expression evaluator names. Those names are absent from the inspected Uprising
1.1 executable; the supplied Uprising source uses `Objects`/`Buildings`.
KW Common defaults `_ArrayIndexVS` and `_ArrayIndexPS` to zero. Its first VS
array uses index 0 for the unskinned variant and index 1 for the skinned variant.
This makes evaluator/variant selection a concrete suspect for the body-only
failure. It has not been observed inside the running game, so it is not yet a
confirmed root cause. Blindly renaming every evaluator is not a complete fix:
the shadow, normal, flat-color and multipass arrays have different contracts.

## JapanRadarShipReborn is not using these KW shaders

`alpha/mods/Reborn/Art/JU/JURADARSHIP_RBRN.w3x` selects the stock
`ObjectsJapan.fx`, technique index 2, and a rigid Normal mesh. It is not the
same shader/geometry path as the Titan body. Previous source/native comparison
passed 40,381 checks, but that establishes serialization consistency rather
than runtime visibility. The custom source also changes the original model's
vertex translation, attachment transform and technique index.

See [Japan Radar Ship visibility](JAPAN_RADAR_SHIP_VISIBILITY.md). A shared W3X
compiler bug has not been proved or excluded. A same-GameObject comparison
between the original EA container graph and the custom graph remains the
discriminating test, followed by isolating technique and transform changes.

## Correct migration work

1. Reserve the complete original Uprising global map, including unused float,
   integer and Boolean globals, using its unmanaged/indirect-constant contract.
   Verify material passes preserve engine-owned values, not merely effect load.
2. Migrate each KW technique's evaluator and VS/PS array contract to Uprising,
   including rigid/skinned selection and auxiliary passes. Preserve KW-specific
   material features explicitly rather than silently dropping them.
3. Validate a skinned body and rigid weapon together, then recoloring, opacity,
   lighting, shadows and unrelated stock models in-game.
4. Investigate the stock-shader Radar Ship independently; do not apply a KW
   shader replacement or speculative W3X matrix fix to it.

Private evidence under `alpha/.work`:

- `Shader-gdi-assembly.txt`
- `ShaderRegisterStateProbe-20261102.ps1`
- `ShaderRegisterStateProbe-20261102.log`
- `ShaderRegisterStateProbe-tint-control-20261102.log`
- `ShaderTintReservationTrial/` (private source control only)

No complete rendering fix or in-game verification is claimed by this report.
