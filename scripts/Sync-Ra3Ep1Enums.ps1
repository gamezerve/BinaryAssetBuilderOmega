[CmdletBinding()]
param(
    [string] $SchemaRoot = (Join-Path (Split-Path -Parent $PSScriptRoot) 'schemas\ra3ep1\xsd'),
    [switch] $Check
)

$ErrorActionPreference = 'Stop'
$repositoryRoot = Split-Path -Parent $PSScriptRoot

$targets = @(
    @{ Source = 'source\SageBinaryData\SageBinaryData\Includes\ModelState.cs'; Schema = 'Includes\ModelState.xsd'; Type = 'ModelConditionFlagType'; InvalidIsMinusOne = $true },
    @{ Source = 'source\SageBinaryData\SageBinaryData\Includes\ObjectStatus.cs'; Schema = 'Includes\ObjectStatus.xsd'; Type = 'ObjectStatusType' },
    @{ Source = 'source\SageBinaryData\SageBinaryData\Includes\GlobalGameData.cs'; Schema = 'Includes\GlobalGameData.xsd'; Type = 'DisabledType' },
    @{ Source = 'source\SageBinaryData\SageBinaryData\ArmorTemplate.cs'; Schema = 'AssetTypeArmorTemplate.xsd'; Type = 'ArmorSetType' },
    @{ Source = 'source\SageBinaryData\SageBinaryData\AttributeModifier.cs'; Schema = 'AssetTypeAttributeModifier.xsd'; Type = 'AttributeModifierCategoryType' },
    @{ Source = 'source\SageBinaryData\SageBinaryData\AttributeModifier.cs'; Schema = 'AssetTypeAttributeModifier.xsd'; Type = 'AttributeType' },
    @{ Source = 'source\SageBinaryData\SageBinaryData\LocomotorTemplate.cs'; Schema = 'AssetTypeLocomotorTemplate.xsd'; Type = 'Appearance' },
    @{ Source = 'source\SageBinaryData\SageBinaryData\LocomotorTemplate.cs'; Schema = 'AssetTypeLocomotorTemplate.xsd'; Type = 'Surface' },
    @{ Source = 'source\SageBinaryData\SageBinaryData\LocomotorTemplate.cs'; Schema = 'AssetTypeLocomotorTemplate.xsd'; Type = 'LocoZ' },
    @{ Source = 'source\SageBinaryData\SageBinaryData\LocomotorTemplate.cs'; Schema = 'AssetTypeLocomotorTemplate.xsd'; Type = 'LocoF' },
    @{ Source = 'source\SageBinaryData\SageBinaryData\LocomotorTemplate.cs'; Schema = 'AssetTypeLocomotorTemplate.xsd'; Type = 'JetLocomotorDataOption' },
    @{ Source = 'source\SageBinaryData\SageBinaryData\LocomotorTemplate.cs'; Schema = 'AssetTypeTerrainAsset.xsd'; Type = 'TerrainClassType' },
    @{ Source = 'source\SageBinaryData\SageBinaryData\WeaponTemplate.cs'; Schema = 'AssetTypeWeaponTemplate.xsd'; Type = 'WeaponFlagsType' },
    @{ Source = 'source\SageBinaryData\SageBinaryData\WeaponTemplate.cs'; Schema = 'AssetTypeWeaponTemplate.xsd'; Type = 'WeaponReloadType' },
    @{ Source = 'source\SageBinaryData\SageBinaryData\WeaponTemplate.cs'; Schema = 'AssetTypeWeaponTemplate.xsd'; Type = 'WeaponPrefireType' },
    @{ Source = 'source\SageBinaryData\SageBinaryData\WeaponTemplate.cs'; Schema = 'AssetTypeWeaponTemplate.xsd'; Type = 'WeaponReAcquireDetailType' },
    @{ Source = 'source\SageBinaryData\SageBinaryData\WeaponTemplate.cs'; Schema = 'AssetTypeWeaponTemplate.xsd'; Type = 'WeaponCollideType' },
    @{ Source = 'source\SageBinaryData\SageBinaryData\WeaponTemplate.cs'; Schema = 'AssetTypeWeaponTemplate.xsd'; Type = 'WeaponAffectsType' },
    @{ Source = 'source\SageBinaryData\SageBinaryData\WeaponTemplate.cs'; Schema = 'AssetTypeWeaponTemplate.xsd'; Type = 'WpnAntiT' },
    @{ Source = 'source\SageBinaryData\SageBinaryData\WeaponTemplate.cs'; Schema = 'AssetTypeWeaponTemplate.xsd'; Type = 'ParalyzeEffectType' },
    @{ Source = 'source\SageBinaryData\SageBinaryData\WeaponTemplate.cs'; Schema = 'AssetTypeWeaponTemplate.xsd'; Type = 'VirtualDamageType' }
)

foreach ($target in $targets) {
    $schemaPath = Join-Path $SchemaRoot $target.Schema
    $document = New-Object System.Xml.XmlDocument
    $document.Load($schemaPath)
    $namespaces = New-Object System.Xml.XmlNamespaceManager($document.NameTable)
    $namespaces.AddNamespace('xs', 'http://www.w3.org/2001/XMLSchema')
    $nodes = $document.SelectNodes("//xs:simpleType[@name='$($target.Type)']//xs:enumeration", $namespaces)
    if ($nodes.Count -eq 0) {
        throw "No values found for $($target.Type) in $schemaPath"
    }

    $values = @($nodes | ForEach-Object { $_.GetAttribute('value') })
    foreach ($value in $values) {
        if ($value -notmatch '^[A-Za-z_][A-Za-z0-9_]*$') {
            throw "XSD value '$value' cannot be emitted as a C# enum member."
        }
    }

    $members = for ($index = 0; $index -lt $values.Count; $index++) {
        $suffix = if ($index -eq $values.Count - 1) { '' } else { ',' }
        $assignment = if ($index -eq 0 -and $target.InvalidIsMinusOne) { ' = -1' } else { '' }
        "    $($values[$index])$assignment$suffix"
    }
    $replacement = "public enum $($target.Type)`r`n{`r`n$($members -join "`r`n")`r`n}"

    $sourcePath = Join-Path $repositoryRoot $target.Source
    $source = [IO.File]::ReadAllText($sourcePath)
    $pattern = "(?s)public enum $([Regex]::Escape($target.Type))\s*\{.*?\r?\n\}"
    $updated = [Regex]::Replace($source, $pattern, $replacement, 1)
    if ($updated -eq $source) {
        Write-Output "$($target.Type): verified ($($values.Count) values)"
        continue
    }
    if ($Check) {
        throw "$($target.Type) does not match $schemaPath. Run scripts\Sync-Ra3Ep1Enums.ps1."
    }
    [IO.File]::WriteAllText($sourcePath, $updated, [Text.UTF8Encoding]::new($false))
    Write-Output "$($target.Type): $($values.Count) values"
}
