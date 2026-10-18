# Reborn: recompile the exact reviewed BuildTime=1 Conscript XML through Core and all original graph controls; never export a cached BIG as a new build.
[CmdletBinding()]
param([Parameter(Mandatory)][string]$ModXml,[Parameter(Mandatory)][string]$StaticManifest,[Parameter(Mandatory)][string]$GlobalManifest,[Parameter(Mandatory)][string]$CompilerDirectory,[Parameter(Mandatory)][string]$OutputRoot,[switch]$ValidateOnly)
$ErrorActionPreference='Stop';$sdk=Join-Path (Split-Path -Parent $PSScriptRoot) 'alpha'
try {
    $settings=@{};foreach($line in Get-Content -LiteralPath (Join-Path $sdk 'uprising-alpha.paths')){if($line -and -not $line.StartsWith('#')){$pair=$line.Split('=',2);$settings[$pair[0]]=$pair[1]}}
    if(-not $settings.UprisingXmlRoot -or -not(Test-Path -LiteralPath $settings.UprisingXmlRoot -PathType Container)){throw 'Conscript requires UprisingXmlRoot in alpha/uprising-alpha.paths.'}
    # Reborn: exact canonical source admission is deliberately narrower than arbitrary GameObject authoring; unknown edits must fail before expensive qualification.
    $readerSettings=[Xml.XmlReaderSettings]::new();$readerSettings.DtdProcessing=[Xml.DtdProcessing]::Prohibit;$readerSettings.XmlResolver=$null;$readerSettings.MaxCharactersInDocument=1048576
    $reader=[Xml.XmlReader]::Create($ModXml,$readerSettings);try{$xml=[Xml.XmlDocument]::new();$xml.PreserveWhitespace=$true;$xml.XmlResolver=$null;$xml.Load($reader)}finally{$reader.Dispose()}
    $sourceHash=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($xml.DocumentElement.OuterXml)))
    if($sourceHash-cne 'B7EDFA37B5F1A26FA2ED5610BFEB2B2F05496B5BF150B9E068F9A4AB066DC6D6'){throw 'Only the reviewed complete original-ID Conscript XML with BuildTime=1 is admitted; other edits are not yet enabled.'}
    Write-Output 'Warning: ALPHA005 Bounded complete Conscript BuildTime=1 only; no general GameObject edits or recruitable new identities.'
    $scratch=Join-Path $OutputRoot 'temp';$null=New-Item -ItemType Directory -Path $scratch
    $previousTemp=$env:TEMP;$previousTmp=$env:TMP
    try {
        # Reborn: diagnostics create temporary qualification fixtures; contain all of them in this build's owned work generation.
        $env:TEMP=$scratch;$env:TMP=$scratch
        & (Join-Path $CompilerDirectory 'BinaryAssetBuilder.ManifestInspector.exe') conscript-alpha-build $settings.UprisingXmlRoot $StaticManifest $GlobalManifest $ModXml (Join-Path $OutputRoot 'conscript')
        if($LASTEXITCODE-ne 0){throw 'Conscript source/native/metadata/stream qualification failed.'}
    } finally {$env:TEMP=$previousTemp;$env:TMP=$previousTmp}
    $receiptPath=Join-Path $OutputRoot 'conscript/override/trial-plan.json';$receipt=Get-Content -LiteralPath $receiptPath -Raw|ConvertFrom-Json
    $package=Join-Path $OutputRoot 'conscript/override/RebornConscriptProbe.big'
    if($receipt.BuildTime-ne 1 -or $receipt.NewIdentity -or -not $receipt.OfflineValidated -or (Get-FileHash -LiteralPath $package -Algorithm SHA256).Hash-cne $receipt.ArchiveSha256 -or $receipt.SourceSha256-cne $sourceHash){throw 'Fresh Conscript package/readback receipt differs.'}
    if($ValidateOnly){Write-Output 'Reborn: complete Conscript qualification passed; validation-only, no final publication.';return}
    [IO.File]::WriteAllText((Join-Path $OutputRoot 'alpha-build.json'),([pscustomobject]@{BuildReceipt=$receiptPath;PackagePath=$package;Profile='bounded-conscript-buildtime1';GameExecuted=$false;InstalledFilesModified=$false}|ConvertTo-Json))
} catch {Write-Output ('Error: ALPHA120 '+$_.Exception.Message);Write-Output 'Critical: ALPHA121 Conscript alpha build failed.';exit 1}
