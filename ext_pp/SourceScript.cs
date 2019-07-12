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
                if (subgparams[i].StartsWith(ExtensionProcessor.settings.Keywords.IncludeStatement))
                {
                    string prm = subgparams[i].Trim();
                    int gennr = Int32.Parse(prm.Substring(ExtensionProcessor.settings.Keywords.TypeGenKeyword.Length + 1, prm.Length - ExtensionProcessor.settings.Keywords.TypeGenKeyword.Length));
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
            Includes = FindStatements(ExtensionProcessor.settings.Keywords.IncludeStatement);
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
                    ExtensionProcessor.settings.Keywords.TypeGenKeyword + i);
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
            string file = statement.Trim().Remove(0, ExtensionProcessor.settings.Keywords.IncludeStatement.Length).Trim();
            int idx = file.IndexOf(ExtensionProcessor.settings.Keywords.Separator);
            genParams = null;
            if (idx != -1)
            {
                genParams =
                    file.Substring(idx + 1, file.Length - idx - 1)
                        .Split(ExtensionProcessor.settings.Keywords.Separator)
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
    }
}