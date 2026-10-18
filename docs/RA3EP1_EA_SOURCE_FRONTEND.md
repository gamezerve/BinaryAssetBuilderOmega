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
| GameObject edits | Still only the six scalar fields plus ID, on the complete Conscript shape |
| LogicCommand | Explicit UNIT_BUILD with one Object targeting the same local GameObject; no options/special payloads |
| inheritFrom / instance includes | Actual Core inheritance, including captured base chains; derived Conscript must be attribute-only and resolved output must retain the admitted complete shape |
| reference includes | Not enabled; explicit error |
| Defines / $ expressions | Existing bounded definition engine, including imported metadata definitions and `=$NAME` substitutions before inheritance; not arbitrary expression execution |
| DATA: XML alias | Mod data root only; no implicit fallback to official source directories |
| ART:/AUDIO: aliases / W3X processing | Not connected to this frontend; explicit error, no skipped model sources |
| xi:include execution | Not connected; explicit error |
| Nonempty Tags | Not enabled; explicit error |
| Arbitrary module additions/removals/edits | Not enabled by accepting an EA document envelope |

The existing experimental armor frontend has its own bounded preprocessing
support. That does not automatically extend to this GameObject frontend.

## Can the untouched official Conscript XML compile?

**Not yet.** It inherits BaseInfantry/BaseObject, includes GlobalDefines and
many DATA:/ART: resources, and contains nested source composition. Those require
the actual preprocessing/resource pipeline, not merely a namespace/header or a
schema validator. This frontend deliberately refuses that unprocessed graph;
it does not silently substitute the captured flattened Conscript for it.

All attribute order is irrelevant to XML meaning. Child order is different:
an XSD sequence constrains it, while choices/repeated lists have their own rules.
Likewise, optional/default and inherited fields may be omitted under their
specific schema/source rules; required fields cannot simply disappear. Even
schema-valid edits outside the current native Conscript profile still refuse.
These distinctions also apply inside modules.

## Implementation

`ConscriptSourceInput` reads bounded secure XML (no DTD/network entity resolution),
captures at most 32 source files/four MiB, detects cycles and refuses filesystem
escape/reparse paths. It compares expanded element/attribute names rather than
prefix placement or raw OuterXml strings. Script/leaf text remains significant.
Attributes are aligned before Core assigns strong-reference occurrence indices;
element/list order is preserved.

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
`SdkInstanceInheritanceProfile`, not a second inheritance implementation. A new
opt-in complete-Conscript copy gate schema/structure-checks the actual authored
base before allowing the existing Core NodeJoiner to copy it. Earlier generic
tree profiles retain their previous limits. Meaningful Lua/leaf text is preserved;
only structural indentation is removed for the Core joiner's node handling.
Populated derived child merges remain explicitly closed. The final resolved object
still passes complete native/metadata/reference/stream checks.

Next major frontend work: official source-root/resource closure and actual W3X,
then broader child/module edits. Full arbitrary GameObject authoring is not complete.

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
