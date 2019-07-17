using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp_plugins
{
    struct Breakpoint
    {
        private int line;
        private string filename;
        private int stage;

        private Breakpoint(int line, string filename, int stage)
        {
            this.line = line;
            this.filename = filename;
            this.stage = stage;
        }

        public bool Break(int line, string filename)
        {
            return this.line == line && (this.filename == "*" || this.filename == filename);
        }

        public static Breakpoint[] Parse(string breakpointstr)
        {
            return Parse(breakpointstr.Pack(" ").ToArray());
        }

        private static string GetInput()
        {
            return Console.ReadLine();

        }

        public static Breakpoint[] Parse(string[] breakpointarr)
        {
            List<Breakpoint> points = new List<Breakpoint>();
            foreach (var breakpoint in breakpointarr)
            {
                bool continueCreation = true;
                List<string> args = breakpoint.Pack(":").ToList();
                int idx;
                Breakpoint b = new Breakpoint(1, "*", -1);
                if ((idx = args.IndexOf("file")) != -1 && args.Count - 1 > idx)
                {
                    b.filename = args[idx + 1];
                    b.filename = Path.GetFullPath(b.filename);
                }
                if ((idx = args.IndexOf("stage")) != -1 && args.Count - 1 > idx)
                {
                    bool exit = false;
                    do
                    {

                        if (!continueCreation) args[idx + 1] = GetInput();
                        if (!continueCreation && args[idx + 1] == "-dbg-exit") exit = true;
                        if (!exit && !int.TryParse(args[idx + 1], out b.stage))
                        {
                            Logger.Log(DebugLevel.LOGS, "Stage is not a valid integer. To abort type -dbg-exit",
                                Verbosity.LEVEL1);
                            continueCreation = false;
                        }
                        else if (!exit) continueCreation = true;

                    } while (!continueCreation && !exit);
                }
                if ((idx = args.IndexOf("line")) != -1 && args.Count - 1 > idx)
                {
                    bool exit = false;
                    do
                    {

                        if (!continueCreation) args[idx + 1] = GetInput();
                        if (!continueCreation && args[idx + 1] == "-dbg-exit") exit = true;
                        if (!exit && !int.TryParse(args[idx + 1], out b.line))
                        {
                            Logger.Log(DebugLevel.LOGS, "Line is not a valid integer. To abort type -dbg-exit",
                                Verbosity.LEVEL1);
                            continueCreation = false;
                        }
                        else if (!exit) continueCreation = true;

                    } while (!continueCreation && !exit);
                }

                if (continueCreation) points.Add(b);
            }

            return points.ToArray();

        }
    }

    public class CLIDebugger : AbstractPlugin
    {
        public override PluginType PluginType => PluginType.FULL_SCRIPT_PLUGIN;
        public override ProcessStage ProcessStages => ProcessStage.ON_LOAD_STAGE | ProcessStage.ON_MAIN;

        public override string[] Prefix => new string[] { "dbg" };
        public string[] Breakpoints = null;
        private List<Breakpoint> _breakpoints = new List<Breakpoint>();
        private bool isBreaking = false;
        private int lineCount = 0;


        public override List<CommandInfo> Info { get; } = new List<CommandInfo>()
        {
            new CommandInfo("set-breakpoint", "bp", PropertyHelper.GetFieldInfo(typeof(CLIDebugger), nameof(Breakpoints)),
                "Sets the breakpoints for the session.\n" +
                "Syntax: \nfile:<filepath> file:<filepath> file:<filepath>\n" +
                "file:<filepath>:line:<line_nr>\n" +
                "stage:<stage_nr>\n" +
                "stage:<stage_nr>:file:<filepath>...\n" +
                "StageNrs: 1 = OnLoad; 2 = OnMain; 4 = OnFinishUp"),
        };



        public override void Initialize(Settings settings, ISourceManager sourceManager, IDefinitions defs)
        {
            settings.ApplySettings(Info, this);
            _breakpoints = Breakpoint.Parse(Breakpoints).ToList();
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
            List<string> source = file.GetSource().ToList();

            foreach (var breakpoint in _breakpoints)
            {
                for (lineCount = 0; lineCount < source.Count; lineCount++)
                {
                    if (isBreaking)
                    {
                        do
                        {
                            Logger.Log(DebugLevel.LOGS, "Type -dbg-continue to contine processing\nType -dbg-exit to exit the program\n" +
                                                                    "Type -dbg-file to list the current file from line you set the breakpoint\n" +
                                                                    "-dbg-file-all to list the whole file\n" +
                                                                    "-dbg-dump <pathtofile> dumps the current file source.\n" +
                                                                    "-dbg-add-bp <breakpointstring>", Verbosity.LEVEL1);
                            string getInput = Console.ReadLine();
                            if (getInput == "-dbg-continue")
                            {
                                isBreaking = false;
                                return true;
                            }
                            else if (getInput == "-dbg-exit")
                            {
                                return false;
                            }
                            else if (getInput == "-dbg-file")
                            {
                                source.TakeLast(source.Count - lineCount).ToList().ForEach(Console.WriteLine);
                            }
                            else if (getInput == "-dbg-file-all")
                            {
                                source.ForEach(Console.WriteLine);
                            }
                            else if (getInput.StartsWith("-dbg-dump "))
                            {
                                string ff = getInput.Split(" ")[1];
                                if (ff != "")File.WriteAllLines(ff, source);
                            }
                            else if (getInput.StartsWith("-dbg-add-bp "))
                            {
                                Breakpoint[] ff = Breakpoint.Parse(getInput.Split(" "));
                                _breakpoints.AddRange(ff);
                            }
                        } while (isBreaking);
                    }
                    if (breakpoint.Break(lineCount, file.GetKey()))
                    {
                        isBreaking = true;
                    }
                }

            }



            return true;
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


        public string LineStage(string source)
        {
            return source;
        }

    }
}