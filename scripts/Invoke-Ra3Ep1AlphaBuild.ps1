# Reborn: EA BuildStudio backend prototype; warn about alpha limitations, refuse unsupported input, build only the validated fixed recipe without install/launch.
[CmdletBinding()]
param([Parameter(Mandatory)][string]$ModXml,[switch]$ValidateOnly)
$ErrorActionPreference='Stop'
$repo=Split-Path -Parent $PSScriptRoot
try {
    if($PSVersionTable.PSVersion.Major-lt 7){throw 'PowerShell 7 is required.'}
    Write-Output 'WARNING ALPHA001: Partial SDK support. Unsupported assets will fail this build, not be skipped.'
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
        $directory=Join-Path $repo ('artifacts/RebornAlphaInput-'+[guid]::NewGuid().ToString('N'))
        $null=New-Item -ItemType Directory -Path $directory
        $snapshot=Join-Path $directory 'selected-armor.xml'
        [IO.File]::WriteAllText($snapshot,$root.OuterXml)
        $exe=Join-Path $repo 'source/BinaryAssetBuilder.ManifestInspector/bin/x86/Release/net8.0/BinaryAssetBuilder.ManifestInspector.exe'
        if(-not (Test-Path -LiteralPath $exe -PathType Leaf)){throw 'Build the current inspector before using this prototype.'}
        $json=@(& $exe armor-authoring-preflight $snapshot (Join-Path $directory 'admitted'))
        if($LASTEXITCODE-ne 0){$json|Write-Output;throw 'Selected XML failed alpha armor admission.'}
        $admission=($json-join [Environment]::NewLine)|ConvertFrom-Json
        if($admission.GunPercent-ne 5000){throw 'GUN input is valid, but changed-value packaging is not enabled yet. Current package-enabled coefficient is 5000.'}
        if($ValidateOnly){Write-Output ('Reborn: selected mod input admitted; validation only, no package built. '+$directory);return}
        Write-Output 'WARNING ALPHA002: This prototype rebuilds the canonical fixed recipe after admitting equivalent selected XML. It does not yet build arbitrary asset declarations.'
        $build=& (Join-Path $PSScriptRoot 'Build-Ra3Ep1ArmorMod.ps1')
        $receipt=[pscustomobject]@{SelectedModXml=$ModXml;SelectedModXmlSha256=([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($bytes)));AdmissionReport=(Join-Path $directory 'admitted/admission.json');BuildReceipt=$build.ReceiptPath;SelectedInputAdmitted=$true;CanonicalFixedRecipeUsed=$true;InstalledFilesModified=$false;GameExecuted=$false}
        [IO.File]::WriteAllText((Join-Path $directory 'alpha-build.json'),($receipt|ConvertTo-Json))
        Write-Output ('Reborn: build succeeded. BIG: '+$build.PackagePath)
        Write-Output ('Reborn: config: '+$build.ConfigPath)
        Write-Output ('Reborn: alpha receipt: '+(Join-Path $directory 'alpha-build.json'))
    }finally{$file.Dispose()}
}catch{
    Write-Output ('ERROR ALPHA100: '+$_.Exception.Message)
    exit 1
}
