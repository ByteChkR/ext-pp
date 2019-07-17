﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ADL;
using ADL.Configs;
using ADL.Crash;
using ADL.Streams;
using ext_pp;
using ext_pp_base;
using ext_pp_base.settings;
using MatchType = ADL.MatchType;
using Utils = ext_pp_base.Utils;

namespace ext_pp_cli
{
    public class CLI : ILoggable
    {
        private static string HelpText = "To list all available commands type: ext_pp_cli --help or -h";

        private static string CLIHeader = "\n\next_pp_cli version: " + Assembly.GetAssembly(typeof(CLI)).GetName().Version + "\nCopyright by Tim Akermann\nGithub: https://github.com/ByteChkR/ext-pp\n\n";

        private static string Version = "\next_pp_cli: " + Assembly.GetAssembly(typeof(CLI)).GetName().Version +
                                        "\next_pp_base: " + Assembly.GetAssembly(typeof(Utils)).GetName().Version +
                                        "\next_pp: " + Assembly.GetAssembly(typeof(Definitions)).GetName().Version;



        private List<CommandInfo> Info => new List<CommandInfo>()
        {
            new CommandInfo("input", "i", PropertyHelper<CLI>.GetFieldInfo(x=>x.Input),
                "--input filepath,filepath filepath,filepath\r\n\t, separated = the same compilation\r\n\t[space] separated means gets queued after the compilation of the first one"),
            new CommandInfo("output", "o", PropertyHelper<CLI>.GetFieldInfo(x=>x.Output),
                "--output filepath filepath filepath\r\n\t[space] separated list of output files."),
            new CommandInfo("defines", "d", PropertyHelper<CLI>.GetFieldInfo(x=>x.DefinesParams),
                "--defines <vars>\r\n\t, [space] separated list of predefines values"),
            new CommandInfo("chain", "c", PropertyHelper<CLI>.GetFieldInfo(x=>x.ChainParams),
                "--chain [filepath]\r\n\t, separated list of plugins\r\n\t\t[filepath]:pluginname => loads a plugin by assembly name\r\n\t\t[filepath]:prefix => loads a plugin with prefix\r\n\t\t[filepath]:(collection) => loads a list(chain) of plugins from an IChainCollection with the specified name\r\n\t\tthe plugins in the /plugin folder can be directly accessed by using the prefix instead of the lines above"),
            new CommandInfo("log-to-file", "l2f",PropertyHelper<CLI>.GetFieldInfo(x=>x.LogToFileParams),
                "--log-to-file <file> <settings>\r\n\tCreates a log file with the settings\r\n\t\t<mask>:<timestamp>\r\n\t\tdefault: all:true"),
            new CommandInfo("write-to-console", "w2c", PropertyHelper<CLI>.GetFieldInfo(x=>x.OutputToConsole),
                "--write2console [bool]\r\n\tWrites the result into the cout stream\r\n\tSets the verbosity to silent if not specified otherwise"),
            new CommandInfo("verbosity", "v", PropertyHelper<CLI>.GetFieldInfo(x=>x.DebugLvl),
                "--verbosity <int>\r\n\tSets the debug output granularity"),
            new CommandInfo("version", "vv", PropertyHelper<CLI>.GetFieldInfo(x=>x.ShowVersion),
            "--version\r\n\tdisplays the current version"),
            new CommandInfo("no-chain-collection", "nc", PropertyHelper<CLI>.GetFieldInfo(x=>x.NoCollections),
                "The CLI will not search for a ChainCollection in the specified assembly"),
            new CommandInfo("help", "h", PropertyHelper<CLI>.GetFieldInfo(x=>x.HelpParams),
                "	\t--help <chainstr>\r\n\t\tlists the commands of the CLI or with supplied chain, it will display the help info of each plugin."),
            new CommandInfo("pm-refresh", "pm-r", PropertyHelper<CLI>.GetFieldInfo(x=>x.PluginRefresh),
                "--pm-refresh\r\n\t\tRefreshes the Plugin Manager."),
            new CommandInfo("pm-add", "pm-a", PropertyHelper<CLI>.GetFieldInfo(x=>x.PluginAdd),
                "--pm-add <folder>\r\n\t\tAdds a folder with plugins to the Plugin Manager. All pluins in that folder can be referenced by their prefixes when specifies in --chain or --help"),
            new CommandInfo("pm-list-dir", "pm-ld", PropertyHelper<CLI>.GetFieldInfo(x=>x.PluginListDirs),
                "--pm-list-dir\r\n\t\tLists all Included dictionaries in Plugin Manager"),
            new CommandInfo("pm-list-file", "pm-lf", PropertyHelper<CLI>.GetFieldInfo(x=>x.PluginListIncs),
                "--pm-list-file\r\n\t\tLists all Included and Cached Files in Plugin Manager" ),
            new CommandInfo("pm-list-manual-files", "pm-lmf", PropertyHelper<CLI>.GetFieldInfo(x=>x.PluginListManIncs),
                "--pm-list-manual-files\r\n\t\tLists all Manually Included and Cached Files in Plugin Manager" ),
            new CommandInfo("pm-list-all", "pm-la", PropertyHelper<CLI>.GetFieldInfo(x=>x.PluginListAll),
                "--pm-list-all\r\n\t\tLists all Cached data."),
        };

