using System.IO;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp_plugins
{
    public class WarningPlugin : IPlugin
    {
        public string[] Cleanup => new string[0];
        private readonly string _warningKeyword = Settings.WarningStatement;
        private readonly string _separator = Settings.Separator;


        public void Initialize(Settings settings, ISourceManager sourceManager, IDefinitions defTable)
        {

        }

        public bool Process(ISourceScript file, ISourceManager todo, IDefinitions defs)
        {
            Logger.Log(DebugLevel.LOGS, "Discovering Warnings...", Verbosity.LEVEL3);
            string[] warnings = Utils.FindStatements(file.GetSource(), _warningKeyword);
            foreach (var t in warnings)
            {
                Logger.Log(DebugLevel.ERRORS, "Warning(" + Path.GetFileName(file.GetFilePath()) + "): " + warnings.Unpack(_separator), Verbosity.ALWAYS_SEND);
            }

            Logger.Log(DebugLevel.LOGS, "Warning Detection Finished", Verbosity.LEVEL3);
            return true;
        }

    }
}