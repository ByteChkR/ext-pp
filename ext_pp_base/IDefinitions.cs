namespace ext_pp_base
{
    /// <summary>
    /// Contains the Values on what is Defined as a Variable when Processing the text
    /// </summary>
    public interface IDefinitions
    {
        /// <summary>
        /// Set an array of definitions to true
        /// </summary>
        /// <param name="keys">The keys that will be set</param>
        void Set(string[] keys);
        /// <summary>
        /// Set an array of definitions to false
        /// </summary>
        /// <param name="keys">The keys that will be unset</param>
        void Unset(string[] keys);
        /// <summary>
        /// Set a specific definition to true
        /// </summary>
        /// <param name="key">The key that will be set</param>
        void Set(string key);
        /// <summary>
        /// Set a specific definition to false
        /// </summary>
        /// <param name="key">The key that will be unset</param>
        void Unset(string key);
        /// <summary>
        /// Returns true if the definition is "set" and returns false if the definition is "unset"
        /// </summary>
        /// <param name="key">The key to be checked</param>
        /// <returns>returns true when the key is set.</returns>
        bool Check(string key);
    }
}