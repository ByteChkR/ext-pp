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
                "Sets the keyword that is used to include other files into the build process."),
            new CommandInfo("set-include-inline", "ii", PropertyHelper.GetFieldInfo(typeof(IncludePlugin), nameof(IncludeInlineKeyword)),
                "Sets the keyword that is used to insert other files directly into the current file"),
            new CommandInfo("set-separator","s", PropertyHelper.GetFieldInfo(typeof(IncludePlugin), nameof(Separator)),
                "Sets the separator that is used to separate the include statement from the filepath"),
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

            this.Log(DebugLevel.LOGS, Verbosity.LEVEL5, "Disovering Include Statments...");
            List<string> source = script.GetSource().ToList();
            string currentPath = Path.GetDirectoryName(Path.GetFullPath(script.GetFilePath()));

            for (int i = source.Count - 1; i >= 0; i--)
            {
                if (Utils.IsStatement(source[i], IncludeInlineKeyword))
                {
                    this.Log(DebugLevel.LOGS, Verbosity.LEVEL6, "Found Inline Include Statement...");
                    string[] args = Utils.SplitAndRemoveFirst(source[i], Separator);
                    if (args.Length == 0)
                    {

                        this.Log(DebugLevel.WARNINGS, Verbosity.LEVEL1, "No File Specified");
                        continue;
                    }

                    if (Utils.FileExistsRelativeTo(currentPath, args[0]))
                    {
                        this.Log(DebugLevel.LOGS, Verbosity.LEVEL6, "Replacing Inline Keyword with file content");
                        source.RemoveAt(i);

                        source.InsertRange(i, File.ReadAllLines(args[0]));
                    }
                    else
                        this.Log(DebugLevel.WARNINGS, Verbosity.LEVEL1, "File does not exist: {0}" , args[0]);
                }
            }
            script.SetSource(source.ToArray());


            string[] incs = Utils.FindStatements(source.ToArray(), IncludeKeyword);


            foreach (var includes in incs)
            {
                this.Log(DebugLevel.LOGS, Verbosity.LEVEL5, "Processing Statement: {0}", includes);
                bool tmp = GetISourceScript(sourceManager, includes, currentPath, out List<ISourceScript> sources);
                if (tmp)
                {
                    foreach (var sourceScript in sources)
                    {
                        this.Log(DebugLevel.LOGS, Verbosity.LEVEL6, "Processing Include: {0}" , Path.GetFileName(sourceScript.GetFilePath()));

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

            this.Log(DebugLevel.LOGS, Verbosity.LEVEL5, "Inclusion of Files Finished");
            return true;

        }


        private bool GetISourceScript(ISourceManager manager, string statement, string currentPath, out List<ISourceScript> scripts)
        {
            var vars = Utils.SplitAndRemoveFirst(statement, Separator);
            //genParams = new string[0];
            scripts = new List<ISourceScript>();
            if (vars.Length != 0)
            {
                //filePath = vars[0];
                if (!manager.GetComputingScheme()(vars, currentPath, out var filepath, out var key, out var pluginCache))
                {
                    this.Log(DebugLevel.ERRORS, Verbosity.LEVEL1, "Invalid Include Statement");
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
                        this.Log(DebugLevel.ERRORS, Verbosity.LEVEL1, "Could not find File: {0}", currentPath);
                        scripts.RemoveAt(index);
                    }
                }


                return true;

            }


            return false;
        }
    }
}