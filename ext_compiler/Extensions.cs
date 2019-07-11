using System.Collections.Generic;
using System.Linq;

namespace ext_compiler.extensions
{
    public static class Extensions
    {

        public static string Unpack(this string[] arr)
        {
            string s = "";
            for (int i = 0; i < arr.Length; i++)
            {
                s += arr[i];
                if (i < arr.Length - 1) s += ",";
            }

            return s;
        }

        

        public static void Add(this List<SourceScript> list, SourceScript script, bool CheckForExistingKey)
        {

            if (CheckForExistingKey && list.ContainsFile(script.key)) return;
            list.Add(script);

        }

        public static bool ContainsFile(this List<SourceScript> files, string key)
        {
            return files.Count(x => x.key == key) > 0;
        }

        public static int IndexOf(this List<SourceScript> files, string key)
        {
            for (int i = 0; i < files.Count; i++)
            {
                if (files[i].key == key) return i;
            }

            return -1;
        }
    }
}