using System.Reflection;

namespace ext_pp_base
{
    public struct CommandInfo
    {
        public readonly string HelpText;
        public readonly string Command;
        public readonly string ShortCut;
        public readonly bool IncludeGlobal;
        public readonly FieldInfo Field;
        public CommandInfo(string command, string shortcut, FieldInfo field, string helpText, bool global = false)
        {
            Command = command;
            Field = field;
            HelpText = helpText;
            ShortCut = shortcut;
            IncludeGlobal = global;
        }

        public override string ToString()
        {
            return Command + "(" + ShortCut + "): " + HelpText;
        }
    }
}