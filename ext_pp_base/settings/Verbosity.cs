namespace ext_pp_base.settings
{
    public enum Verbosity
    {
        /// <summary>
        /// Lowest Verbosity Level, no output on console.
        /// </summary>
        SILENT=0,
        /// <summary>
        /// Only critical errors and general information
        /// </summary>
        ALWAYS_SEND,
        LEVEL1,
        LEVEL2,
        LEVEL3,
        LEVEL4,
        LEVEL5,
        LEVEL6,
        /// <summary>
        /// Highest Level of verbosity, you will get every log that gets sent.
        /// </summary>
        LEVEL7


    }
}