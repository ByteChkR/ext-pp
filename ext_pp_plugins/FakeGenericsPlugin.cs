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
        public override string[] Prefix => new string[] { "gen", "FakeGen" };
        public override PluginType PluginType => PluginType.FULL_SCRIPT_PLUGIN;
        public override ProcessStage ProcessStages => Stage.ToLower()=="onload" ? ProcessStage.ON_LOAD_STAGE : ProcessStage.ON_MAIN;
        public string Stage = "onmain";
        public string GenericKeyword = "#type";
        public string Separator = " ";
        public override List<CommandInfo> Info { get; } = new List<CommandInfo>()
        {
            new CommandInfo("set-genkeyword","g", PropertyHelper.GetFieldInfo(typeof(FakeGenericsPlugin), nameof(GenericKeyword)),
                "set-genkeyword [generic keyword] *#type*\r\n\t\t\tSets the keyword that is used when writing pseudo generic code."),
            new CommandInfo("set-separator", "s", PropertyHelper.GetFieldInfo(typeof(FakeGenericsPlugin), nameof(Separator)),
                "set-separator [separator keyword] * *\r\n\t\t\tSets the separator that is used to separate different generic types"),
            new CommandInfo("set-stage", "ss", PropertyHelper.GetFieldInfo(typeof(FakeGenericsPlugin), nameof(Stage)),
                "set-stage [OnLoad|OnMain] *OnMain*\r\n\t\t\tSets the Stage Type of the Plugin to be Executed OnLoad or OnFinishUp"),
        };

        public override void Initialize(Settings settings, ISourceManager sourceManager, IDefinitions defs)
        {

            settings.ApplySettings(Info, this);
            sourceManager.SetComputingScheme(ComputeNameAndKey_Generic);
        }


        private bool ComputeNameAndKey_Generic(string[] vars, string currentPath, out string filePath, out string key,
            out Dictionary<string, object> pluginCache)
        {
            pluginCache = new Dictionary<string, object>();
            filePath = key = "";
            if (vars.Length == 0) return false;
            string[] genParams = vars.Length > 1 ?
                vars.SubArray(1, vars.Length - 1).ToArray() : new string[0];
            string dir = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(currentPath);
            key =
                filePath = Path.GetFullPath(vars[0]);
            Directory.SetCurrentDirectory(dir);
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