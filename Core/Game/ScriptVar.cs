using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirSuperiority.Core
{
    public interface IScriptVar
    {
        string Name { get; }
        bool ReadOnly { get; }
    }

    /// <summary>
    /// Represents a script variable object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ScriptVar<T> : IScriptVar
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="value">The initial value of the variable.</param>
        /// <param name="defaultValue">The default value of the variable.</param>
        /// <param name="isReadonly">Whether the variable is readonly.</param>
        public ScriptVar(string name, T value, T defaultValue, bool isReadonly)
        {
            Name = name;
            Value = value;
            Default = defaultValue;
            ReadOnly = isReadonly;
        }

        public ScriptVar(string name, T value, T defaultValue) : 
            this(name, value, defaultValue, false)
        { }

        /// <summary>
        /// The name alias for the script var.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The current value of the script var.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// The default value of the script var.
        /// </summary>
        public T Default { get; }

        /// <summary>
        /// Whether the script var is read-only.
        /// </summary>
        public bool ReadOnly { get; }
    }
}
