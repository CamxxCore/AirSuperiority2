using System;
using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Helpers;
using AirSuperiority.ScriptBase.Logic;
using AirSuperiority.ScriptBase.Types;
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

        private bool setBias = false;

        public AISteeringController(Player player) : base(player)
        {
            sessionMgr = ScriptThread.GetOrAddExtension<SessionManager>();
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
     //       if (Player.Vehicle.Ref.IsDamaged || Player.Vehicle.Ref.Health < 900) return;

            if (gameTime < steerBiasEndTime)
            {
                if (Player.Vehicle.Ref.Velocity.Length() < 80.0f && Player.Vehicle.Ref.HeightAboveGround > 100.0F)
                {
                   // ScriptMain.DebugPrint("set steer bias for " + Player.Name + " " + Player.Vehicle.Ref.Velocity.Length());

                    Player.Vehicle.Ref.ApplyForce(steerBias * 2.7f, new Vector3(
                        Probability.GetBoolean(60.0f) ? 1.5f :
                        Probability.GetBoolean(50.0f) ? 1.0f : -1.5f,
                        Probability.GetBoolean(50.0f) ? 1.0f : 
                        Probability.GetBoolean(50.0f) ? 0.0f : -1.5f, 0));
                }
            }
            else if (Probability.GetBoolean(0.098f))
            {
                var myHeading = Player.Vehicle.Ref.Heading;

                for (int x = 0; x < sessionMgr.Current.NumPlayers; x++)
                {
                    SessionPlayer otherPlayer = sessionMgr.Current.Players[x];

                    if (Player != otherPlayer.PlayerRef &&
                        Player.Info.Sess.TeamNum != otherPlayer.TeamIdx &&
                        Player.Position.DistanceTo(otherPlayer.PlayerRef.Position) < 200.0f)
                    {
                        var otherHeading = otherPlayer.PlayerRef.Vehicle.Ref.Heading;

                        if (otherHeading.DistanceTo(myHeading) < 5.0f)
                        {
                            var dir = Vector3.Normalize(otherPlayer.PlayerRef.Position - Player.Position);

                            var dot = Vector3.Dot(dir, Player.Vehicle.Ref.ForwardVector);

                            if (dot < -0.1f)
                            {
                                targettedWaitTime = gameTime + new Random().Next(800, 2000);
                                setBias = true;
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

                if (setBias && gameTime > targettedWaitTime)
                {
                    Vector3 dir = Player.Vehicle.Ref.UpVector;

                    if (Probability.GetBoolean(0.50f))
                    {
                        dir = -dir;
                    }

                    SetSteerBias(dir, new Random().Next(2000, 9000));
                }
            }

            base.OnUpdate(gameTime);
        }
    }
}
