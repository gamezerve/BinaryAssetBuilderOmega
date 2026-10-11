# Editable armor input admission

## Supported input

This is the bounded input stage used by the editable alpha backend toward the
[Armor Preview](RA3EP1_SDK_RELEASE_PLAN.md). `ArmorAuthoringAdmission` accepts one
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
`Build-Ra3Ep1ArmorMod.ps1` remains a separate exact-recipe regression tool.
For editable builds, use `Invoke-Ra3Ep1AlphaBuild.ps1 -ModXml <input>` under
PowerShell 7. It also accepts an EA-style AssetDeclaration containing exactly
one supported armor. An optional `-StaticManifest` selects the unpacked stock
baseline; the development default is the local D:\TEMP static manifest.
The new `armor-authored-build <standalone-xml> <new-output-directory>
<ep1-static-manifest>` command binds the snapshot to fresh Core identities,
actual intermediate `.asset` commit, manifest/link output and a verified
15-entry default-reader BIG plus config/SKU fragments. It does not modify
installed files or launch a game. General production/cache policy stays closed.

`authored-build.json` records the actual coefficient, snapshot/package hashes,
stock comparison and negative controls. The earlier `admission.json` remains
an input-stage receipt, not proof that a mod was built; use the final build receipt.

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

The authored build regression now passes values 0, 100, 250, 5000 and 10000,
with fresh Core instance hashes, only the GUN float changing in BIN payloads,
identical repeated GUN-250 packaging, preserved original inputs, occupied-output
refusal, three candidate corruptions and mismatched source/package refusal.
GUN-5000 SHA256 remains
`DD7DE09E3A81B9714DC2861ADE0AA9ECDFE72A1851EDD1ED20BE2155EF31C2DC`.
Evidence: `artifacts/RebornAuthoredRegression-20261011-a`.
A complete selected-XML backend run with the GUN-250 sample passed at
`RebornAlphaInput-608b465d5c9d45f4a7ef5b663d282ff9`; its package SHA256 is
`E97EAEE9D2B423AC7360C9A401FAB1EC9A55B60F50B9F02BB74CEDC9E8B38F7E`.
These coefficients were validated offline, not newly tested in-game.

The existing complete `compiler-self-test` also finished successfully after
rerunning outside the sandbox: its initial owned temporary WAV rename was
denied by the environment. A parallel fixed-reproducer build encountered an
MSBuild apphost lock while that suite was still running; this was a test
scheduling issue, not a successful build or a compiler correctness result.
The fixed reproducer subsequently passed serially, including all five PoC
steps, static/WorldBuilder bounded comparisons and package controls, at
`RebornArmorDefault-af0b454f22214417a1b3e85a50428b6f`; its digest is unchanged.
