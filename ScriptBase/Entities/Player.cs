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
            Name = Info.Name;   
            return this;
        }

        public virtual void Create()
        {
            ActiveTarget = null;
            Info.Sess.State = PlayerState.Playing;      
        }

        /// <summary>
        /// Method to be called after creating the entity for the first time to setup extensions.
        /// </summary>
        public virtual void SetupExtensions()
        {
            GetOrCreateExtension<IRFlareManager>();
            GetOrCreateExtension<EngineExtinguisher>();
            GetOrCreateExtension<RespawnManager>();
            GetOrCreateExtension<LandingGearManager>();
        }

        /// <summary>
        /// Pool of extensions attached to this player.
        /// </summary>
        public PlayerExtensionPool Extensions { get; } = new PlayerExtensionPool();

        /// <summary>
        /// Get a script extension attached to this player.
        /// </summary>
        public T GetExtension<T>() where T : PlayerExtensionBase
        {
            return Extensions.Get<T>();
        }

        /// <summary>
        /// Add a new script extension for this player.
        /// </summary>
        /// <param name="extension"></param>
        public void AddExtension(PlayerExtensionBase item)
        {
            if (!Extensions.Contains(item))
            {
                Extensions.Add(item);
            }
        }

        /// <summary>
        /// Get a script extension attached to this player or create it if it doesn't exit.
        /// </summary>
        public T GetOrCreateExtension<T>() where T : PlayerExtensionBase
        {
            var item = GetExtension<T>();

            if (item == null)
            {
                item = (T)Activator.CreateInstance(typeof(T), BaseThread, this);

                AddExtension(item);
            }

            return item;
        }

        /// <summary>
        /// Assign a <see cref="ScriptPed"/> and <see cref="ScriptPlane"/> instance to this player.
        /// (Should be called after setting up instances for each)
        /// </summary>
        /// <param name="ped"></param>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        protected void Manage(Ped ped, Vehicle vehicle)
        {
            DisposePedAndVehicle();    

            Ped = new ScriptPed(BaseThread, ped);

            Vehicle = new ScriptPlane(BaseThread, vehicle);

            // Aggregate dead/ alive events into a single event handler...

            Vehicle.EnterWater += OnEnterWater;

            Vehicle.Alive += OnPlayerAlive;

            Vehicle.Undrivable += OnPlayerDead;

            Ped.ExitVehicle += OnPlayerDead;

            SetupExtensions();
        }

        private void OnEnterWater(IScriptEntity sender, ScriptEntityEventArgs args)
        {
            var position = Position;

            if (Vehicle.Ref.Velocity.Length() > 30.0f)
            {
             
                Function.Call(Hash.START_PARTICLE_FX_NON_LOOPED_AT_COORD, "scr_ojdg4_water_exp", position.X, position.Y, position.Z, 0.0, 0.0, 0.0, 3.0, 0, 0, 0);
            }

            Function.Call(Hash.ADD_EXPLOSION, position.X, position.Y, position.Z, (int)ExplosionType.Valkyrie, 10.0f, true, true, 1.4f);
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
         /*   if (Function.Call<bool>(Hash.GET_IS_TASK_ACTIVE, Ped.Ref, 484))
            {
                Ped.Ref.Task.ClearAll();
            }*/

            Ped ped = target.Ped.Ref;

            Vector3 position = ped.Position;

            Function.Call(Hash.TASK_PLANE_MISSION,
                Ped.Ref.Handle,
                Vehicle.Ref.Handle,
                target.Vehicle.Ref,
                ped.Handle,
                position.X, position.Y, position.Z,
                (int) VehicleTaskType.CTaskVehicleAttack, 320f, -1.0f, 200.0, 500, 50);

            Ped.Ref.AlwaysKeepTask = true;

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
            Info.Sess.State = PlayerState.Dead;
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
