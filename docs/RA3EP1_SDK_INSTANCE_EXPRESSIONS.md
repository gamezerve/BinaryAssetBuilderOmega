# Expressions before guarded inheritance — October 8, 2026

## Result and limits

Independent `--instance-expressions` / `diagnostic-direct-instance-expressions-v1`
advances the real graph from **388 Validated / 8 RequiresPreprocessing** to
**389 / 7**, over 396 sources and 664 Include edges. The entire
`GlobalData/ObjectCreationLists.xml` owner now validates against the effective
EP1 diagnostic schema. No earlier-valid document regresses. Older
`--instance-metadata` remains 388 / 8; older options and defaults are unchanged.

This is XML preparation and validation, not native compilation. Both audits
still exit 2; ScopedGraphComplete, FullDependencyCoverage and ProductionBuildReady
remain false. Six path issues and 198 missing AUDIO-header dependency occurrences
remain. No output directory is created and no game-load proof is claimed.

## Preparation order and source authority

`SdkInstanceInheritanceProfile.ApplyDocument` proves captured raw Include roles,
paths, asset handles and metadata leaves before substitution. Each source is
prepared child-first in its own definition context, before temporary external
base injection and local overlays. Already-resolved imported values are not
reinterpreted using the importing owner's definitions.

`SdkIncludeDefineProfile.Apply(..., beforeInheritance: true)` assembles the
captured definition table using the previously reviewed ordered alias,
quoted-suffix concatenation and checked nonnegative integer multiply-add forms.
`SdkLocalDefineProfile.ApplyBeforeInheritance` then substitutes only exact,
case-sensitive `=$NAME` asset slots. It does not evaluate arbitrary asset
arithmetic. Expressions in id, inheritFrom, TypeId or namespaced directive
attributes are rejected; inheritance markers are not temporarily deleted to
bypass an earlier guard. Definition collisions, unknown/forward names and stale
source witnesses remain closed.

Existing source, closure, depth, element and expression-slot bounds remain.
ExpressionPreparation evidence connects raw and prepared hashes with definition
origins, evaluations and source witnesses. Failures publish no partial overlays,
markers, imported bases, metadata sources or expression preparation authority.
Final schema binding is mandatory; invalid output yields zero trusted file fields.

## Reviewed ObjectCreationList marker

Official RA3 and EP1 ObjectCreationList declarations derive from BaseAssetType,
not BaseInheritableAsset. Their schema does not declare inheritFrom. The core
InstanceDeclaration.XmlNode setter consumes this marker before validation;
AssetDeclarationDocument still forbids non-inheritable bases found outside Self.
DocumentProcessor processes expressions before overrides before validation.

`SdkSelfAttributeInheritance.ReviewedMarkerType` adds only local ObjectCreationList
under the new expressions profile. The existing bounded ancestry check and
marker consumption apply. Imported ObjectCreationList bases remain refused:
the external BaseInheritableAsset eligibility rule is unchanged. Older metadata
admission still refuses even a literal local ObjectCreationList marker fixture.
No Core code or reference XSD is edited.

## Real owner evidence

- Raw owner SHA-256:
  `17BEC1B39626594740F956EA86813D8372168EFD5DE548DB09363D9D280F5C85`.
- Expression-prepared SHA-256:
  `2D8AF33ACDF3A5517ED131124D8588F88D25C7606E0493097DB93A3B70F1CBCC`.
- Final prepared owner SHA-256:
  `4DF22EA16DC95BAC848AFF71BE6F3562D4310C5C7B6356644E104AAEBAC82443`.
- 54 substitutions, 82 definition origins (80 global plus two local), two
  definition sources and five evaluated definitions using three reviewed forms.
- One local overlay: JUOdessaSupportVehicle_Die_OCL inherits
  JULightTransportVehicle_Die_OCL; one consumed marker with SchemaDeclared=false.
- Two prepared sources, one metadata Include, zero exported metadata assets and
  zero typed file dependencies for this owner.
- GlobalDefines raw SHA-256:
  `F352943CAA81641F574B10EA379C899AB98C5A694D0C0E083A304CCD2EA4A9FC`.

The five evaluated definitions are BASE_CRUSHABLE_OBSTACLE_CANSKIPSHADOW_KINDOF,
FACTORY_REPAIR_DRONE_KINDOF, REPAIR_DRONE_HOME_DECAL_SIZE (440),
SOVIET_REPAIR_DRONE_HOME_DECAL_SIZE (640) and SUMMONED_REPAIR_DRONE_KINDOF.
This does not prove game-object reference closure or native type/hash compatibility.

## Verification and remaining work

`SdkInstanceExpressionsSmokeTest` is registered group 138 and available as
`sdk-instance-expressions-self-test`. Fixtures cover local/imported definition
contexts, child-before-parent substitution, ordered anonymous child append,
local undeclared marker consumption, imported non-inheritable refusal,
earlier-profile isolation, unknown/unsupported/colliding/identity/directive
expressions, stale witnesses, late atomic rejection and final schema failure.
Sources remain unchanged and no output is created.

All 138 compiler groups, 33 enum checks, three Include-classifier fixtures and
three CLI isolation probes pass. CLI rejects conflicting profiles, duplicate
expression flags and expressions on path-only preflight. Final Release/x86
build has zero warnings/errors. Repeated real metadata/expressions audits confirm
388 / 8 versus 389 / 7 and zero earlier-valid regressions.

Seven owners remain blocked: Music and SoundEffects require broader asset
expression evaluation; Voice reaches the existing tree-size/unprefixed lookup
guard; three AI owners reach sibling-ID uniqueness/literal guards; AIP_CC_32
reaches cross-QName child-ID collision refusal. These are first blockers, not
a guarantee that removing them would complete the pipeline. Next: characterize
sibling-ID collisions against the unchanged core without weakening identity
guards or authorizing data-loss behavior.

Models remain 785/1390, typed marshallers 762/1390 and EP1-only coverage 48/48
for both. Manual effort remains **51% complete / 49% remaining**, not a percentage
of working mods. Native dispatch/layout/reference closure, stream emission,
SDK packaging/WorldBuilder integration and in-game validation remain open.
