﻿using System;
using GTA;

namespace AirSuperiority.Core
{
    /// <summary>
    /// Represents a game entity.
    /// </summary>
    public abstract class ScriptEntity<T> : ScriptExtension, IScriptEntity where T : Entity
    {
        /// <summary>
        /// Base game entity reference.
        /// </summary>
        public T Ref { get; }

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
        public ScriptEntity(ScriptThread thread, T baseRef, string name) : base(thread, name)
        {
            Ref = baseRef;
            createdTime = Game.GameTime;
        }

        /// <summary>
        /// Initialize the class.
        /// </summary>
        /// <param name="baseRef"></param>
        public ScriptEntity(ScriptThread thread, T baseRef) : this(thread, baseRef, null)
        { }

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
        public override void OnUpdate(int gameTime)
        {
            if (Ref.IsDead)
            {
                if (deadTicks == 0)
                {
                    OnDead(new ScriptEntityEventArgs(gameTime));
                }

                aliveTicks = 0;
                deadTicks++;
            }

            else
            {
                if (Ref.IsInWater)
                {
                    if (waterTicks == 0)
                        OnEnterWater(new ScriptEntityEventArgs(gameTime));

                    waterTicks++;
                }
                else
                    waterTicks = 0;

                if (aliveTicks == 0)
                    OnAlive(new ScriptEntityEventArgs(gameTime));

                deadTicks = 0;
                aliveTicks++;
            }

            totalTicks++;

            totalTicks = totalTicks % int.MaxValue;

            totalTime = TimeSpan.FromMilliseconds(gameTime - createdTime);
        }

        public void Remove()
        {
            Ref.MarkAsNoLongerNeeded();

            Ref.CurrentBlip?.Remove();

            Ref.Delete();
        }
    }
}