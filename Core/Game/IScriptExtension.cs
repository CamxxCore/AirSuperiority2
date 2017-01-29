using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirSuperiority.Core
{
    /// <summary>
    /// Represents an extension for a <see cref="ScriptThread"/>
    /// </summary>
    interface IScriptExtension
    {
        /// <summary>
        /// The base script thread this extension belongs to.
        /// </summary>
        ScriptThread BaseThread { get; }
    }
}
