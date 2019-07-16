namespace ext_pp_base.settings
{
    public enum ProcessStage
    {
        QUEUED = 0,
        ON_LOAD_STAGE = 1,
        ON_MAIN = 2,
        ON_FINISH_UP = 4,
        FINISHED = 8,
    }
}