using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp_plugins
{
    public class ErrorPlugin : AbstractPlugin
    {

        public override string[] Prefix => new string[] { "err", "Error" };

        public override ProcessStage ProcessStages => Stage.ToLower()=="onfinishup" ? ProcessStage.ON_LOAD_STAGE : ProcessStage.ON_FINISH_UP;
        public override PluginType PluginType => Order.ToLower() == "after" ? PluginType.LINE_PLUGIN_AFTER : PluginType.LINE_PLUGIN_BEFORE;
        
        public string Order = "after";
        public string Stage = "onfinishup";
        public string ErrorKeyword = "#error";
        public string Separator = " ";
        public override List<CommandInfo> Info { get; } = new List<CommandInfo>()
        {
            new CommandInfo("set-error","e", PropertyHelper.GetFieldInfo(typeof(ErrorPlugin), nameof(ErrorKeyword)),
                "set-error [error keyword] *#error*\r\n\t\t\tSets the keyword that is used to trigger errors during compilation"),
            new CommandInfo("set-separator", "s", PropertyHelper.GetFieldInfo(typeof(ErrorPlugin), nameof(Separator)),
                "set-separator [separator] * *\r\n\t\t\tSets the separator that is used to separate the error keyword from the error text"),
            new CommandInfo("set-order", "o", PropertyHelper.GetFieldInfo(typeof(ErrorPlugin), nameof(Order)),
                "set-order [Before|After] *After*\r\n\t\t\tSets the Line Order to be Executed BEFORE the Fullscripts or AFTER the Fullscripts"),
            new CommandInfo("set-stage","ss", PropertyHelper.GetFieldInfo(typeof(ErrorPlugin), nameof(Stage)),
                "set-stage [OnLoad|OnFinishUp] *#OnFinishUp*\r\n\t\t\tSets the Stage Type of the Plugin to be Executed OnLoad or OnFinishUp"),
        };



        public override void Initialize(Settings settings, ISourceManager sourceManager, IDefinitions defTable)
        {

            settings.ApplySettings(Info, this);
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

        public override bool OnLoad_FullScriptStage(ISourceScript script, ISourceManager sourceManager, IDefinitions defTable)
        {
            return FullScriptStage(script, sourceManager, defTable);
        }

        public override bool OnMain_FullScriptStage(ISourceScript script, ISourceManager sourceManager, IDefinitions defTable)
        {
            return FullScriptStage(script, sourceManager, defTable);
        }



        public string LineStage(string source)
        {
            if (!Utils.IsStatement(source, ErrorKeyword)) return source;
            string err = Utils.SplitAndRemoveFirst(source, Separator).Unpack(" ");
            Logger.Crash(new Exception("Error " + err), true);
            return "";
        }



        public bool FullScriptStage(ISourceScript file, ISourceManager todo, IDefinitions defs)
        {
            return true;
        }

    }
}