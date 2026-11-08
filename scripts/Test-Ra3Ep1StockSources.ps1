# Reborn: sequentially qualify every original XML through the production frontend/native compiler without publishing mod output.
[CmdletBinding()]
param([string]$Repository=(Split-Path -Parent $PSScriptRoot),[string]$OutputDirectory,[int]$Take=0,[string[]]$Only,[switch]$RetryFailed,[int]$FileTimeoutSeconds=600,[switch]$ReuseProcess,[string]$StartAfter,[switch]$DeveloperChecks)
$ErrorActionPreference='Stop'
$sdk=Join-Path $Repository 'alpha';$source=Join-Path $sdk 'SageXML';$compiler=Join-Path $sdk 'tools/compiler/BinaryAssetBuilder.ManifestInspector.exe'
if(!$OutputDirectory){$OutputDirectory=Join-Path $sdk '.work/stock-source-coverage'}
$OutputDirectory=[IO.Path]::GetFullPath($OutputDirectory)
if(!$OutputDirectory.StartsWith([IO.Path]::GetFullPath((Join-Path $sdk '.work'))+'\',[StringComparison]::OrdinalIgnoreCase)){throw 'Stock qualification output must stay below the SDK work directory.'}
$null=New-Item -ItemType Directory -Path $OutputDirectory -Force
$identity=Join-Path $OutputDirectory 'suite.json'
if(Test-Path $identity){$saved=Get-Content $identity -Raw|ConvertFrom-Json;if($saved.Format -cne 'RebornStockSourceSuite1' -or $saved.SourceRoot -ine $source){throw 'Not this source qualification directory.'}}
else{[IO.File]::WriteAllText($identity,([pscustomobject]@{Format='RebornStockSourceSuite1';SourceRoot=$source;CreatedUtc=[DateTime]::UtcNow.ToString('o');GameExecuted=$false;ModPublished=$false}|ConvertTo-Json))}
$hashes=@('BinaryAssetBuilder.ManifestInspector.dll','BinaryAssetBuilder.XmlCompiler.dll','BinaryAssetBuilder.W3XCompiler.dll','BinaryAssetBuilder.Core.dll','SageBinaryData.dll') | ForEach-Object {(Get-FileHash (Join-Path (Split-Path $compiler) $_)).Hash}
# Reborn: changing the harness itself also invalidates earlier qualification results.
$hashes+= (Get-FileHash -LiteralPath $PSCommandPath).Hash
$generation=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes(($hashes -join ':'))))
$results=Join-Path $OutputDirectory 'results.jsonl';$last=@{}
if(Test-Path $results){foreach($line in [IO.File]::ReadLines($results)){if($line){$row=$line|ConvertFrom-Json;$last[$row.Relative]=$row}}}
$files=[IO.Directory]::GetFiles($source,'*.xml',[IO.SearchOption]::AllDirectories);[array]::Sort($files,[StringComparer]::OrdinalIgnoreCase)
$inventory=@($files|ForEach-Object {[pscustomobject]@{Relative=[IO.Path]::GetRelativePath($source,$_);Bytes=(Get-Item $_).Length;Sha256=(Get-FileHash $_).Hash}})
# Reborn: every explicitly requested corpus file must exist in the inventoried originals.
if($Only){foreach($requested in $Only){if($inventory.Relative -inotcontains $requested){throw ('Unknown stock source test selection: '+$requested)}}}
# Reborn: a resume boundary must refer to an actual original source.
if($StartAfter -and $inventory.Relative -inotcontains $StartAfter){throw ('Unknown stock resume boundary: '+$StartAfter)}
[IO.File]::WriteAllText((Join-Path $OutputDirectory 'inventory.json'),(ConvertTo-Json -InputObject $inventory -Depth 5))
# Reborn: a fragment is compiled through an actual source-authored consumer, retained in every result.
$planFile=Join-Path $OutputDirectory 'source-plan.json';$planKeyFile=Join-Path $OutputDirectory 'source-plan.key'
$planKey=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($generation+($inventory.Sha256 -join ':'))))
if(!(Test-Path -LiteralPath $planFile) -or !(Test-Path -LiteralPath $planKeyFile) -or [IO.File]::ReadAllText($planKeyFile)-cne $planKey){
    & $compiler stock-source-plan $sdk $planFile
    if($LASTEXITCODE-ne 0){throw 'Original source context inventory failed.'}
    [IO.File]::WriteAllText($planKeyFile,$planKey)
}
$sourcePlan=@{};foreach($row in (Get-Content -LiteralPath $planFile -Raw|ConvertFrom-Json)){$sourcePlan[$row.Relative]=$row}
$processed=0;$caseIndex=0
$env:REBORN_DEVELOPER_BUILD_TESTS=$(if($DeveloperChecks){'1'}else{'0'});$env:REBORN_SILENT_MODE='1'
#-------------------------------------------------------------------------------------------------
<# Reborn: release the bounded sequential worker before restarting it or leaving the corpus run. #>
#-------------------------------------------------------------------------------------------------
function Close-StockWorker {
    if(!$script:stockWorker){return}
    try{if(!$script:stockWorker.HasExited){$script:stockWorker.StandardInput.Close();if(!$script:stockWorker.WaitForExit(5000)){$script:stockWorker.Kill($true);$script:stockWorker.WaitForExit()}}}
    finally{$script:stockWorker.Dispose();$script:stockWorker=$null;$script:stockRequests=0}
}
#-------------------------------------------------------------------------------------------------
<# Reborn: a concurrent read of the checkpoint may briefly deny append; retry opening without duplicating a written result. #>
#-------------------------------------------------------------------------------------------------
function Add-StockResult([string]$Json) {
    $stream=$null
    for($attempt=0;$attempt-lt 20;$attempt++){
        try{$stream=[IO.FileStream]::new($results,[IO.FileMode]::Append,[IO.FileAccess]::Write,[IO.FileShare]::ReadWrite);break}
        catch [IO.IOException]{if($attempt-eq 19){throw};Start-Sleep -Milliseconds 100}
    }
    try{$bytes=[Text.Encoding]::UTF8.GetBytes($Json+"`n");$stream.Write($bytes,0,$bytes.Length);$stream.Flush($true)}
    finally{if($stream){$stream.Dispose()}}
}
#-------------------------------------------------------------------------------------------------
<# Reborn: synchronous requests retain separate logs and full production proof; recycle after at most sixteen files. #>
#-------------------------------------------------------------------------------------------------
function Invoke-ReusedStockCompiler([string[]]$Arguments,[string]$Log,[string]$Work) {
    if($script:stockWorker -and ($script:stockWorker.HasExited -or $script:stockRequests-ge 32)){Close-StockWorker}
    if(!$script:stockWorker){
        $start=[Diagnostics.ProcessStartInfo]::new($compiler);$start.UseShellExecute=$false;$start.CreateNoWindow=$true
        $start.RedirectStandardInput=$true;$start.RedirectStandardOutput=$true;$start.RedirectStandardError=$true;$start.WorkingDirectory=$sdk
        $start.ArgumentList.Add('stock-source-worker');$start.ArgumentList.Add($sdk)
        $script:stockWorker=[Diagnostics.Process]::new();$script:stockWorker.StartInfo=$start;$null=$script:stockWorker.Start()
        $script:stockWorkerError=$script:stockWorker.StandardError.ReadToEndAsync();$script:stockRequests=0
    }
    $request=[pscustomobject]@{Arguments=$Arguments;Log=$Log;Work=$Work}|ConvertTo-Json -Compress
    $script:stockWorker.StandardInput.WriteLine($request);$script:stockWorker.StandardInput.Flush();$script:stockRequests++
    $reply=$script:stockWorker.StandardOutput.ReadLineAsync()
    if(!$reply.Wait($FileTimeoutSeconds*1000)){
        $script:stockWorker.Kill($true);$script:stockWorker.WaitForExit();Close-StockWorker
        [IO.File]::AppendAllText($Log,"`nERROR: Sequential stock compiler timed out.");return 124
    }
    $code=0
    if(![int]::TryParse($reply.Result,[ref]$code)){
        $detail=$script:stockWorkerError.GetAwaiter().GetResult();Close-StockWorker
        [IO.File]::AppendAllText($Log,"`nERROR: Sequential stock worker exited without a result. "+$detail);return 1
    }
    return $code
}
#-------------------------------------------------------------------------------------------------
<# Reborn: drain both child pipes asynchronously; each process is isolated and timeout is reported as unqualified, never as a pass. #>
#-------------------------------------------------------------------------------------------------
function Invoke-StockCompiler([string[]]$Arguments,[string]$Log,[string]$Work) {
    if($ReuseProcess){return Invoke-ReusedStockCompiler $Arguments $Log $Work}
    $start=[Diagnostics.ProcessStartInfo]::new($compiler);$start.UseShellExecute=$false;$start.CreateNoWindow=$true
    $start.RedirectStandardOutput=$true;$start.RedirectStandardError=$true;$start.WorkingDirectory=$sdk
    foreach($argument in $Arguments){$start.ArgumentList.Add($argument)}
    $start.Environment['TEMP']=$Work;$start.Environment['TMP']=$Work
    $process=[Diagnostics.Process]::new();$process.StartInfo=$start
    try{
        $null=$process.Start();$stdout=$process.StandardOutput.ReadToEndAsync();$stderr=$process.StandardError.ReadToEndAsync()
        $exited=$process.WaitForExit($FileTimeoutSeconds*1000)
        if(!$exited){$process.Kill($true);$process.WaitForExit()}
        [IO.File]::WriteAllText($Log,$stdout.GetAwaiter().GetResult()+"`n"+$stderr.GetAwaiter().GetResult())
        return $(if($exited){$process.ExitCode}else{124})
    }finally{$process.Dispose()}
}
#-------------------------------------------------------------------------------------------------
<# Reborn: delete only this case's generated work after checking absolute containment and every descendant for redirection. #>
#-------------------------------------------------------------------------------------------------
function Remove-StockCaseWork([string]$Work,[string]$CaseRoot) {
    if([IO.Path]::GetDirectoryName([IO.Path]::GetFullPath($Work)) -ine [IO.Path]::GetFullPath($CaseRoot)){throw 'Unexpected stock case cleanup target.'}
    foreach($item in @(Get-Item $Work)+@(Get-ChildItem $Work -Force -Recurse)){if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected stock case retained.'}}
    Remove-Item -LiteralPath $Work -Force -Recurse
}
try{foreach($file in $inventory){
    $caseIndex++
    if($Only -and $Only -inotcontains $file.Relative){continue}
    if($StartAfter -and [StringComparer]::OrdinalIgnoreCase.Compare($file.Relative,$StartAfter)-le 0){continue}
    if(Test-Path (Join-Path $OutputDirectory 'stop.request')){Write-Output 'Reborn: stopped at the requested case boundary.';break}
    $previous=$last[$file.Relative]
    if($previous -and $previous.SourceSha256 -ceq $file.Sha256 -and $previous.CompilerGeneration -ceq $generation -and $previous.SourcePlanSha256 -ceq $planKey -and [bool]$previous.DeveloperChecks -eq [bool]$DeveloperChecks -and (!$RetryFailed -or $previous.Status -eq 'Passed')){continue}
    if($Take -gt 0 -and $processed-ge $Take){break}
    $processed++;$clock=[Diagnostics.Stopwatch]::StartNew()
    $caseRoot=Join-Path $OutputDirectory ('cases/'+$caseIndex.ToString('D5'));$null=New-Item -ItemType Directory -Path $caseRoot -Force
    $work=Join-Path $caseRoot ('work-'+[guid]::NewGuid().ToString('N'));$null=New-Item -ItemType Directory -Path (Join-Path $work 'input/Data') -Force
    $context=$sourcePlan[$file.Relative];$testEntry=if($context){$context.Entry}else{$file.Relative}
    $wrapper=Join-Path $work 'input/Data/Static_Mod.xml';$escaped=[Security.SecurityElement]::Escape('DATA:'+$testEntry.Replace('\','/'))
    [IO.File]::WriteAllText($wrapper,('<AssetDeclaration xmlns="uri:ea.com:eala:asset"><!-- Reborn: emit exactly one original source file and its authored all-Includes. --><Includes><Include type="all" source="'+$escaped+'" /></Includes></AssetDeclaration>'))
    $stage='Preparation';$status='Failed';$errorText='';$assetCount=0;$packageHash=$null;$contextSources=@();$emittedOrigins=@()
    # Reborn: an empty declaration has source-only evidence and must never acquire native-qualified status.
    $sourceOnly=$false;$sourceProofHash=$null
    try{
        $prepared=Join-Path $work 'source';$prepareLog=Join-Path $caseRoot 'prepare.log'
        $code=Invoke-StockCompiler @('conscript-source-prepare',$wrapper,$prepared,$sdk,$source) $prepareLog $work
        if($code-ne 0){
            $errorText=[IO.File]::ReadAllText($prepareLog)
            if($code-eq 124){$status='Timeout'}
            elseif($errorText.Contains('This route requires a supported emitted asset; imported bases must use type=instance.')){
                # Reborn: the dedicated validator repeats real graph/schema/freshness checks and refuses any nonempty emitted closure.
                $emptyReceipt=Join-Path $work 'source-only.json';$emptyLog=Join-Path $caseRoot 'source-only.log'
                $emptyCode=Invoke-StockCompiler @('stock-source-empty-validate',$wrapper,$emptyReceipt,$sdk,$source) $emptyLog $work
                if($emptyCode-eq 0){
                    $empty=Get-Content -LiteralPath $emptyReceipt -Raw|ConvertFrom-Json
                    if(!$empty.SourceOnlyQualified -or $empty.AssetCount-ne 0 -or $empty.NativeAssetsEmitted){throw 'Empty source evidence claims native output.'}
                    $contextSources=@($empty.Sources|Where-Object {$_.Path.StartsWith($source+'\',[StringComparison]::OrdinalIgnoreCase)}|ForEach-Object {[pscustomobject]@{Relative=[IO.Path]::GetRelativePath($source,$_.Path);Sha256=$_.Sha256}})
                    if(!($contextSources|Where-Object {$_.Relative-ieq $file.Relative -and $_.Sha256-ceq $file.Sha256})){throw 'Empty-source proof did not capture the requested unchanged original.'}
                    Copy-Item -LiteralPath $emptyReceipt -Destination (Join-Path $caseRoot 'source-only-proof.json') -Force
                    $sourceProofHash=(Get-FileHash -LiteralPath $emptyReceipt).Hash
                    $sourceOnly=$true;$status='Passed';$stage='SourceOnly';$errorText=''
                }else{$errorText=[IO.File]::ReadAllText($emptyLog);if($emptyCode-eq 124){$status='Timeout'}}
            }
        }
        else{
            $receipt=Get-Content (Join-Path $prepared 'source-input.json') -Raw|ConvertFrom-Json;$assetCount=$receipt.AssetCount;$contextSources=@($receipt.Sources|Where-Object {$_.Path.StartsWith($source+'\',[StringComparison]::OrdinalIgnoreCase)}|ForEach-Object {[pscustomobject]@{Relative=[IO.Path]::GetRelativePath($source,$_.Path);Sha256=$_.Sha256}});$emittedOrigins=@($receipt.Origins);$stage='Native'
            $compiled=Join-Path $work 'compiled';$nativeLog=Join-Path $caseRoot 'native.log'
            $code=Invoke-StockCompiler @('conscript-alpha-build',$source,(Join-Path $sdk 'catalogs/ra3ep1/streams/static/static.manifest'),(Join-Path $sdk 'catalogs/ra3ep1/streams/global/global.manifest'),$receipt.GamePath,$compiled) $nativeLog $work
            if($code-ne 0){$errorText=[IO.File]::ReadAllText($nativeLog);if($code-eq 124){$status='Timeout'}}
            else{
                $nativeManifest=Join-Path $compiled 'stream-readback/diagnostic.manifest'
                if(Test-Path $nativeManifest){$packageHash=(Get-FileHash $nativeManifest).Hash}
                else{
                    # Reborn: the singleton production route qualifies its linked stream inside the independently read-back trial BIG.
                    $packages=@(Get-ChildItem -LiteralPath $compiled -Filter 'RebornConscriptProbe.big' -File -Recurse)
                    if($packages.Count-ne 1){throw 'No unique qualified native stream/package was produced.'}
                    $packageHash=(Get-FileHash $packages[0].FullName).Hash
                }
                # Reborn: attach a native pass only to the unchanged source and the real captured consumer closure.
                if((Get-FileHash -LiteralPath (Join-Path $source $file.Relative)).Hash -cne $file.Sha256){throw 'Original source changed during corpus qualification.'}
                if($context.Kind-eq 'ConsumerContext' -and !($contextSources|Where-Object {$_.Relative -ieq $file.Relative -and $_.Sha256 -ceq $file.Sha256})){throw 'Selected consumer did not capture the requested source fragment.'}
                $status='Passed'
            }
        }
    }catch{$errorText=$_.Exception.ToString()}
    $clock.Stop()
    $message=($errorText -split "`n" | Where-Object {$_ -match 'Error:|ERROR:|Exception:|UNSUPPORTED|Unsupported|not reviewed|Unreviewed'} | Select-Object -First 1)
    if(!$message -and $errorText){$message=($errorText -split "`n" | Select-Object -First 1)}
    if($sourceOnly){$message='Source-only qualification; no native assets emitted.'}
    $class=if($status-eq 'Passed' -and $sourceOnly){'SourceOnlyQualified'}elseif($status-eq 'Passed'){'NativeQualified'}elseif($status-eq 'Timeout'){'Timeout'}elseif($errorText-match 'not reviewed|Unreviewed|Unsupported emitted|UNSUPPORTEDASSET|not supported yet|not implemented|processor is not connected|no reviewed native|outside the reviewed|requires a supported emitted'){'CompilerSupport'}elseif($errorText-match 'XmlException|XML declaration|invalid XML|Data at the root|mismatched|unexpected end|element cannot contain whitespace|Pattern constraint|invalid according to its datatype|has invalid child element|Empty or hash-colliding local module ID:\s*(\r|\n|$)'){'SourceOrSchema'}elseif($errorText-match 'missing|not found|FileNotFound|Source graph:'){'Dependency'}else{'NativeOrHarness'}
    # Reborn: context qualification is useful source-consumer evidence, not standalone native emission of a fragment.
    if($status-eq 'Passed' -and !$sourceOnly -and $context.Kind-eq 'ConsumerContext'){$class='ContextQualified'}
    if($status-ne 'Passed' -and $context.Kind-eq 'UnreferencedSourceFragment'){$class='SourceContextRequired'}
    $row=[pscustomobject]@{Relative=$file.Relative;SourceSha256=$file.Sha256;SourcePlanSha256=$planKey;CompilerGeneration=$generation;DeveloperChecks=[bool]$DeveloperChecks;TestEntry=$testEntry;SourceKind=$context.Kind;Status=$status;Stage=$stage;Class=$class;Assets=$assetCount;Seconds=[Math]::Round($clock.Elapsed.TotalSeconds,3);Message=[string]$message;NativeEvidenceSha256=$packageHash;SourceEvidenceSha256=$sourceProofHash;ContextSources=$contextSources;EmittedOrigins=$emittedOrigins;LogDirectory=$caseRoot;CompletedUtc=[DateTime]::UtcNow.ToString('o')}
    Add-StockResult ($row|ConvertTo-Json -Compress -Depth 5);$last[$file.Relative]=$row
    [IO.File]::WriteAllText((Join-Path $caseRoot 'result.json'),($row|ConvertTo-Json -Depth 5))
    $counts=@($last.Values|Group-Object Status|ForEach-Object {[pscustomobject]@{Status=$_.Name;Count=$_.Count}})
    # Reborn: never label old-generation passes as proof of the current compiler and original include closure.
    $current=@($last.Values|Where-Object {$_.CompilerGeneration -ceq $generation -and $_.SourcePlanSha256 -ceq $planKey})
    $currentCounts=@($current|Group-Object Status|ForEach-Object {[pscustomobject]@{Status=$_.Name;Count=$_.Count}})
    [IO.File]::WriteAllText((Join-Path $OutputDirectory 'progress.json'),([pscustomobject]@{TotalFiles=$inventory.Count;Attempted=$last.Count;Current=$file.Relative;Counts=$counts;CurrentGenerationAttempted=$current.Count;CurrentGenerationRemaining=($inventory.Count-$current.Count);CurrentGenerationCounts=$currentCounts;CompilerGeneration=$generation}|ConvertTo-Json -Depth 6))
    Write-Output ('['+$caseIndex+'/'+$inventory.Count+'] '+$status+' '+$file.Relative+' ('+$row.Seconds+'s): '+$row.Message)
    Remove-StockCaseWork $work $caseRoot
}}finally{Close-StockWorker}
Write-Output ('Reborn: sequential stock qualification checkpoint: '+$last.Count+'/'+$inventory.Count+' files attempted. '+$results)
