# Isolated EP1 ShaderOverride compiler profile

## Outcome and scope

`Ra3Ep1ShaderOverridePlugin` now exposes a bounded native compiler entry separate
from the legacy KW plugin. Four supplied source fixtures pass actual Settings
descriptor, PluginRegistry and document/schema/default/hash stages, then produce
the same BIN/RELO/IMP as the [native proof](RA3EP1_SHADER_OVERRIDE_NATIVE.md).
There is no asset-specific stock correction. PsychicCrush's literal source and
explicit stock variant remain different at the documented replacement word.

This does not register ShaderOverride in the production registry, compile shader
materials or establish an end-to-end SDK build/mod load. All three output/cache
policy flags remain false. Registry and document production calls reject before
accessing source/cache/output paths.

## Identity and eligibility

- ProfileName: `RA3EP1-ShaderOverride-Experimental-v1`; VersionNumber: 1.
- Observed TypeId `BCC23F6C`, TypeHash `3D5B1D16`, Tokenized=false.
- Local ProcessingHash: `0x3D5B1D16 ^ 0x45503131`, distinct from other profiles.
- Target aggregate: `5454A8E9`, not a claim of a complete registered EP1 table.
- Win32 only. Initialization/reinitialization on an unsupported platform revokes
  readiness; successful Win32 reinitialization recovers it.
- Each type-information call returns a detached object; unsupported types fail
  without KW fallback. Explicit descriptor UseBuildCache=true cannot override
  the plugin's false cache/document-reuse policies.

The admitted subset has one standalone ShaderOverride root and 1–16 schema-bound
ShaderOverrideRule children in source order. Priority is schema-validated as an
unsigned integer with default 1. Technique names are limited to `Default`.
Required replacement and optional condition materials must be nonempty ASCII
alphanumeric/underscore basenames ending in `.fx`, at most 128 characters total.
This is deliberately narrower than the official XSD; it is not a general material
name or technique implementation. Typed prefixes, dependency suffixes, paths,
whitespace, non-ASCII names and formulas are rejected rather than guessed.

The root must be schema-bound in the EA asset namespace and match the declaration's
instance identity. Only id/Priority and rule material/technique attributes are
admitted, plus exact core-injected TypeIds matching the bound schema type hashes.
Unknown/namespaced controls, inheritance, custom data and any strong/weak/file
dependency metadata reject. POIDs hash material names directly; they do not become
strong imports or document weak-reference entries.

After these checks, a detached copy removes the already-checked TypeId attributes
and revalidates current values using the document's schema set. Thus a stale
validation annotation cannot authorize an edited negative/overflowing Priority.
Marshalling does not modify normalized source XML. Native IMP must remain empty.

## Verification

`Ep1ShaderOverrideProfileSmokeTest` verifies:

- Four actual compiler entries match source-native BIN and RELO exactly, with no
  imports and unchanged normalized source XML.
- Actual document default insertion yields 40/12/0 for a single catch-all rule,
  priority 1 and null optional condition. A current-value priority of UINT_MAX
  survives without truncation; negative/overflow/formula values reject.
- A schema-valid PsychicCrush replacement edit changes only bytes 100–103, proving
  literal source compilation without a hidden stock-specific exception.
- Wrong root identity, wrong root/rule TypeId, inheritance attributes, alternate
  techniques, empty conditions, malformed material names, foreign/unknown
  attributes, nested controls, seventeen rules and unvalidated input reject.
- Unexpected strong/weak/file metadata and custom-data state reject.
- A deliberately poisoned resident declaration is replaced by unchanged source
  despite the caller requesting UsePrecompiled=true. Caller options are unchanged.
- Production calls reject before missing source/null cache access. Metadata is
  detached; unsupported types/platforms reject and Win32 reinitialization recovers.
- No production manifest is emitted. The harness writes only its own temporary
  input fixtures; original source and game data remain untouched.

Full Release/x86 inspector and builder builds pass with zero warnings/errors;
all 66 compiler groups and layout checks pass. The real static shader comparison
still gives three literal matches and one explicitly labeled stock-variant match.
Model inventory remains 785/1,390 and typed marshallers 762/1,390. Overall effort
stays approximately 50%; the major registry, packaging and runtime gates remain.

## Files and reproduction

- `source/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder/XmlCompiler/Ra3Ep1ShaderOverridePlugin.cs`:
  Initialize, GetExtendedTypeInformation, ProcessInstance and eligibility guards.
- `source/BinaryAssetBuilder.ManifestInspector/Ep1ShaderOverrideProfileSmokeTest.cs`:
  real descriptor/document/compiler and rejection/reload tests.
- `tests/fixtures/Ep1ShaderOverrideProfile.xml`: explicit diagnostic-only binding.
- `ShaderOverrideNativeSmokeTest.Compile`: the previously golden-checked native
  baseline now shared internally with the profile harness.

Run with the Release/x86 ManifestInspector executable:

```text
ep1-shader-profile-self-test
shader-override-self-test "D:\TEMP\Red Alert 3 Uprising Source Data\Static Data\data\static.manifest"
compiler-self-test
layout-self-test
```

The profile test does not itself require/read stock BIN data; equivalence to the
native baseline and the separate stock comparisons are distinct checks. Source
fingerprints and the PsychicCrush discrepancy are recorded in the native proof.

## Next work

The later [modifier/shader graph proof](RA3EP1_MODIFIER_SHADER_GRAPH.md) combines
both diagnostics in one real document and verifies local target identity through
normalization, resolution and native import selection. Production gates remain
closed. Next, test Includes and mixed local/external stream boundaries before a
narrow multi-family diagnostic writer; this is not an unrestricted SDK configuration.
Remaining stock shader variants, FXShaderMaterial custom-data compilation,
inheritance/expressions, complete EP1 type registration, cached native reuse,
SDK/WorldBuilder packaging and actual game loading still need separate proof.
