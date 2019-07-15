using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ADL;
using ADL.Configs;
using ADL.Crash;
using ADL.Streams;
using ext_pp_base;
using ext_pp_base.settings;
using ext_pp_plugins;
using MatchType = ADL.MatchType;

namespace ext_pp
{
    public class CompilerConsole
    {
        private readonly Settings _settings;
        private readonly string CLIHeader = "\n\next_pp version: " + Assembly.GetExecutingAssembly().GetName().Version+ "\nCopyright by Tim Akermann\nGithub: https://github.com/ByteChkR/ext-pp\n\n";


        public CompilerConsole(string[] args)
        {
            _settings = new Settings();
            InitAdl();
            #region Preinformation

            var vset = false;
            var isShort = false;
            if ((isShort = args.Contains("-v")) || args.Contains("--verbosity"))
            {
                vset = true;
                var idx = args.ToList().IndexOf(isShort ? "-v" : "--verbosity");
                if (!int.TryParse(args[idx + 1], out var level))
                {
                    Logger.Log(DebugLevel.WARNINGS, "Could not read Verbosity: " + args[idx + 1], Verbosity.ALWAYS_SEND);
                }
                else
                {
                    Logger.VerbosityLevel = (Verbosity)level;
                }
            }




            Logger.Log(DebugLevel.LOGS, CLIHeader, Verbosity.ALWAYS_SEND);


            isShort = false;
            string lf = "";
            if ((isShort = args.Contains("-l2f")) || args.Contains("--logToFile"))
            {
                var idx = args.ToList().IndexOf(isShort ? "-l2f" : "--logToFile");
                lf = args[idx + 1];
                AddLogOutput(lf);
                Logger.Log(DebugLevel.LOGS, "Log to file flag found and path set to: " + lf, Verbosity.LEVEL1);
            }

            if (args.Contains("-2c") || args.Contains("--writeToConsole"))
            {
                _settings.WriteToConsole = true;
                if (!vset && Logger.VerbosityLevel > 0)
                {
                    Logger.VerbosityLevel = Verbosity.SILENT;
                }
                else if (vset && Logger.VerbosityLevel > 0)
                {
                    Logger.Log(DebugLevel.WARNINGS, "Writing to console while explicitly turning on verbosity makes the console output unusable" + lf, Verbosity.LEVEL1);
                }
                Logger.Log(DebugLevel.LOGS, "Log to file flag found and set ", Verbosity.LEVEL1);
            }

            #endregion


            ProcessInput(args, out var input, out var output, out var defs, out var chain);


            PreProcessor pp = new PreProcessor(_settings);
            pp.SetFileProcessingChain(chain);
            var source = pp.Compile(input, new Definitions(defs));


            if (_settings.WriteToConsole)
            {
                if (output != "")
                {
                    if (File.Exists(output))
                        File.Delete(output);
                    File.WriteAllLines(output, source);
                }
                foreach (var s in source)
                {
                    Console.WriteLine(s);
                }
            }
            else
            {
                if (File.Exists(output))
                    File.Delete(output);
                File.WriteAllLines(output, source);
            }

        }


        private static void AddLogOutput(string file)
        {
            LogTextStream lts = new LogTextStream(File.OpenWrite(file), -1, MatchType.MatchAll, true);
            Debug.AddOutputStream(lts);
        }

