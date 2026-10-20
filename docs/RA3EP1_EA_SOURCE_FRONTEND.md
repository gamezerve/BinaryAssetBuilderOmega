# Explicit EA-shaped XML source frontend

This supersedes the automatic recruitment bridge: a GameObject ID **never**
causes the compiler to invent a LogicCommand. The preceding new-ID package had
user-reported gameplay success; that report does not establish all EA source
preprocessing semantics or arbitrary GameObject/module editing.

## Live source layout

Use only **alpha/EALAModStudio.exe → RebornTest → Build Mod**.

- `alpha/mods/RebornTest/data/mod.xml`: XML declaration, EA AssetDeclaration,
  namespace declarations, empty Tags and two explicit all-XML Includes.
- `Conscript.xml`: attribute-only derived unit, with explicit instance base and imported definition.
- `ConscriptBase.xml`: complete authored Conscript base; imported for inheritance, never emitted as an asset.
- `Defines.xml`: `REBORN_CONSCRIPT_BUILD_TIME=2`, referenced as `=$REBORN_CONSCRIPT_BUILD_TIME`.
- `CommandData.xml`: EA AssetDeclaration containing the explicit existing
  construction-command override and its `<Object>RebornConscriptTrial</Object>`.

Removing the command Include emits only the GameObject. A custom object is not
automatically recruitable; a construction command must reference it and an
existing CommandSet must reference that command. The sample explicitly overrides
the already referenced stock command, so no CommandSet edit is needed. New
command IDs can be compiled but do not acquire consumers automatically.

## What is supported now

| Source feature | Current support |
| --- | --- |
| XML declaration / UTF-8 / comments | Yes |
| EA AssetDeclaration and default asset namespace | Yes |
| xsi and xi namespace declarations | Yes; declarations alone execute nothing |
| Empty Tags / Includes | Yes |
| Include type=all to local XML | Yes, relative to declaring XML, confined to mod data |
| Multiple source files | Yes, one GameObject plus zero/one UNIT_BUILD LogicCommand |
| Attribute order and indentation between elements | Flexible; qualified structure comparison, not whole-XML byte whitelist |
| Schema defaults / optional attributes | Compared after default expansion; omission works only if it retains the admitted Conscript semantics/native graph |
| Child element order | Must satisfy the official XSD; list/module order is not arbitrarily rearranged |
| Custom GameObject/command IDs | ASCII identifiers, 1–128 characters; stock GameObject collisions other than the explicit Conscript override refuse |
| GameObject edits | 63 root primitives, 15 fixed enum/KindOf/weak-ID values, optional stock-target root strong references with complete dependency reindexing, six dynamic root ASCII strings, 412 direct module primitive/enum/fixed-flag slots, editable local module IDs, existing nested numeric owners and object ID, on the complete Conscript topology |
| LogicCommand | Explicit UNIT_BUILD with one Object targeting the same local GameObject; no options/special payloads |
| inheritFrom / instance includes | Actual Core merging of schema-valid partial GameObject bases, nested keyed children and derived bodies; final output must retain the admitted complete native graph |
| reference includes | Not enabled; explicit error |
| Defines / $ expressions | Existing bounded definition engine, including imported metadata definitions and `=$NAME` substitutions before inheritance; not arbitrary expression execution |
| DATA: XML alias | Mod Data first, then SDK SageXML (or explicit local SdkDataRoot) |
| ART:/AUDIO: aliases | Mod-first/SDK-fallback path resolution; ART basename two-character fanout; see source-search contract |
| W3X/audio resource processing | Still not connected to this frontend; explicit error after path resolution, no skipped model sources |
| xi:include execution | Literal XML fragments using EA `xmlns(prefix=...) xpointer(/prefix:Root/child::*)`, confined relative/DATA lookup, captured hashes/search precedence; not arbitrary XPointer, network, text or fallback execution |
| Nonempty Tags | Not enabled; explicit error |
| Existing module primitive attributes | Enabled through the schema/native/token contract; [complete inventory](RA3EP1_MODULE_PRIMITIVE_CAPABILITIES.md) |
| Existing nonempty schema-typed strong-reference targets | May retarget to compatible, uniquely resolved stock assets; emitted occurrence identities are checked independently against authored source and catalog |
| Arbitrary module additions/removals/reordering, nested child/list/enum edits or non-root reference presence changes | Not enabled by accepting an EA envelope, direct module attribute values or local ID changes |

The existing experimental armor frontend has its own bounded preprocessing
support. That does not automatically extend to this GameObject frontend.

