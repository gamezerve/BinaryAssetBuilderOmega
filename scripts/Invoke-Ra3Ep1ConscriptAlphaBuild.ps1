# Reborn: prepare explicit EA XML sources before bounded native compilation; never invent a LogicCommand from the GameObject ID.
[CmdletBinding()]
param([Parameter(Mandatory)][string]$ModXml,[Parameter(Mandatory)][string]$StaticManifest,[Parameter(Mandatory)][string]$GlobalManifest,[Parameter(Mandatory)][string]$CompilerDirectory,[Parameter(Mandatory)][string]$OutputRoot,[switch]$ValidateOnly,[string]$GlobalStreamContext,[string]$MapStaticPackage)
$ErrorActionPreference='Stop'
# Reborn: source preparation resolves the relocated SDK and bundled runtime before invoking the compiler apphost.
. (Join-Path $PSScriptRoot 'Ra3Ep1PortablePaths.ps1')
$sdk=Get-RebornSdkRoot $PSScriptRoot
try {
    $settings=Read-RebornSdkPaths $sdk
    if(-not $settings.UprisingXmlRoot -or -not(Test-Path -LiteralPath $settings.UprisingXmlRoot -PathType Container)){throw 'Conscript requires UprisingXmlRoot in alpha/uprising-alpha.paths.'}
    $exe=Join-Path $CompilerDirectory 'BinaryAssetBuilder.ManifestInspector.exe';$prepared=Join-Path $OutputRoot 'source'
    # Reborn: use the same process for source preparation and native compilation so it owns the XML locks.
    $stockData=if($settings.SdkDataRoot){$settings.SdkDataRoot}else{Join-Path $sdk 'SageXML'}
    $scratch=Join-Path $OutputRoot 'temp';$null=New-Item -ItemType Directory -Path $scratch;$previousTemp=$env:TEMP;$previousTmp=$env:TMP
    try {
        # Reborn: contain all qualification intermediates in this invocation's owned work generation.
        $env:TEMP=$scratch;$env:TMP=$scratch
        # Reborn: static assets import freshly compiled global mod definitions instead of requiring duplicate XML includes.
        $compileArgs=@('sdk-compile-xml-stream',$ModXml,$prepared,$sdk,$stockData,$settings.UprisingXmlRoot,$StaticManifest,$GlobalManifest,(Join-Path $OutputRoot 'conscript'))
        if($GlobalStreamContext){$compileArgs+=$GlobalStreamContext}
        # Reborn: carry the same-build static catalog through compilation and WorldBuilder resolution.
        if($MapStaticPackage){if(!$GlobalStreamContext){throw 'Map static dependencies require a build stream context.'};$compileArgs+=@('map',$MapStaticPackage)}
        & $exe @compileArgs
        if($LASTEXITCODE-ne 0){throw 'Conscript source/native/metadata/stream qualification failed.'}
    } finally {$env:TEMP=$previousTemp;$env:TMP=$previousTmp}
    $inputReceipt=Get-Content -LiteralPath (Join-Path $prepared 'source-input.json') -Raw|ConvertFrom-Json
    $identity=$inputReceipt.Identity;$primaryType=$inputReceipt.PrimaryType;$assetCount=$inputReceipt.AssetCount
    $supportOnly=$primaryType-cne 'GameObject'
    # Reborn: the native compiler classifies actual catalog-backed stock overrides; Conscript is not the only legal override identity.
    $trials=@(foreach($candidate in $(if($supportOnly){@('support')}else{@('override','new-id')})){if(Test-Path -LiteralPath (Join-Path $OutputRoot ('conscript/'+$candidate+'/trial-plan.json')) -PathType Leaf){$candidate}})
    if($trials.Count-ne 1){throw 'Expected exactly one fresh native package receipt.'}
    $trial=$trials[0];$newIdentity=(-not $supportOnly -and $trial-ceq 'new-id');$receiptPath=Join-Path $OutputRoot ('conscript/'+$trial+'/trial-plan.json');$receipt=Get-Content -LiteralPath $receiptPath -Raw|ConvertFrom-Json
    $package=Join-Path $OutputRoot ('conscript/'+$trial+'/RebornConscriptProbe.big')
    # Reborn: resolve referenced WorldBuilder assets in the private package before receipt validation and publication.
    $resolveArgs=@('sdk-resolve-worldbuilder',$package,(Join-Path $sdk 'catalogs/ra3ep1'),$StaticManifest,$GlobalManifest,$inputReceipt.GamePath)
    if($GlobalStreamContext){$resolveArgs+=$GlobalStreamContext}
    if($MapStaticPackage){$resolveArgs+=$MapStaticPackage}
    & $exe @resolveArgs;if($LASTEXITCODE-ne 0){throw 'WorldBuilder asset resolution failed; no new package was published.'}
    $receipt=Get-Content -LiteralPath $receiptPath -Raw|ConvertFrom-Json
    if((-not $supportOnly -and $receipt.NewIdentity-ne $newIdentity) -or $receipt.Asset-cne ($primaryType+':'+$identity) -or $receipt.AssetCount-ne $assetCount -or -not $receipt.OfflineValidated -or (Get-FileHash -LiteralPath $package -Algorithm SHA256).Hash-cne $receipt.ArchiveSha256 -or $receipt.SourceSha256-cne $inputReceipt.PreparedSourceSha256){throw 'Fresh explicit-source package/readback receipt differs.'}
    # Reborn: source validation finished in the native stream process; publication uses its qualified output.
    Write-Output ('Compilation completed: '+$assetCount+' asset(s); native, metadata and stream qualification passed.')
    if($ValidateOnly){Write-Output 'Explicit source qualification passed; validation-only, no publication.';return}
    [IO.File]::WriteAllText((Join-Path $OutputRoot 'alpha-build.json'),([pscustomobject]@{BuildReceipt=$receiptPath;PackagePath=$package;Profile='conscript-explicit-source';AuthoredSources=$inputReceipt.Sources;PathSearches=$inputReceipt.Searches;AutomaticCommandGenerated=$false;GameExecuted=$false;InstalledFilesModified=$false}|ConvertTo-Json -Depth 8))
} catch {Write-Output ('Error: ALPHA120 '+$_.Exception.Message);Write-Output 'Critical: ALPHA121 Conscript alpha build failed.';exit 1}
