# Reborn: verify actual PowerShell map filtering and the UTF-8 UI handoff without invoking the compiler.
param([Parameter(Mandatory)][string]$FixtureSdk,[Parameter(Mandatory)][string]$MapsScript)
$ErrorActionPreference='Stop'
. $MapsScript
function Write-SdkBuildLine([string]$Text) { }
$checks=0
#-------------------------------------------------------------------------------------------------
<# Reborn: selection failures must remain explicit rather than compiling a larger set. #>
#-------------------------------------------------------------------------------------------------
function Assert-Selection([bool]$Condition,[string]$Name) { if(!$Condition){throw $Name};$script:checks++ }
#-------------------------------------------------------------------------------------------------
<# Reborn: malformed, stale and out-of-mod map requests must fail before any native tool could run. #>
#-------------------------------------------------------------------------------------------------
function Assert-Rejected([scriptblock]$Action,[string]$Name) { $rejected=$false;try{& $Action | Out-Null}catch{$rejected=$true};Assert-Selection $rejected $Name }
$data=Join-Path $FixtureSdk 'mods/Alpha/Data'
$root=Join-Path $data 'Maps/Official'
$all=@(Get-RebornOfficialMapFolders $data)
Assert-Selection ($all.Count-eq 6) 'All map discovery'
$paths=@(Read-RebornMapSelection (Join-Path $FixtureSdk 'selection-for-backend.txt'))
$selected=@(Get-RebornOfficialMapFolders $data -OfficialMapDirectories $paths)
Assert-Selection ($selected.Count-eq 2 -and $selected[0].Name-eq 'CAMP_A01_BrightonBeach_Smith' -and $selected[1].Name-eq 'CAMP_S01_LeningradCanals_Rao') 'UI handoff selects exactly two maps'
$single=@(Get-RebornOfficialMapFolders $data -OfficialMapDirectory (Join-Path $root 'All'))
Assert-Selection ($single.Count-eq 1 -and $single[0].Name-eq 'All') 'Legacy single-map path and All-named folder'
$special=@(Get-RebornOfficialMapFolders $data -OfficialMapDirectories @((Join-Path $root 'Map with spaces;Unicode_Ü')))
Assert-Selection ($special.Count-eq 1) 'Unicode spaces and semicolon paths'
$case=@(Get-RebornOfficialMapFolders $data -OfficialMapDirectories @($paths[0].ToUpperInvariant(),$paths[0],$paths[1]))
Assert-Selection ($case.Count-eq 2) 'Case-insensitive deduplication'
Assert-Rejected {Get-RebornOfficialMapFolders $data -OfficialMapDirectories @()} 'Empty does not mean All'
Assert-Rejected {Get-RebornOfficialMapFolders $data -OfficialMapDirectories @($paths[0],(Join-Path $root 'Missing'))} 'Missing selected map'
Assert-Rejected {Get-RebornOfficialMapFolders $data -OfficialMapDirectories @((Join-Path $FixtureSdk 'mods/Beta/Data/Maps/Official/BetaMap'))} 'Other mod refused'
Assert-Rejected {Get-RebornOfficialMapFolders $data -OfficialMapDirectories @((Join-Path $paths[0] 'Nested'))} 'Nested folder refused'
Assert-Rejected {Get-RebornOfficialMapFolders $data -OfficialMapDirectories @('CAMP_A01_BrightonBeach_Smith')} 'Relative path refused'
Assert-Rejected {Get-RebornOfficialMapFolders $data -OfficialMapDirectory $paths[0] -OfficialMapDirectories $paths} 'Conflicting selection inputs'
$empty=Join-Path $FixtureSdk 'empty-selection.txt';[IO.File]::WriteAllText($empty,'')
Assert-Rejected {Read-RebornMapSelection $empty} 'Empty payload refused'
Assert-Rejected {Read-RebornMapSelection (Join-Path $FixtureSdk 'not-found.txt')} 'Missing payload refused'
$missingSource=Join-Path $root 'NoSource';$null=New-Item -ItemType Directory -Path $missingSource
Assert-Rejected {Get-RebornOfficialMapFolders $data -OfficialMapDirectories @($paths[0],$missingSource)} 'Missing map.xml refused'
Write-Output ('Reborn: map selection backend PASS ('+$checks+' checks; no map compilation).')