## Can the untouched official Conscript XML compile?

**Not with every original resource Include unchanged.** ART/W3X source processing
is still not connected. However, the user's current Reborn source (with those
resource Includes commented out) now passes real BaseObject/BaseInfantry merging,
GlobalDefines substitution and its ParachuteDrawSoviet XInclude expansion.
The source is not replaced with a captured flattened fixture. Bare mixed-case
reference names compare semantically with the captured qualified names; actual
authored references still pass independent Core/catalog/native import checks.

All attribute order is irrelevant to XML meaning. Child order is different:
an XSD sequence constrains it, while choices/repeated lists have their own rules.
Likewise, optional/default and inherited fields may be omitted under their
specific schema/source rules; required fields cannot simply disappear. Even
schema-valid edits outside the current native Conscript profile still refuse.
These distinctions also apply inside modules.

## Implementation

`ConscriptSourceInput` reads bounded secure XML (no DTD/network entity resolution),
captures an ordinary Include graph of at most 32 source files/four MiB, detects cycles and refuses filesystem
escape/reparse paths. It compares expanded element/attribute names rather than
prefix placement or raw OuterXml strings. Script/leaf text remains significant.
Attributes are aligned before Core assigns strong-reference occurrence indices;
element/list order is preserved.
The fragment session additionally bounds reads to 32 XML files/four MiB and
16 inclusion levels, and exports their hashes/search witnesses into the final
source receipt. These are current alpha resource budgets, not claimed EA limits.

Defaults are materialized **only in comparison copies**. Emitting every optional
empty-string default would create additional allocations in existing marshalers;
the actual submitted graph retains authored optional storage. Complete native
and metadata checks still catch unintended differences. Source diagnostics now
identify the first differing field instead of reporting only a fingerprint.

Source preparation writes a provenance receipt and detached game/command XML in
the owned temporary build generation. The command compiler reads only that
explicit captured command. All source hashes are rechecked before publication.
Final build receipts expose `AutomaticCommandGenerated=false` and AuthoredSources.
Temporary frontend paths, like other backend paths, remain provenance only.

The source bridge uses `SdkSourcePathAudit`, `SdkIncludeDefineProfile` and
`SdkInstanceInheritanceProfile`. Its explicit GameObject source route delegates
partial/nested merging to the existing Core NodeJoiner through
`SdkGameObjectInheritance`; intermediate schema-valid bases are not required to
contain a complete Conscript cost/module graph. Defaults are checked on a clone,
not injected into inherited source. Earlier generic diagnostic profiles keep
their original limits. Meaningful Lua/leaf text is preserved; only structural
indentation is removed for Core node handling. `SdkGameObjectFragments` captures
literal fragment sources and precedence witnesses alongside ordinary Includes.
The final resolved object still passes the complete reviewed shape, native,
metadata, reference and stream checks. This does not enable arbitrary new module
topologies or unreviewed resource processors.

Next major frontend work: official source-root/resource closure and actual W3X,
then broader child/module edits. Full arbitrary GameObject authoring is not complete.

The mod-over-SDK source root search is now connected; exact precedence,
local stock-library configuration and resource limitations are documented in
[Mod-first SDK source search](RA3EP1_SOURCE_SEARCH.md).

## Instance/definition integration update

RebornTest now has five source files. Editing the definition changes the actual
derived BuildTime, while the base remains source-only. Missing definitions/bases,
reference-role Includes, nonempty Tags and include cycles are refusal controls.
This is source preparation, not replacement with a stored flattened object.

Integration checks passed: the grouped source-language suite, the existing four
Include/definition/inheritance/expression suites, and **21/21 alpha regression
cases**, including the unchanged BuildTime=1 golden archive and the restored
five-file developer source. Studio's exact-reference CLR4 loader also passed.
No game was launched and the user's Reborn sources/outputs were not changed.
Evidence is local under `alpha/.work/inheritance-*.log` and `dev-sdk-test.json`;
these generated logs are not shipped as source assets.

## Verification — October 18, 2026

<!-- Reborn: this historical checkpoint remains for traceability; current source/inheritance verification is recorded below. -->

- Grouped frontend checks passed: three-file source graph, reversed attributes,
  matching omitted default, changed-default refusal, object-only/no hidden
  command, and include-cycle refusal.
- Alpha regression **21/21** passed, including all earlier armor/weapon profiles,
  the old BuildTime=1 golden archive, scalar publication/refusal and restored
  explicit-source developer build. User Reborn sources/outputs were unchanged.
