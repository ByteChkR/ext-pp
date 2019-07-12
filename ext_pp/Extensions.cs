using System.Collections.Generic;
using System.Linq;
using ext_pp.settings;

namespace ext_pp
{
    internal static class Extensions
    {
        
        public static string Unpack(this IEnumerable<string> arr)
        {
            string s = "";
            for (int i = 0; i < arr.Count(); i++)
            {
                s += arr.ElementAt(i);
                if (i < arr.Count() - 1) s += Settings.Separator;
            }

            return s;
        }

        public static IEnumerable<string> Pack(this string arr)
        {
            return arr.Split(Settings.Separator);
        }

        public static string[] GetStatementValues(this string statement)
        {
            if (string.IsNullOrEmpty(statement)) return new string[0];

            string[] ret = statement.Split(Settings.Separator);

            return ret.SubArray(1, ret.Length - 1).ToArray();
        }


        public static IEnumerable<T> SubArray<T>(this IEnumerable<T> arr, int start, int length)
        {
            T[] ret = new T[length];
            for (int i = start; i < start + length; i++)
            {
                ret.SetValue(arr.ElementAt(i), i - start);
            }

            return ret;
        }

        public static IEnumerable<T> SubArray<T>(this IEnumerable<T> arr, int length)
        {
            return SubArray(arr, 0, length);
        }

        public static void AddFile(this List<SourceScript> list, SourceScript script, bool CheckForExistingKey)
        {
            if (CheckForExistingKey && list.ContainsFile(script.Key)) return;
            list.Add(script);

        }

        public static bool ContainsFile(this List<SourceScript> files, string key)
        {
            return IndexOfFile(files, key) != -1;
        }

        public static int IndexOfFile(this List<SourceScript> files, string key)
        {
            for (int i = 0; i < files.Count; i++)
            {
                if (files[i].Key == key) return i;
            }

            return -1;
        }
    }
}