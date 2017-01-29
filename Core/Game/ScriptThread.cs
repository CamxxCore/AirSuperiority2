using GTA;
using System.Collections.Generic;
using System.Threading;

namespace AirSuperiority.Core
{
    /// <summary>
    /// Base class for a script thread.
    /// </summary>
    public abstract class ScriptThread : Script
    {
        /// <summary>
        /// Script extension pool.
        /// </summary>
        public ScriptExtensionPool Extensions { get; }

        public ScriptThread()
        {
            Tick += (s,e) => OnUpdate(Game.GameTime);
            Extensions = new ScriptExtensionPool();
        }

        /// <summary>
        /// Adds a script extension to this thread.
        /// </summary>
        /// <param name="extension"></param>
        public void AddExtension(ScriptExtension extension)
        {
            if (!Extensions.Contains(extension))
            {
                extension.BaseThread = this;

                Extensions.Add(extension);
            }
        }

        /// <summary>
        /// Get a script extension from the underlying pool by its type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public T Get<T>() where T : ScriptExtension
        {
            return Extensions.Get<T>();
        }

        /// <summary>
        /// Updates the thread.
        /// </summary>
        public virtual void OnUpdate(int gameTime)
        {
            for (int i = 0; i < Extensions.Count; i++)
            {
                Extensions[i].OnUpdate(gameTime);
            }
        }

        /// <summary>
        /// Removes the thread and all extensions.
        /// </summary>
        /// <param name="A_0"></param>
        protected override void Dispose(bool A_0)
        {
            Tick -= (s, e) => OnUpdate(Game.GameTime);

            for (int i = 0; i < Extensions.Count; i++)
            {
                Extensions[i].Dispose();
            }

            base.Dispose(A_0);
        }
    }
}
