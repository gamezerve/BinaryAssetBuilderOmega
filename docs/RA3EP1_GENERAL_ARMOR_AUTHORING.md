# General armor authoring backend

This is the first broader type-based authoring path, not another fixed unit
recipe. `GeneralArmorAuthoredBuild.cs` reuses the existing EP1 ArmorTemplate
plugin, variable-length tokenizer, actual DocumentProcessor and single-document
experimental OutputManager capability. A separate bounded set capability now
freezes every selected armor; shared native layout and ordinary production
output policy are unchanged. The existing exported g alpha retains its
fixed-yard admission until a separate adapter/export milestone.

## Admitted surface

The historical command accepts one standalone ArmorTemplate or one member of
AssetDeclaration. The explicit set command accepts 1..32 ArmorTemplate members
in one AssetDeclaration (or a standalone armor). Duplicate names and duplicate
case-insensitive engine lookup IDs are refused before creating output. This
includes case-only name differences; hash comparison uses Core's InstanceHandle
API, not a replacement hashing algorithm. The 32-member limit is an experimental
resource bound, not the final SDK design limit.
User-selected IDs follow `[A-Za-z_][A-Za-z0-9_.-]{0,127}`; they are no longer
restricted to AlliedConYardArmor/SovietConYardArmor in this backend. XML is
bounded to 64 KiB. Zero through 64 leaf Armor entries are allowed, preserving
source order. Each requires explicit Damage and Percent; Damage must be a valid
official enum, unique within the armor and not ALL. The ALL behavior and duplicate
damage precedence remain unproven and deliberately refused.

Five root scalars are optional: Default, DamageScalar, SideDamageScalar,
RearDamageScalar and FlankedPenalty. Official EP1 schema defaults are inserted.
Scalar/entry percentages accept unsigned finite literals 0..10000, including
decimal values and an optional `%` suffix. EA percentage normalization applies:
`75.5` and `75.5%` both compile as `75.5 * 0.01f`. This is not a ratio-literal
syntax where 0.755 represents 75.5 percent.

Inheritance, definitions, formulas, includes, mixed asset types and other types
are still refused rather than ignored. DTDs, processing instructions, unexpected
attributes/children and injected TypeId are refused. This is bounded general
armor authoring, not unrestricted full SDK XML semantics.

## Output and validation

`armor-general-authored-build <one-armor.xml> <new-output-directory>` holds the
original source read-only and creates one owned canonical document. It loads
that document through real Core and binds its real asset name, instance ID/hash,
TypeId 3A6C5E8E and TypeHash A0E237D8. Native values, enums, count/pointer,
every token/terminator and relocation locations are checked against authored
schema-normalized input. Expected BIN length is `128 + 56 * entry-count`;
RELO is empty for zero entries and 8 bytes otherwise; IMP is empty.

The existing scope freezes identity/source/XML and compiled bytes before
intermediate .asset, manifest, BIN/RELO/IMP publication. Source edits, XML identity
edits and equal-length committed payload corruption are refused and restored
only in owned test files. Two readers validate the linked output. The existing
diagnostic BIG4 readback/corruption tests are preserved.

After those checks, a separate default-reader candidate contains fifteen exact
entries in the mod/mod_l/mod_m families, with blank CRLF version markers. The
existing framing serializer is reused; no armor-trial fixed-yard admission is
bypassed or widened. Exact directory/ranges/full bytes and config/SKU fragments
are independently checked. Candidate payload/config/SKU corruptions are refused.
Output is `package/RebornGeneralArmor.big`, `general_1.0.cfg` and
`general_1.0.skudef`; the latter two contain only `add-big RebornGeneralArmor.big`.
Config paths must fit the existing conservative 240-character printable ASCII
bound. No installation, launch or replacement stock SKU is produced.

The final `general-armor-build.json` distinguishes the diagnostic archive from
the default-reader candidate and records source/package hashes, actual Core
identity and validation evidence. Failed partial outputs are retained without a
success receipt; existing output directories are refused. Filesystem checks
reject ordinary redirected output ancestry, not hostile concurrent path races.

New armor IDs alone do not visibly affect gameplay: a GameObject must reference
them. This milestone does not establish GameObject compilation or runtime
consumption of newly created armor identities. The default-reader candidate is
offline verified, not a gameplay certification.
Stock-name/hash collision inventory is not yet part of this standalone backend;
it remains a gate for general asset/reference publication.

## Regression batch

`armor-general-authored-self-test <new-output-directory>` runs five inputs and
their repeats: two different empty-armor identities, one-entry decimal/scalar
input, two-entry reordered damage kinds and a thirteen-entry armor under a new
ID. Diagnostic/default-reader packages repeat byte-identically; changed identity
or source shapes produce different packages. Original source remains unchanged,
occupied outputs are refused and eleven unsupported-input controls pass.

