using System;
using System.Diagnostics;

namespace AirSuperiority.ScriptBase.Helpers
{
    /// <summary>
    /// Class for measuring internal performance and timing of method calls.
    /// </summary>
    public static class CodeTimer
    {
        private static Stopwatch sw = new Stopwatch();

        static CodeTimer()
        {

        }

        public static void Start()
        {
            sw.Reset();
            sw.Start();
        }

        public static void Stop()
        {
            sw.Stop();
        }

        public static long GetElapsed()
        {
            return sw.ElapsedMilliseconds;
        }
    }
}
