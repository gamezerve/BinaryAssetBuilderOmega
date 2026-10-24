# Reborn: exercise W3X capture, dependency audit and real support-stream/BIG readback in a private generation, never the user's mod output.
[CmdletBinding()]
param([Parameter(Mandatory)][string]$OutputDirectory,[Parameter(Mandatory)][string]$StaticManifest,[Parameter(Mandatory)][string]$GlobalManifest)
$ErrorActionPreference='Stop';$repo=Split-Path -Parent $PSScriptRoot;$sdk=Join-Path $repo 'alpha';$exe=Join-Path $repo 'source/BinaryAssetBuilder.ManifestInspector/bin/x86/Release/net8.0/BinaryAssetBuilder.ManifestInspector.exe'
if(Test-Path -LiteralPath $OutputDirectory){throw 'A fresh test output directory is required.'}
$OutputDirectory=[IO.Path]::GetFullPath($OutputDirectory);$mod=Join-Path $OutputDirectory 'mod';$data=Join-Path $mod 'Data';$art=Join-Path $mod 'Art/Re';$null=New-Item -ItemType Directory -Path $data,$art
$source=Join-Path $art 'RebornContainer.w3x';$entry=Join-Path $data 'Static_Mod.xml';$prepared=Join-Path $OutputDirectory 'prepared'
# Reborn: explicit new containers reference existing WorldBuilder model parts; one empty container covers the optional hierarchy/list defaults.
$w3x=@'
<!-- Reborn: independently authored container fixtures; original mesh/hierarchy/collision payloads stay external. -->
<AssetDeclaration xmlns="uri:ea.com:eala:asset">
  <W3DContainer id="RebornPortContainer" Hierarchy="VL_RussianPortBuilding">
    <SubObject BoneIndex="0" SubObjectID="REBORNSKIN"><RenderObject><Mesh>VL_RUSSIANPORTBUILDING.NEWSKIN_FILL01</Mesh></RenderObject></SubObject>
    <SubObject BoneIndex="2"><RenderObject><CollisionBox>VL_RUSSIANPORTBUILDING.OBBOX02</CollisionBox></RenderObject></SubObject>
  </W3DContainer>
  <W3DContainer id="RebornEmptyContainer"/>
</AssetDeclaration>
'@
[IO.File]::WriteAllText($source,$w3x)
[IO.File]::WriteAllText($entry,'<!-- Reborn: repeated all imports emit shared W3X assets only once. --><AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes><Include type="all" source="ART:RebornContainer.w3x"/><Include type="all" source="ART:RebornContainer.w3x"/></Includes></AssetDeclaration>')
& $exe conscript-source-prepare $entry $prepared $sdk *> (Join-Path $OutputDirectory 'prepare.log');if($LASTEXITCODE-ne 0){throw 'W3X source preparation failed; see prepare.log.'}
$input=Get-Content -LiteralPath (Join-Path $prepared 'source-input.json') -Raw|ConvertFrom-Json
if($input.AssetCount-ne 2-or @($input.Sources).Count-ne 2-or $input.PrimaryType-cne 'W3DContainer'-or $input.Origins[0].Path-cne $source){throw 'W3X capture/emission/source provenance differs.'}
& $exe sdk-missing-dependencies $entry $sdk $StaticManifest $GlobalManifest (Join-Path $OutputDirectory 'dependencies.json') *> (Join-Path $OutputDirectory 'audit.log');if($LASTEXITCODE-ne 0){throw 'W3X dependency audit failed; see audit.log.'}
& $exe conscript-alpha-build (Join-Path $sdk 'SageXML') $StaticManifest $GlobalManifest (Join-Path $prepared 'game.xml') (Join-Path $OutputDirectory 'compiled') *> (Join-Path $OutputDirectory 'compile.log');if($LASTEXITCODE-ne 0){throw 'W3DContainer native/stream/BIG readback failed; see compile.log.'}
$receipt=Get-Content -LiteralPath (Join-Path $OutputDirectory 'compiled/support/trial-plan.json') -Raw|ConvertFrom-Json
if($receipt.AssetCount-ne 2-or -not $receipt.OfflineValidated-or $receipt.ArchiveEntryCount-ne 15-or $receipt.GameExecuted){throw 'Container support-stream receipt differs.'}
# Reborn: a source owner using inheritance may all-include a W3X alongside its instance base; the art file is not a definition-only leaf.
$owner=Join-Path $data 'Owner.xml';$ownerPrepared=Join-Path $OutputDirectory 'owner-prepared'
[IO.File]::WriteAllText((Join-Path $data 'Base.xml'),'<!-- Reborn: instance-only source base must not be emitted. --><AssetDeclaration xmlns="uri:ea.com:eala:asset"><LogicCommandSet id="RebornContainerBaseSet"/></AssetDeclaration>')
[IO.File]::WriteAllText($owner,'<!-- Reborn: inherited support owner and real all-imported W3X. --><AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes><Include type="all" source="ART:RebornContainer.w3x"/><Include type="instance" source="Base.xml"/></Includes><LogicCommandSet id="RebornContainerSet" inheritFrom="RebornContainerBaseSet"/></AssetDeclaration>')
& $exe conscript-source-prepare $owner $ownerPrepared $sdk *> (Join-Path $OutputDirectory 'owner-prepare.log');if($LASTEXITCODE-ne 0){throw 'Inherited source with W3X all-include failed; see owner-prepare.log.'}
& $exe conscript-alpha-build (Join-Path $sdk 'SageXML') $StaticManifest $GlobalManifest (Join-Path $ownerPrepared 'game.xml') (Join-Path $OutputDirectory 'owner-compiled') *> (Join-Path $OutputDirectory 'owner-compile.log');if($LASTEXITCODE-ne 0){throw 'Inherited owner/container mixed stream failed; see owner-compile.log.'}
$ownerReceipt=Get-Content -LiteralPath (Join-Path $OutputDirectory 'owner-compiled/support/trial-plan.json') -Raw|ConvertFrom-Json
if($ownerReceipt.AssetCount-ne 3-or @($ownerReceipt.Assets|Where-Object {$_-ceq 'LogicCommandSet:RebornContainerBaseSet'}).Count){throw 'Instance-only base leaked into the W3X mixed package.'}
# Reborn: a missing container dependency must be reported at its W3X source coordinate, not silently accepted because the GameObject is valid.
[IO.File]::WriteAllText($source,$w3x.Replace('VL_RUSSIANPORTBUILDING.NEWSKIN_FILL01','RebornMissingContainerMesh'))
& $exe sdk-missing-dependencies $entry $sdk $StaticManifest $GlobalManifest (Join-Path $OutputDirectory 'missing.json') *> (Join-Path $OutputDirectory 'missing.log');if($LASTEXITCODE-eq 0){throw 'Missing W3X mesh was accepted.'}
$missing=Get-Content -LiteralPath (Join-Path $OutputDirectory 'missing.json') -Raw|ConvertFrom-Json
if(-not @($missing.Findings|Where-Object {$_.Code-ceq 'MISSINGASSET'-and $_.Target-like '*RebornMissingContainerMesh'-and $_.Source-ceq $source-and $_.Line-gt 0}).Count){throw 'Missing W3X mesh source coordinate was lost.'}
Write-Output 'Reborn: W3DContainer integration: PASS (W3X all/shared capture, WorldBuilder imports, authored/empty containers, stock ABI, inherited owner+W3X, stream/BIG readback, missing mesh with source line; no game or user output changed).'
