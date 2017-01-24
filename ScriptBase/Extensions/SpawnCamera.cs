using AirSuperiority.Core;
using GTA;
using GTA.Math;
using GTA.Native;

namespace AirSuperiority.ScriptBase.Extensions
{
    public class SpawnCamera : ScriptEntityExtension<Vehicle>
    {
        private Camera mainCamera = World.CreateCamera(Vector3.Zero, new Vector3(), 50f);

        public Camera MainCamera { get { return mainCamera; } }

        public bool Active
        {
            get { return mainCamera.IsActive; }
        }

        public override void OnEntityAttached(ScriptEntity<Vehicle> entity)
        {
            entity.Alive += OnEntityAlive;

            base.OnEntityAttached(entity);
        }

        /// <summary>
        /// Event to fire when the entity is alive again, at which point we want to start interpolating the camera..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnEntityAlive(IScriptEntity sender, ScriptEntityEventArgs args)
        {
            mainCamera.Position = Entity.Ref.GetOffsetInWorldCoords(new Vector3(-2f, -2f, 36f));

            mainCamera.Rotation = Entity.Ref.Rotation + new Vector3(-90, 0, 0);

            Start();
        }

        public override void OnUpdate()
        {          
            base.OnUpdate();
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


