# Reborn: a source-context witness must come from real all/instance/XInclude edges, never a precompiled reference stream.
param([Parameter(Mandatory)][string]$Compiler,[Parameter(Mandatory)][string]$OutputRoot)
$ErrorActionPreference='Stop'
if(Test-Path -LiteralPath $OutputRoot){throw 'Fresh private source-plan fixture required.'}
$taskSource=Join-Path $OutputRoot 'SageXML'
$taskWork=Join-Path $OutputRoot '.work'
$null=New-Item -ItemType Directory -Path $taskSource,$taskWork
$taskInputs=@{
    'Definitions.xml'='<AssetDeclaration xmlns="uri:ea.com:eala:asset"><Defines><Define name="COST" value="1"/></Defines></AssetDeclaration>'
    'Module.xml'='<Behaviors xmlns="uri:ea.com:eala:asset"/>'
    'Actual.xml'='<AssetDeclaration xmlns="uri:ea.com:eala:asset" xmlns:xi="http://www.w3.org/2001/XInclude"><Includes><Include type="instance" source="Definitions.xml"/></Includes><GameObject id="Actual"><xi:include href="Module.xml"/></GameObject></AssetDeclaration>'
    'global.xml'='<AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes><Include type="all" source="Definitions.xml"/><Include type="all" source="Module.xml"/></Includes></AssetDeclaration>'
    'ReferenceMap.xml'='<AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes><Include type="reference" source="DATA:global.xml"/></Includes><GameMap id="ReferenceMap"/></AssetDeclaration>'
}
foreach($taskName in $taskInputs.Keys){[IO.File]::WriteAllText((Join-Path $taskSource $taskName),$taskInputs[$taskName])}
$taskPlan=Join-Path $taskWork 'plan.json'
& $Compiler stock-source-plan $OutputRoot $taskPlan
if($LASTEXITCODE -ne 0){throw 'Source context fixture failed.'}
$taskRows=Get-Content -LiteralPath $taskPlan -Raw|ConvertFrom-Json
foreach($taskName in @('Definitions.xml','Module.xml')){
    $taskRow=$taskRows|Where-Object Relative -EQ $taskName
    if($taskRow.Kind -ne 'ConsumerContext' -or $taskRow.Entry -ne 'Actual.xml'){throw ('Real source consumer lost: '+$taskName)}
    if($taskRow.Consumers -contains 'ReferenceMap.xml'){throw 'Precompiled reference was treated as a source consumer.'}
}
Write-Output 'Reborn: source-plan regression PASS (instance and XInclude retained; precompiled reference excluded).'
