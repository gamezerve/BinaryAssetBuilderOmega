# Bounded standalone diagnostic build command

## Outcome and usage

The Release/x86 ManifestInspector now has an explicit diagnostic input command:

```text
diagnostic-build <source.xml> <new-output-directory> [physical.manifest=runtime.manifest ...]
diagnostic-build-self-test
```

From the repository root after building the inspector, a minimal example is:

```powershell
# Reborn: choose a fresh child directory; diagnostic output is not a playable Uprising mod.
.\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe diagnostic-build tests/fixtures/DiagnosticBuildProbe.xml Release/MyDiagnosticPoC
```

The parent directory must already exist and the requested output directory must
not exist. Choose a different new directory for subsequent runs. This command
does not overwrite previous output. The checked-in example contains one local
ShaderOverride and one modifier referring to it; no external manifest is needed.
Its BIN payload is 100 bytes (40 shader + 60 modifier), with linked BIN/RELO/IMP
file sizes 108/28/16. DIAGNOSTIC_ONLY.txt accompanies all four generated files.

Optional external arguments are explicit pairs, for example
`"D:\GameData\static.manifest=base\static.manifest"`. The left side is a physical
lookup file; the right side is a relative game-visible runtime name. These names
are serialized only: no external BIN data is compiled, copied or packaged.

## Admission and deliberate v1 limits

- One standalone EA AssetDeclaration XML, at most 1 MiB / 1,048,576 characters.
- 1–32 AttributeModifier/ShaderOverride roots; root IDs are 1–128 ASCII letters,
  digits, underscore, dash or dot characters. Immediate Modifier/Rule records
  are admitted only under their matching root type.
- DTDs, Includes, other asset/control elements, inheritance, overrides, definitions
  and unresolved expressions are rejected. Include behavior is separately tested
  by the integration harness but is not opened to this public diagnostic v1 entry.
- Native eligibility remains that of the existing isolated Win32 profiles:
  shader rules are bounded to sixteen, literal material basenames and Default
  techniques; modifiers use only the explicitly proven controls/import kinds.
- At most eight unique physical/runtime external mappings; each manifest is at
  most 16 MiB, structurally valid, linked EP1 v7/aggregate and not patch-based.
  Runtime paths use the existing core validator: relative .manifest names with
  no traversal, root, colon, NUL or empty segments. Names normalize case/slashes
  before uniqueness checks.
- A nonlocal strong dependency must occur in exactly one mapped manifest. Local
  declarations take precedence. External ShaderOverride must match the proven
  native type hash/tokenization; FXList evidence is identity metadata only, not
  native FX processor or payload compatibility.
- Compiled BIN+RELO+IMP payload total must stay at or below 1 MiB.
- Output must be a new child of an existing non-reparse parent chain. Existing
  output files/directories and reparse ancestors are rejected.

No full SDK configuration or production readiness is inferred from these limits.
The complete EP1 registry/aggregate derivation, wider schemas/processors and actual
game loading remain incomplete.

## Build and publication path

The command freezes the preflighted XML and parsed external manifests into owned
temporary input snapshots. Core loading/resolution uses those snapshots, not
original files reopened after checks. Original user XML/game manifests are not
modified. Manifest source names are the sanitized snapshot name `source.xml`;
original absolute paths are not embedded in output.

The real document pipeline uses both explicitly mapped experimental plugins with
GenerateOutput=false and cache/precompiled reuse disabled. The existing private
dependency seam resolves ordered identities. All local shaders are emitted before
modifiers, then ordinal name order makes this bounded acyclic graph deterministic;
this is not the production stable sorter or a general dependency graph builder.
Native compiler entries and the existing identity checksum helper provide data
and identity stamps. Checksum is not a payload digest.

All serialized bytes are built in memory using existing utility/header writers.
An owned same-parent staging directory receives four stream files and the warning
notice. Exact staged bytes, inspector metadata, utility names/type/instance
hashes/reference IDs, absolute native offsets and every native slice must agree.
Only then does a same-parent Directory.Move publish the new destination; it never
replaces an existing folder. A destination created during verification causes
publication rejection and retains that other writer's data.

On failure, cleanup removes only the known files from the newly owned staging
directory, then attempts nonrecursive directory removal. Input snapshots are
similarly cleaned. Unexpected files/reparse state or blocked cleanup are retained
and reported to stderr, not recursively deleted. Settings.Current is restored on
success/failure. The optional internal staging hook is for fault-injection tests
only and is never supplied by the command.

This does not call OutputManager.CommitManifest/LinkStream, intermediate/cache
asset commits or packaging. Experimental production flags remain false; the
command's isolated serializer is diagnostic-only, not a production gate override.

## Verification

BoundedDiagnosticBuildSmokeTest checks local and mixed external inputs, deterministic
snapshot output, native selectors and normalized runtime mapping. Unsupported
roots/Includes/controls, malformed priority/duration, unresolved targets, DTD,
non-ASCII IDs, excess roots, unsafe/duplicate mappings and wrong-target manifests
reject without the requested output appearing. Original XML and caller settings
remain unchanged. Existing outputs retain their marker files.

Fault injection corrupts staged BIN bytes to prove late verification rejection
and cleanup. A publication race creates the requested folder after preflight;
its owner marker survives and no diagnostic manifest is written there. No known
staging directories remain in these tests.

Both Release/x86 builds, all 70 compiler groups, layout checks and 33 enum mappings
pass. The actual command was also run on DiagnosticBuildProbe.xml, producing
`Release/DiagnosticBuildPoC-20261001/diagnostic.manifest` locally; generated data is
ignored by Git. Inventory remains 785/1,390 models and 762/1,390 typed marshallers.
Overall effort remains approximately 50%; this command is not a playable mod.

## Files and next gate

- BoundedDiagnosticBuild.cs: Build, ValidateSource, Serialize, Verify and owned
  publication/cleanup guards.
- BoundedDiagnosticBuildSmokeTest.cs: admission, mapping, publication and late
  failure/race tests.
- tests/fixtures/DiagnosticBuildProbe.xml: standalone two-root example.
- Program: diagnostic-build and diagnostic-build-self-test command dispatch.

Next extend the diagnostic entry to bounded Include graphs with approved source
snapshots and reliable source identity attribution. Production stream lifecycle,
FX custom processing, complete EP1 type identity, cache/inheritance, SDK and
WorldBuilder packaging and Uprising runtime loading still require separate proof.
