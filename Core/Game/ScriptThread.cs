using GTA;

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
            Tick += (s,e) => OnUpdate();
            Extensions = new ScriptExtensionPool();
        }

        /// <summary>
        /// Adds a script extension to this thread.
        /// </summary>
        /// <param name="extension"></param>
        public void AddExtension(string identifier, ScriptExtension extension)
        {
            if (!Extensions.ContainsKey(identifier))
            {
                extension.BaseThread = this;

                Extensions.Add(identifier, extension);
            }
        }

        public ScriptExtension GetExtension(string identifier)
        {
            ScriptExtension extension;

            Extensions.TryGetValue(identifier, out extension);

            return extension;
        }

        /// <summary>
        /// Updates the thread.
        /// </summary>
        public virtual void OnUpdate()
        {
            foreach (var extension in Extensions.Values)
            {
                extension.OnUpdate();
            }
        }

        /// <summary>
        /// Removes the thread and all extensions.
        /// </summary>
        /// <param name="A_0"></param>
        protected override void Dispose(bool A_0)
        {
            foreach (var extension in Extensions.Values)
            {
                extension.Dispose();
            }

            base.Dispose(A_0);
        }
    }
}
