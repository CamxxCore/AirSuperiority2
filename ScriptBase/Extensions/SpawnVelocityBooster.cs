using AirSuperiority.Core;
using GTA;
using Player = AirSuperiority.ScriptBase.Entities.Player;

namespace AirSuperiority.ScriptBase.Extensions
{
    /// <summary>
    /// An extension that boosts the forward velocity of the vehicle on spawning.
    /// </summary>
    public class SpawnVelocityBooster : PlayerExtensionBase
    {
        private const int TotalBoostTime = 5000;

        private int boostEndTime = 0;

        private bool boostActive = false;

        public SpawnVelocityBooster(ScriptThread thread, Player player) : base(thread, player)
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
                    Player.Vehicle.Ref.ApplyForce(Player.Vehicle.Ref.ForwardVector * 2f);
                }
            }

            base.OnUpdate(gameTime);
        }
    }
}
