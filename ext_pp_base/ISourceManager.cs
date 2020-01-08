using System.Collections.Generic;
using System.Reflection;
using ext_pp_base.settings;

namespace ext_pp_base
{
    /// <summary>
    /// A data object that is used to hold information about the sourcescript file and key
    /// </summary>
    public class ImportResult
    {
        /// <summary>
        /// The underlying data structure that is used to hold the custom data
        /// </summary>
        private readonly Dictionary<string, object> _data = new Dictionary<string, object>();
        /// <summary>
        /// A flag indicating the success state of the Import Operation
        /// </summary>
        private bool _result;
        /// <summary>
        /// Sets a key value pair.
        /// </summary>
        /// <param name="key">The key to be set</param>
        /// <param name="value">the value that will be set</param>
        public void SetValue(string key, object value)
        {
            if (_data.ContainsKey(key))
            {
                _data[key] = value;

            }
            else
            {
                _data.Add(key, value);
            }
        }

        /// <summary>
        /// Returns a value from the custom data
        /// </summary>
        /// <param name="key">the key of the value</param>
        /// <returns>the object</returns>
        public object GetValue(string key)
        {
            return _data[key];
        }
        /// <summary>
        /// Returns a value from the custom data
        /// </summary>
        /// <param name="key">the key of the value</param>
        /// <returns>the object cast to string</returns>
        public string GetString(string key)
        {
            return (string)_data[key];
        }

        /// <summary>
        /// Sets the result of the operation
        /// </summary>
        /// <param name="result">the result</param>
        public void SetResult(bool result)
        {
            _result = result;
        }

        /// <summary>
        /// Checks the Custom data for a specific key
        /// </summary>
        /// <param name="key">The key to be checked</param>
        /// <returns>True if the key is contained in the data</returns>
        public bool ContainsKey(string key)
        {
            return _data.ContainsKey(key);
        }

        /// <summary>
        /// Removes an entry from the data
        /// </summary>
        /// <param name="key">the key to be removed</param>
        public void RemoveEntry(string key)
        {
            if (_data.ContainsKey(key))
            {
                _data.Remove(key);
            }
        }

        /// <summary>
        /// Implicit operator to convert to bool
        /// </summary>
        /// <param name="obj">The object to be converted</param>
        public static implicit operator bool(ImportResult obj)
        {
            return obj._result;
        }
    }


    /// <summary>
    /// Defines the scheme on how the keys for identifying scripts get created.
    /// </summary>
    /// <param name="var"></param>
    /// <param name="currentPath"></param>
    /// <param name="filePath"></param>
    /// <param name="key"></param>
    /// <param name="pluginCache"></param>
    /// <returns></returns>
    public delegate ImportResult DelKeyComputingScheme(string[] var, string currentPath);

    /// <summary>
    /// Interface that contains all methods for loading and managing source code.
    /// </summary>
    public interface ISourceManager
    {
        /// <summary>
        /// Sets the computing scheme to a custom scheme that will then be used to assign keys to scripts
        /// </summary>
        /// <param name="scheme"></param>
        void SetComputingScheme(DelKeyComputingScheme scheme);

        /// <summary>
        /// Returns the computing scheme
        /// </summary>
        /// <returns></returns>
        DelKeyComputingScheme GetComputingScheme();

        /// <summary>
        /// Fixes the order of the file tree if a script was being loaded and is now referenced (again)
        /// by removing it from the lower position and readding it at the top
        /// </summary>
        /// <param name="script"></param>
        void FixOrder(ISourceScript script);

        /// <summary>
        /// Returns true if the scripts key is contained in the manager
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        bool IsIncluded(ISourceScript script);

        /// <summary>
        /// Adds a script to the to do list of the source manager.
        /// Will do nothing if already included
        /// </summary>
        /// <param name="script"></param>
        void AddToTodo(ISourceScript script);

        /// <summary>
        /// Returns the index of the file with the matching key
        /// returns -1 when the key is not present
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        int IndexOfFile(string key);

        /// <summary>
        /// Convenience wrapper to create a source script without knowing the actual type of the script.
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="file"></param>
        /// <param name="key"></param>
        /// <param name="pluginCache"></param>
        /// <returns></returns>
        bool TryCreateScript(out ISourceScript script, string separator, IFileContent file,
            ImportResult importInfo);
    }
}