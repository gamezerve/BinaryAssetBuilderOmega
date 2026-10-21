# Reborn: test presentation independently of mod sources, package generation and game processes.
$ErrorActionPreference='Stop'
. (Join-Path $PSScriptRoot 'Ra3Ep1BuildLog.ps1')
$scratch=Join-Path ([IO.Path]::GetTempPath()) ('RebornBuildLog-'+[guid]::NewGuid().ToString('N'))
$null=New-Item -ItemType Directory -Path $scratch
$detail=Join-Path $scratch 'detail.log'
try{
    $shown=@(Start-SdkBuildPhase 6 'Building static data';Write-SdkCompilerLine '{' $detail;Write-SdkCompilerLine '  "RuntimeReady": false' $detail;Write-SdkCompilerLine '}' $detail;Write-SdkCompilerLine 'ERROR: Source.xml:4: missing definition TEST' $detail;Write-SdkCompilerLine 'Warning: ALPHA005 Partial support' $detail;Write-SdkCompilerLine '   at Internal.Namespace.Method()' $detail;Write-SdkCompilerLine 'Reborn: source path [mod] DATA:Test.xml -> Test.xml' $detail;Write-SdkCompilerLine 'Native graph: PASS (internal long diagnostic evidence)' $detail;Stop-SdkBuildPhase)
    # Reborn: original physical filenames, spaces and line/column coordinates must survive the GUI-facing severity translation unchanged.
    $shown+=Write-SdkCompilerLine 'ERROR: C:\Mod Files\base.xml(17,9): Unsupported field.' $detail
    $text=$shown -join "`n";$raw=Get-Content -LiteralPath $detail -Raw
    if($text-notmatch '(?m)^Error: C:\\Mod Files\\base\.xml\(17,9\): Unsupported field\.' -or $raw-notmatch 'ERROR: C:\\Mod Files\\base\.xml\(17,9\):'){throw 'Original XML source coordinates lost in build log.'}
    if($text-notmatch 'Step 6: Building static data' -or $text-notmatch 'Step time taken: \d\d:' -or $text-notmatch '(?m)^Error: Source.xml:4:' -or $text-notmatch '(?m)^Warning: ALPHA005' -or $text-notmatch '\[\d\d:\d\d:\d\d\.\d{7}\] Reborn: source path'){throw 'Phase, time, source or severity display differs.'}
    if($text-match 'RuntimeReady|Internal.Namespace|internal long diagnostic evidence' -or $text-notmatch 'Native graph: PASS' -or $raw-notmatch 'internal long diagnostic evidence' -or $raw-notmatch 'RuntimeReady' -or $raw-notmatch 'Internal.Namespace' -or $raw-notmatch 'ERROR: Source.xml:4:'){throw 'Readable/full diagnostic separation lost evidence.'}
    if(($script:BuildLogLines -join "`n")-cne $text){throw 'Saved/displayed readable lines differ.'}
    Write-Output 'Reborn: build log presentation: PASS (phases, timings, severity, source, JSON/stack separation, raw evidence).'
}finally{
    # Reborn: remove only the one explicitly owned file and empty temporary directory, without recursive deletion.
    if(Test-Path -LiteralPath $detail){Remove-Item -LiteralPath $detail};[IO.Directory]::Delete($scratch,$false)
}
