using ext_pp_base.settings;

namespace ext_pp_base
{
    /// <summary>
    /// Abstract Plugin but with OnLoad_FullScriptStage and OnMain_FullScriptStage overriden and fixed plugin type toggle
    /// </summary>
    public abstract class AbstractFullScriptPlugin : AbstractPlugin
    {
        /// <summary>
        /// Specifies the plugin type. Fullscript or Line Script
        /// </summary>
        public override PluginType PluginTypeToggle { get; } = PluginType.FULL_SCRIPT_PLUGIN;
        /// <summary>
        /// Gets called once on each file.
        /// Looping Through All the Files
        ///     Looping Through All the plugins
        /// </summary>
        /// <param name="script">the current source script</param>
        /// <param name="sourceManager">the current source manager</param>
        /// <param name="defTable">the current definitions</param>
        /// <returns>state of the process(if false will abort processing)</returns>
        public override bool OnLoad_FullScriptStage(ISourceScript script, ISourceManager sourceManager, IDefinitions defTable)
        {
            return FullScriptStage(script, sourceManager, defTable);
        }
        /// <summary>
        /// Gets called once on each file.
        /// Looping Through All the Files
        ///     Looping Through All the plugins
        /// </summary>
        /// <param name="script">the current source script</param>
        /// <param name="sourceManager">the current source manager</param>
        /// <param name="defTable">the current definitions</param>
        /// <returns>state of the process(if false will abort processing)</returns>
        public override bool OnMain_FullScriptStage(ISourceScript script, ISourceManager sourceManager, IDefinitions defTable)
        {
            return FullScriptStage(script, sourceManager, defTable);
        }
        /// <summary>
        /// Gets called once on each file.
        /// Looping Through All the Files
        ///     Looping Through All the plugins
        /// </summary>
        /// <param name="script">the current source script</param>
        /// <param name="sourceManager">the current source manager</param>
        /// <param name="defTable">the current definitions</param>
        /// <returns>state of the process(if false will abort processing)</returns>
        public abstract bool FullScriptStage(ISourceScript script, ISourceManager sourceManager,
            IDefinitions defTable);


    }
}