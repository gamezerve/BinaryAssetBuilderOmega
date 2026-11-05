# Reborn: versioned Documents publication is separate from private compiler staging and never touches a game installation.

# Reborn: OneDrive cloud placeholders are not path redirections; inspect the actual tag instead of rejecting every ReparsePoint attribute.
if(-not ('Reborn.PackageReparseTag' -as [type])){
    Add-Type -TypeDefinition @'
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
namespace Reborn {
    // Reborn: read only reparse metadata without following a junction, symlink or cloud placeholder.
    public static class PackageReparseTag {
        [DllImport("kernel32.dll", CharSet=CharSet.Unicode, SetLastError=true)]
        private static extern SafeFileHandle CreateFile(string path, uint access, uint share, IntPtr security, uint mode, uint flags, IntPtr template);
        [DllImport("kernel32.dll", SetLastError=true)]
        private static extern bool DeviceIoControl(SafeFileHandle handle, uint code, IntPtr input, uint inputLength, byte[] output, uint outputLength, out uint returned, IntPtr overlapped);
        // Reborn: allow only the Microsoft cloud-tag family; name-surrogate and unknown reparse tags remain refused.
        public static bool IsCloud(string path) {
            using (var handle=CreateFile(path,0,7,IntPtr.Zero,3,0x02200000,IntPtr.Zero)) {
                if(handle.IsInvalid) throw new Win32Exception(Marshal.GetLastWin32Error());
                byte[] bytes=new byte[16384]; uint returned;
                if(!DeviceIoControl(handle,0x000900A8,IntPtr.Zero,0,bytes,(uint)bytes.Length,out returned,IntPtr.Zero)) throw new Win32Exception(Marshal.GetLastWin32Error());
                return returned>=8 && (BitConverter.ToUInt32(bytes,0)&0xFFFF0FFFu)==0x9000001Au;
            }
        }
    }
}
'@
}

