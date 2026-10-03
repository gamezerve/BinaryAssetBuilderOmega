# Bounded diagnostic build command and Include graphs

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

A checked-in Include example uses the same command:

```powershell
# Reborn: the instance Include emits the referenced child shader, not its unused sibling.
.\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe diagnostic-build tests/fixtures/DiagnosticIncludeProbe.xml Release/MyIncludePoC
```

DiagnosticIncludeShaders.xml supplies two shaders. Only RebornIncludedShader is
selected by the parent's modifier, yielding two output entries and the same
108/28/16 linked stream sizes. The child entry's source is `input-0001.xml` and
the parent's is `source.xml`; original absolute source paths are not embedded.

Optional external arguments are explicit pairs, for example
`"D:\GameData\static.manifest=base\static.manifest"`. The left side is a physical
lookup file; the right side is a relative game-visible runtime name. These names
are serialized only: no external BIN data is compiled, copied or packaged.

For a three-family example, use DiagnosticFilterProbe.xml instead. Its `all`
Include emits one shader and one weak filter before the parent modifier.
Linked BIN/RELO/IMP sizes are 240/36/16. See
[filter integration and weak-ID limits](RA3EP1_DIAGNOSTIC_FILTER_BUILD.md).

A four-family FX example now uses DiagnosticFXProbe.xml with explicit external
audio mappings. It emits five entries and 572/72/44 linked BIN/RELO/IMP bytes.
See [FX integration, real CLI usage and audio limits](RA3EP1_DIAGNOSTIC_FX_BUILD.md).

Authored/local Multisound is now the fifth narrow family. DiagnosticMultisoundProbe.xml
builds a three-root sound/FX/modifier chain using stock global AudioEvent metadata,
with linked sizes 216/28/36. See [Multisound command usage and limits](RA3EP1_DIAGNOSTIC_MULTISOUND_BUILD.md).

AudioEvent is now the sixth checked family. DiagnosticAudioEventProbe.xml builds
an event/sound/FX/modifier chain using explicit external AudioFile metadata.
The actual CLI example emits four entries and 376/44/44 linked streams.
See [AudioEvent command integration and limits](RA3EP1_DIAGNOSTIC_AUDIOEVENT_BUILD.md).

## Admission and deliberate limits

- One entry EA AssetDeclaration and at most sixteen reachable XML files, each
  at most 1 MiB / 1,048,576 characters; aggregate parsed XML is at most two MiB
  of characters. At most 32 Include edges and eight edges of nesting are admitted.
- 1–32 AttributeModifier/ShaderOverride/ObjectFilterAsset/AudioEvent/Multisound/FXList roots across the entire graph; root IDs are 1–128 ASCII letters,
  digits, underscore, dash or dot characters. Immediate Modifier/Rule records
  are admitted only under their matching root type. ObjectFilterAsset admits
  an immediate Filter and its direct IncludeThing weak leaves; wider filter
  eligibility remains closed by the existing native profile. FXList admits only
  a direct NuggetList with zero to two direct Sound leaves; other nuggets/nesting reject.
  Multisound admits direct weighted Subsound leaves only, under its isolated profile.
  AudioEvent admits direct Attack/Sound/Decay reference leaves and the five checked
  PitchShift/PerFilePitchShift/Delay/InitialDelay/NonInterruptibleTime ranges.
- Only `all` and `instance` Includes are admitted, preserving their real core
  selection semantics. Includes-only wrapper documents are allowed; at least
  one native asset must ultimately be selected. Reference Includes, cycles,
  duplicate identities across distinct files and other asset/control elements,
  DTDs, inheritance, overrides, definitions, authored TypeIds and unresolved expressions reject.
  Sound Value must be an authored literal reference, never a pre-normalized backslash selector.
  Subsound text likewise rejects authored backslash selectors/expressions.
  AudioEvent reference text in all three lists also rejects authored suffixes/expressions.
  Selected local asset dependency cycles reject during deterministic dependency-first ordering.
- Include sources must be relative .xml paths beneath the entry directory,
  resolved relative to each including file. Absolute paths, traversal/dot/empty
  segments, macros, reserved syntax, trailing dots/spaces and reparse files or
  ancestor directories reject. This deliberately excludes `../` even when a
  particular resolved path would remain inside the entry tree.
- Native eligibility remains that of the existing isolated Win32 profiles:
  shader rules are bounded to sixteen, literal material basenames and Default
  techniques; modifiers use only the explicitly proven controls/import kinds.
  Filters allow only NONE rule/alignment, empty/ENEMIES relationship, the proven
  individual kind masks and at most fifteen IncludeThing leaves. Weak names
  use the official unqualified-name syntax; colon/type prefixes are rejected
  by the official schema. Weak target presence is not checked or implied.
  FX accepts only the isolated stock-proven empty/two-Sound subset: INVALID weather,
  false/default booleans and at most one FLYING source mask per Sound. Concrete
  audio targets are AudioEvent or Multisound, including locally compiled Multisounds.
  Multisound admits 0–32 children and default/PLAY_ONE control, no optional pitch/
  percentage controls or LOOP. AudioEvent uses its isolated finite-value/32-reference
  profile with default child Volume, supported ranges and limited control tokens.
  Other audio descendants/options remain closed.
