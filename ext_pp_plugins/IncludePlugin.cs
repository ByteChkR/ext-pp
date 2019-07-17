using System.Collections.Generic;
using System.IO;
using System.Linq;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp_plugins
{
    public class IncludePlugin : AbstractPlugin
    {
        public override string[] Cleanup => new string[] { IncludeKeyword };
        public override PluginType PluginType => PluginType.FULL_SCRIPT_PLUGIN;
        public override ProcessStage ProcessStages => ProcessStage.ON_MAIN;
        public override string[] Prefix => new string[] { "inc" ,"Include"};
        public string IncludeKeyword = "#include";
        public string IncludeInlineKeyword = "#includeinl";
        public string Separator = " ";
        public override List<CommandInfo> Info { get; } = new List<CommandInfo>()
        {
            new CommandInfo("set-include", "i", PropertyHelper.GetFieldInfo(typeof(IncludePlugin), nameof(IncludeKeyword)),
                "set-include [include keyword] *#include*\r\n\t\t\tSets the keyword that is used to include other files into the build process."),
            new CommandInfo("set-include-inline", "ii", PropertyHelper.GetFieldInfo(typeof(IncludePlugin), nameof(IncludeInlineKeyword)),
                "set-include-inline [include keyword] *#includeinl*\r\n\t\t\tSets the keyword that is used to insert other files directly into the current file"),
            new CommandInfo("set-separator","s", PropertyHelper.GetFieldInfo(typeof(IncludePlugin), nameof(Separator)),
                "set-separator [separator keyword] * *\r\n\t\t\tSets the separator that is used to separate the include statement from the filepath")
        };
        public override void Initialize(Settings settings, ISourceManager sourceManager, IDefinitions defTable)
        {

            settings.ApplySettings(Info, this);
        }

        public override bool OnMain_FullScriptStage(ISourceScript script, ISourceManager sourceManager, IDefinitions defTable)
        {
            return FullScriptStage(script, sourceManager, defTable);
        }

        public bool FullScriptStage(ISourceScript script, ISourceManager sourceManager, IDefinitions defs)
        {

            Logger.Log(DebugLevel.LOGS, "Disovering Include Statments...", Verbosity.LEVEL4);
            List<string> source = script.GetSource().ToList();
            string currentPath = Path.GetDirectoryName(Path.GetFullPath(script.GetFilePath()));

            for (int i = source.Count - 1; i >= 0; i--)
            {
                if (Utils.IsStatement(source[i], IncludeInlineKeyword))
                {
                    Logger.Log(DebugLevel.LOGS, "Found Inline Include Statement...", Verbosity.LEVEL4);
                    string[] args = Utils.SplitAndRemoveFirst(source[i], Separator);
                    if (args.Length == 0)
                    {

                        Logger.Log(DebugLevel.WARNINGS, "No File Specified", Verbosity.LEVEL1);
                        continue;
                    }

                    if (Utils.FileExistsRelativeTo(currentPath, args[0]))
                    {
                        Logger.Log(DebugLevel.LOGS, "Replacing Inline Keyword with file content", Verbosity.LEVEL4);
                        source.RemoveAt(i);

                        source.InsertRange(i, File.ReadAllLines(args[0]));
                    }
                    else
                        Logger.Log(DebugLevel.WARNINGS, "File does not exist: " + args[0], Verbosity.LEVEL1);
                }
            }
            script.SetSource(source.ToArray());


            string[] incs = Utils.FindStatements(source.ToArray(), IncludeKeyword);


            foreach (var includes in incs)
            {
                Logger.Log(DebugLevel.LOGS, "Processing Statement: " + includes, Verbosity.LEVEL5);
                bool tmp = GetISourceScript(sourceManager, includes, currentPath, out List<ISourceScript> sources);
                if (tmp)
                {
                    foreach (var sourceScript in sources)
                    {
                        Logger.Log(DebugLevel.LOGS, "Processing Include: " + sourceScript.GetKey(), Verbosity.LEVEL5);

                        if (!sourceManager.IsIncluded(sourceScript))
                        {
                            sourceManager.AddToTodo(sourceScript);
                        }
                        else
                        {
                            sourceManager.FixOrder(sourceScript);
                        }

                    }

                }
                else
                {
                    /*if (path != "")*/
                    return false; //We crash if we didnt find the file. but if the user forgets to specify the path we will just log the error
                }

            }

            Logger.Log(DebugLevel.LOGS, "Inclusion of Files Finished", Verbosity.LEVEL4);
            return true;

        }


        private bool GetISourceScript(ISourceManager manager, string statement, string currentPath, out List<ISourceScript> scripts)
        {
            var vars = Utils.SplitAndRemoveFirst(statement, Separator);
            //genParams = new string[0];
            scripts = new List<ISourceScript>();
            if (vars.Length != 0)
            {
                string filepath, key;
                Dictionary<string, object> pluginCache;
                //filePath = vars[0];
                if (!manager.GetComputingScheme()(vars, currentPath, out filepath, out key, out pluginCache))
                {
                    Logger.Log(DebugLevel.ERRORS, "Invalid Include Statement", Verbosity.LEVEL1);
                    return false;

                }


                if (filepath.EndsWith("\\*") || filepath.EndsWith("/*"))
                {
                    string[] files = Directory.GetFiles(filepath.Substring(0, filepath.Length - 2));
                    foreach (var file in files)
                    {

                        if (manager.CreateScript(out ISourceScript iss, Separator, file, key.Replace(filepath, file), pluginCache))
                        {

                            scripts.Add(iss);
                        }
                    }
                }
                else
                {
                    if (manager.CreateScript(out ISourceScript iss, Separator, filepath, key, pluginCache))
                        scripts.Add(iss);
                }


                for (var index = scripts.Count - 1; index >= 0; index--)
                {
                    var sourceScript = scripts[index];
                    if (!Utils.FileExistsRelativeTo(currentPath, sourceScript.GetFilePath()))
                    {
                        Logger.Log(DebugLevel.ERRORS, "Could not find File: " + currentPath, Verbosity.LEVEL1);
                        scripts.RemoveAt(index);
                    }
                }


                return true;

            }


            return false;
        }
    }
}