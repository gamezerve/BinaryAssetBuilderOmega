# Reborn: stable SDK front end; preserve editable sources, publish versioned Documents mods, never change game installation files or launch a game.
[CmdletBinding()]
param([string]$ModName='RebornTest',[string]$ModXml,[switch]$ValidateOnly,[string]$StaticManifest,[string]$GlobalManifest,[string]$CompilerDirectory,[string]$OutputRoot,[string]$StudioSession,[string]$ModVersion='1.00',[string]$FinalModsRoot,[ValidateSet(0,1)][int]$BuildGlobalData=1,[ValidateSet(0,1)][int]$DeveloperBuildTests=0,[ValidateSet(0,1)][int]$BuildAptUI=0,[ValidateSet(0,1)][int]$BuildMaps=0,[ValidateSet(0,1)][int]$SilentMode=0,[ValidateSet(0,1)][int]$BuildStaticData=1,[ValidateSet(0,1)][int]$CreateBigFiles=1,
    # Reborn: All omits the directory override.
    [string]$OfficialMapDirectory,
    # Reborn: Studio sends checked folders in an owned UTF-8 file, avoiding command-line length and array-binding ambiguity.
    [string]$OfficialMapSelectionFile,[ValidateSet(0,1)][int]$ForceRebuildMaps=0)

