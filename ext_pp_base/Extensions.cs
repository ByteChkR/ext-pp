using System;
using System.Collections.Generic;
using System.Linq;

namespace ext_pp_base
{
    public static class Extensions
    {
        /// <summary>
        /// Concats the array into a string separated by the separator
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
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
            return str.All(char.IsDigit);
        }

        private delegate bool TryParse(string val, out object value);
        private static Dictionary<Type, TryParse> _parser = new Dictionary<Type, TryParse>()
        {
            {typeof(string), CreateTryParser<string>()},
            {typeof(int), CreateTryParser<int>()},
            {typeof(float), CreateTryParser<float>()},
            {typeof(char), CreateTryParser<char>()},
            {typeof(bool), CreateTryParser<bool>()}
        };

        private static TryParse CreateTryParser<T>()
        {
            TryParse ret = null;

            if (typeof(T) == typeof(int)) ret = (string val, out object value) =>
            {
                bool r = int.TryParse(val, out int v);
                value = v;
                return r;
            };
            else if (typeof(T) == typeof(float)) ret = (string val, out object value) =>
            {
                bool r = float.TryParse(val, out float v);
                value = v;
                return r;
            };
            else if (typeof(T) == typeof(char)) ret = (string val, out object value) =>
            {
                bool r = char.TryParse(val, out char v);
                value = v;
                return r;
            };
            else if (typeof(T) == typeof(bool)) ret = (string val, out object value) =>
            {
                bool r = bool.TryParse(val, out bool v);
                if (val == "") r = v = true;
                value = v;
                return r;
            };
            else if (typeof(T) == typeof(string)) ret = (string val, out object value) =>
            {
                value = val;
                return true;
            };


            return ret;

        }

        public static T[] ParseArray<T>(string[] obj)
        {
            return ParseArray(typeof(T), obj).OfType<T>().ToArray();
        }

        public static object[] ParseArray(Type t, string[] obj)
        {
            object[] ret = new object[obj.Length];
            for (var index = 0; index < obj.Length; index++)
            {
                var s = obj[index];
                ret[index] = Parse(t, obj[index]);
            }

            return ret;
        }

        public static object Parse(Type t, string obj)
        {
            _parser[t](obj, out object val);
            return val;
        }

        public static T Parse<T>(string obj)
        {
            return (T)Parse(typeof(T), obj);
        }


    }
}