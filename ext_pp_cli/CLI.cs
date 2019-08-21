using ADL;
using ADL.Configs;
using ADL.Crash;
using ADL.Streams;
using ext_pp;
using ext_pp_base;
using ext_pp_base.settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MatchType = ADL.MatchType;
using Utils = ext_pp_base.Utils;

namespace ext_pp_cli
{

    /// <summary>
    /// Command Line Interface of the EXT_PP project.
    /// </summary>
    public class CLI : ILoggable
    {


        public delegate bool CommandHandler();

        /// <summary>
        /// Default helptext to show.
        /// </summary>
        public static string HelpText { get; } = "To list all available commands type: ext_pp_cli --help or -h";

        /// <summary>
        /// Simple header to assert dominance
        /// </summary>
        public static string CliHeader { get; } = "\n\next_pp_cli version: " +
                                           Assembly.GetAssembly(typeof(CLI)).GetName().Version +
                                           "\nCopyright by Tim Akermann\nGithub: https://github.com/ByteChkR/ext-pp\n\n";

        /// <summary>
        /// A version string.
        /// </summary>
        public static string Version { get; } = "\next_pp_cli: " + Assembly.GetAssembly(typeof(CLI)).GetName().Version +
                                          "\next_pp_base: " + Assembly.GetAssembly(typeof(Utils)).GetName().Version +
                                          "\next_pp: " + Assembly.GetAssembly(typeof(Definitions)).GetName().Version;


        /// <summary>
        /// Instance of the plugin manager.
        /// </summary>
        private readonly PluginManager _pluginManager;


        /// <summary>
        /// Shuts down the cli by removing all output streams from adl.
        /// </summary>


        #region Commands

        private List<CommandHandler> CommandApplyOrder => new List<CommandHandler>
        {
            VerbosityLevelCommandHandler,
            ReadmeCommandHandler,
            ShowVersionCommandHandler,
            ListAllPluginsCommandHandler,
            AddPluginsCommandHandler,
            ListPluginDirsCommandHandler,
            ListPluginIncsCommandHandler,
            ListPluginManIncsCommandHandler,
            RefreshPluginsCommandHandler,
            Help_Command,
            HelpAll_Command,
            LogToFileCommandHandler,
            DefineParamsCommandHandler,
            ChainParamsCommandHanlder,
            InOutCommandHandler
        };


        #region CommandInfo

