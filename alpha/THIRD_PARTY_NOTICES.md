# Component attribution and candidate notices

This file records observed source provenance and supplied notice files. It is
not a legal opinion, certification of redistribution rights or endorsement.
PublicRedistributionReviewed remains false for the packaging candidate.

## BinaryAssetBuilder source lineage

The upstream repository is https://github.com/Qibbi/BinaryAssetBuilder.
The user's port repository is https://github.com/gamezerve/BinaryAssetBuilderOmega.
The repository LICENSE is preserved verbatim in this archive and names
Copyright (c) 2020 Jana Mohn under the MIT License. Do not remove that attribution
or substitute this summary for the full license. Source lineage and license
presence do not independently clear third-party/generated contributions.

The focused compiler contains project-built Inspector, Core, Utility,
XmlCompiler and SageBinaryData assemblies. Its deps.json currently declares
those project libraries and no NuGet library entry. This is not a claim that
every developer command or every optional native backend is part of this alpha.
Only the documented armor build path is supported.

## Microsoft .NET host and external runtimes

The native x86 apphost originates from the installed .NET SDK host pack. The
framework-dependent .NET 8 runtime, PowerShell 7 and .NET Framework 4 are external
prerequisites and are not bundled. The candidate carries unmodified,
explicitly supplied installed-.NET notice files at:

- notices/dotnet/LICENSE.txt
- notices/dotnet/ThirdPartyNotices.txt

Their SHA256 values are recorded in overlay-manifest.json. These installed
notice copies are informational; matching notice coverage and applicable terms
for the exact apphost build still require release review. They do not license
EA files or the complete project. Source URLs within supplied notices are
attributions, not setup download instructions.

## EA and schema dependencies

EA EALAModStudio.exe, EALABuild.dll, the SDK EULA and the five official schema
files are excluded from the overlay archive. The user must supply the matching
files from their existing local SDK/schema collection. Setup copies them into
the isolated local workspace only after hash checks; their separate terms are
not replaced by the repository MIT license. Game executables, installed BIG
archives, stock streams, WorldBuilder, native codecs and game content are not
provided by the overlay.

## Remaining review boundary

SageBinaryData layout provenance, focused fixture/schema-derived material,
contributions and the exact host notice coverage must still be checked before a
public release. This attribution inventory must not be presented as completed
rights clearance. Local setup/config receipts and their absolute paths are not
part of the distribution payload.
