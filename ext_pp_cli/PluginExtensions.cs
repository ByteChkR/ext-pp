using System.Collections.Generic;
using ext_pp_base;

namespace ext_pp_cli
{
    /// <summary>
    /// Useful extensions for the CLI when working with Plugins
    /// </summary>
    internal static class PluginExtensions
    {
        /// <summary>
        /// Returns a list of all commands with information.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static List<string> ListAllCommands(this List<CommandInfo> info, string[] prefix)
        {
            List<string> ret = new List<string>();
            foreach (var cmd in info)
            {
                ret.Add("--" + prefix.Unpack("/") + ":" + cmd.ToString());
            }

            return ret;
        }

        /// <summary>
        /// Returns a list of command info.
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="listCommands"></param>
        /// <returns></returns>
        public static List<string> ListInfo(this AbstractPlugin plugin, bool listCommands)
        {
            List<string> ret = new List<string>
            {
                "Plugin Name: " + plugin.GetType().Name,
                "Plugin Namespace: " + plugin.GetType().Namespace,
                "Plugin Version: " + plugin.GetType().Assembly.GetName().Version,
                "Plugin Include Global: " + plugin.IncludeGlobal,
                "Plugin Prefixes: " + plugin.Prefix.Unpack(", ")
            };

            if (listCommands)
            {
                ret.Add("Plugin Commands:");
                ret.AddRange(ListAllCommands(plugin.Info, plugin.Prefix));
            }
            ret.Add("");
            return ret;
        }

        /// <summary>
        /// Converts the Plugin to a basic markdown text that can be used to generate readmes.
        /// </summary>
        /// <param name="plugin"></param>
        /// <returns></returns>
        public static string[] ToMarkdown(this AbstractPlugin plugin)
        {
            List<string> ret = new List<string>
            {
                "______________________________________________",
              "#### "+plugin.GetType().Name+ " Information:",
              "",
              "* Prefix: "+plugin.Prefix.Unpack(", "),
              "* Commands:",
              ""
            };

            string tab = "\t\t";

            for (int i = 0; i < plugin.Info.Count; i++)
            {

                string[] helpt = plugin.Info[i].HelpText.Split("\n");
                ret.Add(tab + plugin.Info[i].Command + "/" + plugin.Info[i].ShortCut);
                ret.Add(tab + "\t" + helpt.Unpack("\n\t" + tab));
            }

            return ret.ToArray();
        }
    }
}