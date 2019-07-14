using System.Collections.Generic;
using System.Linq;
using ext_pp.settings;

namespace ext_pp
{
    internal static class Extensions
    {
        
        public static string Unpack(this IEnumerable<string> arr, string separator)
        {
            var s = "";
            var enumerable = arr as string[] ?? arr.ToArray();
            for (var i = 0; i < enumerable.Count(); i++)
            {
                s += enumerable.ElementAt(i);
                if (i < enumerable.Count() - 1) s += separator;
            }

            return s;
        }

        public static IEnumerable<string> Pack(this string arr, string separator)
        {
            return arr.Split(separator);
        }

        public static IEnumerable<T> SubArray<T>(this IEnumerable<T> arr, int start, int length)
        {
            var ret = new T[length];
            var enumerable = arr as T[] ?? arr.ToArray();
            for (var i = start; i < start + length; i++)
            {
                ret.SetValue(enumerable.ElementAt(i), i - start);
            }

            return ret;
        }

        public static IEnumerable<T> SubArray<T>(this IEnumerable<T> arr, int length)
        {
            return SubArray(arr, 0, length);
        }
    }
}