        public string[] LogToFileParams = null;
        private bool LogToFile => LogToFileParams != null && LogToFileParams.Length != 0;
        private bool OutputToConsole = false;
        public string[] Input = new string[0];
        public string[] Output = new string[0];
        public string[] DefinesParams = null;
        public bool NoCollections = false;
        public string[] ChainParams = null;
        public string[] HelpParams = null;
        public Verbosity DebugLvl = Verbosity.LEVEL1;
        public bool ShowVersion = false;
        public string[] PluginAdd = null;
        public bool PluginRefresh = false;
        public bool PluginListDirs = false;
        public bool PluginListIncs = false;
        public bool PluginListManIncs = false;
        public bool PluginListAll = false;

        private Definitions _defs;
        private PluginManager _pluginManager;
        private readonly Settings _settings;
        private List<AbstractPlugin> _chain;



        public CLI(string[] args)
        {
            InitAdl();

            List<string> arf = args.ToList();
            for (int i = 0; i < arf.Count; i++)
            {
                if (arf[i].StartsWith('@'))
                {
                    string path = arf[i].TrimStart('@');
                    arf.RemoveAt(i);
                    if (File.Exists(path))
                    {
                        arf.InsertRange(i, File.ReadAllLines(path).Unpack(" ").Pack(" "));
                    }
                    else
                    {
                        Logger.Crash(new FileNotFoundException("Can not find: " + path));
                    }
                }
            }

            args = arf.ToArray();

            PreApply(_settings = new Settings(AnalyzeArgs(args)));







            if (ShowVersion)
            {
                this.Log(DebugLevel.LOGS, Version, Verbosity.LEVEL1);
                return;
            }


            this.Log(DebugLevel.LOGS, CLIHeader, Verbosity.LEVEL1);

            _pluginManager = new PluginManager();


            if (PluginListAll)
            {
                _pluginManager.ListAllCachedData();
                return;
            }


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

                if (!PluginRefresh && !PluginListDirs && !PluginListIncs && !PluginListManIncs) return;
            }

            if (PluginListDirs)
            {
                _pluginManager.ListCachedFolders();
                if (!PluginRefresh && !PluginListIncs && !PluginListManIncs) return;
            }

            if (PluginListIncs)
            {
                _pluginManager.ListCachedPlugins();
                if (!PluginRefresh && !PluginListManIncs) return;
            }

            if (PluginListManIncs)
            {
                _pluginManager.ListManualCachedPlugins();
                if (!PluginRefresh) return;
            }

            if (PluginRefresh)
            {
                _pluginManager.Refresh();
                return;
            }

