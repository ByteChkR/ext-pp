using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ADL;
using ADL.Configs;
using ADL.Crash;
using ADL.Streams;
using ext_compiler.extensions;
using ext_compiler.settings;
using MatchType = ADL.MatchType;

namespace ext_compiler
{
    public static class CompilerConsole
    {
        private static Dictionary<string, FieldInfo> _keyWordHandles = null;

        private static Dictionary<string, FieldInfo> KeyWordHandles => _keyWordHandles ?? (_keyWordHandles = GetInfo());

        private static Dictionary<string, FieldInfo> GetInfo()
        {
            return new Dictionary<string, FieldInfo>()
            {

                {"d", PropertyHelper<Keywords>.GetFieldInfo(x=>x.DefineStatement)},
                {"u", PropertyHelper<Keywords>.GetFieldInfo(x=>x.UndefineStatement)},
                {"if", PropertyHelper<Keywords>.GetFieldInfo(x=>x.IfStatement)},
                {"elif", PropertyHelper<Keywords>.GetFieldInfo(x=>x.ElseIfStatement)},
                {"else", PropertyHelper<Keywords>.GetFieldInfo(x=>x.ElseStatement)},
                {"eif", PropertyHelper<Keywords>.GetFieldInfo(x=>x.EndIfStatement)},
                {"w", PropertyHelper<Keywords>.GetFieldInfo(x=>x.WarningStatement)},
                {"e", PropertyHelper<Keywords>.GetFieldInfo(x=>x.ErrorStatement)},
                {"i", PropertyHelper<Keywords>.GetFieldInfo(x=>x.IncludeStatement)},
                {"t", PropertyHelper<Keywords>.GetFieldInfo(x=>x.TypeGenKeyword)},

            };
        }

