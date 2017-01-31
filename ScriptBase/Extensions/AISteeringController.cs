using System;
using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Types;
using AirSuperiority.ScriptBase.Logic;
using AirSuperiority.ScriptBase.Helpers;
using GTA;
using GTA.Math;
using GTA.Native;
using Player = AirSuperiority.ScriptBase.Entities.Player;

namespace AirSuperiority.ScriptBase.Extensions
{
    /// <summary>
    /// Fighter jet engine extinguisher extension.
    /// </summary>
    public class AISteeringController : PlayerExtensionBase
    {
        Vector3 steerBias;

        private int steerBiasEndTime = 0;

        private int targettedWaitTime = 0;

        private SessionManager sessionMgr;

        public AISteeringController(ScriptThread thread, Player player) : base(thread, player)
        {
            sessionMgr = thread.Get<SessionManager>();
        }

        public void SetSteerBias(Vector3 direction, int duration)
        {
            steerBias = direction;
            steerBiasEndTime = Game.GameTime + duration;
        }

        public override void OnPlayerAttached(Player player)
        {
            Function.Call(Hash.STEER_UNLOCK_BIAS, player.Vehicle.Ref, true);

            base.OnPlayerAttached(player);
        }

        public override void OnUpdate(int gameTime)
        {
            if (Player.Vehicle.Ref.IsDamaged) return;

            if (gameTime < steerBiasEndTime)
            {
                if (Player.Vehicle.Ref.Velocity.Length() < 80.0f)
                {
                    // ScriptMain.DebugPrint("set steer bias for " + Player.Name + " " + Player.Vehicle.Ref.Velocity.Length());

                    Player.Vehicle.Ref.ApplyForce(steerBias * 3.7f, new Vector3(Probability.GetBoolean(60.0f) ? 1.6f : -1.6f, 0, Probability.GetBoolean(50.0f) ? 1.0f : 0.0f));
                }
            }
            else
            {
                var myHeading = Player.Vehicle.Ref.Heading;

                for (int x = 0; x < sessionMgr.Current.NumPlayers; x++)
                {
                    SessionPlayer otherPlayer = sessionMgr.Current.Players[x];

                    if (Player == otherPlayer.EntityRef || Player.Info.Sess.TeamNum == otherPlayer.TeamIdx) continue;

                    if (Player.Position.DistanceTo(otherPlayer.EntityRef.Position) < 350.0f ||
                        Player.Vehicle.Ref.HeightAboveGround > 20.0f &&
                         Probability.GetBoolean(0.7998f))
                    {
                        var otherHeading = otherPlayer.EntityRef.Vehicle.Ref.Heading;

                        if (Utility.IsBetween(otherHeading, myHeading - 5.0f, myHeading + 5.0f))
                        {
                            var direction = Vector3.Normalize(otherPlayer.EntityRef.Position - Player.Position);

                            var dot = Vector3.Dot(direction, Player.Vehicle.Ref.ForwardVector);

                            if (dot < -0.1f)
                            {
                                targettedWaitTime = gameTime + new Random().Next(800, 2000);
                            }
                        }
                    }
                }

                /* if (Function.Call<bool>(Hash.IS_PLAYER_TARGETTING_ENTITY, Game.Player.Handle, Player.Vehicle.Ref) && 
                         Player.Vehicle.Ref.HeightAboveGround > 20.0f && 
                         Helpers.Probability.GetBoolean(0.7998f))
                     {
                         targettedWaitTime = gameTime + new Random().Next(800, 2000);
                     }*/

                if (gameTime > targettedWaitTime)
                {
                    Vector3 dir = Player.Vehicle.Ref.UpVector;

                    if (Probability.GetBoolean(0.50f))
                    {
                        dir = -dir;
                    }

                    SetSteerBias(dir, new Random().Next(800, 12000));
                }
            }

            base.OnUpdate(gameTime);
        }
    }
}
