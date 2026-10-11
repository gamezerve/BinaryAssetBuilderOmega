# Alpha compiler metadata and provenance audit — October 11, 2026

## Outcome

The first overlay contained absolute local PDB paths in five managed DLLs.
Not shipping PDB files did not remove those PE CodeView strings. Workspace
preparation now builds candidates with DebugType=None/DebugSymbols=false; this
does not change project developer defaults or serializer logic. Overlay
preparation now refuses private ASCII/UTF16 path hints or absolute managed debug
paths before creating its output directory. Audit reports remain outside ZIPs.

`Get-Ra3Ep1AlphaBinaryAudit.ps1` reads exactly eight named compiler/runtime files,
each limited to 16 MiB. It uses PE metadata rather than loading their code,
records machine/debug/assembly-reference/PInvoke metadata and counts specific
private-path hints without exposing their values. This is not an exhaustive
binary privacy scanner, native import-table disassembler or legal review.

## Evidence and regression

- Original source `UprisingAlpha-20261011-e`: five managed files have private
  path hints and absolute debug paths. The updated overlay exporter refuses
  this source before creating `UprisingOverlay-private-refusal`; its separate
  audit report is retained.
- Fresh source `UprisingAlpha-20261011-f`: all eight files have zero private
  hints under the bounded patterns. Five managed DLLs have no absolute CodeView
  paths. Their remaining deterministic debug entry is not a local PDB path.
- The native x86 .NET apphost still has its SDK vendor CodeView path,
  `D:\a\_work\1\s\artifacts\obj\win-x86.Release\corehost\apphost\standalone\apphost.pdb`.
  It is not this user's source path. The gate permits this non-managed vendor
  entry; it does not claim all absolute strings in the executable are removed.
- New overlay `UprisingOverlay-20261011-b` passed the metadata gate; ZIP SHA256
  `8547CAC6DD24ADDEF08FDDD503A1010B31B22526E63587DF20F7E0461AF2F207`.
- Fresh setup `UprisingOverlay-test-b` passes external-file exclusion, path-free
  manifest, mismatched dependency refusal before copy, occupied refusal, input
  preservation, both sample builds and DTD/critical-marker refusal.
- GUN-5000 and GUN-250 retain their earlier exact BIG digests. Removing symbols
  changed compiler file hashes, not these authored mod bytes. No new GUI/game
  test or full compiler regression run is claimed.

## Distribution provenance checklist

| Component | Observed source evidence | Remaining release review |
| --- | --- | --- |
| Managed Core/Utility/XmlCompiler/Inspector | Repository source; current and earliest inspected LICENSE name Jana Mohn, MIT; no PackageReference in these projects | Preserve upstream license/attribution and review contributed/generated portions, not merely DLL names |
| SageBinaryData | Repository source/history predates current port; armor file later modified for EP1 | Review original versus generated/schema-derived portions; repository license presence is not a blanket third-party clearance |
| .NET apphost | Native x86 host supplied by installed SDK, not EA | Match relevant SDK/runtime distribution notices; installed dotnet has LICENSE.txt and ThirdPartyNotices.txt, neither automatically bundled by this step |
| EA Studio/EALABuild/EULA | External pinned user inputs | Continue excluding from candidate ZIP; initialization copies into local user workspace only |
| Five official schema files | External pinned user inputs | Continue excluding from candidate ZIP; other revisions are not silently interchangeable |
| Focused fixtures and sample XML | Repository-authored harnesses reference official definitions | Review embedded/derived content and notices before public release |

The deps.json libraries are five project entries (Inspector/Core/Utility/
XmlCompiler/SageBinaryData), with no NuGet-library entry. Framework-dependent
runtimeconfig requests Microsoft.NETCore.App 8.0.0; the runtime remains external.
This inventory does not prove all potential command paths lack optional native
dependencies. Only the documented armor backend is supported by the alpha.

PublicRedistributionReviewed remains false. Old local/overlay candidates are
retained, not rewritten or deleted. The new candidate is local and unuploaded;
only source scripts and documentation are published to the user's repository.
