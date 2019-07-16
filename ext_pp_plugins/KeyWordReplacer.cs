using System;
using System.Collections.Generic;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp_plugins
{
    public class KeyWordReplacer : AbstractPlugin
    {
        public override PluginType PluginType => (After ? PluginType.LINE_PLUGIN_AFTER : PluginType.LINE_PLUGIN_BEFORE);
        public override ProcessStage ProcessStages => OnLoad ? ProcessStage.ON_LOAD_STAGE : ProcessStage.ON_FINISH_UP;

        public bool After = true;
        public bool OnLoad = false;
        public bool NoDefaultKeywords = false;
        public string DateTimeFormatString = "dd/MM/yyyy hh:mm:ss";
        public string DateFormatString = "dd/MM/yyyy";
        public string TimeFormatString = "hh:mm:ss";
        public string SurroundingChar = "$";
        public override string[] Prefix => new string[] { "kwr" };
        public string[] Keywords = null;
        private DateTime time;
        private Dictionary<string, string> _keywords
        {
            get
            {
                Dictionary<string, string> ret = new Dictionary<string, string>();
                if (!NoDefaultKeywords)
                {

                    ret.Add(SurroundingChar + "DATE_TIME" + SurroundingChar, time.ToString(DateTimeFormatString));
                    ret.Add(SurroundingChar + "DATE" + SurroundingChar, time.ToString(DateFormatString));
                    ret.Add(SurroundingChar + "TIME" + SurroundingChar, time.ToString(TimeFormatString));
                }

                if (Keywords == null) return ret;

                for (int i = 0; i < Keywords.Length; i++)
                {
                    string[] s = Keywords[i].Split(":");
                    ret.Add(s[0], s[1]);
                }

                return ret;
            }
        }

        public override List<CommandInfo> Info { get; } = new List<CommandInfo>()
        {
            new CommandInfo("a", PropertyHelper.GetFieldInfo(typeof(KeyWordReplacer), nameof(After)),
                "Sets the Plugin type to be executed after the full script plugins"),
            new CommandInfo("l", PropertyHelper.GetFieldInfo(typeof(KeyWordReplacer), nameof(OnLoad)),
                "Sets the Plugin type to be On Load instead of On Finish Up"),
            new CommandInfo("nod", PropertyHelper.GetFieldInfo(typeof(KeyWordReplacer), nameof(NoDefaultKeywords)),
                "Disables the default Keywords."),
            new CommandInfo("dtf", PropertyHelper.GetFieldInfo(typeof(KeyWordReplacer), nameof(DateTimeFormatString)),
                "Sets the Plugin Default DATE_TIME format string"),
            new CommandInfo("tf", PropertyHelper.GetFieldInfo(typeof(KeyWordReplacer), nameof(TimeFormatString)),
                "Sets the Plugin Default TIME format string"),
            new CommandInfo("df", PropertyHelper.GetFieldInfo(typeof(KeyWordReplacer), nameof(DateFormatString)),
                "Sets the Plugin Default DATE format string"),
            new CommandInfo("sc", PropertyHelper.GetFieldInfo(typeof(KeyWordReplacer), nameof(DateFormatString)),
                "Sets the Surrounding char that escapes the variable names"),

        };



        public override void Initialize(Settings settings, ISourceManager sourceManager, IDefinitions defs)
        {
            settings.ApplySettings(Info, this);
            time = DateTime.Now;

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
            Dictionary<string, string> keywords = _keywords;
            foreach (var keyword in keywords)
            {
                if (source.Contains(keyword.Key))
                {
                    Logger.Log(DebugLevel.LOGS, "Replacing " + keyword.Key + " with " + keyword.Value, Verbosity.LEVEL5);
                    source = source.Replace(keyword.Key, keyword.Value);
                }
            }


            return source;
        }

    }
}