using System.Collections.Generic;
using System.IO;
using System.Linq;
using ext_pp.settings;

namespace ext_pp
{
    public static class Utils
    {

        public static List<string> RemoveStatements(List<string> source, string[] statements)
        {

            Logger.Log(DebugLevel.LOGS, "Removing Leftover Statements", Verbosity.LEVEL2);
            for (var i = source.Count - 1; i >= 0; i--)
            {
                foreach (var t in statements)
                {
                    if (source[i].Trim().StartsWith(t)) source.RemoveAt(i);
                }
            }
            return source;
        }

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

        public static bool FileExists(string currentPath, string file)
        {
            var p = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(Path.GetDirectoryName(currentPath));
            var ret = File.Exists(file);


            Directory.SetCurrentDirectory(p);
            return ret;
        }

        public static string[] FindStatements(string[] source, string statement)
        {
            return source.ToList().Where(x => x.Trim().StartsWith(statement)).ToArray();
        }
    }
}