using System;
using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Helpers;
using AirSuperiority.ScriptBase.Types.Metadata;
using GTA.Native;

namespace AirSuperiority.ScriptBase.Logic
{
    public class DisplayManager : ScriptExtension
    {
        private RankBar rankBar = new RankBar();

        private Leaderboard leaderboard = new Leaderboard();

        private TeamScoreboard teamHUD = new TeamScoreboard();

        private InputManager inputMgr;

        private SessionManager sessionMgr;

        private bool showLeaderboard = false;

        public override void OnThreadAttached(ScriptThread thread)
        {
            inputMgr = thread.GetExtension("input") as InputManager;

            sessionMgr = thread.GetExtension("sess") as SessionManager;

            base.OnThreadAttached(thread);
        }

        public void ShowLeaderboard()
        {
            showLeaderboard = true;
        }

        public void ShowRankBar(int currentRank, int currentXP, int newXP, int colour = 116, int duration = 1000, int animationSpeed = 1000)
        {
            rankBar.Show(currentRank, currentXP, newXP, colour, duration, animationSpeed);
        }

        public void SetTeamSlotFromMetadata(int slotIdx, TeamAssetMetadata metadata)
        {
            teamHUD.SetTeamAsset(slotIdx, metadata.FriendlyName, metadata.ImageAsset);
        }

        public override void OnUpdate()
        {
            rankBar.Update();

            if (showLeaderboard)
            {
                Function.Call(Hash.HIDE_HUD_AND_RADAR_THIS_FRAME);

                leaderboard.Draw();

                if (inputMgr.DisabledControlReleased(202) || inputMgr.DisabledControlReleased(238))
                {
                    showLeaderboard = false;
                }

                if (inputMgr.DisabledControlPressed(241) || inputMgr.DisabledControlPressed(188))
                {
                    leaderboard.HandleScrollUp();
                }

                else if (inputMgr.DisabledControlPressed(242) || inputMgr.DisabledControlPressed(187))
                {
                    leaderboard.HandleScrollDown();
                }
            }

            if (sessionMgr.SessionActive)
            {
                teamHUD.Draw();
            }

            base.OnUpdate();
        }
    }
}
