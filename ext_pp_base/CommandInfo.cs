using System.Reflection;

namespace ext_pp_base
{
    public struct CommandMetaData
    {
        public readonly string HelpText;
        public readonly string Command;
        public readonly string ShortCut;
        public readonly bool IncludeGlobal;

        public CommandMetaData(string command, string shortcut, string helpText, bool global)
        {

            Command = command;
            HelpText = helpText;
            ShortCut = shortcut;
            IncludeGlobal = global;
        }
        public override string ToString()
        {
            return Command + "(" + ShortCut + "): " + HelpText;
        }
    }
    public struct CommandInfo
    {

        public string HelpText => Meta.HelpText;
        public string Command=>Meta.Command;
        public string ShortCut => Meta.ShortCut;
        public bool IncludeGlobal => Meta.IncludeGlobal;

        public readonly FieldInfo Field;
        public readonly CommandMetaData Meta;
        public readonly object DefaultIfNotSpecified;
        public CommandInfo(string command, string shortcut, FieldInfo field, string helpText, object defaultIfNotSpecified = null, bool global = false)
        {
            Field = field;
            Meta=new CommandMetaData(command, shortcut, helpText, global);
            DefaultIfNotSpecified = defaultIfNotSpecified;
        }

        public override string ToString()
        {
            return Meta.ToString();
        }
    }
}