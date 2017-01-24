using System;

namespace AirSuperiority.Core
{
    public delegate void ScriptEntityEventHandler(IScriptEntity sender, ScriptEntityEventArgs args);

    /// <summary>
    /// Event args for a script entity event.
    /// </summary>
    public sealed class ScriptEntityEventArgs : EventArgs
    {
        public ScriptEntityEventArgs(IScriptEntity entity)
        {
            Entity = entity;
        }

        /// <summary>
        /// The entity that fired the event
        /// </summary>
        public IScriptEntity Entity { get; private set; }
    }
}
