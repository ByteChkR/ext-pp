using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp_plugins
{
    public class ExceptionPlugin : AbstractLinePlugin
    {
        public override string[] Prefix => new[] { "ex", "ExceptionPlugin" };
        public override ProcessStage ProcessStages => Stage.ToLower(CultureInfo.InvariantCulture) == "onfinishup" ? ProcessStage.ON_LOAD_STAGE : ProcessStage.ON_FINISH_UP;
        public override PluginType PluginTypeToggle => Order.ToLower(CultureInfo.InvariantCulture) == "after" ? PluginType.LINE_PLUGIN_AFTER : PluginType.LINE_PLUGIN_BEFORE;

        public string Order { get; set; } = "after";
        public string Stage { get; set; } = "onfinishup";
        public string WarningKeyword { get; set; } = "#warning";
        public string ErrorKeyword { get; set; } = "#error";
        public string Separator { get; set; } = " ";

        public override List<CommandInfo> Info { get; } = new List<CommandInfo>
        {
            new CommandInfo("set-error","e", PropertyHelper.GetPropertyInfo(typeof(ExceptionPlugin), nameof(ErrorKeyword)),
                "Sets the keyword that is used to trigger errors during compilation"),
            new CommandInfo("set-warning", "w", PropertyHelper.GetPropertyInfo(typeof(ExceptionPlugin), nameof(WarningKeyword)),
                "sets the keyword that is used to trigger warnings during compilation"),
            new CommandInfo("set-separator", "s", PropertyHelper.GetPropertyInfo(typeof(ExceptionPlugin), nameof(Separator)),
                "Sets the separator that is used to separate different generic types"),
            new CommandInfo("set-order","o", PropertyHelper.GetPropertyInfo(typeof(ExceptionPlugin), nameof(Order)),
                "Sets the Line Order to be Executed BEFORE the Fullscripts or AFTER the Fullscripts"),
            new CommandInfo("set-stage", "ss", PropertyHelper.GetPropertyInfo(typeof(ExceptionPlugin), nameof(Stage)),
                "Sets the Stage Type of the Plugin to be Executed OnLoad or OnFinishUp"),
        };
        public override void Initialize(Settings settings, ISourceManager sourceManager, IDefinitions defTable)
        {
            settings.ApplySettings(Info, this);
        }



        public override string LineStage(string source)
        {
            if (Utils.IsStatement(source, WarningKeyword))
            {
                string err = Utils.SplitAndRemoveFirst(source, Separator).Unpack(" ");
                this.Log(DebugLevel.ERRORS, Verbosity.LEVEL1, "Warning: {0}", err);
                return "";
            }


            if (Utils.IsStatement(source, ErrorKeyword))
            {
                string err = Utils.SplitAndRemoveFirst(source, Separator).Unpack(" ");
                Logger.Crash(new ProcessorException("Error " + err), true);
                return "";
            }

            return source;
        }




    }
}