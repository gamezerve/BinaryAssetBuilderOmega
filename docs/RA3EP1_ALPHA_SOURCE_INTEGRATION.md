# Experimental armor source integration — October 13, 2026

The general armor source bridge now participates in the existing EA Studio
driver's single build step. The old fixed armor and AK47 routes keep their
admission/golden bytes; unsupported fixed input is not retried as generic XML.
The preserved g workspace was not changed.

## Implementation

- `scripts/Invoke-Ra3Ep1AlphaBuild.ps1` selects the source route for an
  AssetDeclaration with metadata, custom armor IDs or more than two armors.
  `-ArmorSource` explicitly selects it for legacy-shaped declarations.
  Includes resolve from the original selected mod data directory, never the
  generated snapshot directory. ALPHA004 describes the experimental boundary.
- `GeneralArmorSourceBuild.Run(..., validateOnly)` and the new
  `armor-source-preflight` CLI prepare/schema-check the entire confined source
  graph without creating native output. The source environment uses focused
  path geometry, not a false full-SDK schema-catalog parity requirement.
- Admission and actual build must agree on every original source SHA-256 and
  prepared digest. The driver also checks native/package receipts and the
  selected entry hash before publishing `alpha-build.json` with
  `Profile=armor-source`. Windows read sharing locks hold sources through each
  source build. No installation, registry lookup or game launch occurs.
- `alpha/mods/ArmorSource/data/{mod,Base,Defs}.xml` demonstrates a prepared
  imported base, definition-only all leaf, GUN substitution and two owner armors.
  Imported `RebornSourceBase` is not emitted. `RebornNewArmor` has no consumer.
- The workspace adds only these three samples and the focused
  `ArmorSourcePipeline.xsd`; it does not require `CnC3Types.xsd` or the repo
  source tree. Existing official armor/weapon dependencies remain external.

## Scope

The route supports 1..32 owner armors, unique engine IDs across the captured
closure, 0..64 unique non-ALL damage entries per armor, optional scalar defaults,
bounded decimal/% literals, reviewed exact `=$NAME` fields/definition forms,
confined direct instance bases and local/imported inheritance. See
[source bridge](RA3EP1_ARMOR_SOURCE_BRIDGE.md) and [alpha README](../alpha/README.md).

Full EA list replacement/merge semantics, same-ID source shadowing, arbitrary
expressions/all Includes, reference Includes, definition overrides, nonempty
Tags, mixed streams, general weapons, GameObjects and art remain unsupported.
Validation-only may retain driver diagnostics but produces no BIG/final alpha
success receipt. Successful offline packages are not gameplay certificates.

## Local verification

- `artifacts/StudioSrc-a/batch-report.json`: **18/18 focused groups pass** in
  36.390 seconds excluding the compiler build. All eight runtime hashes stayed
  unchanged within that run. Full was not rerun; shared Core/native/profile
  implementations were not modified by this integration.
- `UprisingAlpha-20261013-h`: eight samples pass without a source tree, with
  seven old golden hashes unchanged. New source package SHA-256:
  `388042AE568089533DE454E69A6C03377C1DD0C0F909AE32CFB2E2A71D9100FF`.
  A plain three-custom-ID declaration also passes automatic routing. Source
  validation-only creates no package. Five source failures (missing Include,
  unknown definition, case-colliding ID, mixed source and unsupported arithmetic)
  produce ALPHA100/ALPHA101 without a final success receipt. Existing DTD/weapon
  refusal controls remain covered; all inventory input hashes stay unchanged.
- `UprisingAlpha-20261013-hx`: fresh LOCAL-USE-ONLY ZIP extraction repeats the
  eight-sample/refusal checks. ZIP SHA-256:
  `80E83D6E23BAA001C5D0EB4967B0FFC9890EB0428752676903689D6430944F30`.
- Exact two-reference CLR4 CodeDOM/interface/config/options harness passes.
  It does not open a Studio window or invoke build callbacks; actual h GUI
  observation is pending. The initial compiler invocation used a slash-form
  relative input rejected by the legacy compiler; the absolute Windows path
  invocation compiled and ran successfully.
- `UprisingAlpha-20261013-ho-OVERLAY-CANDIDATE.zip`: 37 project/notice files,
  24 external dependencies; no EA executable/schema payload or absolute
  preparation provenance. SHA-256:
  `97A30B059D4D4B8C3AEF8F3D42EEE4BB3E42E45539ED1C23D135EB3D8951C2A3`.
  Clean setup/build at `UprisingAlpha-20261013-hs`, dependency digest mismatch
  refusal and occupied-setup preservation pass. Public redistribution and
  exact apphost notice coverage remain unreviewed.
- `UprisingAlpha-20261013-hr`: relocated configured overlay repeats all eight
  sample/refusal checks. Internal paths are repaired with an exact config
  backup; original configuration and existing output sentinel stay unchanged.
  Missing dependencies and duplicate keys are refused without mutation.

The next user-facing test is to open the h workspace's `EALAModStudio.exe`,
select **ArmorSource**, and build. Verify ALPHA004 and the final source receipt.
Any gameplay test requires separate installation/launch preparation and explicit
authorization. New-ID gameplay needs a GameObject consumer, not merely an armor
declaration. No game or installed files were touched in this milestone.
