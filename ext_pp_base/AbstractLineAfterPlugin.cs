using ext_pp_base.settings;

namespace ext_pp_base
{
    public abstract class AbstractLineAfterPlugin : AbstractLinePlugin
    {
        public override PluginType PluginTypeToggle { get; } = PluginType.LINE_PLUGIN_AFTER;
    }
}