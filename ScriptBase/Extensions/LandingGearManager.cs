using AirSuperiority.Core;
using GTA;
using Player = AirSuperiority.ScriptBase.Entities.Player;

namespace AirSuperiority.ScriptBase.Extensions
{
    /// <summary>
    /// An extension that retracts the planes landing gear on spawning.
    /// </summary>
    public class LandingGearManager : PlayerExtensionBase
    {
        private const int TotalWaitTime = 1600;

        private int gearTriggerTime = 0;

        private bool waitActive = false;

        public LandingGearManager(ScriptThread thread, Player player) : base(thread, player)
        { }

        public override void OnPlayerAttached(Player player)
        {
            player.OnAlive += Entity_Alive;

            base.OnPlayerAttached(player);
        }

        private void Entity_Alive(object sender, System.EventArgs args)
        {
            gearTriggerTime = Game.GameTime + TotalWaitTime;

            waitActive = true;
        }

        public override void OnUpdate(int gameTime)
        {
            if (waitActive && gameTime > gearTriggerTime)
            {
                Player.Vehicle.LandingGearState = LandingGearState.Closing;

                waitActive = false;
            }

            base.OnUpdate(gameTime);
        }
    }
}
