# Reborn: prepare a read-only two-case config-consumption experiment with a whitespace-only probe; never launch the game or alter stock files.
[CmdletBinding()]
param([Parameter(Mandatory=$true)][string]$ImagePath,[Parameter(Mandatory=$true)][string]$BaselineImagePath,[Parameter(Mandatory=$true)][string]$SkuDefinitionPath,[Parameter(Mandatory=$true)][string]$LauncherPath,[Parameter(Mandatory=$true)][string]$ConfigPath,[switch]$SelfTest,[switch]$AsJson)
$ErrorActionPreference='Stop'
# Reborn: whitespace content alone is insufficient; guard the post-reader basename identity copy before planning any future native trial.
. (Join-Path $PSScriptRoot 'Ra3Ep11ConfigNamePolicy.ps1')
$smokeSelfTest=$SelfTest;$smokeAsJson=$AsJson
$smokeInventory=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11ConfigInventory.ps1') -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath
$SelfTest=$smokeSelfTest;$AsJson=$smokeAsJson
$smokeLauncher=& (Join-Path $PSScriptRoot 'Get-Ra3Ep11LauncherProfile.ps1') -LauncherPath $LauncherPath

#-------------------------------------------------------------------------------------------------
<# Reborn: admit a short explicit ASCII config path and nonempty whitespace-only content, never an unreviewed config directive. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11ReadOnlyProbe([byte[]]$Bytes,[string]$Path) {
    if(-not [IO.Path]::IsPathFullyQualified($Path) -or $Path.Length-gt 240 -or $Path-match '[^\x20-\x7E]|[";]' -or $Bytes.Length-lt 1 -or $Bytes.Length-gt 64){throw 'Config-read probe exceeds conservative path/content policy.'}
    foreach($value in $Bytes){if($value-notin @(9,10,13,32)){throw 'Config-read probe must contain whitespace only, with no directives or comments.'}}
    Assert-Ep11ConfigNamePolicy $Path
}
# Reborn: reject a relative caller path before normalization can disguise it as an absolute request.
if(-not [IO.Path]::IsPathFullyQualified($ConfigPath)){throw 'Smoke config path must be explicitly absolute.'}
$smokeConfigFull=[IO.Path]::GetFullPath($ConfigPath)
$smokeConfigBytes=Read-RuntimeInput $smokeConfigFull 64
Assert-Ep11ReadOnlyProbe $smokeConfigBytes $smokeConfigFull
$smokeConfigHash=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($smokeConfigBytes))
$smokeRoot=[IO.Path]::GetDirectoryName([IO.Path]::GetFullPath($SkuDefinitionPath))
# Reborn: restored archive availability does not establish a clean stock baseline while known loose overrides remain.
$smokeLooseMetadata=@(foreach($kind in @('bin','imp','manifest','relo')){
    $loosePath=Join-Path $smokeRoot ('Data/mapmetadata.'+$kind)
    if(Test-Path -LiteralPath $loosePath -PathType Leaf){$looseItem=Get-Item -LiteralPath $loosePath;[pscustomobject]@{Path=$looseItem.FullName;Bytes=$looseItem.Length}}
})
$smokeLooseMapsPresent=Test-Path -LiteralPath (Join-Path $smokeRoot 'Data/maps') -PathType Container
$smokeKnownOverrides=($smokeLooseMetadata.Count -gt 0 -or $smokeLooseMapsPresent)
if(-not [IO.Path]::GetFullPath($LauncherPath).Equals([IO.Path]::Combine($smokeRoot,'RA3EP1.exe'),[StringComparison]::OrdinalIgnoreCase)){throw 'Smoke launcher and SKU must belong to the same installation.'}
if($smokeConfigFull.StartsWith($smokeRoot.TrimEnd('\')+'\',[StringComparison]::OrdinalIgnoreCase)){throw 'Config-read probe must remain outside the installed game tree.'}
if($SelfTest){
    # Reborn: exercise detached whitespace content and scope/size faults; no malformed config is passed to a native process.
    foreach($bytes in @([byte[]]@(10),[byte[]]@(9,32,13,10))){Assert-Ep11ReadOnlyProbe $bytes $smokeConfigFull}
    foreach($fault in @(
        [pscustomobject]@{Bytes=[byte[]]::new(0);Path=$smokeConfigFull},
        [pscustomobject]@{Bytes=[byte[]]@(0);Path=$smokeConfigFull},
        [pscustomobject]@{Bytes=[Text.Encoding]::ASCII.GetBytes('set-search-path big:;.');Path=$smokeConfigFull},
        [pscustomobject]@{Bytes=[byte[]]::new(65);Path=$smokeConfigFull},
        [pscustomobject]@{Bytes=[byte[]]@(10);Path='relative.cfg'},
        [pscustomobject]@{Bytes=[byte[]]@(10);Path=$smokeConfigFull+';other'},
        [pscustomobject]@{Bytes=[byte[]]@(10);Path=$smokeConfigFull+'"'},
        [pscustomobject]@{Bytes=[byte[]]@(10);Path=$smokeConfigFull+('x'*240)},
        [pscustomobject]@{Bytes=[byte[]]@(10);Path=$smokeConfigFull+'é'}
    )){$rejected=$false;try{Assert-Ep11ReadOnlyProbe $fault.Bytes $fault.Path}catch{$rejected=$true};if(-not $rejected){throw 'Invalid read-only probe fixture admitted.'}}
}
$smokeConfigAfter=Read-RuntimeInput $smokeConfigFull 64
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($smokeConfigAfter))-cne $smokeConfigHash){throw 'Config-read probe changed during planning.'}
# Reborn: argument arrays are experiment requests, not recovered launcher forwarding; no command string is handed to a shell or process API.
$smokeCases=@(
    [pscustomobject]@{Name='baseline';LauncherPath=[IO.Path]::GetFullPath($LauncherPath);WorkingDirectory=$smokeRoot;RequestedArgumentList=@();ExpectedConfigRead=$null},
    [pscustomobject]@{Name='isolated-config-read';LauncherPath=[IO.Path]::GetFullPath($LauncherPath);WorkingDirectory=$smokeRoot;RequestedArgumentList=@('-modconfig',$smokeConfigFull);ExpectedConfigRead=$smokeConfigFull}
)
$report=[pscustomobject]@{
    ImageSha256=$smokeInventory.ImageSha256;SkuDefinitionPath=[IO.Path]::GetFullPath($SkuDefinitionPath);SkuDefinitionSha256=$smokeInventory.SkuDefinitionSha256
    LauncherPath=$smokeLauncher.LauncherPath;LauncherSha256=$smokeLauncher.LauncherSha256
    RequiredObservedGamePath=[IO.Path]::GetFullPath($ImagePath);ConfigPath=$smokeConfigFull;ConfigBytes=$smokeConfigBytes.Length;ConfigSha256=$smokeConfigHash;ConfigDirectiveCount=0
    ConfiguredArchiveCount=$smokeInventory.ConfiguredArchiveCount;AvailableArchiveCount=$smokeInventory.AvailableArchiveCount;MissingConfiguredArchives=$smokeInventory.MissingConfiguredArchives
    InstallationProfile=$(if($smokeInventory.MissingConfiguredArchives.Count-ne 0){'IncompleteStock-CampaignMissing-NoAlias'}elseif($smokeKnownOverrides){'ConfiguredArchivesPresent-KnownLooseOverrides'}else{'ConfiguredArchivesPresent-StockPurityUnverified'})
    KnownLooseMetadataFiles=$smokeLooseMetadata;LooseMapDirectoryPresent=$smokeLooseMapsPresent;KnownLooseOverridesPresent=$smokeKnownOverrides
    # Reborn: this targeted existence inventory neither authenticates stock files nor measures effective VFS precedence.
    StockBaselineCleanProven=$false;LooseOverridePrecedenceProven=$false
    Cases=$smokeCases;ConfigReadSignalRequired='Attributable successful read of the exact probe by the observed 1.1 process; argument presence or menu alone is insufficient.'
    WhitespaceFixturesExecuted=$(if($SelfTest){2}else{0});PolicyRefusalsExecuted=$(if($SelfTest){9}else{0})
    ReadOnly=$true;TargetExecuted=$false;ArgumentsForwardedProven=$false;ConfigConsumedProven=$false;EffectiveModPathsProven=$false
    AuthoredAssetsIncluded=$false;ModPackageLoaded=$false;ProductionBuildReady=$false;AutomaticLaunchAuthorizedByReport=$false
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 8}else{$report}
