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

namespace ext_pp_cli
{
    public class CLI
    {
        public static string HelpText = "To list all available commands type: ext_pp_cli -l self";

        private readonly string CLIHeader = "\n\next_pp version: " + Assembly.GetExecutingAssembly().GetName().Version + "\nCopyright by Tim Akermann\nGithub: https://github.com/ByteChkR/ext-pp\n\n";


        private List<CommandInfo> Info => new List<CommandInfo>()
        {
            new CommandInfo("i", PropertyHelper.GetFieldInfo(typeof(CLI), nameof(_input)),
                "Sets the input file(required)\nUsage: -i <pathtofile>"),
            new CommandInfo("o", PropertyHelper.GetFieldInfo(typeof(CLI), nameof(_output)),
                "Sets the output file(not required when writing to console(-w2c))\nUsage: -o <pathtofile>"),
            new CommandInfo("input", PropertyHelper.GetFieldInfo(typeof(CLI), nameof(_input)),
                "Sets the input file(required)\nUsage: -input <pathtofile>"),
            new CommandInfo("output", PropertyHelper.GetFieldInfo(typeof(CLI), nameof(_output)),
                "Sets the output file(not required when writing to console(-w2c))\nUsage: -output <pathtofile>"),
            new CommandInfo("defs", PropertyHelper.GetFieldInfo(typeof(CLI), nameof(_defStr)),
                "Predefines definitions that are set on the beginning of the process.\nUsage: -defs \"DEFINE DEFINE ...\""),
            new CommandInfo("chain", PropertyHelper.GetFieldInfo(typeof(CLI), nameof(_chainStr)),
                "Predefines definitions that are set on the beginning of the process.\nUsage: -chain <pathtodll> => loads all files\n-chain<pathtodll>:<pluginname> => loads a specific plugin\n-chain <pathtodll>:plugin:plugin2 => loading is chainable\n-chain \"<pathtodll>:plugin1 <pathtodll>:plugin2>\" => load plugins from different files."),
            new CommandInfo("l2f", PropertyHelper.GetFieldInfo(typeof(CLI), nameof(_ltf)),
                "Logs the console output to the file specified.\nUsage: -l2f <pathtofile>"),
            new CommandInfo("w2c", PropertyHelper.GetFieldInfo(typeof(CLI), nameof(_wtc)),
                "Writes the result in the console(automatically turns of debug output if not specifically set).\nUsage: -w2c <pathtofile>"),
            new CommandInfo("logToFile", PropertyHelper.GetFieldInfo(typeof(CLI), nameof(_ltf)),
                "Logs the console output to the file specified.\nUsage: -logToFile <pathtofile>"),
            new CommandInfo("writeToConsole", PropertyHelper.GetFieldInfo(typeof(CLI), nameof(_wtc)),
                "Writes the result in the console(automatically turns of debug output if not specifically set).\nUsage: -writeToConsole <pathtofile>"),
            new CommandInfo("v", PropertyHelper.GetFieldInfo(typeof(CLI), nameof(_verbStr)),
                "Sets the verbosity of the debug logs. \nUsage: -v [number 0 to ~8]"),
            new CommandInfo("verbosity", PropertyHelper.GetFieldInfo(typeof(CLI), nameof(_verbStr)),
                "Sets the verbosity of the debug logs. \nUsage: -verbosity [number 0 to ~8]")
        };

        public string _ltf = null;
        private bool _logToFile => _ltf != null;
        public string _wtc = null;
        private bool _outputToConsole => _wtc != null;
        public string _input = null;
        public string _output = null;
        public string _defStr = "";
        public string _chainStr = null;
        public string _verbStr = null;

        private Definitions _defs;
        private Settings _settings;
        private List<IPlugin> _chain;
        public CLI(string[] args)
        {
            InitAdl();

            if (args.Length == 1 && File.Exists(args[0]))
            {
                args = File.ReadAllLines(args[0]).Unpack(" ").Pack(" ").ToArray();
            }

            int argInd;
            if ((argInd = args.ToList().IndexOf("-l")) != -1 ||
                (argInd = args.ToList().IndexOf("-ll")) != -1 &&
                args.Length > argInd + 1)
            {
                bool shortDesc = args[argInd] == "-l";
                string file = args[argInd + 1];

                if (file != "self")
                {
                    List<IPlugin> plugins = CreatePluginChain(file).ToList();
                    Logger.Log(DebugLevel.LOGS, "Listing Plugins: ", Verbosity.SILENT);
                    foreach (var plugin in plugins)
                    {
                        Logger.Log(DebugLevel.LOGS, "\n" + plugin.ListInfo(!shortDesc).Unpack("\n"), Verbosity.SILENT);
                    }
                }
                else
                {
                    Logger.Log(DebugLevel.LOGS, "\n" + Info.ListAllCommands(new[] { "ext_pp_cli" }).Unpack("\n"), Verbosity.SILENT);
                }

                return;
            }


            Apply(_settings = new Settings(AnalyzeArgs(args)));

            PreProcessor pp = new PreProcessor();
            pp.SetFileProcessingChain(_chain);



            if ((_output == null && !_outputToConsole) || _input == null)
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
                    File.WriteAllLines(_output, src);
                }

