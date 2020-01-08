using CommandRunner;

namespace ext_pp_cli_core
{
    public class ModuleInfo : AbstractCmdModuleInfo
    {
        public override string[] Dependencies => new[]
        {
            "ADL.dll",
            "ADL.Crash.dll",
            "ext_pp_base.dll",
            "ext_pp.dll"
        };
        public override string ModuleName => "extpp";
        public override void RunArgs(string[] args)
        {
            CLI.Main(args);
        }
    }
}