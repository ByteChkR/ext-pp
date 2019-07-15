using System.Collections.Generic;
using System.Reflection;
using ext_pp_base.settings;

namespace ext_pp_base
{
   /// <summary>
   /// Specifies the functionality needed to be incorporated in the processing chain of ext_pp
   /// </summary>
    public interface IPlugin
    {

        string[] Prefix { get; }
        bool IncludeGlobal { get; }
        List<CommandInfo> Info { get; }

        /// <summary>
        /// Gets called once on each file.
        /// Looping Through All the Files
        ///     Looping Through All the plugins
        /// </summary>
        /// <param name="script"></param>
        /// <param name="sourceManager"></param>
        /// <param name="defTable"></param>
        /// <returns>state of the process(if false will abort processing)</returns>
        bool Process(ISourceScript script, ISourceManager sourceManager, IDefinitions defTable);

        /// <summary>
        /// A list of statements that need to be removed as a last step of the processing routine
        /// </summary>
        string[] Cleanup { get; }

        /// <summary>
        /// Initialization of the plugin
        /// Set all your changes to the objects here(not in the actual processing)
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="sourceManager"></param>
        /// <param name="defTable"></param>
        void Initialize(Settings settings, ISourceManager sourceManager, IDefinitions defTable);

    }
}