using System;
using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Types;
using AirSuperiority.ScriptBase.Extensions;
using AirSuperiority.ScriptBase.Helpers;
using GTA;
using GTA.Math;
using GTA.Native;

namespace AirSuperiority.ScriptBase.Entities
{
    public delegate void GameParticipantEventHandler(GameParticipant sender, EventArgs e);

    /// <summary>
    /// Top- most abstraction of a player instance. Holds both the vehicle and ped instance for a player.
    /// </summary>
    public class GameParticipant : ScriptComponent, IGameParticipant
    {
        /// <summary>
        /// Managed ped object associated to this player.
        /// </summary>
        public ManagedPed Ped { get; protected set; }

        /// <summary>
        /// Managed fighter istance associated to this player.
        /// </summary>
        public ManagedFighter Vehicle { get; protected set; }

        /// <summary>
        /// Player info.
        /// </summary>
        public PlayerInfo Info { get; }

        /// <summary>
        /// Active target (AI only)
        /// </summary>
        public GameParticipant ActiveTarget { get; private set; }

        /// <summary>
        /// Position of the player.
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return Vehicle.Ref.Position;
            }
        }

        /// <summary>
        /// Whether the player has died
        /// </summary>
        public bool IsDead
        {
            get
            {
                return Ped.Ref.IsDead;
            }
        }

        private PlayerInfo info;

        public event GameParticipantEventHandler OnDead;

        public GameParticipant(string playerName, int teamIndex) : base(playerName)
        {
            info.Name = playerName;
            info.Sess.TeamNum = teamIndex;
            info.Sess.State = PlayerState.Inactive;
        }

        public virtual void CreateEntity(LevelSpawn spawnPoint)
        {
            info.Sess.State = PlayerState.Respawning;
  
            Vector3 position = Utility.EnsureValidSpawnPos(spawnPoint.Position);

            Model model = new Model(new VehicleHash[] {
                VehicleHash.Lazer,
                VehicleHash.Besra,
                VehicleHash.Hydra }.GetRandomItem());

            if (!model.IsLoaded)
                model.Request(1000);

            var vehicle = World.CreateVehicle(model, position, spawnPoint.Heading);

            vehicle.LodDistance = 2000;
            vehicle.EngineRunning = true;
            vehicle.BodyHealth = 0.01f;
            vehicle.MaxHealth = 1;

            Function.Call(Hash.SET_VEHICLE_EXPLODES_ON_HIGH_EXPLOSION_DAMAGE, vehicle, true);

            model = new Model(PedHash.Pilot02SMM);

            if (!model.IsLoaded)
                model.Request(1000);

            Ped ped = World.CreatePed(model, position);

            Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 1, 1);

            Function.Call(Hash.SET_PED_COMBAT_ATTRIBUTES, ped, 52, 0);

            Function.Call(Hash.SET_PED_CAN_SWITCH_WEAPON, ped, false);

            ped.SetIntoVehicle(vehicle, VehicleSeat.Driver);

            Manage(ped, vehicle);
        }

        /// <summary>
        /// Assign a ManagedPed and ManagedVehicle instance to this player.
        /// </summary>
        /// <param name="Pilot"></param>
        /// <param name="Vehicle"></param>
        /// <returns></returns>
        public virtual void Manage(Ped ped, Vehicle vehicle)
        {
            Ped = new ManagedPed(ped);

            Vehicle = new ManagedFighter(vehicle);

            Vehicle.AddExtension(new VehicleLandingGearManager());

            Ped.Alive += OnParticipantAlive;

            Ped.Dead += (s, e) => OnParticipantDead(new EventArgs());
            Ped.EnterWater += (s, e) => OnParticipantDead(new EventArgs());
            Ped.ExitVehicle += (s, e) => OnParticipantDead(new EventArgs());
        }

        /// <summary>
        /// Fight against the specifed opponent.
        /// </summary>
        /// <param name="opponent"></param>
        public void FightAgainst(GameParticipant opponent)
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
        /// Register a specified amount of kills.
        /// </summary>
        /// <param name="count"></param>
        public void RegisterKills(int count)
        {
            info.Sess.Stats.TotalKills += count;
        }

        /// <summary>
        /// Register a specified amount of deaths.
        /// </summary>
        /// <param name="count"></param>
        public void RegisterDeaths(int count)
        {
            info.Sess.Stats.TotalDeaths += count;
        }

        /// <summary>
        /// Set target and begin vehicle mission.
        /// </summary>
        /// <param name="opponent"></param>
        public void SetTarget(GameParticipant opponent)
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

        /// <summary>
        /// Fired when the participant has spawned.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnParticipantAlive(IScriptEntity sender, ScriptEntityEventArgs args)
        {
            info.Sess.SpawnedTime = Game.GameTime;
            info.Sess.State = PlayerState.Playing;
        }

        /// <summary>
        /// Fired when the participant has died.
        /// </summary>
        /// <param name="e"></param>
        private void OnParticipantDead(EventArgs e)
        {
            info.Sess.State = PlayerState.Dead;
            OnDead?.Invoke(this, e);
        }

        /// <summary>
        /// Update the class
        /// </summary>
        /// <param name="gameTime"></param>
        public override void OnUpdate(int gameTime)
        {
            Ped.OnUpdate(gameTime);

            Vehicle.OnUpdate(gameTime);

            base.OnUpdate(gameTime);
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
