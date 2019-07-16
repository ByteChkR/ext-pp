using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp_plugins
{
    public class WarningPlugin : AbstractPlugin
    {
        public override string[] Prefix => new string[] { "wrn" };
        public override ProcessStage ProcessStages => OnLoad ? ProcessStage.ON_LOAD_STAGE : ProcessStage.ON_FINISH_UP;
        public override PluginType PluginType => FullScript ?
            PluginType.FULL_SCRIPT_PLUGIN :
            (After ? PluginType.LINE_PLUGIN_AFTER : PluginType.LINE_PLUGIN_BEFORE);

        public bool FullScript = false;
        public bool After = true;
        public bool OnLoad = false;
        public string WarningKeyword = "#warning";
        public string Separator = " ";

        public override List<CommandInfo> Info { get; } = new List<CommandInfo>()
        {
            new CommandInfo("w", PropertyHelper.GetFieldInfo(typeof(WarningPlugin), nameof(WarningKeyword)),
                "Sets the keyword that will be used to trigger warnings during compilation"),
            new CommandInfo("s", PropertyHelper.GetFieldInfo(typeof(WarningPlugin), nameof(Separator)),
                "Sets the characters that will be used to separate strings"),
            new CommandInfo("f", PropertyHelper.GetFieldInfo(typeof(ErrorPlugin), nameof(FullScript)),
                "Sets the Plugin Type from line plugin to Full script plugin. To give more debug output at cost of performance"),
            new CommandInfo("a", PropertyHelper.GetFieldInfo(typeof(ErrorPlugin), nameof(After)),
                "Sets the Plugin type to be executed after the full script plugins"),
            new CommandInfo("l", PropertyHelper.GetFieldInfo(typeof(ErrorPlugin), nameof(OnLoad)),
                "Sets the Plugin type to be On Load instead of On Finish Up"),
        };
        public override void Initialize(Settings settings, ISourceManager sourceManager, IDefinitions defTable)
        {
            settings.ApplySettings(Info, this);
        }

        public override string OnLoad_LineStage(string source)
        {
            return LineStage(source);
        }

        public override string OnMain_LineStage(string source)
        {
            return LineStage(source);
        }

        public override string OnFinishUp_LineStage(string source)
        {
            return LineStage(source);
        }

        public override bool OnLoad_FullScriptStage(ISourceScript script, ISourceManager sourceManager, IDefinitions defTable)
        {
            return FullScriptStage(script, sourceManager, defTable);
        }

        public override bool OnMain_FullScriptStage(ISourceScript script, ISourceManager sourceManager, IDefinitions defTable)
        {
            return FullScriptStage(script, sourceManager, defTable);
        }



        public string LineStage(string source)
        {
            if (!Utils.IsStatement(source, WarningKeyword)) return source;
            string err = Utils.SplitAndRemoveFirst(source, Separator).Unpack(" ");
            Logger.Log(DebugLevel.ERRORS, "Warning: " + err, Verbosity.LEVEL1);
            return "";
        }


        public bool FullScriptStage(ISourceScript file, ISourceManager todo, IDefinitions defs)
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