using GTA;
using GTA.Math;
using GTA.Native;
using Player = AirSuperiority.ScriptBase.Entities.Player;

namespace AirSuperiority.ScriptBase.Extensions
{
    /// <summary>
    /// An extension that lerps the camera frame from a set position behind the vehicle when it spawns into the world.
    /// </summary>
    public class SpawnLerpingCamera : PlayerExtensionBase
    {
        private Camera mainCamera = World.CreateCamera(Vector3.Zero, new Vector3(), 50f);

        public Camera MainCamera { get { return mainCamera; } }

        public bool Active
        {
            get { return mainCamera.IsActive; }
        }

        public SpawnLerpingCamera(Player player) : base(player)
        { }

        public override void OnPlayerAttached(Player player)
        {
            player.OnAlive += OnEntityAlive;

            base.OnPlayerAttached(player);
        }

        /// <summary>
        /// Event to fire when the entity is alive again, at which point we want to start interpolating the camera..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnEntityAlive(object sender, System.EventArgs args)
        {
            mainCamera.Position = Player.Vehicle.Ref.GetOffsetInWorldCoords(new Vector3(-2f, -75f, 110f));

            mainCamera.Rotation = Player.Vehicle.Ref.Rotation + new Vector3(-60, 0, 0);

            Start();
        }

        public override void OnUpdate(int gameTime)
        {
            if (World.RenderingCamera.Handle == mainCamera.Handle)
            {
                Game.DisableAllControlsThisFrame(0);
            }

            base.OnUpdate(gameTime);
        }

        /// <summary>
        /// Change camera view, start spline sequence.
        /// </summary>
        public void Start()
        {
            mainCamera.IsActive = true;
            World.RenderingCamera = mainCamera;
            Function.Call(Hash.RENDER_SCRIPT_CAMS, 0, 1, 3000, 0, 0);
        }

        /// <summary>
        /// Stop sequence, switch to main camera view.
        /// </summary>
        public void Stop()
        {
            mainCamera.IsActive = false;

            World.RenderingCamera = null;
        }

        /// <summary>
        /// Destroy all cameras
        /// </summary>
        public void Destroy()
        {
            mainCamera.Destroy();
            mainCamera = null;
        }

        public override void Dispose()
        {
            Destroy();
            base.Dispose();
        }
    }
}


