using System;
using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Types;
using AirSuperiority.ScriptBase.Extensions;
using GTA;
using GTA.Math;
using GTA.Native;

namespace AirSuperiority.ScriptBase.Entities
{
    public delegate void GamePlayerEventHandler(Player sender, EventArgs e);

    /// <summary>
    /// Top- most abstraction of a player instance. 
    /// Aggregates all player components including its <see cref="ScriptPed"/> and <see cref="ScriptPlane"/> instance.
    /// </summary>
    public class Player : ScriptExtension, IPlayer
    {
        /// <summary>
        /// Managed <see cref="ScriptPed"/> associated to this player.
        /// </summary>
        public ScriptPed Ped { get; protected set; }

        /// <summary>
        /// Managed <see cref="ScriptPlane"/> associated to this player.
        /// </summary>
        public ScriptPlane Vehicle { get; protected set; }

        /// <summary>
        /// Player info.
        /// </summary>
        public PlayerInfo Info { get; private set; }

        /// <summary>
        /// Active target (AI only)
        /// </summary>
        public Player ActiveTarget { get; private set; }

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

        public event GamePlayerEventHandler OnDead, OnAlive;

        public Player(ScriptThread thread) : base(thread)
        { }

        public Player InitializeFrom(PlayerInfo info)
        {
            Info = info;
            Name = info.Name;
            return this;
        }

        public virtual void Create(LevelSpawn spawnPoint)
        {
            Info.Sess.State = PlayerState.Respawning;
        }

        /// <summary>
        /// Method to be called after creating the entity for the first time to setup extensions.
        /// </summary>
        public virtual void SetupExtensions()
        {
            CreateExtension(new RespawnManager(BaseThread, this));
            CreateExtension(new LandingGearManager(BaseThread, this));
        }

        /// <summary>
        /// Pool of extensions attached to this player.
        /// </summary>
        public PlayerExtensionPool Extensions { get; } = new PlayerExtensionPool();

        /// <summary>
        /// Get a script extension attached to this player.
        /// </summary>
        /// <param name="extension"></param>
        public void CreateExtension(PlayerExtensionBase extension)
        {
            if (!Extensions.Contains(extension))
            {
                Extensions.Add(extension);
            }
        }

        /// <summary>
        /// Get a script extension attached to this player.
        /// </summary>
        public T GetExtension<T>() where T : PlayerExtensionBase, new()
        {
            return Extensions.Get<T>();
        }

        /// <summary>
        /// Assign a <see cref="ScriptPed"/> and <see cref="ScriptPlane"/> instance to this player.
        /// </summary>
        /// <param name="ped"></param>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public void Manage(Ped ped, Vehicle vehicle)
        {
            DisposePedAndVehicle();    

            Ped = new ScriptPed(BaseThread, ped);

            Vehicle = new ScriptPlane(BaseThread, vehicle);

            // Aggregate dead/ alive events into a single event handler...

            Vehicle.Alive += OnPlayerAlive;

            Vehicle.Undrivable += OnPlayerDead;

            Ped.ExitVehicle += OnPlayerDead;
        }

        /// <summary>
        /// Fight against the specifed opponent.
        /// </summary>
        /// <param name="opponent"></param>
        public void FightAgainst(Player opponent)
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
            Info.Sess.TeamNum = teamIdx;
        }

        /// <summary>
        /// Register a specified amount of kills.
        /// </summary>
        /// <param name="count"></param>
        public void RegisterKills(int count)
        {
            Info.Sess.Stats.TotalKills += count;
        }

        /// <summary>
        /// Register a specified amount of deaths.
        /// </summary>
        /// <param name="count"></param>
        public void RegisterDeaths(int count)
        {
            Info.Sess.Stats.TotalDeaths += count;
        }

        /// <summary>
        /// Persue the target.
        /// </summary>
        /// <param name="target"></param>
        public void PersueTarget(Player target)
        {
            Ped ped = target.Ped.Ref;

            Vector3 position = ped.Position;

            Function.Call(Hash.TASK_PLANE_MISSION,
                Ped.Ref.Handle,
                Vehicle.Ref.Handle,
                target.Vehicle.Ref,
                ped.Handle,
                position.X, position.Y, position.Z,
                (int) VehicleTaskType.CTaskVehicleAttack, 70.0, -1.0, 40.0f, 500, 100);

            ActiveTarget = target;
        }

       /// <summary>
       /// Clear the active target.
       /// </summary>
        public void ClearActiveTarget()
        {
            ActiveTarget = null;
        }

        /// <summary>
        /// Fired when the player has spawned.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnPlayerAlive(IScriptEntity sender, ScriptEntityEventArgs args)
        {
            Info.Sess.LastSpawnedTime = args.GameTime;

            Info.Sess.State = PlayerState.Playing;

            OnAlive?.Invoke(this, args);
        }

        /// <summary>
        /// Fired when the player has died.
        /// </summary>
        /// <param name="e"></param>
        private void OnPlayerDead(IScriptEntity sender, ScriptEntityEventArgs args)
        {
            OnDead?.Invoke(this, args);
        }

        /// <summary>
        /// Clear all active extensions from the player.
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
        /// Removes the ped and vehicle from the world.
        /// </summary>
        public void DisposePedAndVehicle()
        {
            Ped?.Dispose();
            Vehicle?.Dispose();
        }

        public void Remove()
        {
            Ped?.Remove();
            Vehicle?.Remove();
        }

        public override void Dispose()
        {
            ClearExtensions();

            DisposePedAndVehicle();

            Remove();

            base.Dispose();
        }
    }
}
