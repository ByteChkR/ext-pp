using System.Collections.Generic;
using System.IO;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp_plugins
{
    public class IncludePlugin : IPlugin
    {
        public string[] Cleanup => new string[0];
        public string[] Prefix => new string[] { "inc" };
        public bool IncludeGlobal => true;
        public string IncludeKeyword = "#include";
        public string Separator = " ";
        public List<CommandInfo> Info { get; } = new List<CommandInfo>()
        {
            new CommandInfo("i", PropertyHelper.GetFieldInfo(typeof(IncludePlugin), nameof(IncludeKeyword)),
                "Sets the keyword that will be used to reference other files during processing"),
            new CommandInfo("s", PropertyHelper.GetFieldInfo(typeof(IncludePlugin), nameof(Separator)),
                "Sets the characters that will be used to separate strings")
        };
        public void Initialize(Settings settings, ISourceManager sourceManager, IDefinitions defTable)
        {

            settings.ApplySettingsFlatString(Info, this);
        }

        public bool Process(ISourceScript script, ISourceManager sourceManager, IDefinitions defs)
        {

            Logger.Log(DebugLevel.LOGS, "Disovering Include Statments...", Verbosity.LEVEL4);
            string[] incs = Utils.FindStatements(script.GetSource(), IncludeKeyword);


            foreach (var includes in incs)
            {
                Logger.Log(DebugLevel.LOGS, "Processing Statement: " + includes, Verbosity.LEVEL5);
                bool tmp = GetIncludeSourcePath(sourceManager, includes, Path.GetDirectoryName(Path.GetFullPath(script.GetFilePath())), out var path, out var key, out Dictionary<string, object> pluginCache);
                if (tmp)
                {

                    ISourceScript ss = sourceManager.CreateScript(Separator, path, key, pluginCache);
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
                    Logger.Log(DebugLevel.ERRORS, "Could not find File: " + path, Verbosity.LEVEL1);
                    /*if (path != "")*/
                    return false; //We crash if we didnt find the file. but if the user forgets to specify the path we will just log the error
                }

            }

            Logger.Log(DebugLevel.LOGS, "Inclusion of Files Finished", Verbosity.LEVEL4);
            return true;

        }


        private bool GetIncludeSourcePath(ISourceManager manager, string statement, string currentPath, out string filePath, out string key, out Dictionary<string, object> pluginCache)
        {
            var vars = Utils.SplitAndRemoveFirst(statement, Separator);
            //genParams = new string[0];
            key = filePath = "";
            pluginCache = new Dictionary<string, object>();
            if (vars.Length != 0)
            {

                //filePath = vars[0];
                if (!manager.GetComputingScheme()(vars, out filePath, out key, out pluginCache))
                {
                    return false;
                }



                if (!Utils.FileExistsRelativeTo(currentPath, filePath))
                {
                    return false;
                }

                return true;

            }


            return false;
        }
    }
}