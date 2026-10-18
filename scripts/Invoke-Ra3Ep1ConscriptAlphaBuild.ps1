# Reborn: prepare explicit EA XML sources before bounded native compilation; never invent a LogicCommand from the GameObject ID.
[CmdletBinding()]
param([Parameter(Mandatory)][string]$ModXml,[Parameter(Mandatory)][string]$StaticManifest,[Parameter(Mandatory)][string]$GlobalManifest,[Parameter(Mandatory)][string]$CompilerDirectory,[Parameter(Mandatory)][string]$OutputRoot,[switch]$ValidateOnly)
$ErrorActionPreference='Stop';$sdk=Join-Path (Split-Path -Parent $PSScriptRoot) 'alpha'
try {
    $settings=@{};foreach($line in Get-Content -LiteralPath (Join-Path $sdk 'uprising-alpha.paths')){if($line -and -not $line.StartsWith('#')){$pair=$line.Split('=',2);$settings[$pair[0]]=$pair[1]}}
    if(-not $settings.UprisingXmlRoot -or -not(Test-Path -LiteralPath $settings.UprisingXmlRoot -PathType Container)){throw 'Conscript requires UprisingXmlRoot in alpha/uprising-alpha.paths.'}
    $exe=Join-Path $CompilerDirectory 'BinaryAssetBuilder.ManifestInspector.exe';$prepared=Join-Path $OutputRoot 'source'
    & $exe conscript-source-prepare $ModXml $prepared;if($LASTEXITCODE-ne 0){throw 'EA source preparation failed; see the XML/include/schema diagnostic above.'}
    $inputReceipt=Get-Content -LiteralPath (Join-Path $prepared 'source-input.json') -Raw|ConvertFrom-Json
    $identity=$inputReceipt.Identity;$hasCommand=[bool]$inputReceipt.CommandPath;$newIdentity=$identity-cne 'SovietAntiInfantryInfantry'
    Write-Output 'Warning: ALPHA005 EA source frontend: one complete Conscript-shaped GameObject, six scalar edits, optional explicit UNIT_BUILD LogicCommand. Defaults/attribute order supported; general inheritance/ART/W3X not yet connected.'
    Write-Output ('Reborn: authored asset count: '+$(if($hasCommand){2}else{1})+'; no automatic LogicCommand generation.')
    $scratch=Join-Path $OutputRoot 'temp';$null=New-Item -ItemType Directory -Path $scratch;$previousTemp=$env:TEMP;$previousTmp=$env:TMP
    try {
        # Reborn: contain all qualification intermediates in this invocation's owned work generation.
        $env:TEMP=$scratch;$env:TMP=$scratch
        & $exe conscript-alpha-build $settings.UprisingXmlRoot $StaticManifest $GlobalManifest $inputReceipt.GamePath (Join-Path $OutputRoot 'conscript')
        if($LASTEXITCODE-ne 0){throw 'Conscript source/native/metadata/stream qualification failed.'}
    } finally {$env:TEMP=$previousTemp;$env:TMP=$previousTmp}
    $trial=if($newIdentity){'new-id'}else{'override'};$receiptPath=Join-Path $OutputRoot ('conscript/'+$trial+'/trial-plan.json');$receipt=Get-Content -LiteralPath $receiptPath -Raw|ConvertFrom-Json
    $package=Join-Path $OutputRoot ('conscript/'+$trial+'/RebornConscriptProbe.big')
    if($receipt.NewIdentity-ne $newIdentity -or $receipt.Asset-cne ('GameObject:'+$identity) -or $receipt.AssetCount-ne $(if($hasCommand){2}else{1}) -or -not $receipt.OfflineValidated -or (Get-FileHash -LiteralPath $package -Algorithm SHA256).Hash-cne $receipt.ArchiveSha256 -or $receipt.SourceSha256-cne $inputReceipt.PreparedSourceSha256){throw 'Fresh explicit-source package/readback receipt differs.'}
    # Reborn: recheck every source, including included command XML, before publication rather than trusting only prepared-source hashes.
    foreach($source in $inputReceipt.Sources){if((Get-FileHash -LiteralPath $source.Path -Algorithm SHA256).Hash-cne $source.Sha256){throw ('Authored source changed during compilation: '+$source.Path)}}
    if($ValidateOnly){Write-Output 'Reborn: explicit source qualification passed; validation-only, no publication.';return}
    [IO.File]::WriteAllText((Join-Path $OutputRoot 'alpha-build.json'),([pscustomobject]@{BuildReceipt=$receiptPath;PackagePath=$package;Profile='conscript-explicit-source';AuthoredSources=$inputReceipt.Sources;AutomaticCommandGenerated=$false;GameExecuted=$false;InstalledFilesModified=$false}|ConvertTo-Json -Depth 5))
} catch {Write-Output ('Error: ALPHA120 '+$_.Exception.Message);Write-Output 'Critical: ALPHA121 Conscript alpha build failed.';exit 1}
