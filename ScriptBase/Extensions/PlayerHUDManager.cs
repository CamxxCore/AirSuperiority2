using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Types;
using AirSuperiority.ScriptBase.Logic;
using AirSuperiority.ScriptBase.Entities;
using AirSuperiority.ScriptBase.Helpers;
using GTA.Math;
using GTA;
using Player = AirSuperiority.ScriptBase.Entities.Player;

namespace AirSuperiority.ScriptBase.Extensions
{
    public class PlayerHUDManager : PlayerExtensionBase
    {
        private DisplayManager displayMgr;

        private SessionManager sessionMgr;

        struct OffscreenTrackerTarget
        {

        }

        public PlayerHUDManager(ScriptThread thread, Player player) : base(thread, player)
        {
            displayMgr = thread.Get<DisplayManager>();
            sessionMgr = thread.Get<SessionManager>();
        }

        public override void OnUpdate(int gameTime)
        {
            for (int x = 1; x < sessionMgr.Current.NumPlayers; x++)
            {
                var otherPlayer = sessionMgr.Current.Players[x];
  
                if (Player == otherPlayer.EntityRef || Player.Info.Sess.TeamNum == otherPlayer.TeamIdx) continue;
           

                /*    if (otherPlayer.EntityRef.ActiveTarget == Player && Player.Position.DistanceTo(otherPlayer.EntityRef.Position) < 200.0f)
                    {
                        var otherHeading = otherPlayer.EntityRef.Vehicle.Ref.Heading;

                        if (Player.Vehicle.Ref.Heading > otherHeading - 8.0f || Player.Vehicle.Ref.Heading < otherHeading + 8.0f)
                        {
                            var direction = Vector3.Normalize(otherPlayer.EntityRef.Position - Player.Position);

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
