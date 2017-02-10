using GTA;
using Player = AirSuperiority.ScriptBase.Entities.Player;

namespace AirSuperiority.ScriptBase.Extensions
{
    /// <summary>
    /// An extension that boosts the forward velocity of the vehicle on spawning.
    /// </summary>
    public class SpawnVelocityBooster : PlayerExtensionBase
    {
        private const int TotalBoostTime = 4000;

        private int boostEndTime = 0;

        private bool boostActive = false;

        public SpawnVelocityBooster(Player player) : base(player)
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
            boostEndTime = Game.GameTime + TotalBoostTime;

            boostActive = true;
        }

        public override void OnUpdate(int gameTime)
        {
            if (boostActive)
            {
                if (gameTime >= boostEndTime)
                {
                    boostActive = false;
                }

                else
                {
                    Player.Vehicle.Ref.ApplyForce(Player.Vehicle.Ref.ForwardVector * 0.524f);
                    Player.Vehicle.Ref.ApplyForce((Player.Vehicle.Ref.ForwardVector + Player.Vehicle.Ref.UpVector) * 0.224f);
                }
            }

            base.OnUpdate(gameTime);
        }
    }
}
