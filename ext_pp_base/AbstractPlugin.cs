using System.Collections.Generic;
using System.Reflection;
using ext_pp_base.settings;

namespace ext_pp_base
{
    /// <summary>
    /// Specifies the functionality needed to be incorporated in the processing chain of ext_pp
    /// </summary>
    public abstract class AbstractPlugin
    {

        public abstract string[] Prefix { get; }
        public virtual bool IncludeGlobal => false;
        public virtual PluginType PluginType => PluginType.FULL_SCRIPT_PLUGIN;
        public virtual ProcessStage ProcessStages => ProcessStage.ON_MAIN;

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

        public virtual bool OnLoad_FullScriptStage(ISourceScript script, ISourceManager sourceManager,
            IDefinitions defTable)
        {
            return true;
        }

        public virtual string OnLoad_LineStage(string source)
        {
            return source;
        }

        public virtual string OnMain_LineStage(string source)
        {
            return source;
        }

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