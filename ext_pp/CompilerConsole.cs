﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ADL;
using ADL.Configs;
using ADL.Crash;
using ADL.Streams;
using ext_pp.settings;
using MatchType = ADL.MatchType;

namespace ext_pp
{
    public class CompilerConsole
    {
        private readonly Settings _settings;

        public CompilerConsole(string[] args)
        {
            _settings = new Settings();
            InitAdl();
            #region Preinformation

            var isShort = false;


            var vset = false;
            if ((isShort = args.Contains("-v")) || args.Contains("--verbosity"))
            {
                vset = true;
                var idx = args.ToList().IndexOf(isShort ? "-v" : "--verbosity");
                if (!int.TryParse(args[idx + 1], out var level))
                {
                    Logger.Log(DebugLevel.WARNINGS, "Verbosity level needs to be a positive number.", Verbosity.ALWAYS_SEND);
                }
                else
                {
                    _settings.VerbosityLevel = (Verbosity)level;

                    Logger.Log(DebugLevel.LOGS, "Verbosity Level set to " + _settings.VerbosityLevel, Verbosity.LEVEL1);
                }
            }
            isShort = false;
            string lf = "";
            if ((isShort = args.Contains("-l2f")) || args.Contains("--logToFile"))
            {
                var idx = args.ToList().IndexOf(isShort ? "-l2f" : "--logToFile");
                lf = args[idx + 1];

                Logger.Log(DebugLevel.LOGS, "Log File is set to " + lf, Verbosity.LEVEL1);
                AddLogOutput(lf);
            }

            if (args.Contains("-2c") || args.Contains("--writeToConsole"))
            {
                Logger.Log(DebugLevel.LOGS, "Writing to console. ", Verbosity.LEVEL1);
                _settings.WriteToConsole = true;
                if (!vset && _settings.VerbosityLevel > 0)
                {
                    _settings.VerbosityLevel = Verbosity.SILENT;
                }
                else if (vset && _settings.VerbosityLevel > 0)
                    Logger.Log(DebugLevel.WARNINGS, "Writing to console. paired with log output. the output may only be usable for testing purposes.", Verbosity.ALWAYS_SEND);

            }




            #endregion
            ProcessInput(args, out var input, out var output, out var defs);

            Logger.Log(DebugLevel.LOGS, "Input file: " + input, Verbosity.ALWAYS_SEND);
            Logger.Log(DebugLevel.LOGS, "Output file: " + output, Verbosity.ALWAYS_SEND);




            PreProcessor pp = new PreProcessor(_settings);
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

        public bool ProcessInput(string[] args, out string input, out string output, out Dictionary<string, bool> defs)
        {
            defs = new Dictionary<string, bool>();
            input = output = "";


            #region Argument Analysis

            for (var i = 0; i < args.Length - 1; i++)
            {
                switch (args[i])
                {
                    case "-i":
                    case "--input":
                        {
                            var dirname = Path.GetDirectoryName(args[i + 1]);
                            if (dirname != "") Directory.SetCurrentDirectory(dirname);
                            input = args[i + 1];
                            break;
                        }

                    case "-o":
                    case "--output":
                        output = args[i + 1];
                        break;
                    case "-rd":
                    case "--resolveDefine":
                        {
                            if (!bool.TryParse(args[i + 1], out _settings.ResolveDefine))
                            {
                                Logger.Log(DebugLevel.WARNINGS, "Resolve Define flag needs to be either \"true\" or \"false\"", Verbosity.ALWAYS_SEND);
                            }
                            else
                            {
                                Logger.Log(DebugLevel.LOGS, "Resolve Define flag set to " + _settings.ResolveDefine, Verbosity.LEVEL1);
                            }

                            break;
                        }

                    case "-ru":
                    case "--resolveUndefine":
                        {
                            if (!bool.TryParse(args[i + 1], out _settings.ResolveUnDefine))
                            {
                                Logger.Log(DebugLevel.WARNINGS, "Resolve Undefine flag needs to be either \"true\" or \"false\"", Verbosity.ALWAYS_SEND);
                            }
                            else
                            {
                                Logger.Log(DebugLevel.LOGS, "Resolve Undefine flag set to " + _settings.ResolveUnDefine, Verbosity.LEVEL1);
                            }

                            break;
                        }

                    case "-rc":
                    case "--resolveConditions":
                        {
                            if (!bool.TryParse(args[i + 1], out _settings.ResolveConditions))
                            {
                                Logger.Log(DebugLevel.WARNINGS, "Resolve Conditions flag needs to be either \"true\" or \"false\"", Verbosity.ALWAYS_SEND);
                            }
                            else
                            {
                                if (!_settings.ResolveConditions)
                                {
                                    _settings.ResolveDefine = _settings.ResolveUnDefine = false;
                                }
                                Logger.Log(DebugLevel.LOGS, "Resolve Conditions flag set to " + _settings.ResolveConditions, Verbosity.LEVEL1);
                            }

                            break;
                        }

                    case "-ri":
                    case "--resolveInclude":
                        {
                            if (!bool.TryParse(args[i + 1], out _settings.ResolveIncludes))
                            {
                                Logger.Log(DebugLevel.WARNINGS, "Resolve Includes flag needs to be either \"true\" or \"false\"", Verbosity.ALWAYS_SEND);
                            }
                            else
                            {
                                Logger.Log(DebugLevel.LOGS, "Resolve Includes flag set to " + _settings.ResolveIncludes, Verbosity.LEVEL1);
                            }

                            break;
                        }

                    case "-rg":
                    case "--resolveGenerics":
                        {
                            if (!bool.TryParse(args[i + 1], out _settings.ResolveGenerics))
                            {
                                Logger.Log(DebugLevel.WARNINGS, "Resolve Generics flag needs to be either \"true\" or \"false\"", Verbosity.ALWAYS_SEND);
                            }
                            else
                            {
                                Logger.Log(DebugLevel.LOGS, "Resolve Generics flag set to " + _settings.ResolveGenerics, Verbosity.LEVEL1);
                            }

                            break;
                        }

                    case "-ee":
                    case "--enableErrors":
                        {
                            if (!bool.TryParse(args[i + 1], out _settings.EnableErrors))
                            {
                                Logger.Log(DebugLevel.WARNINGS, "Enable Errors flag needs to be either \"true\" or \"false\"", Verbosity.ALWAYS_SEND);
                            }
                            else
                            {
                                Logger.Log(DebugLevel.LOGS, "Enable Errors flag set to " + _settings.EnableErrors, Verbosity.LEVEL1);
                            }

                            break;
                        }

                    default:
                        {
                            if (args[i] == "-ee" || args[i] == "--enableWarnings")
                            {
                                if (!bool.TryParse(args[i + 1], out _settings.EnableErrors))
                                {
                                    Logger.Log(DebugLevel.WARNINGS, "Enable Warnings flag needs to be either \"true\" or \"false\"", Verbosity.ALWAYS_SEND);
                                }
                                else
                                {
                                    Logger.Log(DebugLevel.LOGS, "Enable Warnings flag set to " + _settings.EnableWarnings, Verbosity.LEVEL1);
                                }
                            }
                            else if (args[i] == "-ss" || args[i] == "--setSeparator")
                            {
                                _settings.Separator = args[i + 1];
                                Logger.Log(DebugLevel.LOGS, "Separator " + _settings.Separator,
                                    Verbosity.LEVEL1);

                            }
                            else if (args[i] == "-nO" || args[i] == "--notOperator")
                            {

                                _settings.NotOperator = args[i + 1];
                                Logger.Log(DebugLevel.LOGS, "Not Operator: " + _settings.NotOperator,
                                    Verbosity.LEVEL1);

                            }
                            else if (args[i] == "-aO" || args[i] == "--andOperator")
                            {

                                _settings.AndOperator = args[i + 1];
                                Logger.Log(DebugLevel.LOGS, "And Operator: " + _settings.NotOperator,
                                    Verbosity.LEVEL1);

                            }
                            else if (args[i] == "-oo" || args[i] == "--orOperator")
                            {

                                _settings.OrOperator = args[i + 1];
                                Logger.Log(DebugLevel.LOGS, "Or Operator: " + _settings.NotOperator,
                                    Verbosity.LEVEL1);

                            }
                            else if (args[i].StartsWith("-kw") || args[i].StartsWith("--keyWord"))
                            {

                                int idx = args[i].IndexOf(':') + 1;
                                string prop = args[i].Substring(idx);

                                if (!_settings.KeyWordHandles.ContainsKey(prop))
                                {
                                    Logger.Log(DebugLevel.WARNINGS, "Invalid Property Key.",
                                        Verbosity.ALWAYS_SEND);
                                }
                                else
                                {
                                    FieldInfo pi = _settings.KeyWordHandles[prop];
                                    Logger.Log(DebugLevel.LOGS, "Property set: " + pi.Name + "=" + args[i + 1],
                                        Verbosity.LEVEL1);
                                    pi.SetValue(null, args[i + 1]);
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

                                    if (!defs.ContainsKey(args[j]))
                                    {
                                        defs.Add(args[j], true);
                                    }
                                    else if (!defs[args[j]])
                                        defs[args[j]] = true;

                                }
                                Logger.Log(DebugLevel.LOGS, "Added " + defs.Count + " Global Definitions", Verbosity.LEVEL1);
                                foreach (var def in defs)
                                {
                                    Logger.Log(DebugLevel.LOGS, "Added Global Definition " + def.Key + ": " + def.Value.ToString(), Verbosity.LEVEL2);
                                }
                            }

                            break;
                        }
                }
            }
            #endregion

            #region CheckErrors

            if (input == "" || output == "" && !_settings.WriteToConsole)
            {
                Logger.Log(DebugLevel.ERRORS, "Invalid Command.", Verbosity.ALWAYS_SEND);
                Logger.Log(DebugLevel.LOGS, Settings.HelpText, Verbosity.ALWAYS_SEND);

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

            CrashHandler.Initialize((int)DebugLevel.ERRORS, false);
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
