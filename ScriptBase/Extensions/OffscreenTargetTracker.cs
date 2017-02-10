using System;
using System.Drawing;
using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Helpers;
using AirSuperiority.ScriptBase.Logic;
using GTA;
using GTA.Native;
using GTA.Math;
using Player = AirSuperiority.ScriptBase.Entities.Player;

namespace AirSuperiority.ScriptBase.Extensions
{
    public class OffscreenTargetTracker : PlayerExtensionBase
    {
        private Sprite sprite;

        private TrackerTarget target;

        private SessionManager sessionMgr;

        private int changeTargetTime = 0;

        class TrackerTarget
        {
            public Player Player;
            public float Distance;
          
            public TrackerTarget(Player player, float distance)
            {
                Player = player;
                Distance = distance;
            }
        }

        public OffscreenTargetTracker(Player player) : base(player)
        {
            sessionMgr = ScriptThread.GetOrAddExtension<SessionManager>();
            Vector3 txdRes = Function.Call<Vector3>(Hash.GET_TEXTURE_RESOLUTION, "basejumping", "Arrow_Pointer");
            var xPos = (int) ((Game.ScreenResolution.Width + txdRes.X) / 2);
            sprite = new Sprite("basejumping", "Arrow_Pointer", new Point(xPos + 50, 40), new Size((int)txdRes.X, (int)txdRes.Y));
        }

        public override void OnUpdate(int gameTime)
        {
            base.OnUpdate(gameTime);

            if (Game.GameTime - changeTargetTime > 15000)
            {
                target = null;

                var z = Player.Position.Z;

                for (int x = 1; x < sessionMgr.Current.NumPlayers; x++)
                {
                    var otherPlayer = sessionMgr.Current.Players[x];

                    if (Player == otherPlayer.PlayerRef || Player.Info.Sess.TeamNum == otherPlayer.TeamIdx) continue;

                    if (Player.Vehicle.Ref.IsOnScreen)
                    {
                        target = null;
                        break;
                    }

                    float dist = Player.Position.DistanceTo(otherPlayer.PlayerRef.Position);

                    if (dist < 1000.0f && Math.Abs(z - otherPlayer.PlayerRef.Position.Z) < 10.0f && !otherPlayer.PlayerRef.Vehicle.Ref.IsDamaged)
                    {
                        if (target == null || dist < target.Distance)
                        {
                            target = new TrackerTarget(otherPlayer.PlayerRef, dist);

                            changeTargetTime = Game.GameTime;

                         /*   if (Function.Call<bool>(Hash.IS_PLAYER_TARGETTING_ENTITY, Game.Player.Handle, otherPlayer.PlayerRef.Vehicle.Ref))
                            {
                                break;
                            }*/
                        }
                    }
                }
            }

            if (target != null && target.Player.Vehicle.Ref.IsAlive && !target.Player.Vehicle.Ref.IsOnScreen)
            {
                DrawTracker(target.Player);
            }    
        }

        private unsafe void DrawTracker(Player target)
        {
            float heading = GameplayCamera.Position.HeadingTo(target.Position);
            sprite.Heading = heading;
            sprite.Draw();
        }
    }
}
