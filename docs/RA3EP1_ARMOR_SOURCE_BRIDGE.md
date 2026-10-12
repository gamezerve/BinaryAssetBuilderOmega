# Armor source semantics to real Core output

`GeneralArmorSourceBuild.cs` connects the existing captured Include/definition/
inheritance profile to the general armor set compiler. This is an offline CLI
backend milestone, now connected to an experimental Studio alpha route, not runtime certification.
The old literal single/set commands keep their original admission rules.

## Workflow

`armor-source-build <source-root> <entry.xml> <new-output-directory>` uses an
explicit existing source root and an EA AssetDeclaration entry inside it. Output
must be fresh and outside that root. No registry/environment lookup, installation,
game launch or stock-file replacement occurs.

The source graph is audited with a focused source/output geometry check and
SdkSourcePathAudit. It does not require or claim full SDK schema-catalog parity;
the staged armor schema dependencies and focused harness suffice for this route.
Every captured XML is rechecked against its SHA-256 and held with FileShare.Read
through preparation and compilation. On Windows this denies concurrent writes
and deletion. Schema types come from the staged official EP1 files via the focused
ArmorSourcePipeline.xsd harness, which mirrors the official metadata container
order/types; this harness is not a replacement full SDK schema.

SdkInstanceInheritanceProfile(expressions:true) prepares each defining source
before importing bases. It resolves definitions before Core inheritance overlays,
retains source provenance and rejects partial unsupported preparation. The
processed owner must pass schema validation and zero-file-field binding before
consumed metadata is removed. GeneralArmorAuthoredBuild.AdmitSet then rechecks
literal armor admission, including lookup-ID collisions and duplicate damage
kinds. Real Core/scoped intermediate assets and linked streams are published;
all native/token values, identities, checksum and member slices are verified.

Output includes prepared-armors.xml, compiled/general-armor-build.json, the
default-reader BIG/config/SKU candidate and armor-source-build.json. The latter
is the final source-closure receipt: raw source hashes and preprocessing evidence
are distinct from the compiled payload receipt. A nested build receipt alone
certifies only its canonical input, not the original multi-file source closure.
Partial failed outputs are retained without a successful outer receipt.

## Supported source subset

- Source-backed `type="instance"` Includes provide directly visible same-type
  inheritance bases. Includes may use confined relative paths or DATA aliases;
  transitive preparation is supported without flattening descendants into direct
  visibility. Imported bases are not automatically compiled into the owner's
  output stream. Only owner-declared armors are emitted.
- `type="all"` is restricted to Include-free, definition-only metadata leaves.
  It does not import arbitrary assets or precompiled manifests.
- Local and imported `inheritFrom="BaseId"` or `ArmorTemplate:BaseId` can copy
  inherited scalars and armor lists through existing Core joins. Two populated
  lists can append distinct Damage kinds in base-then-derived order, including
  imported/local chains. This is append, not Damage-key replacement.
  Local inheritance chains and scalar overrides are supported. Final schema
  defaults are inserted by the existing general armor compiler.
- Asset fields accept exact `=$NAME` references. Definitions may be literals
  or the already reviewed backward-visible alias, suffix-concatenation and
  nonnegative integer multiply-add forms. Resolved percentage values still must
  satisfy the general armor literal bounds. Direct arbitrary arithmetic such as
  `Percent="=1+2"` remains refused.

Example owner (the included file defines RemoteBase and may have its own
definition-only Includes):

```xml
<!-- Reborn: imported base values are prepared in their defining source before owner overrides. -->
<AssetDeclaration xmlns="uri:ea.com:eala:asset">
  <Includes>
    <Include type="instance" source="DATA:Base.xml" />
  </Includes>
  <Defines>
    <Define name="SIDE" value="50" />
  </Defines>
  <ArmorTemplate id="MyDerivedArmor" inheritFrom="RemoteBase"
                 SideDamageScalar="=$SIDE" />
</AssetDeclaration>
```

## Limits and explicit refusals

