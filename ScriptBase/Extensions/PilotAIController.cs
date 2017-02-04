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

        private int lastShotAtTime = 0;

        private Random random = new Random();

        private Vector3 destination;

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

        private void SetRandomDestination()
        {
            destination = Utility.GetRandomPositionInArea(levelMgr.Level.AIBounds.Min, levelMgr.Level.AIBounds.Max);

            Function.Call(Hash.TASK_PLANE_MISSION,
                           Player.Ped.Ref,
                           Player.Vehicle.Ref,
                           0,
                           0,
                           destination.X, destination.Y, destination.Z,
                           6, 309.0, 26.0f, 200.0, 500.0, 20.0);
        }

        public override void OnUpdate(int gameTime)
        {
            base.OnUpdate(gameTime);

            var position = Player.Position;

            if (Function.Call<bool>(Hash.IS_OBJECT_NEAR_POINT, 0x9A3207B7, position.X, position.Y, position.Z, (float)random.Next(50, 200)) &&
                position.DistanceTo(GameplayCamera.Position) < 500.0f &&
                Probability.GetBoolean(0.0015f) &&
                !flareMgr.CooldownActive)
            {
                flareMgr.Start();
            }

            if (gameTime > state.NextDecisionTime)
            {
                // If we arent chasing a target, they have moved too far away, or have been chasing a target for a set amount of time, make a new decision.
                if (Player.ActiveTarget == null ||
                    position.DistanceTo(Player.ActiveTarget.Position) > 1000.0f ||
                    Game.GameTime - attackStartedTime > 30000)
                {
                    var otherPlayer = sessionMgr.Current.Players[0];

                    float dist = Player.Position.DistanceTo(otherPlayer.EntityRef.Position);

                    AITarget target = new AITarget(otherPlayer.EntityRef, dist);

                    if (target.Distance > 1000.0f)
                    {
                        target = null;

                        for (int x = 1; x < sessionMgr.Current.NumPlayers; x++)
                        {
                            otherPlayer = sessionMgr.Current.Players[x];

                            // We don't want ourselves, or any of our team members...
                            if (Player == otherPlayer.EntityRef || Player.Info.Sess.TeamNum == otherPlayer.TeamIdx) continue;

                            // Get the distance between us and the potential target..
                            dist = position.DistanceTo(otherPlayer.EntityRef.Position);

                            // If they are closer than any of the previous candidates, add them as the best candidate.
                            if (dist < 1000.0f && (target == null || dist < target.Distance))
                            {
                                target = new AITarget(otherPlayer.EntityRef, dist);
                            }
                        }
                    }

                    // we found a target, change our status to fighting
                    if (target != null && Player.Info.Sess.TeamNum != target.Player.Info.Sess.TeamNum)
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

                state.SetNextDecisionTime(gameTime + random.Next(10000, 30000));
            }

            switch (state.Status)
            {
                case AIStatus.FightOther:
                    {
                        if (gameTime - lastShotAtTime > 1600 && Player.ActiveTarget != null)
                        {
                            var direction = Vector3.Normalize(Player.ActiveTarget.Position - position);

                            var p = Player.ActiveTarget.Position;

                            if (position.DistanceTo(p) < 200.0f && Vector3.Dot(direction, Utility.RotationToDirection(Player.Vehicle.Ref.Rotation)) > 0.1f)
                            {
                                Function.Call(Hash.SET_VEHICLE_SHOOT_AT_TARGET, Player.Ped.Ref, Player.ActiveTarget.Vehicle.Ref, p.X, p.Y, p.Z);

                                ScriptMain.DebugPrint("Shoot at target (" + Player.Name + " > " + Player.ActiveTarget.Name + ")");

                                lastShotAtTime = gameTime;
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

                        break;
                    }
            }
        }
    }
}
