using ext_pp_base.settings;

namespace ext_pp_base
{
    public abstract class AbstractLinePlugin : AbstractPlugin
    {
        

        public override string OnLoad_LineStage(string source)
        {
            return LineStage(source);
        }

        public override string OnMain_LineStage(string source)
        {
            return LineStage(source);
        }

        public override string OnFinishUp_LineStage(string source)
        {
            return LineStage(source);
        }

        public abstract string LineStage(string source);
    }
}