# Nested Includes and mixed local/external modifier targets

## Outcome and boundary

A real parent → child → leaf XML chain now passes both `instance` and `all`
Include modes through DocumentProcessor.ProcessIncludedDocuments. The parent
modifier references the included local shader and a metadata-only external FXList
target simultaneously. Real normalization, dependency resolution and native
modifier/shader compiler entries preserve both ordered target identities.

This test exposed and fixed two core failure/retry bugs. Experimental output,
compiled-cache and precompiled-document policies remain closed. No production
manifest/BIN/RELO/IMP is emitted, and no FXList native processor is introduced.

## Included and external identity proof

`ModifierShaderPipeline.xsd` now admits an optional Includes container using the
official Include type, before its bounded modifier/shader choice. Original EP1
schemas are unchanged. Each test mode owns an independent temporary graph:

- Parent: one synthetic AttributeModifier with StartFX and Shader references.
- Child: an `all` Include of leaf.xml, with no own native roots.
- Leaf: one synthetic ShaderOverride with priority 50 and a Default/Null.fx rule.
- External: one synthetic v7 manifest containing the FX identity, no game BIN.

Parent `instance` inclusion locates the shader in TentativeInstances; `all`
locates it in AllInstances. Its declaration's source remains the real leaf path,
not an inlined parent XML element. AddOutputInstance queues parent and local
shader only; the external FX is recorded but not queued for compilation.

The modifier produces 60/8/12 BIN/RELO/IMP: StartFX at BIN offset 12, optional
Shader pointer at 40 targeting the reference record at 56. RELO is
`[40, FFFFFFFF]`; IMP contains offsets 12 and 56 plus its FFFFFFFF sentinel.
The test decodes each one-biased BIN value against the actual ordered validated
table and verifies exact FX/shader TypeId and InstanceId. It does not assume
the dependency order from field offsets. The included shader produces 40/12/0.
No native output or type fingerprint is claimed for the synthetic FX manifest.

## Failure/reload coverage

For both inclusion modes:

- Changing only leaf priority 50 → 51 changes that native word through unchanged
  parent/child sources; other BIN bytes and RELO remain identical.
- Removing the external mapping rejects two consecutive dependency attempts;
  restoration recovers. Replacing the same manifest path with a different FX
  identity also rejects twice, and restoring its identity recovers.
- Deleting the harness-owned leaf rejects twice as FileNotFound. Recreating it
  recovers priority 50 without retaining stale native content or mutating caller
  UsePrecompiled=true options.
- A `reference` Include requests a child production build under current core
  semantics. Mapped experimental profiles reject it rather than bypassing the
  production policy to make the fixture pass.
- A real leaf → parent cycle rejects twice as CircularDependency; invalid leaf
  Priority rejects twice as SchemaValidation. Repairing the leaf recovers.
- The real cycle/recovery test also runs with ordinary resident document reuse
  enabled (null native plugin), proving shared stack cleanup is not limited to
  fresh experimental declarations. This additional pass is metadata-only.

## Actual core bugs and fixes

First, a previously read but subsequently deleted Include source was accepted:
SessionCache's resident FileHashItem.Exists snapshot remained true, and LoadXml's
missing-file fallback could supply an empty document. ProcessIncludedDocuments
now checks current physical source existence before accepting ordinary Include
input. Successful precompiled-reference early exits and explicit stream-reference
paths remain separate; the fix does not mandate XML for those already-resolved
compiled stream paths. With ErrorLevel > 0 the missing source is FileNotFound;
the existing permissive ErrorLevel=0 behavior remains unchanged.

Second, after adding that missing-file rejection, the next retry incorrectly
reported CircularDependency: failed recursive calls had left document/diagnostic
stack entries and Processing state behind. ProcessDocumentInternal now owns
depth snapshots and finally unwinds only its own entries, clearing Processing
on success, early return and failure. Successful-only stack pops were removed
from ProcessDocumentContents. Active ancestor entries remain during recursion,
so real cycles still fail, then cleanly unwind for the next attempt. Stream-stack
ownership also moves to this finally path; its actual production-output failure
path is not exercised by this diagnostic test because production remains blocked.

Normal native layouts, processing hashes, document/session formats and target
versions are unchanged. These are current-input acceptance and transient-state
cleanup fixes, not new EP1 cache compatibility claims.

## Implementation and verification

- `source/BinaryAssetBuilder.Core/BinaryAssetBuilder/Core/SageXml/DocumentProcessor.cs`:
  ProcessIncludedDocuments current-source guard; ProcessDocumentInternal finally
  cleanup; ProcessDocumentContents no longer owns caller stack pops.
- `source/BinaryAssetBuilder.ManifestInspector/IncludedModifierShaderSmokeTest.cs`:
  independent instance/all graphs, CheckGraph native selector proof and repeated
  failure/recovery checks.
- `tests/fixtures/ModifierShaderPipeline.xsd`: optional official-typed Includes.

Run the Release/x86 inspector with `included-modifier-shader-self-test`,
`compiler-self-test` and `layout-self-test`. Full builder/inspector builds and
all 68 compiler groups pass; 33 schema enum mappings pass. The inspector rebuild
reports the existing CS0649 warning in Set<T>._theNop, not a new test/profile
warning. Model inventory remains 785/1,390; typed marshallers remain 762/1,390.
Overall effort remains approximately 50%; no in-game compatibility is claimed.

## Next gate

The later [two-family diagnostic stream proof](RA3EP1_MODIFIER_SHADER_STREAM.md)
now writes and reads a fixed resolved graph while preserving the full production
registry gate. A validated bounded diagnostic build entry and production lifecycle
remain open. Actual reference-Include stream compilation, complete
EP1 type identity, FX custom processing, inheritance/expressions, native cache
reuse, SDK/WorldBuilder packaging and game loading remain open.
