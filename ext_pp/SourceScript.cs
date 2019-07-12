using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using ext_pp.settings;

namespace ext_pp
{
    public class SourceScript
    {

        public string Filepath;
        public string[] Source = new string[0];
        private readonly string[] _genParam;
        public string[] Includes;
        public string Key => Filepath + GenParamAppendix;


        public string GenParamAppendix
        {
            get
            {
                string gp = _genParam.Unpack();
                if (_genParam != null && _genParam.Length > 0) gp = "." + gp;
                return gp;
            }
        }

        #region Static

        private static string[] ResolveGenericIncludes(string[] gparam, string[] subgparams)
        {
            for (int i = 0; i < subgparams.Length; i++)
            {
                if (subgparams[i].StartsWith(Settings.IncludeStatement))
                {
                    string prm = subgparams[i].Trim();
                    int gennr = Int32.Parse(prm.Substring(Settings.TypeGenKeyword.Length + 1, prm.Length - Settings.TypeGenKeyword.Length));
                    subgparams[i] = gparam[gennr];
                }
            }

            return subgparams;
        }
        public static List<string> RemoveStatements(List<string> source, string[] statements)
        {

            Logger.Log(DebugLevel.LOGS, "Removing Leftover Statements", Verbosity.LEVEL2);
            for (int i = source.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < statements.Length; j++)
                {
                    if (source[i].Trim().StartsWith(statements[j])) source.RemoveAt(i);
                }
            }
            return source;
        }
        #endregion


        public SourceScript(string path, string[] genParams)
        {
            this._genParam = genParams;
            Filepath = path;
        }


        public void Load()
        {
            if (!LoadSource())
            {
                Logger.Crash(new Exception("Could not load Source file"));
            }
        }

        public void DiscoverIncludes()
        {
            Logger.Log(DebugLevel.LOGS, "Discovering Include Statements.", Verbosity.LEVEL2);
            Includes = FindStatements(Settings.IncludeStatement);
        }


        public void ResolveGenericParams()
        {
            if (_genParam != null)
            {
                Logger.Log(DebugLevel.LOGS, "Resolving Generic Parameters", Verbosity.LEVEL2);
                ResolveGenerics();
            }
        }

        private bool LoadSource()
        {

            Source = new string[0];
            if (File.Exists(Filepath))
            {
                Logger.Log(DebugLevel.LOGS, "Loading File: " + Filepath, Verbosity.LEVEL3);
                Source = File.ReadAllLines(Filepath);


                return true;
            }
            return false;
        }


        private void ResolveGenerics()
        {
            for (int i = _genParam.Length - 1; i >= 0; i--)
            {
                ReplaceKeyWord(_genParam[i],
                    Settings.TypeGenKeyword + i);
            }

        }

        public void ReplaceKeyWord(string replacement, string keyword)
        {

            for (int i = 0; i < Source.Length; i++)
            {
                if (Source[i].Contains(keyword))
                {
                    Source[i] = Source[i].Replace(keyword, replacement);
                }
            }



        }


        public void RemoveStatementLines(string statement)
        {
            List<string> ret = Source.ToList();
            for (int i = ret.Count - 1; i >= 0; i--)
            {
                if (ret[i].Trim().StartsWith(statement))
                {
                    ret.RemoveAt(i);
                }
            }

            Source = ret.ToArray();
        }




        public string GetSourceFileFromIncludeStatement(string statement, string dir, out string[] genParams)
        {
            string file = statement.Trim().Remove(0, Settings.IncludeStatement.Length).Trim();
            int idx = file.IndexOf(Settings.Separator);
            genParams = null;
            if (idx != -1)
            {
                genParams =
                    file.Substring(idx + 1, file.Length - idx - 1)
                        .Split(Settings.Separator)
                        .Select(x => x.Trim()).ToArray();
                file = file.Substring(0, idx);

            }

            string p = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(dir);
            if (!File.Exists(file))
            {
                Logger.Crash(new Exception("Could not find include file " + file));
            }
            else
            {
                file = Path.GetFullPath(file);
            }
            Directory.SetCurrentDirectory(p);
            return file;
        }

        public string[] FindStatements(string statement)
        {
            return Source.ToList().Where(x => x.Trim().StartsWith(statement)).ToArray();
        }



        #region Warnings and Errors

        private static void ProcessWarningsAndErrors(List<SourceScript> tree)
        {
            Logger.Log(DebugLevel.LOGS, "Processing Warnings and Errors", Verbosity.LEVEL1);

            if (!Settings.EnableErrors && !Settings.EnableWarnings)
                return;
            foreach (var sourceScript in tree)
            {
                ProcessWarningsAndErrors(sourceScript);
            }
        }

        private static void ProcessWarningsAndErrors(SourceScript script)
        {
            List<string> warnings = new List<string>();
            if (Settings.EnableWarnings)
            {
                warnings = script.FindStatements(Settings.WarningStatement).ToList();
                for (int i = 0; i < warnings.Count; i++)
                {
                    Logger.Log(DebugLevel.WARNINGS, "Warning: (" + script.Filepath + "): " + warnings[i].GetStatementValues().Unpack(), Verbosity.ALWAYS_SEND);
                }
            }

            List<string> errs = new List<string>();
            if (Settings.EnableErrors)
            {
                errs = script.FindStatements(Settings.ErrorStatement).ToList();
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
            bool sucess = LoadSourceTree(file, tree, globalTable);
            ProcessWarningsAndErrors(tree);
            return sucess;
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
                if (Settings.ResolveConditions) sucess &= ConditionalResolver.ResolveConditions(script, globalTable);
                if (Settings.ResolveGenerics) script.ResolveGenericParams();
                sources.AddFile(script, true);
                if (Settings.ResolveIncludes)
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

