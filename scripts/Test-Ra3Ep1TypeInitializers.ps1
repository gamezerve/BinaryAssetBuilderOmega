# Reborn: validate limited static instruction evidence and private-memory faults without target execution, general disassembly or compiler registration changes.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath)
$ErrorActionPreference='Stop'
$audit=Join-Path $PSScriptRoot 'Get-Ra3Ep1TypeInitializers.ps1'
$first=& $audit -ImagePath $ImagePath -SelfTest
$second=& $audit -ImagePath $ImagePath -AsJson|ConvertFrom-Json
if($first.DecodedInitializers-ne 150 -or $first.PlainTemplateCount-ne 43 -or $first.VtableTemplateCount-ne 107 -or
    $first.RuntimeRowsNotCovered-ne 1192 -or $first.FaultCasesExecuted-ne 8 -or -not $first.FaultTestsPassed -or
    ($first.Rows|ConvertTo-Json -Depth 8 -Compress) -cne ($second.Rows|ConvertTo-Json -Depth 8 -Compress) -or
    $first.TargetExecuted -or $first.FullTableCallersVerified -or $first.Ep1ProcessingHashRecovered -or $first.ProductionBuildReady){throw 'Initializer observation/repeat contract differs.'}
$texture=@($first.Rows|Where-Object Name -ceq Texture)[0]
$armor=@($first.Rows|Where-Object Name -ceq ArmorTemplate)[0]
if($texture.Offset-cne '0x007BDF00' -or $texture.HashStoreVa-cne '0x00CBE850' -or $texture.TypeHash-cne '0x9F5FF8DA' -or
    $texture.Sha256-cne '3F0537FE986CE34EDDCB75764C0E290368D341EC17CCB2CC853A3AFC9FBD028F' -or
    $armor.Length-ne 34 -or $armor.HasVtableWrite -or @($first.Rows|Where-Object Name -ceq PathMusicEvent).Count-ne 0){throw 'Reviewed Texture/Armor/uncovered music distinction differs.'}
$rejected=$false
try{$null=& $audit -ImagePath (Join-Path (Split-Path -Parent $PSScriptRoot) 'audio.dll')}catch{$rejected=$true}
if(-not $rejected){throw 'Unreviewed image admitted to initializer decoder.'}
Write-Output 'Type initializer tests: PASS; 150 named hash-to-object writes, 43 plain/107 vtable templates, exact repeat JSON, Texture/Armor/music distinction, eight memory faults and public artifact rejection; target not executed.'
