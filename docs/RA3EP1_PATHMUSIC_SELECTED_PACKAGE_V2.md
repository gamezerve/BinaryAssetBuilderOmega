# Actual selected PathMusic package v2 — October 9, 2026

## Result and scope

Explicit experimental package v2 commands now serialize **actual native buffers
returned by the privately registered local plugin**, after unchanged Core owner
selection and dependency preparation. They do not replace those buffers with a
second snapshot-only compilation. Frozen native snapshot equality remains an
independent admission check before the actual buffers enter serialization.

The profile carries local TypeHash `48E303B8`, synthetic ProcessingHash domain
`52424D49`, current Core InstanceHash and ordered/padded Core checksum.
AllTypesHash remains zero. These are not recovered EA/EP1 metadata, a production
SDK build or a playable mod. Standard GenerateOutput=true/CompileInstance/
OutputManager, authentic AUDIO resolution, cache/reuse and game loading remain
closed or unproved. The [local type contract](RA3EP1_PATHMUSIC_SELECTED_COMPILER_V2.md)
documents the non-EA TypeHash recipe and actual Core dependency stage.

## Exact profile and compatibility

```text
diagnostic.manifest
diagnostic.bin
diagnostic.relo
diagnostic.imp
EXPERIMENTAL_CORE_V2.txt
```

Profile v2 is a local policy version, not a new manifest format: the container
remains linked little-endian v7. The marker declares the local/synthetic domains,
actual selected plugin payload origin, limitations, and exact source/header
SHA-256 plus diagnostic fingerprint. A root-level comment may preserve owner
InstanceHashes; raw fingerprint binding still rejects an old package after such
an input change.

V1 experimental and legacy diagnostic commands retain their existing identities,
markers and serialized bytes. Exact file sets, notice bytes and private hash
policies prevent cross-profile acceptance. Native BIN/RELO/IMP payload tails are
equal between v1 and v2 for the same input; type/checksum fields and marker differ.
The marker is neither historical publication authentication nor a game safety gate.

## Implementation and safe publication

`source/BinaryAssetBuilder.ManifestInspector/PathMusicControlledCompiler.cs`:

- `CompileSelectedPayload` captures freshly compiled selected plugin AssetBuffers
  only after all native, identity and checksum checks. Each array is cloned before
  the private compiler lifetime ends; successful capture is followed by another
  current snapshot check and settings restoration.
- Payload/Report objects returned internally are detached evidence, not an input
  authority accepted by the package publisher.

`PathMusicCorePreparation.SelectedPayload` brackets that fresh compile with the
original preparation's raw/Core checks. `PublishSelectedPackage` and
`VerifySelectedPackage` use privately owned native snapshots and preparation;
there is no caller-supplied report/hash/payload overload.

`PathMusicPackageProbe.Selected` privately pairs the fresh payload with captured
processing/document/profile flags, exact ordered name/ID/InstanceHash, native
lengths/SHA, zero imports and raw input fingerprint agreement. It clones actual
chunks into a private policy. Shared framing consumes those chunks directly,
while older policies continue to use their prior snapshot compilation.

Verification regenerates a fresh actual selected payload, checks all five expected
files byte-for-byte, then independently reads metadata/offsets through
ManifestReader and Utility.Manifest. Manual native decoding checks event/cache,
padding, exact local weak IDs, pointer relocation offset 8 and linked framing.
Both readers require the selected local TypeHash and expected checksum.

Publication derives selected payload evidence before staging, checks captured
current raw/Core inputs before staging and immediately before no-overwrite rename,
and verifies staged bytes against the private captured policy with both readers.
The final freshness gate reprocesses Core identity; **it does not rerun selected
native compilation**. Independent public verification does rerun that compilation.
This is captured selected evidence tied to unchanged raw inputs, not an attested
historical Core transaction or atomic filesystem lock. ABA/reparse/concurrent
staging-write guarantees are not claimed.

Failed owned staging is retained and its exact path reported. Existing directories
or files are never replaced. Internal fault callbacks remain owned-test-only;
CLI exposes no callback or guard bypass. No official source/schema, registry,
environment setting, codec or reference DLL is modified/executed.

## Validation and reproduction

Use the x86 Release inspector from the repository root:

```text
pathmusic-experimental-core-v2-package tests/fixtures/PathMusicAuthoredProbe <new-package-directory>
pathmusic-experimental-core-v2-verify tests/fixtures/PathMusicAuthoredProbe <existing-package-directory>
pathmusic-selected-package-v2-self-test
compiler-self-test
```

The output parent must exist and destination must not. Two-event CLI fixture:
manifest 240 bytes, BIN/RELO/IMP 44/16/8 bytes, marker 682 bytes; local TypeHash
`48E303B8`, checksum `117BE305`, InstanceHashes `9CE92A17`/`BA9DF8C5`.
Package/verify and older v1 package creation exit 0. Both v1/v2 cross-profile
directions, existing output and missing arguments exit 1. Demo output is retained
under ignored `artifacts/Reborn-SelectedMusicV2-CLI-fe19150bf3be4e5c8042471217e7d4b5`.

Focused tests cover 1/3/8 actual selected payload packages, independent identity
table/checksum expectations, two-reader/manual native readback, weak self/cycles,
repeat bytes/v1 isolation/native equality; detached payload/report mutation;
corruption of every file, missing/extra members, stale-before-staging and explicit
refresh rejection, raw-comment provenance, late input faults, competing directory,
existing file preservation, restored recovery and retained failure staging.

All 165 compiler groups pass. Inventory remains 785/1,390 models, 762/1,390
marshallers and 48/48 EP1-only coverage. Engineering estimate remains
**52% complete / 48% remaining**; local selected publication does not establish
authentic EP1 processing/type-table, official AUDIO or playable game compatibility.

Next: investigate authentic EP1 processing/type-table provenance and unresolved
AUDIO/header dependencies against the local official/reference/stock evidence.
The local proof now reaches selected native package publication/readback; further
experimental version increments alone would not resolve the production blockers.
