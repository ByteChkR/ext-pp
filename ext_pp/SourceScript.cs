using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using ext_compiler.extensions;
using ext_compiler.settings;

namespace ext_compiler
{
    public class SourceScript
    {

        public string filepath;
        public string[] source = new string[0];
        public string[] genParam;
        public string[] includes;
        public string key => filepath + GenParamAppendix;


        public string GenParamAppendix
        {
            get
            {
                string gp = genParam.Unpack();
                if (genParam != null && genParam.Length > 0) gp = "." + gp;
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
            this.genParam = genParams;
            filepath = path;
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
            includes = FindStatements(ExtensionProcessor.settings.Keywords.IncludeStatement);
        }


        public void ResolveGenericParams()
        {
            if (genParam != null)
            {
                Logger.Log(DebugLevel.LOGS, "Resolving Generic Parameters", Verbosity.LEVEL2);
                ResolveGenerics();
            }
        }

        private bool LoadSource()
        {

            source = new string[0];
            if (File.Exists(filepath))
            {
                Logger.Log(DebugLevel.LOGS, "Loading File: " + filepath, Verbosity.LEVEL3);
                source = File.ReadAllLines(filepath);


                return true;
            }
            return false;
        }


        private void ResolveGenerics()
        {
            for (int i = genParam.Length - 1; i >= 0; i--)
            {
                ReplaceKeyWord(genParam[i],
                    ExtensionProcessor.settings.Keywords.TypeGenKeyword + i);
            }

        }

        public void ReplaceKeyWord(string replacement, string keyword)
        {

            for (int i = 0; i < source.Length; i++)
            {
                if (source[i].Contains(keyword))
                {
                    source[i] = source[i].Replace(keyword, replacement);
                }
            }



        }


        public void RemoveStatementLines(string statement)
        {
            List<string> ret = source.ToList();
            for (int i = ret.Count - 1; i >= 0; i--)
            {
                if (ret[i].Trim().StartsWith(statement))
                {
                    ret.RemoveAt(i);
                }
            }

            source = ret.ToArray();
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
            return source.ToList().Where(x => x.Trim().StartsWith(statement)).ToArray();
        }
    }
}