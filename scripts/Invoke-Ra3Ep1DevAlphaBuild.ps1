# Reborn: stable local SDK front end; keep editable mod sources, publish only BIG/SKUDef plus receipts, never install or launch.
[CmdletBinding()]
param([string]$ModName='RebornTest',[string]$ModXml,[switch]$ValidateOnly,[string]$StaticManifest,[string]$GlobalManifest,[string]$CompilerDirectory,[string]$OutputRoot,[string]$StudioSession)
$ErrorActionPreference='Stop';$repo=Split-Path -Parent $PSScriptRoot;$sdk=Join-Path $repo 'alpha';$lock=$null;$run=$null;$publishMutex=$null;$cancelEvent=$null;$publishHeld=$false;$runOwned=$false;$stageOwned=$false
#-------------------------------------------------------------------------------------------------
<# Reborn: delete only a newly owned work/output generation after proving containment and refusing every redirected descendant. #>
#-------------------------------------------------------------------------------------------------
function Remove-DevAlphaOwned([string]$Path,[string]$Parent){$resolved=[IO.Path]::GetFullPath($Path);if([IO.Path]::GetDirectoryName($resolved)-ine [IO.Path]::GetFullPath($Parent)){throw 'Cleanup target is not a direct owned child.'};foreach($item in @(Get-Item -LiteralPath $resolved -Force)+@(Get-ChildItem -LiteralPath $resolved -Force -Recurse)){if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected cleanup target retained.'}};Remove-Item -LiteralPath $resolved -Recurse -Force}
try{
    if($PSVersionTable.PSVersion.Major-lt 7){throw 'PowerShell 7 is required.'}
    # Reborn: only a Studio-created session supplies cancellation/publication synchronization; unrelated CLI builds remain independent.
    if($StudioSession){if($StudioSession-cnotmatch '^[a-f0-9]{32}$'){throw 'Invalid Studio session.'};$publishMutex=[Threading.Mutex]::OpenExisting('Local\RebornStudio-'+$StudioSession+'-Publish');$cancelEvent=[Threading.EventWaitHandle]::OpenExisting('Local\RebornStudio-'+$StudioSession+'-Cancel');if($cancelEvent.WaitOne(0)){throw 'Studio build cancelled before starting.'}}
    if(-not $ModXml){$ModXml=Join-Path $sdk ('mods/'+$ModName+'/data/mod.xml')};$ModXml=[IO.Path]::GetFullPath($ModXml)
    $ModName=Split-Path -Leaf (Split-Path -Parent (Split-Path -Parent $ModXml))
    if($ModName-notmatch '^[A-Za-z0-9_-]{1,48}$' -or $ModXml-ine (Join-Path $sdk ('mods/'+$ModName+'/data/mod.xml'))){throw 'Select alpha/mods/<name>/data/mod.xml; paths cannot escape the live SDK.'}
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
    # Reborn: standalone GameObject XML uses the explicit complete-Conscript route; other assets retain their original admission and compiler path.
    $xmlSettings=[Xml.XmlReaderSettings]::new();$xmlSettings.DtdProcessing=[Xml.DtdProcessing]::Prohibit;$xmlSettings.XmlResolver=$null;$xmlSettings.MaxCharactersInDocument=1048576
    $reader=[Xml.XmlReader]::Create($ModXml,$xmlSettings);try{$null=$reader.MoveToContent();$isConscript=$reader.LocalName-ceq 'GameObject';$isDeclaration=$reader.LocalName-ceq 'AssetDeclaration'}finally{$reader.Dispose()}
    # Reborn: detect GameObject declarations in a real local XML closure without an automatic production command.
    $searchArgs=@($sdk);if($settings.SdkDataRoot){$searchArgs+=$settings.SdkDataRoot}
    if($isDeclaration){$kind=@(& (Join-Path $expectedCompiler 'BinaryAssetBuilder.ManifestInspector.exe') conscript-source-kind $ModXml @searchArgs);if($LASTEXITCODE-ne 0){throw ($kind -join [Environment]::NewLine)};$isConscript=($kind -join '').Trim()-ceq 'conscript'}
    $backend=if($isConscript){'Invoke-Ra3Ep1ConscriptAlphaBuild.ps1'}else{'Invoke-Ra3Ep1AlphaBuild.ps1'}
    $arguments=@('-NoProfile','-File',(Join-Path $PSScriptRoot $backend),'-ModXml',$ModXml,'-StaticManifest',$StaticManifest,'-GlobalManifest',$GlobalManifest,'-CompilerDirectory',$expectedCompiler,'-OutputRoot',$run)
    if($ValidateOnly){$arguments+='-ValidateOnly'}
    # Reborn: the armor source backend uses the same explicit SDK search roots as GameObject routing/preparation.
    if(-not $isConscript){$arguments+=@('-SdkRoot',$sdk);if($settings.SdkDataRoot){$arguments+=@('-SdkDataRoot',$settings.SdkDataRoot)}}
    & $settings.PowerShell7 @arguments *> $log;$code=$LASTEXITCODE;Get-Content -LiteralPath $log|Write-Output
    if($code-ne 0){Copy-Item -LiteralPath $log -Destination (Join-Path $sdk '.work/last-failure.log') -Force;throw 'Alpha backend failed; previous successful output is unchanged. See alpha/.work/last-failure.log.'}
    if($ValidateOnly){Copy-Item -LiteralPath $log -Destination (Join-Path $sdk '.work/last-validation.log') -Force;return}
    $receipts=@(Get-ChildItem -LiteralPath $run -Filter alpha-build.json -Recurse);if($receipts.Count-ne 1){throw 'Expected one successful alpha backend receipt.'}
    $receipt=Get-Content -LiteralPath $receipts[0].FullName -Raw|ConvertFrom-Json;$core=Get-Content -LiteralPath $receipt.BuildReceipt -Raw|ConvertFrom-Json
    $package=if($receipt.PackagePath){$receipt.PackagePath}elseif($core.PackagePath){$core.PackagePath}else{$core.DefaultReaderPackagePath}
    $package=[IO.Path]::GetFullPath($package);if(-not $package.StartsWith($run+[IO.Path]::DirectorySeparatorChar,[StringComparison]::OrdinalIgnoreCase)){throw 'Backend package escaped its private work generation.'}
    # Reborn: close cancellation must never terminate the worker halfway through staged publication/rollback of a last-good package.
    if($publishMutex){try{$publishHeld=$publishMutex.WaitOne()}catch [Threading.AbandonedMutexException]{$publishHeld=$true};if($cancelEvent.WaitOne(0)){throw 'Studio build cancelled before publication.'}}
    # Reborn: refuse a late higher-priority override even if the originally selected SDK file's hash is unchanged.
    foreach($selection in $receipt.PathSearches){
        $winner=$null
        foreach($candidate in $selection.Candidates){for($ancestor=$candidate;$ancestor;$ancestor=[IO.Path]::GetDirectoryName($ancestor)){if((Test-Path -LiteralPath $ancestor) -and ((Get-Item -LiteralPath $ancestor -Force).Attributes-band [IO.FileAttributes]::ReparsePoint)){throw 'Redirected alias search candidate.'}};if(Test-Path -LiteralPath $candidate -PathType Container){throw 'Alias candidate became a directory.'};if(Test-Path -LiteralPath $candidate -PathType Leaf){$winner=$candidate;break}}
        if($winner-ine $selection.PhysicalPath){throw ('Alias precedence changed: '+$selection.LogicalPath)}
    }
    $stage=Join-Path $expectedOutput ('.stage-'+[guid]::NewGuid().ToString('N'));$null=New-Item -ItemType Directory -Path $stage;$stageOwned=$true
    $big=$ModName+'.big';Copy-Item -LiteralPath $package -Destination (Join-Path $stage $big);$hash=(Get-FileHash -LiteralPath $package -Algorithm SHA256).Hash
    if((Get-FileHash -LiteralPath (Join-Path $stage $big) -Algorithm SHA256).Hash-cne $hash){throw 'Published BIG differs from the verified backend package.'}
    [IO.File]::WriteAllText((Join-Path $stage 'mod_1.0.skudef'),("add-big "+$big+"`n"),[Text.Encoding]::ASCII);Copy-Item -LiteralPath $log -Destination (Join-Path $stage 'last-build.log')
    $output=Join-Path $expectedOutput $ModName;$sku=Join-Path $output 'mod_1.0.skudef';if($sku.Length-gt 240){throw 'Choose a shorter mod directory for the conservative game path bound.'}
    $result=[pscustomobject]@{Format='RebornLiveAlpha1';ModName=$ModName;Source=$ModXml;SourceSha256=(Get-FileHash -LiteralPath $ModXml -Algorithm SHA256).Hash;BigPath=(Join-Path $output $big);SkuDefPath=$sku;PackageSha256=$hash;CompilerFiles=$toolchain.Files;Backend=$receipt;Core=$core;GameExecuted=$false;InstalledFilesModified=$false}
    [IO.File]::WriteAllText((Join-Path $stage 'build.json'),($result|ConvertTo-Json -Depth 20))
    # Reborn: replace only a complete managed output directory; custom notes/files cause refusal instead of deletion.
    if(Test-Path -LiteralPath $output){$old=Get-Content -LiteralPath (Join-Path $output 'build.json') -Raw|ConvertFrom-Json;if($old.Format-cne 'RebornLiveAlpha1'){throw 'Unmanaged output directory retained.'};foreach($file in Get-ChildItem -LiteralPath $output -Force){if($file.PSIsContainer -or $file.Name-notin @($big,'mod_1.0.skudef','build.json','last-build.log')){throw 'Unmanaged output file retained.'}}}
    $previous=Join-Path $expectedOutput ('.previous-'+[guid]::NewGuid().ToString('N'));if(Test-Path -LiteralPath $output){[IO.Directory]::Move($output,$previous)}
    try{[IO.Directory]::Move($stage,$output)}catch{if(Test-Path -LiteralPath $previous){[IO.Directory]::Move($previous,$output)};throw}
    if(Test-Path -LiteralPath $previous){Remove-DevAlphaOwned $previous $expectedOutput}
    Write-Output ('Reborn: live SDK output: '+$result.BigPath);Write-Output ('Reborn: SKUDef: '+$sku)
}catch{Write-Output ('Error: ALPHA110 '+$_.Exception.Message);Write-Output 'Critical: ALPHA111 Stable SDK build failed; no successful new publication.';exit 1}
finally{try{if($stageOwned -and (Test-Path -LiteralPath $stage)){Remove-DevAlphaOwned $stage (Join-Path $sdk 'builtmods')};if($runOwned -and (Test-Path -LiteralPath $run)){Remove-DevAlphaOwned $run (Join-Path $sdk '.work')}}finally{if($lock){$lock.Dispose()};if($publishHeld){$publishMutex.ReleaseMutex()};if($publishMutex){$publishMutex.Dispose()};if($cancelEvent){$cancelEvent.Dispose()}}}
