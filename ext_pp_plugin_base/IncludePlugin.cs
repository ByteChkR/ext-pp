using System.Collections.Generic;
using System.IO;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp_plugins
{
    public class IncludePlugin : IPlugin
    {
        public string[] Cleanup => new string[0];
        private readonly string _includeKeyword = Settings.IncludeStatement;
        private readonly string _separator = Settings.Separator;
        public IncludePlugin(Settings settings)
        {

        }

        public bool Process(ASourceScript script, ASourceManager sourceManager, ADefinitions defs)
        {

            Logger.Log(DebugLevel.LOGS, "Disovering Include Statments...", Verbosity.LEVEL3);
            string[] incs = Utils.FindStatements(script.GetSource(), _includeKeyword);


            foreach (var includes in incs)
            {
                Logger.Log(DebugLevel.LOGS, "Processing Statement: " + includes, Verbosity.LEVEL4);
                bool tmp = GetIncludeSourcePath(includes, Path.GetDirectoryName(Path.GetFullPath(script.GetFilePath())), out var path, out var key, out Dictionary<string, object> pluginCache);
                if (tmp)
                {

                    ASourceScript ss = sourceManager.CreateScript(_separator, path, key, pluginCache);
                    if (!sourceManager.IsIncluded(ss))
                    {
                        sourceManager.AddToTodo(ss);
                    }
                    else
                    {
                        sourceManager.FixOrder(ss);
                    }

                }
                else
                {
                    Logger.Log(DebugLevel.ERRORS, "Could not find File: " + path, Verbosity.ALWAYS_SEND);
                    /*if (path != "")*/ return false; //We crash if we didnt find the file. but if the user forgets to specify the path we will just log the error
                }

            }

            Logger.Log(DebugLevel.LOGS, "Inclusion of Files Finished", Verbosity.LEVEL3);
            return true;

        }


        private bool GetIncludeSourcePath(string statement, string currentPath, out string filePath, out string key, out Dictionary<string, object> pluginCache)
        {
            var vars = Utils.SplitAndRemoveFirst(statement, _separator);
            //genParams = new string[0];
            key=filePath = "";
            pluginCache = new Dictionary<string, object>();
            if (vars.Length != 0)
            {

                //filePath = vars[0];
                if(!ASourceManager.KeyComputingScheme(vars, out filePath, out key, out pluginCache))
                {
                    return false;
                }
                


                if (!Utils.FileExists(currentPath, filePath))
                {
                    return false;
                }

                return true;

            }


            return false;
        }
    }
}