# Actual original Conscript: complete metadata qualification

## Outcome — October 18, 2026

The actual original Conscript graph now has independently stock-qualified
metadata, not only complete native semantics or the earlier synthetic selected
root's field-name coverage.

| Output / proof | Result |
| --- | ---: |
| Original root names and wire tags | 126 / 126 |
| Disjoint root-owned metadata intervals | 21 |
| Metadata prefix | 88,824 bytes |
| Native graph | 39,360 bytes |
| BIN | 128,184 bytes |
| RELO | 1,800 bytes |
| IMP | 1,104 bytes |
| Shared Core strong occurrences | 275 |
| Metadata/native corruption refusals | 22,212 |

All 49 original modules, six populated EP1 root owners, geometry, three
ArmorSets, the original locomotor, VisionInfo and ordinary root fields retain
the [complete native qualification](RA3EP1_CONSCRIPT_ORIGINAL_NATIVE.md).
There is still exactly one explicitly bounded Script CRLF/LF equivalence.
Stock native size is 39,364 bytes; compiled pointer layout and that string
allocation need not be byte-identical. Metadata prefix size does match stock,
but equal size alone is never the qualification criterion.

## Production change

`source/BinaryAssetBuilder.XmlCompiler/BinaryAssetBuilder/XmlCompiler/Ep1GameObjectEmptyRootProjection.cs`
adds an explicit `ComposeOriginal(native, schemas)` entry point. It shares the
existing insertion/coordinate-shift algorithm, retaining the previous
`Compose(native, schemas)` behavior without admitting original-only variants
through the selected-root default route.

The actual original source has three absent pointer owners that the selected
root fixture populated. The new route emits their required null root rows:

| Field | Native slot | Wire type |
| --- | ---: | --- |
| DisplayNameTransformed | 408 | 1A2933AA |
| Flammability | 464 | 10444D6C |
| LiveCameraOffset | 528 | 29368077 |

All three native slots must be zero before composition. Nonzero values refuse;
no populated metadata ABI is inferred from the null cases. These rows add 36
bytes to the otherwise 123-name original profile. Production uses reviewed
ABI declarations and schemas, never stock metadata templates or captured
per-asset native coordinates. The native tail is copied once, relocations and
auxiliary source coordinates are shifted, and common Core occurrence indices
remain unchanged.

## Independent oracle

`source/BinaryAssetBuilder.ManifestInspector/GameObjectOriginalMetadataSmokeTest.cs`
is called after the actual original whole-native proof, inside the existing
`gameobject-original-native-self-test` diagnostic. It does not widen the alpha
build dispatcher or write a game package.

The oracle checks exact stock root names/tags, every direct root native
binding and every null root target. Root-owned metadata intervals are derived
from bounded stock rows; generated intervals must be disjoint and cover the
entire prefix, including the root directory/sentinel, without gaps or orphans.

For Draws, AI, Body and client containers, independent complete typed native
walks translate stock native coordinates before comparing every table byte.
Behavior metadata uses the existing independent per-field-context proof;
shared stock allocations are not mistaken for one-to-one native coordinates.
The six EP1 owners and geometry likewise use complete typed native walks and
field-context metadata proof. Armor/locomotor/VisionInfo and empty-list tables
use independently walked root coordinates. No initial table is qualified by
regenerating expected bytes through the token producer.

After all intervals pass, the independently qualified expected metadata is
frozen for exhaustive mutation checks: every four-byte metadata word is
flipped separately (22,206 controls), the three original-only null slots are
made nonzero, both auxiliary streams are removed separately, and a native
BuildTime byte is corrupted. Each must explicitly refuse. Full native-tail
and auxiliary inversion, deterministic composition and input-buffer isolation
are also checked.

The enclosing original-source diagnostic still hash-brackets the official
three-object chain and definitions read-only. Source assembly remains an
explicitly qualified captured chain, not a general inheritance/include engine.

## Verification evidence

The strict `gameobject-original-native-self-test` now runs this metadata proof
after whole-native qualification. A fresh Focused batch with explicit official
XML root and four isolated workers passed **74/74 groups in 389.051 seconds**.
The combined actual-original case took **79.091 seconds**; unchanged selected
126-field and all-module cases passed in **46.883 / 40.478 seconds**.

Evidence: `artifacts/ConscriptOriginalMetadata-a/batch-report.json` and
`gameobject-original-native.log`. All eight frozen compiler/runtime hashes
stayed unchanged during the batch. Incremental batch build: zero warnings and
errors. No Full run, game execution, installation modification or alpha
capability expansion. The initial dependency-rebuilding compile succeeded
with 111 warnings and zero errors; that is separate from the incremental
batch build. Timings include overlapping diagnostics, not a mod-build benchmark.

## Remaining gate

This metadata proof alone does not publish a stream. The subsequent
[scoped original Conscript stream proof](RA3EP1_CONSCRIPT_ORIGINAL_STREAM.md)
now writes and independently reads back the qualified pair with its ordered
external metadata references. General production dependency publication,
arbitrary/new-ID GameObject authoring, playable packaging, LUA execution and
gameplay remain separate gates.
