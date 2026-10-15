# Root movement and combat composition

October 15, 2026. `LocomotorSet` now joins the selected root/armor/body/SMC/
destruction/weapon native/token graph. This is an offline partial-object milestone,
not unrestricted GameObject authoring or an alpha capability expansion.

## Independent stock contract

The root count/items fields are at +428/+432. Inline records are twelve bytes:
Speed float at +0, Condition enum at +4, Locomotor strong reference at +8.
The list token is `6E6BF482`; member types are `645864F3`, `845D12B2`,
`FED20C6C`. A count header, N markers and N three-field/terminator tables require
`12 + 60*N` metadata bytes. The writer generates these; it copies no stock prefix.

Four independent slices establish both fields and engine condition values:

| Stock GameObject | Speeds | Conditions (native values) |
| --- | --- | --- |
| SovietAntiInfantryInfantry | 50 | NORMAL (0) |
| SovietAntiNavyShipTech2 | 75 / 75 / 75 | NORMAL (0) / ATTACKING (12) / EXITING_PRODUCTION_STRUCTURE (17) |
| AlliedArtilleryVehicle | 75 / 75 / 0 | NORMAL (0) / NORMAL (0) / SPECIALABILITY_ACTIVE (18) |
| JapanFortressShip | 40 / 30 | NORMAL (0) / SUPERSONIC (6) |

Each stock reference resolves to a unique LocomotorTemplate in the stock catalog.
Isolated captured-index native marshalling reproduces the complete movement record
arrays. These native comparisons are distinct from the actual-Core named source
tests below; they are not whole-stock-object equality.

`Ep1RootLocomotorSetType` uses INVALID=-1 and the twenty official names. Neither
the legacy shared enum (with COMBO2/COMBO3) nor the separate toggle-state encoding
is changed. The scoped root model preserves the twelve-byte ABI. Missing optional
attributes stay native zero; notably omitted Condition is NORMAL, not INVALID.

## Shared source and ownership proof

`gameobject-movement-composition-self-test <static> <global>` validates raw source
against the reviewed official candidate, then normalizes named references through
actual Core using a focused schema whose attribute types/defaults/pipeline
annotations and enum inventory are checked against that candidate. Both target
catalogs must be valid linked EP1 metadata; every reference must resolve uniquely.

The 69 cases cover 0/1/3/32 movement records, six selected behavior orders,
absent/present long root strings, all twenty condition literals and omitted
movement attributes. Armor, Body/ranges, weapon/upgrade and root references share
the same native graph and occurrence table. Repeated BIN/RELO/IMP output must be
identical. The independent verifier checks movement metadata, values, real target
identity/one-biased index and complete native/auxiliary round trips.

The owner decoder requires aligned, bounded, disjoint movement/armor/string
storage before Behaviors, exact root relocation/import ownership, finite Speed,
and condition -1..18. The 32-record ceiling is an explicit diagnostic profile
bound, not an assertion about the engine limit. Fifteen input controls include
bad bounds, aliasing, missing auxiliary ownership, nonfinite speed, illegal enum,
unknown target and malformed scalar. Sixteen detached metadata corruptions refuse.

Present movement adds one root token: 90 reviewed fields when ArmorSet and Body
are also present; absent movement retains the previous 89-field output. This is
still not the complete stock GameObject token inventory.

## Scope and files

- `GameObject/Ep1LocomotorSet.cs`, `GameObject/Ep1RootEnums.cs`: scoped native data.
- `Marshaler.GameObject.cs`: separate root movement marshalling overload.
- `Ep1GameObjectStringTokens.cs`: disjoint allocation/import ownership.
- `Ep1GameObjectDestructionProjection.cs`: generated movement metadata.
- `GameObjectMovementCompositionSmokeTest.cs`, `RootMovementPipeline.xsd`:
  stock/source/composition/corruption gates; one new Focused batch group.

LocomotorTemplate payload compilation, movement/AI execution, geometry/draw,
remaining root containers/modules, general source semantics, complete object
publication, package selection and gameplay remain separate unfinished gates.
No game or installed files are touched. Existing alpha admission remains unchanged.

## Final regression

`artifacts/GameObjectMovement-a/batch-report.json`: **42/42 Focused groups pass in
116.493 seconds**, with eight unchanged compiler/runtime hashes. GameExecuted and
InstalledFilesModified are false. The final standalone build has zero warnings
and errors. The Full suite and in-game movement were not tested in this milestone.
