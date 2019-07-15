using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp_plugins
{
    public class WarningPlugin : IPlugin
    {
        public string[] Cleanup => new string[0];
        public string[] Prefix => new string[] { "wrn" };
        public bool IncludeGlobal => true;
        public string WarningKeyword = "#warning";
        public string Separator = " ";

        public Dictionary<string, FieldInfo> Info { get; } = new Dictionary<string, FieldInfo>()
        {
            {"w", PropertyHelper.GetFieldInfo(typeof(WarningPlugin), nameof(WarningKeyword))},
            {"s", PropertyHelper.GetFieldInfo(typeof(WarningPlugin), nameof(Separator))}
        };
        public void Initialize(Settings settings, ISourceManager sourceManager, IDefinitions defTable)
        {

            settings.ApplySettingsFlatString(Info, this);
        }

        public bool Process(ISourceScript file, ISourceManager todo, IDefinitions defs)
        {
            Logger.Log(DebugLevel.LOGS, "Discovering Warnings...", Verbosity.LEVEL4);
            string[] warnings = Utils.FindStatements(file.GetSource(), WarningKeyword);
            foreach (var t in warnings)
            {
                Logger.Log(DebugLevel.ERRORS, "Warning(" + Path.GetFileName(file.GetFilePath()) + "): " + warnings.Unpack(Separator), Verbosity.LEVEL1);
            }

            Logger.Log(DebugLevel.LOGS, "Warning Detection Finished", Verbosity.LEVEL4);
            return true;
        }

    }
}