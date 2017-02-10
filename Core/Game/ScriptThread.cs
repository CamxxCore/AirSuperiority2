using System;
using System.Collections.Generic;
using System.Threading;
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
        private static ScriptExtensionPool extensions = new ScriptExtensionPool();

        /// <summary>
        /// Script vars.
        /// </summary>
        private static ScriptVarCollection vars = new ScriptVarCollection();

        public ScriptThread()
        {
            Tick += (s,e) => OnUpdate(Game.GameTime);;
        }

        /// <summary>
        /// Get a script var attached to this thread.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ScriptVar<T> GetVar<T>(string name)
        {
            return vars.Get<T>(name);
        }

        /// <summary>
        /// Set the value of a script var attached to this thread.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool SetVar<T>(string name, T value)
        {
            ScriptVar<T> var = vars.Get<T>(name);

            if (var != null && !var.ReadOnly)
            {
                var.Value = value;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Register a new script var and add it to the collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name of the var</param>
        /// <param name="value">The initial value</param>
        /// <param name="defaultValue">The default (reset) value</param>
        public static void RegisterVar<T>(string name, T initialValue)
        {
            vars.Add(name, initialValue, initialValue);
        }

        /// <summary>
        /// Adds a script extension to this thread.
        /// </summary>
        /// <param name="extension"></param>
        public static void AddExtension(ScriptExtension extension) 
        {
            if (!extensions.Contains(extension))
            {
                extensions.Add(extension);
            }
        }

        /// <summary>
        /// Adds a script extension to this thread.
        /// </summary>
        /// <param name="extension"></param>
        public static void AddExtension<T>() where T : ScriptExtension, new()
        {
            T extension = extensions.Get<T>();

            if (extension == null)
            {
                extension = new T();

                extensions.Add(extension);
            }
        }

        /// <summary>
        /// Get a script extension from the underlying pool by its type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected static T GetExtension<T>() where T : ScriptExtension
        {
            return extensions.Get<T>();
        }

        /// <summary>
        /// Get an extension, or create it if it doesn't exist.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetOrAddExtension<T>() where T : ScriptExtension, new()
        {
            var item = GetExtension<T>();

            if (item == null)
            {
                item = new T();

                AddExtension(item);
            }

            return item;
        }

        /// <summary>
        /// Updates the thread.
        /// </summary>
        public virtual void OnUpdate(int gameTime)
        {
            for (int i = 0; i < extensions.Count; i++)
            {
                extensions[i].OnUpdate(gameTime);
            }
        }

        /// <summary>
        /// Removes the thread and all extensions.
        /// </summary>
        /// <param name="A_0"></param>
        protected override void Dispose(bool A_0)
        {
            Tick -= (s, e) => OnUpdate(Game.GameTime);

            for (int i = 0; i < extensions.Count; i++)
            {
                extensions[i].Dispose();
            }

            base.Dispose(A_0);
        }
    }
}
