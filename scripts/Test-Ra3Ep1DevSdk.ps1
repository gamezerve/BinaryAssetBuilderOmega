# Reborn: exercise preserved fixtures only in RebornTest; never compile or modify the user's Reborn mod.
[CmdletBinding()]
param()
$ErrorActionPreference='Stop';$repo=Split-Path -Parent $PSScriptRoot;$sdk=Join-Path $repo 'alpha';$runtime=(Get-Process -Id $PID).Path;$driver=Join-Path $PSScriptRoot 'Invoke-Ra3Ep1DevAlphaBuild.ps1'
$fixtures=Join-Path $repo 'tests/fixtures/live-alpha';$data=Join-Path $sdk 'mods/RebornTest/data';$output=Join-Path $sdk 'builtmods/RebornTest';$input=Join-Path $data 'mod.xml';$package=Join-Path $output 'RebornTest.big'
$rows=[Collections.Generic.List[object]]::new();$userSource=Join-Path $sdk 'mods/Reborn/data/mod.xml';$userHash=(Get-FileHash -LiteralPath $userSource).Hash
$userReceipt=Join-Path $sdk 'builtmods/Reborn/build.json';$userOutputHash=if(Test-Path -LiteralPath $userReceipt){(Get-FileHash -LiteralPath $userReceipt).Hash}else{$null}
#-------------------------------------------------------------------------------------------------
<# Reborn: remove only temporary developer data after preserving the original directory and refusing redirected ancestors or children. #>
#-------------------------------------------------------------------------------------------------
function Remove-TestData {
    if([IO.Path]::GetFullPath($data)-ine (Join-Path $sdk 'mods/RebornTest/data')){throw 'Unexpected cleanup target.'}
    if(Test-Path -LiteralPath $data){foreach($item in @(Get-Item -LiteralPath $data)+@(Get-ChildItem -LiteralPath $data -Recurse -Force)){if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected data retained.'}};Remove-Item -LiteralPath $data -Recurse -Force}
}
#-------------------------------------------------------------------------------------------------
<# Reborn: reuse the developer slot instead of adding temporary mods to Studio's list. #>
#-------------------------------------------------------------------------------------------------
function Set-TestFixture([string]$Name){Remove-TestData;Copy-Item -LiteralPath (Join-Path $fixtures ($Name+'/data')) -Destination $data -Recurse}
#-------------------------------------------------------------------------------------------------
<# Reborn: verify exit status, four final files, exact SKUDef bytes and package/receipt agreement for the developer slot. #>
#-------------------------------------------------------------------------------------------------
function Invoke-TestCase([string]$Label,[bool]$ExpectedSuccess){
    $log=Join-Path $sdk ('.work/test-'+$Label+'.log');& $runtime -NoProfile -File $driver -ModName RebornTest *> $log
    if(($LASTEXITCODE-eq 0)-ne $ExpectedSuccess){throw ('Unexpected test result: '+$log)}
    if($ExpectedSuccess){$receipt=Get-Content -LiteralPath (Join-Path $output 'build.json') -Raw|ConvertFrom-Json;if($receipt.Format-cne 'RebornLiveAlpha1' -or (Get-FileHash -LiteralPath $package).Hash-cne $receipt.PackageSha256 -or @(Get-ChildItem -LiteralPath $output -File).Count-ne 4 -or (Get-Content -LiteralPath $receipt.SkuDefPath -Raw)-cne "add-big RebornTest.big`n" -or @(Get-ChildItem -LiteralPath $output -Filter *.cfg).Count){throw 'Output contract differs.'}}
    $rows.Add([pscustomobject]@{Name=$Label;Passed=$true;ExpectedSuccess=$ExpectedSuccess})
}
$saved=Join-Path $sdk ('.work/RebornTest-original-'+[guid]::NewGuid().ToString('N'))
foreach($path in @($data,(Join-Path $sdk '.work'))){for($item=Get-Item -LiteralPath $path;$null-ne $item;$item=$item.Parent){if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected test root refused.'}}}
$testLock=[IO.File]::Open((Join-Path $sdk '.work/regression.lock'),[IO.FileMode]::OpenOrCreate,[IO.FileAccess]::ReadWrite,[IO.FileShare]::None)
[IO.Directory]::Move($data,$saved)
try {
    foreach($name in @('Reborn','ArmorCannon','CombinedArmor','CombinedArmorStrong','ConscriptAK47','ArmorSource','ConscriptBuildTime1')){Set-TestFixture $name;Invoke-TestCase $name $true}
    # Reborn: newly source-compiled Conscript must reproduce the game-observed archive, not just a success label.
    $conscriptHash=(Get-FileHash -LiteralPath $package).Hash;if($conscriptHash-cne '3FCDFFB9909B39E9C3A6B75AC50791A19D3EFBAE5A83D6169C174197C50C8FB2'){throw 'Conscript golden differs.'}
    foreach($edit in @(@('BuildTime','not-a-real'),@('UnknownRootField','2'))){Set-TestFixture ConscriptBuildTime1;$xml=[Xml.XmlDocument]::new();$xml.PreserveWhitespace=$true;$xml.Load($input);$xml.DocumentElement.SetAttribute($edit[0],$edit[1]);$xml.Save($input);Invoke-TestCase ('ConscriptReject'+$edit[0]) $false;if((Get-FileHash -LiteralPath $package).Hash-cne $conscriptHash){throw 'Rejected Conscript replaced output.'}}
    # Reborn: one grouped authored profile exercises all six values through the public Studio backend, not only isolated diagnostic compilation.
    Set-TestFixture ConscriptBuildTime1;$xml=[Xml.XmlDocument]::new();$xml.PreserveWhitespace=$true;$xml.Load($input);$ns=[Xml.XmlNamespaceManager]::new($xml.NameTable);$ns.AddNamespace('a','uri:ea.com:eala:asset')
    foreach($edit in @(@('.','BuildTime','2'),@('a:ObjectResourceInfo/a:BuildCost','Amount','25'),@('a:Body/a:ActiveBody','MaxHealth','500'),@('a:VisionInfo','VisionRange','350'),@('a:VisionInfo','ShroudClearingRange','650'),@('a:LocomotorSet','Speed','90'))){$xml.DocumentElement.SelectSingleNode($edit[0],$ns).SetAttribute($edit[1],$edit[2])};$xml.Save($input)
    Invoke-TestCase ConscriptCombinedScalars $true;$combinedHash=(Get-FileHash -LiteralPath $package).Hash;if($combinedHash-ceq $conscriptHash){throw 'Grouped scalar edits did not change BIG.'}
    foreach($bad in @(@('a:ObjectResourceInfo/a:BuildCost','Amount','1.5'),@('a:Body/a:ActiveBody','MaxHealth','NaN'),@('a:VisionInfo','VisionRange','not-a-real'),@('a:LocomotorSet','Locomotor','UnreviewedLocomotor'))){$copy=$xml.CloneNode($true);$copyNs=[Xml.XmlNamespaceManager]::new($copy.NameTable);$copyNs.AddNamespace('a','uri:ea.com:eala:asset');$copy.DocumentElement.SelectSingleNode($bad[0],$copyNs).SetAttribute($bad[1],$bad[2]);$copy.Save($input);Invoke-TestCase ('ConscriptReject'+$bad[1]) $false;if((Get-FileHash -LiteralPath $package).Hash-cne $combinedHash){throw 'Invalid scalar changed last good package.'}}
    # Reborn: public Studio compilation must resize one stock string and allocate one formerly absent string in the same package.
    $xml.DocumentElement.SetAttribute('Description','Desc:RebornDynamicConscriptDescription');$xml.DocumentElement.SetAttribute('DescriptionTransformed','Desc:RebornDynamicConscriptDescriptionTransformed');$xml.Save($input);Invoke-TestCase ConscriptDynamicRootStrings $true;if((Get-FileHash -LiteralPath $package).Hash-ceq $combinedHash){throw 'Dynamic root strings did not change BIG.'}
    # Reborn: removing both root imports must publish 273 references while reindexing every downstream dependency alongside resized strings.
    $xml.DocumentElement.RemoveAttribute('UnitTypeIcon');$xml.DocumentElement.SetAttribute('UnitIntro','');$xml.Save($input);Invoke-TestCase ConscriptRootImportRemoval $true;$rootReceipt=Get-Content -LiteralPath (Join-Path $output 'build.json') -Raw|ConvertFrom-Json;if($rootReceipt.Core.ReferenceOccurrences-ne 273){throw 'Removed root imports retained a fixed dependency count in the receipt.'}
    # Reborn: one public Studio package combines draw, physics, death, AI and body primitives with dynamic strings and changed root import topology.
    $xml.DocumentElement.SelectSingleNode('a:Draws/a:ScriptedModelDraw',$ns).SetAttribute('InitialRecoilSpeed','2.5');$xml.DocumentElement.SelectSingleNode('a:Behaviors/a:Physics',$ns).SetAttribute('GravityMultiplier','1.25');$xml.DocumentElement.SelectSingleNode('a:Behaviors/a:SlowDeath',$ns).SetAttribute('SinkDelay','250ms');$xml.DocumentElement.SelectSingleNode('a:AI/a:AIUpdate',$ns).SetAttribute('StopChaseDistance','42.5');$xml.DocumentElement.SelectSingleNode('a:AI/a:AIUpdate',$ns).SetAttribute('StandGround','1');$xml.DocumentElement.SelectSingleNode('a:Body/a:ActiveBody',$ns).SetAttribute('MaxHealthDamagedPercentage','37%');$xml.Save($input);Invoke-TestCase ConscriptCombinedModulePrimitives $true;$moduleHash=(Get-FileHash -LiteralPath $package).Hash
    # Reborn: an invalid module scalar must fail and retain the last fully authored package.
    $xml.DocumentElement.SelectSingleNode('a:AI/a:AIUpdate',$ns).SetAttribute('StopChaseDistance','NaN');$xml.Save($input);Invoke-TestCase ConscriptRejectModulePrimitive $false;if((Get-FileHash -LiteralPath $package).Hash-cne $moduleHash){throw 'Invalid module scalar replaced the last good package.'}
    Set-TestFixture Reborn;Invoke-TestCase ArmorRepeat1 $true;$first=(Get-FileHash -LiteralPath $package).Hash;Invoke-TestCase ArmorRepeat2 $true;if((Get-FileHash -LiteralPath $package).Hash-cne $first){throw 'Repeat differs.'}
    $xml=[Xml.XmlDocument]::new();$xml.Load($input);$gun=@($xml.GetElementsByTagName('Armor')|Where-Object {$_.GetAttribute('Damage')-ceq 'GUN'});$gun[0].SetAttribute('Percent','250');$xml.Save($input);Invoke-TestCase ArmorEdit $true;$changed=(Get-FileHash -LiteralPath $package).Hash;if($changed-ceq $first){throw 'Edit did not change BIG.'}
    [IO.File]::WriteAllText($input,'<broken');Invoke-TestCase InvalidXml $false;if((Get-FileHash -LiteralPath $package).Hash-cne $changed){throw 'Failed build replaced output.'}
    # Reborn: use valid input so rejection isolates the shared lock rather than the preceding malformed XML.
    Set-TestFixture Reborn;$lock=[IO.File]::Open((Join-Path $sdk '.sdk.lock'),[IO.FileMode]::Open,[IO.FileAccess]::ReadWrite,[IO.FileShare]::None);try{Invoke-TestCase SdkLock $false}finally{$lock.Dispose()}
} finally {
    # Reborn: restore every original developer file, including auxiliary experiments; the user's mod and output remain untouched.
    try{Remove-TestData;[IO.Directory]::Move($saved,$data)}finally{$testLock.Dispose()}
}
Invoke-TestCase RestoredDeveloperSource $true
# Reborn: the live new-ID profile must publish its real local consumer, not merely another original-ID scalar package.
$restored=[Xml.XmlDocument]::new();$restored.Load($input)
if($restored.DocumentElement.LocalName-ceq 'AssetDeclaration'){
    $receipt=Get-Content -LiteralPath (Join-Path $output 'build.json') -Raw|ConvertFrom-Json
    if($receipt.Core.Asset-cne 'GameObject:RebornConscriptTrial' -or $receipt.Core.AssetCount-ne 2 -or -not $receipt.Core.RecruitableNewIdentity -or $receipt.Core.CommandOverride-cne 'LogicCommand:Command_ConstructSovietAntiInfantryInfantry' -or $receipt.Backend.AutomaticCommandGenerated -or @($receipt.Backend.AuthoredSources).Count-ne 5){throw 'Explicit source/command publication receipt differs.'}
    $rows.Add([pscustomobject]@{Name='RecruitmentLocalConsumer';Passed=$true;ExpectedSuccess=$true})
}
if((Get-FileHash -LiteralPath $userSource).Hash-cne $userHash -or ($userOutputHash -and (Get-FileHash -LiteralPath $userReceipt).Hash-cne $userOutputHash) -or (-not $userOutputHash -and (Test-Path -LiteralPath $userReceipt))){throw 'User Reborn changed.'}
$report=[pscustomobject]@{Passed=$true;Cases=$rows;GameExecuted=$false;UserSamplesModified=$false;UserRebornBuilt=$false;DeveloperSourceRestored=$true};[IO.File]::WriteAllText((Join-Path $sdk '.work/dev-sdk-test.json'),($report|ConvertTo-Json -Depth 6));$report
