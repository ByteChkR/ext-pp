using System.Collections.Generic;

namespace ext_pp_base
{
    public abstract class ASourceManager
    {
        public delegate bool ComputeFileNameAndKey(string[] vars, out string filePath, out string key, out Dictionary<string, object> pluginCache);

        public static ComputeFileNameAndKey KeyComputingScheme
        {
            get { return _computeScheme; }
            set
            {
                SetScheme = true;
                _computeScheme = value;
            }
        }
        protected static bool SetScheme = false;
        private static ComputeFileNameAndKey _computeScheme = null;

        public abstract void FixOrder(ASourceScript script);
        public abstract bool IsIncluded(ASourceScript script);
        public abstract void AddToTodo(ASourceScript script);
        public abstract int IndexOfFile(string key);

        public abstract ASourceScript CreateScript(string separator, string file, string key,
            Dictionary<string, object> pluginCache);
    }
}