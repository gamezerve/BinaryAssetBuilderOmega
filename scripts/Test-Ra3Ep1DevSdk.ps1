# Reborn: exercise preserved fixtures only in RebornTest; never compile or modify the user's Reborn mod.
[CmdletBinding()]
param([switch]$IncludeRebornSourceMirror,[switch]$ReferenceRetargetingOnly,[switch]$ModuleValuesOnly,[switch]$DiagnosticsOnly,[switch]$VariableBehaviorsOnly,[switch]$UpgradeStructuresOnly,[switch]$RootTextOnly,[switch]$CombatListsOnly,[switch]$MultiAssetOnly,[switch]$CommandSetsOnly)
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
    # Reborn: final Documents-style output must also remain unchanged when a later fixture is rejected.
    $finalDirectory=Join-Path $sdk '.work/test-final-mods/RebornTest';$priorFinal=@{}
    if(-not $ExpectedSuccess -and (Test-Path -LiteralPath $finalDirectory)){foreach($file in Get-ChildItem -LiteralPath $finalDirectory -File -Force | Where-Object Name -NE '.reborn-publish.lock'){$priorFinal[$file.FullName]=(Get-FileHash -LiteralPath $file.FullName).Hash}}
    # Reborn: automated fixtures publish only to isolated SDK work space, never the user's Documents mod library.
    $log=Join-Path $sdk ('.work/test-'+$Label+'.log');& $runtime -NoProfile -File $driver -ModName RebornTest -FinalModsRoot (Join-Path $sdk '.work/test-final-mods') *> $log
    if(($LASTEXITCODE-eq 0)-ne $ExpectedSuccess){throw ('Unexpected test result: '+$log)}
    # Reborn: every real build needs readable phases and an explicit failure/success result, not replayed JSON receipts.
    $display=Get-Content -LiteralPath $log -Raw
    if($display-notmatch 'Step 1: Clearing build cache' -or $display-notmatch 'Step time taken:' -or $display-match '(?m)^\s*"(?:RequestedArguments|CompilerFiles|WorkingDirectoryMustBeInstallation)"\s*:'){throw ('Readable build log contract differs: '+$log)}
    if($ExpectedSuccess){if($display-notmatch 'Step 11: Creating skudef file' -or $display-notmatch 'Publication completed;'){throw ('Successful phase summary absent: '+$log)}}elseif($display-notmatch 'Build Failed during Step '){throw ('Failure phase absent: '+$log)}
    if($ExpectedSuccess){$receipt=Get-Content -LiteralPath (Join-Path $output 'build.json') -Raw|ConvertFrom-Json;if($receipt.Format-cne 'RebornLiveAlpha1' -or (Get-FileHash -LiteralPath $package).Hash-cne $receipt.PackageSha256 -or @(Get-ChildItem -LiteralPath $output -File).Count-ne 4 -or (Get-Content -LiteralPath $receipt.SkuDefPath -Raw)-cne "add-big RebornTest.big`n" -or @(Get-ChildItem -LiteralPath $output -Filter *.cfg).Count){throw 'Output contract differs.'};if((Get-Content -LiteralPath (Join-Path $output 'last-build.log') -Raw)-notmatch '(?m)^Build Completed\r?$'){throw 'Saved successful log has no completion marker.'}}
    # Reborn: verify the actual final archives/SKU instead of treating the internal compatibility cache as publication proof.
    if($ExpectedSuccess){
        # Reborn: SKUDef and receipt must agree on optional Misc presence, including removal of an earlier managed archive.
        $final=$receipt.FinalPackage;$expectedSku="add-big RebornTest_1.00_Streams.big`r`n"
        if($final.MiscEntries -gt 0){if(-not $final.MiscPath -or -not(Test-Path -LiteralPath $final.MiscPath)){throw 'Missing nonempty Misc package.'};$expectedSku+="add-big RebornTest_1.00_Misc.big`r`n"}elseif($final.MiscPath -or (Test-Path -LiteralPath (Join-Path $finalDirectory 'RebornTest_1.00_Misc.big'))){throw 'Unexpected empty Misc package.'}
        if(-not $final -or $final.Version -cne '1.00' -or $final.Directory -ine $finalDirectory -or (Get-FileHash -LiteralPath $final.StreamsPath).Hash -cne $receipt.PackageSha256 -or (Get-Content -LiteralPath $final.SkuDefPath -Raw) -cne $expectedSku){throw 'Versioned final publication contract differs.'}
    }
    else{foreach($path in $priorFinal.Keys){if(-not(Test-Path -LiteralPath $path) -or (Get-FileHash -LiteralPath $path).Hash -cne $priorFinal[$path]){throw 'Rejected fixture changed the final published package.'}}}
    $rows.Add([pscustomobject]@{Name=$Label;Passed=$true;ExpectedSuccess=$ExpectedSuccess})
}
$saved=Join-Path $sdk ('.work/RebornTest-original-'+[guid]::NewGuid().ToString('N'))
foreach($path in @($data,(Join-Path $sdk '.work'))){for($item=Get-Item -LiteralPath $path;$null-ne $item;$item=$item.Parent){if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected test root refused.'}}}
$testLock=[IO.File]::Open((Join-Path $sdk '.work/regression.lock'),[IO.FileMode]::OpenOrCreate,[IO.FileAccess]::ReadWrite,[IO.FileShare]::None)
[IO.Directory]::Move($data,$saved)
try {
    # Reborn: an explicitly requested reproduction copies user source read-only into the developer slot; it never builds or replaces the user's output.
    if($IncludeRebornSourceMirror){
        $userData=Join-Path $sdk 'mods/Reborn/data'
        # Reborn: refuse redirected source ancestry before any recursive listing/copy of the user's data.
        for($item=Get-Item -LiteralPath $userData;$null-ne $item;$item=$item.Parent){if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'User source mirror refuses redirected ancestry.'}}
        $userFiles=@(Get-ChildItem -LiteralPath $userData -Recurse -Force)
        if(($userFiles|Where-Object {($_.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0}) -or ($userFiles|Where-Object {-not $_.PSIsContainer}|Measure-Object Length -Sum).Sum-gt 16MB){throw 'User source mirror refuses redirected or oversized data.'}
        $sourceHashes=@{};foreach($file in $userFiles|Where-Object {-not $_.PSIsContainer}){$sourceHashes[$file.FullName]=(Get-FileHash -LiteralPath $file.FullName).Hash}
        Remove-TestData;Copy-Item -LiteralPath $userData -Destination $data -Recurse;Invoke-TestCase RebornSourceMirror $true
        $mirror=Get-Content -LiteralPath (Join-Path $output 'build.json') -Raw|ConvertFrom-Json
        if($mirror.Backend.AutomaticCommandGenerated -or $mirror.Core.AssetCount-ne 1 -or -not @($mirror.Backend.AuthoredSources|Where-Object {$_.Path.EndsWith('ParachuteDrawSoviet.xml')})){throw 'Reborn source/fragment reproduction receipt differs.'}
        foreach($file in $sourceHashes.Keys){if((Get-FileHash -LiteralPath $file).Hash-cne $sourceHashes[$file]){throw 'User source changed during read-only reproduction.'}}
    }
    # Reborn: qualify reference freedom with combined native-layout edits and publication-preserving refusals without replaying unrelated armor matrices.
    if($CommandSetsOnly){
        # Reborn: compile stock override and inherited new-ID sets without a GameObject; one missing strong Cmd must preserve the successful package.
        Set-TestFixture ConscriptBuildTime1
        [IO.File]::WriteAllText($input,'<!-- Reborn: standalone command-set mod. --><AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes><Include type="all" source="DATA:CommandSets.xml"/></Includes></AssetDeclaration>')
        $setsPath=Join-Path $data 'CommandSets.xml';$sets='<!-- Reborn: explicit stock and new command-set identities, no dummy unit. --><AssetDeclaration xmlns="uri:ea.com:eala:asset"><LogicCommandSet id="SovietBarracksCommandSet"><Cmd>Command_ConstructSovietAntiInfantryInfantry</Cmd></LogicCommandSet><LogicCommandSet id="RebornStandaloneSet" inheritFrom="SovietBarracksCommandSet"/></AssetDeclaration>'
        [IO.File]::WriteAllText($setsPath,$sets);Invoke-TestCase CommandSetsWithoutGameObject $true
        $receipt=Get-Content -LiteralPath (Join-Path $output 'build.json') -Raw|ConvertFrom-Json
        if($receipt.Core.AssetCount-ne 2 -or $receipt.Core.Asset-cne 'LogicCommandSet:SovietBarracksCommandSet' -or @($receipt.Core.Assets|Where-Object {$_ -like 'GameObject:*'}).Count){throw 'Standalone command-set graph receipt differs.'}
        $good=(Get-FileHash -LiteralPath $package).Hash
        [IO.File]::WriteAllText($setsPath,$sets.Replace('Command_ConstructSovietAntiInfantryInfantry','Command_RebornMissing'));Invoke-TestCase CommandSetMissingCmd $false
        if((Get-FileHash -LiteralPath $package).Hash-cne $good){throw 'Missing command replaced a successful command-set package.'}
    }elseif($MultiAssetOnly){
        # Reborn: explicitly authored local graph, deliberately declared in consumer-first order, with real DATA includes/definitions/inheritance.
        Set-TestFixture ConscriptBuildTime1;$game=[Xml.XmlDocument]::new();$game.PreserveWhitespace=$true;$game.Load($input);$game.DocumentElement.SetAttribute('id','RebornMixedConscript');$game.SelectSingleNode('//*[local-name()="DisplayName"]').InnerText='Name:RebornMixedConscript'
        $armors=@($game.SelectNodes('/*/*[local-name()="ArmorSet"]'));$armors[0].SetAttribute('Armor','RebornMixedVeteranArmor');$armors[-1].SetAttribute('Armor','RebornMixedArmor')
        $weapons=@($game.SelectNodes('//*[local-name()="WeaponSlotHardpoint"]/*[local-name()="Weapon"]'));$weapons[0].SetAttribute('Template','RebornMixedRifle');$weapons[1].SetAttribute('Template','RebornMixedAlternateRifle')
        $assets=[Xml.XmlDocument]::new();$assets.PreserveWhitespace=$true;$assets.LoadXml('<AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes><Include type="all" source="DATA:MixedDefines.xml"/></Includes></AssetDeclaration>');$null=$assets.DocumentElement.AppendChild($assets.ImportNode($game.DocumentElement,$true))
        $stockSets=[Xml.XmlDocument]::new();$stockSets.Load((Join-Path $sdk 'SageXML/GlobalData/LogicCommandSet.xml'));$set=$stockSets.SelectSingleNode('//*[local-name()="LogicCommandSet"][@id="SovietBarracksCommandSet"]');if(-not $set){throw 'Stock command-set source is missing.'};$set=$assets.ImportNode($set,$true);$cmd=$assets.CreateElement('Cmd','uri:ea.com:eala:asset');$cmd.InnerText='Command_ConstructRebornMixedConscript';$null=$set.AppendChild($cmd);$null=$assets.DocumentElement.AppendChild($set)
        $support=[Xml.XmlDocument]::new();$support.LoadXml('<AssetDeclaration xmlns="uri:ea.com:eala:asset"><LogicCommand id="Command_ConstructRebornMixedConscript" Type="UNIT_BUILD"><Object>RebornMixedConscript</Object></LogicCommand><ArmorTemplate id="RebornMixedArmor" Default="100"><Armor Damage="GUN" Percent="50"/><Armor Damage="CANNON" Percent="25"/></ArmorTemplate><ArmorTemplate id="RebornMixedVeteranArmor" inheritFrom="RebornMixedArmor" Default="50"/></AssetDeclaration>');foreach($node in @($support.DocumentElement.ChildNodes|Where-Object {$_ -is [Xml.XmlElement]})){$null=$assets.DocumentElement.AppendChild($assets.ImportNode($node,$true))}
        $rifle=[Xml.XmlDocument]::new();$rifle.Load((Join-Path $repo 'tests/fixtures/WeaponAuthoringProbe.xml'));$rifle.DocumentElement.SetAttribute('id','RebornMixedRifle');$rifle.DocumentElement.SetAttribute('AttackRange','175');$rifle.DocumentElement.SetAttribute('ClipSize','5');$rifle.SelectSingleNode('//*[local-name()="DamageNugget"]').SetAttribute('Damage','=$MIXED_DAMAGE');$null=$assets.DocumentElement.AppendChild($assets.ImportNode($rifle.DocumentElement,$true))
        $derived=[Xml.XmlDocument]::new();$derived.LoadXml('<WeaponTemplate xmlns="uri:ea.com:eala:asset" id="RebornMixedAlternateRifle" inheritFrom="RebornMixedRifle" AttackRange="225"/>');$null=$assets.DocumentElement.AppendChild($assets.ImportNode($derived.DocumentElement,$true))
        $assetsPath=Join-Path $data 'MixedAssets.xml';$assets.Save($assetsPath)
        [IO.File]::WriteAllText((Join-Path $data 'MixedDefines.xml'),'<!-- Reborn: the weapon uses an imported real definition. --><AssetDeclaration xmlns="uri:ea.com:eala:asset"><Defines><Define name="MIXED_DAMAGE" value="50"/></Defines></AssetDeclaration>')
        [IO.File]::WriteAllText($input,'<!-- Reborn: all seven assets are explicit included declarations. --><AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes><Include type="all" source="DATA:MixedAssets.xml"/></Includes></AssetDeclaration>')
        Invoke-TestCase MultiAssetGraph $true;$good=(Get-FileHash -LiteralPath $package).Hash
        $receipt=Get-Content -LiteralPath (Join-Path $output 'build.json') -Raw|ConvertFrom-Json;if($receipt.Core.AssetCount-ne 7 -or -not $receipt.Core.MultiAsset -or -not $receipt.Core.DeclaredBarracksRecruitmentChain -or -not $receipt.Core.RecruitableNewIdentity -or @($receipt.Core.Assets).Count-ne 7 -or @($receipt.Core.LocalEdges).Count-lt 5 -or $receipt.Backend.AutomaticCommandGenerated){throw 'Mixed graph receipt differs.'}
        # Reborn: local publication roles and dependency ordering must not depend on textual declaration order.
        $reordered=$assets.CloneNode($true);$nodes=@($reordered.DocumentElement.ChildNodes|Where-Object {$_ -is [Xml.XmlElement] -and $_.LocalName -cne 'Includes'});[array]::Reverse($nodes);foreach($node in $nodes){$null=$reordered.DocumentElement.AppendChild($node)};$reordered.Save($assetsPath);Invoke-TestCase MultiAssetReordered $true;$good=(Get-FileHash -LiteralPath $package).Hash
        foreach($kind in @('MissingWeapon','MissingCommand','Duplicate','WeaponShape','NonfiniteWeapon','AuthoredTypeId')){
            $bad=$assets.CloneNode($true)
            switch($kind){
                MissingWeapon{$node=$bad.SelectSingleNode('//*[local-name()="WeaponTemplate"][@id="RebornMixedAlternateRifle"]');$null=$node.ParentNode.RemoveChild($node)}
                MissingCommand{$bad.SelectSingleNode('//*[local-name()="Cmd" and text()="Command_ConstructRebornMixedConscript"]').InnerText='Command_RebornMissing'}
                Duplicate{$node=$bad.SelectSingleNode('//*[local-name()="ArmorTemplate"][@id="RebornMixedArmor"]');$null=$bad.DocumentElement.AppendChild($node.CloneNode($true))}
                # Reborn: genuinely unsupported topology and invalid scalar/discriminator data must fail without replacing the previous package.
                WeaponShape{$node=$bad.SelectSingleNode('//*[local-name()="DamageNugget"]');$extra=$node.CloneNode($true);$extra.SetAttribute('id','ExtraDamage');$null=$node.ParentNode.AppendChild($extra)}
                NonfiniteWeapon{$bad.SelectSingleNode('//*[local-name()="WeaponTemplate"][@id="RebornMixedRifle"]').SetAttribute('AttackRange','NaN')}
                AuthoredTypeId{$bad.SelectSingleNode('//*[local-name()="DamageNugget"]').SetAttribute('TypeId','1')}
            }
            $bad.Save($assetsPath);Invoke-TestCase ('MultiAssetReject'+$kind) $false;if((Get-FileHash -LiteralPath $package).Hash-cne $good){throw 'Invalid mixed source replaced the successful package.'}
        }
    }elseif($VariableBehaviorsOnly -or $UpgradeStructuresOnly -or $RootTextOnly -or $CombatListsOnly){
        # Reborn: one combined Studio source changes behavior count/order/local IDs, fixed root/module values and an explicit new-object consumer.
        Set-TestFixture ConscriptBuildTime1;$xml=[Xml.XmlDocument]::new();$xml.PreserveWhitespace=$true;$xml.Load($input)
        $xml.DocumentElement.SetAttribute('id','RebornConscriptTrial');$xml.DocumentElement.SetAttribute('Scale','1.25');$xml.DocumentElement.SetAttribute('ThreatLevel','15')
        $xml.SelectSingleNode('//*[local-name()="ActiveBody"]').SetAttribute('MaxHealth','500');$xml.SelectSingleNode('//*[local-name()="AIUpdate"]').SetAttribute('StopChaseDistance','42.5')
        # Reborn: combine localization-key freedom with real variable modules, retargeted weapons, a custom object and an explicit consumer.
        if($RootTextOnly){$xml.SelectSingleNode('//*[local-name()="DisplayName"]').InnerText='Name:RebornCustomConscript';$xml.DocumentElement.SetAttribute('Description','Desc:RebornCustomDescription');$xml.DocumentElement.SetAttribute('TypeDescription','Type:RebornCustomType');$child=$xml.CreateElement('DisplayNameTransformed','uri:ea.com:eala:asset');$child.InnerText='Name:RebornCustomTransformed';$null=$xml.DocumentElement.InsertAfter($child,$xml.SelectSingleNode('//*[local-name()="DisplayName"]'))}
        $behaviors=$xml.SelectSingleNode('/*/*[local-name()="Behaviors"]');$original=@($behaviors.ChildNodes|Where-Object {$_ -is [Xml.XmlElement]})
        for($i=0;$i-lt $original.Count;$i+=2){$null=$behaviors.RemoveChild($original[$i])}
        for($i=0;$i-lt 12;$i++){$clone=$original[$i].CloneNode($true);$clone.SetAttribute('id',('RebornVariableClone_'+$i));$null=$behaviors.AppendChild($clone)}
        $ordered=@($behaviors.ChildNodes|Where-Object {$_ -is [Xml.XmlElement]});[array]::Reverse($ordered);foreach($node in $ordered){$null=$behaviors.AppendChild($node)}
        # Reborn: a real Studio package must combine graph edits with authored strong and typed weak asset targets.
        $weapon=$xml.SelectSingleNode('//*[local-name()="Weapon" and contains(@Template,"SovietAntiInfantryInfantryAK47")]');if(-not $weapon){throw 'Combined weapon control is missing.'};$weapon.SetAttribute('Template','JapanArcherInfantryEnergyBow')
        $weakEdits=0;foreach($trigger in $xml.SelectNodes('//*[local-name()="TriggeredBy"]')){if($trigger.InnerText.Trim() -cin @('Upgrade_JapanFaction','Upgrade_AlliesFaction')){$trigger.InnerText=if($trigger.InnerText.Trim() -ceq 'Upgrade_JapanFaction'){'Upgrade_AlliesFaction'}else{'Upgrade_JapanFaction'};$weakEdits++}};if(-not $weakEdits){throw 'Combined upgrade control is missing.'}
        # Reborn: remove stock row-count/reference-presence assumptions in the actual Studio driver, not only a standalone native probe.
        if($CombatListsOnly){
            foreach($entry in @($xml.DocumentElement.ChildNodes|Where-Object {$_ -is [Xml.XmlElement] -and $_.LocalName -ceq 'ArmorSet'})){$null=$xml.DocumentElement.RemoveChild($entry)}
            $anchor=$xml.SelectSingleNode('/*/*[local-name()="LocomotorSet"]')
            for($i=0;$i-lt 40;$i++){$entry=$xml.CreateElement('ArmorSet','uri:ea.com:eala:asset');$entry.SetAttribute('Armor','AlliedConYardArmor');if($i%2-eq 0){$entry.SetAttribute('DamageFX','InfantryDamageFX')};$entry.SetAttribute('Conditions','VETERAN SHRINK_EFFECT');$null=$xml.DocumentElement.InsertBefore($entry,$anchor)}
            $module=$weapon.ParentNode.ParentNode;$model=$weapon.ParentNode.CloneNode($true)
            foreach($child in @($module.ChildNodes|Where-Object {$_ -is [Xml.XmlElement]})){$null=$module.RemoveChild($child)}
            for($i=0;$i-lt 10;$i++){$hp=$xml.CreateElement('WeaponSlotHardpoint','uri:ea.com:eala:asset');$hp.SetAttribute('ID',($i+1).ToString());$hp.SetAttribute('AutoChooseWeapon','1');for($j=0;$j-lt 17;$j++){$row=$model.FirstChild.CloneNode($true);if($row -isnot [Xml.XmlElement]){$row=@($model.ChildNodes|Where-Object {$_ -is [Xml.XmlElement]})[0].CloneNode($true)};$row.SetAttribute('Ordering',$(if($j%2-eq 0){'SECONDARY_WEAPON'}else{'PRIMARY_WEAPON'}));if($j%3-eq 0){$row.SetAttribute('Upgrade','Upgrade_Veterancy_HEROIC')};if($j%4-eq 1){$row.RemoveAttribute('Template')};$null=$hp.AppendChild($row)};$null=$module.AppendChild($hp)}
            $xml.SelectSingleNode('//*[local-name()="DisplayName"]').InnerText='Name:RebornCombatLists'
        }
        # Reborn: test list growth and a newly present optional nested record together with the existing composed graph.
        if($UpgradeStructuresOnly){
            $module=$xml.SelectSingleNode('//*[local-name()="ModelConditionUpgrade"]')
            foreach($child in @($module.ChildNodes|Where-Object {$_ -is [Xml.XmlElement] -and $_.LocalName -cin @('TriggeredBy','ConflictsWith','CustomAnimAndDuration')})){$null=$module.RemoveChild($child)}
            $animation=$xml.CreateElement('CustomAnimAndDuration','uri:ea.com:eala:asset');$animation.SetAttribute('AnimState','USER_8');$animation.SetAttribute('Frames','12');$animation.SetAttribute('Trigger','3');$null=$module.AppendChild($animation)
            foreach($entry in @(@('TriggeredBy','Upgrade_JapanFaction'),@('TriggeredBy','Upgrade_AlliesFaction'),@('TriggeredBy','Upgrade_Veterancy_HEROIC'),@('ConflictsWith','Upgrade_AlliesFaction'),@('ConflictsWith','Upgrade_JapanFaction'))){$child=$xml.CreateElement($entry[0],'uri:ea.com:eala:asset');$child.InnerText=$entry[1];$null=$module.AppendChild($child)}
        }
        $declaration=[Xml.XmlDocument]::new();$declaration.PreserveWhitespace=$true;$declaration.LoadXml('<!-- Reborn: developer-only variable Behaviors and explicit production command. --><AssetDeclaration xmlns="uri:ea.com:eala:asset"><Tags/><Includes/></AssetDeclaration>');$null=$declaration.DocumentElement.AppendChild($declaration.ImportNode($xml.DocumentElement,$true))
        $command=[Xml.XmlDocument]::new();$command.LoadXml('<LogicCommand xmlns="uri:ea.com:eala:asset" id="Command_ConstructSovietAntiInfantryInfantry" Type="UNIT_BUILD"><Object>RebornConscriptTrial</Object></LogicCommand>');$null=$declaration.DocumentElement.AppendChild($declaration.ImportNode($command.DocumentElement,$true));$declaration.Save($input)
        Invoke-TestCase VariableBehaviorsCombined $true;$good=(Get-FileHash -LiteralPath $package).Hash
        $receipt=Get-Content -LiteralPath (Join-Path $output 'build.json') -Raw|ConvertFrom-Json
        $expectedPrefix=if($UpgradeStructuresOnly){84216}else{84120}
        if(((-not $CombatListsOnly) -and ($receipt.Core.MetadataPrefix-ne $expectedPrefix -or $receipt.Core.ReferenceOccurrences-ne 271)) -or ($CombatListsOnly -and ($receipt.Core.MetadataPrefix-le 84120 -or $receipt.Core.ReferenceOccurrences-le 271)) -or $receipt.Core.AssetCount-ne 2 -or $receipt.Backend.AutomaticCommandGenerated){throw 'Variable source package evidence differs.'}
        if($CombatListsOnly){$expectedPrefix=$receipt.Core.MetadataPrefix}
        # Reborn: complete omission is another supported source shape, not a silent reuse of old armor/hardpoint arrays.
        if($CombatListsOnly){$empty=$declaration.CloneNode($true);foreach($node in @($empty.SelectNodes('//*[local-name()="GameObject"]/*[local-name()="ArmorSet"] | //*[local-name()="WeaponSetUpdate"]/*[local-name()="WeaponSlotHardpoint"]'))){$null=$node.ParentNode.RemoveChild($node)};$empty.Save($input);Invoke-TestCase CombatListsOmitted $true;$good=(Get-FileHash -LiteralPath $package).Hash}
        # Reborn: successful omission must remove both weak-list arrays and the optional nested allocation, not leave stale package bytes.
        if($UpgradeStructuresOnly){
            $empty=$declaration.CloneNode($true);$module=$empty.SelectSingleNode('//*[local-name()="ModelConditionUpgrade"]');foreach($child in @($module.ChildNodes|Where-Object {$_ -is [Xml.XmlElement] -and $_.LocalName -cin @('TriggeredBy','ConflictsWith','CustomAnimAndDuration')})){$null=$module.RemoveChild($child)};$empty.Save($input)
            Invoke-TestCase UpgradeNestedOmitted $true;$good=(Get-FileHash -LiteralPath $package).Hash
            $emptyReceipt=Get-Content -LiteralPath (Join-Path $output 'build.json') -Raw|ConvertFrom-Json;if($emptyReceipt.Core.MetadataPrefix-ne 84108){throw 'Omitted upgrade metadata extent differs.'}
        }
        $negativeCases=if($UpgradeStructuresOnly){@('Nested','Collision','UpgradeMissing','UpgradeSchema')}elseif($CombatListsOnly){@('CombatMissing','CombatSchema')}else{@('Nested','Collision')}
        foreach($kind in $negativeCases){
            $bad=$declaration.CloneNode($true);$list=$bad.SelectSingleNode('//*[local-name()="GameObject"]/*[local-name()="Behaviors"]');$nodes=@($list.ChildNodes|Where-Object {$_ -is [Xml.XmlElement]})
            switch($kind){
                Nested{$bad.SelectSingleNode('//*[local-name()="DieMuxData"]').SetAttribute('DeathTypes','CRUSHED ELECTROCUTED')}
                Collision{$nodes[1].SetAttribute('id',$nodes[0].GetAttribute('id'))}
                UpgradeMissing{$bad.SelectSingleNode('//*[local-name()="ModelConditionUpgrade"]/*[local-name()="TriggeredBy"]').InnerText='RebornMissingUpgrade'}
                UpgradeSchema{$bad.SelectSingleNode('//*[local-name()="CustomAnimAndDuration"]').SetAttribute('Frames','-1')}
                # Reborn: only genuinely missing named assets and invalid official enum values should reject combat input.
                CombatMissing{$bad.SelectSingleNode('//*[local-name()="ArmorSet"][@Armor]').SetAttribute('Armor','RebornMissingArmor')}
                CombatSchema{$bad.SelectSingleNode('//*[local-name()="WeaponSlotHardpoint"]/*[local-name()="Weapon"]').SetAttribute('Ordering','RebornInvalidOrdering')}
            }
            $bad.Save($input);Invoke-TestCase ('VariableBehaviorsReject'+$kind) $false;if((Get-FileHash -LiteralPath $package).Hash-cne $good){throw 'Rejected variable graph replaced the successful package.'}
        }
        # Reborn: the same variable shape must work through DATA includes, definitions and real instance inheritance, not only a flattened test XML.
        $base=[Xml.XmlDocument]::new();$base.PreserveWhitespace=$true;$base.LoadXml('<AssetDeclaration xmlns="uri:ea.com:eala:asset"/>');$game=$base.ImportNode($xml.DocumentElement,$true);$game.SetAttribute('id','VariableBehaviorBase');$null=$base.DocumentElement.AppendChild($game);$base.Save((Join-Path $data 'variable-base.xml'))
        [IO.File]::WriteAllText((Join-Path $data 'variable-defines.xml'),'<!-- Reborn: imported developer build-time definition. --><AssetDeclaration xmlns="uri:ea.com:eala:asset"><Defines><Define name="VARIABLE_BUILD" value="1"/></Defines></AssetDeclaration>')
        $inherited=[Xml.XmlDocument]::new();$inherited.PreserveWhitespace=$true;$inherited.LoadXml('<!-- Reborn: variable behavior graph through the normal EA source pipeline. --><AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes><Include type="all" source="DATA:variable-defines.xml"/><Include type="instance" source="DATA:variable-base.xml"/></Includes><GameObject id="RebornConscriptTrial" inheritFrom="VariableBehaviorBase" BuildTime="=$VARIABLE_BUILD"/></AssetDeclaration>');$null=$inherited.DocumentElement.AppendChild($inherited.ImportNode($command.DocumentElement,$true));$inherited.Save($input)
        Invoke-TestCase VariableBehaviorsInherited $true
        $receipt=Get-Content -LiteralPath (Join-Path $output 'build.json') -Raw|ConvertFrom-Json;if($receipt.Core.MetadataPrefix-ne $expectedPrefix -or ((-not $CombatListsOnly)-and $receipt.Core.ReferenceOccurrences-ne 271) -or @($receipt.Backend.AuthoredSources).Count-ne 3){throw 'Variable inherited source proof differs.'}
    }elseif($DiagnosticsOnly){
        # Reborn: failed XML builds must expose the actual physical source coordinate in Studio-readable output and retain the last successful package.
        $prior=if(Test-Path -LiteralPath $package){(Get-FileHash -LiteralPath $package).Hash}else{$null}
        foreach($kind in @('Schema','Scalar','Module','Structure','Syntax')){
            Set-TestFixture ConscriptBuildTime1;$xml=[Xml.XmlDocument]::new();$xml.PreserveWhitespace=$true;$xml.Load($input)
            switch($kind){
                Schema{$xml.DocumentElement.SetAttribute('UnknownRootField','1')}
                Scalar{$xml.DocumentElement.SetAttribute('Scale','NaN')}
                Module{$xml.SelectSingleNode('//*[local-name()="Physics"]').SetAttribute('GravityMultiplier','NaN')}
                # Reborn: valid replacement keys are supported; only an actual wire-encoding error should fail this case.
                Structure{$xml.SelectSingleNode('//*[local-name()="DisplayName"]').InnerText='Name:Invalidé'}
            }
            $xml.Save($input);if($kind-eq 'Syntax'){[IO.File]::WriteAllText($input,"<?xml version=`"1.0`"?>`n`n<GameObject>`n</Wrong>")}
            $label='XmlDiagnostics'+$kind;Invoke-TestCase $label $false
            $text=Get-Content -LiteralPath (Join-Path $sdk ('.work/test-'+$label+'.log')) -Raw
            if($text-notmatch ([regex]::Escape($input)+'\([1-9][0-9]*,[1-9][0-9]*\):') -or $text-match 'source context;'){throw ('Exact XML source coordinate missing: '+$label)}
            if($prior -and (Get-FileHash -LiteralPath $package).Hash-cne $prior){throw 'Rejected XML replaced the last successful package.'}
        }
    }elseif($ReferenceRetargetingOnly -or $ModuleValuesOnly){
        # Reborn: preserve the fixture's significant script/mixed-content whitespace while editing attributes, as in the full regression path.
        Set-TestFixture ConscriptBuildTime1;$xml=[Xml.XmlDocument]::new();$xml.PreserveWhitespace=$true;$xml.Load($input);$ns=[Xml.XmlNamespaceManager]::new($xml.NameTable);$ns.AddNamespace('a','uri:ea.com:eala:asset')
        $weapons=@($xml.SelectNodes('//a:WeaponSlotHardpoint/a:Weapon',$ns));if($weapons.Count-ne 2){throw 'Weapon fixture topology differs.'}
        foreach($weapon in $weapons){$weapon.SetAttribute('Template','JapanArcherInfantryEnergyBow')}
        $xml.DocumentElement.RemoveAttribute('UnitTypeIcon');$xml.DocumentElement.SetAttribute('UnitIntro','');$xml.DocumentElement.SetAttribute('EditorName','RebornReferenceRetargetingCombined')
        $xml.DocumentElement.SelectSingleNode('a:Behaviors/a:Physics',$ns).SetAttribute('GravityMultiplier','1.25');$xml.DocumentElement.SelectSingleNode('a:AI/a:AIUpdate',$ns).SetAttribute('StopChaseDistance','42.5')
        # Reborn: combine renamed local scope, reviewed enum/flag edits, reference retargeting, root import removal and resized strings in one published package.
        if($ModuleValuesOnly){
            $modules=@($xml.SelectNodes('//a:Draws/* | //a:Behaviors/* | //a:AI/* | //a:Body/* | //a:ClientUpdates/* | //a:ClientBehaviors/*',$ns));if($modules.Count-ne 49){throw 'Module fixture topology differs.'}
            $names=@{};for($i=0;$i-lt $modules.Count;$i++){$old=$modules[$i].GetAttribute('id');$names[$old]='RebornSdkLocal_'+$i;$modules[$i].SetAttribute('id',$names[$old])}
            foreach($module in $modules){if($module.HasAttribute('AssociatedModule')){$module.SetAttribute('AssociatedModule',$names[$module.GetAttribute('AssociatedModule')])}}
            foreach($draw in $xml.SelectNodes('//a:TracerModelDraw',$ns)){$draw.SetAttribute('WeaponSlotType','SECONDARY_WEAPON')}
            $xml.SelectSingleNode('//a:SpecialPower',$ns).SetAttribute('ChangeWeather','SNOWY');$xml.SelectSingleNode('//a:FreefallUpdate',$ns).SetAttribute('ModelConditionsToSet','FREEFALL DYING');$xml.SelectSingleNode('//a:HitReactionBehavior',$ns).SetAttribute('RequiredDeathTypes','ALL')
        }
        $xml.Save($input);Invoke-TestCase $(if($ModuleValuesOnly){'ConscriptModuleValuesCombined'}else{'ConscriptReferenceCombined'}) $true;$referenceHash=(Get-FileHash -LiteralPath $package).Hash
        $referenceReceipt=Get-Content -LiteralPath (Join-Path $output 'build.json') -Raw|ConvertFrom-Json;if($referenceReceipt.Core.ReferenceOccurrences-ne 273){throw 'Combined reference presence count differs.'}
        # Reborn: every failed module edit must leave the complete last-good package untouched, including hash-colliding local IDs.
        if($ModuleValuesOnly){foreach($kind in @('Enum','Flags','Identity')){$badXml=$xml.CloneNode($true);$badNs=[Xml.XmlNamespaceManager]::new($badXml.NameTable);$badNs.AddNamespace('a','uri:ea.com:eala:asset');switch($kind){Enum{$badXml.SelectSingleNode('//a:TracerModelDraw',$badNs).SetAttribute('WeaponSlotType','RebornBadEnum')}Flags{$badXml.SelectSingleNode('//a:FreefallUpdate',$badNs).SetAttribute('ModelConditionsToSet','RebornBadFlag')}Identity{$nodes=$badXml.SelectNodes('//a:Draws/*',$badNs);$nodes[1].SetAttribute('id',$nodes[0].GetAttribute('id').ToUpperInvariant())}};$badXml.Save($input);Invoke-TestCase ('ConscriptModuleReject'+$kind) $false;if((Get-FileHash -LiteralPath $package).Hash-cne $referenceHash){throw 'Rejected module edit replaced good output.'}}}
        foreach($bad in @('RebornMissingWeapon','GameObject:SovietAntiInfantryInfantry','JapanArcherInfantryEnergyBow\1')){
            $weapons[0].SetAttribute('Template',$bad);$xml.Save($input);Invoke-TestCase ('ConscriptReferenceReject'+$rows.Count) $false
            if((Get-FileHash -LiteralPath $package).Hash-cne $referenceHash){throw 'Rejected reference replaced the last good package.'}
        }
    }else{
    foreach($name in @('Reborn','ArmorCannon','CombinedArmor','CombinedArmorStrong','ConscriptAK47','ArmorSource','ConscriptBuildTime1')){Set-TestFixture $name;Invoke-TestCase $name $true}
    # Reborn: newly source-compiled Conscript must reproduce the game-observed archive, not just a success label.
    $conscriptHash=(Get-FileHash -LiteralPath $package).Hash;if($conscriptHash-cne '3FCDFFB9909B39E9C3A6B75AC50791A19D3EFBAE5A83D6169C174197C50C8FB2'){throw 'Conscript golden differs.'}
    # Reborn: one negative Studio batch must list two absent assets and two absent definitions, including a broken definition chain, without replacing the golden package.
    Set-TestFixture ConscriptBuildTime1;$original=[Xml.XmlDocument]::new();$original.Load($input);$auditXml=[Xml.XmlDocument]::new();$auditXml.LoadXml('<AssetDeclaration xmlns="uri:ea.com:eala:asset"><Defines><Define name="CHAIN" value="=$MISSING_ONE"/><Define name="OTHER" value="=$MISSING_TWO"/></Defines></AssetDeclaration>');$owner=$auditXml.ImportNode($original.DocumentElement,$true);$owner.SetAttribute('UnitTypeIcon','RebornMissingIcon');$owner.SetAttribute('UnitIntro','RebornMissingIntro');$owner.SetAttribute('BuildTime','=$CHAIN');$null=$auditXml.DocumentElement.AppendChild($owner);$auditXml.Save($input)
    Invoke-TestCase MissingDependencyBatch $false;$missing=Get-Content -LiteralPath (Join-Path $sdk '.work/last-dependencies.json') -Raw|ConvertFrom-Json
    if(@($missing.Findings|Where-Object Code -EQ 'MISSINGASSET').Count-ne 2 -or @($missing.Findings|Where-Object Code -EQ 'MISSINGDEF').Count-ne 2 -or -not @($missing.Findings|Where-Object {$_.Code-eq 'MISSINGDEF' -and $_.Owner-eq 'Define:CHAIN'}) -or $missing.StockAssets-lt 1 -or $missing.ModAssets-ne 1 -or (Get-FileHash -LiteralPath $package).Hash-cne $conscriptHash){throw 'Grouped missing dependency evidence or last-good package differs.'}
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
    }
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
