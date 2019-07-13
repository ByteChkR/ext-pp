using System;
using System.Collections.Generic;
using System.Linq;
using ext_pp.settings;

namespace ext_pp
{
    public static class ConditionalResolver
    {

        public static bool ResolveConditions(List<SourceScript> tree, Dictionary<string, bool> globalTable = null)
        {
            if (tree == null)
            {
                Logger.Crash(new NullReferenceException("File Tree is null."));
                return false;
            }

            Logger.Log(DebugLevel.LOGS, "Resolving Conditions in file tree", Verbosity.LEVEL2);
            if (tree.Count == 0)
            {
                Logger.Log(DebugLevel.WARNINGS, "File Tree is empty.", Verbosity.ALWAYS_SEND);
                return true;
            }

            foreach (var t in tree)
            {
                if (!ResolveConditions(t, globalTable)) return false;
            }

            return true;

        }

        public static bool ResolveConditions(SourceScript script, Dictionary<string, bool> globalTable = null)
        {
            if (script == null)
            {
                Logger.Crash(new NullReferenceException("Script is null."));
                return false;
            }
            if (globalTable == null)
                globalTable = new Dictionary<string, bool>();


            Logger.Log(DebugLevel.LOGS, "Resolving conditions in file: " + script.Key, Verbosity.LEVEL2);
            return ResolveConditions(script, globalTable, 1);
        }

        private static bool ResolveConditions(SourceScript script, Dictionary<string, bool> currentGlobal, int pass)
        {
            Logger.Log(DebugLevel.LOGS, "Condition Resolver Pass: " + pass, Verbosity.LEVEL4);
            List<string> solvedFile = new List<string>();
            var openIf = 0;
            var foundConditions = false;
            var elseIsValid = false;
            for (var i = 0; i < script.Source.Length; i++)
            {
                var line = script.Source[i].Trim();
                if (line.StartsWith(Settings.IfStatement))
                {
                    var size = GetBlockSize(script.Source, i);
                    Logger.Log(DebugLevel.LOGS, "Found Conditional Statement: " + line, Verbosity.LEVEL5);
                    if (EvaluateConditionalStatements(currentGlobal, line))
                    {
                        if (elseIsValid) elseIsValid = false;
                        solvedFile.AddRange(script.Source.SubArray(i + 1, size));
                    }
                    else
                    {
                        elseIsValid = true;
                    }

                    openIf++;
                    i += size;
                    foundConditions = true;

                }
                else if (line.StartsWith(Settings.ElseIfStatement))
                {

                    if (openIf > 0)
                    {
                        var size = GetBlockSize(script.Source, i);

                        Logger.Log(DebugLevel.LOGS, "Found Conditional Statement: " + line, Verbosity.LEVEL5);
                        if (elseIsValid && EvaluateConditionalStatements(currentGlobal, line))
                        {
                            elseIsValid = false;
                            solvedFile.AddRange(script.Source.SubArray(i + 1, size));
                        }
                        i += size;
                        foundConditions = true;
                    }
                    else
                    {
                        Logger.Crash(new Exception("the else if statement: " +
                                                   Settings.ElseIfStatement +
                                                   " must be preceded with " +
                                                   Settings.IfStatement), false);
                        return false;
                    }


                }
                else if (line.StartsWith(Settings.ElseStatement))
                {
                    if (openIf > 0)
                    {
                        var size = GetBlockSize(script.Source, i);
                        Logger.Log(DebugLevel.LOGS, "Found Conditional Else Statement: " + line, Verbosity.LEVEL5);
                        if (elseIsValid)
                            solvedFile.AddRange(script.Source.SubArray(i + 1, size));

                        i += size;
                        foundConditions = true;
                    }
                    else
                    {
                        Logger.Crash(new Exception("the else statement: " +
                                                   Settings.ElseStatement +
                                                   " must be preceded with " +
                                                   Settings.IfStatement), false);
                        return false;
                    }
                }
                else if (line.StartsWith(Settings.EndIfStatement))
                {
                    if (openIf > 0)
                        openIf--;
                    else
                    {

                        Logger.Crash(new Exception("the endif statement: " +
                                                   Settings.EndIfStatement +
                                                   " must be preceded with " +
                                                   Settings.IfStatement), false);
                        return false;
                    }
                }
                else if (Settings.ResolveDefine &&
                         line.StartsWith(Settings.DefineStatement))
                {
                    DefineInGlobalTable(currentGlobal, line.GetStatementValues());
                    solvedFile.Add(script.Source[i]);
                }
                else if (Settings.ResolveUnDefine &&
                         line.StartsWith(Settings.UndefineStatement))
                {
                    UnDefineInGlobalTable(currentGlobal, line.GetStatementValues());
                    solvedFile.Add(script.Source[i]);
                }
                else
                {
                    solvedFile.Add(script.Source[i]);
                }

            }

            script.Source = solvedFile.ToArray();

            return !foundConditions || ResolveConditions(script, currentGlobal, pass + 1);
        }

        private static bool EvaluateConditionalStatements(IReadOnlyDictionary<string, bool> globalTable, string statement)
        {
            Logger.Log(DebugLevel.LOGS, "Evaluating Conditional Statement", Verbosity.LEVEL6);

            var cs = statement.GetStatementValues().ToArray();

            return cs.Aggregate(true, (current, t) => current & EvaluateExpression(globalTable, t));
        }

        private static bool EvaluateExpression(IReadOnlyDictionary<string, bool> globalTable, string expression)
        {
            Logger.Log(DebugLevel.LOGS, "Evaluating Expression: " + expression, Verbosity.LEVEL7);
            var val = globalTable.ContainsKey(expression) && globalTable[expression];
            Logger.Log(DebugLevel.LOGS, "Expression Evaluated: " + val, Verbosity.LEVEL7);

            return val;
        }

        private static void DefineInGlobalTable(Dictionary<string, bool> globalTable, string[] defines)
        {
            if (globalTable == null) throw new ArgumentNullException(nameof(globalTable));
            Logger.Log(DebugLevel.LOGS, "Defining Symbols: " + defines.Unpack(), Verbosity.LEVEL5);
            SetInGlobalTable(globalTable, defines, true);
        }

        private static void UnDefineInGlobalTable(Dictionary<string, bool> globalTable, string[] defines)
        {
            if (globalTable == null) throw new ArgumentNullException(nameof(globalTable));
            Logger.Log(DebugLevel.LOGS, "Undefining Symbols: " + defines.Unpack(), Verbosity.LEVEL5);
            SetInGlobalTable(globalTable, defines, false);
        }



        private static void SetInGlobalTable(IDictionary<string, bool> globalTable, IEnumerable<string> defines, bool set)
        {
            foreach (var t in defines)
            {
                if (globalTable.ContainsKey(t)) globalTable[t] = set;
                else globalTable.Add(t, set);
            }
        }



        private static int GetBlockSize(IReadOnlyList<string> source, int start)
        {
            var tolerance = 0;
            for (var i = start + 1; i < source.Count; i++)
            {
                var line = source[i].Trim();
                if (line.StartsWith(Settings.IfStatement))
                {
                    i += GetBlockSize(source, i);
                    tolerance++;
                }

                else if (line.StartsWith(Settings.EndIfStatement) ||
                         line.StartsWith(Settings.ElseIfStatement) ||
                         line.StartsWith(Settings.ElseStatement))
                {
                    if (tolerance == 0)
                        return i - start - 1;
                    if (line.StartsWith(Settings.EndIfStatement)) tolerance--;
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