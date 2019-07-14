using System.Collections.Generic;

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
            change(key, true);
        }

        public void Unset(string key)
        {
            change(key, false);
        }

        private void change(string key, bool state)
        {
            if (_definitions.ContainsKey(key)) _definitions[key] = state;
            else _definitions.Add(key, state);
        }

        public bool Check(string key)
        {
            return _definitions.ContainsKey(key) && _definitions[key];
        }

    }
}