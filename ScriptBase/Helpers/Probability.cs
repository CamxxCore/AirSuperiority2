using System;

namespace AirSuperiority.ScriptBase.Helpers
{
    public static partial class Probability
    {
        private static int lastCheckedTime = 0;

        /// <summary>
        /// Checks for a conditon given a % of chance and interval
        /// </summary>
        /// <param name="chance">% chance of success</param>
        /// <returns>rand</returns>
        public static bool GetBoolean(float chance)
        {
            return GetBoolean(chance, 0);
        }

        /// <summary>
        /// Checks for a conditon given a % of chance and interval
        /// </summary>
        /// <param name="chance">% chance of success</param>
        /// <param name="checkInterval">The amount of time that needs to pass before checking for another value</param>
        /// <returns>rand</returns>
        public static bool GetBoolean(float chance, int checkInterval)
        {
            return GetBoolean(chance, checkInterval, Guid.NewGuid().GetHashCode());
        }

        public static bool GetBoolean(float chance, int checkInterval, int seed)
        {
            if (checkInterval > 0 && Environment.TickCount - lastCheckedTime < checkInterval)
            {
                return false;
            }

            lastCheckedTime = Environment.TickCount;

            int rdMax = (int)(chance * 1000.0f);

            return new Random(seed).Next(0, 1000) < rdMax;
        }
    }
}
