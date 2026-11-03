# Reborn: exercise production graphs above old fixture quotas, real stream emission and actual cycle rejection without editing or publishing a user mod.
[CmdletBinding()]
param([Parameter(Mandatory)][string]$OutputDirectory,[string]$CompilerDirectory)
$ErrorActionPreference='Stop'
$repo=Split-Path -Parent $PSScriptRoot;$sdk=Join-Path $repo 'alpha'
. "$PSScriptRoot/Ra3Ep1PortablePaths.ps1"
$settings=Read-RebornSdkPaths $sdk
if(-not $CompilerDirectory){$CompilerDirectory=Join-Path $repo 'source/BinaryAssetBuilder.ManifestInspector/bin/x86/Release/net8.0'}
$exe=Join-Path $CompilerDirectory 'BinaryAssetBuilder.ManifestInspector.exe'
$output=[IO.Path]::GetFullPath($OutputDirectory)
if([IO.Path]::GetDirectoryName($output)-ine (Join-Path $sdk '.work') -or (Test-Path -LiteralPath $output)){throw 'Use a fresh direct child of alpha/.work.'}
$null=New-Item -ItemType Directory -Path $output
#-------------------------------------------------------------------------------------------------
<# Reborn: capture compiler evidence per case; unexpected failures stop before any user-owned output could be touched. #>
#-------------------------------------------------------------------------------------------------
function Invoke-ScaleCase([string]$Name,[string[]]$Arguments,[switch]$RejectCycle){
    $log=Join-Path $output ($Name+'.log');& $exe @Arguments *> $log;$code=$LASTEXITCODE
    if($RejectCycle){$text=[IO.File]::ReadAllText($log);if($code-eq 0 -or $text-notmatch 'IncludeCycle' -or $text-match 'GraphLimit'){throw ('Expected actual Include cycle diagnostic: '+$log)}}
    elseif($code-ne 0){throw ('Source scale case failed: '+$log)}
    Write-Output ('Reborn: '+$Name+' PASS')
}
# Reborn: 700 distinct source files and 5,200 Include edges also produce 700 independently linked script assets.
$wide=Join-Path $output 'wide/Data';$null=New-Item -ItemType Directory -Path $wide
for($i=0;$i-lt 700;$i++){[IO.File]::WriteAllText((Join-Path $wide ('part'+$i+'.xml')),('<AssetDeclaration xmlns="uri:ea.com:eala:asset"><GameScriptList id="ScaleScript_'+$i+'"/></AssetDeclaration>'))}
$builder=[Text.StringBuilder]::new();$null=$builder.Append('<AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes>')
for($i=0;$i-lt 5200;$i++){$null=$builder.Append('<Include type="all" source="part'+($i%700)+'.xml"/>')}
$null=$builder.Append('</Includes></AssetDeclaration>');$entry=Join-Path $wide 'Static_Mod.xml';[IO.File]::WriteAllText($entry,$builder.ToString())
$prepared=Join-Path $output 'wide/prepared';Invoke-ScaleCase 'wide-source' @('conscript-source-prepare',$entry,$prepared,$sdk)
$receipt=Get-Content (Join-Path $prepared 'source-input.json') -Raw|ConvertFrom-Json
if(@($receipt.Sources).Count-ne 701 -or $receipt.AssetCount-ne 700){throw 'Wide graph source/asset counts differ.'}
# Reborn: the Studio dependency preflight must accept the same large graph instead of restoring its old quota before compilation.
Invoke-ScaleCase 'wide-dependency-audit' @('sdk-missing-dependencies',$entry,$sdk,$settings.StaticManifest,$settings.GlobalManifest,(Join-Path $output 'wide-audit.json'))
$audit=Get-Content (Join-Path $output 'wide-audit.json') -Raw|ConvertFrom-Json
if(-not $audit.SourceGraphComplete -or $audit.ModAssets-ne 700){throw 'Large dependency preflight is incomplete.'}
Invoke-ScaleCase 'wide-stream' @('conscript-alpha-build',$settings.UprisingXmlRoot,$settings.StaticManifest,$settings.GlobalManifest,(Join-Path $prepared 'game.xml'),(Join-Path $output 'wide/compiled'))
$build=Get-Content (Join-Path $output 'wide/compiled/support/trial-plan.json') -Raw|ConvertFrom-Json
if($build.AssetCount-ne 700 -or -not $build.OfflineValidated){throw 'Large emitted stream count/readback differs.'}
# Reborn: a 600-edge valid Include chain must prepare without a 32-depth rejection or recursive emitter stack growth.
$deep=Join-Path $output 'deep/Data';$null=New-Item -ItemType Directory -Path $deep
for($i=0;$i-le 600;$i++){
    $body=if($i-eq 600){'<GameScriptList id="DeepSource"/>'}else{'<Includes><Include type="all" source="part'+($i+1)+'.xml"/></Includes>'}
    [IO.File]::WriteAllText((Join-Path $deep ('part'+$i+'.xml')),('<AssetDeclaration xmlns="uri:ea.com:eala:asset">'+$body+'</AssetDeclaration>'))
}
Invoke-ScaleCase 'deep-source' @('conscript-source-prepare',(Join-Path $deep 'part0.xml'),(Join-Path $output 'deep/prepared'),$sdk)
# Reborn: more than 512 visible definitions remain valid; imported expression resolution must retain its complete table.
$defines=Join-Path $output 'defines/Data';$null=New-Item -ItemType Directory -Path $defines
$builder=[Text.StringBuilder]::new();$null=$builder.Append('<AssetDeclaration xmlns="uri:ea.com:eala:asset"><Defines>')
for($i=0;$i-lt 2048;$i++){$null=$builder.Append('<Define name="VALUE_'+$i+'" value="42"/>')}
$null=$builder.Append('</Defines></AssetDeclaration>');[IO.File]::WriteAllText((Join-Path $defines 'Definitions.xml'),$builder.ToString())
[IO.File]::WriteAllText((Join-Path $defines 'Static_Mod.xml'),'<AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes><Include type="all" source="Definitions.xml"/></Includes><UpgradeTemplate id="ScaleDefinitions" BuildCost="=$VALUE_0"/></AssetDeclaration>')
Invoke-ScaleCase 'definitions' @('conscript-source-prepare',(Join-Path $defines 'Static_Mod.xml'),(Join-Path $output 'defines/prepared'),$sdk)
[xml]$resolved=Get-Content (Join-Path $output 'defines/prepared/game.xml') -Raw
if($resolved.DocumentElement.GetAttribute('BuildCost')-ne '42'){throw 'Imported definition value differs.'}
# Reborn: a real source transaction may exceed both the former per-file 4 MiB and aggregate 32 MiB allowances.
$large=Join-Path $output 'large-xml/Data';$null=New-Item -ItemType Directory -Path $large
$padding='x'*(6*1048576);$includes=[Text.StringBuilder]::new()
for($i=0;$i-lt 6;$i++){
    [IO.File]::WriteAllText((Join-Path $large ('large'+$i+'.xml')),('<AssetDeclaration xmlns="uri:ea.com:eala:asset"><!--'+$padding+'--></AssetDeclaration>'))
    $null=$includes.Append('<Include type="all" source="large'+$i+'.xml"/>')
}
[IO.File]::WriteAllText((Join-Path $large 'Static_Mod.xml'),('<AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes>'+$includes.ToString()+'</Includes><GameScriptList id="LargeXml"/></AssetDeclaration>'))
Invoke-ScaleCase 'large-xml' @('conscript-source-prepare',(Join-Path $large 'Static_Mod.xml'),(Join-Path $output 'large-xml/prepared'),$sdk)
# Reborn: local inheritance resolves bases first instead of rejecting a valid chain after thirty-two ancestors.
$inherit=Join-Path $output 'inherit/Data';$null=New-Item -ItemType Directory -Path $inherit
$builder=[Text.StringBuilder]::new();$null=$builder.Append('<AssetDeclaration xmlns="uri:ea.com:eala:asset">')
# Reborn: alternating qualified and bare names must resolve to identical same-type handles throughout the chain.
for($i=0;$i-lt 300;$i++){$prefix=if($i%2){'UpgradeTemplate:'}else{''};$basis=if($i-eq 0){' BuildCost="42"'}else{' inheritFrom="'+$prefix+'ScaleUpgrade_'+($i-1)+'"'};$null=$builder.Append('<UpgradeTemplate id="ScaleUpgrade_'+$i+'"'+$basis+'/>')}
$null=$builder.Append('</AssetDeclaration>');[IO.File]::WriteAllText((Join-Path $inherit 'Static_Mod.xml'),$builder.ToString())
Invoke-ScaleCase 'inheritance' @('conscript-source-prepare',(Join-Path $inherit 'Static_Mod.xml'),(Join-Path $output 'inherit/prepared'),$sdk)
[xml]$inherited=Get-Content (Join-Path $output 'inherit/prepared/additional.xml') -Raw
if($inherited.DocumentElement.LastChild.GetAttribute('BuildCost')-ne '42'){throw 'Long inheritance chain value differs.'}
# Reborn: more than sixty-four direct instance Includes and an empty all-import must preserve ordinary inheritance visibility.
$external=Join-Path $output 'external/Data';$null=New-Item -ItemType Directory -Path $external
$builder=[Text.StringBuilder]::new();$null=$builder.Append('<AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes>')
for($i=0;$i-lt 130;$i++){[IO.File]::WriteAllText((Join-Path $external ('empty'+$i+'.xml')),'<AssetDeclaration xmlns="uri:ea.com:eala:asset"/>');$null=$builder.Append('<Include type="instance" source="empty'+$i+'.xml"/>')}
[IO.File]::WriteAllText((Join-Path $external 'Base.xml'),'<AssetDeclaration xmlns="uri:ea.com:eala:asset"><UpgradeTemplate id="ExternalBase" BuildCost="42"/></AssetDeclaration>')
$null=$builder.Append('<Include type="instance" source="Base.xml"/><Include type="all" source="empty0.xml"/></Includes><UpgradeTemplate id="ExternalDerived" inheritFrom="ExternalBase"/></AssetDeclaration>')
[IO.File]::WriteAllText((Join-Path $external 'Static_Mod.xml'),$builder.ToString())
Invoke-ScaleCase 'external-inheritance' @('conscript-source-prepare',(Join-Path $external 'Static_Mod.xml'),(Join-Path $output 'external/prepared'),$sdk)
[xml]$externalResult=Get-Content (Join-Path $output 'external/prepared/game.xml') -Raw
if($externalResult.DocumentElement.GetAttribute('BuildCost')-ne '42'){throw 'External inheritance/no-op include value differs.'}
# Reborn: removing quotas must not turn an actual cyclic Include into a silent partial graph.
$cycle=Join-Path $output 'cycle/Data';$null=New-Item -ItemType Directory -Path $cycle
[IO.File]::WriteAllText((Join-Path $cycle 'a.xml'),'<AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes><Include type="all" source="b.xml"/></Includes><GameScriptList id="Cycle"/></AssetDeclaration>')
[IO.File]::WriteAllText((Join-Path $cycle 'b.xml'),'<AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes><Include type="all" source="a.xml"/></Includes></AssetDeclaration>')
Invoke-ScaleCase 'actual-cycle' @('conscript-source-prepare',(Join-Path $cycle 'a.xml'),(Join-Path $output 'cycle/prepared'),$sdk) -RejectCycle
Write-Output 'Reborn: source scale PASS (701 sources, 5200 edges, 700 emitted assets, 600 Include levels, 2048 definitions, 36 MiB XML, 300 inherited assets; actual cycles rejected; no user mod or Documents publication).'
