﻿using System;
using System.Reflection;
using ext_pp_base.settings;

namespace ext_pp_base
{
    public static class Logger
    {
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