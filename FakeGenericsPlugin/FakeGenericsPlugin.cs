using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp_plugins
{
    public class FakeGenericsPlugin : AbstractFullScriptPlugin
    {
        public override string[] Prefix => new[] { "gen", "FakeGen" };
        public override ProcessStage ProcessStages => Stage.ToLower(CultureInfo.InvariantCulture) == "onload" ? ProcessStage.ON_LOAD_STAGE : ProcessStage.ON_MAIN;
        public string Stage { get; set; } = "onmain";
        public string GenericKeyword { get; set; } = "#type";
        public string Separator { get; set; } = " ";

        public override List<CommandInfo> Info { get; } = new List<CommandInfo>
        {
            new CommandInfo("set-genkeyword","g", PropertyHelper.GetPropertyInfo(typeof(FakeGenericsPlugin), nameof(GenericKeyword)),
                "Sets the keyword that is used when writing pseudo generic code."),
            new CommandInfo("set-separator", "s", PropertyHelper.GetPropertyInfo(typeof(FakeGenericsPlugin), nameof(Separator)),
                "Sets the separator that is used to separate different generic types"),
            new CommandInfo("set-stage", "ss", PropertyHelper.GetPropertyInfo(typeof(FakeGenericsPlugin), nameof(Stage)),
                "Sets the Stage Type of the Plugin to be Executed OnLoad or OnFinishUp"),
        };

        public override void Initialize(Settings settings, ISourceManager sourceManager, IDefinitions defs)
        {

            settings.ApplySettings(Info, this);
            sourceManager.SetComputingScheme(ComputeNameAndKey_Generic);
        }



        private ImportResult ComputeNameAndKey_Generic(string[] vars, string currentPath)
        {
            ImportResult ret = new ImportResult();

            string filePath = "";

            if (!Utils.TryResolvePathIncludeParameter(vars))
            {
                return ret;
            }

            string[] genParams = vars.Length > 1 ?
                vars.SubArray(1, vars.Length - 1).ToArray() : new string[0];

            string rel = Path.Combine(currentPath, vars[0]);
            string key = Path.GetFullPath(rel);

            filePath = key;
            key += (genParams.Length > 0 ? "." + genParams.Unpack(Separator) : "");
            if (genParams.Length != 0)
            {
                ret.SetValue("genParams", genParams);
            }
            ret.SetValue("filename", filePath);
            ret.SetValue("key", key);
            ret.SetResult(true);
            return ret;
        }


        public override bool FullScriptStage(ISourceScript file, ISourceManager sourceManager, IDefinitions defs)
        {
            if (!file.HasValueOfType<string[]>("genParams"))
            {
                return true; //No error, we just dont have any generic parameters to replace.
            }

            string[] GenParams = file.GetValueFromCache<string[]>("genParams");

            this.Log(DebugLevel.LOGS, Verbosity.LEVEL5, "Discovering Generic Keywords...");
            if (GenParams != null && GenParams.Length > 0)
            {
                for (var i = GenParams.Length - 1; i >= 0; i--)
                {

                    this.Log(DebugLevel.LOGS, Verbosity.LEVEL6, "Replacing Keyword {0}{1} with {2} in file {3}", GenericKeyword, i, GenParams[i], file.GetKey());
                    Utils.ReplaceKeyWord(file.GetSource(), GenParams[i],
                        GenericKeyword + i);
                }
            }


            this.Log(DebugLevel.LOGS, Verbosity.LEVEL5, "Generic Keyword Replacement Finished");

            return true;
        }
    }
}