## Multiple armors in one stream

`armor-general-set-build <armor-declaration.xml> <new-output-directory>` uses
the same schema admission and native/token validation per member, then the
separate `CreateExperimentalGeneralArmorSetDiagnostic` capability. Each original
instance, XML, source and compiler buffer is frozen independently. Historical
single/fixed-pair factories remain restricted. One manifest and one linked
BIN/RELO/IMP family contain all members, not concatenated standalone packages.

A minimal authored set (the source file is read-only during the build):

```xml
<!-- Reborn: independently named armors share one declaration and one compiled stream. -->
<AssetDeclaration xmlns="uri:ea.com:eala:asset">
  <ArmorTemplate id="MyGunArmor" Default="100">
    <Armor Damage="GUN" Percent="250.5" />
  </ArmorTemplate>
  <ArmorTemplate id="MyCannonArmor">
    <Armor Damage="CANNON" Percent="25%" />
  </ArmorTemplate>
</AssetDeclaration>
```

Run the ManifestInspector command with the authored XML and a fresh output
directory. This produces an offline candidate; it neither installs nor starts
the game and does not add these identities to a GameObject automatically.

Both readers check every identity. Full payload slices are verified in manifest
order at cumulative offsets, including stream magic/checksum and exact EOF.
The checksum is independently reconstructed from serialized identity rows with
Core's capacity padding, including the 256/512/1024-byte table transitions.
Empty and nonempty armor lists may coexist. The final member's identity and
committed payload are corrupted deliberately to prove scope traversal extends
beyond the former two-member case. Source corruption controls also remain.
Foreign same-handle objects and incomplete selections are refused before manifest
publication. Payload routing validates the entire set once per public scope
entry rather than repeatedly revalidating suffixes while locating a member.
The set receipt adds ArmorCount and ArmorIds; PackagePath is the default-reader
candidate for a set, whereas the single command retains its separate BIG4
diagnostic archive. No new stock collision inventory or gameplay claim is made.

`armor-general-set-self-test <new-output-directory>` exercises 1, 2, 3, 13 and 32
members, including both a custom pair and the historical Allied/Soviet pair,
with mixed list shapes and byte-identical repeat packages. Eight
pre-output refusals cover empty/oversized sets, duplicate/case-colliding names,
invalid final-member percent, includes, inheritance and DTD input. Occupied
outputs and historical single/pair capability isolation are also checked.

The groups are included in Test-Ra3Ep1Batch.ps1: 17 focused groups and 19 in
Full mode. Existing fixed armor/weapon checks remain independent regressions.
Broader gameplay/alpha integration is not implied by diagnostic source-semantic
groups passing in the same matrix.

Earlier single-armor verification on October 13: incremental Release/x86 build passes with
zero warnings/errors; the complete focused matrix passes **16/16** in 26.407
seconds excluding its 4.69-second compiler build. All eight compiler/runtime
file hashes remain unchanged. Final local evidence:
`artifacts/BatchGeneralFinal-fcec0ef88115499c84485cc1acd23d2c/batch-report.json`
and `armor-general-authoring.log`, with the general armor receipts under its
`general-armors` child. The broad Full suite was not rerun for this wrapper;
shared native/Core/tokenizer layouts were unchanged. No game was executed.

## Set milestone verification — October 13

The initial expanded Full run passed **19/19** groups in 585.18 seconds,
including the broad compiler and authored armor field regressions. Evidence:
`artifacts/BatchArmorSets-20261013-a/batch-report.json`. Its multi-armor group
used the initial 3/32-member cases before the routing optimization and final
checksum/boundary controls; do not treat it as a Full rerun of final code.

Final code passed **17/17** focused groups in **32.522 seconds**, with a
27.08-second Release/x86 compiler build (zero errors, 54 analyzer warnings;
no GeneralArmorAuthoredBuild nullable warning). The expanded set group passed
in **14.956 seconds**: six inputs and repeats, independent checksum tables,
all-member identity/slice checks, final-member corruption/foreign/incomplete
selection refusals, capability isolation and eight pre-output refusals.
All eight compiler/runtime hashes stayed unchanged during both batch runs.
Final evidence: `artifacts/BatchArmorSets-20261013-final/batch-report.json`,
`armor-general-sets.log` and the receipts under `armor-sets`.
The broad compiler was not rerun after final scope-routing optimization;
the final focused matrix reran single/pair scopes and armor/weapon regressions.
No game, installation or exported alpha capability was changed.
