# Reborn: refresh only derived portable support files; editable mods, art, audio and SDK settings are preserved.
param([string]$Repository=(Split-Path -Parent $PSScriptRoot),[string]$Sdk=(Join-Path (Split-Path -Parent $PSScriptRoot) 'alpha'))
$ErrorActionPreference='Stop'
#-------------------------------------------------------------------------------------------------
<# Reborn: copy a known development resource tree into its SDK-owned relative location without deleting user data. #>
#-------------------------------------------------------------------------------------------------
function Copy-RebornSupportTree([string]$Source,[string]$Destination) {
    foreach($file in Get-ChildItem -LiteralPath $Source -File -Recurse){
        $relative=$file.FullName.Substring([IO.Path]::GetFullPath($Source).TrimEnd('\').Length+1)
        $target=Join-Path $Destination $relative;$null=New-Item -ItemType Directory -Force -Path (Split-Path -Parent $target)
        Copy-Item -LiteralPath $file.FullName -Destination $target -Force
    }
}
$null=New-Item -ItemType Directory -Force -Path (Join-Path $Sdk 'scripts')
foreach($name in @('Invoke-Ra3Ep1DevAlphaBuild.ps1','Invoke-Ra3Ep1ConscriptAlphaBuild.ps1','Invoke-Ra3Ep1AlphaBuild.ps1','New-Ra3Ep1MissingArtPlaceholders.ps1','Ra3Ep1BuildLog.ps1','Ra3Ep1GlobalData.ps1','Ra3Ep1BuildStages.ps1','Ra3Ep1Maps.ps1','Ra3Ep1MapCache.ps1','Ra3Ep1FinalPackage.ps1','Ra3Ep1PortablePaths.ps1')){
    Copy-Item -LiteralPath (Join-Path $Repository ('scripts/'+$name)) -Destination (Join-Path $Sdk ('scripts/'+$name)) -Force
}
# Reborn: legacy Windows PowerShell runs synchronization too, so avoid requiring its unavailable Path.GetRelativePath API.
Copy-RebornSupportTree (Join-Path $Repository 'tests/fixtures') (Join-Path $Sdk 'tools/resources/tests/fixtures')
Copy-RebornSupportTree (Join-Path $Repository 'schemas/ra3ep1/xsd') (Join-Path $Sdk 'tools/resources/schemas/ra3ep1/xsd')
# Reborn: keep dependency/copy guidance inside the portable folder rather than linking only to repository documentation.
if(Test-Path -LiteralPath (Join-Path $Repository 'docs/PORTABLE_ALPHA.md')){
    $null=New-Item -ItemType Directory -Force -Path (Join-Path $Sdk 'Documentation')
    Copy-Item -LiteralPath (Join-Path $Repository 'docs/PORTABLE_ALPHA.md') -Destination (Join-Path $Sdk 'Documentation/PortableSDK.md') -Force
}
