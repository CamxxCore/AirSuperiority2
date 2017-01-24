using System;

namespace AirSuperiority.Core
{
    /// <summary>
    /// Base class for a script thread extension.
    /// </summary>
    public class ScriptExtension : ScriptComponent, IScriptExtension, IDisposable
    {
        /// <summary>
        /// The base thread this extension belongs to.
        /// </summary>
        public ScriptThread BaseThread
        {
            get
            {
                return baseThread;
            }

            set
            {
                baseThread = value;
                OnThreadAttached(baseThread);
            }
        }

        private ScriptThread baseThread;

        /// <summary>
        /// Fired when a thread is attached to this instance.
        /// </summary>
        /// <param name="entity"></param>
        public virtual void OnThreadAttached(ScriptThread thread)
        { }

        public virtual void Dispose()
        { }
    }
}
