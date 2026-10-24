# Reborn: build original model sources and edited weighted skin in private folders; never modify live mod sources or Documents packages.
[CmdletBinding()]
param([Parameter(Mandatory)][string]$OutputDirectory,[Parameter(Mandatory)][string]$ArtDirectory,[Parameter(Mandatory)][string]$StaticManifest,[Parameter(Mandatory)][string]$GlobalManifest,[Parameter(Mandatory)][string]$WorldBuilderManifest)
$ErrorActionPreference='Stop';$repo=Split-Path -Parent $PSScriptRoot;$sdk=Join-Path $repo 'alpha';$exe=Join-Path $repo 'source/BinaryAssetBuilder.ManifestInspector/bin/x86/Release/net8.0/BinaryAssetBuilder.ManifestInspector.exe'
if(Test-Path -LiteralPath $OutputDirectory){throw 'A fresh private output directory is required.'}
$OutputDirectory=[IO.Path]::GetFullPath($OutputDirectory);$data=Join-Path $OutputDirectory 'mod/Data';$art=Join-Path $OutputDirectory 'mod/Art/VL';$null=New-Item -ItemType Directory -Path $data,$art
# Reborn: copy only the six explicitly selected witnesses, with mesh/box names retaining their dots.
$names=@('VL_RUSSIANPORTBUILDING.NEWSKIN_CIV01.w3x','VL_RUSSIANPORTBUILDING.NEWSKIN_FILL01.w3x','VL_RUSSIANPORTBUILDING.OBBOX01.w3x','VL_RUSSIANPORTBUILDING.OBBOX02.w3x','VL_RUSSIANPORTBUILDING_CTR.w3x','VL_RUSSIANPORTBUILDING_HRC.w3x')
foreach($name in $names){Copy-Item -LiteralPath (Join-Path $ArtDirectory $name) -Destination (Join-Path $art $name)}
$includes=($names|ForEach-Object {'<Include type="all" source="ART:'+$_+'"/>'})-join '';$entry=Join-Path $data 'Static_Mod.xml'
[IO.File]::WriteAllText($entry,'<!-- Reborn: private complete model graph. --><AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes>'+$includes+'</Includes></AssetDeclaration>')
$prepared=Join-Path $OutputDirectory 'prepared';$compiled=Join-Path $OutputDirectory 'compiled'
& $exe conscript-source-prepare $entry $prepared $sdk *> (Join-Path $OutputDirectory 'prepare.log');if($LASTEXITCODE-ne 0){throw 'Geometry preparation failed; see prepare.log.'}
& $exe conscript-alpha-build (Join-Path $sdk 'SageXML') $StaticManifest $GlobalManifest (Join-Path $prepared 'game.xml') $compiled *> (Join-Path $OutputDirectory 'compile.log');if($LASTEXITCODE-ne 0){throw 'Geometry compilation failed; see compile.log.'}
& $exe w3d-geometry-stock-self-test $WorldBuilderManifest (Join-Path $compiled 'stream-readback/diagnostic.manifest') *> (Join-Path $OutputDirectory 'stock.log');if($LASTEXITCODE-ne 0){throw 'Geometry stock comparison failed; see stock.log.'}
# Reborn: exercise a genuinely edited mesh with two influences per vertex, one position stream and a custom dotted identity.
$xml=[Xml.XmlDocument]::new();$xml.XmlResolver=$null;$xml.Load((Join-Path $art $names[0]));$mesh=$xml.DocumentElement.ChildNodes|Where-Object {$_.NodeType-eq 'Element'}|Select-Object -First 1
$mesh.SetAttribute('id','RebornWeighted.Mesh');$bones=$mesh.ChildNodes|Where-Object {$_.NodeType-eq 'Element'-and $_.LocalName-ceq 'BoneInfluences'}|Select-Object -First 1
$second=$bones.CloneNode($true);$old=@($bones.ChildNodes|Where-Object {$_.NodeType-eq 'Element'});$new=@($second.ChildNodes|Where-Object {$_.NodeType-eq 'Element'})
for($i=0;$i-lt $old.Count;$i++){$old[$i].SetAttribute('Weight','0.5');$new[$i].SetAttribute('Weight','0.5');$new[$i].SetAttribute('Bone',$(if($old[$i].GetAttribute('Bone')-ceq '0'){'1'}else{'0'}))}
$null=$mesh.InsertAfter($second,$bones);$weighted=Join-Path $data 'Weighted.xml';$xml.Save($weighted)
$weightPrepared=Join-Path $OutputDirectory 'weighted-prepared'
& $exe conscript-source-prepare $weighted $weightPrepared $sdk *> (Join-Path $OutputDirectory 'weighted-prepare.log');if($LASTEXITCODE-ne 0){throw 'Edited skin preparation failed.'}
& $exe conscript-alpha-build (Join-Path $sdk 'SageXML') $StaticManifest $GlobalManifest (Join-Path $weightPrepared 'game.xml') (Join-Path $OutputDirectory 'weighted-compiled') *> (Join-Path $OutputDirectory 'weighted-compile.log');if($LASTEXITCODE-ne 0){throw 'Edited skin compile/readback failed; see weighted-compile.log.'}
# Reborn: mismatched vertex-channel lengths must fail with a source diagnostic before unsafe conversion starts.
$null=$bones.RemoveChild($old[-1]);$xml.Save($weighted)
& $exe conscript-source-prepare $weighted (Join-Path $OutputDirectory 'bad-prepared') $sdk *> (Join-Path $OutputDirectory 'bad-prepare.log');if($LASTEXITCODE-eq 0){throw 'Malformed skin was accepted.'}
$failure=Get-Content -LiteralPath (Join-Path $OutputDirectory 'bad-prepare.log') -Raw
if($failure-notmatch 'Weighted.xml\([1-9][0-9]*,[1-9][0-9]*\): Mesh BoneInfluences count differs from Vertices'){throw 'Malformed mesh channel/source-field coordinate diagnostic differs.'}
Write-Output 'Reborn: W3D geometry integration: PASS (original hierarchy/boxes/meshes vs EP1, complete local model graph, edited two-weight skin, malformed-channel refusal, stream/BIG readback; no game or user output changed).'
