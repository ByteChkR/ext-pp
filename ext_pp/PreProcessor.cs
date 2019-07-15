using System.Collections.Generic;
using System.IO;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp
{

    /// <summary>
    /// 
    /// </summary>
    public class PreProcessor
    {
        /// <summary>
        /// List of loaded plugins
        /// </summary>
        private List<IPlugin> _plugins = new List<IPlugin>();


        /// <summary>
        /// 
        /// </summary>
        private readonly string _sep = " ";
        //Create Global Definitions
        //Create Stack for All the processing steps(Stack<List<IPlugin>>

        /// <summary>
        /// Returns the List of statements from all the plugins that are remaining in the file and need to be removed as a last step
        /// </summary>
        public List<string> CleanUpList
        {
            get
            {
                var ret = new List<string>();

                foreach (var plugin in _plugins)
                {
                    ret.AddRange(plugin.Cleanup);
                }
                return ret;

            }
        }

        /// <summary>
        /// Sets the File Processing Chain
        /// 0 => First Plugin that gets executed
        /// </summary>
        /// <param name="fileProcessors"></param>
        public void SetFileProcessingChain(List<IPlugin> fileProcessors)
        {
            _plugins = fileProcessors;
        }


        /// <summary>
        /// Compiles a File with the definitions and settings provided
        /// </summary>
        /// <param name="file">FilePath of the file.</param>
        /// <param name="settings"></param>
        /// <param name="defs">Definitions</param>
        /// <returns>Array of Compiled Lines</returns>
        public string[] Compile(string file, Settings settings = null, IDefinitions defs = null)
        {

            Logger.Log(DebugLevel.LOGS, "Starting Post Processor...", Verbosity.LEVEL2);
            ISourceScript[] src = Process(file, settings, defs);
            return Compile(src);
        }


        /// <summary>
        /// Initializing all Plugins with the settings, definitions and the source manager for this compilation
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="def"></param>
        /// <param name="sourceManager"></param>
        private void InitializePlugins(Settings settings, IDefinitions def, ISourceManager sourceManager)
        {
            Logger.Log(DebugLevel.LOGS, "Initializing Plugins...", Verbosity.LEVEL2);
            foreach (var plugin in _plugins)
            {
                Logger.Log(DebugLevel.LOGS, "Initializing Plugin: " + plugin.GetType().Name, Verbosity.LEVEL3);

                plugin.Initialize(settings.GetSettingsWithPrefix(plugin.Prefix, plugin.IncludeGlobal), sourceManager, def);
            }
        }

        /// <summary>
        /// Compiles the Provided source array into a single file. And removes all remaining statements
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public string[] Compile(ISourceScript[] src)
        {
            Logger.Log(DebugLevel.LOGS, "Starting Compilation of File Tree...", Verbosity.LEVEL2);
            List<string> ret = new List<string>();
            for (var i = src.Length - 1; i >= 0; i--)
            {
                ret.AddRange(src[i].GetSource());
            }

            return Utils.RemoveStatements(ret, CleanUpList.ToArray()).ToArray();
        }


        /// <summary>
        /// Processes the file with the settings, definitions and the source manager specified.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="settings"></param>
        /// <param name="defs"></param>
        /// <returns>Returns a list of files that can be compiled in reverse order</returns>
        public ISourceScript[] Process(string file, Settings settings = null, IDefinitions defs = null)
        {
            defs = defs ?? new Definitions();
            SourceManager sm = new SourceManager();

            InitializePlugins(settings, defs, sm);

            Logger.Log(DebugLevel.LOGS, "Starting Processing of File :" + file, Verbosity.LEVEL2);
            file = Path.GetFullPath(file);

            ISourceScript ss = sm.CreateScript(_sep, file, file, new Dictionary<string, object>());
            List<ISourceScript> all = new List<ISourceScript>();
            sm.AddToTodo(ss);
            do
            {
                Logger.Log(DebugLevel.LOGS, "Selecting File :" + ss.GetKey(), Verbosity.LEVEL2);
                for (int j = 0; j < _plugins.Count; j++)
                {

                    Logger.Log(DebugLevel.LOGS, "Running Plugin :" + _plugins[j] + " on file " + file, Verbosity.LEVEL3);
                    if (!_plugins[j].Process(ss, sm, defs))
                    {
                        Logger.Log(DebugLevel.ERRORS, "Processing was aborted by Plugin: " + _plugins[j], Verbosity.LEVEL1);
                        return new ISourceScript[0];
                    }
                }

                sm.SetDone(ss);
                ss = sm.NextItem;
            } while (ss != null);

            return sm.GetList().ToArray();

        }
    }
}