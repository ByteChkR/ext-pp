using System;
using System.Collections.Generic;
using System.Linq;
using ext_compiler.extensions;
using ext_compiler.settings;

namespace ext_compiler
{
    public static class ConditionalResolver
    {

        public static void ResolveConditions(List<SourceScript> tree, Dictionary<string, bool> globalTable = null)
        {
            Logger.Log(DebugLevel.LOGS, "Resolving Conditions in file tree", Verbosity.LEVEL2);
            if (globalTable == null)
                globalTable = new Dictionary<string, bool>();

            for (int i = 0; i < tree.Count; i++)
            {
                ResolveConditions(tree[i], globalTable);
            }

        }

        public static void ResolveConditions(SourceScript script, Dictionary<string, bool> globalTable)
        {

            Logger.Log(DebugLevel.LOGS, "Resolving conditions in file: " + script.key, Verbosity.LEVEL2);
            ResolveConditions(script, globalTable, 1);
        }

        private static void ResolveConditions(SourceScript script, Dictionary<string, bool> currentGlobal, int pass)
        {
            Logger.Log(DebugLevel.LOGS, "Condition Resolver Pass: " + pass, Verbosity.LEVEL4);
            List<string> l = new List<string>();
            int openIf = 0;
            bool foundConditions = false;
            bool elseisValid = false;
            for (int i = 0; i < script.source.Length; i++)
            {
                string line = script.source[i].Trim();
                if (line.StartsWith(ExtensionProcessor.settings.Keywords.IfStatement))
                {
                    int size = GetBlockSize(script.source, i);
                    Logger.Log(DebugLevel.LOGS, "Found Conditional Statement: " + line, Verbosity.LEVEL5);
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
                else if (line.StartsWith(ExtensionProcessor.settings.Keywords.ElseIfStatement))
                {

                    if (openIf > 0)
                    {
                        int size = GetBlockSize(script.source, i);

                        Logger.Log(DebugLevel.LOGS, "Found Conditional Statement: " + line, Verbosity.LEVEL5);
                        if (elseisValid && EvaluateConditionalStatements(currentGlobal, line))
                        {

                            if (elseisValid) elseisValid = false;
                            l.AddRange(script.source.SubArray(i + 1, size));

                        }
                        i += size;

                        foundConditions = true;

                    }
                    else
                        Logger.Crash(new Exception("the else if statement: " +
                                            ExtensionProcessor.settings.Keywords.ElseIfStatement +
                                            " must be preceeded with " +
                                            ExtensionProcessor.settings.Keywords.IfStatement));

                }
                else if (line.StartsWith(ExtensionProcessor.settings.Keywords.ElseStatement))
                {
                    if (openIf > 0)
                    {
                        int size = GetBlockSize(script.source, i);
                        Logger.Log(DebugLevel.LOGS, "Found Conditional Else Statement: " + line, Verbosity.LEVEL5);
                        if (elseisValid)
                            l.AddRange(script.source.SubArray(i + 1, size));

                        i += size;
                        foundConditions = true;
                    }
                    else
                        Logger.Crash(new Exception("the else statement: " +
                                            ExtensionProcessor.settings.Keywords.ElseStatement +
                                            " must be preceeded with " +
                                            ExtensionProcessor.settings.Keywords.IfStatement));
                }
                else if (line.StartsWith(ExtensionProcessor.settings.Keywords.EndIfStatement))
                {
                    if (openIf > 0)
                        openIf--;
                    else
                        Logger.Crash(new Exception("the endif statement: " +
                                            ExtensionProcessor.settings.Keywords.EndIfStatement +
                                            " must be preceeded with " +
                                            ExtensionProcessor.settings.Keywords.IfStatement));
                }
                else if (ExtensionProcessor.settings.ResolveDefine &&
                         line.StartsWith(ExtensionProcessor.settings.Keywords.DefineStatement))
                {
                    DefineInGlobalTable(currentGlobal, line.GetStatementValues());
                    l.Add(script.source[i]);
                }
                else if (ExtensionProcessor.settings.ResolveUnDefine &&
                         line.StartsWith(ExtensionProcessor.settings.Keywords.UndefineStatement))
                {
                    UnDefineInGlobalTable(currentGlobal, line.GetStatementValues());
                    l.Add(script.source[i]);
                }
                else
                {
                    l.Add(script.source[i]);
                }

            }

            script.source = l.ToArray();

            if (foundConditions)
                ResolveConditions(script, currentGlobal, pass + 1);

        }

        private static bool EvaluateConditionalStatements(Dictionary<string, bool> globalTable, string statement)
        {
            Logger.Log(DebugLevel.LOGS, "Evaluating Conditional Statement", Verbosity.LEVEL6);

            string[] cs = statement.GetStatementValues();

            bool ret = true;
            for (int i = 0; i < cs.Length; i++)
            {
                ret &= EvaluateExpression(globalTable, cs[i]);
            }

            return ret;
        }

        private static bool EvaluateExpression(Dictionary<string, bool> globalTable, string expression)
        {
            Logger.Log(DebugLevel.LOGS, "Evaluating Expression: " + expression, Verbosity.LEVEL7);
            bool val = globalTable.ContainsKey(expression) && globalTable[expression];
            Logger.Log(DebugLevel.LOGS, "Expression Evaluated: " + val, Verbosity.LEVEL7);

            return val;
        }

        private static void DefineInGlobalTable(Dictionary<string, bool> globalTable, string[] defines)
        {
            Logger.Log(DebugLevel.LOGS, "Defining Symbols: " + defines.Unpack(), Verbosity.LEVEL5);
            SetInGlobalTable(globalTable, defines, true);
        }

        private static void UnDefineInGlobalTable(Dictionary<string, bool> globalTable, string[] defines)
        {
            Logger.Log(DebugLevel.LOGS, "Undefining Symbols: " + defines.Unpack(), Verbosity.LEVEL5);
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
                if (line.StartsWith(ExtensionProcessor.settings.Keywords.IfStatement))
                {
                    i += GetBlockSize(source, i);
                    tolerance++;
                }

                else if (line.StartsWith(ExtensionProcessor.settings.Keywords.EndIfStatement) ||
                         line.StartsWith(ExtensionProcessor.settings.Keywords.ElseIfStatement) ||
                         line.StartsWith(ExtensionProcessor.settings.Keywords.ElseStatement))
                {
                    if (tolerance == 0)
                        return i - start - 1;
                    if (line.StartsWith(ExtensionProcessor.settings.Keywords.EndIfStatement)) tolerance--;
                }
            }

            Logger.Crash(new Exception("Invalid usage of If statement")
            {
                Data = { { "source", source }, { "start", start } }
            });
            return -1; //Not getting here since it crashes in Logger.Crash
        }

    }
}