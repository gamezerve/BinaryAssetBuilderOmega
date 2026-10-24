# Reborn: shared readable Studio reporting; raw compiler output is retained separately, never discarded.
$script:BuildLogLines=[Collections.Generic.List[string]]::new()
$script:BuildClock=[Diagnostics.Stopwatch]::StartNew()
$script:PhaseClock=$null
$script:PhaseLabel='Initializing'
#-------------------------------------------------------------------------------------------------
<# Reborn: capture exactly the human-readable lines displayed by Studio for the saved build log. #>
#-------------------------------------------------------------------------------------------------
function Write-SdkBuildLine([string]$Message){$script:BuildLogLines.Add($Message);Write-Output $Message}
#-------------------------------------------------------------------------------------------------
<# Reborn: time real phases independently while keeping the original SDK step numbering. #>
#-------------------------------------------------------------------------------------------------
function Start-SdkBuildPhase([int]$Number,[string]$Label){$script:PhaseLabel="Step ${Number}: $Label";$script:PhaseClock=[Diagnostics.Stopwatch]::StartNew();Write-SdkBuildLine $script:PhaseLabel}
#-------------------------------------------------------------------------------------------------
<# Reborn: do not label skipped or failed work as a successful compiler operation. #>
#-------------------------------------------------------------------------------------------------
function Stop-SdkBuildPhase([string]$Note){if($Note){Write-SdkBuildLine $Note};if($script:PhaseClock){$script:PhaseClock.Stop();Write-SdkBuildLine ('Step time taken: '+$script:PhaseClock.Elapsed.ToString('c'));$script:PhaseClock=$null}}
#-------------------------------------------------------------------------------------------------
<# Reborn: preserve raw lines on disk and show diagnostics/progress without JSON receipts or stack-trace noise. #>
#-------------------------------------------------------------------------------------------------
function Write-SdkCompilerLine([string]$Line,[string]$DetailPath){
    [IO.File]::AppendAllText($DetailPath,$Line+[Environment]::NewLine)
    $trim=$Line.Trim()
    # Reborn: "Reborn:" is this toolchain's internal source tag, not the selected mod's name; the raw detail log keeps it, the display drops it.
    if($trim -cmatch '^Reborn:\s*(.+)$'){$rest=$Matches[1];$trim=[string][char]::ToUpperInvariant($rest[0])+$rest.Substring(1)}
    $trim=$trim -creplace '^((?:ERROR|Error|Warning|Critical):\s*)Reborn:\s*','$1'
    if($trim -match '^(?:ERROR|Error|Warning|Critical):'){
        # Reborn: severity must remain at the beginning for EA Studio's Error Log classifier.
        Write-SdkBuildLine ($trim -creplace '^ERROR:','Error:')
    }elseif($trim -match '^\[\d\d:\d\d:\d\d'){
        # Reborn: retain an existing compiler clock instead of double-prefixing it with the wrapper clock.
        Write-SdkBuildLine $trim
    }elseif($trim -match '^(.+?: PASS)\b'){
        # Reborn: keep qualification progress readable; coverage counters, internal paths and caveats remain intact in the raw log.
        Write-SdkBuildLine ('['+$script:BuildClock.Elapsed.ToString('c')+'] '+$Matches[1])
    }elseif($trim -cmatch '^Source path \[' -or $trim -cmatch '^Authored asset count:' -or $trim -cmatch '^Dependency audit:' -or $trim -match '^(?:Compiling |Resolving references:|Generating stream:|.* Stream complete$)' -or $trim -match "^Loading 'file://" -or $trim -match '^Statistics [A-Za-z]+: Instances Processed \d+,' -or $trim -match '^(?:Validation|Compilation) (?:started|completed): ' -or $trim -match '^[^\\/:\[\]]+\.xml: Compiling [A-Za-z]+:\S+ \[[0-9a-f]{8}:[0-9a-f]{8}\]$'){
        Write-SdkBuildLine ('['+$script:BuildClock.Elapsed.ToString('c')+'] '+$trim)
    }
}
