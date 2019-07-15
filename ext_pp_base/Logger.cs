using System;
using ext_pp_base.settings;

namespace ext_pp_base
{
    public static class Logger
    {
        /// <summary>
        /// The Verbosity level
        /// Everything lower than this will be sent to the log output
        /// </summary>
        public static Verbosity VerbosityLevel= Verbosity.LEVEL2;

        /// <summary>
        /// Logs a message in the specified mask and verbosity level
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="message"></param>
        /// <param name="level"></param>
        public static void Log(int mask, string message, Verbosity level)
        {
            if (level <= VerbosityLevel)
            {
                for (int i = (int)Verbosity.LEVEL3; i < (int)level; i++)
                {
                    message = '\t' + message;
                }
                ADL.Debug.Log(mask, "[" + Enum.GetName(typeof(Verbosity), level) + "]" + message);
            }

        }
        /// <summary>
        /// Logs a message in the specified Debug and VerbosityLevel
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="message"></param>
        /// <param name="level"></param>
        public static void Log(DebugLevel mask, string message, Verbosity level)
        {
            Log((int)mask, message, level);
        }


        /// <summary>
        /// Implements ADL.Crash to log a Exeption to the output stream as detailed as possible.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="throwEx">if it should only log and not throw the exception specify this to be false</param>
        public static void Crash(Exception ex, bool throwEx = true)
        {
            ADL.Crash.CrashHandler.Log(ex);
            if (throwEx) throw ex;
        }
    }
}