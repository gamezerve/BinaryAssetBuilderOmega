//===================================================//
//===== ABOUT =======================================//
//===================================================//
//                                                   //
// Extended build script for the Red Alert 3 Mod SDK //
//                                                   //
// Author: Bibber                                    //
// eMail: m.bibber@live.com                           //
// Homepage: http://bibber.bplaced.net               //
//                                                   //
//===================================================//
//===== PROGRAM =====================================//
//===================================================//

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Win32;

namespace EALABuild
{
    class BuildScript: EALABuildScript
    {
		#region Member variables
        Dictionary<string, Object> currentGUIData;
        static int MOD_BUILD_STEPS = 10;
        static string DEFAULT_GAME_VERSION = "1.12";
		static string DEFAULT_MOD_VERSION = "1.00";
		
		// Variables
		object Reg;
		string Cmd = Environment.GetEnvironmentVariable("comspec");
		string PersonalDirectory;
		string SDKDirectory;
		string UserDataLeafName;
		
		string RA3Dir;
		string RA3DataDir;
		string BuiltModsPath;
		string BinaryAssetBuilder;
		string AssetResolver;
		string HashFix;
		string LoDStreamBuilder;
		string MakeBig;
		string AssetMerger;
		string StreamSorter;
		
		string Mod;
		string ModPath;
		string ModAdditionalFilesPath;
		string ModAssetsPath;
		string ModDataPath;
		string BuiltModPath;
		string BuiltModDataPath;
		string ModInstallPath;
		
		string ModBabProj;
		string ModXml;
		string ModBase;
		string ModBigMisc;
		string ModBigStreams;
		string ModSkudef;
		#endregion

		#region Implementation of EALABuildScript interface.
        public static void CopyFolders(string source, string destination)
        {
			if (!Directory.Exists(destination)) Directory.CreateDirectory(destination);
			
            DirectoryInfo di = new DirectoryInfo(source);
            CopyFiles(source, destination);

            foreach (DirectoryInfo d in di.GetDirectories())
            {
                string newDir = Path.Combine(destination, d.Name);
                CopyFolders(d.FullName, newDir);
            }
        }

        public static void CopyFiles(string source, string destination)
        {
            DirectoryInfo di = new DirectoryInfo(source);
            FileInfo[] files = di.GetFiles();

            foreach (FileInfo f in files)
            {
                string sourceFile = f.FullName;
                string destFile = Path.Combine(destination, f.Name);
                File.Copy(sourceFile, destFile, true);
            }
        }

