using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirSuperiority.Core
{
    /// <summary>
    /// Represents a component of a script.
    /// </summary>
    public interface IScriptComponent
    {
        /// <summary>
        /// Gets the name of this component.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// Unique Id.
        /// </summary>
        Guid Guid { get; }
    }
}
