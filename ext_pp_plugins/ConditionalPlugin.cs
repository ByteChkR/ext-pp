using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp_plugins
{
    public class ConditionalPlugin : AbstractPlugin
    {

        private static StringBuilder _sb = new StringBuilder();

        public override string[] Cleanup => new [] { DefineKeyword, UndefineKeyword };
        public override string[] Prefix => new [] { "con", "Conditional" };
        public override ProcessStage ProcessStages => Stage.ToLower(CultureInfo.InvariantCulture) == "onload" ? ProcessStage.ON_LOAD_STAGE : ProcessStage.ON_MAIN;
        public override PluginType PluginTypeToggle => PluginType.FULL_SCRIPT_PLUGIN;
        public string StartCondition { get; set; } = "#if";
        public string ElseIfCondition { get; set; } = "#elseif";
        public string ElseCondition { get; set; } = "#else";
        public string EndCondition { get; set; } = "#endif";
        public string UndefineKeyword { get; set; } = "#undefine";
        public string DefineKeyword { get; set; } = "#define";
        public string OrOperator { get; set; } = "||";
        public string NotOperator { get; set; } = "!";
        public string AndOperator { get; set; } = "&&";
        public string Separator { get; set; } = " ";
        public bool EnableDefine { get; set; } = true;
        public bool EnableUndefine { get; set; } = true;
        public string Stage { get; set; } = "onload";


        public override List<CommandInfo> Info { get; } = new List<CommandInfo>
        {
            new CommandInfo("set-define", "d", PropertyHelper.GetPropertyInfo(typeof(ConditionalPlugin), nameof(DefineKeyword)),
                "Sets the keyword that is used to define variables during the compilation."),
            new CommandInfo("set-undefine", "u", PropertyHelper.GetPropertyInfo(typeof(ConditionalPlugin), nameof(UndefineKeyword)),
                "Sets the keyword that is used to undefine previously defined variables during the compilation."),
            new CommandInfo("set-if","if", PropertyHelper.GetPropertyInfo(typeof(ConditionalPlugin), nameof(StartCondition)),
                "Sets the keyword that is used to start a new condition block."),
            new CommandInfo("set-elseif","elif", PropertyHelper.GetPropertyInfo(typeof(ConditionalPlugin), nameof(ElseIfCondition)),
                "Sets the keyword that is used to continue a previously started condition block with another condition block."),
            new CommandInfo("set-else", "else", PropertyHelper.GetPropertyInfo(typeof(ConditionalPlugin), nameof(ElseCondition)),
            "Sets the keyword that is used to start a new condition block that is taken when the previous blocks evaluated to false."),
            new CommandInfo("set-endif", "eif", PropertyHelper.GetPropertyInfo(typeof(ConditionalPlugin), nameof(EndCondition)),
                "Sets the keyword that is used to end a previously started condition block."),
            new CommandInfo("set-not","n", PropertyHelper.GetPropertyInfo(typeof(ConditionalPlugin), nameof(NotOperator)),
                "Sets the keyword that is used to negate an expression in if conditions."),
            new CommandInfo("set-and","a", PropertyHelper.GetPropertyInfo(typeof(ConditionalPlugin), nameof(AndOperator)),
                "Sets the keyword for the logical AND operator"),
            new CommandInfo("set-or","o", PropertyHelper.GetPropertyInfo(typeof(ConditionalPlugin), nameof(OrOperator)),
                "Sets the keyword for the logical OR operator"),
            new CommandInfo("enable-define", "eD", PropertyHelper.GetPropertyInfo(typeof(ConditionalPlugin), nameof(EnableDefine)),
                "Enables/Disables the detection of define statements(defines can still be set via the defines object/the command line)"),
            new CommandInfo("enable-undefine","eU", PropertyHelper.GetPropertyInfo(typeof(ConditionalPlugin), nameof(EnableUndefine)),
                "Enables/Disables the detection of undefine statements"),
            new CommandInfo("set-stage", "ss", PropertyHelper.GetPropertyInfo(typeof(ConditionalPlugin), nameof(Stage)),
                "Sets the Stage Type of the Plugin to be Executed OnLoad or OnFinishUp"),
            new CommandInfo("set-separator", "s", PropertyHelper.GetPropertyInfo(typeof(ConditionalPlugin), nameof(Separator)),
                "Sets the separator that is used to separate different generic types"),
        };


        public override void Initialize(Settings settings, ISourceManager sourceManager, IDefinitions defs)
        {
            settings.ApplySettings(Info, this);

        }

        public override bool OnLoad_FullScriptStage(ISourceScript script, ISourceManager sourceManager, IDefinitions defTable)
        {
            return FullScriptStage(script, sourceManager, defTable);
        }

        public override bool OnMain_FullScriptStage(ISourceScript script, ISourceManager sourceManager, IDefinitions defTable)
        {
            return FullScriptStage(script, sourceManager, defTable);
        }

        public bool FullScriptStage(ISourceScript file, ISourceManager todo, IDefinitions defs)
        {
            this.Log(DebugLevel.LOGS, Verbosity.LEVEL4, "Starting Condition Solver passes on file: {0}", Path.GetFileName(file.GetFilePath()));
            bool ret = true;
            int openIf = 0;
            bool foundConditions = false;
            bool elseIsValid = false;
            bool expectEndOrIf = false;
            List<string> lastPass = file.GetSource().ToList();
            List<string> solvedFile = new List<string>();
            int passCount = 0;
            do
            {
                passCount++;
                this.Log(DebugLevel.LOGS, Verbosity.LEVEL5, "Starting Condition Solver pass: {0}", passCount);

                foundConditions = false;
                elseIsValid = false;
                for (int i = 0; i < lastPass.Count; i++)
                {
                    string line = lastPass[i].TrimStart();
                    if (IsKeyWord(line, StartCondition))
                    {
                        this.Log(DebugLevel.LOGS,  Verbosity.LEVEL5, "Found a {0} Statement", StartCondition);
                        bool r = EvaluateConditional(line, defs);
                        this.Log(DebugLevel.LOGS, Verbosity.LEVEL5, "Evaluation: {0}", r);
                        elseIsValid = !r;
                        int size = GetBlockSize(lastPass, i);
                        if (r)
                        {
                            solvedFile.AddRange(lastPass.SubArray(i + 1, size));
                            this.Log(DebugLevel.LOGS, Verbosity.LEVEL5, "Adding Branch To Solved File.");
                        }

                        openIf++;
                        i += size;
                        foundConditions = true;
                        expectEndOrIf = false;
                    }
                    else if (elseIsValid && IsKeyWord(line, ElseIfCondition))
                    {
                        if (!expectEndOrIf && openIf > 0)
                        {
                            this.Log(DebugLevel.LOGS, Verbosity.LEVEL5, "Found a {0} Statement", ElseIfCondition);
                            bool r = EvaluateConditional(line, defs);
                        this.Log(DebugLevel.LOGS, Verbosity.LEVEL5, "Evaluation: {0}", r);
                            elseIsValid = !r;
                            int size = GetBlockSize(lastPass, i);
                            if (r)
                            {
                                solvedFile.AddRange(lastPass.SubArray(i + 1, size));
                                this.Log(DebugLevel.LOGS, Verbosity.LEVEL5, "Adding Branch To Solved File.");
                            }

                            i += size;
                            foundConditions = true;
                        }
                        else if (expectEndOrIf)
                        {
                            this.Log(DebugLevel.ERRORS, Verbosity.LEVEL1, "A {0} can not be followed by an {1}", ElseCondition, ElseIfCondition);
                            ret = false;
                            break;
                        }
                        else
                        {
                            this.Log(DebugLevel.ERRORS, Verbosity.LEVEL1, "A {0} should be preceeded by a {1}", ElseIfCondition, StartCondition);
                            ret = false;
                            break;
                        }
                    }
                    else if (IsKeyWord(line, ElseCondition))
                    {
                        if (openIf > 0)
                        {

                            this.Log(DebugLevel.LOGS, Verbosity.LEVEL5, "Found a {0} Statement", ElseCondition);
                            var size = GetBlockSize(lastPass, i);
                            if (elseIsValid)
                            {
                                solvedFile.AddRange(lastPass.SubArray(i + 1, size));
                                this.Log(DebugLevel.LOGS, Verbosity.LEVEL5, "Adding Branch To Solved File.");
                            }
                            else
                            {
                                this.Log(DebugLevel.LOGS, Verbosity.LEVEL5, "Ignored since a previous condition was true");
                            }
                            i += size;
                            foundConditions = true;
                            expectEndOrIf = true;
                        }
                        else
                        {
                            this.Log(DebugLevel.ERRORS, Verbosity.LEVEL1, "A {0} should be preceeded by a {1}", ElseCondition, StartCondition);
                            ret = false;
                            break;
                        }
                    }
                    else if (IsKeyWord(line, EndCondition))
                    {
                        if (openIf > 0)
                        {
                            expectEndOrIf = false;
                            openIf--;
                        }
                        else
                        {
                            ret = false;

                            this.Log(DebugLevel.ERRORS, Verbosity.LEVEL1, "A {0} should be preceeded by a {1}", EndCondition, StartCondition);
                            break;
                        }
                    }
                    else if (EnableDefine &&
                             line.StartsWith(DefineKeyword))
                    {

                        this.Log(DebugLevel.LOGS, Verbosity.LEVEL5, "Found a {0} Statement", DefineKeyword); ;
                        defs.Set(Utils.SplitAndRemoveFirst(line, Separator));
                        solvedFile.Add(lastPass[i]);
                    }
                    else if (EnableUndefine &&
                             line.StartsWith(UndefineKeyword))
                    {
                        this.Log(DebugLevel.LOGS, Verbosity.LEVEL5, "Found a {0} Statement", UndefineKeyword);
                        defs.Unset(Utils.SplitAndRemoveFirst(line, Separator));
                        solvedFile.Add(lastPass[i]);
                    }
                    else
                    {
                        solvedFile.Add(lastPass[i]);
                    }
                }

                if (ret)
                {
                    lastPass = solvedFile;
                }
                else
                {
                    break;
                }
                solvedFile = new List<string>();



            } while (foundConditions);

            file.SetSource(lastPass.ToArray());


            this.Log(DebugLevel.LOGS, Verbosity.LEVEL4, "Conditional Solver Finished");

            return ret;
        }


        private int GetBlockSize(IReadOnlyList<string> source, int start)
        {
            this.Log(DebugLevel.LOGS, Verbosity.LEVEL6, "Finding End of conditional block...");
            var tolerance = 0;
            for (var i = start + 1; i < source.Count; i++)
            {
                var line = source[i].Trim();
                if (line.StartsWith(StartCondition))
                {
                    this.Log(DebugLevel.LOGS, Verbosity.LEVEL7, "Found nested opening conditional block...");
                    i += GetBlockSize(source, i); //Skip Indices that are "inside" the if clause
                    tolerance++;
                }

                else if (line.StartsWith(EndCondition) ||
                         line.StartsWith(ElseIfCondition) ||
                         line.StartsWith(ElseCondition))
                {
                    if (tolerance == 0)
                    {

                        this.Log(DebugLevel.LOGS, Verbosity.LEVEL6, "Found correct ending conditional block...");
                        return i - start - 1;
                    }
                    if (line.StartsWith(EndCondition))
                    {

                        this.Log(DebugLevel.LOGS, Verbosity.LEVEL7, "Found an ending conditional block...");
                        tolerance--;
                    }
                }
            }

            return -1; //Not getting here since it crashes in this.Crash
        }

        private bool EvaluateConditional(string expression, IDefinitions defs)
        {
            string condition = FixCondition(Utils.SplitAndRemoveFirst(expression, Separator).Unpack(Separator));

            string[] cs = condition.Pack(Separator).ToArray();
            return EvaluateConditional(cs, defs);
        }
        private bool EvaluateConditional(string[] expression, IDefinitions defs)
        {

            this.Log(DebugLevel.LOGS, Verbosity.LEVEL7, "Evaluating Expression: {0}" , expression.Unpack(" "));

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
                    if (expectOperator)
                    {
                        isOr = false;
                    }
                    expectOperator = true;

                    int size = IndexOfClosingBracket(expression, i) - i - 1;
                    bool tmp = EvaluateConditional(expression.SubArray(i + 1, size).ToArray(), defs);
                    if (isOr)
                    {
                        ret |= tmp;
                    }
                    else
                    {
                        ret &= tmp;
                    }
                    i += size;
                }
                else
                {
                    if (expectOperator)
                    {
                        isOr = false;
                    }
                    expectOperator = true;
                    bool tmp = EvaluateExpression(expression[i], defs);
                    if (isOr)
                    {
                        ret |= tmp;
                    }
                    else
                    {
                        ret &= tmp;
                    }
                }
            }

            return ret;
        }

        private bool EvaluateExpression(string expression, IDefinitions defs)
        {
            this.Log(DebugLevel.LOGS, Verbosity.LEVEL6, "Evaluating Expression: {0}", expression);
            bool neg = expression.StartsWith(NotOperator);
            if (expression == NotOperator)
            {
                this.Log(DebugLevel.ERRORS, Verbosity.LEVEL1, "Single not Operator found. Will break the compilation.");
                return false;
            }

            string exp=expression;
            if (neg)
            {
                exp = expression.Substring(1, expression.Length - 1);
            }

            var val = defs.Check(exp);

            val = neg ? !val : val;

            return val;
        }
        private string FixCondition(string line)
        {

            this.Log(DebugLevel.LOGS, Verbosity.LEVEL6, "Fixing expression: {0}" , line);
            List<char> ret = new List<char>();

            string r = line;
            r = SurroundWithSpaces(r, OrOperator);
            r = SurroundWithSpaces(r, AndOperator);
            r = SurroundWithSpaces(r, "(");
            r = SurroundWithSpaces(r, ")");
            string rr = Utils.RemoveExcessSpaces(r, Separator, this);

            this.Log(DebugLevel.LOGS, Verbosity.LEVEL6, "Fixed condition(new): {0}" , rr);
            return rr;

        }

        private int IndexOfClosingBracket(string[] expression, int openBracketIndex)
        {
            this.Log(DebugLevel.LOGS, Verbosity.LEVEL7, "Finding Closing Bracket...");
            int tolerance = 0;
            for (int i = openBracketIndex + 1; i < expression.Length; i++)
            {
                if (expression[i] == "(")
                {
                    this.Log(DebugLevel.LOGS, Verbosity.LEVEL8, "Found Nested opening Bracket, adjusting tolerance.");
                    tolerance++;
                }
                else if (expression[i] == ")")
                {
                    if (tolerance == 0)
                    {

                        this.Log(DebugLevel.LOGS, Verbosity.LEVEL7, "Found Correct Closing Bracket");
                        return i;
                    }
                    this.Log(DebugLevel.LOGS, Verbosity.LEVEL8, "Found Nested Closing Bracket, adjusting tolerance.");
                    tolerance--;
                }
            }

            return -1;
        }


        private string SurroundWithSpaces(string line, string keyword)
        {
            _sb.Clear();
            _sb.Append(line);
            this.Log(DebugLevel.LOGS, Verbosity.LEVEL7, "Surrounding {0} with spaces...", keyword);
            _sb.Replace(keyword, " " + keyword + " ");
            return _sb.ToString();
        }



        private static bool IsKeyWord(string line, string keyword)
        {
            string tmp = line.TrimStart();

            return tmp.StartsWith(keyword);
        }

    }
}