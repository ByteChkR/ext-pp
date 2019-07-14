using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ext_pp.settings;

namespace ext_pp.plugins
{
    public class IncludePlugin : IPlugin
    {
        private readonly string _includeKeyword;
        private readonly string _separator;
        private readonly Settings _settings;
        public IncludePlugin(Settings settings)
        {
            _settings = settings;
            _includeKeyword = settings.IncludeStatement;
            _separator = settings.Separator.ToString();
        }

        public bool Process(SourceScript script, SourceManager sourceManager, Definitions defs)
        {
            string[] incs = Utils.FindStatements(script.Source, _includeKeyword);
            

            foreach (var includes in incs)
            {

                bool tmp= GetIncludeSourcePath(includes, Path.GetDirectoryName(Path.GetFullPath(script.Filepath)), out var path, out var genParams);
                if (tmp)
                {
                    
                    SourceScript ss=new SourceScript(_settings, path, genParams);
                    if (!sourceManager.IsIncluded(ss))
                    {
                        sourceManager.AddToTodo(ss);
                    }
                    else
                    {
                        sourceManager.FixOrder(ss);
                        Logger.Log(DebugLevel.LOGS, "Fixing Source Order", Verbosity.LEVEL3);
                    }

                }
                else return false;

            }

            return true;

        }

        private bool GetIncludeSourcePath(string statement, string currentPath, out string filePath, out string[] genParams)
        {
            var vars = statement.GetStatementValues(_separator);
            genParams = new string[0];
            filePath = "";
            if (vars.Length != 0)
            {
                filePath = vars[0];
                genParams = vars.Length > 1 ?
                    vars.SubArray(1, vars.Length - 1).ToArray() : genParams;

                if (!Utils.FileExists(currentPath, filePath))
                {
                    Logger.Crash(new Exception("Could not find include file " + filePath), false);
                    return false;
                }
                else
                {
                    filePath = Path.GetFullPath(filePath);
                    return true;
                }

            }


            Logger.Crash(new Exception("Empty Include Statement Found"), false);
            return false;
        }
    }
}