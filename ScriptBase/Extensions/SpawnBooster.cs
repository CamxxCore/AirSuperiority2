using AirSuperiority.Core;
using GTA;

namespace AirSuperiority.ScriptBase.Extensions
{
    public class SpawnBooster : ScriptEntityExtension<Vehicle>
    {
        private const int TotalBoostTime = 5000;

        private int boostEndTime = 0;

        private bool boostActive = false;

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
            boostEndTime = Game.GameTime + TotalBoostTime;

            boostActive = true;
        }

        public override void OnUpdate()
        {
            if (boostActive)
            {
                if (Game.GameTime >= boostEndTime)
                {
                    boostActive = false;
                }

                else
                {
                    Entity.Ref.ApplyForce(Entity.Ref.ForwardVector * 2f);
                }
            }

            base.OnUpdate();
        }
    }
}
