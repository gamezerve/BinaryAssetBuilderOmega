using System.Buffers.Binary;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Hashing;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: cover optional weapon sound/FX storage and real Core selectors without editing or rebuilding the user's mod.
internal static class WeaponOptionalReferencesSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: test absent, explicit empty and populated references together, retain the stock token golden and verify normal stream/package output. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global,string output)
    {
        output=SdkEnvironmentPreflight.Absolute(output);if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh weapon reference test output required.");Directory.CreateDirectory(output);var saved=Settings.Current;
        try{
            string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;var official=ConscriptSourceInput.Schemas();Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=new[]{manifest,global}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).ToArray();var baseline=Xml(File.ReadAllText(Path.Combine(fixtures,"WeaponAuthoringProbe.xml")));var declarations=new List<string>();int checks=0;
            // Reborn: independent retail warhead/bomb records also encode an omitted clip sound as a null native pointer and null metadata target.
            var stockDocument=ManifestReader.Read(SdkEnvironmentPreflight.Read(manifest,64*1048576));
            foreach(string name in new[]{"WeaponTemplate:AlliedTimeBomb_Level1","WeaponTemplate:AlliedPrecisionBombWarhead"}){
                var asset=stockDocument.Assets.Single(a=>a.Name==name);if(asset.Tokenized!=1||asset.TypeHash!=0x27E8D984u)throw new InvalidDataException("Optional weapon stock fingerprint differs.");long offset=8;foreach(var item in stockDocument.Assets){if(ReferenceEquals(item,asset))break;offset+=item.InstanceDataSize;}
                var bytes=AssetStreamProbe.ReadRange(Path.ChangeExtension(manifest,".bin"),null,offset,asset.InstanceDataSize);var fields=GameObjectStockReview.Tokens(bytes,0);int nativeRoot=fields[FastHash.GetHashCode("AttackRange")].Offset-4;
                if(fields[FastHash.GetHashCode("FireSoundPerClip")].Offset!=0||BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(nativeRoot+88,4))!=0)throw new InvalidDataException("Retail null weapon sound pointer/metadata differ.");checks++;
            }
            for(int audio=0;audio<3;audio++)for(int fx=0;fx<3;fx++)for(int veteran=0;veteran<3;veteran++){
                var source=(XmlDocument)baseline.CloneNode(true);var root=source.DocumentElement!;root.SetAttribute("id",$"RebornOptionalWeapon_{audio}_{fx}_{veteran}");Set("FireSoundPerClip",audio);Set("FireFX",fx);Set("FireVeteranFX",veteran);
                var tokens=Check(source,out var native,out var instance);int shift=audio==0?4:0;
                if(native.InstanceBuffer.Length!=528-shift||tokens.InstanceBuffer.Length!=2508-shift||tokens.ImportsBuffer.Length!=0)throw new InvalidDataException("Optional weapon sound/FX token framing differs.");
                // Reborn: all populated fields retain the exact previously proved AK-47 tokenized bytes.
                if(audio==2&&fx==2&&veteran==2){var legacy=WeaponTokenReproduction.Encode(native);if(!legacy.InstanceBuffer.SequenceEqual(tokens.InstanceBuffer)||!legacy.RelocationBuffer.SequenceEqual(tokens.RelocationBuffer))throw new InvalidDataException("Populated weapon token golden changed.");checks++;}
                // Reborn: removing the sound nulls its metadata target; an explicit empty sound keeps an allocated zero word.
                if(BinaryPrimitives.ReadUInt32LittleEndian(tokens.InstanceBuffer.AsSpan(344,4))!=(audio==0?0u:2320u))throw new InvalidDataException("Optional sound metadata target differs.");
                if(audio==1&&BinaryPrimitives.ReadUInt32LittleEndian(native.InstanceBuffer.AsSpan(340,4))!=0)throw new InvalidDataException("Empty weapon sound allocated a nonzero reference.");
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[268]^=4;Refuse(()=>WeaponTokenReproduction.EncodeAuthored(corrupt));checks++;
                var badReference=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};BinaryPrimitives.WriteUInt32LittleEndian(badReference.InstanceBuffer.AsSpan(104,4),uint.MaxValue);Refuse(()=>ConscriptMultiAssetBuild.VerifyWeaponReferences(source,badReference,instance,schemas));checks++;
                declarations.Add(source.DocumentElement!.OuterXml);checks++;
                // Reborn: preserve the same XML spelling states instead of substituting a stock sound for silence.
                void Set(string name,int state){if(state==0)root.RemoveAttribute(name);else if(state==1)root.SetAttribute(name,"");}
            }
            // Reborn: every other already-described inline FX/audio word can carry an actual strong reference too.
            var extra=(XmlDocument)baseline.CloneNode(true);extra.DocumentElement!.SetAttribute("id","RebornAllWeaponReferences");extra.DocumentElement.SetAttribute("FireFlankFX","FX_MuzzleSoviet");extra.DocumentElement.SetAttribute("PreAttackFX","FX_MuzzleSoviet");extra.DocumentElement.SetAttribute("SingleAmmoReloadedNotFullSound","SOV_Conscript_GunFireMS");extra.DocumentElement.SetAttribute("ClipReloadedSound","SOV_Conscript_GunFireMS");Check(extra,out _,out _);declarations.Add(extra.DocumentElement.OuterXml);checks++;
            // Reborn: a real unsupported projectile nugget is diagnosed at the nugget, not at an absent optional sound.
            var projectile=(XmlDocument)baseline.CloneNode(true);projectile.DocumentElement!.RemoveAttribute("FireSoundPerClip");projectile.GetElementsByTagName("Nuggets",SchemaSet.XmlNamespace)[0]!.InnerXml="<ProjectileNugget xmlns='uri:ea.com:eala:asset' ProjectileTemplate='RebornProjectile' WarheadTemplate='RebornWarhead'/>";
            try{ConscriptMultiAssetBuild.AdmitWeapon(projectile,official,fixtures);throw new InvalidOperationException("Unimplemented projectile layout accepted.");}catch(InvalidDataException error){if(SdkXmlDiagnostics.Node(error)?.LocalName!="ProjectileNugget")throw;checks++;}
            string entry=Path.Combine(output,"mod.xml");File.WriteAllText(entry,"<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+string.Concat(declarations)+"</AssetDeclaration>");string prepared=Path.Combine(output,"prepared");ConscriptSourceInput.Prepare(entry,prepared);if(!ConscriptMultiAssetBuild.TryRun(Path.Combine(prepared,"game.xml"),manifest,global,Path.Combine(output,"built")))throw new InvalidDataException("Weapon-only source pipeline was not selected.");
            Console.WriteLine($"Reborn: optional weapon references: PASS ({checks} storage/reference/corruption controls; 27 absent/empty/populated combinations plus all seven sound/FX slots; two retail null-sound witnesses, unchanged populated stock token golden and 28-asset linked stream/package readback; no user output or game launch).");

            //-------------------------------------------------------------------------------------------------
            /** Reborn: run the real source admission, typed target resolution, Core normalization, native writer and authored token encoder. */
            //-------------------------------------------------------------------------------------------------
            Chunk Check(XmlDocument source,out Chunk native,out InstanceDeclaration instance){ConscriptMultiAssetBuild.AdmitWeapon(source,official,fixtures);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var copy=Xml(instance.XmlNode.OuterXml);foreach(XmlElement nugget in copy.GetElementsByTagName("DamageNugget",SchemaSet.XmlNamespace))nugget.SetAttribute("TypeId",FastHash.GetHashCode("DamageNuggetType").ToString());native=WeaponNativeProbe.Compile(copy);ConscriptMultiAssetBuild.VerifyWeaponReferences(source,native,instance,schemas);return WeaponTokenReproduction.EncodeAuthored(native);}
        }finally{Settings.Current=saved;}
    }
    // Reborn: private detached source copies keep tests away from editable Reborn XML.
    private static XmlDocument Xml(string text)=>SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(text));
    // Reborn: malformed native ownership or reference selectors must fail instead of silently entering a stream.
    private static void Refuse(Action action){try{action();}catch(InvalidDataException){return;}throw new InvalidOperationException("Invalid weapon payload accepted.");}
}
