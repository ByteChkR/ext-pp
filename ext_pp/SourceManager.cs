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
        /// <param name="scheme">The delegate that will be used to determine the key and path in the source manager</param>
        public void SetComputingScheme(DelKeyComputingScheme scheme)
        {
            if (scheme == null)
            {
                return;
            }
            _computeScheme = scheme;
            this.Log(DebugLevel.LOGS, Verbosity.LEVEL2, "Changed Computing Scheme to: {0}", scheme.Method.Name);
        }

        /// <summary>
        /// Returns the Queued items that are waiting for computation
        /// </summary>
        /// <returns>Size of the internal queue</returns>
        public int GetTodoCount()
        {
            return _doneState.Count(x => x == ProcessStage.QUEUED);
        }

        /// <summary>
        /// Returns the computing scheme
        /// </summary>
        /// <returns>the computing scheme</returns>
        public DelKeyComputingScheme GetComputingScheme()
        {
            return _computeScheme;
        }

        /// <summary>
        /// The default implementation of the key matching calculation
        /// </summary>
        /// <param name="vars">The import string in a source script</param>
        /// <param name="currentPath">the current path of the preprocessor</param>
        /// <returns>A result object.</returns>
        private static ImportResult ComputeFileNameAndKey_Default(string[] vars, string currentPath)
        {
            ImportResult ret = new ImportResult();

            if (vars.Length == 0)
            {
                return ret;
            }
            
            string dir = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(currentPath);

            string key = Path.GetFullPath(vars[0]);
            ret.SetValue("filename", key);

            Directory.SetCurrentDirectory(dir);

            ret.SetValue("key", key);
            ret.SetResult(true);

            return ret;
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
        /// <param name="script">The script that got referenced.</param>
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
        /// <param name="script">The script to check for</param>
        /// <returns>True if the script is included.</returns>
        public bool IsIncluded(ISourceScript script)
        {
            return _sources.Any(x => x.GetKey() == script.GetKey());
        }

        /// <summary>
        /// Adds a script to the to do list of the source manager.
        /// Will do nothing if already included
        /// </summary>
        /// <param name="script">The script to enqueue for computation</param>
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
        /// <param name="script">The script to set the stage for</param>
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
        /// <returns>The internal list of all scripts.</returns>
        public List<ISourceScript> GetList()
        {
            return _sources;
        }


        /// <summary>
        /// Adds a file to a list while checking for the key
        /// </summary>
        /// <param name="script">The file to be added.</param>
        /// <param name="checkForExistingKey">A flag to optionally check if the key of the file is already existing</param>
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
        /// <param name="key">the key to search for</param>
        /// <returns>true if the file is contained in the source manager</returns>
        private bool ContainsFile(string key)
        {
            return IndexOfFile(key) != -1;
        }

        /// <summary>
        /// Returns the index of the file with the matching key
        /// returns -1 when the key is not present
        /// </summary>
        /// <param name="key">the key to search for</param>
        /// <returns>the index of the file or -1 if not found</returns>
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
        /// <param name="separator">the separator used.</param>
        /// <param name="file">the path of the file</param>
        /// <param name="key">the key of the file</param>
        /// <param name="importInfo">the import info of the key and path importation</param>
        /// <returns>the success state of the operation</returns>
        public bool TryCreateScript(out ISourceScript script, string separator, string file, string key, ImportResult importInfo)
        {
            if (LockScriptCreation)
            {
                script = null;
                this.Warning("A Plugin is trying to add a file outside of the main stage. Is the configuration correct?");
                return false;
            }

            script = new SourceScript(separator, file, key, importInfo);
            return true;
        }
    }
}