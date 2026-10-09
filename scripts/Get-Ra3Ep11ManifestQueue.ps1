# Reborn: connect reviewed 1.1 config manifest strings to reader creation and a scoped wrapper load path, without asserting native execution or all stream provenance.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$queueSelfTest=$SelfTest;$queueAsJson=$AsJson
$config=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11ConfigConsumer.ps1') -ImagePath $ImagePath
$ownership=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11ReaderOwnership.ps1') -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath
$SelfTest=$queueSelfTest;$AsJson=$queueAsJson

#-------------------------------------------------------------------------------------------------
<# Reborn: pin complete queue initialization/load driver, direct-reader load wrapper, source probe and stock manifest/suffix literal block. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11ManifestQueueCode([byte[]] $Bytes) {
    foreach($slice in @(
        [pscustomobject]@{Offset=0x24d440;Length=661;Hash='820BEBD72382DCD8AA3CC16FBB3F83928C3900B1E048E5D74C29A10B253B67D1'},
        [pscustomobject]@{Offset=0xd05e0;Length=293;Hash='28E2CA1AE08C232364CE4096A7C6B4C49BE7A51595E78107DB74D8EEFF28D69A'},
        [pscustomobject]@{Offset=0xaa5b0;Length=187;Hash='CE83460F3D007DF8BB84D6F95BBB387570669734CE2DEAFA03249091F9907214'},
        [pscustomobject]@{Offset=0x8260ac;Length=40;Hash='3636805A57E5943EDEEADFA01BF075009F029A13F14B883256131CB2D5D341FE'}
    )){
        if($slice.Offset+$slice.Length-gt $Bytes.Length){throw 'Manifest queue slice outside image.'}
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$slice.Offset..($slice.Offset+$slice.Length-1)]))-cne $slice.Hash){throw 'Reviewed manifest queue changed.'}
    }
}
Assert-Ep11ManifestQueueCode $ep11Bytes
if($SelfTest){
    # Reborn: detached faults cover queue gate, factory/value storage, source probe/load gate, wrapper arguments and stock literal identity.
    foreach($offset in @(0x24d49f,0x24d4ac,0x24d4cf,0x24d4ed,0x24d568,0x24d69e,0x24d6af,0xd05f4,0xd0668,0xd0672,0xd0677,0xd067d,0xd0682,0xd06db,0xaa5c3,0xaa61e,0x8260b4,0x8260c4)){
        $fault=[byte[]]$ep11Bytes.Clone();$fault[$offset]=$fault[$offset]-bxor 1;$rejected=$false
        try{Assert-Ep11ManifestQueueCode $fault}catch{$rejected=$true};if(-not $rejected){throw 'Manifest queue byte fault admitted.'}
    }
}
$queueAfter=Read-RuntimeInput $ImagePath 16777216
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($queueAfter))-cne $ownership.ImageSha256 -or $config.ImageSha256-cne $ownership.ImageSha256){throw 'Manifest queue image identity differs.'}
$report=[pscustomobject]@{
    ImageSha256=$ownership.ImageSha256;ReviewedPins=4;ConfigQueueVa=$config.AddManifestAppendsToGlobalListVa;QueueEndVa='0x00CF2358';QueueDriverVa='0x0064D440'
    ReaderVectorOwnerOffset=864;QueueEntryStride=4;QueueFactoryVa='0x004AAC80';QueueFactoryCallVa='0x0064D4CF';QueueFactoryArgumentTwo=1
    QueueReaderCreationWhenVectorEmptyRecovered=$true;QueueDrainedOrClearedInReviewedDriver=$false;FactoryNullResultRejectedBeforeVectorAppend=$false
    DefaultStaticManifest='static.manifest';DefaultModManifest='mod.manifest';VariantSuffixes=@('_L','_M');VariantSetterVa='0x004AA300';QueueVariantSetterArgumentOne=1
    QueueProbeVa='0x004AA5B0';QueueLoadCallVa='0x0064D6AF';DirectReaderLoadVa='0x004D05E0';QueueDirectLoadArgumentTwo=0;QueueLoadConditionalOnProbeAndDriverBranch=$true
    DirectReaderMembershipGateVa='0x0045F060';DirectReaderListOwnerVa='0x00CF1578';DirectReaderSentinelVa='0x00CF1584';DirectReaderWrapperCallVa='0x004D06DB'
    DirectReaderWrapperSourceArgumentOrdinal=2;DirectReaderWrapperSourceIsAddressOfStoredNodePayload=$true;DirectReaderWrapperSixthArgumentIsHelperArgumentTwo=$true
    ScopedQueueNinthLoaderArgumentValue=0;ReviewedWrapperCallerPathsIncludingPriorIterators=3;AllWrapperCallerPathsRecovered=$false
    FullVariantFilenameAndFallbackSemanticsRecovered=$false;FullDriverStartupReachabilityRecovered=$false;FullBigMountSemanticsRecovered=$false;CompleteStreamPointerProvenanceRecovered=$false
    ProductionBuildReady=$false;ModPackageLoaded=$false;ReadOnly=$true;TargetExecuted=$false;ManagedInspectorHashCommandExecuted=$false
    FaultTestsPassed=[bool]$SelfTest;MemoryFaultsExecuted=$(if($SelfTest){18}else{0})
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 7}else{$report}
