using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp
{

    /// <summary>
    /// A class that keeps track on what scripts are loaded and their processing state.
    /// This class also defines a Compute Scheme to alter the keys the file gets matched with, to enable loading the same file multiple times.
    /// </summary>
    public class SourceManager : ISourceManager, ILoggable
    {
        /// <summary>
        /// List of Scripts that are included in this Processing run
        /// </summary>
        private readonly List<ISourceScript> _sources = new List<ISourceScript>();

        /// <summary>
        /// The processing states of the scripts included.
        /// </summary>
        private readonly List<ProcessStage> _doneState = new List<ProcessStage>();

        /// <summary>
        /// The compute scheme that is used to assign keys to scripts(or instances of scripts)
        /// </summary>
        private DelKeyComputingScheme _computeScheme;

        /// <summary>
        /// Empty Constructor
        /// Sets the compute scheme to the default(the file name)
        /// </summary>
        public SourceManager(List<AbstractPlugin> pluginChain)
        {
            SetComputingScheme(ComputeFileNameAndKey_Default);
        }



        /// <summary>
        /// Sets the computing scheme to a custom scheme that will then be used to assign keys to scripts
        /// </summary>
        /// <param name="scheme"></param>
        public void SetComputingScheme(DelKeyComputingScheme scheme)
        {
            if (scheme == null)
            {
                return;
            }
            _computeScheme = scheme;
            this.Log(DebugLevel.LOGS, Verbosity.LEVEL2, "Changed Computing Scheme to: {0}", scheme.Method.Name);
        }

        public int GetTodoCount()
        {
            return _doneState.Count(x => x == ProcessStage.QUEUED);
        }

        /// <summary>
        /// Returns the computing scheme
        /// </summary>
        /// <returns></returns>
        public DelKeyComputingScheme GetComputingScheme()
        {
            return _computeScheme;
        }

        /// <summary>
        /// The default implementation of the key matching calculation
        /// </summary>
        /// <param name="vars"></param>
        /// <param name="filePath"></param>
        /// <param name="key"></param>
        /// <param name="pluginCache"></param>
        /// <returns></returns>
        private static bool ComputeFileNameAndKey_Default(string[] vars, string currentPath, out string filePath, out string key, out Dictionary<string, object> pluginCache)
        {
            pluginCache = new Dictionary<string, object>();
            filePath = "";
            key = "";

            if (vars.Length == 0)
            {
                return false;
            }
            string dir = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(currentPath);
            key =
                filePath = Path.GetFullPath(vars[0]);
            Directory.SetCurrentDirectory(dir);
            return true;
        }

        /// <summary>
        /// Returns the next item that can be processed
        /// if no items left returns null
        /// </summary>
        public ISourceScript NextItem
        {
            get
            {
                for (int i = 0; i < _doneState.Count; i++)
                {
                    if (_doneState[i] == ProcessStage.QUEUED)
                    {
                        return _sources[i];
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Fixes the order of the file tree if a script was being loaded and is now referenced (again)
        /// by removing it from the lower position and readding it at the top
        /// </summary>
        /// <param name="script"></param>
        public void FixOrder(ISourceScript script)
        {
            this.Log(DebugLevel.LOGS, Verbosity.LEVEL3, "Fixing Build Order of file: {0}", Path.GetFileName(script.GetFilePath()));
            int idx = IndexOfFile(script.GetKey());
            var a = _sources[idx];
            var ab = _doneState[idx];
            _doneState.RemoveAt(idx);
            _doneState.Add(ab);
            _sources.RemoveAt(idx);
            AddFile(a, true);
        }


        /// <summary>
        /// Returns true if the scripts key is contained in the manager
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public bool IsIncluded(ISourceScript script)
        {
            return _sources.Any(x => x.GetKey() == script.GetKey());
        }

        /// <summary>
        /// Adds a script to the to do list of the source manager.
        /// Will do nothing if already included
        /// </summary>
        /// <param name="script"></param>
        public void AddToTodo(ISourceScript script)
        {
            if (!IsIncluded(script))
            {
                this.Log(DebugLevel.LOGS, Verbosity.LEVEL3, "Adding Script to Todo List: {0}", Path.GetFileName(script.GetFilePath()));
                AddFile(script, false);
                _doneState.Add(ProcessStage.QUEUED);
            }
        }

        /// <summary>
        /// Sets the processing state of the script to done
        /// it will not be returned by the NextItem property.
        /// </summary>
        /// <param name="script"></param>
        public void SetState(ISourceScript script, ProcessStage stage)
        {
            if (IsIncluded(script))
            {
                _doneState[IndexOfFile(script.GetKey())] = stage;

                this.Log(DebugLevel.LOGS, Verbosity.LEVEL3, "Finished Script: {0}", Path.GetFileName(script.GetFilePath()));
            }
        }

        /// <summary>
        /// Returns the List of Scripts that are in this Source Manager object
        /// </summary>
        /// <returns></returns>
        public List<ISourceScript> GetList()
        {
            return _sources;
        }


        /// <summary>
        /// Adds a file to a list while checking for the key
        /// </summary>
        /// <param name="script"></param>
        /// <param name="checkForExistingKey"></param>
        private void AddFile(ISourceScript script, bool checkForExistingKey)
        {
            if (checkForExistingKey && ContainsFile(script.GetKey()))
            {
                return;
            }
            _sources.Add(script);

        }

        /// <summary>
        /// Returns true when the source manager contains a script with the key specified
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool ContainsFile(string key)
        {
            return IndexOfFile(key) != -1;
        }

        /// <summary>
        /// Returns the index of the file with the matching key
        /// returns -1 when the key is not present
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int IndexOfFile(string key)
        {
            for (var i = 0; i < _sources.Count; i++)
            {
                if (_sources[i].GetKey() == key)
                {
                    return i;
                }
            }

            return -1;
        }

        private bool LockScriptCreation = true;

        public void SetLock(bool state)
        {
            LockScriptCreation = state;
        }

        /// <summary>
        /// Convenience wrapper to create a source script without knowing the actual type of the script.
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="file"></param>
        /// <param name="key"></param>
        /// <param name="pluginCache"></param>
        /// <returns></returns>
        public bool TryCreateScript(out ISourceScript script, string separator, string file, string key, Dictionary<string, object> pluginCache)
        {
            if (LockScriptCreation)
            {
                script = null;
                this.Log(DebugLevel.WARNINGS, Verbosity.LEVEL1, "A Plugin is trying to add a file outside of the main stage. Is the configuration correct?");
                return false;
            }

            script = new SourceScript(separator, file, key, pluginCache);
            return true;
        }
    }
}