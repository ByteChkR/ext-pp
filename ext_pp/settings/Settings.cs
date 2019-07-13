using System;
using System.Collections.Generic;
using System.Reflection;

namespace ext_pp.settings
{
    public static class Settings
    {
        public static string HelpText =
            "Parameter:\r\n-i|--input <path>\r\n-o|--output <path>\r\n Optional Parameter:	\t-rd|--resolveDefine [true|false]  \r\n\t-ru|--resolveUndefine [true|false]  \r\n\t-rc|--resolveConditions [true|false]  \r\n\t-ri|--resolveInclude [true|false]  \r\n\t-rg|--resolveGenerics [true|false]  \r\n\t-ee|--enableErrors [true|false]  \r\n\t-ew|--enableWarnings [true|false]  \r\n\t-def|--defines [DefineSymbols]  \r\n\t-v|--verbosity [0(Silent)-10(Maximum Debug Log)]\r\n\t-ss|--setSeparator [char]\r\n\t-2c|--writeToConsole\r\n\t-kw:d|--keyWord:d [defineStatement]\r\n\t-kw:u|--keyWord:u [unDefineStatement]\r\n\t-kw:if|--keyWord:if [ifStatement]\r\n\t-kw:elif|--keyWord:elif [elseIfStatement]\r\n\t-kw:else|--keyWord:else [elseStatement]\r\n\t-kw:eif|--keyWord:eif [endIfStatement]\r\n\t-kw:w|--keyWord:w [warningStatement]\r\n\t-kw:e|--keyWord:e [errorStatement]\r\n\t-kw:i|--keyWord:i [includeStatement]\r\n\t-kw:t|--keyWord:t [typeGenStatement]";

        public static bool ResolveDefine = true;
        public static bool ResolveUnDefine = true;
        public static bool ResolveConditions = true;
        public static bool ResolveIncludes = true;
        public static bool ResolveGenerics = true;
        public static bool EnableWarnings = true;
        public static bool EnableErrors = true;
        public static Verbosity VerbosityLevel = Verbosity.ALWAYS_SEND;
        public static bool WriteToConsole = false;


        private static Dictionary<string, FieldInfo> _keyWordHandles = null;

        public static Dictionary<string, FieldInfo> KeyWordHandles => _keyWordHandles ?? (_keyWordHandles = GetInfo());

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

        public static string IncludeStatement = "#include";
        public static string TypeGenKeyword = "#type";
        public static string ErrorStatement = "#error";
        public static string WarningStatement = "#warning";
        public static string IfStatement = "#if";
        public static string ElseIfStatement = "#elseif";
        public static string ElseStatement = "#else";
        public static string EndIfStatement = "#endif";
        public static string DefineStatement = "#define";
        public static string UndefineStatement = "#undefine";
        public static char Separator = ' ';

        public static List<string> CleanUpList
        {
            get
            {
                var ret = new List<string>();

                if (!ResolveConditions) ResolveDefine = ResolveUnDefine = false;

                if (ResolveUnDefine) ret.Add(UndefineStatement);
                if (ResolveDefine) ret.Add(DefineStatement);
                if (ResolveConditions) ret.AddRange(new []
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