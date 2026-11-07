# Reborn: exercise real Studio-driver builds in the owned developer slot and restore its source/output afterwards.
param([string]$Repo=(Split-Path -Parent $PSScriptRoot))
$ErrorActionPreference='Stop'
$sdk=Join-Path $Repo 'alpha';$run=Join-Path $sdk ('.work/persistent-stage-tests-'+[guid]::NewGuid().ToString('N'))
$null=New-Item -ItemType Directory -Path $run
$mod=Join-Path $sdk 'mods/RebornTest';$cache=Join-Path $sdk 'builtmods/RebornTest'
$backupMod=Join-Path $run 'original-mod';$backupCache=Join-Path $run 'original-cache'
foreach($path in @($mod,$cache,$backupMod,$backupCache)){
    $absolute=[IO.Path]::GetFullPath($path)
    if(!$absolute.StartsWith($sdk+'\',[StringComparison]::OrdinalIgnoreCase)){throw 'Test path escaped SDK.'}
}
if(Test-Path -LiteralPath $mod){[IO.Directory]::Move($mod,$backupMod)}
if(Test-Path -LiteralPath $cache){[IO.Directory]::Move($cache,$backupCache)}
$driver=Join-Path $Repo 'scripts/Invoke-Ra3Ep1DevAlphaBuild.ps1';$pwsh=Join-Path $sdk 'runtime/powershell/pwsh.exe'
$final=Join-Path $run 'final';$script:case=0
#-------------------------------------------------------------------------------------------------
<# Reborn: each case uses a separate worker, so successful reuse cannot depend on process-local memory. #>
#-------------------------------------------------------------------------------------------------
function Build([int]$Static,[int]$Global,[int]$UI,[int]$Big,[bool]$Success=$true,[int]$Maps=0,[int]$Silent=1,[bool]$Validate=$false){
    $script:case++;$log=Join-Path $run ('case-'+$script:case+'.log')
    $validation=@();if($Validate){$validation+='-ValidateOnly'}
    & $pwsh -NoProfile -File $driver -ModName RebornTest -FinalModsRoot $final -BuildStaticData $Static -BuildGlobalData $Global -BuildAptUI $UI -CreateBigFiles $Big -BuildMaps $Maps -SilentMode $Silent @validation *> $log
    $code=$LASTEXITCODE
    if(($code-eq 0)-ne $Success){Get-Content $log -Tail 25;throw ('Unexpected case '+$script:case+' result '+$code)}
    Write-Output ('Case '+$script:case+' passed; exit='+$code)
}
#-------------------------------------------------------------------------------------------------
<# Reborn: compare generation pointers rather than paths embedded in disposable backend receipts. #>
#-------------------------------------------------------------------------------------------------
function Index {Get-Content (Join-Path $cache 'current.json') -Raw|ConvertFrom-Json}
function Require([bool]$Value,[string]$Message){if(!$Value){throw $Message}}
try{
    $data=Join-Path $mod 'Data';$null=New-Item -ItemType Directory -Path (Join-Path $data 'AdditionalMaps'),(Join-Path $data 'AptUI/Probe') -Force
    $static='<AssetDeclaration xmlns="uri:ea.com:eala:asset"><!-- Reborn: strong import exercises saved global dependency lookup. --><VideoEventList id="RebornStageSequence"><EventList><Movie MovieAsset="RebornStageMovie" /></EventList></VideoEventList></AssetDeclaration>'
    $global='<AssetDeclaration xmlns="uri:ea.com:eala:asset"><!-- Reborn: standalone global provider for stage reuse tests. --><Video id="RebornStageMovie" Filename="stage-test.vp6" Volume="80" /></AssetDeclaration>'
    $staticPath=Join-Path $data 'Static_Mod.xml';$globalPath=Join-Path $data 'AdditionalMaps/MapMetaData_Probe.xml'
    [IO.File]::WriteAllText($staticPath,$static);[IO.File]::WriteAllText($globalPath,$global)
    [IO.File]::WriteAllText((Join-Path $data 'AptUI/Probe.xml'),'<AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes><Include type="all" source="Probe/Probe.xml" /></Includes></AssetDeclaration>')
    [IO.File]::WriteAllText((Join-Path $data 'AptUI/Probe/Probe.xml'),'<AssetDeclaration xmlns="uri:ea.com:eala:asset"><AptDatData id="RebornStageUI" File="Probe.dat" /></AssetDeclaration>')
    $dat=Join-Path $data 'AptUI/Probe/Probe.dat';[IO.File]::WriteAllText($dat,'Reborn stage UI version one')
    Build 1 1 1 0
    $first=Index;Require (@($first.Stages.PSObject.Properties).Count -eq 3) 'Compile-only stages were not saved.'
    Require (!(Test-Path -LiteralPath $final)) 'Compile-only build published final output.'
    [IO.File]::WriteAllText($staticPath,'<deliberately-broken');[IO.File]::WriteAllText($globalPath,$global.Replace('Volume="80"','Volume="70"'))
    Build 0 1 0 1
    $second=Index;Require ($first.Stages.Static.Generation -ceq $second.Stages.Static.Generation) 'Global-only build changed Static.'
    Require ($first.Stages.AptUI.Generation -ceq $second.Stages.AptUI.Generation) 'Global-only build changed AptUI.'
    Require ($first.Stages.Global.Generation -cne $second.Stages.Global.Generation) 'Global-only build did not replace Global.'
    [IO.File]::WriteAllText($dat,'Reborn stage UI version two')
    Build 0 0 1 1
    $third=Index;Require ($second.Stages.Global.Generation -ceq $third.Stages.Global.Generation) 'UI-only build changed Global.'
    Require ($second.Stages.AptUI.Generation -cne $third.Stages.AptUI.Generation) 'UI-only build did not replace UI.'
    # Reborn: non-silent packaging must also ignore malformed XML in an unselected stage.
    Build 0 0 0 1 $true 0 0
    $big=Join-Path $final 'RebornTest/RebornTest_1.00_Streams.big';$goodHash=(Get-FileHash $big).Hash;$goodIndex=(Get-FileHash (Join-Path $cache 'current.json')).Hash
    [IO.File]::WriteAllText($globalPath,'<broken');[IO.File]::WriteAllText($dat,'Uncommitted UI data')
    Build 0 1 1 1 $false
    Require ((Get-FileHash $big).Hash -ceq $goodHash) 'Failed build changed installed output.'
    Require ((Get-FileHash (Join-Path $cache 'current.json')).Hash -ceq $goodIndex) 'Failed build changed successful stages.'
    [IO.File]::WriteAllText($globalPath,$global.Replace('id="RebornStageMovie"','id="RebornStageOtherMovie"'))
    Build 0 1 0 1 $false
    Require ((Get-FileHash (Join-Path $cache 'current.json')).Hash -ceq $goodIndex) 'Incompatible global provider was committed.'
    [IO.File]::WriteAllText($globalPath,$global);[IO.File]::WriteAllText($staticPath,$static)
    Build 0 0 0 1 $true 1
    # Reborn: migrate a real combined package through the old receipt format without rebuilding any stage.
    $modern=Join-Path $run 'modern-cache';[IO.Directory]::Move($cache,$modern);$null=New-Item -ItemType Directory -Path $cache
    Copy-Item -LiteralPath $big -Destination (Join-Path $cache 'RebornTest.big')
    [IO.File]::WriteAllText((Join-Path $cache 'mod_1.0.skudef'),"add-big RebornTest.big`n")
    [IO.File]::WriteAllText((Join-Path $cache 'build.json'),([pscustomobject]@{Format='RebornLiveAlpha1';ModName='RebornTest';PackageSha256=(Get-FileHash $big).Hash;Backend=@{};Core=@{};GlobalData=@{Streams=@()}}|ConvertTo-Json -Depth 5))
    Build 0 0 0 1
    Require (!(Test-Path -LiteralPath (Join-Path $cache 'RebornTest.big'))) 'Redundant legacy combined archive remains.'
    $migrated=Index;Require ($null-ne $migrated.Stages.Static -and $null-ne $migrated.Stages.Global -and $null-ne $migrated.Stages.AptUI) 'Legacy families were not migrated.'
    # Reborn: saved generations must detect corruption and permit rebuilding the affected selected family.
    $staticBin=Join-Path $cache ('Static/'+$migrated.Stages.Static.Generation+'/files/data/mod.bin')
    $bytes=[IO.File]::ReadAllBytes($staticBin);$bytes[$bytes.Length-1]=$bytes[$bytes.Length-1]-bxor 1;[IO.File]::WriteAllBytes($staticBin,$bytes)
    Build 0 0 0 1 $false
    Build 1 0 0 1
    # Reborn: normal advisory checks must also run successfully when all providers come from saved outputs.
    Build 0 0 0 1 $true 0 0
    # Reborn: rebuilding an empty family must remove its previous pages rather than silently reuse them.
    Remove-Item -LiteralPath (Join-Path $data 'AptUI/Probe.xml')
    Build 0 0 1 0
    $empty=Index;$emptyReceipt=Get-Content (Join-Path $cache ('AptUI/'+$empty.Stages.AptUI.Generation+'/stage.json')) -Raw|ConvertFrom-Json
    Require (@($emptyReceipt.Files | Where-Object Path -match '^files[\\/]').Count -eq 0) 'Empty AptUI rebuild retained stale pages.'
    Build 0 0 0 1
    # Reborn: validation-only work may compile privately but never update the successful stage pointer.
    $beforeValidation=(Get-FileHash (Join-Path $cache 'current.json')).Hash
    Build 0 1 0 0 $true 0 1 $true
    Require ((Get-FileHash (Join-Path $cache 'current.json')).Hash -ceq $beforeValidation) 'Validation-only build saved a generation.'
    # Reborn: a final Misc packaging failure must also retain both the old package and all successful families.
    $beforeFailure=(Get-FileHash $big).Hash;$misc=Join-Path $mod 'Misc';$null=New-Item -ItemType Directory -Path $misc -Force
    [IO.File]::WriteAllText((Join-Path $misc (([char]0x00e7)+'.txt')),'Invalid non-ASCII archive name')
    [IO.File]::WriteAllText($globalPath,$global.Replace('Volume="80"','Volume="65"'))
    Build 0 1 0 1 $false
    Require ((Get-FileHash $big).Hash -ceq $beforeFailure) 'Final packaging failure replaced the published package.'
    Require ((Get-FileHash (Join-Path $cache 'current.json')).Hash -ceq $beforeValidation) 'Final packaging failure committed a stage.'
    Write-Output ('All persistent-stage integration cases passed. Logs: '+$run)
    [IO.File]::WriteAllText((Join-Path $run 'passed.txt'),'All cases passed.')
}finally{
    # Reborn: remove only these explicitly owned developer test paths, then restore both original directories.
    foreach($path in @($mod,$cache)){
        $absolute=[IO.Path]::GetFullPath($path)
        if($absolute -cnotin @([IO.Path]::GetFullPath($mod),[IO.Path]::GetFullPath($cache)) -or !$absolute.StartsWith($sdk+'\',[StringComparison]::OrdinalIgnoreCase)){throw 'Refused test cleanup.'}
        if(Test-Path -LiteralPath $absolute){Remove-Item -LiteralPath $absolute -Recurse -Force}
    }
    if(Test-Path -LiteralPath $backupMod){[IO.Directory]::Move($backupMod,$mod)}
    if(Test-Path -LiteralPath $backupCache){[IO.Directory]::Move($backupCache,$cache)}
}
