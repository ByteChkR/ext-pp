﻿using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using ext_pp_base.settings;

namespace ext_pp_base
{

    public class ImportResult
    {
        private readonly Dictionary<string, object> _data = new Dictionary<string, object>();
        private bool _result;
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

        public object GetValue(string key)
        {
            return _data[key];
        }

        public string GetString(string key)
        {
            return (string)_data[key];
        }

        public void SetResult(bool result)
        {
            _result = result;
        }

        public bool ContainsKey(string key)
        {
            return _data.ContainsKey(key);
        }

        public void RemoveEntry(string key)
        {
            if (_data.ContainsKey(key))
            {
                _data.Remove(key);
            }
        }

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
        bool TryCreateScript(out ISourceScript script, string separator, string file, string key,
            ImportResult importInfo);
    }
}