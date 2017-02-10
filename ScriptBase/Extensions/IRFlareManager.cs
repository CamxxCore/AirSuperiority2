using System;
using System.Collections.Generic;
using System.Linq;
using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Entities;
using AirSuperiority.ScriptBase.Helpers;
using AirSuperiority.ScriptBase.Logic;
using GTA.Math;
using GTA.Native;
using GTA;
using Player = AirSuperiority.ScriptBase.Entities.Player;

namespace AirSuperiority.ScriptBase.Extensions
{
    /// <summary>
    /// An extension that adds IR countermeasures for planes.
    /// </summary>
    public class IRFlareManager : PlayerExtensionBase
    {
        /// <summary>
        /// Max time for dropping flares (ms). <b>Default =</b> 5200
        /// </summary>
        public int MaxDropTime { get; set; } = 4200;

        /// <summary>
        /// Max time for cooldown (ms). <b>Default =</b> 3100
        /// </summary>
        public int CooldownTime { get; set; } = 22000;

        /// <summary>
        /// Frequency of flare drops (ms). <b>Default =</b> 250
        /// </summary>
        public int FlareDropInterval { get; set; } = 100;

        private Vehicle vehicle;

        private List<IRFlareSequence> sequenceList =
            new List<IRFlareSequence>();

        private int timeSinceLastDrop = 0;

        private bool bCooldownActive = false;

        public bool CooldownActive { get { return bCooldownActive; } }

        private SequenceFlags sqFlags = SequenceFlags.NotRunning;

        /// <summary>
        /// If any sequence is running.
        /// </summary>
        public bool AnySequenceActive
        {
            get
            {
                return sqFlags.HasFlag(SequenceFlags.Active);
            }
        }

        public IRFlareManager(Player player) : base(player)
        { }

        public override void OnPlayerAttached(Player player)
        {
            player.OnAlive += Player_OnAlive;

            base.OnPlayerAttached(player);
        }

        private void Player_OnAlive(Player sender, EventArgs e)
        {
            sequenceList.ForEach(x => x.RemoveAll());

            sequenceList.Clear();

            SetupWithVehicle(sender.Vehicle.Ref);
        }

        /// <summary>
        /// Initializes the class for the given vehicle.
        /// </summary>
        /// <param name="vehicle">Target vehicle</param>
        /// <returns></returns>
        public IRFlareManager SetupWithVehicle(Vehicle vehicle)
        {
            this.vehicle = vehicle;
            AddFlareSequence(new Vector3(0.0f, -0.3f, -2f), ForceType.Left, 11f, true);
            AddFlareSequence(new Vector3(0.0f, -0.3f, -2f), ForceType.Right, 11f, true);
            AddFlareSequence(new Vector3(0.0f, -0.3f, -2f), ForceType.Left, 7.8f, true);
            AddFlareSequence(new Vector3(0.0f, -0.3f, -2f), ForceType.Right, 7.8f, true);
            AddFlareSequence(new Vector3(0.0f, -0.3f, -2f), ForceType.Down, 6f, true);
            return this;
        }

        /// <summary>
        /// Add a new flare sequence with the given parameters
        /// </summary>
        /// <param name="dropOffset">The offset from the vehicle where the flares will be dropped.</param>
        /// <param name="force">The type of directional force to use when ejecting the flares.</param>
        /// <param name="forceMultiplier">The scale of forces to be used.</param>
        /// <param name="randomVel">Randomize ejection velocity to simulate drag/ environmental factors</param>
        private void AddFlareSequence(Vector3 dropOffset, ForceType force, float forceMultiplier, bool randomVel)
        {
            Vector3 min, max;
            vehicle.Model.GetDimensions(out min, out max);
            IRFlareSequence s = new IRFlareSequence(vehicle, FlareDropInterval, MaxDropTime, new Vector3(dropOffset.X, dropOffset.Y, min.Z), force, forceMultiplier, randomVel);
            s.OnCompleted += OnFlareSequenceCompleted;
            sequenceList.Add(s);
        }

        /// <summary>
        /// Runs when an active flare sequence has completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnFlareSequenceCompleted(object sender, EventArgs args)
        {
            for (int i = sequenceList.Count; i > -1; i--)
            {
                SequenceFlags flag = (SequenceFlags)(1 << i);

                if (sqFlags.HasFlag(flag))
                {
                    sqFlags &= ~flag;

                    break;
                }
            }

            if (!AnySequenceActive && Player is LocalPlayer)
            {
                UI.Notify("IR flares recharging.");
            }
        }

