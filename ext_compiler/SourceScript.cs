using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ext_compiler.extensions;

namespace ext_compiler
{
    public class SourceScript
    {
        public static string IncludeStatement = "#include";
        public static string TypeGenKeyword = "#type";
        //public static string ErrorStatement = "#error";
        //public static string WarningStatement = "#warning";

        public string key
        {
            get { return filepath + GenParamAppendix; }
        }
        public string filepath;
        public string[] source = new string[0];
        public string[] genParam;

        public string GenParamAppendix
        {
            get
            {
                string gp = genParam.Unpack();
                if (genParam != null && genParam.Length > 0) gp = "." + gp;
                return gp;
            }
        }
        public string[] includes;
        public SourceScript(string path, string[] genParams)
        {
            this.genParam = genParams;
            filepath = path;
        }

        public void Load()
        {
            if (!LoadSource(filepath, TypeGenKeyword, genParam))
            {
                throw new Exception("Could not load Source file");
            }

            Console.WriteLine("Discovering Include Statements.");
            includes = FindStatements(SourceScript.IncludeStatement);


        }



        private bool LoadSource(string path, string typeGenKeyword, string[] genParams = null)
        {

            source = new string[0];
            if (File.Exists(path))
            {
                Console.WriteLine("Loading File: " + path);
                source = File.ReadAllLines(path);
                if (genParams != null)
                {
                    ResolveGenerics(genParams, typeGenKeyword);
                }

                return true;
            }
            return false;
        }


        private void ResolveGenerics(string[] genTypes, string typeGenKeyword)
        {
            Console.WriteLine("Resolving Generics...");
            for (int i = genTypes.Length - 1; i >= 0; i--)
            {
                ReplaceKeyWord(genTypes[i], typeGenKeyword + i);
            }

        }

        private void ReplaceKeyWord(string replacement, string keyword)
        {

            for (int i = 0; i < source.Length; i++)
            {
                if (source[i].Contains(keyword))
                {
                    source[i] = source[i].Replace(keyword, replacement);
                }
            }



        }
        private static string[] ResolveGenericIncludes(string[] gparam, string[] subgparams)
        {
            for (int i = 0; i < subgparams.Length; i++)
            {
                if (subgparams[i].StartsWith(TypeGenKeyword))
                {
                    string prm = subgparams[i].Trim();
                    int gennr = Int32.Parse(prm.Substring(TypeGenKeyword.Length + 1, prm.Length - TypeGenKeyword.Length));
                    subgparams[i] = gparam[gennr];
                }
            }

            return subgparams;
        }

        public static List<string> RemoveIncludeStatements(List<string> source)
        {

            Console.WriteLine("Post Processing Include Statements..");
            for (int i = source.Count - 1; i >= 0; i--)
            {
                if (source[i].StartsWith(SourceScript.IncludeStatement)) source.RemoveAt(i);
            }
            return source;
        }

        public string GetSourceFileFromIncludeStatement(string statement, string dir, out string[] genParams)
        {
            string file = statement.Remove(0, IncludeStatement.Length).Trim();
            int idx = file.IndexOf(' ');
            genParams = null;
            if (idx != -1)
            {
                genParams =
                    file.Substring(idx + 1, file.Length - idx - 1)
                        .Split(',').Select(x => x.Trim()).ToArray();
                file = file.Substring(0, idx);

            }

            string p = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(dir);
            if (!File.Exists(file))
            {
                throw new Exception("Could not find include file");
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
            return source.ToList().Where(x => x.StartsWith(statement)).ToArray();
        }
    }
}