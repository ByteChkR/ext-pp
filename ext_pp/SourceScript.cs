using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ext_pp.settings;

namespace ext_pp
{
    public class SourceScript
    {

        public readonly string Filepath;
        private readonly string[] _genParam;

        public string[] Source = new string[0];
        public string Key => Filepath + GenParamAppendix;


        private string[] _includes;


        private string GenParamAppendix
        {
            get
            {
                var gp = _genParam.Unpack();
                if (_genParam != null && _genParam.Length > 0) gp = "." + gp;
                return gp;
            }
        }

        #region Static

        //private static string[] ResolveGenericIncludes(string[] gparam, string[] subgparams)
        //{
        //    for (var i = 0; i < subgparams.Length; i++)
        //    {
        //        if (!subgparams[i].StartsWith(Settings.IncludeStatement)) continue;
        //        var prm = subgparams[i].Trim();
        //        var gennr = int.Parse(prm.Substring(Settings.TypeGenKeyword.Length + 1, prm.Length - Settings.TypeGenKeyword.Length));
        //        subgparams[i] = gparam[gennr];
        //    }

        //    return subgparams;
        //}

        #endregion


        public SourceScript(string path, string[] genParams)
        {
            _genParam = genParams;
            Filepath = path;
        }




        private bool ResolveConditionals(Dictionary<string, bool> glob)
        {
            Logger.Log(DebugLevel.LOGS, "Resolving conditions in file: " + Key, Verbosity.LEVEL2);

            return ConditionalResolver.ResolveConditions(this, glob);
        }

        private void DiscoverIncludes()
        {
            Logger.Log(DebugLevel.LOGS, "Discovering Include Statements.", Verbosity.LEVEL2);
            _includes = Utils.FindStatements(Source, Settings.IncludeStatement);
        }


        private bool Load()
        {

            bool ret;
            if (!(ret = LoadSource()))
            {
                Logger.Crash(new Exception("Could not load Source file"));
            }

            return ret;
        }

        private bool LoadSource()
        {

            Source = new string[0];
            if (!File.Exists(Filepath)) return false;
            Logger.Log(DebugLevel.LOGS, "Loading File: " + Filepath, Verbosity.LEVEL3);
            Source = File.ReadAllLines(Filepath);


            return true;
        }


        private void ResolveGenerics()
        {
            if (_genParam != null && _genParam.Length > 0)
            {
                Logger.Log(DebugLevel.LOGS, "Resolving Generic Parameters", Verbosity.LEVEL2);

                for (var i = _genParam.Length - 1; i >= 0; i--)
                {
                    Utils.ReplaceKeyWord(Source, _genParam[i],
                        Settings.TypeGenKeyword + i);
                }
            }
            else
            {
                Logger.Log(DebugLevel.LOGS, "No Generic Parameters found", Verbosity.LEVEL2);

            }

        }

        private string GetIncludeSourcePath(string statement, out string[] genParams)
        {
            var vars = statement.GetStatementValues();
            genParams = null;
            string file = "";
            if (vars.Length != 0)
            {
                file = vars[0];
                genParams = vars.Length > 1 ?
                    vars.SubArray(1, vars.Length - 1).ToArray() : null;

                if (!Utils.FileExists(Filepath, file))
                    Logger.Crash(new Exception("Could not find include file " + file));
                else file = Path.GetFullPath(file);

                return file;
            }


            Logger.Crash(new Exception("Empty Include Statement Found"), false);
            return "";
        }

        

        



        #region Warnings and Errors


        public void ProcessWarningsAndErrors()
        {
            var warnings = new List<string>();
            if (Settings.EnableWarnings)
            {
                warnings = Utils.FindStatements(Source, Settings.WarningStatement).ToList();
                foreach (var t in warnings)
                {
                    Logger.Log(DebugLevel.WARNINGS, "Warning: (" + Filepath + "): " + t.GetStatementValues().Unpack(), Verbosity.ALWAYS_SEND);
                }
            }

            var errs = new List<string>();
            if (Settings.EnableErrors)
            {
                errs = Utils.FindStatements(Source, Settings.ErrorStatement).ToList();

                foreach (var t in errs)
                {
                    Logger.Log(DebugLevel.ERRORS, "Error: (" + Filepath + "): " + t.GetStatementValues().Unpack(), Verbosity.ALWAYS_SEND);
                }
            }

            if (errs.Count != 0)
            {
                var e = new Exception("One or more errors in source code.");
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
            var success = LoadSourceTree(file, tree, null, globalTable);

            return success;
        }



        private static bool LoadSourceTree(string file, List<SourceScript> sources, string[] genParams, Dictionary<string, bool> globalTable)
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
            var success = true;
            if (!sources.ContainsFile(script.Key))
            {
                if (globalTable == null) globalTable = new Dictionary<string, bool>();
                success &= script.Load();
                if (Settings.ResolveConditions)
                {
                    Logger.Log(DebugLevel.LOGS, "Resolving conditions in file: " + script.Key, Verbosity.LEVEL2);
                    success &= script.ResolveConditionals(globalTable);

                }
                if (Settings.ResolveGenerics) script.ResolveGenerics();
                sources.AddFile(script, true);
                script.ProcessWarningsAndErrors();
                if (!Settings.ResolveIncludes) return success;
                script.DiscoverIncludes();
                foreach (var t in script._includes)
                {
                    Logger.Log(DebugLevel.LOGS, "processing include statement: " + t, Verbosity.LEVEL3);
                    var f = script.GetIncludeSourcePath(t, out var gparams);

                    success &= LoadSourceTree(f, sources, gparams, globalTable);
                }
            }
            else
            {
                var idx = sources.IndexOfFile(script.Key);
                var a = sources[idx];
                sources.RemoveAt(idx);
                sources.AddFile(a, true);
                Logger.Log(DebugLevel.LOGS, "Fixing Source Order", Verbosity.LEVEL3);
            }
            return success;
        }

        #endregion
    }
}

