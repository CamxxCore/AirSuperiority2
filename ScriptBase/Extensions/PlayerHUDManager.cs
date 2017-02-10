using System;
using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Logic;
using Player = AirSuperiority.ScriptBase.Entities.Player;

namespace AirSuperiority.ScriptBase.Extensions
{
    public class PlayerHUDManager : PlayerExtensionBase
    {
        private SessionManager sessionMgr;

        public PlayerHUDManager(Player player) : base(player)
        {
            sessionMgr = ScriptThread.GetOrAddExtension<SessionManager>();
        }

        public override void OnUpdate(int gameTime)
        {
            for (int x = 1; x < sessionMgr.Current.NumPlayers; x++)
            {
                var otherPlayer = sessionMgr.Current.Players[x];
  
                if (Player == otherPlayer.PlayerRef || Player.Info.Sess.TeamNum == otherPlayer.TeamIdx) continue;
           

                /*    if (otherPlayer.PlayerRef.ActiveTarget == Player && Player.Position.DistanceTo(otherPlayer.PlayerRef.Position) < 200.0f)
                    {
                        var otherHeading = otherPlayer.PlayerRef.Vehicle.Ref.Heading;

                        if (Player.Vehicle.Ref.Heading > otherHeading - 8.0f || Player.Vehicle.Ref.Heading < otherHeading + 8.0f)
                        {
                            var direction = Vector3.Normalize(otherPlayer.PlayerRef.Position - Player.Position);

                            var dot = Vector3.Dot(direction, Player.Vehicle.Ref.ForwardVector);

                            if (dot < -0.6f)
                            {
                                displayMgr.ShowWarningThisFrame("ENEMY LOCK");
                            }           
                        }
                    }*/
            }

            base.OnUpdate(gameTime);
        }
    }
}