The graph is bounded to 32 sources, 64 Include edges, 64 KiB per original file
and one MiB aggregate. Final owner output remains limited to 1..32 armors, each
with 0..64 unique non-ALL damage entries and finite unsigned percentages
0..10000. Original files must be plain unprefixed EA source XML. Nonempty Tags,
mixed types, art/audio resources, reference Includes, arbitrary all Includes,
DTD/PI content, unsafe paths, missing/cyclic graphs and conflicting definitions
are refused. The source profile does not silently turn unsupported metadata or
assets into skipped successful output.
The captured closure must have unique engine armor lookup IDs across all source
files, including imported bases and case-only name differences. This conservative
bridge does not admit same-ID source shadowing or infer first/last-wins behavior.
Definition override directives are likewise refused by the selected profile.

Armor child replacement by Damage is **not** established. The reused Core joiner
has identity-based child semantics; adding a same-damage leaf to an inherited
list may append rather than replace. Duplicate damage kinds in the final armor
are refused. Do not claim that all EA armor inheritance/list override syntax is
supported from scalar and one-sided list-copy evidence alone.

Distinct-Damage append is now independently tested: a four-file graph adds
SNIPER at an imported intermediate base and CANNON at its owner, while a local
base/derived pair adds GUN then CANNON. Prepared order and complete real package
bytes equal a separately authored literal declaration. Editing the appended
definition-derived coefficient changes the package. No shared joiner or source
admission policy changed. The already exported h compiler also builds the new
`samples/armor/source-append/mod.xml` graph; this corrects an overly narrow
description of its existing source semantics, not a new runtime certification.

The follow-up focused run passes 18/18 groups in 39.504 seconds with unchanged
compiler/runtime hashes (`artifacts/ArmorAppend-a/batch-report.json`). The source
bridge group, including the new append controls, passes in 4.041 seconds. The
append example built through the preserved h driver/compiler produces one owner
armor with GUN=250.5%, CANNON=40%, Default=75.5% and SideDamageScalar=125%.
Its default-reader BIG SHA-256 is
`491A7718F395056BB7ECCA0B24D688108B7608CFC1CC7335A96A66D33962B0B5`.
No new workspace inventory, actual GUI observation or gameplay claim is added.

Source checks are bounded current snapshots, not protection against malicious
filesystem races or a general cross-platform transactional build. Staged schema
catalog parity is not EA ProcessingHash/type-table equivalence. New IDs still
need consuming GameObjects for observable gameplay; stock collision inventories,
GameObject publication, actual new Studio GUI observation and art/custom-data remain separate
gates. No new gameplay proof is claimed.

## Regression

`armor-source-self-test <new-output-directory>` combines a four-file graph,
transitive/direct instance bases, definition-only all Includes, imported/local
inheritance, root/list values and definition alias/multiply-add evaluation.
It compares the resulting package byte-for-byte with an independently written
literal three-armor declaration and a repeated source build. A changed imported
definition must alter package bytes; original sources remain unchanged during
successful builds. Nineteen pre-output refusals plus occupied-output refusal
cover missing/unsupported expressions, bases/types, unsafe/reference/cyclic
Includes, unknown/mixed identities, out-of-range values, nonempty Tags, PI,
lookup collisions (also across imported/owner sources), definition collision/
override directives and unsupported two-sided duplicate-damage list behavior.

The batch matrix now has 19 focused groups and 21 with Full enabled, including
the separate stock/native GameObject consumer projection. Existing
literal armor, fixed armor/weapon and diagnostic source regressions stay
independent; this connection does not relax their policies. The experimental alpha
route is separately warned and never used as a fallback after a failed fixed input.

`armor-source-preflight` accepts the same arguments and emits a validation summary
without creating the requested output directory or compiling a package. The alpha
driver compares all original source hashes and the prepared digest between this
admission and the real build before publishing its final receipt. See the alpha
README for the supported source subset and automatic route selection.

Final verification on October 13: **18/18** focused groups passed in **43.618
seconds**, excluding a **5.33-second** incremental Release/x86 build with zero
warnings/errors. The source bridge group passed in **11.128 seconds**. All eight
compiler/runtime file hashes stayed unchanged during the run. Local evidence:
`artifacts/ArmorSrc-final/batch-report.json`, `armor-source-bridge.log` and the
source/compiled receipts under its `as` child. The broad Full suite was not
rerun: this milestone adds an inspector adapter and focused source harness;
shared Core/native/tokenizer/profile implementations were not changed. No game,
installed files or exported alpha workspace was modified.
