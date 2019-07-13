using System;
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

        public CompilerConsole(string[] args)
        {

            #region Preinformation

            var vset = false;
            var isShort = false;
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
                    Settings.VerbosityLevel = (Verbosity)level;

                    Logger.Log(DebugLevel.LOGS, "Verbosity Level set to " + Settings.VerbosityLevel, Verbosity.LEVEL1);
                }
            }

            if (args.Contains("-2c") || args.Contains("--writeToConsole"))
            {
                Logger.Log(DebugLevel.LOGS, "Writing to console. ", Verbosity.LEVEL1);
                Settings.WriteToConsole = true;
                if (!vset && Settings.VerbosityLevel > 0)
                {
                    Settings.VerbosityLevel = Verbosity.SILENT;
                }
                else if (vset && Settings.VerbosityLevel > 0)
                    Logger.Log(DebugLevel.WARNINGS, "Writing to console. paired with log output. the output may only be usable for testing purposes.", Verbosity.ALWAYS_SEND);

            }

            #endregion
            ProcessInput(args, out var input, out var output, out var defs);

            Logger.Log(DebugLevel.LOGS, "Input file: " + input, Verbosity.ALWAYS_SEND);
            Logger.Log(DebugLevel.LOGS, "Output file: " + output, Verbosity.ALWAYS_SEND);


            var source = ExtensionProcessor.CompileFile(input, defs);


            if (Settings.WriteToConsole)
            {
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

        public static bool ProcessInput(string[] args, out string input, out string output, out Dictionary<string, bool> defs)
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
                        if (!bool.TryParse(args[i + 1], out Settings.ResolveDefine))
                        {
                            Logger.Log(DebugLevel.WARNINGS, "Resolve Define flag needs to be either \"true\" or \"false\"", Verbosity.ALWAYS_SEND);
                        }
                        else
                        {
                            Logger.Log(DebugLevel.LOGS, "Resolve Define flag set to " + Settings.ResolveDefine, Verbosity.LEVEL1);
                        }

                        break;
                    }

                    case "-ru":
                    case "--resolveUndefine":
                    {
                        if (!bool.TryParse(args[i + 1], out Settings.ResolveUnDefine))
                        {
                            Logger.Log(DebugLevel.WARNINGS, "Resolve Undefine flag needs to be either \"true\" or \"false\"", Verbosity.ALWAYS_SEND);
                        }
                        else
                        {
                            Logger.Log(DebugLevel.LOGS, "Resolve Undefine flag set to " + Settings.ResolveUnDefine, Verbosity.LEVEL1);
                        }

                        break;
                    }

                    case "-rc":
                    case "--resolveConditions":
                    {
                        if (!bool.TryParse(args[i + 1], out Settings.ResolveConditions))
                        {
                            Logger.Log(DebugLevel.WARNINGS, "Resolve Conditions flag needs to be either \"true\" or \"false\"", Verbosity.ALWAYS_SEND);
                        }
                        else
                        {
                            if (!Settings.ResolveConditions)
                            {
                                Settings.ResolveDefine = Settings.ResolveUnDefine = false;
                            }
                            Logger.Log(DebugLevel.LOGS, "Resolve Conditions flag set to " + Settings.ResolveConditions, Verbosity.LEVEL1);
                        }

                        break;
                    }

                    case "-ri":
                    case "--resolveInclude":
                    {
                        if (!bool.TryParse(args[i + 1], out Settings.ResolveIncludes))
                        {
                            Logger.Log(DebugLevel.WARNINGS, "Resolve Includes flag needs to be either \"true\" or \"false\"", Verbosity.ALWAYS_SEND);
                        }
                        else
                        {
                            Logger.Log(DebugLevel.LOGS, "Resolve Includes flag set to " + Settings.ResolveIncludes, Verbosity.LEVEL1);
                        }

                        break;
                    }

                    case "-rg":
                    case "--resolveGenerics":
                    {
                        if (!bool.TryParse(args[i + 1], out Settings.ResolveGenerics))
                        {
                            Logger.Log(DebugLevel.WARNINGS, "Resolve Generics flag needs to be either \"true\" or \"false\"", Verbosity.ALWAYS_SEND);
                        }
                        else
                        {
                            Logger.Log(DebugLevel.LOGS, "Resolve Generics flag set to " + Settings.ResolveGenerics, Verbosity.LEVEL1);
                        }

                        break;
                    }

                    case "-ee":
                    case "--enableErrors":
                    {
                        if (!bool.TryParse(args[i + 1], out Settings.EnableErrors))
                        {
                            Logger.Log(DebugLevel.WARNINGS, "Enable Errors flag needs to be either \"true\" or \"false\"", Verbosity.ALWAYS_SEND);
                        }
                        else
                        {
                            Logger.Log(DebugLevel.LOGS, "Enable Errors flag set to " + Settings.EnableErrors, Verbosity.LEVEL1);
                        }

                        break;
                    }

                    default:
                    {
                        if (args[i] == "-ee" || args[i] == "--enableWarnings")
                        {
                            if (!bool.TryParse(args[i + 1], out Settings.EnableErrors))
                            {
                                Logger.Log(DebugLevel.WARNINGS, "Enable Warnings flag needs to be either \"true\" or \"false\"", Verbosity.ALWAYS_SEND);
                            }
                            else
                            {
                                Logger.Log(DebugLevel.LOGS, "Enable Warnings flag set to " + Settings.EnableWarnings, Verbosity.LEVEL1);
                            }
                        }
                        else if (args[i] == "-ss" || args[i] == "--setSeparator")
                        {
                            if (!char.TryParse(args[i + 1], out Settings.Separator))
                            {
                                Logger.Log(DebugLevel.WARNINGS, "Invalid Separator. Only one character.",
                                    Verbosity.ALWAYS_SEND);
                            }
                            else
                            {
                                Logger.Log(DebugLevel.LOGS, "Separator " + Settings.Separator,
                                    Verbosity.LEVEL1);
                            }
                        }
                        else if (args[i] == "-n" || args[i] == "--negation")
                        {
                            if (!char.TryParse(args[i + 1], out Settings.NegateStatement))
                            {
                                Logger.Log(DebugLevel.WARNINGS, "Invalid Negation. Only one character.",
                                    Verbosity.ALWAYS_SEND);
                            }
                            else
                            {
                                Logger.Log(DebugLevel.LOGS, "Negation " + Settings.Separator,
                                    Verbosity.LEVEL1);
                            }
                        }
                            else if (args[i].StartsWith("-kw") || args[i].StartsWith("--keyWord"))
                        {

                            int idx = args[i].IndexOf(':') + 1;
                            string prop = args[i].Substring(idx);
                    
                            if (!Settings.KeyWordHandles.ContainsKey(prop))
                            {
                                Logger.Log(DebugLevel.WARNINGS, "Invalid Property Key.",
                                    Verbosity.ALWAYS_SEND);
                            }
                            else
                            {
                                FieldInfo pi = Settings.KeyWordHandles[prop];
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

            if (input == "" || output == "" && !Settings.WriteToConsole)
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
            InitAdl();

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
