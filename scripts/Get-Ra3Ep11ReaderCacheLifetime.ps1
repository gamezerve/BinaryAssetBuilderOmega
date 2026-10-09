# Reborn: pin scoped 1.1 reader destructor/cache helper bodies without executing native code or claiming full tree, lock or reference-count semantics.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[Parameter(Mandatory=$true)] [string] $BaselineImagePath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$cacheSelfTest=$SelfTest;$cacheAsJson=$AsJson
$lifetime=. (Join-Path $PSScriptRoot 'Get-Ra3Ep11ReaderListLifetime.ps1') -ImagePath $ImagePath -BaselineImagePath $BaselineImagePath
$SelfTest=$cacheSelfTest;$AsJson=$cacheAsJson

#-------------------------------------------------------------------------------------------------
<# Reborn: preserve complete reviewed destructor, lookup, removal, publication, comparison and member-cleanup bodies as independent 1.1 byte pins. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11ReaderCacheCode([byte[]] $Bytes) {
    foreach($slice in @(
        [pscustomobject]@{Offset=0x9b6f0;Length=316;Hash='F86ED20FC65823CE7AF66ED8B45D2B2C6F559D5ACBC4CBE9FCD3A891804DB12B'},
        [pscustomobject]@{Offset=0x277520;Length=181;Hash='7209E29363D4EDE62D94C15D064542666C1A40DB1460386D97E2ED37F1CCE119'},
        [pscustomobject]@{Offset=0x970f0;Length=81;Hash='8CFCD38361252A1C28C6E7B7E205B1D3CC4B7ABE05D4E512F9ACBB78D2872381'},
        [pscustomobject]@{Offset=0x2a34b0;Length=158;Hash='3A10F09083E6FEC1BF5C26FB3FD05B6C467E5FB9983F3F01EC005643FB6CF986'},
        [pscustomobject]@{Offset=0x495d0;Length=99;Hash='4B8D763CBF9869B5EAC8385074472F043D4C6A954F8DA8A21885D394E2281C13'},
        [pscustomobject]@{Offset=0xd1970;Length=63;Hash='FCB60D5B37FF7A4737D38EF8E668422640AD9AEA28ED98E4BA198B2CFCFA1AD3'}
    )){
        if($slice.Offset+$slice.Length-gt $Bytes.Length){throw 'Cache lifetime slice outside image.'}
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$slice.Offset..($slice.Offset+$slice.Length-1)]))-cne $slice.Hash){throw 'Reviewed cache lifetime changed.'}
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: evaluate only the observed deleting-thunk low-bit predicate on detached UInt32 flags; this does not invoke any free or destructor. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11DeletingFlagDecision([uint32] $Flags) {
    [pscustomobject]@{Flags=$Flags;DestructorCalled=$true;ReaderObjectFreeRequested=[bool]($Flags-band 1)}
}
Assert-Ep11ReaderCacheCode $ep11Bytes
$decisions=@(foreach($value in @([uint32]0,[uint32]1,[uint32]2,[uint32]3,[uint32]::MaxValue)){Get-Ep11DeletingFlagDecision $value})
if($SelfTest){
    # Reborn: retain exact low-bit behavior, including nonzero flags that do not request the reader object's allocation free.
    $expected=@($false,$true,$false,$true,$true)
    foreach($index in 0..4){if(-not $decisions[$index].DestructorCalled -or $decisions[$index].ReaderObjectFreeRequested-ne $expected[$index]){throw 'Deleting flag predicate differs.'}}
    # Reborn: mutate detached bytes across both cache paths, removal, both publication returns, comparison and member cleanup.
    foreach($offset in @(0x9b702,0x9b723,0x9b736,0x9b747,0x9b78b,0x9b79e,0x9b7af,0x9b7d2,0x9b815,0x27753a,0x2775bd,0x2775cb,0x970f9,0x9712d,0x2a3536,0x2a3542,0x495fe,0xd1994)){
        $fault=[byte[]]$ep11Bytes.Clone();$fault[$offset]=$fault[$offset]-bxor 1;$rejected=$false
        try{Assert-Ep11ReaderCacheCode $fault}catch{$rejected=$true}
        if(-not $rejected){throw 'Cache lifetime code fault admitted.'}
    }
}
$cacheAfter=Read-RuntimeInput $ImagePath 16777216
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($cacheAfter))-cne $lifetime.ImageSha256){throw 'Cache lifetime image changed during review.'}
$report=[pscustomobject]@{
    ImageSha256=$lifetime.ImageSha256;DestructorVa='0x0049B6F0';LookupVa='0x00677520';RemovalVa='0x004970F0';PublicationVa='0x006A34B0';LessThanVa='0x004495D0';MemberCleanupVa='0x004D1970'
    ReviewedWholeBodyPins=6;OrdinaryCacheVa='0x00CC4ADC';OrdinarySentinelVa='0x00CC4AE0';TagTwoCacheVa='0x00CC4AF8';TagTwoSentinelVa='0x00CC4AFC'
    OrdinaryDestructorKeyReaderOffset=16;TagTwoDestructorKeyReaderOffset=12;ScopedTwoCacheRemovalCallsRecovered=$true
    RemovalDecrementsCacheCountOffset=20;RemovalFreesNodeInReviewedBody=$true;RemovalDirectlyFreesReaderPayload=$false
    PublicationExistingPayloadAddressOffset=32;PublicationInsertedPayloadAddressOffset=32;ScopedPublicationPayloadSlotRecovered=$true
    DestructorCallsResourceCleanup=$true;DestructorAdditionalFreedReaderFields=@(8,12,16,20);DestructorMemberCleanupReaderOffsets=@(96,92,88);DestructorFinalVtableVa='0x00BF7EB8'
    DeletingFlagDecisions=$decisions;HealthyListInsertionRecovered=$lifetime.FreshNodePayloadRecoveredUnderHealthySentinelInvariant
    KeyNormalizationSemanticsRecovered=$false;ByteComparisonCalleeSemanticsRecovered=$false;FullTreeLookupAndInsertionRecovered=$false;FullTreeRemovalRecovered=$false
    MemberCleanupLockAndReferenceSafetyProven=$false;AllSourceObjectBindingsRecovered=$false;DeletingTargetFullCacheLifetimeRecovered=$false;CompleteStreamPointerProvenanceRecovered=$false
    ProductionBuildReady=$false;ModPackageLoaded=$false;ReadOnly=$true;TargetExecuted=$false;ManagedInspectorHashCommandExecuted=$false
    FaultTestsPassed=[bool]$SelfTest;DeletingFlagFixturesExecuted=$(if($SelfTest){5}else{0});MemoryFaultsExecuted=$(if($SelfTest){18}else{0})
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 7}else{$report}
