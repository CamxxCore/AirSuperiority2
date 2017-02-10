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
    public class PilotAIController : PlayerExtensionBase
    {
        public AIState State { get { return state; } }

        private AIState state;

        private SessionManager sessionMgr;
        private LevelManager levelMgr;
        private IRFlareManager flareMgr;
        private EngineExtinguisher extinguisher;

        private int attackStartedTime = 0;

        private int lastShotAtTime = 0;

        private Random random = new Random();

        private Vector3 destination;

        private readonly float intelligenceBias = 
           Probability.GetBoolean(0.10f) ? Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 0.00054f, 0.000678f) :
            Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 0.0003f, 0.00054f);

        class AITarget
        {
            public Player Player;
            public float Distance;

            public AITarget(Player player, float distance)
            {
                Player = player;
                Distance = distance;
            }
        }

        public PilotAIController(Player player) : base(player)
        {
            sessionMgr = ScriptThread.GetOrAddExtension<SessionManager>();
            levelMgr = ScriptThread.GetOrAddExtension<LevelManager>();
        }

        public override void OnPlayerAttached(Player player)
        {
            flareMgr = player.GetExtension<IRFlareManager>();

            extinguisher = player.GetExtension<EngineExtinguisher>();

            base.OnPlayerAttached(player);
        }

        private void SetRandomDestination()
        {
            destination = Utility.GetRandomPositionInArea(levelMgr.Level.AIBounds.Min, levelMgr.Level.AIBounds.Max);

            Function.Call(Hash.TASK_PLANE_MISSION,
                           Player.Ped.Ref,
                           Player.Vehicle.Ref,
                           0,
                           0,
                           destination.X, destination.Y, destination.Z,
                           6, 309.0, 26.0f, 200.0, 1000.0, 20.0);
        }

        private void MakeDecision()
        {
            var otherPlayer = sessionMgr.Current.Players[0];

            float dist = Player.Position.DistanceTo(otherPlayer.PlayerRef.Position);

            AITarget target = new AITarget(otherPlayer.PlayerRef, dist);

            if ((target.Distance > 1000.0f || Player.Info.Sess.TeamNum == target.Player.Info.Sess.TeamNum)) // distance to the local player is too great.. find another ai player to fight.
            {
                target = null;

                for (int x = 0; x < sessionMgr.Current.NumPlayers; x++)
                {
                    otherPlayer = sessionMgr.Current.Players[x];

                    // We don't want ourselves, or any of our team members...
                    if (Player == otherPlayer.PlayerRef || Player.Info.Sess.TeamNum == otherPlayer.TeamIdx) continue;

                    // Get the distance between us and the potential target..
                    dist = Player.Position.DistanceTo(otherPlayer.PlayerRef.Position);

                    // If they are closer than any of the previous candidates, add them as the best candidate.
                    if (dist < 1000.0f && (target == null || dist < target.Distance))
                    {
                        target = new AITarget(otherPlayer.PlayerRef, dist);
                    }
                }
           }

            // we found a target, change our status to fighting
            if (target != null)
            {
                Player.PersueTarget(target.Player);

                ScriptMain.DebugPrint("PilotAIController: {0} chose to fight the nearby player {1}", Player.Name, target.Player.Name);

                state.Status = AIStatus.FightOther;

                attackStartedTime = Game.GameTime;
            }

            // no target found ): We will just fly randomly..
            else
            {
                SetRandomDestination();

                state.Status = AIStatus.RandomFlight;
            }
        }

        private bool tooFar = false;
        public override void OnUpdate(int gameTime)
        {
            base.OnUpdate(gameTime);

            var position = Player.Position;

            if (!ScriptThread.GetVar<bool>("scr_hardcore").Value)
            {
                if (Function.Call<bool>(Hash.IS_OBJECT_NEAR_POINT, 0x9A3207B7, position.X, position.Y, position.Z, (float)random.Next(50, 200)) &&
                    position.DistanceTo(GameplayCamera.Position) < 700.0f &&
                    !flareMgr.CooldownActive &&
                    Probability.GetBoolean(0.0020f + intelligenceBias))
                {
                    flareMgr.Start();
                }

                /*(  if (Player.Vehicle.Ref.Health < Player.Vehicle.Ref.MaxHealth &&
                      !extinguisher.CooldownActive &&
                      Probability.GetBoolean(intelligenceBias) )
                  {
                      ScriptMain.DebugPrint("use extinguisher (" + Name + ")");
                      extinguisher.Start();
                  }*/
            }

            if (gameTime > state.NextDecisionTime)
            {
                // If we arent chasing a target, they have moved too far away, or have been chasing a target for a set amount of time, make a new decision.
                if (Player.ActiveTarget == null || Player.ActiveTarget.Ped.Ref.IsDead ||
                    position.DistanceTo(Player.ActiveTarget.Position) > 1000.0f ||
                    !Utility.IsPositionInArea(Player.ActiveTarget.Position, levelMgr.Level.Bounds.Min, levelMgr.Level.Bounds.Max) ||
                    Game.GameTime - attackStartedTime > 30000)
                {
                    MakeDecision();
                }

                state.SetNextDecisionTime(gameTime + random.Next(1000, 5000));
            }

            switch (state.Status)
            {
                case AIStatus.FightOther:
                    {
                        if (Player.ActiveTarget != null)
                        {
                            if (Player.ActiveTarget.Position.DistanceTo(Player.Position) > 600.0f)
                            {
                                if (!tooFar)
                                {
                                    var destination = Player.ActiveTarget.Position;

                                    Function.Call(Hash.TASK_PLANE_MISSION,
                                                   Player.Ped.Ref,
                                                   Player.Vehicle.Ref,
                                                   0,
                                                   0,
                                                   destination.X, destination.Y, destination.Z,
                                                   6, 309.0, 26.0f, 200.0, 1000.0, 20.0);

                                    tooFar = true;
                                }
                            }

                            else if (Function.Call<int>(Hash.GET_ACTIVE_VEHICLE_MISSION_TYPE, Player.Vehicle.Ref) != 6)
                            {
                                tooFar = false;
                                Player.PersueTarget(Player.ActiveTarget);
                                UI.ShowSubtitle("persuing " + Function.Call<int>(Hash.GET_ACTIVE_VEHICLE_MISSION_TYPE, Player.Vehicle.Ref).ToString());
                            }
                        }     


                        break;
                    }

                case AIStatus.RandomFlight:
                    {
                        if (position.DistanceTo(destination) < 15.0f)
                        {
                            SetRandomDestination();

                            ScriptMain.DebugPrint("Set new random destination for " + Player.Name);
                        }

                        else if (Player.Position.DistanceTo(sessionMgr.Current.Players[0].PlayerRef.Position) < 1000.0f &&
                            Player.Info.Sess.TeamNum != sessionMgr.Current.Players[0].TeamIdx)
                        {
                            Player.PersueTarget(sessionMgr.Current.Players[0].PlayerRef);

                            state.Status = AIStatus.FightOther;
                        }

                        break;
                    }
            }

            for (int i = 0; i < sessionMgr.Current.NumPlayers; i++)
            {
                if (gameTime - lastShotAtTime > 900)
                {
                    var otherPlayer = sessionMgr.Current.Players[i];

                    if (otherPlayer.TeamIdx == Player.Info.Sess.TeamNum) continue;

                    var p = otherPlayer.PlayerRef.Position;

                    var direction = Vector3.Normalize(p - position);

                    var otherHeading = otherPlayer.PlayerRef.Vehicle.Ref.Heading;

                    if (Probability.GetBoolean(0.1f + intelligenceBias) && position.DistanceTo(p) < 400.0f)
                    {
                        Function.Call(Hash.SET_VEHICLE_SHOOT_AT_TARGET, Player.Ped.Ref, otherPlayer.PlayerRef.Vehicle.Ref, p.X, p.Y, p.Z);

                        ScriptMain.DebugPrint("Shoot at target (" + Player.Name + " > " + otherPlayer.PlayerRef.Name + ") team idx:" + otherPlayer.PlayerRef.Info.Sess.TeamNum.ToString() + " sess team:" + otherPlayer.TeamIdx);

                        lastShotAtTime = gameTime;
                    }
                }
            }
        }
    }
}
