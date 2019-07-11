using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ext_compiler.extensions;

namespace ext_compiler
{
    public static class ExtensionCompiler
    {
        public static string IncludeStatement = "#include";
        public static string TypeGenKeyword = "#type";


        public static string[] CompileFile(string path)
        {
            path = Path.GetFullPath(path);
            string[] ret = CompileTree(LoadSourceTree(path));

            Console.WriteLine("Compilation DONE!");
            return ret;
        }

        private static string[] CompileTree(List<SourceScript> tree)
        {
            List<string> ret = new List<string>();
            for (int i = tree.Count - 1; i >= 0; i--)
            {
                Console.WriteLine("Compiling File: " + tree[i].filepath);
                ret.AddRange(tree[i].source);
            }

            return RemoveIncludeStatements(ret).ToArray();
        }


        private static List<string> RemoveIncludeStatements(List<string> source)
        {

            Console.WriteLine("Post Processing Include Statements..");
            for (int i = source.Count - 1; i >= 0; i--)
            {
                if (source[i].StartsWith(IncludeStatement)) source.RemoveAt(i);
            }
            return source;
        }

        private static List<SourceScript> LoadSourceTree(string file)
        {
            List<SourceScript> ret = new List<SourceScript>();
            Console.WriteLine("Creating Source Dependency Tree..");
            return LoadSourceTree(file, ret);
        }
        private static List<SourceScript> LoadSourceTree(string file, List<SourceScript> sources, string[] genParams = null)
        {

            if (!LoadSource(file, out string[] source, genParams))
            {
                throw new Exception("Could not load source file");
            }

            if (genParams == null)
            {
                Console.WriteLine("Preparing Compilation of " + file);

                LoadSourceTree(file, "", source, sources);
            }
            else
                foreach (string genParam in genParams)
                {
                    Console.WriteLine("Preparing Compilation of " + file + " with generic type: " + genParam);
                    LoadSourceTree(file, genParam, source, sources);
                }

            return sources;
        }

        private static List<SourceScript> LoadSourceTree(string file, string genParam, string[] source, List<SourceScript> sources)
        {
            if (genParam != "") genParam = "." + genParam;
            if (!sources.ContainsFile(file + genParam))
                sources.Add(new SourceScript(file + genParam, source), true);
            else
            {
                int idx = sources.IndexOf(file + genParam);
                SourceScript a = sources[idx];
                sources.RemoveAt(idx);
                sources.Add(a);

                Console.WriteLine("Fixing Source Order");

                return sources;
            }
            List<string> todo = FindStatements(IncludeStatement, source).ToList();
            for (int i = 0; i < todo.Count; i++)
            {

                string f = GetSourceFileFromIncludeStatement(todo[i], Path.GetDirectoryName(file), out string[] gparams);

                LoadSourceTree(f, sources, gparams);

            }

            return sources;
        }

        private static string[] ResolveGenericIncludes(string[] gparam, string[] subgparams)
        {
            for (int i = 0; i < subgparams.Length; i++)
            {
                if (subgparams[i].StartsWith(TypeGenKeyword))
                {
                    string prm = subgparams[i].Trim();
                    int gennr = int.Parse(prm.Substring(TypeGenKeyword.Length + 1, prm.Length - TypeGenKeyword.Length));
                    subgparams[i] = gparam[gennr];
                }
            }

            return subgparams;
        }

        private static string GetSourceFileFromIncludeStatement(string statement, string dir, out string[] genParams)
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

        private static string[] FindStatements(string statement, string[] source)
        {
            return source.ToList().Where(x => x.StartsWith(statement)).ToArray();
        }

        private static string[] ReplaceKeyWord(string replacement, string keyword, string[] source)
        {

            for (int i = 0; i < source.Length; i++)
            {
                if (source[i].Contains(keyword))
                {
                    source[i] = source[i].Replace(keyword, replacement);
                }
            }

            return source;
        }

        private static bool LoadSource(string path, out string[] source, string[] genParams = null)
        {

            source = new string[0];
            if (File.Exists(path))
            {
                Console.WriteLine("Loading File: " + path);
                source = File.ReadAllLines(path);
                if (genParams != null)
                {
                    source = ResolveGenerics(genParams, source);
                }

                return true;
            }
            return false;
        }

        private static string[] ResolveGenerics(string[] genTypes, string[] source)
        {
            Console.WriteLine("Resolving Generics...");
            for (int i = genTypes.Length - 1; i >= 0; i--)
            {
                source = ReplaceKeyWord(genTypes[i], TypeGenKeyword + i, source);
            }

            return source;
        }

    }
}