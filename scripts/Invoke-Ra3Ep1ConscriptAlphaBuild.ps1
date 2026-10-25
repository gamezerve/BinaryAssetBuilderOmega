# Reborn: prepare explicit EA XML sources before bounded native compilation; never invent a LogicCommand from the GameObject ID.
[CmdletBinding()]
param([Parameter(Mandatory)][string]$ModXml,[Parameter(Mandatory)][string]$StaticManifest,[Parameter(Mandatory)][string]$GlobalManifest,[Parameter(Mandatory)][string]$CompilerDirectory,[Parameter(Mandatory)][string]$OutputRoot,[switch]$ValidateOnly,[string]$GlobalStreamContext)
$ErrorActionPreference='Stop';$sdk=Join-Path (Split-Path -Parent $PSScriptRoot) 'alpha'
try {
    $settings=@{};foreach($line in Get-Content -LiteralPath (Join-Path $sdk 'uprising-alpha.paths')){if($line -and -not $line.StartsWith('#')){$pair=$line.Split('=',2);$settings[$pair[0]]=$pair[1]}}
    if(-not $settings.UprisingXmlRoot -or -not(Test-Path -LiteralPath $settings.UprisingXmlRoot -PathType Container)){throw 'Conscript requires UprisingXmlRoot in alpha/uprising-alpha.paths.'}
    $exe=Join-Path $CompilerDirectory 'BinaryAssetBuilder.ManifestInspector.exe';$prepared=Join-Path $OutputRoot 'source'
    # Reborn: standard SDK roots are adjacent to Studio; an explicit local stock XML override changes only the SageXML root.
    $searchArgs=@($sdk);if($settings.SdkDataRoot){$searchArgs+=$settings.SdkDataRoot}
    & $exe conscript-source-prepare $ModXml $prepared @searchArgs;if($LASTEXITCODE-ne 0){throw 'EA source preparation failed; see the XML/include/schema diagnostic above.'}
    $inputReceipt=Get-Content -LiteralPath (Join-Path $prepared 'source-input.json') -Raw|ConvertFrom-Json
    $identity=$inputReceipt.Identity;$hasCommand=[bool]$inputReceipt.CommandPath
    # Reborn: preserve the legacy receipt shape while routing an explicit command-set primary without a fabricated unit.
    $primaryType=if($inputReceipt.PrimaryType){$inputReceipt.PrimaryType}else{'GameObject'};$supportOnly=$primaryType-cne 'GameObject'
    # Reborn: report the fixed and variable source profiles separately so variable Behaviors do not imply arbitrary nested GameObject support.
    if(-not $supportOnly){
    Write-Output 'Warning: ALPHA005 Partial GameObject support. Fixed graph: reviewed root/module fields, local IDs, optional root imports, existing typed reference retargeting, six dynamic root strings and existing nested numeric owners.'
    # Reborn: reference retargeting is shared with variable Behaviors; only actual unimplemented layouts/resources remain restricted.
    # Reborn: do not advertise already implemented text editing as an unsupported capability.
    Write-Output 'Warning: ALPHA006 Partial native layout support: variable Behaviors, root text, ArmorSet lists, hardpoint/Weapon lists with optional imports, fixed fields, compatible targets and source-sized upgrade lists/animation are supported together. Turret/contained weapon slots, other unimplemented native layouts and resource compilation still require implementation.'
    }else{Write-Output ('Building explicit '+$primaryType+' support assets; no GameObject is required or generated.')}
    $assetCount=if($null-ne $inputReceipt.AssetCount){[int]$inputReceipt.AssetCount}else{if($hasCommand){2}else{1}}
    # Reborn: reached only after the dependency audit, include/inheritance preparation and schema/source admission all passed.
    Write-Output ('Validation completed: '+@($inputReceipt.Sources).Count+' source file(s), '+$assetCount+' asset(s) admitted.')
    Write-Output ('Compilation started: '+$assetCount+' asset(s).')
    $scratch=Join-Path $OutputRoot 'temp';$null=New-Item -ItemType Directory -Path $scratch;$previousTemp=$env:TEMP;$previousTmp=$env:TMP
    try {
        # Reborn: contain all qualification intermediates in this invocation's owned work generation.
        $env:TEMP=$scratch;$env:TMP=$scratch
        # Reborn: static assets import freshly compiled global mod definitions instead of requiring duplicate XML includes.
        $compileArgs=@('conscript-alpha-build',$settings.UprisingXmlRoot,$StaticManifest,$GlobalManifest,$inputReceipt.GamePath,(Join-Path $OutputRoot 'conscript'));if($GlobalStreamContext){$compileArgs+=$GlobalStreamContext}
        & $exe @compileArgs
        if($LASTEXITCODE-ne 0){throw 'Conscript source/native/metadata/stream qualification failed.'}
    } finally {$env:TEMP=$previousTemp;$env:TMP=$previousTmp}
    # Reborn: the native compiler classifies actual catalog-backed stock overrides; Conscript is not the only legal override identity.
    $trials=@(foreach($candidate in $(if($supportOnly){@('support')}else{@('override','new-id')})){if(Test-Path -LiteralPath (Join-Path $OutputRoot ('conscript/'+$candidate+'/trial-plan.json')) -PathType Leaf){$candidate}})
    if($trials.Count-ne 1){throw 'Expected exactly one fresh native package receipt.'}
    $trial=$trials[0];$newIdentity=(-not $supportOnly -and $trial-ceq 'new-id');$receiptPath=Join-Path $OutputRoot ('conscript/'+$trial+'/trial-plan.json');$receipt=Get-Content -LiteralPath $receiptPath -Raw|ConvertFrom-Json
    $package=Join-Path $OutputRoot ('conscript/'+$trial+'/RebornConscriptProbe.big')
    if((-not $supportOnly -and $receipt.NewIdentity-ne $newIdentity) -or $receipt.Asset-cne ($primaryType+':'+$identity) -or $receipt.AssetCount-ne $assetCount -or -not $receipt.OfflineValidated -or (Get-FileHash -LiteralPath $package -Algorithm SHA256).Hash-cne $receipt.ArchiveSha256 -or $receipt.SourceSha256-cne $inputReceipt.PreparedSourceSha256){throw 'Fresh explicit-source package/readback receipt differs.'}
    # Reborn: recheck every source, including included command XML, before publication rather than trusting only prepared-source hashes.
    foreach($source in $inputReceipt.Sources){if((Get-FileHash -LiteralPath $source.Path -Algorithm SHA256).Hash-cne $source.Sha256){throw ('Authored source changed during compilation: '+$source.Path)}}
    # Reborn: completion requires the native/metadata/stream qualification exit, receipt/package readback and unchanged authored sources.
    Write-Output ('Compilation completed: '+$assetCount+' asset(s); native, metadata and stream qualification passed.')
    if($ValidateOnly){Write-Output 'Explicit source qualification passed; validation-only, no publication.';return}
    [IO.File]::WriteAllText((Join-Path $OutputRoot 'alpha-build.json'),([pscustomobject]@{BuildReceipt=$receiptPath;PackagePath=$package;Profile='conscript-explicit-source';AuthoredSources=$inputReceipt.Sources;PathSearches=$inputReceipt.Searches;AutomaticCommandGenerated=$false;GameExecuted=$false;InstalledFilesModified=$false}|ConvertTo-Json -Depth 8))
} catch {Write-Output ('Error: ALPHA120 '+$_.Exception.Message);Write-Output 'Critical: ALPHA121 Conscript alpha build failed.';exit 1}
