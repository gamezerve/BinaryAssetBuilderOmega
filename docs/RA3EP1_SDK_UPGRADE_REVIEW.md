# Upgrade singleton normalization review — October 8, 2026

## Outcome and boundary

The two Allied upgrade owners can be normalized by the existing managed core
without losing their complementary requirements. This is a **focused read-only
review**, not admission of GlobalData/Upgrade.xml into the typed graph and not
proof of stock EA compiler behavior or native Uprising compatibility.

`Upgrade_AlliedTech2` authors RequiredObject=AlliedRefinery in one GameDependency
and ForbiddenModelConditions=STRUCTURE_UNPACKING in a second. AlliedTech3 uses
NeededUpgrade=Upgrade_AlliedTech2 plus the same condition. The reviewed schema
allows only one GameDependency. The unchanged NodeJoiner folds each pair into
one branch that retains both the reference and condition; both isolated merged
owners pass the full pinned diagnostic effective-schema binding.

The typed graph intentionally remains **387 Validated / 9 RequiresPreprocessing**.
Upgrade remains blocked by the pre-allocation occurrence check. No new generic
normalization profile, cardinality relaxation, source repair or schema change
was introduced. No output files or game/native stream proof.

## Why pre-join schema validity is not the whole contract

`DocumentProcessor.ProcessDocumentContents` calls ProcessExpressions, then
ProcessOverrides, then Validate. `NodeJoiner.Override` copies the base followed
by the derived node. `ReplaceXmlNode` selects same-name singleton children using
SelectSame with idMatchRequired=false; it then recurses into the selected branch.
Consequently two authored singleton branches can become one before validation.

Owned regressions pin these distinct behaviors:

- The authored duplicate singleton fixture is schema-invalid; its complementary
  merged result is valid and preserves the object/upgrade reference and condition.
- Reversing these disjoint complementary branches gives the same merged result.
- Conflicting attributes use last-write-wins. Final schema validation can pass
  even though an earlier condition was lost.
- Two repeated anonymous RequiredObject leaves preserve multiplicity rather than
  deduplicating. A singleton parent does not make its reference list a singleton.
- An intentionally unknown field in an owned fixture remains schema-invalid.
  This is a synthetic negative test, not a naming error in the actual XML.
- The current filter scope still refuses the duplicate singleton document
  atomically, with no processed hash or overlay/filter witnesses.

The initial suspected EVA naming drift was disproved by the direct attribute
inventory: actual LocalPlayerBuildOnHoldEvaEvent and
LocalPlayerBuildCancelledEvaEvent names are declared in the EP1 schema. There is
no evidence supporting a rename. All authored top-level upgrade field names in
the inspected file are declared (including inherited id/inheritFrom).

## Official RA3 cross-check

The originally supplied nested official-schema path is not present on this host.
The available read-only reference is:
`D:\OneDrive\CNC Files\CnC_Modding_Support-main (Official XML, Schema, Script, Shader, Maps)\Red Alert 3\Schemas (RA3)`.

Its AssetTypeUpgradeTemplate.xsd also declares GameDependency 0..1. Its
GameDependency.xsd declares repeated RequiredObject/ForbiddenUpgrade/NeededUpgrade,
optional singleton ObjectFilter and the same four attribute names as EP1.
The inspected particle/type/cardinality and field-name inventories match EP1;
the files are not byte-identical, and this is not a full-schema equivalence claim.
This blocker is therefore not evidence that EP1 changed GameDependency to a list.

The existing KW-derived `SageBinaryData.UpgradeTemplate.GameDependency` is a
single GameDependencyType pointer, and the marshaler uses GetChildNode for that
field. This supports the existing singleton representation, but does not prove
EP1 offsets/layouts, type hashes or runtime upgrade behavior.

## Reproducible focused command

The Release/x86 inspector exposes:

```text
sdk-upgrade-semantics-self-test
sdk-upgrade-semantics-review <absolute-upgrade.xml>
```

Review accepts a bounded absolute existing XML file, prohibits external DTDs,
uses the pinned reviewed effective schema and requires exactly the known two
complementary source shapes and an empty local BasePurchasableUpgrade. It consumes
the local inheritance marker for the isolated experiment, invokes the unchanged
core and validates only the resulting owners. It does not follow Includes,
prepare the whole source library, resolve game-object references or write files.

Input allocation is capped through an opened stream at 4 MiB; source length/hash
are rechecked before evidence is returned. The JSON explicitly reports ReadOnly,
PartialOwnerReview and ProductionBuildReady=false. CLI refuses missing arguments,
relative paths and nonexistent input (exit 1). Successful review exit 0 means
only the focused experiment completed, not that the source graph is build-ready.

Real source:
`D:\OneDrive\CNC Files\CnC_Modding_Support-main (Official XML, Schema, Script, Shader, Maps)\Uprising\Xml (Uprising)\GlobalData\Upgrade.xml`

Raw SHA-256:
`A6083FCC3E990CC31EDBA7B4EE7D08C914181C7B2236C25AC7FB8691C2B47206`

| Owner | Raw branches | Merged branches | Raw valid | Merged valid |
| --- | --- | --- | --- | --- |
| Upgrade_AlliedTech2 | 2 | 1 | false | true |
| Upgrade_AlliedTech3 | 2 | 1 | false | true |

Isolated processed document SHA-256 witnesses (not whole-library/native hashes):

- AlliedTech2: `D9E78E2FDF6395A65C0F7232653496B26FDE62FEDCF14EC13164D3F510FD331A`
- AlliedTech3: `18292B0EEF817DB7E22D98EAB67B6E857F4E495AA5E1B615FEE8D3B627F51E1F`

The focused review was repeated after strengthening bounded input reads and
retained identical hashes and results.

## Next implementation gate

Add a separate, narrow singleton-normalization scope only after proving all of:

1. Named UpgradeTemplate/GameDependencyType with the reviewed singleton shape.
2. Anonymous direct complementary branches, disjoint attributes and bounded
   literal weak-reference leaves; no conflicting fields, IDs, directives,
   text concatenation, nested ObjectFilter merging or unknown fields.
3. Pre-allocation source/tree/amplification bounds before core folding.
4. Actual ordered leaves and attribute union equal the prediction after folding.
5. The entire prepared Upgrade owner document validates, while earlier profiles
   still refuse it; source hashes and failure evidence stay atomic.

Imported instance provenance, final field binding, native requirement hashes,
dispatch and in-game upgrade behavior remain separate gates. Do not infer that
the partial successful review raises the whole-library validation count.

Registered compiler groups become 135. Model/marshaller coverage stays 785/1390
and 762/1390; EP1-only coverage stays 48/48 for both. Manual weighted effort stays
**51% complete / 49% remaining**. This characterization removes uncertainty
about one blocker; it does not by itself expand production support.

Validation: the focused group, all 135 compiler groups, 33 enum checks, three
Include-classifier fixtures, three review CLI refusal probes and the repeated
real owner review passed. The unchanged graph was re-audited at 387 / 9 with
Upgrade still RequiresPreprocessing, expected exit 2 and no output directory.
Final Release/x86 build has zero warnings/errors; git diff whitespace checks pass.
