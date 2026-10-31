// Reborn: adapter for EA Mod BuildStudio; publish Documents mods while preserving reference SDK/game installation files and never launching a game.
using System;
using System.Collections.Generic;
using System.IO;
using EALABuild;

namespace EALABuild
{
    class BuildScript : EALABuildScript
    {
        private string runtime;
        private string driver;
        // Reborn: require an explicit stock baseline location instead of inheriting the developer's default.
        private string staticManifest;
        // Reborn: optional legacy-config compatibility; weapon builds require an explicitly configured global manifest.
        private string globalManifest;
        // Reborn: standalone workspaces use checked prebuilt compiler and output directories, never hidden repository paths.
        private string compilerDirectory;
        private string outputRoot;
        // Reborn: the isolated CLR4 helper adds close protection without changing EA's executable or CodeDOM reference list.
        private Type lifecycle;
        private string buildHost;
        // Reborn: configuration failure must not leave the build callback using partially initialized adapter state.
        private bool initialized;

        //-------------------------------------------------------------------------------------------------
        /** Reborn: load explicit local adapter configuration instead of RA3 registry paths or inherited destructive cleanup stages. */
        //-------------------------------------------------------------------------------------------------
        public void initialize()
        {
            // Reborn: retry initialization explicitly after a prior failure; Install is idempotent in the lifecycle helper.
            initialized = false;
            BuildHelper.AutomaticallyResetBuildHelper = true;
            BuildHelper.EnableBenchmarking = true;
            // Reborn: EA's actual CodeDOM loader references only System.dll/EALABuild.dll, not System.Xml.dll.
            Dictionary<string, string> settings = ReadPaths(Path.Combine(StaticPaths.CurrentDirectory, "uprising-alpha.paths"));
            runtime = settings["PowerShell7"];
            driver = settings["Driver"];
            staticManifest = settings["StaticManifest"];
            globalManifest = settings.ContainsKey("GlobalManifest") ? settings["GlobalManifest"] : null;
            compilerDirectory = settings["CompilerDirectory"];
            outputRoot = Path.Combine(StaticPaths.CurrentDirectory, "builtmods");
            CheckPath(runtime); CheckPath(driver); CheckPath(staticManifest);
            if (globalManifest != null) CheckPath(globalManifest);
            CheckDirectory(compilerDirectory); CheckDirectory(outputRoot);
            // Reborn: an optional stock DATA override changes the library root, not the required four-key Studio workflow.
            if (settings.ContainsKey("SdkDataRoot")) CheckDirectory(settings["SdkDataRoot"]);
            buildHost = Path.Combine(StaticPaths.CurrentDirectory, "tools\\studio\\RebornStudioLifecycle.exe");
            CheckPath(buildHost);
            lifecycle = System.Reflection.Assembly.LoadFrom(buildHost).GetType("Reborn.StudioLifecycle", true);
            Lifecycle("Install");
            initialized = true;
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: keep the familiar selected-mod build action but invoke only the reviewed alpha backend; unsupported input must return failure. */
        //-------------------------------------------------------------------------------------------------
        public void buildMod(string modName, Dictionary<string, Object> gui)
        {
            // Reborn: surface the original initialization error instead of dereferencing a missing lifecycle/runtime later.
            if (!initialized) initialize();
            string name = BuildHelper.BuildTarget;
            // Reborn: the selected mod folder determines the validated source path and final package basename.
            if (String.IsNullOrEmpty(name) || name.Length > 48 || name.IndexOfAny(new char[] { '\\', '/', '"', ':', ';', '\r', '\n' }) >= 0 || name == "." || name == "..")
                throw new InvalidDataException("Invalid alpha mod name.");
            // Reborn: the static mod entry remains distinct from the stock DATA:static.xml library.
            string input = Path.Combine(Path.Combine(Path.Combine(StaticPaths.CurrentDirectory, "mods"), name), "data\\Static_Mod.xml");
            CheckPath(input);
            // Reborn: preserve the entered numeric version verbatim in final archive and SKUDef filenames.
            string version = gui.ContainsKey("modversion") ? (string)gui["modversion"] : "1.00";
            if (!System.Text.RegularExpressions.Regex.IsMatch(version ?? "", @"\A[0-9]{1,4}(\.[0-9]{1,4}){1,3}\z"))
                throw new InvalidDataException("Mod Version must be numeric, for example 1.00 or 1.2.3.");
            BuildHelper.CurrentBuildSteps = 1;
            // Reborn: separate the user's Reborn workspace from developer tests, including the strictly bounded complete Conscript build.
            // Reborn: keep the build header short; backend warnings describe the selected profile and detailed receipts stay on disk.
            BuildHelper.DisplayLine("Uprising SDK Alpha. Unsupported requested features fail the build; previous successful output is retained on failure.");
            // Reborn: session resources exist before EA starts the host; the suspended worker cannot escape its own job.
            string session = (string)Lifecycle("Begin");
            RunExecutableArguments request = new RunExecutableArguments(buildHost);
            // Reborn: bind every Studio build to the configured read-only stock baseline.
            request.Arguments = "--run " + session + " \"" + runtime + "\" -NoProfile -File \"" + driver + "\" -StudioSession " + session + " -ModXml \"" + input + "\" -StaticManifest \"" + staticManifest
                + "\" -CompilerDirectory \"" + compilerDirectory + "\" -OutputRoot \"" + outputRoot + "\" -ModVersion \"" + version + "\"";
			bool buildAptUI = gui.ContainsKey("buildaptui") && Convert.ToBoolean(gui["buildaptui"]);
			request.Arguments += " -BuildAptUI " + (buildAptUI ? "1" : "0");
            // Reborn: preserve four-key armor workspaces without inferring a weapon dependency path.
            if (globalManifest != null) request.Arguments += " -GlobalManifest \"" + globalManifest + "\"";
            // Reborn: EA persists this native checkbox; pass an explicit numeric value so unchecked is never lost in switch binding.
            bool buildGlobal = !gui.ContainsKey("buildglobal") || Convert.ToBoolean(gui["buildglobal"]);
            request.Arguments += " -BuildGlobalData " + (buildGlobal ? "1" : "0");
            // Reborn: unchecked is the normal fast build; an explicit checkbox enables developer repeat/corruption/stock comparison tests.
            bool developerTests = gui.ContainsKey("developertests") && Convert.ToBoolean(gui["developertests"]);
            request.Arguments += " -DeveloperBuildTests " + (developerTests ? "1" : "0");
            request.RedirectOutput = true; request.CreateWindow = false;
            // Reborn: retain the SDK's step-counter protocol before RunStep advances it for the callback.
            try
            {
                if (BuildHelper.GetNextExecutableStep() != 1)
                    throw new InvalidDataException("Unexpected alpha build step state.");
                BuildHelper.RunStep(StepType.RunExecutable, request);
            }
            catch { Lifecycle("Complete"); throw; }
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: complete the single backend operation without advancing to unvalidated RA3 postprocessors. */
        //-------------------------------------------------------------------------------------------------
        public void onStepSuccess(int stepId) { Lifecycle("Complete"); BuildHelper.OnBuildComplete(); }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: make an unsuccessful backend visible and retain diagnostic output without automatic retry or cleanup. */
        //-------------------------------------------------------------------------------------------------
        public void onStepFailure(int stepId) { Lifecycle("Complete"); BuildHelper.DisplayLine("Alpha build failed; no install or launch was attempted. Review the build output."); }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: keep Windows Forms types in the separately compiled helper, preserving EA's exact two-reference CodeDOM contract. */
        //-------------------------------------------------------------------------------------------------
        private object Lifecycle(string method) { return lifecycle.GetMethod(method).Invoke(null, null); }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: this alpha has exactly one indivisible validation/build/package operation. */
        //-------------------------------------------------------------------------------------------------
        public bool shouldExecuteStep(int stepId) { return stepId == 1; }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: retain EA's options pane while explicitly describing this limited backend. */
        //-------------------------------------------------------------------------------------------------
        public List<GUIElementData> GetModBuildGUIData()
        {
            List<GUIElementData> options = new List<GUIElementData>();
            options.Add(new GUIElementData("alpha", "Uprising Alpha: Validate XML / Build Data / Make BIG", GUIElementData.LabelType, ScriptIndex.ModScript));
            options.Add(new GUIElementData("scope", "Fixed yard samples or 1..32 source armors with Includes/Defines/inheritFrom (experimental)", GUIElementData.LabelType, ScriptIndex.ModScript));
            // Reborn: distinguish reviewed variable behavior shapes from unrestricted nested GameObject/resource authoring.
            options.Add(new GUIElementData("weapon", "Conscript: reviewed variable Behaviors + fixed fields; unsupported layouts fail", GUIElementData.LabelType, ScriptIndex.ModScript));
            options.Add(new GUIElementData("buildaptui", "Build AptUI", GUIElementData.CheckBoxType, ScriptIndex.ModScript));
			// Reborn: compile AdditionalMaps/MapMetaData_*.xml before static data, using EA's familiar persisted toggle.
            options.Add(new GUIElementData("buildglobal", "Build Global Data", GUIElementData.CheckBoxType, ScriptIndex.ModScript));
            // Reborn: EA's new checkbox defaults unchecked and persists the user's later selection using its own GUI data store.
            options.Add(new GUIElementData("developertests", "Developer Build Tests", GUIElementData.CheckBoxType, ScriptIndex.ModScript));
            // Reborn: use EA's own persisted text-input control rather than a separate settings interface.
            options.Add(new GUIElementData("mv", "Mod Version:", GUIElementData.LabelType, ScriptIndex.ModScript));
            options.Add(new GUIElementData("modversion", "1.00", GUIElementData.InputBoxType, ScriptIndex.ModScript));
            return options;
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: reject command-line quoting ambiguity and missing explicit local paths before requesting an external process. */
        //-------------------------------------------------------------------------------------------------
        private static void CheckPath(string path)
        {
            if (String.IsNullOrEmpty(path) || !Path.IsPathRooted(path) || path.IndexOf('"') >= 0 || path.IndexOf('\r') >= 0 || path.IndexOf('\n') >= 0 || !File.Exists(path))
                throw new InvalidDataException("Adapter requires existing absolute file paths.");
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: require explicitly configured existing directories with unambiguous process argument quoting. */
        //-------------------------------------------------------------------------------------------------
        private static void CheckDirectory(string path)
        {
            if (String.IsNullOrEmpty(path) || !Path.IsPathRooted(path) || path.IndexOf('"') >= 0 || path.IndexOf('\r') >= 0 || path.IndexOf('\n') >= 0 || !Directory.Exists(path))
                throw new InvalidDataException("Adapter requires existing absolute directory paths.");
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: require four legacy keys and accept optional backend source, localization and WorldBuilder lookup settings. */
        //-------------------------------------------------------------------------------------------------
        private static Dictionary<string, string> ReadPaths(string path)
        {
            if (new FileInfo(path).Length > 16384) throw new InvalidDataException("Alpha path configuration exceeds bound.");
            Dictionary<string, string> settings = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (string line in File.ReadAllLines(path))
            {
                if (line.Length == 0 || line.StartsWith("#", StringComparison.Ordinal)) continue;
                int separator = line.IndexOf('=');
                if (separator <= 0) throw new InvalidDataException("Invalid alpha path setting.");
                string key = line.Substring(0, separator);
                // Reborn: official source root is used only by the bounded Conscript build driver; legacy configurations remain valid.
                // Reborn: backend localization availability is optional; accept its setting without making the CSF a startup dependency.
                // Reborn: WorldBuilderManifest is consumed by dependency diagnostics, not passed as a runtime stream or copied into packages.
                if ((key != "PowerShell7" && key != "Driver" && key != "StaticManifest" && key != "CompilerDirectory" && key != "GlobalManifest" && key != "WorldBuilderManifest" && key != "UprisingXmlRoot" && key != "SdkDataRoot" && key != "StockStrings") || settings.ContainsKey(key))
                    throw new InvalidDataException("Unknown or duplicate alpha path setting.");
                settings.Add(key, line.Substring(separator + 1));
            }
            if (!settings.ContainsKey("PowerShell7") || !settings.ContainsKey("Driver") || !settings.ContainsKey("StaticManifest") || !settings.ContainsKey("CompilerDirectory"))
                throw new InvalidDataException("Alpha requires all four base path settings; GlobalManifest enables weapon builds.");
            return settings;
        }
    }
}
