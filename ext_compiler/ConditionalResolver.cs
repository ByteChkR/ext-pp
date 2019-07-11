using System;
using System.Collections.Generic;
using System.Linq;
using ext_compiler.extensions;

namespace ext_compiler
{
    public static class ConditionalResolver
    {

        public static void ResolveConditions(List<SourceScript> tree, Dictionary<string, bool> globalTable = null)
        {
            Console.WriteLine("Resolving Conditions in file tree");
            if (globalTable == null)
                globalTable = new Dictionary<string, bool>();

            for (int i = 0; i < tree.Count; i++)
            {
                Console.WriteLine("Resolving Conditions in file " + tree[i].filepath);
                ResolveConditions(tree[i], globalTable, 1);
            }

        }

        private static void ResolveConditions(SourceScript script, Dictionary<string, bool> currentGlobal, int pass)
        {
            Console.WriteLine("Pass: " + pass);
            List<string> l = new List<string>();
            int openIf = 0;
            bool foundConditions = false;
            bool elseisValid = false;
            for (int i = 0; i < script.source.Length; i++)
            {
                string line = script.source[i].Trim();
                if (line.StartsWith(SourceScript.IfStatement))
                {
                    int size = GetBlockSize(script.source, i);
                    if (EvaluateConditionalStatements(currentGlobal, line))
                    {

                        if (elseisValid) elseisValid = false;
                        l.AddRange(script.source.SubArray(i + 1, size));

                    }
                    else
                    {
                        elseisValid = true;
                    }
                    openIf++;
                    i += size;
                    foundConditions = true;

                }
                else if (line.StartsWith(SourceScript.ElseIfStatement))
                {

                    if (openIf > 0)
                    {
                        int size = GetBlockSize(script.source, i);
                        if (elseisValid && EvaluateConditionalStatements(currentGlobal, line))
                        {

                            if (elseisValid) elseisValid = false;
                            l.AddRange(script.source.SubArray(i + 1, size));

                        }
                        i += size;

                        foundConditions = true;

                    }
                    else
                        throw new Exception("the else if statement: " +
                                            SourceScript.ElseIfStatement +
                                            " must be preceeded with " +
                                            SourceScript.IfStatement);

                }
                else if (line.StartsWith(SourceScript.ElseStatement))
                {
                    if (openIf > 0)
                    {
                        int size = GetBlockSize(script.source, i);
                        if (elseisValid)
                            l.AddRange(script.source.SubArray(i + 1, size));

                        i += size;
                        foundConditions = true;
                    }
                    else
                        throw new Exception("the else statement: " +
                                            SourceScript.ElseStatement +
                                            " must be preceeded with " +
                                            SourceScript.IfStatement);
                }
                else if (line.StartsWith(SourceScript.EndIfStatement))
                {
                    if (openIf > 0)
                        openIf--;
                    else
                        throw new Exception("the endif statement: " +
                                            SourceScript.EndIfStatement +
                                            " must be preceeded with " +
                                            SourceScript.IfStatement);
                }
                else if (line.StartsWith(SourceScript.DefineStatement))
                {
                    DefineInGlobalTable(currentGlobal, SourceScript.GetValues(line));
                }
                else if (line.StartsWith(SourceScript.UndefineStatement))
                {
                    UnDefineInGlobalTable(currentGlobal, SourceScript.GetValues(line));
                }
                else
                {
                    l.Add(script.source[i]);
                }

            }

            script.source = l.ToArray();

            if (foundConditions) ResolveConditions(script, currentGlobal,pass+1);
            
        }

        private static bool EvaluateConditionalStatements(Dictionary<string, bool> globalTable, string statement)
        {
            string[] cs = statement.Split(" ");

            bool ret = true;
            for (int i = 1; i < cs.Length; i++)
            {
                ret &= EvaluateExpression(globalTable, cs[i]);
            }

            return ret;
        }

        private static bool EvaluateExpression(Dictionary<string, bool> globalTable, string expression)
        {
            return globalTable.ContainsKey(expression) && globalTable[expression];
        }

        private static void DefineInGlobalTable(Dictionary<string, bool> globalTable, string[] defines)
        {
            SetInGlobalTable(globalTable, defines, true);
        }

        private static void UnDefineInGlobalTable(Dictionary<string, bool> globalTable, string[] defines)
        {
            SetInGlobalTable(globalTable, defines, false);
        }



        private static void SetInGlobalTable(Dictionary<string, bool> globalTable, string[] defines, bool set)
        {
            for (int i = 0; i < defines.Length; i++)
            {
                if (globalTable.ContainsKey(defines[i])) globalTable[defines[i]] = set;
                else globalTable.Add(defines[i], set);
            }
        }

        

        private static int GetBlockSize(string[] source, int start)
        {
            int tolerance = 0;
            for (int i = start + 1; i < source.Length; i++)
            {
                string line = source[i].Trim();
                if (line.StartsWith(SourceScript.IfStatement))
                {
                    i += GetBlockSize(source, i);
                    tolerance++;
                }

                else if (line.StartsWith(SourceScript.EndIfStatement) ||
                         line.StartsWith(SourceScript.ElseIfStatement) ||
                         line.StartsWith(SourceScript.ElseStatement))
                {
                    if (tolerance == 0)
                        return i - start-1;
                    if (line.StartsWith(SourceScript.EndIfStatement)) tolerance--;
                }
            }

            throw new Exception("Invalid usage of If statement");
        }

    }
}