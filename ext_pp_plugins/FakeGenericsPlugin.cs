using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp_plugins
{
    public class FakeGenericsPlugin : IPlugin
    {
        public string[] Cleanup => new string[0];
        public string[] Prefix => new string[] { "gen" };
        public bool IncludeGlobal => true;
        public string GenericKeyword = "#type";
        public string Separator = " ";
        public List<CommandInfo> Info { get; } = new List<CommandInfo>()
        {
            new CommandInfo("g", PropertyHelper.GetFieldInfo(typeof(FakeGenericsPlugin), nameof(GenericKeyword)),
                "Sets the keyword that will be replaced with the parameters supplied when adding a file."),
            new CommandInfo("s", PropertyHelper.GetFieldInfo(typeof(FakeGenericsPlugin), nameof(Separator)),
                "Sets the characters that will be used to separate strings")
        };

        public void Initialize(Settings settings, ISourceManager sourceManager, IDefinitions defs)
        {

            settings.ApplySettings(Info, this);
            sourceManager.SetComputingScheme(ComputeNameAndKey_Generic);
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
            key += (genParams.Length > 0 ? "." + genParams.Unpack(Separator) : "");
            if (genParams.Length != 0)
                pluginCache.Add("genParams", genParams);
            return true;
        }

        public bool Process(ISourceScript file, ISourceManager sourceManager, IDefinitions defs)
        {
            if (!file.HasValueOfType<string[]>("genParams")) return true; //No error, we just dont have any generic parameters to replace.

            string[] GenParams = file.GetValueFromCache<string[]>("genParams");

            Logger.Log(DebugLevel.LOGS, "Discovering Generic Keywords...", Verbosity.LEVEL4);
            if (GenParams != null && GenParams.Length > 0)
            {
                for (var i = GenParams.Length - 1; i >= 0; i--)
                {

                    Logger.Log(DebugLevel.LOGS, "Replacing Keyword " + GenericKeyword + i + " with " + GenParams[i] + " in file " + file.GetKey(), Verbosity.LEVEL5);
                    Utils.ReplaceKeyWord(file.GetSource(), GenParams[i],
                        GenericKeyword + i);
                }
            }


            Logger.Log(DebugLevel.LOGS, "Generic Keyword Replacement Finished", Verbosity.LEVEL4);

            return true;
        }
    }
}