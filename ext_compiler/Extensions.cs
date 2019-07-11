using System.Collections.Generic;
using System.Linq;

namespace ext_compiler.extensions
{
    public static class Extensions
    {
        public static void Add(this List<SourceScript> list, SourceScript script, bool CheckForExistingKey)
        {

            if (CheckForExistingKey && list.ContainsFile(script.filepath)) return;
            list.Add(script);

        }

        public static bool ContainsFile(this List<SourceScript> files, string filePath)
        {
            return files.Count(x => x.filepath == filePath) > 0;
        }

        public static int IndexOf(this List<SourceScript> files, string filePath)
        {
            for (int i = 0; i < files.Count; i++)
            {
                if (files[i].filepath == filePath) return i;
            }

            return -1;
        }
    }
}