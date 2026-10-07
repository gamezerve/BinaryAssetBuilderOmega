# Reborn: owned in-memory classifier fixtures require neither official XML nor a compiler/native codec invocation.
$ErrorActionPreference = 'Stop'
$diagnostic = 'Imported base documents must be Include-free; transitive visibility remains closed.'
$report = [pscustomobject]@{
    SourcePaths = [pscustomobject]@{
        StoppedAtLimit = $false
        Sources = @('owner1','owner2','owner3','base1','base2','leaf') | ForEach-Object { [pscustomobject]@{ PhysicalPath = $_; Sha256 = "hash-$_" } }
        Includes = @(
            [pscustomobject]@{ Document='owner1'; Kind='instance'; PhysicalPath='base1' },
            [pscustomobject]@{ Document='owner2'; Kind='instance'; PhysicalPath='base1' },
            [pscustomobject]@{ Document='owner3'; Kind='instance'; PhysicalPath='base2' },
            [pscustomobject]@{ Document='base1'; Kind='instance'; PhysicalPath='leaf' },
            [pscustomobject]@{ Document='base2'; Kind='all'; PhysicalPath='leaf' }
        )
    }
    TypedSources = [pscustomobject]@{
        Graph = [pscustomobject]@{
            Documents = @('owner1','owner2','owner3','base1','base2','leaf') | ForEach-Object {
                [pscustomobject]@{
                    SourcePath = $_
                    Status = $(if ($_ -like 'owner*') { 'RequiresPreprocessing' } else { 'Validated' })
                    Inheritance = [pscustomobject]@{ Diagnostics = $(if ($_ -like 'owner*') { @($diagnostic) } else { @() }) }
                }
            }
        }
    }
}
# Reborn: shared bases coalesce only in the summary; each blocked owner and captured source hash remains explicit.
$result = $report | & "$PSScriptRoot/Get-Ra3Ep1InstanceIncludeBlockers.ps1"
if ($result.BlockedOwnerCount -ne 3 -or $result.UniqueFirstImportedSources -ne 2 -or
    $result.CandidateOwnerCount -ne 2 -or $result.OtherVisibilityOwnerCount -ne 1 -or
    $result.Groups[0].BlockedOwners -ne 2 -or $result.Groups[0].ImportedRawSha256 -ne 'hash-base1' -or
    !$result.ReadOnly -or !$result.SnapshotOnly -or $result.ProductionBuildReady) { throw 'Classification fixture failed.' }
# Reborn: partial inventories and contradictory first-blocker evidence must fail closed rather than produce candidate readiness.
$report.SourcePaths.StoppedAtLimit = $true
$refused = $false
try { $null = $report | & "$PSScriptRoot/Get-Ra3Ep1InstanceIncludeBlockers.ps1" } catch { $refused = $true }
if (!$refused) { throw 'Partial graph was accepted.' }
$report.SourcePaths.StoppedAtLimit = $false
$report.SourcePaths.Includes = @($report.SourcePaths.Includes | Where-Object Document -ne 'base1')
$refused = $false
try { $null = $report | & "$PSScriptRoot/Get-Ra3Ep1InstanceIncludeBlockers.ps1" } catch { $refused = $true }
if (!$refused) { throw 'Contradictory diagnostic was accepted.' }
'Instance Include blocker classifier: 3 fixtures passed.'
