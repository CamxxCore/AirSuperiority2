using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Helpers;
using AirSuperiority.ScriptBase.Types.Metadata;
using GTA;
using GTA.Math;
using GTA.Native;

namespace AirSuperiority.ScriptBase.Logic
{
    public class DisplayManager : ScriptExtension
    {
        private InputManager inputMgr;

        private SessionManager sessionMgr;

        private bool showLeaderboard = false;

        private bool showScoreboard = false;

        private RankBar rankBar = new RankBar();

        private Leaderboard leaderboard = new Leaderboard();

        private TeamScoreboard scoreboard = new TeamScoreboard();

        private DebugOutput dbgOutput = new DebugOutput();

        private FighterDisplay fighterHUD = new FighterDisplay();

        public DisplayManager(ScriptThread thread) : base(thread)
        {
            inputMgr = thread.Get<InputManager>();
            sessionMgr = thread.Get<SessionManager>();
        }

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
        /// Update team scoreboard from a metadata entry.
        /// </summary>
        /// <param name="slotIdx"></param>
        /// <param name="metadata"></param>
        public void SetTeamSlotFromMetadata(int slotIdx, TeamAssetMetadata metadata)
        {
            scoreboard.SetTeamAsset(slotIdx, metadata.FriendlyName, metadata.ImageAsset);
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
            dbgOutput.Update();

            rankBar.Update();

            if (sessionMgr != null && sessionMgr.SessionActive)
            {             
                if (showScoreboard)
                {
                    scoreboard.Draw();
                }
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

            base.OnUpdate(gameTime);
        }
    }
}
