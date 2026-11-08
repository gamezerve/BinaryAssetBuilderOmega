# Reborn: verify source-backed direct-instance ordering without compiling or changing any user mod.
[CmdletBinding()]
param([Parameter(Mandatory)][string]$Repository,[Parameter(Mandatory)][string]$OutputDirectory)
$ErrorActionPreference='Stop'
$sdk=Join-Path $Repository 'alpha';$exe=Join-Path $sdk 'tools/compiler/BinaryAssetBuilder.ManifestInspector.exe'
$OutputDirectory=[IO.Path]::GetFullPath($OutputDirectory)
if(!$OutputDirectory.StartsWith([IO.Path]::GetFullPath((Join-Path $sdk '.work'))+'\',[StringComparison]::OrdinalIgnoreCase)-or(Test-Path -LiteralPath $OutputDirectory)){throw 'Fresh private SDK workspace required.'}
$data=Join-Path $OutputDirectory 'input/Data';$null=New-Item -ItemType Directory -Path $data
$ns='uri:ea.com:eala:asset'
[IO.File]::WriteAllText((Join-Path $data 'first.xml'),('<AssetDeclaration xmlns="'+$ns+'"><ArmorTemplate id="Shared" Default="20"/></AssetDeclaration>'))
[IO.File]::WriteAllText((Join-Path $data 'last.xml'),('<AssetDeclaration xmlns="'+$ns+'"><ArmorTemplate id="sHARED" Default="30"/></AssetDeclaration>'))
$include='<Includes><Include type="instance" source="first.xml"/><Include type="instance" source="last.xml"/></Includes>'
#-------------------------------------------------------------------------------------------------
<# Reborn: run a real preparation request and require its expected exit status. #>
#-------------------------------------------------------------------------------------------------
function Prepare-Case([string]$Name,[string]$Body,[bool]$Accepted){
    $entry=Join-Path $data ($Name+'.xml');[IO.File]::WriteAllText($entry,('<AssetDeclaration xmlns="'+$ns+'">'+$include+$Body+'</AssetDeclaration>'))
    & $exe conscript-source-prepare $entry (Join-Path $OutputDirectory $Name) $sdk (Join-Path $sdk 'SageXML') *> (Join-Path $OutputDirectory ($Name+'.log'))
    if(($LASTEXITCODE-eq 0)-ne $Accepted){throw ('Unexpected preparation result: '+$Name)}
}
Prepare-Case 'selected' '<ArmorTemplate id="Probe" inheritFrom="Shared"/>' $true
[xml]$selected=Get-Content -LiteralPath (Join-Path $OutputDirectory 'selected/game.xml') -Raw
if($selected.DocumentElement.GetAttribute('Default')-ne '30'){throw 'Later direct instance did not supply the inherited fields.'}
Prepare-Case 'unused' '<ArmorTemplate id="Probe" Default="10"/>' $true
Prepare-Case 'duplicate-owner' '<ArmorTemplate id="Probe"/><ArmorTemplate id="Probe"/>' $false
if((Get-Content -LiteralPath (Join-Path $OutputDirectory 'duplicate-owner.log') -Raw)-notmatch 'DUPLICATEASSET|Duplicate|duplicate'){throw 'Duplicate-owner rejection lost its identity diagnostic.'}
[IO.File]::WriteAllText((Join-Path $data 'last.xml'),('<AssetDeclaration xmlns="'+$ns+'"><ArmorTemplate id="Shared"/><ArmorTemplate id="Shared"/></AssetDeclaration>'))
Prepare-Case 'duplicate-import' '<ArmorTemplate id="Probe" inheritFrom="Shared"/>' $false
Write-Output 'Reborn: direct instance source ordering: PASS (later source selected, unused candidates accepted, duplicate owner/import declarations rejected).'
