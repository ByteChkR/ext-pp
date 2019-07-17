﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp
{

    /// <summary>
    /// 
    /// </summary>
    public class PreProcessor : ILoggable
    {
        /// <summary>
        /// List of loaded plugins
        /// </summary>
        private List<AbstractPlugin> _plugins = new List<AbstractPlugin>();


        /// <summary>
        /// 
        /// </summary>
        private readonly string _sep = " ";
        //Create Global Definitions
        //Create Stack for All the processing steps(Stack<List<IPlugin>>

        /// <summary>
        /// Returns the List of statements from all the plugins that are remaining in the file and need to be removed as a last step
        /// </summary>
        private List<string> CleanUpList
        {
            get
            {
                var ret = new List<string>();

                foreach (var plugin in _plugins)
                {
                    ret.AddRange(plugin.Cleanup);
                }
                return ret;

            }
        }

        /// <summary>
        /// Sets the File Processing Chain
        /// 0 => First Plugin that gets executed
        /// </summary>
        /// <param name="fileProcessors"></param>
        public void SetFileProcessingChain(List<AbstractPlugin> fileProcessors)
        {
            _plugins = fileProcessors;
        }


        /// <summary>
        /// Compiles a File with the definitions and settings provided
        /// </summary>
        /// <param name="files">FilePaths of the files.</param>
        /// <param name="settings"></param>
        /// <param name="defs">Definitions</param>
        /// <returns>Array of Compiled Lines</returns>
        public string[] Compile(string[] files, Settings settings = null, IDefinitions defs = null)
        {

            this.Log(DebugLevel.LOGS, "Starting Post Processor...", Verbosity.LEVEL1);
            ISourceScript[] src = Process(files, settings, defs);
            return Compile(src);
        }


        /// <summary>
        /// Initializing all Plugins with the settings, definitions and the source manager for this compilation
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="def"></param>
        /// <param name="sourceManager"></param>
        private void InitializePlugins(Settings settings, IDefinitions def, ISourceManager sourceManager)
        {
            this.Log(DebugLevel.LOGS, "Initializing Plugins...", Verbosity.LEVEL1);
            foreach (var plugin in _plugins)
            {
                this.Log(DebugLevel.LOGS, "Initializing Plugin: " + plugin.GetType().Name, Verbosity.LEVEL2);

                plugin.Initialize(settings.GetSettingsWithPrefix(plugin.Prefix, plugin.IncludeGlobal), sourceManager, def);
            }
        }

        /// <summary>
        /// Compiles the Provided source array into a single file. And removes all remaining statements
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        private string[] Compile(ISourceScript[] src)
        {
            this.Log(DebugLevel.LOGS, "Starting Compilation of File Tree...", Verbosity.LEVEL2);
            List<string> ret = new List<string>();
            for (var i = src.Length - 1; i >= 0; i--)
            {
                ret.AddRange(src[i].GetSource());
            }

            this.Log(DebugLevel.LOGS, "Finished Compilation...", Verbosity.LEVEL2);
            this.Log(DebugLevel.LOGS, "Cleaning up: " + CleanUpList.Unpack(", "), Verbosity.LEVEL3);

            string[] rrr = Utils.RemoveStatements(ret, CleanUpList.ToArray(),this).ToArray();
            this.Log(DebugLevel.LOGS, "Total Lines: " + rrr.Length, Verbosity.LEVEL2);
            return rrr;
        }


        private delegate bool PluginStage(ISourceScript script, ISourceManager sourceManager, IDefinitions defTable);


        /// <summary>
        /// Processes the file with the settings, definitions and the source manager specified.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="settings"></param>
        /// <param name="defs"></param>
        /// <returns>Returns a list of files that can be compiled in reverse order</returns>
        public ISourceScript[] Process(string[] files, Settings settings = null, IDefinitions defs = null)
        {
            string dir = Directory.GetCurrentDirectory();
            defs = defs ?? new Definitions();
            SourceManager sm = new SourceManager(_plugins);

            InitializePlugins(settings, defs, sm);

            this.Log(DebugLevel.LOGS, "Starting Processing of Files: " + files.Unpack(", "), Verbosity.LEVEL1);
            foreach (var file in files)
            {
                string f = Path.GetFullPath(file);
                Directory.SetCurrentDirectory(Path.GetDirectoryName(f));

                sm.SetLock(false);
                sm.CreateScript(out ISourceScript sss, _sep, f, f, new Dictionary<string, object>());
                sm.SetLock(true);
                List<ISourceScript> all = new List<ISourceScript>();
                sm.AddToTodo(sss);
            }

            ISourceScript ss = sm.NextItem;

            do
            {

                if (!(ss as SourceScript).IsSourceLoaded) RunStages(ProcessStage.ON_LOAD_STAGE, ss, sm, defs);

                this.Log(DebugLevel.PROGRESS, "Remaining Files: " + sm.GetTodoCount(), Verbosity.LEVEL1);
                this.Log(DebugLevel.LOGS, "Selecting File :" + Path.GetFileName(ss.GetFilePath()), Verbosity.LEVEL2);
                //RUN MAIN
                sm.SetLock(false);
                RunStages(ProcessStage.ON_MAIN, ss, sm, defs);
                sm.SetLock(true);
                sm.SetState(ss, ProcessStage.ON_FINISH_UP);
                ss = sm.NextItem;
            } while (ss != null);


            Directory.SetCurrentDirectory(dir);
            ISourceScript[] ret = sm.GetList().ToArray();
            this.Log(DebugLevel.LOGS, "Finishing Up...", Verbosity.LEVEL1);
            foreach (var finishedScript in ret)
            {
                this.Log(DebugLevel.LOGS, "Selecting File :" + Path.GetFileName(finishedScript.GetFilePath()), Verbosity.LEVEL2);
                RunStages(ProcessStage.ON_FINISH_UP, finishedScript, sm, defs);
            }
            this.Log(DebugLevel.LOGS, "Finished Processing Files.", Verbosity.LEVEL1);
            return ret;

        }

        private bool RunStages(ProcessStage stage, ISourceScript script, ISourceManager sourceManager,
            IDefinitions defTable)
        {
            if (!RunPluginStage(PluginType.LINE_PLUGIN_BEFORE, stage, script, sourceManager, defTable)) return false;
            if (stage != ProcessStage.ON_FINISH_UP)
                if (!RunPluginStage(PluginType.FULL_SCRIPT_PLUGIN, stage, script, sourceManager, defTable)) return false;
            if (!RunPluginStage(PluginType.LINE_PLUGIN_AFTER, stage, script, sourceManager, defTable)) return false;
            return true;
        }



        private bool RunPluginStage(PluginType type, ProcessStage stage, ISourceScript script, ISourceManager sourceManager, IDefinitions defTable)
        {
            List<AbstractPlugin> chain = AbstractPlugin.GetPluginsForStage(_plugins, type, stage);


            bool ret = true;

            if (type == PluginType.FULL_SCRIPT_PLUGIN)
            {
                ret = RunFullScriptStage(chain, stage, script, sourceManager, defTable);
            }
            else if (type == PluginType.LINE_PLUGIN_BEFORE || type == PluginType.LINE_PLUGIN_AFTER)
            {
                string[] src = script.GetSource();
                RunLineStage(chain, stage, src);
                script.SetSource(src);
            }
            if (!ret)
            {
                return false;
            }


            return true;
        }



        private void RunLineStage(List<AbstractPlugin> _lineStage, ProcessStage stage, string[] source)
        {
            foreach (var abstractPlugin in _lineStage)
            {
                this.Log(DebugLevel.LOGS, "Running Plugin :" + abstractPlugin + ":" + stage, Verbosity.LEVEL3);
                for (int i = 0; i < source.Length; i++)
                {
                    if (stage == ProcessStage.ON_LOAD_STAGE)
                    {
                        source[i] = abstractPlugin.OnLoad_LineStage(source[i]);
                    }
                    else if (stage == ProcessStage.ON_MAIN)
                    {
                        source[i] = abstractPlugin.OnMain_LineStage(source[i]);
                    }
                    else if (stage == ProcessStage.ON_FINISH_UP)
                    {
                        source[i] = abstractPlugin.OnFinishUp_LineStage(source[i]);
                    }
                }
            }

        }


        private bool RunFullScriptStage(List<AbstractPlugin> _fullScriptStage, ProcessStage stage, ISourceScript script, ISourceManager sourceManager, IDefinitions defTable)
        {
            foreach (var abstractPlugin in _fullScriptStage)
            {
                bool ret = true;
                this.Log(DebugLevel.LOGS, "Running Plugin :" + abstractPlugin + ":" + stage + " on file " + Path.GetFileName(script.GetFilePath()), Verbosity.LEVEL3);
                if (stage == ProcessStage.ON_LOAD_STAGE)
                {
                    ret = abstractPlugin.OnLoad_FullScriptStage(script, sourceManager, defTable);
                }
                else if (stage == ProcessStage.ON_MAIN)
                {
                    ret = abstractPlugin.OnMain_FullScriptStage(script, sourceManager, defTable);
                }

                if (!ret)
                {
                    this.Log(DebugLevel.ERRORS, "Processing was aborted by Plugin: " + abstractPlugin,
                        Verbosity.LEVEL1);
                    return false;
                }
            }

            return true;
        }
    }
}