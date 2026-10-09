# Reborn: recover circular-sentinel insertion aliases and separate resource cleanup from conditional reader deletion using static evidence and synthetic memory only.
[CmdletBinding()]
param([Parameter(Mandatory=$true)] [string] $ImagePath,[switch] $SelfTest,[switch] $AsJson)
$ErrorActionPreference='Stop'
$listSelfTest=$SelfTest;$listAsJson=$AsJson
$ownership=. (Join-Path $PSScriptRoot 'Get-Ra3Ep1ReaderOwnership.ps1') -ImagePath $ImagePath
$SelfTest=$listSelfTest;$AsJson=$listAsJson

#-------------------------------------------------------------------------------------------------
<# Reborn: pin insertion, resource cleanup, source-list release/reset, node cleanup, release thunk and concrete vtable without executing native instructions. #>
#-------------------------------------------------------------------------------------------------
function Assert-ReaderListLifetimeCode([byte[]] $Bytes) {
    foreach($slice in @(
        [pscustomobject]@{Offset=0xce7a2;Length=40;Hash='4073888A4E990364DC4ADA277FC564E472F4B3AC12D6DF1EB37D30457883ACAA'},
        [pscustomobject]@{Offset=0x184d0;Length=89;Hash='728DE97E1D5F8950333F8C2462DA48D3E21E7E55D0AB2CB56465B709389FBCFD'},
        [pscustomobject]@{Offset=0xabb2b;Length=38;Hash='57D0DE938767B5BBA4FB517579D661140592F01AEFF05282AFD3B2A735BAE96D'},
        [pscustomobject]@{Offset=0x96fd0;Length=95;Hash='94D41BAA994B1C68A1E1A3A50E9489177FEB58C281FA6D7B8C40403016543F10'},
        [pscustomobject]@{Offset=0x17190;Length=14;Hash='2A807D2070C8724D202C529406CFB1919B5F31A8B22075A126CDF19C30436235'},
        [pscustomobject]@{Offset=0x7f2e78;Length=64;Hash='7FC81588CFC3DAD2011F9367DED4F86F6FF2428AAC682867A117B925EC8DC1F0'},
        [pscustomobject]@{Offset=0x9f4a0;Length=30;Hash='CA0145427B3A295292F8934FACF9DC0F5945EB2D2D33E78991283FB08DFB3A5B'}
    )){
        if($slice.Offset+$slice.Length-gt $Bytes.Length){throw 'Reader lifetime slice outside image.'}
        if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData([byte[]]$Bytes[$slice.Offset..($slice.Offset+$slice.Length-1)]))-cne $slice.Hash){throw 'Reviewed reader lifetime changed.'}
    }
}

#-------------------------------------------------------------------------------------------------
<# Reborn: validate synthetic circular-list linkage under an explicit diagnostic bound; this is not native corruption handling or memory dereferencing. #>
#-------------------------------------------------------------------------------------------------
function Get-SyntheticReaderNodes([hashtable] $Memory,[long] $Sentinel) {
    if(-not $Memory.ContainsKey($Sentinel) -or -not $Memory.ContainsKey($Sentinel+4)){throw 'Synthetic sentinel missing.'}
    $seen=@{};$previous=$Sentinel;$current=[long]$Memory[$Sentinel];$nodes=[Collections.Generic.List[long]]::new()
    while($current-ne $Sentinel){
        if($nodes.Count-ge 100 -or $seen.ContainsKey($current) -or -not $Memory.ContainsKey($current) -or
            -not $Memory.ContainsKey($current+4) -or -not $Memory.ContainsKey($current+8) -or [long]$Memory[$current+4]-ne $previous){throw 'Synthetic circular-list invariant differs.'}
        $seen[$current]=$true;$nodes.Add($current);$previous=$current;$current=[long]$Memory[$current]
    }
    if([long]$Memory[$Sentinel+4]-ne $previous){throw 'Synthetic sentinel tail differs.'}
    return ,($nodes.ToArray())
}

#-------------------------------------------------------------------------------------------------
<# Reborn: model the exact observed link-write order so the first-node previous alias updates sentinel.next before reloading and storing the reader. #>
#-------------------------------------------------------------------------------------------------
function Invoke-SyntheticReaderInsertion([hashtable] $Memory,[long] $Sentinel,[long] $NewNode,[uint32] $Reader) {
    $null=Get-SyntheticReaderNodes $Memory $Sentinel
    if($Memory.ContainsKey($NewNode) -or $Memory.ContainsKey($NewNode+4) -or $Memory.ContainsKey($NewNode+8)){throw 'Synthetic new node overlaps existing words.'}
    $first=[long]$Memory[$Sentinel]
    $Memory[$NewNode]=$first
    $Memory[$NewNode+4]=[long]$Memory[$first+4]
    $previous=[long]$Memory[$first+4]
    $Memory[$previous]=$NewNode
    $Memory[$first+4]=$NewNode
    $reloaded=[long]$Memory[$Sentinel]
    $Memory[$reloaded+8]=$Reader
    if($reloaded-ne $NewNode){throw 'Synthetic insertion did not reload the new first node.'}
    $null=Get-SyntheticReaderNodes $Memory $Sentinel
    return $reloaded
}

