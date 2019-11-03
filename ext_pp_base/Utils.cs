using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using ext_pp_base.settings;

namespace ext_pp_base
{
    /// <summary>
    /// A utility class that contains various string operations
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Removes all lines of the source that start with one of the statements
        /// It takes care of possible indentations and spaces
        /// </summary>
        /// <param name="source">the input source</param>
        /// <param name="statements">statements that need to be removed from the source</param>
        /// <returns>the cleaned list of source code lines</returns>
        public static List<string> RemoveStatements(List<string> source, string[] statements, ILoggable logobj)
        {
            for (var i = source.Count - 1; i >= 0; i--)
            {
                foreach (var t in statements)
                {

                    if (source[i].Trim().StartsWith(t))
                    {

                        Logger.Log(logobj, DebugLevel.LOGS, Verbosity.LEVEL7, "Removing statement {0} on line {1}", t, i);
                        source.RemoveAt(i);
                        break;
                    }
                }
            }
            return source;
        }

        /// <summary>
        /// Removes all the excess spaces around the specified separator
        /// </summary>
        /// <param name="line">The line to operate on</param>
        /// <param name="separator">the separator to be used</param>
        /// <param name="logobj">The object from where all resulting logs come from</param>
        /// <returns>the fixed line without any excess spaces</returns>
        public static string RemoveExcessSpaces(string line, string separator, ILoggable logobj)
        {
            string ret = line.Split(separator, StringSplitOptions.RemoveEmptyEntries).Unpack(separator);
            Logger.Log(logobj, DebugLevel.LOGS, Verbosity.LEVEL7, "Removing Excess Spaces: {0} => {1}", line, ret);
            return ret;
        }


        /// <summary>
        /// Replaces a keyword(single sequence of characters) with a replacement in the source lines supplied.
        /// </summary>
        /// <param name="source">the source to operate on</param>
        /// <param name="replacement">the replacement sequence</param>
        /// <param name="keyword">the keyword that will be pasted for the replacement</param>
        public static void ReplaceKeyWord(string[] source, string replacement, string keyword)
        {

            for (var i = 0; i < source.Length; i++)
            {
                if (source[i].Contains(keyword))
                {
                    source[i] = source[i].Replace(keyword, replacement);
                }
            }
        }

        /// <summary>
        /// Returns true if the path is valid relative to the current path(the current script that is processed
        /// </summary>
        /// <param name="currentPath">the current path of the program</param>
        /// <param name="file">the relative file path</param>
        /// <returns>true if the relative path is pointing towards a valid file.</returns>
        public static bool FileExistsRelativeTo(string currentPath, string file)
        {
            bool ret1 = IOCallbacks.Callback.FileExists(Path.Combine(currentPath, file));
            //var p = Directory.GetCurrentDirectory();
            //Directory.SetCurrentDirectory(currentPath);
            //var ret = File.Exists(file);
            //Directory.SetCurrentDirectory(p);
            return ret1;
        }

        /// <summary>
        /// Returns true if the path is valid relative to the current path(the current script that is processed
        /// </summary>
        /// <param name="currentPath">the current path of the program</param>
        /// <param name="file">the relative file path</param>
        /// <returns>true if the relative path is pointing towards a valid file.</returns>
        public static bool FileExistsRelativeTo(string currentPath, IFileContent file)
        {
            if (file.HasValidFilepath) return FileExistsRelativeTo(currentPath, file.GetFilePath());
            return true;
        }



        /// <summary>
        /// Returns a list of lines where the line start with statement
        /// </summary>
        /// <param name="source">the source to operate on</param>
        /// <param name="statement">the statement to be checked for</param>
        /// <returns>A list of all lines that start with the specified statements</returns>
        public static string[] FindStatements(string[] source, string statement)
        {
            return source.ToList().Where(x => IsStatement(x, statement)).ToArray();
        }

        /// <summary>
        /// Returns true if the source starts with the statement
        /// </summary>
        /// <param name="line">the line to operate on</param>
        /// <param name="statement">the statement to search for</param>
        /// <returns></returns>
        public static bool IsStatement(string line, string statement)
        {
            return line.Trim().StartsWith(statement);
        }

        /// <summary>
        /// Splits a line by the separator and removes the first entry
        /// Gets used for include to just be able to get the path
        /// </summary>
        /// <param name="statement">The statement to operate on</param>
        /// <param name="separator">the separator used to split</param>
        /// <returns>the array without the first entry</returns>
        public static string[] SplitAndRemoveFirst(string statement, string separator)
        {
            if (String.IsNullOrEmpty(statement))
            {
                return new string[0];
            }

            var ret = statement.Split(separator);

            return ret.SubArray(1, ret.Length - 1).ToArray();
        }

        /// <summary>
        /// A Delegate used to create different parsers
        /// </summary>
        /// <param name="val">the string input</param>
        /// <param name="value">the output of the parser</param>
        /// <returns>success state of the operation</returns>
        private delegate bool TryParse(string val, out object value);
        /// <summary>
        /// The List of implemented parsers.
        /// </summary>
        private static readonly Dictionary<Type, TryParse> _parser = new Dictionary<Type, TryParse>
        {
            {typeof(string), CreateTryParser<string>()},
            {typeof(int), CreateTryParser<int>()},
            {typeof(float), CreateTryParser<float>()},
            {typeof(char), CreateTryParser<char>()},
            {typeof(bool), CreateTryParser<bool>()},
        };

