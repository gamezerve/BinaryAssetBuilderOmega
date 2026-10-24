# W3DContainer authoring

<!-- Reborn: document actual W3X container support without implying that other W3X processors are enabled. -->
Use Studio normally. `Include type="all" source="ART:filename.w3x"` reads the
W3X's EA AssetDeclaration and emits its W3DContainer declarations with the mod.
Mod Art and SDK Art lookup follow the existing alias/fanout rules. Direct XML
declarations and container-only mods use the same build path.

The official Uprising XSD controls Hierarchy, SubObject, BoneIndex,
SubObjectID, and the Mesh/CollisionBox choice under RenderObject. Empty lists,
optional hierarchy and default SubObjectID are supported. Custom IDs and local
container references are resolved with normal mod-before-stock precedence.
Every referenced hierarchy, mesh and collision box must exist in the mod or
stock catalog. WorldBuilder metadata supplies external render/hierarchy
identities; their payloads are not copied into the mod. Catalog availability
does not establish that a particular map loads that stream at runtime.

The existing XML marshaler writes native BIN/RELO/IMP. Three original EP1
container graphs qualify the 16-byte root and 16-byte subobject layout, strings
and typed imports. Authored output has independent field/auxiliary checks and
the normal stream/BIG readback. W3X files are hash-tracked source dependencies,
including their declaring path in diagnostics and compile progress.

W3DMesh, W3DHierarchy and W3DCollisionBox now have their own
[compiler integration](RA3EP1_W3D_GEOMETRY.md). W3DAnimation remains a separate
processor and still produces an unsupported-type error. Referencing an existing
asset does not require compiling it. Audio processing and texture formats beyond
the existing TGA profile are unchanged.

Developer regression: `scripts/Test-Ra3Ep1W3DContainer.ps1` uses an isolated
output directory and original static/global manifests to exercise shared W3X
includes, authored/empty containers, WorldBuilder dependencies, native/stream/BIG
readback, and missing mesh diagnostics with W3X file/line information. It does
not change Reborn sources, Documents packages, or the game installation.

<!-- Reborn: record the completed private qualification without treating offline package readback as gameplay proof. -->
Validation on October 24, 2026: the integration test passed, including an
inherited source owner with an all-imported W3X and an instance-only base.
The user's VL_RUSSIANPORTBUILDING_CTR.w3x compiled independently with all five
external references. A private build of the current Reborn source graph also
passed: 28 source files, 11 emitted assets, including two GameObjects, one
W3DContainer and two Texture/PackedTextureImage pairs. Its native/stream/BIG
readback passed; no Documents package was replaced and no game was launched.
