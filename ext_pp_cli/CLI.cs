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

namespace ext_pp_cli
{
    public class CLI
    {
        private readonly string CLIHeader = "\n\next_pp version: " + Assembly.GetExecutingAssembly().GetName().Version + "\nCopyright by Tim Akermann\nGithub: https://github.com/ByteChkR/ext-pp\n\n";


        private Dictionary<string, FieldInfo> Info => new Dictionary<string, FieldInfo>()
        {
            {"i", PropertyHelper.GetFieldInfo(typeof(CLI), nameof(_input))},
            {"o", PropertyHelper.GetFieldInfo(typeof(CLI), nameof(_output))},
            {"input", PropertyHelper.GetFieldInfo(typeof(CLI), nameof(_input))},
            {"output", PropertyHelper.GetFieldInfo(typeof(CLI), nameof(_output))},
            {"defs", PropertyHelper.GetFieldInfo(typeof(CLI), nameof(_defStr))},
            {"chain", PropertyHelper.GetFieldInfo(typeof(CLI), nameof(_chainStr))},
            {"l2f", PropertyHelper.GetFieldInfo(typeof(CLI), nameof(_ltf))},
            {"w2c", PropertyHelper.GetFieldInfo(typeof(CLI), nameof(_wtc))},
            {"logToFile", PropertyHelper.GetFieldInfo(typeof(CLI), nameof(_ltf))},
            {"writeToConsole", PropertyHelper.GetFieldInfo(typeof(CLI), nameof(_wtc))},
            {"v", PropertyHelper.GetFieldInfo(typeof(CLI), nameof(_verbStr))},
            {"verbosity", PropertyHelper.GetFieldInfo(typeof(CLI), nameof(_verbStr))}
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
            Apply(_settings = new Settings(AnalyzeArgs(args)));

            PreProcessor pp = new PreProcessor();
            pp.SetFileProcessingChain(_chain);



            if ((_output == null && !_outputToConsole) || _input == null)
            {

                Logger.Log(DebugLevel.ERRORS, "Not enough arguments specified. Aborting..", Verbosity.LEVEL1);
                return;
            }

            string[] src = pp.Compile(_input, _settings, _defs);

            if (_outputToConsole)
            {
                if (_output != null)
                {
                    File.WriteAllLines(_output, src);
                }

                for (int i = 0; i < src.Length; i++)
                {
                    Console.WriteLine(src[i]);
                }
            }
            else File.WriteAllLines(_output, src);

        }

        public void Apply(Settings settings)
        {

            settings.ApplySettingsFlatString(Info, this);
            if (_logToFile) AddLogOutput(_ltf);

            if (_verbStr != null && int.TryParse(_verbStr, out int v))
            {

                Logger.VerbosityLevel = (Verbosity)(v);

                Logger.Log(DebugLevel.LOGS, "Verbosity Level set to: " + Logger.VerbosityLevel, Verbosity.LEVEL1);
            }
            Logger.Log(DebugLevel.LOGS, CLIHeader, Verbosity.LEVEL1);

            _defs = _defStr == null ?
                new Definitions() :
                new Definitions(_defStr.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => new KeyValuePair<string, bool>(x, true)).ToDictionary(x => x.Key, x => x.Value));
            if (_chainStr != null)
            {
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





        private static void InitAdl()
        {

            CrashHandler.Initialize((int)DebugLevel.INTERNAL_ERROR, false);
            Debug.LoadConfig((AdlConfig)new AdlConfig().GetStandard());
            Debug.SetAllPrefixes("[ERRORS]", "[WARNINGS]", "[LOGS]");
            Debug.CheckForUpdates = false;
            Debug.AdlWarningMask = (int)DebugLevel.WARNINGS;
            var lts = new LogTextStream(
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
            new CLI(args);
#if DEBUG
            Console.ReadLine();
#endif
        }
    }
}