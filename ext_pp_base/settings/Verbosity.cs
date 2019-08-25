namespace ext_pp_base.settings
{

    /// <summary>
    /// An enum used to give logs an importance.
    /// </summary>
    public enum Verbosity
    {
        /// <summary>
        /// Lowest Verbosity Level, no output on console.
        /// </summary>
        SILENT=0,
        /// <summary>
        /// Only critical errors and general information
        /// </summary>
        LEVEL1,
        LEVEL2,
        LEVEL3,
        LEVEL4,
        LEVEL5,
        LEVEL6,
        LEVEL7,
        /// <summary>
        /// Highest Level of verbosity, you will get every log that gets sent.
        /// </summary>
        LEVEL8


    }
}