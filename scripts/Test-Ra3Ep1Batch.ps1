# Reborn: batch independent offline compiler/source/authoring checks; failures are collected, never converted to playable feature claims.
[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$OutputDirectory,
    [string]$StaticManifest,
    [string]$GlobalManifest,
    [ValidateRange(1,4)][int]$Workers=2,
    [switch]$Full,
    [switch]$SchedulerControl
)
$ErrorActionPreference='Stop'
if($PSVersionTable.PSVersion.Major-lt 7){throw 'PowerShell 7 is required.'}
$repo=Split-Path -Parent $PSScriptRoot
$OutputDirectory=[IO.Path]::GetFullPath($OutputDirectory)
if(Test-Path -LiteralPath $OutputDirectory){throw 'Batch output must be fresh; no existing artifacts are overwritten.'}
$parent=Split-Path -Parent $OutputDirectory
if(-not (Test-Path -LiteralPath $parent -PathType Container)){throw 'Batch output requires an existing parent.'}
for($item=Get-Item -LiteralPath $parent;$null-ne $item;$item=$item.Parent){
    if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected batch output refused.'}
}
if(-not $SchedulerControl){
    foreach($path in @($StaticManifest,$GlobalManifest)){
        if(-not $path -or -not [IO.Path]::IsPathFullyQualified($path) -or -not (Test-Path -LiteralPath $path -PathType Leaf)){throw 'Explicit existing static/global manifests are required.'}
    }
}
$null=New-Item -ItemType Directory -Path $OutputDirectory
$cases=[Collections.Generic.List[object]]::new()
#-------------------------------------------------------------------------------------------------
# Reborn: fixed command recipes are controlled by this script, never loaded from untrusted config or interpreted as shell expressions.
#-------------------------------------------------------------------------------------------------
function Add-BatchCase([string]$Name,[string]$Evidence,[string[]]$Arguments){
    $cases.Add([pscustomobject]@{Name=$Name;Evidence=$Evidence;Arguments=$Arguments})
}
$exe=Join-Path $repo 'source/BinaryAssetBuilder.ManifestInspector/bin/x86/Release/net8.0/BinaryAssetBuilder.ManifestInspector.exe'
$compilerHash=$null
$compilerFiles=@()
if($SchedulerControl){
    # Reborn: disposable process controls prove nonzero-exit continuation; no compiler, manifests, game or install files are involved.
    $exe=(Get-Process -Id $PID).Path
    foreach($case in @(@('control-pass',0),@('control-fail',7),@('control-after-failure',0))){
        Add-BatchCase $case[0] 'SchedulerControlOnly' @('-NoProfile','-Command',('# Reborn: owned scheduler fixture.'+[Environment]::NewLine+'Write-Output "Reborn: scheduler fixture"; exit '+$case[1]))
    }
}else{
    # Reborn: build exactly once, then bind every worker/report to the same compiler bytes.
    & dotnet build (Join-Path $repo 'source/BinaryAssetBuilder.ManifestInspector/BinaryAssetBuilder.ManifestInspector.csproj') --no-restore -c Release -p:Platform=x86 -m:1 -nr:false -v:q *> (Join-Path $OutputDirectory 'compiler-build.log')
    if($LASTEXITCODE-ne 0){throw 'Batch compiler build failed; no worker started. Inspect compiler-build.log.'}
    $compilerHash=(Get-FileHash -LiteralPath $exe -Algorithm SHA256).Hash
    # Reborn: the apphost alone does not identify managed compiler code; freeze the complete runtime/assembly dependency set.
    $compilerFiles=@(foreach($name in @('BinaryAssetBuilder.ManifestInspector.exe','BinaryAssetBuilder.ManifestInspector.dll',
        'BinaryAssetBuilder.ManifestInspector.deps.json','BinaryAssetBuilder.ManifestInspector.runtimeconfig.json',
        'BinaryAssetBuilder.Core.dll','BinaryAssetBuilder.Utility.dll','BinaryAssetBuilder.XmlCompiler.dll','SageBinaryData.dll')){
        $path=Join-Path (Split-Path -Parent $exe) $name
        [pscustomobject]@{Name=$name;Path=$path;Sha256=(Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash}
    })
    foreach($case in @(
        @('layout','layout-self-test'),
        @('armor-input','armor-authoring-input-self-test'),
        @('weapon-input','weapon-authoring-input-self-test'),
        @('includes-defines','sdk-include-defines-self-test'),
        @('definition-subset','sdk-definition-subset-self-test'),
        @('inheritance','sdk-instance-inheritance-self-test'),
        @('expressions','sdk-instance-expressions-self-test'),
        @('file-reference-catalog','sdk-file-reference-catalog-self-test'),
        @('document-reuse','document-reuse-self-test'),
        @('dependency-resolution','dependency-resolution-self-test'),
        @('dependency-hash','dependency-hash-self-test'),
        @('checksum','checksum-self-test'))){Add-BatchCase $case[0] 'DiagnosticOnly' @($case[1])}
    Add-BatchCase 'weapon-authored-values' 'BoundedScopedPackage' @('weapon-authored-build-self-test',(Join-Path $OutputDirectory 'weapon-builds'),$StaticManifest,$GlobalManifest)
    # Reborn: independently pin stock GameObject armor-consumer links before permitting a source/compiler/publication path.
    Add-BatchCase 'gameobject-stock' 'BoundedStockNativeProjection' @('gameobject-stock-self-test',$StaticManifest)
    # Reborn: map every native root field against stock tokens without claiming indirect module semantics.
    Add-BatchCase 'gameobject-root-layout' 'BoundedRootLayoutEvidence' @('gameobject-root-layout-self-test',$StaticManifest)
    # Reborn: six module-container graphs and actual inline AI marshaler controls are one batched offline gate.
    Add-BatchCase 'gameobject-modules' 'BoundedModuleTopology' @('gameobject-module-self-test',$StaticManifest)
    # Reborn: three real module dispatch/payload implementations share one stock/edited-source regression gate.
    Add-BatchCase 'ep1-basic-modules' 'BoundedModuleNativePayload' @('ep1-basic-module-self-test',$StaticManifest)
    # Reborn: destruction/tech/LUA native dispatch and complete selected graphs share one gate; captured FX and callback limits remain explicit.
    Add-BatchCase 'ep1-allied-modules' 'BoundedCapturedIndexModuleNativePayload' @('ep1-allied-modules-self-test',$StaticManifest)
    # Reborn: nested effect/status graphs and Core-normalized strong imports share one new update-family gate.
    Add-BatchCase 'ep1-damage-motion' 'BoundedNestedModuleNativePayload' @('ep1-damage-motion-self-test',$StaticManifest)
    # Reborn: stock toggle/base/nested anchors and detached corruptions are a layout gate, not a new compiler capability.
    Add-BatchCase 'toggle-status-layout' 'BoundedNestedLayoutReview' @('toggle-status-layout-self-test',$StaticManifest)
    # Reborn: selected complete standalone AI graphs include present pointer values and real Core-normalized weapon imports, not enclosing module dispatch.
    Add-BatchCase 'ep1-toggle-ai' 'BoundedStandaloneAiNativePayload' @('ep1-toggle-ai-self-test',$StaticManifest)
    # Reborn: concrete module graphs currently use captured-index diagnostics; normal source/dispatch/publication is deliberately separate.
    Add-BatchCase 'ep1-toggle-module' 'BoundedCapturedIndexModuleNativePayload' @('ep1-toggle-module-self-test',$StaticManifest)
    # Reborn: independently catalog-normalized full module reference/default graphs must not substitute captured stock dependency indices.
    Add-BatchCase 'ep1-toggle-source' 'BoundedCoreNormalizedModuleNativePayload' @('ep1-toggle-source-self-test',$StaticManifest,$GlobalManifest)
    # Reborn: actual named source references must bind stock and newly compiled armor without captured numeric dependency indices.
    Add-BatchCase 'gameobject-armor-source' 'BoundedNamedNativeProjection' @('gameobject-armor-source-self-test',(Join-Path $OutputDirectory 'go'),$StaticManifest)
    Add-BatchCase 'armor-core' 'BoundedScopedStream' @('ep1-armor-document-self-test',(Join-Path $OutputDirectory 'armor-core'),$StaticManifest)
    Add-BatchCase 'armor-pair-core' 'BoundedScopedStream' @('armor-pair-document-self-test',(Join-Path $OutputDirectory 'armor-pair-core'),$StaticManifest)
    # Reborn: general single-armor identities/list/literals are tested jointly without widening the exported alpha gate.
    Add-BatchCase 'armor-general-authoring' 'ExperimentalGeneralArmorPackage' @('armor-general-authored-self-test',(Join-Path $OutputDirectory 'general-armors'))
    # Reborn: independently exercise multiple user-defined identities in one real stream, including the bounded maximum.
    Add-BatchCase 'armor-general-sets' 'ExperimentalGeneralArmorSetPackage' @('armor-general-set-self-test',(Join-Path $OutputDirectory 'armor-sets'))
    # Reborn: connect captured source semantics to native set/package verification, without expanding the exported alpha.
    Add-BatchCase 'armor-source-bridge' 'ExperimentalArmorSourcePackage' @('armor-source-self-test',(Join-Path $OutputDirectory 'as'))
    if($Full){
        Add-BatchCase 'armor-authored-fields' 'BoundedScopedPackage' @('armor-authored-build-self-test',(Join-Path $OutputDirectory 'armor-builds'),$StaticManifest)
        Add-BatchCase 'compiler-broad' 'DiagnosticOnly' @('compiler-self-test')
    }
}
$rows=[Collections.Generic.List[object]]::new()
$active=[Collections.Generic.List[object]]::new()
$next=0;$started=[DateTime]::UtcNow
while($next-lt $cases.Count -or $active.Count-gt 0){
    while($next-lt $cases.Count -and $active.Count-lt $Workers){
        $case=$cases[$next];$next++
        $temporary=Join-Path $OutputDirectory ($case.Name+'-temp');$null=New-Item -ItemType Directory -Path $temporary
        $info=[Diagnostics.ProcessStartInfo]::new($exe)
        $info.WorkingDirectory=$repo;$info.UseShellExecute=$false;$info.CreateNoWindow=$true
        $info.RedirectStandardOutput=$true;$info.RedirectStandardError=$true
        # Reborn: separate processes isolate Core static settings/caches; each owns its TEMP/TMP, files and logs.
        $info.Environment['TEMP']=$temporary;$info.Environment['TMP']=$temporary
        foreach($argument in $case.Arguments){$info.ArgumentList.Add($argument)}
        try{
            $process=[Diagnostics.Process]::Start($info)
            $active.Add([pscustomobject]@{Case=$case;Process=$process;Started=[DateTime]::UtcNow;
                Output=$process.StandardOutput.ReadToEndAsync();Error=$process.StandardError.ReadToEndAsync()})
            Write-Output ('Reborn: batch started '+$case.Name)
        }catch{
            $rows.Add([pscustomobject]@{Name=$case.Name;Evidence=$case.Evidence;Status='StartFailed';ExitCode=$null;Error=$_.Exception.Message;ElapsedSeconds=0;Log=$null})
        }
    }
    foreach($job in @($active.ToArray())){
        if(-not $job.Process.HasExited){continue}
        $job.Process.WaitForExit()
        $log=Join-Path $OutputDirectory ($job.Case.Name+'.log')
        [IO.File]::WriteAllText($log,$job.Output.GetAwaiter().GetResult()+[Environment]::NewLine+$job.Error.GetAwaiter().GetResult())
        $code=$job.Process.ExitCode;$status=if($code-eq 0){'Passed'}else{'Failed'}
        $rows.Add([pscustomobject]@{Name=$job.Case.Name;Evidence=$job.Case.Evidence;Status=$status;ExitCode=$code;
            ElapsedSeconds=[Math]::Round(([DateTime]::UtcNow-$job.Started).TotalSeconds,3);Log=$log})
        Write-Output ('Reborn: batch '+$status+' '+$job.Case.Name+' (exit '+$code+')')
        $job.Process.Dispose();$null=$active.Remove($job)
    }
    # Reborn: short scheduler polling does not serialize child tests or pause user-visible progress for long intervals.
    if($active.Count-gt 0){Start-Sleep -Milliseconds 100}
}
$failed=@($rows|Where-Object Status -NE 'Passed')
$unchanged=$true
foreach($entry in $compilerFiles){if((Get-FileHash -LiteralPath $entry.Path -Algorithm SHA256).Hash-cne $entry.Sha256){$unchanged=$false}}
$report=[pscustomobject]@{Mode=$(if($SchedulerControl){'SchedulerControl'}elseif($Full){'Full'}else{'Focused'});
    CompilerPath=$exe;CompilerSha256=$compilerHash;CompilerFiles=$compilerFiles;CompilerUnchanged=$unchanged;Workers=$Workers;
    PlannedGroups=$cases.Count;CompletedGroups=$rows.Count;PassedGroups=$rows.Count-$failed.Count;FailedGroups=$failed.Count;
    ElapsedSeconds=[Math]::Round(([DateTime]::UtcNow-$started).TotalSeconds,3);Cases=@($rows|Sort-Object Name);
    GameExecuted=$false;InstalledFilesModified=$false;AlphaCapabilitiesExpanded=$false;
    Limitation='Diagnostic source semantics are not general playable publication; a batch is separate packages, not a mixed-asset stream.'}
[IO.File]::WriteAllText((Join-Path $OutputDirectory 'batch-report.json'),($report|ConvertTo-Json -Depth 7))
Write-Output ('Reborn: batch complete: '+$report.PassedGroups+'/'+$report.PlannedGroups+' groups passed; report '+(Join-Path $OutputDirectory 'batch-report.json'))
if($SchedulerControl){
    if($rows.Count-ne 3 -or $failed.Count-ne 1 -or @($rows|Where-Object {$_.Name-ceq 'control-fail' -and $_.ExitCode-eq 7}).Count-ne 1 -or @($rows|Where-Object {$_.Name-ceq 'control-after-failure' -and $_.Status-ceq 'Passed'}).Count-ne 1){throw 'Scheduler failure/continuation controls failed.'}
    Write-Output 'Reborn: scheduler controls PASS; expected failure is retained in the report.'
}elseif($failed.Count-gt 0 -or -not $unchanged){exit 1}
