using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security;
using ADL;
using ext_pp.plugins;
using ext_pp.settings;

namespace ext_pp
{
    public class PreProcessor
    {
        private List<IPlugin> _plugins = new List<IPlugin>();
        private readonly Settings _settings;

        private readonly string _sep = Settings.Separator;
        //Create Global Definitions
        //Create Stack for All the processing steps(Stack<List<IPlugin>>
        public PreProcessor(Settings settings)
        {
            _settings = settings;

        }

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

        public void SetFileProcessingChain(List<IPlugin> fileProcessors)
        {
            _plugins = fileProcessors;
        }

        public string[] Compile(string file, Definitions defs = null)
        {

            Logger.Log(DebugLevel.LOGS, "Starting Post Processor...", Verbosity.LEVEL1);
            SourceScript[] src = Process(file, defs);
            return Compile(src);
        }

        public string[] Compile(SourceScript[] src)
        {
            Logger.Log(DebugLevel.LOGS, "Starting Compilation of File Tree...", Verbosity.LEVEL1);
            List<string> ret = new List<string>();
            for (var i = src.Length - 1; i >= 0; i--)
            {
                ret.AddRange(src[i].Source);
            }

            return Utils.RemoveStatements(ret, CleanUpList.ToArray()).ToArray();
        }

        public SourceScript[] Process(string file, Definitions defs = null)
        {

            Logger.Log(DebugLevel.LOGS, "Starting Processing of File :" + file, Verbosity.LEVEL1);
            file = Path.GetFullPath(file);
            defs = defs ?? new Definitions();
            SourceManager sm = new SourceManager();

            SourceScript ss = new SourceScript(_sep, file, new string[0]);
            List<SourceScript> all = new List<SourceScript>();
            sm.AddToTodo(ss);
            do
            {
                Logger.Log(DebugLevel.LOGS, "Selecting File :" + ss.Key, Verbosity.LEVEL1);
                for (int j = 0; j < _plugins.Count; j++)
                {

                    Logger.Log(DebugLevel.LOGS, "Running Plugin :" + _plugins[j] + " on file " + file, Verbosity.LEVEL2);
                    if (!_plugins[j].Process(ss, sm, defs))
                    {
                        Logger.Log(DebugLevel.ERRORS, "Processing was aborted by Plugin: "+_plugins[j], Verbosity.ALWAYS_SEND);
                        return new SourceScript[0];
                    }
                }

                sm.SetDone(ss);
                ss = sm.NextItem;
            } while (ss != null);

            return sm.GetList().ToArray();

        }
    }
}