        static void Main(string[] args)
        {
            #region Preinformation

            bool isShort = false;
            bool vset = false;
            if ((isShort = args.Contains("-v")) || args.Contains("--verbosity"))
            {
                vset = true;
                int idx = args.ToList().IndexOf(isShort ? "-v" : "--verbosity");
                if (!int.TryParse(args[idx+1], out var level))
                {
                    Logger.Log(DebugLevel.WARNINGS, "Enable Warnings flag needs to be either \"true\" or \"false\"", Verbosity.ALWAYS_SEND);
                }
                else
                {
                    ExtensionProcessor.settings.VerbosityLevel = (Verbosity)level;

                    Logger.Log(DebugLevel.LOGS, "Enable Warnings flag set to " + ExtensionProcessor.settings.VerbosityLevel, Verbosity.LEVEL1);
                }
            }

            if (args.Contains("-2c") || args.Contains("--writeToConsole"))
            {
                Logger.Log(DebugLevel.LOGS, "Writing to console. ", Verbosity.LEVEL1);
                ExtensionProcessor.settings.WriteToConsole = true;
                if (!vset && ExtensionProcessor.settings.VerbosityLevel > 0)
                {
                    ExtensionProcessor.settings.VerbosityLevel = Verbosity.SILENT;
                }
            }

            #endregion

            InitADL();

            Dictionary<string, bool> defs = new Dictionary<string, bool>();
            string input = "";
            string output = "";

            #region Argument Analysis

            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == "-i" || args[i] == "--input")
                {
                    string dirname = Path.GetDirectoryName(args[i + 1]);
                    if (dirname != "") Directory.SetCurrentDirectory(dirname);
                    input = args[i + 1];
                }
                else if (args[i] == "-o" || args[i] == "--output")
                {
                    output = args[i + 1];
                }
                else if (args[i] == "-rd" || args[i] == "--resolveDefine")
                {
                    if (!bool.TryParse(args[i + 1], out ExtensionProcessor.settings.ResolveDefine))
                    {
                        Logger.Log(DebugLevel.WARNINGS, "Resolve Define flag needs to be either \"true\" or \"false\"", Verbosity.ALWAYS_SEND);
                    }
                    else
                    {
                        Logger.Log(DebugLevel.LOGS, "Resolve Define flag set to " + ExtensionProcessor.settings.ResolveDefine, Verbosity.LEVEL1);
                    }
                }
                else if (args[i] == "-ru" || args[i] == "--resolveUndefine")
                {
                    if (!bool.TryParse(args[i + 1], out ExtensionProcessor.settings.ResolveUnDefine))
                    {
                        Logger.Log(DebugLevel.WARNINGS, "Resolve Undefine flag needs to be either \"true\" or \"false\"", Verbosity.ALWAYS_SEND);
                    }
                    else
                    {
                        Logger.Log(DebugLevel.LOGS, "Resolve Undefine flag set to " + ExtensionProcessor.settings.ResolveUnDefine, Verbosity.LEVEL1);
                    }
                }
                else if (args[i] == "-rc" || args[i] == "--resolveConditions")
                {
                    if (!bool.TryParse(args[i + 1], out ExtensionProcessor.settings.ResolveConditions))
                    {
                        Logger.Log(DebugLevel.WARNINGS, "Resolve Conditions flag needs to be either \"true\" or \"false\"", Verbosity.ALWAYS_SEND);
                    }
                    else
                    {
                        if (!ExtensionProcessor.settings.ResolveConditions)
                        {
                            ExtensionProcessor.settings.ResolveDefine = ExtensionProcessor.settings.ResolveUnDefine = false;
                        }
                        Logger.Log(DebugLevel.LOGS, "Resolve Conditions flag set to " + ExtensionProcessor.settings.ResolveConditions, Verbosity.LEVEL1);
                    }
                }
                else if (args[i] == "-ri" || args[i] == "--resolveInclude")
                {
                    if (!bool.TryParse(args[i + 1], out ExtensionProcessor.settings.ResolveIncludes))
                    {
                        Logger.Log(DebugLevel.WARNINGS, "Resolve Includes flag needs to be either \"true\" or \"false\"", Verbosity.ALWAYS_SEND);
                    }
                    else
                    {
                        Logger.Log(DebugLevel.LOGS, "Resolve Includes flag set to " + ExtensionProcessor.settings.ResolveIncludes, Verbosity.LEVEL1);
                    }
                }
                else if (args[i] == "-rg" || args[i] == "--resolveGenerics")
                {
                    if (!bool.TryParse(args[i + 1], out ExtensionProcessor.settings.ResolveGenerics))
                    {
                        Logger.Log(DebugLevel.WARNINGS, "Resolve Generics flag needs to be either \"true\" or \"false\"", Verbosity.ALWAYS_SEND);
                    }
                    else
                    {
                        Logger.Log(DebugLevel.LOGS, "Resolve Generics flag set to " + ExtensionProcessor.settings.ResolveGenerics, Verbosity.LEVEL1);
                    }
                }
                else if (args[i] == "-ee" || args[i] == "--enableErrors")
                {
                    if (!bool.TryParse(args[i + 1], out ExtensionProcessor.settings.EnableErrors))
                    {
                        Logger.Log(DebugLevel.WARNINGS, "Enable Errors flag needs to be either \"true\" or \"false\"", Verbosity.ALWAYS_SEND);
                    }
                    else
                    {
                        Logger.Log(DebugLevel.LOGS, "Enable Errors flag set to " + ExtensionProcessor.settings.EnableErrors, Verbosity.LEVEL1);
                    }
                }
                else if (args[i] == "-ee" || args[i] == "--enableWarnings")
                {
                    if (!bool.TryParse(args[i + 1], out ExtensionProcessor.settings.EnableErrors))
                    {
                        Logger.Log(DebugLevel.WARNINGS, "Enable Warnings flag needs to be either \"true\" or \"false\"", Verbosity.ALWAYS_SEND);
                    }
                    else
                    {
                        Logger.Log(DebugLevel.LOGS, "Enable Warnings flag set to " + ExtensionProcessor.settings.EnableWarnings, Verbosity.LEVEL1);
                    }
                }
                else if (args[i] == "-ss" || args[i] == "--setSeparator")
                {
                    if (!char.TryParse(args[i + 1], out ExtensionProcessor.settings.Keywords.Separator))
                    {
                        Logger.Log(DebugLevel.WARNINGS, "Invalid Separator. Only one character.",
                            Verbosity.ALWAYS_SEND);
                    }
                    else
                    {
                        Logger.Log(DebugLevel.LOGS, "Separator " + ExtensionProcessor.settings.Keywords.Separator,
                            Verbosity.LEVEL1);
                    }
                }
                else if (args[i].StartsWith("-kw") || args[i].StartsWith("--keyWord"))
                {

                    int idx = args[i].IndexOf(':') + 1;
                    string prop = args[i].Substring(idx);
                    if (!KeyWordHandles.ContainsKey(prop))
                    {
                        Logger.Log(DebugLevel.WARNINGS, "Invalid Property Key.",
                            Verbosity.ALWAYS_SEND);
                    }
                    else
                    {
                        FieldInfo pi = KeyWordHandles[prop];
                        Logger.Log(DebugLevel.LOGS, "Property set: " + pi.Name + "=" + args[i + 1],
                            Verbosity.LEVEL1);
                        pi.SetValue(ExtensionProcessor.settings.Keywords, args[i + 1]);
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
            }
            #endregion

            #region CheckErrors

            if (input == "" || (output == "" && !ExtensionProcessor.settings.WriteToConsole))
            {
                Logger.Log(DebugLevel.ERRORS, "Invalid Command.", Verbosity.ALWAYS_SEND);
                Logger.Log(DebugLevel.LOGS, Settings.HelpText, Verbosity.ALWAYS_SEND);
                return;
            }

            #endregion

            Logger.Log(DebugLevel.LOGS, "Input file: " + input, Verbosity.ALWAYS_SEND);
            Logger.Log(DebugLevel.LOGS, "Output file: " + output, Verbosity.ALWAYS_SEND);


            string[] source = ExtensionProcessor.CompileFile(input);


            if (ExtensionProcessor.settings.WriteToConsole)
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

#if DEBUG
            Console.ReadLine();
#endif
        }



        static void InitADL()
        {
            
            CrashHandler.Initialize((int)DebugLevel.ERRORS, false);
            Debug.LoadConfig((AdlConfig)new AdlConfig().GetStandard());
            Debug.SetAllPrefixes("[ERRORS]", "[WARNINGS]", "[LOGS]");
            Debug.CheckForUpdates = false;
            Debug.AdlWarningMask = (int)DebugLevel.WARNINGS;
            LogTextStream lts = new LogTextStream(
                Console.OpenStandardOutput(),
                -1,
                MatchType.MatchAll,
                true);
            
            Debug.AddOutputStream(lts);

        }


    }
}
