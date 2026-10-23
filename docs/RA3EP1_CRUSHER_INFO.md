# CrusherInfo and full refinery compilation

<!-- Reborn: document real source/native/package milestones separately from runtime or unrestricted GameObject coverage. -->
`CrusherInfo` no longer has to match the captured Conscript. Existing EP1
scalar attributes, packed booleans, percentages, timing and the typed CrushWeapon
reference follow source values. The optional whole record may be absent or
authored empty/defaulted.

CrusherLevel and CrushableLevel use the official signed `xs:byte` range
(-128..127). The independent native proof now has an actual SByte codec instead
of treating a signed byte as a float or unsigned byte. Official `pipelineOnly`
attributes, including CrusherInfo/id, remain available for XML inheritance but
are not required to have runtime native fields. This exemption requires the
resolved schema annotation; it does not ignore arbitrary unknown attributes.

Stock Conscript and SovietRefinery independently qualify the native record and
metadata, including the inherited Soviet building CrushableLevel=40.
Populated ExtraCrushLevels and ExtraCrushKillDelays remain explicitly unsupported;
their nested native/metadata families still need qualification.

## Existing identities are overrides

The single-object build no longer refuses every stock GameObject identity except
Conscript. An explicitly authored stock name is an override. A different stock
name sharing the same type/instance hash still fails. Single- and multi-asset
package receipts distinguish StockOverride from NewIdentity, and primary-object
reference counts come from the actual primary manifest entry.

The Studio build adapter selects exactly one fresh override/new-id receipt
instead of assuming every non-Conscript name is new. No command is generated
implicitly; existing commands may continue referring to an overridden stock ID.

## Evidence — October 23, 2026

- Four crusher stream/package cases passed: inherited building values, defaults,
  omission and edited scalars/weapon/booleans. 180 native and 64 metadata faults
  were rejected. Evidence: `alpha/.work/crusher-batch-20261023-b`.
- Original Conscript passed separately (5 native/10 metadata faults):
  `alpha/.work/crusher-original-20261023-a`.
- Preparation of the user's Reborn completed using 15 source files and emitted
  one explicit `GameObject:SovietRefinery`:
  `alpha/.work/crusher-reborn-20261023-a/source-input.json`.
- That prepared snapshot passed the complete native/metadata/stream/BIG route:
  27 authored Behaviors, 124 external reference occurrences, metadata prefix
  46116. Fresh evidence: `alpha/.work/crusher-refinery-diagnostic-20261023-c/override`.
  The receipt has NewIdentity=false, StockOverride=true and OfflineValidated=true.
  Its archive SHA-256 equals the preceding independent diagnostic build:
  `D229F8661AFFD3DD8E761088540D2E52EC71FCA7A3CE54FC9DA527624B05CB8B`.
- The real Studio adapter script also passed source preparation, native/package
  compilation, override receipt selection and source/archive hash readback with
  `ValidateOnly`, using the prepared developer snapshot as input:
  `alpha/.work/crusher-studio-route-20261023-c`. This did not publish a Documents
  mod, modify the user's Reborn files or click/launch the Studio/game UI.

These checks do not prove gameplay. Developer packages are confined to `.work`;
the user's source files, published Documents mod files and game installation are
not changed. The normal user entry point remains `alpha/EALAModStudio.exe`.
This is a complete build of this refinery snapshot, not unrestricted support for
all GameObjects, every module family or ART/W3X resource compilation.

Implementation: `GameObjectExtendedRootContract.cs`, `SchemaNativeContract.cs`,
`ConscriptModuleScalarContract.cs`, `ConscriptVariableBehaviorBuild.cs`,
`GameObjectConscriptTrialSmokeTest.cs`, `ConscriptMultiAssetBuild.cs` and
`Invoke-Ra3Ep1ConscriptAlphaBuild.ps1`.
