# Reborn: conservative detached filename policy for reviewed EP1 post-config identity copies; not a native parser or runtime-safety certificate.
#-------------------------------------------------------------------------------------------------
<# Reborn: model basename/first-underscore identity length and reject copies that cannot include a NUL in the reviewed 16-byte destination. #>
#-------------------------------------------------------------------------------------------------
function Get-Ep11ConfigNameProfile([string]$Path) {
    $name=[IO.Path]::GetFileName($Path)
    if([string]::IsNullOrEmpty($name) -or $name-match '[^\x20-\x7E]'){throw 'Config name must be nonempty ASCII.'}
    $underscore=$name.IndexOf('_')
    $identity=if($underscore-ge 0){$name.Substring(0,$underscore)}else{$name}
    [pscustomobject]@{BaseName=$name;Identity=$identity;IdentityAsciiBytes=$identity.Length;DestinationBytes=16;CopyFits=($identity.Length-lt 16);RuntimeSafetyProven=$false}
}
#-------------------------------------------------------------------------------------------------
<# Reborn: refuse the known-too-long identity before any launch; fitting this one copy does not prove later startup or version parsing. #>
#-------------------------------------------------------------------------------------------------
function Assert-Ep11ConfigNamePolicy([string]$Path) {
    $profile=Get-Ep11ConfigNameProfile $Path
    if(-not $profile.CopyFits){throw 'Config basename identity exceeds the reviewed 15-byte plus NUL limit; do not launch this probe.'}
}
