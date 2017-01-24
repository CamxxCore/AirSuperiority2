using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Types;
using AirSuperiority.ScriptBase.Extensions;
using GTA;
using GTA.Math;
using GTA.Native;

namespace AirSuperiority.ScriptBase.Entities
{
    /// <summary>
    /// Top- most abstraction of a player instance. Holds both the vehicle and ped instance for a player.
    /// </summary>
    public sealed class PlayerParticipant : ScriptComponent
    {
        /// <summary>
        /// Managed ped object associated to this player.
        /// </summary>
        public ManagedPed Ped { get; private set; }

        /// <summary>
        /// Managed fighter istance associated to this player.
        /// </summary>
        public ManagedFighter Vehicle { get; private set; }

        /// <summary>
        /// Player info.
        /// </summary>
        public PlayerInfo Info { get; }

        /// <summary>
        /// Active target (AI only)
        /// </summary>
        public PlayerParticipant ActiveTarget { get; private set; }

        private PlayerInfo info;

        public PlayerParticipant()
        { }

        public PlayerParticipant(ManagedPed ped, ManagedFighter vehicle)
        {
            Ped = ped;
            Vehicle = vehicle;
            CreateExtensions();
        }

        /// <summary>
        /// Assign a ManagedPed and ManagedVehicle instance to this player.
        /// </summary>
        /// <param name="Pilot"></param>
        /// <param name="Vehicle"></param>
        /// <returns></returns>
        public PlayerParticipant Manage(Ped ped, Vehicle vehicle)
        {
            Ped = new ManagedPed(ped);
            Vehicle = new ManagedFighter(vehicle);
            CreateExtensions();
            return this;
        }

        private void CreateExtensions()
        {
            if (Ped.IsHuman)
            {
                Vehicle.AddExtension(new SpawnCamera());
                Vehicle.AddExtension(new SpawnBooster());
            }
        }

        /// <summary>
        /// Fight against the specifed fighter.
        /// </summary>
        /// <param name="opponent"></param>
        public void FightAgainst(PlayerParticipant opponent)
        {
            Ped.Ref.Task.FightAgainst(opponent.Ped.Ref);
        }

        /// <summary>
        /// Clear all ped tasks.
        /// </summary>
        public void ClearTasks()
        {
            Ped.Ref.Task.ClearAll();
        }

        /// <summary>
        /// Assign this fighter a team.
        /// </summary>
        /// <param name="newTeam"></param>
        public void AssignTeam(int teamIdx)
        {
            info.Sess.TeamNum = teamIdx;
        }

        /// <summary>
        /// Set target and begin vehicle mission.
        /// </summary>
        /// <param name="opponent"></param>
        public void SetTarget(PlayerParticipant opponent)
        {
            if (Ped.IsHuman) return; // Only applies to AI players..

            Ped ped = opponent.Ped.Ref;

            Vector3 position = ped.Position;

            Function.Call(Hash.TASK_PLANE_MISSION,
                Ped.Ref.Handle,
                Vehicle.Ref.Handle,
                ped.Handle,
                ped.Handle,
                position.X, position.Y, position.Z,
                6, 5.0, -1.0, 30.0, 500, 50);

            ActiveTarget = opponent;
        }

        /// <summary>
        /// Set active target for vehicle mission natives.
        /// </summary>
        /// <param name="fighter"></param>
        public void ClearActiveTarget()
        {
            ActiveTarget = null;
        }

        public override void OnUpdate()
        {
            Ped.OnUpdate();

            Vehicle.OnUpdate();

            if (Ped.IsHuman)
            {
                if (Game.IsControlJustPressed(0, Control.ScriptLB) || Game.IsControlJustPressed(0, (Control)48))
                {
                    Vehicle.DoFireExtinguisher();
                }

                else if (Game.IsControlJustPressed(0, Control.ScriptRB) || Game.IsControlJustPressed(0, (Control)337))
                {
                    Vehicle.DoIRFlares();
                }
            }

            else
            {

            }

            base.OnUpdate();
        }

        /// <summary>
        /// Removes the ped and vehicle from the world.
        /// </summary>
        public void Remove()
        {
            if (!Ped.IsHuman)
            {
                Ped.Dispose();
            }

            Vehicle.Dispose();
        }
    }
}
