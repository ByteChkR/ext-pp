using System.Collections.Generic;
using ext_pp.settings;

namespace ext_pp.plugins
{
    public class FakeGenericsPlugin : IPlugin
    {
        public string[] Cleanup => new string[0];
        private readonly string _genericKeyword = Settings.TypeGenKeyword;
        public FakeGenericsPlugin(Settings settings)
        {
        }

        public bool Process(SourceScript file, SourceManager sourceManager, Definitions defs)
        {
            Logger.Log(DebugLevel.LOGS, "Discovering Generic Keywords...", Verbosity.LEVEL3);
            if (file.GenParam != null && file.GenParam.Length > 0)
            {
                for (var i = file.GenParam.Length - 1; i >= 0; i--)
                {

                    Logger.Log(DebugLevel.ERRORS, "Replacing Keyword " + _genericKeyword+i + " with " + file.GenParam[i] + " in file " + file.Key, Verbosity.LEVEL4);
                    Utils.ReplaceKeyWord(file.Source, file.GenParam[i],
                        _genericKeyword + i);
                }
            }


            Logger.Log(DebugLevel.LOGS, "Generic Keyword Replacement Finished", Verbosity.LEVEL3);

            return true;
        }
    }
}