# Reborn: exercise DDS capture/embedding and legacy TGA coexistence in private output, never changing user mod sources or Documents packages.
[CmdletBinding()]
param([Parameter(Mandatory)][string]$OutputDirectory,[Parameter(Mandatory)][string]$ArtDirectory,[Parameter(Mandatory)][string]$StaticManifest,[Parameter(Mandatory)][string]$GlobalManifest,[Parameter(Mandatory)][string]$WorldBuilderManifest)
$ErrorActionPreference='Stop';$repo=Split-Path -Parent $PSScriptRoot;$sdk=Join-Path $repo 'alpha';$exe=Join-Path $repo 'source/BinaryAssetBuilder.ManifestInspector/bin/x86/Release/net8.0/BinaryAssetBuilder.ManifestInspector.exe'
if(Test-Path -LiteralPath $OutputDirectory){throw 'Fresh private test directory required.'}
$OutputDirectory=[IO.Path]::GetFullPath($OutputDirectory);$data=Join-Path $OutputDirectory 'mod/Data';$art=Join-Path $OutputDirectory 'mod/Art';$null=New-Item -ItemType Directory -Path $data,(Join-Path $art 'VL'),(Join-Path $art 'LL'),(Join-Path $art 'ZZ')
& $exe dds-texture-self-test;if($LASTEXITCODE-ne 0){throw 'DDS format/refusal self-test failed.'}
$names=@('VL_RussianPortBuilding','VL_RussianPortBuilding_NRM','VL_RussianPortBuilding_SPM');$includes=''
foreach($name in $names){foreach($extension in @('.xml','.dds')){Copy-Item -LiteralPath (Join-Path $ArtDirectory ($name+$extension)) -Destination (Join-Path $art ('VL/'+$name+$extension))};$includes+='<Include type="all" source="ART:VL/'+$name+'.xml"/>'}
# Reborn: two different source folders deliberately use the same relative filename; the large DDS also exceeds the old one-MiB stream readback fixture quota.
foreach($case in @(@('LL','RebornLargeDDS','DXT5',1024,1048576),@('ZZ','RebornSmallDDS','DXT1',4,8))){
    $bytes=[byte[]]::new(128+[int]$case[4]);[BitConverter]::GetBytes([uint32]0x20534444).CopyTo($bytes,0);[BitConverter]::GetBytes([uint32]124).CopyTo($bytes,4);[BitConverter]::GetBytes([uint32]0x1007).CopyTo($bytes,8);[BitConverter]::GetBytes([uint32]$case[3]).CopyTo($bytes,12);[BitConverter]::GetBytes([uint32]$case[3]).CopyTo($bytes,16);[BitConverter]::GetBytes([uint32]32).CopyTo($bytes,76);[BitConverter]::GetBytes([uint32]4).CopyTo($bytes,80);[Text.Encoding]::ASCII.GetBytes($case[2]).CopyTo($bytes,84);[BitConverter]::GetBytes([uint32]0x1000).CopyTo($bytes,108)
    [IO.File]::WriteAllBytes((Join-Path $art ($case[0]+'/image.dds')),$bytes);[IO.File]::WriteAllText((Join-Path $art ($case[0]+'/Texture.xml')),'<!-- Reborn: relative DDS source binding fixture. --><AssetDeclaration xmlns="uri:ea.com:eala:asset">'+"`n"+'<Texture id="'+$case[1]+'" File="image.dds"/>'+"`n"+'</AssetDeclaration>');$includes+='<Include type="all" source="ART:'+$case[0]+'/Texture.xml"/>'
}
$entry=Join-Path $data 'Static_Mod.xml';[IO.File]::WriteAllText($entry,'<!-- Reborn: explicit texture-only mod, without a placeholder GameObject. --><AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes>'+$includes+'</Includes></AssetDeclaration>')
$prepared=Join-Path $OutputDirectory 'prepared';$compiled=Join-Path $OutputDirectory 'compiled'
& $exe conscript-source-prepare $entry $prepared $sdk *> (Join-Path $OutputDirectory 'prepare.log');if($LASTEXITCODE-ne 0){throw 'DDS preparation failed; see prepare.log.'}
& $exe conscript-alpha-build (Join-Path $sdk 'SageXML') $StaticManifest $GlobalManifest (Join-Path $prepared 'game.xml') $compiled *> (Join-Path $OutputDirectory 'compile.log');if($LASTEXITCODE-ne 0){throw 'DDS compile/stream/BIG readback failed; see compile.log.'}
& $exe dds-texture-stock-self-test $WorldBuilderManifest (Join-Path $compiled 'stream-readback/diagnostic.manifest');if($LASTEXITCODE-ne 0){throw 'Stock DDS preservation differs.'}
$receipt=Get-Content -LiteralPath (Join-Path $compiled 'support/trial-plan.json') -Raw|ConvertFrom-Json;if($receipt.AssetCount-ne 5-or -not $receipt.OfflineValidated){throw 'DDS texture-only package receipt differs.'}
# Reborn: an actual unsupported recompression request must name its declaring XML/field instead of silently preserving the wrong format.
$bad=Join-Path $art 'LL/Texture.xml';$xml=[xml](Get-Content -LiteralPath $bad -Raw);$xml.DocumentElement.Texture.SetAttribute('OutputFormat','DXT1');$xml.Save($bad)
& $exe conscript-source-prepare $entry (Join-Path $OutputDirectory 'invalid-prepared') $sdk *> (Join-Path $OutputDirectory 'conversion.log');if($LASTEXITCODE-eq 0){throw 'Unsupported DDS conversion was accepted.'}
if((Get-Content -LiteralPath (Join-Path $OutputDirectory 'conversion.log') -Raw)-notmatch 'Texture.xml\([1-9][0-9]*,[1-9][0-9]*\): DDS OutputFormat conversion'){throw 'DDS conversion source field coordinate was lost.'}
Write-Output 'Reborn: DDS integration: PASS (3 original mipmapped DDS textures exact, texture-only mod, relative-path collisions, >1 MiB stream/BIG, unsupported conversion source coordinates; no game/user output changed).'
