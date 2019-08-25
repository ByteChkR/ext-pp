namespace ext_pp_base.settings
{
    /// <summary>
    /// Debug level is used to indicate the type of debug logs that are sent.
    /// </summary>
    public enum DebugLevel
    {
        ALL=-1,
        NONE=0,
        ERRORS=1,
        WARNINGS=2,
        LOGS=4,
        INTERNAL_ERROR=8,
        PROGRESS=16
    }
}