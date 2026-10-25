# Reborn: compare fast/developer output and real regression execution in fresh private sources, never in a live mod or Documents.
[CmdletBinding()]
param([Parameter(Mandatory)][string]$OutputDirectory,[Parameter(Mandatory)][string]$StaticManifest,[Parameter(Mandatory)][string]$GlobalManifest)
$ErrorActionPreference='Stop';$repo=Split-Path -Parent $PSScriptRoot;$sdk=Join-Path $repo 'alpha';$output=[IO.Path]::GetFullPath($OutputDirectory)
if([IO.Path]::GetDirectoryName($output)-ine (Join-Path $sdk '.work') -or (Test-Path -LiteralPath $output)){throw 'Use a fresh direct child of alpha/.work.'}
$settings=@{};foreach($line in Get-Content -LiteralPath (Join-Path $sdk 'uprising-alpha.paths')){if($line -and -not $line.StartsWith('#')){$pair=$line.Split('=',2);$settings[$pair[0]]=$pair[1]}}
$data=Join-Path $output 'mod/Data';$null=New-Item -ItemType Directory -Path $data
$baseline=[Xml.XmlDocument]::new();$baseline.XmlResolver=$null;$baseline.Load((Join-Path $repo 'tests/fixtures/live-alpha/ConscriptBuildTime1/data/mod.xml'))
$source=[Xml.XmlDocument]::new();$source.XmlResolver=$null;$source.LoadXml('<!-- Reborn: source build-mode parity fixture. --><AssetDeclaration xmlns="uri:ea.com:eala:asset"/>');$null=$source.DocumentElement.AppendChild($source.ImportNode($baseline.DocumentElement,$true));$empty=$source.CreateElement('W3DContainer','uri:ea.com:eala:asset');$empty.SetAttribute('id','RebornBuildModeEmpty');$null=$source.DocumentElement.AppendChild($empty)
$entry=Join-Path $data 'Static_Mod.xml';[IO.File]::WriteAllText($entry,$source.OuterXml);$compiler=Join-Path $sdk 'tools/compiler';$exe=Join-Path $compiler 'BinaryAssetBuilder.ManifestInspector.exe';$savedMode=$env:REBORN_DEVELOPER_BUILD_TESTS;$evidence=@()
try{
    foreach($mode in @(0,1)){
        # Reborn: the same inherited process setting used by Studio must reach every native compiler subprocess.
        $env:REBORN_DEVELOPER_BUILD_TESTS=[string]$mode;$run=Join-Path $output ('mode-'+$mode);$null=New-Item -ItemType Directory -Path $run;$log=Join-Path $output ('mode-'+$mode+'.log');$clock=[Diagnostics.Stopwatch]::StartNew()
        & $settings.PowerShell7 -NoProfile -File (Join-Path $PSScriptRoot 'Invoke-Ra3Ep1ConscriptAlphaBuild.ps1') -ModXml $entry -StaticManifest $StaticManifest -GlobalManifest $GlobalManifest -CompilerDirectory $compiler -OutputRoot $run -ValidateOnly *> $log
        $clock.Stop();if($LASTEXITCODE-ne 0){throw ('Build mode '+$mode+' failed; see '+$log)}
        $receipt=Get-Content -LiteralPath (Join-Path $run 'conscript/override/trial-plan.json') -Raw|ConvertFrom-Json
        if(-not $receipt.OfflineValidated -or ($mode-eq 0 -and $receipt.PackageCorruptionsRejected-ne 0) -or ($mode-eq 1 -and $receipt.PackageCorruptionsRejected-le 0)){throw 'Build-mode package verification/test count differs.'}
        $text=Get-Content -LiteralPath $log -Raw;if(($mode-eq 0 -and $text.Contains('W3DContainer stock qualification: PASS')) -or ($mode-eq 1 -and -not $text.Contains('W3DContainer stock qualification: PASS'))){throw 'Optional stock regression did not follow build mode.'}
        $evidence+=[pscustomobject]@{DeveloperBuildTests=[bool]$mode;ElapsedMs=$clock.ElapsedMilliseconds;ArchiveSha256=$receipt.ArchiveSha256;Corruptions=$receipt.PackageCorruptionsRejected}
    }
    if($evidence[0].ArchiveSha256-cne $evidence[1].ArchiveSha256){throw 'Fast and developer builds produced different BIG payloads.'}
    # Reborn: disabling developer tests must never bypass ordinary XML/schema admission.
    $bad=[Xml.XmlDocument]::new();$bad.LoadXml($source.OuterXml);$bad.DocumentElement.FirstChild.SetAttribute('RebornUnsupportedField','true');$invalid=Join-Path $data 'Invalid.xml';[IO.File]::WriteAllText($invalid,$bad.OuterXml)
    foreach($mode in @(0,1)){$env:REBORN_DEVELOPER_BUILD_TESTS=[string]$mode;& $exe conscript-source-prepare $invalid (Join-Path $output ('invalid-'+$mode)) $sdk *> (Join-Path $output ('invalid-'+$mode+'.log'));if($LASTEXITCODE-eq 0){throw 'Invalid source accepted in build mode '+$mode}}
    [IO.File]::WriteAllText((Join-Path $output 'mode-report.json'),($evidence|ConvertTo-Json));Write-Output ('Reborn: build modes: PASS (identical BIG, fast=0/developer>0 corruptions, optional stock tests, invalid XML refused in both modes). '+($evidence|ConvertTo-Json -Compress))
}finally{$env:REBORN_DEVELOPER_BUILD_TESTS=$savedMode}