        /// <summary>
        /// List of Command infos that are directly used by the CLI
        /// They have no prefix
        /// </summary>
        private static List<CommandInfo> Info => new List<CommandInfo>
        {
            new CommandInfo("input", "i", PropertyHelper<CLI>.GetPropertyInfo(x=>x.Input),
                "--input filepath,filepath filepath,filepath\r\n\t, separated = the same compilation\r\n\t[space] separated means gets queued after the compilation of the first one"),
            new CommandInfo("output", "o", PropertyHelper<CLI>.GetPropertyInfo(x=>x.Output),
                "--output filepath filepath filepath\r\n\t[space] separated list of output files."),
            new CommandInfo("defines", "d", PropertyHelper<CLI>.GetPropertyInfo(x=>x.DefinesParams),
                "--defines <vars>\r\n\t, [space] separated list of predefines values"),
            new CommandInfo("chain", "c", PropertyHelper<CLI>.GetPropertyInfo(x=>x.ChainParams),
                "--chain [filepath]\r\n\t, separated list of plugins\r\n\t\t[filepath]:pluginname => loads a plugin by assembly name\r\n\t\t[filepath]:prefix => loads a plugin with prefix\r\n\t\t[filepath]:(collection) => loads a list(chain) of plugins from an IChainCollection with the specified name\r\n\t\tthe plugins in the /plugin folder can be directly accessed by using the prefix instead of the lines above"),
            new CommandInfo("log-to-file", "l2f",PropertyHelper<CLI>.GetPropertyInfo(x=>x.LogToFileParams),
                "--log-to-file <file> <settings>\r\n\tCreates a log file with the settings\r\n\t\t<mask>:<timestamp>\r\n\t\tdefault: all:true"),
            new CommandInfo("write-to-console", "w2c", PropertyHelper<CLI>.GetPropertyInfo(x=>x.OutputToConsole),
                "--write2console [bool]\r\n\tWrites the result into the cout stream\r\n\tSets the verbosity to silent if not specified otherwise"),
            new CommandInfo("verbosity", "v", PropertyHelper<CLI>.GetPropertyInfo(x=>x.DebugLvl),
                "--verbosity <int>\r\n\tSets the debug output granularity"),
            new CommandInfo("version", "vv", PropertyHelper<CLI>.GetPropertyInfo(x=>x.ShowVersion),
            "--version\r\n\tdisplays the current version"),
            new CommandInfo("no-chain-collection", "nc", PropertyHelper<CLI>.GetPropertyInfo(x=>x.NoCollections),
                "The CLI will not search for a ChainCollection in the specified assembly"),
            new CommandInfo("help", "h", PropertyHelper<CLI>.GetPropertyInfo(x=>x.HelpParams),
                "	\t--help <chainstr>\r\n\t\tlists the commands of the CLI or with supplied chain, it will display the help info of each plugin."),
            new CommandInfo("help-all", "hh", PropertyHelper<CLI>.GetPropertyInfo(x=>x.HelpAllParams),
                "	\t--help-all <chainstr>\r\n\t\tlists the commands of the CLI or with supplied chain, it will display the help info of each plugin."),
            new CommandInfo("pm-refresh", "pm-r", PropertyHelper<CLI>.GetPropertyInfo(x=>x.PluginRefresh),
                "--pm-refresh\r\n\t\tRefreshes the Plugin Manager."),
            new CommandInfo("pm-add", "pm-a", PropertyHelper<CLI>.GetPropertyInfo(x=>x.PluginAdd),
                "--pm-add <folder>\r\n\t\tAdds a folder with plugins to the Plugin Manager. All pluins in that folder can be referenced by their prefixes when specifies in --chain or --help"),
            new CommandInfo("pm-list-dir", "pm-ld", PropertyHelper<CLI>.GetPropertyInfo(x=>x.PluginListDirs),
                "--pm-list-dir\r\n\t\tLists all Included dictionaries in Plugin Manager"),
            new CommandInfo("pm-list-file", "pm-lf", PropertyHelper<CLI>.GetPropertyInfo(x=>x.PluginListIncs),
                "--pm-list-file\r\n\t\tLists all Included and Cached Files in Plugin Manager" ),
            new CommandInfo("pm-list-manual-files", "pm-lmf", PropertyHelper<CLI>.GetPropertyInfo(x=>x.PluginListManIncs),
                "--pm-list-manual-files\r\n\t\tLists all Manually Included and Cached Files in Plugin Manager" ),
            new CommandInfo("pm-list-all", "pm-la", PropertyHelper<CLI>.GetPropertyInfo(x=>x.PluginListAllCommandHandler),
                "--pm-list-all\r\n\t\tLists all Cached data."),
        };

        #endregion

        #region CommandFields

        /// <summary>
        /// Contains the Parameters for the -l2f and --logToFile commands.
        /// </summary>
        public string[] LogToFileParams { get; set; }

        /// <summary>
        /// A flag that is used to determine if the log2file flag was set.
        /// </summary>
        private bool LogToFile => LogToFileParams != null && LogToFileParams.Length != 0;

        /// <summary>
        /// Flag to output the result to the console.
        /// </summary>
        public bool OutputToConsole { get; set; }

        /// <summary>
        /// The input files
        /// </summary>
        public string[] Input { get; set; } = new string[0];

        /// <summary>
        /// The input files
        /// </summary>
        public string[] ReadmeArgs { get; set; } = new string[0];

        /// <summary>
        /// The output files.
        /// </summary>
        public string[] Output { get; set; } = new string[0];

        /// <summary>
        /// Predefined definitions from --defines and -defs
        /// </summary>
        public string[] DefinesParams { get; set; }

