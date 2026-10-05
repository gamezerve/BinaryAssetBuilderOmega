# Reborn: inspect explicit SDK environment metadata only; never build, discover via registry, clear caches or create output directories.
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('ra3ep1')]
    [string] $Target,
    [Parameter(Mandatory = $true)]
    [string] $SchemaRoot,
    [Parameter(Mandatory = $true)]
    [string] $SourceRoot,
    [Parameter(Mandatory = $true)]
    [string] $SourceEntry,
    [Parameter(Mandatory = $true)]
    [string] $OutputDirectory,
    [string[]] $ExternalMappings = @(),
    [string] $InspectorPath = (Join-Path (Split-Path -Parent $PSScriptRoot) 'source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe'),
    [switch] $AsJson
)

$ErrorActionPreference = 'Stop'
# Reborn: require an already built explicitly selected inspector; this wrapper does not restore/build dependencies or execute SDK batch scripts.
if (-not [IO.Path]::IsPathFullyQualified($InspectorPath) -or -not (Test-Path -LiteralPath $InspectorPath -PathType Leaf)) {
    throw 'An existing absolute inspector executable is required; build Release/x86 separately.'
}
# Reborn: pass path/mapping values as arguments, never evaluate them as shell expressions or discover substitutes from environment variables.
$preflightArguments = @('sdk-preflight', $Target, $SchemaRoot, $SourceRoot, $SourceEntry, $OutputDirectory) + $ExternalMappings
$preflightText = & $InspectorPath @preflightArguments
if ($LASTEXITCODE -ne 0) { throw "SDK environment preflight failed (exit $LASTEXITCODE); no SDK output was created." }
$preflightJson = $preflightText -join [Environment]::NewLine
# Reborn: read-only planning flags cannot be silently treated as a production build-ready result by the wrapper.
$preflightReport = $preflightJson | ConvertFrom-Json
if ($preflightReport.Target -cne 'ra3ep1' -or $preflightReport.ReadOnly -ne $true -or $preflightReport.SnapshotOnly -ne $true -or $preflightReport.ProductionBuildReady -ne $false) {
    throw 'Inspector result does not carry the expected read-only diagnostic planning contract.'
}
if ($AsJson) { $preflightJson } else { $preflightReport }
