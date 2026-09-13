[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string] $Assembly,

    [Parameter(Mandatory = $true)]
    [string] $MethodToken,

    [Parameter(Mandatory = $true)]
    [string] $Schema,

    [Parameter(Mandatory = $true)]
    [string] $ComplexType
)

$ErrorActionPreference = 'Stop'
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$inspector = Join-Path $repositoryRoot 'source\BinaryAssetBuilder.ManifestInspector\bin\x86\Release\net8.0\BinaryAssetBuilder.ManifestInspector.exe'
if (-not (Test-Path -LiteralPath $inspector -PathType Leaf)) {
    throw "Build BinaryAssetBuilder.ManifestInspector first: $inspector"
}

$document = New-Object System.Xml.XmlDocument
$document.Load((Resolve-Path $Schema))
$namespaces = New-Object System.Xml.XmlNamespaceManager($document.NameTable)
$namespaces.AddNamespace('xs', 'http://www.w3.org/2001/XMLSchema')
$attributes = @($document.SelectNodes("//xs:complexType[@name='$ComplexType']//xs:attribute | //xs:complexType[@name='$ComplexType']//xs:element", $namespaces) |
    ForEach-Object { $_.GetAttribute('name') } |
    Where-Object { $_ })
$attributeSet = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
$attributes | ForEach-Object { [void] $attributeSet.Add($_) }

$lines = @(& $inspector assembly-il $Assembly $MethodToken)
$pending = $null
$awaitingOffset = $false
$rows = [Collections.Generic.List[object]]::new()
foreach ($line in $lines) {
    if ($line -match 'ldsflda\s+.*@([^@?]+)\?\$AA@') {
        $candidate = $Matches[1]
        if ($attributeSet.Contains($candidate)) {
            $pending = $candidate
            $awaitingOffset = $false
            continue
        }
    }
    if ($null -eq $pending) { continue }
    if ($line -match 'ldarg\.1\s*$') {
        $awaitingOffset = $true
        continue
    }
    if ($awaitingOffset -and $line -match 'ldc\.i4(?:\.s)?\s+(-?\d+)') {
        $rows.Add([pscustomobject]@{ Offset = [int]$Matches[1]; Attribute = $pending })
        $pending = $null
        $awaitingOffset = $false
        continue
    }
    if ($awaitingOffset -and $line -match 'ldc\.i4\.([0-8])\s*$') {
        $rows.Add([pscustomobject]@{ Offset = [int]$Matches[1]; Attribute = $pending })
        $pending = $null
        $awaitingOffset = $false
        continue
    }
    if ($awaitingOffset -and $line -match 'call\s+') {
        $rows.Add([pscustomobject]@{ Offset = 0; Attribute = $pending })
        $pending = $null
        $awaitingOffset = $false
    }
}

$rows | Sort-Object Offset, Attribute | Format-Table -AutoSize
$missing = @($attributes | Where-Object { $_ -notin $rows.Attribute })
if ($missing.Count -ne 0) {
    Write-Output "Unmapped XSD attributes: $($missing -join ', ')"
}
