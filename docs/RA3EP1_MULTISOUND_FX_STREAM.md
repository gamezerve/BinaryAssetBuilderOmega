# Fixed local Multisound / FX mixed-stream proof

## Outcome — October 3, 2026

A fixed three-level Include graph now compiles and round-trips this chain:
AttributeModifier → local FXList → local Multisound → two external AudioEvents.
The Multisound is actually compiled into the diagnostic stream, not substituted
by a stock external Multisound identity. An unused instance-Include sound with a
missing target remains tentative and does not enter output.

| Compiled root, in dependency-first order | Native BIN/RELO/IMP | Source |
|---|---|---|
| RebornLocalMultisound | 72/8/12 | sound.xml |
| RebornSoundFX | 80/12/8 | fx.xml |
| RebornSoundModifier | 56/0/8 | parent.xml |
| Total | 208/20/28 | Three included documents |

Linked BIN/RELO/IMP files are **216/28/36** bytes (eight-byte headers included).
The synthetic-mapping manifest is 425 bytes, with four reference tuples / 32
reference-buffer bytes and three explicit runtime mappings. A 491-byte diagnostic
warning accompanies it. Repeat builds are identical; existing output is preserved.
These files are test artifacts in a new owned temporary directory, not a mod.

The optional real pass was executed with
`D:\TEMP\Red Alert 3 Uprising Source Data\Global Data\data\global.manifest`.
GDI_Commando_VoiceDie and GDI_Engineer_VoiceDie resolve to the exact concrete
AudioEvent identities with TypeId 844D7B9F, TypeHash 560C2E45 and Tokenized=0.
Native chain bytes match the synthetic baseline; serialized local/external
selectors and utility readback pass. This pass reads external metadata only;
it does not rebuild, copy or validate external AudioEvent/AudioFile payloads.

## Checks and implementation

`MultisoundFXStreamSmokeTest.Run` uses official schema components in
`tests/fixtures/MultisoundFXPipeline.xsd` and explicitly isolated modifier/FX/
Multisound plugins. Extracted FX enum declarations are drift-checked.
Real core recursive dependency preparation selects exactly the three local roots;
compiler dispatch and ordering derive from this closure rather than forcing every
included declaration into output. Production/cache/reuse policies stay closed.

The proof uses the shared diagnostic serializer and verifier, plus independent
selector assertions: two AudioEvent imports inside Multisound, one concrete local
Multisound import inside FX, and one local FX import inside the modifier.
List count/pointer, PLAY_ONE control, weights 1000/800 and absent optional-pointer
fields are checked at explicit offsets independently of compiler buffer lengths.
Both manifest readers must agree on references, source names and linked offsets;
every compiled native slice must match.

External checks were extracted unchanged from BoundedDiagnosticBuild.Build into
`ValidateExternalDependencies`, shared by the public command and this fixed
proof. Every selected nonlocal identity must occur exactly once in the approved
metadata, and selected shader/audio hashes/tokenization must match observed EP1
fingerprints. Duplicate identities across mapped metadata, missing selected
records and wrong selected AudioEvent hashes/flags reject. Unselected records
remain outside the fingerprint gate.

A newly explicit stream requirement rejects any selected root whose prepared
reference table is null or has the wrong count. This matters because the existing
modifier import profile intentionally validates original normalized selectors,
not the prepared concrete table: after recursive leaf failure it can still marshal
that original-only representation. The fixed chain therefore checks readiness at
the stream gate; FX and Multisound entries also reject independently. The modifier
profile's earlier native-import contract was not silently changed.

Verification includes:

- Repeated compilation/serialization, actual Include closure, tentative exclusion
  and concrete local/external selectors/source attribution.
- Wrong selected audio hash and tokenization, cross-mapping duplicate and missing
  metadata rejection using the exact shared command gate.
- **20 corruption cases**, with corresponding snapshot expectations deliberately
  changed too: three streams' magic/checksum/truncation/padding (12), four native
  selectors, relocation/import offsets (2), external reference order and the local
  Multisound reference changed to BaseAudioEventInfo (2). Independent verifier
  checks reject every case and original owned bytes are restored.
- Repeated external leaf loss invalidates all three prepared tables. Recovery
  restores identical native output; no failed preparation is serialized.
- Editing the included sound leaf to GDI_FireHawk_VoiceDie refreshes the concrete
  target through both ancestors and produces a verified retargeted stream. Source
  restoration recovers native bytes. The user/game XML originals are untouched.
- Existing diagnostic output is never replaced by the test helper; production
  policy rejection occurs before missing input/cache access. Caller Settings are
  restored. Tests write only their own temporary sources/metadata/artifacts.

Both Release/x86 projects build; all **80 compiler groups**, layout checks and
33 enum mappings pass. The existing public FX command test still matches three
full stock native goldens and reference tuples after the shared-gate refactor.
Inventory remains 785/1,390 models and 762/1,390 typed marshallers.

## Reproduce and limitations

After a Release/x86 inspector build, run from the repository root:

```powershell
# Reborn: verify a fixed owned diagnostic chain with actual stock metadata, never production audio output.
.\source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe multisound-fx-stream-self-test "D:\TEMP\Red Alert 3 Uprising Source Data\Global Data\data\global.manifest"
```

Omit the argument for fixed synthetic metadata tests. Optional physical manifests
are read-only. Their generated `data/stock-N.manifest` runtime names are test
mappings, not packaging instructions or verified game load paths.

This is a fixed self-test, not a general input/output command. It does not extend
DiagnosticSourceGraph, public root admission, staging/race guarantees or arbitrary
cycle/ordering support. Public diagnostic-build still admits four families and
rejects authored Multisound roots. Its existing snapshot/publication machinery
is unchanged apart from the shared prepared-table gate.

Next: admit the narrow Multisound subset through the public snapshot grammar and
schema, extend dependency-first ordering safely, reject unsupported local cycles,
and test arbitrary bounded Include graphs, immutable metadata/source snapshots,
mapping limits, retarget/loss/recovery and staged publication. AudioEvent/AudioFile
native recovery/codecs, wider processors, SDK/WorldBuilder packaging and actual
Uprising loading remain open. Overall effort stays approximately **50% complete /
50% remaining**: this closes fixed-chain proof, not end-to-end audio or game loading.

Related: [checked sound profile](RA3EP1_MULTISOUND_PROFILE.md),
[native sound proof](RA3EP1_MULTISOUND_NATIVE.md),
[public FX command](RA3EP1_DIAGNOSTIC_FX_BUILD.md).
