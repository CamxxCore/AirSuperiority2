using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Helpers;
using AirSuperiority.ScriptBase.Types;
using GTA;
using GTA.Native;

namespace AirSuperiority.ScriptBase.Logic
{
    public class DisplayManager : ScriptExtension
    {
        private bool showLeaderboard = false;

        private bool showScoreboard = false;

        private RankBar rankBar = new RankBar();

        private Leaderboard leaderboard = new Leaderboard();

        private TeamScoreboard scoreboard = new TeamScoreboard();

        private DebugOutput dbgOutput = new DebugOutput();

        private FighterDisplay fighterHUD = new FighterDisplay();

        private KillPopup killPopup = new KillPopup();

        /// <summary>
        /// Show the leaderboard.
        /// </summary>
        public void ShowLeaderboard()
        {
            showLeaderboard = true;
        }

        /// <summary>
        /// Show the scoreboard.
        /// </summary>
        public void ShowScoreboard()
        {
            showScoreboard = true;
        }

        /// <summary>
        /// Hide the scoreboard.
        /// </summary>
        public void HideScoreboard()
        {
            showScoreboard = false;
        }

        public void ShowWarningThisFrame(string text)
        {
            fighterHUD.ShowWarning(text);
        }

        public void ShowKillPopup(string name, int teamIndex)
        {
            var teamInfo = ScriptThread.GetOrAddExtension<SessionManager>().GetTeamByIndex(teamIndex);

            killPopup.SetPlayerInfo(name, teamInfo.Asset.SecondaryAssetPath, ((TeamColor)teamIndex).ToSystemColor());

            killPopup.ShowTimed();
        }

        /// <summary>
        /// Show the rank bar with the given arguments.
        /// </summary>
        /// <param name="currentRank"></param>
        /// <param name="currentXP"></param>
        /// <param name="newXP"></param>
        /// <param name="colour"></param>
        /// <param name="duration"></param>
        /// <param name="animationSpeed"></param>
        public void ShowRankBar(int currentRank, int currentXP, int newXP, int colour = 116, int duration = 1000, int animationSpeed = 1000)
        {
            rankBar.Show(currentRank, currentXP, newXP, colour, duration, animationSpeed);
        }

        /// <summary>
        /// Update a team scoreboard slot.
        /// </summary>
        /// <param name="slotIdx"></param>
        /// <param name="metadata"></param>
        public void SetTeamScore(int slotIdx, int score)
        {
            scoreboard.SetTeamScore(slotIdx, score);
        }

        /// <summary>
        /// Setup a team scoreboard slot.
        /// </summary>
        /// <param name="slotIdx"></param>
        /// <param name="metadata"></param>
        public void SetupTeamSlot(int slotIdx, string teamTeam, string imageAssetPath)
        {
            scoreboard.SetTeamAsset(slotIdx, teamTeam, imageAssetPath);
        }

        /// <summary>
        /// Write debug text to the screen.
        /// </summary>
        /// <param name="text"></param>
        public void WriteDebugLine(string text)
        {
            dbgOutput.AddLine(text);
        }

        /// <summary>
        /// Update the class.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void OnUpdate(int gameTime)
        {
            if (!Game.IsScreenFadedOut)
            {
                dbgOutput.Update();

                rankBar.Update();

                if (ScriptThread.GetVar<bool>("scr_activesession").Value)
                {
                    if (showScoreboard)
                    {
                        scoreboard.Draw();
                    }

                    killPopup.Draw();
                }

                if (showLeaderboard)
                {                   
                    Function.Call(Hash.HIDE_HUD_AND_RADAR_THIS_FRAME);

                    leaderboard.Draw();

                    if (Game.IsDisabledControlJustPressed(0, (Control)202) || Game.IsDisabledControlJustPressed(0, (Control)238))
                    {
                        showLeaderboard = false;
                    }

                    if (Game.IsDisabledControlJustPressed(0, (Control)241) || Game.IsDisabledControlJustPressed(0, (Control)188))
                    {
                        leaderboard.HandleScrollUp();
                    }

                    else if (Game.IsDisabledControlJustPressed(0, (Control)242) || Game.IsDisabledControlJustPressed(0, (Control)187))
                    {
                        leaderboard.HandleScrollDown();
                    }
                }
            }

            base.OnUpdate(gameTime);
        }
    }
}
