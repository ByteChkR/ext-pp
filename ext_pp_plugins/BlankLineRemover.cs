using System.Collections.Generic;
using System.Dynamic;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp_plugins
{
    public class BlankLineRemover : AbstractPlugin
    {
        public override PluginType PluginType => (After ? PluginType.LINE_PLUGIN_AFTER : PluginType.LINE_PLUGIN_BEFORE);
        public override ProcessStage ProcessStages => OnLoad ? ProcessStage.ON_LOAD_STAGE : ProcessStage.ON_FINISH_UP;

        public bool After = true;
        public bool OnLoad = false;
        public string BlankLineRemovalKeyword = "###remove###";
        public override string[] Prefix => new[] { "blr" };
        public override string[] Cleanup => new[] { BlankLineRemovalKeyword };


        public override List<CommandInfo> Info { get; } = new List<CommandInfo>()
        {
            new CommandInfo("k", PropertyHelper.GetFieldInfo(typeof(BlankLineRemover), nameof(After)),
                "Sets the Plugin keyword that gets used when removing blank lines from the files."),
            new CommandInfo("a", PropertyHelper.GetFieldInfo(typeof(BlankLineRemover), nameof(After)),
                "Sets the Plugin type to be executed after the full script plugins"),
            new CommandInfo("l", PropertyHelper.GetFieldInfo(typeof(BlankLineRemover), nameof(OnLoad)),
                "Sets the Plugin type to be On Load instead of On Finish Up"),
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
                Logger.Log(DebugLevel.LOGS, "Adding " + BlankLineRemovalKeyword +" for line removal later", Verbosity.LEVEL4);
                return BlankLineRemovalKeyword;
            }
            return source;
        }

    }
}