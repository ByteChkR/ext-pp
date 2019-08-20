﻿using System.Collections.Generic;
using System.Globalization;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp_plugins
{
    public class ToUpperPlugin : AbstractLinePlugin
    {
        public override PluginType PluginTypeToggle => (Order.ToLower(CultureInfo.InvariantCulture) == "after" ? PluginType.LINE_PLUGIN_AFTER : PluginType.LINE_PLUGIN_BEFORE);
        public override ProcessStage ProcessStages => Stage.ToLower(CultureInfo.InvariantCulture) == "onload" ? ProcessStage.ON_LOAD_STAGE : ProcessStage.ON_FINISH_UP;

        public string Order { get; set; } = "after";
        public string Stage { get; set; } = "onfinishup";

        public override List<CommandInfo> Info =>
            new List<CommandInfo>
            {
                new CommandInfo("set-order", "o", PropertyHelper.GetPropertyInfo(typeof(KeyWordReplacer), nameof(Order)),
                    "Sets the Line Order to be Executed BEFORE the Fullscripts or AFTER the Fullscripts"),
                new CommandInfo("set-stage","ss", PropertyHelper.GetPropertyInfo(typeof(KeyWordReplacer), nameof(Stage)),
                    "Sets the Stage Type of the Plugin to be Executed OnLoad or OnFinishUp"),
            };


        public override string LineStage(string source)
        {
            return source.ToUpper(CultureInfo.InvariantCulture);
        }

        public override string[] Prefix => new[] { "tup", "ToUpper" };
        /// <summary>
        /// No initialization needed for this plugin.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="sourceManager"></param>
        /// <param name="defTable"></param>
        public override void Initialize(Settings settings, ISourceManager sourceManager, IDefinitions defTable)
        {
            // No initialization needed for this plugin.
        }

    }
}