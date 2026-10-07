# Reborn: classify first imported-Include blockers from an existing captured graph without reading XML, changing visibility or authorizing builds.
[CmdletBinding()]
param(
    [Parameter(Mandatory, ValueFromPipeline)] [object] $Report
)

process {
    # Reborn: reject partial/unbounded reports; this is graph evidence classification, not a fresh disk snapshot or compiler admission.
    $ErrorActionPreference = 'Stop'
    $paths = $Report.SourcePaths
    $documents = @($Report.TypedSources.Graph.Documents)
    if ($null -eq $paths -or $null -eq $Report.TypedSources.Graph -or $paths.StoppedAtLimit -or
        @($paths.Sources).Count -gt 512 -or @($paths.Includes).Count -gt 4096 -or $documents.Count -gt 512) {
        throw 'A complete bounded typed-source graph report is required.'
    }
    $sources = [Collections.Generic.Dictionary[string,object]]::new([StringComparer]::OrdinalIgnoreCase)
    $bindings = [Collections.Generic.Dictionary[string,object]]::new([StringComparer]::OrdinalIgnoreCase)
    foreach ($source in $paths.Sources) {
        if (!$sources.TryAdd($source.PhysicalPath, $source)) { throw 'Duplicate captured source identity.' }
    }
    foreach ($document in $documents) {
        if (!$sources.ContainsKey($document.SourcePath) -or !$bindings.TryAdd($document.SourcePath, $document)) {
            throw 'Missing or duplicate typed document identity.'
        }
    }
    $rows = [Collections.Generic.List[object]]::new()
    foreach ($document in $documents) {
        if (@($document.Inheritance.Diagnostics) -notcontains 'Imported base documents must be Include-free; transitive visibility remains closed.') { continue }
        # Reborn: mirror only the first Include-bearing direct source in captured declaration order; later sources and later gates remain unclassified.
        $first = $null
        $childEdges = @()
        foreach ($edge in @($paths.Includes | Where-Object { $_.Document -ieq $document.SourcePath })) {
            if ($null -eq $edge.PhysicalPath -or !$sources.ContainsKey($edge.PhysicalPath)) { throw 'Blocked owner has an uncaptured direct source.' }
            $candidate = @($paths.Includes | Where-Object { $_.Document -ieq $edge.PhysicalPath })
            if ($candidate.Count -gt 0) { $first = $edge.PhysicalPath; $childEdges = $candidate; break }
        }
        if ($null -eq $first -or !$bindings.ContainsKey($first)) { throw 'Include-free diagnostic has no captured Include-bearing imported source.' }
        $kinds = @($childEdges.Kind | Sort-Object -Unique)
        $rows.Add([pscustomobject]@{
            BlockedOwner = $document.SourcePath
            FirstImportedSource = $first
            ImportedRawSha256 = $sources[$first].Sha256
            ImportedStandaloneStatus = $bindings[$first].Status
            DirectIncludeCount = $childEdges.Count
            IncludeKinds = $kinds
            Classification = $(if ($kinds.Count -eq 1 -and $kinds[0] -ceq 'instance') { 'InstanceOnlyCandidate' } else { 'OtherVisibilityRequired' })
        })
    }
    # Reborn: candidate counts never assert schema/copy/expression/file-provenance readiness and never expose transitive assets as direct bases.
    [pscustomobject]@{
        ReadOnly = $true
        SnapshotOnly = $true
        ProductionBuildReady = $false
        BlockedOwnerCount = $rows.Count
        UniqueFirstImportedSources = @($rows.FirstImportedSource | Sort-Object -Unique).Count
        CandidateOwnerCount = @($rows | Where-Object Classification -eq 'InstanceOnlyCandidate').Count
        OtherVisibilityOwnerCount = @($rows | Where-Object Classification -eq 'OtherVisibilityRequired').Count
        Groups = @($rows | Group-Object FirstImportedSource | ForEach-Object {
            [pscustomobject]@{
                FirstImportedSource = $_.Name
                BlockedOwners = $_.Count
                ImportedStandaloneStatus = $_.Group[0].ImportedStandaloneStatus
                IncludeKinds = $_.Group[0].IncludeKinds
                Classification = $_.Group[0].Classification
                ImportedRawSha256 = $_.Group[0].ImportedRawSha256
            }
        } | Sort-Object -Property @{Expression='BlockedOwners';Descending=$true},FirstImportedSource)
        Owners = $rows.ToArray()
        Limitations = @('First-blocker/check-order evidence only, not exhaustive readiness.', 'Captured hashes are not reread or verified against current disk.', 'Instance-only candidates still require separate recursive preparation, direct-definition eligibility, closure witnesses and bounded regression tests.')
    }
}
