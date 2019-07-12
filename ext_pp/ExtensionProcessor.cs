using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ext_pp.settings;

namespace ext_pp
{
    public static class ExtensionProcessor
    {
        public static Settings settings = new Settings();

        #region Compile

        public static string[] CompileFile(string path, Dictionary<string, bool> globalTable = null)
        {

            bool sucess = LoadSourceTree(path, out var ss, globalTable);
            string[] ret = CompileTree(ss);
            Logger.Log(DebugLevel.LOGS, sucess ? "Compilation DONE!" : "One or more errors in compilation",
                Verbosity.ALWAYS_SEND);

            return ret;
        }

        private static string[] CompileTree(List<SourceScript> tree)
        {
            List<string> ret = new List<string>();
            ProcessWarningsAndErrors(tree);


            for (int i = tree.Count - 1; i >= 0; i--)
            {
                Logger.Log(DebugLevel.LOGS, "Compiling File: " + tree[i].Filepath, Verbosity.LEVEL1);

                ret.AddRange(tree[i].Source);
            }

            return SourceScript.RemoveStatements(ret, settings.CleanUpList.ToArray()).ToArray();
        }

        #endregion

        #region Warnings and Errors

        private static void ProcessWarningsAndErrors(List<SourceScript> tree)
        {
            Logger.Log(DebugLevel.LOGS, "Processing Warnings and Errors", Verbosity.LEVEL1);

            if (!settings.EnableErrors && !settings.EnableWarnings)
                return;
            foreach (var sourceScript in tree)
            {
                ProcessWarningsAndErrors(sourceScript);
            }
        }

        private static void ProcessWarningsAndErrors(SourceScript script)
        {
            List<string> warnings = new List<string>();
            if (settings.EnableWarnings)
            {
                warnings = script.FindStatements(settings.Keywords.WarningStatement).ToList();
                for (int i = 0; i < warnings.Count; i++)
                {
                    Logger.Log(DebugLevel.WARNINGS, "Warning: (" + script.Filepath + "): " + warnings[i].GetStatementValues().Unpack(), Verbosity.ALWAYS_SEND);
                }
            }

            List<string> errs = new List<string>();
            if (settings.EnableErrors)
            {
                errs = script.FindStatements(settings.Keywords.ErrorStatement).ToList();
                for (int i = 0; i < errs.Count; i++)
                {
                    Logger.Log(DebugLevel.ERRORS, "Error: (" + script.Filepath + "): " + errs[i].GetStatementValues().Unpack(), Verbosity.ALWAYS_SEND);
                }
            }

            if (errs.Count != 0)
            {
                Exception e = new Exception("One or more errors in source code.");
                e.Data.Add("warnings", warnings);
                e.Data.Add("errors", errs);

                Logger.Crash(e);
            }
        }


        #endregion

        #region Loading and Processing




        public static bool LoadSourceTree(string file, out List<SourceScript> tree, Dictionary<string, bool> globalTable = null)
        {
            if (!File.Exists(file))
            {
                Logger.Crash(new FileNotFoundException(file + "not found"));
            }

            file = Path.GetFullPath(file);
            tree = new List<SourceScript>();
            Logger.Log(DebugLevel.LOGS, "Creating Source Dependency Tree..", Verbosity.LEVEL1);

            return LoadSourceTree(file, tree, globalTable);
        }
        private static bool LoadSourceTree(string file, List<SourceScript> sources, Dictionary<string, bool> globalTable, string[] genParams = null)
        {
            SourceScript ss;

            if (genParams == null)
            {
                Logger.Log(DebugLevel.LOGS, "Preparing Compilation of " + file, Verbosity.LEVEL2);
                ss = new SourceScript(file, new string[0]);
                return LoadSourceTree(ss, sources, globalTable);
            }
            else
            {
                Logger.Log(DebugLevel.LOGS, "Preparing Compilation of " + file + " with generic types", Verbosity.LEVEL2);
                ss = new SourceScript(file, genParams);
                return LoadSourceTree(ss, sources, globalTable);
            }

        }

        private static bool LoadSourceTree(SourceScript script, List<SourceScript> sources, Dictionary<string, bool> globalTable)
        {
            bool sucess = true;
            if (!sources.ContainsFile(script.Key))
            {
                if (globalTable == null) globalTable = new Dictionary<string, bool>();
                script.Load();
                if (settings.ResolveConditions) sucess &= ConditionalResolver.ResolveConditions(script, globalTable);
                if (settings.ResolveGenerics) script.ResolveGenericParams();
                sources.AddFile(script, true);
                if (settings.ResolveIncludes)
                {
                    script.DiscoverIncludes();
                    for (int i = 0; i < script.Includes.Length; i++)
                    {
                        Logger.Log(DebugLevel.LOGS, "processing include statement: " + script.Includes[i], Verbosity.LEVEL3);
                        string f = script.GetSourceFileFromIncludeStatement(script.Includes[i], Path.GetDirectoryName(script.Filepath), out string[] gparams);

                        sucess &= LoadSourceTree(f, sources, globalTable, gparams);
                    }
                }
            }
            else
            {
                int idx = sources.IndexOfFile(script.Key);
                SourceScript a = sources[idx];
                sources.RemoveAt(idx);
                sources.AddFile(a, true);

                Logger.Log(DebugLevel.LOGS, "Fixing Source Order", Verbosity.LEVEL3);

            }

            return sucess;
        }

        #endregion

    }
}