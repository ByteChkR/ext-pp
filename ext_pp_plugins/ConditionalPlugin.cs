using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp_plugins
{
    public class ConditionalPlugin : IPlugin
    {
        public string[] Cleanup => new string[] { DefineKeyword, UndefineKeyword };
        public string[] Prefix => new string[] { "con" };
        public bool IncludeGlobal => true;

        public string StartCondition = "#if";
        public string ElseIfCondition = "#elseif";
        public string ElseCondition = "#else";
        public string EndCondition = "#endif";
        public string UndefineKeyword = "#undefine";
        public string DefineKeyword = "#define";
        public string OrOperator = "||";
        public string NotOperator = "!";
        public string AndOperator = "&&";
        public string Separator = " ";
        public bool EnableDefine = true;
        public bool EnableUndefine = true;

        public List<CommandInfo> Info { get; } = new List<CommandInfo>()
        {
            new CommandInfo("d", PropertyHelper.GetFieldInfo(typeof(ConditionalPlugin), nameof(DefineKeyword)), 
                "Sets the Keyword that defines variables when processing the file"),
            new CommandInfo("u", PropertyHelper.GetFieldInfo(typeof(ConditionalPlugin), nameof(UndefineKeyword)),
                "Sets the Keyword that undefines variables when processing the file"),
            new CommandInfo("if", PropertyHelper.GetFieldInfo(typeof(ConditionalPlugin), nameof(StartCondition)),
                "Sets the Keyword that starts an conditional block"),
            new CommandInfo("elif", PropertyHelper.GetFieldInfo(typeof(ConditionalPlugin), nameof(ElseIfCondition)),
                "Sets the Keyword that can follow an conditional block with another conditional block"),
            new CommandInfo("else", PropertyHelper.GetFieldInfo(typeof(ConditionalPlugin), nameof(ElseCondition)),
            "Sets the Keyword that gets selected if all previous conditional blocks evaluatet to false"),
            new CommandInfo("eif", PropertyHelper.GetFieldInfo(typeof(ConditionalPlugin), nameof(EndCondition)),
                "Sets the Keyword that ends an conditional block if no others are immediately following"),
            new CommandInfo("n", PropertyHelper.GetFieldInfo(typeof(ConditionalPlugin), nameof(NotOperator)),
                "Sets the characters for the NOT operator"),
            new CommandInfo("a", PropertyHelper.GetFieldInfo(typeof(ConditionalPlugin), nameof(AndOperator)),
                "Sets the characters for the AND operator"),
            new CommandInfo("o", PropertyHelper.GetFieldInfo(typeof(ConditionalPlugin), nameof(OrOperator)),
                "Sets the characters for the OR operator"),
            new CommandInfo("eD", PropertyHelper.GetFieldInfo(typeof(ConditionalPlugin), nameof(EnableDefine)),
                "Sets the characters that will be used to separate strings"),
            new CommandInfo("o", PropertyHelper.GetFieldInfo(typeof(ConditionalPlugin), nameof(OrOperator)),
                "Sets the characters for the OR operator"),
            new CommandInfo("eU", PropertyHelper.GetFieldInfo(typeof(ConditionalPlugin), nameof(EnableUndefine)),
                "Sets the characters that will be used to separate strings"),
        };


        public void Initialize(Settings settings, ISourceManager sourceManager, IDefinitions defs)
        {
            settings.ApplySettings(Info, this);

        }

        public bool Process(ISourceScript file, ISourceManager todo, IDefinitions defs)
        {
            Logger.Log(DebugLevel.LOGS, "Starting Condition Solver passes on file: " + file.GetKey(), Verbosity.LEVEL4);
            bool ret = true;
            int openIf = 0;
            bool foundConditions = false;
            bool elseIsValid = false;
            List<string> lastPass = file.GetSource().ToList();
            List<string> solvedFile = new List<string>();
            int passCount = 0;
            do
            {
                passCount++;
                Logger.Log(DebugLevel.LOGS, "Starting Condition Solver pass: " + passCount, Verbosity.LEVEL4);

                foundConditions = false;
                elseIsValid = false;
                for (int i = 0; i < lastPass.Count; i++)
                {
                    string line = lastPass[i].TrimStart();
                    if (IsKeyWord(line, StartCondition))
                    {
                        bool r = EvaluateConditional(line, defs);
                        elseIsValid = !r;
                        int size = GetBlockSize(lastPass, i);
                        if (r)
                        {
                            solvedFile.AddRange(lastPass.SubArray(i + 1, size));
                            Logger.Log(DebugLevel.LOGS, "Adding Branch To Solved File.", Verbosity.LEVEL4);
                        }

                        openIf++;
                        i += size;
                        foundConditions = true;
                    }
                    else if (elseIsValid && IsKeyWord(line, ElseIfCondition))
                    {
                        if (openIf > 0)
                        {
                            bool r = EvaluateConditional(line, defs);
                            elseIsValid = !r;
                            int size = GetBlockSize(lastPass, i);
                            if (r)
                            {
                                solvedFile.AddRange(lastPass.SubArray(i + 1, size));
                                Logger.Log(DebugLevel.LOGS, "Adding Branch To Solved File.", Verbosity.LEVEL4);
                            }

                            i += size;
                            foundConditions = true;
                        }
                        else
                        {
                            Logger.Log(DebugLevel.ERRORS, "A " + ElseIfCondition + " should be preceeded by an " + StartCondition, Verbosity.LEVEL1);
                            ret = false;
                            break;
                        }
                    }
                    else if (IsKeyWord(line, ElseCondition))
                    {
                        if (openIf > 0)
                        {

                            Logger.Log(DebugLevel.LOGS, "Found Else Statement", Verbosity.LEVEL4);
                            var size = GetBlockSize(lastPass, i);
                            if (elseIsValid)
                            {
                                solvedFile.AddRange(lastPass.SubArray(i + 1, size));
                                Logger.Log(DebugLevel.LOGS, "Adding Branch To Solved File.", Verbosity.LEVEL4);
                            }
                            else
                            {
                                Logger.Log(DebugLevel.LOGS, "Ignored since a previous condition was true", Verbosity.LEVEL4);
                            }
                            i += size;
                            foundConditions = true;
                        }
                        else
                        {
                            Logger.Log(DebugLevel.ERRORS, "A " + ElseCondition + " should be preceeded by an " + StartCondition, Verbosity.LEVEL1);
                            ret = false;
                            break;
                        }
                    }
                    else if (IsKeyWord(line, EndCondition))
                    {
                        if (openIf > 0)
                            openIf--;
                        else
                        {
                            ret = false;

                            Logger.Log(DebugLevel.ERRORS, "A " + EndCondition + " should be preceeded by an " + StartCondition, Verbosity.LEVEL1);
                            break;
                        }
                    }
                    else if (EnableDefine &&
                             line.StartsWith(DefineKeyword))
                    {

                        Logger.Log(DebugLevel.LOGS, "Found a " + DefineKeyword + " Statement", Verbosity.LEVEL4);
                        defs.Set(Utils.SplitAndRemoveFirst(line, Separator));
                        solvedFile.Add(lastPass[i]);
                    }
                    else if (EnableUndefine &&
                             line.StartsWith(UndefineKeyword))
                    {
                        Logger.Log(DebugLevel.LOGS, "Found a " + UndefineKeyword + " Statement", Verbosity.LEVEL4);
                        defs.Unset(Utils.SplitAndRemoveFirst(line, Separator));
                        solvedFile.Add(lastPass[i]);
                    }
                    else
                    {
                        solvedFile.Add(lastPass[i]);
                    }
                }

                if (ret) lastPass = solvedFile;
                else break;
                solvedFile = new List<string>();



            } while (foundConditions);

            file.SetSource(lastPass.ToArray());


            Logger.Log(DebugLevel.LOGS, "Conditional Solver Finished", Verbosity.LEVEL4);

            return ret;
        }


        private int GetBlockSize(IReadOnlyList<string> source, int start)
        {
            Logger.Log(DebugLevel.LOGS, "Finding End of conditional block...", Verbosity.LEVEL5);
            var tolerance = 0;
            for (var i = start + 1; i < source.Count; i++)
            {
                var line = source[i].Trim();
                if (line.StartsWith(StartCondition))
                {
                    Logger.Log(DebugLevel.LOGS, "Found nested opening conditional block...", Verbosity.LEVEL5);
                    i += GetBlockSize(source, i);
                    tolerance++;
                }

                else if (line.StartsWith(EndCondition) ||
                         line.StartsWith(ElseIfCondition) ||
                         line.StartsWith(ElseCondition))
                {
                    if (tolerance == 0)
                    {

                        Logger.Log(DebugLevel.LOGS, "Found correct ending conditional block...", Verbosity.LEVEL5);
                        return i - start - 1;
                    }
                    if (line.StartsWith(EndCondition))
                    {

                        Logger.Log(DebugLevel.LOGS, "Found an ending conditional block...", Verbosity.LEVEL5);
                        tolerance--;
                    }
                }
            }

            return -1; //Not getting here since it crashes in Logger.Crash
        }

        private bool EvaluateConditional(string expression, IDefinitions defs)
        {

            Logger.Log(DebugLevel.LOGS, "Found condition: " + expression, Verbosity.LEVEL4);
            string condition = FixCondition(Utils.SplitAndRemoveFirst(expression, Separator).Unpack(Separator));

            string[] cs = condition.Pack(Separator).ToArray();
            return EvaluateConditional(cs, defs);
        }
        private bool EvaluateConditional(string[] expression, IDefinitions defs)
        {

            Logger.Log(DebugLevel.LOGS, "Evaluating Condition...", Verbosity.LEVEL4);

            bool ret = true;
            bool isOr = false;
            bool expectOperator = false;

            for (int i = 0; i < expression.Length; i++)
            {
                if (expression[i] == OrOperator || expression[i] == AndOperator)
                {
                    isOr = expression[i] == OrOperator;
                    expectOperator = false;
                }
                else if (expression[i] == "(")
                {
                    //i++;
                    if (expectOperator) isOr = false;
                    expectOperator = true;

                    int size = IndexOfClosingBracket(expression, i) - i-1;
                    bool tmp = EvaluateConditional(expression.SubArray(i+1, size).ToArray(), defs);
                    if (isOr) ret |= tmp;
                    else ret &= tmp;
                    i += size;
                }
                else
                {
                    if (expectOperator) isOr = false;
                    expectOperator = true;
                    bool tmp = EvaluateExpression(expression[i], defs);
                    if (isOr) ret |= tmp;
                    else ret &= tmp;
                }
            }

            return ret;
        }

        private bool EvaluateExpression(string expression, IDefinitions defs)
        {
            Logger.Log(DebugLevel.LOGS, "Evaluating Expression: " + expression, Verbosity.LEVEL5);
            bool neg = expression.StartsWith(NotOperator);
            if (expression == NotOperator)
            {
                Logger.Log(DebugLevel.ERRORS, "Single not Operator found. Will break the compilation.", Verbosity.LEVEL1);
                return false;
            }

            if (neg) expression = expression.Substring(1, expression.Length - 1);

            var val = defs.Check(expression);

            val = neg ? !val : val;

            return val;
        }
        private string FixCondition(string line)
        {

            Logger.Log(DebugLevel.LOGS, "Fixing condition: " + line, Verbosity.LEVEL5);
            List<char> ret = new List<char>();

            string r = line;
            r = SurroundWithSpaces(r, OrOperator);
            r = SurroundWithSpaces(r, AndOperator);
            r = SurroundWithSpaces(r, "(");
            r = SurroundWithSpaces(r, ")");
            string rr = Utils.RemoveExcessSpaces(r, Separator);

            Logger.Log(DebugLevel.LOGS, "Fixed condition(new): " + rr, Verbosity.LEVEL5);
            return rr;

        }

        private static int IndexOfClosingBracket(string[] expression, int openBracketIndex)
        {
            Logger.Log(DebugLevel.LOGS, "Finding Closing Bracket...", Verbosity.LEVEL6);
            int tolerance = 0;
            for (int i = openBracketIndex+1; i < expression.Length; i++)
            {
                if (expression[i] == "(")
                {
                    Logger.Log(DebugLevel.LOGS, "Found Nested opening Bracket, adjusting tolerance.", Verbosity.LEVEL6);
                    tolerance++;
                }
                else if (expression[i] == ")")
                {
                    if (tolerance == 0)
                    {

                        Logger.Log(DebugLevel.LOGS, "Found Correct Closing Bracket", Verbosity.LEVEL6);
                        return i;
                    }
                    Logger.Log(DebugLevel.LOGS, "Found Nested Closing Bracket, adjusting tolerance.", Verbosity.LEVEL6);
                    tolerance--;
                }
            }

            return -1;
        }

        private static string SurroundWithSpaces(string line, string keyword)
        {
            StringBuilder sb = new StringBuilder(line);
            sb.Replace(keyword, " " + keyword + " ");
            string ret = sb.ToString();
            Logger.Log(DebugLevel.LOGS, "Removing Excess Spaces: " + line + " => " + ret, Verbosity.LEVEL6);
            return ret;
        }

        

        private static bool IsKeyWord(string line, string keyword)
        {
            string tmp = line.TrimStart();

            return tmp.StartsWith(keyword);
        }

    }
}