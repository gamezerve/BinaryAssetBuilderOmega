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
        if($root.NamespaceURI-cne 'uri:ea.com:eala:asset'){throw 'Expected EA asset namespace.'}
        if($root.LocalName-ceq 'AssetDeclaration'){
            if(@($root.Attributes|Where-Object Name -NE 'xmlns').Count-ne 0 -or $root.ChildNodes.Count-ne 1 -or $root.FirstChild-isnot [Xml.XmlElement]){throw 'Alpha declaration must contain exactly one standalone armor; includes and other types are not build-enabled.'}
            $root=$root.FirstChild
        }
        if($root.LocalName-cne 'ArmorTemplate'){throw ('Asset type is not build-enabled in this alpha: '+$root.LocalName)}
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
        $json=@(& $exe armor-authoring-preflight $snapshot (Join-Path $directory 'admitted'))
        if($LASTEXITCODE-ne 0){$json|Write-Output;throw 'Selected XML failed alpha armor admission.'}
        $admission=($json-join [Environment]::NewLine)|ConvertFrom-Json
        if($ValidateOnly){Write-Output ('Reborn: selected mod input admitted; validation only, no package built. '+$directory);return}
        Write-Output 'Warning: ALPHA002 Only AlliedConYardArmor GUN Percent 0..10000 is build-enabled; all thirteen flattened entries are required. New coefficient runtime behavior is not independently proven.'
        $output=Join-Path $directory 'build'
        & $exe armor-authored-build $snapshot $output $StaticManifest
        if($LASTEXITCODE-ne 0){throw 'Authored XML/Core/package build failed.'}
        $buildReceipt=Join-Path $output 'authored-build.json'
        $build=Get-Content -LiteralPath $buildReceipt -Raw|ConvertFrom-Json
        if(-not $build.ActualCoreOutputUsed -or -not $build.AuthoredPayloadVerified -or $build.GunPercent-ne $admission.GunPercent){throw 'Authored build receipt differs from admitted input.'}
        $receipt=[pscustomobject]@{SelectedModXml=$ModXml;SelectedModXmlSha256=([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($bytes)));AdmissionReport=(Join-Path $directory 'admitted/admission.json');BuildReceipt=$buildReceipt;GunPercent=$build.GunPercent;CompilerPath=$exe;CompilerSha256=(Get-FileHash -LiteralPath $exe -Algorithm SHA256).Hash;PrebuiltCompilerUsed=[bool]$CompilerDirectory;SelectedInputAdmitted=$true;CanonicalFixedRecipeUsed=$false;InstalledFilesModified=$false;GameExecuted=$false}
        [IO.File]::WriteAllText((Join-Path $directory 'alpha-build.json'),($receipt|ConvertTo-Json))
        Write-Output ('Reborn: build succeeded. BIG: '+$build.PackagePath)
        Write-Output ('Reborn: config: '+$build.ConfigPath)
        Write-Output ('Reborn: alpha receipt: '+(Join-Path $directory 'alpha-build.json'))
    }finally{$file.Dispose()}
}catch{
    Write-Output ('Error: ALPHA100 '+$_.Exception.Message)
    exit 1
}
