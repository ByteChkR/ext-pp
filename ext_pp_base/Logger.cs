using System;
using System.Reflection;
using ext_pp_base.settings;

namespace ext_pp_base
{
    public static class Logger
    {
        public static bool ThrowOnError { get; set; } = true;
        public static bool ThrowOnWarning { get; set; }

        public static int ErrorCount { get; private set; }
        public static int WarningCount { get; private set; }

        public static void ResetWarnErrorCounter()
        {
            ErrorCount = 0;
            WarningCount = 0;
        }

        /// <summary>
        /// The Verbosity level
        /// Everything lower than this will be sent to the log output
        /// </summary>
        public static Verbosity VerbosityLevel { get; set; } = Verbosity.LEVEL2;

        /// <summary>
        /// Logs a message in the specified mask and verbosity level
        /// </summary>
        /// <param name="mask">the mask that is used to log</param>
        /// <param name="level">the debug level of the log</param>
        /// <param name="format">the format string(the message)</param>
        /// <param name="objs">the format params</param>
        private static void Log(int mask, Verbosity level, string format, params object[] objs)
        {
            if (level <= VerbosityLevel)
            {
                ADL.Debug.Log(mask, string.Format(format, objs));
            }

        }

        /// <summary>
        /// Logs a message in the specified Debug and VerbosityLevel
        /// </summary>
        /// <param name="mask">the mask that is used to log</param>
        /// <param name="level">the debug level of the log</param>
        /// <param name="format">the format string(the message)</param>
        /// <param name="objs">the format params</param>
        private static void Log(DebugLevel mask, Verbosity level, string format, params object[] objs)
        {
            Log((int)mask, level, format, objs);
        }


        /// <summary>
        /// Extension of ILoggable. Is used to log "from" an object.
        /// </summary>
        /// <param name="obj">The ILoggable that the log is sent from</param>
        /// <param name="mask">the mask that is used to log</param>
        /// <param name="level">the debug level of the log</param>
        /// <param name="format">the format string(the message)</param>
        /// <param name="objs">the format params</param>
        public static void Log(this ILoggable obj, DebugLevel mask, Verbosity level, string format, params object[] objs)
        {
            Log(mask, level, "[" + obj.GetType().Name + "]" + format, objs);
        }


        public static void Warning(this ILoggable obj, string format, params object[] objs)
        {
            WarningCount++;
            if (ThrowOnWarning)
            {
                Crash(obj, format, true, objs);
            }
            else
            {
                Log(DebugLevel.WARNINGS, Verbosity.LEVEL1, format, objs);
            }
        }

        public static void Error(this ILoggable obj, string format, params object[] objs)
        {
            ErrorCount++;
            if (ThrowOnError)
            {
                Crash(obj, format, true,objs );
            }
            else
            {
                Log(DebugLevel.ERRORS, Verbosity.SILENT, format, objs);
            }
        }

        public static void Crash(this ILoggable obj, string format, bool throwEx, params object[] objs)
        {
            Crash(new ProcessorException(string.Format(format, objs)), throwEx);
        }

        


        /// <summary>
        /// Implements ADL.Crash to log a Exeption to the output stream as detailed as possible.
        /// </summary>
        /// <param name="ex">The exception</param>
        /// <param name="throwEx">if it should only log and not throw the exception specify this to be false</param>
        public static void Crash(Exception ex, bool throwEx)
        {
            ADL.Crash.CrashHandler.Log(ex);
            if (throwEx)
            {
                throw ex;
            }
        }

        public static void Crash(Exception ex)
        {
            Crash(ex, false);
        }
    }
}