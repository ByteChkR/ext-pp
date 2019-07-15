using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using ext_pp_base;
using ext_pp_plugins;

namespace ext_pp_cli
{
    internal static class PluginExtensions
    {
        public static List<string> ListAllCommands(this List<CommandInfo> info, string[] prefix)
        {
            List<string> ret = new List<string>();
            foreach (var cmd in info)
            {
                ret.Add("[" + prefix.Unpack("/") + "]" + cmd.ToString());
            }

            return ret;
        }

        public static List<string> ListInfo(this IPlugin plugin, bool listCommands)
        {
            List<string> ret = new List<string>();
            ret.Add("Plugin Name: "+plugin.GetType().Name);
            ret.Add("Plugin Namespace: "+ plugin.GetType().Namespace);
            ret.Add("Plugin Version: " + plugin.GetType().Assembly.GetName().Version);
            ret.Add("Plugin Include Global: " + plugin.IncludeGlobal);
            ret.Add("Plugin Prefixes: " + plugin.Prefix.Unpack(", "));

            if (listCommands)
            {
                ret.Add("Plugin Commands:");
                ret.AddRange(ListAllCommands(plugin.Info, plugin.Prefix));
            }
            ret.Add("");
            return ret;
        }
    }
}