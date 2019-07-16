using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp_plugins
{
    public class ErrorPlugin : AbstractPlugin
    {

        public override string[] Prefix => new string[] { "err" };

        public override ProcessStage ProcessStages => OnLoad ? ProcessStage.ON_LOAD_STAGE : ProcessStage.ON_FINISH_UP;
        public override PluginType PluginType => FullScript ?
            PluginType.FULL_SCRIPT_PLUGIN :
            (After ? PluginType.LINE_PLUGIN_AFTER : PluginType.LINE_PLUGIN_BEFORE);

        public bool FullScript = false;
        public bool After = true;
        public bool OnLoad = false;
        public string ErrorKeyword = "#error";
        public string Separator = " ";
        public override List<CommandInfo> Info { get; } = new List<CommandInfo>()
        {
            new CommandInfo("e", PropertyHelper.GetFieldInfo(typeof(ErrorPlugin), nameof(ErrorKeyword)),
                "Sets the keyword that will be used to trigger errors during compilation"),
            new CommandInfo("s", PropertyHelper.GetFieldInfo(typeof(ErrorPlugin), nameof(Separator)),
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
            if (!Utils.IsStatement(source, ErrorKeyword)) return source;
            string err = Utils.SplitAndRemoveFirst(source, Separator).Unpack(" ");
            Logger.Log(DebugLevel.ERRORS, "Error " + err, Verbosity.LEVEL1);
            return "";
        }



        public bool FullScriptStage(ISourceScript file, ISourceManager todo, IDefinitions defs)
        {
            Logger.Log(DebugLevel.LOGS, "Discovering Errors...", Verbosity.LEVEL4);
            string[] errors = Utils.FindStatements(file.GetSource(), ErrorKeyword);
            foreach (var t in errors)
            {
                Logger.Log(DebugLevel.ERRORS, "Error(" + Path.GetFileName(file.GetFilePath()) + "): " + errors.Unpack(Separator), Verbosity.LEVEL1);
            }

            Logger.Log(DebugLevel.LOGS, "Error Detection Finished", Verbosity.LEVEL4);
            return errors.Length == 0;
        }

    }
}