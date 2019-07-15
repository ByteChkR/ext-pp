using System.Reflection;

namespace ext_pp_base
{
    public struct CommandInfo
    {
        public readonly string HelpText;
        public readonly string Command;
        public readonly FieldInfo Field;
        public CommandInfo(string command, FieldInfo field, string helpText = "No help text available for this item")
        {
            Command = command;
            Field = field;
            HelpText = helpText;
        }

        public override string ToString()
        {
            return Command + "(" + Field.Name + "): " + HelpText;
        }
    }
}