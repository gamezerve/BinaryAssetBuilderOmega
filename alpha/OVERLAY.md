# Uprising SDK Alpha overlay candidate

This archive is a packaging candidate, not a reviewed public release. Unlike
the local workspace ZIP, it excludes EA Studio/EALABuild/EULA files, the five
official schema files, stock streams, live configuration and absolute preparation
provenance. The relative manifest pins the currently tested dependency hashes.
The included compiler and fixtures still require a separate provenance/license
review; absence of EA executables alone is not a distribution approval.

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
Armor250 and Build Mod. Only AlliedConYardArmor GUN integer 0..10000 is supported.
Output goes to fresh builtmods subdirectories; unsupported XML fails visibly.
No automatic mod installation or game launch is provided. Setup's configuration
and receipt contain local paths and must not be added to a public archive.
