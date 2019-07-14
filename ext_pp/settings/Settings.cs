using System;
using System.Collections.Generic;
using System.Reflection;

namespace ext_pp.settings
{
    public class Settings
    {
        public static string HelpText =
            "Parameter:\r\n-i|--input <path>\r\n-o|--output <path>\r\n Optional Parameter:	\t-rd|--resolveDefine [true|false]  \r\n\t-ru|--resolveUndefine [true|false]  \r\n\t-rc|--resolveConditions [true|false]  \r\n\t-ri|--resolveInclude [true|false]  \r\n\t-rg|--resolveGenerics [true|false]  \r\n\t-ee|--enableErrors [true|false]  \r\n\t-ew|--enableWarnings [true|false]  \r\n\t-def|--defines [DefineSymbols]  \r\n\t-v|--verbosity [0(Silent)-10(Maximum Debug Log)]\r\n\t-ss|--setSeparator [char]\r\n\t-2c|--writeToConsole\r\n\t-kw:d|--keyWord:d [defineStatement]\r\n\t-kw:u|--keyWord:u [unDefineStatement]\r\n\t-kw:if|--keyWord:if [ifStatement]\r\n\t-kw:elif|--keyWord:elif [elseIfStatement]\r\n\t-kw:else|--keyWord:else [elseStatement]\r\n\t-kw:eif|--keyWord:eif [endIfStatement]\r\n\t-kw:w|--keyWord:w [warningStatement]\r\n\t-kw:e|--keyWord:e [errorStatement]\r\n\t-kw:i|--keyWord:i [includeStatement]\r\n\t-kw:t|--keyWord:t [typeGenStatement]";

        public bool ResolveDefine = true;
        public bool ResolveUnDefine = true;
        public bool ResolveConditions = true;
        public bool ResolveIncludes = true;
        public bool ResolveGenerics = true;
        public bool EnableWarnings = true;
        public bool EnableErrors = true;
        public Verbosity VerbosityLevel = Verbosity.ALWAYS_SEND;
        public bool WriteToConsole = false;
        public bool LogToFile = false;


        private Dictionary<string, FieldInfo> _keyWordHandles = null;

        public Dictionary<string, FieldInfo> KeyWordHandles => _keyWordHandles ?? (_keyWordHandles = GetInfo());

        private static Dictionary<string, FieldInfo> GetInfo()
        {
            return new Dictionary<string, FieldInfo>()
            {
                {"d", PropertyHelper.GetFieldInfo(typeof(Settings), nameof(DefineStatement))},
                {"u", PropertyHelper.GetFieldInfo(typeof(Settings), nameof(UndefineStatement))},
                {"if", PropertyHelper.GetFieldInfo(typeof(Settings), nameof(IfStatement))},
                {"elif", PropertyHelper.GetFieldInfo(typeof(Settings), nameof(ElseIfStatement))},
                {"else", PropertyHelper.GetFieldInfo(typeof(Settings), nameof(ElseStatement))},
                {"eif", PropertyHelper.GetFieldInfo(typeof(Settings), nameof(EndIfStatement))},
                {"w", PropertyHelper.GetFieldInfo(typeof(Settings), nameof(WarningStatement))},
                {"e", PropertyHelper.GetFieldInfo(typeof(Settings), nameof(ErrorStatement))},
                {"i", PropertyHelper.GetFieldInfo(typeof(Settings), nameof(IncludeStatement))},
                {"t", PropertyHelper.GetFieldInfo(typeof(Settings), nameof(TypeGenKeyword))},
            };
        }

        public string IncludeStatement = "#include";
        public string TypeGenKeyword = "#type";
        public string ErrorStatement = "#error";
        public string WarningStatement = "#warning";
        public string IfStatement = "#if";
        public string ElseIfStatement = "#elseif";
        public string ElseStatement = "#else";
        public string EndIfStatement = "#endif";
        public string DefineStatement = "#define";
        public string UndefineStatement = "#undefine";
        public string NotOperator = "!";
        public string AndOperator = "&&";
        public string OrOperator = "||";
        public string Separator = " ";

        public List<string> CleanUpList
        {
            get
            {
                var ret = new List<string>();

                if (!ResolveConditions) ResolveDefine = ResolveUnDefine = false;

                if (ResolveUnDefine) ret.Add(UndefineStatement);
                if (ResolveDefine) ret.Add(DefineStatement);
                if (ResolveConditions) ret.AddRange(new[]
                {
                    IfStatement ,
                    ElseIfStatement,
                    ElseStatement,
                    EndIfStatement
                });
                if (ResolveIncludes) ret.Add(IncludeStatement);
                if (EnableErrors) ret.Add(ErrorStatement);
                if (EnableWarnings) ret.Add(WarningStatement);
                return ret;

            }
        }
    }
}