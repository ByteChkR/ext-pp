using System;
using System.Diagnostics;
using ext_pp.settings;

namespace ext_pp
{
    public static class Logger
    {
        public static void Log(int mask, string message, Verbosity level)
        {
            if (level <= ExtensionProcessor.settings.VerbosityLevel)
                ADL.Debug.Log(mask, "[" + Enum.GetName(typeof(Verbosity), level) + "]" + message);
        }

        public static void Log(DebugLevel mask, string message, Verbosity level)
        {
            Log((int)mask, message, level);
        }

        public static void Crash(Exception ex, bool throwEx=true)
        {
            ADL.Crash.CrashHandler.Log(ex);
            if(throwEx)throw ex;
        }
    }
}