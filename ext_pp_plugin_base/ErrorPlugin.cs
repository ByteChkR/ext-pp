using System.IO;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp_plugins
{
    public class ErrorPlugin : IPlugin
    {

        public string[] Cleanup => new string[0];
        private readonly string _errorKeyword = Settings.ErrorStatement;
        private readonly string _separator = Settings.Separator;
        public ErrorPlugin(Settings settings)
        {

        }

        public bool Process(ASourceScript file, ASourceManager todo, ADefinitions defs)
        {
            Logger.Log(DebugLevel.LOGS, "Discovering Errors...", Verbosity.LEVEL3);
            string[] errors = Utils.FindStatements(file.GetSource(), _errorKeyword);
            foreach (var t in errors)
            {
                Logger.Log(DebugLevel.ERRORS, "Error(" + Path.GetFileName(file.GetFilePath()) + "): " + errors.Unpack(_separator), Verbosity.ALWAYS_SEND);
            }

            Logger.Log(DebugLevel.LOGS, "Error Detection Finished", Verbosity.LEVEL3);
            return errors.Length == 0;
        }

    }
}