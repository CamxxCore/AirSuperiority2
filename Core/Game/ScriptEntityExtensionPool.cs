using System.Collections.Generic;

namespace AirSuperiority.Core
{
    /// <summary>
    /// Class to hold script entity extensions.
    /// </summary>
    public class ScriptEntityExtensionPool<T> :
        List<ScriptEntityExtension<T>> where T : GTA.Entity
    { }
}