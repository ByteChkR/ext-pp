namespace ext_pp_base
{
    /// <summary>
    /// An object that contains the script source and other information
    /// </summary>
    public interface ISourceScript
    {
        /// <summary>
        /// Returns the full filepath of this script.
        /// </summary>
        /// <returns>the filepath of the source</returns>
        string GetFilePath();

        /// <summary>
        /// Returns the key that got assigned to the script
        /// </summary>
        /// <returns>the key of the file/source</returns>
        string GetKey();

        /// <summary>
        /// returns the source that is cached
        /// if the source was not loaded before it will load it from the file path specified
        /// </summary>
        /// <returns>the source of the file</returns>
        string[] GetSource();

        /// <summary>
        /// Sets the cached version of the source
        /// </summary>
        /// <param name="source">the updated source</param>
        void SetSource(string[] source);


        /// <summary>
        /// Adds a value to the plugin cache to be read later during the processing
        /// </summary>
        /// <param name="key">the key</param>
        /// <param name="value">the value</param>
        void AddValueToCache(string key, object value);

        /// <summary>
        /// Returns true if the plugin cache contains an item of type T with key
        /// </summary>
        /// <typeparam name="T">The type to be checked for</typeparam>
        /// <param name="key">the key that is checked.</param>
        /// <returns>false if nonexsistant or not the specified type</returns>
        bool HasValueOfType<T>(string key);

        /// <summary>
        /// Returns the value of type T with key.
        /// </summary>
        /// <typeparam name="T">The typy of the value</typeparam>
        /// <param name="key">the key of the corresponding value</param>
        /// <returns>the value casted to type t</returns>
        T GetValueFromCache<T>(string key);
    }
}