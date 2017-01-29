﻿using GTA;

namespace AirSuperiority.Core
{
    /// <summary>
    /// Represents a ped.
    /// </summary>
    public class ScriptPed : ScriptEntity<Ped>
    {
        /// <summary>
        /// Fired when the ped has entered a vehicle.
        /// </summary>
        public event ScriptEntityEventHandler EnterVehicle;

        /// <summary>
        /// Fired when the ped has exited a vehicle.
        /// </summary>
        public event ScriptEntityEventHandler ExitVehicle;


        private int vehicleTicks = 0;

        /// <summary>
        /// If the ped is a local player/ human.
        /// </summary>
        public bool IsHuman
        {
            get { return Ref == Game.Player.Character; }
        }

        public ScriptPed(ScriptThread thread, Ped baseRef) : base(thread, baseRef)
        { }

        protected virtual void OnEnterVehicle(ScriptEntityEventArgs e)
        {
            EnterVehicle?.Invoke(this, e);
        }

        protected virtual void OnExitVehicle(ScriptEntityEventArgs e)
        {
            ExitVehicle?.Invoke(this, e);
        }

        public override void Dispose()
        {
            if (!IsHuman)
            {
                base.Dispose();
            }
        }

        public override void OnUpdate(int gameTime)
        {
            if (Ref.IsInVehicle())
            {
                if (vehicleTicks == 0)
                    OnEnterVehicle(new ScriptEntityEventArgs(gameTime));
                vehicleTicks++;
            }

            else
            {
                if (vehicleTicks > 0)
                {
                    OnExitVehicle(new ScriptEntityEventArgs(gameTime));
                    vehicleTicks = 0;
                }
            }

            base.OnUpdate(gameTime);
        }
    }
}
