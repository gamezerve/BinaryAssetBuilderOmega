# Uprising SDK Alpha overlay candidate

This archive is a packaging candidate, not a reviewed public release. Unlike
the local workspace ZIP, it excludes EA Studio/EALABuild/EULA files, the five
official schema files, stock streams, live configuration and absolute preparation
provenance. The relative manifest pins the currently tested dependency hashes.
The included compiler and fixtures still require a separate provenance/license
review; absence of EA executables alone is not a distribution approval.
Candidate builds now omit managed debug symbols to avoid local PDB paths, and
packaging refuses bounded private-path hints. The .NET apphost's vendor debug
metadata is distinct from user paths; this does not replace a notice review.
THIRD_PARTY_NOTICES.md records component attribution. The complete project
LICENSE and explicitly supplied installed-.NET license/third-party notices are
included unchanged and pinned in the manifest. Exact apphost notice coverage
remains unreviewed; including these copies is not redistribution clearance.

Extract to a short ASCII path. Use PowerShell 7 to initialize that fresh copy:

```powershell
# Reborn: use your existing SDK and matching schema inputs; originals remain read-only and no game starts.
& .\scripts\Initialize-Ra3Ep1AlphaOverlay.ps1 `
  -StudioDirectory 'C:\YOUR\RA3SDK' `
  -SchemaDirectory 'C:\YOUR\UprisingSchemas' `
  -StaticManifest 'C:\YOUR\UNPACKED\data\static.manifest' `
  -PowerShell7 'C:\YOUR\PowerShell7\pwsh.exe'
```

SchemaDirectory must contain AssetTypeArmorTemplate.xsd, Includes/Base.xsd,
Includes/Ref.xsd, Includes/DamageDef.xsd and Base/AssetBase.xsd matching the
manifest hashes. StudioDirectory must contain the tested EALAModStudio.exe,
EALABuild.dll and EULA for RA3 MOD SDK.rtf. Other revisions are refused rather
than assumed compatible. PowerShell 7, x86 .NET 8 and .NET Framework 4 are
external prerequisites; setup does not download or install them.

Setup checks the complete input set before copying dependencies into this
isolated overlay. Configured/occupied destinations are refused. If copying
fails, retain the partial workspace for inspection and start with a fresh
extraction; no rollback deletion or overwriting is performed. Never merge this
overlay into your original SDK or game directory.

After setup, open this copy's EALAModStudio.exe manually, select Reborn or
Armor250, ArmorCannon, SovietArmor or CombinedArmor and Build Mod. Only AlliedConYardArmor or
SovietConYardArmor's thirteen ordered
Percent values and five optional root scalars accept integers 0..10000. Expanded
edits are offline validated; Allied GUN/CANNON have user-observed gameplay evidence,
and Soviet GUN are user-observed in gameplay. CombinedArmor builds both distinct
identities into one shared stream, with one of each required and no third asset.
The combined package and other edits are not independently gameplay proven.
Output goes to fresh builtmods subdirectories; unsupported XML fails visibly.
No automatic mod installation or game launch is provided. Setup's configuration
and receipt contain local paths and must not be added to a public archive.

## Relocating a configured copy

Close Studio and active builds before moving/copying your workspace. From its
new location run:

```powershell
# Reborn: derive driver/compiler paths from this copy and retain a byte-exact backup of old configuration.
& .\scripts\Update-Ra3Ep1AlphaPaths.ps1
```

Existing PowerShell7/StaticManifest settings are retained only if their files
still exist. On another computer, pass new explicit -PowerShell7 and
-StaticManifest paths. Driver and CompilerDirectory always resolve to the new
workspace. Missing stock stream siblings, redirected paths, malformed config
and oversized/non-ASCII workspace paths are refused before config replacement.
The helper preserves samples/build outputs and does not move files, relaunch
Studio, change registry or install runtimes. Old config is retained as a unique
uprising-alpha.paths.&lt;guid&gt;.bak. Backups contain local paths and are not
distribution payload. This still is not a self-contained runtime bundle.
