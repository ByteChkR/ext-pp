using System.Collections.Generic;
using System.Dynamic;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp_plugins
{
    public class BlankLineRemover : AbstractPlugin
    {
        public override PluginType PluginType => (Order.ToLower() == "after" ? PluginType.LINE_PLUGIN_AFTER : PluginType.LINE_PLUGIN_BEFORE);
        public override ProcessStage ProcessStages => Stage.ToLower() == "onload" ? ProcessStage.ON_LOAD_STAGE : ProcessStage.ON_FINISH_UP;

        public string Order { get; set; } = "after";
        public string Stage { get; set; } = "onfinishup";
        public string BlankLineRemovalKeyword { get; set; } = "###remove###";
        public override string[] Prefix => new[] { "blr", "BLRemover" };
        public override string[] Cleanup => new[] { BlankLineRemovalKeyword };


        public override List<CommandInfo> Info { get; } = new List<CommandInfo>()
        {
            new CommandInfo("set-removekeyword", "k", PropertyHelper.GetPropertyInfo(typeof(BlankLineRemover), nameof(BlankLineRemovalKeyword)),
                "This will get inserted whenever a blank line is detected. This will be removed in the native cleanup of the PreProcessor"),
            new CommandInfo("set-order", "o", PropertyHelper.GetPropertyInfo(typeof(BlankLineRemover), nameof(Order)),
                "Sets the Line Order to be Executed BEFORE the Fullscripts or AFTER the Fullscripts"),
            new CommandInfo("set-stage","ss", PropertyHelper.GetPropertyInfo(typeof(BlankLineRemover), nameof(Stage)),
                "Sets the Stage Type of the Plugin to be Executed OnLoad or OnFinishUp"),
        };


        public override void Initialize(Settings settings, ISourceManager sourceManager, IDefinitions defs)
        {
            settings.ApplySettings(Info, this);

        }

        public override bool OnLoad_FullScriptStage(ISourceScript script, ISourceManager sourceManager,
            IDefinitions defTable)
        {
            return FullScriptStage(script, sourceManager, defTable);
        }

        public override bool OnMain_FullScriptStage(ISourceScript script, ISourceManager sourceManager,
            IDefinitions defTable)
        {
            return FullScriptStage(script, sourceManager, defTable);
        }

        public bool FullScriptStage(ISourceScript file, ISourceManager todo, IDefinitions defs)
        {
            return true;
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


        public string LineStage(string source)
        {
            if (source.Trim() == "")
            {
                this.Log(DebugLevel.LOGS, Verbosity.LEVEL6, "Adding {0} for line removal later", BlankLineRemovalKeyword);
                return BlankLineRemovalKeyword;
            }
            return source;
        }

    }
}