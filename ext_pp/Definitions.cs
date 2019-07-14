using System.Collections.Generic;
using ext_pp.settings;

namespace ext_pp
{
    public class Definitions
    {
        private Dictionary<string, bool> _definitions;

        public Definitions() : this(new Dictionary<string, bool>()) { }
        public Definitions(Dictionary<string, bool> definitions)
        {
            _definitions = definitions;
        }

        public void Set(string[] keys)
        {
            foreach (var key in keys)
            {
                Set(key);
            }
        }

        public void Unset(string[] keys)
        {
            foreach (var key in keys)
            {
                Unset(key);
            }
        }

        public void Set(string key)
        {
            Change(key, true);
        }

        public void Unset(string key)
        {
            Change(key, false);
        }

        private void Change(string key, bool state)
        {

            Logger.Log(DebugLevel.LOGS, "Setting Key: " + key + " to value: " + state, Verbosity.LEVEL5);
            if (_definitions.ContainsKey(key)) _definitions[key] = state;
            else _definitions.Add(key, state);
        }

        public bool Check(string key)
        {
            return _definitions.ContainsKey(key) && _definitions[key];
        }

    }
}