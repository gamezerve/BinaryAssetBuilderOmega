# Reborn: recompile the complete reviewed Conscript with six bounded scalar edits through Core; never export a cached BIG as a new build.
[CmdletBinding()]
param([Parameter(Mandatory)][string]$ModXml,[Parameter(Mandatory)][string]$StaticManifest,[Parameter(Mandatory)][string]$GlobalManifest,[Parameter(Mandatory)][string]$CompilerDirectory,[Parameter(Mandatory)][string]$OutputRoot,[switch]$ValidateOnly)
$ErrorActionPreference='Stop';$sdk=Join-Path (Split-Path -Parent $PSScriptRoot) 'alpha'
try {
    $settings=@{};foreach($line in Get-Content -LiteralPath (Join-Path $sdk 'uprising-alpha.paths')){if($line -and -not $line.StartsWith('#')){$pair=$line.Split('=',2);$settings[$pair[0]]=$pair[1]}}
    if(-not $settings.UprisingXmlRoot -or -not(Test-Path -LiteralPath $settings.UprisingXmlRoot -PathType Container)){throw 'Conscript requires UprisingXmlRoot in alpha/uprising-alpha.paths.'}
    # Reborn: exact canonical source admission is deliberately narrower than arbitrary GameObject authoring; unknown edits must fail before expensive qualification.
    $readerSettings=[Xml.XmlReaderSettings]::new();$readerSettings.DtdProcessing=[Xml.DtdProcessing]::Prohibit;$readerSettings.XmlResolver=$null;$readerSettings.MaxCharactersInDocument=1048576
    $reader=[Xml.XmlReader]::Create($ModXml,$readerSettings);try{$xml=[Xml.XmlDocument]::new();$xml.PreserveWhitespace=$true;$xml.XmlResolver=$null;$xml.Load($reader)}finally{$reader.Dispose()}
    $sourceHash=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($xml.DocumentElement.OuterXml)))
    # Reborn: normalize only six reviewed numeric attributes before the existing full-source fingerprint; unsupported edits still fail closed.
    $shape=$xml.CloneNode($true);$ns=[Xml.XmlNamespaceManager]::new($shape.NameTable);$ns.AddNamespace('a','uri:ea.com:eala:asset')
    $fields=@(@('.','BuildTime','1',0.1,600),@('a:ObjectResourceInfo/a:BuildCost','Amount','100',0,100000),@('a:Body/a:ActiveBody','MaxHealth','100.0',1,100000),@('a:VisionInfo','VisionRange','200',1,5000),@('a:VisionInfo','ShroudClearingRange','500.0',1,5000),@('a:LocomotorSet','Speed','50',1,500))
    foreach($field in $fields){$nodes=$shape.DocumentElement.SelectNodes($field[0],$ns);if($nodes.Count-ne 1 -or -not $nodes[0].HasAttribute($field[1])){throw ('Missing/ambiguous Conscript scalar: '+$field[1])};$text=$nodes[0].GetAttribute($field[1]);$value=0.0;if(-not [double]::TryParse($text,[Globalization.NumberStyles]::Float,[Globalization.CultureInfo]::InvariantCulture,[ref]$value) -or [double]::IsNaN($value) -or [double]::IsInfinity($value) -or $value-lt $field[3] -or $value-gt $field[4] -or ($field[1]-ceq 'Amount' -and $text-cnotmatch '^[0-9]+$')){throw ('Invalid/out-of-range Conscript scalar: '+$field[1])};$nodes[0].SetAttribute($field[1],$field[2])}
    $shapeHash=[Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([Text.Encoding]::UTF8.GetBytes($shape.DocumentElement.OuterXml)))
    if($shapeHash-cne 'B7EDFA37B5F1A26FA2ED5610BFEB2B2F05496B5BF150B9E068F9A4AB066DC6D6'){throw 'Only the complete original-ID Conscript with six reviewed scalar edits is admitted; other edits remain unsupported.'}
    Write-Output 'Warning: ALPHA005 Conscript: BuildTime, BuildCost Amount, MaxHealth, VisionRange, ShroudClearingRange and LocomotorSet Speed are editable. Other edits/new identities remain unsupported; new combinations need gameplay testing.'
    $scratch=Join-Path $OutputRoot 'temp';$null=New-Item -ItemType Directory -Path $scratch
    $previousTemp=$env:TEMP;$previousTmp=$env:TMP
    try {
        # Reborn: diagnostics create temporary qualification fixtures; contain all of them in this build's owned work generation.
        $env:TEMP=$scratch;$env:TMP=$scratch
        & (Join-Path $CompilerDirectory 'BinaryAssetBuilder.ManifestInspector.exe') conscript-alpha-build $settings.UprisingXmlRoot $StaticManifest $GlobalManifest $ModXml (Join-Path $OutputRoot 'conscript')
        if($LASTEXITCODE-ne 0){throw 'Conscript source/native/metadata/stream qualification failed.'}
    } finally {$env:TEMP=$previousTemp;$env:TMP=$previousTmp}
    $receiptPath=Join-Path $OutputRoot 'conscript/override/trial-plan.json';$receipt=Get-Content -LiteralPath $receiptPath -Raw|ConvertFrom-Json
    $package=Join-Path $OutputRoot 'conscript/override/RebornConscriptProbe.big'
    if($receipt.BuildTime-ne [double]::Parse($xml.DocumentElement.GetAttribute('BuildTime'),[Globalization.CultureInfo]::InvariantCulture) -or $receipt.StreamProfile-cne 'bounded-conscript-scalars' -or $receipt.NewIdentity -or -not $receipt.OfflineValidated -or (Get-FileHash -LiteralPath $package -Algorithm SHA256).Hash-cne $receipt.ArchiveSha256 -or $receipt.SourceSha256-cne $sourceHash){throw 'Fresh Conscript package/readback receipt differs.'}
    # Reborn: the final receipt must describe every authored value, not just BuildTime and an internally consistent package hash.
    $actualNs=[Xml.XmlNamespaceManager]::new($xml.NameTable);$actualNs.AddNamespace('a','uri:ea.com:eala:asset')
    foreach($field in $fields){$authored=[double]::Parse($xml.DocumentElement.SelectSingleNode($field[0],$actualNs).GetAttribute($field[1]),[Globalization.CultureInfo]::InvariantCulture);$observed=$receipt.EditableValues.PSObject.Properties[$field[1]];if($null-eq $observed -or [math]::Abs([double]$observed.Value-$authored)-gt [math]::Max(0.00001,[math]::Abs($authored)*0.000001)){throw ('Fresh scalar receipt differs: '+$field[1])}}
    if($ValidateOnly){Write-Output 'Reborn: complete Conscript qualification passed; validation-only, no final publication.';return}
    [IO.File]::WriteAllText((Join-Path $OutputRoot 'alpha-build.json'),([pscustomobject]@{BuildReceipt=$receiptPath;PackagePath=$package;Profile='bounded-conscript-scalars';GameExecuted=$false;InstalledFilesModified=$false}|ConvertTo-Json))
} catch {Write-Output ('Error: ALPHA120 '+$_.Exception.Message);Write-Output 'Critical: ALPHA121 Conscript alpha build failed.';exit 1}
