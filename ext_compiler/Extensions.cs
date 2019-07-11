using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ext_compiler.extensions
{
    public static class Extensions
    {

        public static string Unpack(this string[] arr, char seperator=',')
        {
            string s = "";
            for (int i = 0; i < arr.Length; i++)
            {
                s += arr[i];
                if (i < arr.Length - 1) s += seperator;
            }

            return s;
        }


        public static T[] SubArray<T>(this T[] arr, int start, int length)
        {
            T[] ret = new T[length];
            for (int i = start; i < start+length; i++)
            {
                ret[i - start] = arr[i];
            }

            return ret;
        }

        public static T[] SubArray<T>(this T[] arr, int length)
        {
            return SubArray(arr, 0, length);
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