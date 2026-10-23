# Scripted draw shader materials

<!-- Reborn: document the original material ABI and separate shader binding authoring from shader/resource compilation. -->
`ScriptedModelDraw/ModelConditionState/Material` now has a source-owned native
and metadata path. It uses the existing native marshaler rather than copying
material bytes from a stock building.

Supported schema fields are `ShaderName`, `TechniqueName`, `TechniqueIndex`
and optional `Constants`. Constants may contain `Texture`, `Float`, `Int` and
`Bool` entries in authored order, with arbitrary names. Float entries contain
one to four `Value` elements, as required by the official XSD. Texture values
resolve through the shared strong-reference catalog. Omitted material, omitted
constants and an explicit empty constants list are supported. Synthetic
`TypeId` attributes are compiler internals, not required author XML.

## Implementation

- `Ep1ScriptedDrawTokenizer.cs`: optional material allocation, concrete class
  wrappers, constant base tables, float vector metadata and pointer rebasing.
- `GameObjectBodyProjectionSmokeTest.cs`: nested shader discriminator dispatch
  on the selected complete-GameObject compilation path.
- `SchemaNativeContract.cs`: independent source/native values, concrete types,
  allocations, relocations and import occurrence identities.
- `StockMetadataGrammar.cs`: concrete polymorphic metadata grammar learned
  from real stock records, not from the metadata producer.
- `ShaderMaterialContract.cs`: original SovietBarracks/JapanWarFactory material
  witnesses, including all four concrete constant classes.

The material record is 28 bytes; its constant pointer array owns separately
allocated concrete records. Texture, Int and Bool constants occupy 16 bytes;
Float constants occupy 20 bytes plus their vector allocation. All concrete
records share the 12-byte FXShaderConstant base. Metadata row tags and class
wrappers are checked against original stock tables.

This does not compile new `.fx` shader programs, textures or models, and does
not implement unrelated draw children such as FXEvent, ShadowInfo or Turret.
Build validation is not runtime/rendering evidence. Private regression and
Reborn validation outputs remain under `alpha/.work`; no user mod source or
Documents output is rewritten by these checks.
