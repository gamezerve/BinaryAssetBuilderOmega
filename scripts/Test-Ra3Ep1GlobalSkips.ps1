# Reborn: exercise optional global inputs in private source trees without changing either live mod or publishing packages.
[CmdletBinding()]
param([Parameter(Mandatory)][string]$OutputDirectory)
$ErrorActionPreference='Stop';$repo=Split-Path -Parent $PSScriptRoot;$sdk=Join-Path $repo 'alpha';$work=[IO.Path]::GetFullPath($OutputDirectory)
if([IO.Path]::GetDirectoryName($work)-ine (Join-Path $sdk '.work') -or (Test-Path -LiteralPath $work)){throw 'Use a fresh direct child of alpha/.work.'}
$null=New-Item -ItemType Directory -Path $work
. (Join-Path $PSScriptRoot 'Ra3Ep1BuildLog.ps1')
. (Join-Path $PSScriptRoot 'Ra3Ep1GlobalData.ps1')
$settings=@{};foreach($line in Get-Content -LiteralPath (Join-Path $sdk 'uprising-alpha.paths')){if($line -and -not $line.StartsWith('#')){$pair=$line.Split('=',2);$settings[$pair[0]]=$pair[1]}}
$rows=@()
foreach($case in @('MissingDirectory','EmptyDirectory','NoMatchingFiles','BlankFile','EmptyDeclaration','EmptyIncludeGraph','InstanceOnly','MalformedXml','MissingInclude','MissingAssetId','WrongNamespace')){
    $caseRoot=Join-Path $work $case;$data=Join-Path $caseRoot 'mod/Data';$build=Join-Path $caseRoot 'build';$maps=Join-Path $data 'AdditionalMaps'
    $null=New-Item -ItemType Directory -Path $data,$build
    if($case-ne 'MissingDirectory'){$null=New-Item -ItemType Directory -Path $maps}
    $entry=Join-Path $maps 'MapMetaData_Test.xml';$ea='uri:ea.com:eala:asset'
    switch($case){
        NoMatchingFiles{[IO.File]::WriteAllText((Join-Path $maps 'Other.xml'),'not selected')}
        BlankFile{[IO.File]::WriteAllText($entry," `r`n`t")}
        EmptyDeclaration{[IO.File]::WriteAllText($entry,('<AssetDeclaration xmlns="'+$ea+'"><Tags/><Includes/></AssetDeclaration>'))}
        EmptyIncludeGraph{
            [IO.File]::WriteAllText($entry,('<AssetDeclaration xmlns="'+$ea+'"><Includes><Include type="all" source="DATA:EmptyLeaf.xml"/></Includes></AssetDeclaration>'))
            [IO.File]::WriteAllText((Join-Path $data 'EmptyLeaf.xml'),('<AssetDeclaration xmlns="'+$ea+'"><Defines><Define name="UNUSED_VALUE" value="1"/></Defines></AssetDeclaration>'))
        }
        InstanceOnly{
            [IO.File]::WriteAllText($entry,('<AssetDeclaration xmlns="'+$ea+'"><Includes><Include type="instance" source="DATA:Base.xml"/></Includes></AssetDeclaration>'))
            [IO.File]::WriteAllText((Join-Path $data 'Base.xml'),('<AssetDeclaration xmlns="'+$ea+'"><UpgradeTemplate id="UnemittedBase" Type="OBJECT"/></AssetDeclaration>'))
        }
        MalformedXml{[IO.File]::WriteAllText($entry,'<AssetDeclaration>')}
        MissingInclude{[IO.File]::WriteAllText($entry,('<AssetDeclaration xmlns="'+$ea+'"><Includes><Include type="all" source="DATA:Missing.xml"/></Includes></AssetDeclaration>'))}
        MissingAssetId{[IO.File]::WriteAllText($entry,('<AssetDeclaration xmlns="'+$ea+'"><UpgradeTemplate Type="OBJECT"/></AssetDeclaration>'))}
        WrongNamespace{[IO.File]::WriteAllText($entry,'<AssetDeclaration xmlns="wrong-namespace"/>')}
    }
    $result=$null;$failed=$false;$shown=@()
    try{$shown=@(Build-RebornGlobalData $data $sdk $settings $settings.StaticManifest $settings.GlobalManifest $settings.CompilerDirectory $build (Join-Path $caseRoot 'detail.log') ([ref]$result))}catch{$failed=$true}
    $expectedFailure=$case-in @('MalformedXml','MissingInclude','MissingAssetId','WrongNamespace')
    if($failed-ne $expectedFailure){throw ('Unexpected optional global result: '+$case)}
    if(-not $failed){
        if($result.Streams.Count-ne 0){throw ('Unexpected global output: '+$case)}
        if(($shown-join "`n")-notmatch '(?m)^Skipped: \[No input\] ' -or @(Get-ChildItem -LiteralPath $build -Filter *.big -Recurse).Count){throw ('Missing yellow-skip reason or unexpected package: '+$case)}
    }
    [IO.File]::WriteAllLines((Join-Path $caseRoot 'shown.log'),[string[]]$shown)
    $rows+= [pscustomobject]@{Case=$case;Passed=$true;ExpectedFailure=$expectedFailure}
}
[IO.File]::WriteAllText((Join-Path $work 'report.json'),(ConvertTo-Json -InputObject @($rows)))
Write-Output ('Reborn: optional global inputs PASS ('+$rows.Count+' cases; missing/blank/empty source graphs skipped, malformed XML and missing explicit includes still fail; no mod build/publication).')
