using System.Collections.Generic;
using System.IO;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp
{
    internal class SourceScript : ISourceScript
    {

        /// <summary>
        /// The full filepath of the script
        /// </summary>
        private readonly string _filepath;
        /// <summary>
        /// if the source was requested at least once, it remains cached in here
        /// </summary>
        private string[] _source = null;

        /// <summary>
        /// 
        /// </summary>
        private readonly string _separator;

        /// <summary>
        /// The key that will get assigned to this script.
        /// </summary>
        private readonly string _key;

        /// <summary>
        /// A Cache that is shared with all plugins to exchange information between different processing steps
        /// </summary>
        private readonly Dictionary<string, object> _pluginCache;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="path"></param>
        /// <param name="key"></param>
        /// <param name="pluginCache"></param>
        public SourceScript(string separator, string path, string key, Dictionary<string, object> pluginCache)
        {
            _key = key;
            _pluginCache = pluginCache;
            _filepath = path;
            _separator = separator;
        }

        /// <summary>
        /// Returns the full filepath of this script.
        /// </summary>
        /// <returns></returns>
        public string GetFilePath()
        {
            return _filepath;
        }


        /// <summary>
        /// Returns the key that got assigned to the script
        /// </summary>
        /// <returns></returns>
        public string GetKey()
        {
            return _key;
        }

        /// <summary>
        /// returns the source that is cached
        /// if the source was not loaded before it will load it from the file path specified
        /// </summary>
        /// <returns></returns>
        public string[] GetSource()
        {
            if (_source == null) Load();
            return _source;
        }

        /// <summary>
        /// Sets the cached version of the source
        /// </summary>
        /// <param name="source"></param>
        public void SetSource(string[] source)
        {
            _source = source;
        }



        /// <summary>
        /// Returns true if the plugin cache contains an item of type T with key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool HasValueOfType<T>(string key)
        {
            return _pluginCache.ContainsKey(key) && _pluginCache[key].GetType() == typeof(T);
        }

        /// <summary>
        /// Returns the value of type T with key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetValueFromCache<T>(string key)
        {
            return (T)_pluginCache[key];
        }

        /// <summary>
        /// Adds a value to the plugin cache to be read later during the processing
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddValueToCache(string key, object value)
        {
            _pluginCache.Add(key, value);
        }

        /// <summary>
        /// Loads the source code of the file into memory
        /// </summary>
        /// <returns></returns>
        public bool Load()
        {

            bool ret;
            if (!(ret = LoadSource()))
            {
                Logger.Log(DebugLevel.ERRORS, "Could not load file: " + _filepath, Verbosity.LEVEL1);

            }

            return ret;
        }

        /// <summary>
        /// returns true if the loading succeeded
        /// </summary>
        /// <returns></returns>
        private bool LoadSource()
        {

            _source = new string[0];
            if (!File.Exists(_filepath)) return false;
            _source = File.ReadAllLines(_filepath);


            return true;
        }

    }
}

