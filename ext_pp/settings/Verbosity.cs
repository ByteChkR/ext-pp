namespace ext_compiler.settings
{
    public enum Verbosity
    {
        /// <summary>
        /// Lowest Verbosity Level, no output on console.
        /// </summary>
        SILENT=0,
        /// <summary>
        /// Only critical errors
        /// </summary>
        ALWAYS_SEND = 1,
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