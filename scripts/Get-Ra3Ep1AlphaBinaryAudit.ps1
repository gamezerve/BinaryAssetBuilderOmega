# Reborn: bounded read-only PE/debug/path audit of the eight alpha compiler/runtime files; do not load executable code or claim redistribution rights.
[CmdletBinding()]
param([Parameter(Mandatory)][string]$CompilerDirectory,[Parameter(Mandatory)][string]$OutputPath)
$ErrorActionPreference='Stop'
if($PSVersionTable.PSVersion.Major-lt 7){throw 'PowerShell 7 is required.'}
$CompilerDirectory=[IO.Path]::GetFullPath($CompilerDirectory);$OutputPath=[IO.Path]::GetFullPath($OutputPath)
if(Test-Path -LiteralPath $OutputPath){throw 'Audit output exists; nothing overwritten.'}
foreach($path in @($CompilerDirectory,(Split-Path -Parent $OutputPath))){
    for($item=Get-Item -LiteralPath $path;$null-ne $item;$item=$item.Parent){if(($item.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Redirected audit path refused.'}}
}
$rows=[Collections.Generic.List[object]]::new()
foreach($name in @('BinaryAssetBuilder.ManifestInspector.exe','BinaryAssetBuilder.ManifestInspector.dll',
    'BinaryAssetBuilder.Core.dll','BinaryAssetBuilder.Utility.dll','BinaryAssetBuilder.XmlCompiler.dll','SageBinaryData.dll',
    'BinaryAssetBuilder.ManifestInspector.deps.json','BinaryAssetBuilder.ManifestInspector.runtimeconfig.json')){
    $path=Join-Path $CompilerDirectory $name;$file=Get-Item -LiteralPath $path
    if($file.Length-gt 16MB -or ($file.Attributes-band [IO.FileAttributes]::ReparsePoint)-ne 0){throw 'Audit file oversized or redirected.'}
    $bytes=[IO.File]::ReadAllBytes($path)
    # Reborn: count bounded ASCII/UTF16 path hints; report classifications, not private path values.
    $ascii=[Text.Encoding]::ASCII.GetString($bytes);$unicode=[Text.Encoding]::Unicode.GetString($bytes)
    $privatePattern='(?i)(?:[A-Z]:[\\/]Users[\\/]|[A-Z]:[\\/]OneDrive[\\/]|[\\/]\.codex[\\/]|\.chatgpt-projects[\\/])'
    $privateHints=[regex]::Matches($ascii,$privatePattern).Count+[regex]::Matches($unicode,$privatePattern).Count
    $debugRows=[Collections.Generic.List[object]]::new();$references=[Collections.Generic.List[string]]::new()
    $imports=[Collections.Generic.List[string]]::new();$machine=$null;$managed=$false
    if($name.EndsWith('.dll') -or $name.EndsWith('.exe')){
        $stream=[IO.MemoryStream]::new($bytes,$false)
        $pe=[Reflection.PortableExecutable.PEReader]::new($stream)
        try{
            $machine=$pe.PEHeaders.CoffHeader.Machine.ToString();$managed=$pe.HasMetadata
            foreach($entry in $pe.ReadDebugDirectory()){
                $debugPath=$null;$absolute=$false
                if($entry.Type-eq [Reflection.PortableExecutable.DebugDirectoryEntryType]::CodeView){
                    $data=$pe.ReadCodeViewDebugDirectoryData($entry);$absolute=[IO.Path]::IsPathRooted($data.Path)
                    $debugPath=if($absolute){'[absolute path redacted]'}else{$data.Path}
                }
                $debugRows.Add([pscustomobject]@{Type=$entry.Type.ToString();Path=$debugPath;AbsolutePath=$absolute})
            }
            if($managed){
                $metadata=[Reflection.Metadata.PEReaderExtensions]::GetMetadataReader($pe)
                foreach($handle in $metadata.AssemblyReferences){$reference=$metadata.GetAssemblyReference($handle);$references.Add($metadata.GetString($reference.Name))}
                foreach($handle in $metadata.MethodDefinitions){
                    $method=$metadata.GetMethodDefinition($handle)
                    if(($method.Attributes-band [Reflection.MethodAttributes]::PinvokeImpl)-ne 0){
                        $import=$method.GetImport();$module=$metadata.GetModuleReference($import.Module)
                        $imports.Add($metadata.GetString($module.Name)+'!'+$metadata.GetString($import.Name))
                    }
                }
            }
        }finally{$pe.Dispose();$stream.Dispose()}
    }
    $rows.Add([pscustomobject]@{File=$name;Bytes=$bytes.Length;Sha256=(Get-FileHash -LiteralPath $path).Hash;
        Machine=$machine;Managed=$managed;PrivatePathHintCount=$privateHints;DebugEntries=$debugRows;
        AssemblyReferences=$references;NativeMethodImports=@($imports|Sort-Object -Unique)})
}
$deps=Get-Content -LiteralPath (Join-Path $CompilerDirectory 'BinaryAssetBuilder.ManifestInspector.deps.json') -Raw|ConvertFrom-Json
$libraries=@($deps.libraries.PSObject.Properties|ForEach-Object {[pscustomobject]@{Name=$_.Name;Type=$_.Value.type}})
$report=[pscustomobject]@{Files=$rows;DeclaredLibraries=$libraries;
    FilesWithPrivateHints=@($rows|Where-Object PrivatePathHintCount -GT 0|ForEach-Object File);
    AbsoluteDebugPaths=@($rows|Where-Object {@($_.DebugEntries|Where-Object AbsolutePath).Count-gt 0}|ForEach-Object File);
    Scope='Eight bounded compiler/runtime files; ASCII/UTF16 hints and PE metadata, not exhaustive privacy/license analysis.';
    PublicRedistributionReviewed=$false;GameExecuted=$false;CodeExecutedFromAuditedFiles=$false}
[IO.File]::WriteAllText($OutputPath,($report|ConvertTo-Json -Depth 8))
$report
