# Reborn: prepare a workspace-only ConYard trial config/SKU and three-variant archive; there is intentionally no Run switch.
[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$PocReport,
    # Reborn: opt into the separate default-mod-reader experiment without changing the original queue candidate.
    [switch]$DefaultReader,
    [string]$GameRoot='D:\Program Files (x86)\Steam\steamapps\common\Command and Conquer Red Alert 3 Uprising'
)
$ErrorActionPreference='Stop'
if($PSVersionTable.PSVersion.Major -lt 7){throw 'Trial preparation requires PowerShell 7.'}
$repo=Split-Path -Parent $PSScriptRoot
. (Join-Path $PSScriptRoot 'Ra3Ep11ConfigNamePolicy.ps1')
#-------------------------------------------------------------------------------------------------
<# Reborn: reject filesystem redirection and oversized reports before reading the selected diagnostic evidence. #>
#-------------------------------------------------------------------------------------------------
function Assert-RebornTrialPath([string]$Path) {
    if(-not [IO.Path]::IsPathFullyQualified($Path)){throw 'Trial paths must be absolute.'}
    for($item=Get-Item -LiteralPath $Path;$null -ne $item;$item=if($item.PSIsContainer){$item.Parent}else{$item.Directory}){
        if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected trial path refused.'}
    }
}
$PocReport=[IO.Path]::GetFullPath($PocReport)
Assert-RebornTrialPath $PocReport
if((Get-Item -LiteralPath $PocReport).Length-gt 1MB){throw 'PoC report exceeds one MiB.'}
$proof=Get-Content -LiteralPath $PocReport -Raw|ConvertFrom-Json
foreach($field in @('AllStepsPassed','BehaviorOverridePrepared','DiagnosticArmorPackageValidated','OutputManagerCommitLinkValidated')){
    if($proof.$field -isnot [bool] -or -not $proof.$field){throw "Required PoC evidence absent: $field"}
}
if($proof.OverrideTarget-cne 'ArmorTemplate:AlliedConYardArmor' -or $proof.OverrideDamage-cne 'GUN' -or $proof.OverridePercent-ne 5000 -or
    @($proof.DiagnosticPackages).Count-ne 1 -or @($proof.Steps).Count-ne 5 -or @($proof.Steps|Where-Object {$_.ExitCode-ne 0}).Count-ne 0){throw 'ConYard PoC profile differs.'}
