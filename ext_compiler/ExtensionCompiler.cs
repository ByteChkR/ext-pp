using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ext_compiler.extensions;

namespace ext_compiler
{
    public static class ExtensionCompiler
    {
        private static Dictionary<string, bool> defs = new Dictionary<string, bool>();
        public static string[] CompileFile(string path)
        {

            string[] ret = CompileTree(LoadSourceTree(path));

            Console.WriteLine("Compilation DONE!");
            return ret;
        }

        private static string[] CompileTree(List<SourceScript> tree)
        {
            List<string> ret = new List<string>();
            ProcessWarningsAndErrors(tree);


            for (int i = tree.Count - 1; i >= 0; i--)
            {
                Console.WriteLine("Compiling File: " + tree[i].filepath);

                ret.AddRange(tree[i].source);
            }

            return SourceScript.RemoveStatements(ret, new[] { SourceScript.WarningStatement, SourceScript.ErrorStatement, SourceScript.DefineStatement, SourceScript.UndefineStatement, SourceScript.IncludeStatement }).ToArray();
        }



        private static void ProcessWarningsAndErrors(List<SourceScript> tree)
        {
            foreach (var sourceScript in tree)
            {
                ProcessWarningsAndErrors(sourceScript);
            }
        }

        private static void ProcessWarningsAndErrors(SourceScript script)
        {
            Console.WriteLine("Processing Warnings and Errors");
            List<string> warnings = script.FindStatements(SourceScript.WarningStatement).ToList();
            for (int i = 0; i < warnings.Count; i++)
            {
                Console.WriteLine("Warning(" + script.filepath + "): " + SourceScript.GetValues(warnings[i]).Unpack(' '));
            }
            List<string> errs = script.FindStatements(SourceScript.ErrorStatement).ToList();
            for (int i = 0; i < errs.Count; i++)
            {
                Console.WriteLine("Error: (" + script.filepath + "): " + SourceScript.GetValues(errs[i]).Unpack(' '));
            }

            if (errs.Count != 0)
            {
                Exception e = new Exception("One or more errors in source code.");
                e.Data.Add("warnings", warnings);
                e.Data.Add("errors", errs);

                throw e;
            }
        }


        public static List<SourceScript> LoadSourceTree(string file)
        {
            file = Path.GetFullPath(file);
            List<SourceScript> ret = new List<SourceScript>();
            Console.WriteLine("Creating Source Dependency Tree..");
            LoadSourceTree(file, ret);
            return ret;
        }
        private static void LoadSourceTree(string file, List<SourceScript> sources, string[] genParams = null)
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

        }

        private static void LoadSourceTree(SourceScript script, List<SourceScript> sources)
        {
            if (!sources.ContainsFile(script.key))
            {
                script.Load();
                ConditionalResolver.ResolveConditions(script, new Dictionary<string, bool>());
                script.ResolveGenericParams();
                script.DiscoverIncludes();
                sources.Add(script, true);
                for (int i = 0; i < script.includes.Length; i++)
                {
                    Console.WriteLine("processing include statement: " + script.includes[i]);
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