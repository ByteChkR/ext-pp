using System;
using System.Collections.Generic;
using System.Globalization;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp_plugins
{
    public class KeyWordReplacer : AbstractLinePlugin
    {

        public bool NoDefaultKeywords { get; set; }
        public string DateTimeFormatString { get; set; } = "dd/MM/yyyy hh:mm:ss";
        public string DateFormatString { get; set; } = "dd/MM/yyyy";
        public string TimeFormatString { get; set; } = "hh:mm:ss";
        public string SurroundingChar { get; set; } = "$";
        public override string[] Prefix => new[] { "kwr", "KWReplacer" };
        public string[] Keywords { get; set; }
        private DateTime time;
        private Dictionary<string, string> _keywords
        {
            get
            {
                Dictionary<string, string> ret = new Dictionary<string, string>();
                if (!NoDefaultKeywords)
                {

                    ret.Add("DATE_TIME", time.ToString(DateTimeFormatString));
                    ret.Add("DATE", time.ToString(DateFormatString));
                    ret.Add("TIME", time.ToString(TimeFormatString));
                }

                if (Keywords == null)
                {
                    return ret;
                }
                for (int i = 0; i < Keywords.Length; i++)
                {
                    string[] s = Keywords[i].Split(":");
                    ret.Add(s[0], s[1]);
                }

                return ret;
            }
        }

        public override List<CommandInfo> Info { get; } = new List<CommandInfo>
        {
            new CommandInfo("set-order", "o", PropertyHelper.GetPropertyInfo(typeof(KeyWordReplacer), nameof(Order)),
                "Sets the Line Order to be Executed BEFORE the Fullscripts or AFTER the Fullscripts"),
            new CommandInfo("set-stage","ss", PropertyHelper.GetPropertyInfo(typeof(KeyWordReplacer), nameof(Stage)),
                "Sets the Stage Type of the Plugin to be Executed OnLoad or OnFinishUp"),
            new CommandInfo("no-defaultkeywords","nod", PropertyHelper.GetPropertyInfo(typeof(KeyWordReplacer), nameof(NoDefaultKeywords)),
                "Disables $TIME$, $DATE$ and $DATE_TIME$"),
            new CommandInfo("set-dtformat","dtf", PropertyHelper.GetPropertyInfo(typeof(KeyWordReplacer), nameof(DateTimeFormatString)),
                "Sets the datetime format string used when setting the default variables"),
            new CommandInfo("set-tformat", "tf", PropertyHelper.GetPropertyInfo(typeof(KeyWordReplacer), nameof(TimeFormatString)),
                "Sets the time format string used when setting the default variables"),
            new CommandInfo("set-dformat", "df", PropertyHelper.GetPropertyInfo(typeof(KeyWordReplacer), nameof(DateFormatString)),
                "Sets the date format string used when setting the default variables"),
            new CommandInfo("set-surrkeyword","sc", PropertyHelper.GetPropertyInfo(typeof(KeyWordReplacer), nameof(SurroundingChar)),
                "Sets the Surrounding char that escapes the variable names"),
            new CommandInfo("set-kwdata","kwd", PropertyHelper.GetPropertyInfo(typeof(KeyWordReplacer), nameof(Keywords)),
                "Sets the Keywords that need to be replaced with values. <keyword>:<value>"),

        };



        public override void Initialize(Settings settings, ISourceManager sourceManager, IDefinitions defs)
        {
            settings.ApplySettings(Info, this);
            time = DateTime.Now;

        }

        public override string LineStage(string source)
        {
            string ret = source;
            Dictionary<string, string> keywords = _keywords;
            foreach (var keyword in keywords)
            {
                string key = SurroundingChar + keyword.Key + SurroundingChar;
                if (ret.Contains(key))
                {
                    this.Log(DebugLevel.LOGS, Verbosity.LEVEL6, "Replacing {0} with {1}", key, keyword.Value);
                    ret = ret.Replace(key, keyword.Value);
                }
            }


            return ret;
        }

    }
}