using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: exercise particle source/native compilation and FX-to-particle dependencies without touching live mods.
internal static class FXParticleSystemCompilerSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare original stock, edited particles, omitted optional modules and final linked stream readback. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global,string output)
    {
        output=SdkEnvironmentPreflight.Absolute(output);if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh particle test output required.");Directory.CreateDirectory(output);var saved=Settings.Current;
        try{
            string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;var official=ConscriptSourceInput.Schemas();Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=new[]{manifest,global}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).ToArray();FXParticleSystemCompiler.Qualify(manifest,schemas,catalog);
            var original=GameObjectExtendedRootContract.Original("FXParticleSystems/Civ_Destruction_Tiny.xml","FXParticleSystemTemplate").Single();var assets=new List<string>();
            for(int i=0;i<4;i++){
                var source=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(original.OuterXml));source.DocumentElement!.SetAttribute("id","RebornParticle"+i);
                // Reborn: vary source scalars and defaults independently of the original stock template.
                if(i==1){source.DocumentElement.SetAttribute("id","Civ_Destruction_Weeny");source.DocumentElement.SetAttribute("Priority","ALWAYS_RENDER");foreach(var node in source.GetElementsByTagName("HalfSize").OfType<XmlElement>()){node.SetAttribute("x","8");node.SetAttribute("y","8");node.SetAttribute("z","8");}foreach(var node in source.GetElementsByTagName("Size").OfType<XmlElement>()){node.SetAttribute("Low","2");node.SetAttribute("High","3");}foreach(var node in source.GetElementsByTagName("SizeRate").OfType<XmlElement>()){node.SetAttribute("Low","2");node.SetAttribute("High","4");}foreach(var node in source.GetElementsByTagName("Gpu").OfType<XmlElement>()){node.SetAttribute("Shader","GpuParticleDistort.fx");}}
                if(i==2){foreach(var node in source.GetElementsByTagName("Gpu").OfType<XmlElement>()){node.SetAttribute("SingleRow","2");node.SetAttribute("EnableSoftParticles","false");}foreach(var node in source.GetElementsByTagName("Default").OfType<XmlElement>())foreach(var child in node.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName is "SizeRateDamping" or "AngularDampingXY").ToArray())node.RemoveChild(child);}
                // Reborn: both event variants must preserve ordered concrete classes and their stock FX imports.
                if(i==2){var events=source.GetElementsByTagName("Event").OfType<XmlElement>().Single();events.InnerXml="<Life xmlns='uri:ea.com:eala:asset' EventFX='FX_NONE' PerParticle='true'><EventTime Low='1' High='2'/></Life><Collision xmlns='uri:ea.com:eala:asset' EventFX='FX_NONE' OrientFXToTerrain='true'><HeightOffset Low='0' High='2'/></Collision>";}
                if(i==3){foreach(var node in source.DocumentElement.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName is "Physics" or "Draw" or "Volume" or "Velocity" or "Event"))node.RemoveAll();foreach(var node in source.DocumentElement.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="Update").ToArray())source.DocumentElement.RemoveChild(node);}
                var proof=FXParticleSystemCompiler.ProofSource(source,schemas,catalog);var normalized=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,normalized.OuterXml);var native=FXParticleSystemCompiler.Compile(instance.XmlNode,official);var refs=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();SchemaNativeContract.Verify(proof.DocumentElement!,typeof(FXParticleSystemTemplate),0,native,refs);
                // Reborn: changed native scalars must fail source validation even when references are unchanged.
                var bad=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};bad.InstanceBuffer[4]^=1;try{SchemaNativeContract.Verify(proof.DocumentElement!,typeof(FXParticleSystemTemplate),0,bad,refs);throw new InvalidOperationException("Corrupt particle root accepted.");}catch(InvalidDataException){}
                assets.Add(source.DocumentElement.OuterXml);
            }
            // Reborn: verify every official root shader ordinal and the omitted default; preserve the legacy KW enum value separately.
            string[] shaderNames={"INVALID_SHADER","ADDITIVE","ADDITIVE_ALPHA_TEST","ALPHA","ALPHA_TEST","MULTIPLY","ADDITIVE_NO_DEPTH_TEST","ALPHA_NO_DEPTH_TEST","W3D_ALPHA"};
            if((int)FXParticleSystem_ShaderType.W3D_ALPHA!=9)throw new InvalidDataException("Legacy KW shader enum was changed by the EP1 fix.");
            for(int shader=0;shader<=shaderNames.Length;shader++){
                var source=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(original.OuterXml));source.DocumentElement!.SetAttribute("id","RebornShader"+shader);
                if(shader==shaderNames.Length)source.DocumentElement.RemoveAttribute("Shader");else source.DocumentElement.SetAttribute("Shader",shaderNames[shader]);
                var proof=FXParticleSystemCompiler.ProofSource(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,SchemaNativeContract.Resolve(source,schemas,catalog).OuterXml);var native=FXParticleSystemCompiler.Compile(instance.XmlNode,official);
                SchemaNativeContract.Verify(proof.DocumentElement!,typeof(FXParticleSystemTemplate),0,native,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());
                if(System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(native.InstanceBuffer.AsSpan(8,4))!=(uint)(shader==shaderNames.Length?1:shader))throw new InvalidDataException("EP1 particle shader ordinal/default mismatch.");
                assets.Add(source.DocumentElement.OuterXml);
            }
            // Reborn: test complete retail RenderObject graphs and populated strings/counts/percentages at the 16-byte EP1 stride.
            var drawable=GameObjectExtendedRootContract.Original("FXParticleSystems/AlliedAAGun01.xml","FXParticleSystemTemplate").Single();
            for(int variant=0;variant<2;variant++){
                var source=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(drawable.OuterXml));source.DocumentElement!.SetAttribute("id","RebornDrawable"+variant);
                if(variant==1){var draw=source.GetElementsByTagName("Draw",SchemaSet.XmlNamespace).OfType<XmlElement>().Single().ChildNodes.OfType<XmlElement>().Single();draw.SetAttribute("SinkRate","1.5");draw.SetAttribute("MultiRenderObjects","true");draw.SetAttribute("SinkOnTerrainCollision","true");int item=0;foreach(var set in draw.ChildNodes.OfType<XmlElement>()){set.SetAttribute("RenderGroup","RebornGroup"+item);set.SetAttribute("NumObjects",(++item).ToString());set.SetAttribute("Percent",(item*20).ToString());}}
                // Reborn: nonzero X/Y/Z angle channels and the world-space rotation catch shifted legacy update fields.
                if(variant==1){var update=source.GetElementsByTagName("Update",SchemaSet.XmlNamespace).OfType<XmlElement>().Single().ChildNodes.OfType<XmlElement>().Single();update.SetAttribute("Rotation","ROTATE_IN_WORLD_SPACE");string[] names={"AngleX","AngularRateX","AngleY","AngularRateY","AngleZ","AngularRateZ","AngularDamping"};foreach(var child in update.ChildNodes.OfType<XmlElement>().Where(e=>names.Contains(e.LocalName)).ToArray())update.RemoveChild(child);var before=update.ChildNodes.OfType<XmlElement>().Single(e=>e.LocalName=="StartSize");for(int n=0;n<names.Length;n++){var child=source.CreateElement(names[n],SchemaSet.XmlNamespace);child.SetAttribute("Low",n.ToString());child.SetAttribute("High",(n+2).ToString());update.InsertBefore(child,before);}}
                var proof=FXParticleSystemCompiler.ProofSource(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,SchemaNativeContract.Resolve(source,schemas,catalog).OuterXml);var native=FXParticleSystemCompiler.Compile(instance.XmlNode,official);
                SchemaNativeContract.Verify(proof.DocumentElement!,typeof(FXParticleSystemTemplate),0,native,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());
                if(System.Runtime.InteropServices.Marshal.SizeOf<FXParticleDrawRenderObjectObjectSet>()!=20||System.Runtime.InteropServices.Marshal.SizeOf<Ep1FXParticleDrawRenderObjectObjectSet>()!=16)throw new InvalidDataException("Legacy/EP1 ObjectSet separation changed.");
                assets.Add(source.DocumentElement.OuterXml);
            }
            // Reborn: exercise all ending-change fields, omitted defaults and edited signed ranges before final packaging.
            var lightning=GameObjectExtendedRootContract.Original("FXParticleSystems/TeslaFireLightning01.xml","FXParticleSystemTemplate").Single();
            for(int variant=0;variant<3;variant++){
                var source=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(lightning.OuterXml));source.DocumentElement!.SetAttribute("id","RebornLightning"+variant);
                var sets=source.GetElementsByTagName("RandomSet",SchemaSet.XmlNamespace).OfType<XmlElement>().ToArray();
                if(variant==1)foreach(var set in sets){set.RemoveAll();int n=1;foreach(string name in new[]{"StartAmplitude","EndAmplitudeChange","StartFrequency","EndFrequencyChange","StartPhase","EndPhaseChange"}){var range=source.CreateElement(name,SchemaSet.XmlNamespace);range.SetAttribute("Low",(-n).ToString());range.SetAttribute("High",(n++).ToString());set.AppendChild(range);}}
                if(variant==2)foreach(var set in sets)set.RemoveAll();
                var proof=FXParticleSystemCompiler.ProofSource(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,SchemaNativeContract.Resolve(source,schemas,catalog).OuterXml);var native=FXParticleSystemCompiler.Compile(instance.XmlNode,official);
                SchemaNativeContract.Verify(proof.DocumentElement!,typeof(FXParticleSystemTemplate),0,native,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());assets.Add(source.DocumentElement.OuterXml);
            }
            // Reborn: link an authored FXList to a newly compiled particle identity through the normal production pipeline.
            assets.Add("<FXList id='RebornParticleFX'><NuggetList><ParticleSystem Particle='Civ_Destruction_Weeny'/></NuggetList></FXList>");// Reborn: instance-only particle bases and local definitions use the same Core inheritance route as other assets.
            var baseSource=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(original.OuterXml));baseSource.DocumentElement!.SetAttribute("id","RebornParticleBase");File.WriteAllText(Path.Combine(output,"base.xml"),"<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+baseSource.DocumentElement.OuterXml+"</AssetDeclaration>");assets.Add("<FXParticleSystemTemplate id='RebornParticleInherited' inheritFrom='RebornParticleBase' SystemLifetime='=$PARTICLE_LIFETIME'/>");
            string entry=Path.Combine(output,"mod.xml");File.WriteAllText(entry,"<AssetDeclaration xmlns='uri:ea.com:eala:asset'><Includes><Include type='instance' source='base.xml'/></Includes><Defines><Define name='PARTICLE_LIFETIME' value='3'/></Defines>"+string.Join("",assets)+"</AssetDeclaration>");string prepared=Path.Combine(output,"prepared");ConscriptSourceInput.Prepare(entry,prepared);if(!ConscriptMultiAssetBuild.TryRun(Path.Combine(prepared,"game.xml"),manifest,global,Path.Combine(output,"built")))throw new InvalidDataException("Particle production route not selected.");
            Console.WriteLine("Reborn: particle templates: PASS (complete original stock native graph, all nine shader ordinals/omitted default, original/edited 16-byte RenderObject lists, all lightning End*Change ranges/defaults, unchanged KW enum, edited GPU/volume/damping defaults, empty roles, both event kinds, instance/inheritance/definitions, corruption rejection, authored FX dependency and stream/package readback; no live mod publication or game launch).");
        }finally{Settings.Current=saved;}
    }
}
