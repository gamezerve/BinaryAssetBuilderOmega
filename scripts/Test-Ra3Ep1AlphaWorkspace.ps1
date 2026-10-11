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
foreach($name in @('Reborn','Armor250','ArmorCannon')){
    $before=@(Get-ChildItem -LiteralPath $output -Directory).FullName
    $log=Join-Path $test ($name+'.log')
    & $runtime -NoProfile -File $driver -ModXml (Join-Path $Workspace ('mods/'+$name+'/data/mod.xml')) -StaticManifest $StaticManifest -CompilerDirectory $compiler -OutputRoot $output *> $log
    if($LASTEXITCODE-ne 0){throw ('Prebuilt workspace build failed: '+$log)}
    $created=@(Get-ChildItem -LiteralPath $output -Directory|Where-Object FullName -NotIn $before)
    if($created.Count-ne 1){throw 'Expected exactly one fresh alpha output.'}
    $receipt=Get-Content -LiteralPath (Join-Path $created[0].FullName 'alpha-build.json') -Raw|ConvertFrom-Json
    $build=Get-Content -LiteralPath $receipt.BuildReceipt -Raw|ConvertFrom-Json
    # Reborn: preserve both legacy golden packages and independently pin the new non-GUN example.
    $expected=switch($name){'Reborn'{5000};'Armor250'{250};'ArmorCannon'{100}}
    $digest=switch($name){'Reborn'{'DD7DE09E3A81B9714DC2861ADE0AA9ECDFE72A1851EDD1ED20BE2155EF31C2DC'};'Armor250'{'E97EAEE9D2B423AC7360C9A401FAB1EC9A55B60F50B9F02BB74CEDC9E8B38F7E'};'ArmorCannon'{'40C09781BAB82643FBBC6DF5A8DFA674B74BF16B8B93F4A5B605FF5778F0BE7F'}}
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
