using System.Collections.Generic;
using ext_pp_base;
using ext_pp_base.settings;

namespace ext_pp
{
    /// <summary>
    /// Contains the Values on what is Defined as a Variable when Processing the text
    /// </summary>
    public class Definitions : IDefinitions
    {
        /// <summary>
        /// Dictionary to keep track of what is defined and what is not
        /// </summary>
        private readonly Dictionary<string, bool> _definitions;

        /// <summary>
        /// Convenience Wrapper
        /// </summary>
        public Definitions() : this(new Dictionary<string, bool>()) { }


        /// <summary>
        /// Creates a Definitions Object with predefined definitions
        /// </summary>
        /// <param name="definitions">the predefined definitions</param>
        public Definitions(Dictionary<string, bool> definitions)
        {
            _definitions = definitions;
        }

        /// <summary>
        /// Set an array of definitions to true
        /// </summary>
        /// <param name="keys"></param>
        public void Set(string[] keys)
        {
            foreach (var key in keys)
            {
                Set(key);
            }
        }

        /// <summary>
        /// Set an array of definitions to false
        /// </summary>
        /// <param name="keys"></param>
        public void Unset(string[] keys)
        {
            foreach (var key in keys)
            {
                Unset(key);
            }
        }

        /// <summary>
        /// Set a specific definition to true
        /// </summary>
        /// <param name="key"></param>
        public void Set(string key)
        {
            Change(key, true);
        }


        /// <summary>
        /// Set a specific definition to false
        /// </summary>
        /// <param name="key"></param>
        public void Unset(string key)
        {
            Change(key, false);
        }


        /// <summary>
        /// Change the definition state.
        /// </summary>
        /// <param name="key">definition name</param>
        /// <param name="state"></param>
        private void Change(string key, bool state)
        {

            Logger.Log(DebugLevel.LOGS, "Setting Key: " + key + " to value: " + state, Verbosity.LEVEL5);
            if (_definitions.ContainsKey(key)) _definitions[key] = state;
            else _definitions.Add(key, state);
        }

        /// <summary>
        /// Returns true if the definition is "set" and returns false if the definition is "unset"
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Check(string key)
        {
            return _definitions.ContainsKey(key) && _definitions[key];
        }

    }
}