        /// <summary>
        /// Forces the cli to ignore collections.
        /// </summary>
        public bool NoCollections { get; set; }

        /// <summary>
        /// Contains the parameters for the plugin chain
        /// </summary>
        public string[] ChainParams { get; set; }

        /// <summary>
        /// Contains the parameters for the help parameter
        /// </summary>
        public string[] HelpParams { get; set; }

        /// <summary>
        /// Contains the parameters for the help all parameter
        /// </summary>
        public string[] HelpAllParams { get; set; }

        /// <summary>
        /// Debug level of the process.
        /// </summary>
        public Verbosity DebugLvl { get; set; } = Verbosity.LEVEL1;

        /// <summary>
        /// Show the version at the start and exit.
        /// </summary>
        public bool ShowVersion { get; set; }

        /// <summary>
        /// parameter for the --pm-add and -pm-a commands.
        /// </summary>
        public string[] PluginAdd { get; set; }

        /// <summary>
        /// Flag if the settings contain the -pm-r/--pm-refresh command
        /// </summary>
        public bool PluginRefresh { get; set; }

        /// <summary>
        /// Flag if the settings contain the -pm-ld/--pm-list-dirr command
        /// </summary>
        public bool PluginListDirs { get; set; }

        /// <summary>
        /// Flag if the settings contain the -pm-lf/--pm-list-file command
        /// </summary>
        public bool PluginListIncs { get; set; }

        /// <summary>
        /// Flag if the settings contain the -pm-lmf/--pm-list-manual-files command
        /// </summary>
        public bool PluginListManIncs { get; set; }

        /// <summary>
        /// Flag if the settings contain the -pm-a/--pm-all command
        /// </summary>
        public bool PluginListAllCommandHandler { get; set; }

        /// <summary>
        /// Definitions.
        /// </summary>
        private Definitions _defs;

        /// <summary>
        /// the chain used for it.
        /// </summary>
        private List<AbstractPlugin> _chain;

        #endregion

        #region Help

        private bool Help_Command()
        {
            return Help_Command_Unfied(HelpParams, false);
        }

        private bool HelpAll_Command()
        {
            return Help_Command_Unfied(HelpAllParams, true);
        }

        private bool Help_Command_Unfied(string[] helpParams, bool extendedDescription)
        {
            if (helpParams == null)
            {
                return false;
            }

            if (helpParams.Length == 0)
            {
                this.Log(DebugLevel.LOGS, Verbosity.SILENT, "\n{0}", Info.ListAllCommands(new[] { "" }).Unpack("\n"));
                return true;
            }

            foreach (var file in helpParams)
            {
                if (file != "self")
                {

                    ParseChainSyntax(file, out string path, out string[] names);
                    if (_pluginManager.DisplayHelp(path, names, !extendedDescription))
                    {
                        continue;
                    }

                    List<AbstractPlugin> plugins = CreatePluginChain(new[] { file }, true).ToList();
                    this.Log(DebugLevel.LOGS, Verbosity.SILENT, "Listing Plugins: ");
                    foreach (var plugin in plugins)
                    {
                        this.Log(DebugLevel.LOGS, Verbosity.SILENT, "\n{0}", plugin.ListInfo(true).Unpack("\n"));
                    }
                }
                else
                {
                    this.Log(DebugLevel.LOGS, Verbosity.SILENT, "\n{0}", Info.ListAllCommands(new[] { "" }).Unpack("\n"));
                }


            }

            return true;
        }

        #endregion

        #region Input/Output

        private bool InOutCommandHandler()
        {
            //No Other code since the input and output arrays are populated with reflection
            //Error Checking
            if ((Output.Length == 0 && !OutputToConsole) || Input.Length == 0)
            {
                this.Log(DebugLevel.ERRORS, Verbosity.SILENT, "Not enough arguments specified. Aborting..");
                this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, HelpText);
                return true;
            }

            if (Input.Length > Output.Length)
            {
                this.Log(DebugLevel.ERRORS, Verbosity.SILENT, "Not enough outputs specified. Aborting..");
                return true;
            }

            return false;
        }

