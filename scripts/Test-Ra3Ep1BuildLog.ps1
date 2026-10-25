# Reborn: test presentation independently of mod sources, package generation and game processes.
$ErrorActionPreference='Stop'
. (Join-Path $PSScriptRoot 'Ra3Ep1BuildLog.ps1')
$scratch=Join-Path ([IO.Path]::GetTempPath()) ('RebornBuildLog-'+[guid]::NewGuid().ToString('N'))
$null=New-Item -ItemType Directory -Path $scratch
$detail=Join-Path $scratch 'detail.log'
try{
    $shown=@(Start-SdkBuildPhase 6 'Building static data';Write-SdkCompilerLine '{' $detail;Write-SdkCompilerLine '  "RuntimeReady": false' $detail;Write-SdkCompilerLine '}' $detail;Write-SdkCompilerLine 'ERROR: Source.xml:4: missing definition TEST' $detail;Write-SdkCompilerLine 'Warning: ALPHA005 Partial support' $detail;Write-SdkCompilerLine '   at Internal.Namespace.Method()' $detail;Write-SdkCompilerLine 'Reborn: source path [mod] DATA:Test.xml -> Test.xml' $detail;Write-SdkCompilerLine 'Native graph: PASS (internal long diagnostic evidence)' $detail;Write-SdkCompilerLine "Loading 'file:///C:/Mod%20Files/Soviet/SovietRefinery.xml'" $detail;Write-SdkCompilerLine 'sovietrefinery.xml: Compiling GameObject:SovietRefinery [942fff2d:0123abcd]' $detail;Write-SdkCompilerLine 'notes.txt: Compiling GameObject:Fake' $detail;Write-SdkCompilerLine 'ERROR: Reborn: tagged internal failure' $detail;Write-SdkCompilerLine 'Validation completed: 17 source file(s), 2 asset(s) admitted.' $detail;Write-SdkCompilerLine 'Compilation started: 2 asset(s).' $detail;Write-SdkCompilerLine 'Compilation completed: 2 asset(s); native, metadata and stream qualification passed.' $detail;Write-SdkCompilerLine 'Resolving references: 2 assets dependency-sorted; 1 local and 40 stock reference occurrences resolved.' $detail;Write-SdkCompilerLine 'Linking binary, relocation and import data verified by readback; mod Stream complete' $detail;Write-SdkCompilerLine 'Statistics GameObject: Instances Processed 2, Total Processing Time 00:00:01, Longest Processing Time 00:00:01 (GameObject:A), Average Processing Time 00:00:00.5' $detail;Stop-SdkBuildPhase)
    # Reborn: original physical filenames, spaces and line/column coordinates must survive the GUI-facing severity translation unchanged.
    $shown+=Write-SdkCompilerLine 'ERROR: C:\Mod Files\base.xml(17,9): Unsupported field.' $detail
    # Reborn: W3X asset compilation uses the same readable EA-style source line as XML compilation.
    $shown+=Write-SdkCompilerLine 'port_ctr.w3x: Compiling W3DContainer:RebornPort [942fff2d:0123abcd]' $detail
    # Reborn: retired authored-count/no-auto-command chatter must stay out of the readable log even if an older backend still sends it.
    if(@(Write-SdkCompilerLine 'Authored asset count: 19; no automatic LogicCommand generation.' $detail).Count-ne 0){throw 'Retired authored asset count reached the readable log.'}
    $text=$shown -join "`n";$raw=Get-Content -LiteralPath $detail -Raw
    if($text-notmatch '(?m)^Error: C:\\Mod Files\\base\.xml\(17,9\): Unsupported field\.' -or $raw-notmatch 'ERROR: C:\\Mod Files\\base\.xml\(17,9\):'){throw 'Original XML source coordinates lost in build log.'}
    if($text-notmatch '\] port_ctr\.w3x: Compiling W3DContainer:RebornPort'){throw 'W3X compilation progress was hidden.'}
    # Reborn: phase duration is followed by the local completion time, with a valid 24-hour clock in square brackets.
    if($text-notmatch 'Step 6: Building static data' -or $text-notmatch '(?m)^Step time taken: \d\d:\d\d:\d\d\.\d{7} \[(?:[01]\d|2[0-3]):[0-5]\d:[0-5]\d\]\r?$' -or $text-notmatch '(?m)^Error: Source.xml:4:' -or $text-notmatch '(?m)^Warning: ALPHA005' -or $text-notmatch '\[\d\d:\d\d:\d\d\.\d{7}\] Source path \[mod\] DATA:Test\.xml'){throw 'Phase, time, source or severity display differs.'}
    # Reborn: EA-style source/asset progress is shown with the wrapper clock; malformed look-alikes stay in the raw log only.
    if($text-notmatch "\] Loading 'file:///C:/Mod%20Files/Soviet/SovietRefinery\.xml'" -or $text-notmatch '\] sovietrefinery\.xml: Compiling GameObject:SovietRefinery \[942fff2d:0123abcd\]' -or $text-notmatch '\] Validation completed: 17 source' -or $text-notmatch '\] Compilation started: 2 asset' -or $text-notmatch '\] Compilation completed: 2 asset' -or $text-notmatch '\] Resolving references: 2 assets' -or $text-notmatch '\] Linking .* mod Stream complete' -or $text-notmatch '\] Statistics GameObject: Instances Processed 2,' -or $text-match 'notes\.txt: Compiling' -or $raw-notmatch 'notes\.txt: Compiling'){throw 'EA-style Loading/Compiling display differs.'}
    # Reborn: the toolchain tag must not appear in the user's display (it reads like the selected mod name), but stays in the raw log.
    if($text-cmatch '(?m)(?:^|\] |: )Reborn:' -or $text-cnotmatch '(?m)^Error: tagged internal failure' -or $raw-notmatch 'Reborn: source path' -or $raw-notmatch 'ERROR: Reborn: tagged internal failure'){throw 'Toolchain tag leaked into display or was lost from the raw log.'}
    if($text-match 'RuntimeReady|Internal.Namespace|internal long diagnostic evidence' -or $text-notmatch 'Native graph: PASS' -or $raw-notmatch 'internal long diagnostic evidence' -or $raw-notmatch 'RuntimeReady' -or $raw-notmatch 'Internal.Namespace' -or $raw-notmatch 'ERROR: Source.xml:4:'){throw 'Readable/full diagnostic separation lost evidence.'}
    if(($script:BuildLogLines -join "`n")-cne $text){throw 'Saved/displayed readable lines differ.'}
    # Reborn: repeated audit/preparation selections are shown once, while changed winners and repeated diagnostics stay visible.
    $repeated=@(Write-SdkCompilerLine 'Reborn: source path [mod] DATA:Test.xml -> Test.xml' $detail;Write-SdkCompilerLine 'Source path [mod] DATA:Test.xml -> Test.xml' $detail)
    if($repeated.Count-ne 0){throw 'Duplicate source selection reached the readable log.'}
    $changed=@(Write-SdkCompilerLine 'Reborn: source path [sdk] DATA:Test.xml -> Stock.xml' $detail;Write-SdkCompilerLine 'Reborn: source path [mod] DATA:Other.xml -> Test.xml' $detail;Write-SdkCompilerLine 'Warning: repeated diagnostic' $detail;Write-SdkCompilerLine 'Warning: repeated diagnostic' $detail)
    if($changed.Count-ne 4){throw 'Distinct source selection or repeated warning was suppressed.'}
    if(@((Get-Content -LiteralPath $detail) | Where-Object {$_-ceq 'Reborn: source path [mod] DATA:Test.xml -> Test.xml'}).Count-ne 2){throw 'Raw duplicate source evidence was lost.'}
    # Reborn: a new build in the same PowerShell session must show its first source selection again.
    . (Join-Path $PSScriptRoot 'Ra3Ep1BuildLog.ps1')
    if(@(Write-SdkCompilerLine 'Reborn: source path [mod] DATA:Test.xml -> Test.xml' $detail).Count-ne 1){throw 'Source selection display was not reset for the next build.'}
    # Reborn: copying retail dependencies must remain visible in Studio, without turning successful resolution into another warning.
    if(@(Write-SdkCompilerLine 'Resolving WorldBuilder assets: 20 added with dependencies.' $detail;Write-SdkCompilerLine 'WorldBuilder asset added: W3DContainer:Fixture' $detail).Count-ne 2){throw 'WorldBuilder resolution progress was hidden.'}
    # Reborn: loading native row grammar is ordinary fast-build progress, never a successful developer test claim.
    $metadata=@(Write-SdkCompilerLine 'Native metadata ready: Scripted FXEvent (6 stock records).' $detail);if($metadata.Count-ne 1 -or $metadata[0]-match 'PASS|Warning:'){throw 'Fast metadata progress was hidden or mislabeled.'}
    # Reborn: final cross-stream usage summaries remain visible as ordinary progress.
    if(@(Write-SdkCompilerLine 'Asset usage audit: 19 packaged assets; 0 without static references.' $detail).Count-ne 1){throw 'Asset usage summary was hidden.'}
    # Reborn: validation progress stays ordinary text; only a post-resolution failure uses the warning classifier.
    $pending=@(Write-SdkCompilerLine "WorldBuilder asset pending 'W3DContainer:Fixture' (referenced by GameObject:Owner) Source: Fixture.xml(7,3)." $detail)
    if($pending.Count-ne 1 -or $pending[0]-match 'Warning:'){throw 'Pending WorldBuilder progress was hidden or classified as a warning.'}
    if(@(Write-SdkCompilerLine "Warning: WORLDBUILDERASSET 'W3DContainer:Missing' remains unresolved after WorldBuilder resolution." $detail).Count-ne 1){throw 'Post-resolution warning was hidden.'}
    Write-Output 'Reborn: build log presentation: PASS (phases, timings, severity, source, EA-style loading/compiling lines, JSON/stack separation, raw evidence).'
}finally{
    # Reborn: remove only the one explicitly owned file and empty temporary directory, without recursive deletion.
    if(Test-Path -LiteralPath $detail){Remove-Item -LiteralPath $detail};[IO.Directory]::Delete($scratch,$false)
}
