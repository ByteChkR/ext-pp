using System.Collections.Generic;

namespace ext_compiler.settings
{
    public class Settings
    {
        public static string HelpText =
            "Parameter:\r\n-i|--input <path>\r\n-o|--output <path>\r\nOptional Parameter:\r\n-rd|--resolveDefine [true|false]\r\n-ru|--resolveUndefine [true|false]\r\n-rc|--resolveConditions [true|false]\r\n-ri|--resolveInclude [true|false]\r\n-rg|--resolveGenerics [true|false]\r\n-ee|--enableErrors [true|false]\r\n-ew|--enableWarnings [true|false]\r\n-def|--defines [DefineSymbols]\r\n-v|--verbosity [0(Silent)-10(Maximum Debug Log)]";

        public bool ResolveDefine = true;
        public bool ResolveUnDefine = true;
        public bool ResolveConditions = true;
        public bool ResolveIncludes = true;
        public bool ResolveGenerics = true;
        public bool EnableWarnings = true;
        public bool EnableErrors = true;
        public Verbosity VerbosityLevel = Verbosity.ALWAYS_SEND;
        public Keywords Keywords = new Keywords();


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