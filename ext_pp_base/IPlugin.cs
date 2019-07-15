namespace ext_pp_base
{
   
    public interface IPlugin
    {
        bool Process(ASourceScript script, ASourceManager sourceManager, ADefinitions defTable);

        string[] Cleanup { get; }

    }
}