        /// <summary>
        /// Creates parser from type.
        /// </summary>
        /// <typeparam name="T">Resulting type of the parser</typeparam>
        /// <returns>The parser for type T</returns>
        private static TryParse CreateTryParser<T>()
        {
            TryParse ret = null;

            if (typeof(T) == typeof(int))
            {
                ret = (string val, out object value) =>
                {
                    bool r = Int32.TryParse(val, out int v);
                    value = v;
                    return r;
                };
            }
            else if (typeof(T) == typeof(float))
            {
                ret = (string val, out object value) =>
                {
                    bool r = Single.TryParse(val, out float v);
                    value = v;
                    return r;
                };
            }
            else if (typeof(T) == typeof(char))
            {
                ret = (string val, out object value) =>
                {
                    bool r = Char.TryParse(val, out char v);
                    value = v;
                    return r;
                };
            }
            else if (typeof(T) == typeof(bool))
            {
                ret = (string val, out object value) =>
                {
                    bool r = Boolean.TryParse(val, out bool v);
                    if (String.IsNullOrEmpty(val))
                    {
                        r = true;
                        v = true;
                    }

                    value = v;
                    return r;
                };
            }
            else if (typeof(T) == typeof(string))
            {
                ret = (string val, out object value) =>
                {
                    value = val;
                    return true;
                };
            }


            return ret;

        }

        /// <summary>
        /// generic version of parsing an array of string to array of T
        /// </summary>
        /// <typeparam name="T">Target array type</typeparam>
        /// <param name="obj">the array of strings to cast</param>
        /// <param name="defaul">default value</param>
        /// <returns>The obj array cast to T</returns>
        public static T[] ParseArray<T>(string[] obj, object defaul)
        {
            return ParseArray(typeof(T), obj, defaul).OfType<T>().ToArray();
        }


        /// <summary>
        /// non generic version of parsing an array of string to array of T
        /// </summary>
        /// <param name="t">The target type</param>
        /// <param name="obj">the array of strings to cast</param>
        /// <param name="defaul">default value</param>
        /// <returns>The obj array cast to T</returns>
        public static object[] ParseArray(Type t, string[] obj, object defaul)
        {
            if (obj == null)
            {
                return null;
            }
            object[] ret = new object[obj.Length];
            for (var index = 0; index < obj.Length; index++)
            {

                if (t.IsEnum)
                {
                    ret[index] = ParseEnum(t, obj[index], defaul);
                }
                else
                {
                    ret[index] = Parse(t, obj[index], defaul);
                }
            }

            return ret;
        }

        /// <summary>
        /// parsing a string to object of type T
        /// </summary>
        /// <param name="t">The target type</param>
        /// <param name="obj">the string to cast</param>
        /// <param name="defaul">default value</param>
        /// <returns>The obj cast to T</returns>
        public static object Parse(Type t, string obj, object defaul)
        {
            if (t.IsEnum)
            {
                return ParseEnum(t, obj, defaul);
            }
            _parser[t](obj, out object val);
            return val ?? defaul;
        }

        /// <summary>
        /// generic version of parsing a string to object of type T
        /// </summary>
        /// <typeparam name="T">Target array type</typeparam>
        /// <param name="obj">the array of strings to cast</param>
        /// <param name="defaul">default value</param>
        /// <returns>The obj array cast to T</returns>
        public static T Parse<T>(string obj, object defaul)
        {
            return (T)Parse(typeof(T), obj, defaul);
        }

        /// <summary>
        /// Parses an input string to an enum.
        /// Supports simple AND/OR operations and can be specified as digit and by name
        /// </summary>
        /// <param name="enu">The target type</param>
        /// <param name="input">the string to cast</param>
        /// <param name="defaul">default value</param>
        /// <returns>The input cast to T</returns>
        private static object ParseEnum(Type enu, string input, object defaul)
        {
            if (input.IsAllDigits())
            {
                return Int32.Parse(input);
            }

            if (!enu.IsEnum)
            {
                return defaul;
            }

            int ret = -1;

            string[] ands = input.Split('&', StringSplitOptions.RemoveEmptyEntries);
            foreach (var and in ands)
            {
                string[] ors = and.Split("|", StringSplitOptions.RemoveEmptyEntries);
                int r = -1;
                foreach (var or in ors)
                {
                    string enumStr = or.Trim();
                    if (r == -1)
                    {
                        r = (int)Enum.Parse(enu, enumStr);
                    }
                    else
                    {
                        r |= (int)Enum.Parse(enu, enumStr);
                    }
                }

                if (ret == -1)
                {
                    ret = r;
                }
                else
                {
                    ret &= r;
                }
            }

            return ret;
        }

        public static bool TryResolvePathIncludeParameter(string[] vars)
        {
            if (vars.Length == 0)
            {
                return false;
            }

            if (vars[0].StartsWith('\"') && vars[0].EndsWith('\"'))
            {
                vars[0] = vars[0].Substring(1, vars[0].Length - 2);
            }

            return true;
        }
    }
}