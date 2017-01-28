using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirSuperiority.Core
{
    /// <summary>
    /// Represents an object that can be updated by a script.
    /// </summary>
    interface IScriptUpdatable
    {
        /// <summary>
        /// Method to be fired each frame.
        /// </summary>
        void OnUpdate(int gameTime);
    }
}
