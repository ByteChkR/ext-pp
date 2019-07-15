using System.Collections.Generic;
using System.IO;
using ext_pp_base;

namespace ext_pp
{
    internal class SourceScript : ASourceScript
    {

        public readonly string Filepath;
        private string[] _source = null;
        private readonly string _separator;


        private readonly string _key;


        private readonly Dictionary<string, object> _pluginCache;


        public SourceScript(string separator, string path, string key, Dictionary<string, object> pluginCache)
        {
            _key = key;
            _pluginCache = pluginCache;
            Filepath = path;
            _separator = separator;
        }

        public override string GetFilePath()
        {
            return Filepath;
        }

        public override string GetKey()
        {
            return _key;
        }

        public override string[] GetSource()
        {
            if (_source == null) Load();
            return _source;
        }

        public override void SetSource(string[] source)
        {
            _source = source;
        }


        public override bool HasValueOfType<T>(string key)
        {
            return _pluginCache.ContainsKey(key) && _pluginCache[key].GetType() == typeof(T);
        }

        public override T GetValueFromCache<T>(string key)
        {
            return (T)_pluginCache[key];
        }

        public override void AddValueToCache(string key, object value)
        {
            _pluginCache.Add(key, value);
        }

        public override bool Load()
        {

            bool ret;
            if (!(ret = LoadSource()))
            {
                //DDEBUG LOG HERE

            }

            return ret;
        }

        private bool LoadSource()
        {

            _source = new string[0];
            if (!File.Exists(Filepath)) return false;
            _source = File.ReadAllLines(Filepath);


            return true;
        }

    }
}

