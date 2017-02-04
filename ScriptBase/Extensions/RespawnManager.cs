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

        /// <summary>
        /// Duration of the fade out animation on respawning.
        /// </summary>
        private const int FadeOutDuration = 600;

        /// <summary>
        /// Duration of the fade in animation on respawning.
        /// </summary>
        private const int FadeInDuration = 600;

        /// <summary>
        /// Max time a player can be out of bounds before triggering a respawn.
        /// </summary>
        private const int OutOfBoundsDuration = 9000;

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

          /*  if (bIsLocal && Player.Ped.Ref.IsDead)
            {
                Function.Call(Hash.RESURRECT_PED, Player.Ped.Ref);
            }*/

            player.OnDead += OnPlayerDead;

            Function.Call(Hash._DISABLE_AUTOMATIC_RESPAWN, true);

            base.OnPlayerAttached(player);
        }

        /// <summary>
        /// Fired when the player has died.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPlayerDead(Player sender, EventArgs e)
        {
            if (waitActive) return;
            //ScriptMain.DebugPrint("Extensions.RespawnManager: Starting respawn...");

            if (bIsLocal)
            {
                Function.Call(Hash.IGNORE_NEXT_RESTART, true);

                Utility.FadeOutScreen(FadeOutDuration);

                respawnTriggerTime = Game.GameTime + LocalPlayerWaitTime;
            }

            else
            {
                respawnTriggerTime = Game.GameTime + AIPlayerWaitTime;
            }

            bOutOfBounds = false;

            waitActive = true;
        }

        /// <summary>
        /// Returns whether the player is within the level bounds.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns></returns>
        private bool WorldPositionInLevelBoundsFrame(int gameTime)
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
                        if (Game.GameTime - outOfBoundsTime > OutOfBoundsDuration)
                        {
                            Function.Call(Hash._STOP_SCREEN_EFFECT, "SwitchOpenMichaelIn");

                            var spawnPoint = levelMgr.GetSpawnPoint(Player.Info.Sess.TeamNum);

                            Player.Create();
                        }

                        else
                        {
                            if (bIsLocal)
                            {
                                int secToRespawn = (OutOfBoundsDuration - (Game.GameTime - outOfBoundsTime)) / 1000;

                                UI.ShowSubtitle("Time until respawn: " + secToRespawn.ToString(), 1);
                            }
                        }
                    }

                    if (bIsLocal)
                    {
                        displayMgr.ShowWarningThisFrame("Leaving The Combat Area");
                    }
                }

                return false;
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

                return true;
            }
        }

        public override void OnUpdate(int gameTime)
        {
            Function.Call(Hash.SET_FADE_OUT_AFTER_DEATH, false);

            Function.Call(Hash.SET_FADE_IN_AFTER_LOAD, true);

            WorldPositionInLevelBoundsFrame(gameTime);

            if (waitActive && gameTime > respawnTriggerTime || Player.Info.Sess.State == PlayerState.Inactive)
            {
                //ScriptMain.DebugPrint("Doing spawn for {0}... Player team index is {1}", Player.Name, Player.Info.Sess.TeamNum);

                var spawnPoint = levelMgr.GetSpawnPoint(Player.Info.Sess.TeamNum);

                if (bIsLocal)
                {
                    Function.Call(Hash.RESURRECT_PED, Player.Ped.Ref);

                    Function.Call(Hash._RESET_LOCALPLAYER_STATE);
                }

                Player.Create();

                if (bIsLocal)
                    Utility.FadeInScreen(FadeInDuration);

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
