# Reborn: report only unchanged originals tested with the exact current compiler/harness/source-plan generation; never promote stale passes.
[CmdletBinding()]
param([string]$Repository=(Split-Path -Parent $PSScriptRoot),[string]$SuiteDirectory,[switch]$RequireComplete)
$ErrorActionPreference='Stop'
$taskSdk=Join-Path $Repository 'alpha'
if(!$SuiteDirectory){$SuiteDirectory=Join-Path $taskSdk '.work/stock-source-coverage'}
$SuiteDirectory=[IO.Path]::GetFullPath($SuiteDirectory)
if(!$SuiteDirectory.StartsWith([IO.Path]::GetFullPath((Join-Path $taskSdk '.work'))+'\',[StringComparison]::OrdinalIgnoreCase)){throw 'Coverage reports must remain below the SDK work directory.'}
$taskCompiler=Join-Path $taskSdk 'tools/compiler'
$taskHashes=@('BinaryAssetBuilder.ManifestInspector.dll','BinaryAssetBuilder.XmlCompiler.dll','BinaryAssetBuilder.W3XCompiler.dll','BinaryAssetBuilder.Core.dll','SageBinaryData.dll')|ForEach-Object {(Get-FileHash -LiteralPath (Join-Path $taskCompiler $_)).Hash}
$taskHarness=Join-Path $Repository 'scripts/Test-Ra3Ep1StockSources.ps1'
$taskHashes+=(Get-FileHash -LiteralPath $taskHarness).Hash
$taskGeneration=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes(($taskHashes -join ':'))))
$taskSource=Join-Path $taskSdk 'SageXML'
$taskFiles=[IO.Directory]::GetFiles($taskSource,'*.xml',[IO.SearchOption]::AllDirectories)
[array]::Sort($taskFiles,[StringComparer]::OrdinalIgnoreCase)
$taskInventory=@($taskFiles|ForEach-Object {[pscustomobject]@{Relative=[IO.Path]::GetRelativePath($taskSource,$_);Sha256=(Get-FileHash -LiteralPath $_).Hash}})
$taskPlanKey=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($taskGeneration+($taskInventory.Sha256 -join ':'))))
$taskLatest=[Collections.Generic.Dictionary[string,object]]::new([StringComparer]::OrdinalIgnoreCase)
foreach($taskLine in [IO.File]::ReadLines((Join-Path $SuiteDirectory 'results.jsonl'))){if($taskLine){$taskRow=$taskLine|ConvertFrom-Json;$taskLatest[$taskRow.Relative]=$taskRow}}
$taskCurrent=[Collections.Generic.List[object]]::new();$taskRemaining=[Collections.Generic.List[string]]::new()
foreach($taskItem in $taskInventory){
    $taskRow=$null
    if($taskLatest.TryGetValue($taskItem.Relative,[ref]$taskRow) -and $taskRow.CompilerGeneration -ceq $taskGeneration -and $taskRow.SourceSha256 -ceq $taskItem.Sha256 -and $taskRow.SourcePlanSha256 -ceq $taskPlanKey -and !$taskRow.DeveloperChecks){$taskCurrent.Add($taskRow)}
    else{$taskRemaining.Add($taskItem.Relative)}
}
$taskPassed=@($taskCurrent|Where-Object Status -eq 'Passed')
$taskFailed=@($taskCurrent|Where-Object Status -ne 'Passed')
$taskSupport=@($taskFailed|Where-Object {$_.Class -notin @('Dependency','SourceOrSchema','SourceContextRequired')})
# Reborn: source-only/context qualifications are reported honestly and never counted as standalone native passes.
$taskCounts=@($taskCurrent|Group-Object Status,Class|ForEach-Object {[pscustomobject]@{Status=$_.Group[0].Status;Class=$_.Group[0].Class;Count=$_.Count}})
$taskResult=[pscustomobject]@{
    Format='RebornStockCoverageReport1';CreatedUtc=[DateTime]::UtcNow.ToString('o');CompilerGeneration=$taskGeneration;SourcePlanSha256=$taskPlanKey
    OriginalFiles=$taskInventory.Count;CurrentGenerationAttempted=$taskCurrent.Count;Remaining=$taskRemaining.Count
    Passed=$taskPassed.Count;Failed=$taskFailed.Count;SupportOrHarnessFailures=$taskSupport.Count
    EveryOriginalTestedWithCurrentGeneration=($taskRemaining.Count-eq 0)
    AllRecordedSupportAndHarnessFailuresResolved=($taskRemaining.Count-eq 0 -and $taskSupport.Count-eq 0)
    EveryOriginalPassed=($taskRemaining.Count-eq 0 -and $taskFailed.Count-eq 0)
    NativeStandalone=@($taskPassed|Where-Object Class -eq 'NativeQualified').Count
    SourceConsumerQualified=@($taskPassed|Where-Object Class -eq 'ContextQualified').Count
    SourceOnlyQualified=@($taskPassed|Where-Object Class -eq 'SourceOnlyQualified').Count
    Counts=$taskCounts;RemainingFiles=@($taskRemaining);Failures=@($taskFailed|Select-Object Relative,Stage,Class,Message,LogDirectory)
    CompilerAndHarnessSha256=$taskHashes;ModPublished=$false;GameExecuted=$false
}
$taskReport=Join-Path $SuiteDirectory 'coverage-current-report.json'
[IO.File]::WriteAllText($taskReport,($taskResult|ConvertTo-Json -Depth 9))
Write-Output ('Reborn: current-generation coverage: '+$taskCurrent.Count+'/'+$taskInventory.Count+'; passed='+$taskPassed.Count+'; failed='+$taskFailed.Count+'; unresolved support/harness='+$taskSupport.Count+'.')
Write-Output ('Reborn: evidence report: '+$taskReport)
if($RequireComplete -and ($taskRemaining.Count-ne 0 -or $taskSupport.Count-ne 0)){throw 'Coverage is incomplete or still contains unresolved support/harness failures; no completion claim is allowed.'}
