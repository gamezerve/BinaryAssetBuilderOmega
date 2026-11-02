param(
    [Parameter(Mandatory=$true)][string]$KwShaders,
    [Parameter(Mandatory=$true)][string]$UprisingShaders,
    [Parameter(Mandatory=$true)][string]$OutputDirectory
)
$ErrorActionPreference='Stop'
# Reborn: generate only a fresh private source copy; never patch the supplied EA reference trees or an existing authored directory.
$KwShaders=[IO.Path]::GetFullPath($KwShaders);$UprisingShaders=[IO.Path]::GetFullPath($UprisingShaders);$OutputDirectory=[IO.Path]::GetFullPath($OutputDirectory)
if(Test-Path -LiteralPath $OutputDirectory){throw 'The candidate output directory must be new.'}
foreach($root in @($KwShaders,$UprisingShaders)){if(-not [IO.Directory]::Exists($root)){throw ('Source directory not found: '+$root)};if($OutputDirectory.StartsWith($root.TrimEnd('\')+'\',[StringComparison]::OrdinalIgnoreCase)){throw 'Candidate output must be outside both original source trees.'}}
$encoding=[Text.Encoding]::GetEncoding(1252)
$registerPath=Join-Path $UprisingShaders 'RegisterMap.fxh';$map=[IO.File]::ReadAllText($registerPath,$encoding)
$fields=@{AmbientLightColor=@('REGISTER_AMBIENT_LIGHT_COLOR',4);DirectionalLight=@('REGISTER_DIRECTIONAL_LIGHTS_FIRST',5);PointLight=@('REGISTER_POINT_LIGHTS_FIRST',89);Cloud=@('REGISTER_CLOUD_SETUP',117);NoCloudMultiplier=@('REGISTER_NO_CLOUD_MULTIPLIER',3);RecolorColor=@('REGISTER_RECOLOR_COLOR',0);ShadowMapWorldToShadow=@('REGISTER_MAT_WORLD_TO_SHADOW',113);OpacityOverride=@('REGISTER_OPACITY_OVERRIDE',1);TintColor=@('REGISTER_TINT_COLOR',2);EyePosition=@('REGISTER_CAMERA_POSITION',123);ViewProjection=@('REGISTER_VIEWPROJECTION_MATRIX_FIRST',119);WorldBones=@('REGISTER_SKINNING_BONE_TRANSFORMS',128)}
foreach($field in $fields.Keys){$found=[regex]::Match($map,'(?m)^#define\s+'+$fields[$field][0]+'\s+(\d+)');if(-not $found.Success -or [int]$found.Groups[1].Value -ne $fields[$field][1]){throw ('Unreviewed Uprising register map: '+$field)}}
[IO.Directory]::CreateDirectory($OutputDirectory)|Out-Null
$changes=New-Object 'Collections.Generic.List[string]'
foreach($file in Get-ChildItem -LiteralPath $KwShaders -File){if($file.Extension -in @('.fx','.fxh')){[IO.File]::WriteAllBytes((Join-Path $OutputDirectory $file.Name),[IO.File]::ReadAllBytes($file.FullName))}}
foreach($name in @('Objects.fxh','Skinning.fxh')){
    $path=Join-Path $OutputDirectory $name;$content=[IO.File]::ReadAllText($path,$encoding).Replace("`r`n","`n")
    foreach($field in ($fields.Keys|Sort-Object)){
        $register=[int]$fields[$field][1]
        $pattern='(?m)^(?:shared\s+)?(float\w*|Sas\w+|CloudSetup|BoneTransform)\s+'+$field+'(\[[^\n]*?\])?(?:\s*:\s*\w+(?:\([^\n]*\))?)?(?=\s*(?:<|=|;|\r?$))'
        # Reborn: retain each original declaration/annotation/initializer while assigning only its reviewed fixed register.
        $content=[regex]::Replace($content,$pattern,[Text.RegularExpressions.MatchEvaluator]{param($match)
            $changes.Add($name+':'+$field)
            '// Reborn: use the original Uprising fixed constant register.'+"`nshared "+$match.Groups[1].Value+' '+$field+$match.Groups[2].Value+' : register('+$(if($field -eq 'WorldBones'){'vs, '}else{''})+'c'+$register+')'
        })
    }
    if($name -eq 'Skinning.fxh'){
        if(-not $content.Contains('MaxSkinningBones = 80')){throw 'Unreviewed KW skinning source; expected the original 80-bone declaration.'}
        $content=[regex]::Replace($content,'(?m)^static const int MaxSkinningBones = 80;','// Reborn: Uprising reserves c128..c255 for 64 quaternion/translation bone pairs.'+"`nstatic const int MaxSkinningBones = 64;")
        $content=[regex]::Replace($content,'(?ms)^(?:shared )?BoneTransform WorldBones_L\[[^\n]+(?:\s*:\s*register\([^\n]*\))?\s*<.*?>;','// Reborn: both quality paths use the same Uprising uploaded bone register window.'+"`n#define WorldBones_L WorldBones")
        $content=$content.Replace('float4x3 World : World;','// Reborn: reserve the original Uprising non-skinned world matrix registers.'+"`nfloat4x3 World : World : register(vs, c124);")
    }else{
        $content=$content.Replace('AlphaBlendEnable = ( OpacityOverride < 0.99);','// Reborn: engine-managed opacity cannot participate in a CPU effect-state expression.'+"`n#if !EXPRESSION_EVALUATOR_ENABLED`n        AlphaBlendEnable = ( OpacityOverride < 0.99);`n#endif")
        $content=[regex]::Replace($content,'(?m)^[ \t]*float4 lighting = lit\( diffuseTerm, specularTerm, SpecularExponent \);','// Reborn: preserve the old implicit scalar conversion with explicit modern-HLSL scalar inputs.'+"`n        float4 lighting = lit( diffuseTerm.x, specularTerm.x, SpecularExponent );")
    }
    [IO.File]::WriteAllText($path,$content,$encoding)
}
# Reborn: keep provenance and the incomplete rendering status beside the generated sources instead of claiming a completed port.
$receipt=[ordered]@{Format='RebornKwShaderLoaderCandidate1';KwSource=$KwShaders;UprisingSource=$UprisingShaders;RegisterMapSha256=(Get-FileHash -LiteralPath $registerPath -Algorithm SHA256).Hash;Changes=@($changes.ToArray());EvaluatorMigrationComplete=$false;RenderingValidated=$false}
[IO.File]::WriteAllText((Join-Path $OutputDirectory 'loader-candidate.json'),($receipt|ConvertTo-Json -Depth 6),[Text.Encoding]::UTF8)
Write-Output ('Reborn: experimental loader source copy prepared: '+$OutputDirectory)
