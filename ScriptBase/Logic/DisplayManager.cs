using System.Drawing;
using AirSuperiority.Core;
using AirSuperiority.ScriptBase.Helpers;
using AirSuperiority.ScriptBase.Types.Metadata;
using GTA;
using GTA.Native;
using GTA.Math;

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

        /// <summary>
        /// Show the leaderboard.
        /// </summary>
        public void ShowLeaderboard()
        {
            showLeaderboard = true;
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
            teamHUD.SetTeamAsset(slotIdx, metadata.FriendlyName, metadata.ImageAsset);
        }

        /// <summary>
        /// Draw entity heads up display.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="color"></param>
        /// <param name="subText"></param>
        /// <param name="sizeMultiplier"></param>
        /// <param name="withText"></param>
        /// <param name="drawRect"></param>
        private static void DrawSquare(Point location, Color color, string subText, float sizeMultiplier, bool withText, bool drawRect = true)
        {
            if (drawRect)
            {
                UIRectangle rect;
                rect = new UIRectangle(new Point((int)((location.X - 25) * sizeMultiplier), (int)(location.Y * sizeMultiplier)), new Size((int)(4 * sizeMultiplier), (int)(54 * sizeMultiplier)));
                rect.Color = color;
                rect.Draw();
                rect = new UIRectangle(new Point((int)((location.X + 25) * sizeMultiplier), (int)(location.Y * sizeMultiplier)), new Size((int)(4 * sizeMultiplier), (int)(54 * sizeMultiplier)));
                rect.Color = color;
                rect.Draw();
                rect = new UIRectangle(new Point((int)((location.X - 25) * sizeMultiplier), (int)((location.Y + 54) * sizeMultiplier)), new Size((int)(52 * sizeMultiplier), (int)(4 * sizeMultiplier)));
                rect.Color = color;
                rect.Draw();
                rect = new UIRectangle(new Point((int)((location.X - 25) * sizeMultiplier), (int)(location.Y * sizeMultiplier)), new Size((int)(52 * sizeMultiplier), (int)(4 * sizeMultiplier)));
                rect.Color = color;
                rect.Draw();
            }

            if (withText)
            {
                var text = new UIText(subText, new Point((int)((location.X - 5) * sizeMultiplier), (int)((location.Y + 60) * sizeMultiplier)), 0.3f, Color.White, GTA.Font.ChaletComprimeCologne, false);
                text.Draw();
            }
        }

        /// <summary>
        /// Get viewport camera position in world coords.
        /// </summary>
        /// <returns></returns>
        private Vector3 GetCameraPosition()
        {
            return (Function.Call<bool>(Hash.DOES_CAM_EXIST, World.RenderingCamera.Handle) == false ?
                GameplayCamera.Position :
                World.RenderingCamera.Position);
        }

        /// <summary>
        /// Update the class.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void OnUpdate(int gameTime)
        {
            rankBar.Update();

            if (sessionMgr.SessionActive)
            {
                teamHUD.Draw();

                foreach (var player in sessionMgr.Current.Players)
                {
                    var pos = player.EntityRef.Position;

                    float dist = (pos - GetCameraPosition()).Length();

                    float scale = 6.0f / dist;

                    Function.Call(Hash.SET_DRAW_ORIGIN, pos.X, pos.Y, pos.Z, 0);

                    var color = sessionMgr.GetTeamByIndex(player.TeamIdx).TeamColor;

                    DrawSquare(new Point(0, -32), color.ToSystemColor(),
                        string.Format("name: {0}", player.EntityRef.Name), scale * 0.37f,
                        dist > 0.42f,
                        dist > 0.58f);

                    Function.Call(Hash.CLEAR_DRAW_ORIGIN);
                }
            }

            if (showLeaderboard)
            {
                Function.Call(Hash.HIDE_HUD_AND_RADAR_THIS_FRAME);

                leaderboard.Draw();

                if (Game.IsDisabledControlJustPressed(0, (Control)202) || Game.IsDisabledControlJustPressed(0, (Control)2238))
                {
                    showLeaderboard = false;
                }

                if (Game.IsDisabledControlJustPressed(0, (Control)2241) || Game.IsDisabledControlJustPressed(0, (Control)2188))
                {
                    leaderboard.HandleScrollUp();
                }

                else if (Game.IsDisabledControlJustPressed(0, (Control)2242) || Game.IsDisabledControlJustPressed(0, (Control)2187))
                {
                    leaderboard.HandleScrollDown();
                }
            }

            base.OnUpdate(gameTime);
        }
    }
}