		// This function is called immediately after the script constructor in the GUI load process.
        public void initialize()
        {
			/*
			* This will automatically reset the counter in BuildHelper, and the GUI will
			* set the type of build in BuildHelper.  If it is not enabled, call 
			* BuildHelper.Reset, and set BuildHelper.CurrentBuildType = to either BuildType.ModBuild
			* or BuildType.MapBuild in the buildMap and buildMod functions
			*/
			
            BuildHelper.AutomaticallyResetBuildHelper = true;
            BuildHelper.EnableBenchmarking = true;
			
			// Personal Directory
			PersonalDirectory = Environment.ExpandEnvironmentVariables(Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\User Shell Folders", "Personal", "").ToString());
			
			// SDK Directory
			Reg = Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{F6A3F605-7B10-4939-8D3D-4594332C1649}", "InstallLocation", null);
			if (Reg == null) { 
				Reg = Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{F6A3F605-7B10-4939-8D3D-4594332C1649}", "InstallLocation", null);
				if (Reg == null) { 
					SDKDirectory = StaticPaths.CurrentDirectory;
				} else {
					SDKDirectory = Reg.ToString();
				}
			} else {
				SDKDirectory = Reg.ToString();
			}
			
			// Game Install Dir
			Reg = Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Electronic Arts\\Electronic Arts\\Red Alert 3", "Install Dir", null);
			if (Reg == null) { 
				Reg = Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Wow6432Node\\Electronic Arts\\Electronic Arts\\Red Alert 3", "Install Dir", null);
				if (Reg == null) { 
					RA3Dir = StaticPaths.CurrentDirectory;
				} else {
					RA3Dir = Reg.ToString();
				}
			} else {
				RA3Dir = Reg.ToString();
			}
			
			// User Data Leaf Name
			Reg = Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Electronic Arts\\Electronic Arts\\Red Alert 3", "UserDataLeafName", null);
			if (Reg == null) { 
				Reg = Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Wow6432Node\\Electronic Arts\\Electronic Arts\\Red Alert 3", "UserDataLeafName", null);
				if (Reg == null) { 
					UserDataLeafName = "Red Alert 3";
				} else {
					UserDataLeafName = Reg.ToString();
				}
			} else {
				UserDataLeafName = Reg.ToString();
			}
			
			// Variables
			RA3DataDir = RA3Dir + "\\data";
			BuiltModsPath = SDKDirectory + "\\builtmods";
			BinaryAssetBuilder = SDKDirectory + "\\tools\\binaryassetbuilder.exe";
			AssetResolver = SDKDirectory + "\\tools\\modassetresolver.exe";
			HashFix = SDKDirectory + "\\tools\\hashfix.exe";
			LoDStreamBuilder = SDKDirectory + "\\tools\\lodstreambuilder.exe";
			MakeBig = SDKDirectory + "\\tools\\makebig.exe";
			AssetMerger = SDKDirectory + "\\tools\\assetmerger.exe";
			StreamSorter = SDKDirectory + "\\tools\\streamsorter.exe";
        }

		// This function is called on the successful completion of a build step.
        public void onStepSuccess(int stepId)
        {
            if (BuildHelper.GetNextExecutableStep() != -1)
            {
                executeModBuildStep(BuildHelper.CurrentStep);
            }
            else
            {
                BuildHelper.OnBuildComplete();
            }
        }

		// This function is called on the failure of a build step.
        public void onStepFailure(int stepId)
        {
            BuildHelper.DisplayLine(String.Format("Build failed on step {0}", BuildHelper.CurrentStep-1));
        }

		// This function is called to determine if a step should execute.
        public bool shouldExecuteStep(int stepId)
        {
            if (currentGUIData.ContainsKey("step" + stepId))
            {
                if ((bool)currentGUIData["step" + stepId])
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

		// This function is called when the user elects to build a mod
        public void buildMod(string modName, Dictionary<string, Object> GUIResults)
        {
            currentGUIData = GUIResults;
            BuildHelper.CurrentBuildSteps = MOD_BUILD_STEPS;
			
			//Variables
			Mod = BuildHelper.BuildTarget;
			ModPath = SDKDirectory + "\\mods\\" + Mod;
			ModAdditionalFilesPath = ModPath + "\\misc";
			ModAssetsPath = ModPath + "\\assets";
			ModDataPath = ModPath + "\\data";
			BuiltModPath = BuiltModsPath + "\\mods\\" + Mod;
			BuiltModDataPath = BuiltModPath + "\\data";
			ModInstallPath = PersonalDirectory + "\\" + UserDataLeafName + "\\Mods\\" + Mod;
			
			ModBabProj = ModPath + "\\mod.babproj";
			ModXml = ModDataPath + "\\mod.xml";
			ModBase = Mod + "_" + (string)currentGUIData["modversion"];
			ModBigMisc = ModBase + "_Misc.big";
			ModBigStreams = ModBase + "_Streams.big";
			ModSkudef = ModBase + ".skudef";
			
            executeModBuildStep(BuildHelper.GetNextExecutableStep());
        }
		
		/*
		* This function is called during the GUI initialization to generate
		* the GUI for the mod build options section.
		*/
        public List<GUIElementData> GetModBuildGUIData()
        {
            List<GUIElementData> modGUIData = new List<GUIElementData>();
			modGUIData.Add(new GUIElementData("step1", "Clear Build Cache", GUIElementData.CheckBoxType, ScriptIndex.ModScript));
			modGUIData.Add(new GUIElementData("step2", "Clear Session Cache", GUIElementData.CheckBoxType, ScriptIndex.ModScript));
			modGUIData.Add(new GUIElementData("step3", "Clear Built Mod", GUIElementData.CheckBoxType, ScriptIndex.ModScript));
			modGUIData.Add(new GUIElementData("sp1", "", GUIElementData.LabelType, ScriptIndex.ModScript));
			modGUIData.Add(new GUIElementData("step4", "Build AptUI", GUIElementData.CheckBoxType, ScriptIndex.ModScript));
            modGUIData.Add(new GUIElementData("step5", "Build Global Data", GUIElementData.CheckBoxType, ScriptIndex.ModScript));
            modGUIData.Add(new GUIElementData("step6", "Build Static Data", GUIElementData.CheckBoxType, ScriptIndex.ModScript));
			modGUIData.Add(new GUIElementData("step7", "Merge Assets", GUIElementData.CheckBoxType, ScriptIndex.ModScript));
            modGUIData.Add(new GUIElementData("step8", "Fix Assets", GUIElementData.CheckBoxType, ScriptIndex.ModScript));
            modGUIData.Add(new GUIElementData("sp2", "", GUIElementData.LabelType, ScriptIndex.ModScript));
			modGUIData.Add(new GUIElementData("step9", "Create BIG Files", GUIElementData.CheckBoxType, ScriptIndex.ModScript));
			modGUIData.Add(new GUIElementData("step10", "Create Skudef File", GUIElementData.CheckBoxType, ScriptIndex.ModScript));
            modGUIData.Add(new GUIElementData("mv", "Mod Version:", GUIElementData.LabelType, ScriptIndex.ModScript));
            modGUIData.Add(new GUIElementData("modversion", DEFAULT_MOD_VERSION, GUIElementData.InputBoxType, ScriptIndex.ModScript));
			return modGUIData;
        }
        #endregion
	
        private void executeModBuildStep(int stepId)
        {
            switch(stepId)
            {
                case 1:
					BuildHelper.DisplayLine(String.Format("Step {0}: Clearing build cache",
							stepId));
                    RunExecutableArguments clearBuildCache = new RunExecutableArguments(Cmd);
                    clearBuildCache.Arguments = String.Format("/C (@echo off) & (cd /D \"{0}\")"
						+ " & (if exist \"{1}\\cache\" (rd \"{1}\\cache\" /S /Q))",
						SDKDirectory, BuiltModsPath);
					BuildHelper.RunStep(StepType.RunExecutable, clearBuildCache);
                    break;
                case 2:
					BuildHelper.DisplayLine(String.Format("Step {0}: Clearing session cache",
							stepId));
                    RunExecutableArguments clearSessionCache = new RunExecutableArguments(Cmd);
                    clearSessionCache.Arguments = String.Format("/C (@echo off) & (cd /D \"{0}\")"
						+ " & (if exist \"{1}\\builtmods\" (rd \"{1}\\builtmods\" /S /Q))"
						+ " & (for /R \"{2}\" %I in (\"*.asset\") do (del \"%I\" /F /Q))"
						+ " & (if exist \"{1}\\binaryassetbuilder.sessioncache.xml\" (del \"{1}\\binaryassetbuilder.sessioncache.xml\" /F /Q))"
						+ " & (if exist \"{1}\\binaryassetbuilder.sessioncache.xml.old\" (del \"{1}\\binaryassetbuilder.sessioncache.xml.old\" /F /Q))"
						+ " & (if exist \"{1}\\binaryassetbuilder.sessioncache.xml.deflate\" (del \"{1}\\binaryassetbuilder.sessioncache.xml.deflate\" /F /Q))"
						+ " & (if exist \"{1}\\binaryassetbuilder.sessioncache.xml.deflate.old\" (del \"{1}\\binaryassetbuilder.sessioncache.xml.deflate.old\" /F /Q))"
						+ " & (if exist \"{1}\\stringhashes.xml\" (del \"{1}\\stringhashes.xml\" /F /Q))",
						SDKDirectory, BuiltModsPath, BuiltModPath);
					BuildHelper.RunStep(StepType.RunExecutable, clearSessionCache);
                    break;
                case 3:
					BuildHelper.DisplayLine(String.Format("Step {0}: Clearing built mod",
							stepId));
                    RunExecutableArguments clearMod = new RunExecutableArguments(Cmd);
                    clearMod.Arguments = String.Format("/C (@echo off) & (cd /D \"{0}\")"
						+ " & (for /R \"{1}\" %I in (\"*.*\") do ("
							+ "(if not \"%~xI\" == \".asset\" (del \"%I\" /F /Q))"
						+ "))",
						SDKDirectory, BuiltModPath);
					BuildHelper.RunStep(StepType.RunExecutable, clearMod);
                    break;
                case 4:
					BuildHelper.DisplayLine(String.Format("Step {0}: Building AptUI",
						stepId));
					RunExecutableArguments argsAptUI = new RunExecutableArguments(Cmd);
					argsAptUI.Arguments = String.Format("/C (@echo off) & (cd /D \"{0}\")"
						+ " & (for %I in (\"{1}\\aptui\\*\") do (del \"%I\" /F /Q))"
						+ " & (for %I in (\"{3}\\aptui\\*.xml\") do ("
							+ "(\"{2}\" \"%I\" /od:\"{4}\" /iod:\"{4}\" /csc:false /ls:true /osh:false /pc:true /res:true /slowclean:true /ss:true /art:\".;.\\Mods\\{5}\\Art;.\\Mods;.\\Art\" /audio:\".;.\\Mods\\{5}\\Audio;.\\Mods;.\\Audio\" /data:\".;.\\Mods\\{5}\\Data;.\\Mods;.\\SageXml\")"
							+ " & (if exist \"{1}\\aptui\\%~nI.manifest\" (echo. >\"{1}\\aptui\\%~nI.version\"))"
						+ "))",
						SDKDirectory, BuiltModDataPath, BinaryAssetBuilder, ModDataPath, BuiltModsPath, Mod);
					BuildHelper.RunStep(StepType.RunExecutable, argsAptUI);
					break;
                case 5:
					BuildHelper.DisplayLine(String.Format("Step {0}: Building global data",
						stepId));
					RunExecutableArguments argsGlobal = new RunExecutableArguments(Cmd);
					argsGlobal.Arguments = String.Format("/C (@echo off) & (cd /D \"{0}\")"
						+ " & (for %I in (\"{1}\\additionalmaps\\mapmetadata_*\") do (del \"%I\" /F /Q))"
						+ " & (for %I in (\"{3}\\additionalmaps\\mapmetadata_*.xml\") do ("
							+ "(\"{2}\" \"%I\" /od:\"{4}\" /iod:\"{4}\" /csc:false /ls:true /osh:false /pc:true /res:true /slowclean:true /ss:true /art:\".;.\\Mods\\{5}\\Art;.\\Mods;.\\Art\" /audio:\".;.\\Mods\\{5}\\Audio;.\\Mods;.\\Audio\" /data:\".;.\\Mods\\{5}\\Data;.\\Mods;.\\SageXml\")"
							+ " & (if exist \"{1}\\additionalmaps\\%~nI.manifest\" (echo. >\"{1}\\additionalmaps\\%~nI.version\"))"
						+ "))",
						SDKDirectory, BuiltModDataPath, BinaryAssetBuilder, ModDataPath, BuiltModsPath, Mod);
					BuildHelper.RunStep(StepType.RunExecutable, argsGlobal);
					break;
                case 6:
					BuildHelper.DisplayLine(String.Format("Step {0}: Building static data",
						stepId));
                    RunExecutableArguments args = new RunExecutableArguments(Cmd);
					args.Arguments = String.Format("/C (@echo off) & (cd /D \"{0}\")"
						+ " & (if exist \"{1}\\mod.bin\" (del \"{1}\\mod.bin\" /F /Q))"
						+ " & (if exist \"{1}\\mod.imp\" (del \"{1}\\mod.imp\" /F /Q))"
						+ " & (if exist \"{1}\\mod.manifest\" (del \"{1}\\mod.manifest\" /F /Q))"
						+ " & (if exist \"{1}\\mod.relo\" (del \"{1}\\mod.relo\" /F /Q))"
						+ " & (if exist \"{1}\\mod.version\" (del \"{1}\\mod.version\" /F /Q))"
						+ " & (for %I in (\"{1}\\mod_*\") do (del \"%I\" /F /Q))"
						+ " & (\"{2}\" \"{3}\" /od:\"{4}\" /iod:\"{4}\" /csc:false /ls:true /osh:false /pc:true /res:true /slowclean:true /ss:true /art:\".;.\\Mods\\{5}\\Art;.\\Mods;.\\Art\" /audio:\".;.\\Mods\\{5}\\Audio;.\\Mods;.\\Audio\" /data:\".;.\\Mods\\{5}\\Data;.\\Mods;.\\SageXml\")"
						+ " & (if exist \"{1}\\mod.manifest\" (echo.>>\"{1}\\mod.version\"))",
						SDKDirectory, BuiltModDataPath, BinaryAssetBuilder, ModXml, BuiltModsPath, Mod);
					
					if (((bool)currentGUIData["step7"] == false) && ((bool)currentGUIData["step8"] == false)) {
						args.Arguments = args.Arguments + String.Format(" & (\"{0}\" \"{1}\\mod.manifest\" /f)",
							LoDStreamBuilder, BuiltModDataPath);
					}
					
					BuildHelper.RunStep(StepType.RunExecutable, args);
                    break;
                case 7:
					BuildHelper.DisplayLine(String.Format("Step {0}: Merging assets",
						stepId));
					RunExecutableArguments argsMerge = new RunExecutableArguments(Cmd);
					argsMerge.Arguments = String.Format("/V:ON /C (@echo off) & (cd /D \"{0}\")"
						+ " & (if exist \"{2}\\mod.manifest\" (if exist \"{3}\\mod\" ("
							+ "(\"{1}\" \"{2}\\mod.manifest\" \"{3}\\mod\")"
						+ ")))"
						+ " & (for %I in (\"{2}\\additionalmaps\\mapmetadata_*.manifest\") do ("
							+ "(if exist \"{3}\\additionalmaps\\%~nI\" ("
								+ "(\"{1}\" \"%I\" \"{3}\\additionalmaps\\%~nI\")"
								+ " & (\"{4}\" \"%I\")"
							+ "))"
						+ "))"
						+ " & (for %I in (\"{2}\\aptui\\*.manifest\") do ("
							+ "(if exist \"{3}\\aptui\\%~nI\" ("
								+ "(\"{1}\" \"%I\" \"{3}\\aptui\\%~nI\")"
								+ " & (\"{4}\" \"%I\")"
							+ "))"
						+ "))"
						+ " & (cd /D \"{0}\")",
						SDKDirectory, AssetMerger, BuiltModDataPath, ModAssetsPath, StreamSorter);

					if ((bool)currentGUIData["step8"] == false) {
						argsMerge.Arguments = argsMerge.Arguments + String.Format(" & (if exist \"{1}\\mod.manifest\" ("
							+ "(\"{2}\" \"{1}\\mod.manifest\")"
							+ " & (\"{0}\" \"{1}\\mod.manifest\" /f)"
						+ "))",
							LoDStreamBuilder, BuiltModDataPath, StreamSorter);
					}
					
					BuildHelper.RunStep(StepType.RunExecutable, argsMerge);
					break;
                case 8:
					BuildHelper.DisplayLine(String.Format("Step {0}: Fixing assets",
						stepId));
					RunExecutableArguments argsFix = new RunExecutableArguments(Cmd);
					argsFix.Arguments = String.Format("/C (@echo off) & (cd /D \"{0}\")"
						+ " & (\"{2}\" \"{1}\\mod.manifest\" \"{6}\\sagexml\\worldbuilder.manifest\")"
						+ " & (\"{3}\" \"{1}\\mod.manifest\" \"{7}\\wbdata_12.big\" \"{6}\\audio.manifest\" \"{6}\\global.manifest\" \"{6}\\static.manifest\" \"{6}\\stringhashes.xml\")"
						+ " & (\"{4}\" \"{1}\\mod.manifest\")"
						+ " & (\"{5}\" \"{1}\\mod.manifest\" /f)",
						SDKDirectory, BuiltModDataPath, HashFix, AssetResolver, StreamSorter, LoDStreamBuilder, BuiltModsPath, RA3DataDir);
					BuildHelper.RunStep(StepType.RunExecutable, argsFix);
					break;
                case 9:
					BuildHelper.DisplayLine(String.Format("Step {0}: Creating big files",
							stepId));
					RunExecutableArguments argsBig = new RunExecutableArguments(Cmd);
					argsBig.Arguments = String.Format("/C (@echo off) & (cd /D \"{0}\")"
						+ " & (if exist \"{2}\" (\"{1}\" -f \"{2}\" -x:*.asset -o:\"{3}\\{4}\"))"
						+ " & (if exist \"{6}\" (\"{1}\" -f \"{6}\" -x:*.asset -o:\"{3}\\{5}\"))",
						SDKDirectory, MakeBig, BuiltModPath, ModInstallPath, ModBigStreams, ModBigMisc, ModAdditionalFilesPath);
                    BuildHelper.RunStep(StepType.RunExecutable, argsBig);
                    break;
                case 10:
					BuildHelper.DisplayLine(String.Format("Step {0}: Creating skudef file",
							stepId));
					RunExecutableArguments argsSkudef = new RunExecutableArguments(Cmd);
					argsSkudef.Arguments = String.Format("/C (@echo off) & (cd /D \"{0}\")"
						+ " & (if exist \"{0}\\{2}\" (del \"{0}\\{2}\" /F /Q))"
						+ " & (echo mod-game {3}>\"{2}\")"
						+ " & (if exist \"{0}\\{1}\" (echo add-big {1}>>\"{2}\"))"
						+ " & (if exist \"{0}\\{4}\" (echo add-big {4}>>\"{2}\"))",
						ModInstallPath, ModBigStreams, ModSkudef, DEFAULT_GAME_VERSION, ModBigMisc);
                    BuildHelper.RunStep(StepType.RunExecutable, argsSkudef);
                    break;
                default:
                    // unknown build step, this is probably the end
					onStepSuccess(-1);
                    break;
            }
        }
    }
}

//===================================================//
//===================================================//
//===================================================//