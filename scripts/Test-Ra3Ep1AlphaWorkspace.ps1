# Reborn: exercise a prepared local workspace's prebuilt backend and own negative fixtures without opening Studio or a game.
[CmdletBinding()]
param([Parameter(Mandatory)][string]$Workspace,[Parameter(Mandatory)][string]$StaticManifest)
$ErrorActionPreference='Stop'
if($PSVersionTable.PSVersion.Major-lt 7){throw 'PowerShell 7 is required.'}
$Workspace=[IO.Path]::GetFullPath($Workspace)
$inventory=Get-Content -LiteralPath (Join-Path $Workspace 'workspace-preparation.json') -Raw|ConvertFrom-Json
foreach($entry in $inventory.Files){
    if([IO.Path]::IsPathRooted($entry.Relative) -or $entry.Relative.Contains('..') -or $entry.Relative.Contains(':')){throw 'Invalid workspace inventory path.'}
    $path=Join-Path $Workspace $entry.Relative
    # Reborn: a redirected subdirectory is refused as well as a redirected file before inventory reads.
    for($item=Get-Item -LiteralPath $path;$null-ne $item;$item=if($item.PSIsContainer){$item.Parent}else{$item.Directory}){
        if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected workspace inventory refused.'}
    }
    if((Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash-ne $entry.Sha256){throw ('Workspace inventory mismatch: '+$entry.Relative)}
}
if(Test-Path -LiteralPath (Join-Path $Workspace 'source')){throw 'Clean alpha test must not contain a source build tree.'}
$test=Join-Path $Workspace ('offline-test-'+[guid]::NewGuid().ToString('N'))
$null=New-Item -ItemType Directory -Path $test
$runtime=(Get-Process -Id $PID).Path
$driver=Join-Path $Workspace 'scripts/Invoke-Ra3Ep1AlphaBuild.ps1'
$compiler=Join-Path $Workspace 'tools/compiler'
$output=Join-Path $Workspace 'builtmods'
$rows=[Collections.Generic.List[object]]::new()
# Reborn: execute the copied driver in child PowerShell processes so failure exit codes cannot terminate the test harness.
# Reborn: exercise both pair revisions alongside all four unchanged single-armor golden packages.
foreach($name in @('Reborn','Armor250','ArmorCannon','SovietArmor','CombinedArmor','CombinedStrong')){
    # Reborn: every exported sample must satisfy the unchanged Studio adapter's mod-name bound.
    if($name.Length-gt 15){throw 'Sample exceeds Studio mod-name length.'}
    $before=@(Get-ChildItem -LiteralPath $output -Directory).FullName
    $log=Join-Path $test ($name+'.log')
    & $runtime -NoProfile -File $driver -ModXml (Join-Path $Workspace ('mods/'+$name+'/data/mod.xml')) -StaticManifest $StaticManifest -CompilerDirectory $compiler -OutputRoot $output *> $log
    if($LASTEXITCODE-ne 0){throw ('Prebuilt workspace build failed: '+$log)}
    $created=@(Get-ChildItem -LiteralPath $output -Directory|Where-Object FullName -NotIn $before)
    if($created.Count-ne 1){throw 'Expected exactly one fresh alpha output.'}
    $receipt=Get-Content -LiteralPath (Join-Path $created[0].FullName 'alpha-build.json') -Raw|ConvertFrom-Json
    $build=Get-Content -LiteralPath $receipt.BuildReceipt -Raw|ConvertFrom-Json
    # Reborn: pair receipts have two independent member value sets; verify the combined golden without treating it as one GUN scalar.
    if($name -cin @('CombinedArmor','CombinedStrong')){
        # Reborn: pin each complete shared-stream package independently; a single-member digest cannot satisfy this check.
        $digest=if($name-ceq 'CombinedStrong'){'A9878D5120EF7037C2B2C4ABEC2679BBB6B57EEC9B3D35CCFE2644A8AA368385'}else{'826859F2CD92D840B37D0AABB239DF1C799F433A3A8014912F34757AEF5554E3'}
        if($receipt.ArmorCount-ne 2 -or $build.ArmorCount-ne 2 -or @($receipt.Assets).Count-ne 2 -or @($build.Assets).Count-ne 2 -or $build.ArchiveSha256-cne $digest -or -not $receipt.PrebuiltCompilerUsed -or -not $build.ActualCoreOutputUsed -or -not $build.AuthoredPayloadVerified -or $build.GameExecuted){throw 'Combined package receipt differs.'}
        # Reborn: pair builds obey the same isolated prebuilt compiler boundary as single-armor samples.
        if($receipt.CompilerPath-cne (Join-Path $compiler 'BinaryAssetBuilder.ManifestInspector.exe') -or (Test-Path -LiteralPath (Join-Path $created[0].FullName 'compiler-build.log'))){throw 'Combined build escaped the prebuilt compiler boundary.'}
        foreach($identity in @('AlliedConYardArmor','SovietConYardArmor')){
            $member=@($build.Assets|Where-Object ArmorId -CEQ $identity)
            $expected=if($identity-ceq 'AlliedConYardArmor' -and $name-ceq 'CombinedArmor'){250}else{5000}
            if($member.Count-ne 1 -or $member[0].EditableFloatCount-ne 18 -or $member[0].Percentages.'Armor/GUN'-ne $expected){throw 'Combined member values differ.'}
        }
        $text=Get-Content -LiteralPath $log -Raw
        if(-not $text.Contains('Warning: ALPHA001') -or -not $text.Contains('Warning: ALPHA002') -or $text.Contains('Error: ALPHA100') -or $text.Contains('Critical: ALPHA101')){throw 'Combined callback diagnostics differ.'}
        $rows.Add([pscustomobject]@{Sample=$name;ArmorCount=2;Receipt=(Join-Path $created[0].FullName 'alpha-build.json');ArchiveSha256=$digest})
        continue
    }
    # Reborn: preserve both legacy golden packages and independently pin the new non-GUN example.
    $expected=switch($name){'Reborn'{5000};'Armor250'{250};'ArmorCannon'{100};'SovietArmor'{5000}}
    $digest=switch($name){'Reborn'{'DD7DE09E3A81B9714DC2861ADE0AA9ECDFE72A1851EDD1ED20BE2155EF31C2DC'};'Armor250'{'E97EAEE9D2B423AC7360C9A401FAB1EC9A55B60F50B9F02BB74CEDC9E8B38F7E'};'ArmorCannon'{'40C09781BAB82643FBBC6DF5A8DFA674B74BF16B8B93F4A5B605FF5778F0BE7F'};'SovietArmor'{'0414E554AE9049631B163C508806F2A5EC2AE277030CE1A5DEE9374150C483BD'}}
    # Reborn: independently check faction identity in both receipts, not just the numeric GUN value and known archive digest.
    $identity=if($name-ceq 'SovietArmor'){'SovietConYardArmor'}else{'AlliedConYardArmor'}
    if($receipt.ArmorId-cne $identity -or $build.ArmorId-cne $identity){throw 'Sample armor identity differs.'}
    if(-not $receipt.PrebuiltCompilerUsed -or $receipt.CompilerPath-cne (Join-Path $compiler 'BinaryAssetBuilder.ManifestInspector.exe') -or $build.GunPercent-ne $expected -or $build.ArchiveSha256-cne $digest -or $build.GameExecuted -or -not $build.AuthoredPayloadVerified){throw 'Workspace receipt/package differs.'}
    if($receipt.EditableFloatCount-ne 18 -or $build.EditableFloatCount-ne 18 -or ($name-eq 'ArmorCannon' -and $build.Percentages.'Armor/CANNON'-ne 5000)){throw 'Expanded armor values differ.'}
    if(Test-Path -LiteralPath (Join-Path $created[0].FullName 'compiler-build.log')){throw 'Standalone build unexpectedly rebuilt repository source.'}
    $text=Get-Content -LiteralPath $log -Raw
    if(-not $text.Contains('Warning: ALPHA001') -or -not $text.Contains('Warning: ALPHA002')){throw 'EA-compatible warning prefixes missing.'}
    # Reborn: successful packages must not carry the failure marker that drives EA Studio's callback.
    if($text.Contains('Error: ALPHA100') -or $text.Contains('Critical: ALPHA101')){throw 'Successful build emitted a failure diagnostic.'}
    $rows.Add([pscustomobject]@{Sample=$name;GunPercent=$expected;Receipt=(Join-Path $created[0].FullName 'alpha-build.json');ArchiveSha256=$digest})
}
# Reborn: a DTD in an owned negative fixture must fail before publishing any success receipt/package.
$invalid=Join-Path $test 'invalid.xml'
[IO.File]::WriteAllText($invalid,'<!-- Reborn: owned rejection fixture. --><!DOCTYPE x [<!ENTITY x "1">]><ArmorTemplate xmlns="uri:ea.com:eala:asset" id="AlliedConYardArmor"/>')
$before=@(Get-ChildItem -LiteralPath $output -Directory).FullName
$negativeLog=Join-Path $test 'invalid.log'
& $runtime -NoProfile -File $driver -ModXml $invalid -StaticManifest $StaticManifest -CompilerDirectory $compiler -OutputRoot $output *> $negativeLog
# Reborn: assert both detailed error and critical callback markers; EA Studio does not inspect process ExitCode.
$negativeExit=$LASTEXITCODE
$negativeText=Get-Content -LiteralPath $negativeLog -Raw
if($negativeExit-ne 1 -or -not $negativeText.Contains('Error: ALPHA100') -or -not $negativeText.Contains('Critical: ALPHA101')){throw 'Unsupported input did not return the expected error and Studio failure marker.'}
if(@(Get-ChildItem -LiteralPath $output -Directory|Where-Object FullName -NotIn $before).Count-ne 0){throw 'Invalid XML created alpha output.'}
foreach($entry in $inventory.Files){if((Get-FileHash -LiteralPath (Join-Path $Workspace $entry.Relative) -Algorithm SHA256).Hash-ne $entry.Sha256){throw 'Workspace input modified by build.'}}
$report=[pscustomobject]@{Workspace=$Workspace;InventoryPassed=$true;Samples=$rows;
    UnsupportedXmlRefused=$true;EaDiagnosticPrefixesPassed=$true;CriticalFailureMarkerPassed=$true;SuccessHasNoFailureMarker=$true;SourceTreeAbsent=$true;PrebuiltModePassed=$true;
    StudioExecuted=$false;GameExecuted=$false;PublicRedistributionReviewed=$false;UiValidated=$false}
[IO.File]::WriteAllText((Join-Path $test 'workspace-test.json'),($report|ConvertTo-Json -Depth 5))
$report
