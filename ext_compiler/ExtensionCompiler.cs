using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ext_compiler.extensions;

namespace ext_compiler
{
    public static class ExtensionCompiler
    {
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

            return SourceScript.RemoveIncludeStatements(ret).ToArray();
        }




        private static List<SourceScript> LoadSourceTree(string file)
        {
            List<SourceScript> ret = new List<SourceScript>();
            Console.WriteLine("Creating Source Dependency Tree..");
            return LoadSourceTree(file, ret);
        }
        private static List<SourceScript> LoadSourceTree(string file, List<SourceScript> sources, string[] genParams = null)
        {
            SourceScript ss;

            if (genParams == null)
            {
                Console.WriteLine("Preparing Compilation of " + file);
                ss = new SourceScript(file, new string[0]);
                LoadSourceTree(ss, sources);
            }
            else
            {


                Console.WriteLine("Preparing Compilation of " + file + " with generic types");
                ss = new SourceScript(file, genParams);
                LoadSourceTree(ss, sources);
            }

            return sources;
        }

        private static void LoadSourceTree(SourceScript script, List<SourceScript> sources)
        {
            if (!sources.ContainsFile(script.key))
            {
                script.Load();
                sources.Add(script, true);
                for (int i = 0; i < script.includes.Length; i++)
                {

                    string f = script.GetSourceFileFromIncludeStatement(script.includes[i], Path.GetDirectoryName(script.filepath), out string[] gparams);

                    LoadSourceTree(f, sources, gparams);

                }
            }
            else
            {
                int idx = sources.IndexOf(script.key);
                SourceScript a = sources[idx];
                sources.RemoveAt(idx);
                sources.Add(a);

                Console.WriteLine("Fixing Source Order");

            }
        }


    }
}