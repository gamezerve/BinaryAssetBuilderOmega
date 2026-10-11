# Reborn: EA BuildStudio alpha backend; compile admitted selected XML through real Core without install/launch.
[CmdletBinding()]
param([Parameter(Mandatory)][string]$ModXml,[switch]$ValidateOnly,
    [string]$StaticManifest='D:\TEMP\Red Alert 3 Uprising Source Data\Static Data\data\static.manifest',
    [string]$CompilerDirectory,[string]$OutputRoot)
$ErrorActionPreference='Stop'
$repo=Split-Path -Parent $PSScriptRoot
#-------------------------------------------------------------------------------------------------
# Reborn: reject redirected output/compiler ancestry before using an explicitly supplied local directory.
#-------------------------------------------------------------------------------------------------
function Test-AlphaDirectory([string]$Path){
    if(-not [IO.Path]::IsPathFullyQualified($Path)){throw 'Alpha directory must be absolute.'}
    $item=Get-Item -LiteralPath $Path
    if(-not $item.PSIsContainer){throw 'Alpha directory must exist.'}
    for(;$null-ne $item;$item=$item.Parent){
        if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected alpha directory refused.'}
    }
}
try {
    if($PSVersionTable.PSVersion.Major-lt 7){throw 'PowerShell 7 is required.'}
    Write-Output 'Warning: ALPHA001 Partial SDK support. Unsupported assets will fail this build, not be skipped.'
    $ModXml=[IO.Path]::GetFullPath($ModXml)
    for($item=Get-Item -LiteralPath $ModXml;$null-ne $item;$item=if($item.PSIsContainer){$item.Parent}else{$item.Directory}){
        if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected mod input refused.'}
    }
    $file=[IO.File]::Open($ModXml,[IO.FileMode]::Open,[IO.FileAccess]::Read,[IO.FileShare]::Read)
    try {
        if($file.Length-eq 0 -or $file.Length-gt 16KB){throw 'Alpha mod XML is limited to 16 KiB.'}
        $bytes=[byte[]]::new($file.Length);$file.ReadExactly($bytes)
        # Reborn: disallow DTDs and includes; accept one standalone armor or one declaration containing only that armor.
        $settings=[Xml.XmlReaderSettings]::new();$settings.DtdProcessing=[Xml.DtdProcessing]::Prohibit;$settings.XmlResolver=$null;$settings.MaxCharactersInDocument=16384
        $settings.IgnoreWhitespace=$true;$settings.IgnoreComments=$true
        $stream=[IO.MemoryStream]::new($bytes,$false);$reader=[Xml.XmlReader]::Create($stream,$settings)
        try{$xml=[Xml.XmlDocument]::new();$xml.XmlResolver=$null;$xml.Load($reader)}finally{$reader.Dispose();$stream.Dispose()}
        $root=$xml.DocumentElement
        # Reborn: a pair stays a declaration for its separate admission/compile path; the historical one-member path keeps its exact snapshot.
        $isPair=$false
        if($root.NamespaceURI-cne 'uri:ea.com:eala:asset'){throw 'Expected EA asset namespace.'}
        if($root.LocalName-ceq 'AssetDeclaration'){
            if(@($root.Attributes|Where-Object Name -NE 'xmlns').Count-ne 0 -or $root.ChildNodes.Count -notin @(1,2) -or @($root.ChildNodes|Where-Object {$_-isnot [Xml.XmlElement]}).Count-ne 0){throw 'Alpha declaration requires one or two supported armors; includes and other types are not build-enabled.'}
            if($root.ChildNodes.Count-eq 2){$isPair=$true}else{$root=$root.FirstChild}
        }
        if(-not $isPair -and $root.LocalName-cne 'ArmorTemplate'){throw ('Asset type is not build-enabled in this alpha: '+$root.LocalName)}
        # Reborn: keep all generated validation artifacts isolated; original selected-mod XML remains read-only.
        # Reborn: explicit standalone output stays under a pre-existing checked root; developer defaults remain repository-local.
        if(-not $OutputRoot){$OutputRoot=Join-Path $repo 'artifacts'}
        Test-AlphaDirectory $OutputRoot
        $directory=Join-Path $OutputRoot ('RebornAlphaInput-'+[guid]::NewGuid().ToString('N'))
        $null=New-Item -ItemType Directory -Path $directory
        $snapshot=Join-Path $directory 'selected-armor.xml'
        [IO.File]::WriteAllText($snapshot,$root.OuterXml)
        $exe=Join-Path $repo 'source/BinaryAssetBuilder.ManifestInspector/bin/x86/Release/net8.0/BinaryAssetBuilder.ManifestInspector.exe'
        # Reborn: prebuilt mode is explicit for isolated alpha workspaces; normal repository mode still builds current source.
        if($CompilerDirectory){
            Test-AlphaDirectory $CompilerDirectory
            $exe=Join-Path $CompilerDirectory 'BinaryAssetBuilder.ManifestInspector.exe'
            if(-not (Test-Path -LiteralPath $exe -PathType Leaf)){throw 'Prebuilt alpha compiler is missing.'}
            if(((Get-Item -LiteralPath $exe).Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected alpha compiler refused.'}
        }else{
            $log=Join-Path $directory 'compiler-build.log'
            & dotnet build (Join-Path $repo 'source/BinaryAssetBuilder.ManifestInspector/BinaryAssetBuilder.ManifestInspector.csproj') --no-restore -c Release -p:Platform=x86 -m:1 -nr:false -v:q *> $log
            if($LASTEXITCODE-ne 0){throw ('Compiler build failed; see '+$log)}
        }
        # Reborn: two assets use their own frozen-document capability and receipt; never concatenate independently built single-asset streams.
        if($isPair){
            $json=@(& $exe armor-pair-authoring-preflight $snapshot (Join-Path $directory 'admitted'))
            if($LASTEXITCODE-ne 0){$json|Write-Output;throw 'Pair XML failed admission.'}
            $admission=($json-join [Environment]::NewLine)|ConvertFrom-Json
            if($ValidateOnly){Write-Output ('Reborn: pair input admitted; no package built. '+$directory);return}
            # Reborn: distinguish observed pair revisions from an arbitrary newly authored package's unproven runtime behavior.
            Write-Output 'Warning: ALPHA002 Two supported yard armors share one stream; 18 editable percentages each. Soviet GUN=5000 and Allied GUN=5000 have user-observed combined-stream evidence in separate revisions. Allied GUN=250 and other edits remain unconfirmed; this new build is not automatically gameplay-verified.'
            $output=Join-Path $directory 'build'
            & $exe armor-pair-authored-build $snapshot $output $StaticManifest
            if($LASTEXITCODE-ne 0){throw 'Pair Core/package build failed.'}
            $buildReceipt=Join-Path $output 'pair-build.json'
            $build=Get-Content -LiteralPath $buildReceipt -Raw|ConvertFrom-Json
            if($admission.ArmorCount-ne 2 -or $build.ArmorCount-ne 2 -or @($admission.Assets).Count-ne 2 -or @($build.Assets).Count-ne 2 -or -not $build.ActualCoreOutputUsed -or -not $build.AuthoredPayloadVerified -or -not $build.StockBaselineCompared){throw 'Pair receipt shape or compiler proof differs.'}
            foreach($member in $admission.Assets){
                $compiled=@($build.Assets|Where-Object ArmorId -CEQ $member.ArmorId)
                if($compiled.Count-ne 1 -or $member.EditableFloatCount-ne 18 -or $compiled[0].EditableFloatCount-ne 18 -or @($member.Percentages.PSObject.Properties).Count-ne 18 -or @($compiled[0].Percentages.PSObject.Properties).Count-ne 18){throw 'Pair member receipt differs.'}
                foreach($property in $member.Percentages.PSObject.Properties){
                    if($null-eq $compiled[0].Percentages.PSObject.Properties[$property.Name] -or $compiled[0].Percentages.PSObject.Properties[$property.Name].Value-ne $property.Value){throw 'Pair compiled percentage differs.'}
                }
            }
            $receipt=[pscustomobject]@{ArmorCount=2;Assets=$build.Assets;SelectedModXml=$ModXml;SelectedModXmlSha256=([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($bytes)));BuildReceipt=$buildReceipt;CompilerPath=$exe;PrebuiltCompilerUsed=[bool]$CompilerDirectory;InstalledFilesModified=$false;GameExecuted=$false}
            [IO.File]::WriteAllText((Join-Path $directory 'alpha-build.json'),($receipt|ConvertTo-Json -Depth 6))
            Write-Output ('Reborn: build succeeded. BIG: '+$build.PackagePath)
            Write-Output ('Reborn: config: '+$build.ConfigPath)
            return
        }
        $json=@(& $exe armor-authoring-preflight $snapshot (Join-Path $directory 'admitted'))
        if($LASTEXITCODE-ne 0){$json|Write-Output;throw 'Selected XML failed alpha armor admission.'}
        $admission=($json-join [Environment]::NewLine)|ConvertFrom-Json
        if($ValidateOnly){Write-Output ('Reborn: selected mod input admitted; validation only, no package built. '+$directory);return}
        Write-Output 'Warning: ALPHA002 Standalone AlliedConYardArmor or SovietConYardArmor: thirteen ordered coefficients and five optional scalars, integer 0..10000. Allied GUN/CANNON and Soviet GUN have user-observed gameplay evidence; other edits require runtime testing.'
        $output=Join-Path $directory 'build'
        & $exe armor-authored-build $snapshot $output $StaticManifest
        if($LASTEXITCODE-ne 0){throw 'Authored XML/Core/package build failed.'}
        $buildReceipt=Join-Path $output 'authored-build.json'
        $build=Get-Content -LiteralPath $buildReceipt -Raw|ConvertFrom-Json
        if(-not $build.ActualCoreOutputUsed -or -not $build.AuthoredPayloadVerified -or $build.GunPercent-ne $admission.GunPercent){throw 'Authored build receipt differs from admitted input.'}
        # Reborn: require the authored identity at both stages; an old compiler cannot silently relabel another faction's package.
        if($admission.ArmorId-cne $root.GetAttribute('id') -or $build.ArmorId-cne $admission.ArmorId){throw 'Compiled armor identity differs from selected input.'}
        # Reborn: compare the whole expanded authored value set, not only GUN; old narrow compiler receipts fail closed.
        if($build.EditableFloatCount-ne 18 -or $admission.EditableFloatCount-ne 18 -or @($build.Percentages.PSObject.Properties).Count-ne 18 -or @($admission.Percentages.PSObject.Properties).Count-ne 18){throw 'Expanded armor receipt must describe all eighteen fields.'}
        foreach($property in $admission.Percentages.PSObject.Properties){
            if($null-eq $build.Percentages.PSObject.Properties[$property.Name] -or $build.Percentages.PSObject.Properties[$property.Name].Value-ne $property.Value){throw 'Compiled percentage receipt differs from admitted input.'}
        }
        $receipt=[pscustomobject]@{SelectedModXml=$ModXml;SelectedModXmlSha256=([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($bytes)));AdmissionReport=(Join-Path $directory 'admitted/admission.json');BuildReceipt=$buildReceipt;GunPercent=$build.GunPercent;EditableFloatCount=18;Percentages=$build.Percentages;CompilerPath=$exe;CompilerSha256=(Get-FileHash -LiteralPath $exe -Algorithm SHA256).Hash;PrebuiltCompilerUsed=[bool]$CompilerDirectory;SelectedInputAdmitted=$true;CanonicalFixedRecipeUsed=$false;InstalledFilesModified=$false;GameExecuted=$false}
        # Reborn: expose the verified selected identity in the final alpha receipt as well as the admission/Core receipts.
        $receipt|Add-Member -NotePropertyName ArmorId -NotePropertyValue $build.ArmorId
        [IO.File]::WriteAllText((Join-Path $directory 'alpha-build.json'),($receipt|ConvertTo-Json))
        Write-Output ('Reborn: build succeeded. BIG: '+$build.PackagePath)
        Write-Output ('Reborn: config: '+$build.ConfigPath)
        Write-Output ('Reborn: alpha receipt: '+(Join-Path $directory 'alpha-build.json'))
    }finally{$file.Dispose()}
}catch{
    Write-Output ('Error: ALPHA100 '+$_.Exception.Message)
    # Reborn: EA Studio ignores the child exit code and only Critical sets stepFailed; retain the detailed Errors row and explicitly fail the GUI step.
    Write-Output 'Critical: ALPHA101 Alpha build failed; no successful package publication. See ALPHA100 for details.'
    exit 1
}
