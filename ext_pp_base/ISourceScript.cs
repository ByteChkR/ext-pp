namespace ext_pp_base
{
    public interface ISourceScript
    {
        /// <summary>
        /// Returns the full filepath of this script.
        /// </summary>
        /// <returns></returns>
        string GetFilePath();

        /// <summary>
        /// Returns the key that got assigned to the script
        /// </summary>
        /// <returns></returns>
        string GetKey();

        /// <summary>
        /// returns the source that is cached
        /// if the source was not loaded before it will load it from the file path specified
        /// </summary>
        /// <returns></returns>
        string[] GetSource();

        /// <summary>
        /// Sets the cached version of the source
        /// </summary>
        /// <param name="source"></param>
        void SetSource(string[] source);

 
        /// <summary>
        /// Adds a value to the plugin cache to be read later during the processing
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void AddValueToCache(string key, object value);

        /// <summary>
        /// Returns true if the plugin cache contains an item of type T with key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        bool HasValueOfType<T>(string key);

        /// <summary>
        /// Returns the value of type T with key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetValueFromCache<T>(string key);
    }
}