using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp_plugins
{
    public class ErrorPlugin : IPlugin
    {

        public string[] Cleanup => new string[0];
        public string[] Prefix => new string[] { "err" };
        public bool IncludeGlobal => true;
        public string ErrorKeyword = "#error";
        public string Separator = " ";
        public List<CommandInfo> Info { get; } = new List<CommandInfo>()
        {
            new CommandInfo("e", PropertyHelper.GetFieldInfo(typeof(ErrorPlugin), nameof(ErrorKeyword)),
                "Sets the keyword that will be used to trigger errors during compilation"),
            new CommandInfo("s", PropertyHelper.GetFieldInfo(typeof(ErrorPlugin), nameof(Separator)),
                "Sets the characters that will be used to separate strings")
        };



        public void Initialize(Settings settings, ISourceManager sourceManager, IDefinitions defTable)
        {

            settings.ApplySettings(Info, this);
        }

        public bool Process(ISourceScript file, ISourceManager todo, IDefinitions defs)
        {
            Logger.Log(DebugLevel.LOGS, "Discovering Errors...", Verbosity.LEVEL4);
            string[] errors = Utils.FindStatements(file.GetSource(), ErrorKeyword);
            foreach (var t in errors)
            {
                Logger.Log(DebugLevel.ERRORS, "Error(" + Path.GetFileName(file.GetFilePath()) + "): " + errors.Unpack(Separator), Verbosity.LEVEL1);
            }

            Logger.Log(DebugLevel.LOGS, "Error Detection Finished", Verbosity.LEVEL4);
            return errors.Length == 0;
        }

    }
}