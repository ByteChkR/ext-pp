using ext_pp_base.settings;

namespace ext_pp_base
{
    public abstract class AbstractLineBeforePlugin : AbstractLinePlugin
    {

        public override PluginType PluginTypeToggle { get; } = PluginType.LINE_PLUGIN_BEFORE;
    }
}