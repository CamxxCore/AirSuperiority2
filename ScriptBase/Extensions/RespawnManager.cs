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

        public RespawnManager(ScriptThread thread, Player player) : base(thread, player)
        {
            sessionMgr = thread.Get<SessionManager>();
            levelMgr = thread.Get<LevelManager>();
        }

        public override void OnPlayerAttached(Player player)
        {
            player.OnDead += OnPlayerDead;

            base.OnPlayerAttached(player);
        }

        private void OnPlayerDead(Player sender, EventArgs e)
        {
            //ScriptMain.DebugPrint("Extensions.RespawnManager: Starting respawn...");

            if (Player.Ped.IsHuman)
            {
                Function.Call(Hash.SET_FADE_OUT_AFTER_DEATH, 0);

                Function.Call(Hash._DISABLE_AUTOMATIC_RESPAWN, true);

                Function.Call(Hash.IGNORE_NEXT_RESTART, true);

                Utility.FadeOutScreen(FadeOutDuration);

                respawnTriggerTime = Game.GameTime + LocalPlayerWaitTime;
            }

            else
            {
                respawnTriggerTime = Game.GameTime + AIPlayerWaitTime;
            }

            waitActive = true;
        }

        public override void OnUpdate(int gameTime)
        {
            if (waitActive && gameTime > respawnTriggerTime)
            {
                //ScriptMain.DebugPrint("Extensions.RespawnManager: Doing spawn for {0}... Player team index is {1}", Player.Name, Player.Info.Sess.TeamNum);

                if (Player.Ped.IsHuman)
                {
                    Function.Call(Hash.RESURRECT_PED, Player.Ped.Ref);

                    Function.Call(Hash._RESET_LOCALPLAYER_STATE);
                }

                var spawnPoint = levelMgr.GetSpawnPoint(Player.Info.Sess.TeamNum);

                Player.Create(spawnPoint);

                if (Player.Ped.IsHuman)
                {     
                    Utility.FadeInScreen(FadeInDuration);
                }

                waitActive = false;
            }

            base.OnUpdate(gameTime);
        }
    }
}
