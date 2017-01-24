using System;
using GTA;

namespace AirSuperiority.Core
{
    /// <summary>
    /// Represents a game entity.
    /// </summary>
    public abstract class ScriptEntity<T> : ScriptComponent, IScriptEntity, IDisposable where T : Entity
    {
        /// <summary>
        /// Base entity reference.
        /// </summary>
        public T Ref { get; }

        /// <summary>
        /// Pool of extensions attached to this entity.
        /// </summary>
        public ScriptEntityExtensionPool<T> Extensions { get; }

        /// <summary>
        /// Fired when the entity has been revived.
        /// </summary>
        public event ScriptEntityEventHandler Alive;

        /// <summary>
        /// Fired when the entity has died.
        /// </summary>
        public event ScriptEntityEventHandler Dead;

        /// <summary>
        /// Fired when the entity enters water.
        /// </summary>
        public event ScriptEntityEventHandler EnterWater;

        /// <summary>
        /// Total entity ticks.
        /// </summary>
        public int TotalTicks
        {
            get { return totalTicks; }
        }

        /// <summary>
        /// Total time entity has been available to the script.
        /// </summary>
        public TimeSpan TotalTime
        {
            get { return totalTime; }
        }

        /// <summary>
        /// Total ticks for which the entity has been alive.
        /// </summary>
        public int AliveTicks
        {
            get { return aliveTicks; }
        }

        /// <summary>
        /// Total ticks for which the entity has been dead.
        /// </summary>
        public int DeadTicks
        {
            get { return deadTicks; }
        }

        /// <summary>
        /// Total ticks for which the entity has been in water.
        /// </summary>
        public int InWaterTicks
        {
            get { return waterTicks; }
        }

        /// <summary>
        /// Time at which the entity was made avilable to the script.
        /// </summary>
        public int CreatedTime
        {
            get { return createdTime; }
        }

        private TimeSpan totalTime;

        private int createdTime;

        private int deadTicks, aliveTicks, waterTicks, totalTicks;

        /// <summary>
        /// Initialize the class.
        /// </summary>
        /// <param name="baseRef"></param>
        /// <param name="name"></param>
        public ScriptEntity(T baseRef, string name) : base(name)
        {
            Ref = baseRef;
            Extensions = new ScriptEntityExtensionPool<T>();
            createdTime = Game.GameTime;
        }

        /// <summary>
        /// Initialize the class.
        /// </summary>
        /// <param name="baseRef"></param>
        public ScriptEntity(T baseRef) : this(baseRef, null)
        { }

        /// <summary>
        /// Add a script extension to this entity.
        /// </summary>
        /// <param name="extension"></param>
        public void AddExtension(ScriptEntityExtension<T> extension)
        {
            if (!Extensions.Contains(extension))
            {
                extension.Entity = this;

                Extensions.Add(extension);
            }
        }

        /// <summary>
        /// Fired when the entity has died.
        /// </summary>
        protected virtual void OnDead(ScriptEntityEventArgs e)
        {
            Dead?.Invoke(this, e);
        }

        protected virtual void OnAlive(ScriptEntityEventArgs e)
        {
            Alive?.Invoke(this, e);
        }

        protected virtual void OnEnterWater(ScriptEntityEventArgs e)
        {
            EnterWater?.Invoke(this, e);
        }

        /// <summary>
        /// Call this method each tick to update entity related information.
        /// </summary>
        public override void OnUpdate()
        {
            foreach (var extension in Extensions)
            {
                extension.OnUpdate();
            }

            if (Ref.IsDead)
            {
                if (deadTicks == 0)
                    OnDead(new ScriptEntityEventArgs(this));

                aliveTicks = 0;
                deadTicks++;
            }

            else
            {
                if (Ref.IsInWater)
                {
                    if (waterTicks == 0)
                        OnEnterWater(new ScriptEntityEventArgs(this));

                    waterTicks++;
                }
                else
                    waterTicks = 0;

                if (aliveTicks == 0)
                    OnAlive(new ScriptEntityEventArgs(this));

                deadTicks = 0;
                aliveTicks++;
            }

            totalTicks++;

            totalTicks = totalTicks % int.MaxValue;

            totalTime = TimeSpan.FromMilliseconds(Game.GameTime - createdTime);
        }

        /// <summary>
        /// Dispose of all active extensions for this entity.
        /// </summary>
        public void ClearExtensions()
        {
            foreach (var extension in Extensions)
            {
                extension.Dispose();
            }

            Extensions.Clear();
        }

        /// <summary>
        /// Remove from the world.
        /// </summary>
        public virtual void Dispose()
        {
            ClearExtensions();

            Ref.CurrentBlip?.Remove();

            Ref.Delete();
        }
    }
}
