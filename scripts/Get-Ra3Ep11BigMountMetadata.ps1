# Reborn: recover scoped native BIG mounting metadata and header classes using complete byte pins, detached headers and read-only stock directories, not engine execution.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[Parameter(Mandatory=$true)] [string] $SkuDefinitionPath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$mountSelfTest=$SelfTest;$mountAsJson=$AsJson
$stock=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11StockPackagePreflight.ps1') -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath -SkuDefinitionPath $SkuDefinitionPath
$SelfTest=$mountSelfTest;$AsJson=$mountAsJson

#-------------------------------------------------------------------------------------------------
<# Reborn: pin whole native mount, archive-node constructor, four-byte classifier and directory-size helper independently for this 1.1 image. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11BigMountCode([byte[]] $Bytes) {
    foreach($slice in @(
        [pscustomobject]@{Offset=0x595160;Length=704;Hash='707ECA613CADFAAE2094A6C599E4FCA3C860F7E862F2E395847CDD4F240475DC'},
        [pscustomobject]@{Offset=0x5950f0;Length=101;Hash='C638A2D700873D63016B3087B037BD7AE7AA902E8C12AE7F4CFC2E1606D99DE7'},
        [pscustomobject]@{Offset=0x596060;Length=119;Hash='7707317097AE454D02CADBA91882F07F268213973FF1D41578AE0233EFD7A15B'},
        [pscustomobject]@{Offset=0x5961a0;Length=79;Hash='CE15137BA76C8FFBF3C5A38CC624A11C7278E774ACE9B6F436206CDCB29930BA'}
    )){
        if($slice.Offset+$slice.Length-gt $Bytes.Length){throw 'BIG mount slice outside image.'}
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$slice.Offset..($slice.Offset+$slice.Length-1)]))-cne $slice.Hash){throw 'Reviewed BIG mount metadata changed.'}
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: model the observed fixed-header classification/size arithmetic only; the diagnostic requires sixteen bytes even though native helpers do not prove that bound. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11DetachedBigHeader([byte[]] $Header) {
    if($Header.Length-lt 16){throw 'Detached BIG metadata requires a sixteen-byte header.'}
    $magic=[Text.Encoding]::ASCII.GetString($Header,0,4);$class=3
    if($magic-ceq 'Viv4'){$class=2}elseif([Text.Encoding]::ASCII.GetString($Header,0,3)-ceq 'BIG'){$class=0}elseif($Header[0]-eq 0xc0 -and $Header[1]-eq 0xfb){$class=1}
    [uint32]$size=0
    if($class-eq 0 -or $class-eq 2){$size=Read-ArchiveU32 $Header 12}elseif($class-eq 1){$size=4+256*[uint32]$Header[2]+$Header[3]}
    return [pscustomobject]@{Class=$class;HeaderBytes=$size}
}
Assert-Ep11BigMountCode $ep11Bytes
$headers=@(foreach($archive in $stock.Archives){
    $snapshot=Read-ArchiveDirectory $archive.Path
    if($snapshot.Length-ne $archive.ArchiveBytes -or [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($snapshot.Bytes))-cne $archive.DirectorySha256){throw 'Native BIG metadata input changed after stock preflight.'}
    $decoded=Get-Ep11DetachedBigHeader ([byte[]]$snapshot.Bytes[0..15])
    if($decoded.Class-ne 0 -or $decoded.HeaderBytes-ne $snapshot.Bytes.Length){throw 'Stock BIG metadata classification differs from validated directory.'}
    [pscustomobject]@{Archive=[IO.Path]::GetFileName($archive.Path);Magic=$snapshot.Magic;NativeHeaderClass=$decoded.Class;NativeDeclaredDirectoryBytes=$decoded.HeaderBytes;DirectorySha256=$archive.DirectorySha256}
})
if($SelfTest){
    # Reborn: BIG4/BIGF/BIGX share the observed native class; Viv4 and C0FB use alternate size forms, unknown magic gives no directory size.
    foreach($magic in @('BIG4','BIGF','BIGX','Viv4','C0FB','NONE')){
        $fixture=[byte[]]::new(16);[Text.Encoding]::ASCII.GetBytes($magic).CopyTo($fixture,0);$fixture[15]=64
        if($magic-eq 'C0FB'){$fixture[0]=0xc0;$fixture[1]=0xfb;$fixture[2]=0;$fixture[3]=60}
        $decoded=Get-Ep11DetachedBigHeader $fixture
        $expectedClass=$(switch($magic){'Viv4'{2};'C0FB'{1};'NONE'{3};default{0}});$expectedSize=$(if($magic-eq 'NONE'){0}else{64})
        if($decoded.Class-ne $expectedClass -or $decoded.HeaderBytes-ne $expectedSize){throw 'Detached native BIG header arithmetic differs.'}
    }
    $rejected=$false;try{$null=Get-Ep11DetachedBigHeader ([byte[]]::new(15))}catch{$rejected=$true};if(-not $rejected){throw 'Truncated detached BIG header admitted.'}
    # Reborn: fault detached native images across option bits, size lookup, allocator, publication, constructor, classification and size arithmetic.
    foreach($offset in @(0x595194,0x595249,0x5953a9,0x5953d7,0x5953f6,0x5953fc,0x595108,0x59608a,0x5960b6,0x5960c5,0x5961c6,0x5961dc)){
        $fault=[byte[]]$ep11Bytes.Clone();$fault[$offset]=$fault[$offset]-bxor 1;$rejected=$false
        try{Assert-Ep11BigMountCode $fault}catch{$rejected=$true};if(-not $rejected){throw 'BIG mount metadata byte fault admitted.'}
    }
}
$mountAfter=Read-RuntimeInput $ImagePath 16777216
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($mountAfter))-cne $stock.ImageSha256){throw 'BIG mount engine identity changed during review.'}
$report=[pscustomobject]@{
    ImageSha256=$stock.ImageSha256;MountVa='0x00995160';NodeConstructorVa='0x009950F0';HeaderClassifierVa='0x00996060';DirectorySizeVa='0x009961A0';ReviewedWholeBodyPins=4
    ConfigAddBigMode=2;NativeModeLowBitControlsAlternateNameAttempt=$true;NativeModeBitTwoCallsIndexHelper=$true;IndexHelperVa='0x00996880'
    NativeInitialHeaderRequestBytes=16;ArchiveNodeAllocationBytes=104;ArchiveListFirstOffset=16;ArchiveListTailOffset=20;ArchiveListCountOffset=24
    ScopedArchiveListAppendRecovered=$true;NativeClassifierIgnoresFourthBigMagicByte=$true;StrictDiagnosticBigReaderAcceptsOnlyBigFourOrBigF=$true
    StockHeaders=$headers;StockHeaderCount=$headers.Count;ArchiveDirectoryMetadataBytesRead=$stock.ArchiveDirectoryMetadataBytesRead+($stock.Archives|Measure-Object DirectoryBytes -Sum).Sum;ArchivePayloadBytesRead=0
    NativeInitialHeaderReadResultChecked=$false;NativeDirectoryReadResultChecked=$false;NativeAllocationFailureSafetyProven=$false;FullIndexAndPathLookupSemanticsRecovered=$false
    FullArchivePrecedenceAndUnmountSemanticsRecovered=$false;ConfiguredStockArchiveSetComplete=$stock.ConfiguredStockArchiveSetComplete;MissingConfiguredArchives=$stock.MissingConfiguredArchives
    ReadOnly=$true;TargetExecuted=$false;ModPackageLoaded=$false;AuthenticEp1ProcessingHashRecovered=$false;ProductionBuildReady=$false
    FaultTestsPassed=[bool]$SelfTest;HeaderArithmeticFixturesExecuted=$(if($SelfTest){6}else{0});TruncatedHeaderFixturesExecuted=$(if($SelfTest){1}else{0});MemoryFaultsExecuted=$(if($SelfTest){12}else{0})
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 8}else{$report}