#-------------------------------------------------------------------------------------------------
<# Reborn: build bounded detached empty/successive insertion scenarios and expose traversal order; no native allocator or destructor is emulated. #>
#-------------------------------------------------------------------------------------------------
function Get-SyntheticReaderScenario([uint32[]] $Readers) {
    if($Readers.Count-gt 100){throw 'Synthetic reader scenario exceeds diagnostic bound.'}
    $sentinel=[long]0x1000;$memory=@{};$memory[$sentinel]=$sentinel;$memory[$sentinel+4]=$sentinel
    for($index=0;$index-lt $Readers.Count;$index++){
        $null=Invoke-SyntheticReaderInsertion $memory $sentinel ([long](0x2000+64*$index)) $Readers[$index]
    }
    $nodes=Get-SyntheticReaderNodes $memory $sentinel
    [pscustomobject]@{InsertedCount=$Readers.Count;FirstNode=[long]$memory[$sentinel];TailNode=[long]$memory[$sentinel+4];TraversalReaders=@(foreach($node in $nodes){[uint32]$memory[$node+8]});FreshNodePayloadAliasRecovered=$true}
}
Assert-ReaderListLifetimeCode $bytes
$scenarios=@(Get-SyntheticReaderScenario ([uint32[]]@());Get-SyntheticReaderScenario ([uint32[]]@(11));Get-SyntheticReaderScenario ([uint32[]]@(11,22));Get-SyntheticReaderScenario ([uint32[]]@(11,22,33)))
if($SelfTest){
    # Reborn: require prepend/LIFO traversal for empty and one/two/three-reader fixtures under healthy sentinel linkage.
    $expected=@('','11','22,11','33,22,11')
    foreach($index in 0..3){if(($scenarios[$index].TraversalReaders -join ',')-cne $expected[$index]){throw 'Synthetic reader traversal differs.'}}
    # Reborn: reject detached broken previous links and a non-sentinel cycle as diagnostic policy, not an engine safety claim.
    foreach($faultKind in @('previous','cycle')){
        $sentinel=[long]0x1000;$node=[long]0x2000;$memory=@{}
        $memory[$sentinel]=$node;$memory[$sentinel+4]=$node;$memory[$node]=$sentinel;$memory[$node+4]=$sentinel;$memory[$node+8]=[uint32]11
        if($faultKind-eq 'previous'){$memory[$node+4]=$node}else{$memory[$node]=$node}
        $rejected=$false;try{$null=Invoke-SyntheticReaderInsertion $memory $sentinel ([long]0x3000) 22}catch{$rejected=$true}
        if(-not $rejected){throw 'Broken synthetic list admitted.'}
    }
    # Reborn: detached byte faults cover link alias, payload reload, cleanup fields, release slot, sentinel reset, node free and concrete deleting slot.
    foreach($offset in @(0xce7c0,0xce7c5,0xce7c7,0x184e0,0x18517,0x1851f,0xabb3a,0xabb4c,0x9701b,0x17196,0x7f2eb4,0x9f4b0)){
        $fault=[byte[]]$bytes.Clone();$fault[$offset]=$fault[$offset]-bxor 1;$rejected=$false
        try{Assert-ReaderListLifetimeCode $fault}catch{$rejected=$true}
        if(-not $rejected){throw 'Reader lifetime code fault admitted.'}
    }
}
$listAfter=Read-RuntimeInput $ImagePath 16777216
if([Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($listAfter))-cne $ownership.ImageSha256){throw 'Lifetime image changed during review.'}
$report=[pscustomobject]@{
    ImageSha256=$ownership.ImageSha256;InsertionVa='0x004CE7A2';CleanupVa='0x004184D0';SourceListReleaseVa='0x004ABB2B';NodeCleanupVa='0x00496FD0';ReleaseThunkVa='0x00417190'
    OwnerSentinelOffset=12;NodeReaderOffset=8;FreshNodePayloadRecoveredUnderHealthySentinelInvariant=$true;TraversalAfterPrepend='newest first'
    SourceListResetRestoresSelfLinks=$true;ConcreteReleaseVirtualSlot=8;ConcreteReleaseDelegatedSlot=60;ConcreteDeletingTargetVa='0x0049F4A0'
    ResourceCleanupFreesReaderObjectInReviewedBody=$false;ResourceCleanupClearsSourceField=24;ResourceCleanupClearsLinkedField=84
    ScopedResourceCleanupRecovered=$true;ScopedSourceListReleaseRecovered=$true;ConcreteReleaseConditionalObjectFreeRecovered=$true;DeletingTargetFullCacheLifetimeRecovered=$false
    AllListInitializersAndInvariantsProven=$false;ArbitraryCallbackSafetyProven=$false;AllSourceObjectBindingsRecovered=$false;CompleteStreamPointerProvenanceRecovered=$false
    ProductionBuildReady=$false;ReadOnly=$true;TargetExecuted=$false;ManagedInspectorHashCommandExecuted=$ownership.ManagedInspectorHashCommandExecuted
    Scenarios=$scenarios;FaultTestsPassed=[bool]$SelfTest;InsertionScenarioFixturesExecuted=$(if($SelfTest){4}else{0});BrokenListFixturesExecuted=$(if($SelfTest){2}else{0});MemoryFaultsExecuted=$(if($SelfTest){12}else{0})
}
if($AsJson){ConvertTo-Json -InputObject $report -Depth 7}else{$report}
