# Reborn: one-command diagnostic armor build

## What this delivers

`scripts/Invoke-Ra3Ep1ArmorPoc.ps1` runs the existing focused armor compiler
proofs together, rather than requiring users to assemble historical commands.
It creates a fresh isolated artifact directory and never changes installed game
files, runs Uprising, enables production output or bypasses hash guards.
This consolidates and freshly reproduces existing compiler capability; it is
not a newly implemented serializer or a successful mod-load milestone.

Under PowerShell 7, from the repository directory:

```powershell
# Reborn: rebuild the local diagnostic compiler and validate both actual unpacked Uprising samples; no game is launched.
./scripts/Invoke-Ra3Ep1ArmorPoc.ps1
```

The default stock inputs are the user's existing unpacked manifests:

- `D:\TEMP\Red Alert 3 Uprising Source Data\Static Data\data\static.manifest`
- `D:\TEMP\Red Alert 3 Uprising Source Data\WorldBuilder\data\worldbuilder.manifest`

`-StaticManifest` and `-WorldBuilderManifest` can explicitly select equivalent
unpacked inputs. Both must exist; missing inputs do not downgrade to fixture-only
success. Requires the .NET SDK, restored local dependencies and x86 .NET runtime
used by the existing inspector. Build uses `--no-restore`, one MSBuild node and
no node reuse; it does not download dependencies. A missing dependency fails
with its local build log rather than installing tools automatically.

## Five recorded steps

1. Rebuild current Release/x86 inspector source.
2. Exercise the actual opt-in armor plugin/registry entry.
3. Exercise official-schema/document/default/hash stages and reuse guards.
4. Compare the populated armor output to the Static sample's 744 BIN / 8 RELO
   bytes and validate diagnostic stream writing/readback.
5. Separately compare the empty-list armor to the WorldBuilder sample's
   128 BIN / 0 RELO bytes; read only bounded BIN ranges, not the 1.2+ GB file.

Each step records its exit code and log. The first failure stops subsequent
steps and writes a failed `readiness.json`; previous outputs are never reused
as proof. Successful reports include generated stream lengths and SHA-256s.

Important distinction: both output folders contain the populated eleven-item
diagnostic fixture. The WorldBuilder comparison validates a separate empty-list
in-memory fixture; its output folder is not a dump of the house-armor asset.
Linked stream files include eight-byte headers, so populated files are
752 BIN / 16 RELO / 8 IMP bytes, not their 744/8/0 payload lengths.

## Actual reproduction — October 11, 2026

Local ignored artifact: `RebornArmorPoc-7fb0eb9c4a8144ea82e5aa69d8d7b1b7`.
All five steps exit 0; logs explicitly confirm both real golden byte matches.
The first attempt (`RebornArmorPoc-c5c49b822f7a4f529ffa238b67f761b0`) failed
at the default parallel build, before tests. It remains a failed report;
single-node/no-reuse build resolved this local build invocation problem.
No claim about a compiler-source fault is inferred from the first build's
zero-error failure summary.

The full existing compiler self-test and layout self-test also passed after
the rebuild. The first sandboxed full-suite attempt stopped at an access-denied
move of its own generated temporary audio file; the scoped-permission rerun
completed with `Uprising compiler self-test: OK`. Neither run used game
instrumentation or native audio codecs. The runner's five steps are a focused
entry point, not five new compiler groups or a replacement for the full suite.

## Direct next gate, not another debugger subproject

Follow-up: [actual Core armor streams](RA3EP1_ARMOR_CORE_STREAMS.md) are now
written under `document/core-empty`, `core-repeat`, `core-edited` and
`core-populated`, using actual Core InstanceHash and padded checksum values.
The runner reports `CoreIdentityStreamsValidated` separately. The original
`static`/`worldbuilder` golden fixtures retain the fixed diagnostic checksum
and zero InstanceHash. All stream families are marked `DIAGNOSTIC_ONLY.txt`;
none is an approved playable mod. Production registry policy remains disabled.

The [actual OutputManager follow-up](RA3EP1_ARMOR_OUTPUT_MANAGER.md) now runs
intermediate `.asset` commit, output selection, manifest commit and linking
inside step 3 through a private one-document diagnostic scope. The runner
reports `OutputManagerCommitLinkValidated` and its fresh output directory;
production output remains disabled. The standalone `document/core-*` family
still uses the separate diagnostic serializer.

Next resolve runtime-compatible armor identity and production eligibility.
Then create a reversible package with independent readback and request the
separate runtime test authorization when it is actually ready. Existing
config-consumption evidence remains a runtime gate; it does not prevent
offline compiler/package work. No universal tokenizer, music compiler or
complete type-table port is required to begin this focused path.

Progress should be reported by these deliverables. More tests/commits or a
repeated historical 52% estimate are not substitutes for an authored asset
loading in the game. A defensible date for that result is not established yet.
