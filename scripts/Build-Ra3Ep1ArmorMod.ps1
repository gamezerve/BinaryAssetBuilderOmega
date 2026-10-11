# Reborn: reproduce the fixed user-tested ConYard XML-to-default-mod package recipe; never install files or launch the game.
[CmdletBinding()]
param(
    [string]$StaticManifest='D:\TEMP\Red Alert 3 Uprising Source Data\Static Data\data\static.manifest',
    [string]$WorldBuilderManifest='D:\TEMP\Red Alert 3 Uprising Source Data\WorldBuilder\data\worldbuilder.manifest',
    [string]$GameRoot='D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising',
    [switch]$SelfTest
)
$ErrorActionPreference='Stop'
if($PSVersionTable.PSVersion.Major-lt 7){throw 'PowerShell 7 is required.'}
$repo=Split-Path -Parent $PSScriptRoot
$validatedDigest='DD7DE09E3A81B9714DC2861ADE0AA9ECDFE72A1851EDD1ED20BE2155EF31C2DC'
#-------------------------------------------------------------------------------------------------
<# Reborn: require the exact previously user-tested recipe, while refusing to inherit runtime success for a newly generated package. #>
#-------------------------------------------------------------------------------------------------
function Assert-RebornArmorRecipe($Candidate) {
    if($Candidate.CandidatePrepared-isnot [bool] -or -not $Candidate.CandidatePrepared -or
        $Candidate.OfflinePackageValidated-isnot [bool] -or -not $Candidate.OfflinePackageValidated -or
        $Candidate.ConfigContentsValidated-isnot [bool] -or -not $Candidate.ConfigContentsValidated -or
        $Candidate.StreamBytesUnchanged-isnot [bool] -or -not $Candidate.StreamBytesUnchanged -or
        $Candidate.Route-cne 'default-mod-reader' -or $Candidate.StreamStem-cne 'data\mod' -or
        $null-ne $Candidate.ManifestQueueArgument -or $Candidate.VersionMarkerHex-cne '0D0A' -or
        $Candidate.ArchiveEntryCount-ne 15 -or $Candidate.ArchiveBytes-ne 3552 -or
        $Candidate.ArchiveSha256-cne $validatedDigest -or
        (@($Candidate.StreamVariants)-join '|')-cne '|_l|_m') {throw 'Build differs from the user-tested fixed armor recipe.'}
    foreach($claim in @('GameExecuted','OverrideAppliedInGame','ConfigConsumptionProven','ManifestLoadProven','RuntimeReady')){
        if($Candidate.$claim-isnot [bool] -or $Candidate.$claim){throw "New package cannot inherit runtime claim: $claim"}
    }
}
if($SelfTest){
    # Reborn: detached admission tests never invoke the compiler, inspect an installation or query processes.
    $valid=[pscustomobject]@{CandidatePrepared=$true;OfflinePackageValidated=$true;ConfigContentsValidated=$true;StreamBytesUnchanged=$true;Route='default-mod-reader';StreamStem='data\mod';ManifestQueueArgument=$null;VersionMarkerHex='0D0A';ArchiveEntryCount=15;ArchiveBytes=3552;ArchiveSha256=$validatedDigest;StreamVariants=@('','_l','_m');GameExecuted=$false;OverrideAppliedInGame=$false;ConfigConsumptionProven=$false;ManifestLoadProven=$false;RuntimeReady=$false}
    Assert-RebornArmorRecipe $valid
    $faults=@{Route='config-manifest-queue';ArchiveSha256=('0'*64);ArchiveEntryCount=12;ManifestQueueArgument='mod.manifest';VersionMarkerHex='5F6D6F64';StreamVariants=@('','_m','_l');CandidatePrepared='true';OverrideAppliedInGame=$true;GameExecuted=$true;RuntimeReady=$null}
    foreach($field in $faults.Keys){
        $copy=$valid.PSObject.Copy();$copy.$field=$faults[$field];$refused=$false
        try{Assert-RebornArmorRecipe $copy}catch{$refused=$true}
        if(-not $refused){throw "Recipe fault accepted: $field"}
    }
    'Armor build recipe self-test: PASS (one positive, ten refusals; no build or game).'
    return
}
$fixture=Join-Path $repo 'tests/fixtures/AlliedConYardArmorProbe.xml'
$sourceHash=(Get-FileHash -LiteralPath $fixture -Algorithm SHA256).Hash
# Reborn: start from current authored XML and a fresh scoped OutputManager build, not a previous package or cached readiness file.
$poc=& (Join-Path $PSScriptRoot 'Invoke-Ra3Ep1ArmorPoc.ps1') -StaticManifest $StaticManifest -WorldBuilderManifest $WorldBuilderManifest
if($poc.AllStepsPassed-isnot [bool] -or -not $poc.AllStepsPassed){throw 'Armor compilation or stock comparison failed.'}
$readiness=Join-Path $poc.OutputDirectory 'readiness.json'
$prepared=& (Join-Path $PSScriptRoot 'New-Ra3Ep1ArmorTrial.ps1') -DefaultReader -PocReport $readiness -GameRoot $GameRoot
$candidate=$prepared.Candidate
Assert-RebornArmorRecipe $candidate
if((Get-FileHash -LiteralPath $fixture -Algorithm SHA256).Hash-cne $sourceHash){throw 'Authored XML changed during build.'}
if((Get-FileHash -LiteralPath $candidate.ArchivePath -Algorithm SHA256).Hash-cne $validatedDigest){throw 'Generated package changed before receipt.'}
# Reborn: record reproducibility and historical user evidence separately from this build's untested runtime state.
$receipt=[pscustomobject]@{
    Scope='Fixed AlliedConYardArmor GUN-5000 reproducer; not an arbitrary XML compiler'
    CreatedUtc=[DateTime]::UtcNow.ToString('o');SourceXml=$fixture;SourceXmlSha256=$sourceHash
    PocReport=$readiness;PreparationReport=(Join-Path $candidate.OutputDirectory 'preparation.json')
    PackagePath=$candidate.ArchivePath;ConfigPath=$candidate.ConfigPath;ModSkuPath=$candidate.ModSkuPath
    PackageSha256=$validatedDigest;MatchesUserTestedPackageBytes=$true
    HistoricalBehaviorEvidence='artifacts/ArmorLaunch-c550944c19c8488ab90d16d4eaaa72cd/user-observation.json'
    BuildAndStockComparisonsPassed=$true;PackageControlsPassed=$true
    ThisBuildGameExecuted=$false;ThisBuildOverrideObserved=$false;InstalledFilesModified=$false;GeneralProductionOutputEnabled=$false
}
$receiptPath=Join-Path $candidate.OutputDirectory 'build-receipt.json'
[IO.File]::WriteAllText($receiptPath,($receipt|ConvertTo-Json -Depth 5))
$receipt|Add-Member -NotePropertyName ReceiptPath -NotePropertyValue $receiptPath
$receipt
