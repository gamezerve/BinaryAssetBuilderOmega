# Editable armor input admission

## Supported input

This is the first input stage toward the [Armor Preview](RA3EP1_SDK_RELEASE_PLAN.md),
not an editable mod builder yet. `ArmorAuthoringAdmission` accepts one
standalone `ArmorTemplate` in `uri:ea.com:eala:asset`, with the existing
`AlliedConYardArmor` identity and exactly thirteen ordered flattened entries.
Only the `GUN` entry's `Percent` is editable: canonical integers 0..10000.
For example, 250 compiles to multiplier 2.5; it is not a percentage increase
of 250 on top of the stock value.

Input is bounded to sixteen KiB before allocation. DTDs/entity resolution,
includes, inheritance, formulas, new types/identities, changed non-GUN entries,
unknown attributes, duplicates, changed ordering and non-integer percentages
are refused. Comments/formatting are discarded when constructing the canonical
snapshot. Ordinary reparse ancestry and existing output destinations are refused.
This is trusted-local authoring admission, not a hostile concurrent-filesystem
security boundary.

Both the admitted input and a GUN-100 baseline pass through the existing
schema/token compiler. All BIN bytes outside offset 788's four-byte float
must match; full RELO bytes must match and IMP must remain empty. A successful
preflight writes only `admitted-armor.xml` and `admission.json`, with input and
snapshot hashes. It does **not** emit a manifest, linked streams, BIG or CFG,
nor does it reuse the earlier runtime validation for changed values.

Example under PowerShell 7, from the repository directory after a build:

```powershell
# Reborn: stage the editable 250-percent example only; no linked-stream publication or game launch.
& .\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe armor-authoring-preflight .\samples\armor\AlliedConYardArmor.xml .\artifacts\MyArmorAdmission
```

Choose a fresh output directory for every run. The fixed
`Build-Ra3Ep1ArmorMod.ps1` does not accept this snapshot yet. Next work binds it
to fresh Core instance identities, the actual scoped OutputManager and a
parameterized default-reader package. No existing production/cache policy
was relaxed for this input stage.

## Validation — October 11, 2026

Build passed with zero errors; existing warnings remain. The new self-test
passed five values (0, 100, 250, 5000, 10000) and seventeen deliberate refusals.
The fixed-5000 input staged successfully at `RebornArmorAdmission-20261011-a`.
The GUN-250 sample staged at `RebornArmorAdmission-20261011-b`; its occupied
output control refused and preserved the existing snapshot hash. The complete
fixed mod build regression passed at `RebornArmorDefault-aeab222fa60f4dc2b1c2198736a8d100`,
with five fresh PoC steps and package controls; its digest still matches the
user-tested package exactly. The full compiler suite was not rerun for this additive command.
No game, codec or large WorldBuilder dump is part of admission.
