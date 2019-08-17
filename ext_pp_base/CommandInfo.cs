using System;
using System.Reflection;

namespace ext_pp_base
{
    /// <summary>
    /// A Struct that contains all the information about the plugin
    /// </summary>
    [Serializable]
    public struct CommandMetaData
    {


        public string HelpText { get; }

        /// <summary>
        /// The shortcut for the command
        /// Can be accessed with -
        /// </summary>
        public string ShortCut { get; }

        /// <summary>
        /// If this parameter can be set by a global prefix
        /// </summary>
        public bool IncludeGlobal { get; }

        /// <summary>
        /// The command.
        /// Can be accessed with --
        /// </summary>
        public string Command { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="command"></param>
        /// <param name="shortcut"></param>
        /// <param name="helpText"></param>
        /// <param name="global"></param>
        public CommandMetaData(string command, string shortcut, string helpText, bool global)
        {

            Command = command;
            HelpText = helpText;
            ShortCut = shortcut;
            IncludeGlobal = global;
        }

        /// <summary>
        /// Writes the meta data as readable text.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Command + "(" + ShortCut + "): " + HelpText;
        }
    }

    /// <summary>
    /// A struct that is used to define custom commands.
    /// </summary>
    public struct CommandInfo
    {
        /// <summary>
        /// The help text of the command
        /// </summary>
        public string HelpText => Meta.HelpText;
        /// <summary>
        /// The primary command.
        /// Can be accessed with --
        /// </summary>
        public string Command => Meta.Command;
        /// <summary>
        /// The shortcut for the command
        /// Can be accessed with -
        /// </summary>
        public string ShortCut => Meta.ShortCut;
        /// <summary>
        /// If this parameter can be set by a global prefix
        /// </summary>
        public bool IncludeGlobal => Meta.IncludeGlobal;

        /// <summary>
        /// The field that will be set with reflection
        /// </summary>
        public readonly FieldInfo Field;
        /// <summary>
        /// Wrapper to separate serializable info from the command info.
        /// </summary>
        public readonly CommandMetaData Meta;
        /// <summary>
        /// If true will set the value of the command to the default value if not specified directly in the settings
        /// </summary>
        public readonly object DefaultIfNotSpecified;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="command"></param>
        /// <param name="shortcut"></param>
        /// <param name="field"></param>
        /// <param name="helpText"></param>
        /// <param name="defaultIfNotSpecified"></param>
        /// <param name="global"></param>
        public CommandInfo(string command, string shortcut, FieldInfo field, string helpText, object defaultIfNotSpecified = null, bool global = false)
        {
            Field = field;
            Meta = new CommandMetaData(command, shortcut, helpText, global);
            DefaultIfNotSpecified = defaultIfNotSpecified;
        }

        /// <summary>
        /// Writes the information as readable text.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Meta.ToString();
        }
    }
}