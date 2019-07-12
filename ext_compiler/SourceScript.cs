using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using ext_compiler.extensions;

namespace ext_compiler
{
    public class SourceScript
    {
        public static string IncludeStatement = "#include";
        public static string TypeGenKeyword = "#type";
        public static string ErrorStatement = "#error";
        public static string WarningStatement = "#warning";
        public static string IfStatement = "#if";
        public static string ElseIfStatement = "#elseif";
        public static string ElseStatement = "#else";
        public static string EndIfStatement = "#endif";
        public static string DefineStatement = "#define";
        public static string UndefineStatement = "#undefine";




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
        }

        public void DiscoverIncludes()
        {
            Console.WriteLine("Discovering Include Statements.");
            includes = FindStatements(SourceScript.IncludeStatement);
        }

        public void ResolveGenericParams()
        {
            if (genParam != null)
            {
                Console.WriteLine("Resolving Generic Parameters");
                ResolveGenerics(genParam, SourceScript.TypeGenKeyword);
            }
        }

       

        public static string[] GetValues(string statement)
        {
            string[] ret = statement.Split(' ');
            return ret.SubArray(1, ret.Length - 1);

        }

        private bool LoadSource(string path, string typeGenKeyword, string[] genParams = null)
        {

            source = new string[0];
            if (File.Exists(path))
            {
                Console.WriteLine("Loading File: " + path);
                source = File.ReadAllLines(path);


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


        public static List<string> RemoveStatements(List<string> source, string[] statements)
        {

            Console.WriteLine("Removing Leftover Statements");
            for (int i = source.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < statements.Length; j++)
                {
                    if (source[i].Trim().StartsWith(statements[j])) source.RemoveAt(i);
                }
            }
            return source;
        }

        public string GetSourceFileFromIncludeStatement(string statement, string dir, out string[] genParams)
        {
            string file = statement.Trim().Remove(0, IncludeStatement.Length).Trim();
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
            return source.ToList().Where(x => x.Trim().StartsWith(statement)).ToArray();
        }
    }
}