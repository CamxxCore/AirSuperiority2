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
        private const int TotalWaitTime = 1800;

        private int respawnTriggerTime = 0;

        private bool waitActive = false;

        private SessionManager sessionMgr;

        public RespawnManager(ScriptThread thread, Player player) : base(thread, player)
        {
            sessionMgr = ScriptMain.GetSessionManager();
        }

        public override void OnPlayerAttached(Player player)
        {
            player.OnDead += OnPlayerDead;

            base.OnPlayerAttached(player);
        }

        private void OnPlayerDead(Player sender, EventArgs e)
        {
            if (waitActive) return;

            ScriptMain.DebugPrint("Extensions.RespawnManager: Starting respawn...");

            if (Player.Ped.IsHuman)
            {
                Utility.FadeOutScreen(900);

                respawnTriggerTime = Game.GameTime + TotalWaitTime;
            }

            else respawnTriggerTime = 0;

            waitActive = true;
        }

        public override void OnUpdate(int gameTime)
        {
            if (waitActive && gameTime > respawnTriggerTime)
            {
                ScriptMain.DebugPrint("Extensions.RespawnManager: Doing spawn for {0}... Player team index is {1}", Player.Name, Player.Info.Sess.TeamNum);

                var team = sessionMgr.GetTeamByIndex(Player.Info.Sess.TeamNum);

                Player.Create(team.SpawnPoint);

                if (Player.Ped.IsHuman)
                {
                    Utility.FadeInScreen(700);
                }

                waitActive = false;
            }

            base.OnUpdate(gameTime);
        }
    }
}