        public override void OnUpdate(int gameTime)
        {
            sequenceList.ForEach(x => x.Update());

            if (bCooldownActive && Game.GameTime - timeSinceLastDrop > MaxDropTime + CooldownTime)
            {
                if (Player is LocalPlayer)
                {
                    UI.Notify("IR flares available.");

                    var soundMgr = ScriptThread.GetOrAddExtension<SoundManager>();

                    soundMgr.PlayExternalSound(Properties.Resources.flares_equip);
                }

                bCooldownActive = false;
            }

            if (AnySequenceActive)
            {
                var projectileEntities = World.GetNearbyEntities(vehicle.Position - vehicle.ForwardVector * 55, 53.0f)
                    .Where(x => (uint)x.Model.Hash == 0x9A3207B7); // 2586970039

                foreach (var projectile in projectileEntities)
                {
                    projectile.ApplyForce(projectile.RightVector * 5);
                }
            }

            base.OnUpdate(gameTime);
        }

        /// <summary>
        /// Start flare sequence.
        /// </summary>
        public void Start()
        {
            if (Game.GameTime - timeSinceLastDrop > MaxDropTime + CooldownTime)
            {
                RemoveAll();

                for (int i = 0; i < sequenceList.Count; i++)
                {
                    sequenceList[i].Start();

                    sqFlags |= (SequenceFlags)(1 << i);
                }

                if (Player is LocalPlayer)
                {
                    var soundMgr = ScriptThread.GetOrAddExtension<SoundManager>();

                    soundMgr.PlayExternalSound(Properties.Resources.flares_equip1);
                }

                timeSinceLastDrop = Game.GameTime;

                bCooldownActive = true;
            }
        }

        /// <summary>
        /// Remove all flares.
        /// </summary>
        public void RemoveAll()
        {
            sequenceList.ForEach(x => x.RemoveAll());
        }

        public override void Dispose()
        {
            RemoveAll();

            base.Dispose();
        }

        [Flags]
        enum SequenceFlags
        {
            NotRunning = 0,
            Active = 1
        }
    }

    public enum ForceType
    {
        Up,
        Down,
        Left,
        Right
    }

    public sealed class IRFlareSequence
    {
        private bool dropActive;
        private Vehicle baseVehicle;
        private int intervalTimeout, dropTimeout;
        private int dropInterval, maxDropTime;
        private Vector3 dropOffset;
        private ForceType forceType;
        private float forceMultiplier;
        private bool randomizeVel;
        private bool cleanupCompleted = true;

        public event EventHandler OnCompleted;

        private List<IRFlare> activeFlares = new List<IRFlare>();

        /// <summary>
        /// Active flares collection
        /// </summary>
        public List<IRFlare> ActiveFlares { get { return activeFlares; } }

        /// <summary>
        /// Time between bomb drop (ms)
        /// </summary>
        public int DropInterval { get { return dropInterval; } }

        /// <summary>
        /// Whether the sequence has fully completed
        /// </summary>
        public bool Completed { get { return cleanupCompleted; } }

        /// <summary>
        /// Whether to randomize the velocity at dropping time
        /// </summary>
        public bool RandomDropVelocity { get { return randomizeVel; } }

        /// <summary>
        /// Initialize the class
        /// </summary>
        /// <param name="baseVehicle">Target vehicle</param>
        /// <param name="dropInterval">Interval between flares</param>
        /// <param name="maxDropTime">Max time to drop flares</param>
        /// <param name="dropOffset">Spawn offset relative to base vehicle</param>
        /// <param name="forceType">The type of directional force to apply on spawn</param>
        /// <param name="forceMultiplier">Force multiplier</param>
        public IRFlareSequence(Vehicle baseVehicle, int dropInterval, int maxDropTime, Vector3 dropOffset, ForceType forceType, float forceMultiplier, bool randomizeVel)
        {
            this.baseVehicle = baseVehicle;
            this.dropInterval = dropInterval;
            this.maxDropTime = maxDropTime;
            this.dropOffset = dropOffset;
            this.forceType = forceType;
            this.forceMultiplier = forceMultiplier;
            this.randomizeVel = randomizeVel;
        }

        private void OnSequenceCompleted(EventArgs args)
        {
            OnCompleted?.Invoke(this, args);
        }

        /// <summary>
        /// Start the flare drop sequence
        /// </summary>
        public void Start()
        {
            RemoveAll();
            intervalTimeout = Game.GameTime + dropInterval;
            dropTimeout = Game.GameTime + maxDropTime;
            cleanupCompleted = false;
            dropActive = true;
        }

        /// <summary>
        /// Remove all active flares
        /// </summary>
        public void RemoveAll()
        {
            if (activeFlares.Count > 0)
            {
                activeFlares.ForEach(x => x.Remove());
                activeFlares.Clear();
            }
        }

