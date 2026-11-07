# Reborn: reverse a real original distance list and independently inspect the emitted native float array; publish no mod and launch no game.
[CmdletBinding()]
param([string]$Repository='D:\TEMP\BinaryAssetBuilderOmega2-uprising')
$ErrorActionPreference='Stop'
$sdk=Join-Path $Repository 'alpha'
$compiler=Join-Path $sdk 'tools/compiler/BinaryAssetBuilder.ManifestInspector.exe'
$work=Join-Path $sdk ('.work/stock-target-distance-order/'+[guid]::NewGuid().ToString('N'))
$null=New-Item -ItemType Directory -Path $work -Force
$original=[Xml.XmlDocument]::new();$original.Load((Join-Path $sdk 'SageXML/SkirmishAI/AITargetHeuristicLibrary.xml'))
$ns=[Xml.XmlNamespaceManager]::new($original.NameTable);$ns.AddNamespace('ea','uri:ea.com:eala:asset')
$source=$original.SelectSingleNode('//ea:AITargetingHeuristic[@id="BuildingPlacementHeuristic"]',$ns)
if(!$source -or $source.HasAttribute('inheritFrom')){throw 'Original independent placement heuristic is required.'}
$probe=[Xml.XmlDocument]::new();$probe.LoadXml('<AssetDeclaration xmlns="uri:ea.com:eala:asset"><!-- Reborn: test the original schema sortBy rule without editing the original library. --></AssetDeclaration>')
$root=$probe.ImportNode($source,$true);$root.SetAttribute('id','RebornTargetDistanceOrderProbe')
$distances=@($root.ChildNodes|Where-Object {$_.LocalName-eq 'BuildingPlacementDistance'})
foreach($distance in $distances){$null=$root.RemoveChild($distance)}
[array]::Reverse($distances)
foreach($distance in $distances){$null=$root.AppendChild($distance)}
$null=$probe.DocumentElement.AppendChild($root)
$entry=Join-Path $work 'probe.xml';$probe.Save($entry)
$env:REBORN_DEVELOPER_BUILD_TESTS='0';$env:REBORN_SILENT_MODE='1'
$prepared=Join-Path $work 'source';$output=Join-Path $work 'compiled'
& $compiler conscript-source-prepare $entry $prepared $sdk (Join-Path $sdk 'SageXML') *> (Join-Path $work 'prepare.log')
if($LASTEXITCODE-ne 0){throw ('Target order source preparation failed: '+$work)}
& $compiler conscript-alpha-build (Join-Path $sdk 'SageXML') (Join-Path $sdk 'catalogs/ra3ep1/streams/static/static.manifest') (Join-Path $sdk 'catalogs/ra3ep1/streams/global/global.manifest') (Join-Path $prepared 'game.xml') $output *> (Join-Path $work 'native.log')
if($LASTEXITCODE-ne 0){throw ('Target order native qualification failed: '+$work)}
# Reborn: the independently read-back untokenized EP1 root has its count/pointer pair at +200/+204.
$bytes=[IO.File]::ReadAllBytes((Join-Path $output 'stream-readback/diagnostic.bin'))
if([BitConverter]::ToUInt32($bytes,0)-ne 0xBABB0000L){throw 'Linked native stream header differs.'}
$count=[BitConverter]::ToUInt32($bytes,208);$pointer=[BitConverter]::ToUInt32($bytes,212)
if($count-ne 5 -or $pointer-lt 212 -or ($pointer+8+4*$count)-gt $bytes.Length){throw 'Target distance native allocation differs.'}
$actual=@(for($i=0;$i-lt $count;$i++){[BitConverter]::ToSingle($bytes,8+$pointer+4*$i)})
$expected=@(200,500,1000,3000,7000)
if(($actual-join ',')-cne ($expected-join ',')){throw ('Original distance sortBy rule differs: '+($actual-join ','))}
Write-Output ('Reborn: target distance order PASS: '+($actual-join ', ')+'; evidence: '+$work)
