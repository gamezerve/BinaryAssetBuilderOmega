# Reborn: verify the one live SDK's sample builds, repeat publication, failure preservation and cooperative tool lock without launching Studio or a game.
[CmdletBinding()]
param()
$ErrorActionPreference='Stop';$repo=Split-Path -Parent $PSScriptRoot;$sdk=Join-Path $repo 'alpha';$runtime=(Get-Process -Id $PID).Path;$driver=Join-Path $PSScriptRoot 'Invoke-Ra3Ep1DevAlphaBuild.ps1'
$rows=[Collections.Generic.List[object]]::new()
#-------------------------------------------------------------------------------------------------
<# Reborn: capture each backend invocation to a bounded latest log while checking its explicit exit status and stable final output contract. #>
#-------------------------------------------------------------------------------------------------
function Invoke-DevSdkCase([string]$Name,[bool]$ExpectedSuccess){$log=Join-Path $sdk ('.work/test-'+$Name+'.log');& $runtime -NoProfile -File $driver -ModName $Name *> $log;$ok=$LASTEXITCODE-eq 0;if($ok-ne $ExpectedSuccess){throw ('Unexpected build result: '+$log)};if($ok){$output=Join-Path $sdk ('builtmods/'+$Name);$receipt=Get-Content -LiteralPath (Join-Path $output 'build.json') -Raw|ConvertFrom-Json;if($receipt.Format-cne 'RebornLiveAlpha1' -or (Get-FileHash -LiteralPath $receipt.BigPath -Algorithm SHA256).Hash-cne $receipt.PackageSha256 -or (Test-Path -LiteralPath (Join-Path $output 'mod_1.0.cfg')) -or (Get-Content -LiteralPath $receipt.SkuDefPath -Raw)-cne ('add-big '+$Name+".big`n")){throw 'Live output contract differs.'}};$rows.Add([pscustomobject]@{Name=$Name;Passed=$true;ExpectedSuccess=$ExpectedSuccess})}
foreach($name in @('Reborn','ArmorCannon','CombinedArmor','CombinedArmorStrong','ConscriptAK47','ArmorSource')){Invoke-DevSdkCase $name $true}
$first=(Get-FileHash -LiteralPath (Join-Path $sdk 'builtmods/Reborn/Reborn.big') -Algorithm SHA256).Hash;Invoke-DevSdkCase 'Reborn' $true;if((Get-FileHash -LiteralPath (Join-Path $sdk 'builtmods/Reborn/Reborn.big') -Algorithm SHA256).Hash-cne $first){throw 'Repeat package differs.'}
$probe='DevProbe'+[guid]::NewGuid().ToString('N').Substring(0,8);$probeRoot=Join-Path $sdk ('mods/'+$probe);$null=New-Item -ItemType Directory -Path (Join-Path $probeRoot 'data');$input=Join-Path $probeRoot 'data/mod.xml'
try{
    Copy-Item -LiteralPath (Join-Path $sdk 'mods/Reborn/data/mod.xml') -Destination $input;Invoke-DevSdkCase $probe $true;$package=Join-Path $sdk ('builtmods/'+$probe+'/'+$probe+'.big');$before=(Get-FileHash -LiteralPath $package -Algorithm SHA256).Hash
    # Reborn: edit only this test's fresh source, never a user sample; a genuinely changed selected value must change the output.
    $xml=[Xml.XmlDocument]::new();$xml.Load($input);$entry=@($xml.GetElementsByTagName('Armor')|Where-Object {$_.GetAttribute('Damage')-ceq 'GUN'});if($entry.Count-ne 1){throw 'Probe GUN source differs.'};$entry[0].SetAttribute('Percent','250');$xml.Save($input);Invoke-DevSdkCase $probe $true;$changed=(Get-FileHash -LiteralPath $package -Algorithm SHA256).Hash;if($changed-ceq $before){throw 'Authored edit retained the old BIG.'}
    [IO.File]::WriteAllText($input,'<broken');Invoke-DevSdkCase $probe $false;if((Get-FileHash -LiteralPath $package -Algorithm SHA256).Hash-cne $changed){throw 'Failed build replaced the last good package.'}
    $lock=[IO.File]::Open((Join-Path $sdk '.sdk.lock'),[IO.FileMode]::Open,[IO.FileAccess]::ReadWrite,[IO.FileShare]::None);try{Invoke-DevSdkCase 'Reborn' $false}finally{$lock.Dispose()}
    $rows.Add([pscustomobject]@{Name='SourceEditFailurePreservationAndLock';Passed=$true})
}finally{
    # Reborn: remove only this invocation's GUID-named source/output fixtures after resolving their exact direct-child parents.
    foreach($pair in @(@($probeRoot,(Join-Path $sdk 'mods')),@((Join-Path $sdk ('builtmods/'+$probe)),(Join-Path $sdk 'builtmods')))){if((Split-Path -Parent $pair[0])-ine $pair[1]){throw 'Probe cleanup escaped its parent.'};if(Test-Path -LiteralPath $pair[0]){foreach($item in Get-ChildItem -LiteralPath $pair[0] -Force -Recurse){if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected probe retained.'}};Remove-Item -LiteralPath $pair[0] -Recurse -Force}}
}
$report=[pscustomobject]@{Passed=$true;Cases=$rows;GameExecuted=$false;UserSamplesModified=$false};[IO.File]::WriteAllText((Join-Path $sdk '.work/dev-sdk-test.json'),($report|ConvertTo-Json -Depth 6));$report