        public bool ProcessInput(string[] args, out string input, out string output, out Dictionary<string, bool> defs, out List<IPlugin> chain)
        {
            defs = new Dictionary<string, bool>();
            input = output = "";
            Dictionary<Type, IPlugin> _chain = new Dictionary<Type, IPlugin>()
            {
                {typeof(FakeGenericsPlugin), new FakeGenericsPlugin(_settings)},
                {typeof(ConditionalPlugin), new ConditionalPlugin(_settings)},
                {typeof(IncludePlugin), new IncludePlugin(_settings)},
                {typeof(WarningPlugin), new WarningPlugin(_settings)},
                {typeof(ErrorPlugin), new ErrorPlugin(_settings)}
            };

            #region Argument Analysis

            for (var i = 0; i < args.Length - 1; i++)
            {
                switch (args[i])
                {
                    case "-i":
                    case "--input":
                        {
                            if (File.Exists(args[i + 1]))
                            {
                                var dirname = Path.GetDirectoryName(args[i + 1]);
                                if (dirname != "") Directory.SetCurrentDirectory(dirname);
                                input = args[i + 1];
                            }
                            else
                            {
                                Logger.Log(DebugLevel.ERRORS, "Could not read Input Path: " + args[i + 1], Verbosity.ALWAYS_SEND);
                            }
                            break;
                        }

                    case "-o":
                    case "--output":
                        output = args[i + 1];
                        break;
                    case "-ed":
                    case "--enableDefine":
                        {

                            Logger.Log(DebugLevel.LOGS, "Enable Define Flag set to: " + args[i + 1], Verbosity.LEVEL1);
                            _settings.Set("eDef", args[i + 1]);

                            break;
                        }

                    case "-eu":
                    case "--enableUndefine":
                        {
                            Logger.Log(DebugLevel.LOGS, "Enable Undefine Flag set to: " + args[i + 1], Verbosity.LEVEL1);
                            _settings.Set("eUdef", args[i + 1]);

                            break;
                        }

                    case "-ec":
                    case "--enableConditions":
                        {
                            if (!bool.TryParse(args[i + 1], out bool enable))
                            {
                                Logger.Log(DebugLevel.WARNINGS, "Could not parse Enable Conditions flag: " + args[i + 1], Verbosity.LEVEL1);
                            }
                            else
                            {
                                Logger.Log(DebugLevel.LOGS, "Enable Conditions Flag set to: " + args[i + 1], Verbosity.LEVEL1);

                                if (!enable)
                                {
                                    _chain.Remove(typeof(ConditionalPlugin));
                                    // _settings.ResolveDefine = _settings.ResolveUnDefine = false;
                                }
                            }

                            break;
                        }

                    case "-ei":
                    case "--enableInclude":
                        {
                            if (!bool.TryParse(args[i + 1], out bool enable))
                            {
                                Logger.Log(DebugLevel.WARNINGS, "Could not parse Enable Include flag: " + args[i + 1], Verbosity.LEVEL1);
                            }
                            else
                            {
                                Logger.Log(DebugLevel.LOGS, "Enable Include Flag set to: " + args[i + 1], Verbosity.LEVEL1);
                                if (!enable)
                                {
                                    _chain.Remove(typeof(IncludePlugin));
                                }
                            }

                            break;
                        }

                    case "-eg":
                    case "--enableGenerics":
                        {
                            if (!bool.TryParse(args[i + 1], out bool resolve))
                            {
                                Logger.Log(DebugLevel.WARNINGS, "Could not parse Enable Generics flag: " + args[i + 1], Verbosity.LEVEL1);
                            }
                            else
                            {
                                Logger.Log(DebugLevel.LOGS, "Enable Generics Flag set to: " + args[i + 1], Verbosity.LEVEL1);
                                _chain.Remove(typeof(FakeGenericsPlugin));
                            }

                            break;
                        }

                    case "-ee":
                    case "--enableErrors":
                        {
                            if (!bool.TryParse(args[i + 1], out bool enable))
                            {
                                Logger.Log(DebugLevel.WARNINGS, "Could not parse Enable Errors flag: " + args[i + 1], Verbosity.LEVEL1);
                            }
                            else
                            {
                                Logger.Log(DebugLevel.LOGS, "Enable Errors Flag set to: " + args[i + 1], Verbosity.LEVEL1);
                                if (!enable)
                                {
                                    _chain.Remove(typeof(ErrorPlugin));
                                }
                            }

                            break;
                        }

                    default:
                        {
                            if (args[i] == "-ee" || args[i] == "--enableWarnings")
                            {
                                if (!bool.TryParse(args[i + 1], out bool enable))
                                {
                                    Logger.Log(DebugLevel.WARNINGS, "Could not parse Enable Warnings flag: " + args[i + 1], Verbosity.LEVEL1);
                                }
                                else
                                {
                                    Logger.Log(DebugLevel.LOGS, "Enable Warnings Flag set to: " + args[i + 1], Verbosity.LEVEL1);
                                    if (!enable)
                                    {
                                        _chain.Remove(typeof(WarningPlugin));
                                    }
                                }
                            }
                            else if (args[i].StartsWith("-kw") || args[i].StartsWith("--keyWord"))
                            {

                                int idx = args[i].IndexOf(':') + 1;
                                string prop = args[i].Substring(idx);

                                if (!_settings.KeyWordHandles.ContainsKey(prop))
                                {
                                    Logger.Log(DebugLevel.WARNINGS, "Could not find Keyword: " + prop, Verbosity.LEVEL1);
                                }
                                else
                                {
                                    FieldInfo pi = _settings.KeyWordHandles[prop];
                                    pi.SetValue(null, args[i + 1]);
                                    Logger.Log(DebugLevel.LOGS, "Keyword " + prop + " was set to: " + args[i + 1], Verbosity.LEVEL1);
                                }

                            }
                            else if (args[i] == "-def" || args[i] == "--defines")
                            {
                                for (int j = i + 1; j < args.Length; j++)
                                {
                                    if (args[j].StartsWith('-'))
                                    {
                                        i = j;
                                        break;
                                    }

                                    Logger.Log(DebugLevel.LOGS, "Predefining " + args[i + 1] + "." + args[i + 1], Verbosity.LEVEL1);
                                    if (!defs.ContainsKey(args[j]))
                                    {
                                        defs.Add(args[j], true);
                                    }
                                    else if (!defs[args[j]])
                                        defs[args[j]] = true;

                                }
                            }

                            break;
                        }
                }
            }

            chain = _chain.Values.ToList();
            #endregion

            #region CheckErrors



            if (input == "" || output == "" && !_settings.WriteToConsole)
            {

                Logger.Log(DebugLevel.ERRORS, "No input or no output specified", Verbosity.ALWAYS_SEND);
#if DEBUG
                Console.ReadLine();
#endif
                return false;
            }
            #endregion

            return true;


        }

        private static void Main(string[] args)
        {
            var cc = new CompilerConsole(args);

#if DEBUG
            Console.ReadLine();
#endif
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


    }
}
