using System.Collections.Generic;
using System.Linq;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp_plugins
{
    public class MultiLinePlugin : AbstractPlugin
    {
        public override PluginType PluginType => PluginType.FULL_SCRIPT_PLUGIN;
        public override ProcessStage ProcessStages => Stage.ToLower()=="onload" ?  ProcessStage.ON_LOAD_STAGE: ProcessStage.ON_MAIN;

        public string Stage = "onload";
        public string MultiLineKeyword = "__";
        public override string[] Prefix => new string[] { "mlp", "MultiLine" };


        public override List<CommandInfo> Info { get; } = new List<CommandInfo>()
        {
            new CommandInfo("set-stage", "ss", PropertyHelper.GetFieldInfo(typeof(MultiLinePlugin), nameof(Stage)),
                "Sets the Stage Type of the Plugin to be Executed OnLoad or OnFinishUp"),
            new CommandInfo("set-mlkeyword", "mlk", PropertyHelper.GetFieldInfo(typeof(MultiLinePlugin), nameof(MultiLineKeyword)),
                "Sets the keyword that is used to detect when to lines should be merged. The line containing the keyword will be merges with the next line in the file"),
        };



        public override void Initialize(Settings settings, ISourceManager sourceManager, IDefinitions defs)
        {

            settings.ApplySettings(Info, this);

        }

        public override bool OnLoad_FullScriptStage(ISourceScript script, ISourceManager sourceManager,
            IDefinitions defTable)
        {
            return FullScriptStage(script, sourceManager, defTable);
        }

        public override bool OnMain_FullScriptStage(ISourceScript script, ISourceManager sourceManager,
            IDefinitions defTable)
        {
            return FullScriptStage(script, sourceManager, defTable);
        }

        public bool FullScriptStage(ISourceScript file, ISourceManager todo, IDefinitions defs)
        {
            List<string> source = file.GetSource().ToList();
            for (int i = source.Count - 1; i >= 0; i--)
            {

                if (i < source.Count - 1 && source[i].TrimEnd().EndsWith(MultiLineKeyword))
                {
                    string newstr = source[i].Substring(0, source[i].Length - MultiLineKeyword.Length) + source[i + 1];
                    source.RemoveAt(i + 1);
                    source[i] = newstr;
                }
            }
            file.SetSource(source.ToArray());
            return true;
        }

        public override string OnLoad_LineStage(string source)
        {
            return LineStage(source);
        }

        public override string OnMain_LineStage(string source)
        {
            return LineStage(source);
        }

        public override string OnFinishUp_LineStage(string source)
        {
            return LineStage(source);
        }


        public string LineStage(string source)
        {
            return source;
        }

    }
}