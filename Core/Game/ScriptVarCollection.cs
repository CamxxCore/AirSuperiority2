using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirSuperiority.Core
{
    public class ScriptVarCollection : Dictionary<string, IScriptVar>
    {
        public ScriptVarCollection() : base(StringComparer.OrdinalIgnoreCase)
        { }

        /// <summary>
        /// Add a var to the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="initialValue"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public ScriptVar<T> Add<T>(string name, T initialValue, T defaultValue)
        {
            var var = new ScriptVar<T>(name, initialValue, defaultValue);
            Add(name, var);
            return var;
        }

        /// <summary>
        /// Get an extension from the pool by its type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ScriptVar<T> Get<T>(string name)
        {
            return this[name] as ScriptVar<T>;
        }
    }
}