        #endregion

        #region Readme

        private bool ReadmeCommandHandler()
        {
            if (ReadmeArgs.Length == 2)
            {
                this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, "Generating Readme for file: {0}", ReadmeArgs[0]);
                PluginManager pm = new PluginManager();
                List<string> ht = GenerateReadme(pm.FromFile(ReadmeArgs[0]));

                this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, "Writing Readme to file: {0}", ReadmeArgs[1]);
                File.WriteAllLines(ReadmeArgs[1], ht.ToArray());
                return true;
            }

            return false;
        }

        private List<string> GenerateReadme(List<AbstractPlugin> plugins)
        {
            List<string> ret = new List<string>();

            foreach (var abstractPlugin in plugins)
            {
                this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, "Generating Readme for plugin: {0}", abstractPlugin.GetType().Name);
                ret.AddRange(abstractPlugin.ToMarkdown());
            }

            return ret;
        }

        #endregion

        #region ShowVersion

        private bool ShowVersionCommandHandler()
        {
            if (ShowVersion)
            {
                this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, Version);
                return true;
            }

            return false;
        }

        #endregion

        #region PluginManager

        #region PluginList

        private bool ListAllPluginsCommandHandler()
        {
            if (PluginListAllCommandHandler)
            {
                _pluginManager.ListAllCachedData();
                return true;
            }

            return false;
        }

        private bool ListPluginDirsCommandHandler()
        {
            if (PluginListDirs)
            {
                _pluginManager.ListCachedFolders();
                if (!PluginRefresh && !PluginListIncs && !PluginListManIncs)
                {
                    return true;
                }
            }

            return false;
        }

        private bool ListPluginIncsCommandHandler()
        {
            if (PluginListIncs)
            {
                _pluginManager.ListCachedPlugins(false);
                if (!PluginRefresh && !PluginListManIncs)
                {
                    return true;
                }
            }

            return false;
        }

        private bool ListPluginManIncsCommandHandler()
        {
            if (PluginListManIncs)
            {
                _pluginManager.ListManuallyCachedFiles();
                if (!PluginRefresh)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region PluginAddRemove

        private bool RefreshPluginsCommandHandler()
        {
            if (PluginRefresh)
            {
                _pluginManager.Refresh();
                return true;
            }

            return false;
        }

        private bool AddPluginsCommandHandler()
        {
            if (PluginAdd != null && PluginAdd.Length != 0)
            {
                foreach (var s in PluginAdd)
                {

                    FileAttributes attr = File.GetAttributes(s);
                    if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        _pluginManager.AddFolder(s);
                    }
                    else
                    {
                        _pluginManager.AddFile(s);
                    }
                }

                if (!PluginRefresh && !PluginListDirs && !PluginListIncs && !PluginListManIncs)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #endregion

        #region LogToFile

        private bool LogToFileCommandHandler()
        {
            if (LogToFile)
            {
                string[] arguments = LogToFileParams.Length > 1 ? LogToFileParams.SubArray(1, LogToFileParams.Length - 1).ToArray() : new string[0];
                KeyValuePair<int, bool> ts = ParseLogParams(arguments.Length != 0 ? arguments[0] : "");

                AddLogOutput(LogToFileParams[0], ts.Key, ts.Value);

            }
            return false;
        }

        /// <summary>
        /// Adds a log output to the ADL system that is writing to a file.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="mask"></param>
        /// <param name="timestamp"></param>
        private static void AddLogOutput(string file, int mask, bool timestamp)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
            LogTextStream lll = new LogTextStream(File.OpenWrite(file), mask, MatchType.MatchAll, timestamp);
            Debug.AddOutputStream(lll);
        }

        /// <summary>
        /// Implements the log params syntax
        /// int:bool
        /// default: -1:true
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static KeyValuePair<int, bool> ParseLogParams(string input)
        {
            int mask = -1;
            bool timestamp = true;
            string[] vars = input.Split(":");
            if (vars.Length > 0)
            {

                if (vars[0] != "all")
                {
                    mask = (int)Utils.Parse(typeof(DebugLevel), vars[0], -1);
                }
                if (vars.Length > 1)
                {
                    bool.TryParse(vars[1], out timestamp);
                }
            }
            return new KeyValuePair<int, bool>(mask, timestamp);

        }

        #endregion

        #region Defines

        private bool DefineParamsCommandHandler()
        {
            if (DefinesParams == null)
            {
                _defs = new Definitions();
            }
            else
            {
                _defs = new Definitions(DefinesParams.Select(x => new KeyValuePair<string, bool>(x, true)).
                    ToDictionary(x => x.Key, x => x.Value));
            }

            return false;
        }

        #endregion

        #region Chain

        private bool ChainParamsCommandHanlder()
        {
            if (ChainParams != null)
            {
                _chain = CreatePluginChain(ChainParams, NoCollections).ToList();
                this.Log(DebugLevel.LOGS, Verbosity.LEVEL2, "{0} Plugins Loaded..", _chain.Count);
            }
            else
            {
                this.Log(DebugLevel.ERRORS, Verbosity.LEVEL1, "Not plugin chain specified. 0 Plugins Loaded..");
                _chain = new List<AbstractPlugin>();
            }

            return false;
        }

        /// <summary>
        /// Implements the Chain Syntax
        /// path:plugin:plugin:plugin
        /// path:collection:plugin
        /// prefix:prefix:prefix
        /// prefix prefix prefix
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="path"></param>
        /// <param name="names"></param>
        private void ParseChainSyntax(string arg, out string path, out string[] names)
        {
            if (arg.Contains(':'))
            {
                var tmp = arg.Split(':').ToList();
                names = tmp.SubArray(1, tmp.Count - 1).ToArray();
                path = tmp[0];
            }
            else
            {
                path = arg; //Set the path
                names = new[] { path };
                if (!_pluginManager.TryGetPathByPrefix(arg, out path) && !_pluginManager.TryGetPathByName(arg, out path))
                {
                    names = null; //Will change path if it matches prefix
                }
            }

        }

        private List<AbstractPlugin> CreateChainCollection(Assembly asm, string name)
        {
            if (TryCreateChainCollection(asm, name, out IChainCollection collection))
            {
                List<AbstractPlugin> r = collection.Chain
                    .Select(x => (AbstractPlugin)Activator.CreateInstance(x)).ToList();
                this.Log(DebugLevel.LOGS, Verbosity.LEVEL2, "Creating Chain Collection with Plugins: {0}", r.Select(x => x.GetType().Name).Unpack(", "));
                return r;
            }
            else
            {
                this.Log(DebugLevel.LOGS, Verbosity.LEVEL2, "Could not find a Chain collection in the specified file.");

            }

            return new List<AbstractPlugin>();
        }

        private static bool TryCreateChainCollection(Assembly asm, string name, out IChainCollection collection)
        {
            Type[] types = asm.GetTypes();
            if (name == "*")
            {
                Type tt = types.FirstOrDefault(x => x.GetInterfaces().Contains(typeof(IChainCollection)));
                if (tt != null)
                {
                    collection = (IChainCollection)Activator.CreateInstance(tt);
                }
                collection = null;
            }
            else
            {
                collection = types.Where(x => x.GetInterfaces().Contains(typeof(IChainCollection)))
                    .Select(x => (IChainCollection)Activator.CreateInstance(x)).FirstOrDefault(x => x.Name == name);
            }

            return collection != null;

        }

        private List<AbstractPlugin> ProcessChainCollection(string[] names, Assembly asm)
        {
            List<AbstractPlugin> ret = new List<AbstractPlugin>();

            if (names == null)
            {
                ret.AddRange(CreateChainCollection(asm, "*"));
            }
            else
            {
                names[0] = names[0].Trim('(', ')');
                this.Log(DebugLevel.LOGS, Verbosity.LEVEL2, "Searching Chain Collection: {0}", names[0]);

                ret.AddRange(CreateChainCollection(asm, names[0]));
            }

            return ret;
        }

        private List<AbstractPlugin> ProcessPluginChain(string[] names, string path)
        {
            List<AbstractPlugin> ret = new List<AbstractPlugin>();
            this.Log(DebugLevel.LOGS, Verbosity.LEVEL4, "Loading {0} in file {1}", names == null ? "all plugins" : names.Unpack(", "), path);

            if (names == null)
            {

                ret.AddRange(_pluginManager.FromFile(path));
            }
            else
            {
                List<AbstractPlugin> plugins = _pluginManager.FromFile(path);
                for (int i = 0; i < names.Length; i++)
                {
                    for (int j = 0; j < plugins.Count; j++)
                    {
                        if (plugins[j].Prefix.Contains(names[i]))
                        {
                            this.Log(DebugLevel.LOGS, Verbosity.LEVEL5, "Creating instance of: {0}", plugins[j].GetType().Name);
                            ret.Add(plugins[j]);
                        }
                    }
                }
            }

            return ret;
        }

        private List<AbstractPlugin> GetPluginsFromPath(string path, string[] names, bool noCollection)
        {
            List<AbstractPlugin> ret = new List<AbstractPlugin>();

            if (File.Exists(path))
            {

                bool isCollection = names != null && names.Length == 1 && names[0].StartsWith('(') &&
                                    names[0].EndsWith(')');
                if ((names == null && !noCollection) || isCollection)
                {
                    Assembly asm = Assembly.LoadFile(Path.GetFullPath(path));
                    ret.AddRange(ProcessChainCollection(names, asm));
                }
                else
                {
                    ret.AddRange(ProcessPluginChain(names, path));
                }
            }
            else
            {

                this.Log(DebugLevel.ERRORS, Verbosity.LEVEL1, "Could not load file: {0}", path);
            }

            return ret;

        }

        /// <summary>
        /// Creates the Plugin chain based on a argument in ChainSyntax.
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="noCollection"></param>
        /// <returns></returns>
        private IEnumerable<AbstractPlugin> CreatePluginChain(string[] arg, bool noCollection)
        {

            this.Log(DebugLevel.LOGS, Verbosity.LEVEL3, "Creating Plugin Chain...");
            List<AbstractPlugin> ret = new List<AbstractPlugin>();

            string[] names = null;
            string path = "";
            foreach (var plugin in arg)
            {
                ParseChainSyntax(plugin, out path, out names);

                ret.AddRange(GetPluginsFromPath(path, names, noCollection));
            }

            return ret;
        }

        #endregion

        #region Verbosity

        private bool VerbosityLevelCommandHandler()
        {
            Logger.VerbosityLevel = DebugLvl;
            this.Log(DebugLevel.LOGS, Verbosity.LEVEL1, "Verbosity Level set to: {0}", Logger.VerbosityLevel);
            return false;
        }


        #endregion

        #endregion

        #region CLIArgumentProcessing

        private static List<string> ComputeFileReferences(List<string> args)
        {
            List<string> ret = args;
            for (int i = 0; i < args.Count; i++)
            {
                if (args[i].StartsWith('@'))
                {
                    string path = args[i].TrimStart('@');
                    args.RemoveAt(i);
                    if (File.Exists(path))
                    {
                        args.InsertRange(i, File.ReadAllLines(path).Unpack(" ").Pack(" "));
                    }
                    else
                    {
                        Logger.Crash(new FileNotFoundException("Can not find: " + path));
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Turns the args into a dictionary that contains keys and n string values.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Dictionary<string, string[]> AnalyzeArgs(string[] args)
        {
            Dictionary<string, string[]> ret = new Dictionary<string, string[]>();
            if (args.Length == 0)
            {
                return ret;
            }
            int cmdIdx = FindNextCommand(args, -1);
            if (cmdIdx == args.Length)
            {
                return ret;
            }
            do
            {
                int tmpidx = FindNextCommand(args, cmdIdx);
                ret.Add(args[cmdIdx], args.SubArray(cmdIdx + 1, tmpidx - cmdIdx - 1).ToArray());

            } while ((cmdIdx = FindNextCommand(args, cmdIdx)) != args.Length);

            return ret;
        }

        /// <summary>
        /// Returns the index of the next occurrence of "-..."
        /// </summary>
        /// <param name="args"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        private static int FindNextCommand(string[] args, int start)
        {
            for (int i = start + 1; i < args.Length; i++)
            {
                if (args[i].StartsWith('-'))
                {
                    return i;
                }
            }

            return args.Length;
        }

        #endregion


        private void Process(PreProcessor pp, Settings settings)
        {
            //Compile/Execute PreProcessor
            for (var index = 0; index < Input.Length; index++)
            {
                var input = Input[index];
                string[] src = pp.Compile(input.Split(','), settings, _defs);

                if (OutputToConsole)
                {
                    if (Output != null && Output.Length > index)
                    {
                        string outp = Path.GetFullPath(Output[index]);
                        string sr = src.Unpack("\n");
                        File.WriteAllText(outp, sr);
                    }
                }
                else
                {
                    string outp = Path.GetFullPath(Output[index]);
                    string sr = src.Unpack("\n");
                    File.WriteAllText(outp, sr);
                }
            }
        }




        /// <summary>
        /// Applies/Brings the configuration of the CLI up and running so it can start the PreProcessing.
        /// </summary>
        /// <param name="settings"></param>
        public bool Apply()
        {

            List<CommandHandler> loadOrder = CommandApplyOrder;

            foreach (var commandHandler in loadOrder)
            {
                if (commandHandler()) //Command wants us to exit the program(work was finished)
                {
                    return false;
                }
            }

            return true;
        }




        /// <summary>
        /// Function that sets up ADL to operate with the DebugLevel enum and more.
        /// </summary>
        private static void InitAdl()
        {

            CrashHandler.Initialize((int)DebugLevel.INTERNAL_ERROR, false);
            Debug.LoadConfig((AdlConfig)new AdlConfig().GetStandard());
            Debug.CheckForUpdates = false;
            Debug.SetAllPrefixes("[ERRORS]", "[WARNINGS]", "[LOGS]", "[INTERNAL_ERROR]", "[PROGRESS]");
            Debug.AddPrefixForMask(-1, "[ALL]");
            Debug.AdlWarningMask = (int)DebugLevel.WARNINGS;
            LogTextStream lts = new LogTextStream(
                Console.OpenStandardOutput(),
                -1,
                MatchType.MatchAll,
                false);

            Debug.AddOutputStream(lts);

        }






        /// <summary>
        /// Constructor that does the parameter analysis.
        /// </summary>
        /// <param name="args"></param>
        public CLI(string[] args)
        {
            InitAdl();

            List<string> arf = ComputeFileReferences(args.ToList());

            string[] arguments = arf.ToArray();

            Settings settings = new Settings(AnalyzeArgs(arguments));

            settings.ApplySettings(Info, this);

            _pluginManager = new PluginManager();



            if (Apply())
            {
                PreProcessor pp = new PreProcessor();
                pp.SetFileProcessingChain(_chain);
                Process(pp, settings);
            }

            this.Log(DebugLevel.LOGS, Verbosity.LEVEL2, "Finished Task(s) in {0}ms", Timer.MS);

        }

        /// <summary>
        /// Main entry point.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            float start = Timer.MS; // Load assembly
            Console.WriteLine(CliHeader, start);
#if DEBUG

            if (args.Length == 0)
            {
                Console.WriteLine(HelpText);
            }
            CLI c;
            string[] arf;
            bool exit=false;
            bool isFirst=true;
            do
            {
                arf = isFirst? args : Console.ReadLine().Pack(" ").ToArray();
                isFirst=false;
                if(arf.Contains("exit"))exit=true;
                c = new CLI(arf);
                Debug.RemoveAllOutputStreams();
                c = null;
            } while (!exit);

#elif RELEASE
            if (args.Length == 0)
            {
                Console.WriteLine(HelpText);
            }
            else
                new CLI(args);
#endif
            //Yeet. Codacy thinks my Entry method is empty.
        }

    }
}