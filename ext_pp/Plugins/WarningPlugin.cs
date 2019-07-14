using System.Collections.Generic;
using System.IO;
using ext_pp.settings;

namespace ext_pp.plugins
{
    public class WarningPlugin:IPlugin
    {
        public string[] Cleanup => new string[0];
        private readonly string _warningKeyword=Settings.WarningStatement;
        private readonly string _separator = Settings.Separator;


        public WarningPlugin(Settings settings)
        {

        }


        public bool Process(SourceScript file,SourceManager todo, Definitions defs)
        {
            Logger.Log(DebugLevel.LOGS, "Discovering Warnings...", Verbosity.LEVEL3);
            string[] warnings = Utils.FindStatements(file.Source, _warningKeyword);
            foreach (var t in warnings)
            {
                Logger.Log(DebugLevel.ERRORS, "Warning(" + Path.GetFileName(file.Filepath) + "): " + warnings.Unpack(_separator), Verbosity.ALWAYS_SEND);
            }

            Logger.Log(DebugLevel.LOGS, "Warning Detection Finished", Verbosity.LEVEL3);
            return true;
        }

    }
}