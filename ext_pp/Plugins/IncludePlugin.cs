using System.Collections.Generic;
using System.IO;
using System.Linq;
using ext_pp.settings;

namespace ext_pp.plugins
{
    public class IncludePlugin : IPlugin
    {
        public string[] Cleanup => new string[0];
        private readonly string _includeKeyword = Settings.IncludeStatement;
        private readonly string _separator = Settings.Separator;
        public IncludePlugin(Settings settings)
        {

        }

        public bool Process(SourceScript script, SourceManager sourceManager, Definitions defs)
        {

            Logger.Log(DebugLevel.LOGS, "Disovering Include Statments...", Verbosity.LEVEL3);
            string[] incs = Utils.FindStatements(script.Source, _includeKeyword);


            foreach (var includes in incs)
            {
                Logger.Log(DebugLevel.LOGS, "Processing Statement: " + includes, Verbosity.LEVEL4);
                bool tmp = GetIncludeSourcePath(includes, Path.GetDirectoryName(Path.GetFullPath(script.Filepath)), out var path, out var genParams);
                if (tmp)
                {
                    SourceScript ss = new SourceScript(_separator, path, genParams);
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

        private bool GetIncludeSourcePath(string statement, string currentPath, out string filePath, out string[] genParams)
        {
            var vars = Utils.SplitAndRemoveFirst(statement, _separator);
            genParams = new string[0];
            filePath = "";
            if (vars.Length != 0)
            {
                filePath = vars[0];
                genParams = vars.Length > 1 ?
                    vars.SubArray(1, vars.Length - 1).ToArray() : genParams;

                if (!Utils.FileExists(currentPath, filePath))
                {
                    return false;
                }
                else
                {
                    filePath = Path.GetFullPath(filePath);
                    return true;
                }

            }


            return false;
        }
    }
}