$package=[IO.Path]::GetFullPath($proof.DiagnosticPackages[0].Path)
$artifactRoot=[IO.Path]::GetFullPath((Join-Path $repo 'artifacts'))+[IO.Path]::DirectorySeparatorChar
if(-not $PocReport.StartsWith($artifactRoot,[StringComparison]::OrdinalIgnoreCase) -or -not $package.StartsWith($artifactRoot,[StringComparison]::OrdinalIgnoreCase)){throw 'Select this repository own PoC artifacts.'}
Assert-RebornTrialPath $package
if((Get-Item -LiteralPath $package).Length-gt 1MB -or (Get-FileHash -LiteralPath $package -Algorithm SHA256).Hash-cne $proof.DiagnosticPackages[0].Sha256){throw 'PoC package digest differs.'}
# Reborn: pin the previously reviewed launcher, selected game and stock SKU read-only; never repair or replace installation files.
$pins=@{
    'RA3EP1.exe'='07694EBBCF21232B1A1B401C07ABC2CFB1EDB9F50FA8D0C36DA0F94820943A4A'
    'Data\ra3ep1_1.1.game'='B1DA83EFD229570BAB9DE3A14E48B2D1E6128E302BD52384858FA8A413AE330B'
    'RA3EP1_english_1.1.SkuDef'='416C0752EA6891F13916D60FB1C8C3B8E5F342F7E919214866FC5D6EE962D75E'
}
Assert-RebornTrialPath $GameRoot
foreach($relative in $pins.Keys){
    $path=Join-Path $GameRoot $relative; Assert-RebornTrialPath $path
    if((Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash-cne $pins[$relative]){throw "Reviewed installation identity differs: $relative"}
}
$sku=Join-Path $GameRoot 'RA3EP1_english_1.1.SkuDef'
$stockArchives=@(Get-Content -LiteralPath $sku|Where-Object {$_ -like 'add-big *'}|ForEach-Object {
    $relative=$_.Substring(8);$path=Join-Path $GameRoot $relative
    Assert-RebornTrialPath $path
    [pscustomobject]@{RelativePath=$relative;Bytes=(Get-Item -LiteralPath $path).Length}
})
foreach($relative in @('Data\maps','Data\mapmetadata.bin','Data\mapmetadata.imp','Data\mapmetadata.manifest','Data\mapmetadata.relo')){
    if(Test-Path -LiteralPath (Join-Path $GameRoot $relative)){throw "Known loose override returned: $relative"}
}
# Reborn: rebuild current local source offline rather than trust a stale preparation binary.
$output=@(& (Get-Command dotnet).Source build (Join-Path $repo 'source/BinaryAssetBuilder.ManifestInspector/BinaryAssetBuilder.ManifestInspector.csproj') --no-restore -c Release -p:Platform=x86 -m:1 -nr:false -v:q 2>&1)
if($LASTEXITCODE-ne 0){$output;throw 'Trial helper build failed.'}
$executable=Join-Path $repo 'source/BinaryAssetBuilder.ManifestInspector/bin/x86/Release/net8.0/BinaryAssetBuilder.ManifestInspector.exe'
$directory=Join-Path $repo ('artifacts/'+$(if($DefaultReader){'RebornArmorDefault-'}else{'RebornArmorTrial-'})+[guid]::NewGuid().ToString('N'))
$config=Join-Path $directory 'reborn_1.0.cfg'; Assert-Ep11ConfigNamePolicy $config
$prepareCommand=if($DefaultReader){'armor-default-trial-prepare'}else{'armor-trial-prepare'}
$controlCommand=if($DefaultReader){'armor-default-trial-controls'}else{'armor-trial-controls'}
$json=@(& $executable $prepareCommand $package $directory)
if($LASTEXITCODE-ne 0){throw 'Trial candidate preparation failed; inspect retained output.'}
$controls=@(& $executable $controlCommand $package $directory)
if($LASTEXITCODE-ne 0){throw 'Trial detached controls failed; do not launch.'}
$plan=$json-join [Environment]::NewLine|ConvertFrom-Json
# Reborn: preserve distinct static/offline versus runtime claims and emit launch arguments as data only, never execute them.
$report=[pscustomobject]@{
    Candidate=$plan;PocReport=$PocReport;GameRoot=$GameRoot;LauncherPath=(Join-Path $GameRoot 'RA3EP1.exe')
    ExpectedChildPath=(Join-Path $GameRoot 'Data\ra3ep1_1.1.game');StockSkuPath=$sku;ReviewedPins=$pins;StockArchives=$stockArchives
    BuildPassed=$true;DetachedControlsPassed=$true;Controls=$controls;ConfigNameProfile=(Get-Ep11ConfigNameProfile $config)
    StockArchivePresenceChecked=$true;StockArchivePayloadAuthenticityProven=$false;KnownLooseOverridesAbsent=$true
    InstalledFilesModified=$false;GameExecuted=$false;RuntimeReady=$false
    RuntimePreflightMustBeRepeated=$true;ExistingProcessesChecked=$false;ConfigConsumptionProven=$false;OverrideAppliedInGame=$false
}
[IO.File]::WriteAllText((Join-Path $directory 'preparation.json'),($report|ConvertTo-Json -Depth 8))
[IO.File]::WriteAllText((Join-Path $directory 'build.log'),(($output|ForEach-Object {$_.ToString()})-join [Environment]::NewLine))
$report