            if (HelpParams != null)
            {
                if (HelpParams.Length == 0)
                {
                    this.Log(DebugLevel.LOGS, "\n" + Info.ListAllCommands(new[] { "" }).Unpack("\n"),
                        Verbosity.SILENT);
                    return;
                }
                foreach (var file in HelpParams)
                {
                    if (file != "self")
                    {
                        List<AbstractPlugin> plugins = CreatePluginChain(new[] { file }, true).ToList();
                        this.Log(DebugLevel.LOGS, "Listing Plugins: ", Verbosity.SILENT);
                        foreach (var plugin in plugins)
                        {
                            this.Log(DebugLevel.LOGS, "\n" + plugin.ListInfo(true).Unpack("\n"),
                                Verbosity.SILENT);
                        }
                    }
                    else
                    {
                        this.Log(DebugLevel.LOGS, "\n" + Info.ListAllCommands(new[] { "" }).Unpack("\n"),
                            Verbosity.SILENT);
                    }


                }

                return;
            }







            Apply(_settings);


            PreProcessor pp = new PreProcessor();

            pp.SetFileProcessingChain(_chain);



            if ((Output.Length == 0 && !OutputToConsole) || Input.Length == 0)
            {

                this.Log(DebugLevel.ERRORS, args.Unpack(", "), Verbosity.SILENT);
                this.Log(DebugLevel.ERRORS, "Not enough arguments specified. Aborting..", Verbosity.SILENT);
                this.Log(DebugLevel.LOGS, HelpText, Verbosity.LEVEL1);
                return;
            }




            if (Input.Length > Output.Length)
            {
                this.Log(DebugLevel.ERRORS, "Not enough outputs specified. Aborting..", Verbosity.SILENT);
                return;
            }

