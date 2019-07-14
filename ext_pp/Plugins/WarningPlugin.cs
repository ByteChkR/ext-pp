using System.Collections.Generic;
using ext_pp.settings;

namespace ext_pp.plugins
{
    public class WarningPlugin:IPlugin
    {
        private readonly string _warningKeyword;
        private readonly string _separator;


        public WarningPlugin(Settings settings)
        {
            _warningKeyword = settings.WarningStatement;
            _separator = settings.Separator.ToString();
        }


        public bool Process(SourceScript file,SourceManager todo, Definitions defs)
        {
            string[] warnings = Utils.FindStatements(file.Source, _warningKeyword);
            foreach (var t in warnings)
            {
                Logger.Log(DebugLevel.WARNINGS, "Warning: (" + file.Filepath + "): " + t.GetStatementValues(_separator).Unpack(_separator), Verbosity.ALWAYS_SEND);
            }

            return true;
        }

    }
}