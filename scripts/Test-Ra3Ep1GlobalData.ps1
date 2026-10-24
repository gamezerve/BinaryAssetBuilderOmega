# Reborn: exercise global/static linking and Studio publication only in the restored developer slot; never build the user's Reborn output.
[CmdletBinding()]
param([Parameter(Mandatory)][string]$OutputDirectory)
$ErrorActionPreference='Stop';$repo=Split-Path -Parent $PSScriptRoot;$sdk=Join-Path $repo 'alpha';$work=[IO.Path]::GetFullPath($OutputDirectory)
if([IO.Path]::GetDirectoryName($work)-ine (Join-Path $sdk '.work') -or (Test-Path -LiteralPath $work)){throw 'Use a fresh direct child of alpha/.work.'}
$null=New-Item -ItemType Directory -Path $work
$data=Join-Path $sdk 'mods/RebornTest/Data';$saved=Join-Path $work 'original-data';$runtime=(Get-Process -Id $PID).Path;$driver=Join-Path $PSScriptRoot 'Invoke-Ra3Ep1DevAlphaBuild.ps1'
$regression=[IO.File]::Open((Join-Path $sdk '.work/regression.lock'),[IO.FileMode]::OpenOrCreate,[IO.FileAccess]::ReadWrite,[IO.FileShare]::None)
$rows=[Collections.Generic.List[object]]::new();$receipt=Join-Path $sdk 'builtmods/RebornTest/build.json'
# Reborn: hash every user source so read-only use of retail-style reference files is independently checked.
$userHashes=@{};Get-ChildItem -LiteralPath (Join-Path $sdk 'mods/Reborn') -File -Recurse | ForEach-Object {$userHashes[$_.FullName]=(Get-FileHash -LiteralPath $_.FullName).Hash}
$moved=$false

#-------------------------------------------------------------------------------------------------
<# Reborn: read the archive directory and selected manifests/markers independently of the packaging implementation. #>
#-------------------------------------------------------------------------------------------------
function Read-TestBig([string]$Path) {
    $reader=[IO.BinaryReader]::new([IO.File]::OpenRead($Path));$entries=@{}
    try{
        if([Text.Encoding]::ASCII.GetString($reader.ReadBytes(4))-cne 'BIG4'){throw 'Expected BIG4.'};$null=$reader.ReadUInt32()
        $count=Read-TestWord $reader;$null=Read-TestWord $reader
        for($i=0;$i-lt $count;$i++){$offset=Read-TestWord $reader;$size=Read-TestWord $reader;$name='';while($c=$reader.ReadByte()){$name+=[char]$c};$position=$reader.BaseStream.Position;$reader.BaseStream.Position=$offset;$entries[$name]=$reader.ReadBytes($size);$reader.BaseStream.Position=$position}
    }finally{$reader.Dispose()}
    return $entries
}

#-------------------------------------------------------------------------------------------------
<# Reborn: BIG4 directory integers are big endian, unlike PowerShell's native UInt32 conversion. #>
#-------------------------------------------------------------------------------------------------
function Read-TestWord($Reader){$bytes=$Reader.ReadBytes(4);[Array]::Reverse($bytes);return [BitConverter]::ToUInt32($bytes,0)}

#-------------------------------------------------------------------------------------------------
<# Reborn: each case uses the actual Studio backend and an isolated final publication root. #>
#-------------------------------------------------------------------------------------------------
function Invoke-GlobalCase([string]$Name,[int]$Enabled,[bool]$Success,[switch]$Validate) {
    $arguments=@('-NoProfile','-File',$driver,'-ModName','RebornTest','-BuildGlobalData',$Enabled,'-FinalModsRoot',(Join-Path $work 'published'))
    if($Validate){$arguments+='-ValidateOnly'}
    & $runtime @arguments > (Join-Path $work ($Name+'.log')) 2>&1
    if(($LASTEXITCODE-eq 0)-ne $Success){throw ('Unexpected global test result: '+$Name+'; see '+(Join-Path $work ($Name+'.log')))}
    $rows.Add([pscustomobject]@{Case=$Name;Passed=$true})
}