        /// <summary>
        /// Update the class
        /// </summary>
        public void Update()
        {
            if (!cleanupCompleted)
            {
                if (activeFlares.Count > 0)
                {
                    for (int i = 0; i < activeFlares.Count; i++)
                    {
                        bool exists;
                        activeFlares[i].Update(out exists);

                        if (!exists)
                        {
                            activeFlares.RemoveAt(i);

                            if (activeFlares.Count <= 0)
                            {
                                OnSequenceCompleted(new EventArgs());
                                cleanupCompleted = true;
                            }
                        }
                    }
                }
            }

            if (dropActive)
            {
                if (Game.GameTime > dropTimeout || !baseVehicle.IsDriveable)
                {
                    dropActive = false;
                    return;
                }

                if (Game.GameTime > intervalTimeout)
                {
                    var rotation = new Vector3(80.0f, 0.0f, baseVehicle.Heading + 180.0f);

                    var newFlare = new IRFlare(baseVehicle.GetOffsetInWorldCoords(dropOffset), rotation);

                    Vector3 force = forceType == ForceType.Down ? -baseVehicle.UpVector :
                        forceType == ForceType.Left ? -baseVehicle.RightVector + new Vector3(0, 0, -0.5f) :
                        forceType == ForceType.Right ? baseVehicle.RightVector + new Vector3(0, 0, -0.5f) :
                        forceType == ForceType.Up ? baseVehicle.UpVector : Vector3.Zero;

                    Vector3 velocity = activeFlares.Count < 1 ?
                           baseVehicle.Velocity + new Vector3(0f, 0f, -10f) :
                           activeFlares[activeFlares.Count - 1].Velocity;

                    if (randomizeVel)
                    {
                        var randItms = Enumerable.Range(0, 3000);

                        float randX = randItms.GetRandomItem() / 1000.0f;
                        float randY = randItms.GetRandomItem() / 1000.0f;
                        float randZ = randItms.GetRandomItem() / 1000.0f;

                        if (Probability.GetBoolean(0.50f))
                        {
                            randX = -randX;
                        }

                        if (Probability.GetBoolean(0.50f))
                        {
                            randY = -randY;
                        }

                        if (Probability.GetBoolean(0.50f))
                        {
                            randZ = -randZ;
                        }

                        var vForce = new Vector3(randX, randY, randZ);

                        velocity += vForce;
                    }

                    newFlare.Velocity = velocity;

                    newFlare.ApplyForce(force * forceMultiplier);

                    newFlare.StartFX();

                    activeFlares.Add(newFlare);

                    intervalTimeout = Game.GameTime + dropInterval + new Random().Next(dropInterval, dropInterval + 50);
                }
            }
        }
    }

    public class IRFlare : Entity
    {
        private int tickTime;
        private bool timerActive;
        private LoopedParticle ptfx;

        public IRFlare(Vector3 position, Vector3 rotation) : base(Create(position, rotation))
        {
            ptfx = new LoopedParticle("core", "proj_flare_fuse");
        }

        private static int Create(Vector3 position, Vector3 rotation)
        {
            var model = new Model("prop_flare_01b");

            if (!model.IsLoaded)
                model.Request(1000);

            var flare = World.CreateProp(model, position, false, false);

            Function.Call(Hash.SET_ENTITY_RECORDS_COLLISIONS, flare.Handle, true);
            Function.Call(Hash.SET_ENTITY_LOAD_COLLISION_FLAG, flare.Handle, true);
            Function.Call(Hash.SET_ENTITY_LOD_DIST, flare.Handle, 1000);
            Function.Call(Hash.SET_OBJECT_PHYSICS_PARAMS, flare.Handle, -1.0f, 1.2f, -1.0f, -1.0f, 0.010988f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f);

            flare.Rotation = rotation;

            return flare.Handle;
        }

        public void StartFX()
        {
            if (!ptfx.IsLoaded)
                ptfx.Load();

            ptfx.Start(new Prop(Handle), 2.4f);

            tickTime = Game.GameTime + 800;
            timerActive = true;
        }

        public void Remove()
        {
            ptfx.Remove();
            Delete();
        }

        public void Update(out bool exists)
        {
            if (timerActive && Game.GameTime > tickTime)
            {
                if (ptfx.Exists && ptfx.Scale > 0.0f)
                {
                    ptfx.Scale -= 0.01f;
                    exists = true;
                }

                else
                {
                    //remove..
                    Remove();
                    timerActive = false;
                    exists = false;
                }
            }

            else
            {
                //entity active
                exists = true;
            }
        }
    }
}
