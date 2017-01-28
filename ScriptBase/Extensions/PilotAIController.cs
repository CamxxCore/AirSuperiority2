using System;
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
            sessionMgr = ScriptMain.GetSessionManager();
            levelMgr = ScriptMain.GetLevelManager();
        }

        public override void OnUpdate(int gameTime)
        {
            base.OnUpdate(gameTime);

            if (gameTime > state.NextDecisionTime && !Player.Ped.Ref.IsDead)
            {
                if (state.Status != AIStatus.FightOther)
                {
                    for (int x = 0; x < sessionMgr.Current.NumPlayers; x++)
                    {
                        SessionPlayer otherPlayer = sessionMgr.Current.Players[x];

                        if (Player == otherPlayer.EntityRef || Player.Info.Sess.TeamNum == otherPlayer.TeamIdx) continue;

                        if (Player.Position.DistanceTo(otherPlayer.EntityRef.Position) < 800.0f)
                        {
                            // if (otherPlayer.EntityRef is LocalPlayer)
                            // {
                            // attack..

                            ScriptMain.DebugPrint("PilotAIController: {0} chose to fight the nearby player {1}", Player.Name, otherPlayer.EntityRef.Name);

                            Player.SetTarget(otherPlayer.EntityRef);

                            state.Status = AIStatus.FightOther;

                            state.SetNextDecisionTime(gameTime + new Random().Next(1000, 20000));

                            return;
                            //  }
                        }
                    }
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
                                  6, 5.0, 500.0, 100.0, 500, 80);

                    state.Status = AIStatus.RandomFlight;

                    ScriptMain.DebugPrint("PilotAIController: {0} chose to fly randomly.", Player.Name);
                }


                state.SetNextDecisionTime(gameTime + new Random().Next(1000, 20000));
            }
        }
    }
}
