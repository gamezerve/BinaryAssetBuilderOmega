# Reborn: test final packaging with disposable owned fixtures; never publish diagnostic packages into Documents.
[CmdletBinding()]
param()
$ErrorActionPreference='Stop'
. (Join-Path $PSScriptRoot 'Ra3Ep1FinalPackage.ps1')
$root=Join-Path (Split-Path -Parent $PSScriptRoot) ('alpha/.work/final-package-test-'+[guid]::NewGuid().ToString('N'))
$null=New-Item -ItemType Directory -Path $root
$misc=Join-Path $root 'Misc';$null=New-Item -ItemType Directory -Path (Join-Path $misc 'Data/Scripts') -Force
# Reborn: fixture writes are test data, including an excluded intermediate and a nested archive-relative path.
[IO.File]::WriteAllText((Join-Path $misc 'Data/Scripts/test.lua'),'-- Reborn: Misc fixture.')
[IO.File]::WriteAllText((Join-Path $misc 'ignored.asset'),'Reborn: not a distributable asset.')
# Reborn: exercise payload readback across multiple bounded buffers, including an empty entry.
$binary=[byte[]]::new(131089);[Random]::new(42).NextBytes($binary)
[IO.File]::WriteAllBytes((Join-Path $misc 'payload.bin'),$binary)
[IO.File]::WriteAllBytes((Join-Path $misc 'empty.dat'),[byte[]]::new(0))
$streams=Join-Path $root 'streams.big';[IO.File]::WriteAllBytes($streams,[byte[]](1,2,3,4))
$mods=Join-Path $root 'Mods';$checks=[Collections.Generic.List[string]]::new()
#-------------------------------------------------------------------------------------------------
<# Reborn: require malformed or conflicting publication to fail instead of silently replacing user files. #>
#-------------------------------------------------------------------------------------------------
function Expect-PackageFailure([scriptblock]$Action,[string]$Name){$failed=$false;try{& $Action | Out-Null}catch{$failed=$true};if(-not $failed){throw ('Expected failure: '+$Name)};$checks.Add($Name)}
$first=Publish-RebornFinalPackage RebornTest 1.00 $streams $misc $mods
if($first.MiscEntries -ne 3 -or (Split-Path -Leaf $first.StreamsPath) -cne 'RebornTest_1.00_Streams.big'){throw 'Version or Misc count differs.'}
if((Get-Content -LiteralPath $first.SkuDefPath -Raw) -cne "add-big RebornTest_1.00_Streams.big`r`nadd-big RebornTest_1.00_Misc.big`r`n"){throw 'SKUDef differs.'};$checks.Add('NamesAndSku')
$raw=[IO.File]::ReadAllBytes($first.MiscPath);if([Text.Encoding]::ASCII.GetString($raw) -notlike '*Data\Scripts\test.lua*' -or [Text.Encoding]::ASCII.GetString($raw) -like '*ignored.asset*'){throw 'Misc relative paths or exclusion differ.'};$checks.Add('MiscPathsAndAssetExclusion')
$hash=(Get-FileHash -LiteralPath $first.MiscPath).Hash
$again=Publish-RebornFinalPackage RebornTest 1.00 $streams $misc $mods
if((Get-FileHash -LiteralPath $again.MiscPath).Hash -cne $hash){throw 'Misc output is not deterministic.'};$checks.Add('SameVersionRebuild')
$note=Join-Path $first.Directory 'user-note.txt';[IO.File]::WriteAllText($note,'Reborn: retain me.')
$second=Publish-RebornFinalPackage RebornTest 2.00 $streams (Join-Path $root 'MissingMisc') $mods
if(-not(Test-Path -LiteralPath $first.SkuDefPath) -or -not(Test-Path -LiteralPath $note) -or $second.MiscPath -or (Test-Path -LiteralPath (Join-Path $second.Directory 'RebornTest_2.00_Misc.big')) -or (Get-Content -LiteralPath $second.SkuDefPath -Raw) -cne "add-big RebornTest_2.00_Streams.big`r`n"){throw 'Other versions/notes or absent Misc differ.'};$checks.Add('OtherVersionsNotesAndMissingMisc')
# Reborn: remove the previous same-version owned archive when Misc disappears, and allow it to return later.
$without=Publish-RebornFinalPackage RebornTest 1.00 $streams (Join-Path $root 'MissingMisc') $mods
if($without.MiscPath -or (Test-Path -LiteralPath $first.MiscPath) -or (Get-Content -LiteralPath $without.SkuDefPath -Raw) -cne "add-big RebornTest_1.00_Streams.big`r`n"){throw 'Stale same-version Misc was not removed.'};$checks.Add('OwnedMiscRemoved')
$empty=Join-Path $root 'EmptyMisc';$null=New-Item -ItemType Directory -Path (Join-Path $empty 'EmptySubfolder') -Force
$emptyResult=Publish-RebornFinalPackage RebornTest 1.00 $streams $empty $mods
if($emptyResult.MiscPath -or $emptyResult.MiscEntries -ne 0 -or (Test-Path -LiteralPath $first.MiscPath)){throw 'Empty Misc produced an archive.'};$checks.Add('EmptyDirectorySkipped')
[IO.File]::WriteAllText((Join-Path $empty 'excluded.ASSET'),'Reborn: excluded intermediate.')
$excluded=Publish-RebornFinalPackage RebornTest 1.00 $streams $empty $mods
if($excluded.MiscPath -or (Get-Content -LiteralPath $excluded.SkuDefPath -Raw) -match '_Misc.big'){throw 'Excluded-only Misc produced an archive or SKU entry.'};$checks.Add('ExcludedOnlyMiscSkipped')
$restored=Publish-RebornFinalPackage RebornTest 1.00 $streams $misc $mods
if($restored.MiscEntries -ne 3 -or (Get-FileHash -LiteralPath $restored.MiscPath).Hash -cne $hash){throw 'Misc did not return after adding files.'};$checks.Add('MiscAddedBack')
Expect-PackageFailure {Publish-RebornFinalPackage RebornTest '../bad' $streams $misc $mods} 'InvalidVersion'
Expect-PackageFailure {Publish-RebornFinalPackage RebornTest "1.00`n" $streams $misc $mods} 'VersionTrailingNewline'
# Reborn: force a failure after the first two previous files moved and require a complete rollback.
$prior=@{};foreach($path in @($second.StreamsPath,$second.SkuDefPath)){$prior[$path]=(Get-FileHash -LiteralPath $path).Hash}
$blocked=[IO.File]::Open($second.SkuDefPath,[IO.FileMode]::Open,[IO.FileAccess]::Read,[IO.FileShare]::Read)
try{Expect-PackageFailure {Publish-RebornFinalPackage RebornTest 2.00 $streams $misc $mods} 'RollbackAfterPartialMove'}finally{$blocked.Dispose()}
foreach($path in $prior.Keys){if((Get-FileHash -LiteralPath $path).Hash -cne $prior[$path]){throw 'Rollback did not restore all previous files.'}}
# Reborn: rollback also restores a removed old Misc when a streams-only replacement cannot move its locked SKU.
$priorMiscHash=(Get-FileHash -LiteralPath $first.MiscPath).Hash
$blocked=[IO.File]::Open($first.SkuDefPath,[IO.FileMode]::Open,[IO.FileAccess]::Read,[IO.FileShare]::Read)
try{Expect-PackageFailure {Publish-RebornFinalPackage RebornTest 1.00 $streams $empty $mods} 'RollbackRestoresRemovedMisc'}finally{$blocked.Dispose()}
if((Get-FileHash -LiteralPath $first.MiscPath).Hash -cne $priorMiscHash){throw 'Rollback lost the old Misc archive.'}
[IO.File]::WriteAllText($first.SkuDefPath,'Reborn: user edited this.')
Expect-PackageFailure {Publish-RebornFinalPackage RebornTest 1.00 $streams $misc $mods} 'ModifiedOwnedFile'
if((Get-Content -LiteralPath $first.SkuDefPath -Raw) -cne 'Reborn: user edited this.'){throw 'User-edited SKU was overwritten.'}
[IO.File]::WriteAllText((Join-Path $first.Directory 'RebornTest_3.00_Streams.big'),'Reborn: unmanaged file.')
Expect-PackageFailure {Publish-RebornFinalPackage RebornTest 3.00 $streams $misc $mods} 'UnmanagedCollision'
# Reborn: hold the destination lock to prove another publisher cannot interleave replacement operations.
$held=[IO.File]::Open((Join-Path $first.Directory '.reborn-publish.lock'),[IO.FileMode]::Open,[IO.FileAccess]::ReadWrite,[IO.FileShare]::None)
try{Expect-PackageFailure {Publish-RebornFinalPackage RebornTest 4.00 $streams $misc $mods} 'ConcurrentPublication'}finally{$held.Dispose()}
# Reborn: cloud tags may pass, but a real junction in Misc must still be refused without following it.
if($IsWindows){
    $junction=Join-Path $root 'RedirectedMisc';$null=New-Item -ItemType Junction -Path $junction -Target $misc
    try{Expect-PackageFailure {Publish-RebornFinalPackage RebornTest 5.00 $streams $junction $mods} 'JunctionRefused'}finally{[IO.Directory]::Delete($junction)}
    $cloud=Join-Path ([Environment]::GetFolderPath([Environment+SpecialFolder]::MyDocuments)) 'Red Alert 3 Uprising'
    if((Test-Path -LiteralPath $cloud) -and ((Get-Item -LiteralPath $cloud -Force).Attributes -band [IO.FileAttributes]::ReparsePoint) -and [Reborn.PackageReparseTag]::IsCloud($cloud)){Assert-RebornPackagePath $cloud;$checks.Add('OneDriveCloudAcceptedReadOnly')}
}
if(@(Get-ChildItem -LiteralPath $first.Directory -Filter '.reborn-stage-*' -Force).Count){throw 'Unexpected abandoned staging directory.'}
[IO.File]::WriteAllText((Join-Path $root 'report.json'),([pscustomobject]@{Passed=$checks.Count;Checks=$checks;DocumentsModified=$false}|ConvertTo-Json))
Write-Output ('Reborn: final packaging PASS ('+$checks.Count+' checks); evidence '+$root)
