[CmdletBinding()]
param(
    [string] $Ra3SchemaRoot = (Join-Path (Split-Path -Parent $PSScriptRoot) 'schemas\ra3\xsd'),
    [string] $Ep1SchemaRoot = (Join-Path (Split-Path -Parent $PSScriptRoot) 'schemas\ra3ep1\xsd'),
    [switch] $Json
)

$ErrorActionPreference = 'Stop'
$repositoryRoot = Split-Path -Parent $PSScriptRoot

function Get-XsdCatalog([string] $root) {
    $resolvedRoot = (Resolve-Path -LiteralPath $root).Path
    $result = [Collections.Generic.List[object]]::new()
    foreach ($file in Get-ChildItem -LiteralPath $resolvedRoot -Recurse -Filter '*.xsd') {
        $document = [Xml.XmlDocument]::new()
        $document.Load($file.FullName)
        $namespaces = [Xml.XmlNamespaceManager]::new($document.NameTable)
        $namespaces.AddNamespace('xs', 'http://www.w3.org/2001/XMLSchema')
        foreach ($kind in @('complexType', 'simpleType')) {
            foreach ($node in $document.SelectNodes("//xs:$kind[@name]", $namespaces)) {
                $result.Add([pscustomobject]@{
                    Name = $node.GetAttribute('name')
                    Kind = $kind
                    File = $file.FullName.Substring($resolvedRoot.Length + 1)
                })
            }
        }
    }
    return $result
}

function Get-RegexNames([string] $directory, [string] $filter, [string] $pattern) {
    $names = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    foreach ($file in Get-ChildItem -LiteralPath $directory -Recurse -Filter $filter) {
        $source = [IO.File]::ReadAllText($file.FullName)
        foreach ($match in [Regex]::Matches($source, $pattern, [Text.RegularExpressions.RegexOptions]::Multiline)) {
            [void] $names.Add($match.Groups[1].Value)
        }
    }
    return $names
}

$ra3 = @(Get-XsdCatalog $Ra3SchemaRoot)
$ep1 = @(Get-XsdCatalog $Ep1SchemaRoot)
$modelNames = Get-RegexNames `
    (Join-Path $repositoryRoot 'source\SageBinaryData\SageBinaryData') `
    '*.cs' `
    '^\s*public\s+(?:unsafe\s+)?(?:struct|enum)\s+([A-Za-z_][A-Za-z0-9_]*)'
$marshallerNames = Get-RegexNames `
    (Join-Path $repositoryRoot 'source\BinaryAssetBuilder.XmlCompiler') `
    'Marshaler*.cs' `
    'Marshal\s*\(Node\s+node,\s*([A-Za-z_][A-Za-z0-9_]*)\s*\*+'

$ra3Complex = @($ra3 | Where-Object Kind -eq 'complexType' | Select-Object -ExpandProperty Name | Sort-Object -Unique)
$ep1Complex = @($ep1 | Where-Object Kind -eq 'complexType' | Select-Object -ExpandProperty Name | Sort-Object -Unique)
$ep1Simple = @($ep1 | Where-Object Kind -eq 'simpleType' | Select-Object -ExpandProperty Name | Sort-Object -Unique)
$ra3ComplexSet = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
foreach ($name in $ra3Complex) {
    [void] $ra3ComplexSet.Add($name)
}
$ep1Only = @($ep1Complex | Where-Object { -not $ra3ComplexSet.Contains($_) } | Sort-Object)

$ep1OnlyDetails = @($ep1Only | ForEach-Object {
    [pscustomobject]@{
        Name = $_
        Model = $modelNames.Contains($_)
        Marshaller = $marshallerNames.Contains($_)
    }
})

$summary = [ordered]@{
    Ep1XsdFiles = @(Get-ChildItem -LiteralPath $Ep1SchemaRoot -Recurse -Filter '*.xsd').Count
    Ep1ComplexTypes = $ep1Complex.Count
    Ep1SimpleTypes = $ep1Simple.Count
    Ep1ComplexModelCoverage = @($ep1Complex | Where-Object { $modelNames.Contains($_) }).Count
    Ep1ComplexMarshallerCoverage = @($ep1Complex | Where-Object { $marshallerNames.Contains($_) }).Count
    Ep1OnlyComplexTypes = $ep1Only.Count
    Ep1OnlyModelCoverage = @($ep1OnlyDetails | Where-Object Model).Count
    Ep1OnlyMarshallerCoverage = @($ep1OnlyDetails | Where-Object Marshaller).Count
}
$summary.Ep1ComplexModelCoveragePercent = [Math]::Round(
    100.0 * $summary.Ep1ComplexModelCoverage / $summary.Ep1ComplexTypes, 1)
$summary.Ep1ComplexMarshallerCoveragePercent = [Math]::Round(
    100.0 * $summary.Ep1ComplexMarshallerCoverage / $summary.Ep1ComplexTypes, 1)
$summary.Ep1OnlyModelCoveragePercent = [Math]::Round(
    100.0 * $summary.Ep1OnlyModelCoverage / $summary.Ep1OnlyComplexTypes, 1)
$summary.Ep1OnlyMarshallerCoveragePercent = [Math]::Round(
    100.0 * $summary.Ep1OnlyMarshallerCoverage / $summary.Ep1OnlyComplexTypes, 1)

$result = [pscustomobject]@{
    Summary = [pscustomobject]$summary
    Ep1OnlyTypes = $ep1OnlyDetails
}

if ($Json) {
    $result | ConvertTo-Json -Depth 4
    return
}

$summary.GetEnumerator() | ForEach-Object { '{0,-32} {1,6}' -f $_.Key, $_.Value }
''
'EP1-only complex types:'
$ep1OnlyDetails | ForEach-Object {
    '  {0,-55} model={1,-5} marshaller={2,-5}' -f $_.Name, $_.Model, $_.Marshaller
}
