using System.Globalization;
using ext_pp_base.settings;

namespace ext_pp_base
{
    /// <summary>
    /// AbstractLinePlugin but with preconfigured PluginTypeToggle, Process Stages and only one function for all passes.
    /// </summary>
    public abstract class AbstractLinePlugin : AbstractPlugin
    {
        /// <summary>
        /// Specifies the plugin type. Fullscript or Line Script
        /// </summary>
        public override PluginType PluginTypeToggle => (Order.ToLower(CultureInfo.InvariantCulture) == "after" ? PluginType.LINE_PLUGIN_AFTER : PluginType.LINE_PLUGIN_BEFORE);

        /// <summary>
        /// Specifies the order on what "event" the plugin should execute
        /// </summary>
        public override ProcessStage ProcessStages => Stage.ToLower(CultureInfo.InvariantCulture) == "onload" ? ProcessStage.ON_LOAD_STAGE : ProcessStage.ON_FINISH_UP;

        public string Order { get; set; } = "after";
        public string Stage { get; set; } = "onfinishup";

        /// <summary>
        /// Gets called once per line on each file.
        /// </summary>
        /// <param name="source">the source line</param>
        /// <returns>The updated line</returns>
        public override string OnLoad_LineStage(string source)
        {
            return LineStage(source);
        }
        /// <summary>
        /// Gets called once per line on each file.
        /// </summary>
        /// <param name="source">the source line</param>
        /// <returns>The updated line</returns>
        public override string OnMain_LineStage(string source)
        {
            return LineStage(source);
        }
        /// <summary>
        /// Gets called once per line on each file.
        /// </summary>
        /// <param name="source">the source line</param>
        /// <returns>The updated line</returns>
        public override string OnFinishUp_LineStage(string source)
        {
            return LineStage(source);
        }
        /// <summary>
        /// Gets called once per line on each file.
        /// </summary>
        /// <param name="source">the source line</param>
        /// <returns>The updated line</returns>
        public abstract string LineStage(string source);
    }
}