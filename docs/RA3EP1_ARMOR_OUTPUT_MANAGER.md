# Reborn: actual armor intermediate commit and linked output

Date: October 11, 2026. Offline diagnostic milestone, not a playable mod.

## Implemented path

`Ep1ArmorDocumentSmokeTest.WriteManagedStream` now exercises:

1. The schema-loaded populated ArmorTemplate and actual armor plugin payload.
2. `OutputManager.CreateExperimentalArmorDiagnostic`, an explicit opt-in
   factory that creates a fresh `RebornArmorOutput-<GUID>` child directory.
3. `GetBinaryAsset` and `AssetDeclarationDocument.PrepareOutputInstances`,
   including the real `BinaryAsset.Commit` / `CommitFromMemory` intermediate
   `.asset` write and the existing padded output checksum calculation.
4. The existing `OutputManager.CommitManifest` and `LinkStream` writers.
5. Independent ManifestReader and Utility.Manifest readback plus exact linked
   header/checksum/length/payload checks.

This is no longer only a separate diagnostic stream serializer. The ordinary
constructor and production document path remain blocked for the experimental
armor profile. No plugin production/cache/reuse eligibility flag is changed.

## Scope and limitations

`ExperimentalArmorOutputScope` is private per manager. It captures one document,
one instance, normalized XML, source bytes, identity fields, logical source,
plugin/settings references and independently recompiled payload bytes. It
requires Win32, linked little-endian output, no cache, postfix, Includes,
inheritance, external manifests, custom data or asset dependencies. Source
reads are bounded to one MiB. Armor fingerprint and profile metadata must match
the existing reviewed experimental registration.

Every commit checks the exact in-memory compiler result. Manifest/link writes
recheck source/profile/selection/checksum and the committed intermediate header
and payload, including equal-length tampering. Output is isolated from existing
stock/mod files. Redirected source/output ancestors are refused; this is a
trusted offline diagnostic API, not a security boundary against malicious
in-process mutation or concurrent filesystem replacement. The existing linker
reuse mechanism remains unchanged; independent readback is still required.

`.version` creation and broad cleanup are deliberately unavailable through the
scope. No `.cdata` is expected for this type; absence is not a missing texture
or audio processor. The intermediate `.asset` stores compiler chunks, while
the linked BIN/RELO/IMP files carry their respective v7 marker and checksum.

## Reproduction and evidence

Run `scripts/Invoke-Ra3Ep1ArmorPoc.ps1` under PowerShell 7. The existing five
steps now include this real commit/link path inside the document step.
`readiness.json` reports `OutputManagerCommitLinkValidated` and
`OutputManagerDirectories` separately from `CoreIdentityStreamsValidated`.
Passing test exit codes are required; mere artifact presence is not sufficient.

Local ignored reproduction: `RebornArmorPoc-eb2a49ca50d440eba3c33a2c60670107`.
All five steps passed; both diagnostic validation flags are true, and 28 stream
files were generated, plus one intermediate `.asset`. The actual writer uses
the populated case's current-Core InstanceHash `E6F463CC` and checksum
`45D90ADD`, with 744/8/0 BIN/RELO/IMP payload bytes.

Negative controls refuse changed in-memory payload, changed committed bytes,
changed XML/source/settings, missing document, cleanup and version publication.
The rejected intermediate cannot create a manifest. The existing standalone
linked-stream regression also passes, including intact reuse, 15 repair cases
and ordinary experimental-profile direct-write denials.
The full existing 165-group compiler suite completed with
`Uprising compiler self-test: OK`; layout and bounded diagnostic build
regressions also passed. No new test group or additional supported asset type
is counted for this extension of the existing armor document test.

## Remaining gate

Production output stays disabled. ProcessingHash is still a project-local
experimental revision; these correct local streams do not prove EA stock
identity compatibility. This does not establish that stock ProcessingHash
equality is mandatory for loading a newly authored mod: the runtime acceptance
contract must be determined, not assumed from compiler-cache identities.
Next resolve the one-type runtime identity/output
contract, then assemble a reversible package with independent readback and a
separately authorized game test. Config consumption and an authored in-game
change are still unproved. No game/debugger was run for this milestone.
