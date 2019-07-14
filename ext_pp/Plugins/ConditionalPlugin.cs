using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using ext_pp.settings;

namespace ext_pp.plugins
{
    public class ConditionalPlugin : IPlugin
    {
        private readonly string _startConditional;
        private readonly string _fallbackConditionalKeyword;
        private readonly string _fallbackKeyword;
        private readonly string _endConditional;
        private readonly string _uDefKeyword;
        private readonly string _defKeyword;
        private readonly string _orOperator;
        private readonly string _notOperator;
        private readonly string _andOperator;
        private readonly string _separator;
        private readonly bool _enableDef;
        private readonly bool _enableUdef;

        public ConditionalPlugin(Settings settings)
        {
            _startConditional = settings.IfStatement;
            _fallbackConditionalKeyword = settings.ElseIfStatement;
            _fallbackKeyword = settings.ElseStatement;
            _endConditional = settings.EndIfStatement;
            _defKeyword = settings.DefineStatement;
            _uDefKeyword = settings.UndefineStatement;
            _enableDef = settings.ResolveDefine;
            _enableUdef = settings.ResolveUnDefine;
            _orOperator = settings.OrOperator;
            _andOperator = settings.AndOperator;
            _notOperator = settings.NotOperator;
            _separator = settings.Separator;
        }

        public bool Process(SourceScript file, SourceManager todo, Definitions defs)
        {
            bool ret = true;
            int openIf = 0;
            bool foundConditions = false;
            bool elseIsValid = false;
            List<string> lastPass = file.Source.ToList();
            List<string> solvedFile = new List<string>();

            do
            {
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
                            }

                            i += size;
                            foundConditions = true;
                        }
                        else
                        {
                            Logger.Crash(new Exception("the else if statement: " +
                                                       _fallbackConditionalKeyword +
                                                       " must be preceded with " +
                                                       _startConditional), false);
                            ret = false;
                            break;
                        }
                    }
                    else if (IsKeyWord(line, _fallbackKeyword))
                    {
                        if (openIf > 0)
                        {
                            var size = GetBlockSize(lastPass, i);
                            Logger.Log(DebugLevel.LOGS, "Found Conditional Else Statement: " + line, Verbosity.LEVEL5);
                            if (elseIsValid)
                                solvedFile.AddRange(lastPass.SubArray(i + 1, size));

                            i += size;
                            foundConditions = true;
                        }
                        else
                        {
                            Logger.Crash(new Exception("the else statement: " +
                                                       _fallbackKeyword +
                                                       " must be preceded with " +
                                                       _startConditional), false);
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

                            Logger.Crash(new Exception("the endif statement: " +
                                                       _endConditional +
                                                       " must be preceded with " +
                                                       _startConditional), false);
                            ret = false;
                            break;
                        }
                    }
                    else if (_enableDef &&
                             line.StartsWith(_defKeyword))
                    {
                        defs.Set(line.GetStatementValues(_separator));
                        solvedFile.Add(lastPass[i]);
                    }
                    else if (_enableUdef &&
                             line.StartsWith(_uDefKeyword))
                    {
                        defs.Unset(line.GetStatementValues(_separator));
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

            file.Source = lastPass.ToArray();
            return ret;
        }


        private int GetBlockSize(IReadOnlyList<string> source, int start)
        {
            var tolerance = 0;
            for (var i = start + 1; i < source.Count; i++)
            {
                var line = source[i].Trim();
                if (line.StartsWith(_startConditional))
                {
                    i += GetBlockSize(source, i);
                    tolerance++;
                }

                else if (line.StartsWith(_endConditional) ||
                         line.StartsWith(_fallbackConditionalKeyword) ||
                         line.StartsWith(_fallbackKeyword))
                {
                    if (tolerance == 0)
                        return i - start - 1;
                    if (line.StartsWith(_endConditional)) tolerance--;
                }
            }

            Logger.Crash(new Exception("Invalid usage of If statement")
            {
                Data = { { "source", source }, { "start", start } }
            });
            return -1; //Not getting here since it crashes in Logger.Crash
        }

        private bool EvaluateConditional(string expression, Definitions defs)
        {
            string condition = FixCondition(expression.GetStatementValues(_separator).Unpack(_separator));

            string[] cs = condition.Pack(_separator).ToArray();
            return EvaluateConditional(cs, defs);
        }
        private bool EvaluateConditional(string[] expression, Definitions defs)
        {


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

        private bool EvaluateExpression(string expression, Definitions defs)
        {
            Logger.Log(DebugLevel.LOGS, "Evaluating Expression: " + expression, Verbosity.LEVEL7);

            bool neg = expression.StartsWith(_notOperator);
            if (expression == _notOperator)
                Logger.Crash(new InvalidOperationException("A Negation should be followed immediately with a variable"));
            if (neg) expression = expression.Substring(1, expression.Length - 1);

            var val = defs.Check(expression);

            val = neg ? !val : val;
            Logger.Log(DebugLevel.LOGS, "Expression Evaluated: " + val, Verbosity.LEVEL7);

            return val;
        }
        private string FixCondition(string line)
        {
            List<char> ret = new List<char>();

            string r = line;
            r = SurroundWithSpaces(r, _orOperator);
            r = SurroundWithSpaces(r, _andOperator);
            r = SurroundWithSpaces(r, "(");
            r = SurroundWithSpaces(r, ")");

            return RemoveExcessSpaces(r);

        }

        private static int IndexOfClosingBracket(string[] expression, int openBracketIndex)
        {
            int tolerance = 0;
            for (int i = openBracketIndex + 1; i < expression.Length; i++)
            {
                if (expression[i] == "(")
                {
                    tolerance++;
                }
                else if (expression[i] == ")")
                {
                    if (tolerance == 0) return i;
                    tolerance--;
                }
            }

            return -1;
        }

        private static string SurroundWithSpaces(string line, string keyword)
        {
            StringBuilder sb = new StringBuilder(line);
            sb.Replace(keyword, " " + keyword + " ");
            return sb.ToString();
        }

        private string RemoveExcessSpaces(string line)
        {
            return line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Unpack(_separator);
        }

        private static bool IsKeyWord(string line, string keyword)
        {
            string tmp = line.TrimStart();

            return tmp.StartsWith(keyword);
        }

    }
}