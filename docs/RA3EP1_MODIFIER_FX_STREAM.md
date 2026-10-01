# Fixed modifier / FX / external-audio stream proof

## Completed gate

`ModifierFXStreamSmokeTest.Run` now proves the checked modifier and FX processors
working together through actual Include parsing, recursive dependency preparation,
diagnostic serialization and independent stream readback. This is a fixed test
graph, not general FX command admission or a playable Uprising mod.

The parent modifier references two local FXList assets from an `instance` Include.
Those FX roots reference three external audio identities. The unused FX_NONE
root in the same Include is not forced into output. Stock audio is not compiled
or copied into the diagnostic stream.

| Serialized root | Direct dependencies | BIN / RELO / IMP bytes | Linked BIN offset |
|---|---|---|---|
| FX_DebrisHitGround | external AudioEvent | 80 / 12 / 8 | 8 |
| FX_ALL_AntiGroundAircraft_VoiceDie | external AudioEvent, Multisound | 252 / 24 / 12 | 88 |
| RebornModifierFXStream | the two local FX roots | 56 / 0 / 12 | 340 |

The native totals are **388 / 36 / 32 bytes**, excluding each stream's eight-byte
header. The manifest contains five ordered dependency pairs (40 bytes), three
explicit normal runtime manifest references and each asset's actual source filename.
Dependency-first ordering is deliberately fixed for this graph; it is not a new
general stable sorter.

## Real stages and independent checks

The fixture imports unchanged official EP1 modifier, armor, FX, audio and support
types. The official shader schema supplies ShaderOverrideRef, without admitting
shader roots or activating a shader compiler. The two extracted FX enums are
drift-checked against official Modules/BaseModules.xsd. No invented asset layout
stubs are added.

The core selects concrete AudioEvent/Multisound identities, retains exactly the
two local FX dependencies and visits only three compiled roots. The test invokes
actual isolated `ProcessInstance` entries. Physical-to-runtime mapping uses the
real `OutputManager.AddExternalManifestReferences` validation seam.

The existing diagnostic serializer and verifier now have internal test access;
the public command's preflight/schema/registry/ranking are unchanged. The proof
writes only its own absent temporary output directory and never calls production
`OutputManager.CommitManifest` or the production linker.

`ManifestReader` and the utility Manifest reader independently verify the linked
v7/EP1 header, identities, source attribution, concrete dependency tuples, runtime
paths, native offsets and exact selected slices. Serialized one-biased selectors
are decoded into the known local FX and concrete external audio targets, not just
compared with normalized XML. Recompilation and serialization are deterministic
across all four streams and the diagnostic warning file.

Shared `BoundedDiagnosticBuild.Verify` is strengthened for all existing admitted
families: it now checks exact stream magic/checksum/length, manifest total and
maximum sizes, reference-buffer size, prefixed-v7 identity and per-entry native
sizes independently of the staged byte snapshot.

## Failure evidence

The test performs 24 owned-file corruption cases:

- Wrong magic/checksum, truncation and appended bytes for each native stream.
- Zeroed selectors for all three Sound imports and both modifier FX imports.
- Changed relocation/import offsets.
- Swapped concrete audio pairs and substitution of the raw BaseAudioEventInfo type.
- Incorrect manifest maximum/entry chunk size and changed runtime manifest path.

For each case the expected byte snapshot is deliberately changed to match the
corrupt file. Independent semantic/native readback must still reject it. Original
bytes are restored afterward. Dependency identity checks are essential because
the existing identity checksum does not hash dependency identities or payload bytes.

Removing the Multisound mapping repeatedly fails recursive preparation and clears
partial ancestor/FX validation. Restoring it yields identical native output.
Successful sibling visits may remain in that failed attempt's private traversal
set; the next preparation starts a new set and rechecks all dependencies. Existing
diagnostic directories are rejected before writing and the original streams remain
unchanged. Experimental production and cache/reuse policies remain closed.

The synthetic external manifests contain identity metadata with placeholder hashes;
they are not audio payload compatibility evidence. Real-game exact FX byte and
concrete identity comparisons remain in the separate
[isolated FX profile test](RA3EP1_FX_PROFILE.md).

```text
modifier-fx-stream-self-test
compiler-self-test
layout-self-test
```

Both Release/x86 projects build; all 76 compiler groups, layout tests and 33 enum
mappings pass. Model/marshaller inventory remains 785/1,390 and 762/1,390.

## Next gate and remaining work

The subsequent [bounded FX command integration](RA3EP1_DIAGNOSTIC_FX_BUILD.md)
now passes, including preflight/snapshots, schema closure, explicit experimental
registration, four-family Includes, audio mapping/ambiguity, deterministic output
and guarded publication. FXList remains excluded from the production registry.

Full audio/particle processing, complete EP1 registration, SDK/WorldBuilder packaging
and in-game Uprising loading remain open. Overall engineering effort remains
approximately 50% complete / 50% remaining; a fixed graph proof does not establish
general command or game compatibility.
