using System.Diagnostics;

namespace ext_pp
{


    /// <summary>
    /// A timer class used to measure time during compilation
    /// </summary>
    public class Timer
    {
        /// <summary>
        /// A internal static timer
        /// </summary>
        internal static Timer GlobalTimer { get; } = new Timer();

        /// <summary>
        /// A static wrapper for the singleton(showing the total ellapsed milliseconds since assembly load.
        /// </summary>
        public static long MS => GlobalTimer.StopWatch.ElapsedMilliseconds;
        
        /// <summary>
        /// The underlying stopwatch
        /// </summary>
        private Stopwatch StopWatch { get; } = new Stopwatch();


        /// <summary>
        /// Starts the Timer
        /// </summary>
        public void Start()
        {
            StopWatch.Start();
        }
        
        /// <summary>
        /// Resets and Starts the timer
        /// </summary>
        /// <returns>ellapsed milliseconds before reset</returns>
        public long Restart()
        {
            StopWatch.Stop();
            long ret = Reset();
            Start();
            return ret;
        }
        /// <summary>
        /// Resets the Timer.
        /// </summary>
        /// <returns>ellapsed milliseconds before reset</returns>
        public long Reset()
        {
            long ret = StopWatch.ElapsedMilliseconds;
            StopWatch.Reset();
            return ret;
        }

    }
}