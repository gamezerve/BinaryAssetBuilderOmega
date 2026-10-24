# W3D geometry authoring

<!-- Reborn: document the actual EP1 geometry ABI instead of treating a connected KW processor as runtime compatibility proof. -->
The stable alpha Studio now builds explicit W3DHierarchy, W3DCollisionBox and
W3DMesh declarations, in addition to W3DContainer. Use ordinary `ART:` W3X
includes or EA AssetDeclaration XML. Models can have custom IDs, including
the dotted names used by EA meshes and collision boxes. Local model references
take priority over the original catalogs. WorldBuilder textures, hierarchies
and other dependencies participate in typed lookup without copying their
payloads into the mod. Lookup availability does not prove runtime loading.

## Runtime differences handled

- EP1 hierarchy: 12-byte root, 100-byte pivot records. Name is a lowercase
  Poid hash; the old KW 8-byte name string layout is incompatible. Parent,
  translation, quaternion and all sixteen fixup matrix fields are written.
- Collision box: 32-byte record with uppercase W3DVector3 coordinates.
  Gameplay Vector3's lowercase components remain unchanged.
- Mesh: normal bounds/triangles/shader fields, vertex streams, declarations
  and bone palette are compiled. EP1 stores ASCII vertex declarations such as
  `p0:00:3f32 n0:0C:3f32 ... i0:44:4u8`; the earlier W3X processor emitted a
  binary D3D9 table. Multiple skin influences use a four-byte normalized
  `w0:...:4u8n` channel. Quantization distributes rounding remainder so byte
  weights sum to 255. The legacy declaration entry point remains separate.
- Shader constants retain actual Texture/Float/Int/Bool classes, values and
  Core-owned texture-reference occurrences. Referenced retail shaders/textures
  do not need recompilation. This does not add an FX shader compiler.

This path embeds PC model/vertex data in BIN, matching the reviewed stock
assets; no separate CDATA file is needed for these models. Native RELO/IMP,
local dependency sorting, stream serialization and BIG readback use the
existing pipeline. The rebuilt W3X processor DLL is included in alpha tool
synchronization and developer compiler snapshots.

## Source handling and validation

XML/W3X preparation now shares the path audit's 4 MiB per source, 32 MiB per
graph and 512-source budgets, replacing the earlier 1 MiB/32-source object
fixture limits. These are compiler resource budgets, not claimed EA engine
limits. Asset values and model IDs are not restricted to a stock sample.
W3X participates in source hashing, provenance, dependency auditing and
EA-style per-asset compile logs. Mismatched channel lengths and invalid
triangle indices fail before unsafe vertex conversion. PC skin indices have
four influences and a byte-indexed palette of at most 256 entries.

Developer validation is in `scripts/Test-Ra3Ep1W3DGeometry.ps1`. It copies six
selected Russian-port model sources into a private fixture, builds the complete
local model graph and compares its hierarchy/boxes/meshes with original EP1
WorldBuilder records: 5 geometry assets, 5,481 vertices and 4,900 triangles.
Names, counts, declaration bytes, packed values, indices, palette and texture
identities compare exactly; float comparison allows only rounded-source export
precision. It also exercises an edited custom mesh with two bone influences
and malformed-channel source diagnostics. No game or Documents output is
modified. W3DAnimation and other unimplemented asset processors remain separate.

<!-- Reborn: keep completed integration evidence separate from unperformed gameplay validation. -->
On October 24, 2026 the current Reborn snapshot also passed private compilation
and stream/BIG readback with 34 sources and 16 emitted assets, including the
complete six-asset model graph. The previous container integration and build-log
regressions passed. Source diagnostics cover model owners and nested shader
Texture constants; malformed mesh channels retain their actual XML/W3X field
coordinates. These are offline results; no game launch was performed.
