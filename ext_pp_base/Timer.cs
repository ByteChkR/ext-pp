using System.Diagnostics;

namespace ext_pp_base
{



    public class Timer
    {
        private static Timer GlobalTimer { get; } = new Timer();
        public static long MS => GlobalTimer.StopWatch.ElapsedMilliseconds;
        

        private Stopwatch StopWatch { get; } = new Stopwatch();
        public long LastLap { get; private set; }

        public Timer()
        {
            Start();
        }

        public void Start()
        {
            StopWatch.Start();
        }

        public long Pause()
        {
            StopWatch.Stop();
            return StopWatch.ElapsedMilliseconds;
        }

        public long RecordLapReturnDelta()
        {
            long old = LastLap;

            LastLap = StopWatch.ElapsedMilliseconds;
            return LastLap-old;
        }

        public long Restart()
        {
            StopWatch.Stop();
            long ret = Reset();
            Start();
            return ret;
        }

        public long Reset()
        {
            long ret = StopWatch.ElapsedMilliseconds;
            StopWatch.Reset();
            return ret;
        }

    }
}