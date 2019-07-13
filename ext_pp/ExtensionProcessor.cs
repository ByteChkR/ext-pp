using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ext_pp.settings;

namespace ext_pp
{
    public static class ExtensionProcessor
    {

        #region Compile

        public static string[] CompileFile(string path, Dictionary<string, bool> globalTable = null)
        {

            var success = SourceScript.LoadSourceTree(path, out var ss, globalTable);


            var ret = CompileTree(ss);
            Logger.Log(DebugLevel.LOGS, success ? "Compilation DONE!" : "One or more errors in compilation",
                Verbosity.ALWAYS_SEND);

            return ret;
        }

        private static string[] CompileTree(List<SourceScript> tree)
        {
            var ret = new List<string>();
            

            for (var i = tree.Count - 1; i >= 0; i--)
            {
                Logger.Log(DebugLevel.LOGS, "Compiling File: " + tree[i].Filepath, Verbosity.LEVEL1);
                
                ret.AddRange(tree[i].Source);
            }

            return SourceScript.RemoveStatements(ret, Settings.CleanUpList.ToArray()).ToArray();
        }

        #endregion


    }
}