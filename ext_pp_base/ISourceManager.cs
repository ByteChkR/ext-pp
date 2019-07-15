using System.Collections.Generic;
using System.Reflection;
using ext_pp_base.settings;

namespace ext_pp_base
{
    public delegate bool DelKeyComputingScheme(string[] vars, out string filePath, out string key, out Dictionary<string, object> pluginCache);

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
        ISourceScript CreateScript(string separator, string file, string key,
            Dictionary<string, object> pluginCache);
    }
}