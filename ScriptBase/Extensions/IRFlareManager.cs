using System.Collections.Generic;
using System.Linq;
using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Helpers;
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
        public int MaxDropTime { get; set; } = 3000;

        /// <summary>
        /// Frequency of flare drops (ms). <b>Default =</b> 250
        /// </summary>
        public int FlareDropInterval { get; set; } = 263;


        private List<IRFlareSequence> activeFlareSequence;

        public IRFlareManager(ScriptThread thread, Player player) : base(thread, player)
        { }

        public override void OnPlayerAttached(Player entity)
        {
            if (activeFlareSequence != null)
            {
                activeFlareSequence.Clear();
            }
            else
            {
                activeFlareSequence = new List<IRFlareSequence>();
            }

            activeFlareSequence.Add(new IRFlareSequence(Player.Vehicle.Ref, FlareDropInterval, MaxDropTime, new Vector3(0, 0.6f, -2.0f), IRFlareSequence.ForceType.Left, 15f));
            activeFlareSequence.Add(new IRFlareSequence(Player.Vehicle.Ref, FlareDropInterval, MaxDropTime, new Vector3(0, -0.6f, -2.0f), IRFlareSequence.ForceType.Right, 15f));
            activeFlareSequence.Add(new IRFlareSequence(Player.Vehicle.Ref, FlareDropInterval, MaxDropTime, new Vector3(0, 0.4f, -2.0f), IRFlareSequence.ForceType.Left, 9f));
            activeFlareSequence.Add(new IRFlareSequence(Player.Vehicle.Ref, FlareDropInterval, MaxDropTime, new Vector3(0, -0.4f, -2.0f), IRFlareSequence.ForceType.Right, 9f));
            activeFlareSequence.Add(new IRFlareSequence(Player.Vehicle.Ref, FlareDropInterval, MaxDropTime, new Vector3(0, 0.0f, -2.0f), IRFlareSequence.ForceType.Down, 2.5f));

            base.OnPlayerAttached(entity);
        }

        public override void OnUpdate(int gameTime)
        {
            if (activeFlareSequence != null)
            {
                activeFlareSequence.ForEach(x => x.Update(gameTime));
            }

            base.OnUpdate(gameTime);
        }

        /// <summary>
        /// Start flare sequence.
        /// </summary>
        public void Start()
        {
            RemoveAll();
            activeFlareSequence?.ForEach(x => x.Start());
        }

        /// <summary>
        /// Remove all flares.
        /// </summary>
        public void RemoveAll()
        {
            activeFlareSequence?.ForEach(x => x.RemoveAll());
        }

        public override void Dispose()
        {
            RemoveAll();

            base.Dispose();
        }
    }

    public class IRFlareSequence
    {
        private bool active;
        private Vehicle baseVehicle;
        private int timeout, dropTimeout;
        private int dropInterval, maxDropTime;
        private Vector3 dropOffset;
        private ForceType forceType;
        private float forceMultiplier;

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
        /// Initialize the class
        /// </summary>
        /// <param name="baseVehicle">Target vehicle</param>
        /// <param name="dropInterval">Interval between flares</param>
        /// <param name="maxDropTime">Max time to drop flares</param>
        /// <param name="dropOffset">Spawn offset relative to base vehicle</param>
        /// <param name="forceType">The type of directional force to apply on spawn</param>
        /// <param name="forceMultiplier">Force multiplier</param>
        public IRFlareSequence(Vehicle baseVehicle, int dropInterval, int maxDropTime, Vector3 dropOffset, ForceType forceType, float forceMultiplier)
        {
            this.baseVehicle = baseVehicle;
            this.dropInterval = dropInterval;
            this.maxDropTime = maxDropTime;
            this.dropOffset = dropOffset;
            this.forceType = forceType;
            this.forceMultiplier = forceMultiplier;
        }

        /// <summary>
        /// Start the flare drop sequence
        /// </summary>
        public void Start()
        {
            RemoveAll();
            timeout = Game.GameTime + dropInterval;
            dropTimeout = Game.GameTime + maxDropTime;
            active = true;
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
        public void Update(int gameTime)
        {
            if (activeFlares.Count > 0)
            {
                var nearbyProjectiles = World.GetNearbyEntities(baseVehicle.Position - baseVehicle.ForwardVector * 50, 50f)
                    .Where(x => x.Model.Hash == -1707997257);

                foreach (var proj in nearbyProjectiles)
                {
                    proj.ApplyForce(-proj.RightVector * 5);
                }

                for (int i = 0; i < activeFlares.Count; i++)
                {
                    bool exists;
                    activeFlares[i].Update(gameTime, out exists);

                    if (!exists)
                    {

                        activeFlares.RemoveAt(i);
                    }
                }
            }

            if (active)
            {
                if (gameTime > dropTimeout)
                {
                    active = false;
                    return;
                }

                if (gameTime > timeout)
                {
                    var rot = new Vector3(80.0f, 0.0f, baseVehicle.Heading + 180.0f);
                    var newFlare = new IRFlare(baseVehicle.GetOffsetInWorldCoords(dropOffset), rot);

                    Vector3 force = forceType == ForceType.Down ? -baseVehicle.UpVector :
                        forceType == ForceType.Left ? -baseVehicle.RightVector :
                        forceType == ForceType.Right ? baseVehicle.RightVector :
                        forceType == ForceType.Up ? baseVehicle.UpVector : Vector3.Zero;

                    newFlare.Velocity = activeFlares.Count < 1 ?
                        baseVehicle.Velocity + new Vector3(0f, 0f, -10f) :
                        activeFlares[activeFlares.Count - 1].Velocity;

                    newFlare.ApplyForce(force * forceMultiplier);

                    newFlare.StartFX();

                    activeFlares.Add(newFlare);

                    timeout = gameTime + dropInterval;
                }
            }
        }

        public enum ForceType
        {
            Up,
            Down,
            Left,
            Right
        }
    }

    public class IRFlare : Entity
    {
        private int tickTime;
        private bool timerActive;
        private LoopedParticle ptfx;

        /// <summary>
        /// Max time the entity will be active before fading fx
        /// </summary>
        private const int MaxAliveTime = 500;

        /// <summary>
        /// Initialize the class
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public IRFlare(Vector3 position, Vector3 rotation) : base(Create(position, rotation))
        {
            ptfx = new LoopedParticle("core", "proj_flare_fuse");
        }

        /// <summary>
        /// Create the flare at the given position with rotation
        /// </summary>
        /// <param name="position">Spawn position</param>
        /// <param name="rotation">Spawn rotation</param>
        /// <returns></returns>
        private static int Create(Vector3 position, Vector3 rotation)
        {
            var model = new Model("prop_flare_01b");

            if (!model.IsLoaded)
                model.Request(1000);

            var flare = World.CreateProp(model, position, false, false);

            Function.Call(Hash.SET_ENTITY_RECORDS_COLLISIONS, flare.Handle, true);
            Function.Call(Hash.SET_ENTITY_LOAD_COLLISION_FLAG, flare.Handle, true);
            Function.Call(Hash.SET_ENTITY_LOD_DIST, flare.Handle, 1000);
            Function.Call(Hash.SET_OBJECT_PHYSICS_PARAMS, flare.Handle, -1.0f, -1.0f, -1.0f, -1.0f, 0.009888f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f, -1.0f);
            flare.Rotation = rotation;

            return flare.Handle;
        }

        /// <summary>
        /// Start particle fx on the entity
        /// </summary>
        public void StartFX()
        {
            if (!ptfx.IsLoaded)
                ptfx.Load();

            ptfx.Start(this, 4f);

            tickTime = Game.GameTime + MaxAliveTime;
            timerActive = true;
        }

        /// <summary>
        /// Remove particle fx and delete the entity
        /// </summary>
        public void Remove()
        {
            ptfx.Remove();
            Delete();
        }

        /// <summary>
        /// Update the class
        /// </summary>
        /// <param name="exists"></param>
        public void Update(int gameTime, out bool exists)
        {
            if (timerActive && gameTime > tickTime)
            {
                if (ptfx.Exists && ptfx.Scale > 0.0f)
                {
                    ptfx.Scale -= 0.1f;
                    exists = true;
                }

                else
                {
                    timerActive = false;
                    exists = false;
                    Remove();
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
