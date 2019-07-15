using System.Collections.Generic;
using System.Reflection;

namespace ext_pp_base.settings
{
    public class Settings
    {
        public static string HelpText =
            "Parameter:\r\n-i|--input <path>\r\n-o|--output <path>\r\n Optional Parameter:	\t-rd|--resolveDefine [true|false]  \r\n\t-ru|--resolveUndefine [true|false]  \r\n\t-rc|--resolveConditions [true|false]  \r\n\t-ri|--resolveInclude [true|false]  \r\n\t-rg|--resolveGenerics [true|false]  \r\n\t-ee|--enableErrors [true|false]  \r\n\t-ew|--enableWarnings [true|false]  \r\n\t-def|--defines [DefineSymbols]  \r\n\t-v|--verbosity [0(Silent)-10(Maximum Debug Log)]\r\n\t-ss|--setSeparator [char]\r\n\t-2c|--writeToConsole\r\n\t-kw:d|--keyWord:d [defineStatement]\r\n\t-kw:u|--keyWord:u [unDefineStatement]\r\n\t-kw:if|--keyWord:if [ifStatement]\r\n\t-kw:elif|--keyWord:elif [elseIfStatement]\r\n\t-kw:else|--keyWord:else [elseStatement]\r\n\t-kw:eif|--keyWord:eif [endIfStatement]\r\n\t-kw:w|--keyWord:w [warningStatement]\r\n\t-kw:e|--keyWord:e [errorStatement]\r\n\t-kw:i|--keyWord:i [includeStatement]\r\n\t-kw:t|--keyWord:t [typeGenStatement]";

        private readonly Dictionary<string, string> _settings;

        public Settings()
        {
            _settings = new Dictionary<string, string>();
        }

        public void Set(string key, string value)
        {
            if (_settings.ContainsKey(key)) _settings[key] = value;
            else _settings.Add(key, value);
            Logger.Log(DebugLevel.LOGS, key + " changed to: " + value, Verbosity.LEVEL1);
        }

        public bool HasKey(string key)
        {
            return _settings.ContainsKey(key);
        }

        public string Get(string key)
        {
            return _settings[key];
        }
        
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
                {"n", PropertyHelper.GetFieldInfo(typeof(Settings), nameof(NotOperator))},
                {"a", PropertyHelper.GetFieldInfo(typeof(Settings), nameof(AndOperator))},
                {"o", PropertyHelper.GetFieldInfo(typeof(Settings), nameof(OrOperator))},
                {"s", PropertyHelper.GetFieldInfo(typeof(Settings), nameof(Separator))},
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
        public static string NotOperator = "!";
        public static string AndOperator = "&&";
        public static string OrOperator = "||";
        public static string Separator = " ";


    }
}