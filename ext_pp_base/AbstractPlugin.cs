using System.Collections.Generic;
using System.Linq;
using ext_pp_base.settings;

namespace ext_pp_base
{
    /// <summary>
    /// Specifies the functionality needed to be incorporated in the processing chain of ext_pp
    /// </summary>
    public abstract class AbstractPlugin : ILoggable
    {

        /// <summary>
        /// Returns the plugins that are meant to be run at the specified stage
        /// </summary>
        /// <param name="plugins"></param>
        /// <param name="type"></param>
        /// <param name="stage"></param>
        /// <returns></returns>
        public static List<AbstractPlugin> GetPluginsForStage(List<AbstractPlugin> plugins, PluginType type, ProcessStage stage)
        {
            return plugins.Where(
                x => ADL.BitMask.IsContainedInMask((int)x.PluginTypeToggle, (int)type, true) &&
                     ADL.BitMask.IsContainedInMask((int)x.ProcessStages, (int)stage, true)).ToList();
        }

        /// <summary>
        /// Returns a list of prefixes the plugin should be able to listen to when receiving settings
        /// </summary>
        public abstract string[] Prefix { get; }
        /// <summary>
        /// A flag that will, when turned on, redirect all settings that have a global prefix
        /// </summary>
        public virtual bool IncludeGlobal => false;

        /// <summary>
        /// Specifies the plugin type. Fullscript or Line Script
        /// </summary>
        public virtual PluginType PluginTypeToggle => PluginType.FULL_SCRIPT_PLUGIN;
        /// <summary>
        /// Specifies the order on what "event" the plugin should execute
        /// </summary>
        public virtual ProcessStage ProcessStages => ProcessStage.ON_MAIN;

        /// <summary>
        /// A list of command infos. This list contains all the different commands of the plugin/program
        /// </summary>
        public virtual List<CommandInfo> Info => new List<CommandInfo>();

        /// <summary>
        /// Gets called once on each file.
        /// Looping Through All the Files
        ///     Looping Through All the plugins
        /// </summary>
        /// <param name="script"></param>
        /// <param name="sourceManager"></param>
        /// <param name="defTable"></param>
        /// <returns>state of the process(if false will abort processing)</returns>
        public virtual bool OnMain_FullScriptStage(ISourceScript script, ISourceManager sourceManager,
            IDefinitions defTable)
        {
            return true;
        }

        /// <summary>
        /// Gets called once on each file.
        /// Looping Through All the Files
        ///     Looping Through All the plugins
        /// </summary>
        /// <param name="script"></param>
        /// <param name="sourceManager"></param>
        /// <param name="defTable"></param>
        /// <returns>state of the process(if false will abort processing)</returns>
        public virtual bool OnLoad_FullScriptStage(ISourceScript script, ISourceManager sourceManager,
            IDefinitions defTable)
        {
            return true;
        }

        /// <summary>
        /// Gets called once per line on each file.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public virtual string OnLoad_LineStage(string source)
        {
            return source;
        }

        /// <summary>
        /// Gets called once per line on each file.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public virtual string OnMain_LineStage(string source)
        {
            return source;
        }

        /// <summary>
        /// Gets called once per line on each file.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public virtual string OnFinishUp_LineStage(string source)
        {
            return source;
        }

        /// <summary>
        /// A list of statements that need to be removed as a last step of the processing routine
        /// </summary>
        public virtual string[] Cleanup => new string[0];

        /// <summary>
        /// Initialization of the plugin
        /// Set all your changes to the objects here(not in the actual processing)
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="sourceManager"></param>
        /// <param name="defTable"></param>
        public abstract void Initialize(Settings settings, ISourceManager sourceManager, IDefinitions defTable);

    }
}