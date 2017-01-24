using System;
using GTA;

namespace AirSuperiority.Core
{
    /// <summary>
    /// An class for extending a <see cref="GTA.Entity"/>
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    public abstract class ScriptEntityExtension<T> : ScriptComponent, IScriptEntityExtension<T>, IDisposable where T : Entity
    {
        /// <summary>
        /// Base entity associated with this extension.
        /// </summary>
        public ScriptEntity<T> Entity
        {
            get { return entity; }

            set
            {
                if (entity != null)
                {
                    OnEntityDetached(entity);
                }

                entity = value;
                OnEntityAttached(entity);
            }
        }

        /// <summary>
        /// Fired when the target entity is attached to this instance.
        /// </summary>
        /// <param name="entity"></param>
        public virtual void OnEntityAttached(ScriptEntity<T> entity)
        { }

        /// <summary>
        /// Fired when the target entity is detached from this instance.
        /// </summary>
        /// <param name="entity"></param>
        public virtual void OnEntityDetached(ScriptEntity<T> entity)
        { }


        private ScriptEntity<T> entity;

        /// <summary>
        /// Dispose the extension and related resources.
        /// </summary>
        public virtual void Dispose()
        {
            if (entity != null)
            {
                OnEntityDetached(entity);

                entity = null;
            }
        }
    }
}
