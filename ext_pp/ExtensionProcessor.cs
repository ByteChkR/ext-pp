using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ext_compiler.extensions;
using ext_compiler.settings;

namespace ext_compiler
{
    public static class ExtensionProcessor
    {
        public static Settings settings = new Settings();

        #region Compile

        public static string[] CompileFile(string path, Dictionary<string, bool> globalTable = null)
        {

            List<SourceScript> ss = LoadSourceTree(path, globalTable);
            string[] ret = CompileTree(ss);

            Logger.Log(DebugLevel.LOGS, "Compilation DONE!", Verbosity.ALWAYS_SEND);
            return ret;
        }

        private static string[] CompileTree(List<SourceScript> tree)
        {
            List<string> ret = new List<string>();
            ProcessWarningsAndErrors(tree);


            for (int i = tree.Count - 1; i >= 0; i--)
            {
                Logger.Log(DebugLevel.LOGS, "Compiling File: " + tree[i].filepath, Verbosity.LEVEL1);

                ret.AddRange(tree[i].source);
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
                    Logger.Log(DebugLevel.WARNINGS, "Warning: (" + script.filepath + "): " + warnings[i].GetStatementValues().Unpack(), Verbosity.ALWAYS_SEND);
                }
            }

            List<string> errs = new List<string>();
            if (settings.EnableErrors)
            {
                errs = script.FindStatements(settings.Keywords.ErrorStatement).ToList();
                for (int i = 0; i < errs.Count; i++)
                {
                    Logger.Log(DebugLevel.ERRORS, "Error: (" + script.filepath + "): " + errs[i].GetStatementValues().Unpack(), Verbosity.ALWAYS_SEND);
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

        


        public static List<SourceScript> LoadSourceTree(string file, Dictionary<string, bool> globalTable=null)
        {
            file = Path.GetFullPath(file);
            List<SourceScript> ret = new List<SourceScript>();
            Logger.Log(DebugLevel.LOGS, "Creating Source Dependency Tree..", Verbosity.LEVEL1);
            LoadSourceTree(file, ret, globalTable);
            return ret;
        }
        private static void LoadSourceTree(string file, List<SourceScript> sources, Dictionary<string, bool> globalTable, string[] genParams = null)
        {
            SourceScript ss;

            if (genParams == null)
            {
                Logger.Log(DebugLevel.LOGS,"Preparing Compilation of " + file,Verbosity.LEVEL2 );
                ss = new SourceScript(file, new string[0]);
                LoadSourceTree(ss, sources, globalTable);
            }
            else
            {
                Logger.Log(DebugLevel.LOGS, "Preparing Compilation of " + file + " with generic types", Verbosity.LEVEL2);
                ss = new SourceScript(file, genParams);
                LoadSourceTree(ss, sources, globalTable);
            }

        }

        private static void LoadSourceTree(SourceScript script, List<SourceScript> sources, Dictionary<string, bool> globalTable)
        {
            if (!sources.ContainsFile(script.key))
            {
                if (globalTable == null) globalTable = new Dictionary<string, bool>();
                script.Load();
                if (settings.ResolveConditions) ConditionalResolver.ResolveConditions(script, globalTable);
                if (settings.ResolveGenerics) script.ResolveGenericParams();
                sources.Add(script, true);
                if (settings.ResolveIncludes)
                {
                    script.DiscoverIncludes();
                    for (int i = 0; i < script.includes.Length; i++)
                    {
                        Logger.Log(DebugLevel.LOGS, "processing include statement: " + script.includes[i], Verbosity.LEVEL3);
                        string f = script.GetSourceFileFromIncludeStatement(script.includes[i], Path.GetDirectoryName(script.filepath), out string[] gparams);

                        LoadSourceTree(f, sources, globalTable, gparams);
                    }
                }
            }
            else
            {
                int idx = sources.IndexOf(script.key);
                SourceScript a = sources[idx];
                sources.RemoveAt(idx);
                sources.Add(a);

                Logger.Log(DebugLevel.LOGS, "Fixing Source Order", Verbosity.LEVEL3);

            }
        }

        #endregion

    }
}