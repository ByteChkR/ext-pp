using System;
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
    public class CLI
    {
        private static string HelpText = "To list all available commands type: ext_pp_cli --help or -h";

        private static string CLIHeader = "\n\next_pp_cli version: " + Assembly.GetExecutingAssembly().GetName().Version + "\nCopyright by Tim Akermann\nGithub: https://github.com/ByteChkR/ext-pp\n\n";




        private List<CommandInfo> Info => new List<CommandInfo>()
        {
            new CommandInfo("input", "i", PropertyHelper<CLI>.GetFieldInfo(x=>x._input),
                "--input filepath,filepath filepath,filepath\r\n\t, separated = the same compilation\r\n\t[space] separated means gets queued after the compilation of the first one"),
            new CommandInfo("output", "o", PropertyHelper<CLI>.GetFieldInfo(x=>x._output),
                "--output filepath filepath filepath\r\n\t[space] separated list of output files."),
            new CommandInfo("defines", "d", PropertyHelper<CLI>.GetFieldInfo(x=>x._defStr),
                "--defines <vars>\r\n\t, [space] separated list of predefines values"),
            new CommandInfo("chain", "c", PropertyHelper<CLI>.GetFieldInfo(x=>x._chainStr),
                "--chain [filepath]\r\n\t, separated list of plugins\r\n\t\t[filepath]:pluginname => loads a plugin by assembly name\r\n\t\t[filepath]:prefix => loads a plugin with prefix\r\n\t\t[filepath]:(collection) => loads a list(chain) of plugins from an IChainCollection with the specified name\r\n\t\tthe plugins in the /plugin folder can be directly accessed by using the prefix instead of the lines above"),
            new CommandInfo("log-to-file", "l2f",PropertyHelper<CLI>.GetFieldInfo(x=>x._ltf),
                "--log-to-file <file> <settings>\r\n\tCreates a log file with the settings\r\n\t\t<mask>:<timestamp>\r\n\t\tdefault: all:true"),
            new CommandInfo("write-to-console", "w2c", PropertyHelper<CLI>.GetFieldInfo(x=>x._outputToConsole),
                "--write2console [bool]\r\n\tWrites the result into the cout stream\r\n\tSets the verbosity to silent if not specified otherwise"),
            new CommandInfo("verbosity", "v", PropertyHelper<CLI>.GetFieldInfo(x=>x._verbStr),
                "--verbosity <int>\r\n\tSets the debug output granularity"),
            new CommandInfo("version", "vv", PropertyHelper<CLI>.GetFieldInfo(x=>x._showHeader),
            "--version\r\n\tdisplays the current version"),
            new CommandInfo("noColl", "nc", PropertyHelper<CLI>.GetFieldInfo(x=>x._noCollections),
                "The CLI will not search for a ChainCollection in the specified assembly"),
            new CommandInfo("help", "h", PropertyHelper<CLI>.GetFieldInfo(x=>x._help),
                "	\t--help <chainstr>\r\n\t\tlists the commands of the CLI or with supplied chain, it will display the help info of each plugin."),

        };

        public string _ltf = null;
        private bool _logToFile => !String.IsNullOrEmpty(_ltf);
        private bool _outputToConsole = false;
        public string _input = null;
        public string _output = null;
        public string[] _defStr = null;
        public bool _noCollections = false;
        public string[] _chainStr = null;
        public string[] _help = null;
        public int _verbStr = (int)Verbosity.LEVEL1;
        public bool _returnPreProcessing => _showHeader || _pluginList || _pluginListLong;
        public string[] _pluginString = null;
        public string[] _pluginStringLong = null;
        public bool _showHeader = false;
        public bool _pluginList => _pluginString != null && _pluginString.Length != 0;
        public bool _pluginListLong => _pluginStringLong != null && _pluginStringLong.Length != 0;


        private Definitions _defs;
        private Settings _settings;
        private List<AbstractPlugin> _chain;



        public CLI(string[] args)
        {
            InitAdl();

            if (args.Length == 1 && File.Exists(args[0]))
            {
                args = File.ReadAllLines(args[0]).Unpack(" ").Pack(" ").ToArray();
            }



            PreApply(_settings = new Settings(AnalyzeArgs(args)));

            Logger.Log(DebugLevel.LOGS, CLIHeader, Verbosity.LEVEL1);

            if (_help != null)
            {
                if (_help.Length == 0)
                {
                    Logger.Log(DebugLevel.LOGS, "\n" + Info.ListAllCommands(new[] { "" }).Unpack("\n"),
                        Verbosity.SILENT);
                    return;
                }
                foreach (var file in _help)
                {
                    if (file != "self")
                    {
                        List<AbstractPlugin> plugins = CreatePluginChain(new[] { file }, true).ToList();
                        Logger.Log(DebugLevel.LOGS, "Listing Plugins: ", Verbosity.SILENT);
                        foreach (var plugin in plugins)
                        {
                            Logger.Log(DebugLevel.LOGS, "\n" + plugin.ListInfo(true).Unpack("\n"),
                                Verbosity.SILENT);
                        }
                    }
                    else
                    {
                        Logger.Log(DebugLevel.LOGS, "\n" + Info.ListAllCommands(new[] { "" }).Unpack("\n"),
                            Verbosity.SILENT);
                    }


                }

                return;
            }

            





            if (_returnPreProcessing) return;

            Apply(_settings);


            PreProcessor pp = new PreProcessor();

            pp.SetFileProcessingChain(_chain);



            if ((_output == "" && !_outputToConsole) || _input == "")
            {
                Logger.Log(DebugLevel.ERRORS, "Not enough arguments specified. Aborting..", Verbosity.SILENT);
                Logger.Log(DebugLevel.LOGS, HelpText, Verbosity.LEVEL1);
                return;
            }

            string[] src = pp.Compile(_input, _settings, _defs);

            if (_outputToConsole)
            {
                if (_output != null)
                {
                    _output = Path.GetFullPath(_output);
                    string sr = src.Unpack("\n");
                    File.WriteAllText(_output, sr);
                }

                for (int i = 0; i < src.Length; i++)
                {
                    Console.WriteLine(src[i]);
                }
            }
            else
            {
                _output = Path.GetFullPath(_output);
                string sr = src.Unpack("\n");
                File.WriteAllText(_output, sr);
            }

        }


        public void PreApply(Settings settings)
        {

            settings.ApplySettings(Info, this);


            Logger.VerbosityLevel = (Verbosity)(_verbStr);
            if (_logToFile)
            {
                lts.Mask = new BitMask<DebugLevel>(DebugLevel.ERRORS | DebugLevel.WARNINGS |
                                                   DebugLevel.INTERNAL_ERROR | DebugLevel.PROGRESS);
            }

            Logger.Log(DebugLevel.LOGS, "Verbosity Level set to: " + Logger.VerbosityLevel, Verbosity.LEVEL1);

        }

        public void Apply(Settings settings)
        {

            if (_logToFile) AddLogOutput(_ltf);

            _defs = _defStr == null ?
                new Definitions() :
                new Definitions(_defStr.Select(x => new KeyValuePair<string, bool>(x, true)).
                    ToDictionary(x => x.Key, x => x.Value));
            if (_chainStr != null)
            {
                if (_chainStr.Length == 1 && _chainStr[0].EndsWith(".chain") && File.Exists(_chainStr[0]))
                {
                    Logger.Log(DebugLevel.LOGS, "Loading .chain File...", Verbosity.LEVEL2);
                    _chainStr = File.ReadAllLines(_chainStr[0]).Unpack(" ").Pack(" ").ToArray();

                    Logger.Log(DebugLevel.LOGS, "Loaded Chain Argument: " + _chainStr.Unpack(" "), Verbosity.LEVEL2);
                }
                _chain = CreatePluginChain(_chainStr, _noCollections).ToList();
                Logger.Log(DebugLevel.LOGS, _chain.Count + " Plugins Loaded..", Verbosity.LEVEL2);
            }
            else
            {
                Logger.Log(DebugLevel.ERRORS, "Not plugin chain specified. 0 Plugins Loaded..", Verbosity.LEVEL1);
                _chain = new List<AbstractPlugin>();
            }
        }

        private static IEnumerable<AbstractPlugin> CreatePluginChain(string[] arg, bool noCollection)
        {
            Logger.Log(DebugLevel.LOGS, "Creating Plugin Chain...", Verbosity.LEVEL3);
            List<AbstractPlugin> ret = new List<AbstractPlugin>();



            string[] names = null;
            string path = "";
            foreach (var plugin in arg)
            {

                if (plugin.Contains(':'))
                {
                    var tmp = plugin.Split(':');
                    names = tmp.SubArray(1, tmp.Length - 1).ToArray();
                    path = tmp[0];
                }
                else
                {
                    path = plugin;
                    names = null;
                }


                if (File.Exists(path))
                {

                    Assembly asm = Assembly.LoadFile(Path.GetFullPath(path));
                    Type[] types = asm.GetTypes();
                    if (!noCollection)
                    {
                        if (names == null)
                        {
                            Type t = types.FirstOrDefault(x => x.GetInterfaces().Contains(typeof(IChainCollection)));
                            if (t != null)
                            {
                                List<AbstractPlugin> r = ((IChainCollection)Activator.CreateInstance(t)).GetChain()
                                    .Select(x => (AbstractPlugin)Activator.CreateInstance(x)).ToList();
                                Logger.Log(DebugLevel.LOGS, "Creating Chain Collection with Plugins: " + r.Select(x => x.GetType().Name).Unpack(", "), Verbosity.LEVEL2);
                                ret.AddRange(r);
                            }
                        }
                        else if (names[0].StartsWith('(') && names[0].EndsWith(')'))
                        {
                            names[0] = names[0].Trim('(', ')');
                            Logger.Log(DebugLevel.LOGS, "Searching Chain Collection: " + names[0], Verbosity.LEVEL2);

                            IChainCollection coll = types.Where(x => x.GetInterfaces().Contains(typeof(IChainCollection)))
                                 .Select(x => (IChainCollection)Activator.CreateInstance(x)).FirstOrDefault(x => x.GetName() == names[0]);

                            if (coll != null)
                            {

                                Logger.Log(DebugLevel.LOGS, "Found Chain Collection: " + names[0], Verbosity.LEVEL2);
                                List<AbstractPlugin> r = coll.GetChain()
                                    .Select(x => (AbstractPlugin)Activator.CreateInstance(x)).ToList();
                                Logger.Log(DebugLevel.LOGS, "Creating Chain Collection with Plugins: " + r.Select(x => x.GetType().Name).Unpack(", "), Verbosity.LEVEL2);
                                ret.AddRange(r);

                            }
                        }
                    }
                    else
                    {
                        Logger.Log(DebugLevel.LOGS, "Loading " + (names == null ? " all plugins" : names.Unpack(", ")) + " in file " + path, Verbosity.LEVEL4);

                        if (names == null)
                        {

                            foreach (var type in types)
                            {
                                if (type.IsSubclassOf(typeof(AbstractPlugin)))
                                {
                                    Logger.Log(DebugLevel.LOGS, "Creating instance of: " + type.Name, Verbosity.LEVEL5);
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
                                        Logger.Log(DebugLevel.LOGS, "Creating instance of: " + types[j].Name, Verbosity.LEVEL5);
                                        ret.Add((AbstractPlugin)Activator.CreateInstance(types[j]));
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {

                    Logger.Log(DebugLevel.ERRORS, "Could not load file: " + path, Verbosity.LEVEL1);
                }
            }

            return ret;
        }

        public static IEnumerable<AbstractPlugin> CreatePluginChain(IEnumerable<Type> chain)
        {
            Logger.Log(DebugLevel.LOGS, "Creating Plugin Chain...", Verbosity.LEVEL3);
            List<AbstractPlugin> ret = new List<AbstractPlugin>();
            Logger.Log(DebugLevel.LOGS, "Loading " + chain.Select(x => x.Name).Unpack(", "), Verbosity.LEVEL4);
            foreach (var type in chain)
            {
                if (type.IsSubclassOf(typeof(AbstractPlugin)))
                {
                    Logger.Log(DebugLevel.LOGS, "Creating instance of: " + type.Name, Verbosity.LEVEL5);
                    ret.Add((AbstractPlugin)Activator.CreateInstance(type));
                }
                else
                {

                    Logger.Log(DebugLevel.WARNINGS, "Type: " + type.Name + " is not an AbstractPlugin", Verbosity.LEVEL2);
                }
            }

            return ret;
        }

        public Dictionary<string, string[]> AnalyzeArgs(string[] args)
        {
            Dictionary<string, string[]> ret = new Dictionary<string, string[]>();
            if (args.Length == 0) return ret;
            int cmdIdx = FindNextCommand(args, -1);
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
            Debug.SetAllPrefixes("[ERRORS]", "[WARNINGS]", "[LOGS]");
            Debug.CheckForUpdates = false;
            Debug.AdlWarningMask = (int)DebugLevel.WARNINGS;
            lts = new LogTextStream(
                Console.OpenStandardOutput(),
                -1,
                MatchType.MatchAll,
                true);

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


        private static void AddLogOutput(string file)
        {
            LogTextStream lts = new LogTextStream(File.OpenWrite(file), -1, MatchType.MatchAll, true);
            Debug.AddOutputStream(lts);
        }


        public static void Main(string[] args)
        {

            DebugLevel dl = (DebugLevel)EnumParser.Parse(typeof(DebugLevel), "ERRORS|WARNINGS");



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