# Reborn: stable SDK front end; preserve editable sources, publish versioned Documents mods, never change game installation files or launch a game.
[CmdletBinding()]
param([string]$ModName='RebornTest',[string]$ModXml,[switch]$ValidateOnly,[string]$StaticManifest,[string]$GlobalManifest,[string]$CompilerDirectory,[string]$OutputRoot,[string]$StudioSession,[string]$ModVersion='1.00',[string]$FinalModsRoot,[ValidateSet(0,1)][int]$BuildGlobalData=1,[ValidateSet(0,1)][int]$DeveloperBuildTests=0)
$ErrorActionPreference='Stop';$repo=Split-Path -Parent $PSScriptRoot;$sdk=Join-Path $repo 'alpha';$lock=$null;$run=$null;$publishMutex=$null;$cancelEvent=$null;$publishHeld=$false;$runOwned=$false;$stageOwned=$false
# Reborn: retain the original Studio envelope while adding truthful numbered backend phases and separate detailed output.
. (Join-Path $PSScriptRoot 'Ra3Ep1BuildLog.ps1')
# Reborn: the final user package targets Documents; OutputRoot remains the private compatibility receipt/cache location.
. (Join-Path $PSScriptRoot 'Ra3Ep1FinalPackage.ps1')
# Reborn: compile early AdditionalMaps streams independently of static data, then include both in the same final Streams BIG.
. (Join-Path $PSScriptRoot 'Ra3Ep1GlobalData.ps1')
# Reborn: every native/global/backend child inherits this build's mode; restore the caller's environment even on cancellation or failure.
$previousDeveloperTests=$env:REBORN_DEVELOPER_BUILD_TESTS
#-------------------------------------------------------------------------------------------------
<# Reborn: delete only a newly owned work/output generation after proving containment and refusing every redirected descendant. #>
#-------------------------------------------------------------------------------------------------
function Remove-DevAlphaOwned([string]$Path,[string]$Parent){$resolved=[IO.Path]::GetFullPath($Path);if([IO.Path]::GetDirectoryName($resolved)-ine [IO.Path]::GetFullPath($Parent)){throw 'Cleanup target is not a direct owned child.'};foreach($item in @(Get-Item -LiteralPath $resolved -Force)+@(Get-ChildItem -LiteralPath $resolved -Force -Recurse)){if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected cleanup target retained.'}};Remove-Item -LiteralPath $resolved -Recurse -Force}
try{
    $env:REBORN_DEVELOPER_BUILD_TESTS=[string]$DeveloperBuildTests
    Write-SdkBuildLine $(if($DeveloperBuildTests){'Build mode: Developer (build tests enabled).'}else{'Build mode: Fast (developer tests disabled; source/reference/output validation remains enabled).'})
    Start-SdkBuildPhase 1 'Clearing build cache';Stop-SdkBuildPhase 'Skipped: this alpha does not use a persistent asset cache.'
    Start-SdkBuildPhase 2 'Clearing session cache';Stop-SdkBuildPhase 'Skipped: this alpha does not use a persistent compiler session cache.'
    Start-SdkBuildPhase 3 'Clearing built mod'
    Write-SdkBuildLine 'Preparing private build workspace; previous successful output will be retained until publication.'
    if($PSVersionTable.PSVersion.Major-lt 7){throw 'PowerShell 7 is required.'}
    # Reborn: validate before compilation, retain formatting such as 1.00, and reject filename/argument injection.
    if($ModVersion -cnotmatch '\A[0-9]{1,4}(\.[0-9]{1,4}){1,3}\z'){throw 'Mod Version must be numeric, for example 1.00 or 1.2.3.'}
    # Reborn: only a Studio-created session supplies cancellation/publication synchronization; unrelated CLI builds remain independent.
    if($StudioSession){if($StudioSession-cnotmatch '^[a-f0-9]{32}$'){throw 'Invalid Studio session.'};$publishMutex=[Threading.Mutex]::OpenExisting('Local\RebornStudio-'+$StudioSession+'-Publish');$cancelEvent=[Threading.EventWaitHandle]::OpenExisting('Local\RebornStudio-'+$StudioSession+'-Cancel');if($cancelEvent.WaitOne(0)){throw 'Studio build cancelled before starting.'}}
    # Reborn: use a separate static mod entry without shadowing the original DATA:static.xml.
    if(-not $ModXml){$ModXml=Join-Path $sdk ('mods/'+$ModName+'/data/Static_Mod.xml')};$ModXml=[IO.Path]::GetFullPath($ModXml)
    $ModName=Split-Path -Leaf (Split-Path -Parent (Split-Path -Parent $ModXml))
    if($ModName-notmatch '^[A-Za-z0-9_-]{1,48}$' -or $ModXml-ine (Join-Path $sdk ('mods/'+$ModName+'/data/Static_Mod.xml'))){throw 'Select alpha/mods/<name>/data/Static_Mod.xml; paths cannot escape the live SDK.'}
    # Reborn: Studio already displays this header; include it once in the saved standalone transcript only.
    $script:BuildLogLines.Insert(0,('Building Mod '+$ModName))
    $settings=@{};foreach($line in Get-Content -LiteralPath (Join-Path $sdk 'uprising-alpha.paths')){if($line -and -not $line.StartsWith('#')){$pair=$line.Split('=',2);$settings[$pair[0]]=$pair[1]}}
    if(-not $StaticManifest){$StaticManifest=$settings.StaticManifest};if(-not $GlobalManifest){$GlobalManifest=$settings.GlobalManifest}
    $expectedCompiler=Join-Path $sdk 'tools/compiler';$expectedOutput=Join-Path $sdk 'builtmods'
    if($CompilerDirectory -and [IO.Path]::GetFullPath($CompilerDirectory)-ine $expectedCompiler){throw 'Stable alpha cannot use an unrelated compiler.'}
    if($OutputRoot -and [IO.Path]::GetFullPath($OutputRoot)-ine $expectedOutput){throw 'Stable alpha outputs stay inside alpha/builtmods.'}
    foreach($path in @($ModXml,$expectedCompiler,$expectedOutput,(Join-Path $sdk '.work'))){for($item=Get-Item -LiteralPath $path;$null-ne $item;$item=if($item.PSIsContainer){$item.Parent}else{$item.Directory}){if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected stable SDK path refused.'}}}
    # Reborn: one lock spans tool generation selection, backend execution and publication; concurrent sync cannot replace a running compiler generation.
    $lock=[IO.File]::Open((Join-Path $sdk '.sdk.lock'),[IO.FileMode]::OpenOrCreate,[IO.FileAccess]::ReadWrite,[IO.FileShare]::None)
    $toolchain=Get-Content -LiteralPath (Join-Path $sdk 'toolchain.json') -Raw|ConvertFrom-Json
    foreach($entry in $toolchain.Files){if((Get-FileHash -LiteralPath (Join-Path $expectedCompiler $entry.Name) -Algorithm SHA256).Hash-cne $entry.Sha256){throw 'Alpha compiler generation differs from its synchronization receipt.'}}
    $runId=if($StudioSession){$StudioSession}else{[guid]::NewGuid().ToString('N').Substring(0,8)}
    # Reborn: a refused occupied directory is never owned by this invocation and must not enter finally cleanup.
    $run=Join-Path $sdk ('.work/'+$runId);$null=New-Item -ItemType Directory -Path $run;$runOwned=$true;$log=Join-Path $run 'build.log'
    if($StudioSession){[IO.File]::WriteAllText((Join-Path $run 'studio-owner.txt'),$StudioSession)}
    # Reborn: routing/XML errors belong to static-data preparation, not cache cleanup.
    $detail=Join-Path $run 'compiler-detail.log'
    Stop-SdkBuildPhase
    Start-SdkBuildPhase 4 'Building AptUI';Stop-SdkBuildPhase 'Skipped: no separate AptUI compiler stage is enabled in this alpha; unsupported requested assets fail validation.'
    Start-SdkBuildPhase 5 'Building global data'
    # Reborn: an unchecked toggle omits global sources from this fresh package; never silently reuse an older source generation.
    $globalBuild=$null
    if($BuildGlobalData){Build-RebornGlobalData (Split-Path -Parent $ModXml) $sdk $settings $StaticManifest $GlobalManifest $expectedCompiler $run $detail ([ref]$globalBuild);if($globalBuild.Streams.Count){Stop-SdkBuildPhase ('Global streams compiled: '+$globalBuild.Streams.Count)}else{Stop-SdkBuildPhase}}else{Stop-SdkBuildPhase 'Skipped: [Optional] Build Global Data is unchecked; no AdditionalMaps streams will be packaged.'}
    Start-SdkBuildPhase 6 'Building static data'
    # Reborn: announce validation before reporting its entry-point load, so progress follows the actual phase order.
    Write-SdkBuildLine ('['+$script:BuildClock.Elapsed.ToString('c')+'] Validation started: dependency audit, source graph and schema admission.')
    Write-SdkBuildLine ('['+$script:BuildClock.Elapsed.ToString('c')+'] Loading '+$ModXml)
    # Reborn: list independent missing definitions/assets before the normal compiler stops at its first unsupported or invalid owner.
    $dependencyReport=Join-Path $run 'dependencies.json'
    $dependencyArguments=@('sdk-missing-dependencies',$ModXml,$sdk,$StaticManifest,$GlobalManifest,$dependencyReport)
    if($globalBuild){$dependencyArguments+=@($(if($settings.SdkDataRoot){$settings.SdkDataRoot}else{Join-Path $sdk 'SageXML'}),$globalBuild.Context)}elseif($settings.SdkDataRoot){$dependencyArguments+=$settings.SdkDataRoot}
    & (Join-Path $expectedCompiler 'BinaryAssetBuilder.ManifestInspector.exe') @dependencyArguments 2>&1 | ForEach-Object {Write-SdkCompilerLine ([string]$_) $detail};$dependencyCode=$LASTEXITCODE
    if(Test-Path -LiteralPath $dependencyReport){Copy-Item -LiteralPath $dependencyReport -Destination (Join-Path $sdk '.work/last-dependencies.json') -Force;Write-SdkBuildLine ('Dependency report: '+(Join-Path $sdk '.work/last-dependencies.json'))}
    if($dependencyCode-ne 0){throw 'Dependency preflight failed. Review the complete missing asset/definition list above.'}
    # Reborn: standalone GameObject XML uses the explicit complete-Conscript route; other assets retain their original admission and compiler path.
    $xmlSettings=[Xml.XmlReaderSettings]::new();$xmlSettings.DtdProcessing=[Xml.DtdProcessing]::Prohibit;$xmlSettings.XmlResolver=$null;$xmlSettings.MaxCharactersInDocument=1048576
    # Reborn: upgrade-only XML uses the same verified support stream route as command sets, including ordinary Studio publication.
    # Reborn: a bare FXList selects the same general support-asset backend as an included FXList.
    # Reborn: a bare W3DContainer uses the same support-asset compiler as a W3X included in an AssetDeclaration.
    $reader=[Xml.XmlReader]::Create($ModXml,$xmlSettings);try{$null=$reader.MoveToContent();$isConscript=$reader.LocalName-cin @('GameObject','FXParticleSystemTemplate','FXList','LogicCommandSet','UpgradeTemplate','W3DContainer','W3DHierarchy','W3DCollisionBox','W3DMesh');$isDeclaration=$reader.LocalName-ceq 'AssetDeclaration'}finally{$reader.Dispose()}
    # Reborn: detect GameObject declarations in a real local XML closure without an automatic production command.
    $searchArgs=@($sdk);if($settings.SdkDataRoot){$searchArgs+=$settings.SdkDataRoot}
    if($isDeclaration){$kind=@(& (Join-Path $expectedCompiler 'BinaryAssetBuilder.ManifestInspector.exe') conscript-source-kind $ModXml @searchArgs);if($LASTEXITCODE-ne 0){throw ($kind -join [Environment]::NewLine)};$isConscript=($kind -join '').Trim()-ceq 'conscript'}
    $backend=if($isConscript){'Invoke-Ra3Ep1ConscriptAlphaBuild.ps1'}else{'Invoke-Ra3Ep1AlphaBuild.ps1'}
    $arguments=@('-NoProfile','-File',(Join-Path $PSScriptRoot $backend),'-ModXml',$ModXml,'-StaticManifest',$StaticManifest,'-GlobalManifest',$GlobalManifest,'-CompilerDirectory',$expectedCompiler,'-OutputRoot',$run)
    if($ValidateOnly){$arguments+='-ValidateOnly'}
    # Reborn: static strong/weak reference lookup includes only globals actually built in this invocation.
    if($isConscript -and $globalBuild){$arguments+=@('-GlobalStreamContext',$globalBuild.Context)}
    # Reborn: the armor source backend uses the same explicit SDK search roots as GameObject routing/preparation.
    if(-not $isConscript){$arguments+=@('-SdkRoot',$sdk);if($settings.SdkDataRoot){$arguments+=@('-SdkDataRoot',$settings.SdkDataRoot)}}
    # Reborn: stream complete lines immediately; JSON/readback diagnostics remain in the full detail log.
    & $settings.PowerShell7 @arguments 2>&1 | ForEach-Object {Write-SdkCompilerLine ([string]$_) $detail};$code=$LASTEXITCODE
    if($code-ne 0){throw 'Alpha backend failed; previous successful output is unchanged.'}
    Stop-SdkBuildPhase
    if($ValidateOnly){Write-SdkBuildLine 'Validation Completed (no publication).';Write-SdkBuildLine ('Build time taken: '+$script:BuildClock.Elapsed.ToString('c'));[IO.File]::WriteAllLines((Join-Path $sdk '.work/last-validation.log'),$script:BuildLogLines);Copy-Item -LiteralPath $detail -Destination (Join-Path $sdk '.work/last-detail.log') -Force;return}
    Start-SdkBuildPhase 7 'Merging assets';Stop-SdkBuildPhase 'Skipped: no separate post-build asset merge is enabled.'
    Start-SdkBuildPhase 8 'Fixing assets'
    $receipts=@(Get-ChildItem -LiteralPath $run -Filter alpha-build.json -Recurse);if($receipts.Count-ne 1){throw 'Expected one successful alpha backend receipt.'}
    $receipt=Get-Content -LiteralPath $receipts[0].FullName -Raw|ConvertFrom-Json;$core=Get-Content -LiteralPath $receipt.BuildReceipt -Raw|ConvertFrom-Json
    # Reborn: report only measured backend counts/identities that actually exist in the selected receipt.
    if($null-ne $core.AssetCount){Write-SdkBuildLine ('Compiled assets: '+$core.AssetCount)}
    if($core.Asset){Write-SdkBuildLine ('Primary asset: '+$core.Asset)}
    $package=if($receipt.PackagePath){$receipt.PackagePath}elseif($core.PackagePath){$core.PackagePath}else{$core.DefaultReaderPackagePath}
    $package=[IO.Path]::GetFullPath($package);if(-not $package.StartsWith($run+[IO.Path]::DirectorySeparatorChar,[StringComparison]::OrdinalIgnoreCase)){throw 'Backend package escaped its private work generation.'}
    Write-SdkBuildLine 'Verified backend receipt and package location; native stream qualification ran during static-data compilation. No legacy RA3 hash-fix tool was run.'
    Stop-SdkBuildPhase
    Start-SdkBuildPhase 9 'Building official maps';Stop-SdkBuildPhase 'Skipped: no map compiler stage is enabled in this alpha; unsupported requested assets fail validation.'
    Start-SdkBuildPhase 10 'Creating big files'
    # Reborn: close cancellation must never terminate the worker halfway through staged publication/rollback of a last-good package.
    if($publishMutex){try{$publishHeld=$publishMutex.WaitOne()}catch [Threading.AbandonedMutexException]{$publishHeld=$true};if($cancelEvent.WaitOne(0)){throw 'Studio build cancelled before publication.'}}
    # Reborn: refuse a late higher-priority override even if the originally selected SDK file's hash is unchanged.
    foreach($selection in $receipt.PathSearches){
        $winner=$null
        foreach($candidate in $selection.Candidates){for($ancestor=$candidate;$ancestor;$ancestor=[IO.Path]::GetDirectoryName($ancestor)){if((Test-Path -LiteralPath $ancestor) -and ((Get-Item -LiteralPath $ancestor -Force).Attributes-band [IO.FileAttributes]::ReparsePoint)){throw 'Redirected alias search candidate.'}};if(Test-Path -LiteralPath $candidate -PathType Container){throw 'Alias candidate became a directory.'};if(Test-Path -LiteralPath $candidate -PathType Leaf){$winner=$candidate;break}}
        if($winner-ine $selection.PhysicalPath){throw ('Alias precedence changed: '+$selection.LogicalPath)}
    }
    $stage=Join-Path $expectedOutput ('.stage-'+[guid]::NewGuid().ToString('N'));$null=New-Item -ItemType Directory -Path $stage;$stageOwned=$true
    # Reborn: recheck global sources, then append their qualified five-file stream families without changing static payloads.
    if($globalBuild){Assert-RebornGlobalSources $globalBuild;if($globalBuild.Streams.Count){$combined=Join-Path $run 'combined-streams.big';& (Join-Path $expectedCompiler 'BinaryAssetBuilder.ManifestInspector.exe') sdk-package-global-streams $package $globalBuild.Context $combined 2>&1 | ForEach-Object {Write-SdkBuildLine ([string]$_)};if($LASTEXITCODE-ne 0){throw 'Global/static Streams BIG assembly failed.'};$package=$combined}}
    # Reborn: only unresolved WorldBuilder use sites become warnings, after extraction and global/static package assembly have finished.
    & (Join-Path $expectedCompiler 'BinaryAssetBuilder.ManifestInspector.exe') sdk-check-worldbuilder-resolution $package $dependencyReport (Join-Path $sdk 'catalogs/ra3ep1') 2>&1 | ForEach-Object {Write-SdkCompilerLine ([string]$_) $detail}
    if($LASTEXITCODE-ne 0){throw 'Post-resolution WorldBuilder dependency check failed.'}
    # Reborn: audit every final asset type after global/static assembly, using weak-use receipts as well as native reference tables.
    & (Join-Path $expectedCompiler 'BinaryAssetBuilder.ManifestInspector.exe') sdk-unused-assets $package $sdk $run $ModXml 2>&1 | ForEach-Object {Write-SdkCompilerLine ([string]$_) $detail}
    if($LASTEXITCODE-ne 0){throw 'Final asset usage audit failed.'}
    $big=$ModName+'.big';Copy-Item -LiteralPath $package -Destination (Join-Path $stage $big);$hash=(Get-FileHash -LiteralPath $package -Algorithm SHA256).Hash
    if((Get-FileHash -LiteralPath (Join-Path $stage $big) -Algorithm SHA256).Hash-cne $hash){throw 'Published BIG differs from the verified backend package.'}
    Write-SdkBuildLine ('Streams BIG prepared and SHA256 verified: '+$ModName+'_'+$ModVersion+'_Streams.big (compiled static/global stream families).')
    if($null-ne $core.ArchiveEntryCount){Write-SdkBuildLine (([int]$core.ArchiveEntryCount+$(if($globalBuild){5*$globalBuild.Streams.Count}else{0})).ToString()+' Files added (verified stream archives).')}
    Stop-SdkBuildPhase
    Start-SdkBuildPhase 11 'Creating skudef file'
    [IO.File]::WriteAllText((Join-Path $stage 'mod_1.0.skudef'),("add-big "+$big+"`n"),[Text.Encoding]::ASCII)
    $output=Join-Path $expectedOutput $ModName;$sku=Join-Path $output 'mod_1.0.skudef';if($sku.Length-gt 240){throw 'Choose a shorter mod directory for the conservative game path bound.'}
    $result=[pscustomobject]@{Format='RebornLiveAlpha1';ModName=$ModName;Source=$ModXml;SourceSha256=(Get-FileHash -LiteralPath $ModXml -Algorithm SHA256).Hash;BigPath=(Join-Path $output $big);SkuDefPath=$sku;PackageSha256=$hash;CompilerFiles=$toolchain.Files;Backend=$receipt;Core=$core;GameExecuted=$false;InstalledFilesModified=$false}
    # Reborn: retain global source provenance separately from the static backend's receipt after private work cleanup.
    $result | Add-Member -NotePropertyName GlobalData -NotePropertyValue ([pscustomobject]@{Enabled=[bool]$BuildGlobalData;Streams=@($(if($globalBuild){$globalBuild.Evidence}))})
    # Reborn: successful receipts distinguish source validation from optional developer regression execution.
    $result | Add-Member -NotePropertyName DeveloperBuildTests -NotePropertyValue ([bool]$DeveloperBuildTests)
    # Reborn: keep unused findings and their source locations in the successful receipt after the private work generation is cleaned.
    $result | Add-Member -NotePropertyName UnusedAssets -NotePropertyValue (Get-Content -LiteralPath (Join-Path $run 'unused-assets.json') -Raw|ConvertFrom-Json)
    [IO.File]::WriteAllText((Join-Path $stage 'build.json'),($result|ConvertTo-Json -Depth 20))
    # Reborn: retain full compiler evidence outside the four-file public package contract before private cleanup.
    Copy-Item -LiteralPath $detail -Destination (Join-Path $sdk '.work/last-detail.log') -Force
    Write-SdkBuildLine ('Internal build receipt: '+(Join-Path $output 'build.json'))
    Write-SdkBuildLine ('Detailed compiler log: '+(Join-Path $sdk '.work/last-detail.log'))
    # Reborn: replace only a complete managed output directory; custom notes/files cause refusal instead of deletion.
    if(Test-Path -LiteralPath $output){$old=Get-Content -LiteralPath (Join-Path $output 'build.json') -Raw|ConvertFrom-Json;if($old.Format-cne 'RebornLiveAlpha1'){throw 'Unmanaged output directory retained.'};foreach($file in Get-ChildItem -LiteralPath $output -Force){if($file.PSIsContainer -or $file.Name-notin @($big,'mod_1.0.skudef','build.json','last-build.log')){throw 'Unmanaged output file retained.'}}}
    # Reborn: publish only qualified streams plus Misc; final SKUDef uses the selected version and Windows Documents known folder.
    $final=Publish-RebornFinalPackage $ModName $ModVersion $package (Join-Path (Split-Path -Parent (Split-Path -Parent $ModXml)) 'Misc') $FinalModsRoot
    # Reborn: Documents is authoritative after commit; a later internal cache failure must not falsely report that no package was published.
    try {
    $result | Add-Member -NotePropertyName FinalPackage -NotePropertyValue $final
    [IO.File]::WriteAllText((Join-Path $stage 'build.json'),($result|ConvertTo-Json -Depth 20))
    Write-SdkBuildLine ('Mod Version: '+$ModVersion)
    Write-SdkBuildLine ('Final Streams BIG: '+$final.StreamsPath)
    # Reborn: skipped Misc must not look like a published archive in the Studio log.
    if($final.MiscEntries -gt 0){Write-SdkBuildLine ('Final Misc BIG: '+$final.MiscPath+' ('+$final.MiscEntries+' files)')}else{Write-SdkBuildLine 'Misc skipped: no packageable files; no Misc BIG or SKUDef entry.'}
    Write-SdkBuildLine ('Final SKUDef: '+$final.SkuDefPath)
    Stop-SdkBuildPhase
    # Reborn: finish the saved log inside staging so a log write failure cannot replace the last good public package.
    $savedLines=@($script:BuildLogLines)+@('Publication completed; game installation files were not modified; no game was launched.','Build Completed',('Build time taken: '+$script:BuildClock.Elapsed.ToString('c')))
    [IO.File]::WriteAllLines((Join-Path $stage 'last-build.log'),[string[]]$savedLines)
    $previous=Join-Path $expectedOutput ('.previous-'+[guid]::NewGuid().ToString('N'));if(Test-Path -LiteralPath $output){[IO.Directory]::Move($output,$previous)}
    try{[IO.Directory]::Move($stage,$output)}catch{if(Test-Path -LiteralPath $previous){[IO.Directory]::Move($previous,$output)};throw}
    if(Test-Path -LiteralPath $previous){Remove-DevAlphaOwned $previous $expectedOutput}
    } catch {Write-SdkBuildLine ('Warning: ALPHA112 Final package published at '+$final.SkuDefPath+'; internal cache/log refresh failed: '+$_.Exception.Message)}
    Write-SdkBuildLine 'Publication completed; game installation files were not modified; no game was launched.'
}catch{
    Write-SdkBuildLine ('Error: ALPHA110 '+$_.Exception.Message)
    Write-SdkBuildLine ('Critical: ALPHA111 Build Failed during '+$script:PhaseLabel+'. No successful new publication.')
    Stop-SdkBuildPhase
    Write-SdkBuildLine ('Build time taken: '+$script:BuildClock.Elapsed.ToString('c'))
    # Reborn: identify whether this failure actually produced backend detail; never point a preflight failure at an older raw log.
    if($runOwned){if($detail -and (Test-Path -LiteralPath $detail)){Copy-Item -LiteralPath $detail -Destination (Join-Path $sdk '.work/last-failure-detail.log') -Force;Write-SdkBuildLine ('Detailed failure log: '+(Join-Path $sdk '.work/last-failure-detail.log'))}else{Write-SdkBuildLine 'No backend compiler log was generated for this failure.'};Write-SdkBuildLine ('Failure log: '+(Join-Path $sdk '.work/last-failure.log'));[IO.File]::WriteAllLines((Join-Path $sdk '.work/last-failure.log'),$script:BuildLogLines)}
    exit 1
}
finally{try{if($stageOwned -and (Test-Path -LiteralPath $stage)){Remove-DevAlphaOwned $stage (Join-Path $sdk 'builtmods')};if($runOwned -and (Test-Path -LiteralPath $run)){Remove-DevAlphaOwned $run (Join-Path $sdk '.work')}}finally{$env:REBORN_DEVELOPER_BUILD_TESTS=$previousDeveloperTests;if($lock){$lock.Dispose()};if($publishHeld){$publishMutex.ReleaseMutex()};if($publishMutex){$publishMutex.Dispose()};if($cancelEvent){$cancelEvent.Dispose()}}}
