namespace ext_pp_base.settings
{

    /// <summary>
    /// An enum that contains all possible process stages.
    /// </summary>
    public enum ProcessStage
    {
        QUEUED = 0,
        ON_LOAD_STAGE = 1,
        ON_MAIN = 2,
        ON_FINISH_UP = 4,
        FINISHED = 8,
    }
}