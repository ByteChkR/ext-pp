using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp_plugins
{
    public class FakeGenericsPlugin : AbstractPlugin
    {
        public override string[] Prefix => new string[] { "gen" };
        public override PluginType PluginType => PluginType.FULL_SCRIPT_PLUGIN;
        public override ProcessStage ProcessStages => OnLoad ? ProcessStage.ON_LOAD_STAGE : ProcessStage.ON_MAIN;
        public bool OnLoad = false;
        public string GenericKeyword = "#type";
        public string Separator = " ";
        public override List<CommandInfo> Info { get; } = new List<CommandInfo>()
        {
            new CommandInfo("g", PropertyHelper.GetFieldInfo(typeof(FakeGenericsPlugin), nameof(GenericKeyword)),
                "Sets the keyword that will be replaced with the parameters supplied when adding a file."),
            new CommandInfo("s", PropertyHelper.GetFieldInfo(typeof(FakeGenericsPlugin), nameof(Separator)),
                "Sets the characters that will be used to separate strings"),
            new CommandInfo("l", PropertyHelper.GetFieldInfo(typeof(ErrorPlugin), nameof(OnLoad)),
                "Sets the Plugin type to be On Load instead of On Main"),
        };

        public override void Initialize(Settings settings, ISourceManager sourceManager, IDefinitions defs)
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

        public override bool OnLoad_FullScriptStage(ISourceScript script, ISourceManager sourceManager, IDefinitions defTable)
        {
            return FullScriptStage(script, sourceManager, defTable);
        }

        public override bool OnMain_FullScriptStage(ISourceScript script, ISourceManager sourceManager, IDefinitions defTable)
        {
            return FullScriptStage(script, sourceManager, defTable);
        }


        public bool FullScriptStage(ISourceScript file, ISourceManager sourceManager, IDefinitions defs)
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