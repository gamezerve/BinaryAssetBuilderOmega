# EP1 modifier-to-shader local graph proof

## Outcome and boundary

Two AttributeModifier roots now reference and compile alongside two ShaderOverride
roots in one real document. The test combines the explicit modifier import
diagnostic (`Ra3Ep1AttributeModifierPlugin(true)`) and the isolated shader plugin
using PluginRegistry.AddPlugin. Both receive observed type metadata through the
normal document loader; no test-only TypeHash assignment or injected graph edge
is needed. The test exercises core normalization, real local reference resolution,
dependency closure and both native compiler entries.

This is an integrated document/dependency/compiler proof, not a full production
build/link/load. The private AddOutputInstance dependency stage is invoked through
the existing diagnostic seam. PrepareOutputInstances' later OutputManager/Commit,
sort/checksum and production linker stages are not called. All experimental
production/cache/session policies remain closed, including when two plugins are
explicitly mapped rather than used as the registry default.

## Fixture and reference semantics

The focused official schema graph admits both root families in arbitrary order.
The generated XML places two synthetic modifier roots before their shader targets,
therefore testing forward references. One Shader attribute uses an unqualified
name; the other uses an explicit `ShaderOverride:` prefix. The shader definitions
are literal IronCurtain and ObjectsWithoutXrayEffect from the stock-proven source
fixture, not new material processor stubs. The synthetic modifiers deliberately
omit FX references so no unported FXList processor is needed.

For each modifier:

1. Core schema/default/reference processing records exactly one strong shader
   dependency and normalizes the Shader attribute with suffix `\\0`.
2. Real ResolveReference/AddOutputInstance resolves that entry to the intended
   local declaration, queues both nodes and records a one-target closure.
3. The import-enabled modifier profile checks the normalized name/type/index
   against its ordered strong metadata and produces `60/8/8` BIN/RELO/IMP.
4. The Shader pointer at BIN offset 40 targets a four-byte record at 56. RELO is
   `[40, FFFFFFFF]`; IMP is `[56, FFFFFFFF]`. BIN word 56 is 1, meaning dependency
   index zero. Decoding that selector chooses the exact validated local shader.
5. The shader compiler produces its native baseline: IronCurtain `180/52/0`,
   ObjectsWithoutXrayEffect `44/16/0`, with observed ShaderOverride TypeHash
   `3D5B1D16`. Modifier TypeHash is `74425C11`.

The final import value is not the target's instance hash. Retargeting a one-slot
reference can leave the same native selector word 1 while changing the dependency
identity. Checking only BIN bytes would miss a stale/wrong target.

## Failure/reload checks and actual bug fixed

Removing the previously valid local IronCurtain target causes UnknownReference
on two consecutive dependency preparations. Each failed attempt clears the
modifier's validated list, dependency closure and visited marker. Restoration
recovers the exact edge. Swapping its strong metadata to the other shader while
leaving normalized XML unchanged is rejected by the compiler.

A new post-validation id edit test initially failed: the modifier profile
accepted changed XML id under the original declaration's asset identity.
`Ra3Ep1AttributeModifierPlugin.ProcessInstance` now compares the current id's
invariant instance hash with InstanceHandle.InstanceId before native allocation.
Both no-import v1 and explicit import v2 are regression-tested. Normal native
bytes, type hashes and processing stamps are unchanged; this adds a rejection
guard to experimental profiles that already forbid compiled/cache reuse.

Editing the actual temporary source to retarget the first modifier causes real
document reload even with UsePrecompiled=true requested. The dependency identity
changes to the second local shader, and decoding the native import selects that
new target. Caller options remain unchanged. Poisoning that shader's injected
TypeId then rejects; a further unchanged-source reload recovers the original
44-byte compiler output. Production attempts reject before missing source/null
cache access. No native stream or manifest is written by this graph test.

## Files and reproduction

- `tests/fixtures/ModifierShaderPipeline.xsd`: official schema includes and the
  bounded two-family container; original EP1 schemas are unchanged.
- `source/BinaryAssetBuilder.ManifestInspector/ModifierShaderGraphSmokeTest.cs`:
  Run and CheckEdge integrate documents, resolution and compiler selectors;
  ExpectMissing/ExpectRejected/ExpectPolicy cover rejection boundaries.
- `DependencyResolutionSmokeTest.Prepare` and Visited: existing private-stage
  reflection seam now shared internally, not exposed in the production API.
- `Ra3Ep1AttributeModifierPlugin.ProcessInstance`: current root identity guard.
- `Ep1AttributeModifierProfileSmokeTest`: no-import stale-id regression.

Run the Release/x86 ManifestInspector:

```text
modifier-shader-graph-self-test
compiler-self-test
layout-self-test
modifier-import-self-test "D:\TEMP\Red Alert 3 Uprising Source Data\Static Data\data\static.manifest"
shader-override-self-test "D:\TEMP\Red Alert 3 Uprising Source Data\Static Data\data\static.manifest"
```

The graph fixture is synthetic integration input with two supplied shader
literals. It is not a stock modifier golden and makes no game-loading claim.
Release/x86 builder and inspector builds, all 67 compiler groups, layout checks
and 33 enum mappings pass. Real static modifier import and shader slice tests
also still pass after the identity guard; PsychicCrush remains labeled as a stock variant.
Game comparisons remain separate bounded slice tests. Model/marshaller counts
remain 785/762 of 1,390. Overall effort remains approximately 50%; leaf integration
does not close the much larger type-table, packaging or runtime gates.

## Next work

The later [nested Include/mixed-target proof](RA3EP1_INCLUDED_MODIFIER_SHADER.md)
now covers real instance/all Includes and external FX metadata, with failure/retry
fixes. Reference-Include production compilation remains blocked. Next prove a
narrowly scoped multi-family diagnostic writer while keeping the full production
registry blocked. FXList/FXShaderMaterial custom processing,
complete EP1 type identity, inheritance/expressions, native cache reuse,
SDK/WorldBuilder packaging and game runtime loading remain open.
