# Immutable Core/native PathMusic preparation — October 9, 2026

## Result and scope

Local music preparation now privately binds actual independently verified Core
identity to the **same captured authored source/header/native closure**. A stale
preparation cannot silently acquire a newly calculated Core hash or serialize
current inputs under an old identity. Both identity and raw snapshots are checked
before and after detached native serialization.

This uses the explicitly synthetic music processing domain `52424D49`, current
Core document version 23, and the existing 1–8-owner direct local profile. It is
**not** a recovered EP1 ProcessingHash, production processor, cache entry, package
identity relabeling, official AUDIO resolution or playable mod. Production flags
remain false. The RA3 reference constants recovered in the preceding milestone
are intentionally not substituted into this preparation.

## Implementation

`source/BinaryAssetBuilder.ManifestInspector/PathMusicCorePreparation.cs`:

- `Read`: admit a strict local source/header snapshot through the reviewed schema;
  independently process fresh actual Core instances; require identical raw source
  and header SHA-256 values, diagnostic fingerprint, ordered owner names/IDs,
  cache semantics and weak-reference counts. Each actual Core hash must equal the
  independent expected hash. Reject mismatched snapshot domains before retaining
  preparation. Native and Core admissions stay separate checks on the same closure.
- Constructor: privately retain the snapshot and detached immutable Core row
  values. No mutable InstanceDeclaration, XmlDocument or exposed schema set escapes.
- `Preflight`: create new paired row arrays carrying CoreInstanceHash together
  with event/cache/alternate values and complete native BIN/RELO hashes/sizes.
  Report edits cannot mutate private preparation.
- `VerifyCurrent`: verify raw snapshots, reprocess fresh actual Core instances with
  a fresh cache, compare every captured row including dependency/default/physical
  path projections and processing/document domain, then verify raw snapshots again.
- `Compile`: VerifyCurrent before and after the existing snapshot serializer;
  return newly detached native buffers only. No implicit refresh, output directory,
  plugin ProcessInstance, cache publication or OutputManager commit is added.

Existing `PathMusicCoreIdentity` checks normalized DOM shape, typed weak handles,
one resolved logical header, independent XML/default/512-block and padded file/type
hashing, and preserves production/cache/reuse refusal in its private hash-only plugin.
Existing `PathMusicAuthoredSnapshot` checks strict literal/header/native identity.
The new object composes these checks rather than weakening either profile.

## Important invalidation cases

Same-size edits with restored timestamps still refuse on an old object. Fresh
preparation admits valid edited inputs and produces current event/cache fields and
Core hashes; exact original input restoration permits original preparation again.

A header comment can change Core file identity without changing native BIN/RELO.
Likewise explicit true and schema-default true cache attributes have identical
native bytes but different authored XML hash provenance. Therefore checking only
native bytes is insufficient to establish that a Core/native binding is current.
Both distinctions are tested explicitly.

The object does not accept arbitrary caller-supplied Core reports, native buffers,
or processing seeds. This proves ownership and current-input pairing within the
local experimental profile, not atomic filesystem locking, adversarial ABA/race
resistance, generalized source inheritance or a production processing contract.

## Commands and validation

From the repository root, invoke the x86 Release inspector:

```text
pathmusic-core-preflight tests/fixtures/PathMusicAuthoredProbe
pathmusic-core-preparation-self-test
compiler-self-test
```

Preflight prepares, compiles in memory, rechecks current identity and emits JSON
only. The checked-in two-event fixture returns exit 0 with repeated identical
reports: Core hashes `9CE92A17`/`BA9DF8C5`, native sizes 20/8/0 and 16/0/0.
Missing arguments return exit 1. No package files are created by this command.

Compiler group **159** exercises 1/3/8 ordered Core/native closures, repeated
preparation, detached report/native arrays, preserved-timestamp source/header
edits, stale VerifyCurrent/Compile refusal, refreshed identity/native values,
native-equal header comments/default provenance, missing header refusal and exact
restoration. Inputs are owned temporary fixtures, never official AUDIO files.
Focused tests and all **159 compiler groups pass**. Initial dependency rebuild
reports three existing warnings and zero errors; incremental x86 Release build
has zero warnings/errors. No native codec or reference DLL is executed.

Model/marshaller coverage remains 785/1,390 and 762/1,390 (EP1-only 48/48).
Last expanded official graph remains 398 valid XML / four AUDIO issues / 198 header
occurrences. No official source/schema/Core/registry changes or codec execution.
Engineering estimate remains **52% complete / 48% remaining**.

## Next gate

Apply this fresh immutable binding as an explicit gate on experimental music
publication, preserving a clearly declared identity domain and independent package
readback. Existing diagnostic packages must not silently change their hash policy.
Then establish the EP1 processing/type-table and authentic dependency or explicit
stock-reference contract before production registration. In-game loading remains
unproved; no static/native/Core/package proof individually replaces that gate.
