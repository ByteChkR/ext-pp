using System;
using ext_pp_base.settings;

namespace ext_pp_base
{
    public static class Logger
    {
        public static Verbosity VerbosityLevel;

        public static void Log(int mask, string message, Verbosity level)
        {
            if (level <= VerbosityLevel)
            {
                for (int i = (int)Verbosity.LEVEL2; i < (int)level; i++)
                {
                    message = '\t' + message;
                }
                ADL.Debug.Log(mask, "[" + Enum.GetName(typeof(Verbosity), level) + "]" + message);
            }

        }

        public static void Log(DebugLevel mask, string message, Verbosity level)
        {
            Log((int)mask, message, level);
        }

        public static void Crash(Exception ex, bool throwEx = true)
        {
            ADL.Crash.CrashHandler.Log(ex);
            if (throwEx) throw ex;
        }
    }
}