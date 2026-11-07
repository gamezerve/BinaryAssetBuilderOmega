# Reborn: verify authored AI budget values by independent native readback and reject malformed heuristic/budget source without publishing or launching a game.
[CmdletBinding()]
param([string]$Repository=(Split-Path -Parent $PSScriptRoot))
$ErrorActionPreference='Stop'
$sdk=Join-Path $Repository 'alpha'
$compiler=Join-Path $sdk 'tools/compiler/BinaryAssetBuilder.ManifestInspector.exe'
$work=Join-Path $sdk ('.work/stock-budget-heuristics/'+[guid]::NewGuid().ToString('N'))
$null=New-Item -ItemType Directory -Path $work -Force
$source=@'
<?xml version="1.0" encoding="utf-8"?>
<AssetDeclaration xmlns="uri:ea.com:eala:asset">
  <!-- Reborn: changed literal values and reordered heuristic types must survive the complete production route. -->
  <AIBudgetStateDefinition id="RebornBudgetLiteralProbe">
    <Heuristic>
      <ConstantHeuristic Weight="0.375"/>
      <OpeningMoveHeuristic Complete="false"/>
      <EnemyNearbyHeuristic Distance="432.25" MinEnemyValue="321" EnemyNearby="false">
        <EnemyFilter Rule="ANY" Include="INFANTRY"/>
      </EnemyNearbyHeuristic>
    </Heuristic>
    <Budget>
      <AccountShare Account="PRODUCTION" Percent="37%"/>
      <AccountShare Account="DEFENSE" Percent="63%"/>
    </Budget>
  </AIBudgetStateDefinition>
</AssetDeclaration>
'@
$entry=Join-Path $work 'probe.xml';[IO.File]::WriteAllText($entry,$source)
$env:REBORN_DEVELOPER_BUILD_TESTS='0';$env:REBORN_SILENT_MODE='1'
$prepared=Join-Path $work 'source';$output=Join-Path $work 'compiled'
& $compiler conscript-source-prepare $entry $prepared $sdk (Join-Path $sdk 'SageXML') *> (Join-Path $work 'prepare.log')
if($LASTEXITCODE-ne 0){throw ('Budget literal source preparation failed: '+$work)}
& $compiler conscript-alpha-build (Join-Path $sdk 'SageXML') (Join-Path $sdk 'catalogs/ra3ep1/streams/static/static.manifest') (Join-Path $sdk 'catalogs/ra3ep1/streams/global/global.manifest') (Join-Path $prepared 'game.xml') $output *> (Join-Path $work 'native.log')
if($LASTEXITCODE-ne 0){throw ('Budget literal native qualification failed: '+$work)}
# Reborn: inspect only the one emitted asset's native bytes after the linked stream's eight-byte header.
$bytes=[IO.File]::ReadAllBytes((Join-Path $output 'stream-readback/diagnostic.bin'))
if([BitConverter]::ToUInt32($bytes,0)-ne 0xBABB0000L){throw 'Linked budget native stream header differs.'}
$root=8;$count=[BitConverter]::ToUInt32($bytes,$root+4);$table=[BitConverter]::ToUInt32($bytes,$root+8)
if($count-ne 3 -or $table-lt 20 -or ($root+$table+12)-gt $bytes.Length){throw 'Budget heuristic pointer array differs.'}
$pointers=@(for($i=0;$i-lt 3;$i++){[BitConverter]::ToUInt32($bytes,$root+$table+4*$i)})
$expected=@(0x51FB6D92L,0x7EE4C9B4L,0x67B985A7L)
for($i=0;$i-lt 3;$i++){if([BitConverter]::ToUInt32($bytes,$root+$pointers[$i])-ne $expected[$i]){throw 'Authored heuristic order/class differs.'}}
if([BitConverter]::ToSingle($bytes,$root+$pointers[0]+4)-ne 0.375){throw 'Constant heuristic value was frozen to stock.'}
if($bytes[$root+$pointers[1]+4]-ne 0){throw 'Opening-move predicate flag differs.'}
if([BitConverter]::ToSingle($bytes,$root+$pointers[2]+4)-ne 432.25 -or [BitConverter]::ToUInt32($bytes,$root+$pointers[2]+8)-ne 321 -or $bytes[$root+$pointers[2]+16]-ne 0){throw 'Enemy heuristic values were frozen to stock.'}
$shares=[BitConverter]::ToUInt32($bytes,$root+12);$items=[BitConverter]::ToUInt32($bytes,$root+16)
if($shares-ne 2 -or [BitConverter]::ToUInt32($bytes,$root+$items)-ne 2 -or [BitConverter]::ToUInt32($bytes,$root+$items+8)-ne 3){throw 'Budget account order differs.'}
if([Math]::Abs([BitConverter]::ToSingle($bytes,$root+$items+4)-0.37)-gt 0.000001 -or [Math]::Abs([BitConverter]::ToSingle($bytes,$root+$items+12)-0.63)-gt 0.000001){throw 'Authored budget shares differ.'}
# Reborn: each malformed document must fail source/schema admission before any native output can be accepted.
$negative=@(
    [pscustomobject]@{Name='duplicate-budget';Xml=$source.Replace('</Budget>','</Budget><Budget/>');Field='Budget'},
    [pscustomobject]@{Name='empty-heuristic';Xml=($source -replace '(?s)<Heuristic>.*?</Heuristic>','<Heuristic/>');Field='Heuristic'},
    [pscustomobject]@{Name='invalid-account';Xml=$source.Replace('Account="PRODUCTION"','Account="UNKNOWN"');Field='Account'}
)
foreach($case in $negative){
    $bad=Join-Path $work ($case.Name+'.xml');[IO.File]::WriteAllText($bad,$case.Xml)
    $log=Join-Path $work ($case.Name+'.log')
    & $compiler conscript-source-prepare $bad (Join-Path $work $case.Name) $sdk (Join-Path $sdk 'SageXML') *> $log
    if($LASTEXITCODE-eq 0 -or [IO.File]::ReadAllText($log)-notmatch [Regex]::Escape($case.Field)){throw ('Invalid budget source was not specifically rejected: '+$case.Name)}
}
Write-Output ('Reborn: budget heuristic literal/native readback PASS; three schema rejection controls PASS; evidence: '+$work)