- Final complete explicit-source build passed; Core/Manifest/Utility readback
  verifies the tokenized GameObject plus native command and local reference.
  Receipt reports three authored sources and AutomaticCommandGenerated=false.
- Current BIG SHA256:
  `1D432B279210D696F4BE6AB7AC92F5D195C7EE7645F3B940FB075BF2BABCC484`.
- Final incremental compiler build: zero warnings/errors. Studio's exact-reference
  CLR4 loader/config/options check passed. Tools were synchronized to live alpha.
- The untouched official Conscript source was actually checked: it is refused
  at `DATA:GlobalData/GlobalDefines.xml`, not accepted through a hidden substitute.
- Evidence under `alpha/.work`: `ea-source-language.log`,
  `ea-source-regression.log`, `dev-sdk-test.json`, `ea-source-final-compiler.log`,
  `ea-source-final-build.log`, `ea-source-studio-loader.log`, and
  `official-conscript-source-check.log`. Final source hashes match the receipt.
- No game/installation changes or new runtime test were performed in this step.

## Reborn source compilation checkpoint — October 20, 2026

- The user's actual Reborn data was copied read-only into the reusable developer
  slot for reproduction. Its BaseObject/BaseInfantry chain, GlobalDefines,
  nested derived body and EA parachute XInclude now prepare five real source
  files without substituting a pre-flattened unit.
- That reproduction successfully generated the one-asset existing-ID Conscript
  BIG/SKUDef through full Core/native/metadata/import/archive checks. No command
  was generated. User Reborn sources and its published output were not changed.
- **27/27 live regression cases passed**, including the reproduction, earlier
  armor/weapon/source variants, grouped missing-name refusal and restoration of
  the ordinary five-file developer sample. The restored sample retains SHA256
  `87241958D0EC4CDB3A133FFC837E023A01B51716924F6B532A30BCBDE7EDF918`.
- Separate source tests passed: partial bases, nested keyed merges, Replace,
  no explicit-default leakage, alias spelling equivalence, wrong-name/type
  refusal (historical pre-retarget source gate), final incomplete-object refusal
  and fragment confinement/cycles.
- Current evidence is `alpha/.work/test-RebornSourceMirror.log` and
  `alpha/.work/dev-sdk-test.json`. No game was launched; this is compilation
  evidence, not a new gameplay result or general resource-processor support.

## Existing strong-reference target freedom — October 20, 2026

<!-- Reborn: replace captured-name pinning with typed target and native occurrence validation, without advertising new native topology. -->

The completed Conscript graph no longer pins its existing nonempty strong
references to the original asset names. This includes nested `Weapon/@Template`,
not just root UnitIntro/UnitTypeIcon. A source-comparison mask validates the
declared target type; it does not rewrite the real authored source. Native
compilation resolves the real target against the configured stock catalog.
An independent schema-typed source traversal derives every expected target
identity, first anchored against the captured original occurrence table. The
emitted table must match it in full, including repeated target occurrences.
Whole BIN/RELO/IMP ownership and metadata/stream/archive checks remain active.

The actual Reborn source with primary weapon `JapanArcherInfantryEnergyBow`
successfully produced BIG/SKUDef from a read-only copy in RebornTest. A grouped
native test also retargeted both existing weapon slots to that same real EP1
weapon, round-tripped the resulting stream and rejected missing targets, wrong
types, authored numeric indices and a forged dependency table. These are offline
compile/package results; no new gameplay observation is claimed.

This capability does not add/remove non-root reference slots, add/reorder native
modules, compile a newly declared WeaponTemplate alongside the unit, or compile
ART resources. Those require their own processor/layout or emitted-source graph
support. Weak IDs and ordinary strings are not reinterpreted as strong imports.

Final focused live regression: **6/6 passed** (combined weapon targets + dynamic
EditorName + physics/AI primitives + both root imports removed; three invalid
reference builds retaining the good package; restored developer source build;
explicit local recruitment consumer). The actual read-only Reborn reproduction
also passed separately on the final compiler. User sources/output were unchanged,
and developer sources were restored. Compiler build: zero warnings/errors;
source inheritance/fragment tests passed; live compiler matches rebuilt output.

The earlier full regression attempt was interrupted by a maintenance sync
contending for the SDK lock; it is not claimed as a completed new 27-case run.
The focused harness preserves script/mixed-content whitespace and uses a
supported root string, not the still-unenabled DisplayName edit. Evidence:
`alpha/.work/dev-sdk-test.json`, `test-RebornSourceMirror.log` and
`test-ConscriptReferenceCombined.log`.
