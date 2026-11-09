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

        // Reborn: anchor portable paths to the running Studio application; referenced assemblies may be shadow-copied on another computer.
        private static string SdkRoot { get { return ResolveSdkRoot(); } }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: prefer the executable application base and retain the EA assembly directory only as a compatibility fallback. */
        //-------------------------------------------------------------------------------------------------
        private static string ResolveSdkRoot()
        {
            string applicationRoot = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
            if (File.Exists(Path.Combine(applicationRoot, "uprising-alpha.paths"))) return applicationRoot;
            string assemblyRoot = Path.GetDirectoryName(typeof(EALABuildScript).Assembly.Location);
            if (!String.IsNullOrEmpty(assemblyRoot) && File.Exists(Path.Combine(assemblyRoot, "uprising-alpha.paths"))) return Path.GetFullPath(assemblyRoot);
            throw new InvalidDataException("Portable SDK root could not be found beside EALAModStudio.exe. Expected: " + Path.Combine(applicationRoot, "uprising-alpha.paths"));
        }

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
            Dictionary<string, string> settings = ReadPaths(Path.Combine(SdkRoot, "uprising-alpha.paths"));
            runtime = settings["PowerShell7"];
            driver = settings["Driver"];
            staticManifest = settings["StaticManifest"];
            globalManifest = settings.ContainsKey("GlobalManifest") ? settings["GlobalManifest"] : null;
            compilerDirectory = settings["CompilerDirectory"];
            outputRoot = Path.Combine(SdkRoot, "builtmods");
            CheckPath(runtime, "PowerShell 7 runtime"); CheckPath(driver, "build driver"); CheckPath(staticManifest, "static manifest");
            if (globalManifest != null) CheckPath(globalManifest, "global manifest");
            // Reborn: a fresh portable copy need not carry previous internal receipts or its empty output directory.
            Directory.CreateDirectory(outputRoot);
            CheckDirectory(compilerDirectory); CheckDirectory(outputRoot);
            // Reborn: an optional stock DATA override changes the library root, not the required four-key Studio workflow.
            if (settings.ContainsKey("SdkDataRoot")) CheckDirectory(settings["SdkDataRoot"]);
            buildHost = Path.Combine(SdkRoot, "tools\\studio\\RebornStudioLifecycle.exe");
            CheckPath(buildHost, "Studio lifecycle helper");
            lifecycle = System.Reflection.Assembly.LoadFrom(buildHost).GetType("Reborn.StudioLifecycle", true);
            Lifecycle("Install");
            initialized = true;
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: keep the familiar selected-mod build action but invoke only the reviewed alpha backend; unsupported input must return failure. */
        //-------------------------------------------------------------------------------------------------
        public void buildMod(string modName, Dictionary<string, Object> gui)
        {
            try
            {
                // Reborn: surface the original initialization error instead of dereferencing a missing lifecycle/runtime later.
                if (!initialized) initialize();
                // Reborn: mark even preflight/selection validation as active, and let the outer failure path stop the indicator.
                Lifecycle("BuildStarting");
                string name = BuildHelper.BuildTarget;
                // Reborn: the selected mod folder determines the validated source path and final package basename.
                if (String.IsNullOrEmpty(name) || name.Length > 48 || name.IndexOfAny(new char[] { '\\', '/', '"', ':', ';', '\r', '\n' }) >= 0 || name == "." || name == "..")
                    throw new InvalidDataException("Invalid alpha mod name.");
                // Reborn: the static mod entry remains distinct from the stock DATA:static.xml library.
                string input = Path.Combine(Path.Combine(Path.Combine(SdkRoot, "mods"), name), "data\\Static_Mod.xml");
                // Reborn: map/global/AptUI-only checks use the selected Data directory without requiring Static_Mod.xml.
                bool buildStatic = !gui.ContainsKey("buildstatic") || Convert.ToBoolean(gui["buildstatic"]);
                bool createBig = !gui.ContainsKey("createbig") || Convert.ToBoolean(gui["createbig"]);
                if (buildStatic) CheckPath(input, "selected mod '" + name + "' entry XML");
                // Reborn: preserve safe numeric versions and alpha/beta labels verbatim in final archive and SKUDef filenames.
                string version = gui.ContainsKey("modversion") ? (string)gui["modversion"] : "1.00";
                ValidateModVersion(version);
                // Reborn: only the map stage consumes the checked folders, and All supplies no directory override.
                bool buildMaps = gui.ContainsKey("buildmaps") && Convert.ToBoolean(gui["buildmaps"]);
                string[] officialMapDirectories = buildMaps ? (string[])Lifecycle("GetSelectedMapDirectories") : null;
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
                // Reborn: pass explicit values for both persisted build-stage toggles.
                request.Arguments += " -BuildStaticData " + (buildStatic ? "1" : "0");
                request.Arguments += " -CreateBigFiles " + (createBig ? "1" : "0");
                // Reborn: Studio always uses fast builds; stale persisted developer checkbox values cannot re-enable maintenance regressions.
                request.Arguments += " -DeveloperBuildTests 0";
                // Reborn: an unchecked map toggle leaves map sources untouched; selected maps compile before final Misc packaging.
                request.Arguments += " -BuildMaps " + (buildMaps ? "1" : "0");
                // Reborn: force a fresh map compile while still updating its last successful generation.
                bool forceRebuildMaps = gui.ContainsKey("forcerebuildmaps") && Convert.ToBoolean(gui["forcerebuildmaps"]);
                request.Arguments += " -ForceRebuildMaps " + (forceRebuildMaps ? "1" : "0");
                // Reborn: Silent Mode skips advisory checks while keeping build failures visible.
                bool silentMode = gui.ContainsKey("silentmode") && Convert.ToBoolean(gui["silentmode"]);
                request.Arguments += " -SilentMode " + (silentMode ? "1" : "0");
                request.RedirectOutput = true; request.CreateWindow = false;
                // Reborn: retain the SDK's step-counter protocol before RunStep advances it for the callback.
                // Reborn: the session owns and cleans its map-list file; only its short path travels through the worker command line.
                if (officialMapDirectories != null)
                {
                    string selectionFile = (string)Lifecycle("SaveMapSelection", SdkRoot, officialMapDirectories);
                    request.Arguments += " -OfficialMapSelectionFile \"" + selectionFile + "\"";
                }
                if (BuildHelper.GetNextExecutableStep() != 1)
                    throw new InvalidDataException("Unexpected alpha build step state.");
                BuildHelper.RunStep(StepType.RunExecutable, request);
            }
            catch
            {
                if (lifecycle != null) { Lifecycle("BuildFailed"); Lifecycle("Complete"); }
                throw;
            }
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: complete the single backend operation without advancing to unvalidated RA3 postprocessors. */
        //-------------------------------------------------------------------------------------------------
        public void onStepSuccess(int stepId) { Lifecycle("BuildSucceeded"); Lifecycle("Complete"); BuildHelper.OnBuildComplete(); }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: make an unsuccessful backend visible and retain diagnostic output without automatic retry or cleanup. */
        //-------------------------------------------------------------------------------------------------
        public void onStepFailure(int stepId) { Lifecycle("BuildFailed"); Lifecycle("Complete"); BuildHelper.DisplayLine("Alpha build failed; no install or launch was attempted. Review the build output."); }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: keep Windows Forms types in the separately compiled helper, preserving EA's exact two-reference CodeDOM contract. */
        //-------------------------------------------------------------------------------------------------
        private object Lifecycle(string method, params object[] arguments) { return lifecycle.GetMethod(method).Invoke(null, arguments); }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: one supervised worker executes the independently selected build and package stages. */
        //-------------------------------------------------------------------------------------------------
        public bool shouldExecuteStep(int stepId) { return stepId == 1; }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: retain EA's options pane while explicitly describing this limited backend. */
        //-------------------------------------------------------------------------------------------------
        public List<GUIElementData> GetModBuildGUIData()
        {
            List<GUIElementData> options = new List<GUIElementData>();
            options.Add(new GUIElementData("alpha", "Uprising Alpha: Validate XML / Build Data / Make BIG", GUIElementData.LabelType, ScriptIndex.ModScript));
            options.Add(new GUIElementData("scope", "Unchecked stages reuse their last successful output", GUIElementData.LabelType, ScriptIndex.ModScript));
            // Reborn: distinguish reviewed variable behavior shapes from unrestricted nested GameObject/resource authoring.
            options.Add(new GUIElementData("weapon", "Create BIG Files can package saved outputs without recompiling", GUIElementData.LabelType, ScriptIndex.ModScript));
            options.Add(new GUIElementData("buildaptui", "Build AptUI", GUIElementData.CheckBoxType, ScriptIndex.ModScript));
			// Reborn: compile AdditionalMaps/MapMetaData_*.xml before static data, using EA's familiar persisted toggle.
            options.Add(new GUIElementData("buildglobal", "Build Global Data", GUIElementData.CheckBoxType, ScriptIndex.ModScript));
            // Reborn: use EA's persisted checkbox control for independent static compilation and final publication.
            options.Add(new GUIElementData("buildstatic", "Build Static Data", GUIElementData.CheckBoxType, ScriptIndex.ModScript));
            // Reborn: preserve EA's persisted checkbox behavior and the historical map stage before BIG creation.
            options.Add(new GUIElementData("buildmaps", "Build Maps", GUIElementData.CheckBoxType, ScriptIndex.ModScript));
            // Reborn: reserve a row for the helper's multi-select map checklist without changing EA's persisted checkbox format.
            options.Add(new GUIElementData("officialmap", "Official maps to build", GUIElementData.LabelType, ScriptIndex.ModScript));
            // Reborn: normal map builds reuse verified output; this toggle bypasses the lookup for the selected maps.
            options.Add(new GUIElementData("forcerebuildmaps", "Force Rebuild Maps", GUIElementData.CheckBoxType, ScriptIndex.ModScript));
            options.Add(new GUIElementData("createbig", "Create BIG Files", GUIElementData.CheckBoxType, ScriptIndex.ModScript));
            // Reborn: use the same persisted option control as the other build toggles.
            options.Add(new GUIElementData("silentmode", "Silent Mode", GUIElementData.CheckBoxType, ScriptIndex.ModScript));
            // Reborn: use EA's own persisted text-input control rather than a separate settings interface.
            options.Add(new GUIElementData("mv", "Mod Version:", GUIElementData.LabelType, ScriptIndex.ModScript));
            options.Add(new GUIElementData("modversion", "1.00", GUIElementData.InputBoxType, ScriptIndex.ModScript));
            return options;
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: allow release labels while excluding whitespace, path separators, argument metacharacters and trailing dots from package names. */
        //-------------------------------------------------------------------------------------------------
        private static void ValidateModVersion(string version)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(version ?? "", @"\A[A-Za-z0-9](?:[A-Za-z0-9._-]{0,46}[A-Za-z0-9])?\z"))
                throw new InvalidDataException("Mod Version must be 1-48 characters using letters, digits, dots, underscores or hyphens, and start/end with a letter or digit. Examples: 1.00, 1.0-beta.");
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: reject command-line quoting ambiguity and missing explicit local paths before requesting an external process. */
        //-------------------------------------------------------------------------------------------------
        private static void CheckPath(string path, string purpose)
        {
            if (String.IsNullOrEmpty(path) || !Path.IsPathRooted(path) || path.IndexOf('"') >= 0 || path.IndexOf('\r') >= 0 || path.IndexOf('\n') >= 0 || !File.Exists(path))
                throw new InvalidDataException("Required " + purpose + " file was not found at the resolved absolute path: " + (path ?? "<null>"));
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
                // Reborn: portable paths are relative to the actual EALAModStudio directory, never the caller's working directory.
                string value = line.Substring(separator + 1);
                settings.Add(key, Path.GetFullPath(Path.IsPathRooted(value) ? value : Path.Combine(SdkRoot, value)));
            }
            if (!settings.ContainsKey("PowerShell7") || !settings.ContainsKey("Driver") || !settings.ContainsKey("StaticManifest") || !settings.ContainsKey("CompilerDirectory"))
                throw new InvalidDataException("Alpha requires all four base path settings; GlobalManifest enables weapon builds.");
            return settings;
        }
    }
}
