# Reborn: reproduce the narrow XML-to-EP1 armor diagnostic path and two bounded stock comparisons; never install a package or launch a game.
[CmdletBinding()]
param(
    [string]$StaticManifest='D:\TEMP\Red Alert 3 Uprising Source Data\Static Data\data\static.manifest',
    [string]$WorldBuilderManifest='D:\TEMP\Red Alert 3 Uprising Source Data\WorldBuilder\data\worldbuilder.manifest'
)
$ErrorActionPreference='Stop'
if($PSVersionTable.PSVersion.Major -lt 7){throw 'Armor PoC requires PowerShell 7.'}
$repo=Split-Path -Parent $PSScriptRoot
foreach($manifest in @($StaticManifest,$WorldBuilderManifest)){
    if(-not [IO.Path]::IsPathFullyQualified($manifest) -or -not (Test-Path -LiteralPath $manifest -PathType Leaf)){
        throw 'Both explicit unpacked stock manifest paths must exist; no fixture-only fallback.'
    }
}
$project=Join-Path $repo 'source/BinaryAssetBuilder.ManifestInspector/BinaryAssetBuilder.ManifestInspector.csproj'
$executable=Join-Path $repo 'source/BinaryAssetBuilder.ManifestInspector/bin/x86/Release/net8.0/BinaryAssetBuilder.ManifestInspector.exe'
$directory=Join-Path $repo ('artifacts/RebornArmorPoc-'+[guid]::NewGuid().ToString('N'))
$null=New-Item -ItemType Directory -Path $directory
$steps=[Collections.Generic.List[object]]::new()
$failure=$null

#-------------------------------------------------------------------------------------------------
<# Reborn: record an existing local build/test command and fail immediately on a nonzero exit; no runtime/game readiness is inferred. #>
#-------------------------------------------------------------------------------------------------
function Invoke-RebornArmorPocStep([string]$Name,[string]$File,[string[]]$Arguments) {
    $log=Join-Path $directory ($Name+'.log')
    $output=@(& $File @Arguments 2>&1)
    $code=$LASTEXITCODE
    [IO.File]::WriteAllText($log,(($output|ForEach-Object {$_.ToString()}) -join [Environment]::NewLine))
    $steps.Add([pscustomobject]@{Name=$Name;ExitCode=$code;Log=$log})
    if($code -ne 0){throw ('Armor PoC step failed: '+$Name+'; see '+$log)}
}
try{
    # Reborn: rebuild current source with cached dependencies only; do not download packages or trust an old inspector binary.
    Invoke-RebornArmorPocStep 'build' (Get-Command dotnet -ErrorAction Stop).Source @('build',$project,'--no-restore','-c','Release','-p:Platform=x86','-m:1','-nr:false')
    Invoke-RebornArmorPocStep 'profile' $executable @('ep1-armor-profile-self-test')
    Invoke-RebornArmorPocStep 'document' $executable @('ep1-armor-document-self-test',(Join-Path $directory 'document'),$StaticManifest)
    Invoke-RebornArmorPocStep 'static-golden' $executable @('armor-token-self-test',(Join-Path $directory 'static'),$StaticManifest)
    Invoke-RebornArmorPocStep 'worldbuilder-golden' $executable @('armor-token-self-test',(Join-Path $directory 'worldbuilder'),$WorldBuilderManifest)
}catch{$failure=$_.Exception.Message}finally{
    # Reborn: hash the actual generated streams for reproducibility, not as proof of native loader acceptance.
    $outputs=@(Get-ChildItem -LiteralPath $directory -Recurse -File|Where-Object {$_.Extension -in @('.manifest','.bin','.relo','.imp')}|ForEach-Object {
        [pscustomobject]@{Path=$_.FullName;Bytes=$_.Length;Sha256=(Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash}
    })
    # Reborn: distinguish current-Core diagnostic streams from the older fixed-checksum golden fixtures; neither is production output.
    $coreDirectories=@('core-empty','core-repeat','core-edited','core-populated')|ForEach-Object {Join-Path $directory ('document/'+$_)}
    $coreOutputsPresent=@($coreDirectories|Where-Object {Test-Path -LiteralPath (Join-Path $_ 'diagnostic.manifest') -PathType Leaf}).Count -eq 4
    # Reborn: identify actual committed armor artifacts separately from the older standalone diagnostic serializer.
    $managedDirectories=@(Get-ChildItem -LiteralPath (Join-Path $directory 'document') -Directory -ErrorAction SilentlyContinue|Where-Object {$_.Name -like 'RebornArmorOutput-*'}|ForEach-Object {$_.FullName})
    $managedOutputsPresent=$managedDirectories.Count -eq 1 -and (Test-Path -LiteralPath (Join-Path $managedDirectories[0] 'armor.manifest') -PathType Leaf)
    # Reborn: archive existence is evidence only after the document step's exact payload and negative package controls pass.
    $packages=@(Get-ChildItem -LiteralPath $directory -Recurse -Filter RebornArmorProbe.big -File|ForEach-Object {
        [pscustomobject]@{Path=$_.FullName;Bytes=$_.Length;Sha256=(Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash}
    })
    $report=[pscustomobject]@{
        Scope='ArmorTemplate diagnostic XML/document/profile and independent stock-byte comparisons'
        CreatedUtc=[DateTime]::UtcNow.ToString('o');OutputDirectory=$directory;Steps=$steps.ToArray();Failure=$failure
        AllStepsPassed=($null -eq $failure -and $steps.Count -eq 5);StaticManifest=[IO.Path]::GetFullPath($StaticManifest)
        WorldBuilderManifest=[IO.Path]::GetFullPath($WorldBuilderManifest);GeneratedStreams=$outputs
        ProductionOutputEnabled=$false;AuthenticProcessingHashProven=$false;GameExecuted=$false;ModLoaded=$false
        CoreIdentityStreamsValidated=($null -eq $failure -and $steps.Count -eq 5 -and $coreOutputsPresent);CoreStreamDirectories=$coreDirectories
        OutputManagerCommitLinkValidated=($null -eq $failure -and $steps.Count -eq 5 -and $managedOutputsPresent);OutputManagerDirectories=$managedDirectories
        DiagnosticArmorPackageValidated=($null -eq $failure -and $steps.Count -eq 5 -and $packages.Count -eq 1);DiagnosticPackages=$packages
        LaunchConfigurationPrepared=$false;BehaviorOverridePrepared=($null -eq $failure -and $steps.Count -eq 5 -and $packages.Count -eq 1)
        OverrideTarget='ArmorTemplate:AlliedConYardArmor';OverrideDamage='GUN';StockPercent=100;OverridePercent=5000
        OverrideAppliedInGame=$false
        NextGate='Prepare and verify effective stream/config loading route, then obtain separate game-test authorization; the AlliedConYardArmor GUN override is prepared but not proven loaded.'
    }
    [IO.File]::WriteAllText((Join-Path $directory 'readiness.json'),($report|ConvertTo-Json -Depth 6))
}
if($null -ne $failure){throw $failure}
$report
