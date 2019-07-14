using System.Collections.Generic;
using ext_pp.settings;

namespace ext_pp.plugins
{
    public class ErrorPlugin : IPlugin
    {

        private readonly string _errorKeyword;
        private readonly string _separator;
        public ErrorPlugin(Settings settings)
        {
            _errorKeyword = settings.ErrorStatement;
            _separator = settings.Separator.ToString();
        }

        public bool Process(SourceScript file, SourceManager todo, Definitions defs)
        {
            string[] warnings = Utils.FindStatements(file.Source, _errorKeyword);
            foreach (var t in warnings)
            {
                Logger.Log(DebugLevel.WARNINGS, "Warning: (" + file.Filepath + "): " + t.GetStatementValues(_separator).Unpack(_separator), Verbosity.ALWAYS_SEND);
            }

            return true;
        }

    }
}