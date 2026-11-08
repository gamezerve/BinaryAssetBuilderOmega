# Reborn: source-only qualification must accept genuine emptiness and refuse real assets, malformed metadata and missing dependencies.
param([Parameter(Mandatory)][string]$Compiler,[Parameter(Mandatory)][string]$Sdk,[Parameter(Mandatory)][string]$OutputRoot)
$ErrorActionPreference='Stop'
if(Test-Path -LiteralPath $OutputRoot){throw 'Fresh empty-source fixture required.'}
$taskData=Join-Path $OutputRoot 'input/Data';$null=New-Item -ItemType Directory -Path $taskData
$taskCases=@(
    @('Empty','<AssetDeclaration xmlns="uri:ea.com:eala:asset"><Tags/><Includes/></AssetDeclaration>',$true),
    @('EmptyInclude','<AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes><Include type="all" source="Empty.xml"/></Includes></AssetDeclaration>',$true),
    @('Nonempty','<AssetDeclaration xmlns="uri:ea.com:eala:asset"><GameObject id="Actual"/></AssetDeclaration>',$false),
    @('InvalidMetadata','<AssetDeclaration xmlns="uri:ea.com:eala:asset"><Tags unknown="1"/></AssetDeclaration>',$false),
    @('MissingInclude','<AssetDeclaration xmlns="uri:ea.com:eala:asset"><Includes><Include type="all" source="Missing.xml"/></Includes></AssetDeclaration>',$false)
)
foreach($taskCase in $taskCases){[IO.File]::WriteAllText((Join-Path $taskData ($taskCase[0]+'.xml')),$taskCase[1])}
foreach($taskCase in $taskCases){
    $taskName=$taskCase[0];$taskReceipt=Join-Path $OutputRoot ($taskName+'.json')
    & $Compiler stock-source-empty-validate (Join-Path $taskData ($taskName+'.xml')) $taskReceipt $Sdk *> (Join-Path $OutputRoot ($taskName+'.log'))
    if(($LASTEXITCODE-eq 0)-ne [bool]$taskCase[2]){throw ('Unexpected empty-source qualification: '+$taskName)}
    if($taskCase[2]){
        $taskResult=Get-Content -LiteralPath $taskReceipt -Raw|ConvertFrom-Json
        if(!$taskResult.SourceOnlyQualified -or $taskResult.AssetCount-ne 0 -or $taskResult.NativeAssetsEmitted){throw 'Empty source was falsely native-qualified.'}
    }elseif(Test-Path -LiteralPath $taskReceipt){throw 'Rejected source emitted qualification evidence.'}
}
Write-Output 'Reborn: empty-source qualification PASS (2 empty closures, 3 refusals; no native assets).'
