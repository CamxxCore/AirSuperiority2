using System;
using System.Drawing;
using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Entities;
using AirSuperiority.ScriptBase.Helpers;
using AirSuperiority.ScriptBase.Logic;
using AirSuperiority.ScriptBase.Types;
using GTA;
using GTA.Math;
using GTA.Native;
using Player = AirSuperiority.ScriptBase.Entities.Player;

namespace AirSuperiority.ScriptBase.Extensions
{
    public class RespawnManager : PlayerExtensionBase
    {
        /// <summary>
        /// The total time we wait after dying before triggering a respawn for the local player.
        /// </summary>
        private const int LocalPlayerWaitTime = 1800;

        /// <summary>
        /// The total time we wait after dying before triggering a respawn for an AI player.
        /// </summary>
        private const int AIPlayerWaitTime = 1200;

        private const int FadeOutDuration = 600;

        private const int FadeInDuration = 600;

        private int respawnTriggerTime = 0;

        private bool waitActive = false;

        private SessionManager sessionMgr;

        private LevelManager levelMgr;

        private DisplayManager displayMgr;

        private bool bOutOfBounds = false;

        private bool bIsLocal = false;

        private int outOfBoundsTime = 0;

        public RespawnManager(ScriptThread thread, Player player) : base(thread, player)
        {
            sessionMgr = thread.Get<SessionManager>();
            levelMgr = thread.Get<LevelManager>();
            displayMgr = thread.Get<DisplayManager>();
        }

        public override void OnPlayerAttached(Player player)
        {
            bIsLocal = player is LocalPlayer;

            player.OnDead += OnPlayerDead;

            Function.Call(Hash.RESURRECT_PED, player.Ped.Ref);

            base.OnPlayerAttached(player);
        }

        private void OnPlayerDead(Player sender, EventArgs e)
        {
            if (waitActive) return;
            //ScriptMain.DebugPrint("Extensions.RespawnManager: Starting respawn...");

            if (bIsLocal)
            {
                Function.Call(Hash._DISABLE_AUTOMATIC_RESPAWN, true);

                Function.Call(Hash.IGNORE_NEXT_RESTART, true);

                Utility.FadeOutScreen(FadeOutDuration);

                respawnTriggerTime = Game.GameTime + LocalPlayerWaitTime;

                bOutOfBounds = false;
            }

            else
            {
                respawnTriggerTime = Game.GameTime + AIPlayerWaitTime;
            }

            waitActive = true;
        }

        public override void OnUpdate(int gameTime)
        {
            if (!Utility.IsPositionInArea(Player.Position, levelMgr.Level.Bounds.Min, levelMgr.Level.Bounds.Max))
            {
                if (!bOutOfBounds)
                {
                    bOutOfBounds = true;
                    outOfBoundsTime = gameTime;

                    if (bIsLocal)
                    {
                        Function.Call(Hash._START_SCREEN_EFFECT, "SwitchOpenMichaelIn", 0, false);
                    }
                }

                else
                {
                    if (Game.GameTime - outOfBoundsTime > 3500)
                    {
                        if (Game.GameTime - outOfBoundsTime > 9000)
                        {
                            Function.Call(Hash._STOP_SCREEN_EFFECT, "SwitchOpenMichaelIn");

                            var spawnPoint = levelMgr.GetSpawnPoint(Player.Info.Sess.TeamNum);

                            Player.Create(spawnPoint);
                        }

                        else
                        {
                            if (bIsLocal)
                            {
                                int secToRespawn = (9000 - (Game.GameTime - outOfBoundsTime)) / 1000;
                                UI.ShowSubtitle("Time until respawn: " + secToRespawn.ToString());
                            }
                        }
                    }

                    if (bIsLocal)
                    {
                        displayMgr.ShowWarningThisFrame("Leaving The Combat Area");
                    }
                }
            }

            else
            {
                if (bOutOfBounds)
                {
                    if (bIsLocal)
                    {
                        Function.Call(Hash._STOP_SCREEN_EFFECT, "SwitchOpenMichaelIn");
                    }

                    bOutOfBounds = false;
                }
            }

            if (waitActive && gameTime > respawnTriggerTime)
            {
                //ScriptMain.DebugPrint("Extensions.RespawnManager: Doing spawn for {0}... Player team index is {1}", Player.Name, Player.Info.Sess.TeamNum);

                if (bIsLocal)
                {
                    Function.Call(Hash.RESURRECT_PED, Player.Ped.Ref);

                    Function.Call(Hash._RESET_LOCALPLAYER_STATE);

                    Utility.FadeInScreen(FadeInDuration);
                }

                var spawnPoint = levelMgr.GetSpawnPoint(Player.Info.Sess.TeamNum);

                Player.Create(spawnPoint);

                waitActive = false;
            }

            base.OnUpdate(gameTime);
        }

        public override void Dispose()
        {
            Function.Call(Hash._DISABLE_AUTOMATIC_RESPAWN, false);

            if (Player is LocalPlayer && Player.Ped.Ref.IsDead)
            {
                Function.Call(Hash.RESURRECT_PED, Player.Ped.Ref);
            }

            base.Dispose();
        }
    }
}