- At most eight unique physical/runtime external mappings; each manifest is at
  most 16 MiB, structurally valid, linked EP1 v7/aggregate and not patch-based.
  Runtime paths use the existing core validator: relative .manifest names with
  no traversal, root, colon, NUL or empty segments. Names normalize case/slashes
  before uniqueness checks.
- A nonlocal strong dependency must occur in exactly one mapped manifest. Local
  declarations take precedence. External ShaderOverride must match the proven
  native type hash/tokenization. Externally referenced FX/audio remains identity
  metadata only, not native dependency payload compatibility. Locally compiled
  FX, Multisound and AudioEvent are limited to isolated checked profiles.
  External AudioFile requires unique stock hash 53C81E47/tokenized false metadata;
  AudioFile roots and encoded audio compilation remain closed.
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
modified. The graph is traversed in Include declaration order. The entry snapshot
is `source.xml`, subsequent unique files are `input-0001.xml`, `input-0002.xml`,
etc. Include source attributes are rewritten to those flat names, with their
all/instance types unchanged. Repeated edges share one snapshot. Manifest source
names identify the actual asset's snapshot document rather than attributing every
asset to its parent; original absolute paths are not embedded in output. A fresh
command reads fresh sources, while an already approved snapshot retains its data.

The real document pipeline uses six explicitly mapped experimental plugins with
GenerateOutput=false and cache/precompiled reuse disabled. The existing private
dependency seam resolves ordered identities. Self/all roots seed the real local
resolution closure; unused instance/tentative roots are not emitted. Actual selected
local dependencies are emitted first, with shader/filter/AudioEvent/Multisound/FX/modifier
rank and ordinal-name tie-breaks. Nested sounds can therefore precede their
alphabetically earlier consumers; selected local cycles reject. This bounded
32-root traversal is not the production stable sorter.
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
roots/controls, malformed priority/duration, unresolved targets, DTD,
non-ASCII IDs, excess roots, unsafe/duplicate mappings and wrong-target manifests
reject without the requested output appearing. Original XML and caller settings
remain unchanged. Existing outputs retain their marker files.

Fault injection corrupts staged BIN bytes to prove late verification rejection
and cleanup. A publication race creates the requested folder after preflight;
its owner marker survives and no diagnostic manifest is written there. No known
staging directories remain in these tests.

DiagnosticIncludeBuildSmokeTest additionally proves nested instance/all selection,
actual included shader native values and manifest source attribution, deterministic
repeat builds, approved snapshots surviving later edits, fresh edit reload,
mixed external FX metadata, removed leaf/cycle repeated failures and restoration,
Includes-only wrappers, nested physical directories and repeated shared edges.
Confinement, reference controls, duplicate identities and graph root/file/depth/edge
limits reject before publication. These are diagnostic checks, not production
cache, packaging or game loading proof.

DiagnosticFilterBuildSmokeTest proves eleven source-derived weak filter goldens,
mixed three-family Include output, native weak ID order, no strong dependency
promotion, edited weak names, tentative exclusion and unsupported-control rejection.
The wider shared schema lives in DiagnosticAssetPipeline.xsd; older two-family
proof fixtures retain their original ModifierShaderPipeline.xsd.

Both Release/x86 builds, all 72 compiler groups, layout checks and 33 enum mappings
pass. The actual command was also run on DiagnosticBuildProbe.xml, producing
`Release/DiagnosticBuildPoC-20261001/diagnostic.manifest` locally; generated data is
ignored by Git. Inventory remains 785/1,390 models and 762/1,390 typed marshallers.
The actual command also built DiagnosticIncludeProbe.xml to
`Release/DiagnosticIncludePoC-20261001/diagnostic.manifest` (Git-ignored).
DiagnosticFilterProbe.xml also built to
`Release/DiagnosticFilterPoC-20261001/diagnostic.manifest` (Git-ignored).
Overall effort remains approximately 50%; this command is not a playable mod.

## Files and next gate

- BoundedDiagnosticBuild.cs: Build, Serialize, Verify and owned
  publication/cleanup guards.
- BoundedDiagnosticBuildSmokeTest.cs: admission, mapping, publication and late
  failure/race tests.
- tests/fixtures/DiagnosticBuildProbe.xml: standalone two-root example.
- DiagnosticSourceGraph.cs: confined graph admission and approved snapshot rewrite.
- DiagnosticIncludeBuildSmokeTest.cs: Include source, closure and rejection tests.
- DiagnosticFilterBuildSmokeTest.cs and DiagnosticAssetPipeline.xsd: three-family
  native integration and restricted declaration schema.
- tests/fixtures/DiagnosticIncludeProbe.xml and DiagnosticIncludeShaders.xml:
  tentative selection example.
- Program: diagnostic-build and diagnostic-build-self-test command dispatch.

The [native FX proof](RA3EP1_FX_NATIVE.md) recovered the optional-pointer base
and three exact stock chunks; narrow FX and Multisound now have checked command
integration. AudioEvent/AudioFile recovery is the next native gate. Production stream lifecycle,
FX custom processing, complete EP1 type identity, cache/inheritance, SDK and
WorldBuilder packaging and Uprising runtime loading still require separate proof.
