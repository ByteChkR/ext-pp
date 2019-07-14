using System.Collections.Generic;

namespace ext_pp.plugins
{
   
    public interface IPlugin
    {
        bool Process(SourceScript script, SourceManager sourceManager, Definitions defTable);

        string[] Cleanup { get; }

    }
}