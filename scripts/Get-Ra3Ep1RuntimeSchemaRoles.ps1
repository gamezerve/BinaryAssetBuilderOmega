# Reborn: classify pinned runtime names by literal staged XSD declarations, not inferred processor registration or native layout readiness.
[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)] [string] $ImagePath,
    [string] $SchemaRoot=(Join-Path (Split-Path -Parent $PSScriptRoot) 'schemas\ra3ep1\xsd'),
    [switch] $SelfTest,
    [switch] $AsJson
)
$ErrorActionPreference='Stop'
# Reborn: preserve wrapper flags while importing the read-only tool's scoped snapshot helper and authentic runtime report.
$roleSelfTest=$SelfTest; $roleAsJson=$AsJson

#-------------------------------------------------------------------------------------------------
<# Reborn: capture a bounded read-only XSD catalog with external XML resolution disabled and explicit regular-file provenance. #>
#-------------------------------------------------------------------------------------------------
function Read-RoleCatalog([string] $Root) {
    if (-not [IO.Path]::IsPathFullyQualified($Root)) { throw 'Schema root must be absolute.' }
    $resolved=(Get-Item -LiteralPath $Root -Force).FullName
    $files=@(Get-ChildItem -LiteralPath $resolved -Recurse -File -Filter '*.xsd' | Sort-Object FullName)
    if ($files.Count -gt 1024 -or $files.Count -eq 0) { throw 'Schema file count exceeds bounds or is empty.' }
    $result=[Collections.Generic.List[object]]::new()
    $total=0L
    foreach($file in $files) {
        # Reborn: reuse the pinned table tool's bounded regular-file snapshot helper, imported only into this script scope.
        $bytes=Read-RuntimeInput $file.FullName 2097152
        $total+=$bytes.Length
        if ($total -gt 67108864) { throw 'Schema catalog exceeds 64 MiB.' }
        $result.Add([pscustomobject]@{ File=[IO.Path]::GetRelativePath($resolved,$file.FullName).Replace('\','/'); Bytes=$bytes; Sha256=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($bytes)) })
    }
    return ,$result.ToArray()
}

#-------------------------------------------------------------------------------------------------
<# Reborn: enumerate global named complex types and only direct typed AssetDeclaration choice elements; do not promote inheritance or suffixes to authored-root authority. #>
#-------------------------------------------------------------------------------------------------
function Get-RoleDeclarations([object[]] $Catalog) {
    $complex=[Collections.Generic.Dictionary[string,object]]::new([StringComparer]::Ordinal)
    $roots=[Collections.Generic.Dictionary[string,object]]::new([StringComparer]::Ordinal)
    foreach($file in $Catalog) {
        $settings=[Xml.XmlReaderSettings]::new(); $settings.DtdProcessing=[Xml.DtdProcessing]::Prohibit; $settings.XmlResolver=$null
        $settings.MaxCharactersInDocument=2097152
        $stream=[IO.MemoryStream]::new($file.Bytes,$false); $reader=[Xml.XmlReader]::Create($stream,$settings)
        try { $doc=[Xml.XmlDocument]::new(); $doc.XmlResolver=$null; $doc.Load($reader) } finally { $reader.Dispose(); $stream.Dispose() }
        $ns=[Xml.XmlNamespaceManager]::new($doc.NameTable); $ns.AddNamespace('xs','http://www.w3.org/2001/XMLSchema')
        $targetNamespace=$doc.DocumentElement.GetAttribute('targetNamespace')
        if ($doc.DocumentElement.LocalName -cne 'schema' -or $targetNamespace -cnotin @('uri:ea.com:eala:asset','uri:ea.com:eala:asset:gamedata')) { throw 'Unexpected schema namespace.' }
        foreach($node in $doc.SelectNodes('/xs:schema/xs:complexType[@name]',$ns)) {
            $name=$node.GetAttribute('name')
            # Reborn: retain every duplicate declaration as evidence; never silently choose a schema-normalization winner.
            if (-not $complex.ContainsKey($name)) { $complex.Add($name,[Collections.Generic.List[object]]::new()) }
            $base=$node.SelectSingleNode('xs:complexContent/xs:extension|xs:complexContent/xs:restriction',$ns)
            $complex[$name].Add([pscustomobject]@{ File=$file.File; Namespace=$targetNamespace; Abstract=($node.GetAttribute('abstract') -in @('true','1')); Base=$(if($base){$base.GetAttribute('base')}else{''}); TypeGroup=$node.GetAttribute('typeGroup','uri:ea.com:eala:asset:schema'); RuntimeWrapper=$node.GetAttribute('runtimeWrapper','uri:ea.com:eala:asset:schema') })
        }
        foreach($node in $doc.SelectNodes('/xs:schema/xs:element[@name="AssetDeclaration"]/xs:complexType/xs:sequence/xs:choice/xs:element[@type]',$ns)) {
            $type=$node.GetAttribute('type')
            if ($type.Contains(':')) { throw 'Prefixed direct root type needs a separately reviewed namespace policy.' }
            if ($roots.ContainsKey($type)) { throw ('Duplicate direct asset type: '+$type) }
            $roots.Add($type,[pscustomobject]@{ Element=$node.GetAttribute('name'); File=$file.File })
        }
    }
    foreach($name in $roots.Keys) { if (-not $complex.ContainsKey($name)) { throw ('Direct root lacks a complex declaration: '+$name) } }
    return [pscustomobject]@{ Complex=$complex; Roots=$roots }
}

# Reborn: dot-source the reviewed read-only tool once, capture its report and restore wrapper flags; no target is executed.
$runtime=. (Join-Path $PSScriptRoot 'Get-Ra3Ep1RuntimeTypeTable.ps1') -ImagePath $ImagePath
$SelfTest=$roleSelfTest; $AsJson=$roleAsJson
$catalog=Read-RoleCatalog $SchemaRoot
$declarations=Get-RoleDeclarations $catalog
# Reborn: stock role flags come from the same pinned bounded evidence bytes, not a second unchecked text read.
$stockBytes=Read-RuntimeInput (Join-Path (Split-Path -Parent $PSScriptRoot) 'docs\RA3EP1_TYPE_TABLE_EVIDENCE.json') 2097152
if ([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($stockBytes)) -cne $runtime.EvidenceSha256) { throw 'Stock role evidence changed.' }
$stock=([Text.Encoding]::UTF8.GetString($stockBytes) | ConvertFrom-Json).Types
$stockNames=[Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
foreach($type in $stock) { [void]$stockNames.Add($type.TypeName) }
$runtimeNames=[Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
$rows=@(foreach($row in $runtime.Rows) {
    [void]$runtimeNames.Add($row.Name)
    if (-not $declarations.Complex.ContainsKey($row.Name)) { throw ('Runtime name has no schema complex declaration: '+$row.Name) }
    $declaration=$declarations.Complex[$row.Name].ToArray()
    if (@($declaration | Where-Object Namespace -cne 'uri:ea.com:eala:asset').Count -gt 0) { throw 'Runtime name has a foreign schema namespace collision.' }
    $direct=$declarations.Roots.ContainsKey($row.Name)
    [pscustomobject]@{ Name=$row.Name; TypeHash=$row.TypeHash; Role=$(if($direct){'direct-asset-choice'}else{'complex-not-direct-choice'}); ObservedStockRoot=$stockNames.Contains($row.Name); Declarations=$declaration; RootElement=$(if($direct){$declarations.Roots[$row.Name].Element}else{$null}); RuntimeSuffix=$row.Name.EndsWith('Runtime',[StringComparison]::Ordinal) }
})
$schemaOnly=@($declarations.Complex.Keys | Where-Object {-not $runtimeNames.Contains($_)} | Sort-Object | ForEach-Object { [pscustomobject]@{ Name=$_; Declarations=$declarations.Complex[$_].ToArray() } })
if ($SelfTest) {
    # Reborn: direct-choice classification excludes nested elements and retains duplicate declarations instead of selecting a winner.
    $positive='<xs:schema xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="uri:ea.com:eala:asset"><xs:complexType name="RebornRole"/><xs:complexType name="RebornRole"/><xs:complexType name="RebornNested"><xs:sequence><xs:element name="FakeRoot" type="RebornRole"/></xs:sequence></xs:complexType><xs:element name="AssetDeclaration"><xs:complexType><xs:sequence><xs:choice><xs:element name="RealRoot" type="RebornRole"/></xs:choice></xs:sequence></xs:complexType></xs:element></xs:schema>'
    $positiveResult=Get-RoleDeclarations @([pscustomobject]@{File='owned-positive.xsd';Bytes=[Text.Encoding]::UTF8.GetBytes($positive)})
    if ($positiveResult.Complex.Count -ne 2 -or $positiveResult.Complex['RebornRole'].Count -ne 2 -or
        $positiveResult.Roots.Count -ne 1 -or $positiveResult.Roots['RebornRole'].Element -cne 'RealRoot') { throw 'Positive role declaration contract differs.' }
    # Reborn: malformed catalogs are private in-memory XML fixtures, not edits to official/staged schemas.
    foreach($case in @('dtd','namespace','unknown-root','prefix')) {
        $xml='<xs:schema xmlns:xs="http://www.w3.org/2001/XMLSchema" targetNamespace="uri:ea.com:eala:asset"><xs:complexType name="RebornRole"/>'
        switch($case) {
            'dtd' { $xml='<!DOCTYPE schema [<!ENTITY probe "Reborn">]>'+$xml }
            'namespace' { $xml=$xml.Replace('uri:ea.com:eala:asset','urn:RebornUnknown') }
            'unknown-root' { $xml+='<xs:element name="AssetDeclaration"><xs:complexType><xs:sequence><xs:choice><xs:element name="Probe" type="Unknown"/></xs:choice></xs:sequence></xs:complexType></xs:element>' }
            'prefix' { $xml+='<xs:element name="AssetDeclaration"><xs:complexType><xs:sequence><xs:choice><xs:element name="Probe" type="xs:string"/></xs:choice></xs:sequence></xs:complexType></xs:element>' }
        }
        $xml+='</xs:schema>'; $rejected=$false
        try { $null=Get-RoleDeclarations @([pscustomobject]@{File='owned.xsd';Bytes=[Text.Encoding]::UTF8.GetBytes($xml)}) } catch { $rejected=$true }
        if (-not $rejected) { throw ('Role fault admitted: '+$case) }
    }
}
# Reborn: compare raw catalog provenance after classification; this is snapshot observation, not atomic schema/game compatibility admission.
$canonical=($catalog | ForEach-Object { $_.File+"`t"+$_.Sha256+"`n" }) -join ''
$after=Read-RoleCatalog $SchemaRoot
$afterCanonical=($after | ForEach-Object { $_.File+"`t"+$_.Sha256+"`n" }) -join ''
if ($canonical -cne $afterCanonical) { throw 'Schema catalog changed during classification.' }
$report=[pscustomobject]@{
    ImageSha256=$runtime.ImageSha256; RuntimeNameHashSha256=$runtime.OrderedNameHashSha256
    SchemaCatalogSha256=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($canonical)))
    SchemaFiles=$catalog.Count; SchemaComplexTypes=$declarations.Complex.Count; RuntimeRows=$rows.Count
    DuplicateComplexNames=@($declarations.Complex.Keys | Where-Object { $declarations.Complex[$_].Count -gt 1 } | Sort-Object)
    DirectChoiceRuntimeTypes=@($rows|Where-Object Role -eq 'direct-asset-choice').Count; NonDirectComplexRuntimeTypes=@($rows|Where-Object Role -eq 'complex-not-direct-choice').Count
    ObservedStockRoots=$stockNames.Count; ObservedNonDirectRoots=@($rows|Where-Object { $_.ObservedStockRoot -and $_.Role -ne 'direct-asset-choice' }).Count
    SchemaOnlyComplexTypes=$schemaOnly.Count; ReadOnly=$true; SchemaCompiled=$false; InheritanceFlattened=$false; RuntimeOnlyProved=$false; ProductionBuildReady=$false; FaultTestsPassed=[bool]$SelfTest; FaultCasesExecuted=$(if($SelfTest){4}else{0})
    Rows=$rows; SchemaOnly=$schemaOnly
}
if ($AsJson) { ConvertTo-Json -InputObject $report -Depth 8 } else { $report }
