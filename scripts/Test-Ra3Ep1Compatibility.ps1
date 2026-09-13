[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidateScript({ Test-Path -LiteralPath $_ -PathType Leaf })]
    [string] $Ra3WorldBuilderManifest,

    [Parameter(Mandatory = $true)]
    [ValidateScript({ Test-Path -LiteralPath $_ -PathType Container })]
    [string] $UprisingDataDirectory
)

$ErrorActionPreference = 'Stop'
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$project = Join-Path $repositoryRoot 'source\BinaryAssetBuilder.ManifestInspector'

& (Join-Path $PSScriptRoot 'Sync-Ra3Ep1Enums.ps1') -Check

dotnet build $project -c Release --no-restore -p:Platform=x86
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$writerFixture = Join-Path $repositoryRoot 'source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\ep1-writer-smoke.manifest'
dotnet run --project $project -c Release --no-build -p:Platform=x86 -- writer-self-test $writerFixture
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

dotnet run --project $project -c Release --no-build -p:Platform=x86 -- layout-self-test
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

dotnet run --project $project -c Release --no-build -p:Platform=x86 -- compiler-self-test
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

dotnet run --project $project -c Release --no-build -p:Platform=x86 -- verify $Ra3WorldBuilderManifest
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$fixtures = @(
    @{ Archive = 'WBData.big'; Entry = 'data\worldbuilder.manifest' },
    @{ Archive = 'StaticStream.big'; Entry = 'data\static.manifest' },
    @{ Archive = 'GlobalStream.big'; Entry = 'data\global.manifest' }
)

foreach ($fixture in $fixtures) {
    $archive = Join-Path $UprisingDataDirectory $fixture.Archive
    if (-not (Test-Path -LiteralPath $archive -PathType Leaf)) {
        throw "Required Uprising fixture was not found: $archive"
    }

    dotnet run --project $project -c Release --no-build -p:Platform=x86 -- verify $archive --entry $fixture.Entry
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    dotnet run --project $project -c Release --no-build -p:Platform=x86 -- utility-verify $archive --entry $fixture.Entry
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}

dotnet run --project $project -c Release --no-build -p:Platform=x86 -- schema-diff `
    (Join-Path $repositoryRoot 'schemas\ra3\xsd') `
    (Join-Path $repositoryRoot 'schemas\ra3ep1\xsd')
exit $LASTEXITCODE