#-------------------------------------------------------------------------------------------------
<# Reborn: refuse redirected path components before reading Misc or replacing owned published files. #>
#-------------------------------------------------------------------------------------------------
function Assert-RebornPackagePath([string]$Path) {
    for($part=[IO.Path]::GetFullPath($Path);$part;$part=[IO.Path]::GetDirectoryName($part)) {
        if((Test-Path -LiteralPath $part) -and ((Get-Item -LiteralPath $part -Force).Attributes -band [IO.FileAttributes]::ReparsePoint)){
            if(-not $IsWindows -or -not [Reborn.PackageReparseTag]::IsCloud($part)){throw ('Redirected package path refused: '+$part)}
        }
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: write and independently read back a streaming BIG4 with root-relative Misc names, bounded 32-bit offsets and unchanged source hashes. #>
#-------------------------------------------------------------------------------------------------
function New-RebornMiscBigs([string]$Source,[string]$Destination,[long]$Limit) {
    if($Limit -lt 16 -or $Limit -gt [uint32]::MaxValue){throw 'Invalid Misc BIG4 size limit.'}
    Assert-RebornPackagePath $Source
    $files=@();$entries=[Collections.Generic.List[object]]::new();$names=[Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
    if(Test-Path -LiteralPath $Source){
        if(-not(Test-Path -LiteralPath $Source -PathType Container)){throw 'Misc must be a directory.'}
        # Reborn: inspect every directory before recursion so junctions are never traversed.
        $pending=[Collections.Generic.Queue[string]]::new();$pending.Enqueue($Source)
        while($pending.Count){foreach($item in Get-ChildItem -LiteralPath $pending.Dequeue() -Force){Assert-RebornPackagePath $item.FullName;if($item.PSIsContainer){$pending.Enqueue($item.FullName)}elseif($item.Extension -ine '.asset'){$files+= $item}}}
    }
    # Reborn: absent, directory-only and excluded-only Misc inputs produce no archive at all.
    [long]$header=16;[long]$size=0
    $archives=[Collections.Generic.List[object]]::new()
    foreach($file in ($files | Sort-Object FullName)){
        $name=[IO.Path]::GetRelativePath($Source,$file.FullName).Replace('/','\')
        if($name -cmatch '[^\x20-\x7e]' -or -not $names.Add($name)){throw ('Invalid or duplicate BIG entry: '+$name)}
        $encoded=[Text.Encoding]::ASCII.GetBytes($name+[char]0)
        $directorySize=8+$encoded.Length
        if((((16+$directorySize+7)-band (-bnot 7))+$file.Length) -gt $Limit){throw ('Misc file exceeds the BIG4 size limit: '+$name)}
        if(((($header+$directorySize+7)-band (-bnot 7))+$size+$file.Length) -gt $Limit){
            $archives.Add($entries)
            $entries=[Collections.Generic.List[object]]::new();$header=16;$size=0
        }
        $header+=$directorySize;$size+=$file.Length
        $entries.Add([pscustomobject]@{Name=$name;Bytes=$encoded;File=$file.FullName;Length=$file.Length;Hash=(Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash})
    }
    if($entries.Count){$archives.Add($entries)}
    for($i=0;$i -lt $archives.Count;$i++){
        $path=if($i -eq 0){$Destination}else{Join-Path ([IO.Path]::GetDirectoryName($Destination)) ([IO.Path]::GetFileNameWithoutExtension($Destination)+'_'+($i+1)+'.big')}
        Write-RebornMiscBig $archives[$i] $path
        [pscustomobject]@{Name=[IO.Path]::GetFileName($path);Entries=$archives[$i].Count}
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: write and verify one BIG4 using only the entries assigned to its part. #>
#-------------------------------------------------------------------------------------------------
function Write-RebornMiscBig($Entries,[string]$Destination) {
    [long]$header=16;[long]$size=0
    foreach($entry in $Entries){$header+=8+$entry.Bytes.Length;$size+=$entry.Length}
    $header=($header+7)-band (-bnot 7);$total=$header+$size
    if($total -gt [uint32]::MaxValue){throw 'Misc BIG exceeds the BIG4 32-bit size limit.'}
    $stream=[IO.File]::Open($Destination,[IO.FileMode]::CreateNew,[IO.FileAccess]::ReadWrite,[IO.FileShare]::None)
    $writer=[IO.BinaryWriter]::new($stream)
    try {
        $writer.Write([Text.Encoding]::ASCII.GetBytes('BIG4'));$writer.Write([uint32]$total)
        # Reborn: the size word is little endian; directory counts, offsets and lengths are big endian.
        foreach($word in @($entries.Count,$header)){$bytes=[BitConverter]::GetBytes([uint32]$word);[Array]::Reverse($bytes);$writer.Write($bytes)}
        [long]$offset=$header
        foreach($entry in $entries){foreach($word in @($offset,$entry.Length)){$bytes=[BitConverter]::GetBytes([uint32]$word);[Array]::Reverse($bytes);$writer.Write($bytes)};$writer.Write([byte[]]$entry.Bytes);$offset+=$entry.Length}
        while($stream.Position -lt $header){$writer.Write([byte]0)}
        foreach($entry in $entries){$input=[IO.File]::OpenRead($entry.File);try{$input.CopyTo($stream)}finally{$input.Dispose()};if((Get-FileHash -LiteralPath $entry.File).Hash -cne $entry.Hash){throw ('Misc changed while packaging: '+$entry.File)}}
        $writer.Flush();if($stream.Length -ne $total){throw 'Misc BIG size changed during packaging.'}
        # Reborn: parse the stored directory independently and hash each stored payload without loading the archive into memory.
        $stream.Position=0;$reader=[IO.BinaryReader]::new($stream,[Text.Encoding]::ASCII,$true)
        if([Text.Encoding]::ASCII.GetString($reader.ReadBytes(4)) -cne 'BIG4' -or $reader.ReadUInt32() -ne $total){throw 'Misc BIG header mismatch.'}
        foreach($expected in @($entries.Count,$header)){$bytes=$reader.ReadBytes(4);[Array]::Reverse($bytes);if([BitConverter]::ToUInt32($bytes,0) -ne $expected){throw 'Misc BIG directory header mismatch.'}}
        $buffer=[byte[]]::new(65536);$expectedOffset=$header
        foreach($entry in $entries){
            foreach($expected in @($expectedOffset,$entry.Length)){$bytes=$reader.ReadBytes(4);[Array]::Reverse($bytes);if([BitConverter]::ToUInt32($bytes,0) -ne $expected){throw 'Misc BIG entry bounds mismatch.'}}
            if([Convert]::ToHexString($reader.ReadBytes($entry.Bytes.Length)) -cne [Convert]::ToHexString($entry.Bytes)){throw 'Misc BIG entry name mismatch.'}
            $directoryPosition=$stream.Position;$stream.Position=$expectedOffset;$hasher=[Security.Cryptography.IncrementalHash]::CreateHash([Security.Cryptography.HashAlgorithmName]::SHA256)
            try{[long]$left=$entry.Length;while($left){$read=$stream.Read($buffer,0,[int][Math]::Min($left,$buffer.Length));if(-not $read){throw 'Truncated Misc payload.'};$hasher.AppendData($buffer,0,$read);$left-=$read};if([Convert]::ToHexString($hasher.GetHashAndReset()) -cne $entry.Hash){throw 'Misc payload hash mismatch.'}}finally{$hasher.Dispose()}
            $stream.Position=$directoryPosition;$expectedOffset+=$entry.Length
        }
    } finally {$writer.Dispose();$stream.Dispose()}
}

#-------------------------------------------------------------------------------------------------
<# Reborn: publish streams, optional nonempty Misc and SKUDef with rollback, preserving other versions and refusing unowned or modified collisions. #>
#-------------------------------------------------------------------------------------------------
function Publish-RebornFinalPackage([string]$ModName,[string]$Version,[string]$Streams,[string]$Misc,[string]$ModsRoot,[long]$MiscBigLimit=[uint32]::MaxValue) {
    if($ModName -cnotmatch '\A[A-Za-z0-9_-]{1,48}\z' -or $Version -cnotmatch '\A[0-9]{1,4}(\.[0-9]{1,4}){1,3}\z'){throw 'Invalid final mod name or version.'}
    if(-not $ModsRoot){$documents=[Environment]::GetFolderPath([Environment+SpecialFolder]::MyDocuments);if(-not $documents){throw 'Windows Documents known folder is unavailable.'};$ModsRoot=Join-Path $documents 'Red Alert 3 Uprising/Mods'}
    # Reborn: receipts always contain absolute paths, including isolated developer test destinations.
    $ModsRoot=[IO.Path]::GetFullPath($ModsRoot)
    $output=Join-Path $ModsRoot $ModName;Assert-RebornPackagePath $output
    $null=New-Item -ItemType Directory -Path $output -Force
    # Reborn: serialize all publishers targeting this directory, including a second SDK checkout.
    $lockPath=Join-Path $output '.reborn-publish.lock';Assert-RebornPackagePath $lockPath
    $publicationLock=[IO.File]::Open($lockPath,[IO.FileMode]::OpenOrCreate,[IO.FileAccess]::ReadWrite,[IO.FileShare]::None)
    $stem=$ModName+'_'+$Version;$streamsName=$stem+'_Streams.big';$miscName=$stem+'_Misc.big';$skuName=$stem+'.skudef';$receiptName='.'+$stem+'.package.json'
    $stage=Join-Path $output ('.reborn-stage-'+[guid]::NewGuid().ToString('N'));$cleanup=$true;$stageOwned=$false
    $owned=@($streamsName,$miscName,$skuName,$receiptName);$saved=[Collections.Generic.List[string]]::new();$added=[Collections.Generic.List[string]]::new()
    try {
        $null=New-Item -ItemType Directory -Path $stage;$stageOwned=$true
        Copy-Item -LiteralPath $Streams -Destination (Join-Path $stage $streamsName)
        $miscParts=@(New-RebornMiscBigs $Misc (Join-Path $stage $miscName) $MiscBigLimit)
        $miscCount=0
        foreach($part in $miscParts){$miscCount+=$part.Entries}
        # Reborn: mirror official SDK order; this SKUDef is a mod fragment, not a replacement game SKU.
        $skuText="add-big $streamsName`r`n";$newFiles=@($streamsName)
        foreach($part in $miscParts){$skuText+="add-big $($part.Name)`r`n";$newFiles+=$part.Name}
        $newFiles+= $skuName
        [IO.File]::WriteAllText((Join-Path $stage $skuName),$skuText,[Text.Encoding]::ASCII)
        $inventory=@(foreach($name in $newFiles){[pscustomobject]@{Name=$name;Sha256=(Get-FileHash -LiteralPath (Join-Path $stage $name)).Hash}})
        if($inventory[0].Sha256 -cne (Get-FileHash -LiteralPath $Streams).Hash){throw 'Final streams copy differs.'}
        $receipt=Join-Path $output $receiptName
        # Reborn: old receipts own only verified exact-version files; extra parts follow the same rollback path.
        $previousFiles=[Collections.Generic.Dictionary[string,string]]::new([StringComparer]::OrdinalIgnoreCase)
        if(Test-Path -LiteralPath $receipt){
            Assert-RebornPackagePath $receipt;$old=Get-Content -LiteralPath $receipt -Raw | ConvertFrom-Json
            if($old.Format -cne 'RebornDocumentsPackage1' -or $old.ModName -cne $ModName -or $old.Version -cne $Version){throw 'Unmanaged final package receipt retained.'}
            $partPattern='\A'+[regex]::Escape($stem)+'_Misc_(?:[2-9]|[1-9][0-9]+)\.big\z'
            foreach($entry in $old.Files){
                if($entry.Name -cnotin @($streamsName,$miscName,$skuName) -and $entry.Name -cnotmatch $partPattern){throw 'Unmanaged final package receipt entry retained.'}
                if(-not $previousFiles.TryAdd($entry.Name,$entry.Sha256)){throw 'Duplicate final package receipt entry retained.'}
            }
            if(-not $previousFiles.ContainsKey($streamsName) -or -not $previousFiles.ContainsKey($skuName)){throw 'Incomplete final package receipt retained.'}
        }
        $owned=@($owned+@($miscParts.Name)+@($previousFiles.Keys) | Select-Object -Unique)
        foreach($name in $owned){
            if($name -ceq $receiptName){continue}
            $target=Join-Path $output $name;Assert-RebornPackagePath $target
            if($previousFiles.ContainsKey($name)){
                if(-not(Test-Path -LiteralPath $target -PathType Leaf) -or (Get-FileHash -LiteralPath $target).Hash -cne $previousFiles[$name]){throw ('Modified final package retained: '+$name)}
            }elseif(Test-Path -LiteralPath $target){throw ('Unmanaged final package collision retained: '+$name)}
        }
        [IO.File]::WriteAllText((Join-Path $stage $receiptName),([pscustomobject]@{Format='RebornDocumentsPackage1';ModName=$ModName;Version=$Version;Files=$inventory;MiscEntries=$miscCount}|ConvertTo-Json -Depth 5))
        # Reborn: only explicitly verified files of this exact version move; unrelated mod files never enter cleanup.
        foreach($name in $owned){$target=Join-Path $output $name;if(Test-Path -LiteralPath $target){[IO.File]::Move($target,(Join-Path $stage ($name+'.previous')));$saved.Add($name)}}
        # Reborn: an owned previous Misc is backed up above but not restored on success when the new source has no packaged files.
        foreach($name in @($newFiles)+@($receiptName)){[IO.File]::Move((Join-Path $stage $name),(Join-Path $output $name));$added.Add($name)}
    }catch{
        # Reborn: a failed rollback must retain the recovery generation, never delete the user's only previous copies.
        try{foreach($name in $added){[IO.File]::Delete((Join-Path $output $name))};foreach($name in $saved){[IO.File]::Move((Join-Path $stage ($name+'.previous')),(Join-Path $output $name))}}catch{$cleanup=$false;throw ('Publication recovery retained at '+$stage+': '+$_.Exception.Message)}
        throw
    }finally{
        # Reborn: remove only the exact fresh staging child after checking that no redirected descendants appeared.
        try{if($stageOwned -and $cleanup -and (Test-Path -LiteralPath $stage)){Assert-RebornPackagePath $stage;foreach($item in Get-ChildItem -LiteralPath $stage -Force -Recurse){Assert-RebornPackagePath $item.FullName};Remove-Item -LiteralPath $stage -Recurse -Force}}finally{$publicationLock.Dispose()}
    }
    $miscPaths=@(foreach($part in $miscParts){Join-Path $output $part.Name})
    return [pscustomobject]@{MiscPaths=$miscPaths;Directory=$output;Version=$Version;StreamsPath=(Join-Path $output $streamsName);MiscPath=$(if($miscCount -gt 0){Join-Path $output $miscName}else{$null});SkuDefPath=(Join-Path $output $skuName);MiscEntries=$miscCount}
}
