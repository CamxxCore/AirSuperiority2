using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirSuperiority.Core
{
    /// <summary>
    /// Base interface for all updatable script components.
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

        /// <summary>
        /// Method to be fired each frame.
        /// </summary>
        void OnUpdate(int gameTime);
    }
}
