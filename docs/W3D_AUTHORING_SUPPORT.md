<!-- Reborn: document actual W3X compiler coverage and distinguish processor tests from playable imported content. -->
# W3D authoring support

`W3DContainer`, `W3DHierarchy`, `W3DMesh` and `W3DAnimation` participate in the
normal alpha source/include, inheritance, reference and stream build paths.
`W3DCollisionBox` remains supported alongside them. Use the usual `ART:` includes
from `Static_Mod.xml` or an included art declaration file.

- Containers retain their authored subobjects, bone indices and typed mesh or
  collision imports. Hierarchies retain pivot names, parent relationships,
  translations, quaternions and fixup matrices in the EP1 layout.
- Meshes support the official geometry enums, position/normal streams, vertex
  colors, tangent/binormal pairs, multiple UVs, skin indices/weights, triangles,
  shader constants and optional AABTree data. Optional AABTree allocation now
  targets the actual owning pointer fields; its polygon index lists are also
  covered by the developer source/native proof.
- Animations support X/Y/Z translation, orientation and visibility channels,
  `FirstFrame`, `BinaryMove`, frame count/rate and hierarchy references. The
  writer emits EP1 runtime channels, not the source-only channel structures.
  Both adaptive delta and timecoded records are exercised. Quaternion timecoded
  values allocate four floats per key, fixing the legacy writer's undersized
  allocation. XML boolean forms `1` and `0` work as well as `true` and `false`.

Compression settings are parsed using the existing processor. Error-bounded
adaptive delta is used when eligible. Timecoded fallback deliberately retains
all source samples; `ForceReductionRate` does not force lossy key removal on
this EP1 path. BinaryMove channels use timecodes so step boundaries survive.
An absent hierarchy stays null. Clips longer than the 15-bit timecode range may
use adaptive delta; a clip requiring timecoded fallback beyond 32,768 frames
reports the encoding limitation rather than wrapping its timestamps.

There is no small allowlist of specific model or animation IDs. XSD failures,
invalid sample/triangle ranges, missing dependencies and native encoding
constraints still produce diagnostics. A successful compile does not guarantee
that shaders/textures imported from another game are available in Uprising or
that an exported model's transforms are visually correct.

## Verification

- `w3d-animation-self-test <EP1 manifest>`: 127 real-dispatch, deterministic
  output, partial-span, BinaryMove, optional/default and corruption checks;
  original scalar/quaternion timecoded/delta records establish the runtime ABI.
- Coordinate proof: 18 checks; relocation proof: 7 checks.
- Original model comparison: 5 stock models, 5,481 vertices and 4,900 triangles.
- The SampleMod art processor fixture builds 19 animations, 18 meshes,
  4 containers, 2 hierarchies and 3 collision boxes. BIN/RELO/IMP/manifest output
  is compared between developer and fast modes.

The private fixture supplies two explicitly labelled texture stand-ins to
exercise all processors. These are not installed in SampleMod or published to
Documents. Those stand-ins belonged to the initial processor-isolation test.
The subsequent missing-texture policy below builds the real art graph without
stand-ins. During the original full-source review, KW-style `GameObject/@BuildCost`
was also found in `SteelTalonsTitan.xml`; that is a separate XML migration issue,
not an Uprising XSD attribute. Imported GDI/NOD/Alien shader dependencies require
their own compatibility review.

38 include spelling corrections were applied to SampleMod's `ArtAssets.xml`.
Missing ART includes remain warnings as requested; a missing included file is
not evidence that a W3D processor discarded it.


## Missing optional shader textures

<!-- Reborn: source diagnostics and material import normalization use one optional-binding policy. -->
A missing `W3DMesh/FXShader/Constants/Texture/Value` target is reported as
`Warning: MISSINGASSET`, including its original source, line, owner and field.
The compiler removes only that optional texture constant from its private
normalization copy **before** Core assigns import indices. The shader parameter
keeps its default behavior. Other constants, valid texture bindings, mesh data
and asset identities remain intact. No placeholder texture, null texture asset
or dangling import is generated; authored W3X files are not rewritten.

The same lookup considers emitted mod assets and the configured stock catalogs,
including WorldBuilder. A texture that becomes available is retained normally.
Ambiguous or wrongly typed references remain errors. Required texture consumers
outside the optional shader binding, malformed XML and missing resource files
of explicitly authored textures are not downgraded by this policy. Missing
bindings may affect appearance according to the selected shader's defaults.

The current SampleMod art audit finds six such uses of `hc_gumammug` and
`hc_guvehiclef`. All six retain their source coordinates as warnings. The real
74-asset art group compiles and passes stream readback without the former test
stand-ins. Six emitted material records independently match exactly their
retained source constant names/order. Twelve focused warning/error, typed,
ambiguous, retained-material and native import controls also pass.
