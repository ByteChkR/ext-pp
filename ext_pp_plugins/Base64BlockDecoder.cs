using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp_plugins
{
    public class Base64BlockDecoder : AbstractPlugin
    {
        public override PluginType PluginTypeToggle => PluginType.FULL_SCRIPT_PLUGIN;
        public override ProcessStage ProcessStages => Stage.ToLower(CultureInfo.InvariantCulture) == "onload" ? ProcessStage.ON_LOAD_STAGE : ProcessStage.ON_MAIN;

        public override string[] Cleanup => new[]
        {
            BlockEncodeStartKeyword, BlockEncodeEndKeyword, BlockDecodeStartKeyword, BlockDecodeEndKeyword
        };


        public string Stage { get; set; } = "onload";

        public string BlockEncodeStartKeyword { get; set; } = "#block encode";

        public string BlockEncodeEndKeyword { get; set; } = "#endblock encode";

        public string BlockDecodeStartKeyword { get; set; } = "#block decode";

        public string BlockDecodeEndKeyword { get; set; } = "#endblock decode";

        public override string[] Prefix => new[] { "b64", "Base64BlockDecoder" };


        public override List<CommandInfo> Info { get; } = new List<CommandInfo>
        {
            new CommandInfo("set-stage","ss", PropertyHelper.GetPropertyInfo(typeof(Base64BlockDecoder), nameof(Stage)),
                "Sets the Stage Type of the Plugin to be Executed OnLoad or OnMain"),
            new CommandInfo("set-start-encode-keyword","ssek", PropertyHelper.GetPropertyInfo(typeof(Base64BlockDecoder), nameof(BlockEncodeStartKeyword)),
                "Sets the keyword that is used to open a Encode block"),
            new CommandInfo("set-end-encode-keyword","seek", PropertyHelper.GetPropertyInfo(typeof(Base64BlockDecoder), nameof(BlockEncodeEndKeyword)),
                "Sets the keyword that is used to end a Encode block"),
            new CommandInfo("set-start-decode-keyword","ssdk", PropertyHelper.GetPropertyInfo(typeof(Base64BlockDecoder), nameof(BlockDecodeStartKeyword)),
                "Sets the keyword that is used to open a Decode block"),
            new CommandInfo("set-end-decode-keyword","sedk", PropertyHelper.GetPropertyInfo(typeof(Base64BlockDecoder), nameof(BlockDecodeEndKeyword)),
                "Sets the keyword that is used to end a Decode block"),
        };



        public override void Initialize(Settings settings, ISourceManager sourceManager, IDefinitions defs)
        {
            List<CommandInfo> info = Info;
            settings.ApplySettings(info, this);
        }

        public override bool OnLoad_FullScriptStage(ISourceScript script, ISourceManager sourceManager,
            IDefinitions defTable)
        {
            return FullScriptStage(script, sourceManager, defTable);
        }

        public override bool OnMain_FullScriptStage(ISourceScript script, ISourceManager sourceManager,
            IDefinitions defTable)
        {
            return FullScriptStage(script, sourceManager, defTable);
        }

        public bool FullScriptStage(ISourceScript file, ISourceManager todo, IDefinitions defs)
        {


            bool done = false;
            do
            {
                done = true;
                List<string> lines = file.GetSource().ToList();
                List<int> removeIndices = new List<int>();
                for (int i = 0; i < lines.Count; i++)
                {
                    this.Log(DebugLevel.LOGS, Verbosity.LEVEL5, "Discovering Block Keywords.");
                    if (lines[i].TrimStart().StartsWith(BlockEncodeStartKeyword))
                    {
                        done = false;
                        removeIndices.Add(i);
                        i++;//Move forward.

                        this.Log(DebugLevel.LOGS, Verbosity.LEVEL6, "Found Block Encode Keyword.");
                        for (; i < lines.Count; i++)
                        {
                            if (lines[i].TrimStart().StartsWith(BlockEncodeEndKeyword))
                            {
                                removeIndices.Add(i);
                                i++;//Move Forward
                                break;
                            }

                            this.Log(DebugLevel.LOGS, Verbosity.LEVEL7, "Encoding line {0}.", i);
                            lines = Encode(lines, i);

                        }
                    }
                    else if (lines[i].TrimStart().StartsWith(BlockDecodeStartKeyword))
                    {
                        done = false;
                        removeIndices.Add(i);
                        i++;//Move forward.
                        this.Log(DebugLevel.LOGS, Verbosity.LEVEL6, "Found Block Decode Keyword.");

                        for (; i < lines.Count; i++)
                        {
                            if (lines[i].TrimStart().StartsWith(BlockDecodeEndKeyword))
                            {
                                removeIndices.Add(i);
                                i++;//Move Forward
                                break;
                            }
                            else
                            {
                                this.Log(DebugLevel.LOGS, Verbosity.LEVEL7, "Decoding line {0}.", i);

                                lines = Decode(lines, i);
                            }
                        }
                    }

                }

                for (int i = removeIndices.Count - 1; i >= 0; i--)
                {
                    lines.RemoveAt(removeIndices[i]);
                }
                removeIndices.Clear();
                

                file.SetSource(lines.ToArray());

            } while (!done);

            return true;
        }


        private static List<string> Decode(List<string> data, int index)
        {

            data[index] = Encoding.UTF8.GetString(Convert.FromBase64String(data[index]));



            return data;
        }


        private static List<string> Encode(List<string> content, int index)
        {

            content[index] = Convert.ToBase64String(Encoding.UTF8.GetBytes(content[index]));



            return content;
        }

        public override string OnLoad_LineStage(string source)
        {
            return LineStage(source);
        }

        public override string OnMain_LineStage(string source)
        {
            return LineStage(source);
        }

        public override string OnFinishUp_LineStage(string source)
        {
            return LineStage(source);
        }


        public static string LineStage(string source)
        {
            return source;
        }

    }
}