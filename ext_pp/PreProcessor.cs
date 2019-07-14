using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security;
using ext_pp.plugins;
using ext_pp.settings;

namespace ext_pp
{
    public class PreProcessor
    {
        private List<SourceScript> _fileTree = new List<SourceScript>();
        private List<IPlugin> _plugins = new List<IPlugin>();
        private Settings _settings;
        //Create Global Definitions
        //Create Stack for All the processing steps(Stack<List<IPlugin>>
        public PreProcessor(Settings settings)
        {
            _settings = settings;
            Logger._level = settings.VerbosityLevel;
            SetFileProcessingChain(new List<IPlugin>()
            {
                new FakeGenericsPlugin(settings),
                new ConditionalPlugin(settings),
                new IncludePlugin(settings),
                new WarningPlugin(settings),
                new ErrorPlugin(settings)

            });
        }

        public void SetFileProcessingChain(List<IPlugin> fileProcessors)
        {
            _plugins = fileProcessors;
        }

        public string[] Compile(string file, Definitions defs = null)
        {
            SourceScript[] src = Process(file, defs);
            return Compile(src);
        }

        public string[] Compile(SourceScript[] src)
        {
            List<string> ret = new List<string>();
            for (var i = src.Length - 1; i >= 0; i--)
            {
                ret.AddRange(src[i].Source);
            }

            return Utils.RemoveStatements(ret, _settings.CleanUpList.ToArray()).ToArray();
        }

        public SourceScript[] Process(string file, Definitions defs = null)
        {
            file = Path.GetFullPath(file);
            defs = defs ?? new Definitions();
            SourceManager sm = new SourceManager();

            SourceScript ss = new SourceScript(_settings, file, new string[0]);
            List<SourceScript> all = new List<SourceScript>();
            sm.AddToTodo(ss);
            do
            {
                for (int j = 0; j < _plugins.Count; j++)
                {
                    _plugins[j].Process(ss, sm, defs);
                }

                sm.SetDone(ss);
                ss = sm.NextItem;
            } while (ss != null);

            return sm.GetList().ToArray();

        }
    }
}