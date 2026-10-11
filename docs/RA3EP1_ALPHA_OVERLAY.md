# User-supplied dependency overlay — October 11, 2026

## Outcome

A separate overlay candidate now excludes the three user-supplied EA Studio
files and five official schema graph files. It keeps 22 named compiler,
fixture, adapter, help/sample/license/setup files. Its manifest contains only
relative file names, hashes and scope flags, not preparation Source/Workspace
paths. Existing local-use workspace ZIPs remain unchanged.

This is a tested packaging boundary, not a legal determination or public
release. The compiled managed assemblies, generated SageBinaryData source,
fixture provenance, notices and possible embedded build paths still need
review. Dependency hashes pin the currently tested inputs rather than making
any official SDK/schema revision interchangeable. There is no signature or
certificate trust mechanism; use only a known project artifact, not an
arbitrary modified manifest/script.

## Files and workflow

- `scripts/New-Ra3Ep1AlphaOverlay.ps1` verifies a prepared workspace inventory,
  copies the explicit remaining files, emits path-free requirements and creates
  a new candidate ZIP. It does not upload or configure it.
- `scripts/Initialize-Ra3Ep1AlphaOverlay.ps1` accepts user SDK/schema roots,
  stock manifest and PowerShell 7. It verifies overlay files and every pinned
  external dependency before copying into the isolated extracted directory.
  It creates builtmods, local path configuration and a local setup receipt.
  Original SDK/game files are read-only inputs. Already configured/occupied
  outputs and redirected paths are refused. On mid-copy failure the partial
  workspace is preserved; no rollback deletion or overwrite is attempted.
- `scripts/Test-Ra3Ep1AlphaOverlay.ps1` inspects ZIP paths, creates fresh positive
  and disposable negative extractions, changes only a negative manifest digest,
  checks rejection before dependency copy, then tests setup and occupied refusal.
  It synthesizes a local-only inventory to reuse the sample/backend controls.
- `alpha/OVERLAY.md` documents explicit prerequisites and initialization.

The setup does not repair registry/runtime installations, download dependencies,
start Studio/game, patch EA executables or install a mod. Bare extraction is not
a configured SDK; run the initializer first. The startup remains manual.

## Evidence

Prepared source: `UprisingAlpha-20261011-e`.
Overlay staging: `UprisingOverlay-20261011-a`.
Tested extraction: `UprisingOverlay-test-a`.
Candidate ZIP SHA256:
`CC9EA0D97DE229D2B729E636CF06B825A9E6C13C30918192D3423AF905E1EFA7`.

The tested archive has no EA executable/EALABuild/EULA, official schema directory,
stock streams, builtmods, live config or absolute preparation JSON. The manifest
has no absolute provenance. A wrong dependency digest refuses before EA copying
or builtmods creation. Clean setup succeeds; a second setup refuses without
changing configuration. Inventory preservation, GUN-5000 and GUN-250 packages,
DTD refusal with ALPHA100/ALPHA101 and success logs without failure markers all
pass. Both packages retain the earlier known digests.

The tests do not scan all binary strings, verify broad processor coverage,
repeat GUI/game testing or grant redistribution rights. `PublicRedistributionReviewed`
remains false. The candidate ZIP is local only; only project scripts/docs are
committed/pushed. Next review compiler/fixture provenance and embedded metadata,
then choose a reviewed release allowlist and dependency/setup policy.

The subsequent [binary audit](RA3EP1_ALPHA_BINARY_AUDIT.md) found local managed
PDB paths in this first candidate. Preparation now omits managed symbols and
overlay export blocks those private hints. Updated candidate `UprisingOverlay-20261011-b`
and clean setup `UprisingOverlay-test-b` pass both sample/refusal regressions.
The earlier candidate is retained for history but is not the preferred package.
Distribution notice/provenance review remains open.

Candidate `UprisingOverlay-20261011-c` adds the relocation helper (23 payload
files). Fresh setup and copied configured-workspace repair both pass, including
byte-exact configuration backup and repeated sample builds. See
[relocation evidence and portability limits](RA3EP1_ALPHA_RELOCATION.md).

Candidate `UprisingOverlay-20261011-d` includes component attribution and two
unchanged explicitly supplied .NET notice files (26 payload files). The exporter
now requires DotNetNoticeDirectory; old invocations without this argument are
incomplete. See [notice preservation and scope](RA3EP1_ALPHA_NOTICES.md) for
fresh setup/relocation evidence and remaining review limitations.
