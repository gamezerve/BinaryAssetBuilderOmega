# Output dependency retry and mapping proof

## Reproduced failure

`AssetDeclarationDocument.AddOutputInstance` populated ValidatedReferencedInstances
before completing strong/weak/file reference checks. If a missing strong reference
threw UnknownReference, the partial list remained non-null. A second preparation
treated it as already validated and could succeed despite the same missing target.
The new test failed against the preceding code exactly at that second attempt.

The same fast path could retain previously successful external identities after
their mappings were removed, and skip later file-reference checks. These are core
dependency validation problems, not schema or native marshalling differences.

## Change

`source/BinaryAssetBuilder.Core/BinaryAssetBuilder/Core/SageXml/AssetDeclarationDocument.cs`
now rebuilds validated reference/dependency metadata for each new preparation
attempt. The per-attempt OutputInstanceSet still prevents recursive revisits. On
failure, the current root's partial lists and visited marker are removed before
rethrowing the original error. Strong reference order, local/external distinctions,
weak tentative inclusion and strict error categories are preserved.

There is no native layout, stream format, type hash or serialized session shape
change in this block. Intermediate processor revisions remain as documented in
the [import encoding proof](RA3EP1_ATTRIBUTE_MODIFIER_IMPORTS.md). The tradeoff is
repeating dependency lookup rather than trusting a stale current-state list.
The external metadata index still caches its validated manifest contents.

## Test scope

`source/BinaryAssetBuilder.ManifestInspector/DependencyResolutionSmokeTest.cs`
loads/defaults/normalizes a source-derived IronCurtain XML through the real document
processor. Its root initially remains unregistered. The harness assigns the observed
modifier TypeHash only to reach the private dependency seam; this does not register
a production plugin. It initializes the same fresh output set as
PrepareOutputInstances and invokes AddOutputInstance, never an OutputManager.

Three tiny synthetic EP1 manifests provide external identity metadata. Their
placeholder type/instance hashes and zero native lengths are not game compatibility
evidence. They exist solely to exercise production reference lookup and preparation.

The scenarios cover:

- Two consecutive strict missing-target failures on the same document.
- Null partial-reference/dependency state and absent visited marker after failure.
- Three ordered external references resolved without queuing target compilation.
- Removal of two target mappings after successful preparation: strict failure.
- Restoration of all mappings: ordered resolution recovers.
- A missing file injected after success: FileNotFound on two attempts, then recovery.
- No BIN/RELO/IMP or production output emitted by the harness.

Run `dependency-resolution-self-test` with the Release/x86 inspector, or run the
full `compiler-self-test`. Full builder/inspector builds, all **62** compiler groups
and layout checks pass. Models/marshallers remain 784/760 of 1,390; the approximate
overall effort estimate remains 50%. The real EP1 import goldens are a separate
proof, not replaced by these synthetic metadata fixtures.

## Remaining gates

This closes the retry/mapping stage, not the complete production output lifecycle.
Local dependency graphs, recursive/cyclic references, derived-type ambiguity,
weak tentative inclusion, inheritance and full compiled asset/link/cache behavior
need broader combined coverage. Experimental profiles remain blocked from all
production writes and compiled/cache reuse. SDK/WorldBuilder packaging and actual
Uprising mod loading remain unvalidated.
