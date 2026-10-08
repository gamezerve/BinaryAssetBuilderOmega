# PathMusic experimental Core package profile v1 — October 9, 2026

## Result and identity policy

Explicit versioned package/verify commands now place actual independently verified
**synthetic-domain Core InstanceHash** into music manifest entries and the existing
Core ordered/padded output checksum into the manifest and linked stream headers.
The older diagnostic commands remain byte-compatible with their SHA-prefix policy;
no production build command or global processor registration is changed.

The profile is not a playable Uprising mod or stock identity recovery:

- ProcessingHash `52424D49` is synthetic, not EA's EP1 ProcessingHash.
- Document version 23 is the current Core version, pinned for this profile.
- TypeId `9A651D89` is observed; TypeHash and AllTypesHash remain zero.
- The manifest container still uses version 7 and linked little-endian streams.
  **Profile v1 is a diagnostic policy version, not a new manifest format version.**
- No official AUDIO resolution, codec execution, production/cache/processor
  admission, WorldBuilder integration or game-load proof is provided.

## Files and readback

The exact five-file set is:

```text
diagnostic.manifest
diagnostic.bin
diagnostic.relo
diagnostic.imp
EXPERIMENTAL_CORE_V1.txt
```

The distinct marker replaces the legacy `DIAGNOSTIC_ONLY.txt`. Both verifiers
require their exact file set and independently regenerate all expected bytes;
neither accepts the other's profile. Renaming a legacy marker alone does not
convert a package: notice and identity/checksum bytes must also match.

The experimental notice declares domain/version/limitations and binds source
SHA-256, header SHA-256 and ordered diagnostic fingerprint to current preparation.
This catches raw provenance changes even when owner Core hashes are unchanged,
for example a comment outside the individual PathMusicEvent XML nodes. It is not
cryptographic authentication or proof of who originally published the package.

ManifestReader and Utility.Manifest independently check metadata and offsets.
Manual native decoding checks event words, cache/padding, weak target IDs and local
closure, 16/20-byte native roots, RELO pointer offset 8 and no imports. Native
payload bytes are identical to the older diagnostic profile; only manifest
identity fields, framing checksum and profile notice differ.

Checksum remains the ordered five-word identity table with 256-byte zero-padded
capacity for the admitted 1–8 owners. Strong reference counts are zero; weak
alternate targets are not imports. See the [checksum contract](RA3EP1_PATHMUSIC_CORE_CHECKSUM.md).

## Implementation and publication safety

`source/BinaryAssetBuilder.ManifestInspector/PathMusicPackageProbe.cs`:

- Private `Policy` and `Experimental` derive identities/checksum only from
  current immutable Core preparation paired with the same native snapshot.
  No arbitrary caller-supplied hashes or preflight report are accepted as policy.
- Pairing requires raw source/header SHA, fingerprint and ordered name/ID agreement,
  with current Core checks before and after deriving the policy.
- Shared private Serialize/Verify/Publish overloads preserve the old null-policy
  path. Existing diagnostic regression fixtures remain unchanged.
- `PublishExperimental` supplies the mandatory current-Core gate to the staging
  publisher. Exact selected-profile readback precedes final raw/Core checks and
  a no-overwrite Directory.Move.

`PathMusicCorePreparation.PublishExperimentalPackage` and
`VerifyExperimentalPackage` use privately owned snapshots. The caller cannot
replace the Core freshness gate. Existing outputs are never overwritten; failed
owned staging is retained with its exact path reported. Internal test callbacks
are fault injection only, not exposed through CLI.

These are bounded snapshot checks, not an atomic filesystem transaction or an
adversarial ABA/reparse/concurrent staging-write guarantee. Publication provenance
is not authenticated. Zero hashes and a warning marker do not make game loading
safe; these files must not be presented as a production SDK release.

## Commands and measured validation

Use the x86 Release inspector from the repository root:

```text
pathmusic-experimental-core-v1-package tests/fixtures/PathMusicAuthoredProbe <new-package-directory>
pathmusic-experimental-core-v1-verify tests/fixtures/PathMusicAuthoredProbe <existing-package-directory>
pathmusic-experimental-core-v1-self-test
compiler-self-test
```

The output parent must exist and the destination must not. Checked-in synthetic
two-event CLI fixture: manifest 240 bytes; BIN/RELO/IMP 44/16/8 bytes; marker
600 bytes. InstanceHashes are `9CE92A17` and `BA9DF8C5`; checksum is `CF2CABDA`.
Package and same-profile verify exit 0. Both cross-profile directions, existing
output and missing arguments exit 1. Ignored demo output is retained under
`artifacts/Reborn-ExperimentalMusicV1-CLI-2e117c706dfc4238a73b77c40abae74a`.

Focused fixtures cover 1/3/8 owners, actual identities/checksum, repeated byte
equality, old diagnostic byte equality, native invariance, cross-profile refusal,
marker disguise, corruption of all five files, missing/extra members, stale input
before staging, explicit refresh, raw-comment provenance, restoration, late input
edits, competing directories, existing files and retained failure staging.

All 162 compiler groups pass. Model/marshaller inventory remains 785/1,390 and
762/1,390; EP1-only coverage remains 48/48. Engineering effort estimate remains
**52% complete / 48% remaining**, not a game compatibility percentage.

Next: a narrowly gated music processor integrated into a controlled Core build,
with the same native/identity/readback contract and unchanged production guards.
Authentic EP1 processing/type-table provenance, missing official AUDIO source
dependencies and in-game loading remain separate open gates.
