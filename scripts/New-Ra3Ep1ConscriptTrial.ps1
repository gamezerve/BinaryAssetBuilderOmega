# Reborn: export only the pinned source-compiled BuildTime=1 override to a fresh short artifact path; never install or execute a game.
[CmdletBinding()]
param([string]$OutputDirectory,[switch]$CheckOnly,[switch]$SelfTest)
$ErrorActionPreference='Stop'
if($PSVersionTable.PSVersion.Major-lt 7){throw 'PowerShell 7 is required.'}
if($SelfTest -and $CheckOnly){throw 'Choose SelfTest or CheckOnly.'}
$repo=Split-Path -Parent $PSScriptRoot
$artifactRoot=[IO.Path]::GetFullPath((Join-Path $repo 'artifacts'))
$source=Join-Path $artifactRoot 'ConscriptAuthoredTrials-a/gameobject-original-native-temp/Reborn-ConscriptTrials-a4cd4f4b682c4cc6bb6e04fbfa63b410/override'
$archiveHash='3FCDFFB9909B39E9C3A6B75AC50791A19D3EFBAE5A83D6169C174197C50C8FB2'
$sourceHash='B7EDFA37B5F1A26FA2ED5610BFEB2B2F05496B5BF150B9E068F9A4AB066DC6D6'
$handles=[Collections.Generic.List[IDisposable]]::new()
. (Join-Path $PSScriptRoot 'Ra3Ep11ConfigNamePolicy.ps1')
#-------------------------------------------------------------------------------------------------
<# Reborn: reject redirected ancestry and bound each frozen read before allocating; ordinary replacement/writes remain denied until verification/export finishes. #>
#-------------------------------------------------------------------------------------------------
function Read-RebornConscriptFile([string]$Path) {
    for($item=Get-Item -LiteralPath $Path;$null-ne $item;$item=if($item.PSIsContainer){$item.Parent}else{$item.Directory}){if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected Conscript path refused.'}}
    $stream=[IO.File]::Open($Path,[IO.FileMode]::Open,[IO.FileAccess]::Read,[IO.FileShare]::Read);$handles.Add($stream)
    if($stream.Length-lt 1 -or $stream.Length-gt 1048576){throw 'Conscript file exceeds bounds.'}
    $bytes=[byte[]]::new([int]$stream.Length);$stream.ReadExactly($bytes);return ,$bytes
}
#-------------------------------------------------------------------------------------------------
<# Reborn: pin the already source/native/metadata/stream/BIG-qualified override, not a mutable newest-build lookup or an untrusted receipt claim. #>
#-------------------------------------------------------------------------------------------------
function Assert-RebornConscriptBytes($Files,$Receipt) {
    if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Files['RebornConscriptProbe.big']))-cne $archiveHash){throw 'Conscript archive digest differs.'}
    if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Files['trial-source.xml']))-cne $sourceHash){throw 'Conscript source digest differs.'}
    $expected=[Text.Encoding]::ASCII.GetBytes("add-big RebornConscriptProbe.big`n")
    foreach($name in @('reborn_1.0.cfg','reborn_1.0.skudef')){if([Convert]::ToHexString([byte[]]$Files[$name])-cne [Convert]::ToHexString($expected)){throw 'Conscript mod fragment differs.'}}
    if($Receipt.Asset-cne 'GameObject:SovietAntiInfantryInfantry' -or $Receipt.BuildTime-ne 1 -or $Receipt.StreamProfile-cne 'bounded-conscript-trial' -or $Receipt.ArchiveSha256-cne $archiveHash -or $Receipt.SourceSha256-cne $sourceHash -or $Receipt.MetadataPrefix-ne 88824 -or $Receipt.ReferenceOccurrences-ne 275 -or $Receipt.ArchiveEntryCount-ne 15 -or $Receipt.OfflineValidated-ne $true -or $Receipt.NewIdentity-ne $false -or $Receipt.GameExecuted-ne $false -or $Receipt.InstalledFilesModified-ne $false -or $Receipt.RuntimeReady-ne $false){throw 'Conscript receipt profile differs.'}
}
try {
    if([string]::IsNullOrWhiteSpace($OutputDirectory)){if($CheckOnly){throw 'CheckOnly requires an explicit output directory.'};$OutputDirectory=Join-Path $artifactRoot ('Conscript-'+[guid]::NewGuid().ToString('N').Substring(0,8))}
    $destination=[IO.Path]::GetFullPath($OutputDirectory)
    if([IO.Path]::GetDirectoryName($destination)-ine $artifactRoot){throw 'Destination must be one direct child of repository artifacts.'}
    for($item=Get-Item -LiteralPath $artifactRoot;$null-ne $item;$item=$item.Parent){if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected artifact root refused.'}}
    $config=Join-Path $destination 'reborn_1.0.cfg'
    if($config.Length-gt 240 -or $config-match '[^\x20-\x7E]|[";]'){throw 'Conscript config exceeds the conservative path policy.'};Assert-Ep11ConfigNamePolicy $config
    $inputDirectory=if($CheckOnly){$destination}else{$source}
    $files=@{};foreach($name in @('RebornConscriptProbe.big','reborn_1.0.cfg','reborn_1.0.skudef','trial-source.xml','DO_NOT_LAUNCH_YET.txt')){$files[$name]=Read-RebornConscriptFile (Join-Path $inputDirectory $name)}
    $receiptBytes=Read-RebornConscriptFile (Join-Path $inputDirectory 'trial-plan.json');$receipt=[Text.Encoding]::UTF8.GetString($receiptBytes)|ConvertFrom-Json;Assert-RebornConscriptBytes $files $receipt
    if($SelfTest){
        $refusals=0
        foreach($name in @('RebornConscriptProbe.big','reborn_1.0.cfg','reborn_1.0.skudef','trial-source.xml')){$bad=$files.Clone();$bad[$name]=([byte[]]$files[$name]).Clone();$bad[$name][0]=$bad[$name][0]-bxor 1;$rejected=$false;try{Assert-RebornConscriptBytes $bad $receipt}catch{$rejected=$true};if(-not $rejected){throw 'Corrupt export bytes accepted.'};$refusals++}
        $badReceipt=$receipt|ConvertTo-Json -Depth 6|ConvertFrom-Json;$badReceipt.BuildTime=4;$rejected=$false;try{Assert-RebornConscriptBytes $files $badReceipt}catch{$rejected=$true};if(-not $rejected){throw 'Wrong authored receipt accepted.'};$refusals++
        [pscustomobject]@{SelfTestPassed=$true;Refusals=$refusals;GameExecuted=$false;FilesCreated=$false};return
    }
    if(-not $CheckOnly){
        if(Test-Path -LiteralPath $destination){throw 'Existing Conscript export is never overwritten.'}
        $null=New-Item -ItemType Directory -Path $destination
        foreach($name in $files.Keys){$output=[IO.File]::Open((Join-Path $destination $name),[IO.FileMode]::CreateNew,[IO.FileAccess]::Write,[IO.FileShare]::None);try{$output.Write([byte[]]$files[$name])}finally{$output.Dispose()}}
        # Reborn: preserve provenance but replace the obsolete long config location; all runtime/gameplay flags remain false.
        $receipt.ConfigPath=$config;$receipt|Add-Member -NotePropertyName ExportedFrom -NotePropertyValue $source -Force
        $receipt|Add-Member -NotePropertyName ConfigPathLength -NotePropertyValue $config.Length -Force
        $json=[Text.Encoding]::UTF8.GetBytes(($receipt|ConvertTo-Json -Depth 6));$output=[IO.File]::Open((Join-Path $destination 'trial-plan.json'),[IO.FileMode]::CreateNew,[IO.FileAccess]::Write,[IO.FileShare]::None);try{$output.Write($json)}finally{$output.Dispose()}
    }
    $readback=@{};foreach($name in $files.Keys){$readback[$name]=Read-RebornConscriptFile (Join-Path $destination $name);if([Convert]::ToHexString([byte[]]$readback[$name])-cne [Convert]::ToHexString([byte[]]$files[$name])){throw 'Conscript export changed bytes.'}}
    $exported=[Text.Encoding]::UTF8.GetString((Read-RebornConscriptFile (Join-Path $destination 'trial-plan.json')))|ConvertFrom-Json;Assert-RebornConscriptBytes $readback $exported
    if($exported.ConfigPath-cne $config){throw 'Export receipt points elsewhere.'}
    [pscustomobject]@{ExportValidated=$true;CheckOnly=[bool]$CheckOnly;Asset=$exported.Asset;BuildTime=1;ArchiveSha256=$archiveHash;OutputDirectory=$destination;ConfigPath=$config;ConfigPathLength=$config.Length;GameExecuted=$false;InstalledFilesModified=$false;RuntimeReady=$false}
}finally{foreach($handle in $handles){$handle.Dispose()}}
