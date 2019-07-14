using System.Collections.Generic;
using ext_pp.settings;

namespace ext_pp.plugins
{
    public class FakeGenericsPlugin : IPlugin
    {
        private readonly string _genericKeyword;
        public FakeGenericsPlugin(Settings settings)
        {
            _genericKeyword = settings.TypeGenKeyword;
        }

        public bool Process(SourceScript file, SourceManager todo, Definitions defs)
        {
            if (file.GenParam != null && file.GenParam.Length > 0)
            {
                Logger.Log(DebugLevel.LOGS, "Resolving Generic Parameters", Verbosity.LEVEL2);

                for (var i = file.GenParam.Length - 1; i >= 0; i--)
                {
                    Utils.ReplaceKeyWord(file.Source, file.GenParam[i],
                        _genericKeyword + i);
                }
            }
            else
            {
                Logger.Log(DebugLevel.LOGS, "No Generic Parameters found", Verbosity.LEVEL2);

            }

            return true;
        }
    }
}