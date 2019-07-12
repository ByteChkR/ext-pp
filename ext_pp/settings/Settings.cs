using System.Collections.Generic;

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
        public Keywords Keywords = new Keywords();
        public bool WriteToConsole = false;


        public List<string> CleanUpList
        {
            get
            {
                List<string> ret = new List<string>();

                if (!ResolveConditions) ResolveDefine = ResolveUnDefine = false;

                if (ResolveUnDefine) ret.Add(Keywords.UndefineStatement);
                if (ResolveDefine) ret.Add(Keywords.DefineStatement);
                if (ResolveConditions) ret.AddRange(new []{ Keywords.IfStatement , Keywords.ElseIfStatement, Keywords.ElseStatement, Keywords.EndIfStatement });
                if (ResolveIncludes) ret.Add(Keywords.IncludeStatement);
                if (EnableErrors) ret.Add(Keywords.ErrorStatement);
                if (EnableWarnings) ret.Add(Keywords.WarningStatement);
                return ret;

            }
        }
    }
}