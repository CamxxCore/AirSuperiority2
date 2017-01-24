using System;
using System.IO;
using System.Reflection;

namespace AirSuperiority.Core
{
    /// <summary>
    /// Static logger class that allows direct logging of anything to a text file
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Write a new entry to the application log file.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Log(string format, params object[] args)
        {
            File.AppendAllText(
                string.Format("{0}.txt", filename), DateTime.Now + " : " + string.Format(format, args) + Environment.NewLine);
        }

        private static readonly string filename = AssemblyName.GetAssemblyName(Assembly.GetExecutingAssembly().FullName).Name;
    }
}
