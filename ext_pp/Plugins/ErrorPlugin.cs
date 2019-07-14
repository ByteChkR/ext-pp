using System.Collections.Generic;
using System.IO;
using ext_pp.settings;

namespace ext_pp.plugins
{
    public class ErrorPlugin : IPlugin
    {

        public string[] Cleanup => new string[0];
        private readonly string _errorKeyword = Settings.ErrorStatement;
        private readonly string _separator = Settings.Separator;
        public ErrorPlugin(Settings settings)
        {

        }

        public bool Process(SourceScript file, SourceManager todo, Definitions defs)
        {
            Logger.Log(DebugLevel.LOGS, "Discovering Errors...", Verbosity.LEVEL3);
            string[] errors = Utils.FindStatements(file.Source, _errorKeyword);
            foreach (var t in errors)
            {
                Logger.Log(DebugLevel.ERRORS, "Error(" + Path.GetFileName(file.Filepath) + "): " + errors.Unpack(_separator), Verbosity.ALWAYS_SEND);
            }

            Logger.Log(DebugLevel.LOGS, "Error Detection Finished", Verbosity.LEVEL3);
            return errors.Length == 0;
        }

    }
}