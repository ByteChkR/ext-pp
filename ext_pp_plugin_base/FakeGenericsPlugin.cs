using System.Collections.Generic;
using System.IO;
using System.Linq;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp_plugins
{
    public class FakeGenericsPlugin : IPlugin
    {
        public string[] Cleanup => new string[0];
        private readonly string _genericKeyword = Settings.TypeGenKeyword;
        public FakeGenericsPlugin(Settings settings)
        {
            ASourceManager.KeyComputingScheme = ComputeNameAndKey_Generic;
        }

        private bool ComputeNameAndKey_Generic(string[] vars, out string filePath, out string key,
            out Dictionary<string, object> pluginCache)
        {
            pluginCache = new Dictionary<string, object>();
            filePath = key = "";
            if (vars.Length == 0) return false;
            string[] genParams = vars.Length > 1 ?
                vars.SubArray(1, vars.Length - 1).ToArray() : new string[0];
            
            key = filePath = Path.GetFullPath(vars[0]);
            key += (genParams.Length > 0 ? "." + genParams.Unpack(Settings.Separator) : "");
            if (genParams.Length != 0)
                pluginCache.Add("genParams", genParams);
            return true;
        }

        public bool Process(ASourceScript file, ASourceManager sourceManager, ADefinitions defs)
        {
            if (!file.HasValueOfType<string[]>("genParams")) return true; //No error, we just dont have any generic parameters to replace.

            string[] GenParams = file.GetValueFromCache<string[]>("genParams");

            Logger.Log(DebugLevel.LOGS, "Discovering Generic Keywords...", Verbosity.LEVEL3);
            if (GenParams != null && GenParams.Length > 0)
            {
                for (var i = GenParams.Length - 1; i >= 0; i--)
                {

                    Logger.Log(DebugLevel.ERRORS, "Replacing Keyword " + _genericKeyword + i + " with " + GenParams[i] + " in file " + file.GetKey(), Verbosity.LEVEL4);
                    Utils.ReplaceKeyWord(file.GetSource(), GenParams[i],
                        _genericKeyword + i);
                }
            }


            Logger.Log(DebugLevel.LOGS, "Generic Keyword Replacement Finished", Verbosity.LEVEL3);

            return true;
        }
    }
}