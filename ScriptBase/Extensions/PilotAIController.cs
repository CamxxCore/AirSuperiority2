using System;
using System.Linq;
using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Helpers;
using AirSuperiority.ScriptBase.Types;
using GTA;
using GTA.Math;
using GTA.Native;
using AirSuperiority.ScriptBase.Logic;
using AirSuperiority.ScriptBase.Entities;
using Player = AirSuperiority.ScriptBase.Entities.Player;

namespace AirSuperiority.ScriptBase.Extensions
{
    /// <summary>
    /// Fighter jet engine extinguisher extension.
    /// </summary>
    public class PilotAIController : PlayerExtensionBase
    {
        private AIState state;

        private SessionManager sessionMgr;

        private LevelManager levelMgr;

        private IRFlareManager flareMgr;

        private int attackStartedTime = 0;

        public PilotAIController(ScriptThread thread, Player player) : base(thread, player)
        {
            sessionMgr = thread.Get<SessionManager>();
            levelMgr = thread.Get<LevelManager>();     
        }

        public override void OnPlayerAttached(Player player)
        {
            flareMgr = player.GetExtension<IRFlareManager>();

            base.OnPlayerAttached(player);
        }

        public override void OnUpdate(int gameTime)
        {
            base.OnUpdate(gameTime);

            var pos = Player.Position;

            if (pos.DistanceTo(Game.Player.Character.Position) < 500.0f && 
                Function.Call<bool>(Hash.IS_OBJECT_NEAR_POINT, 0x9A3207B7, pos.X, pos.Y, pos.Z, 50.0f) && 
                !flareMgr.CooldownActive)
            {
                flareMgr.Start();
            }

            if (gameTime > state.NextDecisionTime && !Player.Ped.Ref.IsDead)
            {                                           
                findPlayer:

                if (Player.ActiveTarget == null || Game.GameTime - attackStartedTime > 60000)
                {
                    SessionPlayer otherPlayer = sessionMgr.Current.Players[0];

                    var dist = Player.Position.DistanceTo(otherPlayer.EntityRef.Position);

                    Tuple<SessionPlayer, float> nearbyPlayer = new Tuple<SessionPlayer, float>(otherPlayer, dist);

                    for (int x = 1; x < sessionMgr.Current.NumPlayers; x++)
                    {
                        otherPlayer = sessionMgr.Current.Players[x];

                        if (Player == otherPlayer.EntityRef || Player.Info.Sess.TeamNum == otherPlayer.TeamIdx) continue;

                        dist = Player.Position.DistanceTo(otherPlayer.EntityRef.Position);
  
                        if (dist < 1000.0f)
                        {                           
                            if (dist < nearbyPlayer.Item2)
                            {
                                nearbyPlayer = new Tuple<SessionPlayer, float>(otherPlayer, dist);
                            }
                        }       
                    }

                    Player.PersueTarget(nearbyPlayer.Item1.EntityRef);

                   // ScriptMain.DebugPrint("PilotAIController: {0} chose to fight the nearby player {1}", Player.Name, otherPlayer.EntityRef.Name);

                    state.Status = AIStatus.FightOther;

                    state.SetNextDecisionTime(gameTime + new Random().Next(6000, 30000));

                    attackStartedTime = Game.GameTime;
                }

                else
                {
                    var dist = Player.Position.DistanceTo(Player.ActiveTarget.Position);

                    if (dist > 1000.0f)
                   {
                        Player.ClearActiveTarget();

                        goto findPlayer;
                    }

                    var direction = Vector3.Normalize(Player.Position - Player.ActiveTarget.Position);

                    if (dist < 300.0f && Vector3.Dot(direction, Player.ActiveTarget.Vehicle.Ref.ForwardVector) < -0.1f)
                    {
                        var p = Player.ActiveTarget.Position;

                       /* if (Function.Call<bool>(Hash.GET_CURRENT_PED_VEHICLE_WEAPON, Player.Ped.Ref, outArg))
                        {
                            var weaponHash = Function.Call<int>(Hash.GET_HASH_KEY, "vehicle_weapon_enemy_lazer");

                            if (outArg.GetResult<int>() != weaponHash)
                                Function.Call(Hash.SET_CURRENT_PED_VEHICLE_WEAPON, Player.Ped.Ref, weaponHash);
                        }*/

                        Function.Call(Hash.SET_VEHICLE_SHOOT_AT_TARGET, Player.Ped.Ref, Player.ActiveTarget.Vehicle.Ref, p.X, p.Y, p.Z);

                  //      ScriptMain.DebugPrint("Shoot at target (" + Player.Name + " > " + Player.ActiveTarget.Name + ")");

                    }

             /*       var targetPos = Player.ActiveTarget.Position;

                    var position = Player.Position;

                    var currentHeading = Player.Vehicle.Ref.Heading;

                    float desiredHeading = Function.Call<float>(Hash.GET_HEADING_FROM_VECTOR_2D, targetPos.X - position.X, targetPos.Y - position.Y);

                    float diff = currentHeading > desiredHeading ?
                        currentHeading - (currentHeading - desiredHeading) :
                        currentHeading + (desiredHeading - currentHeading);

                    var steerBias = diff / 360.0f;

                    Function.Call(Hash.SET_VEHICLE_STEER_BIAS, Player.Vehicle.Ref, steerBias);*/

                    /* else if (dist > 200.0f)
                     {
                         var outArg = new OutputArgument();
                         if (Function.Call<bool>(Hash.GET_CURRENT_PED_VEHICLE_WEAPON, Player.Ped.Ref, outArg))
                         {
                             var weaponHash = Function.Call<int>(Hash.GET_HASH_KEY, "vehicle_weapon_plane_rocket");

                             if (outArg.GetResult<int>() != weaponHash)
                                 Function.Call(Hash.SET_CURRENT_PED_VEHICLE_WEAPON, Player.Ped.Ref, weaponHash);
                         }
                     }

                     else
                     {
                         var outArg = new OutputArgument();
                         if (Function.Call<bool>(Hash.GET_CURRENT_PED_VEHICLE_WEAPON, Player.Ped.Ref, outArg))
                         {
                             var weaponHash = Function.Call<int>(Hash.GET_HASH_KEY, "vehicle_weapon_enemy_lazer");

                             if (outArg.GetResult<int>() != weaponHash)
                                 Function.Call(Hash.SET_CURRENT_PED_VEHICLE_WEAPON, Player.Ped.Ref, weaponHash);
                         }
                     }*/
                }
                // couldn't find an active target. so we'll just fly randomly..

                if (state.Status != AIStatus.RandomFlight)
                {
                    var position = Utility.GetRandomPositionInArea(levelMgr.Level.AIBounds.Min, levelMgr.Level.AIBounds.Max);

                    Function.Call(Hash.TASK_PLANE_MISSION,
                                   Player.Ped.Ref,
                                   Player.Vehicle.Ref,
                                   0,
                                   0,
                                   position.X, position.Y, position.Z,
                                   6, 60.0, 26.0f, 30.0, 500.0, 20.0);

                    state.Status = AIStatus.RandomFlight;

                     //  ScriptMain.DebugPrint("PilotAIController: {0} chose to fly randomly.", Player.Info.Name);
                }


                state.SetNextDecisionTime(gameTime + new Random().Next(1000, 3000));
            }

        }
    }
}
