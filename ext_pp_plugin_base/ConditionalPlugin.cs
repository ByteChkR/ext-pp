using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp_plugins
{
    public class ConditionalPlugin : IPlugin
    {
        public string[] Cleanup => new string[] { _defKeyword, _uDefKeyword };

        private readonly string _startConditional = Settings.IfStatement;
        private readonly string _fallbackConditionalKeyword = Settings.ElseIfStatement;
        private readonly string _fallbackKeyword = Settings.ElseStatement;
        private readonly string _endConditional = Settings.EndIfStatement;
        private readonly string _uDefKeyword = Settings.UndefineStatement;
        private readonly string _defKeyword = Settings.DefineStatement;
        private readonly string _orOperator = Settings.OrOperator;
        private readonly string _notOperator = Settings.NotOperator;
        private readonly string _andOperator = Settings.AndOperator;
        private readonly string _separator = Settings.Separator;
        private readonly bool _enableDef = true;
        private readonly bool _enableUdef = true;

        public ConditionalPlugin(Settings settings)
        {
            if (settings.HasKey("eDef"))
            {

                string ed = settings.Get("eDef");
                if (!bool.TryParse(ed, out _enableDef))
                    Logger.Log(DebugLevel.WARNINGS, "Enable Define Flag could not be parsed: " + ed, Verbosity.LEVEL1);

            }
            if (settings.HasKey("eUdef"))
            {

                string eu = settings.Get("eUdef");
                if (!bool.TryParse(eu, out _enableUdef))
                    Logger.Log(DebugLevel.WARNINGS, "Enable Undefine Flag could not be parsed: " + eu, Verbosity.LEVEL1);
            }
        }

        public bool Process(ASourceScript file, ASourceManager todo, ADefinitions defs)
        {
            Logger.Log(DebugLevel.LOGS, "Starting Condition Solver passes on file: " + file.GetKey(), Verbosity.LEVEL3);
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
                Logger.Log(DebugLevel.LOGS, "Starting Condition Solver pass: " + passCount, Verbosity.LEVEL3);

                foundConditions = false;
                elseIsValid = false;
                for (int i = 0; i < lastPass.Count; i++)
                {
                    string line = lastPass[i].TrimStart();
                    if (IsKeyWord(line, _startConditional))
                    {
                        bool r = EvaluateConditional(line, defs);
                        elseIsValid = !r;
                        int size = GetBlockSize(lastPass, i);
                        if (r)
                        {
                            solvedFile.AddRange(lastPass.SubArray(i + 1, size));
                            Logger.Log(DebugLevel.LOGS, "Adding Branch To Solved File.", Verbosity.LEVEL3);
                        }

                        openIf++;
                        i += size;
                        foundConditions = true;
                    }
                    else if (elseIsValid && IsKeyWord(line, _fallbackConditionalKeyword))
                    {
                        if (openIf > 0)
                        {
                            bool r = EvaluateConditional(line, defs);
                            elseIsValid = !r;
                            int size = GetBlockSize(lastPass, i);
                            if (r)
                            {
                                solvedFile.AddRange(lastPass.SubArray(i + 1, size));
                                Logger.Log(DebugLevel.LOGS, "Adding Branch To Solved File.", Verbosity.LEVEL3);
                            }

                            i += size;
                            foundConditions = true;
                        }
                        else
                        {
                            Logger.Log(DebugLevel.ERRORS, "A " + _fallbackConditionalKeyword + " should be preceeded by an " + _startConditional, Verbosity.ALWAYS_SEND);
                            ret = false;
                            break;
                        }
                    }
                    else if (IsKeyWord(line, _fallbackKeyword))
                    {
                        if (openIf > 0)
                        {

                            Logger.Log(DebugLevel.LOGS, "Found Else Statement", Verbosity.LEVEL3);
                            var size = GetBlockSize(lastPass, i);
                            if (elseIsValid)
                            {
                                solvedFile.AddRange(lastPass.SubArray(i + 1, size));
                                Logger.Log(DebugLevel.LOGS, "Adding Branch To Solved File.", Verbosity.LEVEL3);
                            }
                            else
                            {
                                Logger.Log(DebugLevel.LOGS, "Ignored since a previous condition was true", Verbosity.LEVEL3);
                            }
                            i += size;
                            foundConditions = true;
                        }
                        else
                        {
                            Logger.Log(DebugLevel.ERRORS, "A " + _fallbackKeyword + " should be preceeded by an " + _startConditional, Verbosity.ALWAYS_SEND);
                            ret = false;
                            break;
                        }
                    }
                    else if (IsKeyWord(line, _endConditional))
                    {
                        if (openIf > 0)
                            openIf--;
                        else
                        {
                            ret = false;

                            Logger.Log(DebugLevel.ERRORS, "A " + _endConditional + " should be preceeded by an " + _startConditional, Verbosity.ALWAYS_SEND);
                            break;
                        }
                    }
                    else if (_enableDef &&
                             line.StartsWith(_defKeyword))
                    {

                        Logger.Log(DebugLevel.LOGS, "Found a " + _defKeyword + " Statement", Verbosity.LEVEL3);
                        defs.Set(Utils.SplitAndRemoveFirst(line, _separator));
                        solvedFile.Add(lastPass[i]);
                    }
                    else if (_enableUdef &&
                             line.StartsWith(_uDefKeyword))
                    {
                        Logger.Log(DebugLevel.LOGS, "Found a " + _uDefKeyword + " Statement", Verbosity.LEVEL3);
                        defs.Unset(Utils.SplitAndRemoveFirst(line, _separator));
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


            Logger.Log(DebugLevel.LOGS, "Conditional Solver Finished", Verbosity.LEVEL3);

            return ret;
        }


        private int GetBlockSize(IReadOnlyList<string> source, int start)
        {
            Logger.Log(DebugLevel.LOGS, "Finding End of conditional block...", Verbosity.LEVEL4);
            var tolerance = 0;
            for (var i = start + 1; i < source.Count; i++)
            {
                var line = source[i].Trim();
                if (line.StartsWith(_startConditional))
                {
                    Logger.Log(DebugLevel.LOGS, "Found nested opening conditional block...", Verbosity.LEVEL4);
                    i += GetBlockSize(source, i);
                    tolerance++;
                }

                else if (line.StartsWith(_endConditional) ||
                         line.StartsWith(_fallbackConditionalKeyword) ||
                         line.StartsWith(_fallbackKeyword))
                {
                    if (tolerance == 0)
                    {

                        Logger.Log(DebugLevel.LOGS, "Found correct ending conditional block...", Verbosity.LEVEL4);
                        return i - start - 1;
                    }
                    if (line.StartsWith(_endConditional))
                    {

                        Logger.Log(DebugLevel.LOGS, "Found an ending conditional block...", Verbosity.LEVEL4);
                        tolerance--;
                    }
                }
            }

            return -1; //Not getting here since it crashes in Logger.Crash
        }

        private bool EvaluateConditional(string expression, ADefinitions defs)
        {

            Logger.Log(DebugLevel.LOGS, "Found condition: " + expression, Verbosity.LEVEL3);
            string condition = FixCondition(Utils.SplitAndRemoveFirst(expression, _separator).Unpack(_separator));

            string[] cs = condition.Pack(_separator).ToArray();
            return EvaluateConditional(cs, defs);
        }
        private bool EvaluateConditional(string[] expression, ADefinitions defs)
        {

            Logger.Log(DebugLevel.LOGS, "Evaluating Condition...", Verbosity.LEVEL3);

            bool ret = true;
            bool isOr = false;
            bool expectOperator = false;

            for (int i = 0; i < expression.Length; i++)
            {
                if (expression[i] == _orOperator || expression[i] == _andOperator)
                {
                    isOr = expression[i] == _orOperator;
                    expectOperator = false;
                }
                else if (expression[i] == "(")
                {
                    i++;
                    if (expectOperator) isOr = false;
                    expectOperator = true;

                    int size = IndexOfClosingBracket(expression, i) - i;
                    bool tmp = EvaluateConditional(expression.SubArray(i, size).ToArray(), defs);
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

        private bool EvaluateExpression(string expression, ADefinitions defs)
        {
            Logger.Log(DebugLevel.LOGS, "Evaluating Expression: " + expression, Verbosity.LEVEL4);
            bool neg = expression.StartsWith(_notOperator);
            if (expression == _notOperator)
            {
                Logger.Log(DebugLevel.ERRORS, "Single not Operator found. Will break the compilation.", Verbosity.ALWAYS_SEND);
                return false;
            }

            if (neg) expression = expression.Substring(1, expression.Length - 1);

            var val = defs.Check(expression);

            val = neg ? !val : val;

            return val;
        }
        private string FixCondition(string line)
        {

            Logger.Log(DebugLevel.LOGS, "Fixing condition: " + line, Verbosity.LEVEL4);
            List<char> ret = new List<char>();

            string r = line;
            r = SurroundWithSpaces(r, _orOperator);
            r = SurroundWithSpaces(r, _andOperator);
            r = SurroundWithSpaces(r, "(");
            r = SurroundWithSpaces(r, ")");
            string rr = RemoveExcessSpaces(r);

            Logger.Log(DebugLevel.LOGS, "Fixed condition(new): " + rr, Verbosity.LEVEL4);
            return rr;

        }

        private static int IndexOfClosingBracket(string[] expression, int openBracketIndex)
        {
            Logger.Log(DebugLevel.LOGS, "Finding Closing Bracket...", Verbosity.LEVEL5);
            int tolerance = 0;
            for (int i = openBracketIndex + 1; i < expression.Length; i++)
            {
                if (expression[i] == "(")
                {
                    Logger.Log(DebugLevel.LOGS, "Found Nested opening Bracket, adjusting tolerance.", Verbosity.LEVEL5);
                    tolerance++;
                }
                else if (expression[i] == ")")
                {
                    if (tolerance == 0)
                    {

                        Logger.Log(DebugLevel.LOGS, "Found Correct Closing Bracket", Verbosity.LEVEL5);
                        return i;
                    }
                    Logger.Log(DebugLevel.LOGS, "Found Nested Closing Bracket, adjusting tolerance.", Verbosity.LEVEL5);
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
            Logger.Log(DebugLevel.LOGS, "Removing Excess Spaces: " + line + " => " + ret, Verbosity.LEVEL5);
            return ret;
        }

        private string RemoveExcessSpaces(string line)
        {
            string ret = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Unpack(_separator);
            Logger.Log(DebugLevel.LOGS, "Removing Excess Spaces: " + line + " => " + ret, Verbosity.LEVEL5);
            return ret;
        }

        private static bool IsKeyWord(string line, string keyword)
        {
            string tmp = line.TrimStart();

            return tmp.StartsWith(keyword);
        }

    }
}