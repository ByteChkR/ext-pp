using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ext_pp_base
{
    public static class Extensions
    {
        private static StringBuilder _sb = new StringBuilder();

        /// <summary>
        /// Concats the array into a string separated by the separator
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string Unpack(this IEnumerable<object> arr, string separator)
        {
            _sb.Clear();
            var enumerable = arr as object[] ?? arr.ToArray();
            for (var i = 0; i < enumerable.Count(); i++)
            {
                _sb.Append(enumerable.ElementAt(i));
                if (i < enumerable.Count() - 1) _sb.Append(separator);
            }

            return _sb.ToString();
        }

        /// <summary>
        /// Turns a string into an array split by the separator
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static IEnumerable<string> Pack(this string arr, string separator)
        {
            return arr.Split(separator);
        }


        /// <summary>
        /// Creates a sub array starting from start to start+length
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a sub array starting from 0 to length
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static IEnumerable<T> SubArray<T>(this IEnumerable<T> arr, int length)
        {
            return SubArray(arr, 0, length);
        }


        public static bool IsAllDigits(this string str)
        {
            if (string.IsNullOrEmpty(str)) return false;
            return str.All(char.IsDigit);
        }

        


    }
}