try{
    # Reborn: verify the resolved cleanup target before moving/restoring only the designated developer Data directory.
    if([IO.Path]::GetFullPath($data)-ine (Join-Path $sdk 'mods/RebornTest/Data')){throw 'Unexpected developer data path.'}
    foreach($item in @(Get-Item -LiteralPath $data)+@(Get-ChildItem -LiteralPath $data -Recurse -Force)){if($item.Attributes-band [IO.FileAttributes]::ReparsePoint){throw 'Redirected test source refused.'}}
    [IO.Directory]::Move($data,$saved);$moved=$true
    $null=New-Item -ItemType Directory -Path (Join-Path $data 'AdditionalMaps'),(Join-Path $data 'Sounds')
    Copy-Item -LiteralPath (Join-Path $repo 'tests/fixtures/live-alpha/ReactorAudioCandidate/Data/Sounds/RebornReactorAudio.xml') -Destination (Join-Path $data 'Sounds/RebornAudio.xml')
    # Reborn: copied EA leaves keep relative instance bases, including un-emitted AudioEventOverridable siblings in the retail base file.
    $eventPath=Join-Path $data 'Sounds/RebornAudio.xml';$events=[Xml.XmlDocument]::new();$events.Load($eventPath)
    $includes=$events.CreateElement('Includes','uri:ea.com:eala:asset');$include=$events.CreateElement('Include','uri:ea.com:eala:asset');$include.SetAttribute('type','instance');$include.SetAttribute('source','BaseSoundEffect.xml');$null=$includes.AppendChild($include);$null=$events.DocumentElement.PrependChild($includes)
    $events.SelectSingleNode('//*[local-name()="AudioEvent"]').SetAttribute('inheritFrom','AudioEvent:BaseSoundEffect');$events.Save($eventPath)
    $audio=Join-Path $data 'AdditionalMaps/MapMetaData_Audio.xml';$upgrade=Join-Path $data 'AdditionalMaps/MapMetaData_Upgrades.xml';$entry=Join-Path $data 'Static_Mod.xml'
    [IO.File]::WriteAllText($audio,'<!-- Reborn: nested DATA alias must resolve against the enclosing mod Data root. --><AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes><Include type="all" source="DATA:Sounds/RebornAudio.xml"/></Includes></AssetDeclaration>')
    [IO.File]::WriteAllText($upgrade,'<!-- Reborn: static imports a new identity emitted only in the early global stream. --><AssetDeclaration xmlns="uri:ea.com:eala:asset"><UpgradeTemplate id="RebornGlobalTestUpgrade" Type="OBJECT"/></AssetDeclaration>')
    $static='<!-- Reborn: no global source include is needed to reference a compiled global asset. --><AssetDeclaration xmlns="uri:ea.com:eala:asset"><LogicCommand id="RebornGlobalUpgradeCommand" Type="OBJECT_UPGRADE"><Upgrade>RebornGlobalTestUpgrade</Upgrade></LogicCommand><LogicCommandSet id="RebornGlobalCommandSet"><Cmd>RebornGlobalUpgradeCommand</Cmd></LogicCommandSet></AssetDeclaration>'
    [IO.File]::WriteAllText($entry,$static)
    Invoke-GlobalCase Enabled 1 $true
    $built=Get-Content -LiteralPath $receipt -Raw | ConvertFrom-Json;$big=Read-TestBig $built.FinalPackage.StreamsPath
    if($big.Count-ne 25 -or $built.GlobalData.Streams.Count-ne 2 -or ($built.GlobalData.Streams|Measure-Object AssetCount -Sum).Sum-ne 3 -or $built.Core.AssetCount-ne 2){throw 'Separate global/static asset counts differ.'}
    foreach($stem in @('MapMetaData_Audio','MapMetaData_Upgrades')){foreach($extension in @('.manifest','.bin','.relo','.imp','.version')){if(-not $big.ContainsKey('data\additionalmaps\'+$stem+$extension)){throw 'Missing global archive family.'}};if([Text.Encoding]::ASCII.GetString($big['data\additionalmaps\'+$stem+'.version'])-cne "`r`n"){throw 'Unexpected global version suffix.'}}
    $staticManifest=[Text.Encoding]::ASCII.GetString($big['data\mod.manifest'])
    # Reborn: the loader discovers globals before static subsystems; an audio-only global must depend on audio, never force static loading.
    $audioManifest=[Text.Encoding]::ASCII.GetString($big['data\additionalmaps\MapMetaData_Audio.manifest'])
    # Reborn: pin the restored compatibility baseline only; this offline check does not prove custom audio playback or runtime reader ownership.
    if($audioManifest-match 'static.manifest' -or $audioManifest-notmatch "`u{0001}data\\audio\.manifest`0"){throw 'Global audio differs from the restored compatibility baseline.'}
    if($staticManifest-notmatch "`u{0001}data\\additionalmaps\\MapMetaData_Upgrades\.manifest`0" -or $staticManifest-match 'AudioEvent:SOV_RebornSuperReactor'){throw 'Static/global references differ from the restored compatibility baseline.'}
    $good=(Get-FileHash -LiteralPath $built.FinalPackage.StreamsPath).Hash
    Invoke-GlobalCase ValidateOnly 1 $true -Validate
    if((Get-FileHash -LiteralPath $built.FinalPackage.StreamsPath).Hash-cne $good){throw 'Validation-only changed published output.'}
    # Reborn: failures in phase five and an unchecked missing dependency must preserve the last successful final package.
    [IO.File]::WriteAllText($audio,'<!-- Reborn: intentional malformed XML control. --><AssetDeclaration>')
    Invoke-GlobalCase BadGlobal 1 $false
    if((Get-Content -LiteralPath (Join-Path $work 'BadGlobal.log') -Raw)-notmatch 'Step 5: Building global data'){throw 'Global failure phase missing.'}
    Invoke-GlobalCase DisabledMissingGlobal 0 $false
    if((Get-FileHash -LiteralPath $built.FinalPackage.StreamsPath).Hash-cne $good){throw 'Failed global/static compilation replaced last-good output.'}
    [IO.File]::WriteAllText($entry,$static.Replace('RebornGlobalTestUpgrade','Upgrade_AlliedTech2'))
    Invoke-GlobalCase Disabled 0 $true
    $disabled=Get-Content -LiteralPath $receipt -Raw|ConvertFrom-Json
    if((Read-TestBig $disabled.FinalPackage.StreamsPath).Count-ne 15 -or $disabled.GlobalData.Enabled){throw 'Unchecked toggle packaged stale global streams.'}
    # Reborn: no AdditionalMaps directory is a valid ordinary static-only build with the checkbox checked.
    $globalDirectory=Join-Path $data 'AdditionalMaps';if([IO.Path]::GetDirectoryName([IO.Path]::GetFullPath($globalDirectory))-ine $data){throw 'Unexpected temporary source cleanup.'};Remove-Item -LiteralPath $globalDirectory -Recurse -Force
    Invoke-GlobalCase NoGlobalSources 1 $true
    $absent=Get-Content -LiteralPath $receipt -Raw|ConvertFrom-Json;if((Read-TestBig $absent.FinalPackage.StreamsPath).Count-ne 15 -or @($absent.GlobalData.Streams).Count){throw 'Absent global directory changed static publication.'}
    [IO.File]::WriteAllText((Join-Path $work 'report.json'),(ConvertTo-Json -InputObject @($rows) -Depth 5));Write-Output ('Reborn: global data: PASS ('+$rows.Count+' cases; separate audio/upgrade streams, cross-stream imports, toggle, no-input, validation-only and failed-publication controls).')
}finally{
    try{if($moved){$resolved=[IO.Path]::GetFullPath($data);if($resolved-ine (Join-Path $sdk 'mods/RebornTest/Data')){throw 'Unexpected restore target.'};foreach($item in @(Get-Item -LiteralPath $resolved)+@(Get-ChildItem -LiteralPath $resolved -Recurse -Force)){if($item.Attributes-band [IO.FileAttributes]::ReparsePoint){throw 'Redirected test cleanup refused.'}};Remove-Item -LiteralPath $resolved -Recurse -Force;[IO.Directory]::Move($saved,$resolved)};foreach($path in $userHashes.Keys){if((Get-FileHash -LiteralPath $path).Hash-cne $userHashes[$path]){throw 'User Reborn source changed.'}}}finally{$regression.Dispose()}
}
