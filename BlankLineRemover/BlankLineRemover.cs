using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp_plugins
{
    public class BlankLineRemover : AbstractLinePlugin
    {
        public string BlankLineRemovalKeyword { get; set; } = "###REMOVE###";
        public override string[] Prefix => new[] { "blr", "BLRemover" };
        public override string[] Cleanup => new[] { BlankLineRemovalKeyword, BlankLineRemovalKeyword.ToLower(CultureInfo.InvariantCulture) };


        public override List<CommandInfo> Info { get; } = new List<CommandInfo>
        {
            new CommandInfo("set-removekeyword", "k", PropertyHelper.GetPropertyInfo(typeof(BlankLineRemover), nameof(BlankLineRemovalKeyword)),
                "This will get inserted whenever a blank line is detected. This will be removed in the native cleanup of the PreProcessor"),
            new CommandInfo("set-order", "o", PropertyHelper.GetPropertyInfo(typeof(BlankLineRemover), nameof(Order)),
                "Sets the Line Order to be Executed BEFORE the Fullscripts or AFTER the Fullscripts"),
            new CommandInfo("set-stage","ss", PropertyHelper.GetPropertyInfo(typeof(BlankLineRemover), nameof(Stage)),
                "Sets the Stage Type of the Plugin to be Executed OnLoad or OnFinishUp"),
        };


        public override void Initialize(Settings settings, ISourceManager sourceManager, IDefinitions defs)
        {
            settings.ApplySettings(Info, this);

        }


        public override string LineStage(string source)
        {
            if (source.Trim() == "")
            {
                this.Log(DebugLevel.LOGS, Verbosity.LEVEL6, "Adding {0} for line removal later", BlankLineRemovalKeyword);
                return BlankLineRemovalKeyword;
            }
            return source;
        }

    }
}