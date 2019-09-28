using System.Collections.Generic;
using System.IO;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp
{
    internal class SourceScript : ISourceScript, ILoggable
    {

        /// <summary>
        /// The full filepath of the script
        /// </summary>
        private readonly IFileContent _filepath;
        /// <summary>
        /// if the source was requested at least once, it remains cached in here
        /// </summary>
        private string[] _source;

        /// <summary>
        /// Flag to check if the file was already loaded into memory
        /// </summary>
        public bool IsSourceLoaded => _source != null;


        /// <summary>
        /// The key that will get assigned to this script.
        /// </summary>
        private string Key => _filepath.GetKey();

        /// <summary>
        /// A Cache that is shared with all plugins to exchange information between different processing steps
        /// </summary>
        private readonly ImportResult _importInfo;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="separator">the separator used.</param>
        /// <param name="path">the path to the file</param>
        /// <param name="key">the key of the source file</param>
        /// <param name="pluginCache">the plugin cache that is used.</param>
        public SourceScript(string separator, IFileContent path, ImportResult importInfo)
        {
            _importInfo = importInfo;
            _filepath = path;
        }

        /// <summary>
        /// Returns the full filepath of this script.
        /// </summary>
        /// <returns>the filepath of the source</returns>
        public IFileContent GetFileInterface()
        {
            return _filepath;
        }


        /// <summary>
        /// Returns the key that got assigned to the script
        /// </summary>
        /// <returns>the key of the file/source</returns>
        public string GetKey()
        {
            return Key;
        }

        /// <summary>
        /// returns the source that is cached
        /// if the source was not loaded before it will load it from the file path specified
        /// </summary>
        /// <returns>the source of the file</returns>
        public string[] GetSource()
        {
            if (_source == null)
            {
                Load();
            }
            return _source;
        }

        /// <summary>
        /// Sets the cached version of the source
        /// </summary>
        /// <param name="source">the updated source</param>
        public void SetSource(string[] source)
        {
            _source = source;
        }



        /// <summary>
        /// Returns true if the plugin cache contains an item of type T with key
        /// </summary>
        /// <typeparam name="T">The type to be checked for</typeparam>
        /// <param name="key">the key that is checked.</param>
        /// <returns>false if nonexsistant or not the specified type</returns>
        public bool HasValueOfType<T>(string key)
        {
            return _importInfo.ContainsKey(key) && _importInfo.GetValue(key).GetType() == typeof(T);
        }

        /// <summary>
        /// Returns the value of type T with key.
        /// </summary>
        /// <typeparam name="T">The typy of the value</typeparam>
        /// <param name="key">the key of the corresponding value</param>
        /// <returns>the value casted to type t</returns>
        public T GetValueFromCache<T>(string key)
        {
            return (T)_importInfo.GetValue(key);
        }

        /// <summary>
        /// Adds a value to the plugin cache to be read later during the processing
        /// </summary>
        /// <param name="key">the key</param>
        /// <param name="value">the value</param>
        public void AddValueToCache(string key, object value)
        {
            _importInfo.SetValue(key, value);
        }

        /// <summary>
        /// Loads the source code of the file into memory
        /// </summary>
        /// <returns>the success state of the operation</returns>
        public bool Load()
        {

            bool ret = LoadSource();
            if (!ret)
            {
                this.Error("Could not load file: {0}", _filepath);

            }

            return ret;
        }

        /// <summary>
        /// returns true if the loading succeeded
        /// </summary>
        /// <returns>the success state of the operation</returns>
        private bool LoadSource()
        {
            string dir = Directory.GetCurrentDirectory();
            bool ret = _filepath.TryGetLines(out _source);
            Directory.SetCurrentDirectory(dir);
            return ret;
        }

    }
}

