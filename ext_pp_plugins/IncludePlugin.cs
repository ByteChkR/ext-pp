using System.Collections.Generic;
using System.IO;
using System.Linq;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp_plugins
{
    public class IncludePlugin : AbstractFullScriptPlugin
    {
        public override string[] Cleanup => new[] { IncludeKeyword };
        public override ProcessStage ProcessStages => ProcessStage.ON_MAIN;
        public override string[] Prefix => new[] { "inc", "Include" };
        public string IncludeKeyword { get; set; } = "#include";
        public string IncludeInlineKeyword { get; set; } = "#includeinl";
        public string Separator { get; set; } = " ";

        public override List<CommandInfo> Info { get; } = new List<CommandInfo>
        {
            new CommandInfo("set-include", "i", PropertyHelper.GetPropertyInfo(typeof(IncludePlugin), nameof(IncludeKeyword)),
                "Sets the keyword that is used to include other files into the build process."),
            new CommandInfo("set-include-inline", "ii", PropertyHelper.GetPropertyInfo(typeof(IncludePlugin), nameof(IncludeInlineKeyword)),
                "Sets the keyword that is used to insert other files directly into the current file"),
            new CommandInfo("set-separator","s", PropertyHelper.GetPropertyInfo(typeof(IncludePlugin), nameof(Separator)),
                "Sets the separator that is used to separate the include statement from the filepath"),
            };
        public override void Initialize(Settings settings, ISourceManager sourceManager, IDefinitions defTable)
        {

            settings.ApplySettings(Info, this);
        }


        public override bool FullScriptStage(ISourceScript script, ISourceManager sourceManager, IDefinitions defs)
        {

            this.Log(DebugLevel.LOGS, Verbosity.LEVEL5, "Disovering Include Statments...");
            List<string> source = script.GetSource().ToList();
            string currentPath = Path.GetDirectoryName(script.GetFileInterface().GetFilePath());
            bool hasIncludedInline;
            do
            {
                hasIncludedInline = false;
                for (int i = source.Count - 1; i >= 0; i--)
                {
                    if (Utils.IsStatement(source[i], IncludeInlineKeyword))
                    {
                        this.Log(DebugLevel.LOGS, Verbosity.LEVEL6, "Found Inline Include Statement...");
                        string[] args = Utils.SplitAndRemoveFirst(source[i], Separator);
                        if (args.Length == 0)
                        {

                            this.Warning("No File Specified");
                            continue;
                        }

                        if (Utils.FileExistsRelativeTo(currentPath, args[0]))
                        {
                            this.Log(DebugLevel.LOGS, Verbosity.LEVEL6, "Replacing Inline Keyword with file content");
                            source.RemoveAt(i);

                            source.InsertRange(i, File.ReadAllLines(Path.GetFullPath(args[0], currentPath)));
                            hasIncludedInline = true;
                        }
                        else
                        {
                            this.Warning("File does not exist: {0}", args[0]);
                        }
                    }
                }
                script.SetSource(source.ToArray());
            } while (hasIncludedInline);


            string[] incs = Utils.FindStatements(source.ToArray(), IncludeKeyword);

            foreach (var includes in incs)
            {
                this.Log(DebugLevel.LOGS, Verbosity.LEVEL5, "Processing Statement: {0}", includes);
                bool tmp = GetISourceScript(sourceManager, includes, currentPath, out List<ISourceScript> sources);
                if (tmp)
                {
                    foreach (var sourceScript in sources)
                    {
                        this.Log(DebugLevel.LOGS, Verbosity.LEVEL6, "Processing Include: {0}", Path.GetFileName(sourceScript.GetFileInterface().GetKey()));

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
                    return false; //We crash if we didnt find the file. but if the user forgets to specify the path we will just log the error
                }

            }

            this.Log(DebugLevel.LOGS, Verbosity.LEVEL5, "Inclusion of Files Finished");
            return true;

        }


        private bool GetISourceScript(ISourceManager manager, string statement, string currentPath, out List<ISourceScript> scripts)
        {
            var vars = Utils.SplitAndRemoveFirst(statement, Separator);

            scripts = new List<ISourceScript>();
            if (vars.Length != 0)
            {
                ImportResult importInfo = manager.GetComputingScheme()(vars, currentPath);
                if (!importInfo)
                {
                    this.Error("Invalid Include Statement");
                    return false;

                }

                string filepath = importInfo.GetString("filename");
                importInfo.RemoveEntry("filename");
                string key = importInfo.GetString("key");
                importInfo.RemoveEntry("key");


                if (filepath.EndsWith("\\*") || filepath.EndsWith("/*"))
                {
                    string[] files = Directory.GetFiles(filepath.Substring(0, filepath.Length - 2));
                    foreach (var file in files)
                    {
                        IFileContent cont = new FilePathContent(file);
                        cont.SetKey(key);
                        if (manager.TryCreateScript(out ISourceScript iss, Separator, cont, importInfo))
                        {
                            scripts.Add(iss);
                        }
                    }
                }
                else
                {
                    IFileContent cont = new FilePathContent(filepath);
                    cont.SetKey(key);
                    if (manager.TryCreateScript(out ISourceScript iss, Separator, cont, importInfo))
                    {
                        scripts.Add(iss);
                    }
                }


                for (var index = scripts.Count - 1; index >= 0; index--)
                {
                    var sourceScript = scripts[index];
                    if (sourceScript.GetFileInterface().HasValidFilepath && !Utils.FileExistsRelativeTo(currentPath, sourceScript.GetFileInterface()))
                    {
                        this.Error("Could not find File: {0}", sourceScript.GetFileInterface());
                        scripts.RemoveAt(index);
                    }
                }


                return true;

            }


            return false;
        }
    }
}