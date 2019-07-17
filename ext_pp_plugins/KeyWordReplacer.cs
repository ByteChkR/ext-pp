using System;
using System.Collections.Generic;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp_plugins
{
    public class KeyWordReplacer : AbstractPlugin
    {
        public override PluginType PluginType => (Order.ToLower() == "after" ? PluginType.LINE_PLUGIN_AFTER : PluginType.LINE_PLUGIN_BEFORE);
        public override ProcessStage ProcessStages => Stage.ToLower() == "onload" ? ProcessStage.ON_LOAD_STAGE : ProcessStage.ON_FINISH_UP;

        public string Order = "after";
        public string Stage = "onfinishup";
        public bool NoDefaultKeywords = false;
        public string DateTimeFormatString = "dd/MM/yyyy hh:mm:ss";
        public string DateFormatString = "dd/MM/yyyy";
        public string TimeFormatString = "hh:mm:ss";
        public string SurroundingChar = "$";
        public override string[] Prefix => new string[] { "kwr", "KWReplacer" };
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
            new CommandInfo("set-order", "o", PropertyHelper.GetFieldInfo(typeof(KeyWordReplacer), nameof(Order)),
                "set-order [Before|After] *After*\r\n\t\t\tSets the Line Order to be Executed BEFORE the Fullscripts or AFTER the Fullscripts"),
            new CommandInfo("set-stage","ss", PropertyHelper.GetFieldInfo(typeof(KeyWordReplacer), nameof(Stage)),
                "set-stage [OnLoad|OnFinishUp] *OnFinishUp*\r\n\t\t\tSets the Stage Type of the Plugin to be Executed OnLoad or OnFinishUp"),
            new CommandInfo("no-defaultkeywords","nod", PropertyHelper.GetFieldInfo(typeof(KeyWordReplacer), nameof(NoDefaultKeywords)),
                "no-defaultkeywords [bool] *false*\r\n\t\t\tDisables $TIME$, $DATE$ and $DATE_TIME$"),
            new CommandInfo("set-dtformat","dtf", PropertyHelper.GetFieldInfo(typeof(KeyWordReplacer), nameof(DateTimeFormatString)),
                "set-dtformat [datetimeformatstring] *dd/MM/yyyy hh:mm:ss*\r\n\t\t\tSets the datetime format string used when setting the default variables"),
            new CommandInfo("set-tformat", "tf", PropertyHelper.GetFieldInfo(typeof(KeyWordReplacer), nameof(TimeFormatString)),
                "set-tformat [timeformatstring] *hh:mm:ss*\r\n\t\t\tSets the time format string used when setting the default variables"),
            new CommandInfo("set-dformat", "df", PropertyHelper.GetFieldInfo(typeof(KeyWordReplacer), nameof(DateFormatString)),
                "set-dformat [dateformatstring] *dd/MM/yyyy*\r\n\t\t\tSets the date format string used when setting the default variables"),
            new CommandInfo("set-surrkeyword","sc", PropertyHelper.GetFieldInfo(typeof(KeyWordReplacer), nameof(DateFormatString)),
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
                    this.Log(DebugLevel.LOGS, "Replacing " + keyword.Key + " with " + keyword.Value, Verbosity.LEVEL6);
                    source = source.Replace(keyword.Key, keyword.Value);
                }
            }


            return source;
        }

    }
}