function Test-AptUIReferences {
    param(
        [string]$XmlPath,
        [string]$SourceRoot,
        [System.Collections.Generic.HashSet[string]]$Visited,
        [ref]$ReferenceCount
    )

    $XmlPath=[IO.Path]::GetFullPath($XmlPath)
    if(-not $Visited.Add($XmlPath)){return}

    $settings=[Xml.XmlReaderSettings]::new()
    $settings.DtdProcessing=[Xml.DtdProcessing]::Prohibit
    $settings.XmlResolver=$null

    $document=[Xml.XmlDocument]::new()
    $document.XmlResolver=$null
    $reader=[Xml.XmlReader]::Create($XmlPath,$settings)
    try{
        $document.Load($reader)
    }finally{
        $reader.Dispose()
    }

    $root=[IO.Path]::GetFullPath($SourceRoot).TrimEnd('\','/')
    foreach($node in $document.SelectNodes('//*')){
        foreach($attributeName in @('source','File')){
            $attribute=$node.Attributes[$attributeName]
            if($null-eq $attribute){continue}

            $relativePath=$attribute.Value.Replace('/',[IO.Path]::DirectorySeparatorChar).Replace('\',[IO.Path]::DirectorySeparatorChar)
            $target=[IO.Path]::GetFullPath((Join-Path (Split-Path -Parent $XmlPath) $relativePath))

            if(-not $target.StartsWith($root+[IO.Path]::DirectorySeparatorChar,[StringComparison]::OrdinalIgnoreCase)){
                throw "AptUI reference escapes source directory: $relativePath"
            }

            if(-not [IO.File]::Exists($target)){
                throw "Missing AptUI reference: $relativePath in $XmlPath"
            }

            $ReferenceCount.Value++

            if($node.LocalName -eq 'Include'){
                Test-AptUIReferences -XmlPath $target -SourceRoot $root -Visited $Visited -ReferenceCount $ReferenceCount
            }
        }
    }
}

$ErrorActionPreference='Stop';$repo=Split-Path -Parent $PSScriptRoot
# Reborn: the same driver works in the repository and inside a relocated alpha/scripts directory.
. (Join-Path $PSScriptRoot 'Ra3Ep1PortablePaths.ps1')
$sdk=Get-RebornSdkRoot $PSScriptRoot;$lock=$null;$run=$null;$publishMutex=$null;$cancelEvent=$null;$publishHeld=$false;$runOwned=$false;$stageOwned=$false
# Reborn: retain the original Studio envelope while adding truthful numbered backend phases and separate detailed output.
. (Join-Path $PSScriptRoot 'Ra3Ep1BuildLog.ps1')
# Reborn: the final user package targets Documents; OutputRoot remains the private compatibility receipt/cache location.
. (Join-Path $PSScriptRoot 'Ra3Ep1FinalPackage.ps1')
# Reborn: compile early AdditionalMaps streams independently of static data, then include both in the same final Streams BIG.
. (Join-Path $PSScriptRoot 'Ra3Ep1GlobalData.ps1')
# Reborn: map compilation and _mod staging share the normal SDK lifetime, cancellation and final-package flow.
. (Join-Path $PSScriptRoot 'Ra3Ep1Maps.ps1')
. (Join-Path $PSScriptRoot 'Ra3Ep1MapCache.ps1')
# Reborn: saved Static, Global and AptUI families survive compile-only builds and feed independent packaging.
. (Join-Path $PSScriptRoot 'Ra3Ep1BuildStages.ps1')
# Reborn: every native/global/backend child inherits this build's mode; restore the caller's environment even on cancellation or failure.
# Reborn: propagate the build-local advisory policy to compiler subprocesses.
$previousSilentMode=$env:REBORN_SILENT_MODE
$previousDeveloperTests=$env:REBORN_DEVELOPER_BUILD_TESTS
#-------------------------------------------------------------------------------------------------
<# Reborn: delete only a newly owned work/output generation after proving containment and refusing every redirected descendant. #>
#-------------------------------------------------------------------------------------------------
function Remove-DevAlphaOwned([string]$Path,[string]$Parent){$resolved=[IO.Path]::GetFullPath($Path);if([IO.Path]::GetDirectoryName($resolved)-ine [IO.Path]::GetFullPath($Parent)){throw 'Cleanup target is not a direct owned child.'};foreach($item in @(Get-Item -LiteralPath $resolved -Force)+@(Get-ChildItem -LiteralPath $resolved -Force -Recurse)){if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected cleanup target retained.'}};Remove-Item -LiteralPath $resolved -Recurse -Force}
#-------------------------------------------------------------------------------------------------
<# Reborn: use only a verified previous local package as a reference provider during a map-only check. #>
#-------------------------------------------------------------------------------------------------
function Get-RebornPreviousMapProvider([string]$OutputRoot,[string]$Name) {
    $directory=Join-Path $OutputRoot $Name
    $package=Join-Path $directory ($Name+'.big')
    $receipt=Join-Path $directory 'build.json'
    Assert-RebornPackagePath $package
    Assert-RebornPackagePath $receipt
    if(!(Test-Path -LiteralPath $package -PathType Leaf) -or !(Test-Path -LiteralPath $receipt -PathType Leaf)){return}
    $saved=Get-Content -LiteralPath $receipt -Raw | ConvertFrom-Json
    if($saved.Format -cne 'RebornLiveAlpha1' -or $saved.ModName -cne $Name -or (Get-FileHash -LiteralPath $package).Hash -cne $saved.PackageSha256){
        throw 'Previous mod static package failed receipt verification. Enable Build Static Data to compile fresh dependencies.'
    }
    return $package
}

try{
    # Reborn: All supplies no list; an explicit empty, missing or invalid selection never expands into a full map build.
    $officialMapDirectories=$null
    if($PSBoundParameters.ContainsKey('OfficialMapSelectionFile')){
        if($OfficialMapDirectory){throw 'Choose a single map directory or a map selection file, not both.'}
        $officialMapDirectories=@(Read-RebornMapSelection $OfficialMapSelectionFile)
    }
    # Reborn: compile-only runs never publish generated maps, BIGs or SKUDefs.
    $publishPackage=[bool]($CreateBigFiles -and !$ValidateOnly)
    if(!($BuildAptUI -or $BuildGlobalData -or $BuildStaticData -or $BuildMaps -or $publishPackage)){throw 'Select a build stage or Create BIG Files.'}
    $env:REBORN_SILENT_MODE=[string]$SilentMode
    if($SilentMode){Write-SdkBuildLine "Silent Mode enabled; advisory audits skipped."}
    $env:REBORN_DEVELOPER_BUILD_TESTS=[string]$DeveloperBuildTests
    Write-SdkBuildLine $(if($DeveloperBuildTests){'Build mode: Developer (build tests enabled).'}else{'Build mode: Fast (developer tests disabled; source/reference/output validation remains enabled).'})
    Start-SdkBuildPhase 1 'Clearing build cache';Stop-SdkBuildPhase 'Skipped: this alpha does not use a persistent asset cache.'
    Start-SdkBuildPhase 2 'Clearing session cache';Stop-SdkBuildPhase 'Skipped: this alpha does not use a persistent compiler session cache.'
    Start-SdkBuildPhase 3 'Clearing built mod'
    Write-SdkBuildLine 'Preparing private build workspace; previous successful output will be retained until publication.'
    if($PSVersionTable.PSVersion.Major-lt 7){throw 'PowerShell 7 is required.'}
    # Reborn: validate before compilation, retain formatting such as 1.00, and reject filename/argument injection.
    Assert-RebornModVersion $ModVersion
    # Reborn: only a Studio-created session supplies cancellation/publication synchronization; unrelated CLI builds remain independent.
    if($StudioSession){if($StudioSession-cnotmatch '^[a-f0-9]{32}$'){throw 'Invalid Studio session.'};$publishMutex=[Threading.Mutex]::OpenExisting('Local\RebornStudio-'+$StudioSession+'-Publish');$cancelEvent=[Threading.EventWaitHandle]::OpenExisting('Local\RebornStudio-'+$StudioSession+'-Cancel');if($cancelEvent.WaitOne(0)){throw 'Studio build cancelled before starting.'}}
    # Reborn: use a separate static mod entry without shadowing the original DATA:static.xml.
    if(-not $ModXml){$ModXml=Join-Path $sdk ('mods/'+$ModName+'/data/Static_Mod.xml')};$ModXml=[IO.Path]::GetFullPath($ModXml)
    $ModName=Split-Path -Leaf (Split-Path -Parent (Split-Path -Parent $ModXml))
    if($ModName-notmatch '^[A-Za-z0-9_-]{1,48}$' -or $ModXml-ine (Join-Path $sdk ('mods/'+$ModName+'/data/Static_Mod.xml'))){throw 'Select alpha/mods/<name>/data/Static_Mod.xml; paths cannot escape the live SDK.'}
    # Reborn: Studio already displays this header; include it once in the saved standalone transcript only.
    $script:BuildLogLines.Insert(0,('Building Mod '+$ModName))
    $settings=Read-RebornSdkPaths $sdk
    # Reborn: disposable directories are optional copy inputs and must exist before startup checks or lock/workspace creation.
    Initialize-RebornSdkWorkDirectories $sdk
    if(-not $StaticManifest){$StaticManifest=$settings.StaticManifest};if(-not $GlobalManifest){$GlobalManifest=$settings.GlobalManifest}
    $expectedCompiler=Join-Path $sdk 'tools/compiler';$expectedOutput=Join-Path $sdk 'builtmods'
    if($CompilerDirectory -and [IO.Path]::GetFullPath($CompilerDirectory)-ine $expectedCompiler){throw 'Stable alpha cannot use an unrelated compiler.'}
    if($OutputRoot -and [IO.Path]::GetFullPath($OutputRoot)-ine $expectedOutput){throw 'Stable alpha outputs stay inside alpha/builtmods.'}
    # Reborn: validate the selected Data directory for stage-only builds, even if no static source exists.
    $buildInput=if($BuildStaticData){$ModXml}else{Split-Path -Parent $ModXml}
    foreach($path in @($buildInput,$expectedCompiler,$expectedOutput,(Join-Path $sdk '.work'))){for($item=Get-Item -LiteralPath $path;$null-ne $item;$item=if($item.PSIsContainer){$item.Parent}else{$item.Directory}){if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected stable SDK path refused.'}}}
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
    $compiler=Join-Path $expectedCompiler 'BinaryAssetBuilder.ManifestInspector.exe'
    $selected=@($(if($BuildStaticData){'Static'}),$(if($BuildGlobalData){'Global'}),$(if($BuildAptUI){'AptUI'}),$(if($BuildMaps){'MapCatalog'})) | Where-Object { $_ }
    $stages=Get-RebornBuildStages $expectedOutput $ModName $run $compiler $selected
    $dependencyReport=$null;$receipt=$null;$core=$null

	$aptuiFiles = @()
	$aptuiOutput = $null

	Start-SdkBuildPhase 4 'Building AptUI'
	if($BuildAptUI){
		$aptuiSource=Join-Path (Split-Path -Parent $ModXml) 'AptUI'

		if(Test-Path -LiteralPath $aptuiSource -PathType Container){
			$aptuiFiles=@(Get-ChildItem -LiteralPath $aptuiSource -Filter '*.xml' -File | Sort-Object Name)

			if($aptuiFiles.Count -eq 0){
				Stop-SdkBuildPhase 'Skipped: no AptUI XML files found.'
			}else{
				foreach($aptuiFile in $aptuiFiles){
					Write-SdkBuildLine ('AptUI XML: '+$aptuiFile.Name)
					
					$xmlSettings=[System.Xml.XmlReaderSettings]::new()
					$xmlSettings.DtdProcessing=[System.Xml.DtdProcessing]::Prohibit
					$xmlSettings.XmlResolver=$null

					$reader=[System.Xml.XmlReader]::Create($aptuiFile.FullName,$xmlSettings)
					try{
						while($reader.Read()){}
					}finally{
						$reader.Dispose()
					}

					Write-SdkBuildLine ('AptUI XML validation passed: '+$aptuiFile.Name)
										
					$visited=[System.Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
					$referenceCount=0

					Test-AptUIReferences -XmlPath $aptuiFile.FullName -SourceRoot $aptuiSource -Visited $visited -ReferenceCount ([ref]$referenceCount)

					Write-SdkBuildLine ('AptUI references validated: '+$referenceCount)
					Write-SdkBuildLine ('AptUI XML files checked: '+$visited.Count)

					$elementDirectory=Join-Path $aptuiSource $aptuiFile.BaseName

					if(Test-Path -LiteralPath $elementDirectory -PathType Container){
						$elements=@(Get-ChildItem -LiteralPath $elementDirectory -File -Recurse)
						Write-SdkBuildLine ('AptUI element directory: '+$elementDirectory)
						Write-SdkBuildLine ('AptUI element files: '+$elements.Count)
					}else{
						Write-SdkBuildLine ('AptUI element directory not found: '+$elementDirectory)
					}
				}

								
				$aptuiOutput = Join-Path $run 'aptui-streams'
				$aptuiSchema = Join-Path $sdk 'Schemas\CnC3Types.xsd'

				if (-not (Test-Path -LiteralPath $aptuiSchema -PathType Leaf)) {
					throw "AptUI schema not found: $aptuiSchema"
				}

				New-Item -ItemType Directory -Force -Path $aptuiOutput | Out-Null

				foreach ($aptuiFile in $aptuiFiles) {
					& (Join-Path $expectedCompiler 'BinaryAssetBuilder.ManifestInspector.exe') sdk-compile-aptui $aptuiFile.FullName $aptuiSchema $aptuiOutput 2>&1 |
						ForEach-Object { Write-SdkCompilerLine ([string]$_) $detail }

					if ($LASTEXITCODE -ne 0) {
						throw ('AptUI compilation failed: ' + $aptuiFile.Name)
					}
				}

				Stop-SdkBuildPhase ('AptUI compiled: ' + $aptuiFiles.Count + ' XML file(s).')

			}
		}else{
			Stop-SdkBuildPhase 'Skipped: no AptUI source directory found.'
		}
	}else{
		Stop-SdkBuildPhase 'Skipped: [Optional] Build AptUI is unchecked.'
	}

    if($BuildAptUI -and !$ValidateOnly){
        # Reborn: a deliberately rebuilt empty family removes stale UI pages; an unchecked family is reused.
        $aptStage=New-RebornBuildStage $run 'AptUI' ([pscustomobject]@{Legacy=$false;Pages=@($aptuiFiles | ForEach-Object Name)})
        if($aptuiOutput){& $compiler sdk-export-aptui-stage $aptuiOutput $aptStage.Payload;if($LASTEXITCODE){throw 'Could not stage AptUI output.'}}
        Complete-RebornBuildStage $aptStage $ModName;$stages['AptUI']=$aptStage
    }
    Start-SdkBuildPhase 5 'Building global data'
    $newGlobalBuild=$null
    $referenceGlobals=@(Get-RebornStageGlobalStreams $stages)
    if($BuildGlobalData -or $BuildMaps){
        Build-RebornGlobalData (Split-Path -Parent $ModXml) $sdk $settings $StaticManifest $GlobalManifest $expectedCompiler $run $detail ([ref]$newGlobalBuild) -IncludeGlobalData ([bool]$BuildGlobalData) -IncludeMapCatalog ([bool]$BuildMaps) -ReferenceStreams $referenceGlobals
        if($BuildGlobalData){$stages['Global']=New-RebornGlobalBuildStage $newGlobalBuild 'Global' $run $ModName $compiler}
        if($BuildMaps){$stages['MapCatalog']=New-RebornGlobalBuildStage $newGlobalBuild 'MapCatalog' $run $ModName $compiler}
        Stop-SdkBuildPhase ('Global streams compiled: '+$newGlobalBuild.Streams.Count)
    }else{Stop-SdkBuildPhase 'Reusing the last successful global output.'}
    $streams=@(Get-RebornStageGlobalStreams $stages)
    $context=Join-Path $run 'selected-global-streams.json';[IO.File]::WriteAllText($context,(ConvertTo-Json -InputObject $streams -Depth 8))
    $globalBuild=[pscustomobject]@{Context=$context;MapContext=$context;Streams=$streams;Evidence=@($stages.Values | Where-Object Family -in @('Global','MapCatalog') | ForEach-Object {$_.Metadata.Evidence})}
    Start-SdkBuildPhase 6 'Building static data'
    # Reborn: a map check can use the last compiled catalog even if its static build was never published.
    $catalogPath=Join-Path $sdk ('.work/map-static-catalogs/'+$ModName+'.manifest')
    $package=$null
    if($BuildStaticData){
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
        $reader=[Xml.XmlReader]::Create($ModXml,$xmlSettings);try{$null=$reader.MoveToContent();$isConscript=$reader.LocalName-cin @('GameObject','PurchasePlayerTechButtonTemplate','UnitAbilityButtonTemplate','PlayerTech','ButtonSingleStateData','JoypadCommandBarButtonTemplate','PlayerPowerButtonTemplate','SpecialPowerTemplate','ExperienceLevelTemplate','FXParticleSystemTemplate','FXList','LogicCommandSet','UpgradeTemplate','W3DContainer','W3DHierarchy','W3DCollisionBox','W3DMesh');$isDeclaration=$reader.LocalName-ceq 'AssetDeclaration'}finally{$reader.Dispose()}
        # Reborn: detect GameObject declarations in a real local XML closure without an automatic production command.
        $searchArgs=@($sdk);if($settings.SdkDataRoot){$searchArgs+=$settings.SdkDataRoot}
        if($isDeclaration){$kind=@(& (Join-Path $expectedCompiler 'BinaryAssetBuilder.ManifestInspector.exe') conscript-source-kind $ModXml @searchArgs);if($LASTEXITCODE-ne 0){throw ($kind -join [Environment]::NewLine)};$isConscript=($kind -join '').Trim()-ceq 'conscript'}
        $backend=if($isConscript){'Invoke-Ra3Ep1ConscriptAlphaBuild.ps1'}else{'Invoke-Ra3Ep1AlphaBuild.ps1'}
        $arguments=@('-NoProfile','-File',(Join-Path $PSScriptRoot $backend),'-ModXml',$ModXml,'-StaticManifest',$StaticManifest,'-GlobalManifest',$GlobalManifest,'-CompilerDirectory',$expectedCompiler,'-OutputRoot',$run)
        # Reborn: map validation needs a private static package from this invocation too.
        if($ValidateOnly -and !$BuildMaps){$arguments+='-ValidateOnly'}
        # Reborn: static strong/weak reference lookup includes only globals actually built in this invocation.
        if($isConscript -and $globalBuild){$arguments+=@('-GlobalStreamContext',$globalBuild.Context)}
        # Reborn: the armor source backend uses the same explicit SDK search roots as GameObject routing/preparation.
        if(-not $isConscript){$arguments+=@('-SdkRoot',$sdk);if($settings.SdkDataRoot){$arguments+=@('-SdkDataRoot',$settings.SdkDataRoot)}}
        # Reborn: stream complete lines immediately; JSON/readback diagnostics remain in the full detail log.
        & $settings.PowerShell7 @arguments 2>&1 | ForEach-Object {Write-SdkCompilerLine ([string]$_) $detail};$code=$LASTEXITCODE
        if($code-ne 0){throw 'Alpha backend failed; previous successful output is unchanged.'}
        Stop-SdkBuildPhase
        # Reborn: both map validation and packaging use the verified static output in this private generation.
        if(!$ValidateOnly -or $BuildMaps){
            $receipts=@(Get-ChildItem -LiteralPath $run -Filter alpha-build.json -Recurse)
            if($receipts.Count-ne 1){throw 'Expected one successful alpha backend receipt.'}
            $receipt=Get-Content -LiteralPath $receipts[0].FullName -Raw|ConvertFrom-Json
            $core=Get-Content -LiteralPath $receipt.BuildReceipt -Raw|ConvertFrom-Json
            $package=if($receipt.PackagePath){$receipt.PackagePath}elseif($core.PackagePath){$core.PackagePath}else{$core.DefaultReaderPackagePath}
            $package=[IO.Path]::GetFullPath($package)
            if(-not $package.StartsWith($run+[IO.Path]::DirectorySeparatorChar,[StringComparison]::OrdinalIgnoreCase)){throw 'Backend package escaped its private work generation.'}
            if(!$ValidateOnly){
                $staticStage=New-RebornBuildStage $run 'Static' ([pscustomobject]@{Legacy=$false;Backend=$receipt;Core=$core;Source=$ModXml;SourceSha256=(Get-FileHash -LiteralPath $ModXml).Hash})
                & $compiler sdk-export-build-stage $package $staticStage.Payload Static
                if($LASTEXITCODE){throw 'Could not stage the qualified static output.'}
                Save-RebornStageDiagnostics $staticStage @((Join-Path $run 'conscript')) $dependencyReport
                & $compiler sdk-save-stage-diagnostics $staticStage.Payload $run (Join-Path $staticStage.Directory 'metadata/cached-origins.json')
                if($LASTEXITCODE){throw 'Could not save static diagnostic evidence.'}
                Complete-RebornBuildStage $staticStage $ModName;$stages['Static']=$staticStage
            }
        }
    }else{
        if($stages.ContainsKey('Static')){
            $receipt=$stages['Static'].Metadata.Backend;$core=$stages['Static'].Metadata.Core
            $savedDependencies=Join-Path $stages['Static'].Directory 'metadata/dependencies.json'
            if(Test-Path -LiteralPath $savedDependencies){$dependencyReport=$savedDependencies}
        }
        Stop-SdkBuildPhase 'Reusing the last successful static output.'
    }
    if($publishPackage){
        Start-SdkBuildPhase 7 'Merging assets';Stop-SdkBuildPhase 'Skipped: no separate post-build asset merge is enabled.'
        Start-SdkBuildPhase 8 'Fixing assets'
        if($null-ne $core.AssetCount){Write-SdkBuildLine ('Compiled assets: '+$core.AssetCount)}
        if($core.Asset){Write-SdkBuildLine ('Primary asset: '+$core.Asset)}
        Write-SdkBuildLine 'Verified backend receipt and package location; native stream qualification ran during static-data compilation. No legacy RA3 hash-fix tool was run.'
        Stop-SdkBuildPhase
    }
    Start-SdkBuildPhase 9 'Building maps'
    $mapBuild=@();$cacheRoot=$null;$cacheKey=$null
    if($BuildMaps){
        $mapStaticPackage=$package
        if(!$BuildStaticData){
            Assert-RebornPackagePath $catalogPath
            $savedStatic=if($stages.ContainsKey('Static')){Join-Path $stages['Static'].Payload 'data/mod.manifest'}else{$null}
            if($savedStatic -and (Test-Path -LiteralPath $savedStatic)){
                $mapStaticPackage=$savedStatic
                Write-SdkBuildLine 'Map build uses the selected saved static output.'
            }elseif(Test-Path -LiteralPath $catalogPath -PathType Leaf){
                $mapStaticPackage=$catalogPath
                Write-SdkBuildLine 'Map check uses the static catalog from the last successful local static compile.'
            }else{
                $mapStaticPackage=Get-RebornPreviousMapProvider $expectedOutput $ModName
                if($mapStaticPackage){Write-SdkBuildLine 'Map check uses the verified static catalog from the last successful local package.'}
                else{Write-SdkBuildLine 'Map check uses stock and selected global streams. No previous local mod static catalog was found.'}
            }
        }
        # Reborn: validate fresh and saved maps against the same build-local witnesses before exposing them to packaging.
        $roots=@($sdk,$settings.SdkDataRoot,$settings.UprisingXmlRoot)
        $witness=New-RebornMapWitness $roots
        if(!$ValidateOnly){
            $cacheRoot=Join-Path $expectedOutput ($ModName+'/Maps')
            $cacheKey=Get-RebornMapCacheKey $sdk $settings $expectedCompiler $StaticManifest $GlobalManifest $globalBuild.MapContext $mapStaticPackage $run
        }
        Build-RebornMaps (Split-Path -Parent $ModXml) $sdk $settings $StaticManifest $GlobalManifest $expectedCompiler $run $detail $globalBuild.MapContext ([ref]$mapBuild) $mapStaticPackage -OfficialMapDirectory $OfficialMapDirectory -OfficialMapDirectories $officialMapDirectories -CacheRoot $cacheRoot -CacheKey $cacheKey -Witness $witness -ForceRebuild:($ForceRebuildMaps -or $ValidateOnly)
        if($mapBuild.Count){
            if($cacheRoot -and (Get-RebornMapCacheKey $sdk $settings $expectedCompiler $StaticManifest $GlobalManifest $globalBuild.MapContext $mapStaticPackage $run)-cne $cacheKey){throw 'Shared map providers changed during the build.'}
            $finalWitness=New-RebornMapWitness $roots
            foreach($plan in $mapBuild){Assert-RebornMapWitness $plan $finalWitness}
            if($publishPackage){Publish-RebornMapOutputs $mapBuild (Join-Path (Split-Path -Parent (Split-Path -Parent $ModXml)) 'Misc') -Witnessed}
            $rebuilt=@($mapBuild | Where-Object Fresh).Count
            Stop-SdkBuildPhase ('Maps compiled: '+$rebuilt+'; reused: '+($mapBuild.Count-$rebuilt))
        }else{Stop-SdkBuildPhase}
    }else{Stop-SdkBuildPhase 'Skipped: [Optional] Build Maps is unchecked.'}
    Start-SdkBuildPhase 10 'Creating big files'
    if(!$publishPackage){
        Stop-SdkBuildPhase 'Skipped: [Optional] Final BIG and SKUDef publication is disabled.'
        if(!$ValidateOnly){
            # Reborn: Studio cancellation cannot interrupt the single index update committing all successful stage outputs.
            if($publishMutex){try{$publishHeld=$publishMutex.WaitOne()}catch [Threading.AbandonedMutexException]{$publishHeld=$true};if($cancelEvent.WaitOne(0)){throw 'Studio build cancelled before saving outputs.'}}
            if($newGlobalBuild){Assert-RebornGlobalSources $newGlobalBuild}
            Save-RebornBuildStages $stages $expectedOutput $ModName
            if($cacheRoot){Save-RebornMapCache $cacheRoot $cacheKey $mapBuild}
        }
        Write-SdkBuildLine $(if($ValidateOnly){'Validation Completed (no saved outputs or publication).'}else{'Compilation Completed (outputs saved; no BIG publication).'})
        Write-SdkBuildLine ('Build time taken: '+$script:BuildClock.Elapsed.ToString('c'))
        [IO.File]::WriteAllLines((Join-Path $sdk '.work/last-validation.log'),$script:BuildLogLines)
        if(Test-Path -LiteralPath $detail){Copy-Item -LiteralPath $detail -Destination (Join-Path $sdk '.work/last-detail.log') -Force}
        return
    }
    # Reborn: close cancellation must never terminate the worker halfway through staged publication/rollback of a last-good package.
    if($publishMutex){try{$publishHeld=$publishMutex.WaitOne()}catch [Threading.AbandonedMutexException]{$publishHeld=$true};if($cancelEvent.WaitOne(0)){throw 'Studio build cancelled before publication.'}}
    if(!$stages.ContainsKey('Static') -or !(Test-Path -LiteralPath (Join-Path $stages['Static'].Payload 'data/mod.manifest'))){
        throw 'No successful Static output is available. Enable Build Static Data once before packaging.'
    }
    # Reborn: source replay applies only to fresh compiles; unselected families intentionally represent their last successful sources.
    if($BuildStaticData){Assert-RebornSourceSearch $receipt.PathSearches}
    if($newGlobalBuild){Assert-RebornGlobalSources $newGlobalBuild}
    $package=Join-Path $run 'selected-streams.big'
    New-RebornStagePackage $stages $package $compiler $sdk $run
    Restore-RebornStageDiagnostics $stages $run
    if(-not $SilentMode){
        # Reborn: only unresolved WorldBuilder use sites become warnings, after extraction and global/static package assembly have finished.
        if($dependencyReport){
            & $compiler sdk-check-worldbuilder-resolution $package $dependencyReport (Join-Path $sdk 'catalogs/ra3ep1') 2>&1 | ForEach-Object {Write-SdkCompilerLine ([string]$_) $detail}
            if($LASTEXITCODE-ne 0){throw 'Post-resolution WorldBuilder dependency check failed.'}
        }
        # Reborn: audit every final asset type after global/static assembly, using weak-use receipts as well as native reference tables.
        & (Join-Path $expectedCompiler 'BinaryAssetBuilder.ManifestInspector.exe') sdk-unused-assets $package $sdk $run $ModXml 2>&1 | ForEach-Object {Write-SdkCompilerLine ([string]$_) $detail}
        if($LASTEXITCODE-ne 0){throw 'Final asset usage audit failed.'}
    }
    $hash=(Get-FileHash -LiteralPath $package).Hash
    Stop-SdkBuildPhase
    Start-SdkBuildPhase 11 'Creating skudef file'
    $final=Publish-RebornFinalPackage $ModName $ModVersion $package (Join-Path (Split-Path -Parent (Split-Path -Parent $ModXml)) 'Misc') $FinalModsRoot
    try{
        Save-RebornBuildStages $stages $expectedOutput $ModName
        if($cacheRoot){Save-RebornMapCache $cacheRoot $cacheKey $mapBuild}
        $output=Join-Path $expectedOutput $ModName
        $result=[pscustomobject]@{Format='RebornPersistentBuild1';ModName=$ModName;Source=$ModXml;Backend=$receipt;Core=$core;BigPath=$final.StreamsPath;SkuDefPath=$final.SkuDefPath;CompilerFiles=$toolchain.Files;PackageSha256=$hash;FinalPackage=$final;SelectedStages=$selected;Stages=@($stages.Values | ForEach-Object {[pscustomobject]@{Family=$_.Family;Directory=$_.Directory;CreatedUtc=$_.Receipt.CreatedUtc}});Maps=@($mapBuild | ForEach-Object {[pscustomobject]@{Name=$_.Name;Entry=$_.Entry;Files=$_.Files}});UnusedAssets=$(if(!$SilentMode){Get-Content -LiteralPath (Join-Path $run 'unused-assets.json') -Raw|ConvertFrom-Json}else{$null})}
        $receiptStage=Join-Path $output '.build.json.next';[IO.File]::WriteAllText($receiptStage,($result|ConvertTo-Json -Depth 20));[IO.File]::Move($receiptStage,(Join-Path $output 'build.json'),$true)
        Write-SdkBuildLine ('Internal build receipt: '+(Join-Path $output 'build.json'))
        Write-SdkBuildLine ('Final Streams BIG: '+$final.StreamsPath)
        if($final.MiscEntries -gt 0){Write-SdkBuildLine ('Final Misc BIG: '+($final.MiscPaths -join ', '))}
        Write-SdkBuildLine ('Final SKUDef: '+$final.SkuDefPath)
        Stop-SdkBuildPhase
        [IO.File]::WriteAllLines((Join-Path $output 'last-build.log'),[string[]](@($script:BuildLogLines)+@('Build Completed')))
    }catch{Write-SdkBuildLine ('Warning: ALPHA112 Final package published at '+$final.SkuDefPath+'; saved output/log refresh failed: '+$_.Exception.Message)}
    if(Test-Path -LiteralPath $detail){Copy-Item -LiteralPath $detail -Destination (Join-Path $sdk '.work/last-detail.log') -Force}
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
finally{try{if($stageOwned -and (Test-Path -LiteralPath $stage)){Remove-DevAlphaOwned $stage (Join-Path $sdk 'builtmods')};if($runOwned -and (Test-Path -LiteralPath $run)){Remove-DevAlphaOwned $run (Join-Path $sdk '.work')}}finally{$env:REBORN_SILENT_MODE=$previousSilentMode;$env:REBORN_DEVELOPER_BUILD_TESTS=$previousDeveloperTests;if($lock){$lock.Dispose()};if($publishHeld){$publishMutex.ReleaseMutex()};if($publishMutex){$publishMutex.Dispose()};if($cancelEvent){$cancelEvent.Dispose()}}}