                for (int i = 0; i < src.Length; i++)
                {
                    Console.WriteLine(src[i]);
                }
            }
            else
            {
                _output = Path.GetFullPath(_output);
                File.WriteAllLines(_output, src);
            }

        }

        public void Apply(Settings settings)
        {

            settings.ApplySettingsFlatString(Info, this);
            if (_logToFile) AddLogOutput(_ltf);

            if (_verbStr != null && int.TryParse(_verbStr, out int v))
            {

                Logger.VerbosityLevel = (Verbosity)(v);
                if (_logToFile)
                {
                    lts.Mask = new BitMask<DebugLevel>(DebugLevel.ERRORS | DebugLevel.WARNINGS |
                                                       DebugLevel.INTERNAL_ERROR | DebugLevel.PROGRESS);
                }

                Logger.Log(DebugLevel.LOGS, "Verbosity Level set to: " + Logger.VerbosityLevel, Verbosity.LEVEL1);
            }
            Logger.Log(DebugLevel.LOGS, CLIHeader, Verbosity.LEVEL1);

            _defs = _defStr == null ?
                new Definitions() :
                new Definitions(_defStr.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => new KeyValuePair<string, bool>(x, true)).ToDictionary(x => x.Key, x => x.Value));
            if (_chainStr != null)
            {
                if (_chainStr.EndsWith(".chain") && File.Exists(_chainStr))
                {
                    Logger.Log(DebugLevel.LOGS, "Loading .chain File...", Verbosity.LEVEL2);
                    _chainStr = File.ReadAllLines(_chainStr).Unpack(" ");

                    Logger.Log(DebugLevel.LOGS, "Loaded Chain Argument: " + _chainStr, Verbosity.LEVEL2);
                }
                _chain = CreatePluginChain(_chainStr).ToList();
                Logger.Log(DebugLevel.LOGS, _chain.Count + " Plugins Loaded..", Verbosity.LEVEL2);
            }
            else
            {
                Logger.Log(DebugLevel.ERRORS, "Not plugin chain specified. 0 Plugins Loaded..", Verbosity.LEVEL1);
                _chain = new List<IPlugin>();
            }
        }

        private static IEnumerable<IPlugin> CreatePluginChain(string arg)
        {
            Logger.Log(DebugLevel.LOGS, "Creating Plugin Chain...", Verbosity.LEVEL3);
            List<IPlugin> ret = new List<IPlugin>();
            string[] plugins = arg.Split(' ');


            string[] names = null;
            string path = "";
            foreach (var plugin in plugins)
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

                    Logger.Log(DebugLevel.LOGS, "Loading " + (names == null ? " all plugins" : names.Unpack(", ")) + " in file " + path, Verbosity.LEVEL4);
                    Assembly asm = Assembly.LoadFile(Path.GetFullPath(path));
                    Type[] types = asm.GetTypes();
                    if (names == null)
                    {

                        foreach (var type in types)
                        {
                            if (type.GetInterfaces().Contains(typeof(IPlugin)))
                            {
                                Logger.Log(DebugLevel.LOGS, "Creating instance of: " + type.Name, Verbosity.LEVEL5);
                                ret.Add((IPlugin)Activator.CreateInstance(type));
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
                                    ret.Add((IPlugin)Activator.CreateInstance(types[j]));
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

        public static IEnumerable<IPlugin> CreatePluginChain(IEnumerable<Type> chain)
        {
            Logger.Log(DebugLevel.LOGS, "Creating Plugin Chain...", Verbosity.LEVEL3);
            List<IPlugin> ret = new List<IPlugin>();
            Logger.Log(DebugLevel.LOGS, "Loading " + chain.Select(x => x.Name).Unpack(", "), Verbosity.LEVEL4);
            foreach (var type in chain)
            {
                if (type.GetInterfaces().Contains(typeof(IPlugin)))
                {
                    Logger.Log(DebugLevel.LOGS, "Creating instance of: " + type.Name, Verbosity.LEVEL5);
                    ret.Add((IPlugin)Activator.CreateInstance(type));
                }
                else
                {

                    Logger.Log(DebugLevel.WARNINGS, "Type: " + type.Name + " is not an IPlugin", Verbosity.LEVEL2);
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
            if (args[0] == "-fun")
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