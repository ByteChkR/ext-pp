using ext_pp_base.settings;

namespace ext_pp_base
{
    public abstract class AbstractFullScriptPlugin : AbstractPlugin
    {
        public override PluginType PluginTypeToggle { get; } = PluginType.FULL_SCRIPT_PLUGIN;

        public override bool OnLoad_FullScriptStage(ISourceScript script, ISourceManager sourceManager, IDefinitions defTable)
        {
            return FullScriptStage(script, sourceManager, defTable);
        }

        public override bool OnMain_FullScriptStage(ISourceScript script, ISourceManager sourceManager, IDefinitions defTable)
        {
            return FullScriptStage(script, sourceManager, defTable);
        }

        public abstract bool FullScriptStage(ISourceScript script, ISourceManager sourceManager,
            IDefinitions defTable);


    }
}