using System;

namespace AirSuperiority.Core
{
    /// <summary>
    /// Base class for an updatable script thread extension.
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

        public ScriptExtension(ScriptThread baseThread, string name) : base(name)
        {
            BaseThread = baseThread;
        }

        public ScriptExtension(ScriptThread baseThread) : this(baseThread, null)
        { }

        private ScriptThread baseThread;

        /// <summary>
        /// Fired when a thread is attached to this instance.
        /// </summary>
        /// <param name="entity"></param>
        public virtual void OnThreadAttached(ScriptThread thread)
        {
            baseThread.Extensions.Add(this);
        }

        /// <summary>
        /// Dispose the extension and stop updating it on the base thread.
        /// </summary>
        public virtual void Dispose()
        {
            baseThread.Extensions.Remove(this);
        }
    }
}
