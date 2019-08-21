using ext_pp_base.settings;

namespace ext_pp_base
{
    /// <summary>
    /// AbstractLinePlugin but with fixed plugin type toggle
    /// </summary>
    public abstract class AbstractLineAfterPlugin : AbstractLinePlugin
    {
        /// <summary>
        /// Specifies the plugin type. Fullscript or Line Script
        /// </summary>
        public override PluginType PluginTypeToggle { get; } = PluginType.LINE_PLUGIN_AFTER;
    }
}