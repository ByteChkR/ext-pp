using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ext_pp_base.settings;

namespace ext_pp_base
{
    public static class Utils
    {
        /// <summary>
        /// Removes all lines of the source that start with one of the statements
        /// It takes care of possible indentations and spaces
        /// </summary>
        /// <param name="source"></param>
        /// <param name="statements"></param>
        /// <returns></returns>
        public static List<string> RemoveStatements(List<string> source, string[] statements)
        {
            for (var i = source.Count - 1; i >= 0; i--)
            {
                foreach (var t in statements)
                {

                    if (source[i].Trim().StartsWith(t))
                    {

                        Logger.Log(DebugLevel.LOGS, "Removing statement " + t + " on line " + i, Verbosity.LEVEL7);
                        source.RemoveAt(i);
                    }
                }
            }
            return source;
        }


        public static string RemoveExcessSpaces(string line, string separator)
        {
            string ret = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Unpack(separator);
            Logger.Log(DebugLevel.LOGS, "Removing Excess Spaces: " + line + " => " + ret, Verbosity.LEVEL6);
            return ret;
        }


        /// <summary>
        /// Replaces a keyword(single sequence of characters) with a replacement in the source lines supplied.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="replacement"></param>
        /// <param name="keyword"></param>
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
        /// <param name="currentPath"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool FileExistsRelativeTo(string currentPath, string file)
        {
            var p = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(currentPath);
            var ret = File.Exists(file);
            Directory.SetCurrentDirectory(p);
            return ret;
        }

        /// <summary>
        /// Returns a list of lines where the line start with statement
        /// </summary>
        /// <param name="source"></param>
        /// <param name="statement"></param>
        /// <returns></returns>
        public static string[] FindStatements(string[] source, string statement)
        {
            return source.ToList().Where(x => x.Trim().StartsWith(statement)).ToArray();
        }

        /// <summary>
        /// Splits a line by the separator and removes the first entry
        /// Gets used for include to just be able to get the path
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string[] SplitAndRemoveFirst(string statement, string separator)
        {
            if (string.IsNullOrEmpty(statement)) return new string[0];

            var ret = statement.Split(separator);

            return ret.SubArray(1, ret.Length - 1).ToArray();
        }
    }
}