            for (var index = 0; index < Input.Length; index++)
            {
                var input = Input[index];
                string[] src = pp.Compile(input.Split(','), _settings, _defs);

                if (OutputToConsole)
                {
                    if (Output != null && Output.Length > index)
                    {
                        string outp = Path.GetFullPath(Output[index]);
                        string sr = src.Unpack("\n");
                        File.WriteAllText(outp, sr);
                    }

                    for (int i = 0; i < src.Length; i++)
                    {
                        Console.WriteLine(src[i]);
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


        public void PreApply(Settings settings)
        {

            settings.ApplySettings(Info, this);


            Logger.VerbosityLevel = (Verbosity)(DebugLvl);


            this.Log(DebugLevel.LOGS, "Verbosity Level set to: " + Logger.VerbosityLevel, Verbosity.LEVEL1);

        }

        private KeyValuePair<int, bool> ParseLogParams(string input)
        {
            int mask = -1;
            bool timestamp = true;
            string[] vars = input.Split(":");
            if (vars.Length > 0)
            {

                if (vars[0] != "all")
                    mask = (int)Utils.Parse(typeof(DebugLevel), vars[0], -1);
                if (vars.Length > 1)
                {
                    bool.TryParse(vars[1], out timestamp);
                }
            }
            return new KeyValuePair<int, bool>(mask, timestamp);

        }

        public void Apply(Settings settings)
        {

            if (LogToFile)
            {
                string[] args = LogToFileParams.Length > 1 ? LogToFileParams.SubArray(1, LogToFileParams.Length - 1).ToArray() : new string[0];
                KeyValuePair<int, bool> ts = ParseLogParams(args.Length != 0 ? args[0] : "");

                AddLogOutput(LogToFileParams[0], ts.Key, ts.Value);
                //lts.Mask = new BitMask<DebugLevel>(DebugLevel.ERRORS | DebugLevel.WARNINGS |
                //DebugLevel.INTERNAL_ERROR | DebugLevel.PROGRESS);

            }

            _defs = DefinesParams == null ?
                new Definitions() :
                new Definitions(DefinesParams.Select(x => new KeyValuePair<string, bool>(x, true)).
                    ToDictionary(x => x.Key, x => x.Value));
            if (ChainParams != null)
            {
                _chain = CreatePluginChain(ChainParams, NoCollections).ToList();
                this.Log(DebugLevel.LOGS, _chain.Count + " Plugins Loaded..", Verbosity.LEVEL2);
            }
            else
            {
                this.Log(DebugLevel.ERRORS, "Not plugin chain specified. 0 Plugins Loaded..", Verbosity.LEVEL1);
                _chain = new List<AbstractPlugin>();
            }
        }

        private IEnumerable<AbstractPlugin> CreatePluginChain(string[] arg, bool noCollection)
        {

            this.Log(DebugLevel.LOGS, "Creating Plugin Chain...", Verbosity.LEVEL3);
            List<AbstractPlugin> ret = new List<AbstractPlugin>();

            string[] names = null;
            string path = "";
            foreach (var plugin in arg)
            {

                if (plugin.Contains(':'))
                {
                    var tmp = plugin.Split(':').ToList();
                    names = tmp.SubArray(1, tmp.Count - 1).ToArray();
                    path = tmp[0];
                }
                else
                {
                    path = plugin; //Set the path
                    if (_pluginManager.GetPath(plugin, out path)) //Will change path if it matches prefix
                    { }
                    names = null;
                }


                if (File.Exists(path))
                {

                    Assembly asm = Assembly.LoadFile(Path.GetFullPath(path));
                    Type[] types = asm.GetTypes();
                    bool isCollection = names != null && names.Length == 1 && names[0].StartsWith('(') &&
                                        names[0].EndsWith(')');
                    if ((names == null && !noCollection) || isCollection)
                    {
                        if (names == null)
                        {
                            Type t = types.FirstOrDefault(x => x.GetInterfaces().Contains(typeof(IChainCollection)));
                            if (t != null)
                            {
                                List<AbstractPlugin> r = ((IChainCollection)Activator.CreateInstance(t)).GetChain()
                                    .Select(x => (AbstractPlugin)Activator.CreateInstance(x)).ToList();
                                this.Log(DebugLevel.LOGS, "Creating Chain Collection with Plugins: " + r.Select(x => x.GetType().Name).Unpack(", "), Verbosity.LEVEL2);
                                ret.AddRange(r);
                            }
                        }
                        else
                        {
                            names[0] = names[0].Trim('(', ')');
                            this.Log(DebugLevel.LOGS, "Searching Chain Collection: " + names[0], Verbosity.LEVEL2);

                            IChainCollection coll = types.Where(x => x.GetInterfaces().Contains(typeof(IChainCollection)))
                                 .Select(x => (IChainCollection)Activator.CreateInstance(x)).FirstOrDefault(x => x.GetName() == names[0]);

                            if (coll != null)
                            {

                                this.Log(DebugLevel.LOGS, "Found Chain Collection: " + names[0], Verbosity.LEVEL2);
                                List<AbstractPlugin> r = coll.GetChain()
                                    .Select(x => (AbstractPlugin)Activator.CreateInstance(x)).ToList();
                                this.Log(DebugLevel.LOGS, "Creating Chain Collection with Plugins: " + r.Select(x => x.GetType().Name).Unpack(", "), Verbosity.LEVEL2);
                                ret.AddRange(r);

                            }
                        }
                    }
                    else
                    {
                        this.Log(DebugLevel.LOGS, "Loading " + (names == null ? "all plugins" : names.Unpack(", ")) + " in file " + path, Verbosity.LEVEL4);

                        if (names == null)
                        {

                            foreach (var type in types)
                            {
                                if (type.IsSubclassOf(typeof(AbstractPlugin)))
                                {
                                    this.Log(DebugLevel.LOGS, "Creating instance of: " + type.Name, Verbosity.LEVEL5);
                                    ret.Add((AbstractPlugin)Activator.CreateInstance(type));
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < names.Length; i++)
                            {
                                for (int j = 0; j < types.Length; j++)
                                {
                                    if (types[j].Name == names[i])
                                    {
                                        this.Log(DebugLevel.LOGS, "Creating instance of: " + types[j].Name, Verbosity.LEVEL5);
                                        ret.Add((AbstractPlugin)Activator.CreateInstance(types[j]));
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {

                    this.Log(DebugLevel.ERRORS, "Could not load file: " + path, Verbosity.LEVEL1);
                }
            }

            return ret;
        }

        public IEnumerable<AbstractPlugin> CreatePluginChain(IEnumerable<Type> chain)
        {
            this.Log(DebugLevel.LOGS, "Creating Plugin Chain...", Verbosity.LEVEL3);
            List<AbstractPlugin> ret = new List<AbstractPlugin>();
            this.Log(DebugLevel.LOGS, "Loading " + chain.Select(x => x.Name).Unpack(", "), Verbosity.LEVEL4);
            foreach (var type in chain)
            {
                if (type.IsSubclassOf(typeof(AbstractPlugin)))
                {
                    this.Log(DebugLevel.LOGS, "Creating instance of: " + type.Name, Verbosity.LEVEL5);
                    ret.Add((AbstractPlugin)Activator.CreateInstance(type));
                }
                else
                {

                    this.Log(DebugLevel.WARNINGS, "Type: " + type.Name + " is not an AbstractPlugin", Verbosity.LEVEL1);
                }
            }

            return ret;
        }

        public Dictionary<string, string[]> AnalyzeArgs(string[] args)
        {
            Dictionary<string, string[]> ret = new Dictionary<string, string[]>();
            if (args.Length == 0) return ret;
            int cmdIdx = FindNextCommand(args, -1);
            if (cmdIdx == args.Length) return ret;
            do
            {
                int tmpidx = FindNextCommand(args, cmdIdx);
                ret.Add(args[cmdIdx], args.SubArray(cmdIdx + 1, tmpidx - cmdIdx - 1).ToArray());

            } while ((cmdIdx = FindNextCommand(args, cmdIdx)) != args.Length);

            return ret;
        }



        private static LogTextStream lts;

        private static void InitAdl()
        {

            CrashHandler.Initialize((int)DebugLevel.INTERNAL_ERROR, false);
            Debug.LoadConfig((AdlConfig)new AdlConfig().GetStandard());
            Debug.SetAllPrefixes("[ERRORS]", "[WARNINGS]", "[LOGS]", "[INTERNAL_ERROR]", "[PROGRESS]");
            Debug.CheckForUpdates = false;
            Debug.AdlWarningMask = (int)DebugLevel.WARNINGS;
            lts = new LogTextStream(
                Console.OpenStandardOutput(),
                -1,
                MatchType.MatchAll,
                false);

            Debug.AddOutputStream(lts);

        }

        private int FindNextCommand(string[] args, int start)
        {
            for (int i = start + 1; i < args.Length; i++)
            {
                if (args[i].StartsWith('-')) return i;
            }

            return args.Length;
        }


        private static void AddLogOutput(string file, int mask, bool timestamp)
        {
            if (File.Exists(file)) File.Delete(file);
            LogTextStream lts = new LogTextStream(File.OpenWrite(file), mask, MatchType.MatchAll, timestamp);
            Debug.AddOutputStream(lts);
        }


        public static void Main(string[] args)
        {




            if (args.Length == 0)
            {
                Console.WriteLine(HelpText);
            }
            else if (args[0] == "-fun")
            {
                Directory.SetCurrentDirectory("test");
                GenerateFiles("testfile", int.Parse(args[1]));
            }
            else
                new CLI(args);
#if DEBUG
            Console.ReadLine();
#endif
        }


        #region FunStuff

        public static List<string> GenerateDefineStatements(string defname, int maxNr, int maxDefines)
        {
            List<string> ret = new List<string>();
            Random r = new Random();
            int max = r.Next(1, maxDefines);
            for (int i = 0; i < max; i++)
            {
                ret.Add("#define " + defname + r.Next(0, maxNr));
            }

            return ret;
        }

        public static List<string> GenerateUndefineStatements(string defname, int maxNr, int maxDefines)
        {
            List<string> ret = new List<string>();
            Random r = new Random();
            int max = r.Next(1, maxDefines);
            for (int i = 0; i < max; i++)
            {
                ret.Add("#undefine " + defname + r.Next(0, maxNr));
            }

            return ret;
        }

        public static string GenerateExpression(string defName, int maxDefNr, int maxParams, int chanceToRecurse)
        {
            Random r = new Random();
            if (maxParams == 0) return defName + r.Next(0, maxDefNr);
            int max = r.Next(1, maxParams);
            string expr = "";
            for (int j = 0; j < max; j++)
            {
                int exprType = r.Next(0, 4 + chanceToRecurse);
                if (exprType == 0)
                    expr += defName + r.Next(0, maxDefNr);
                else if (exprType == 1)
                    expr += defName + r.Next(0, maxDefNr);
                else if (exprType == 2)
                    expr += defName + r.Next(0, maxDefNr);
                else if (exprType == 3)
                    expr += defName + r.Next(0, maxDefNr);
                else
                {
                    int maxprm = max - max / 2;
                    expr += "(" + GenerateExpression(defName, maxDefNr, maxprm, chanceToRecurse) + ")";

                }
                if (j != max - 1) expr += (r.Next(0, 2) == 0 ? " || " : " && ");
            }

            return expr;

        }

        public static void GenerateFiles(string filename, int amount)
        {
            Random r = new Random();
            for (int i = 0; i < amount; i++)
            {
                Console.WriteLine("Creating File: " + i + "/" + (amount - 1));
                List<string> ifs = GenerateIfStatements("TESTVAR", 50, 15);
                List<string> incs = GenerateGenericIncludes(filename, amount, 10, 10);
                incs.AddRange(GenerateRandomData(100, 50));

                incs.AddRange(GenerateDefineStatements("TESTVAR", 50, 10));
                incs.AddRange(GenerateUndefineStatements("TESTVAR", 50, 10));
                Shuffle(incs);
                Shuffle(incs);

                for (int j = 0; j < incs.Count; j++)
                {
                    ifs.Insert(r.Next(0, ifs.Count), incs[j]);
                }

                File.WriteAllLines(filename + i + ".txt", ifs);
            }
        }


        public static void Shuffle<T>(IList<T> list)
        {
            Random r = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = r.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static List<string> GenerateRandomData(int size, int datalength)
        {
            Random r = new Random();
            List<string> ret = new List<string>();
            for (int i = 0; i < size; i++)
            {
                string s = "";
                for (int j = 0; j < datalength; j++)
                {
                    s += (char)r.Next((int)'A', (int)'Z');
                }

                ret.Add(s);
            }

            return ret;
        }
        public static List<string> GenerateIfStatements(string defName, int maxDefNr, int maxNr)
        {
            Random r = new Random();
            int max = r.Next(1, maxNr);
            List<string> ret = new List<string>();
            for (int i = 0; i < max; i++)
            {
                string expr = GenerateExpression(defName, maxDefNr, 10, 1);
                ret.Add("#if " + expr);

                ret.Add("#endif");
            }

            return ret;
        }

        public static List<string> GenerateGenericIncludes(string file, int maxNr, int maxIncludes, int maxParams)
        {
            List<string> ret = new List<string>();
            Random r = new Random();
            int max = r.Next(1, maxIncludes);
            for (int i = 0; i < max; i++)
            {
                int paramMax = r.Next(1, maxParams);
                string gens = " ";
                for (int j = 0; j < paramMax; j++)
                {
                    if (r.Next(0, 2) == 0)
                    {
                        gens += "#type" + i + " ";
                    }
                    else
                    {
                        gens += GenerateRandomData(1, 15)[0] + " ";
                    }
                }
                ret.Add("#include " + file + r.Next(0, maxNr) + ".txt" + gens);
            }

            return ret;
        }

        public static List<string> GenerateIncludeStatements(string file, int maxNr, int maxIncludes)
        {
            List<string> ret = new List<string>();
            Random r = new Random();
            int max = r.Next(1, maxIncludes);
            for (int i = 0; i < max; i++)
            {
                ret.Add("#include " + file + r.Next(0, maxNr));
            }

            return ret;
        }

        #endregion
    }
}