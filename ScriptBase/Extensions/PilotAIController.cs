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

        public PilotAIController(ScriptThread thread, Player player) : base(thread, player)
        {
            sessionMgr = thread.Get<SessionManager>();
            levelMgr = thread.Get<LevelManager>();
        }

        public override void OnUpdate(int gameTime)
        {
            base.OnUpdate(gameTime);

            var outArg = new OutputArgument();
            if (Function.Call<bool>(Hash.GET_CURRENT_PED_VEHICLE_WEAPON, Player.Ped.Ref, outArg))
            {
                var weaponHash = Function.Call<int>(Hash.GET_HASH_KEY, "vehicle_weapon_plane_rocket");

                if (outArg.GetResult<int>() != weaponHash)
                    Function.Call(Hash.SET_CURRENT_PED_VEHICLE_WEAPON, Player.Ped.Ref, weaponHash);
            }

            if (gameTime > state.NextDecisionTime && !Player.Ped.Ref.IsDead)
            {
                findPlayer:
                if (Player.ActiveTarget == null)
                {
                    for (int x = 0; x < sessionMgr.Current.NumPlayers; x++)
                    {
                        SessionPlayer otherPlayer = sessionMgr.Current.Players[x];

                        if (Player == otherPlayer.EntityRef || Player.Info.Sess.TeamNum == otherPlayer.TeamIdx) continue;

                        var dist = Player.Position.DistanceTo(otherPlayer.EntityRef.Position);

                        if (Player.ActiveTarget == otherPlayer.EntityRef)
                        {

                        }

                        if (Player.Position.DistanceTo(otherPlayer.EntityRef.Position) < 800.0f && sessionMgr.Current.Players.Where(y => y.EntityRef.ActiveTarget == otherPlayer.EntityRef).Count() < 3)
                        {
                            // if (otherPlayer.EntityRef is LocalPlayer)
                            // {
                            // attack..

   

                            ScriptMain.DebugPrint("PilotAIController: {0} chose to fight the nearby player {1}", Player.Name, otherPlayer.EntityRef.Name);

                            Player.PersueTarget(otherPlayer.EntityRef);

                            state.Status = AIStatus.FightOther;

                            state.SetNextDecisionTime(gameTime + new Random().Next(1000, 12000));

                            return;
                            //  }
                        }
                    }
                }

                else
                {
                    var dist = Player.Position.DistanceTo(Player.ActiveTarget.Position);

                    if (dist > 800.0f)
                    {
                        Player.ClearActiveTarget();

                        goto findPlayer;
                    }

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
                                  6, 5.0, 500.0, 120.0, 500, 80);

                    state.Status = AIStatus.RandomFlight;

                    ScriptMain.DebugPrint("PilotAIController: {0} chose to fly randomly.", Player.Info.Name);
                }


                state.SetNextDecisionTime(gameTime + new Random().Next(1000, 20000));
            }
        }
    }
}
